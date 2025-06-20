Imports System.Data.SqlClient

Partial Class GetLostData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate session
            If Not IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate and sanitize input parameters
            Dim plateno As String = ValidatePlateNumber(Request.QueryString("p"))
            Dim bdt As String = ValidateDateTime(Request.QueryString("b"))
            Dim edt As String = ValidateDateTime(Request.QueryString("e"))
            
            If String.IsNullOrEmpty(plateno) OrElse String.IsNullOrEmpty(bdt) OrElse String.IsNullOrEmpty(edt) Then
                GridView1.DataSource = CreateEmptyDataTable()
                GridView1.DataBind()
                Return
            End If

            GridView1.DataSource = GetData(plateno, bdt, edt)
            GridView1.DataBind()
            
        Catch ex As Exception
            LogSecurityEvent("GetLostData error", ex.Message)
            GridView1.DataSource = CreateEmptyDataTable()
            GridView1.DataBind()
        End Try
    End Sub

    ' SECURITY FIX: Validate session authentication
    Private Function IsUserAuthenticated() As Boolean
        Try
            Return HttpContext.Current.Session("authenticated") IsNot Nothing AndAlso 
                   CBool(HttpContext.Current.Session("authenticated")) AndAlso
                   HttpContext.Current.Session("userid") IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ' SECURITY FIX: Validate plate number
    Private Function ValidatePlateNumber(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then
            Return ""
        End If

        ' Remove dangerous characters and limit length
        Dim sanitized As String = System.Text.RegularExpressions.Regex.Replace(input.Trim(), "[<>\"'%;()&+*/=]", "")
        
        ' Only allow alphanumeric, spaces, and hyphens for plate numbers
        If System.Text.RegularExpressions.Regex.IsMatch(sanitized, "^[A-Za-z0-9\-\s]{1,20}$") Then
            Return sanitized
        End If

        Return ""
    End Function

    ' SECURITY FIX: Validate date time input
    Private Function ValidateDateTime(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then
            Return ""
        End If

        Try
            Dim dateValue As DateTime = DateTime.Parse(input)
            ' Check reasonable date range
            If dateValue >= DateTime.Now.AddYears(-1) AndAlso dateValue <= DateTime.Now.AddDays(1) Then
                Return dateValue.ToString("yyyy/MM/dd HH:mm:ss")
            End If
        Catch
            Return ""
        End Try

        Return ""
    End Function

    ' SECURITY FIX: Create empty data table for error cases
    Private Function CreateEmptyDataTable() As DataTable
        Dim t As New DataTable
        t.Columns.Add(New DataColumn("Sl No"))
        t.Columns.Add(New DataColumn("Plate No"))
        t.Columns.Add(New DataColumn("From Time"))
        t.Columns.Add(New DataColumn("To Time"))
        t.Columns.Add(New DataColumn("From Location"))
        t.Columns.Add(New DataColumn("To Location"))
        t.Columns.Add(New DataColumn("Duration"))
        t.Columns.Add(New DataColumn("Type"))
        
        Dim r As DataRow = t.NewRow()
        r(0) = "No data available"
        r(1) = "-"
        r(2) = "-"
        r(3) = "-"
        r(4) = "-"
        r(5) = "-"
        r(6) = "-"
        r(7) = "-"
        t.Rows.Add(r)
        
        Return t
    End Function

    Private Function GetData(plateno As String, bdt As String, edt As String) As DataTable
        Dim t As New DataTable
        t.Columns.Add(New DataColumn("Sl No"))
        t.Columns.Add(New DataColumn("Plate No"))
        t.Columns.Add(New DataColumn("From Time"))
        t.Columns.Add(New DataColumn("To Time"))
        t.Columns.Add(New DataColumn("From Location"))
        t.Columns.Add(New DataColumn("To Location"))
        t.Columns.Add(New DataColumn("Duration"))
        t.Columns.Add(New DataColumn("Type"))
        
        Dim locObj As New Location()
        Dim counter As Integer = 0
        
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY FIX: Use parameterized query
                Dim query As String = "select vh.timestamp,vh.gps_av,vh.ignition_sensor,vt.userid,vh.lat,vh.lon from (select lat,lon,timestamp,gps_av,ignition_sensor,plateno from vehicle_history where plateno=@plateno and timestamp between @startDate and @endDate) vh left outer join vehicleTBL vt on vt.plateno=vh.plateno"
                
                Using da As New SqlDataAdapter(query, conn)
                    da.SelectCommand.Parameters.AddWithValue("@plateno", plateno)
                    da.SelectCommand.Parameters.AddWithValue("@startDate", bdt)
                    da.SelectCommand.Parameters.AddWithValue("@endDate", edt)
                    
                    Dim ds As New DataSet
                    da.Fill(ds)
                    
                    If ds.Tables(0).Rows.Count > 0 Then
                        ProcessVehicleHistory(ds.Tables(0), t, locObj, plateno, counter)
                    End If
                End Using
            End Using

        Catch ex As Exception
            LogSecurityEvent("GetData database error", ex.Message)
            Dim r As DataRow = t.NewRow()
            r(0) = "Error"
            r(1) = plateno
            r(2) = "Error retrieving data"
            r(3) = "-"
            r(4) = "-"
            r(5) = "-"
            r(6) = "-"
            r(7) = "Error"
            t.Rows.Add(r)
        End Try

        If t.Rows.Count = 0 Then
            Dim r As DataRow = t.NewRow()
            r(0) = "No data"
            r(1) = HttpUtility.HtmlEncode(plateno)
            r(2) = "No data found"
            r(3) = "-"
            r(4) = "-"
            r(5) = "-"
            r(6) = "-"
            r(7) = "No data"
            t.Rows.Add(r)
        End If

        Return t
    End Function

    ' SECURITY FIX: Process vehicle history data safely
    Private Sub ProcessVehicleHistory(dataTable As DataTable, resultTable As DataTable, locObj As Location, plateno As String, ByRef counter As Integer)
        Try
            Dim prevstatus As String = "A"
            Dim prevdatetime As DateTime
            Dim currentstatus As String = "A"
            Dim prevlat As Double
            Dim prevlon As Double
            Dim currentlat As Double
            Dim currentlon As Double
            Dim currentdatetime As DateTime
            Dim tempprevtime As DateTime

            If dataTable.Rows.Count > 0 Then
                prevdatetime = Convert.ToDateTime(dataTable.Rows(0)(0))
                prevlon = Convert.ToDouble(dataTable.Rows(0)("lon"))
                prevlat = Convert.ToDouble(dataTable.Rows(0)("lat"))
                prevstatus = dataTable.Rows(0)(1).ToString.ToUpper()
                
                For i As Integer = 0 To dataTable.Rows.Count - 1
                    Try
                        currentdatetime = Convert.ToDateTime(dataTable.Rows(i)("timestamp"))
                        currentstatus = dataTable.Rows(i)("gps_av").ToString.ToUpper()
                        currentlon = Convert.ToDouble(dataTable.Rows(i)("lon"))
                        currentlat = Convert.ToDouble(dataTable.Rows(i)("lat"))

                        ' Process data loss detection
                        If i > 1 Then
                            ProcessDataLoss(dataTable, resultTable, i, tempprevtime, currentdatetime, prevlat, prevlon, currentlat, currentlon, plateno, locObj, counter)
                        End If

                        ' Process status changes
                        If prevstatus <> currentstatus Then
                            ProcessStatusChange(resultTable, prevstatus, prevdatetime, currentdatetime, prevlat, prevlon, currentlat, currentlon, plateno, locObj, counter)
                            prevdatetime = currentdatetime
                            prevstatus = currentstatus
                        End If

                        tempprevtime = currentdatetime
                        prevlat = currentlat
                        prevlon = currentlon
                        
                    Catch ex As Exception
                        LogSecurityEvent("Vehicle history processing error", ex.Message)
                    End Try
                Next
            End If
            
        Catch ex As Exception
            LogSecurityEvent("ProcessVehicleHistory error", ex.Message)
        End Try
    End Sub

    ' SECURITY FIX: Process data loss safely
    Private Sub ProcessDataLoss(dataTable As DataTable, resultTable As DataTable, index As Integer, tempprevtime As DateTime, currentdatetime As DateTime, prevlat As Double, prevlon As Double, currentlat As Double, currentlon As Double, plateno As String, locObj As Location, ByRef counter As Integer)
        Try
            If dataTable.Rows(index)("ignition_sensor").ToString() = "1" AndAlso 
               dataTable.Rows(index - 1)("ignition_sensor").ToString() = "1" AndAlso 
               (currentdatetime - tempprevtime).TotalMinutes > 15 Then
                
                AddDataRow(resultTable, counter, plateno, tempprevtime, currentdatetime, prevlat, prevlon, currentlat, currentlon, "Data Loss (On)", locObj)
            End If

            If dataTable.Rows(index)("ignition_sensor").ToString() = "0" AndAlso 
               dataTable.Rows(index - 1)("ignition_sensor").ToString() = "0" AndAlso 
               (currentdatetime - tempprevtime).TotalMinutes > 120 Then
                
                AddDataRow(resultTable, counter, plateno, tempprevtime, currentdatetime, prevlat, prevlon, currentlat, currentlon, "Data Loss (Off)", locObj)
            End If
            
        Catch ex As Exception
            LogSecurityEvent("ProcessDataLoss error", ex.Message)
        End Try
    End Sub

    ' SECURITY FIX: Process status change safely
    Private Sub ProcessStatusChange(resultTable As DataTable, prevstatus As String, prevdatetime As DateTime, currentdatetime As DateTime, prevlat As Double, prevlon As Double, currentlat As Double, currentlon As Double, plateno As String, locObj As Location, ByRef counter As Integer)
        Try
            Select Case prevstatus
                Case "V"
                    Dim temptime As TimeSpan = currentdatetime - prevdatetime
                    If temptime.TotalMinutes > 15 Then
                        AddDataRow(resultTable, counter, plateno, prevdatetime, currentdatetime, prevlat, prevlon, currentlat, currentlon, "V - Data", locObj)
                    End If
            End Select
            
        Catch ex As Exception
            LogSecurityEvent("ProcessStatusChange error", ex.Message)
        End Try
    End Sub

    ' SECURITY FIX: Add data row safely with encoding
    Private Sub AddDataRow(resultTable As DataTable, ByRef counter As Integer, plateno As String, fromTime As DateTime, toTime As DateTime, fromLat As Double, fromLon As Double, toLat As Double, toLon As Double, dataType As String, locObj As Location)
        Try
            counter += 1
            Dim r As DataRow = resultTable.NewRow()
            r(0) = counter
            r(1) = HttpUtility.HtmlEncode(plateno)
            r(2) = HttpUtility.HtmlEncode(fromTime.ToString("yyyy/MM/dd HH:mm:ss"))
            r(3) = HttpUtility.HtmlEncode(toTime.ToString("yyyy/MM/dd HH:mm:ss"))
            r(4) = HttpUtility.HtmlEncode(locObj.GetLocation(fromLat, fromLon))
            r(5) = HttpUtility.HtmlEncode(locObj.GetLocation(toLat, toLon))
            r(6) = CInt((toTime - fromTime).TotalMinutes).ToString("0")
            r(7) = HttpUtility.HtmlEncode(dataType)
            resultTable.Rows.Add(r)
            
        Catch ex As Exception
            LogSecurityEvent("AddDataRow error", ex.Message)
        End Try
    End Sub

    ' SECURITY FIX: Secure logging
    Private Sub LogSecurityEvent(eventType As String, message As String)
        Try
            Dim logMessage As String = String.Format("{0}: {1} - User: {2}, IP: {3}", 
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                eventType, 
                If(HttpContext.Current.Session("userid"), "Unknown"), 
                HttpContext.Current.Request.UserHostAddress)
            
            System.Diagnostics.EventLog.WriteEntry("YTL_Security", logMessage, System.Diagnostics.EventLogEntryType.Warning)
        Catch
            ' Fail silently
        End Try
    End Sub
End Class