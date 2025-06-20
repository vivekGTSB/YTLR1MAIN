Public Class GetJobData
    Inherits System.Web.UI.Page
    
    Public plateno As String = ""
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate session
            If Not IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate and sanitize plate number input
            Dim rawPlateno As String = Request.QueryString("plateno")
            plateno = ValidatePlateNumber(rawPlateno)
            
            If String.IsNullOrEmpty(plateno) Then
                Response.Write("Invalid plate number")
                Response.End()
                Return
            End If
            
            hdnPlate.Value = HttpUtility.HtmlEncode(plateno)
            
        Catch ex As Exception
            LogSecurityEvent("GetJobData error", ex.Message)
            Response.Write("An error occurred")
        End Try
    End Sub

    ' SECURITY FIX: Validate session authentication
    Private Function IsUserAuthenticated() As Boolean
        Try
            Return HttpContext.Current.Session("authenticated") IsNot Nothing AndAlso 
                   CBool(HttpContext.Current.Session("authenticated")) AndAlso
                   HttpContext.Current.Session("userid") IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ' SECURITY FIX: Validate plate number input
    Private Function ValidatePlateNumber(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then
            Return ""
        End If

        ' Remove dangerous characters and limit length
        Dim sanitized As String = System.Text.RegularExpressions.Regex.Replace(input.Trim(), "[<>\"'%;()&+*/=]", "")
        
        ' Only allow alphanumeric, spaces, and hyphens for plate numbers
        If System.Text.RegularExpressions.Regex.IsMatch(sanitized, "^[A-Za-z0-9\-\s]{1,20}$") Then
            Return sanitized
        End If

        Return ""
    End Function

    ' SECURITY FIX: Secure logging
    Private Sub LogSecurityEvent(eventType As String, message As String)
        Try
            Dim logMessage As String = String.Format("{0}: {1} - User: {2}, IP: {3}", 
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                eventType, 
                If(HttpContext.Current.Session("userid"), "Unknown"), 
                HttpContext.Current.Request.UserHostAddress)
            
            System.Diagnostics.EventLog.WriteEntry("YTL_Security", logMessage, System.Diagnostics.EventLogEntryType.Warning)
        Catch
            ' Fail silently
        End Try
    End Sub
End Class