Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports Newtonsoft.Json
Imports ASPNetMultiLanguage

Partial Class ClientSoldToManagement
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
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            Dim cmd As New SqlCommand
            Dim dr As SqlDataReader

            cmd.Connection = conn
            cmd.CommandText = "select customerid,customername from EC_soldto"
            conn.Open()
            dr = cmd.ExecuteReader()
            ddlsoldto.Items.Clear()
            ddlsoldto.Items.Add(New ListItem("Select Customer", "0"))
            While dr.Read()
                ddlsoldto.Items.Add(New ListItem(dr("customername"), dr("customerid")))
            End While


            MyBase.OnInit(e)
        Catch ex As Exception
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

    End Sub



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub





End Class
