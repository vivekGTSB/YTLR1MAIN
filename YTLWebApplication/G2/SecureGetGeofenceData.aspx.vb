Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Public Class SecureGetGeofenceData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY: Validate session
            If Not G2SecurityHelper.ValidateSession() Then
                Response.StatusCode = 401
                Response.Write("Unauthorized")
                Response.End()
                Return
            End If
            
            ' SECURITY: Check authorization
            If Not G2SecurityHelper.HasRequiredRole("USER") Then
                Response.StatusCode = 403
                Response.Write("Forbidden")
                Response.End()
                Return
            End If
            
            ' SECURITY: Rate limiting
            Dim clientIP As String = Request.UserHostAddress
            If Not G2SecurityHelper.CheckRateLimit("GeofenceData_" & clientIP, 30, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.Write("Too Many Requests")
                Response.End()
                Return
            End If
            
            ' SECURITY: Validate operation parameter
            Dim operation As String = Request.QueryString("op")
            If Not G2SecurityHelper.ValidateG2Input(operation, G2InputType.AlphaNumeric, 1) Then
                Response.StatusCode = 400
                Response.Write("Invalid operation")
                Response.End()
                Return
            End If
            
            Select Case operation
                Case "0"
                    Response.Write(LoadSecureOssShipToCodeData())
                Case "1"
                    Response.Write(LoadSecureAvlsGeofenceData())
                Case "2"
                    Response.Write(UpdateSecureOssNewShipToCode())
                Case Else
                    Response.StatusCode = 400
                    Response.Write("Invalid operation")
                    Response.End()
                    Return
            End Select
            
            Response.ContentType = "application/json"
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("GEOFENCE_DATA_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Internal Server Error")
        End Try
    End Sub

    Public Function LoadSecureOssShipToCodeData() As String
        Try
            Dim aa As New ArrayList()
            Dim geofencetable As New DataTable
            InitializeShipToCodeTable(geofencetable)
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = "SELECT * FROM oss_ship_to_code WHERE OssSTC = @ossSTC ORDER BY name"
                Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, conn)
                    cmd.Parameters.AddWithValue("@ossSTC", 0)
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        PopulateShipToCodeTable(geofencetable, dr)
                    End Using
                End Using
            End Using
            
            Return ConvertTableToJson(geofencetable)
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("LOAD_OSS_SHIP_TO_CODE_ERROR", ex.Message)
            Return JsonConvert.SerializeObject(New ArrayList())
        End Try
    End Function

    Public Function LoadSecureAvlsGeofenceData() As String
        Try
            Dim aa As New ArrayList()
            Dim geofencetable As New DataTable
            InitializeAvlsGeofenceTable(geofencetable)
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = "SELECT * FROM geofence ORDER BY geofencename"
                Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        PopulateAvlsGeofenceTable(geofencetable, dr)
                    End Using
                End Using
            End Using
            
            Return ConvertTableToJson(geofencetable)
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("LOAD_AVLS_GEOFENCE_ERROR", ex.Message)
            Return JsonConvert.SerializeObject(New ArrayList())
        End Try
    End Function

    Public Function UpdateSecureOssNewShipToCode() As String
        Try
            ' SECURITY: Validate input parameters
            Dim geofenceID As String = Request.QueryString("geofenceID")
            Dim newSTC As String = Request.QueryString("newSTC")
            Dim name As String = Request.QueryString("name")
            
            If Not G2SecurityHelper.ValidateG2Input(geofenceID, G2InputType.GeofenceId) Then
                Return JsonConvert.SerializeObject(New String() {"Invalid geofence ID"})
            End If
            
            If Not G2SecurityHelper.ValidateG2Input(newSTC, G2InputType.AlphaNumeric, 50) Then
                Return JsonConvert.SerializeObject(New String() {"Invalid ship to code"})
            End If
            
            If Not G2SecurityHelper.ValidateG2Input(name, G2InputType.AlphaNumeric, 100) Then
                Return JsonConvert.SerializeObject(New String() {"Invalid name"})
            End If
            
            Dim aa As New ArrayList()
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    Try
                        ' SECURITY: Update geofence with parameterized query
                        Dim updateGeofenceQuery As String = "UPDATE geofence SET shiptocode = @shiptocode, geofencename = @geofencename WHERE geofenceid = @geofenceid"
                        Using cmd1 As SqlCommand = G2SecurityHelper.CreateSecureCommand(updateGeofenceQuery, conn)
                            cmd1.Transaction = transaction
                            cmd1.Parameters.AddWithValue("@shiptocode", newSTC)
                            cmd1.Parameters.AddWithValue("@geofencename", name)
                            cmd1.Parameters.AddWithValue("@geofenceid", geofenceID)
                            cmd1.ExecuteNonQuery()
                        End Using
                        
                        ' SECURITY: Update OSS ship to code with parameterized query
                        Dim updateOssQuery As String = "UPDATE oss_ship_to_code SET OssSTC = @ossSTC WHERE shiptocode = @shiptocode"
                        Using cmd2 As SqlCommand = G2SecurityHelper.CreateSecureCommand(updateOssQuery, conn)
                            cmd2.Transaction = transaction
                            cmd2.Parameters.AddWithValue("@ossSTC", 1)
                            cmd2.Parameters.AddWithValue("@shiptocode", newSTC)
                            cmd2.ExecuteNonQuery()
                        End Using
                        
                        transaction.Commit()
                        aa.Add("1")
                        
                        G2SecurityHelper.LogSecurityEvent("OSS_SHIP_TO_CODE_UPDATE", $"GeofenceId: {geofenceID}, NewSTC: {newSTC}")
                        
                    Catch ex As Exception
                        transaction.Rollback()
                        G2SecurityHelper.LogSecurityEvent("OSS_UPDATE_TRANSACTION_ERROR", ex.Message)
                        aa.Add("0")
                    End Try
                End Using
            End Using
            
            Return JsonConvert.SerializeObject(aa, Formatting.None)
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("UPDATE_OSS_NEW_SHIP_TO_CODE_ERROR", ex.Message)
            Return JsonConvert.SerializeObject(New String() {"0"})
        End Try
    End Function
    
    Private Sub InitializeShipToCodeTable(geofencetable As DataTable)
        geofencetable.Columns.Add(New DataColumn("S No"))
        geofencetable.Columns.Add(New DataColumn("Name"))
        geofencetable.Columns.Add(New DataColumn("Ship To Code"))
        geofencetable.Columns.Add(New DataColumn("Address"))
        geofencetable.Columns.Add(New DataColumn(""))
    End Sub
    
    Private Sub InitializeAvlsGeofenceTable(geofencetable As DataTable)
        geofencetable.Columns.Add(New DataColumn("S No"))
        geofencetable.Columns.Add(New DataColumn("Geofence Name"))
        geofencetable.Columns.Add(New DataColumn("Address"))
        geofencetable.Columns.Add(New DataColumn("ShipToCode"))
    End Sub
    
    Private Sub PopulateShipToCodeTable(geofencetable As DataTable, dr As SqlDataReader)
        While dr.Read()
            Dim r As DataRow = geofencetable.NewRow()
            
            ' SECURITY: Sanitize all data
            r(0) = G2SecurityHelper.SanitizeForHtml(dr("shiptocode").ToString())
            r(1) = G2SecurityHelper.SanitizeForHtml(dr("name").ToString())
            r(2) = G2SecurityHelper.SanitizeForHtml(dr("shiptocode").ToString())
            
            Dim address As String = ""
            If Not IsDBNull(dr("address5")) AndAlso Not String.IsNullOrEmpty(dr("address5").ToString()) Then
                address = G2SecurityHelper.SanitizeForHtml(dr("address5").ToString().Replace(vbCr, " ").Replace(vbLf, " "))
            End If
            r(3) = address
            
            ' SECURITY: Create safe action link
            Dim shipToCode As String = G2SecurityHelper.SanitizeForJavaScript(dr("shiptocode").ToString())
            Dim name As String = G2SecurityHelper.SanitizeForJavaScript(dr("name").ToString())
            r(4) = $"<a href=""javascript:void(0)"" onclick=""MoveShipToCode('{shipToCode}','{name}')"" title=""Copy ShipToCode"" id='{shipToCode}a'><img src='images/rightarrow.png' alt='right' style='width:12px;height:12px;' id='{shipToCode}i'/></a>"
            
            geofencetable.Rows.Add(r)
        End While
        
        ' Add empty row if no data
        If geofencetable.Rows.Count = 0 Then
            Dim r As DataRow = geofencetable.NewRow()
            For i As Integer = 0 To 4
                r(i) = "-"
            Next
            geofencetable.Rows.Add(r)
        End If
    End Sub
    
    Private Sub PopulateAvlsGeofenceTable(geofencetable As DataTable, dr As SqlDataReader)
        While dr.Read()
            Dim r As DataRow = geofencetable.NewRow()
            
            Try
                ' SECURITY: Safely process geofence data
                Dim latlng As String = ""
                If CBool(dr("geofencetype")) Then
                    Dim dataStr As String = dr("data").ToString()
                    If dataStr.Contains(";") AndAlso dataStr.Split(";")(0).Contains(",") Then
                        Dim firstPoint As String = dataStr.Split(";")(0)
                        Dim coords As String() = firstPoint.Split(","c)
                        If coords.Length >= 2 Then
                            latlng = coords(1) & "," & coords(0)
                        End If
                    End If
                Else
                    Dim dataStr As String = dr("data").ToString()
                    If dataStr.Contains(",") Then
                        Dim coords As String() = dataStr.Split(","c)
                        If coords.Length >= 2 Then
                            latlng = coords(1) & "," & coords(0)
                        End If
                    End If
                End If
                
                ' SECURITY: Sanitize all output
                r(0) = G2SecurityHelper.SanitizeForHtml(dr("geofenceid").ToString())
                
                Dim geofenceId As String = G2SecurityHelper.SanitizeForHtml(dr("geofenceid").ToString())
                Dim geofenceName As String = G2SecurityHelper.SanitizeForHtml(dr("geofencename").ToString())
                r(1) = $"<span id='{geofenceId}' style='cursor:pointer;' onclick='selectSTC(this)'>{geofenceName}</span>"
                
                If Not String.IsNullOrEmpty(latlng) Then
                    r(2) = $"<a href='http://maps.google.com/maps?f=q&hl=en&q={HttpUtility.UrlEncode(latlng)}&om=1&t=k' target='_blank' style='text-decoration:none;color:blue;'>{G2SecurityHelper.SanitizeForHtml(latlng)}</a>"
                Else
                    r(2) = "-"
                End If
                
                r(3) = $"<span id='{geofenceId}s'>{G2SecurityHelper.SanitizeForHtml(dr("shiptocode").ToString())}</span>"
                
                geofencetable.Rows.Add(r)
                
            Catch ex As Exception
                G2SecurityHelper.LogSecurityEvent("POPULATE_AVLS_GEOFENCE_ROW_ERROR", ex.Message)
                ' Skip this row and continue
            End Try
        End While
        
        ' Add empty row if no data
        If geofencetable.Rows.Count = 0 Then
            Dim r As DataRow = geofencetable.NewRow()
            For i As Integer = 0 To 3
                r(i) = "-"
            Next
            geofencetable.Rows.Add(r)
        End If
    End Sub
    
    Private Function ConvertTableToJson(geofencetable As DataTable) As String
        Try
            Dim aa As New ArrayList()
            
            For i As Integer = 0 To geofencetable.Rows.Count - 1
                Dim a As New ArrayList()
                For j As Integer = 0 To geofencetable.Columns.Count - 1
                    a.Add(geofencetable.DefaultView.Item(i)(j))
                Next
                aa.Add(a)
            Next
            
            Return JsonConvert.SerializeObject(aa, Formatting.None)
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("CONVERT_TABLE_TO_JSON_ERROR", ex.Message)
            Return JsonConvert.SerializeObject(New ArrayList())
        End Try
    End Function

End Class