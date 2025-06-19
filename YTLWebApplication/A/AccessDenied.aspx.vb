Partial Public Class AccessDenied
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SecurityHelper.LogSecurityEvent("ACCESS_DENIED", "Access denied page accessed")
    End Sub
    
End Class