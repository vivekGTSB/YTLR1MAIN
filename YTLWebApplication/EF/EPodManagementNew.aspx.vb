Imports AspMap
Imports System.Data
Imports System.Data.SqlClient

Partial Class EPodManagementNew
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public show As Boolean = False
    Public sb1 As New StringBuilder()
    Public adminusers As String
    
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' Check authentication using secure helper
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("INIT_ERROR", "Error in OnInit: " & ex.Message)
            Response.Redirect("~/Login.aspx")
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' Validate session
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ddlGeofence.Items.Add(New ListItem("ALL SHIP-TO-CODE", "ALL GEOFENCES"))
                
                Dim query As String = "SELECT DISTINCT shiptocode, geofencename FROM geofence WHERE accesstype='1' AND shiptocode <> '0' ORDER BY geofencename, shiptocode"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    Using da As New SqlDataAdapter(cmd)
                        Dim ds As New DataSet
                        da.Fill(ds)
                        
                        For count As Integer = 0 To ds.Tables(0).Rows.Count - 1
                            Dim geofenceName As String = SecurityHelper.SanitizeForHtml(ds.Tables(0).Rows(count)("geofencename").ToString().ToUpper())
                            Dim shipToCode As String = SecurityHelper.SanitizeForHtml(ds.Tables(0).Rows(count)("shiptocode").ToString().ToUpper())
                            ddlGeofence.Items.Add(New ListItem(geofenceName & "[ " & shipToCode & " ]", ds.Tables(0).Rows(count)("shiptocode").ToString()))
                        Next
                    End Using
                End Using
            End Using

            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                FillTransporters()
            Else
                ' Validate dropdown selection
                If SecurityHelper.ValidateInput(Request.Form("ddlGeofence"), "alphanumeric") Then
                    ddlGeofence.SelectedValue = Request.Form("ddlGeofence")
                End If
            End If
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("PAGE_LOAD_ERROR", "Error in Page_Load: " & ex.Message)
        End Try
    End Sub
    
    Sub FillTransporters()
        Try
            ' Get user info from session
            Dim userid As String = HttpContext.Current.Session("userid").ToString()
            Dim role As String = HttpContext.Current.Session("role").ToString()
            Dim userslist As String = HttpContext.Current.Session("userslist").ToString()
            
            Dim tankerQuery As String = "SELECT plateno FROM vehicleTBL"

            If role = "User" Then
                tankerQuery = "SELECT plateno FROM vehicleTBL WHERE userid = @userid"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                tankerQuery = "SELECT plateno FROM vehicleTBL WHERE userid IN (" & userslist & ")"
            End If
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(tankerQuery, conn)
                    If role = "User" Then
                        cmd.Parameters.AddWithValue("@userid", userid)
                    End If
                    
                    Using da As New SqlDataAdapter(cmd)
                        Dim ds As New DataSet
                        da.Fill(ds)
                        
                        Dim platecond As String = ""
                        For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                            If Not IsDBNull(ds.Tables(0).Rows(i)("plateno")) Then
                                platecond = platecond & "'" & SecurityHelper.SanitizeForSql(ds.Tables(0).Rows(i)("plateno").ToString()) & "',"
                            End If
                        Next

                        If platecond.Length > 3 Then
                            platecond = platecond.Substring(0, platecond.Length - 1)
                            platecond = " WHERE weight_outtime BETWEEN @startDate AND @endDate AND plateno IN (" & platecond & ")"
                        End If

                        Using conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                            Dim transporterQuery As String = "SELECT DISTINCT transporter FROM oss_patch_out " & platecond & " ORDER BY transporter"
                            Using cmd2 As SqlCommand = SecurityHelper.CreateSafeCommand(transporterQuery, conn2)
                                cmd2.Parameters.AddWithValue("@startDate", DateTime.Now.AddDays(-180).ToString("yyyy/MM/dd HH:mm:ss"))
                                cmd2.Parameters.AddWithValue("@endDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                                
                                conn2.Open()
                                Using dr As SqlDataReader = cmd2.ExecuteReader()
                                    ddlTransport.Items.Add("ALL TRANSPORTERS")
                                    
                                    While dr.Read()
                                        If Not IsDBNull(dr("transporter")) Then
                                            Dim transporter As String = SecurityHelper.SanitizeForHtml(dr("transporter").ToString())
                                            ddlTransport.Items.Add(New ListItem(transporter, transporter))
                                        End If
                                    End While
                                End Using
                            End Using
                        End Using
                    End Using
                End Using
            End Using

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("FILL_TRANSPORTERS_ERROR", "Error filling transporters: " & ex.Message)
        End Try
    End Sub
    
    Protected Sub DisplayLogInformation()
        Try
            ' Validate date inputs
            If Not SecurityHelper.ValidateInput(txtBeginDate.Value, "date") OrElse 
               Not SecurityHelper.ValidateInput(txtEndDate.Value, "date") Then
                SecurityHelper.LogSecurityEvent("INVALID_DATE_INPUT", "Invalid date format in DisplayLogInformation")
                Return
            End If

            Dim begindatetime As String = txtBeginDate.Value & " 00:00:00"
            Dim enddatetime As String = txtEndDate.Value & " 23:59:59"
            
            ' Get user info from session
            Dim uid As String = HttpContext.Current.Session("userid").ToString()
            Dim role As String = HttpContext.Current.Session("role").ToString()
            Dim userslist As String = HttpContext.Current.Session("userslist").ToString()
            
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("Patch No"))
            t.Columns.Add(New DataColumn("Driver Ic"))
            t.Columns.Add(New DataColumn("Weight Out Date"))
            t.Columns.Add(New DataColumn("DN No"))
            t.Columns.Add(New DataColumn("QR Code"))
            t.Columns.Add(New DataColumn("Submitted Date"))
            t.Columns.Add(New DataColumn("Status"))
            t.Columns.Add(New DataColumn("UploadStatus"))
            t.Columns.Add(New DataColumn("Transporter"))
            t.Columns.Add(New DataColumn("Weight Out Time"))
            t.Columns.Add(New DataColumn("Submitted Time"))
            t.Columns.Add(New DataColumn("DiversionStatus"))
            t.Columns.Add(New DataColumn("Plateno"))
            t.Columns.Add(New DataColumn("source_supply"))
            t.Columns.Add(New DataColumn("Ship To"))

            Dim statuscond As String = ""
            Dim condition As String = ""
            
            ' Validate dropdown selections
            If SecurityHelper.ValidateInput(ddlstatus.SelectedValue, "numeric") AndAlso ddlstatus.SelectedValue <> "-1" Then
                statuscond = " WHERE UploadStatus = @statusValue"
            End If
            
            If ddlSource.SelectedValue <> "ALL SOURCES" AndAlso SecurityHelper.ValidateInput(ddlSource.SelectedValue, "alphanumeric") Then
                condition = condition & " AND source_supply IN (" & SecurityHelper.SanitizeForSql(ddlSource.SelectedValue) & ")"
            End If

            If ddlTransport.SelectedValue <> "ALL TRANSPORTERS" AndAlso SecurityHelper.ValidateInput(ddlTransport.SelectedValue, "alphanumeric") Then
                condition = condition & " AND transporter = @transporter"
            End If

            If ddlGeofence.SelectedValue <> "ALL GEOFENCES" AndAlso SecurityHelper.ValidateInput(ddlGeofence.SelectedValue, "alphanumeric") Then
                condition = condition & " AND destination_siteid = @geofence"
            End If

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                Dim sqlQuery As String = "SELECT m.destination_sitename, m.source_supply, m.plateno, E.DivStatus, M.transporter, M.Dn_No, E.Patch_No, M.Weight_outtime, E.SubmittedBy, E.QrCode, E.SubmittedDateTime, E.Lat, E.Lon, E.UploadStatus FROM (SELECT * FROM oss_patch_out WHERE Weight_outtime BETWEEN @beginDate AND @endDate " & condition & ") AS M LEFT OUTER JOIN (SELECT * FROM OSS_EXTENSION_TABLE " & statuscond & ") AS E ON M.patch_no = E.Patch_no"
                
                If ddlstatus.SelectedValue <> "-1" Then
                    sqlQuery = "SELECT m.destination_sitename, m.source_supply, m.plateno, E.DivStatus, M.transporter, M.Dn_No, E.Patch_No, M.Weight_outtime, E.SubmittedBy, E.QrCode, E.SubmittedDateTime, E.Lat, E.Lon, E.UploadStatus FROM (SELECT * FROM OSS_EXTENSION_TABLE " & statuscond & ") AS E INNER JOIN (SELECT * FROM oss_patch_out WHERE Weight_outtime BETWEEN @beginDate AND @endDate " & condition & ") AS M ON M.patch_no = E.Patch_no"
                End If
                
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(sqlQuery, conn)
                    cmd.Parameters.AddWithValue("@beginDate", begindatetime)
                    cmd.Parameters.AddWithValue("@endDate", enddatetime)
                    
                    If ddlstatus.SelectedValue <> "-1" Then
                        cmd.Parameters.AddWithValue("@statusValue", ddlstatus.SelectedValue)
                    End If
                    
                    If ddlTransport.SelectedValue <> "ALL TRANSPORTERS" Then
                        cmd.Parameters.AddWithValue("@transporter", ddlTransport.SelectedValue)
                    End If
                    
                    If ddlGeofence.SelectedValue <> "ALL GEOFENCES" Then
                        cmd.Parameters.AddWithValue("@geofence", ddlGeofence.SelectedValue)
                    End If
                    
                    Using da As New SqlDataAdapter(cmd)
                        Dim ds As New DataSet
                        da.Fill(ds)
                        
                        ProcessDataSet(ds, t)
                    End Using
                End Using
            End Using

            Session.Remove("exceltable")
            Session("exceltable") = t
            BuildHtmlTable(t)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("DISPLAY_LOG_ERROR", "Error in DisplayLogInformation: " & ex.Message)
        End Try
    End Sub
    
    Private Sub ProcessDataSet(ds As DataSet, t As DataTable)
        Try
            Dim i As Int64 = 1
            Dim r As DataRow
            
            If ds.Tables(0).Rows.Count > 0 Then
                For ii As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    r = t.NewRow
                    r(0) = SecurityHelper.SanitizeForHtml(ds.Tables(0).Rows(ii)("Patch_No").ToString())
                    r(1) = SecurityHelper.SanitizeForHtml(ds.Tables(0).Rows(ii)("SubmittedBy").ToString())
                    r(2) = Convert.ToDateTime(ds.Tables(0).Rows(ii)("Weight_outtime")).ToString("yyyy/MM/dd")
                    r(3) = SecurityHelper.SanitizeForHtml(ds.Tables(0).Rows(ii)("Dn_No").ToString().ToUpper())
                    
                    If IsDBNull(ds.Tables(0).Rows(ii)("QrCode")) Then
                        r(4) = "--"
                    Else
                        r(4) = SecurityHelper.SanitizeForHtml(ds.Tables(0).Rows(ii)("QrCode").ToString().ToUpper())
                    End If

                    If IsDBNull(ds.Tables(0).Rows(ii)("SubmittedDateTime")) Then
                        r(5) = ""
                    Else
                        r(5) = Convert.ToDateTime(ds.Tables(0).Rows(ii)("SubmittedDateTime")).ToString("yyyy/MM/dd")
                    End If
                    
                    ProcessStatusColumn(ds.Tables(0).Rows(ii), r)
                    
                    r(8) = SecurityHelper.SanitizeForHtml(ds.Tables(0).Rows(ii)("transporter").ToString())
                    r(9) = Convert.ToDateTime(ds.Tables(0).Rows(ii)("Weight_outtime")).ToString("HH:mm:ss")

                    If IsDBNull(ds.Tables(0).Rows(ii)("SubmittedDateTime")) Then
                        r(10) = ""
                    Else
                        r(10) = Convert.ToDateTime(ds.Tables(0).Rows(ii)("SubmittedDateTime")).ToString("HH:mm:ss")
                    End If

                    If IsDBNull(ds.Tables(0).Rows(ii)("DivStatus")) Then
                        r(11) = "No"
                    Else
                        r(11) = If(ds.Tables(0).Rows(ii)("DivStatus"), "Yes", "No")
                    End If

                    r(12) = SecurityHelper.SanitizeForHtml(ds.Tables(0).Rows(ii)("plateno").ToString())
                    r(13) = SecurityHelper.SanitizeForHtml(ds.Tables(0).Rows(ii)("source_supply").ToString())
                    r(14) = SecurityHelper.SanitizeForHtml(ds.Tables(0).Rows(ii)("destination_sitename").ToString())

                    t.Rows.Add(r)
                    i = i + 1
                Next
            Else
                AddEmptyRow(t)
            End If

            If t.Rows.Count = 0 Then
                AddEmptyRow(t)
            End If
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("PROCESS_DATASET_ERROR", "Error processing dataset: " & ex.Message)
        End Try
    End Sub
    
    Private Sub ProcessStatusColumn(row As DataRow, r As DataRow)
        Dim UploadStatus As Int32 = 0
        
        If IsDBNull(row("UploadStatus")) Then
            r(6) = "Pending"
            UploadStatus = 0
        Else
            UploadStatus = Convert.ToInt32(row("UploadStatus").ToString())
            
            If IsDBNull(row("lat")) Then
                Select Case UploadStatus
                    Case 0 : r(6) = "Pending"
                    Case 1 : r(6) = "Submitted"
                    Case 2 : r(6) = "Posted"
                    Case 3 : r(6) = "Rejected"
                    Case Else : r(6) = "Pending"
                End Select
            Else
                Dim lat As String = SecurityHelper.SanitizeForHtml(row("lat").ToString())
                Dim lon As String = SecurityHelper.SanitizeForHtml(row("lon").ToString())
                Dim mapUrl As String = "http://maps.google.com/maps?f=q&hl=en&q=" & lat & "+" & lon & "&om=1&t=k"
                
                Select Case UploadStatus
                    Case 0 : r(6) = "<a href='" & mapUrl & "' target='_blank'>In Progress</a>"
                    Case 1 : r(6) = "<a href='" & mapUrl & "' target='_blank'>Submitted</a>"
                    Case 2 : r(6) = "<a href='" & mapUrl & "' target='_blank'>Posted</a>"
                    Case 3 : r(6) = "<a href='" & mapUrl & "' target='_blank'>Rejected</a>"
                    Case Else : r(6) = "<a href='" & mapUrl & "' target='_blank'>In Progress</a>"
                End Select
            End If
        End If
        
        r(7) = UploadStatus
    End Sub
    
    Private Sub AddEmptyRow(t As DataTable)
        Dim r As DataRow = t.NewRow
        For i As Integer = 0 To 14
            r(i) = "--"
        Next
        t.Rows.Add(r)
    End Sub
    
    Private Sub BuildHtmlTable(t As DataTable)
        sb1.Length = 0

        If t.Rows.Count > 0 Then
            ec = "true"

            sb1.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
            sb1.Append("<thead><tr align=""left""><th>DN No</th><th>Transporter</th><th>Plateno</th><th>Source</th><th>Ship To</th><th>Driver Ic</th><th>Weight Out Time</th><th>Submitted DateTime</th><th>Diversion</th><th>Status</th><th>Update</th></tr></thead>")
            sb1.Append("<tbody>")
            
            Dim counter As Integer = 1
            For i As Integer = 0 To t.Rows.Count - 1
                sb1.Append("<tr>")
                
                Try
                    If Convert.ToInt32(t.DefaultView.Item(i)(7)) > 0 Then
                        sb1.Append("<td style='cursor:pointer;color:blue;' onclick='javascript:openMe(" & SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(0).ToString()) & ")'>")
                    Else
                        sb1.Append("<td>")
                    End If
                Catch ex As Exception
                    sb1.Append("<td>")
                End Try

                sb1.Append(SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(3).ToString()))
                sb1.Append("</td><td>")
                sb1.Append(SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(8).ToString()))
                sb1.Append("</td><td>")
                sb1.Append(SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(12).ToString()))
                sb1.Append("</td><td>")
                sb1.Append(SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(13).ToString()))
                sb1.Append("</td><td>")
                sb1.Append(SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(14).ToString()))
                sb1.Append("</td><td>")
                sb1.Append(SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(1).ToString()))
                sb1.Append("</td><td>")
                sb1.Append(SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(2).ToString()) & " " & SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(9).ToString()))
                sb1.Append("</td><td>")
                sb1.Append(SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(5).ToString()) & " " & SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(10).ToString()))
                sb1.Append("</td><td>")
                sb1.Append(SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(11).ToString()))
                sb1.Append("</td><td>")
                sb1.Append(t.DefaultView.Item(i)(6).ToString()) ' Status column may contain HTML links
                sb1.Append("</td>")
                sb1.Append("<td style='cursor:pointer;color:blue;'><a href='WebPod.aspx?dn=" & SecurityHelper.SanitizeForHtml(t.DefaultView.Item(i)(3).ToString()))
                sb1.Append("' target='mainframe' >Update</a></td></tr>")
                counter += 1
            Next

            sb1.Append("</tbody>")
            sb1.Append("<tfoot><tr align=""left""><th>DN No</th><th>Transporter</th><th>Plateno</th><th>Source</th><th>Ship To</th><th>Driver Ic</th><th>Weight Out Time</th><th>Submitted DateTime</th><th>Diversion</th><th>Status</th><th>Update</th></tr></tfoot>")
            sb1.Append("</table>")
        End If
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub
End Class