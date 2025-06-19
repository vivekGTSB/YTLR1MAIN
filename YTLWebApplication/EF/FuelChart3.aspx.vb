Imports System.Data.SqlClient
Imports ChartDirector
Imports System.Collections.Generic

Partial Class FuelChart3
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.End()
                Return
            End If
            
            DisplayChartFull()
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("FUEL_CHART_ERROR", "Error in FuelChart3: " & ex.Message)
            Response.StatusCode = 500
            Response.End()
        End Try
    End Sub

    Protected Sub DisplayChartFull()
        Try
            ' Validate and sanitize query parameters
            Dim userid As String = SecurityHelper.SanitizeForSql(Request.QueryString("userid"))
            Dim username As String = SecurityHelper.SanitizeForHtml(Request.QueryString("username"))
            Dim plateno As String = SecurityHelper.SanitizeForSql(Request.QueryString("plateno"))
            Dim day As String = Request.QueryString("day")
            
            ' Validate inputs
            If Not SecurityHelper.ValidateInput(userid, "numeric") OrElse
               Not SecurityHelper.ValidateInput(plateno, "plateno") OrElse
               Not SecurityHelper.ValidateInput(day, "date") Then
                SecurityHelper.LogSecurityEvent("INVALID_CHART_PARAMS", "Invalid parameters for fuel chart")
                Response.StatusCode = 400
                Response.End()
                Return
            End If
            
            Dim begindatetime As String = day & " 00:00:00"
            Dim enddatetime As String = day & " 23:59:59"
            Dim tdate As Date = Date.Parse(begindatetime)

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = "SELECT timestamp, gps_av, speed, ignition_sensor, gps_odometer, tank1_level, tank1_volume, tank2_level, tank2_volume FROM vehicle_history WHERE plateno = @plateno AND timestamp BETWEEN @beginDate AND @endDate AND gps_odometer <> -1 ORDER BY timestamp"
                
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@plateno", plateno)
                    cmd.Parameters.AddWithValue("@beginDate", begindatetime)
                    cmd.Parameters.AddWithValue("@endDate", enddatetime)
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim vehicleRecordList As New List(Of VehicleRecord)
                        
                        ' Check if vehicle has 2 tanks
                        Dim is2Tank As Boolean = CheckIfVehicleHas2Tanks(plateno, conn)
                        
                        While dr.Read()
                            Try
                                Dim vr As New VehicleRecord()
                                vr.timeStamp = DateTime.Parse(dr("timestamp").ToString())
                                vr.seconds = vr.timeStamp.TimeOfDay.TotalMinutes
                                vr.gpsAV = dr("gps_av").ToString().Chars(0)
                                vr.speed = Convert.ToDouble(dr("speed"))
                                vr.odometer = Convert.ToDouble(dr("gps_odometer"))
                                
                                Dim l1 As Double = If(IsDBNull(dr("tank1_level")), -1, Convert.ToDouble(dr("tank1_level")))
                                Dim l2 As Double = If(IsDBNull(dr("tank2_level")), -1, Convert.ToDouble(dr("tank2_level")))
                                Dim v1 As Double = If(IsDBNull(dr("tank1_volume")), -1, Convert.ToDouble(dr("tank1_volume")))
                                Dim v2 As Double = If(IsDBNull(dr("tank2_volume")), -1, Convert.ToDouble(dr("tank2_volume")))
                                
                                ' Calculate total volume based on tank configuration
                                If is2Tank Then
                                    If v1 > -1 And v2 > -1 Then
                                        vr.volumn = v1 + v2
                                    ElseIf v1 > -1 And v2 <= -1 Then
                                        vr.volumn = v1
                                    ElseIf v1 <= -1 And v2 > -1 Then
                                        vr.volumn = v2
                                    Else
                                        vr.volumn = -1
                                    End If
                                Else
                                    vr.volumn = v1
                                End If
                                
                                ' Calculate total level
                                If l1 > -1 And l2 > -1 Then
                                    vr.level = l1 + l2
                                ElseIf l1 > -1 And l2 <= -1 Then
                                    vr.level = l1
                                ElseIf l1 <= -1 And l2 > -1 Then
                                    vr.level = l2
                                Else
                                    vr.level = -1
                                End If
                                
                                If vr.level > -1 And vr.volumn > -1 Then
                                    vehicleRecordList.Add(vr)
                                End If
                                
                            Catch ex As Exception
                                ' Skip invalid records
                                Continue While
                            End Try
                        End While
                        
                        If vehicleRecordList.Count > 0 Then
                            GenerateChart(vehicleRecordList, username, plateno, tdate)
                        Else
                            ' Generate empty chart
                            GenerateEmptyChart()
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("DISPLAY_CHART_ERROR", "Error displaying chart: " & ex.Message)
            GenerateEmptyChart()
        End Try
    End Sub
    
    Private Function CheckIfVehicleHas2Tanks(plateno As String, conn As SqlConnection) As Boolean
        Try
            Dim query As String = "SELECT COUNT(*) FROM fuel_tank_check WHERE plateno = @plateno AND tankno = '2'"
            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                cmd.Parameters.AddWithValue("@plateno", plateno)
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                Return count > 0
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function
    
    Private Sub GenerateChart(vehicleRecordList As List(Of VehicleRecord), username As String, plateno As String, tdate As Date)
        Try
            Dim dataY1(vehicleRecordList.Count - 1) As Double
            Dim dataY2(vehicleRecordList.Count - 1) As Double
            Dim dataY3(vehicleRecordList.Count - 1) As Double
            Dim dataY4(vehicleRecordList.Count - 1) As Double
            Dim dataX1(vehicleRecordList.Count - 1) As String
            
            Dim maxvolume As Double = 0
            Dim maxlevel As Double = 0
            
            For j As Integer = 0 To vehicleRecordList.Count - 1
                dataX1(j) = vehicleRecordList(j).timeStamp.ToString("yyyy/MM/dd HH:mm:ss")
                dataY1(j) = vehicleRecordList(j).level
                dataY2(j) = vehicleRecordList(j).volumn
                dataY3(j) = vehicleRecordList(j).odometer
                dataY4(j) = vehicleRecordList(j).speed
                
                If maxvolume < vehicleRecordList(j).volumn Then
                    maxvolume = vehicleRecordList(j).volumn
                End If
                
                If maxlevel < vehicleRecordList(j).level Then
                    maxlevel = vehicleRecordList(j).level
                End If
            Next
            
            ' Create chart
            Dim c As XYChart = New XYChart(750, 450)
            c.addTitle(username.ToUpper() & "  -  " & plateno & "  -  " & tdate.ToString("dd MMMM yyyy"), "Arial Bold", 10, &H9900).setBackground(&HFFFFFF)
            c.setPlotArea(130, 25, 500, 300, &HF4FDEF).setGridColor(&HCCCCCC, &HCCCCCC)
            
            Dim legendBox As LegendBox = c.addLegend(370, 20, False, "Arial Bold", 8)
            legendBox.setAlignment(Chart.BottomCenter)
            legendBox.setBackground(Chart.Transparent, Chart.Transparent)
            
            c.xAxis().setLabels(dataX1)
            c.xAxis().setLabels(dataX1).setFontAngle(45)
            
            If dataX1.Length <= 24 Then
                c.xAxis().setLabelStep(3, 1)
            Else
                c.xAxis().setLabelStep(Convert.ToInt32(dataX1.Length / 24))
            End If
            
            c.xAxis().setTitle("Timestamp")
            
            ' Set up axes
            c.yAxis().setTitle("Odometer (Km)")
            c.yAxis().setColors(&HCC0000, &HCC0000, &HCC0000)
            
            c.yAxis2().setTitle("Tank (mm)")
            c.yAxis2().setColors(&H8000, &H8000, &H8000)
            
            Dim leftAxis As Axis = c.addAxis(Chart.Left, 60)
            leftAxis.setTitle("Speed (Km/h)")
            leftAxis.setColors(&HCC, &HCC, &HCC)
            
            Dim rightAxis As Axis = c.addAxis(Chart.Right, 50)
            rightAxis.setTitle("Tank (l)")
            rightAxis.setColors(&H880088, &H880088, &H880088)
            
            ' Set scales
            c.yAxis().setLinearScale(dataY3(0) - 100, dataY3(dataY3.Length - 1) + 100)
            
            If maxlevel < 700 And maxvolume < 550 Then
                c.yAxis2().setLinearScale(0, 700)
                rightAxis.setLinearScale(0, 550)
            Else
                c.yAxis2().setLinearScale(0, maxlevel + 100)
                rightAxis.setLinearScale(0, maxvolume + 100)
            End If
            
            ' Add data layers
            Dim layer0 As LineLayer = c.addLineLayer(dataY3, &HCC0000, "Odometer (KM)")
            layer0.setLineWidth(2)
            
            Dim layer1 As LineLayer = c.addLineLayer(dataY1, &H8000, "Tank Level (MM)")
            layer1.setLineWidth(2)
            layer1.setUseYAxis2()
            
            Dim layer2 As LineLayer = c.addLineLayer(dataY4, &HCC, "Speed (KM/H)")
            layer2.setUseYAxis(leftAxis)
            
            Dim layer3 As LineLayer = c.addLineLayer(dataY2, &H880088, "Tank Volume (L)")
            layer3.setLineWidth(2)
            layer3.setUseYAxis(rightAxis)
            
            Response.BinaryWrite(c.makeChart2(1))
            Response.ContentType = "image/gif"
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GENERATE_CHART_ERROR", "Error generating chart: " & ex.Message)
            GenerateEmptyChart()
        End Try
    End Sub
    
    Private Sub GenerateEmptyChart()
        Try
            Dim c As XYChart = New XYChart(750, 450)
            c.addTitle("No Data Available", "Arial Bold", 14, &HFF0000)
            c.setPlotArea(130, 100, 500, 300, &HF4FDEF)
            
            Response.BinaryWrite(c.makeChart2(1))
            Response.ContentType = "image/gif"
        Catch ex As Exception
            Response.StatusCode = 500
            Response.End()
        End Try
    End Sub

    Structure VehicleRecord
        Dim timeStamp As DateTime
        Dim seconds As Double
        Dim gpsAV As Char
        Dim lat As Double
        Dim lon As Double
        Dim speed As Double
        Dim odometer As Double
        Dim ignition As Boolean
        Dim level As Double
        Dim volumn As Double
    End Structure
End Class