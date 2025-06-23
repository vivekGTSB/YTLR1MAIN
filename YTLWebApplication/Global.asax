<%@ Application Language="VB" %>
<%@ Import Namespace ="System.Data.SqlClient" %>
<%@ Import Namespace ="System.Web.Http" %>
<%@ Import Namespace ="System.Web.Routing" %>


<script runat="server">

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Add security headers
        Response.Headers.Add("X-Frame-Options", "DENY")
        Response.Headers.Add("X-Content-Type-Options", "nosniff")
        Response.Headers.Add("X-XSS-Protection", "1; mode=block")
        
        ' Force HTTPS in production
        If Not Request.IsLocal AndAlso Not Request.IsSecureConnection Then
            Dim redirectUrl As String = Request.Url.ToString().Replace("http:", "https:")
            Response.Redirect(redirectUrl, True)
        End If
    End Sub

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application startup        
        RouteTable.Routes.MapHttpRoute(name:="DefaultApi",
                                       routeTemplate:="api/{controller}/{id}",
                                       defaults:=New With {
                                       Key .id = System.Web.Http.RouteParameter.[Optional]
                                       })
        
        ' Configure security settings
        ConfigureSecuritySettings()
    End Sub
    
    Private Sub ConfigureSecuritySettings()
        ' Set validation settings
        System.Web.Configuration.HttpCapabilitiesBase.BrowserCapabilitiesProvider = Nothing
        
        ' Configure request validation
        System.Web.Configuration.HttpRuntimeSection.ValidateRequestMode = RequestValidationMode.4.5
        
        ' Configure view state validation
        System.Web.UI.Page.ViewStateUserKey = Guid.NewGuid().ToString()
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown        
    End Sub


    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
        Dim ex As Exception = Server.GetLastError()
        
        ' Log the error securely
        If ex IsNot Nothing Then
            Try
                Dim logPath As String = Server.MapPath("~/Logs/ErrorLog.txt")
                Dim logDir As String = System.IO.Path.GetDirectoryName(logPath)
                
                If Not System.IO.Directory.Exists(logDir) Then
                    System.IO.Directory.CreateDirectory(logDir)
                End If
                
                ' Sanitize error message to prevent log injection
                Dim sanitizedMessage As String = ex.Message.Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ")
                If sanitizedMessage.Length > 500 Then
                    sanitizedMessage = sanitizedMessage.Substring(0, 500) & "..."
                End If
                
                Dim logEntry As String = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - ERROR: {sanitizedMessage} - URL: {Request.Url.ToString()} - IP: {Request.UserHostAddress}{Environment.NewLine}"
                
                System.IO.File.AppendAllText(logPath, logEntry)
            Catch
                ' Fail silently
            End Try
        End If
    End Sub
    
    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started
        Session.Timeout = 30 ' 30 minute timeout
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.      
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("update user_log set status=0,logouttime=@logouttime where userid=@userid and logintime=@logintime", conn)
            cmd.Parameters.AddWithValue("@logouttime", Now.ToString("yyyy-MM-dd HH:mm:ss:fff"))
            cmd.Parameters.AddWithValue("@userid", Session("userid"))
            cmd.Parameters.AddWithValue("@logintime", Session("logintime"))
            conn.Open()
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            ' Log error securely
            Try
                Dim logPath As String = Server.MapPath("~/Logs/SessionLog.txt")
                Dim logEntry As String = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - SESSION_END_ERROR: {ex.Message}{Environment.NewLine}"
                System.IO.File.AppendAllText(logPath, logEntry)
            Catch
                ' Fail silently
            End Try
        End Try
    End Sub

</script>