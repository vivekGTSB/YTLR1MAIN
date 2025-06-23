Imports AspMap
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Script.Services

Partial Class PublicGeofenceSummaryReport
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public show As Boolean = False
    Public sb1 As New StringBuilder()
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim cmd As SqlCommand
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))



            cmd = New SqlCommand("select * from userTBL where role='User' order by username", conn)
            ' da = New SqlDataAdapter("select geofencename,geofenceid from geofence where accesstype='1' order by geofencename", conn2)
            'da = New SqlDataAdapter("select geofencename,geofenceid from geofence where accesstype='1' order by LTRIM(geofencename)", conn2)






            If role = "User" Then
                cmd = New SqlCommand("select * from userTBL where userid='" & userid & "' order by username", conn)

            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select * from userTBL where role='User' and userid in (" & userslist & ") order by username", conn)
                DropDownList1.Items.Add(New ListItem("--All Users--", "--All Users--"))
            Else
                DropDownList1.Items.Add(New ListItem("--All Users--", "--All Users--"))
            End If



            Dim ds As New DataSet
            Dim da As SqlDataAdapter

            ddlGeofence.Items.Add(New ListItem("--All Geofences--", "--All Geofences--"))
            da = New SqlDataAdapter("select geofencename,t1.geofenceid ,isnull(t2.geofenceid,0) as count from geofence t1 left outer join (select * from  user_geofence_favorite where userid=" & userid & ") as  t2 on t1.geofenceid =t2.geofenceid order by count desc ", conn2)
            da.Fill(ds)
            Dim count As Integer = 0
            For count = 0 To ds.Tables(0).Rows.Count - 1
                Dim ls As New ListItem(ds.Tables(0).Rows(count)(0).ToString().ToUpper(), ds.Tables(0).Rows(count)(1))
                If ds.Tables(0).Rows(count)(2) = 0 Then
                    ls.Attributes.Add("favorite", False)
                Else
                    ls.Attributes.Add("favorite", True)
                End If
                ddlGeofence.Items.Add(ls)
                'ddlGeofence.Items.Add(New ListItem(ds.Tables(0).Rows(count)(0).ToString().ToUpper(), ds.Tables(0).Rows(count)(1)))
            Next


            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            While (dr.Read())
                DropDownList1.Items.Add((New ListItem(dr("username").ToString(), dr("userid").ToString())))
            End While
            DropDownList1.SelectedValue = Request.Form("DropDownList1")
            ddlplate.Items.Add(New ListItem("--All Plate No's--", "--All Plate No's--"))
            If role = "User" Then
                cmd = New SqlCommand("select * from vehicleTBL where userid='" & userid & "' order by plateno", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select * from vehicleTBL where userid in (" & userslist & ") order by plateno", conn)
            Else
                cmd = New SqlCommand("select * from vehicleTBL order by plateno", conn)
            End If

            dr = cmd.ExecuteReader()

            While (dr.Read())
                ddlplate.Items.Add((New ListItem(dr("plateno"), dr("plateno"))))
            End While

            dr.Close()
            cmd.CommandText = "select * from geofence_type"
            dr = cmd.ExecuteReader()
            ddlgeofencetype.Items.Clear()
            ddlgeofencetype.Items.Add(New ListItem("ALL", "0"))
            While dr.Read()
                ddlgeofencetype.Items.Add(New ListItem(dr("GeofenceType"), dr("id")))
            End While



            ddlplate.SelectedValue = Request.Form("ddlplate")
            ddlGeofence.SelectedValue = Request.Form("ddlGeofence")


            conn.Close()

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
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")



            Else

                Dim dr As SqlDataReader
                Dim cmd As SqlCommand
                ddlplate.Items.Clear()
                ddlGeofence.Items.Clear()
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                Dim ds As New DataSet
                Dim da As SqlDataAdapter
                Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ddlGeofence.Items.Add(New ListItem("--All Geofences--", "--All Geofences--"))

                If ddlgeofencetype.SelectedValue = 0 Then
                    da = New SqlDataAdapter("select geofencename,t1.geofenceid ,isnull(t2.geofenceid,0) as count from geofence t1 left outer join (select * from  user_geofence_favorite where userid=" & userid & ") as  t2 on t1.geofenceid =t2.geofenceid order by count desc ", conn2)

                Else
                    da = New SqlDataAdapter("select geofencename,t1.geofenceid ,isnull(t2.geofenceid,0) as count from geofence t1 left outer join (select * from  user_geofence_favorite where userid=" & userid & ") as  t2 on t1.geofenceid =t2.geofenceid where t1.Gtype='" & ddlgeofencetype.SelectedValue & "' order by count desc", conn2)

                End If


                da.Fill(ds)
                Dim count As Integer = 0
                For count = 0 To ds.Tables(0).Rows.Count - 1
                    Dim ls As New ListItem(ds.Tables(0).Rows(count)(0).ToString().ToUpper(), ds.Tables(0).Rows(count)(1))
                    If ds.Tables(0).Rows(count)(2) = 0 Then
                        ls.Attributes.Add("favorite", False)
                    Else
                        ls.Attributes.Add("favorite", True)
                    End If
                    ddlGeofence.Items.Add(ls)
                    'ddlGeofence.Items.Add(New ListItem(ds.Tables(0).Rows(count)(0).ToString().ToUpper(), ds.Tables(0).Rows(count)(1)))
                Next







                ddlplate.Items.Add(New ListItem("--All Plate No's--", "--All Plate No's--"))
                cmd = New SqlCommand("select * from vehicleTBL where userid='" & Request.Form("DropDownList1") & "' order by plateno", conn)
                If Request.Form("DropDownList1") = "--All Users--" Then
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
                        ddlplate.Items.Add((New ListItem(dr("plateno"), dr("plateno"))))
                    End While
                Catch ex As Exception
                Finally

                    conn.Close()
                End Try

                Try
                    If conn.State <> ConnectionState.Open Then
                        conn.Open()
                    End If

                Catch ex As Exception

                End Try




                ddlplate.SelectedValue = Request.Form("ddlplate")
                ddlGeofence.SelectedValue = Request.Form("ddlGeofence")
            End If


        Catch ex As Exception

        End Try
    End Sub


    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function ManageFavorite(ByVal geoid As String, ByVal op As Int16) As String
        Dim retval As String = "0"
        Dim userid As String = HttpContext.Current.Request.Cookies("userinfo")("userid")
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try
            Dim cmd As New SqlCommand()
            cmd.Connection = conn
            If op = 0 Then
                cmd.CommandText = "insert into user_geofence_favorite (userid,geofenceid) values (@userid,@geofenceid)"
            Else
                cmd.CommandText = "delete from user_geofence_favorite where userid=@userid and geofenceid=@geofenceid"
            End If
            cmd.Parameters.AddWithValue("@userid", userid)
            cmd.Parameters.AddWithValue("@geofenceid", geoid)
            conn.Open()
            If cmd.ExecuteNonQuery() > 0 Then
                retval = "1"
            End If
        Catch ex As Exception

        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If

        End Try
        Return retval
    End Function

    Protected Sub DisplayLogInformation()
        Try

            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            Dim userid As String = DropDownList1.SelectedValue
            Dim plateno As String = ddlplate.SelectedValue
            Dim uid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")


            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Geofence Name"))
            t.Columns.Add(New DataColumn("In Time"))
            t.Columns.Add(New DataColumn("Out Time"))
            t.Columns.Add(New DataColumn("Duration"))
            t.Columns.Add(New DataColumn("username"))
            t.Columns.Add(New DataColumn("groupname"))
            t.Columns.Add(New DataColumn("Area"))


            Dim query As String = ""
            Dim condition As String = ""
            Dim insidequery As String = ""
            Dim timesfilter As String = ""
            If ddlgeofencetype.SelectedValue = "0" Then
                If ddlGeofence.SelectedValue <> "--All Geofences--" Then
                    condition = " and geofenceid='" & ddlGeofence.SelectedValue & " ' "
                End If
            Else
                If ddlGeofence.SelectedValue <> "--All Geofences--" Then
                    condition = " and geofenceid='" & ddlGeofence.SelectedValue & " ' "
                Else
                    condition = " and geofenceid in (select geofenceid from geofence where gtype='" & ddlgeofencetype.SelectedValue & "')"
                End If
            End If
            If chkinsidereport.Checked Then
                insidequery = " and h.outtimestamp is null"
                timesfilter = " and h.intimestamp>'2022/01/01 '"
            Else
                timesfilter = " and h.intimestamp between '" & begindatetime & "' and '" & enddatetime & "'"
            End If


            If DropDownList1.SelectedValue <> "--All Users--" Then
                If ddlplate.SelectedValue = "--All Plate No's--" Then
                    query = "select m.plateno,m.userid,m.intimestamp,m.outtimestamp,m.id,m.geofencename,u.username ,v.groupname,m.geoarea from (select h.plateno,h.userid,h.intimestamp,h.outtimestamp,h.id,g.geofencename,g.geoarea from geofence g join public_geofence_history  h on g.geofenceid=h.id and  h.userid='" & DropDownList1.SelectedValue & "'  " & timesfilter & " " & condition & " " & insidequery & ")m Join userTBL u on u.userid=m.userid  left outer Join vehicleTBL v on v.plateno= m.plateno"
                Else
                    query = "select m.plateno,m.userid,m.intimestamp,m.outtimestamp,m.id,m.geofencename,u.username ,v.groupname,m.geoarea from (select h.plateno,h.userid,h.intimestamp,h.outtimestamp,h.id,g.geofencename,g.geoarea from geofence g join public_geofence_history  h on g.geofenceid=h.id and h.plateno='" & ddlplate.SelectedValue & "' " & timesfilter & " " & condition & " " & insidequery & ") m Join userTBL u on u.userid=m.userid  left outer Join vehicleTBL v on v.plateno= m.plateno"
                End If
            Else
                If ddlplate.SelectedValue = "--All Plate No's--" Then
                    If role = "SuperUser" Or role = "Operator" Then
                        query = "select m.plateno,m.userid,m.intimestamp,m.outtimestamp,m.id,m.geofencename,u.username ,v.groupname,m.geoarea from (select h.plateno,h.userid,h.intimestamp,h.outtimestamp,h.id,g.geofencename,g.geoarea from geofence g join public_geofence_history  h on g.geofenceid=h.id and h.userid in (" & userslist & ")  " & timesfilter & " " & condition & " " & insidequery & ")m Join userTBL u on u.userid=m.userid  left outer Join vehicleTBL v on v.plateno= m.plateno"
                    ElseIf role = "Admin" Then
                        query = "select m.plateno,m.userid,m.intimestamp,m.outtimestamp,m.id,m.geofencename,u.username ,v.groupname,m.geoarea   from (select h.plateno,h.userid,h.intimestamp,h.outtimestamp,h.id,g.geofencename,g.geoarea from geofence g join public_geofence_history  h on g.geofenceid=h.id  " & timesfilter & " " & condition & " " & insidequery & ")m  Join userTBL u  on u.userid=m.userid  left outer Join vehicleTBL v on v.plateno= m.plateno"
                    End If
                Else
                    query = "select m.plateno,m.userid,m.intimestamp,m.outtimestamp,m.id,m.geofencename,u.username ,v.groupname,m.geoarea   from (select h.plateno,h.userid,h.intimestamp,h.outtimestamp,h.id,g.geofencename,g.geoarea from geofence g join public_geofence_history  h on g.geofenceid=h.id and h.plateno='" & ddlplate.SelectedValue & "' " & timesfilter & " " & condition & " " & insidequery & ")m Join userTBL u  on u.userid=m.userid  left outer Join vehicleTBL v on v.plateno= m.plateno"
                End If
            End If


            'Response.Write(query)

            'Read data from database server
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            'Dim cmd As SqlCommand = New SqlCommand("select plateno,convert(varchar(19),timestamp,120) as date,lat,lon,speed,bearing,odometer,ignition,alert_type,remarks from alert_history " & platecondition & datecondition & alertcondition & " order by timestamp", conn)
            Dim cmd As SqlCommand = New SqlCommand(query, conn)
            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim r As DataRow
                Dim i As Int64 = 1

                While dr.Read
                    r = t.NewRow
                    r(0) = i.ToString()

                    r(1) = dr("plateno")
                    r(2) = dr("geofencename").ToString.ToUpper
                    r(3) = Convert.ToDateTime(dr("intimestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                    If IsDBNull(dr("outtimestamp")) Then
                        r(4) = "-"
                    Else
                        r(4) = Convert.ToDateTime(dr("outtimestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                    End If

                    Dim intime As DateTime = Convert.ToDateTime(dr("intimestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                    Dim outtime As DateTime

                    If IsDBNull(dr("outtimestamp")) Then
                        outtime = DateTime.Now().ToString("yyyy/MM/dd HH:mm:ss")
                        r(5) = "<span style=""color:blue;cursor:pointer;"" onclick=""javascript:DisplayMap('" & intime.AddMinutes(-10).ToString("yyyy/MM/dd HH:mm:ss") & "','" & outtime.AddMinutes(30).ToString("yyyy/MM/dd HH:mm:ss") & "','" & dr("plateno") & "')"">--</span>"
                    Else
                        outtime = Convert.ToDateTime(dr("outtimestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                        r(5) = "<span style=""color:blue;cursor:pointer;"" onclick=""javascript:DisplayMap('" & intime.AddMinutes(-10).ToString("yyyy/MM/dd HH:mm:ss") & "','" & outtime.AddMinutes(10).ToString("yyyy/MM/dd HH:mm:ss") & "','" & dr("plateno") & "')"">" & (outtime - intime).TotalMinutes.ToString("0") & "</span>"

                    End If

                    r(6) = dr("username").ToString.ToUpper
                    If IsDBNull(dr("groupname")) Then
                        r(7) = ""
                    Else
                        r(7) = dr("groupname").ToString.ToUpper
                    End If

                    If dr("geoarea") = "0" Then
                        r(8) = "-"
                    Else
                        r(8) = dr("geoarea")
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
                    t.Rows.Add(r)
                End If

            Catch ex As Exception
                'Response.Write(ex.Message)
            Finally
                conn.Close()
            End Try
            Session.Remove("exceltable")
            Session("exceltable") = t
            sb1.Length = 0


            If (t.Rows.Count > 0) Then
                ec = "true"

                sb1.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")

                sb1.Append("<thead><tr><th>S No</th><th>Plate NO</th><th>User Name</th><th>Group Name</th><th>Geofence Name</th><th>Area</th><th>In Time</th><th>Out Time</th><th align=""right"">Duration (Mins)</th></tr></thead>")

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
                        sb1.Append(t.DefaultView.Item(i)(6))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(7))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(2))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(8))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(3))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(4))
                        sb1.Append("</td><td align=""right"">")
                        sb1.Append(t.DefaultView.Item(i)(5))
                        sb1.Append("</td></tr>")
                        counter += 1
                    Catch ex As Exception

                    End Try
                Next
                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr align=""left""><th>S No</th><th>Plate NO</th><th>User Name</th><th>Group Name</th><th>Geofence Name</th><th>Area</th><th>In Time</th><th>Out Time</th><th>Duration (Mins)</th></tr></tfoot>")
            Else

            End If

        Catch ex As Exception

        End Try

    End Sub




    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub

    Protected Sub ddlgeofencetype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlgeofencetype.SelectedIndexChanged
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            If ddlgeofencetype.SelectedValue = 0 Then
                cmd.CommandText = "select geofencename,t1.geofenceid ,isnull(t2.geofenceid,0) as count from geofence t1 left outer join (select * from  user_geofence_favorite where userid=" & userid & ") as  t2 on t1.geofenceid =t2.geofenceid order by count desc "
            Else
                cmd.CommandText = "select geofencename,t1.geofenceid ,isnull(t2.geofenceid,0) as count from geofence t1 left outer join (select * from  user_geofence_favorite where userid=" & userid & ") as  t2 on t1.geofenceid =t2.geofenceid where t1.Gtype='" & ddlgeofencetype.SelectedValue & "' order by count desc "
            End If
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlGeofence.Items.Clear()
            ddlGeofence.Items.Add(New ListItem("--All Geofences--", "--All Geofences--"))
            While dr.Read()
                Dim ls As New ListItem(dr("geofencename").ToString().ToUpper(), dr("geofenceid"))
                If dr("count") = 0 Then
                    ls.Attributes.Add("favorite", False)
                Else
                    ls.Attributes.Add("favorite", True)
                End If
                ddlGeofence.Items.Add(ls)
            End While
        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
End Class
