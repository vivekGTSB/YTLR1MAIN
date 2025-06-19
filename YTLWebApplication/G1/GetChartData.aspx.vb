Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetChartData
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

            Response.Write(GetSecureChartData())
            Response.ContentType = "application/json"
            
        Catch ex As Exception
            SecurityHelper.LogError("GetChartData error", ex, Server)
            Response.StatusCode = 500
            Response.Write("{""error"":""An error occurred""}")
        End Try
    End Sub

    ' SECURITY FIX: Secure chart data retrieval
    Private Function GetSecureChartData() As String
        Try
            Dim userId As String = SessionManager.GetCurrentUserId()
            Dim userRole As String = SessionManager.GetCurrentUserRole()
            
            ' SECURITY FIX: Role-based access control
            Dim query As String
            Dim parameters As New Dictionary(Of String, Object)
            
            If userRole = "Admin" Then
                query = "SELECT short_name, name, one, onehalf, two, twohalf, three, threehalf, four, fourhalf, five, fiveplus " &
                       "FROM plant_geofence_vehicle ORDER BY shiptoname"
            Else
                query = "SELECT pgv.short_name, pgv.name, pgv.one, pgv.onehalf, pgv.two, pgv.twohalf, pgv.three, pgv.threehalf, pgv.four, pgv.fourhalf, pgv.five, pgv.fiveplus " &
                       "FROM plant_geofence_vehicle pgv INNER JOIN user_plant_access upa ON pgv.plant_id = upa.plant_id " &
                       "WHERE upa.user_id = @userId ORDER BY pgv.shiptoname"
                parameters.Add("@userId", userId)
            End If
            
            Dim dataTable As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)
            Dim resultList As New List(Of List(Of String))
            
            For Each row As DataRow In dataTable.Rows
                Dim rowData As New List(Of String) From {
                    SecurityHelper.SanitizeForHtml(row("short_name").ToString()),
                    SecurityHelper.SanitizeForHtml(row("name").ToString()),
                    SecurityHelper.SanitizeForHtml(row("one").ToString()),
                    SecurityHelper.SanitizeForHtml(row("onehalf").ToString()),
                    SecurityHelper.SanitizeForHtml(row("two").ToString()),
                    SecurityHelper.SanitizeForHtml(row("twohalf").ToString()),
                    SecurityHelper.SanitizeForHtml(row("three").ToString()),
                    SecurityHelper.SanitizeForHtml(row("threehalf").ToString()),
                    SecurityHelper.SanitizeForHtml(row("four").ToString()),
                    SecurityHelper.SanitizeForHtml(row("fourhalf").ToString()),
                    SecurityHelper.SanitizeForHtml(row("five").ToString()),
                    SecurityHelper.SanitizeForHtml(row("fiveplus").ToString())
                }
                resultList.Add(rowData)
            Next
            
            Dim result As New With {
                .aaData = resultList
            }
            
            Return JsonConvert.SerializeObject(result)
            
        Catch ex As Exception
            SecurityHelper.LogError("GetSecureChartData failed", ex, Server)
            Return "{""error"":""Chart data retrieval failed""}"
        End Try
    End Function
End Class