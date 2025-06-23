Imports System.Data
Imports System.Data.SqlClient


Partial Class OSSAreaCodeManagement
    Inherits System.Web.UI.Page
    Public sb As New StringBuilder
    Public opt As String

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            'If Request.Cookies("userinfo") Is Nothing Then
            '    Response.Redirect("Login.aspx")
            'End If
        Catch ex As Exception
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try



        Catch ex As Exception

        End Try

    End Sub

End Class
