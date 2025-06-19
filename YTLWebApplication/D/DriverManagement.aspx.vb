Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports Newtonsoft.Json
Imports ASPNetMultiLanguage


Partial Class DriverManagement
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

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim dr As SqlDataReader
            If role = "User" Then
                cmd = New SqlCommand("select userid,username from userTBL where userid=@userid and username like @username order by username", conn)
                cmd.Parameters.AddWithValue("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ") and username like @username order by username", conn)
            Else
                cmd = New SqlCommand("select userid,username from userTBL where role=@role and username like @username order by username", conn)
                cmd.Parameters.AddWithValue("@role", "User")
            End If
            cmd.Parameters.AddWithValue("@username", "YTL%")

            conn.Open()
            dr = cmd.ExecuteReader()

            If (role <> "User") Then
                ddluserid.Items.Add(New ListItem(Literal39.Text, Literal39.Text))

            End If

            While dr.Read()
                ddluserid.Items.Add(New ListItem(dr("username").ToString().ToUpper(), dr("userid")))

            End While
            conn.Close()

        Catch ex As Exception
        End Try
        MyBase.OnInit(e)
    End Sub

    '<System.Web.Services.WebMethod()>
    'Public Shared Function InsertupdateDriver(ByVal userId As String, ByVal poiname As String, ByVal txtDOB As String, ByVal txtrfid As String, ByVal txtPhone As String, ByVal txtAddress As String, ByVal txtLicenceno As String, ByVal txtIssuingdate As String, ByVal txtExpiryDate As String, ByVal txtFuelCardNo As String, ByVal poiid As String, ByVal opr As Int16, ByVal ic As String, ByVal pwd As String) As String
    '    Dim strqury As String = ""
    '    Dim result As Integer = 0
    '    Dim chkqury As String = ""
    '    Dim cmd As New SqlCommand
    '    Dim dr As SqlDataReader
    '    Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
    '    Try
    '        chkqury = "select * from driver where phoneno='" + txtPhone + "'"
    '        If opr = 0 Then
    '            strqury = "insert into driver(rfid,userid,drivername,dateofbirth,phoneno,address,licenceno,issuingdate,expirydate,fuelcardno,driver_ic,password) "
    '            strqury = strqury + "values ('" & txtrfid & "','" & userId & "','" & poiname.Replace("'", "''") & "','" & txtDOB & "','" & txtPhone & "','" & txtAddress.Replace("'", "''") & "','" & txtLicenceno & "','" & txtIssuingdate & "','" & txtExpiryDate & "','" & txtFuelCardNo & "','" & ic & "','" & pwd & "')"
    '        Else
    '            strqury = "update  driver set userid='" & userId & "',drivername='" & poiname.Replace("'", "''") & "',rfid='" & txtrfid & "',dateofbirth='" & Convert.ToDateTime(txtDOB).ToString("yyyy/MM/dd") & "',phoneno='" & txtPhone & "',address='" & txtAddress & "',licenceno='" & txtLicenceno & "',issuingdate='" & Convert.ToDateTime(txtIssuingdate).ToString("yyyy/MM/dd") & "',expirydate='" & Convert.ToDateTime(txtExpiryDate).ToString("yyyy/MM/dd") & "',fuelcardno='" & txtFuelCardNo & "',password='" & pwd & "',driver_ic='" & ic & "'  where driverid='" & poiid & "' "
    '        End If
    '        If opr = 0 Then
    '            cmd = New SqlCommand(chkqury, conn)
    '            Try
    '                conn.Open()
    '                dr = cmd.ExecuteReader()
    '                If dr.Read() Then
    '                    result = 99
    '                Else
    '                    cmd = New SqlCommand(strqury, conn)
    '                    result = cmd.ExecuteNonQuery()
    '                End If
    '            Catch ex As Exception

    '            Finally
    '                conn.Close()
    '            End Try
    '        Else
    '            Try
    '                conn.Open()
    '                cmd = New SqlCommand(strqury, conn)
    '                result = cmd.ExecuteNonQuery()
    '            Catch ex As Exception
    '            Finally
    '                conn.Close()
    '            End Try
    '        End If



    '    Catch ex As Exception
    '        Return ex.Message & "  " & strqury
    '    Finally
    '        conn.Close()
    '    End Try
    '    Return result.ToString()
    'End Function

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
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid=@userid  and username like @username order by username ", conn)
                    cmd.Parameters.AddWithValue("@userid", userid)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ")  and username like @username order by username", conn)
                Else
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where  role=@role  and username like @username order by username", conn)
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
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid=@userid  and username like @username order by username ", conn)
                cmd.Parameters.AddWithValue("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ")  and username like @username order by username", conn)
            Else
                cmd = New SqlCommand("select userid, username, dbip from userTBL where  role=@role  and username like @username order by username", conn)
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

    '   <System.Web.Services.WebMethod()> _
    '<ScriptMethod(ResponseFormat:=ResponseFormat.Xml)> _
    '   Public Shared Function FillGrid(ByVal ugData As String, ByVal role As String, ByVal userslist As String) As String
    '       Dim json As String = Nothing
    '       Try
    '           Dim suserid As String = ugData

    '           Dim r As DataRow
    '           Dim j As Int32 = 1

    '           Dim drivertable As New DataTable
    '           drivertable.Columns.Add(New DataColumn("chk"))
    '           drivertable.Columns.Add(New DataColumn("sno"))
    '           drivertable.Columns.Add(New DataColumn("drivername"))
    '           drivertable.Columns.Add(New DataColumn("Licence No"))
    '           drivertable.Columns.Add(New DataColumn("Exp Date"))
    '           drivertable.Columns.Add(New DataColumn("Phone No"))
    '           drivertable.Columns.Add(New DataColumn("Address"))
    '           drivertable.Columns.Add(New DataColumn("Fuel Card No"))
    '           drivertable.Columns.Add(New DataColumn("rfid"))

    '           drivertable.Columns.Add(New DataColumn("dob"))
    '           drivertable.Columns.Add(New DataColumn("Issuingdate"))


    '           drivertable.Columns.Add(New DataColumn("userid"))
    '           drivertable.Columns.Add(New DataColumn("status"))

    '           drivertable.Columns.Add(New DataColumn("driveric"))
    '           drivertable.Columns.Add(New DataColumn("pwd"))


    '           If Not suserid = "SELECT USERNAME" Then
    '               Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")) 'connection.sqlConnection))
    '               Dim cmd As SqlCommand = New SqlCommand("select * from driver  where userid =  '" & suserid & "'  order by drivername", conn)
    '               Dim dr As SqlDataReader

    '               If suserid = "ALL" Then
    '                   cmd = New SqlCommand("select * from driver  order by drivername", conn)
    '                   If role = "SuperUser" Or role = "Operator" Then
    '                       cmd = New SqlCommand("select * from driver  where userid in (" & userslist & ")  order by drivername", conn)
    '                   End If
    '               End If


    '               conn.Open()
    '               dr = cmd.ExecuteReader()

    '               Dim drivername As String = ""

    '               While dr.Read
    '                   r = drivertable.NewRow
    '                   drivername = dr("drivername").ToString.ToUpper()
    '                   r(0) = dr("driverid")
    '                   r(1) = j.ToString()
    '                   r(2) = drivername

    '                   r(3) = dr("licenceno").ToString.ToUpper()
    '                   r(4) = Convert.ToDateTime(dr("expirydate")).ToString("yyyy/MM/dd")
    '                   r(5) = dr("phoneno")
    '                   r(6) = dr("address")
    '                   r(7) = dr("fuelcardno")
    '                   r(8) = dr("rfid")

    '                   r(9) = Convert.ToDateTime(dr("dateofbirth")).ToString("yyyy/MM/dd")
    '                   r(10) = Convert.ToDateTime(dr("issuingdate")).ToString("yyyy/MM/dd")


    '                   r(11) = dr("userid")

    '                   If dr("status") Then
    '                       r(12) = "1"
    '                   Else
    '                       r(12) = "0"
    '                   End If

    '                   If IsDBNull(dr("driver_ic")) Then
    '                       r(13) = dr("driver_ic")
    '                   Else
    '                       r(13) = "-"
    '                   End If

    '                   If IsDBNull(dr("password")) Then
    '                       r(14) = dr("password")
    '                   Else
    '                       r(14) = "-"
    '                   End If



    '                   drivertable.Rows.Add(r)
    '                   j = j + 1
    '               End While

    '               conn.Close()

    '           End If

    '           If drivertable.Rows.Count = 0 Then
    '               r = drivertable.NewRow
    '               r(0) = "--"
    '               r(1) = "--"
    '               r(2) = "--"
    '               r(3) = "--"
    '               r(4) = "--"
    '               r(5) = "--"
    '               r(6) = "--"
    '               r(7) = "--"
    '               r(8) = "--"
    '               r(9) = "--"
    '               r(10) = "--"
    '               r(11) = "--"
    '               r(12) = "--"
    '               r(13) = "--"
    '               r(14) = "--"
    '               drivertable.Rows.Add(r)
    '           End If

    '           Dim aa As New ArrayList
    '           Dim a As ArrayList

    '           For j1 As Integer = 0 To drivertable.Rows.Count - 1
    '               Try
    '                   a = New ArrayList
    '                   a.Add(drivertable.DefaultView.Item(j1)(0))
    '                   a.Add(drivertable.DefaultView.Item(j1)(1))
    '                   a.Add(drivertable.DefaultView.Item(j1)(2))
    '                   a.Add(drivertable.DefaultView.Item(j1)(3))
    '                   a.Add(drivertable.DefaultView.Item(j1)(4))
    '                   a.Add(drivertable.DefaultView.Item(j1)(5))
    '                   a.Add(drivertable.DefaultView.Item(j1)(6))
    '                   a.Add(drivertable.DefaultView.Item(j1)(7))

    '                   a.Add(drivertable.DefaultView.Item(j1)(8))
    '                   a.Add(drivertable.DefaultView.Item(j1)(9))
    '                   a.Add(drivertable.DefaultView.Item(j1)(10))
    '                   a.Add(drivertable.DefaultView.Item(j1)(11))
    '                   a.Add(drivertable.DefaultView.Item(j1)(0))
    '                   a.Add(drivertable.DefaultView.Item(j1)(12))
    '                   a.Add(drivertable.DefaultView.Item(j1)(3))
    '                   a.Add(drivertable.DefaultView.Item(j1)(4))
    '                   a.Add(drivertable.DefaultView.Item(j1)(6))
    '                   a.Add(drivertable.DefaultView.Item(j1)(7))
    '                   a.Add(drivertable.DefaultView.Item(j1)(8))

    '                   a.Add(drivertable.DefaultView.Item(j1)(13))
    '                   a.Add(drivertable.DefaultView.Item(j1)(14))

    '                   aa.Add(a)
    '               Catch ex As Exception

    '               End Try
    '           Next
    '           json = JsonConvert.SerializeObject(aa, Formatting.None)


    '       Catch ex As Exception
    '           Return ex.Message
    '       End Try
    '       Return json

    '   End Function



    '   <System.Web.Services.WebMethod()> _
    '   Public Shared Function Activate(ByVal chekitems() As String) As Int16

    '       Dim result As Int16 = 0
    '       Dim strqury As String = ""
    '       Dim strargument As New StringBuilder()

    '       Dim cnt As Int16 = chekitems.Length()
    '       For i As Int16 = 0 To cnt - 1
    '           If i = cnt - 1 Then
    '               strargument.Append("'" & chekitems(i) & "'")
    '           Else
    '               strargument.Append("'" & chekitems(i) & "',")
    '           End If
    '       Next

    '       Try
    '           Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
    '           strqury = "update driver set status=1 where driverid in(" & strargument.ToString() & ")"
    '           Dim cmd As New SqlCommand(strqury, conn)

    '           Try
    '               conn.Open()
    '               result = cmd.ExecuteNonQuery()
    '           Catch ex As Exception

    '           Finally
    '               conn.Close()
    '           End Try

    '       Catch ex As Exception

    '       End Try
    '       Return result
    '   End Function

    '   <System.Web.Services.WebMethod()> _
    '   Public Shared Function InActivate(ByVal chekitems() As String) As Int16

    '       Dim result As Int16 = 0
    '       Dim strqury As String = ""
    '       Dim strargument As New StringBuilder()

    '       Dim cnt As Int16 = chekitems.Length()
    '       For i As Int16 = 0 To cnt - 1
    '           If i = cnt - 1 Then
    '               strargument.Append("'" & chekitems(i) & "'")
    '           Else
    '               strargument.Append("'" & chekitems(i) & "',")
    '           End If
    '       Next

    '       Try
    '           Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
    '           strqury = "update driver set status=0 where driverid in(" & strargument.ToString() & ")"
    '           Dim cmd As New SqlCommand(strqury, conn)
    '           Try
    '               conn.Open()
    '               result = cmd.ExecuteNonQuery()
    '           Catch ex As Exception

    '           Finally
    '               conn.Close()
    '           End Try

    '       Catch ex As Exception

    '       End Try
    '       Return result
    '   End Function


End Class
