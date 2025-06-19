Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Web.Security
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Public Class G2SecurityHelper
    
    ' Enhanced input validation for G2 modules
    Public Shared Function ValidateG2Input(input As String, inputType As G2InputType, Optional maxLength As Integer = 255) As Boolean
        If String.IsNullOrEmpty(input) Then
            Return False
        End If
        
        If input.Length > maxLength Then
            Return False
        End If
        
        ' Check for dangerous patterns
        If ContainsDangerousPatterns(input) Then
            LogSecurityEvent("DANGEROUS_PATTERN_DETECTED", $"Dangerous pattern found in input: {input.Substring(0, Math.Min(50, input.Length))}")
            Return False
        End If
        
        ' Type-specific validation
        Select Case inputType
            Case G2InputType.PlateNumber
                Return Regex.IsMatch(input, "^[A-Za-z0-9\-\s]{1,20}$")
            Case G2InputType.GeofenceId
                Dim id As Integer
                Return Integer.TryParse(input, id) AndAlso id > 0
            Case G2InputType.UserId
                Dim id As Integer
                Return Integer.TryParse(input, id) AndAlso id > 0
            Case G2InputType.DateTime
                Dim dateValue As DateTime
                Return DateTime.TryParse(input, dateValue)
            Case G2InputType.Email
                Return Regex.IsMatch(input, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
            Case G2InputType.PhoneNumber
                Return Regex.IsMatch(input, "^[\+]?[0-9\-\s\(\)]{7,20}$")
            Case G2InputType.AlphaNumeric
                Return Regex.IsMatch(input, "^[A-Za-z0-9\s]{1," & maxLength & "}$")
            Case Else
                Return True
        End Select
    End Function
    
    ' Detect dangerous patterns including SQL injection, XSS, and command injection
    Public Shared Function ContainsDangerousPatterns(input As String) As Boolean
        Dim dangerousPatterns() As String = {
            "(\%27)|(\')|(\-\-)|(\%23)|(#)",
            "((\%3D)|(=))[^\n]*((\%27)|(\')|(\-\-)|(\%3B)|(;))",
            "\w*((\%27)|(\'))((\%6F)|o|(\%4F))((\%72)|r|(\%52))",
            "((\%27)|(\'))union",
            "exec(\s|\+)+(s|x)p\w+",
            "<script[^>]*>.*?</script>",
            "javascript:",
            "vbscript:",
            "onload\s*=",
            "onerror\s*=",
            "onclick\s*=",
            "onmouseover\s*=",
            "eval\s*\(",
            "expression\s*\(",
            "url\s*\(",
            "xp_cmdshell",
            "sp_oacreate",
            "openrowset",
            "bulk\s+insert",
            "shutdown",
            "drop\s+table",
            "truncate\s+table",
            "\.\./",
            "\.\.\\",
            "cmd\.exe",
            "powershell",
            "/bin/sh",
            "/bin/bash"
        }

        For Each pattern As String In dangerousPatterns
            If Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase) Then
                Return True
            End If
        Next
        
        Return False
    End Function
    
    ' Create secure SQL command with parameters
    Public Shared Function CreateSecureCommand(query As String, connection As SqlConnection) As SqlCommand
        Dim cmd As New SqlCommand(query, connection)
        cmd.CommandTimeout = 30
        Return cmd
    End Function
    
    ' HTML encode output to prevent XSS
    Public Shared Function SanitizeForHtml(input As String) As String
        If String.IsNullOrEmpty(input) Then
            Return String.Empty
        End If
        
        Return HttpUtility.HtmlEncode(input)
    End Function
    
    ' JavaScript encode for embedding in JavaScript
    Public Shared Function SanitizeForJavaScript(input As String) As String
        If String.IsNullOrEmpty(input) Then
            Return String.Empty
        End If
        
        Return input.Replace("\", "\\") _
                   .Replace("'", "\'") _
                   .Replace("""", "\""") _
                   .Replace(vbCrLf, "\n") _
                   .Replace(vbCr, "\r") _
                   .Replace(vbTab, "\t") _
                   .Replace("<", "\u003c") _
                   .Replace(">", "\u003e")
    End Function
    
    ' Validate session and get user information
    Public Shared Function ValidateSession() As Boolean
        Try
            If HttpContext.Current.Session Is Nothing Then
                Return False
            End If
            
            ' Check if user is authenticated
            If HttpContext.Current.Session("authenticated") Is Nothing OrElse 
               Not CBool(HttpContext.Current.Session("authenticated")) Then
                Return False
            End If
            
            ' Check session timeout
            If HttpContext.Current.Session("lastActivity") IsNot Nothing Then
                Dim lastActivity As DateTime = CDate(HttpContext.Current.Session("lastActivity"))
                If DateTime.Now.Subtract(lastActivity).TotalMinutes > 30 Then
                    HttpContext.Current.Session.Clear()
                    Return False
                End If
            End If
            
            ' Update last activity
            HttpContext.Current.Session("lastActivity") = DateTime.Now
            
            Return True
        Catch ex As Exception
            LogSecurityEvent("SESSION_VALIDATION_ERROR", ex.Message)
            Return False
        End Try
    End Function
    
    ' Get current user ID safely
    Public Shared Function GetCurrentUserId() As String
        Try
            If ValidateSession() AndAlso HttpContext.Current.Session("userid") IsNot Nothing Then
                Return HttpContext.Current.Session("userid").ToString()
            End If
        Catch ex As Exception
            LogSecurityEvent("GET_USER_ID_ERROR", ex.Message)
        End Try
        Return Nothing
    End Function
    
    ' Get current user role safely
    Public Shared Function GetCurrentUserRole() As String
        Try
            If ValidateSession() AndAlso HttpContext.Current.Session("role") IsNot Nothing Then
                Return HttpContext.Current.Session("role").ToString()
            End If
        Catch ex As Exception
            LogSecurityEvent("GET_USER_ROLE_ERROR", ex.Message)
        End Try
        Return Nothing
    End Function
    
    ' Check if user has required role
    Public Shared Function HasRequiredRole(requiredRole As String) As Boolean
        Try
            Dim userRole As String = GetCurrentUserRole()
            If String.IsNullOrEmpty(userRole) Then
                Return False
            End If
            
            Select Case requiredRole.ToUpper()
                Case "ADMIN"
                    Return userRole.ToUpper() = "ADMIN"
                Case "SUPERUSER"
                    Return userRole.ToUpper() = "ADMIN" OrElse userRole.ToUpper() = "SUPERUSER"
                Case "OPERATOR"
                    Return userRole.ToUpper() = "ADMIN" OrElse userRole.ToUpper() = "SUPERUSER" OrElse userRole.ToUpper() = "OPERATOR"
                Case "USER"
                    Return True ' All authenticated users
                Case Else
                    Return False
            End Select
        Catch ex As Exception
            LogSecurityEvent("ROLE_CHECK_ERROR", ex.Message)
            Return False
        End Try
    End Function
    
    ' Generate CSRF token
    Public Shared Function GenerateCSRFToken() As String
        Try
            Dim token As String = Guid.NewGuid().ToString()
            HttpContext.Current.Session("CSRFToken") = token
            Return token
        Catch ex As Exception
            LogSecurityEvent("CSRF_TOKEN_GENERATION_ERROR", ex.Message)
            Return String.Empty
        End Try
    End Function
    
    ' Validate CSRF token
    Public Shared Function ValidateCSRFToken(submittedToken As String) As Boolean
        Try
            If String.IsNullOrEmpty(submittedToken) Then
                Return False
            End If
            
            Dim sessionToken As String = TryCast(HttpContext.Current.Session("CSRFToken"), String)
            If String.IsNullOrEmpty(sessionToken) Then
                Return False
            End If
            
            Return sessionToken.Equals(submittedToken, StringComparison.Ordinal)
        Catch ex As Exception
            LogSecurityEvent("CSRF_TOKEN_VALIDATION_ERROR", ex.Message)
            Return False
        End Try
    End Function
    
    ' Rate limiting check
    Public Shared Function CheckRateLimit(key As String, maxAttempts As Integer, timeWindow As TimeSpan) As Boolean
        Try
            Dim cacheKey As String = "RateLimit_" & key
            Dim attempts As Integer = 0
            
            If HttpContext.Current.Cache(cacheKey) IsNot Nothing Then
                attempts = CInt(HttpContext.Current.Cache(cacheKey))
            End If
            
            If attempts >= maxAttempts Then
                Return False
            End If
            
            attempts += 1
            HttpContext.Current.Cache.Insert(cacheKey, attempts, Nothing, DateTime.Now.Add(timeWindow), TimeSpan.Zero)
            
            Return True
        Catch ex As Exception
            LogSecurityEvent("RATE_LIMIT_ERROR", ex.Message)
            Return True ' Allow on error to prevent blocking legitimate users
        End Try
    End Function
    
    ' Security event logging
    Public Shared Sub LogSecurityEvent(eventType As String, details As String, Optional userId As String = "")
        Try
            If String.IsNullOrEmpty(userId) Then
                userId = GetCurrentUserId()
            End If
            
            Dim logMessage As String = String.Format("{0:yyyy-MM-dd HH:mm:ss} - {1} - User: {2} - IP: {3} - Details: {4}",
                DateTime.Now,
                eventType,
                If(String.IsNullOrEmpty(userId), "Anonymous", userId),
                HttpContext.Current.Request.UserHostAddress,
                details)
            
            ' Log to file
            Dim logPath As String = HttpContext.Current.Server.MapPath("~/Logs/SecurityLog.txt")
            Dim logDir As String = Path.GetDirectoryName(logPath)
            
            If Not Directory.Exists(logDir) Then
                Directory.CreateDirectory(logDir)
            End If
            
            SyncLock GetType(G2SecurityHelper)
                File.AppendAllText(logPath, logMessage & Environment.NewLine)
            End SyncLock
            
        Catch
            ' Fail silently to prevent information disclosure
        End Try
    End Sub
    
    ' Validate users list format
    Public Shared Function ValidateUsersList(usersList As String) As Boolean
        Try
            If String.IsNullOrEmpty(usersList) Then
                Return False
            End If
            
            ' Remove quotes and validate format
            usersList = usersList.Replace("'", "").Replace(" ", "")
            
            If Not Regex.IsMatch(usersList, "^[0-9,]+$") Then
                Return False
            End If
            
            ' Validate each user ID
            Dim users As String() = usersList.Split(","c)
            For Each user As String In users
                If Not String.IsNullOrEmpty(user) Then
                    Dim userId As Integer
                    If Not Integer.TryParse(user, userId) OrElse userId <= 0 Then
                        Return False
                    End If
                End If
            Next
            
            Return True
        Catch ex As Exception
            LogSecurityEvent("USERS_LIST_VALIDATION_ERROR", ex.Message)
            Return False
        End Try
    End Function
    
End Class

Public Enum G2InputType
    PlateNumber
    GeofenceId
    UserId
    DateTime
    Email
    PhoneNumber
    AlphaNumeric
    General
End Enum