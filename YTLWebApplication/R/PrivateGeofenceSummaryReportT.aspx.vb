Imports AspMap
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic

Partial Class PrivateGeofenceSummaryReportT
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
            Dim ds As New DataSet
            Dim da As SqlDataAdapter
            ddlGeofence.Items.Add(New ListItem("--All Geofences--", "--All Geofences--"))
            cmd = New SqlCommand("select * from userTBL where role='User' order by username", conn)

            da = New SqlDataAdapter("select geofencename,geofenceid from geofence where accesstype='0' order by LTRIM(geofencename)", conn2)

            If role = "User" Then
                cmd = New SqlCommand("select * from userTBL where userid='" & userid & "' order by username", conn)
                da = New SqlDataAdapter("select geofencename,geofenceid from geofence where accesstype='0' and userid = '" & userid & "' order by LTRIM(geofencename)", conn2)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select * from userTBL where role='User' and userid in (" & userslist & ") order by username", conn)
                da = New SqlDataAdapter("select geofencename,geofenceid from geofence where accesstype='0'and userid in (" & userslist & ") order by LTRIM(geofencename)", conn2)
                DropDownList1.Items.Add(New ListItem("--All Users--", "--All Users--"))
            Else
                DropDownList1.Items.Add(New ListItem("--All Users--", "--All Users--"))
            End If
            da.Fill(ds)
            Dim count As Integer = 0
            Dim ddlGeoDict As New Dictionary(Of String, String)
            ddlGeoDict.Add("--All Geofences--", "--All Geofences--")
            For count = 0 To ds.Tables(0).Rows.Count - 1
                If ddlGeoDict.ContainsKey(ds.Tables(0).Rows(count)("geofencename").ToString.ToUpper()) Then
                    Dim geoId As String = ddlGeoDict.Item(ds.Tables(0).Rows(count)("geofencename").ToString.ToUpper())
                    ddlGeoDict(ds.Tables(0).Rows(count)("geofencename").ToString.ToUpper()) = geoId & "," & ds.Tables(0).Rows(count)("geofenceid").ToString.ToUpper()
                Else
                    ddlGeoDict.Add(ds.Tables(0).Rows(count)("geofencename").ToString.ToUpper(), ds.Tables(0).Rows(count)("geofenceid").ToString.ToUpper())
                End If
            Next
            ddlGeofence.DataTextField = "Key"
            ddlGeofence.DataValueField = "Value"
            ddlGeofence.DataSource = ddlGeoDict
            ddlGeofence.DataBind()

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
                da = New SqlDataAdapter("select geofencename,geofenceid from geofence where accesstype='0' and userid = '" & Request.Form("DropDownList1") & "' order by LTRIM(geofencename)", conn)

                cmd = New SqlCommand("select * from vehicleTBL where userid='" & Request.Form("DropDownList1") & "' order by plateno", conn)
                If Request.Form("DropDownList1") = "--All Users--" Then
                    If role = "SuperUser" Or role = "Operator" Then
                        da = New SqlDataAdapter("select geofencename,geofenceid from geofence where accesstype='0'and userid in (" & userslist & ") order by LTRIM(geofencename)", conn)
                        cmd = New SqlCommand("select * from vehicleTBL where userid in (" & userslist & ") order by plateno", conn)
                    Else
                        da = New SqlDataAdapter("select geofencename,geofenceid from geofence where accesstype='0' order by LTRIM(geofencename)", conn)
                        cmd = New SqlCommand("select * from vehicleTBL order by plateno", conn)
                    End If
                End If
                Dim ds As New DataSet
                Try
                    da.Fill(ds)
                    Dim count As Integer = 0
                    Dim ddlGeoDict As New Dictionary(Of String, String)
                    ddlGeoDict.Add("--All Geofences--", "--All Geofences--")
                    For count = 0 To ds.Tables(0).Rows.Count - 1
                        If ddlGeoDict.ContainsKey(ds.Tables(0).Rows(count)("geofencename").ToString.ToUpper()) Then
                            Dim geoId As String = ddlGeoDict.Item(ds.Tables(0).Rows(count)("geofencename").ToString.ToUpper())
                            ddlGeoDict(ds.Tables(0).Rows(count)("geofencename").ToString.ToUpper()) = geoId & "," & ds.Tables(0).Rows(count)("geofenceid").ToString.ToUpper()
                        Else
                            ddlGeoDict.Add(ds.Tables(0).Rows(count)("geofencename").ToString.ToUpper(), ds.Tables(0).Rows(count)("geofenceid").ToString.ToUpper())
                        End If
                    Next
                    ddlGeofence.DataTextField = "Key"
                    ddlGeofence.DataValueField = "Value"
                    ddlGeofence.DataSource = ddlGeoDict
                    ddlGeofence.DataBind()

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


            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Geofence Name"))
            t.Columns.Add(New DataColumn("In Time"))
            t.Columns.Add(New DataColumn("Out Time"))
            t.Columns.Add(New DataColumn("Duration"))
            t.Columns.Add(New DataColumn("username"))
            t.Columns.Add(New DataColumn("groupname"))
            t.Columns.Add(New DataColumn("Log"))


            Dim query As String = ""
            Dim condition As String = ""
            If ddlGeofence.SelectedValue <> "--All Geofences--" Then
                condition = " and geofenceid in (" & ddlGeofence.SelectedValue & " ) "
            End If

            If DropDownList1.SelectedValue <> "--All Users--" Then
                If ddlplate.SelectedValue = "--All Plate No's--" Then
                    query = "select m.plateno,m.userid,m.intimestamp,m.outtimestamp,m.id,m.geofencename,u.username ,v.groupname from (select h.plateno,h.userid,h.intimestamp,h.outtimestamp,h.id,g.geofencename from geofence g join private_geofence_history  h on g.geofenceid=h.id and  h.userid='" & DropDownList1.SelectedValue & "'  and h.intimestamp between '" & begindatetime & "' and '" & enddatetime & "' " & condition & ")m Join userTBL u on u.userid=m.userid  left outer Join vehicleTBL v on v.plateno= m.plateno"
                Else
                    query = "select m.plateno,m.userid,m.intimestamp,m.outtimestamp,m.id,m.geofencename,u.username ,v.groupname from (select h.plateno,h.userid,h.intimestamp,h.outtimestamp,h.id,g.geofencename from geofence g join private_geofence_history  h on g.geofenceid=h.id and h.plateno='" & ddlplate.SelectedValue & "' and h.intimestamp between '" & begindatetime & "' and '" & enddatetime & "' " & condition & ") m Join userTBL u on u.userid=m.userid  left outer Join vehicleTBL v on v.plateno= m.plateno"
                End If
            Else
                If ddlplate.SelectedValue = "--All Plate No's--" Then
                    If role = "SuperUser" Or role = "Operator" Then
                        query = "select m.plateno,m.userid,m.intimestamp,m.outtimestamp,m.id,m.geofencename,u.username ,v.groupname from (select h.plateno,h.userid,h.intimestamp,h.outtimestamp,h.id,g.geofencename from geofence g join private_geofence_history  h on g.geofenceid=h.id and h.userid in (" & userslist & ")  and h.intimestamp between '" & begindatetime & "' and '" & enddatetime & "' " & condition & ")m Join userTBL u on u.userid=m.userid  left outer Join vehicleTBL v on v.plateno= m.plateno"
                    ElseIf role = "Admin" Then
                        query = "select m.plateno,m.userid,m.intimestamp,m.outtimestamp,m.id,m.geofencename,u.username ,v.groupname   from (select h.plateno,h.userid,h.intimestamp,h.outtimestamp,h.id,g.geofencename from geofence g join private_geofence_history  h on g.geofenceid=h.id  and h.intimestamp between '" & begindatetime & "' and '" & enddatetime & "' " & condition & ")m  Join userTBL u  on u.userid=m.userid  left outer Join vehicleTBL v on v.plateno= m.plateno"
                    End If
                Else
                    query = "select m.plateno,m.userid,m.intimestamp,m.outtimestamp,m.id,m.geofencename,u.username ,v.groupname   from (select h.plateno,h.userid,h.intimestamp,h.outtimestamp,h.id,g.geofencename from geofence g join private_geofence_history  h on g.geofenceid=h.id and h.plateno='" & ddlplate.SelectedValue & "' and h.intimestamp between '" & begindatetime & "' and '" & enddatetime & "' " & condition & ")m Join userTBL u  on u.userid=m.userid  left outer Join vehicleTBL v on v.plateno= m.plateno"
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
                    r(4) = Convert.ToDateTime(dr("outtimestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                    Dim intime As DateTime = Convert.ToDateTime(dr("intimestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                    Dim outtime As DateTime = Convert.ToDateTime(dr("outtimestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                    r(5) = "<span style=""color:blue;cursor:pointer;"" onclick=""javascript:DisplayMap('" & intime.AddMinutes(-10).ToString("yyyy/MM/dd HH:mm:ss") & "','" & outtime.AddMinutes(10).ToString("yyyy/MM/dd HH:mm:ss") & "','" & dr("plateno") & "')"">" & (outtime - intime).TotalMinutes.ToString("0") & "</span>"
                    r(6) = dr("username").ToString.ToUpper
                    r(7) = dr("groupname").ToString.ToUpper
                    r(8) = "<span style=""color:blue;cursor:pointer;text-decoration:underline;"" onclick=""javascript:openLog('GetLogData.aspx?p=" & dr("plateno") & "&b=" & intime.ToString("yyyy/MM/dd HH:mm:ss") & "&e=" & outtime.ToString("yyyy/MM/dd HH:mm:ss") & "')"">View Log</span>"
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

                sb1.Append("<thead><tr><th>S No</th><th>Plate NO</th><th>User Name</th><th>Group Name</th><th>Geofence Name</th><th>In Time</th><th>Out Time</th><th align=""right"">Duration (Mins)</th><th>Log Data</th></tr></thead>")

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
                        sb1.Append(t.DefaultView.Item(i)(3))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(4))
                        sb1.Append("</td><td align=""right"">")
                        sb1.Append(t.DefaultView.Item(i)(5))
                        sb1.Append("</td><td align=""center"">")
                        sb1.Append(t.DefaultView.Item(i)(8))
                        sb1.Append("</td></tr>")
                        counter += 1
                    Catch ex As Exception

                    End Try
                Next
                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr align=""left""><th>S No</th><th>Plate NO</th><th>User Name</th><th>Group Name</th><th>Geofence Name</th><th>In Time</th><th>Out Time</th><th>Duration (Mins)</th><th>Log Data</th></tr></tfoot>")
            Else

            End If

        Catch ex As Exception

        End Try

    End Sub




    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub
End Class
