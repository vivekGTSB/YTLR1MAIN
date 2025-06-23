Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports Newtonsoft.Json
Imports ASPNetMultiLanguage

Partial Class SMSNotificationManagement
    Inherits System.Web.UI.Page
    Public opt As String
    Public sb As New StringBuilder()
    
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        ' SECURITY FIX: Validate user session
        If Request.Cookies("userinfo") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If
        
        ' SECURITY FIX: Load ship to codes for dropdown
        LoadShipToCodes()
        
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Page load logic
    End Sub
    
    ' SECURITY FIX: Load ship to codes with proper parameterized query
    Private Sub LoadShipToCodes()
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim cmd As New SqlCommand("SELECT shiptocode, name FROM oss_ship_to_code ORDER BY name", conn)
        
        Try
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            
            sb.Clear()
            sb.Append("<option value='0'>SELECT SHIP TO CODE</option>")
            
            While dr.Read()
                ' SECURITY FIX: HTML encode output
                sb.Append("<option value='" & HttpUtility.HtmlEncode(dr("shiptocode").ToString()) & "'>" & 
                          HttpUtility.HtmlEncode(dr("name").ToString()) & " (" & 
                          HttpUtility.HtmlEncode(dr("shiptocode").ToString()) & ")</option>")
            End While
            
            Session("opt") = sb.ToString()
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("LoadShipToCodes error", ex, Server)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
End Class