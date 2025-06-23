Imports System.Data
Imports System.Data.SqlClient


Partial Class LafargeVDM
    Inherits System.Web.UI.Page
    Public sb As New StringBuilder
    Public opt As String

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
        Catch ex As Exception
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Not Page.IsPostBack Then
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                Dim query As String = "select plateno from vehicleTBL"


                If role = "User" Then
                    query = "select plateno from vehicleTBL where userid ='" & userid & "'"
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    query = "select plateno from vehicleTBL where userid in( " & userslist & ")"
                End If

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand(query, conn)

                Dim dr As SqlDataReader
                Try
                    ddlGeofence.Items.Clear()
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    While dr.Read
                        ddlGeofence.Items.Add(New ListItem(dr("plateno").ToString.ToUpper(), dr("plateno")))
                    End While
                    dr.Close()
                Catch ex As Exception
                    Response.Write(ex.Message)
                Finally
                    conn.Close()
                End Try


                If role = "User" Then
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid='" & userid & "' order by username ", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
                End If

                sb = New StringBuilder()
                Try
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    sb.Length = 0
                    sb.Append("<select  id=""ddluser1"" onchange=""javascript: return refreshTable()""   data-placeholder=""Select User Group"" style=""width:250px;"" class=""chosen""  tabindex=""5"">")
                    sb.Append("<option id=""epty"" value=""""></option>")

                    If role = "SuperUser" Or role = "Admin" Then
                        sb.Append("<option selected=""selected"" value=""0"">SELECT USERNAME</option>")
                        sb.Append("<option value=ALL>All USERS</option>")
                    End If
                    ddluser.Items.Add(New ListItem("SELECT A USER", "0"))
                    ddluser.Items.Add(New ListItem("ALL USERS", "ALL USERS"))
                    Dim firstrecord As Boolean = True
                    While dr.Read
                        ddluser.Items.Add(New ListItem(dr("username").ToString().ToUpper(), dr("userid").ToString()))
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

        Catch ex As Exception

        End Try

    End Sub

End Class
