Partial Public Class Error
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Log the error that brought us here
        Dim ex As Exception = Server.GetLastError()
        
        If ex IsNot Nothing Then
            SecurityHelper.LogError("Unhandled exception", ex, Server)
            Server.ClearError()
        End If
        
        ' Don't show any specific error details to the user
        Response.TrySkipIisCustomErrors = True
    End Sub
End Class