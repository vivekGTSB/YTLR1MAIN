Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetChartVehicle
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

            ' SECURITY FIX: Validate user parameter
            Dim userIdParam As String = Request.QueryString("u")
            If Not SecurityHelper.ValidateUserId(userIdParam) Then
                Response.StatusCode = 400
                Response.Write("Invalid user parameter")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Authorization check
            Dim currentUserId As String = SessionManager.GetCurrentUserId()
            Dim currentUserRole As String = SessionManager.GetCurrentUserRole()
            
            If currentUserRole <> "Admin" AndAlso currentUserId <> userIdParam Then
                Response.StatusCode = 403
                Response.Write("Access denied")
                Response.End()
                Return
            End If

            Response.Write(GetSecureVehicleData(userIdParam))
            Response.ContentType = "application/json"
            
        Catch ex As Exception
            SecurityHelper.LogError("GetChartVehicle error", ex, Server)
            Response.StatusCode = 500
            Response.Write("{""error"":""An error occurred""}")
        End Try
    End Sub

    ' SECURITY FIX: Secure vehicle data retrieval
    Private Function GetSecureVehicleData(userId As String) As String
        Try
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@userId", userId}
            }
            
            Dim query As String = "SELECT CONCAT(UPPER(plateno), '-', CAST(pto AS VARCHAR)) as plateno, UPPER(plateno) as unitid " &
                                 "FROM vehicleTBL WHERE userid = @userId AND active = 1 ORDER BY plateno"
            
            Dim dataTable As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)
            Dim resultList As New List(Of Object)
            
            For Each row As DataRow In dataTable.Rows
                resultList.Add(New With {
                    .Text = SecurityHelper.SanitizeForHtml(row("unitid").ToString()),
                    .Value = SecurityHelper.SanitizeForHtml(row("plateno").ToString())
                })
            Next
            
            Return JsonConvert.SerializeObject(resultList)
            
        Catch ex As Exception
            SecurityHelper.LogError("GetSecureVehicleData failed", ex, Server)
            Return "{""error"":""Vehicle data retrieval failed""}"
        End Try
    End Function
End Class