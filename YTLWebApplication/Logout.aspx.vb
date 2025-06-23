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
                SecurityHelper.LogSecurityEvent("User logout initiated")
                
                ' Clear session securely
                SessionManager.DestroySession("User logout")
                
                ' Clear cookies securely
                If (Not Request.Cookies("userinfo") Is Nothing) Then
                    Dim userinfoCookie As New HttpCookie("userinfo")
                    userinfoCookie.Expires = DateTime.Now.AddDays(-1D)
                    userinfoCookie.HttpOnly = True
                    userinfoCookie.Secure = True
                    Response.Cookies.Add(userinfoCookie)
                End If
                
                If (Not Request.Cookies("accesslevel") Is Nothing) Then
                    Dim accessCookie As New HttpCookie("accesslevel")
                    accessCookie.Expires = DateTime.Now.AddDays(-1D)
                    accessCookie.HttpOnly = True
                    accessCookie.Secure = True
                    Response.Cookies.Add(accessCookie)
                End If
                
                ' Clear all cookies
                Response.Cookies.Clear()
                
                ' Sign out from forms authentication
                FormsAuthentication.SignOut()
                
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("Logout error: " & ex.Message)
                Session.Clear()
                Response.Cookies.Clear()
            End Try
            
            ' Redirect to login page
            Response.Redirect("login.aspx", True)
        End Sub

    End Class

End Namespace