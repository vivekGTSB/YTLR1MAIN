<%@ Application Language="VB" %>
<%@ Import Namespace ="System.Data.SqlClient" %>
<%@ Import Namespace ="System.Web.Http" %>
<%@ Import Namespace ="System.Web.Routing" %>


<script runat="server">

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' SECURITY FIX: Add security headers
        Response.Headers.Add("X-Frame-Options", "DENY")
        Response.Headers.Add("X-Content-Type-Options", "nosniff")
        Response.Headers.Add("X-XSS-Protection", "1; mode=block")
        Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self'")
    End Sub

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application startup        
        RouteTable.Routes.MapHttpRoute(name:="DefaultApi",
                                       routeTemplate:="api/{controller}/{id}",
                                       defaults:=New With {
                                       Key .id = System.Web.Http.RouteParameter.[Optional]
                                       })
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown        
    End Sub


    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
        Dim ex As Exception = Server.GetLastError()
        
        ' SECURITY FIX: Log error but don't expose details
        If ex IsNot Nothing Then
            SecurityHelper.LogError("Application_Error", ex, Server)
            
            ' Redirect to error page
            Response.Clear()
            Server.Transfer("~/Error.aspx")
        End If
    End Sub
    
    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started     
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.      
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As SqlCommand = New SqlCommand("UPDATE user_log SET status=0, logouttime=@logoutTime WHERE userid=@userid AND logintime=@loginTime", conn)
            cmd.Parameters.AddWithValue("@logoutTime", Now.ToString("yyyy-MM-dd HH:mm:ss:fff"))
            cmd.Parameters.AddWithValue("@userid", Session("userid"))
            cmd.Parameters.AddWithValue("@loginTime", Session("logintime"))
            
            conn.Open()
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            ' Log error silently
            Try
                SecurityHelper.LogError("Session_End error", ex, Server)
            Catch
                ' Fail silently
            End Try
        End Try
    End Sub

</script>