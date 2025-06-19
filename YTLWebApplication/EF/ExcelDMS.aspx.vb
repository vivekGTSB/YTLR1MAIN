Imports System.Data.SqlClient
Imports ExcelLibrary
Imports ExcelLibrary.SpreadSheet
Imports System.IO
Imports System.Text.RegularExpressions

Partial Class ExcelDMS
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.End()
                Return
            End If
            
            ' Validate and sanitize query parameters
            Dim tplateno As String = SecurityHelper.SanitizeForSql(Request.QueryString("tplateno"))
            Dim tbdt As String = Request.QueryString("tbdt")
            Dim tedt As String = Request.QueryString("tedt")
            
            ' Validate inputs
            If Not SecurityHelper.ValidateInput(tplateno, "plateno") OrElse
               Not SecurityHelper.ValidateInput(tbdt, "date") OrElse
               Not SecurityHelper.ValidateInput(tedt, "date") Then
                SecurityHelper.LogSecurityEvent("INVALID_EXCEL_PARAMS", "Invalid parameters for Excel export")
                Response.StatusCode = 400
                Response.End()
                Return
            End If
            
            GenerateExcelReport(tplateno, tbdt, tedt)
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("EXCEL_DMS_ERROR", "Error in ExcelDMS: " & ex.Message)
            Response.StatusCode = 500
            Response.End()
        End Try
    End Sub
    
    Private Sub GenerateExcelReport(tplateno As String, tbdt As String, tedt As String)
        Try
            Dim wb As New Workbook()
            Dim sheetrowcounter As Integer = 0
            Dim sheetrowcounter2 As Integer = 0
            
            Dim sheet As New ExcelLibrary.SpreadSheet.Worksheet("DMS (PROJECT GRANDE)")
            
            ' Add header information
            sheet.Cells(sheetrowcounter, 0) = New Cell("DELIVERY MONITORING SUMMARY(PROJECT GRANDE)")
            sheet.Cells(sheetrowcounter, 3) = New Cell("Report Date: " & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            sheet.Cells(sheetrowcounter, 6) = New Cell("S=Suspect, V/V2=Valid/Double Check, P=Pending")
            sheetrowcounter += 2
            
            ' Add column headers
            AddColumnHeaders(sheet, sheetrowcounter)
            sheetrowcounter += 1
            
            ' Get data from session
            Dim t As DataTable = Session("exceltable")
            If t IsNot Nothing Then
                AddDataRows(sheet, t, sheetrowcounter)
            End If
            
            ' Add log report if applicable
            Dim isLog As Boolean = False
            If tplateno <> "ALL PLATES" AndAlso IsValidDateRange(tbdt, tedt) Then
                Dim sheet2 As New ExcelLibrary.SpreadSheet.Worksheet(tplateno & " Log report")
                AddLogReport(sheet2, tplateno, tbdt, tedt, sheetrowcounter2)
                wb.Worksheets.Add(sheet2)
                isLog = True
            End If
            
            wb.Worksheets.Add(sheet)
            
            ' Generate and send Excel file
            Response.Clear()
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("content-disposition", "attachment;filename=DeliveryMonitoringSummaryDaily.xls")
            
            Dim m As New System.IO.MemoryStream()
            wb.SaveToStream(m)
            m.WriteTo(Response.OutputStream)
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GENERATE_EXCEL_ERROR", "Error generating Excel report: " & ex.Message)
            Throw
        End Try
    End Sub
    
    Private Sub AddColumnHeaders(sheet As ExcelLibrary.SpreadSheet.Worksheet, ByRef rowCounter As Integer)
        ' Add shift headers
        sheet.Cells(rowCounter, 8) = New Cell("Out Weight Bridge")
        sheet.Cells(rowCounter, 18) = New Cell("Journey Trial")
        sheet.Cells(rowCounter, 22) = New Cell("Journey Back")
        rowCounter += 1
        
        ' Add detailed column headers
        Dim headers() As String = {
            "Customer/Ship To Name", "SHIP TO CODE", "ORDER NO", "EX", "TPT", "Vehicle No", "MT", "DN No",
            "Date", "Time", "Journey to Cust", "ATA", "ATD", "Time Spent at Site", "Back to Source",
            "PTO ON Time", "PTO OFF Time", "Stop/Idling >15 Mins", "Geofence", "Duration", "PTO", "PTO Status",
            "Stop/Idling >15 Mins", "Geofence", "Duration", "PTO", "PTO Status", "Data Lost & V-Data",
            "Driver Name", "DN Qty", "Travelling {Mins}", "Distance", "Loading {Mins}", "Waiting {Mins}", "Unloading {Mins}"
        }
        
        For i As Integer = 0 To headers.Length - 1
            sheet.Cells(rowCounter, i) = New Cell(headers(i))
            sheet.Cells.ColumnWidth(i) = 3000
        Next
        
        ' Set specific column widths
        sheet.Cells.ColumnWidth(0) = 15000
        sheet.Cells.ColumnWidth(4) = 10000
        sheet.Cells.ColumnWidth(7) = 10000
    End Sub
    
    Private Sub AddDataRows(sheet As ExcelLibrary.SpreadSheet.Worksheet, t As DataTable, ByRef rowCounter As Integer)
        For i As Integer = 0 To t.Rows.Count - 1
            Try
                For j As Integer = 0 To Math.Min(t.Columns.Count - 1, 34)
                    Dim cellValue As Object = t.DefaultView.Item(i)(j)
                    
                    If IsDBNull(cellValue) Then
                        sheet.Cells(rowCounter, j) = New Cell("-")
                    Else
                        Dim stringValue As String = cellValue.ToString()
                        
                        ' Clean HTML tags and sanitize
                        stringValue = StripTags(stringValue)
                        stringValue = stringValue.Replace("<br/>", vbCrLf)
                        
                        ' Try to convert to appropriate data type
                        Dim numericValue As Double
                        If Double.TryParse(stringValue, numericValue) Then
                            sheet.Cells(rowCounter, j) = New Cell(numericValue)
                        Else
                            sheet.Cells(rowCounter, j) = New Cell(SecurityHelper.SanitizeForHtml(stringValue))
                        End If
                    End If
                Next
                rowCounter += 1
            Catch ex As Exception
                WriteLog("Error processing row " & i & ": " & ex.Message)
            End Try
        Next
    End Sub
    
    Private Sub AddLogReport(sheet2 As ExcelLibrary.SpreadSheet.Worksheet, tplateno As String, tbdt As String, tedt As String, ByRef rowCounter As Integer)
        Try
            sheet2.Cells(rowCounter, 0) = New Cell("Log report for plateno: " & tplateno & " From " & tbdt & " To " & tedt)
            rowCounter += 4
            
            ' Add log headers
            Dim logHeaders() As String = {"S No", "Date Time", "GPS", "Speed", "Odometer", "Ignition", "PTO", "Address", "Nearest Town", "Lat", "Lon"}
            For i As Integer = 0 To logHeaders.Length - 1
                sheet2.Cells(rowCounter, i) = New Cell(logHeaders(i))
                sheet2.Cells.ColumnWidth(i) = 3000
            Next
            rowCounter += 1
            
            ' Get log data
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = "SELECT DISTINCT CONVERT(varchar(19),timestamp,120) AS datetime, alarm, vt.pto, gps_av, speed, gps_odometer, ignition_sensor, lat, lon FROM vehicle_history vht JOIN vehicleTBL vt ON vt.plateno = vht.plateno WHERE vt.plateno = @plateno AND gps_av = 'A' AND ignition_sensor <> 0 AND timestamp BETWEEN @beginDate AND @endDate ORDER BY datetime"
                
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@plateno", tplateno)
                    cmd.Parameters.AddWithValue("@beginDate", tbdt)
                    cmd.Parameters.AddWithValue("@endDate", tedt)
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim recordNumber As Integer = 1
                        
                        While dr.Read()
                            Try
                                sheet2.Cells(rowCounter, 0) = New Cell(recordNumber)
                                sheet2.Cells(rowCounter, 1) = New Cell(Convert.ToDateTime(dr("datetime")).ToString("yyyy/MM/dd HH:mm:ss"))
                                sheet2.Cells(rowCounter, 2) = New Cell(SecurityHelper.SanitizeForHtml(dr("gps_av").ToString()))
                                sheet2.Cells(rowCounter, 3) = New Cell(Convert.ToDouble(dr("speed")).ToString("0.00"))
                                sheet2.Cells(rowCounter, 4) = New Cell((Convert.ToDouble(dr("gps_odometer")) / 100.0).ToString("0.00"))
                                sheet2.Cells(rowCounter, 5) = New Cell(If(Convert.ToBoolean(dr("ignition_sensor")), "ON", "OFF"))
                                sheet2.Cells(rowCounter, 6) = New Cell(If(Convert.ToBoolean(dr("pto")), SecurityHelper.SanitizeForHtml(dr("alarm").ToString()), "--"))
                                sheet2.Cells(rowCounter, 7) = New Cell("Address data removed for security")
                                sheet2.Cells(rowCounter, 8) = New Cell("Town data removed for security")
                                sheet2.Cells(rowCounter, 9) = New Cell(Convert.ToDouble(dr("lat")).ToString("0.0000"))
                                sheet2.Cells(rowCounter, 10) = New Cell(Convert.ToDouble(dr("lon")).ToString("0.0000"))
                                
                                rowCounter += 1
                                recordNumber += 1
                            Catch ex As Exception
                                WriteLog("Error processing log record: " & ex.Message)
                            End Try
                        End While
                    End Using
                End Using
            End Using
            
        Catch ex As Exception
            WriteLog("Error adding log report: " & ex.Message)
        End Try
    End Sub
    
    Private Function IsValidDateRange(tbdt As String, tedt As String) As Boolean
        Try
            Dim startDate As DateTime = Convert.ToDateTime(tbdt)
            Dim endDate As DateTime = Convert.ToDateTime(tedt)
            Return DateDiff(DateInterval.Minute, startDate, endDate) <= 1440 ' 24 hours
        Catch
            Return False
        End Try
    End Function
    
    Function StripTags(ByVal html As String) As String
        If String.IsNullOrEmpty(html) Then
            Return String.Empty
        End If
        
        ' Remove HTML tags using regex
        Return Regex.Replace(html, "<.*?>", "")
    End Function

    Protected Sub WriteLog(ByVal message As String)
        Try
            If Not String.IsNullOrEmpty(message) Then
                Dim logPath As String = Server.MapPath("~/App_Data/ExcelLog.txt")
                Dim logEntry As String = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & " - " & SecurityHelper.SanitizeForHtml(message)
                
                Using sw As New StreamWriter(logPath, True)
                    sw.WriteLine(logEntry)
                End Using
            End If
        Catch ex As Exception
            ' Silent fail for logging
        End Try
    End Sub
End Class