Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetClientSoldToManagement
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

            ' SECURITY FIX: Check authorization
            If Not AuthenticationHelper.HasRole("Admin") AndAlso Not AuthenticationHelper.HasRole("SuperUser") Then
                Response.StatusCode = 403
                Response.Write("Access denied")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate operation parameter
            Dim operation As String = ValidateInput(Request.QueryString("op"), "operation")
            
            Select Case operation
                Case "0"
                    Response.Write(GetSecureClientData())
                Case "1"
                    Response.Write(ProcessSecureClientUpdate())
                Case Else
                    Response.StatusCode = 400
                    Response.Write("Invalid operation")
            End Select

        Catch ex As Exception
            SecurityHelper.LogError("GetClientSoldToManagement error", ex, Server)
            Response.StatusCode = 500
            Response.Write("{""error"":""An error occurred""}")
        End Try
    End Sub

    ' SECURITY FIX: Secure client data retrieval
    Private Function GetSecureClientData() As String
        Try
            Dim query As String = "SELECT cu.cuserid, cu.CUsername, cu.pwd, " &
                                 "ISNULL(cu.emaillist, '-') as emaillist, " &
                                 "ISNULL(cu.mobileList, '-') as mobileList, " &
                                 "ISNULL(es.customername, '-') as soldto, " &
                                 "cu.status, cu.soldtoid " &
                                 "FROM EC_client_user cu " &
                                 "LEFT OUTER JOIN ec_soldto es ON cu.soldtoid = es.customerid " &
                                 "WHERE cu.soldtoid <> '0' AND cu.active = 1"
            
            Dim dataTable As DataTable = DatabaseHelper.ExecuteQuery(query, New Dictionary(Of String, Object))
            Dim resultList As New List(Of List(Of String))
            
            If dataTable.Rows.Count = 0 Then
                resultList.Add(New List(Of String) From {"-", "-", "-", "-", "-", "-", "-", "-", "-"})
            Else
                Dim i As Integer = 1
                For Each row As DataRow In dataTable.Rows
                    Dim rowData As New List(Of String) From {
                        i.ToString(),
                        i.ToString(),
                        SecurityHelper.SanitizeForHtml(row("CUsername").ToString()),
                        "****", ' Don't expose passwords
                        SecurityHelper.SanitizeForHtml(row("emaillist").ToString()),
                        SecurityHelper.SanitizeForHtml(row("mobileList").ToString()),
                        SecurityHelper.SanitizeForHtml(row("soldto").ToString()),
                        SecurityHelper.SanitizeForHtml(row("status").ToString()),
                        SecurityHelper.SanitizeForHtml(row("cuserid").ToString()),
                        SecurityHelper.SanitizeForHtml(row("soldtoid").ToString())
                    }
                    resultList.Add(rowData)
                    i += 1
                Next
            End If
            
            Return JsonConvert.SerializeObject(resultList)
            
        Catch ex As Exception
            SecurityHelper.LogError("GetSecureClientData failed", ex, Server)
            Return "{""error"":""Data retrieval failed""}"
        End Try
    End Function

    ' SECURITY FIX: Secure client update processing
    Private Function ProcessSecureClientUpdate() As String
        Try
            ' SECURITY FIX: Validate CSRF token
            If Not SecurityHelper.ValidateCSRFToken(Request.Form("__CSRFToken")) Then
                Return "0" ' Invalid request
            End If

            Dim id As String = ValidateInput(Request.QueryString("prevpid"), "id")
            Dim username As String = ValidateInput(Request.QueryString("cid"), "username")
            Dim password As String = ValidateInput(Request.QueryString("cpwd"), "password")
            Dim email As String = ValidateInput(Request.QueryString("email"), "email")
            Dim mobile As String = ValidateInput(Request.QueryString("mobile"), "mobile")
            Dim soldto As String = ValidateInput(Request.QueryString("soldto"), "soldto")
            
            ' SECURITY FIX: Validate all required fields
            If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
                Return "0" ' Invalid input
            End If
            
            ' SECURITY FIX: Hash password before storing
            Dim hashedPassword As String = PasswordHelper.HashPassword(password)
            
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@username", username},
                {"@password", hashedPassword},
                {"@email", email},
                {"@mobile", mobile},
                {"@soldto", soldto}
            }
            
            Dim query As String
            If id = "0" Then
                ' Check if username already exists
                Dim checkQuery As String = "SELECT COUNT(*) FROM EC_client_user WHERE CUsername = @username"
                Dim existingCount As Integer = CInt(DatabaseHelper.ExecuteScalar(checkQuery, New Dictionary(Of String, Object) From {{"@username", username}}))
                
                If existingCount > 0 Then
                    Return "2" ' Username already exists
                End If
                
                query = "INSERT INTO EC_client_user (CUsername, pwd, emaillist, mobileList, soldtoid, created_date, active) " &
                       "VALUES (@username, @password, @email, @mobile, @soldto, GETDATE(), 1)"
            Else
                parameters.Add("@id", id)
                query = "UPDATE EC_client_user SET CUsername = @username, pwd = @password, " &
                       "emaillist = @email, mobileList = @mobile, soldtoid = @soldto, " &
                       "modified_date = GETDATE() WHERE cuserid = @id"
            End If
            
            Dim rowsAffected As Integer = DatabaseHelper.ExecuteNonQuery(query, parameters)
            
            If rowsAffected > 0 Then
                SecurityHelper.LogSecurityEvent("CLIENT_MANAGEMENT", $"Client {username} {If(id = "0", "created", "updated")}")
                Return "1" ' Success
            Else
                Return "0" ' Failed
            End If
            
        Catch ex As Exception
            SecurityHelper.LogError("ProcessSecureClientUpdate failed", ex, Server)
            Return "0" ' Error
        End Try
    End Function

    ' SECURITY FIX: Enhanced input validation
    Private Function ValidateInput(input As String, inputType As String) As String
        If String.IsNullOrWhiteSpace(input) Then
            Return ""
        End If

        input = SecurityHelper.SanitizeForHtml(input.Trim())
        
        Select Case inputType.ToLower()
            Case "operation"
                If Not Regex.IsMatch(input, "^[0-9]$") Then
                    Return ""
                End If
                
            Case "id"
                If Not Regex.IsMatch(input, "^[0-9]+$") Then
                    Return ""
                End If
                
            Case "username"
                If input.Length > 50 OrElse Not Regex.IsMatch(input, "^[a-zA-Z0-9_@.-]+$") Then
                    Return ""
                End If
                
            Case "password"
                If input.Length < 8 OrElse input.Length > 100 Then
                    Return ""
                End If
                
            Case "email"
                If Not Regex.IsMatch(input, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$") Then
                    Return ""
                End If
                
            Case "mobile"
                If Not Regex.IsMatch(input, "^[0-9+\-\s()]+$") Then
                    Return ""
                End If
                
            Case "soldto"
                If Not Regex.IsMatch(input, "^[0-9]+$") Then
                    Return ""
                End If
        End Select
        
        Return input
    End Function
End Class