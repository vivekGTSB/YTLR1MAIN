Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class receivegprsdata
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Async Function Insert(data() As String) As Threading.Tasks.Task(Of Response)
        Dim result As New Response()
        Try
            Dim dt As New DataTable
            dt.Columns.Add(New DataColumn("data"))
            Dim dr As DataRow
            For Each d As String In data
                dr = dt.NewRow()
                dr(0) = d
                dt.Rows.Add(dr)
            Next
            Dim bc As SqlBulkCopy = New SqlBulkCopy(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"), SqlBulkCopyOptions.FireTriggers)
            bc.DestinationTableName = "receiver_inbox_tmp"
            bc.ColumnMappings.Add("data", "data")
            Await bc.WriteToServerAsync(dt)
            result.Status = 1
            result.Message = "Data Inserted Successfully"
        Catch ex As Exception
            result.Status = 0
            result.Message = "Data Inserted Failed" & ex.Message & " - " & ex.StackTrace
        End Try
        Return result
    End Function
    Class Response
        Public Status As Integer
        Public Message As String
    End Class

End Class