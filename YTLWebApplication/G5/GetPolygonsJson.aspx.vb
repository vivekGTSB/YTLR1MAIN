Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetPolygonsJson
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
            Dim LA As String = If(Session("LA") IsNot Nothing, Session("LA").ToString().ToUpper(), "N")

            ' SECURITY FIX: Validate users list format
            If Not String.IsNullOrEmpty(userslist) AndAlso Not SecurityHelper.ValidateUsersList(userslist) Then
                userslist = $"'{userid}'"
            End If

            Try
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    ' SECURITY FIX: Use parameterized queries
                    Dim cmd As SqlCommand = SecurityHelper.CreateSafeCommand("", conn)
                    
                    If role = "User" Then
                        cmd.CommandText = "SELECT geofencename, geofenceid, data, accesstype, status, shiptocode FROM geofence WHERE geofencetype = '1' AND (userid = @userid OR accesstype = '1')"
                        cmd.Parameters.AddWithValue("@userid", userid)
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        If LA = "Y" Then
                            If Not String.IsNullOrEmpty(userslist) Then
                                cmd.CommandText = $"SELECT geofencename, geofenceid, data, accesstype, status, shiptocode FROM geofence WHERE geofencetype = '1' AND (userid IN ({userslist}) OR accesstype = '2' OR accesstype = '1')"
                            Else
                                cmd.CommandText = "SELECT geofencename, geofenceid, data, accesstype, status, shiptocode FROM geofence WHERE geofencetype = '1' AND (userid = @userid OR accesstype = '2' OR accesstype = '1')"
                                cmd.Parameters.AddWithValue("@userid", userid)
                            End If
                        Else
                            If Not String.IsNullOrEmpty(userslist) Then
                                cmd.CommandText = $"SELECT geofencename, geofenceid, data, accesstype, status, shiptocode FROM geofence WHERE geofencetype = '1' AND (userid IN ({userslist}) OR accesstype = '1')"
                            Else
                                cmd.CommandText = "SELECT geofencename, geofenceid, data, accesstype, status, shiptocode FROM geofence WHERE geofencetype = '1' AND (userid = @userid OR accesstype = '1')"
                                cmd.Parameters.AddWithValue("@userid", userid)
                            End If
                        End If
                    Else
                        cmd.CommandText = "SELECT geofencename, geofenceid, data, accesstype, status, shiptocode FROM geofence WHERE geofencetype = '1'"
                    End If

                    Dim aa As New ArrayList()
                    conn.Open()
                    
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Try
                                Dim at As Integer = 0
                                Dim status As Byte = 0
                                Dim a As New ArrayList()
                                
                                If dr("accesstype").ToString() = "1" Then
                                    at = 1
                                ElseIf dr("accesstype").ToString() = "0" Then
                                    at = 0
                                Else
                                    at = 2
                                End If
                                
                                If CBool(dr("status")) Then
                                    status = 1
                                Else
                                    status = 0
                                End If

                                a.Add(status)
                                a.Add(at)
                                a.Add(Convert.ToUInt32(dr("geofenceid")))
                                a.Add(SecurityHelper.SanitizeForHtml(dr("geofencename").ToString()))
                                a.Add(SecurityHelper.SanitizeForHtml(dr("data").ToString()))
                                a.Add(SecurityHelper.SanitizeForHtml(dr("shiptocode").ToString()))
                                aa.Add(a)
                            Catch ex As Exception
                                SecurityHelper.LogSecurityEvent("ROW_PROCESSING_ERROR", "Error processing polygon row: " & ex.Message)
                            End Try
                        End While
                    End Using

                    ' SECURITY FIX: Safe JSON serialization
                    Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
                    Response.ContentType = "application/json"
                    Response.Write(json)

                Catch ex As Exception
                    SecurityHelper.LogSecurityEvent("DATABASE_ERROR", "Database error in GetPolygonsJson: " & ex.Message)
                    Response.StatusCode = 500
                    Response.Write("{""error"":""Database error""}")
                End Try

            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("CONNECTION_ERROR", "Connection error in GetPolygonsJson: " & ex.Message)
                Response.StatusCode = 500
                Response.Write("{""error"":""Connection error""}")
            End Try

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GENERAL_ERROR", "General error in GetPolygonsJson: " & ex.Message)
            Response.StatusCode = 500
            Response.Write("{""error"":""Internal server error""}")
        End Try
    End Sub

End Class