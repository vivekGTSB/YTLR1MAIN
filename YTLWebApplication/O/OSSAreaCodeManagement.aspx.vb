Imports System.Data
Imports System.Data.SqlClient


Partial Class OSSAreaCodeManagement
    Inherits System.Web.UI.Page
    Public sb As New StringBuilder
    Public opt As String

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Check authentication
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in OSSAreaCodeManagement OnInit: " & ex.Message)
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Add security headers
            Response.Headers.Add("X-Frame-Options", "DENY")
            Response.Headers.Add("X-Content-Type-Options", "nosniff")
            Response.Headers.Add("X-XSS-Protection", "1; mode=block")
            
            ' SECURITY FIX: Check authentication
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in OSSAreaCodeManagement Page_Load: " & ex.Message)
        End Try
    End Sub
End Class