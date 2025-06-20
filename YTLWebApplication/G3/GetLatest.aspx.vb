Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetLatest
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate session
            If Not IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.Write("[]")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Get user data from session instead of cookies
            Dim userid As String = GetSessionValue("userid")
            Dim role As String = GetSessionValue("role")
            Dim userslist As String = GetSessionValue("userslist")
            
            If String.IsNullOrEmpty(userid) OrElse String.IsNullOrEmpty(role) Then
                Response.StatusCode = 401
                Response.Write("[]")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate and sanitize input
            Dim id As String = ValidateNumericInput(Request.QueryString("id"))
            If String.IsNullOrEmpty(id) Then
                id = "0"
            End If

            Dim result As String = GetAlertData(id, userid, role, userslist)
            Response.Write(result)
            Response.ContentType = "application/json"

        Catch ex As Exception
            LogSecurityEvent("GetLatest error", ex.Message)
            Response.Write("[]")
        End Try
    End Sub

    ' SECURITY FIX: Validate session authentication
    Private Function IsUserAuthenticated() As Boolean
        Try
            Return HttpContext.Current.Session("authenticated") IsNot Nothing AndAlso 
                   CBool(HttpContext.Current.Session("authenticated")) AndAlso
                   HttpContext.Current.Session("userid") IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ' SECURITY FIX: Get session value safely
    Private Function GetSessionValue(key As String) As String
        Try
            If HttpContext.Current.Session(key) IsNot Nothing Then
                Return HttpContext.Current.Session(key).ToString()
            End If
        Catch
        End Try
        Return ""
    End Function

    ' SECURITY FIX: Validate numeric input
    Private Function ValidateNumericInput(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then
            Return ""
        End If

        ' Only allow numeric characters
        If System.Text.RegularExpressions.Regex.IsMatch(input.Trim(), "^[0-9]{1,10}$") Then
            Return input.Trim()
        End If

        Return ""
    End Function

    ' SECURITY FIX: Get alert data with parameterized queries
    Private Function GetAlertData(id As String, userid As String, role As String, userslist As String) As String
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim aa As New ArrayList
        
        Try
            Dim condition As String = ""
            Dim cmd As SqlCommand
            
            ' SECURITY FIX: Use parameterized queries based on role
            If role = "User" Then
                Dim query As String = "select * from alert_notification where timestamp between @startTime and @endTime and userid = @userid and id > @id order by timestamp"
                cmd = New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                ' SECURITY FIX: Validate users list
                If ValidateUsersList(userslist) Then
                    Dim query As String = "select * from alert_notification where timestamp between @startTime and @endTime and userid in (" & userslist & ") and id > @id order by timestamp"
                    cmd = New SqlCommand(query, conn)
                Else
                    ' Fallback to single user if validation fails
                    Dim query As String = "select * from alert_notification where timestamp between @startTime and @endTime and userid = @userid and id > @id order by timestamp"
                    cmd = New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userid", userid)
                End If
            Else
                ' Default to single user for unknown roles
                Dim query As String = "select * from alert_notification where timestamp between @startTime and @endTime and userid = @userid and id > @id order by timestamp"
                cmd = New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@userid", userid)
            End If

            ' Add common parameters
            cmd.Parameters.AddWithValue("@startTime", DateTime.Now.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@endTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@id", id)

            conn.Open()
            Using dr As SqlDataReader = cmd.ExecuteReader()
                While dr.Read()
                    Try
                        Dim a As New ArrayList()
                        
                        ' SECURITY FIX: Validate and encode data
                        a.Add(ValidateNumericInput(dr("id").ToString()))
                        a.Add(HttpUtility.HtmlEncode(dr("plateno").ToString()))
                        
                        ' Calculate time difference safely
                        Dim diff As Integer = 0
                        Try
                            diff = CInt((DateTime.Now - DateTime.Parse(dr("timestamp").ToString())).TotalMinutes)
                        Catch
                            diff = 0
                        End Try
                        
                        Dim timestatus As String = ""
                        If diff > 0 Then
                            If diff = 1 Then
                                timestatus = diff & " min ago"
                            Else
                                timestatus = diff & " mins ago"
                            End If
                        Else
                            timestatus = "Just Now"
                        End If
                        a.Add(HttpUtility.HtmlEncode(timestatus))

                        ' SECURITY FIX: Validate alert type
                        Dim alertType As String = ValidateNumericInput(dr("alert_type").ToString())
                        Select Case alertType
                            Case "0"
                                a.Add("PTO ON")
                            Case "1"
                                a.Add("IMMOBILIZER")
                            Case "2"
                                a.Add("OVERSPEED")
                            Case "3"
                                a.Add("PANIC")
                            Case "4"
                                a.Add("POWERCUT")
                            Case "5"
                                a.Add("UNLOCK")
                            Case "6"
                                a.Add("IDLING")
                            Case "7"
                                a.Add("IGNITION OFF")
                            Case "8"
                                a.Add("IGNITION ON")
                            Case "9"
                                a.Add("OVERTIME")
                            Case "10"
                                a.Add("Geofence In")
                            Case "11"
                                a.Add("Geofence out")
                            Case Else
                                a.Add("Unknown")
                        End Select
                        
                        ' SECURITY FIX: Handle extra info safely
                        If alertType = "2" Then
                            a.Add(HttpUtility.HtmlEncode(dr("speed").ToString()))
                        Else
                            If Not IsDBNull(dr("extra_info")) Then
                                a.Add(HttpUtility.HtmlEncode(dr("extra_info").ToString()))
                            Else
                                a.Add("--")
                            End If
                        End If

                        aa.Add(a)
                    Catch ex As Exception
                        LogSecurityEvent("Alert data processing error", ex.Message)
                    End Try
                End While
            End Using

        Catch ex As Exception
            LogSecurityEvent("GetAlertData database error", ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

        Return JsonConvert.SerializeObject(aa, Formatting.None)
    End Function

    ' SECURITY FIX: Validate users list
    Private Function ValidateUsersList(usersList As String) As String
        If String.IsNullOrEmpty(usersList) Then
            Return False
        End If

        ' Check if it contains only numbers, commas, quotes and spaces
        If Not System.Text.RegularExpressions.Regex.IsMatch(usersList, "^[0-9,'\s]+$") Then
            Return False
        End If

        Return True
    End Function

    ' SECURITY FIX: Secure logging
    Private Sub LogSecurityEvent(eventType As String, message As String)
        Try
            Dim logMessage As String = String.Format("{0}: {1} - User: {2}, IP: {3}", 
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                eventType, 
                If(HttpContext.Current.Session("userid"), "Unknown"), 
                HttpContext.Current.Request.UserHostAddress)
            
            System.Diagnostics.EventLog.WriteEntry("YTL_Security", logMessage, System.Diagnostics.EventLogEntryType.Warning)
        Catch
            ' Fail silently
        End Try
    End Sub
End Class