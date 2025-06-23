<%@ Application Language="VB" %>
<%@ Import Namespace ="System.Data.SqlClient" %>
<%@ Import Namespace ="System.Web.Http" %>
<%@ Import Namespace ="System.Web.Routing" %>


<script runat="server">

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' SECURITY FIX: Check for HTTPS
        If Not Request.IsSecureConnection AndAlso Not Request.IsLocal Then
            Dim secureUrl As String = Request.Url.ToString().Replace("http://", "https://")
            Response.Redirect(secureUrl, True)
        End If
        
        ' SECURITY FIX: Add security headers
        Response.Headers.Add("X-Frame-Options", "DENY")
        Response.Headers.Add("X-Content-Type-Options", "nosniff")
        Response.Headers.Add("X-XSS-Protection", "1; mode=block")
        Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains")
        Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:;")
        
        ' SECURITY FIX: Remove server information
        Response.Headers.Remove("Server")
        Response.Headers.Remove("X-AspNet-Version")
        Response.Headers.Remove("X-Powered-By")
    End Sub

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application startup        
        RouteTable.Routes.MapHttpRoute(name:="DefaultApi",
                                       routeTemplate:="api/{controller}/{id}",
                                       defaults:=New With {
                                       Key .id = System.Web.Http.RouteParameter.[Optional]
                                       })
        
        ' SECURITY FIX: Register security module
        Dim modules As System.Collections.Generic.List(Of String) = New System.Collections.Generic.List(Of String)(System.Web.HttpContext.Current.ApplicationInstance.Modules.AllKeys)
        If Not modules.Contains("SecurityModule") Then
            System.Web.HttpContext.Current.ApplicationInstance.AddOnBeginRequestAsync(
                AddressOf SecurityModule.BeginRequest,
                AddressOf SecurityModule.EndRequest)
        End If
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown        
    End Sub


    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' SECURITY FIX: Secure error handling
        Dim ex As Exception = Server.GetLastError()
        If ex IsNot Nothing Then
            ' Log error securely
            Try
                Dim logPath As String = Server.MapPath("~/Logs/ErrorLog.txt")
                Dim logDir As String = System.IO.Path.GetDirectoryName(logPath)
                If Not System.IO.Directory.Exists(logDir) Then
                    System.IO.Directory.CreateDirectory(logDir)
                End If
                
                Dim logEntry As String = String.Format("{0:yyyy/MM/dd HH:mm:ss.fff} - Error: {1}{2}Stack Trace: {3}{2}",
                                                      DateTime.Now,
                                                      ex.Message,
                                                      Environment.NewLine,
                                                      ex.StackTrace)
                
                System.IO.File.AppendAllText(logPath, logEntry)
            Catch
                ' Fail silently
            End Try
            
            ' Redirect to error page
            Server.ClearError()
            Response.Redirect("~/Error.aspx")
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
            ' SECURITY FIX: Use parameterized query
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@userid", Session("userid")},
                {"@logintime", Session("logintime")},
                {"@logouttime", Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}
            }
            
            Dim query As String = "UPDATE user_log SET status=0, logouttime=@logouttime WHERE userid=@userid AND logintime=@logintime"
            DatabaseHelper.ExecuteNonQuery(query, parameters)
        Catch ex As Exception
            ' Log error securely
            Try
                Dim logPath As String = Server.MapPath("~/Logs/SessionLog.txt")
                Dim logDir As String = System.IO.Path.GetDirectoryName(logPath)
                If Not System.IO.Directory.Exists(logDir) Then
                    System.IO.Directory.CreateDirectory(logDir)
                End If
                
                Dim logEntry As String = String.Format("{0:yyyy/MM/dd HH:mm:ss.fff} - Session End Error: {1}{2}",
                                                      DateTime.Now,
                                                      ex.Message,
                                                      Environment.NewLine)
                
                System.IO.File.AppendAllText(logPath, logEntry)
            Catch
                ' Fail silently
            End Try
        End Try
    End Sub

</script>