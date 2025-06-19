Imports System.Text
Imports System.Data

Namespace AVLS

    Partial Class ExcelReport_new
        Inherits System.Web.UI.Page

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Try
                ' Check authentication using secure helper
                If Not AuthenticationHelper.IsUserAuthenticated() Then
                    Response.Redirect("~/Login.aspx")
                    Return
                End If
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("INIT_ERROR", "Error in OnInit: " & ex.Message)
                Response.Redirect("~/Login.aspx")
            Finally
                MyBase.OnInit(e)
            End Try
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                ' Validate session
                If Not AuthenticationHelper.IsUserAuthenticated() Then
                    Response.Redirect("~/Login.aspx")
                    Return
                End If
                
                GenerateVehicleManagementReport()
                
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("EXCEL_REPORT_NEW_ERROR", "Error in ExcelReport_new: " & ex.Message)
                Response.StatusCode = 500
                Response.End()
            End Try
        End Sub
        
        Private Sub GenerateVehicleManagementReport()
            Try
                Dim columncount As Byte = 8

                ' Validate and sanitize query parameters
                Dim title As String = SecurityHelper.SanitizeForHtml(Request.QueryString("title"))
                Dim plateno As String = SecurityHelper.SanitizeForHtml(Request.QueryString("plateno"))
                Dim type As String = SecurityHelper.SanitizeForHtml(Request.QueryString("Type"))
                Dim comName As String = SecurityHelper.SanitizeForHtml(Request.QueryString("comName"))
                
                ' Vehicle statistics
                Dim totaltank1 As String = SecurityHelper.SanitizeForHtml(Request.QueryString("Totaltank1"))
                Dim totaltank2 As String = SecurityHelper.SanitizeForHtml(Request.QueryString("Totaltank2"))
                Dim totalWithOutTank As String = SecurityHelper.SanitizeForHtml(Request.QueryString("TotalWithOutTank"))
                Dim totalPTO As String = SecurityHelper.SanitizeForHtml(Request.QueryString("TotalPTO"))
                Dim totalVehicle As String = SecurityHelper.SanitizeForHtml(Request.QueryString("TotalVehicle"))
                Dim totalExistingVehicle As String = SecurityHelper.SanitizeForHtml(Request.QueryString("totalExistingVehicle"))
                Dim totalNewInstallation As String = SecurityHelper.SanitizeForHtml(Request.QueryString("totalNewInstallation"))
                Dim totalUnbill As String = SecurityHelper.SanitizeForHtml(Request.QueryString("totalUnbill"))

                Dim temp As New DataTable
                InitializeDataTable(temp)
                
                Dim table As DataTable
                If type = "1" Then
                    temp = CType(Session("exceltable"), DataTable)
                    CleanupTableType1(temp)
                    table = temp
                ElseIf type = "2" Then
                    temp = CType(Session("exceltable2"), DataTable)
                    CleanupTableType2(temp)
                    table = temp
                Else
                    table = temp
                End If

                columncount = table.Columns.Count
                
                ' Adjust column count for specific report types
                If title = "Vehicle Log Report" OrElse title = "Vehicle Idling Report" Then
                    columncount = columncount - 1
                End If

                Response.Write("<table>")

                ' Add company name for type 2 reports
                If type = "2" AndAlso Not String.IsNullOrEmpty(comName) Then
                    Response.Write("<tr><td colspan='" & columncount & "' align='left'><b style='font-family:Verdana;font-size:20px;color:#465AE8;'> Company Name : " & comName & "</b></td></tr>")
                End If

                Response.Write("<tr><td colspan='" & columncount & "' align='left'><b style='font-family:Verdana;font-size:20px;color:#465AE8;'>" & title & "</b></td></tr>")

                ' Add vehicle statistics for type 2 reports
                If type = "2" Then
                    AddVehicleStatistics(columncount, totalVehicle, totaltank1, totaltank2, totalWithOutTank, totalPTO, totalExistingVehicle, totalNewInstallation, totalUnbill)
                End If

                Response.Write("<tr><td colspan='" & columncount & "'></td></tr>")
                
                If Not String.IsNullOrEmpty(plateno) Then
                    Response.Write("<tr><td colspan='" & columncount & "' align='left'><b>Vehicle Plate Number : </b>" & plateno & "</td></tr>")
                End If
                
                Response.Write("<tr><td colspan='" & columncount & "' align='left'><b>Report Date : </b> '" & DateTime.Now.ToString("yyyy/MM/dd H:mm:ss tt") & "</td></tr>")
                Response.Write("<tr><td colspan='" & columncount & "'></td></tr>")

                ' Add table headers
                Response.Write("<tr>")
                For j As Int32 = 0 To columncount - 1
                    Response.Write("<th style='background-color: #465AE8; color: #FFFFFF';border-right: black thin solid; border-top: black thin solid; border-left: black thin solid; border-bottom: black thin solid>" & SecurityHelper.SanitizeForHtml(table.Columns(j).Caption) & "</th>")
                Next
                Response.Write("</tr>")

                ' Add table data
                Dim totalRow As Boolean = False
                For j As Int32 = 0 To table.Rows.Count - 1
                    Response.Write("<tr>")

                    For i As Int32 = 0 To columncount - 1
                        Dim cellValue As String = table.Rows(j).Item(i).ToString()
                        
                        If cellValue = "TOTAL" Or cellValue = "" Then
                            totalRow = True
                        End If
                        
                        Dim cellStyle As String = "border-right: black thin solid; border-top: black thin solid; border-left: black thin solid; border-bottom: black thin solid"
                        
                        Response.Write("<td style='" & cellStyle & "'>" & SecurityHelper.SanitizeForHtml(cellValue) & "</td>")
                    Next
                    totalRow = False
                    Response.Write("</tr>")
                Next

                Response.Write("</table>")

                ' Set response headers for Excel download
                Response.ContentType = "application/vnd.ms-excel"
                Response.AddHeader("Content-Disposition", "attachment; filename=" & SecurityHelper.SanitizeForHtml(title) & ".xls;")
                
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("GENERATE_VEHICLE_REPORT_ERROR", "Error generating vehicle management report: " & ex.Message)
                Throw
            End Try
        End Sub
        
        Private Sub InitializeDataTable(temp As DataTable)
            temp.Columns.Add(New DataColumn("chk"))
            temp.Columns.Add(New DataColumn("no"))
            temp.Columns.Add(New DataColumn("username"))
            temp.Columns.Add(New DataColumn("plateno"))
            temp.Columns.Add(New DataColumn("plate no"))
            temp.Columns.Add(New DataColumn("unitid"))
            temp.Columns.Add(New DataColumn("type"))
            temp.Columns.Add(New DataColumn("color"))
            temp.Columns.Add(New DataColumn("model"))
            temp.Columns.Add(New DataColumn("brand"))
            temp.Columns.Add(New DataColumn("groupname"))
            temp.Columns.Add(New DataColumn("speed"))
            temp.Columns.Add(New DataColumn("tank1"))
            temp.Columns.Add(New DataColumn("tank2"))
            temp.Columns.Add(New DataColumn("portno"))
            temp.Columns.Add(New DataColumn("Immobilizer"))
            temp.Columns.Add(New DataColumn("installdate"))
            temp.Columns.Add(New DataColumn("Tank of Level Sensor"))
            temp.Columns.Add(New DataColumn("PTO"))
            temp.Columns.Add(New DataColumn("WeightSensor"))
            temp.Columns.Add(New DataColumn("Billing Type"))
            temp.Columns.Add(New DataColumn("installdate2"))
        End Sub
        
        Private Sub CleanupTableType1(temp As DataTable)
            Try
                temp.Columns.Remove("chk")
                temp.Columns.Remove("plateno")
                temp.Columns.Remove("Tank of Level Sensor")
                temp.Columns.Remove("PTO")
                temp.Columns.Remove("installdate2")
            Catch ex As Exception
                ' Columns may not exist
            End Try
        End Sub
        
        Private Sub CleanupTableType2(temp As DataTable)
            Try
                temp.Columns.Remove("chk")
                temp.Columns.Remove("plateno")
                temp.Columns.Remove("unitid")
                temp.Columns.Remove("type")
                temp.Columns.Remove("color")
                temp.Columns.Remove("model")
                temp.Columns.Remove("brand")
                temp.Columns.Remove("speed")
                temp.Columns.Remove("tank1")
                temp.Columns.Remove("tank2")
                temp.Columns.Remove("portno")
                temp.Columns.Remove("Immobilizer")
                temp.Columns.Remove("WeightSensor")
                temp.Columns.Remove("installdate2")
            Catch ex As Exception
                ' Columns may not exist
            End Try
        End Sub
        
        Private Sub AddVehicleStatistics(columncount As Integer, totalVehicle As String, totaltank1 As String, totaltank2 As String, totalWithOutTank As String, totalPTO As String, totalExistingVehicle As String, totalNewInstallation As String, totalUnbill As String)
            Response.Write("<tr><td colspan='" & columncount & "' align='left'><b style='font-family:Verdana;font-size:20px;color:#465AE8;'>" & "Total Vehicle :" & totalVehicle.Replace(",", "") & "</b></td></tr>")
            Response.Write("<tr><td colspan='" & columncount & "' align='left'><b style='font-family:Verdana;font-size:20px;color:#465AE8;'>" & "Total Tank 1: " & totaltank1 & "</b></td></tr>")
            Response.Write("<tr><td colspan='" & columncount & "' align='left'><b style='font-family:Verdana;font-size:20px;color:#465AE8;'>" & "Total Tank 2: " & totaltank2 & "</b></td></tr>")
            Response.Write("<tr><td colspan='" & columncount & "' align='left'><b style='font-family:Verdana;font-size:20px;color:#465AE8;'>" & "Total Without Tank: " & totalWithOutTank & "</b></td></tr>")
            Response.Write("<tr><td colspan='" & columncount & "' align='left'><b style='font-family:Verdana;font-size:20px;color:#465AE8;'>" & "Total PTO: " & totalPTO & "</b></td></tr>")
            Response.Write("<tr><td colspan='" & columncount & "' align='left'><b style='font-family:Verdana;font-size:20px;color:#465AE8;'>" & "Total Existing Vehicle: " & totalExistingVehicle & "</b></td></tr>")
            Response.Write("<tr><td colspan='" & columncount & "' align='left'><b style='font-family:Verdana;font-size:20px;color:#465AE8;'>" & "Total New Installation: " & totalNewInstallation & "</b></td></tr>")
            Response.Write("<tr><td colspan='" & columncount & "' align='left'><b style='font-family:Verdana;font-size:20px;color:#465AE8;'>" & "Total Unbill Vehicle: " & totalUnbill & "</b></td></tr>")
        End Sub

    End Class
End Namespace