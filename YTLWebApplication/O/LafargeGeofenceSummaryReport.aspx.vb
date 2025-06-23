Imports AspMap
Imports System.Data
Imports System.Data.SqlClient

Partial Class LafargeGeofenceSummaryReport
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

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim cmd As SqlCommand

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim ds As New DataSet
            Dim da As SqlDataAdapter
            ddlGeofence.Items.Add(New ListItem("--All Geofences--", "--All Geofences--"))
            cmd = New SqlCommand("select * from userTBL where role='User' order by username", conn)

            da = New SqlDataAdapter("select geofencename,geofenceid from geofence where accesstype='2' order by LTRIM(geofencename)", conn2)

            If role = "User" Then
                cmd = New SqlCommand("select * from userTBL where userid='" & userid & "' order by username", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select * from userTBL where role='User' and userid in (" & userslist & ") order by username", conn)
                DropDownList1.Items.Add(New ListItem("--All Users--", "--All Users--"))
            Else
                DropDownList1.Items.Add(New ListItem("--All Users--", "--All Users--"))
            End If
            da.Fill(ds)
            Dim count As Integer = 0
            For count = 0 To ds.Tables(0).Rows.Count - 1
                ddlGeofence.Items.Add(New ListItem(ds.Tables(0).Rows(count)(0).ToString().ToUpper(), ds.Tables(0).Rows(count)(1)))
            Next
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            While (dr.Read())
                DropDownList1.Items.Add((New ListItem(dr("username").ToString(), dr("userid").ToString())))
                If role = "Admin" Then
                    If adminusers = "" Then
                        adminusers = dr("userid").ToString()
                    Else
                        adminusers = adminusers & "," & dr("userid").ToString()
                    End If

                End If

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

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            Else
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                Dim dr As SqlDataReader
                Dim cmd As SqlCommand
                ddlplate.Items.Clear()
                ddlGeofence.Items.Clear()
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ddlplate.Items.Add(New ListItem("--All Plate No's--", "--All Plate No's--"))
                ddlGeofence.Items.Add(New ListItem("--All Geofences--", "--All Geofences--"))
                Dim da As SqlDataAdapter
                da = New SqlDataAdapter("select geofencename,geofenceid from geofence where accesstype='2'  order by LTRIM(geofencename)", conn)

                cmd = New SqlCommand("select * from vehicleTBL where userid='" & Request.Form("DropDownList1") & "' order by plateno", conn)
                If Request.Form("DropDownList1") = "--All Users--" Then
                    If role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select * from vehicleTBL where userid in (" & userslist & ") order by plateno", conn)
                    Else
                        cmd = New SqlCommand("select * from vehicleTBL order by plateno", conn)
                    End If
                End If
                Dim ds As New DataSet
                Try
                    da.Fill(ds)
                    Dim count As Integer = 0
                    For count = 0 To ds.Tables(0).Rows.Count - 1
                        ddlGeofence.Items.Add(New ListItem(ds.Tables(0).Rows(count)(0).ToString().ToUpper(), ds.Tables(0).Rows(count)(1)))
                    Next
                    ddlGeofence.SelectedValue = Request.Form("ddlGeofence")
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
                ddlplate.SelectedValue = Request.Form("ddlplate")
            End If


        Catch ex As Exception

        End Try
    End Sub

    Protected Sub DisplayLogInformation()
        Try

            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            Dim userid As String = DropDownList1.SelectedValue
            Dim plateno As String = ddlplate.SelectedValue
            Dim uid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim users() As String
            users = userslist.Split(",")
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Geofence Name"))
            t.Columns.Add(New DataColumn("User Name"))
            t.Columns.Add(New DataColumn("Group Name"))
            t.Columns.Add(New DataColumn("In Time"))
            t.Columns.Add(New DataColumn("Out Time"))
            t.Columns.Add(New DataColumn("Duration"))









            'Dim query As String = ""
            'Dim condition As String = ""
            'If ddlGeofence.SelectedValue <> "--All Geofences--" Then
            '    condition = " and id='" & ddlGeofence.SelectedValue & " ' "
            'End If

            'If DropDownList1.SelectedValue <> "--All Users--" Then
            '    If ddlplate.SelectedValue = "--All Plate No's--" Then
            '        query = "select u.userid,u.username,vt.groupname,gt.geofencename,lgt.timestamp,lgt.plateno,lgt.status,lgt.id    from (select * from lafarge_geofence_history  where userid='" & DropDownList1.SelectedValue & "' and timestamp between '" & begindatetime & "' and '" & enddatetime & "' " & condition & ") lgt left  outer join userTBL u on u.userid=lgt.userid left outer join vehicleTBL vt on vt.plateno=lgt.plateno left outer join geofence gt on gt.geofenceid=lgt.id"
            '    Else
            '        query = "select u.userid,u.username,vt.groupname,gt.geofencename,lgt.timestamp,lgt.plateno,lgt.status,lgt.id    from (select * from lafarge_geofence_history  where plateno='" & ddlplate.SelectedValue & "' and timestamp between '" & begindatetime & "' and '" & enddatetime & "' " & condition & ") lgt left  outer join userTBL u on u.userid=lgt.userid left outer join vehicleTBL vt on vt.plateno=lgt.plateno left outer join geofence gt on gt.geofenceid=lgt.id"
            '    End If
            'Else
            '    If ddlplate.SelectedValue = "--All Plate No's--" Then
            '        If role = "SuperUser" Or role = "Operator" Then
            '            query = "select u.userid,u.username,vt.groupname,gt.geofencename,lgt.timestamp,lgt.plateno,lgt.status,lgt.id    from (select * from lafarge_geofence_history  where userid in (" & userslist & ") and timestamp between '" & begindatetime & "' and '" & enddatetime & "' " & condition & ") lgt left  outer join userTBL u on u.userid=lgt.userid left outer join vehicleTBL vt on vt.plateno=lgt.plateno left outer join geofence gt on gt.geofenceid=lgt.id"
            '        ElseIf role = "Admin" Then
            '            query = "select u.userid,u.username,vt.groupname,gt.geofencename,lgt.timestamp,lgt.plateno,lgt.status,lgt.id    from (select * from lafarge_geofence_history  where  timestamp between '" & begindatetime & "' and '" & enddatetime & "' " & condition & ") lgt left  outer join userTBL u on u.userid=lgt.userid left outer join vehicleTBL vt on vt.plateno=lgt.plateno left outer join geofence gt on gt.geofenceid=lgt.id"
            '        End If
            '    Else
            '        query = "select u.userid,u.username,vt.groupname,gt.geofencename,lgt.timestamp,lgt.plateno,lgt.status,lgt.id    from (select * from lafarge_geofence_history  where plateno='" & ddlplate.SelectedValue & "' and timestamp between '" & begindatetime & "' and '" & enddatetime & "' " & condition & ") lgt left  outer join userTBL u on u.userid=lgt.userid left outer join vehicleTBL vt on vt.plateno=lgt.plateno left outer join geofence gt on gt.geofenceid=lgt.id"
            '    End If
            'End If
            'Response.Write(query)
            'Read data from database server
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("sp_GetGeofencereportprivate", conn)
            cmd.CommandType = CommandType.StoredProcedure

            If DropDownList1.SelectedValue <> "--All Users--" Then
                cmd.Parameters.AddWithValue("@userid", DropDownList1.SelectedValue)
            Else
                If role = "Admin" Then
                    cmd.Parameters.AddWithValue("@userid", adminusers)
                Else
                    cmd.Parameters.AddWithValue("@userid", userslist)
                End If
            End If
            If ddlplate.SelectedValue = "--All Plate No's--" Then
                cmd.Parameters.AddWithValue("@plateno", "ALL")
            Else
                cmd.Parameters.AddWithValue("@plateno", ddlplate.SelectedValue)
            End If
            If ddlGeofence.SelectedValue <> "--All Geofences--" Then
                cmd.Parameters.AddWithValue("@gid", ddlGeofence.SelectedValue)
            Else
                cmd.Parameters.AddWithValue("@gid", "ALL")
            End If
            cmd.Parameters.AddWithValue("@frmdtae", begindatetime)
            cmd.Parameters.AddWithValue("@todtae", enddatetime)

            Dim da As New SqlDataAdapter(cmd)
            Dim ds As New DataSet
            da.Fill(ds)

            Try

                Dim i As Int64 = 1
                Dim r As DataRow
                For ii As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    r = t.NewRow
                    r(0) = i.ToString()
                    r(1) = ds.Tables(0).Rows(ii)("plateno")
                    r(2) = ds.Tables(0).Rows(ii)("geofencename").ToString.ToUpper
                    r(3) = ds.Tables(0).Rows(ii)("username").ToString.ToUpper
                    r(4) = ds.Tables(0).Rows(ii)("groupname").ToString.ToUpper
                    r(5) = Convert.ToDateTime(ds.Tables(0).Rows(ii)("intime")).ToString("yyyy/MM/dd HH:mm:ss")
                    If IsDBNull(ds.Tables(0).Rows(ii)("outtime")) Then
                        r(6) = ""
                        r(7) = ""
                    Else
                        r(6) = Convert.ToDateTime(ds.Tables(0).Rows(ii)("outtime")).ToString("yyyy/MM/dd HH:mm:ss")
                        r(7) = "<span style=""color:blue;cursor:pointer;"" onclick=""javascript:DisplayMap('" & Convert.ToDateTime(ds.Tables(0).Rows(ii)("intime")).AddMinutes(-10).ToString("yyyy/MM/dd HH:mm:ss") & "','" & Convert.ToDateTime(ds.Tables(0).Rows(ii)("outtime")).AddMinutes(10).ToString("yyyy/MM/dd HH:mm:ss") & "','" & ds.Tables(0).Rows(ii)("plateno") & "')"">" & ds.Tables(0).Rows(ii)("duration") & " Min</span>"

                    End If
                    t.Rows.Add(r)
                    i = i + 1
                Next

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

                sb1.Append("<thead><tr align=""left""><th>S No</th><th>Plate No</th><th>Geofence Name</th><th>User Name</th><th>Group Name</th><th>In Time</th><th>Out Time</th><th>Duration</th></tr></thead>")

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
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(7))
                        sb1.Append("</td></tr>")
                        counter += 1
                    Catch ex As Exception
                        Response.Write(ex.Message)
                    End Try
                Next
                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr align=""left""><th>S No</th><th>Plate No</th><th>Geofence Name</th><th>User Name</th><th>Group Name</th><th>In Time</th><th>Out Time</th><th>Duration</th></tr></tfoot>")
            Else

            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub




    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub
End Class
