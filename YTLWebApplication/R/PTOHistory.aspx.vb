Imports AspMap
Imports System.Data
Imports System.Data.SqlClient

Partial Class PTOHistory
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public show As Boolean = False
    Public sb1 As New StringBuilder()
    Dim map As AspMap.Map


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            ' If Not Page.IsPostBack Then

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim cmd As SqlCommand
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))


            cmd = New SqlCommand("select * from userTBL where role='User' order by username", conn)

            If role = "User" Then
                cmd = New SqlCommand("select * from userTBL where userid='" & userid & "' order by username", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select * from userTBL where role='User' and userid in (" & userslist & ") order by username", conn)
                DropDownList1.Items.Add(New ListItem("--ALL USERS--", "--ALL USERS--"))
            Else
                DropDownList1.Items.Add(New ListItem("--ALL USERS--", "--ALL USERS--"))
            End If

            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            While (dr.Read())
                DropDownList1.Items.Add((New ListItem(dr("username").ToString().ToUpper(), dr("userid").ToString())))
            End While
            DropDownList1.SelectedValue = Request.Form("DropDownList1")

            ddlplate.Items.Add(New ListItem("--ALL PLATES--", "--ALL PLATES--"))

            If Request.Form("DropDownList1") <> Nothing Then
                ddlplate.Items.Clear()
                cmd = New SqlCommand("select * from vehicleTBL where userid='" & Request.Form("DropDownList1") & "' order by plateno", conn)
            Else
                If role = "User" Then
                    cmd = New SqlCommand("select * from vehicleTBL where userid='" & userid & "' order by plateno", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select * from vehicleTBL where userid in (" & userslist & ") order by plateno", conn)
                Else
                    cmd = New SqlCommand("select * from vehicleTBL order by plateno", conn)
                End If
            End If

            
            dr = cmd.ExecuteReader()

            While (dr.Read())
                ddlplate.Items.Add((New ListItem(dr("plateno").ToString().ToUpper(), dr("plateno").ToString())))
            End While

            ddlplate.SelectedValue = Request.Form("ddlplate")
            conn.Close()


            ' End If
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
            If Not Request.QueryString("plateno") Is Nothing Then
                DropDownList1.SelectedValue = ""
            End If

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                Dim userid As String = Request.QueryString("u")
                Dim plateno As String = Request.QueryString("p")
                If (plateno <> "") Then
                    Dim cmd As SqlCommand
                    Dim dr As SqlDataReader

                    If userid.IndexOf(",") > 0 Then
                        userid = userid.Substring(0, userid.IndexOf(","))
                    End If

                    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                    cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & userid & "' order by plateno", conn)
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    While dr.Read()
                        ddlplate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                    End While
                    dr.Close()

                    conn.Close()
                    DropDownList1.SelectedValue = userid
                    ddlplate.SelectedValue = plateno
                    DisplayLogInformation()

                End If
            Else
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                DropDownList1.SelectedValue = Request.Form("DropDownList1")
                Dim dr As SqlDataReader
                Dim cmd As SqlCommand
                ddlplate.Items.Clear()
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                ddlplate.Items.Add(New ListItem("--ALL PLATES--", "--ALL PLATES--"))
                If DropDownList1.SelectedValue <> "--SELECT USER NAME--" Then
                    cmd = New SqlCommand("select * from vehicleTBL where userid='" & Request.Form("DropDownList1") & "' order by plateno", conn)
                    If Request.Form("DropDownList1") = "--ALL USERS--" Then
                        If role = "SuperUser" Or role = "Operator" Then
                            cmd = New SqlCommand("select * from vehicleTBL where userid in (" & userslist & ") order by plateno", conn)
                        Else
                            cmd = New SqlCommand("select * from vehicleTBL order by plateno", conn)
                        End If
                    End If
                    Try
                        If conn.State <> ConnectionState.Open Then
                            conn.Open()
                        End If
                        dr = cmd.ExecuteReader()
                        While (dr.Read())
                            ddlplate.Items.Add((New ListItem(dr("plateno").ToString(), dr("plateno").ToString())))
                        End While
                        ddlplate.SelectedValue = Request.Form("ddlplate")
                    Catch ex As Exception
                    Finally
                        conn.Close()
                    End Try
                End If
                End If

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub DisplayLogInformation()
        Try
            map = New AspMap.Map()
            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            Dim userid As String = DropDownList1.SelectedValue
            Dim plateno As String = ddlplate.SelectedValue
            Dim uid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")


            Dim locObj As New Location(userid)

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Username"))
            t.Columns.Add(New DataColumn("GroupName"))
            t.Columns.Add(New DataColumn("Geofence Name/Location"))
            t.Columns.Add(New DataColumn("Nearest Town"))
            t.Columns.Add(New DataColumn("Mile Point"))
            t.Columns.Add(New DataColumn("Lat"))
            t.Columns.Add(New DataColumn("Lon"))
            t.Columns.Add(New DataColumn("From Date Time"))
            t.Columns.Add(New DataColumn("To Date Time"))
            t.Columns.Add(New DataColumn("Duration"))

            Dim query As String = ""
            Dim condition As String = ""

            query = "select p.plateno,p.from_timestamp,p.to_timestamp,p.lat,p.lon,p.userid,u.username,v.groupname from pto_history p Join userTBL u on u.userid=p.userid Join vehicleTBL v On v.plateno=p.plateno and p.from_timestamp between '" & begindatetime & "' and '" & enddatetime & "' and v.pto='1'"
            If DropDownList1.SelectedValue <> "--ALL USERS--" Then
                If ddlplate.SelectedValue = "--ALL PLATES--" Then
                    query = "select p.plateno,p.from_timestamp,p.to_timestamp,p.lat,p.lon,p.userid,u.username,v.groupname from pto_history p Join userTBL u on u.userid=p.userid Join vehicleTBL v On v.plateno=p.plateno and p.userid= '" & DropDownList1.SelectedValue & "'  and p.from_timestamp between '" & begindatetime & "' and '" & enddatetime & "' and v.pto='1'"
                Else
                    query = "select p.plateno,p.from_timestamp,p.to_timestamp,p.lat,p.lon,p.userid,u.username,v.groupname from pto_history p Join userTBL u on u.userid=p.userid Join vehicleTBL v On v.plateno=p.plateno and p.plateno='" & ddlplate.SelectedValue & "' and p.from_timestamp between '" & begindatetime & "' and '" & enddatetime & "' and v.pto='1'"
                End If
            Else
                If ddlplate.SelectedValue = "--ALL PLATES--" Then
                    If role = "SuperUser" Or role = "Operator" Then
                        query = "select p.plateno,p.from_timestamp,p.to_timestamp,p.lat,p.lon,p.userid,u.username,v.groupname from pto_history p Join userTBL u on u.userid=p.userid Join vehicleTBL v On v.plateno=p.plateno and p.userid in (" & userslist & ")  and p.from_timestamp between '" & begindatetime & "' and '" & enddatetime & "' and v.pto='1'"
                    ElseIf role = "Admin" Then
                        query = "select p.plateno,p.from_timestamp,p.to_timestamp,p.lat,p.lon,p.userid,u.username,v.groupname from pto_history p Join userTBL u on u.userid=p.userid Join vehicleTBL v On v.plateno=p.plateno and p.from_timestamp between '" & begindatetime & "' and '" & enddatetime & "' and v.pto='1'"
                    End If
                Else
                    query = "select p.plateno,p.from_timestamp,p.to_timestamp,p.lat,p.lon,p.userid,u.username,v.groupname from pto_history p Join userTBL u on u.userid=p.userid Join vehicleTBL v On v.plateno=p.plateno and p.plateno='" & ddlplate.SelectedValue & "' and p.from_timestamp between '" & begindatetime & "' and '" & enddatetime & "' and v.pto='1'"
                End If
            End If

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand(query, conn)
            ' Response.Write(cmd.CommandText)
            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim r As DataRow
                Dim i As Int64 = 1

                While dr.Read
                    r = t.NewRow
                    r(0) = i.ToString()
                    Dim rs As New Recordset

                    Dim location As String = locObj.GetLocation(Double.Parse(dr("lat")), Double.Parse(dr("lon")))

                    r(1) = dr("plateno")
                    r(2) = dr("username").ToString.ToUpper
                    r(3) = dr("groupname").ToString.ToUpper
                    r(4) = location
                    r(5) = locObj.GetNearestTown(Double.Parse(dr("lat")), Double.Parse(dr("lon")))
                    r(6) = locObj.GetMilePoint(Double.Parse(dr("lat")), Double.Parse(dr("lon")))
                    r(7) = Double.Parse(dr("lat")).ToString("0.0000")
                    r(8) = Double.Parse(dr("lon")).ToString("0.0000")
                    Dim intime As DateTime = Convert.ToDateTime(dr("from_timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                    r(9) = Convert.ToDateTime(dr("from_timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                    If IsDBNull(dr("to_timestamp")) Then
                        r(10) = "--"
                        r(11) = "--"
                    Else
                        Dim outtime As DateTime = Convert.ToDateTime(dr("to_timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                        r(10) = Convert.ToDateTime(dr("to_timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                        r(11) = "<span style=""color:blue;cursor:pointer;"" onclick=""javascript:DisplayMap('" & intime.ToString("yyyy/MM/dd HH:mm:ss") & "','" & outtime.AddSeconds(-1).ToString("yyyy/MM/dd HH:mm:ss") & "','" & dr("plateno") & "')"">" & (outtime - intime).TotalMinutes.ToString("0") & "</span>"
                    End If



                    t.Rows.Add(r)
                    i = i + 1
                End While

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
                    r(11) = "--"
                    t.Rows.Add(r)
                End If

            Catch ex As Exception
                Response.Write(ex.Message)
            Finally
                conn.Close()
            End Try
            Session.Remove("exceltable")
            Session("exceltable") = t
            sb1.Length = 0


            If (t.Rows.Count > 0) Then
                ec = "true"

                sb1.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")

                sb1.Append("<thead><tr align=""left""><th>S No</th><th>Plate No</th><th>Username</th><th>Group Name</th><th>Geofence Name/Location</th><th style='Width:140px;'>Nearest Town</th><th>Mile Point</th><th>Lat</th><th>Lon</th><th>From Date Time</th><th>To Date Time</th><th style='Width:60px;'>Duration (Mins)</th></tr></thead>")

                sb1.Append("<tbody>")
                Dim counter As Integer = 1
                For i As Integer = 0 To t.Rows.Count - 1
                    Try
                        sb1.Append("<tr>")
                        sb1.Append("<td>")
                        sb1.Append(t.DefaultView.Item(i)(0))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(1))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(2))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(3))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(4))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(5))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(6))
                        sb1.Append("</td><td align=""right"">")
                        sb1.Append(t.DefaultView.Item(i)(7))
                        sb1.Append("</td><td align=""right"">")
                        sb1.Append(t.DefaultView.Item(i)(8))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(9))
                        sb1.Append("</td><td >")
                        sb1.Append(t.DefaultView.Item(i)(10))
                        sb1.Append("</td><td align=""right"">")
                        sb1.Append(t.DefaultView.Item(i)(11))
                        sb1.Append("</td></tr>")
                        counter += 1
                    Catch ex As Exception

                    End Try
                Next
                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr align=""left""><th>S No</th><th>Plate No</th><th>Username</th><th>Group Name</th><th>Geofence Name/Location</th><th>Nearest Town</th><th>Mile Point</th><th>Lat</th><th>Lon</th><th>From Date Time</th><th>To Date Time</th><th>Duration (Mins)</th></tr></tfoot>")
                ' sb1.Append("<tfoot><tr align=""left""><th>S No</th><th>Plate No</th><th>Username</th><th>Group Name</th><th>Geofence Name/Location</th><th>From Date Time</th><th>To Date Time</th><th>Lat</th><th>Lon</th><th>Nearest Town</th><th>Mile Point</th><th>Duration (Mins)</th></tr></tfoot>")
            Else

            End If

        Catch ex As Exception

        End Try

    End Sub


    Protected Sub DropDownList1_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles DropDownList1.SelectedIndexChanged


    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub


End Class
