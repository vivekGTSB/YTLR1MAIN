Imports Newtonsoft.Json
Imports System.Data.SqlClient

Partial Class GetUsers
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim json As String = ""
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand

            If role = "User" Then
                cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid,username from userTBL where  userid in(" & userslist & ") order by username", conn)
            Else
                cmd = New SqlCommand("select userid,username from userTBL where role='User' order by username", conn)
            End If
            Dim dr As SqlDataReader


            Dim aa As New ArrayList()

            Try
                conn.Open()
                dr = cmd.ExecuteReader()

                While dr.Read()
                    Try
                        Dim a As New ArrayList()
                        a.Add(dr("userid"))
                        a.Add(dr("username").ToString.ToUpper())
                        aa.Add(a)
                    Catch ex As Exception

                    End Try
                End While

                Dim jss As New Newtonsoft.Json.JsonSerializer()


                json = JsonConvert.SerializeObject(aa, Formatting.None)
            Catch ex As Exception

            Finally
                conn.Close()
            End Try
            Response.Write(json)
            Response.ContentType = "text/plain"
        Catch ex As Exception

        End Try

    End Sub
End Class
