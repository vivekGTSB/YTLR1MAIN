Imports System.IO
Namespace AVLS

    Partial Class Logout
        Inherits System.Web.UI.Page

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
                ' Log the logout event
                Dim username As String = "Unknown"
                Dim userId As String = "Unknown"
                
                If Session("username") IsNot Nothing Then
                    username = Session("username").ToString()
                End If
                
                If Session("userid") IsNot Nothing Then
                    userId = Session("userid").ToString()
                End If
                
                SecurityHelper.LogSecurityEvent("User logout: " & username)
                
                ' Update user_log table to mark session as logged out
                If Not Request.Cookies("userinfo") Is Nothing Then
                    Try
                        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                        Dim parameters As New Dictionary(Of String, Object) From {
                            {"@userid", userId},
                            {"@logouttime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}
                        }
                        
                        Dim query As String = "UPDATE user_log SET status=0, logouttime=@logouttime WHERE userid=@userid AND status=1"
                        DatabaseHelper.ExecuteNonQuery(query, parameters)
                    Catch ex As Exception
                        ' Log error but continue with logout
                        SecurityHelper.LogError("Error updating logout status", ex, Server)
                    End Try
                    
                    ' Clear authentication cookie
                    Dim myCookie As HttpCookie
                    myCookie = New HttpCookie("userinfo")
                    myCookie.Expires = DateTime.Now.AddDays(-1D)
                    Response.Cookies.Add(myCookie)
                End If
                
                ' Clear session
                Session.Clear()
                Session.Abandon()
                
                ' Clear any other cookies
                If (Not Request.Cookies("accesslevel") Is Nothing) Then
                    Dim myCookie As HttpCookie
                    myCookie = New HttpCookie("accesslevel")
                    myCookie.Expires = DateTime.Now.AddDays(-1D)
                    Response.Cookies.Add(myCookie)
                End If
                
                ' Clear all cookies
                Response.Cookies.Clear()
                
                ' Clear authentication ticket
                FormsAuthentication.SignOut()
                
            Catch ex As Exception
                ' Ensure session is cleared even if there's an error
                Session.Clear()
                Response.Cookies.Clear()
                Response.Cookies.Remove("userinfo")
                FormsAuthentication.SignOut()
            End Try
            
            ' Redirect to login page
            Response.Redirect("login.aspx")
        End Sub

    End Class

End Namespace