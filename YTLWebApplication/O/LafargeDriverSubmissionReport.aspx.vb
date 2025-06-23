Imports AspMap
Imports System.Data
Imports System.Data.SqlClient

Partial Class LafargeDriverSubmissionReport
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public show As Boolean = False
    Public sb1 As New StringBuilder()
    Public adminusers As String
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
        Catch ex As Exception

        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")

            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim ds As New DataSet
            Dim da As SqlDataAdapter
            ddlGeofence.Items.Add(New ListItem("ALL SHIP-TO-CODE", "ALL GEOFENCES"))


            da = New SqlDataAdapter("select distinct(shiptocode),geofencename from geofence where accesstype='1' and shiptocode <> '0' order by geofencename,shiptocode", conn2)
            da.Fill(ds)
            Dim count As Integer = 0
            For count = 0 To ds.Tables(0).Rows.Count - 1
                ddlGeofence.Items.Add(New ListItem(ds.Tables(0).Rows(count)("geofencename").ToString().ToUpper() & "[ " & ds.Tables(0).Rows(count)("shiptocode").ToString().ToUpper() & " ]", ds.Tables(0).Rows(count)("shiptocode")))
            Next



            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                FillTransporters()
            Else
                ddlGeofence.SelectedValue = Request.Form("ddlGeofence")
            End If
        Catch ex As Exception

        End Try
    End Sub
    Sub FillTransporters()
        Try


            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim tankerQuery As String = "select plateno from vehicleTBL"

            Dim con As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            If role = "User" Then
                tankerQuery = "select plateno from vehicleTBL where userid='" & userid & "'"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                tankerQuery = "select plateno from vehicleTBL where userid in (" & userslist & ")"
            End If
            Dim ds As New DataSet
            Dim da As New SqlDataAdapter(tankerQuery, con)
            da.Fill(ds)
            Dim platecond As String = ""
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If Not IsDBNull(ds.Tables(0).Rows(i)("plateno")) Then
                    platecond = platecond & "'" & ds.Tables(0).Rows(i)("plateno") & "',"
                End If
            Next


            If platecond.Length > 3 Then
                platecond = platecond.Substring(0, platecond.Length - 1)
                platecond = " where weight_outtime between '" & DateTime.Now.AddDays(-180).ToString("yyyy/MM/dd HH:mm:ss") & "' and '" & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "' and plateno In (" & platecond & ")  "
            End If


            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmd As New SqlCommand("Select  distinct transporter from oss_patch_out " & platecond & " order by transporter ", conn)

            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            ddlTransport.Items.Add("ALL TRANSPORTERS")

            While (dr.Read())
                Try
                    If IsDBNull(dr("transporter")) = False Then
                        ddlTransport.Items.Add(New ListItem(dr("transporter"), dr("transporter")))
                    End If

                Catch ex As Exception

                End Try
            End While

            conn.Close()

        Catch ex As Exception

        End Try
    End Sub
    Protected Sub DisplayLogInformation()
        Try

            Dim begindatetime As String = txtBeginDate.Value & " 00:00:00"
            Dim enddatetime As String = txtEndDate.Value & " 23:59:59"
            Dim uid As String = Request.Cookies("userinfo")("userid")
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim users() As String
            users = userslist.Split(",")
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
            Dim statuscond As String = ""
            Dim condition As String = ""


            Dim con As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim tankerQuery As String = "select plateno from vehicleTBL"
            If role = "User" Then
                tankerQuery = "select plateno from vehicleTBL where userid='" & userid & "'"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                tankerQuery = "select plateno from vehicleTBL where userid in (" & userslist & ")"
            End If
            Dim dss As New DataSet
            Dim daa As New SqlDataAdapter(tankerQuery, con)
            daa.Fill(dss)
            Dim platecond As String = ""
            For i As Integer = 0 To dss.Tables(0).Rows.Count - 1
                If Not IsDBNull(dss.Tables(0).Rows(i)("plateno")) Then
                    platecond = platecond & "'" & dss.Tables(0).Rows(i)("plateno") & "',"
                End If
            Next


            If platecond.Length > 3 Then
                platecond = platecond.Substring(0, platecond.Length - 1)
                platecond = " and plateno in (" & platecond & ")  "
            End If


            If ddlstatus.SelectedValue <> "-1" Then
                statuscond = " where UploadStatus='" & ddlstatus.SelectedValue & "' "
            End If
            If (ddlSource.SelectedValue <> "ALL SOURCES") Then
                condition = condition & " and source_supply in(" & ddlSource.SelectedValue & ") "
            End If

            If (ddlTransport.SelectedValue <> "ALL TRANSPORTERS") Then
                condition = condition & " and transporter='" & ddlTransport.SelectedValue & "'"
            End If

            If (ddlGeofence.SelectedValue <> "ALL GEOFENCES") Then
                condition = condition & " and destination_siteid='" & ddlGeofence.SelectedValue & "'"
            End If

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim sqlQuery As String = "select M.transporter as Destination_SiteName,M.Dn_No,E.Patch_No,M.Weight_outtime,E.SubmittedBy as driver_ic,E.QrCode,E.SubmittedDateTime,E.Lat,E.Lon,E.UploadStatus from (select * from oss_patch_out where Weight_outtime  between @from and @to  " & platecond & " " & condition & ") As M left outer join (Select * from OSS_EXTENSION_TABLE  " & statuscond & ") As E   On M.patch_no=E.Patch_no"
            If ddlstatus.SelectedValue <> "-1" Then
                sqlQuery = "Select M.transporter As Destination_SiteName, M.Dn_No,E.Patch_No,M.Weight_outtime,E.SubmittedBy As driver_ic,E.QrCode,E.SubmittedDateTime,E.Lat,E.Lon,E.UploadStatus from (Select * from OSS_EXTENSION_TABLE  " & statuscond & ") As E inner join  (Select * from oss_patch_out where Weight_outtime  between @from And @To  " & platecond & " " & condition & ") As M   On M.patch_no=E.Patch_no"
            End If
            Dim cmd As SqlCommand = New SqlCommand(sqlQuery, conn)
            cmd.Parameters.AddWithValue("@from", begindatetime)
            cmd.Parameters.AddWithValue("@To", enddatetime)
            'Response.Write(cmd.CommandText)
            Dim da As New SqlDataAdapter(cmd)
            Dim ds As New DataSet
            Dim UploadStatus As Int32 = 0
            da.Fill(ds)
            Try
                Dim i As Int64 = 1
                Dim r As DataRow
                If ds.Tables(0).Rows.Count > 0 Then
                    For ii As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        r = t.NewRow
                        r(0) = ds.Tables(0).Rows(ii)("Patch_No")
                        r(1) = ds.Tables(0).Rows(ii)("driver_ic")

                        r(2) = Convert.ToDateTime(ds.Tables(0).Rows(ii)("Weight_outtime")).ToString("yyyy/MM/dd")
                        r(3) = ds.Tables(0).Rows(ii)("Dn_No").ToString.ToUpper
                        If IsDBNull(ds.Tables(0).Rows(ii)("QrCode")) Then
                            r(4) = "--"
                        Else
                            r(4) = ds.Tables(0).Rows(ii)("QrCode").ToString().ToUpper()
                        End If

                        If IsDBNull(ds.Tables(0).Rows(ii)("SubmittedDateTime")) Then
                            r(5) = ""
                        Else
                            r(5) = Convert.ToDateTime(ds.Tables(0).Rows(ii)("SubmittedDateTime")).ToString("yyyy/MM/dd")
                        End If
                        If IsDBNull(ds.Tables(0).Rows(ii)("UploadStatus")) Then
                            r(6) = "Pending"
                            UploadStatus = 0
                        Else
                            UploadStatus = Convert.ToInt32(ds.Tables(0).Rows(ii)("UploadStatus").ToString())
                            If IsDBNull(ds.Tables(0).Rows(ii)("lat")) Then
                                Select Case UploadStatus
                                    Case 0
                                        r(6) = "Pending"
                                    Case 1
                                        r(6) = "Submitted"
                                    Case 2
                                        r(6) = "Posted"
                                    Case 3
                                        r(6) = "Rejected"
                                    Case Else
                                        r(6) = "Pending"
                                End Select
                            Else
                                Select Case UploadStatus
                                    Case 0
                                        r(6) = "<a href='http://maps.google.com/maps?f=q&amp;hl=en&amp;q=" & ds.Tables(0).Rows(ii)("lat") & "+" & ds.Tables(0).Rows(ii)("lon") & "&amp;om=1&amp;t=k' target='_blank'>In Progress</a>"
                                    Case 1
                                        r(6) = "<a href='http://maps.google.com/maps?f=q&amp;hl=en&amp;q=" & ds.Tables(0).Rows(ii)("lat") & "+" & ds.Tables(0).Rows(ii)("lon") & "&amp;om=1&amp;t=k' target='_blank'>Submitted</a>"
                                    Case 2
                                        r(6) = "<a href='http://maps.google.com/maps?f=q&amp;hl=en&amp;q=" & ds.Tables(0).Rows(ii)("lat") & "+" & ds.Tables(0).Rows(ii)("lon") & "&amp;om=1&amp;t=k' target='_blank'>Posted</a>"
                                    Case 3
                                        r(6) = "<a href='http://maps.google.com/maps?f=q&amp;hl=en&amp;q=" & ds.Tables(0).Rows(ii)("lat") & "+" & ds.Tables(0).Rows(ii)("lon") & "&amp;om=1&amp;t=k' target='_blank'>Rejected</a>"
                                    Case Else
                                        r(6) = "<a href='http://maps.google.com/maps?f=q&amp;hl=en&amp;q=" & ds.Tables(0).Rows(ii)("lat") & "+" & ds.Tables(0).Rows(ii)("lon") & "&amp;om=1&amp;t=k' target='_blank'>In Progress</a>"
                                End Select
                            End If

                        End If
                        r(7) = UploadStatus

                        r(8) = ds.Tables(0).Rows(ii)("Destination_SiteName")

                        r(9) = Convert.ToDateTime(ds.Tables(0).Rows(ii)("Weight_outtime")).ToString("HH:mm:ss")

                        If IsDBNull(ds.Tables(0).Rows(ii)("SubmittedDateTime")) Then
                            r(10) = ""
                        Else
                            r(10) = Convert.ToDateTime(ds.Tables(0).Rows(ii)("SubmittedDateTime")).ToString("HH:mm:ss")
                        End If

                        t.Rows.Add(r)
                        i = i + 1
                    Next
                Else
                    r = t.NewRow
                    r(0) = "--"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    r(6) = "--"
                    r(7) = "--"
                    r(8) = "--"
                    r(9) = "--"
                    r(10) = "--"
                    t.Rows.Add(r)
                End If


                If t.Rows.Count = 0 Then
                    r = t.NewRow
                    r(0) = "--"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    r(6) = "--"
                    r(7) = "--"
                    r(8) = "--"
                    r(9) = "--"
                    r(10) = "--"
                    t.Rows.Add(r)
                End If
                Session.Remove("exceltable")
                Session("exceltable") = t


            Catch ex As Exception
                Response.Write("<br/>S1 : " & ex.StackTrace)
            Finally
                conn.Close()
            End Try
            Session.Remove("exceltable")
            Session("exceltable") = t
            sb1.Length = 0


            If (t.Rows.Count > 0) Then
                ec = "true"

                sb1.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")

                sb1.Append("<thead><tr align=""left""><th>DN No</th><th>Transporter</th><th>Driver Ic</th><th>Weight Out Time</th><th>QR Code</th><th>Submitted DateTime</th><th>Status</th><th>Update</th></tr></thead>")

                sb1.Append("<tbody>")
                Dim counter As Integer = 1
                For i As Integer = 0 To t.Rows.Count - 1
                    Try
                        sb1.Append("<tr>")
                        Try
                            If Convert.ToInt32(t.DefaultView.Item(i)(7)) > 0 Then
                                sb1.Append("<td style='cursor:pointer;color:blue;' onclick='javascript:openMe(" & t.DefaultView.Item(i)(0) & ")'>")
                            Else
                                sb1.Append("<td>")
                            End If
                        Catch ex As Exception
                            sb1.Append("<td>")
                        End Try


                        sb1.Append(t.DefaultView.Item(i)(3))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(8))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(1))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(2) & " " & t.DefaultView.Item(i)(9))
                        'sb1.Append("</td><td>")
                        'sb1.Append(t.DefaultView.Item(i)(3))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(4))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(5) & " " & t.DefaultView.Item(i)(10))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(6))
                        sb1.Append("</td>")
                        sb1.Append("<td style='cursor:pointer;color:blue;'><a href='WebPod.aspx?dn=" & t.DefaultView.Item(i)(3))
                        sb1.Append("' target='mainframe' >Update</a></td></tr>")
                        counter += 1
                    Catch ex As Exception
                        Response.Write("<br/> InnerLoop : " & ex.StackTrace)
                    End Try
                Next

                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr align=""left""><th>DN No</th><th>Transporter</th><th>Driver Ic</th><th>Weight Out Time</th><th>QR Code</th><th>Submitted DateTime</th><th>Status</th><th>Update</th></tr></tfoot>")
            Else

            End If

        Catch ex As Exception
            Response.Write("<br/> Lafoot : " & ex.StackTrace)
        End Try

    End Sub




    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub
End Class
