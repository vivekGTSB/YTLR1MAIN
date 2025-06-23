
Partial Class SuperServiceMangement
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim role As String = Request.Cookies("userinfo")("role")
        If role = "User" Or role = "Operator" Then
            Response.Redirect("Login.aspx")
        End If
        hidrole.Value = role
        hidloginuser.Value = userid
    End Sub
End Class
