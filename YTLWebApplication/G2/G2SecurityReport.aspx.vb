Partial Public Class G2SecurityReport
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' SECURITY: Validate session and admin access
        If Not G2SecurityHelper.ValidateSession() Then
            Response.Redirect("~/Login.aspx")
            Return
        End If
        
        If Not G2SecurityHelper.HasRequiredRole("ADMIN") Then
            Response.Redirect("~/AccessDenied.aspx")
            Return
        End If
        
        G2SecurityHelper.LogSecurityEvent("G2_SECURITY_REPORT_ACCESS", "G2 Security report accessed")
    End Sub
    
End Class