Imports System.Data.SqlClient

Namespace AVLS
    Partial Class hLogin
        Inherits System.Web.UI.Page
        Public errormessage As String

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try
            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub

        Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            Try
                'If UCase("amichung") = UCase(password.Value) And UCase("GussmannAdmin") = UCase(uname.Value) Then
                If UCase("amichung") = UCase(txtPwd.Text) And UCase("GussmannAdmin") = UCase(txtName.Text) Then
                    Response.Cookies("accesslevel")("High") = "True"
Session("Restrict") = "True"
                    Response.Redirect("HistoryReport.aspx")
                Else
                    errormessage = "Login Failed."
                End If
            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try

        End Sub
    End Class

End Namespace