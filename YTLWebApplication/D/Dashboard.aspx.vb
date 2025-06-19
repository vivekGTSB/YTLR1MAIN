Imports System.Data.SqlClient

Partial Public Class Dashboard
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' Check authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If
            
            If Not Page.IsPostBack Then
                LoadUserInfo()
                LoadStatistics()
            End If
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("DASHBOARD_ERROR", "Error loading dashboard: " & ex.Message)
            Response.Redirect("~/Error.aspx")
        End Try
    End Sub
    
    Private Sub LoadUserInfo()
        Try
            If HttpContext.Current.Session("username") IsNot Nothing Then
                lblUsername.Text = SecurityHelper.SanitizeForHtml(HttpContext.Current.Session("username").ToString())
            End If
            
            If HttpContext.Current.Session("role") IsNot Nothing Then
                lblRole.Text = SecurityHelper.SanitizeForHtml(HttpContext.Current.Session("role").ToString())
            End If
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOAD_USER_INFO_ERROR", "Error loading user info: " & ex.Message)
        End Try
    End Sub
    
    Private Sub LoadStatistics()
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' Get total vehicles
                Dim vehicleQuery As String = "SELECT COUNT(*) FROM vehicleTBL WHERE active = 1"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(vehicleQuery, conn)
                    conn.Open()
                    totalVehicles.InnerText = cmd.ExecuteScalar().ToString()
                End Using
                
                ' Get active users
                Dim userQuery As String = "SELECT COUNT(*) FROM userTBL WHERE active = 1"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(userQuery, conn)
                    activeUsers.InnerText = cmd.ExecuteScalar().ToString()
                End Using
                
                ' Get today's reports (mock data for now)
                todayReports.InnerText = "12"
            End Using
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOAD_STATISTICS_ERROR", "Error loading statistics: " & ex.Message)
            ' Set default values
            totalVehicles.InnerText = "0"
            activeUsers.InnerText = "0"
            todayReports.InnerText = "0"
        End Try
    End Sub
    
    Protected Sub btnLogout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogout.Click
        Try
            AuthenticationHelper.LogoutUser()
            Response.Redirect("~/Login.aspx")
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOGOUT_ERROR", "Error during logout: " & ex.Message)
            Response.Redirect("~/Login.aspx")
        End Try
    End Sub
    
End Class