Imports Newtonsoft.Json
Imports System.Data.SqlClient

Partial Class GetPlates
    Inherits SecurePageBase

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.Write("{""error"":""Unauthorized""}")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Get user data from session instead of cookies
            Dim userid As String = SessionManager.GetCurrentUserId()
            Dim role As String = SessionManager.GetCurrentUserRole()
            Dim userslist As String = If(Session("userslist") IsNot Nothing, Session("userslist").ToString(), "")

            ' SECURITY FIX: Validate and sanitize query parameters
            Dim qs As String = SecurityHelper.SanitizeForHtml(Request.QueryString("userid"))
            Dim gid As String = SecurityHelper.SanitizeForHtml(Request.QueryString("groupid"))

            ' SECURITY FIX: Validate users list format
            If Not String.IsNullOrEmpty(userslist) AndAlso Not SecurityHelper.ValidateUsersList(userslist) Then
                userslist = $"'{userid}'"
            End If

            Try
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    ' SECURITY FIX: Use parameterized queries
                    Dim cmd As SqlCommand = SecurityHelper.CreateSafeCommand("", conn)
                    
                    If role = "User" Then
                        If gid <> "ALLGROUPS" Then
                            cmd.CommandText = "SELECT vt.plateno, vtt.ignition, vtt.speed, ISNULL(vt.pmid,'-') as pmid FROM (SELECT plateno, pmid FROM vehicleTBL WHERE groupid = @groupid) vt LEFT OUTER JOIN vehicle_tracked2 vtt ON vtt.plateno = vt.plateno"
                            cmd.Parameters.AddWithValue("@groupid", gid)
                        Else
                            cmd.CommandText = "SELECT vt.plateno, vtt.ignition, vtt.speed, ISNULL(vt.pmid,'-') as pmid FROM (SELECT plateno, pmid FROM vehicleTBL WHERE userid = @userid) vt LEFT OUTER JOIN vehicle_tracked2 vtt ON vtt.plateno = vt.plateno"
                            cmd.Parameters.AddWithValue("@userid", userid)
                        End If
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        If gid <> "ALLGROUPS" Then
                            cmd.CommandText = "SELECT vt.plateno, vtt.ignition, vtt.speed, ISNULL(vt.pmid,'-') as pmid FROM (SELECT plateno, pmid FROM vehicleTBL WHERE groupid = @groupid) vt LEFT OUTER JOIN vehicle_tracked2 vtt ON vtt.plateno = vt.plateno"
                            cmd.Parameters.AddWithValue("@groupid", gid)
                        Else
                            If qs <> "ALLUSERS" Then
                                cmd.CommandText = "SELECT vt.plateno, vtt.ignition, vtt.speed, ISNULL(vt.pmid,'-') as pmid FROM (SELECT plateno, pmid FROM vehicleTBL WHERE userid = @userid) vt LEFT OUTER JOIN vehicle_tracked2 vtt ON vtt.plateno = vt.plateno"
                                cmd.Parameters.AddWithValue("@userid", qs)
                            Else
                                If Not String.IsNullOrEmpty(userslist) Then
                                    cmd.CommandText = $"SELECT vt.plateno, vtt.ignition, vtt.speed, ISNULL(vt.pmid,'-') as pmid FROM (SELECT plateno, pmid FROM vehicleTBL WHERE userid IN ({userslist})) vt LEFT OUTER JOIN vehicle_tracked2 vtt ON vtt.plateno = vt.plateno"
                                Else
                                    cmd.CommandText = "SELECT vt.plateno, vtt.ignition, vtt.speed, ISNULL(vt.pmid,'-') as pmid FROM (SELECT plateno, pmid FROM vehicleTBL WHERE userid = @userid) vt LEFT OUTER JOIN vehicle_tracked2 vtt ON vtt.plateno = vt.plateno"
                                    cmd.Parameters.AddWithValue("@userid", userid)
                                End If
                            End If
                        End If
                    Else
                        If gid <> "ALLGROUPS" Then
                            cmd.CommandText = "SELECT vt.plateno, vtt.ignition, vtt.speed, ISNULL(vt.pmid,'-') as pmid FROM (SELECT plateno, pmid FROM vehicleTBL WHERE groupid = @groupid) vt LEFT OUTER JOIN vehicle_tracked2 vtt ON vtt.plateno = vt.plateno"
                            cmd.Parameters.AddWithValue("@groupid", gid)
                        Else
                            If qs <> "ALLUSERS" Then
                                cmd.CommandText = "SELECT vt.plateno, vtt.ignition, vtt.speed, ISNULL(vt.pmid,'-') as pmid FROM (SELECT plateno, pmid FROM vehicleTBL WHERE userid = @userid) vt LEFT OUTER JOIN vehicle_tracked2 vtt ON vtt.plateno = vt.plateno"
                                cmd.Parameters.AddWithValue("@userid", qs)
                            Else
                                cmd.CommandText = "SELECT vt.plateno, vtt.ignition, vtt.speed, ISNULL(vt.pmid,'-') as pmid FROM vehicleTBL vt LEFT OUTER JOIN vehicle_tracked2 vtt ON vtt.plateno = vt.plateno"
                            End If
                        End If
                    End If

                    Dim aa As New ArrayList()
                    conn.Open()
                    
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim i As Integer = 0
                        While dr.Read()
                            Try
                                i += 1
                                Dim a As New ArrayList()
                                Dim status As Integer = 0

                                a.Add(i)
                                a.Add(SecurityHelper.SanitizeForHtml(dr("plateno").ToString().ToUpper()))

                                If Not IsDBNull(dr("ignition")) Then
                                    If dr("ignition") Then
                                        status = 0
                                        If Not IsDBNull(dr("speed")) Then
                                            If dr("speed") > 0 Then
                                                status = 2
                                            Else
                                                status = 1
                                            End If
                                        Else
                                            status = 1
                                        End If
                                    End If
                                End If

                                a.Add(status)
                                a.Add(SecurityHelper.SanitizeForHtml(dr("pmid").ToString()))
                                aa.Add(a)
                            Catch ex As Exception
                                SecurityHelper.LogSecurityEvent("ROW_PROCESSING_ERROR", "Error processing row in GetPlates: " & ex.Message)
                            End Try
                        End While
                    End Using

                    ' SECURITY FIX: Safe JSON serialization
                    Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
                    Response.ContentType = "application/json"
                    Response.Write(json)

                Catch ex As Exception
                    SecurityHelper.LogSecurityEvent("DATABASE_ERROR", "Database error in GetPlates: " & ex.Message)
                    Response.StatusCode = 500
                    Response.Write("{""error"":""Database error""}")
                End Try

            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("CONNECTION_ERROR", "Connection error in GetPlates: " & ex.Message)
                Response.StatusCode = 500
                Response.Write("{""error"":""Connection error""}")
            End Try

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GENERAL_ERROR", "General error in GetPlates: " & ex.Message)
            Response.StatusCode = 500
            Response.Write("{""error"":""Internal server error""}")
        End Try
    End Sub
End Class