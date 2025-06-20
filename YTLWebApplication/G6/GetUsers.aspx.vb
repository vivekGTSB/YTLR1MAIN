Imports Newtonsoft.Json
Imports System.Data.SqlClient

Partial Class GetUsers
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Authentication check
            If Not IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.Write("Unauthorized")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Add security headers
            AddSecurityHeaders()

            ' SECURITY FIX: Rate limiting
            If Not SecurityHelper.CheckRateLimit("GetUsers_" & GetClientIP(), 50, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.Write("Rate limit exceeded")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Get user data from session
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Dim json As String = GetUsersJson(userid, role, userslist)
            
            Response.Write(json)
            Response.ContentType = "application/json"

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GET_USERS_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Internal server error")
        End Try
    End Sub

    ' SECURITY FIX: Authentication check
    Private Function IsUserAuthenticated() As Boolean
        Try
            Return SecurityHelper.ValidateSession() AndAlso
                   HttpContext.Current.Session("userid") IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ' SECURITY FIX: Add security headers
    Private Sub AddSecurityHeaders()
        Response.Headers.Add("X-Content-Type-Options", "nosniff")
        Response.Headers.Add("X-Frame-Options", "DENY")
        Response.Headers.Add("X-XSS-Protection", "1; mode=block")
        Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate")
    End Sub

    ' SECURITY FIX: Get client IP safely
    Private Function GetClientIP() As String
        Try
            Dim ip As String = Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If String.IsNullOrEmpty(ip) Then
                ip = Request.ServerVariables("REMOTE_ADDR")
            End If
            Return ip
        Catch
            Return "Unknown"
        End Try
    End Function

    Private Function GetUsersJson(userid As String, role As String, userslist As String) As String
        Dim json As String = ""
        Try
            Dim aa As New ArrayList()
            
            ' SECURITY FIX: Build secure query based on role
            Dim query As String = BuildUsersQuery(role, userslist)
            Dim parameters As New Dictionary(Of String, Object)()
            
            ' Add parameters based on role
            If role = "User" Then
                parameters.Add("@userid", userid)
            End If

            Dim dataTable As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            For Each row As DataRow In dataTable.Rows
                Try
                    Dim a As New ArrayList()
                    a.Add(row("userid"))
                    a.Add(SecurityHelper.HtmlEncode(row("username").ToString().ToUpper()))
                    aa.Add(a)
                Catch ex As Exception
                    SecurityHelper.LogSecurityEvent("USER_ROW_ERROR", ex.Message)
                End Try
            Next

            json = JsonConvert.SerializeObject(aa, Formatting.None)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GET_USERS_JSON_ERROR", ex.Message)
            json = "[]"
        End Try

        Return json
    End Function

    ' SECURITY FIX: Build secure users query based on role
    Private Function BuildUsersQuery(role As String, userslist As String) As String
        Select Case role
            Case "User"
                Return "SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username"
            Case "SuperUser", "Operator"
                ' SECURITY FIX: Validate userslist before using in query
                If SecurityHelper.ValidateUsersList(userslist) Then
                    Return $"SELECT userid, username FROM userTBL WHERE userid IN ({userslist}) ORDER BY username"
                Else
                    ' Fallback to empty result if userslist is invalid
                    Return "SELECT userid, username FROM userTBL WHERE 1=0"
                End If
            Case "Admin"
                Return "SELECT userid, username FROM userTBL WHERE role = 'User' ORDER BY username"
            Case Else
                ' Default to empty result for unknown roles
                Return "SELECT userid, username FROM userTBL WHERE 1=0"
        End Select
    End Function

End Class