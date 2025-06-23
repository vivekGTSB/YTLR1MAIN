
Partial Class kokhaw_fleet_help
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("logout.aspx")
            End If
            Dim role As String = Request.Cookies("userinfo")("role")

            If role.ToLower() = "admin" Or role.ToLower() = "operator" Or role.ToLower() = "adminviewer" Then
                tol1.visible = True
                tol2.visible = True
            End If
        End If
    End Sub
End Class
