Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.Web.Script.Services
Imports System.IO.Compression
Imports AspMap
Imports System.Net

Partial Class GetOssEta
    Inherits System.Web.UI.Page
    Public connstr As String
    Public uid As Integer, urole, uuserslist As String

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate authentication
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            ' SECURITY FIX: Get user data from session instead of cookies
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            uid = Convert.ToInt32(userid)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            urole = role
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
            uuserslist = userslist
            
            Response.Write(GetJson())
            Response.ContentType = "application/json"

        Catch ex As Exception
            SecurityHelper.LogError("GetOssEta Page_Load error", ex, Server)
            Response.StatusCode = 500
            Response.Write("An error occurred")
        End Try
    End Sub

    Protected Function GetJson() As String
        Dim json As String = ""
        Dim aa As New ArrayList()

        Try
            Dim unitstable As New DataTable
            unitstable.Columns.Add(New DataColumn("Sno"))
            unitstable.Columns.Add(New DataColumn("DN NO"))
            unitstable.Columns.Add(New DataColumn("Plate NO"))
            unitstable.Columns.Add(New DataColumn("Unit ID"))
            unitstable.Columns.Add(New DataColumn("Group Name"))
            unitstable.Columns.Add(New DataColumn("Source"))
            unitstable.Columns.Add(New DataColumn("Weight Out Time"))
            unitstable.Columns.Add(New DataColumn("Ship To Code"))
            unitstable.Columns.Add(New DataColumn("Ship To Name"))
            unitstable.Columns.Add(New DataColumn("Distance"))
            unitstable.Columns.Add(New DataColumn("duration"))
            unitstable.Columns.Add(New DataColumn("ETA"))
            unitstable.Columns.Add(New DataColumn("Pduration"))

            ' SECURITY FIX: Get user data from session instead of cookies
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Dim tankerQuery As String = "SELECT plateno FROM vehicleTBL"
            If role = "User" Then
                tankerQuery = "SELECT plateno FROM vehicleTBL WHERE userid = @userid"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If SecurityHelper.IsValidUsersList(userslist) Then
                    tankerQuery = "SELECT plateno FROM vehicleTBL WHERE userid IN (" & userslist & ")"
                Else
                    tankerQuery = "SELECT plateno FROM vehicleTBL WHERE userid = @userid"
                End If
            End If

            ' SECURITY FIX: Get geofence data with parameterized query
            Dim ShipToNameDict As New Dictionary(Of Integer, String)
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim shipToCodeQuery As String = "SELECT geofencename,shiptocode FROM geofence WHERE accesstype='1' ORDER BY LTRIM(geofencename)"
                
                ' Get plate numbers with security validation
                Dim plateNumbers As New List(Of String)
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(tankerQuery, conn)
                    If role = "User" OrElse (role <> "Admin" AndAlso Not SecurityHelper.IsValidUsersList(userslist)) Then
                        cmd.Parameters.AddWithValue("@userid", userid)
                    End If

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            If Not IsDBNull(dr("plateno")) Then
                                Dim plateNo As String = dr("plateno").ToString()
                                If SecurityHelper.ValidatePlateNumber(plateNo) Then
                                    plateNumbers.Add(plateNo)
                                End If
                            End If
                        End While
                    End Using
                End Using

                ' Get geofence data
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(shipToCodeQuery, conn)
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Try
                                Dim shipToCode As Integer
                                If Integer.TryParse(dr("shiptocode").ToString(), shipToCode) AndAlso Not ShipToNameDict.ContainsKey(shipToCode) Then
                                    ShipToNameDict.Add(shipToCode, SecurityHelper.SanitizeForHtml(dr("geofencename").ToString().ToUpper()))
                                End If
                            Catch ex As Exception
                                WriteLog("Error processing geofence data: " & ex.Message)
                            End Try
                        End While
                    End Using
                End Using
                conn.Close()
            End Using

            ' Build plate condition for security
            Dim platecond As String = ""
            If plateNumbers.Count > 0 Then
                Dim quotedPlates As New List(Of String)
                For Each plateNo As String In plateNumbers
                    quotedPlates.Add("'" & SecurityHelper.SanitizeForHtml(plateNo) & "'")
                Next
                platecond = " AND plateno IN (" & String.Join(",", quotedPlates) & ")"
            End If

            ' Get vehicle group data
            Dim vehicleDict As New Dictionary(Of String, String)
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim vehicleQuery As String = "SELECT plateno,groupname FROM vehicleTBL WHERE plateno <> '' " & platecond
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(vehicleQuery, conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Try
                                Dim plateNo As String = dr("plateno").ToString()
                                If plateNo.Contains("_") Then
                                    plateNo = plateNo.Split("_"c)(0)
                                End If
                                If Not vehicleDict.ContainsKey(plateNo) Then
                                    vehicleDict.Add(plateNo, SecurityHelper.SanitizeForHtml(dr("groupname").ToString()))
                                End If
                            Catch ex As Exception
                                WriteLog("Error processing vehicle data: " & ex.Message)
                            End Try
                        End While
                    End Using
                    conn.Close()
                End Using
            End Using

            ' Get transporter data
            Dim TransNameDict As New Dictionary(Of String, Integer)
            Using conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                Using cmdT As New SqlCommand("SELECT DISTINCT transporter_name, transporter_id FROM oss_transporter ORDER BY transporter_name", conn2)
                    conn2.Open()
                    Using drT As SqlDataReader = cmdT.ExecuteReader()
                        While drT.Read()
                            Try
                                Dim transporterName As String = drT("transporter_name").ToString().ToUpper()
                                Dim transporterId As Integer = Convert.ToInt32(drT("transporter_id"))
                                If Not TransNameDict.ContainsKey(transporterName) Then
                                    TransNameDict.Add(transporterName, transporterId)
                                End If
                            Catch ex As Exception
                                WriteLog("Error processing transporter data: " & ex.Message)
                            End Try
                        End While
                    End Using
                    conn2.Close()
                End Using

                ' Get OSS ETA data
                Dim bdt As String = DateTime.Now.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss")
                Dim edt As String = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

                Using cmd As New SqlCommand("SELECT * FROM oss_patch_out WHERE weight_outtime BETWEEN @bdt AND @edt AND status='3'", conn2)
                    cmd.Parameters.AddWithValue("@bdt", bdt)
                    cmd.Parameters.AddWithValue("@edt", edt)

                    conn2.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim i As Int32 = 1
                        While dr.Read()
                            Try
                                Dim transporterName As String = dr("transporter").ToString().ToUpper()
                                If TransNameDict.ContainsKey(transporterName) Then
                                    Dim r As DataRow = unitstable.NewRow

                                    r(0) = i.ToString()
                                    r(1) = SecurityHelper.SanitizeForHtml(dr("dn_no").ToString())
                                    r(2) = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
                                    r(3) = SecurityHelper.SanitizeForHtml(dr("unitid").ToString())

                                    Dim plateNo As String = dr("plateno").ToString()
                                    If vehicleDict.ContainsKey(plateNo) Then
                                        r(4) = vehicleDict(plateNo)
                                    Else
                                        r(4) = ""
                                    End If

                                    r(5) = SecurityHelper.SanitizeForHtml(dr("source_supply").ToString())

                                    If Not IsDBNull(dr("weight_outtime")) Then
                                        r(6) = Convert.ToDateTime(dr("weight_outtime")).ToString("yyyy/MM/dd HH:mm:ss")
                                    Else
                                        r(6) = "--"
                                    End If

                                    r(7) = SecurityHelper.SanitizeForHtml(dr("destination_siteid").ToString())

                                    Dim destSiteId As Integer
                                    If Integer.TryParse(dr("destination_siteid").ToString(), destSiteId) AndAlso ShipToNameDict.ContainsKey(destSiteId) Then
                                        r(8) = ShipToNameDict(destSiteId).ToUpper()
                                    Else
                                        r(8) = "--"
                                    End If

                                    If Not IsDBNull(dr("distance")) AndAlso dr("distance").ToString() <> "0" Then
                                        Dim d1 As Double = Convert.ToDouble(dr("distance"))
                                        r(9) = d1.ToString("0.0")
                                    Else
                                        r(9) = "0"
                                    End If

                                    If Not IsDBNull(dr("duration")) Then
                                        Dim iSecond As Double = Convert.ToDouble(dr("duration"))
                                        Dim iSpan As TimeSpan = TimeSpan.FromSeconds(iSecond)

                                        If iSecond > 3600 Then
                                            r(10) = iSpan.Hours.ToString.PadLeft(2, "0"c) & " h " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " min "
                                        ElseIf iSecond > 60 Then
                                            r(10) = iSpan.Minutes.ToString.PadLeft(2, "0"c) & " min "
                                        Else
                                            r(10) = "0"
                                        End If
                                    Else
                                        r(10) = "0"
                                    End If

                                    If IsDBNull(dr("est_arrivaltime")) Then
                                        r(11) = "--"
                                        r(12) = "--"
                                    Else
                                        r(11) = Convert.ToDateTime(dr("est_arrivaltime")).ToString("yyyy/MM/dd HH:mm:ss")
                                        
                                        Dim d11 As DateTime = Convert.ToDateTime(dr("weight_outtime"))
                                        Dim d2 As DateTime = Convert.ToDateTime(dr("est_arrivaltime"))
                                        Dim duration As Double = (d2 - d11).TotalSeconds
                                        Dim iSpan As TimeSpan = TimeSpan.FromSeconds(duration)
                                        
                                        If duration > 3600 Then
                                            r(12) = iSpan.Hours.ToString.PadLeft(2, "0"c) & " h " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " min "
                                        ElseIf duration > 60 Then
                                            r(12) = iSpan.Minutes.ToString.PadLeft(2, "0"c) & " min "
                                        Else
                                            r(12) = "0"
                                        End If
                                    End If

                                    unitstable.Rows.Add(r)
                                    i += 1
                                End If
                            Catch ex As Exception
                                WriteLog("Error processing ETA record: " & ex.Message)
                            End Try
                        End While
                    End Using
                    conn2.Close()
                End Using
            End Using

            If unitstable.Rows.Count = 0 Then
                Dim r As DataRow = unitstable.NewRow
                For i As Integer = 0 To 12
                    r(i) = "--"
                Next
                unitstable.Rows.Add(r)
            End If

            ' Convert to ArrayList for JSON serialization
            For j As Integer = 0 To unitstable.Rows.Count - 1
                Try
                    Dim a As New ArrayList()
                    For k As Integer = 0 To 12
                        a.Add(unitstable.DefaultView.Item(j)(k))
                    Next
                    aa.Add(a)
                Catch ex As Exception
                    WriteLog("Error building result array: " & ex.Message)
                End Try
            Next

            json = JsonConvert.SerializeObject(aa, Formatting.None)
            
            If HttpContext.Current.Request.Headers("Accept-Encoding") IsNot Nothing AndAlso 
               HttpContext.Current.Request.Headers("Accept-Encoding").ToLower().Contains("gzip") Then
                HttpContext.Current.Response.AppendHeader("Content-Encoding", "gzip")
                HttpContext.Current.Response.Filter = New GZipStream(HttpContext.Current.Response.Filter, CompressionMode.Compress)
            End If

        Catch ex As Exception
            WriteLog(ex.Message + " , " + ex.Message)
            json = "{""error"":""An error occurred""}"
        End Try
        
        Return json
    End Function

    Protected Sub WriteLog(ByVal message As String)
        Try
            If message.Length > 0 Then
                Dim logPath As String = Server.MapPath("") & "\Getoss.Log.txt"
                Dim sanitizedMessage As String = SecurityHelper.SanitizeLogMessage(message)
                Dim logEntry As String = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & " - " & sanitizedMessage & Environment.NewLine
                
                SyncLock GetType(GetOssEta)
                    System.IO.File.AppendAllText(logPath, logEntry)
                End SyncLock
            End If
        Catch ex As Exception
            ' Fail silently to prevent information disclosure
        End Try
    End Sub

End Class