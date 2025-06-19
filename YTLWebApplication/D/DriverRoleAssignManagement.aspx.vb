Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports Newtonsoft.Json
Imports ASPNetMultiLanguage
Imports System.Web.Script.Serialization

Public Class DriverRoleAssignManagement
    Inherits System.Web.UI.Page
    Public sb1 As New StringBuilder()
    Public opt As String
    Public sb As New StringBuilder()
    Public suserid As String
    Public userid As String
    Public un As String
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            un = Literal2.Text
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim suserid As String = Request.QueryString("userid")

            'Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            'Dim cmd As SqlCommand = New SqlCommand("select userid,username from usertbl where role='User' order by username", conn)
            'Dim dr As SqlDataReader
            'If role = "User" Then
            '    cmd = New SqlCommand("select userid,username from usertbl where userid='" & userid & "' order by username", conn)
            'ElseIf role = "SuperUser" Or role = "Operator" Then
            '    cmd = New SqlCommand("select userid,username from usertbl where userid in(" & userslist & ") order by username", conn)
            'End If

            'conn.Open()
            'dr = cmd.ExecuteReader()

            'If (role <> "User") Then
            '    ddluser1.Items.Add(New ListItem(Literal39.Text, Literal39.Text))

            'End If

            'While dr.Read()
            '    ddluser1.Items.Add(New ListItem(dr("username").ToString().ToUpper(), dr("userid")))

            'End While
            'conn.Close()

        Catch ex As Exception
        End Try
        MyBase.OnInit(e)
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function InsertupdateDriver(ByVal userId As String, ByVal plateno As String, ByVal owner As String, ByVal firstdriver As String, ByVal secdriver As String) As String
        Dim strqury As String = ""
        Dim result As Integer = 0
        Dim chkqury As String = ""
        Dim cmd As New SqlCommand
        Dim cmd2 As New SqlCommand
        Dim dr As SqlDataReader
        Dim plateuserid = 0
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try
            chkqury = "select top 1 t2.userid,t1.plateno from driver_role_assign t1 left join vehicleTBL t2 on t1.plateno=t2.plateno where t1.plateno=@plateno"
            strqury = "insert into driver_role_assign(plateno,ownerdid,f_driver,s_driver) values (@plateno,@owner,@fdriver,@sdriver);"
            strqury = strqury & "insert into driver_role_assign_log (plateno,ownerdid,f_driver,s_driver,action,updateby,timestamp,userid) values (@plateno,@owner,@fdriver,@sdriver,'new',@updateby,@ts,@userid)"
            cmd = New SqlCommand(chkqury, conn)
            cmd.Parameters.AddWithValue("@plateno", plateno)
            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                If dr.Read() Then
                    strqury = "update driver_role_assign set ownerdid=@owner,f_driver=@fdriver,s_driver=@sdriver where plateno=@plateno;"
                    strqury = strqury & "insert into driver_role_assign_log (plateno,ownerdid,f_driver,s_driver,action,updateby,timestamp,userid) values (@plateno,@owner,@fdriver,@sdriver,'update',@updateby,@ts,@userid)"
                        plateuserid = dr("userid")
                End If
                dr.Close()
            Catch ex As Exception
            End Try

            Try
                cmd2 = New SqlCommand(strqury, conn)
                cmd2.Parameters.AddWithValue("@plateno", plateno)
                cmd2.Parameters.AddWithValue("@owner", owner)
                cmd2.Parameters.AddWithValue("@fdriver", firstdriver)
                cmd2.Parameters.AddWithValue("@sdriver", secdriver)
                cmd2.Parameters.AddWithValue("@updateby", userId)
                cmd2.Parameters.AddWithValue("@userid", plateuserid)
                cmd2.Parameters.AddWithValue("@ts", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                result = cmd2.ExecuteNonQuery()
            Catch ex As Exception
            End Try

        Catch ex As Exception
            Return ex.Message & "  " & strqury
        Finally
            conn.Close()
        End Try
        Return result.ToString()
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            suserid = ss.Value

            If Not Page.IsPostBack Then
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                uid.Value = userid
                rle.Value = role
                ulist.Value = userslist
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand
                If role = "User" Then
                    cmd = New SqlCommand("select userid, username, dbip from usertbl where userid=@userid and username like @username order by username ", conn)
                    cmd.Parameters.AddWithValue("@userid", userid)
                    ulist.Value = userid
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid, username, dbip from usertbl where userid in (" & userslist & ") and username like @username order by username", conn)
                ElseIf role = "Admin" Then
                    cmd = New SqlCommand("select userid, username, dbip from usertbl where username like @username order by username", conn)
                Else
                    cmd = New SqlCommand("select userid, username, dbip from usertbl where  role=@role and username like @username order by username", conn)
                    cmd.Parameters.AddWithValue("@role", "User")
                End If
                cmd.Parameters.AddWithValue("@username", "YTL%")
                Dim dr As SqlDataReader
                sb = New StringBuilder()
                Try
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    sb.Length = 0
                    sb.Append("<select  id=""ddluser1"" onchange=""javascript: return refreshTable()""   data-placeholder=""Select User Group"" style=""width:250px;"" class=""chosen""  tabindex=""5"">")
                    sb.Append("<option id=""epty"" value=""""></option>")

                    If role = "SuperUser" Or role = "Admin" Then
                        sb.Append("<option selected=""selected"" value=""SELECT USERNAME""> " & Literal39.Text & "</option>")
                        sb.Append("<option value=ALL USERS>" & Literal40.Text & "</option>")
                    End If

                    Dim firstrecord As Boolean = True
                    While dr.Read
                        sb.Append("<option  value=")
                        sb.Append(dr("userid"))
                        sb.Append(">")
                        sb.Append(dr("username").ToString().ToUpper())
                        sb.Append("</option>")
                    End While
                    sb.Append("</select>")
                    dr.Close()
                Catch ex As Exception
                    '  Response.Write(ex.Message)
                Finally
                    conn.Close()
                End Try

                opt = sb.ToString()
            End If
            fillDrop()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub fillDrop()
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            If role = "User" Then
                cmd = New SqlCommand("select userid, username, dbip from usertbl where userid=@userid and username like @username order by username ", conn)
                cmd.Parameters.AddWithValue("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid, username, dbip from usertbl where userid in (" & userslist & ") and username like @username order by username", conn)
            ElseIf role = "Admin" Then
                cmd = New SqlCommand("select userid, username, dbip from usertbl where username like @username order by username", conn)
            Else
                cmd = New SqlCommand("select userid, username, dbip from avls_user_table where  role=@role and username like @username order by username", conn)
                cmd.Parameters.AddWithValue("@role", "User")
            End If
            cmd.Parameters.AddWithValue("@username", "YTL%")
            Dim dr As SqlDataReader
            sb = New StringBuilder()
            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                suserid = ss.Value
                sb.Length = 0
                sb.Append("<select  id=""ddluser1"" onchange=""javascript: return refreshTable()""   data-placeholder=""Select User Group"" style=""width:250px;"" class=""chosen""  tabindex=""5"">")
                sb.Append("<option id=""epty"" value=""""></option>")
                If role <> "User" Then
                    sb.Append("<option selected=""selected"" value=""SELECT USERNAME"">" & Literal39.Text & "</option>")
                End If
                If suserid <> "ALL USERS" Then
                    If role = "SuperUser" Or role = "Admin" Then
                        sb.Append("<option value=ALL USERS>" & Literal40.Text & "</option>")
                    End If

                End If
                Dim i As Integer = 0
                While dr.Read
                    userid = dr("userid")
                    If role = "User" Then
                        Dim ct As Integer = 0
                        Dim firstrecord As Boolean = True
                        If (firstrecord = True) Then
                            firstrecord = False
                            suserid = dr("userid")
                        End If
                        If ct = 0 Then
                            sb.Append("<option selected=""selected""  value=")
                            sb.Append(dr("userid"))
                            sb.Append(">")
                            sb.Append(dr("username").ToString().ToUpper())
                            sb.Append("</option>")

                        End If
                        ct = +1
                    Else
                    End If

                    If suserid = "ALL USERS" Then
                        If i = 0 Then
                            sb.Append("<option selected=""selected"" value=ALL USERS>" & Literal40.Text & "</option>")
                            i = +1
                        End If

                    End If
                    If role <> "User" Then
                        If userid = suserid Then
                            sb.Append("<option selected=""selected""  value=")
                            sb.Append(">")
                            sb.Append(dr("username").ToString().ToUpper())
                            sb.Append("</option>")
                        Else
                            sb.Append("<option  value=")
                            sb.Append(dr("userid"))
                            sb.Append(">")
                            sb.Append(dr("username").ToString().ToUpper())
                            sb.Append("</option>")
                        End If

                    End If
                End While
                sb.Append("</select>")
                dr.Close()
            Catch ex As Exception
                '  Response.Write(ex.Message)
            Finally
                conn.Close()
            End Try

            opt = sb.ToString()
        Catch ex As Exception

        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function FillGrid(ByVal ugData As String, ByVal role As String, ByVal userslist As String) As String
        Dim json As String = Nothing
        Try
            Dim suserid As String = ugData

            Dim r As DataRow
            Dim j As Int32 = 1

            Dim drivertable As New DataTable

            drivertable.Columns.Add(New DataColumn("SNo"))
            drivertable.Columns.Add(New DataColumn("Plate No"))
            drivertable.Columns.Add(New DataColumn("Owner Name"))
            drivertable.Columns.Add(New DataColumn("Driver Role"))
            drivertable.Columns.Add(New DataColumn("First Driver"))
            drivertable.Columns.Add(New DataColumn("Second Driver"))
            drivertable.Columns.Add(New DataColumn("Action"))

            If Not suserid = "SELECT USERNAME" Then
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")) 'connection.sqlConnection))
                Dim cmd As SqlCommand
                If role = "User" Then
                    cmd = New SqlCommand("select t1.plateno,t2.drivername,t3.f_driver,t3.s_driver,t3.ownerdid,t2.isowner,f.drivername as ownername,f2.drivername as firstdriver,f3.drivername as secdriver from vehicleTBL t1 left join driver_role_assign t3 on t1.plateno=t3.plateno left join driver t2 on t2.driverid=t3.ownerdid left join (select drivername,driverid from driver)as f on f.driverid=t3.ownerdid left join (select drivername,driverid from driver)as f2 on f2.driverid=t3.f_driver left join (select drivername,driverid from driver)as f3 on f3.driverid=t3.s_driver where t1.userid =  @suserid   order by t1.plateno", conn)
                Else
                    cmd = New SqlCommand("select t1.plateno,t2.drivername,t3.f_driver,t3.s_driver,t3.ownerdid,t2.isowner,f.drivername as ownername,f2.drivername as firstdriver,f3.drivername as secdriver from vehicleTBL t1 left join driver_role_assign t3 on t1.plateno=t3.plateno left join driver t2 on t2.driverid=t3.ownerdid left join (select drivername,driverid from driver)as f on f.driverid=t3.ownerdid left join (select drivername,driverid from driver)as f2 on f2.driverid=t3.f_driver left join (select drivername,driverid from driver)as f3 on f3.driverid=t3.s_driver where t1.userid =  @suserid   order by t1.plateno", conn)
                End If
                cmd.Parameters.AddWithValue("@suserid", suserid)

                Dim dr As SqlDataReader

                If suserid = "ALL" Then
                    cmd = New SqlCommand("select t1.plateno,t2.drivername,t3.f_driver,t3.s_driver,t3.ownerdid,t2.isowner,f.drivername as ownername,f2.drivername as firstdriver,f3.drivername as secdriver from vehicleTBL t1 left join driver_role_assign t3 on t1.plateno=t3.plateno left join driver t2 on t2.driverid=t3.ownerdid left join (select drivername,driverid from driver)as f on f.driverid=t3.ownerdid left join (select drivername,driverid from driver)as f2 on f2.driverid=t3.f_driver left join (select drivername,driverid from driver)as f3 on f3.driverid=t3.s_driver order by t1.plateno", conn)
                    If role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select t1.plateno,t2.drivername,t3.f_driver,t3.s_driver,t3.ownerdid,t2.isowner,f.drivername as ownername,f2.drivername as firstdriver,f3.drivername as secdriver from vehicleTBL t1 left join driver_role_assign t3 on t1.plateno=t3.plateno left join driver t2 on t2.driverid=t3.ownerdid left join (select drivername,driverid from driver)as f on f.driverid=t3.ownerdid left join (select drivername,driverid from driver)as f2 on f2.driverid=t3.f_driver left join (select drivername,driverid from driver)as f3 on f3.driverid=t3.s_driver where t1.userid in (" & userslist & ") order by t1.plateno", conn)
                    End If
                End If


                conn.Open()
                dr = cmd.ExecuteReader()

                Dim drivername As String = ""
                Dim ownerid = 0
                Dim fdriverid = 0
                Dim sdriverid = 0
                While dr.Read
                    r = drivertable.NewRow
                    'drivername = "-"
                    'If IsDBNull(dr("drivername")) = False Then
                    '    drivername = dr("drivername").ToString.ToUpper()
                    'End If
                    r(0) = j
                    r(1) = dr("plateno")
                    'r(2) = drivername
                    If IsDBNull(dr("ownername")) Then
                        r(2) = "-"
                    Else
                        r(2) = dr("ownername")
                    End If
                    If IsDBNull(dr("firstdriver")) Then
                        r(3) = "-"
                        fdriverid = 0
                    Else
                        r(3) = dr("firstdriver")
                        fdriverid = dr("f_driver")
                    End If
                    If IsDBNull(dr("secdriver")) Then
                        r(4) = "-"
                        sdriverid = 0
                    Else
                        r(4) = dr("secdriver")
                        sdriverid = dr("s_driver")
                    End If
                    Dim driverid = -1
                    If IsDBNull(dr("ownerdid")) Then
                    ElseIf Convert.ToInt32(dr("ownerdid")) = -1 Then
                    Else
                        driverid = Convert.ToInt32(dr("ownerdid"))
                    End If
                    r(5) = "<a href=""javascript:void(0)"" onclick=""openUpdatePopup('" & dr("plateno") & "'," & driverid & "," & fdriverid & "," & sdriverid & ")"">Update</a>"
                    drivertable.Rows.Add(r)
                    j = j + 1
                End While
                conn.Close()

            End If

            If drivertable.Rows.Count = 0 Then
                r = drivertable.NewRow
                r(0) = "--"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                r(5) = "--"
                drivertable.Rows.Add(r)
            End If
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable1")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session.Remove("exceltable3")
            Dim exceltable As New DataTable
            exceltable = drivertable.Copy()

            exceltable.Columns.Remove("sno")
            exceltable.Columns.Remove("action")
            HttpContext.Current.Session("exceltable") = exceltable

            Dim aa As New ArrayList
            Dim a As ArrayList

            For j1 As Integer = 0 To drivertable.Rows.Count - 1
                Try
                    a = New ArrayList
                    a.Add(drivertable.DefaultView.Item(j1)(0))
                    a.Add(drivertable.DefaultView.Item(j1)(1))
                    a.Add(drivertable.DefaultView.Item(j1)(2))
                    a.Add(drivertable.DefaultView.Item(j1)(3))
                    a.Add(drivertable.DefaultView.Item(j1)(4))
                    a.Add(drivertable.DefaultView.Item(j1)(5))
                    aa.Add(a)
                Catch ex As Exception

                End Try
            Next
            json = JsonConvert.SerializeObject(aa, Formatting.None)


        Catch ex As Exception
            Return ex.Message
        End Try
        Return json

    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetDriverList(ByVal userid As String, ByVal plateno As String) As String

        Dim result As Int16 = 0
        Dim modald1 As driverinfo = New driverinfo()
        Dim listd1 As List(Of driverinfo) = New List(Of driverinfo)
        'Dim modald2 As driverinfo = New driverinfo()
        'Dim listd2 As List(Of driverinfo) = New List(Of driverinfo)
        Dim modal As driverdata = New driverdata()
        Dim list As List(Of driverdata) = New List(Of driverdata)
        Dim lastd1 = 0
        Dim lastd2 = 0
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            'Dim getlatestdriver = "select f1.drivername,f2.drivername,t1.f_driver,t1.s_driver from driver_role_assign t1 left join (select drivername,driverid from driver) as f1 on t1.f_driver=f1.driverid left join (select drivername,driverid from driver) as f2 on t1.s_driver=f2.driverid where t1.plateno=@plateno"
            'Dim cmd As New SqlCommand(getlatestdriver, conn)
            'cmd.Parameters.AddWithValue("@plateno", plateno)
            Try
                conn.Open()
                'Dim dr = cmd.ExecuteReader
                'If dr.HasRows Then
                '    While dr.Read
                '        If IsDBNull(dr("f_driver")) Then
                '        Else
                '            lastd1 = dr("f_driver")
                '        End If
                '        If IsDBNull(dr("s_driver")) Then
                '        Else
                '            lastd2 = dr("s_driver")
                '        End If
                '    End While
                '    dr.Close()
                'End If
                Dim getdriver = "select * from driver where userid=@userid"
                Dim cmd2 As New SqlCommand(getdriver, conn)
                cmd2.Parameters.AddWithValue("@userid", userid)
                Dim dr2 = cmd2.ExecuteReader
                If dr2.HasRows Then
                    While dr2.Read
                        modald1 = New driverinfo()
                        'modald2 = New driverinfo()
                        'If CInt(dr2("driverid")) <> lastd1 Then
                        modald1.drivername = dr2("drivername")
                            modald1.driverid = dr2("driverid")
                            listd1.Add(modald1)
                        'End If
                        'If CInt(dr2("driverid")) <> lastd1 Then
                        '    modald2.drivername = dr2("drivername")
                        '    modald2.driverid = dr2("driverid")
                        '    listd2.Add(modald2)
                        'End If
                    End While
                    dr2.Close()
                End If
                modal.plateno = plateno
                modal.driver1 = listd1
                'modal.driver2 = listd2
            Catch ex As Exception
                Throw
            Finally
                conn.Close()
            End Try

        Catch ex As Exception
            Throw
        End Try
        Dim jscript As New JavaScriptSerializer
        Return jscript.Serialize(modal)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetOwnerList(ByVal userid As String, ByVal plateno As String) As String

        Dim result As Int16 = 0
        Dim modal = New driverinfo()
        Dim list = New List(Of driverinfo)
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Try
                conn.Open()
                Dim getdriver = "select * from driver where userid=@userid and isowner=1"
                Dim cmd2 As New SqlCommand(getdriver, conn)
                cmd2.Parameters.AddWithValue("@userid", userid)
                Dim dr2 = cmd2.ExecuteReader
                If dr2.HasRows Then
                    While dr2.Read
                        modal = New driverinfo()
                        modal.driverid = dr2("driverid")
                        modal.drivername = dr2("drivername")
                        list.Add(modal)
                    End While
                    dr2.Close()
                End If

            Catch ex As Exception
                Throw
            Finally
                conn.Close()
            End Try

        Catch ex As Exception
            Throw
        End Try
        Dim jscript As New JavaScriptSerializer
        Return jscript.Serialize(list)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function Delete(ByVal chekitems() As String) As Int16

        Dim result As Int16 = 0
        Dim strqury As String = ""
        Dim strargument As New StringBuilder()

        Dim cnt As Int16 = chekitems.Length()
        For i As Int16 = 0 To cnt - 1
            If i = cnt - 1 Then
                strargument.Append("'" & chekitems(i) & "'")
            Else
                strargument.Append("'" & chekitems(i) & "',")
            End If
        Next

        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            strqury = "Delete from avls_driver_table  where driverid in(" & strargument.ToString() & ")"
            Dim cmd As New SqlCommand(strqury, conn)
            Try
                conn.Open()
                result = cmd.ExecuteNonQuery()
            Catch ex As Exception

            Finally
                conn.Close()
            End Try

        Catch ex As Exception

        End Try
        Return result
    End Function

    Public Structure driverdata
        Public plateno As String
        Public driver1 As List(Of driverinfo)
    End Structure

    Public Structure driverinfo
        Public driverid As Integer
        Public drivername As String
    End Structure
End Class