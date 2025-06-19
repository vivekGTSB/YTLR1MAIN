Imports System.Text
Imports System.Data

Partial Class ClientLoginExcel
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub


    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region
    Public sbrHTML As StringBuilder
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim columncount As Byte = 8

        Dim title As String = "Client Login Report"

        If Not Session("exceltable") Is Nothing Then

            Dim table As New DataTable
            table = Session("exceltable")
            columncount = table.Columns.Count
            Response.Write("<table>")
            Response.Write("<tr><td colspan='" & columncount & "' align='left'><b style='font-family:Verdana;font-size:20px;color:#465AE8;'>" & title & "</b></td></tr>")
            Response.Write("<tr><td colspan='" & columncount & "'></td></tr>")
            Response.Write("<tr><td colspan='" & columncount & "' align='left'><b>Report Date : </b>" & DateTime.Now & "</td></tr>")
            Response.Write("<tr><td colspan='" & columncount & "'></td></tr>")

            Response.Write("<tr>")
            For j As Int32 = 0 To columncount - 1
                If j <> 0 Then
                    Response.Write("<th style='background-color: #465AE8; color: #FFFFFF';border-right: black thin solid; border-top: black thin solid; border-left: black thin solid; border-bottom: black thin solid>" & table.Columns(j).Caption & "</th>")
                End If
            Next
            Response.Write("</tr>")

            For j As Int32 = 0 To table.Rows.Count - 1
                Response.Write("<tr>")
                For i As Int32 = 0 To columncount - 1
                    If i <> 0 Then
                        Response.Write("<td style='border-right: black thin solid; border-top: black thin solid; border-left: black thin solid; border-bottom: black thin solid'>" & table.Rows(j).Item((i)).ToString() & "</td>")
                    End If
                Next
                Response.Write("</tr>")
            Next
            Response.Write("</table>")
        End If

        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "attachment; filename=" & CStr(title & "_" & Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy/MM/dd")).Replace(" ", "_") & ".xls;")

    End Sub


End Class