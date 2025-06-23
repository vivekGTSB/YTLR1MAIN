Imports System.Data.SqlClient
Imports System.Web
Imports System.Web.Services

Public Class receivegprsdata1 : Implements IHttpHandler

    Public Async Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "text/plain"
        Try
            Dim content As String = context.Request("data")
            Dim data() As String = content.Substring(1, content.Length - 1).Split(",")
            Dim dt As New DataTable
            dt.Columns.Add(New DataColumn("data"))
            Dim dr As DataRow
            For Each d As String In data
                dr = dt.NewRow()
                dr(0) = d
                dt.Rows.Add(dr)
            Next
            Dim bc As SqlBulkCopy = New SqlBulkCopy(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"), SqlBulkCopyOptions.FireTriggers)
            bc.DestinationTableName = "receiver_inbox_tmp_vvk"
            bc.ColumnMappings.Add("data", "data")
            Await bc.WriteToServerAsync(dt)
            context.Response.Write("Success")
        Catch ex As Exception
            context.Response.Write("Fail - " & ex.Message & " - " & ex.StackTrace)
        End Try
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property



End Class