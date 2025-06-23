Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports Newtonsoft.Json
Imports ASPNetMultiLanguage

Partial Class SoldToManagement
    Inherits System.Web.UI.Page

    Public sb1 As New StringBuilder()
    Public opt As String
    Public sb As New StringBuilder()
    Public suserid As String
    Public userid As String
    Public un As String
    
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try
            un = "User Name"
            
            ' SECURITY FIX: Validate user session
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            MyBase.OnInit(e)
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("OnInit error", ex, Server)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Page load logic
    End Sub
End Class