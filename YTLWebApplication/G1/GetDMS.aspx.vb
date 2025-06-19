Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json

Partial Class GetDMS
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

            ' SECURITY FIX: Validate and sanitize input parameters
            Dim operation As String = ValidateInput(Request.QueryString("op"), "operation")
            Dim userId As String = SessionManager.GetCurrentUserId()
            
            If String.IsNullOrEmpty(userId) Then
                Response.StatusCode = 401
                Response.Write("Invalid session")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Process request based on operation
            Select Case operation
                Case "getData"
                    Response.Write(GetSecureData(userId))
                Case "updateData"
                    Response.Write(UpdateSecureData(userId))
                Case Else
                    Response.StatusCode = 400
                    Response.Write("Invalid operation")
            End Select

            Response.ContentType = "application/json"
            
        Catch ex As Exception
            SecurityHelper.LogError("GetDMS error", ex, Server)
            Response.StatusCode = 500
            Response.Write("{""error"":""An error occurred""}")
        End Try
    End Sub

    ' SECURITY FIX: Secure data retrieval with parameterized queries
    Private Function GetSecureData(userId As String) As String
        Try
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@userId", userId}
            }
            
            Dim query As String = "SELECT id, name, description, created_date FROM dms_data WHERE user_id = @userId ORDER BY created_date DESC"
            Dim dataTable As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)
            
            Dim resultList As New List(Of Object)
            
            For Each row As DataRow In dataTable.Rows
                resultList.Add(New With {
                    .id = SecurityHelper.SanitizeForHtml(row("id").ToString()),
                    .name = SecurityHelper.SanitizeForHtml(row("name").ToString()),
                    .description = SecurityHelper.SanitizeForHtml(row("description").ToString()),
                    .created_date = row("created_date").ToString()
                })
            Next
            
            Return JsonConvert.SerializeObject(resultList)
            
        Catch ex As Exception
            SecurityHelper.LogError("GetSecureData failed", ex, Server)
            Return "{""error"":""Data retrieval failed""}"
        End Try
    End Function

    ' SECURITY FIX: Secure data update with validation
    Private Function UpdateSecureData(userId As String) As String
        Try
            ' SECURITY FIX: Validate CSRF token
            If Not SecurityHelper.ValidateCSRFToken(Request.Form("__CSRFToken")) Then
                Return "{""error"":""Invalid request""}"
            End If

            Dim name As String = ValidateInput(Request.Form("name"), "name")
            Dim description As String = ValidateInput(Request.Form("description"), "description")
            
            If String.IsNullOrEmpty(name) OrElse String.IsNullOrEmpty(description) Then
                Return "{""error"":""Invalid input data""}"
            End If

            Dim parameters As New Dictionary(Of String, Object) From {
                {"@userId", userId},
                {"@name", name},
                {"@description", description},
                {"@createdDate", DateTime.Now}
            }
            
            Dim query As String = "INSERT INTO dms_data (user_id, name, description, created_date) VALUES (@userId, @name, @description, @createdDate)"
            Dim rowsAffected As Integer = DatabaseHelper.ExecuteNonQuery(query, parameters)
            
            If rowsAffected > 0 Then
                SecurityHelper.LogSecurityEvent("DMS_DATA_CREATED", $"User {userId} created DMS data: {name}")
                Return "{""success"":true,""message"":""Data updated successfully""}"
            Else
                Return "{""error"":""Update failed""}"
            End If
            
        Catch ex As Exception
            SecurityHelper.LogError("UpdateSecureData failed", ex, Server)
            Return "{""error"":""Update operation failed""}"
        End Try
    End Function

    ' SECURITY FIX: Enhanced input validation
    Private Function ValidateInput(input As String, inputType As String) As String
        If String.IsNullOrWhiteSpace(input) Then
            Return ""
        End If

        ' Remove dangerous characters
        input = SecurityHelper.SanitizeForHtml(input.Trim())
        
        ' Validate based on input type
        Select Case inputType.ToLower()
            Case "operation"
                If Not Regex.IsMatch(input, "^[a-zA-Z]+$") Then
                    Return ""
                End If
                
            Case "name"
                If input.Length > 100 OrElse Not Regex.IsMatch(input, "^[a-zA-Z0-9\s\-_]+$") Then
                    Return ""
                End If
                
            Case "description"
                If input.Length > 500 Then
                    input = input.Substring(0, 500)
                End If
        End Select
        
        Return input
    End Function
End Class