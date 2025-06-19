Imports System.Data.SqlClient
Imports System.Data
Imports System.Collections.Generic

Partial Class ClientFeedBackReport
    Inherits System.Web.UI.Page
    Public show As Boolean = False

    Public divgrid As Boolean = False
    Public ec As String = "false"
    Public sb1 As New StringBuilder()
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
        Catch ex As Exception
        End Try
        MyBase.OnInit(e)
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try

            If Page.IsPostBack = False Then

                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")

                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")

                Dim cmd As SqlCommand
                Dim dr As SqlDataReader

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                cmd = New SqlCommand("select CUserName ,CUserId  from EC_client_user where status =1  order by CUserName", conn)

                conn.Open()
                dr = cmd.ExecuteReader()
                ddlUsername.Items.Clear()
                ddlUsername.Items.Add(New ListItem("ALL", "0"))
                While dr.Read()
                    ddlUsername.Items.Add(New ListItem(dr("CUserName"), dr("CUserId")))
                End While
                dr.Close()

                conn.Close()

                FillGrid()
            End If



        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub
    Public Sub FillGrid()
        Dim cmd As SqlCommand
        Try

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim userstable As New DataTable
            Dim ok As String = "no"
            Dim condition As String = ""

            Dim strBeginDateTime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim strEndDateTime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            If Not ddlUsername.SelectedValue = "0" Then
                condition = " and t1.tuserid  ='" & ddlUsername.SelectedValue & "'"
            Else
                condition = ""
            End If

            Dim r As DataRow
            userstable.Rows.Clear()

            userstable.Columns.Add(New DataColumn("S No"))
            userstable.Columns.Add(New DataColumn("Plateno"))
            userstable.Columns.Add(New DataColumn("DN No"))
            userstable.Columns.Add(New DataColumn("Company Name"))
            userstable.Columns.Add(New DataColumn("Username"))
            userstable.Columns.Add(New DataColumn("Source Supply"))
            userstable.Columns.Add(New DataColumn("CSD"))
            userstable.Columns.Add(New DataColumn("CQ"))
            userstable.Columns.Add(New DataColumn("DT"))
            userstable.Columns.Add(New DataColumn("DB"))
            userstable.Columns.Add(New DataColumn("Remarks"))
            userstable.Columns.Add(New DataColumn("timestamp"))





            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            If Not userid = "--Select User Name--" Then

                cmd = New SqlCommand("SELECT Plateno,isnull (dnno,'-') dnno,isnull(t3.CompanyName,'-') CompanyName ,t2.CUserName  ,SourceSupply,CSD,CQ,DT,DB,Remarks,t1.timestamp FROM Feedback t1 left outer join EC_client_user t2 on t1.tuserid = t2.CUserId  left outer join EC_company t3 on t1.companyId = t3.CompanyId   where t1.timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' " & condition & " order by timestamp ", conn)
                conn.Open()
                ' Response.Write(cmd.CommandText)
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                Dim i As Int32 = 1
                While dr.Read()
                    r = userstable.NewRow
                    r(0) = i.ToString()
                    r(1) = dr("Plateno")
                    r(2) = dr("dnno")
                    r(3) = dr("CompanyName")
                    r(4) = dr("CUserName")
                    r(5) = dr("SourceSupply")
                    Select Case dr("CSD")
                        Case "-1"
                            r(6) = "NA"
                        Case "0"
                            r(6) = "<span style='color:red'>Negative</span>"
                        Case "1"
                            r(6) = "<span style='color:Blue'>Neutral</span>"
                        Case "2"
                            r(6) = "<span style='color:Green'>Positive</span>"
                    End Select

                    Select Case dr("CQ")
                        Case "-1"
                            r(7) = "NA"
                        Case "0"
                            r(7) = "<span style='color:red'>Negative</span>"
                        Case "1"
                            r(7) = "<span style='color:Blue'>Neutral</span>"
                        Case "2"
                            r(7) = "<span style='color:Green'>Positive</span>"
                    End Select

                    Select Case dr("DT")
                        Case "-1"
                            r(8) = "NA"
                        Case "0"
                            r(8) = "<span style='color:red'>Negative</span>"
                        Case "1"
                            r(8) = "<span style='color:Blue'>Neutral</span>"
                        Case "2"
                            r(8) = "<span style='color:Green'>Positive</span>"
                    End Select

                    Select Case dr("DB")
                        Case "-1"
                            r(9) = "NA"
                        Case "0"
                            r(9) = "<span style='color:red'>Negative</span>"
                        Case "1"
                            r(9) = "<span style='color:Blue'>Neutral</span>"
                        Case "2"
                            r(9) = "<span style='color:green'>Positive</span>"
                    End Select


                    r(10) = dr("Remarks")
                    r(11) = Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd HH:mm:ss")


                    userstable.Rows.Add(r)
                    i = i + 1
                    ok = "yes"
                End While

                conn.Close()
            End If
            If ok = "no" Then
                r = userstable.NewRow
                r(0) = "-"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                r(5) = "-"
                r(6) = "-"
                r(7) = "-"
                r(8) = "-"
                r(9) = "-"
                r(10) = "-"
                r(11) = "-"


                userstable.Rows.Add(r)

            End If

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session("exceltable") = userstable
            ec = "true"
            sb1.Length = 0
            sb1.Append("<thead><tr><th style=""width:35px;"">S No</th><th>Plateno</th><th style=""width:150px;"">DN No</th><th >Company Name</th><th>User Name</th><th>Source Supply</th><th>Transparency Of Status</th><th>Quality</th><th>Delivery Time</th><th>Driver Behaviour</th><th>Remarks</th><th style=""width:220px;"">Timestamp</th></tr></thead>")
            sb1.Append("<tbody>")
            Dim counter As Integer = 1
            For i As Integer = 0 To userstable.Rows.Count - 1

                sb1.Append("<td>")
                sb1.Append(userstable.DefaultView.Item(i)(0))
                sb1.Append("</td>")
                sb1.Append("<td>")
                sb1.Append(userstable.DefaultView.Item(i)(1))
                sb1.Append("</td>")
                sb1.Append("<td style=""width:150px;"">")
                sb1.Append(userstable.DefaultView.Item(i)(2))
                sb1.Append("</td>")
                sb1.Append("<td style=""white-space:nowrap;"">")
                sb1.Append(userstable.DefaultView.Item(i)(3))
                sb1.Append("</td>")
                sb1.Append("<td style=""white-space:nowrap;"">")
                sb1.Append(userstable.DefaultView.Item(i)(4))
                sb1.Append("</td>")
                sb1.Append("<td>")
                sb1.Append(userstable.DefaultView.Item(i)(5))
                sb1.Append("</td>")
                sb1.Append("<td>")
                sb1.Append(userstable.DefaultView.Item(i)(6))
                sb1.Append("</td>")
                sb1.Append("<td>")
                sb1.Append(userstable.DefaultView.Item(i)(7))
                sb1.Append("</td>")
                sb1.Append("<td>")
                sb1.Append(userstable.DefaultView.Item(i)(8))
                sb1.Append("</td>")
                sb1.Append("<td>")
                sb1.Append(userstable.DefaultView.Item(i)(9))
                sb1.Append("</td>")
                sb1.Append("<td>")
                sb1.Append(userstable.DefaultView.Item(i)(10))
                sb1.Append("</td>")
                sb1.Append("<td style=""white-space:nowrap;"">")
                sb1.Append(userstable.DefaultView.Item(i)(11))
                sb1.Append("</td>")
                sb1.Append("</tr>")
                counter += 1
            Next
            sb1.Append("</tbody>")
            sb1.Append("<tfoot><tr><th style=""width:35px;"">S No</th><th>Plateno</th><th style=""width:150px;"">DN No</th><th  >Company Name</th><th>User Name</th><th>Source Supply</th><th>Transparency Of Status</th><th>Quality</th><th>Delivery Time</th><th>Driver Behaviour</th><th>Remarks</th><th style=""width:220px;"">Timestamp</th></tr></tfoot>")

        Catch ex As Exception

            Response.Write(ex.Message)
            Response.Write(cmd.CommandText)
        End Try
    End Sub



    'Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton.Click
    '    FillGrid()
    'End Sub



    Protected Sub ImageButton1_Click(sender As Object, e As EventArgs)
        FillGrid()
    End Sub
End Class
