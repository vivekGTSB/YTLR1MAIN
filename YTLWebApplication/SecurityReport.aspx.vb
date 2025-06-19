Partial Public Class SecurityReport
    Inherits SecurePageBase
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Only allow admin access to security report
        If Not AuthenticationHelper.HasRole("Admin") Then
            Response.Redirect("~/AccessDenied.aspx")
            Return
        End If
        
        SecurityHelper.LogSecurityEvent("SECURITY_REPORT_ACCESS", "Security report accessed")
    End Sub
    
End Class