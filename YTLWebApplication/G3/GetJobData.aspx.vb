Public Class GetJobData
    Inherits System.Web.UI.Page
    Public plateno As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            plateno = Request.QueryString("plateno")
            If plateno <> "" Then
                plateno = plateno.Trim()
            End If
            hdnPlate.Value = plateno
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

End Class