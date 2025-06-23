Imports System.Text
Imports System.Data
Partial Class PubGeoExcelReport
    Inherits System.Web.UI.Page
    Public sbrHTML As StringBuilder
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim columncount As Byte = 0

        Dim title As String = Request.QueryString("title")


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
            If title = "Public Geofences" Then
                For j As Int32 = 0 To columncount - 1
                    If j <> 0 And j <> 4 And j <> 3 Then
                        Response.Write("<th style='background-color: #465AE8; color: #FFFFFF';border-right: black thin solid; border-top: black thin solid; border-left: black thin solid; border-bottom: black thin solid>" & table.Columns(j).Caption & "</th>")
                    End If
                Next
            Else
                For j As Int32 = 0 To columncount - 1
                    If j <> 0 And j <> 4 And j <> 3 And j <> 6 Then
                        Response.Write("<th style='background-color: #465AE8; color: #FFFFFF';border-right: black thin solid; border-top: black thin solid; border-left: black thin solid; border-bottom: black thin solid>" & table.Columns(j).Caption & "</th>")
                    End If
                Next
            End If

            
            Response.Write("</tr>")

            Dim totalRow As Boolean = False

            For j As Int32 = 0 To table.Rows.Count - 1
                Response.Write("<tr>")

                For i As Int32 = 0 To columncount - 1
                    If title = "Public Geofences" Then
                        If i <> 0 And i <> 4 And i <> 3 Then
                            Response.Write("<td style='border-right: black thin solid; border-top: black thin solid; border-left: black thin solid; border-bottom: black thin solid'>" & table.Rows(j).Item((i)).ToString() & "</td>") 'background-color: #FFFFE1;
                        End If
                    Else
                        If i <> 0 And i <> 4 And i <> 3 And i <> 6 Then
                            Response.Write("<td style='border-right: black thin solid; border-top: black thin solid; border-left: black thin solid; border-bottom: black thin solid'>" & table.Rows(j).Item((i)).ToString() & "</td>") 'background-color: #FFFFE1;
                        End If
                    End If

                Next
                totalRow = False
                Response.Write("</tr>")
            Next

            Response.Write("</table>")
        End If
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "attachment; filename=" & CStr(title & "_" & Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy/MM/dd")).Replace(" ", "_") & ".xls;")

    End Sub
End Class
