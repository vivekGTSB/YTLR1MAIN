Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetAlertInbox
    Inherits SecurePageBase

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.Write("Unauthorized")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate date parameters
            Dim beginDate As String = ValidateDate(Request.QueryString("b"))
            Dim endDate As String = ValidateDate(Request.QueryString("e"))
            
            If String.IsNullOrEmpty(beginDate) OrElse String.IsNullOrEmpty(endDate) Then
                Response.StatusCode = 400
                Response.Write("Invalid date parameters")
                Response.End()
                Return
            End If

            Response.Write(GetSecureAlertData(beginDate, endDate))
            Response.ContentType = "application/json"
            
        Catch ex As Exception
            SecurityHelper.LogError("GetAlertInbox error", ex, Server)
            Response.StatusCode = 500
            Response.Write("{""error"":""An error occurred""}")
        End Try
    End Sub

    ' SECURITY FIX: Secure alert data retrieval
    Private Function GetSecureAlertData(beginDate As String, endDate As String) As String
        Try
            Dim userId As String = SessionManager.GetCurrentUserId()
            Dim userRole As String = SessionManager.GetCurrentUserRole()
            
            ' SECURITY FIX: Role-based data access
            Dim query As String
            Dim parameters As New Dictionary(Of String, Object)
            
            If userRole = "Admin" Then
                query = "SELECT source, sequence, data, username, mobileno, PhoneNo, Server, Action, " &
                       "CASE WHEN insertdate IS NULL THEN '-' ELSE CAST(DATEDIFF(MINUTE, insertdate, actiondate) AS VARCHAR) END as Responsetime " &
                       "FROM alert_inbox WHERE sequence BETWEEN @beginDate AND @endDate AND actiontaken = '1' ORDER BY sequence DESC"
            Else
                query = "SELECT ai.source, ai.sequence, ai.data, u.username, u.mobileno, u.phoneno as PhoneNo, 'Server' as Server, ai.Action, " &
                       "CASE WHEN ai.insertdate IS NULL THEN '-' ELSE CAST(DATEDIFF(MINUTE, ai.insertdate, ai.actiondate) AS VARCHAR) END as Responsetime " &
                       "FROM alert_inbox ai INNER JOIN userTBL u ON ai.userid = u.userid " &
                       "WHERE ai.sequence BETWEEN @beginDate AND @endDate AND ai.actiontaken = '1' AND ai.userid = @userId ORDER BY ai.sequence DESC"
                parameters.Add("@userId", userId)
            End If
            
            parameters.Add("@beginDate", beginDate & " 00:00:00")
            parameters.Add("@endDate", endDate & " 23:59:59")
            
            Dim dataTable As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)
            Dim resultList As New List(Of List(Of String))
            
            If dataTable.Rows.Count = 0 Then
                ' Add empty row if no data
                resultList.Add(New List(Of String) From {"--", "--", "--", "--", "--", "--", "--", "--", "--", "--"})
            Else
                For Each row As DataRow In dataTable.Rows
                    Dim rowData As New List(Of String) From {
                        "",
                        SecurityHelper.SanitizeForHtml(row("source").ToString()),
                        SecurityHelper.SanitizeForHtml(row("sequence").ToString()),
                        SecurityHelper.SanitizeForHtml(row("data").ToString()),
                        SecurityHelper.SanitizeForHtml(row("username").ToString()),
                        SecurityHelper.SanitizeForHtml(row("mobileno").ToString()),
                        SecurityHelper.SanitizeForHtml(row("PhoneNo").ToString()),
                        SecurityHelper.SanitizeForHtml(row("Server").ToString()),
                        SecurityHelper.SanitizeForHtml(row("Action").ToString()),
                        SecurityHelper.SanitizeForHtml(row("Responsetime").ToString())
                    }
                    resultList.Add(rowData)
                Next
            End If
            
            Return JsonConvert.SerializeObject(resultList)
            
        Catch ex As Exception
            SecurityHelper.LogError("GetSecureAlertData failed", ex, Server)
            Return "{""error"":""Data retrieval failed""}"
        End Try
    End Function

    ' SECURITY FIX: Date validation
    Private Function ValidateDate(dateString As String) As String
        If String.IsNullOrWhiteSpace(dateString) Then
            Return ""
        End If
        
        Dim dateValue As DateTime
        If Not DateTime.TryParse(dateString, dateValue) Then
            Return ""
        End If
        
        ' Ensure date is within reasonable range
        If dateValue < New DateTime(2000, 1, 1) OrElse dateValue > DateTime.Now.AddDays(1) Then
            Return ""
        End If
        
        Return dateValue.ToString("yyyy-MM-dd")
    End Function
End Class