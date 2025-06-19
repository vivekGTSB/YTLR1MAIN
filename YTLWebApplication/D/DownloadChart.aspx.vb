
Partial Class DownloadChart
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim title As String = Request.QueryString("title")
            Response.ContentType = "image/png"
            Response.BinaryWrite(Session("Chart"))
            Response.AddHeader("Content-Disposition", "attachment; filename=" & title & ".png;")
        Catch ex As Exception

        End Try
    End Sub
End Class
