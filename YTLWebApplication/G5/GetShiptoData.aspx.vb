Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Public Class GetShiptoData
    Inherits SecurePageBase

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.Write("{""error"":""Unauthorized""}")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate and sanitize query parameters
            Dim bdt As String = SecurityHelper.SanitizeForHtml(Request.QueryString("bdt"))
            Dim edt As String = SecurityHelper.SanitizeForHtml(Request.QueryString("edt"))
            Dim typeParam As String = SecurityHelper.SanitizeForHtml(Request.QueryString("type"))

            ' SECURITY FIX: Validate date parameters
            If Not SecurityHelper.ValidateDate(bdt) OrElse Not SecurityHelper.ValidateDate(edt) Then
                Response.StatusCode = 400
                Response.Write("{""error"":""Invalid date format""}")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate type parameter
            Dim type As Integer = 0
            If Not Integer.TryParse(typeParam, type) OrElse type < 0 OrElse type > 2 Then
                Response.StatusCode = 400
                Response.Write("{""error"":""Invalid type parameter""}")
                Response.End()
                Return
            End If

            Response.ContentType = "application/json"
            Response.Write(FillGrid(bdt, edt, type))

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GENERAL_ERROR", "Error in GetShiptoData: " & ex.Message)
            Response.StatusCode = 500
            Response.Write("{""error"":""Internal server error""}")
        End Try
    End Sub

    Public Function FillGrid(bdt As String, edt As String, type As Integer) As String
        Try
            Dim pendingdesti As New List(Of Integer)
            Dim aa As New ArrayList()

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                ' SECURITY FIX: Get pending destinations with parameterized query
                GetPendingDestinations(conn, bdt, edt, type, pendingdesti)
                
                ' SECURITY FIX: Get ship-to data with parameterized query
                GetShipToData(conn, bdt, edt, type, pendingdesti, aa)
            End Using

            Return JsonConvert.SerializeObject(aa, Formatting.None)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("DATABASE_ERROR", "Error in FillGrid: " & ex.Message)
            Return "{""error"":""Database error""}"
        End Try
    End Function

    Private Sub GetPendingDestinations(conn As SqlConnection, bdt As String, edt As String, type As Integer, pendingdesti As List(Of Integer))
        Try
            Dim query As String = ""
            Select Case type
                Case 0
                    query = "SELECT DISTINCT destination_siteid FROM oss_patch_out WHERE weight_outtime BETWEEN @bdt AND @edt AND status = 2"
                Case 1
                    query = "SELECT DISTINCT destination_siteid FROM oss_patch_out WHERE weight_outtime BETWEEN @bdt AND @edt AND status = 2 AND productcode IN (SELECT productid FROM oss_product_master WHERE producttype = 1)"
                Case 2
                    query = "SELECT DISTINCT destination_siteid FROM oss_patch_out WHERE weight_outtime BETWEEN @bdt AND @edt AND status = 2 AND productcode IN (SELECT productid FROM oss_product_master WHERE producttype = 2)"
            End Select

            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                cmd.Parameters.AddWithValue("@bdt", bdt)
                cmd.Parameters.AddWithValue("@edt", edt)
                
                conn.Open()
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        pendingdesti.Add(Convert.ToInt32(dr("destination_siteid")))
                    End While
                End Using
                conn.Close()
            End Using

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("PENDING_DESTINATIONS_ERROR", "Error getting pending destinations: " & ex.Message)
        End Try
    End Sub

    Private Sub GetShipToData(conn As SqlConnection, bdt As String, edt As String, type As Integer, pendingdesti As List(Of Integer), aa As ArrayList)
        Try
            Dim query As String = ""
            Select Case type
                Case 0
                    query = "SELECT geofencename as shiptoname, t1.shiptocode, t2.count FROM YTLDB.dbo.geofence as t1, (SELECT DISTINCT TOP 50000 destination_siteid as shiptocode, COUNT(*) as count FROM oss_patch_out WHERE weight_outtime BETWEEN @bdt AND @edt GROUP BY destination_siteid ORDER BY count DESC) as t2 WHERE t1.shiptocode = t2.shiptocode ORDER BY t1.geofencename"
                Case 1
                    query = "SELECT t1.name as shiptoname, t2.shiptocode, t2.count FROM oss_ship_to_code as t1, (SELECT TOP 50000 destination_siteid as shiptocode, COUNT(*) as count FROM oss_patch_out WHERE weight_outtime BETWEEN @bdt AND @edt AND productcode IN (SELECT productid FROM oss_product_master WHERE producttype = 1) GROUP BY destination_siteid ORDER BY count DESC) as t2 WHERE t1.shiptocode = t2.shiptocode ORDER BY t1.name"
                Case 2
                    query = "SELECT t1.name as shiptoname, t2.shiptocode, t2.count FROM oss_ship_to_code as t1, (SELECT TOP 50000 destination_siteid as shiptocode, COUNT(*) as count FROM oss_patch_out WHERE weight_outtime BETWEEN @bdt AND @edt AND productcode IN (SELECT productid FROM oss_product_master WHERE producttype = 2) GROUP BY destination_siteid ORDER BY count DESC) as t2 WHERE t1.shiptocode = t2.shiptocode ORDER BY t1.name"
            End Select

            Dim ShiptoTable As New DataTable()
            ShiptoTable.Columns.Add(New DataColumn("Code"))
            ShiptoTable.Columns.Add(New DataColumn("Name"))
            ShiptoTable.Columns.Add(New DataColumn("Status"))

            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                cmd.Parameters.AddWithValue("@bdt", bdt)
                cmd.Parameters.AddWithValue("@edt", edt)
                
                conn.Open()
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        Try
                            Dim r As DataRow = ShiptoTable.NewRow()
                            r(0) = SecurityHelper.SanitizeForHtml(dr("shiptocode").ToString())
                            r(1) = SecurityHelper.SanitizeForHtml($"{dr("shiptoname").ToString().ToUpper()} @ {dr("count")} - {dr("shiptocode")}")
                            
                            If pendingdesti.Contains(Convert.ToInt32(dr("shiptocode"))) Then
                                r(2) = "1"
                            Else
                                r(2) = "0"
                            End If
                            
                            ShiptoTable.Rows.Add(r)
                        Catch ex As Exception
                            SecurityHelper.LogSecurityEvent("SHIPTO_ROW_ERROR", "Error processing ship-to row: " & ex.Message)
                        End Try
                    End While
                End Using
                conn.Close()
            End Using

            If ShiptoTable.Rows.Count = 0 Then
                Dim r As DataRow = ShiptoTable.NewRow()
                r(0) = "-"
                r(1) = "-"
                r(2) = "-"
                ShiptoTable.Rows.Add(r)
            End If

            For i As Integer = 0 To ShiptoTable.Rows.Count - 1
                Try
                    Dim a As New ArrayList()
                    a.Add(ShiptoTable.Rows(i)(0))
                    a.Add(ShiptoTable.Rows(i)(1))
                    a.Add(ShiptoTable.Rows(i)(2))
                    aa.Add(a)
                Catch ex As Exception
                    SecurityHelper.LogSecurityEvent("SHIPTO_ARRAY_ERROR", "Error adding ship-to data to array: " & ex.Message)
                End Try
            Next

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("SHIPTO_DATA_ERROR", "Error getting ship-to data: " & ex.Message)
        End Try
    End Sub

End Class