Partial Public Class AccessDenied
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Log the access denied event
        Dim username As String = "Unknown"
        Dim requestedUrl As String = "Unknown"
        
        If Session("username") IsNot Nothing Then
            username = Session("username").ToString()
        End If
        
        If Request.UrlReferrer IsNot Nothing Then
            requestedUrl = Request.UrlReferrer.ToString()
        End If
        
        SecurityHelper.LogSecurityEvent("ACCESS_DENIED", $"User: {username}, Attempted to access: {requestedUrl}")
        
        ' Add security headers
        Response.Headers.Add("X-Frame-Options", "DENY")
        Response.Headers.Add("X-Content-Type-Options", "nosniff")
        Response.Headers.Add("X-XSS-Protection", "1; mode=block")
        Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains")
    End Sub
    
End Class