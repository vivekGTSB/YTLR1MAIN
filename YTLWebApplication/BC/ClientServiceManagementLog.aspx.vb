Imports System.Data.SqlClient

Partial Class ClientServiceManagementLog
    Inherits System.Web.UI.Page
    Public company As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim username As String = Request.Cookies("userinfo")("username")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            company = Request.Cookies("userinfo")("companyname")
            txtsearchdate.Value = Now().ToString("yyyy/MM/dd")
            txttodate.Value = Now().ToString("yyyy/MM/dd")
            hidrole.Value = role
            hidloginuser.Value = userid
            Dim cmd As SqlCommand
            Dim dr As SqlDataReader

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Try
                ddluser.Items.Clear()

                If role = "User" Then
                    ddluser.Items.Add(New ListItem(username, userid))
                ElseIf role = "SuperUser" Then
                    cmd = New SqlCommand("select userid,username from usertbl where userid in (" & userslist & ") order by username", conn)
                    ddluser.Items.Add(New ListItem("--Select Username--", "--Select Username--"))
                Else
                    cmd = New SqlCommand("select userid,username from usertbl where role='User'  order by username", conn)
                    ddluser.Items.Add(New ListItem("--Select Username--", "--Select Username--"))
                End If
                If role <> "User" Then
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    While dr.Read()
                        ddluser.Items.Add(New ListItem(dr("username"), dr("userid")))
                    End While
                    conn.Close()
                End If
                ddlplatenumbers.Items.Clear()
                ddlplatenumbers.Items.Add(New ListItem("--Select Plate No--", "--Select Plate No--"))
                cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & userid & "' order by plateno", conn)
                If role = "SuperUser" Then
                    cmd = New SqlCommand("select plateno from vehicleTBL where userid in (" & Request.Cookies("userinfo")("userslist") & ") order by plateno", conn)
                Else
                    cmd = New SqlCommand("select plateno from vehicleTBL where userid in (select userid from usertbl where role='User') order by plateno", conn)
                End If
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlplatenumbers.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                End While
                dr.Close()
                conn.Close()

            Catch ex As Exception
                Response.Write(ex.Message)
            Finally
                cmd.Dispose()
                dr.Close()
                conn.Dispose()
            End Try

        End If
    End Sub
End Class
