Imports System.Data.SqlClient

Partial Class ClientServiceManagement
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            hidrole.Value = role
            hidloginuser.Value = userid
            Dim cmd As SqlCommand
            Dim dr As SqlDataReader

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Try

                ddlplatenumbers.Items.Clear()
                ddlplatenumbers.Items.Add(New ListItem("--Select Plate No--", "--Select Plate No--"))
                'cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & userid & "' order by plateno", conn)
                'If role = "SuperUser" Then
                '    cmd = New SqlCommand("select plateno from vehicleTBL where userid in (" & Request.Cookies("userinfo")("userslist") & ") order by plateno", conn)
                'End If
                If role = "SuperUser" Then
                    'Get user list from cookie and validate it
                    Dim usersList As String = Request.Cookies("userinfo")("userslist")
                    Dim userArray() As String = usersList.Split(","c)

                    'Build parameterized IN clause
                    Dim inClause As New List(Of String)()
                    For i As Integer = 0 To userArray.Length - 1
                        inClause.Add("@user" & i)
                    Next

                    Dim sql As String = "SELECT plateno FROM vehicleTBL WHERE userid IN (" & String.Join(",", inClause) & ") ORDER BY plateno"
                    cmd = New SqlCommand(sql, conn)

                    ' Add parameters safely
                    For i As Integer = 0 To userArray.Length - 1
                        cmd.Parameters.AddWithValue("@user" & i, userArray(i).Trim())
                    Next
                Else
                    Dim sql As String = "SELECT plateno FROM vehicleTBL WHERE userid = @userid ORDER BY plateno"
                    cmd = New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@userid", userid)
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
