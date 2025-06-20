Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Security.Cryptography
Imports System.Text

Namespace AVLS
    Partial Class Login
        Inherits System.Web.UI.Page
        Public errormessage As String = ""
        Public foc As String = "uname"
        
        Private Const MAX_LOGIN_ATTEMPTS As Integer = 5
        Private Const LOCKOUT_DURATION_MINUTES As Integer = 15
        Private Const SESSION_TIMEOUT_MINUTES As Integer = 30

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try
                ' Add comprehensive security headers
                AddSecurityHeaders()
                
                ImageButton1.Attributes.Add("onclick", "return enhancedValidation()")
                
                ' Clear any existing sessions securely
                ClearExistingSessions()

            Catch ex As Exception
                LogSecurityEvent("Page_Load error: " & ex.Message, Request.UserHostAddress)
                errormessage = "An error occurred. Please try again."
            End Try
        End Sub

        Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            Try
                ' Comprehensive input validation
                If Not ValidateInput() Then
                    Return
                End If
                
                ' Check for account lockout
                If IsAccountLocked(uname.Value.Trim()) Then
                    errormessage = "Account temporarily locked due to multiple failed login attempts. Please try again later."
                    foc = "uname"
                    LogSecurityEvent("Login attempt on locked account: " & uname.Value, Request.UserHostAddress)
                    Return
                End If
                
                ' Rate limiting check
                If IsRateLimited() Then
                    errormessage = "Too many requests. Please wait before trying again."
                    LogSecurityEvent("Rate limit exceeded from IP: " & Request.UserHostAddress, Request.UserHostAddress)
                    Return
                End If
                
                ' Authenticate user with secure methods
                If AuthenticateUser(uname.Value.Trim(), password.Value) Then
                    ' Successful login
                    ClearFailedAttempts(uname.Value.Trim())
                    ClearRateLimit()
                    RedirectToMainPage()
                Else
                    ' Failed login
                    RecordFailedAttempt(uname.Value.Trim())
                    errormessage = "Invalid username or password."
                    foc = "password"
                    LogSecurityEvent("Failed login attempt for user: " & uname.Value, Request.UserHostAddress)
                End If

            Catch ex As Exception
                LogSecurityEvent("Login error: " & ex.Message, Request.UserHostAddress)
                errormessage = "An error occurred during login. Please try again."
            End Try
        End Sub

        ' Add comprehensive security headers
        Private Sub AddSecurityHeaders()
            With Response.Headers
                .Add("X-Frame-Options", "DENY")
                .Add("X-Content-Type-Options", "nosniff")
                .Add("X-XSS-Protection", "1; mode=block")
                .Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains")
                .Add("Referrer-Policy", "strict-origin-when-cross-origin")
                .Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self'")
                .Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()")
            End With
        End Sub

        ' Enhanced input validation
        Private Function ValidateInput() As Boolean
            ' Username validation
            If String.IsNullOrWhiteSpace(uname.Value) Then
                errormessage = "Please enter a username."
                foc = "uname"
                Return False
            End If
            
            If Not SecurityHelper.ValidateInput(uname.Value, "username") Then
                errormessage = "Invalid username format."
                foc = "uname"
                LogSecurityEvent("Invalid username format: " & uname.Value, Request.UserHostAddress)
                Return False
            End If
            
            ' Password validation
            If String.IsNullOrWhiteSpace(password.Value) Then
                errormessage = "Please enter a password."
                foc = "password"
                Return False
            End If
            
            If password.Value.Length > 100 Then
                errormessage = "Password too long."
                foc = "password"
                Return False
            End If
            
            ' Check for dangerous patterns
            If SecurityHelper.ContainsDangerousPatterns(uname.Value) OrElse SecurityHelper.ContainsDangerousPatterns(password.Value) Then
                errormessage = "Invalid characters detected."
                LogSecurityEvent("Dangerous patterns in login attempt", Request.UserHostAddress)
                Return False
            End If
            
            Return True
        End Function

        ' Secure authentication with password hashing
        Private Function AuthenticateUser(username As String, password As String) As Boolean
            Try
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim query As String = "SELECT pwd, role, userid, username, userslist, access, timestamp, usertype, remark, dbip, companyname, customrole FROM userTBL WHERE username = @username AND drcaccess = '0'"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@username", username)
                        
                        conn.Open()
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            If dr.Read() Then
                                Dim storedPassword As String = dr("pwd").ToString()
                                Dim hashedPassword As String = SecurityHelper.HashPassword(password)
                                
                                ' For backward compatibility, check both hashed and plain text
                                Dim isValidPassword As Boolean = (storedPassword = hashedPassword) OrElse (storedPassword = password)
                                
                                If isValidPassword Then
                                    ' Check account status
                                    Dim access As Byte = CByte(dr("access"))
                                    If Not CheckAccountAccess(access, dr) Then
                                        Return False
                                    End If
                                    
                                    ' Create secure session
                                    CreateSecureSession(dr)
                                    
                                    ' Log successful login
                                    LogUserLogin(dr("userid").ToString(), username)
                                    
                                    Return True
                                End If
                            End If
                        End Using
                    End Using
                End Using
                
                Return False
                
            Catch ex As Exception
                SecurityHelper.LogError("Authentication error", ex, Server)
                Return False
            End Try
        End Function

        ' Create secure session
        Private Sub CreateSecureSession(dr As SqlDataReader)
            Try
                ' Clear any existing session
                Session.Clear()
                
                ' Set session values with security
                Session("authenticated") = True
                Session("userid") = dr("userid").ToString()
                Session("username") = dr("username").ToString()
                Session("role") = dr("role").ToString()
                Session("usertype") = dr("usertype").ToString()
                Session("companyname") = dr("companyname").ToString()
                Session("loginTime") = DateTime.Now
                Session("lastActivity") = DateTime.Now
                Session("ipAddress") = Request.UserHostAddress
                Session("userAgent") = Request.UserAgent
                Session.Timeout = SESSION_TIMEOUT_MINUTES
                
                ' Process users list securely
                Dim userslist As String = ProcessUsersList(dr("userslist").ToString())
                Session("userslist") = userslist
                Session("customrole") = dr("customrole").ToString()
                
                ' Set special privileges
                Dim userId As String = dr("userid").ToString()
                If {"6941", "3342", "439", "742", "1967", "2029", "2041", "2068", "3107", "3352", "8100"}.Contains(userId) Then
                    Session("LA") = "Y"
                Else
                    Session("LA") = "N"
                End If
                
            Catch ex As Exception
                SecurityHelper.LogError("Session creation failed", ex, Server)
                Throw New SecurityException("Session creation failed")
            End Try
        End Sub

        ' Process users list with validation
        Private Function ProcessUsersList(usersList As String) As String
            Try
                If String.IsNullOrEmpty(usersList) Then
                    Return ""
                End If
                
                Dim users() As String = usersList.Split(","c)
                Dim validUsers As New List(Of String)
                
                For Each user As String In users
                    Dim userId As String = user.Trim()
                    If SecurityHelper.ValidateUserId(userId) Then
                        validUsers.Add($"'{userId}'")
                    End If
                Next
                
                Return String.Join(",", validUsers)
                
            Catch ex As Exception
                SecurityHelper.LogError("Users list processing failed", ex, Server)
                Return ""
            End Try
        End Function

        Private Function CheckAccountAccess(access As Byte, row As SqlDataReader) As Boolean
            Select Case access
                Case 1
                    errormessage = If(IsDBNull(row("remark")), "Dear Customer, Your account is overdue. Kindly remit the total amount due immediately.", "Dear Customer, " & row("remark").ToString())
                    Return False
                    
                Case 2, 3, 4
                    Dim accessdays() As SByte = {0, -1, 7, 14, 31}
                    Dim denydatetime As DateTime = DateTime.Parse(row("timestamp").ToString())
                    Dim temptime As TimeSpan = DateTime.Now - denydatetime
                    
                    If temptime.TotalDays > accessdays(access) Then
                        errormessage = If(IsDBNull(row("remark")), "Dear Customer, Your account is overdue. Kindly remit the total amount due immediately.", "Dear Customer, " & row("remark").ToString())
                        Return False
                    Else
                        Session("accountWarning") = True
                        Session("warningMessage") = If(IsDBNull(row("remark")), "Dear Customer, Your account is overdue. Kindly remit the total amount due immediately.", "Dear Customer, " & row("remark").ToString())
                    End If
            End Select
            
            Return True
        End Function

        ' Enhanced user login logging
        Private Sub LogUserLogin(userid As String, username As String)
            Try
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim query As String = "INSERT INTO user_log(userid,logintime,logouttime,hostaddress,browser,applicationversion,url,status,width,height,lat,lon,acc) VALUES(@userid,@logintime,@logintime,@hostaddress,@browser,@appversion,@url,1,@width,@height,@lat,@lon,@acc)"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@userid", userid)
                        cmd.Parameters.AddWithValue("@logintime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"))
                        cmd.Parameters.AddWithValue("@hostaddress", Request.UserHostAddress)
                        cmd.Parameters.AddWithValue("@browser", Request.Browser.Browser & " " & Request.Browser.Version)
                        cmd.Parameters.AddWithValue("@appversion", "YTL AVLS Secure v2.0")
                        cmd.Parameters.AddWithValue("@url", Request.Url.ToString())
                        cmd.Parameters.AddWithValue("@width", If(IsNumeric(Request.Form("w")), Request.Form("w"), 0))
                        cmd.Parameters.AddWithValue("@height", If(IsNumeric(Request.Form("h")), Request.Form("h"), 0))
                        cmd.Parameters.AddWithValue("@lat", If(IsNumeric(Request.Form("lat")), Request.Form("lat"), 0))
                        cmd.Parameters.AddWithValue("@lon", If(IsNumeric(Request.Form("lon")), Request.Form("lon"), 0))
                        cmd.Parameters.AddWithValue("@acc", If(IsNumeric(Request.Form("acc")), Request.Form("acc"), 0))
                        
                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
                
            Catch ex As Exception
                SecurityHelper.LogError("Login logging failed", ex, Server)
            End Try
        End Sub

        Private Sub RedirectToMainPage()
            Response.Redirect("Main.aspx?n=" & Server.UrlEncode(Session("username").ToString()))
        End Sub

        ' Clear existing sessions
        Private Sub ClearExistingSessions()
            Try
                Session.Clear()
                Session.Abandon()
                
                ' Clear cookies
                If Request.Cookies("userinfo") IsNot Nothing Then
                    Dim userinfoCookie As New HttpCookie("userinfo") With {
                        .Expires = DateTime.Now.AddDays(-1),
                        .Value = "",
                        .HttpOnly = True,
                        .Secure = Request.IsSecureConnection
                    }
                    Response.Cookies.Add(userinfoCookie)
                End If
                
                If Request.Cookies("accesslevel") IsNot Nothing Then
                    Dim accessCookie As New HttpCookie("accesslevel") With {
                        .Expires = DateTime.Now.AddDays(-1),
                        .Value = "",
                        .HttpOnly = True,
                        .Secure = Request.IsSecureConnection
                    }
                    Response.Cookies.Add(accessCookie)
                End If
                
            Catch ex As Exception
                SecurityHelper.LogError("Session clearing failed", ex, Server)
            End Try
        End Sub

        ' Account lockout mechanism
        Private Function IsAccountLocked(username As String) As Boolean
            Try
                Dim cacheKey As String = "FailedAttempts_" & username
                Dim attempts As Integer = 0
                
                If HttpContext.Current.Cache(cacheKey) IsNot Nothing Then
                    attempts = CInt(HttpContext.Current.Cache(cacheKey))
                End If
                
                Return attempts >= MAX_LOGIN_ATTEMPTS
                
            Catch ex As Exception
                SecurityHelper.LogError("Account lock check failed", ex, Server)
                Return False
            End Try
        End Function

        Private Sub RecordFailedAttempt(username As String)
            Try
                Dim cacheKey As String = "FailedAttempts_" & username
                Dim attempts As Integer = 0
                
                If HttpContext.Current.Cache(cacheKey) IsNot Nothing Then
                    attempts = CInt(HttpContext.Current.Cache(cacheKey))
                End If
                
                attempts += 1
                
                ' Cache for lockout duration
                HttpContext.Current.Cache.Insert(cacheKey, attempts, Nothing, DateTime.Now.AddMinutes(LOCKOUT_DURATION_MINUTES), TimeSpan.Zero)
                
                LogSecurityEvent($"Failed login attempt #{attempts} for user: {username}", Request.UserHostAddress)
                
            Catch ex As Exception
                SecurityHelper.LogError("Failed attempt recording failed", ex, Server)
            End Try
        End Sub

        Private Sub ClearFailedAttempts(username As String)
            Try
                Dim cacheKey As String = "FailedAttempts_" & username
                HttpContext.Current.Cache.Remove(cacheKey)
            Catch ex As Exception
                SecurityHelper.LogError("Clear failed attempts failed", ex, Server)
            End Try
        End Sub

        ' Rate limiting
        Private Function IsRateLimited() As Boolean
            Try
                Dim clientIP As String = Request.UserHostAddress
                Dim cacheKey As String = "RateLimit_" & clientIP
                Dim requestCount As Integer = 0
                
                If HttpContext.Current.Cache(cacheKey) IsNot Nothing Then
                    requestCount = CInt(HttpContext.Current.Cache(cacheKey))
                End If
                
                If requestCount = 0 Then
                    HttpContext.Current.Cache.Insert(cacheKey, 1, Nothing, DateTime.Now.AddMinutes(1), TimeSpan.Zero)
                    Return False
                Else
                    requestCount += 1
                    HttpContext.Current.Cache.Insert(cacheKey, requestCount, Nothing, DateTime.Now.AddMinutes(1), TimeSpan.Zero)
                    Return requestCount > 10 ' 10 requests per minute
                End If
                
            Catch ex As Exception
                SecurityHelper.LogError("Rate limit check failed", ex, Server)
                Return False
            End Try
        End Function

        Private Sub ClearRateLimit()
            Try
                Dim clientIP As String = Request.UserHostAddress
                Dim cacheKey As String = "RateLimit_" & clientIP
                HttpContext.Current.Cache.Remove(cacheKey)
            Catch ex As Exception
                SecurityHelper.LogError("Clear rate limit failed", ex, Server)
            End Try
        End Sub

        ' Security event logging
        Private Sub LogSecurityEvent(message As String, ipAddress As String)
            Try
                SecurityHelper.LogSecurityEvent("LOGIN_SECURITY", message, "")
            Catch
                ' Fail silently
            End Try
        End Sub
    End Class
End Namespace