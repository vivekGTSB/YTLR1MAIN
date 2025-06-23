Imports System.Data.SqlClient
Public Class ReprocessOSSJobAll
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim pno As String = Request.QueryString("pno")
        ReprocessJob(pno)
    End Sub
    Public Sub ReprocessJob(ByVal pno As String)
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim cmd As SqlCommand
        Try
            cmd = New SqlCommand("update oss_patch_out set  reprocess='1', status='11',remarks='Auto Reprocess From OSS Mgmt' where patch_no in (" & pno & ")", conn)
            conn.Open()
            cmd.ExecuteNonQuery()
        Catch ex As Exception
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
End Class