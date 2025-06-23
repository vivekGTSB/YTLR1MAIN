Imports System.Data.SqlClient
Public Class ReprocessOSSJob
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim bdt As String = Request.QueryString("bdt")
        Dim edt As String = Request.QueryString("edt")
        Dim pno As String = Request.QueryString("pno")
        ReprocessJob(bdt, edt, pno)
    End Sub
    Public Sub ReprocessJob(ByVal bdt As String, ByVal edt As String, ByVal pno As String)
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim cmd As SqlCommand
        Try
            cmd = New SqlCommand("update oss_patch_out set  reprocess='1', status='11',remarks='Auto Reprocess From OSS Mgmt' where patch_no='" & pno & "'", conn)
            conn.Open()
            cmd.ExecuteNonQuery()

        Catch ex As Exception
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Response.Redirect("OssManagementT.aspx?bdt=" & bdt & "&edt=" & edt & "")
    End Sub
End Class