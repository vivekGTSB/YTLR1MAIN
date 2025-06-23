Imports System.Data.SqlClient

Namespace AVLS

    Partial Class PasswordManagement
        Inherits System.Web.UI.Page
        Public errormessage As String
        Public backpage As String = "Management.html"

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub


        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Request.Cookies("userinfo") Is Nothing Then
                    Server.Transfer("Login.aspx")
                End If

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return mysubmit()")

                    Dim userid As String = Request.Cookies("userinfo")("userid")
                    Dim role As String = Request.Cookies("userinfo")("role")
                    Dim userslist As String = Request.Cookies("userinfo")("userslist")

                    If role = "Admin" Then
                        backpage = "AdminManagement.html"
                    End If

                    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                    Dim cmd As SqlCommand = New SqlCommand("select * from userTBL where userid='" & userid & "'", conn)

                    conn.Open()
                    Dim dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.Read() Then
                        usernametextbox.Text = dr("username")
                    End If

                    conn.Close()
                End If

            Catch ex As Exception
                errormessage = ex.Message
            End Try
        End Sub

        Private Sub ImageButton1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            Try
                Dim userid As String = Request.Cookies("userinfo")("userid")

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                Dim username As String = usernametextbox.Text.Trim()
                Dim password As String = passwordtextbox.Text.Trim()
                Dim newpassword As String = newpasswordtextbox.Text.Trim()

                Dim cmd As SqlCommand = New SqlCommand("select * from userTBL where userid='" & userid & "'", conn)
                Try
                    conn.Open()

                    Dim dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.Read() Then
                        If password.ToUpper() = dr("pwd").ToString().ToUpper() Then
                            cmd = New SqlCommand("update userTBL set username='" & username & "',pwd='" & newpassword & "',pwdstatus=1 where userid ='" & userid & "'", conn)
                            cmd.ExecuteNonQuery()

                            If cmd.ExecuteNonQuery() > 0 Then
                                errormessage = "Your password successfully updated."
                            End If
                        Else
                            errormessage = "Your old password is invalid."
                        End If
                    End If
                    conn.Close()

                Catch ex As Exception
                    errormessage = ex.Message
                End Try

            Catch ex As Exception
                errormessage = ex.Message
            End Try
        End Sub
    End Class

End Namespace
