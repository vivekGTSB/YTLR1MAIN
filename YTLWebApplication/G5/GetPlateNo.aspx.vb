Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.DataRow
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class GetPlateNo
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

            ' SECURITY FIX: Validate and sanitize input
            Dim userId As String = SecurityHelper.SanitizeForHtml(Request.QueryString("userId"))
            If String.IsNullOrEmpty(userId) Then
                Response.StatusCode = 400
                Response.Write("{""error"":""Invalid user ID""}")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Get user data from session instead of cookies
            Dim sessionUserId As String = SessionManager.GetCurrentUserId()
            Dim role As String = SessionManager.GetCurrentUserRole()
            Dim userslist As String = If(Session("userslist") IsNot Nothing, Session("userslist").ToString(), "")

            ' SECURITY FIX: Validate users list format
            If Not String.IsNullOrEmpty(userslist) AndAlso Not SecurityHelper.ValidateUsersList(userslist) Then
                userslist = $"'{sessionUserId}'"
            End If

            Dim list As New ArrayList()

            Try
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    ' SECURITY FIX: Use parameterized queries
                    Dim query As String = ""
                    Dim cmd As SqlCommand = SecurityHelper.CreateSafeCommand("", conn)

                    If userId <> "--All Users--" Then
                        query = "SELECT plateno FROM vehicleTBL WHERE userid = @userid ORDER BY plateno"
                        cmd.CommandText = query
                        cmd.Parameters.AddWithValue("@userid", userId)
                    Else
                        If role = "SuperUser" Or role = "Operator" Then
                            If Not String.IsNullOrEmpty(userslist) Then
                                query = $"SELECT plateno FROM vehicleTBL WHERE userid IN ({userslist}) ORDER BY plateno"
                                cmd.CommandText = query
                            Else
                                query = "SELECT plateno FROM vehicleTBL WHERE userid = @userid ORDER BY plateno"
                                cmd.CommandText = query
                                cmd.Parameters.AddWithValue("@userid", sessionUserId)
                            End If
                        ElseIf role = "User" Then
                            query = "SELECT plateno FROM vehicleTBL WHERE userid = @userid ORDER BY plateno"
                            cmd.CommandText = query
                            cmd.Parameters.AddWithValue("@userid", sessionUserId)
                        Else
                            query = "SELECT plateno FROM vehicleTBL ORDER BY plateno"
                            cmd.CommandText = query
                        End If
                    End If

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim plateItem As New ArrayList()
                            plateItem.Add(SecurityHelper.SanitizeForHtml(dr("plateno").ToString().ToUpper()))
                            list.Add(plateItem)
                        End While
                    End Using
                End Using

            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("DATABASE_ERROR", "Error in GetPlateNo: " & ex.Message)
                Response.StatusCode = 500
                Response.Write("{""error"":""Database error""}")
                Response.End()
                Return
            End Try

            ' SECURITY FIX: Safe JSON serialization
            Dim json As String = JsonConvert.SerializeObject(list, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GENERAL_ERROR", "Error in GetPlateNo: " & ex.Message)
            Response.StatusCode = 500
            Response.Write("{""error"":""Internal server error""}")
        End Try
    End Sub

End Class