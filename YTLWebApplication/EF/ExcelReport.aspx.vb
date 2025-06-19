Imports System.Text
Imports System.Data

Namespace AVLS

    Partial Class ExcelReport
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

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load
            Try
                ' Validate session
                If Not AuthenticationHelper.IsUserAuthenticated() Then
                    Response.Redirect("~/Login.aspx")
                    Return
                End If
                
                GenerateExcelReport()
                
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("EXCEL_REPORT_ERROR", "Error in ExcelReport: " & ex.Message)
                Response.StatusCode = 500
                Response.End()
            End Try
        End Sub
        
        Private Sub GenerateExcelReport()
            Try
                Dim columncount As Byte = 8

                ' Validate and sanitize query parameters
                Dim title As String = SecurityHelper.SanitizeForHtml(Request.QueryString("title"))
                Dim plateno As String = SecurityHelper.SanitizeForHtml(Request.QueryString("plateno"))
                Dim reportperiod As String = SecurityHelper.SanitizeForHtml(Request.QueryString("rperoid"))
                Dim username As String = SecurityHelper.SanitizeForHtml(Request.QueryString("username"))
                Dim reporttype As String = SecurityHelper.SanitizeForHtml(Request.QueryString("reporttype"))
                Dim otrack As String = SecurityHelper.SanitizeForHtml(Request.QueryString("otrack"))
                Dim internal As String = SecurityHelper.SanitizeForHtml(Request.QueryString("internal"))
                Dim external As String = SecurityHelper.SanitizeForHtml(Request.QueryString("external"))
                Dim trips As String = SecurityHelper.SanitizeForHtml(Request.QueryString("trips"))
                Dim tonnage As String = SecurityHelper.SanitizeForHtml(Request.QueryString("tonnage"))

                If Session("exceltable") IsNot Nothing Then
                    Dim table As DataTable = CType(Session("exceltable"), DataTable)
                    
                    columncount = table.Columns.Count
                    
                    ' Adjust column count for specific report types
                    Select Case title
                        Case "Vehicle Log Report", "Vehicle Idling Report", "Vehicle Speed Report", "Vehicle_Speed_Report", "Vehicle Harsh Breaking Report", "Vehicle Geofence Report"
                            columncount = columncount - 1
                        Case "Vehicle_Power_Cut_Events_Report", "Vehicle_All_Events_Report", "Vehicle_Panic_Events_Report", "Vehicle_Geofence_Events_Report", "Vehicle_Immobilizer_Events_Report"
                            columncount = columncount - 1
                    End Select
                    
                    ' Generate HTML table
                    Response.Write("<table>")
                    Response.Write("<tr><td colspan='" & columncount & "' align='left'><b style='font-family:Verdana;font-size:20px;color:#465AE8;'>" & title & "</b></td></tr>")
                    Response.Write("<tr><td colspan='" & columncount & "'></td></tr>")
                    
                    ' Add report metadata
                    AddReportMetadata(columncount, username, plateno, reporttype, reportperiod, trips, tonnage, otrack)
                    
                    Response.Write("<tr><td colspan='" & columncount & "' align='left'><b>Report Date : </b>" & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "</td></tr>")
                    Response.Write("<tr><td colspan='" & columncount & "'></td></tr>")
                    
                    ' Add special messages if available
                    If Session("OVMonthlyAllAng") IsNot Nothing Then
                        Response.Write("<tr><td colspan='" & columncount & "' align='left' style='color: #FF0000'><b>" & SecurityHelper.SanitizeForHtml(Session("OVMonthlyAllAng").ToString()) & "</b></td></tr>")
                        Response.Write("<tr><td colspan='" & columncount & "'></td></tr>")
                        Session("OVMonthlyAllAng") = ""
                    End If

                    ' Add table headers
                    Response.Write("<tr>")
                    For j As Int32 = 0 To columncount - 1
                        Dim headerText As String = table.Columns(j).Caption
                        
                        ' Clean up header text
                        Select Case headerText
                            Case "Start Location1"
                                headerText = "Start Location"
                            Case "End Location1"
                                headerText = "End Location"
                            Case "Location Name"
                                headerText = "Address"
                        End Select
                        
                        Response.Write("<th style='background-color: #465AE8; color: #FFFFFF';border-right: black thin solid; border-top: black thin solid; border-left: black thin solid; border-bottom: black thin solid>" & SecurityHelper.SanitizeForHtml(headerText) & "</th>")
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
                            If totalRow Then
                                cellStyle = "background-color:#FFA280;" & cellStyle
                            End If
                            
                            Response.Write("<td style='" & cellStyle & "'>" & SecurityHelper.SanitizeForHtml(cellValue) & "</td>")
                        Next
                        totalRow = False
                        Response.Write("</tr>")
                    Next

                    Response.Write("</table>")
                    
                    ' Add additional tables if available
                    AddAdditionalTables()
                End If

                ' Set response headers for Excel download
                Response.ContentType = "application/vnd.ms-excel"
                Dim filename As String = SecurityHelper.SanitizeForHtml(title & "_" & plateno & "_" & Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy/MM/dd")).Replace(" ", "_")
                Response.AddHeader("Content-Disposition", "attachment; filename=" & filename & ".xls;")

                ' Clean up session
                Session.Remove("exceltable")
                Session.Remove("exceltable2")
                Session.Remove("exceltable3")
                
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("GENERATE_EXCEL_REPORT_ERROR", "Error generating Excel report: " & ex.Message)
                Throw
            End Try
        End Sub
        
        Private Sub AddReportMetadata(columncount As Integer, username As String, plateno As String, reporttype As String, reportperiod As String, trips As String, tonnage As String, otrack As String)
            If Not String.IsNullOrEmpty(username) Then
                Response.Write("<tr><td colspan='" & columncount & "' align='left'><b>UserName : </b>" & username & "</td></tr>")
            End If
            
            If Not String.IsNullOrEmpty(plateno) Then
                Dim labelText As String = If(Request.QueryString("title") = "Vehicles Violation Weekly Summary Report", "Report Period", "Vehicle Plate Number")
                Response.Write("<tr><td colspan='" & columncount & "' align='left'><b>" & labelText & " : </b>" & plateno & "</td></tr>")
            End If
            
            If reporttype = "1" AndAlso Not String.IsNullOrEmpty(reportperiod) Then
                Response.Write("<tr><td colspan='" & columncount & "' align='left'><b>Report Period  : </b>" & reportperiod & "</td></tr>")
            End If

            If Not String.IsNullOrEmpty(trips) Then
                Response.Write("<tr><td colspan='" & columncount & "' align='left'><b>Trips Loaded/ Total Order Trip  : </b>" & trips & "</td></tr>")
            End If
            
            If Not String.IsNullOrEmpty(tonnage) Then
                Response.Write("<tr><td colspan='" & columncount & "' align='left'><b>Tonnage Loaded/Order Tonnage  : </b>" & tonnage & "</td></tr>")
            End If
            
            If Not String.IsNullOrEmpty(otrack) Then
                Response.Write("<tr><td colspan='" & columncount & "' align='left'><b>Hours to Dateline  : </b>" & otrack & "</td></tr>")
            End If
            
            If Session("internaltruck") IsNot Nothing Then
                Response.Write("<tr><td colspan='" & columncount & "' align='left'><b>Internal Trucks  : </b>" & SecurityHelper.SanitizeForHtml(Session("internaltruck").ToString()) & "</td></tr>")
            End If
            
            If Session("externaltruck") IsNot Nothing Then
                Response.Write("<tr><td colspan='" & columncount & "' align='left'><b>External Trucks  : </b>" & SecurityHelper.SanitizeForHtml(Session("externaltruck").ToString()) & "</td></tr>")
            End If
        End Sub
        
        Private Sub AddAdditionalTables()
            ' Add second table if available
            If Session("exceltable2") IsNot Nothing Then
                Dim table2 As DataTable = CType(Session("exceltable2"), DataTable)
                
                Response.Write("<br/><br/><table border='1'>")
                Response.Write("<tr>")
                For j As Int32 = 0 To table2.Columns.Count - 1
                    Dim headerText As String = If(table2.Columns(j).Caption = "Location Name", "Address", table2.Columns(j).Caption)
                    Response.Write("<th style='background-color: #465AE8; color: #FFFFFF'>" & SecurityHelper.SanitizeForHtml(headerText) & "</th>")
                Next
                Response.Write("</tr>")

                For j As Int32 = 0 To table2.Rows.Count - 1
                    Response.Write("<tr>")
                    For i As Int32 = 0 To table2.Columns.Count - 1
                        Response.Write("<td>" & SecurityHelper.SanitizeForHtml(table2.Rows(j).Item(i).ToString()) & "</td>")
                    Next
                    Response.Write("</tr>")
                Next
                Response.Write("</table>")
            End If

            ' Add third table if available
            If Session("exceltable3") IsNot Nothing Then
                Dim table3 As DataTable = CType(Session("exceltable3"), DataTable)
                
                Response.Write("<br/><br/><table border='1'>")
                Response.Write("<tr>")
                For j As Int32 = 0 To table3.Columns.Count - 1
                    Response.Write("<th style='background-color: #465AE8; color: #FFFFFF'>" & SecurityHelper.SanitizeForHtml(table3.Columns(j).Caption) & "</th>")
                Next
                Response.Write("</tr>")

                For j As Int32 = 0 To table3.Rows.Count - 1
                    Response.Write("<tr>")
                    For i As Int32 = 0 To table3.Columns.Count - 1
                        Response.Write("<td>" & SecurityHelper.SanitizeForHtml(table3.Rows(j).Item(i).ToString()) & "</td>")
                    Next
                    Response.Write("</tr>")
                Next
                Response.Write("</table>")
            End If
        End Sub

    End Class
End Namespace