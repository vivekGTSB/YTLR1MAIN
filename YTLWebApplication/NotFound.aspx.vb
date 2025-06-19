Partial Public Class NotFound
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SecurityHelper.LogSecurityEvent("PAGE_NOT_FOUND", "404 page accessed for: " & Request.Url.ToString())
    End Sub
    
End Class