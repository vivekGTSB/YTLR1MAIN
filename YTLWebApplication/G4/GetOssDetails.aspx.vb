Imports System.Data.SqlClient
Imports System.Collections.Generic

Partial Class GetOssDetails
    Inherits System.Web.UI.Page
    Public sb1 As New StringBuilder()

    Public Sub FillGrid()
        Try
            ' SECURITY FIX: Validate authentication
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate and sanitize DN ID parameter
            Dim dnid As String = SecurityHelper.SanitizeForHtml(Request.QueryString("p"))
            If String.IsNullOrEmpty(dnid) OrElse Not SecurityHelper.ValidateInput(dnid, 50, "^[A-Za-z0-9_-]+$") Then
                sb1.Append("<tbody><tr><td colspan='14'>Invalid DN ID</td></tr></tbody>")
                Return
            End If

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
                                SecurityHelper.LogError("Error processing geofence data", ex, Server)
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
                                SecurityHelper.LogError("Error processing vehicle data", ex, Server)
                            End Try
                        End While
                    End Using
                    conn.Close()
                End Using
            End Using

            ' Get OSS details with parameterized query
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("chk"))
            t.Columns.Add(New DataColumn("S NO"))
            t.Columns.Add(New DataColumn("Plate NO"))
            t.Columns.Add(New DataColumn("Unit ID"))
            t.Columns.Add(New DataColumn("Transporter"))
            t.Columns.Add(New DataColumn("Source"))
            t.Columns.Add(New DataColumn("DN NO"))
            t.Columns.Add(New DataColumn("Weight Outtime"))
            t.Columns.Add(New DataColumn("Ship To Code"))
            t.Columns.Add(New DataColumn("Ship To Name"))
            t.Columns.Add(New DataColumn("ATA"))
            t.Columns.Add(New DataColumn("GPS Point"))
            t.Columns.Add(New DataColumn("Status"))
            t.Columns.Add(New DataColumn("Remarks"))
            t.Columns.Add(New DataColumn("DN ID"))
            t.Columns.Add(New DataColumn("Group Name"))

            Using conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                ' SECURITY FIX: Use parameterized query
                Dim query As String = "SELECT * FROM oss_patch_out WHERE dn_id = @dnid AND status = 7"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn2)
                    cmd.Parameters.AddWithValue("@dnid", dnid)

                    conn2.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim i As Int32 = 1
                        While dr.Read()
                            Try
                                Dim r As DataRow = t.NewRow

                                r(0) = SecurityHelper.SanitizeForHtml(dr("patch_no").ToString())
                                r(1) = i.ToString()
                                r(2) = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
                                r(3) = SecurityHelper.SanitizeForHtml(dr("unitid").ToString())

                                If IsDBNull(dr("transporter")) Then
                                    r(4) = "--"
                                Else
                                    r(4) = SecurityHelper.SanitizeForHtml(dr("transporter").ToString())
                                End If

                                r(5) = SecurityHelper.SanitizeForHtml(dr("source_supply").ToString())
                                r(6) = SecurityHelper.SanitizeForHtml(dr("dn_no").ToString())

                                If Not IsDBNull(dr("weight_outtime")) Then
                                    r(7) = Convert.ToDateTime(dr("weight_outtime")).ToString("yyyy/MM/dd HH:mm:ss")
                                Else
                                    r(7) = "--"
                                End If

                                r(8) = SecurityHelper.SanitizeForHtml(dr("destination_siteid").ToString())

                                Dim destSiteId As Integer
                                If Integer.TryParse(dr("destination_siteid").ToString(), destSiteId) AndAlso ShipToNameDict.ContainsKey(destSiteId) Then
                                    r(9) = ShipToNameDict(destSiteId).ToUpper()
                                Else
                                    r(9) = "--"
                                End If

                                If Not IsDBNull(dr("ata_datetime")) Then
                                    r(10) = Convert.ToDateTime(dr("ata_datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                                Else
                                    r(10) = "--"
                                End If

                                If Not IsDBNull(dr("lat")) AndAlso Not IsDBNull(dr("lon")) Then
                                    Dim lat As Double = Convert.ToDouble(dr("lat"))
                                    Dim lon As Double = Convert.ToDouble(dr("lon"))
                                    If SecurityHelper.ValidateCoordinate(lat.ToString(), lon.ToString()) Then
                                        r(11) = lat.ToString("0.0000") & "," & lon.ToString("0.0000")
                                    Else
                                        r(11) = "--"
                                    End If
                                Else
                                    r(11) = "--"
                                End If

                                Dim status As String = dr("status").ToString()
                                Select Case status
                                    Case "0"
                                        If Not IsDBNull(dr("destination_siteid")) Then
                                            status = "Waiting To Process"
                                        Else
                                            status = "Waiting for Ship To Code"
                                        End If
                                    Case "1" : status = "No GPS Device"
                                    Case "2" : status = "Pending Destination Set Up"
                                    Case "3" : status = "In Progress"
                                    Case "4" : status = "Geofence In"
                                    Case "5" : status = "Inside Geofence"
                                    Case "6" : status = "Geofence Out"
                                    Case "7" : status = "Delivery Completed"
                                    Case "8" : status = "Delivery Completed (E)"
                                    Case "10" : status = "Timeout"
                                    Case "11" : status = "Reprocess Job"
                                    Case "12" : status = "Delivery Completed (D)"
                                    Case "13" : status = "Delivery Completed (P)"
                                    Case "14" : status = "No GPS Data"
                                    Case Else : status = "Unknown"
                                End Select

                                r(12) = status

                                If Not IsDBNull(dr("remarks")) Then
                                    r(13) = SecurityHelper.SanitizeForHtml(dr("remarks").ToString())
                                Else
                                    r(13) = "--"
                                End If

                                r(14) = SecurityHelper.SanitizeForHtml(dr("dn_id").ToString())

                                Dim plateNo As String = dr("plateno").ToString()
                                If vehicleDict.ContainsKey(plateNo) Then
                                    r(15) = vehicleDict(plateNo)
                                Else
                                    r(15) = ""
                                End If

                                t.Rows.Add(r)
                                i += 1

                            Catch ex As Exception
                                SecurityHelper.LogError("Error processing OSS detail record", ex, Server)
                            End Try
                        End While
                    End Using
                    conn2.Close()
                End Using
            End Using

            If t.Rows.Count = 0 Then
                Dim r As DataRow = t.NewRow
                For i As Integer = 0 To 15
                    r(i) = "--"
                Next
                t.Rows.Add(r)
            End If

            ' Build HTML table
            sb1.Append("<thead><tr align=""left""><th>S No</th><th>Plate NO</th><th>Unit ID</th><th>Group Name</th><th>Transporter</th><th>Source</th><th>DN ID</th><th>DN NO</th><th>Weight Out Time</th><th>Ship To Code</th><th>Ship To Name</th><th>ATA</th><th>GPS Point</th><th>Status</th></tr></thead>")
            sb1.Append("<tbody>")

            For i As Integer = 0 To t.Rows.Count - 1
                Try
                    sb1.Append("<tr>")
                    sb1.Append("<td>" & t.DefaultView.Item(i)(1) & "</td>") ' S No
                    sb1.Append("<td>" & t.DefaultView.Item(i)(2) & "</td>") ' Plate NO
                    sb1.Append("<td>" & t.DefaultView.Item(i)(3) & "</td>") ' Unit ID
                    sb1.Append("<td>" & t.DefaultView.Item(i)(15) & "</td>") ' Group Name
                    sb1.Append("<td>" & t.DefaultView.Item(i)(4) & "</td>") ' Transporter
                    sb1.Append("<td>" & t.DefaultView.Item(i)(5) & "</td>") ' Source
                    sb1.Append("<td>" & t.DefaultView.Item(i)(14) & "</td>") ' DN ID
                    sb1.Append("<td>" & t.DefaultView.Item(i)(6) & "</td>") ' DN NO
                    sb1.Append("<td>" & t.DefaultView.Item(i)(7) & "</td>") ' Weight Out Time
                    sb1.Append("<td>" & t.DefaultView.Item(i)(8) & "</td>") ' Ship To Code
                    sb1.Append("<td>" & t.DefaultView.Item(i)(9) & "</td>") ' Ship To Name
                    sb1.Append("<td>" & t.DefaultView.Item(i)(10) & "</td>") ' ATA
                    sb1.Append("<td>" & t.DefaultView.Item(i)(11) & "</td>") ' GPS Point
                    sb1.Append("<td>" & t.DefaultView.Item(i)(12) & "</td>") ' Status
                    sb1.Append("</tr>")
                Catch ex As Exception
                    SecurityHelper.LogError("Error building HTML row", ex, Server)
                End Try
            Next

            sb1.Append("</tbody>")
            sb1.Append("<tfoot><tr style=""font-weight:bold;"" align=""left""><th>S No</th><th>Plate NO</th><th>Unit ID</th><th>Group Name</th><th>Transporter</th><th>Source</th><th>DN ID</th><th>DN NO</th><th>Weight Out Time</th><th>Ship To Code</th><th>Ship To Name</th><th>ATA</th><th>GPS Point</th><th>Status</th></tr></tfoot>")

        Catch ex As Exception
            SecurityHelper.LogError("FillGrid error", ex, Server)
            sb1.Append("<tbody><tr><td colspan='14'>An error occurred while loading data</td></tr></tbody>")
        End Try
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        FillGrid()
    End Sub
End Class