Imports System.Data.SqlClient
Imports System.Security.Cryptography

Public Class AuthenticationHelper
    
    ' Validate user session and return user info
    Public Shared Function ValidateUserSession(request As HttpRequest) As UserInfo
        Try
            If request.Cookies("userinfo") Is Nothing Then
                Return Nothing
            End If
            
            Dim userCookie = request.Cookies("userinfo")
            Dim userId As String = userCookie("userid")
            Dim role As String = userCookie("role")
            Dim sessionToken As String = userCookie("sessiontoken")
            
            ' Validate basic format
            If Not SecurityHelper.IsValidUserId(userId) OrElse 
               Not SecurityHelper.IsValidRole(role) OrElse 
               String.IsNullOrEmpty(sessionToken) Then
                Return Nothing
            End If
            
            ' Validate session token against database
            If Not ValidateSessionToken(userId, sessionToken) Then
                Return Nothing
            End If
            
            Dim userInfo As New UserInfo()
            userInfo.UserId = Integer.Parse(userId)
            userInfo.Role = role
            userInfo.UsersList = userCookie("userslist")
            userInfo.SessionToken = sessionToken
            
            Return userInfo
            
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error validating session: " & ex.Message)
            Return Nothing
        End Try
    End Function
    
    ' Validate session token against database
    Private Shared Function ValidateSessionToken(userId As String, sessionToken As String) As Boolean
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As New SqlCommand("SELECT COUNT(*) FROM user_sessions WHERE userid = @userid AND session_token = @token AND expires_at > @now AND is_active = 1", conn)
                cmd.Parameters.AddWithValue("@userid", userId)
                cmd.Parameters.AddWithValue("@token", sessionToken)
                cmd.Parameters.AddWithValue("@now", DateTime.Now)
                
                conn.Open()
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                Return count > 0
            End Using
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error validating session token: " & ex.Message)
            Return False
        End Try
    End Function
    
    ' Create new session
    Public Shared Function CreateUserSession(userId As Integer, role As String) As String
        Try
            Dim sessionToken As String = SecurityHelper.GenerateSecureToken()
            Dim expiresAt As DateTime = DateTime.Now.AddHours(8) ' 8 hour session
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' Invalidate old sessions
                Dim invalidateCmd As New SqlCommand("UPDATE user_sessions SET is_active = 0 WHERE userid = @userid", conn)
                invalidateCmd.Parameters.AddWithValue("@userid", userId)
                
                ' Create new session
                Dim createCmd As New SqlCommand("INSERT INTO user_sessions (userid, session_token, created_at, expires_at, is_active) VALUES (@userid, @token, @created, @expires, 1)", conn)
                createCmd.Parameters.AddWithValue("@userid", userId)
                createCmd.Parameters.AddWithValue("@token", sessionToken)
                createCmd.Parameters.AddWithValue("@created", DateTime.Now)
                createCmd.Parameters.AddWithValue("@expires", expiresAt)
                
                conn.Open()
                invalidateCmd.ExecuteNonQuery()
                createCmd.ExecuteNonQuery()
            End Using
            
            Return sessionToken
            
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error creating session: " & ex.Message)
            Return Nothing
        End Try
    End Function
    
    ' Invalidate user session
    Public Shared Sub InvalidateUserSession(userId As Integer, sessionToken As String)
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As New SqlCommand("UPDATE user_sessions SET is_active = 0 WHERE userid = @userid AND session_token = @token", conn)
                cmd.Parameters.AddWithValue("@userid", userId)
                cmd.Parameters.AddWithValue("@token", sessionToken)
                
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error invalidating session: " & ex.Message)
        End Try
    End Sub
    
    ' Check if user has required permission
    Public Shared Function HasPermission(userInfo As UserInfo, requiredRole As String) As Boolean
        If userInfo Is Nothing Then
            Return False
        End If
        
        Return SecurityHelper.HasPermission(userInfo.Role, requiredRole)
    End Function
    
    ' Redirect to login if not authenticated
    Public Shared Sub RequireAuthentication(response As HttpResponse, userInfo As UserInfo)
        If userInfo Is Nothing Then
            response.Redirect("Login.aspx")
        End If
    End Sub
    
    ' Require specific role
    Public Shared Sub RequireRole(response As HttpResponse, userInfo As UserInfo, requiredRole As String)
        RequireAuthentication(response, userInfo)
        
        If Not HasPermission(userInfo, requiredRole) Then
            response.Redirect("AccessDenied.aspx")
        End If
    End Sub
    
End Class

' User information class
Public Class UserInfo
    Public Property UserId As Integer
    Public Property Role As String
    Public Property UsersList As String
    Public Property SessionToken As String
End Class