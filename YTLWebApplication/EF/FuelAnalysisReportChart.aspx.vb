Imports System.Data.SqlClient
Imports ChartDirector
Imports System.Collections.Generic

Partial Class FuelAnalysisReportChart
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' Validate session
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If
            
            ' Get user info from session
            Dim userid As String = HttpContext.Current.Session("userid").ToString()
            Dim role As String = HttpContext.Current.Session("role").ToString()
            Dim userslist As String = HttpContext.Current.Session("userslist").ToString()

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")

            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                LoadUsers(userid, role, userslist)
            End If
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("PAGE_LOAD_ERROR", "Error in Page_Load: " & ex.Message)
        End Try
    End Sub
    
    Private Sub LoadUsers(userid As String, role As String, userslist As String)
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = "SELECT userid, username FROM userTBL WHERE role='User' ORDER BY username"
                
                If role = "User" Then
                    query = "SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username"
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    query = "SELECT userid, username FROM userTBL WHERE userid IN (" & userslist & ") ORDER BY username"
                End If
                
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    If role = "User" Then
                        cmd.Parameters.AddWithValue("@userid", userid)
                    End If
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        ddlUsername.Items.Clear()
                        ddlUsername.Items.Add("--Select User Name--")
                        
                        While dr.Read()
                            ddlUsername.Items.Add(New ListItem(SecurityHelper.SanitizeForHtml(dr("username").ToString()), dr("userid").ToString()))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOAD_USERS_ERROR", "Error loading users: " & ex.Message)
        End Try
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        Try
            If SecurityHelper.ValidateInput(ddlUsername.SelectedValue, "numeric") AndAlso ddlUsername.SelectedValue <> "--Select User Name--" Then
                LoadPlateNumbers(ddlUsername.SelectedValue)
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("DDL_USERNAME_CHANGED_ERROR", "Error in ddlUsername_SelectedIndexChanged: " & ex.Message)
        End Try
    End Sub
    
    Private Sub LoadPlateNumbers(userId As String)
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = "SELECT plateno FROM vehicleTBL WHERE userid = @userid ORDER BY plateno"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userid", userId)
                    
                    conn.Open()
                    ddlpleate.Items.Clear()
                    ddlpleate.Items.Add("--Select Plate No--")
                    
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            ddlpleate.Items.Add(New ListItem(SecurityHelper.SanitizeForHtml(dr("plateno").ToString()), dr("plateno").ToString()))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOAD_PLATES_ERROR", "Error loading plate numbers: " & ex.Message)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        Try
            ' Validate inputs
            If Not SecurityHelper.ValidateInput(txtBeginDate.Value, "date") OrElse 
               Not SecurityHelper.ValidateInput(txtEndDate.Value, "date") Then
                SecurityHelper.LogSecurityEvent("INVALID_DATE_INPUT", "Invalid date format in ImageButton1_Click")
                Return
            End If
            
            If Not SecurityHelper.ValidateInput(ddlpleate.SelectedValue, "plateno") Then
                SecurityHelper.LogSecurityEvent("INVALID_PLATE_INPUT", "Invalid plate number: " & ddlpleate.SelectedValue)
                Return
            End If
            
            DisplayFuelAnalysisReport()
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("IMAGEBUTTON_CLICK_ERROR", "Error in ImageButton1_Click: " & ex.Message)
        End Try
    End Sub
    
    Private Sub DisplayFuelAnalysisReport()
        Try
            Dim plateno As String = ddlpleate.SelectedValue
            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            
            ' Create data tables for report
            Dim summaryTable As New DataTable
            summaryTable.Columns.Add("Begin Date Time")
            summaryTable.Columns.Add("End Date Time")
            summaryTable.Columns.Add("Max Speed")
            summaryTable.Columns.Add("Mileage")
            summaryTable.Columns.Add("Fuel")
            summaryTable.Columns.Add("Fuel Cost")
            summaryTable.Columns.Add("Liter/KM")
            summaryTable.Columns.Add("KM/Liter")
            summaryTable.Columns.Add("Cost/liter")
            
            Dim idlingTable As New DataTable
            idlingTable.Columns.Add("Idling Time")
            idlingTable.Columns.Add("Idling Fuel")
            idlingTable.Columns.Add("Hour Idling Fuel")
            idlingTable.Columns.Add("Idling Cost")
            idlingTable.Columns.Add("Total Idling Cost")
            
            Dim detailTable As New DataTable
            detailTable.Columns.Add("S No")
            detailTable.Columns.Add("Date Time")
            detailTable.Columns.Add("GPS AV")
            detailTable.Columns.Add("Speed")
            detailTable.Columns.Add("Ignition")
            detailTable.Columns.Add("Odometer")
            detailTable.Columns.Add("OdometerIncrement")
            detailTable.Columns.Add("Tank Level")
            detailTable.Columns.Add("Tank Volume")
            detailTable.Columns.Add("Tank Level 2")
            detailTable.Columns.Add("Tank Volume 2")
            detailTable.Columns.Add("Total Volume")
            
            ' Process fuel analysis data
            ProcessFuelData(plateno, begindatetime, enddatetime, summaryTable, idlingTable, detailTable)
            
            ' Bind data to grids
            GridView2.DataSource = summaryTable
            GridView2.DataBind()
            
            GridView3.DataSource = idlingTable
            GridView3.DataBind()
            
            GridView1.PageSize = Convert.ToInt32(noofrecords.SelectedValue)
            GridView1.DataSource = detailTable
            GridView1.DataBind()
            
            ' Store for Excel export
            Session.Remove("exceltable")
            Session("exceltable") = detailTable
            
            ec = "true"
            show = True
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("DISPLAY_FUEL_ANALYSIS_ERROR", "Error displaying fuel analysis: " & ex.Message)
        End Try
    End Sub
    
    Private Sub ProcessFuelData(plateno As String, beginDateTime As String, endDateTime As String, 
                               summaryTable As DataTable, idlingTable As DataTable, detailTable As DataTable)
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' Get vehicle history data
                Dim query As String = "SELECT timestamp, gps_av, speed, ignition_sensor, gps_odometer, tank1_level, tank1_volume, tank2_level, tank2_volume FROM vehicle_history WHERE plateno = @plateno AND timestamp BETWEEN @beginDate AND @endDate AND gps_av = 'A' ORDER BY timestamp"
                
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@plateno", plateno)
                    cmd.Parameters.AddWithValue("@beginDate", beginDateTime)
                    cmd.Parameters.AddWithValue("@endDate", endDateTime)
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim recordCount As Integer = 1
                        Dim totalDistance As Double = 0
                        Dim totalFuel As Double = 0
                        Dim totalIdlingTime As Double = 0
                        Dim previousOdometer As Double = 0
                        Dim previousVolume As Double = 0
                        Dim firstRecord As Boolean = True
                        
                        While dr.Read()
                            Dim r As DataRow = detailTable.NewRow()
                            
                            r("S No") = recordCount
                            r("Date Time") = Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                            r("GPS AV") = SecurityHelper.SanitizeForHtml(dr("gps_av").ToString())
                            r("Speed") = Convert.ToDouble(dr("speed")).ToString("0.00")
                            r("Ignition") = If(Convert.ToBoolean(dr("ignition_sensor")), "ON", "OFF")
                            
                            Dim currentOdometer As Double = Convert.ToDouble(dr("gps_odometer")) / 100.0
                            r("Odometer") = currentOdometer.ToString("0.00")
                            
                            If Not firstRecord Then
                                Dim odometerIncrement As Double = currentOdometer - previousOdometer
                                r("OdometerIncrement") = odometerIncrement.ToString("0.00")
                                totalDistance += odometerIncrement
                            Else
                                r("OdometerIncrement") = "0.00"
                                firstRecord = False
                            End If
                            
                            ' Process tank data
                            Dim tank1Level As Double = If(IsDBNull(dr("tank1_level")), 0, Convert.ToDouble(dr("tank1_level")))
                            Dim tank1Volume As Double = If(IsDBNull(dr("tank1_volume")), 0, Convert.ToDouble(dr("tank1_volume")))
                            Dim tank2Level As Double = If(IsDBNull(dr("tank2_level")), 0, Convert.ToDouble(dr("tank2_level")))
                            Dim tank2Volume As Double = If(IsDBNull(dr("tank2_volume")), 0, Convert.ToDouble(dr("tank2_volume")))
                            
                            r("Tank Level") = tank1Level.ToString("0.00")
                            r("Tank Volume") = tank1Volume.ToString("0.00")
                            r("Tank Level 2") = tank2Level.ToString("0.00")
                            r("Tank Volume 2") = tank2Volume.ToString("0.00")
                            
                            Dim totalVolume As Double = tank1Volume + tank2Volume
                            r("Total Volume") = totalVolume.ToString("0.00")
                            
                            ' Calculate fuel consumption
                            If Not firstRecord AndAlso previousVolume > 0 AndAlso totalVolume > 0 Then
                                Dim fuelDiff As Double = previousVolume - totalVolume
                                If fuelDiff > 0 Then
                                    totalFuel += fuelDiff
                                End If
                            End If
                            
                            previousOdometer = currentOdometer
                            previousVolume = totalVolume
                            detailTable.Rows.Add(r)
                            recordCount += 1
                        End While
                        
                        ' Create summary data
                        If detailTable.Rows.Count > 0 Then
                            CreateSummaryData(summaryTable, idlingTable, beginDateTime, endDateTime, 
                                            totalDistance, totalFuel, totalIdlingTime)
                        End If
                    End Using
                End Using
            End Using
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("PROCESS_FUEL_DATA_ERROR", "Error processing fuel data: " & ex.Message)
        End Try
    End Sub
    
    Private Sub CreateSummaryData(summaryTable As DataTable, idlingTable As DataTable, 
                                 beginDateTime As String, endDateTime As String, 
                                 totalDistance As Double, totalFuel As Double, totalIdlingTime As Double)
        Try
            ' Summary row
            Dim summaryRow As DataRow = summaryTable.NewRow()
            summaryRow("Begin Date Time") = beginDateTime
            summaryRow("End Date Time") = endDateTime
            summaryRow("Mileage") = totalDistance.ToString("0.00")
            summaryRow("Fuel") = totalFuel.ToString("0.00")
            
            ' Calculate fuel efficiency
            Dim fuelCost As Double = totalFuel * 2.10 ' Assuming RM 2.10 per liter
            summaryRow("Fuel Cost") = fuelCost.ToString("0.00")
            
            If totalDistance > 0 Then
                summaryRow("Liter/KM") = (totalFuel / totalDistance).ToString("0.00")
                summaryRow("KM/Liter") = (totalDistance / totalFuel).ToString("0.00")
                summaryRow("Cost/liter") = (fuelCost / totalDistance).ToString("0.00")
            Else
                summaryRow("Liter/KM") = "0.00"
                summaryRow("KM/Liter") = "0.00"
                summaryRow("Cost/liter") = "0.00"
            End If
            
            summaryTable.Rows.Add(summaryRow)
            
            ' Idling row
            Dim idlingRow As DataRow = idlingTable.NewRow()
            idlingRow("Idling Time") = TimeSpan.FromMinutes(totalIdlingTime).ToString("hh\:mm\:ss")
            
            Dim idlingFuel As Double = totalIdlingTime * 0.5 ' Assuming 0.5L per hour idling
            idlingRow("Idling Fuel") = idlingFuel.ToString("0.00")
            idlingRow("Hour Idling Fuel") = "0.50"
            idlingRow("Idling Cost") = (0.5 * 2.10).ToString("0.00")
            idlingRow("Total Idling Cost") = (idlingFuel * 2.10).ToString("0.00")
            
            idlingTable.Rows.Add(idlingRow)
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("CREATE_SUMMARY_DATA_ERROR", "Error creating summary data: " & ex.Message)
        End Try
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                ' Apply alternating row styles securely
                If e.Row.RowIndex Mod 2 = 1 Then
                    e.Row.BackColor = System.Drawing.Color.Lavender
                End If
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GRIDVIEW_ROWDATABOUND_ERROR", "Error in GridView1_RowDataBound: " & ex.Message)
        End Try
    End Sub

End Class