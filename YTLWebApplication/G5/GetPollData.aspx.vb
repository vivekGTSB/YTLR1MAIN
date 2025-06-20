Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Collections

Partial Class GetPollData
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
            Dim plateno As String = SecurityHelper.SanitizeForHtml(Request.QueryString("plateno"))
            If String.IsNullOrEmpty(plateno) Then
                Response.StatusCode = 400
                Response.Write("{""error"":""Invalid plate number""}")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate plate number format
            If Not SecurityHelper.ValidatePlateNumber(plateno) Then
                Response.StatusCode = 400
                Response.Write("{""error"":""Invalid plate number format""}")
                Response.End()
                Return
            End If

            plateno = plateno.Trim()
            Dim aa As New ArrayList()
            Dim t As New DataTable()

            t.Columns.Add(New DataColumn("DateTime"))
            t.Columns.Add(New DataColumn("Message"))
            t.Columns.Add(New DataColumn("MobileNo"))
            t.Columns.Add(New DataColumn("Status"))

            ' SECURITY FIX: Validate date range
            Dim edt As DateTime = DateTime.Now
            Dim bdt As DateTime = edt.AddHours(-24)

            Try
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    ' SECURITY FIX: Use parameterized queries for inbox data
                    Dim cmd As SqlCommand = SecurityHelper.CreateSafeCommand(
                        "SELECT datetime, message, mobileno FROM sms_inbox WHERE plateno = @plateno AND datetime BETWEEN @bdt AND @edt ORDER BY datetime",
                        conn)
                    cmd.Parameters.AddWithValue("@plateno", plateno)
                    cmd.Parameters.AddWithValue("@bdt", bdt.ToString("yyyy/MM/dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@edt", edt.ToString("yyyy/MM/dd HH:mm:ss"))

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim r As DataRow = t.NewRow()
                            r(0) = DateTime.Parse(dr("datetime").ToString()).ToString("yyyy/MM/dd HH:mm:ss")
                            r(1) = SecurityHelper.SanitizeForHtml(dr("message").ToString())
                            r(2) = SecurityHelper.SanitizeForHtml(dr("mobileno").ToString())
                            r(3) = 0
                            t.Rows.Add(r)
                        End While
                    End Using

                    ' SECURITY FIX: Use parameterized queries for outbox data
                    cmd = SecurityHelper.CreateSafeCommand(
                        "SELECT datetime, message, mobileno FROM sms_outbox WHERE plateno = @plateno AND datetime BETWEEN @bdt AND @edt ORDER BY datetime",
                        conn)
                    cmd.Parameters.AddWithValue("@plateno", plateno)
                    cmd.Parameters.AddWithValue("@bdt", bdt.ToString("yyyy/MM/dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@edt", edt.ToString("yyyy/MM/dd HH:mm:ss"))

                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim r As DataRow = t.NewRow()
                            r(0) = DateTime.Parse(dr("datetime").ToString()).ToString("yyyy/MM/dd HH:mm:ss")
                            r(1) = SecurityHelper.SanitizeForHtml(dr("message").ToString())
                            r(2) = SecurityHelper.SanitizeForHtml(dr("mobileno").ToString())
                            r(3) = 1
                            t.Rows.Add(r)
                        End While
                    End Using
                End Using

                ' Process results
                For c As Integer = 0 To t.Rows.Count - 1
                    Dim a As New ArrayList()
                    a.Add(c + 1)
                    a.Add(t.DefaultView.Item(c)(0))
                    a.Add(t.DefaultView.Item(c)(2))
                    
                    Dim message As String = t.DefaultView.Item(c)(1).ToString()
                    If message.StartsWith("CGUS") Then
                        message = "Sent Polling Command"
                    End If
                    a.Add(message)
                    a.Add(t.DefaultView.Item(c)(3))
                    aa.Add(a)
                Next

                If t.Rows.Count = 0 Then
                    Dim a As New ArrayList()
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    aa.Add(a)
                End If

                ' SECURITY FIX: Safe JSON response
                Dim json As String = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
                Response.ContentType = "application/json"
                Response.Write(json)

            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("DATABASE_ERROR", "Error in GetPollData: " & ex.Message)
                Response.StatusCode = 500
                Response.Write("{""error"":""Database error""}")
            End Try

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GENERAL_ERROR", "Error in GetPollData: " & ex.Message)
            Response.StatusCode = 500
            Response.Write("{""error"":""Internal server error""}")
        End Try
    End Sub

End Class