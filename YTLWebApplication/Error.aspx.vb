Partial Public Class Error
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Log the error but don't expose details to the user
        Dim ex As Exception = Server.GetLastError()
        
        If ex IsNot Nothing Then
            SecurityHelper.LogError("Unhandled exception", ex, Server)
            Server.ClearError()
        End If
        
        ' Add security headers
        Response.Headers.Add("X-Frame-Options", "DENY")
        Response.Headers.Add("X-Content-Type-Options", "nosniff")
        Response.Headers.Add("X-XSS-Protection", "1; mode=block")
        Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains")
    End Sub
    
End Class