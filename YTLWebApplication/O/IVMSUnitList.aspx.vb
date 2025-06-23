Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic

Imports Newtonsoft.Json


Partial Class IVMSUnitList
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

            Dim cmd As New SqlCommand()
            cmd.Connection = conn
            cmd.CommandText = "select distinct versionid from unitLST order by versionid"
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            sb.Append("<select  id=""ddlunit"" onchange=""javascript: return refreshTable()""  data-placeholder=""Select User Group"" style=""width:250px;"" class=""chosen""  tabindex=""5"">")
            sb.Append("<option id=""epty"" value=""""></option>")
            sb.Append("<option selected=""selected"" value=""0"">Select Unit Version</option>")
            sb.Append("<option  value=""All"">ALL Versions</option>")
            While dr.Read()
                sb.Append("<option  value=" & dr("versionid") & ">" & dr("versionid") & "</option>")
            End While

            opt = sb.ToString()



            MyBase.OnInit(e)
        Catch ex As Exception

        End Try

    End Sub



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub





End Class
