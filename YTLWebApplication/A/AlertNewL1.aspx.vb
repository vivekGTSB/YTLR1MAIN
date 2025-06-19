Partial Class AlertNewL
    Inherits System.Web.UI.Page
    Public Shared bd, ed As String
    Public Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            txtBeginDate.Value = Now.ToString("yyyy/MM/dd")
            bd = txtBeginDate.Value
            txtEndDate.Value = Now.AddDays(1).ToString("yyyy/MM/dd")
            ed = txtEndDate.Value
        End If
    End Sub
End Class
