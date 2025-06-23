Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class POIManagement
    Inherits System.Web.UI.Page
    Public sb1 As New StringBuilder()
    Public addPOI As String = "AddPOI.aspx"
    Public opt As String
    Public sb As New StringBuilder()
    Public suserid As String
    Public userid As String
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim suserid As String = Request.QueryString("userid")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("select userid,username from userTBL where role='User' order by username", conn)
            Dim dr As SqlDataReader
            If role = "User" Then
                cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ") order by username", conn)
            End If

            conn.Open()
            dr = cmd.ExecuteReader()

        Catch ex As Exception
        End Try
        MyBase.OnInit(e)
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            suserid = ss.Value

            If Not Page.IsPostBack Then
                ImageButton1.Attributes.Add("onclick", "return deleteconfirmation();")
                ImageButton2.Attributes.Add("onclick", "return deleteconfirmation();")
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select userid, username, dbip from userTBL where  role='User' order by username", conn)
                If role = "User" Then
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid='" & userid & "' order by username ", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
                End If
                Dim dr As SqlDataReader
                sb = New StringBuilder()
                Try
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    sb.Length = 0
                    sb.Append("<select  id=""ddluser1"" onchange=""javascript: return refreshTable()""   data-placeholder=""Select User Group"" style=""width:250px;"" class=""chosen""  tabindex=""5"">")
                    sb.Append("<option id=""epty"" value=""""></option>")

                    If role = "SuperUser" Or role = "Admin" Then
                        sb.Append("<option selected=""selected"" value=""0"">SELECT USERNAME</option>")
                        sb.Append("<option value=--AllUsers-->--All Users--</option>")
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

            FillGrid()
        Catch ex As Exception

        End Try

    End Sub
    Private Sub fillDrop()
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("select userid, username, dbip from userTBL where  role='User' order by username", conn)
            If role = "User" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid='" & userid & "' order by username ", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
            End If
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
                    sb.Append("<option selected=""selected"" value=""0"">SELECT USERNAME</option>")
                End If
                If suserid <> "--AllUsers--" Then

                    If role = "SuperUser" Or role = "Admin" Then

                        sb.Append("<option value=--AllUsers-->--All Users--</option>")
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

                    If suserid = "--AllUsers--" Then
                        If i = 0 Then
                            sb.Append("<option selected=""selected"" value=--AllUsers-->--All Users--</option>")
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


    Private Sub FillGrid()
        Try


            Dim r As DataRow
            Dim j As Int32 = 1

            Dim poistable As New DataTable
            poistable.Columns.Add(New DataColumn("chk"))
            poistable.Columns.Add(New DataColumn("S No"))
            poistable.Columns.Add(New DataColumn("POI Name"))
            poistable.Columns.Add(New DataColumn("User Name"))
            poistable.Columns.Add(New DataColumn("Transporter"))
            poistable.Columns.Add(New DataColumn("Lat"))
            poistable.Columns.Add(New DataColumn("Lon"))
            poistable.Columns.Add(New DataColumn("Modified Datetime"))


            If Not suserid = "--Select User Name--" Then


                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")) 'connection.sqlConnection))
                Dim cmd As SqlCommand = New SqlCommand("select p.poiid,p.poitype,a.username,p.poiname,p.lat,p.lon,convert(varchar(19),p.modifieddatetime,120) as modifieddatetime,a.companyname from poi_new as p, userTBL as a  where p.userid =  '" & suserid & "' and a.userid = p.userid  and accesstype='0' order by poiname", conn)
                Dim dr As SqlDataReader

                If suserid = "--AllUsers--" Then
                    Dim role As String = Request.Cookies("userinfo")("role")
                    Dim userslist As String = Request.Cookies("userinfo")("userslist")
                    cmd = New SqlCommand("select p.poiid,p.poitype,a.username,p.poiname,p.lat,p.lon,convert(varchar(19),p.modifieddatetime,120) as modifieddatetime,a.companyname from poi_new as p, userTBL as a  where a.role='User' and a.userid = p.userid  and accesstype='0' order by poiname", conn)
                    If role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select p.poiid,p.poitype,a.username,p.poiname,p.lat,p.lon,convert(varchar(19),p.modifieddatetime,120) as modifieddatetime,a.companyname from poi_new as p, userTBL as a  where p.userid in( " & userslist & ") and a.userid = p.userid  and accesstype='0' order by poiname", conn)
                    End If

                End If


                conn.Open()
                dr = cmd.ExecuteReader()

                Dim poiname As String = ""

                While dr.Read
                    r = poistable.NewRow


                    poiname = dr("poiname").ToString.ToUpper()


                    If poiname.Length > 50 Then
                        poiname = poiname.Substring(0, 50) & "..."
                    End If
                    If LimitUserAccess() = True Then
                        r(0) = "<input type=""checkbox"" name=""chk"" value=""" & dr("poiid") & """/>"
                        r(2) = poiname
                    Else
                        r(0) = "<input type=""checkbox"" name=""chk"" value=""" & dr("poiid") & """/>"
                        r(2) = "<a href=""UpdatePOI.aspx?poiid=" & dr("poiid") & """'>" & poiname & "</a>"

                    End If
                    r(1) = j.ToString()
                    r(3) = dr("username").ToString.ToUpper()
                    r(4) = dr("companyname")
                    r(5) = System.Convert.ToDouble(dr("lat")).ToString("0.000000")
                    r(6) = System.Convert.ToDouble(dr("lon")).ToString("0.000000")
                    If IsDBNull(dr("modifieddatetime")) = False Then
                        Dim p = DateTime.Parse(dr("modifieddatetime")).ToString("yyyy/MM/dd  HH:mm:ss")
                        p = p.Replace("-", "/")
                        r(7) = p

                    End If


                    poistable.Rows.Add(r)
                    j = j + 1
                End While

                conn.Close()

            End If

            If poistable.Rows.Count = 0 Then
                r = poistable.NewRow
                r(0) = "<input type=""checkbox"" name=""chk"" value=""" & """/>"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                r(5) = "-"
                r(6) = "-"
                r(7) = "-"
                poistable.Rows.Add(r)
            End If

            Session("exceltable") = poistable

            sb1.Length = 0
            sb1.Append("<thead><tr><th align=""center"" style=""width:20px;""><input type='checkbox' onclick=""javascript:checkall(this);""/></th><th style=""width:30px;"" >S No</th><th>POI Name</th><th style=""width:120px;"">User Name</th><th style=""width:230px;"">Transporter</th><th>Lat</th><th>Lon</th><th style=""width:140px;"">Modify Datetime</th></tr></thead>")
            Dim counter As Integer = 1

            sb1.Append("<tbody>")
            For i As Integer = 0 To poistable.Rows.Count - 1
                Try

                    sb1.Append("<tr>")
                    sb1.Append("<td>")
                    sb1.Append(poistable.DefaultView.Item(i)(0))
                    sb1.Append("</td>")
                    sb1.Append("<td>")
                    sb1.Append(poistable.DefaultView.Item(i)(1))
                    sb1.Append("</td>")
                    sb1.Append("<td>")
                    sb1.Append(poistable.DefaultView.Item(i)(2))
                    sb1.Append("</td>")
                    sb1.Append("<td>")
                    sb1.Append(poistable.DefaultView.Item(i)(3))
                    sb1.Append("</td>")
                    sb1.Append("<td>")
                    sb1.Append(poistable.DefaultView.Item(i)(4))
                    sb1.Append("</td>")
                    sb1.Append("<td>")
                    sb1.Append(poistable.DefaultView.Item(i)(5))
                    sb1.Append("</td>")
                    sb1.Append("<td>")
                    sb1.Append(poistable.DefaultView.Item(i)(6))
                    sb1.Append("</td>")
                    sb1.Append("<td>")
                    sb1.Append(poistable.DefaultView.Item(i)(7))
                    sb1.Append("</td>")


                    sb1.Append("</tr>")

                    counter += 1
                Catch ex As Exception

                End Try
            Next


            sb1.Append("<tfoot><tr><th align=""center""  style=""width:20px;""><input type='checkbox' onclick=""javascript:checkall(this);""/></th><th style=""width:35px;"" >S No</th><th>POI Name</th><th>User Name</th><th style=""width:160px;"">Transporter</th><th>Lat</th><th>Lon</th><th style=""width:120px;"">Modify Datetime</th></tr></tr></tfoot>")
            sb1.Append("</tbody>")

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub



    Protected Sub DeletePOI()
        Try

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand

            Dim poiides() As String = Request.Form("chk").Split(",")

            For i As Int32 = 0 To poiides.Length - 1
                cmd = New SqlCommand("delete from poi_new where poiid='" & poiides(i) & "'", conn)
                Try
                    conn.Open()
                    cmd.ExecuteNonQuery()
                Catch ex As Exception

                Finally
                    conn.Close()
                End Try

            Next
            FillGrid()
        Catch ex As Exception

        End Try
    End Sub


    Function getUserLevel() As String
        Try
            Dim cmd As SqlCommand
            Dim Userlevel As String
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            cmd = New SqlCommand("select usertype from userTBL where userid='" & Request.Cookies("userinfo")("userid") & "'", conn)
            conn.Open()
            Userlevel = cmd.ExecuteScalar()
            conn.Close()

            Return Userlevel
        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Function

    Function LimitUserAccess() As Boolean
        If getUserLevel() = "7" Then
            ImageButton1.Visible = False
            ImageButton2.Visible = False
            Return True
        Else
            Return False
        End If
    End Function

    Protected Sub ImageButton2_Click(sender As Object, e As System.EventArgs) Handles ImageButton2.Click
        DeletePOI()
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DeletePOI()

    End Sub
End Class
