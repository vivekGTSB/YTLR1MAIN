Imports Newtonsoft.Json
Imports System.Data.SqlClient

Partial Class SecureGetGroups
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY: Validate session
            If Not G2SecurityHelper.ValidateSession() Then
                Response.StatusCode = 401
                Response.Write("Unauthorized")
                Response.End()
                Return
            End If
            
            ' SECURITY: Check authorization
            If Not G2SecurityHelper.HasRequiredRole("USER") Then
                Response.StatusCode = 403
                Response.Write("Forbidden")
                Response.End()
                Return
            End If
            
            ' SECURITY: Rate limiting
            Dim clientIP As String = Request.UserHostAddress
            If Not G2SecurityHelper.CheckRateLimit("GetGroups_" & clientIP, 60, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.Write("Too Many Requests")
                Response.End()
                Return
            End If
            
            Dim json As String = GetSecureGroups()
            Response.Write(json)
            Response.ContentType = "application/json"
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("GET_GROUPS_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Internal Server Error")
        End Try
    End Sub
    
    Private Function GetSecureGroups() As String
        Try
            ' SECURITY: Get user data from session
            Dim userid As String = G2SecurityHelper.GetCurrentUserId()
            Dim role As String = G2SecurityHelper.GetCurrentUserRole()
            
            If String.IsNullOrEmpty(userid) OrElse String.IsNullOrEmpty(role) Then
                Return JsonConvert.SerializeObject(New ArrayList())
            End If
            
            ' SECURITY: Validate userid parameter
            Dim qs As String = Request.QueryString("userid")
            If Not String.IsNullOrEmpty(qs) AndAlso Not G2SecurityHelper.ValidateG2Input(qs, G2InputType.AlphaNumeric, 50) Then
                Return JsonConvert.SerializeObject(New ArrayList())
            End If
            
            Dim userslist As String = HttpContext.Current.Session("userslist").ToString()
            If Not G2SecurityHelper.ValidateUsersList(userslist) Then
                userslist = userid ' Fallback to current user only
            End If
            
            Dim aa As New ArrayList()
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = BuildSecureGroupQuery(role, userid, qs, userslist)
                Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, conn)
                    
                    ' SECURITY: Add parameters based on role and query
                    AddGroupQueryParameters(cmd, role, userid, qs, userslist)
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Try
                                Dim a As New ArrayList()
                                a.Add(G2SecurityHelper.SanitizeForHtml(dr("groupid").ToString()))
                                a.Add(G2SecurityHelper.SanitizeForHtml(dr("groupname").ToString().ToUpper()))
                                aa.Add(a)
                            Catch ex As Exception
                                G2SecurityHelper.LogSecurityEvent("PROCESS_GROUP_RECORD_ERROR", ex.Message)
                                ' Continue with next record
                            End Try
                        End While
                    End Using
                End Using
            End Using
            
            Return JsonConvert.SerializeObject(aa, Formatting.None)
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("GET_SECURE_GROUPS_ERROR", ex.Message)
            Return JsonConvert.SerializeObject(New ArrayList())
        End Try
    End Function
    
    Private Function BuildSecureGroupQuery(role As String, userid As String, qs As String, userslist As String) As String
        Dim query As String = "SELECT groupid, groupname FROM vehicle_group"
        
        Select Case role.ToUpper()
            Case "USER"
                query &= " WHERE userid = @userid"
                
            Case "SUPERUSER", "OPERATOR"
                If Not String.IsNullOrEmpty(qs) AndAlso qs <> "ALLUSERS" Then
                    query &= " WHERE userid = @targetUserId"
                Else
                    query &= " WHERE userid IN (" & userslist & ")"
                End If
                
            Case "ADMIN"
                If Not String.IsNullOrEmpty(qs) AndAlso qs <> "ALLUSERS" Then
                    query &= " WHERE userid = @targetUserId"
                End If
                ' Admin can see all groups if no specific user requested
                
            Case Else
                ' Unknown role, restrict to current user
                query &= " WHERE userid = @userid"
        End Select
        
        query &= " ORDER BY groupname"
        Return query
    End Function
    
    Private Sub AddGroupQueryParameters(cmd As SqlCommand, role As String, userid As String, qs As String, userslist As String)
        Select Case role.ToUpper()
            Case "USER"
                cmd.Parameters.AddWithValue("@userid", userid)
                
            Case "SUPERUSER", "OPERATOR"
                If Not String.IsNullOrEmpty(qs) AndAlso qs <> "ALLUSERS" Then
                    cmd.Parameters.AddWithValue("@targetUserId", qs)
                End If
                ' userslist is already embedded in the query for IN clause
                
            Case "ADMIN"
                If Not String.IsNullOrEmpty(qs) AndAlso qs <> "ALLUSERS" Then
                    cmd.Parameters.AddWithValue("@targetUserId", qs)
                End If
                
            Case Else
                cmd.Parameters.AddWithValue("@userid", userid)
        End Select
    End Sub

End Class