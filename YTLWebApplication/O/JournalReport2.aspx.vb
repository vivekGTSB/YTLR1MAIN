Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Collections.Generic

Partial Class JournalReport2
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Public plateno As String
    Public companyname_global As String
    Dim dsPrice As New Data.DataSet
    Public statisticstable As New DataTable
    Dim addressFunction As New Address()

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim cmd As SqlCommand
            Dim dr As SqlDataReader

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
          
            cmd = New SqlCommand("select userid, username from userTBL where role='User' order by username", conn)
            If role = "User" Then
                cmd = New SqlCommand("select userid, username from userTBL where userid='" & userid & "'", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid, username from userTBL where userid in (" & userslist & ") order by username", conn)
            End If
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While

            If role = "User" Then
                ddlUsername.Items.Remove("--Select User Name--")
                ddlUsername.SelectedValue = userid
            End If
            dr.Close()
            cmd.Dispose()
            conn.Close()
            conn.Dispose()
      
        Catch ex As Exception

        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            End If
            ImageButton1.Attributes.Add("onclick", "return mysubmit()")

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click

        Try

            'Dim oWatch As New System.Diagnostics.Stopwatch
            'oWatch.Start()
            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":59"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            Dim count As Integer = DateDiff(DateInterval.Day, Convert.ToDateTime(begindatetime), Convert.ToDateTime(enddatetime))
            If (count > 2 And cbRoute.Checked = True) Then
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, GetType(Page), "Script", "alert('Please select 1 ~ 3 day only, in order to display route information');", True)
            Else
                If ddlUsername.SelectedValue <> "--Select User Name--" And ddlGroupName.SelectedValue <> "--Group Name--" Then
                    DisplayIdlingInformation()
                    ColorMessage.Visible = True
                End If
            End If
            'oWatch.Stop()
            'lblWatch0.Text = "Processing Time: " & CDbl(oWatch.ElapsedMilliseconds.ToString()) / 1000 & " seconds"
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub DisplayIdlingInformation()
        Try

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

             Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        
            Dim cmd, cmd2 As SqlCommand
            Dim dr1, dr2 As SqlDataReader
            Dim r As DataRow

            Dim x As Int32 = 1
            Dim stoptemtime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim idletemtime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim movingtemtime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim starttime As String = ""
            Dim endtime As String = ""

            Dim startaddress As String = ""
            Dim endaddress As String = ""
            Dim startlat As Double = 0
            Dim startlon As Double = 0
            Dim endlat As Double = 0
            Dim endlon As Double = 0

            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            Dim companyname As String = Request.Cookies("userinfo")("companyname")
            companyname_global = companyname

            Dim totalStop As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)
            Dim totalIdling As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)
            Dim totalTraveling As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)
            Dim totalSpeed90 As Integer = 0
            Dim totalSpeed95 As Integer = 0
            Dim totalOdometer As Double = 0
            Dim totalFuel As Double = 0
            Dim totalRefuel As Double = 0
            Dim speed_90 As Integer = 0
            Dim speed_95 As Integer = 0

            Dim locObj As New Location(ddlUsername.SelectedValue)

            Dim t As New DataTable

            t.Columns.Add(New DataColumn("No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Start"))
            t.Columns.Add(New DataColumn("End"))
            t.Columns.Add(New DataColumn("Stop"))
            t.Columns.Add(New DataColumn("Idling"))
            t.Columns.Add(New DataColumn("Travelling"))
            t.Columns.Add(New DataColumn("Start Location1"))
            t.Columns.Add(New DataColumn("End Location1"))
            t.Columns.Add(New DataColumn("start Location"))
            t.Columns.Add(New DataColumn("End Location"))
            t.Columns.Add(New DataColumn("Groupname"))
            t.Columns.Add(New DataColumn("Mileage", GetType(Double)))
            t.Columns.Add(New DataColumn("Fuel", GetType(Double)))
            t.Columns.Add(New DataColumn("LiterRefuel", GetType(Double)))
            t.Columns.Add(New DataColumn("speed95", GetType(Double)))
            t.Columns.Add(New DataColumn("speed90", GetType(Double)))
            t.Columns.Add(New DataColumn("route"))
            t.Columns.Add(New DataColumn("idlingCost", GetType(Double)))
            t.Columns.Add(New DataColumn("IdlingLiter", GetType(Double)))
            t.Columns.Add(New DataColumn("Stop_min", GetType(Double)))
            t.Columns.Add(New DataColumn("Idling_min", GetType(Double)))
            t.Columns.Add(New DataColumn("Travelling_min", GetType(Double)))
            t.Columns.Add(New DataColumn("PTO"))

            If ddlGroupName.SelectedValue = "All" Then
                cmd = New SqlCommand("select plateno,Groupname from vehicleTBL where userid='" & ddlUsername.SelectedValue & "' order by plateno", conn)
            Else
                cmd = New SqlCommand("select plateno,Groupname from vehicleTBL where userid='" & ddlUsername.SelectedValue & "' and groupname = '" & ddlGroupName.SelectedValue & "' order by plateno", conn)
            End If

            'Dim da_geofence As SqlDataAdapter
            'Dim dsGeofenceList As New DataSet
            'conn2.Open()
            'da_geofence = New SqlDataAdapter("select lat, lon, geofencename from geofence_polygon", conn2)
            'da_geofence.Fill(dsGeofenceList)
            'conn2.Close()

            conn.Open()

            dr1 = cmd.ExecuteReader()
            cmd.Dispose()
            Dim plateno As String = ""
            While dr1.Read()
                plateno = dr1("plateno")
                ''(gps_av='A' or (gps_av='V' and ignition_sensor='0'))
                Dim da As SqlDataAdapter = New SqlDataAdapter("select distinct convert(varchar(19),timestamp,120) as datetime,speed,ignition_sensor,lat,lon, alarm from vehicle_history where plateno ='" & plateno & "' and gps_av='A' and timestamp between '" & begindatetime & "' and '" & enddatetime & "' order by datetime asc", conn)

                Dim dsIdling As DataSet = New DataSet()
                da.Fill(dsIdling)

                Dim prevstatus As String = "Traveling"
                Dim presentstatus As String = "Traveling"

                Dim prevTime As DateTime
                Dim presenttime As DateTime

                Dim tempprevtime As DateTime
                Dim totalidelingtime As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)

                If dsIdling.Tables(0).Rows.Count > 0 Then
                    prevTime = dsIdling.Tables(0).Rows(0)("datetime")
                    presenttime = prevTime
                    tempprevtime = prevTime
                    starttime = prevTime.ToString("yyyy-MM-dd HH:mm:ss")
                    startlat = dsIdling.Tables(0).Rows(0)("lat")
                    startlon = dsIdling.Tables(0).Rows(0)("lon")
                    If (dsIdling.Tables(0).Rows(0)("ignition_sensor") = 1) And (dsIdling.Tables(0).Rows(0)("speed") = 0) Then ' And dsIdling.Tables(0).Rows(0)("gps_av") = "A" Then
                        prevstatus = "Ideling"
                    ElseIf (dsIdling.Tables(0).Rows(0)("ignition_sensor") = 0) Then
                        prevstatus = "Stop"
                    Else
                        prevstatus = "Traveling"
                    End If
                End If

                speed_95 = 0
                speed_90 = 0

                Dim PTO As String = ""
                Dim count3 As Integer = 0
                Dim count4 As Integer = 1

                For i As Int32 = 1 To dsIdling.Tables(0).Rows.Count - 1
                    presenttime = dsIdling.Tables(0).Rows(i)("datetime")
                    endlat = dsIdling.Tables(0).Rows(i)("lat")
                    endlon = dsIdling.Tables(0).Rows(i)("lon")

                    If (dsIdling.Tables(0).Rows(i)("ignition_sensor") = 1) And (dsIdling.Tables(0).Rows(i)("speed") = 0) Then ' And dsIdling.Tables(0).Rows(i)("gps_av") = "A" Then
                        presentstatus = "Ideling"
                    ElseIf (dsIdling.Tables(0).Rows(i)("ignition_sensor") = 0) Then
                        presentstatus = "Stop"
                    Else
                        presentstatus = "Traveling"
                    End If

                    If dsIdling.Tables(0).Rows(i)("alarm") = "ON" And count3 = 0 Then
                        PTO += count4.ToString() + ". PTO start at : " & Convert.ToDateTime(dsIdling.Tables(0).Rows(i)("datetime")).ToString("yyyy-MM-dd H:mm") & " and "
                        count3 += 1
                        count4 += 1
                    ElseIf dsIdling.Tables(0).Rows(i)("alarm") = "OFF" And count3 = 1 Then
                        PTO += " end at : " & Convert.ToDateTime(dsIdling.Tables(0).Rows(i)("datetime")).ToString("yyyy-MM-dd H:mm") & "<br/>"
                        count3 = 0
                    End If

                    If prevstatus <> presentstatus Then
                        If prevTime <> tempprevtime Then
                            Dim tfirstdatetime As DateTime = prevTime
                            Dim tseconddatetime As DateTime = tempprevtime 'presenttime
                            Dim temptime As TimeSpan = tseconddatetime - tfirstdatetime

                            If temptime.TotalMinutes >= 1 Then
                                If prevstatus = "Ideling" Then
                                    totalidelingtime += temptime
                                    totalIdling = totalIdling + temptime

                                ElseIf prevstatus = "Stop" Then
                                    'stoptemtime += temptime
                                    'totalStop = totalStop + temptime
                                Else
                                    movingtemtime += temptime
                                    totalTraveling = totalTraveling + temptime
                                End If
                            End If
                        End If

                        prevTime = presenttime
                        prevstatus = presentstatus
                    End If

                    tempprevtime = presenttime
                    endtime = presenttime.ToString("yyyy-MM-dd HH:mm:ss")

                    If (dsIdling.Tables(0).Rows(i)("speed") > 95) Then
                        speed_95 += 1
                    ElseIf (dsIdling.Tables(0).Rows(i)("speed") > 90) Then
                        speed_90 += 1
                    End If

                Next

                totalSpeed90 += speed_90
                totalSpeed95 += speed_95

                If prevTime <> presenttime Then
                    Dim tfirstdatetime As DateTime = prevTime
                    Dim tseconddatetime As DateTime = presenttime

                    Dim temptime As TimeSpan = tseconddatetime - tfirstdatetime
                    If prevstatus = "Stop" Then
                        'stoptemtime += temptime
                    ElseIf prevstatus = "Ideling" Then
                        'temptime = temptime.Add(TimeSpan.Parse("00:01:00"))  'purposely add in to match the vehicleidlingreport
                        totalidelingtime += temptime
                        totalIdling = totalIdling + temptime

                    ElseIf prevstatus = "Traveling" Then
                        'temptime = temptime.Add(TimeSpan.Parse("00:01:00"))  'purposely add in to match the vehicleidlingreport
                        movingtemtime += temptime
                        totalTraveling = totalTraveling + temptime
                    End If

                    If dsIdling.Tables(0).Rows.Count > 0 Then
                        endtime = presenttime.ToString("yyyy-MM-dd HH:mm:ss")
                    End If
                Else
                    If dsIdling.Tables(0).Rows.Count > 0 Then
                        endtime = presenttime.ToString("yyyy-MM-dd HH:mm:ss")
                    End If
                End If

                If txtEndDate.Value = Now().ToString("yyyy/MM/dd") Then
                    stoptemtime = DateTime.Parse(Now.ToString("yyyy-MM-dd HH:mm:ss")) - DateTime.Parse(begindatetime) - totalidelingtime - movingtemtime
                Else
                    stoptemtime = DateTime.Parse(enddatetime) - DateTime.Parse(begindatetime) - totalidelingtime - movingtemtime
                    stoptemtime = stoptemtime.Add(TimeSpan.Parse("00:00:59"))
                End If

                totalStop = totalStop + stoptemtime

                Dim Mileage As Double = 0
                Dim FuelConsumption As Double = 0
                Dim refuelCount As Double = 0
                'If cbFuel.Checked = True Then

                Dim fuelobj As New FuelMath1(plateno)
                'Dim dFuel As New RefuelBeta(plateno, begindatetime, enddatetime)
                Dim dieselPrice As Double
                Dim dTable As New DataTable
                'Dim dPrice As New DataTable

                ''''''''''''refuel''''''''''''''''''
                'dTable = dFuel.rTable
                'dPrice = dFuel.fuelPrice()

                'For i As Int32 = 0 To dTable.Rows.Count - 1
                '    Dim temp1 As Double = CDbl(fuelobj.CalcTankVolume(dTable.Rows(i)("BeforeLevel")) + fuelobj.CalcTankVolume2(dTable.Rows(i)("BeforeLevel2"))).ToString("0.00")
                '    Dim temp2 As Double = CDbl(fuelobj.CalcTankVolume(dTable.Rows(i)("AfterLevel")) + fuelobj.CalcTankVolume2(dTable.Rows(i)("AfterLevel2"))).ToString("0.00")
                '    Dim result As Double = temp2 - temp1
                '    If result > 20 Then
                '        refuelCount += CDbl(result).ToString("0.00")
                '    End If
                'Next
                'totalRefuel += refuelCount
                ''''''''''''end of refuel''''''''''''''''''

                ''''''''''' odometer and fuel comsumption'''''''''
                Dim dateCounter As Integer = DateDiff(DateInterval.Day, Convert.ToDateTime(begindatetime), Convert.ToDateTime(enddatetime))

                For Count As Int32 = 0 To dateCounter
                    Dim dFuel2 As New RefuelBeta3(plateno, Convert.ToDateTime(begindatetime).AddDays(Count).ToString("yyyy/MM/dd 00:00:00"), Convert.ToDateTime(begindatetime).AddDays(Count).ToString("yyyy/MM/dd 23:59:59"))

                    ''If Count = 0 Then
                    dTable = dFuel2.rTable

                    For i As Int32 = 0 To dTable.Rows.Count - 1
                        Dim temp1 As Double = CDbl(fuelobj.CalcTankVolume(dTable.Rows(i)("BeforeLevel")) + fuelobj.CalcTankVolume2(dTable.Rows(i)("BeforeLevel2"))).ToString("0.00")
                        Dim temp2 As Double = CDbl(fuelobj.CalcTankVolume(dTable.Rows(i)("AfterLevel")) + fuelobj.CalcTankVolume2(dTable.Rows(i)("AfterLevel2"))).ToString("0.00")
                        Dim result As Double = temp2 - temp1
                        If result > 20 Then
                            refuelCount += CDbl(result).ToString("0.00")
                        End If
                    Next
                    ''End If

                    'If dFuel2.fuelstartdate <> "" Then

                    Mileage += dFuel2.fuelOdometerTotal
                    If dFuel2.fuelConsumptionTotal > 0 Then
                        FuelConsumption += dFuel2.fuelConsumptionTotal
                    End If
                    'End If
                Next
                totalFuel += FuelConsumption
                totalOdometer += Mileage
                totalRefuel += refuelCount
                ''''''''''' End of odometer and fuel comsumption'''''''''

                ''''''''''geofence'''''''''''''
                Dim prevgeofencename As String = ""
                ''''''''''end of geofence''''''

                r = t.NewRow
                r(0) = x
                r(1) = plateno
                r(2) = starttime
                r(3) = endtime
                r(4) = stoptemtime
                r(5) = totalidelingtime
                r(6) = movingtemtime
                r(20) = stoptemtime.TotalSeconds
                r(21) = totalidelingtime.TotalSeconds
                r(22) = movingtemtime.TotalSeconds
                'r(7) = getAddress(startlat, startlon)
                If cbLocation.Checked Then
                    r(7) = locObj.GetLocation(startlat, startlon)
                    r(8) = locObj.GetLocation(endlat, endlon)
                Else
                    r(7) = ""
                    r(8) = ""
                End If

                r(11) = dr1("GroupName")
                If r(4) = "00:00:00" And r(5) = "00:00:00" And r(6) = "00:00:00" Then
                    r(4) = "-"
                    r(5) = "-"
                    r(6) = "-"
                End If

                If r(7) Is DBNull.Value Then
                    r(7) = "-"
                    r(9) = "-"
                End If

                If r(8) Is DBNull.Value Then
                    r(8) = "-"
                    r(10) = "-"
                End If
                r(12) = Mileage
                r(13) = FuelConsumption
                r(14) = refuelCount
                r(15) = speed_95
                r(16) = speed_90
                Try
                    r(17) = prevgeofencename 'prevgeofencename.Substring(0, prevgeofencename.Length - 3)
                Catch ex As Exception
                    r(17) = ""
                End Try
                r(19) = ((3 / 60) * Convert.ToDouble(totalidelingtime.TotalMinutes.ToString())).ToString("N2")

                'Dim dFuel As New RefuelBeta(ddlpleate.SelectedValue, begindatetime, enddatetime)

                'Dim drPrice2 As DataRow() = dPrice.Select("StartDate <= #" & starttime & "# And EndDate >= #" & fuelstartdate & "#")
                'dieselPrice = CDbl(drPrice2(0)(2))

                r(18) = Convert.ToDouble(r(19) * 1.8).ToString("N2")
                If PTO = "" Then
                    PTO = "OFF"
                End If
                r(23) = PTO

                t.Rows.Add(r)
                x = x + 1

                presenttime = New DateTime
                prevTime = New DateTime
                startlat = 0
                startlon = 0
                endlat = 0
                endlon = 0
                plateno = ""
                starttime = "-"
                endtime = "-"
                startaddress = "-"
                endaddress = "-"
                stoptemtime = New TimeSpan(0, 0, 0)
                totalidelingtime = New TimeSpan(0, 0, 0)
                movingtemtime = New TimeSpan(0, 0, 0)
                'GridView1.PageSize = noofrecords.SelectedValue
                'GridView1.DataSource = t
                'GridView1.DataBind()
                dsIdling.Dispose()
                da.Dispose()
            End While
            'r = t.NewRow
            'r(0) = ""
            'r(1) = ""
            'r(2) = ""
            'r(3) = "TOTAL"
            'r(4) = totalStop
            'r(5) = totalIdling
            'r(6) = totalTraveling
            'r(7) = ""
            't.Rows.Add(r)

            If t.Rows.Count = 0 Then
                r = t.NewRow
                r(0) = "--"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                r(5) = "--"
                r(6) = "--"
                r(7) = "--"
                r(8) = "--"
                r(9) = "--"
                r(10) = "--"
                r(11) = "--"
                r(12) = "--"
                r(13) = "--"
                r(14) = "--"
                r(15) = "--"
                r(16) = "--"
                r(17) = "--"

                t.Rows.Add(r)
            End If

            GridView1.PageSize = noofrecords.SelectedValue
            GridView1.DataSource = t
            GridView1.DataBind()

            '' This part use for email
            '' description: get top 10 of idling, speed, traveling and odometer
            Dim TOP10_Idling As New DataTable
            t.DefaultView.Sort = "Idling DESC"
            TOP10_Idling = t.DefaultView.ToTable()

            Dim TOP10_Speed90 As New DataTable
            t.DefaultView.Sort = "speed90 DESC"
            TOP10_Speed90 = t.DefaultView.ToTable()

            Dim TOP10_Speed95 As New DataTable
            t.DefaultView.Sort = "speed95 DESC"
            TOP10_Speed95 = t.DefaultView.ToTable()

            Dim TOP10_Travelling As New DataTable
            t.DefaultView.Sort = "Travelling DESC"
            TOP10_Travelling = t.DefaultView.ToTable()

            Dim TOP10_Mileage As New DataTable
            t.DefaultView.Sort = "Mileage DESC"
            TOP10_Mileage = t.DefaultView.ToTable()

            Dim top10 As New DataTable
            Dim Top10_r As DataRow

            top10.Columns.Add(New DataColumn("Plate No"))
            top10.Columns.Add(New DataColumn("Idlling"))
            top10.Columns.Add(New DataColumn("Plate No2"))
            top10.Columns.Add(New DataColumn("speed90"))
            top10.Columns.Add(New DataColumn("Plate No3"))
            top10.Columns.Add(New DataColumn("speed95"))
            top10.Columns.Add(New DataColumn("Plate No4"))
            top10.Columns.Add(New DataColumn("Travelling"))
            top10.Columns.Add(New DataColumn("Plate No5"))
            top10.Columns.Add(New DataColumn("Mileage", GetType(Double)))
            top10.Columns.Add(New DataColumn("no"))

            For i As Integer = 0 To 9
                Top10_r = top10.NewRow
                Top10_r(0) = TOP10_Idling.Rows(i)("Plate No")
                Top10_r(1) = TOP10_Idling.Rows(i)("Idling")
                Top10_r(2) = TOP10_Speed90.Rows(i)("Plate No")
                Top10_r(3) = TOP10_Speed90.Rows(i)("speed90")
                Top10_r(4) = TOP10_Speed95.Rows(i)("Plate No")
                Top10_r(5) = TOP10_Speed95.Rows(i)("speed95")
                Top10_r(6) = TOP10_Travelling.Rows(i)("Plate No")
                Top10_r(7) = TOP10_Travelling.Rows(i)("Travelling")
                Top10_r(8) = TOP10_Mileage.Rows(i)("Plate No")
                Top10_r(9) = TOP10_Mileage.Rows(i)("Mileage")
                Top10_r(10) = i + 1
                top10.Rows.Add(Top10_r)
            Next

            GridView4.DataSource = top10
            GridView4.DataBind()
            '' end of part use for email
            DirectCast(GridView1.FooterRow.FindControl("lblReFuel"), Label).Text = totalRefuel.ToString("N2")
            DirectCast(GridView1.FooterRow.FindControl("lblFuel"), Label).Text = totalFuel.ToString("N2")
            DirectCast(GridView1.FooterRow.FindControl("lblOdomerter"), Label).Text = totalOdometer.ToString("N2")
            DirectCast(GridView1.FooterRow.FindControl("lblSpeed90"), Label).Text = totalSpeed90.ToString()
            DirectCast(GridView1.FooterRow.FindControl("lblSpeed95"), Label).Text = totalSpeed95.ToString()
            DirectCast(GridView1.FooterRow.FindControl("lblStop"), Label).Text = totalStop.ToString()
            DirectCast(GridView1.FooterRow.FindControl("lblidling"), Label).Text = totalIdling.ToString()
            DirectCast(GridView1.FooterRow.FindControl("lbltravelling"), Label).Text = totalTraveling.ToString()
            ec = "true"

            If cbVehicleMovement.Checked = False Then
                GridView1.Columns(5).Visible = False
                GridView1.Columns(6).Visible = False
                GridView1.Columns(7).Visible = False
            Else
                GridView1.Columns(5).Visible = True
                GridView1.Columns(6).Visible = True
                GridView1.Columns(7).Visible = True
            End If

            If cbLocation.Checked = False Then
                GridView1.Columns(13).Visible = False
                GridView1.Columns(12).Visible = False
            Else
                GridView1.Columns(13).Visible = True
                GridView1.Columns(12).Visible = True
            End If

            If cbFuel.Checked = False Then
                GridView1.Columns(16).Visible = False
                GridView1.Columns(14).Visible = False
                GridView1.Columns(15).Visible = False
            Else
                GridView1.Columns(16).Visible = True
                GridView1.Columns(14).Visible = True
                GridView1.Columns(15).Visible = True
            End If

            If cbSpeeding.Checked = False Then
                GridView1.Columns(10).Visible = False
                GridView1.Columns(11).Visible = False
            Else
                GridView1.Columns(10).Visible = True
                GridView1.Columns(11).Visible = True
            End If

            If cbRoute.Checked = False Then
                GridView1.Columns(18).Visible = False
            Else
                GridView1.Columns(18).Visible = True
            End If

            Session.Remove("exceltable")
            Session.Remove("exceltable2")

            Session("exceltable") = t
            GridView1.Dispose()
            conn.Close()
            conn.Dispose()
            cmd.Dispose()
            dr1.Close()
            cmd.Dispose()
            r.Table.Dispose()
            t.Dispose()
            If GridView1.PageCount > 1 Then
                show = True
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging

        GridView1.PageSize = noofrecords.SelectedValue
        GridView1.DataSource = Session("exceltable")
        GridView1.PageIndex = e.NewPageIndex
        GridView1.DataBind()

        ec = "true"
        show = True

    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                If cbRoute.Checked = True Then
                    Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
                    Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
                    Dim GetPlateNo As String = DirectCast(e.Row.FindControl("Label1"), WebControls.Label).Text

                    Dim dt As DataTable = GetIdling_ang(GetPlateNo, "15", begindatetime, enddatetime, ddlUsername.SelectedValue, companyname_global)
                    'Dim dt2 As DataTable = GetIdling_ang2(GetPlateNo, "15", begindatetime, enddatetime, ddlUsername.SelectedValue, companyname)
                    'GetIdling2.
                    DirectCast(e.Row.FindControl("GridView2"), WebControls.GridView).DataSource = dt
                    DirectCast(e.Row.FindControl("GridView2"), WebControls.GridView).DataBind()
                End If
            End If
            If e.Row.RowType = DataControlRowType.Footer Then
                e.Row.Style.Add("BORDER-TOP", "BLACK 3px solid")
                e.Row.Style.Add("BORDER-BOTTOM", "BLACK 3px solid")
            End If
        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        If (ddlUsername.SelectedIndex <> 0) Then
            Dim cmd As SqlCommand
            Dim dr As SqlDataReader

            ddlGroupName.Items.Clear()
            ddlGroupName.Items.Add(New ListItem("All", "All"))

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            cmd = New SqlCommand("select distinct groupname from vehicleTBL where userid = " & ddlUsername.SelectedValue & " order by groupname", conn)

            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlGroupName.Items.Add(New ListItem(dr("groupname"), dr("groupname")))
            End While

            dr.Close()
            cmd.Dispose()
            conn.Close()
            conn.Dispose()
        End If
    End Sub

    Protected Sub cbVehicleMovement_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbVehicleMovement.CheckedChanged
        Try
            If cbVehicleMovement.Checked = False Then
                GridView1.Columns(5).Visible = False
                GridView1.Columns(6).Visible = False
                GridView1.Columns(7).Visible = False
            Else
                GridView1.Columns(5).Visible = True
                GridView1.Columns(6).Visible = True
                GridView1.Columns(7).Visible = True
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub cbFuel_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbFuel.CheckedChanged
        Try
            If cbFuel.Checked = False Then
                GridView1.Columns(16).Visible = False
                GridView1.Columns(14).Visible = False
                GridView1.Columns(15).Visible = False
            Else
                GridView1.Columns(16).Visible = True
                GridView1.Columns(14).Visible = True
                GridView1.Columns(15).Visible = True
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub gvSorting(ByVal SortExpression As String, ByVal SortDirection As String, ByVal tooptipDirection As String, ByVal id As String)

        Dim t As DataTable
        t = Session("exceltable")
        t.DefaultView.Sort = SortExpression & " " & SortDirection
        t = t.DefaultView.ToTable()

        Dim totalRefuel As String = DirectCast(GridView1.FooterRow.FindControl("lblReFuel"), Label).Text
        Dim totalFuel As String = DirectCast(GridView1.FooterRow.FindControl("lblFuel"), Label).Text
        Dim totalOdometer As String = DirectCast(GridView1.FooterRow.FindControl("lblOdomerter"), Label).Text
        Dim totalSpeed90 As String = DirectCast(GridView1.FooterRow.FindControl("lblSpeed90"), Label).Text
        Dim totalSpeed95 As String = DirectCast(GridView1.FooterRow.FindControl("lblSpeed95"), Label).Text
        Dim totalStop As String = DirectCast(GridView1.FooterRow.FindControl("lblStop"), Label).Text
        Dim totalIdling As String = DirectCast(GridView1.FooterRow.FindControl("lblidling"), Label).Text
        Dim totalTraveling As String = DirectCast(GridView1.FooterRow.FindControl("lbltravelling"), Label).Text

        GridView1.DataSource = t
        GridView1.DataBind()

        DirectCast(GridView1.FooterRow.FindControl("lblReFuel"), Label).Text = totalRefuel.ToString()
        DirectCast(GridView1.FooterRow.FindControl("lblFuel"), Label).Text = totalFuel.ToString()
        DirectCast(GridView1.FooterRow.FindControl("lblOdomerter"), Label).Text = totalOdometer.ToString()
        DirectCast(GridView1.FooterRow.FindControl("lblSpeed90"), Label).Text = totalSpeed90.ToString()
        DirectCast(GridView1.FooterRow.FindControl("lblSpeed95"), Label).Text = totalSpeed95.ToString()
        DirectCast(GridView1.FooterRow.FindControl("lblStop"), Label).Text = totalStop.ToString()
        DirectCast(GridView1.FooterRow.FindControl("lblidling"), Label).Text = totalIdling.ToString()
        DirectCast(GridView1.FooterRow.FindControl("lbltravelling"), Label).Text = totalTraveling.ToString()

        DirectCast(GridView1.HeaderRow.FindControl(id), LinkButton).ToolTip = tooptipDirection

    End Sub

    Protected Sub LinkButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim id As String = DirectCast(sender, LinkButton).ID.ToString()
        Dim SortExpression As String = DirectCast(sender, LinkButton).SkinID.ToString()
        Dim SortDirection As String = DirectCast(sender, LinkButton).ToolTip.ToString()
        Dim tooptipDirection As String

        If SortDirection = "Ascending" Then
            SortDirection = "asc"
            tooptipDirection = "Descending"
        Else
            SortDirection = "desc"
            tooptipDirection = "Ascending"
        End If
        gvSorting(SortExpression, SortDirection, tooptipDirection, id)
    End Sub

    Protected Sub cbLocation_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbLocation.CheckedChanged
        If cbLocation.Checked = False Then
            GridView1.Columns(12).Visible = False
            GridView1.Columns(13).Visible = False
        Else
            GridView1.Columns(13).Visible = True
            GridView1.Columns(12).Visible = True
        End If
    End Sub

    Protected Sub cbSpeeding_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbSpeeding.CheckedChanged
        If cbSpeeding.Checked = False Then
            GridView1.Columns(10).Visible = False
            GridView1.Columns(11).Visible = False
        Else
            GridView1.Columns(10).Visible = True
            GridView1.Columns(11).Visible = True
        End If
    End Sub

    Public Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)

    End Sub

    Protected Sub btnExport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnExport.Click
        Response.ClearContent()
        Response.Buffer = True
        Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", "JournalReport.xls"))
        Response.ContentType = "application/ms-excel"
        Dim sw As New StringWriter()
        Dim htw As New HtmlTextWriter(sw)

        'GridView2.DataBind()
        htw.Write("<b style='color:#005583'>Journal Report</b>")
        htw.Write("<br><br>")
        htw.Write("User Name : " & ddlUsername.SelectedItem.ToString())
        htw.Write("<br>")
        htw.Write("Report Date : " & DateTime.Now)
        GridView1.Font.Name = "Verdana"

        GridView1.RenderControl(htw)
        sw.ToString()

        Response.Write(sw.ToString())
        Response.[End]()

    End Sub

    Protected Sub cbRoute_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbRoute.CheckedChanged
        Try
            If cbRoute.Checked = False Then
                GridView1.Columns(18).Visible = False
            Else
                GridView1.Columns(18).Visible = True
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Function GetIdlingData(ByVal plateno As String, ByVal IdlingMin As String, ByVal startDT As String, ByVal endDT As String, ByVal UID As String) As DataTable

        Dim begindatetime As String = startDT
        Dim enddatetime As String = endDT

        Dim connection As New Redirect(UID)
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))

        Dim cmd As SqlCommand = New SqlCommand("select distinct convert(varchar(19),timestamp,120) as datetime,speed,ignition_sensor,lon,lat from vehicle_history where plateno ='" & plateno & "' and (gps_av='A' or (gps_av='V' and ignition_sensor='0')) and  timestamp between '" & begindatetime & "' and '" & enddatetime & "' order by datetime asc", conn)

        Dim t As New DataTable
        Dim r As DataRow

        t.Columns.Add("lat")
        t.Columns.Add("lon")
        t.Columns.Add("IdlingTime")
        t.Columns.Add("StartAndEndDate")
        t.Columns.Add("StartDate")

        Dim stoptime As TimeSpan = New TimeSpan(0, 0, 0)
        Dim ideltime As TimeSpan = New TimeSpan(0, 0, 0)
        Dim movingtime As TimeSpan = New TimeSpan(0, 0, 0)

        Dim lon() As Double = {}
        Dim lat() As Double = {}

        Dim plon As Double
        Dim plat As Double

        Dim stopsstring As String = "Stops" & Environment.NewLine
        Dim idelstring As String = "idel" & Environment.NewLine
        Dim movingstring As String = "moving" & Environment.NewLine

        Dim imagestatus As String = "no"
        Dim prevstatus As String = "stop"
        Dim prevtime As DateTime = DateTime.Parse(begindatetime)
        Dim prevLat As Double = 0
        Dim prevLon As Double = 0


        Dim currentstatus As String = "stop"
        Dim currenttime As DateTime = DateTime.Parse(begindatetime)


        Dim temptime As TimeSpan = New TimeSpan(0, 0, 0)
        Dim idling As TimeSpan = New TimeSpan(0, IdlingMin, 0)

        conn.Open()
        Dim dr As SqlDataReader = cmd.ExecuteReader()

        Dim i As Integer = 0
        Dim enter As String = "no"


        While dr.Read()


            imagestatus = "yes"

            If enter = "no" Then

                enter = "Yes"
            End If
            currenttime = dr("datetime")
            If dr("ignition_sensor") = 1 And dr("speed") > 10 Then
                currentstatus = "moving"
            ElseIf dr("ignition_sensor") = 1 And dr("speed") < 10 Then
                currentstatus = "idle"
            Else
                currentstatus = "stop"
            End If

            If prevstatus <> currentstatus Then
                temptime = currenttime - prevtime
                Dim minutes As Int16 = temptime.TotalMinutes()
                'Dim second As Int16 = temptime.TotalSeconds - (minutes * 60)

                Select Case prevstatus
                    Case "stop"

                    Case "moving"

                    Case "idle"

                        If temptime > New TimeSpan(0, 0, 0) Then

                            If temptime > idling Then

                                If prevLat = 0 Then
                                    prevLat = Math.Round(dr("lat"), 6)
                                    prevLon = Math.Round(dr("lon"), 6)
                                End If

                                r = t.NewRow()
                                r(0) = prevLat
                                r(1) = prevLon
                                r(2) = Math.Round(temptime.TotalMinutes, 2)
                                r(3) = prevtime.ToString("yyyy-MMM-dd HH:mm tt") & " To " & currenttime.ToString("HH:mm tt")
                                r(4) = prevtime.ToString("yyyy-MMM-dd HH:mm tt")
                                t.Rows.Add(r)

                            End If

                        End If

                End Select
                prevLat = Math.Round(dr("lat"), 6)
                prevLon = Math.Round(dr("lon"), 6)
                prevtime = currenttime
                prevstatus = currentstatus
            End If

        End While

        If prevtime <> currenttime Then

            temptime = currenttime - prevtime
            Dim minutes As Int16 = temptime.TotalMinutes()
            'Dim second As Int16 = temptime.TotalSeconds - (minutes * 60)
            Select Case prevstatus
                Case "stop"

                Case "moving"

                Case "idle"

                    If temptime > New TimeSpan(0, 0, 0) Then

                        If temptime > idling Then

                            r = t.NewRow()
                            r(0) = Math.Round(plat, 6)
                            r(1) = Math.Round(plon, 6)
                            r(2) = Math.Round(temptime.TotalMinutes, 2)
                            r(3) = prevtime.ToString("yyyy-MMM-dd HH:mm tt") & " To " & currenttime.ToString("HH:mm tt")
                            r(4) = prevtime.ToString("yyyy-MMM-dd HH:mm tt")
                            t.Rows.Add(r)

                        End If

                    End If

            End Select

            prevtime = currenttime
            prevstatus = currentstatus
        End If

        conn.Close()

        Return t

    End Function

    Public Function GetIdling_ang(ByVal plateno As String, ByVal IdlingMin As String, ByVal startDT As String, ByVal endDT As String, ByVal UID As String, ByVal companyname As String) As DataTable

        Dim prevgeofencename As String = ""

        Dim OnIgnition As String = ""
        Dim geofencename As String = ""
        Dim geofenceComplete As Boolean = False
        Dim OnDateTime As String = ""
        Dim currentDateTime As String = ""
        Dim onlat, onlon, offlat, offlon As Double
        Dim address As String = ""

        Dim prevgeofencestatus As Byte = 0
        Dim count As Integer = 0
        Dim temp1_Status As String = "0"
        Dim currentgeofencestatus As Byte = 0

        Dim xlat, ylon As Double
        Dim temp_lat As Double = 0
        Dim temp_lon As Double = 0
        Dim temp_Prelat As Double = 0
        Dim temp_Prelon As Double = 0
        Dim temp1 As String = ""
        Dim temp2 As String = ""
        Dim temp3 As String = ""
        Dim temp4 As String = ""
        Dim temp5 As String = ""
        Dim count_temp As Integer = 0
        Dim count_POI As Integer = 0
        Dim dictionary As New Dictionary(Of String, String)()
        Dim dictionary_POI As New Dictionary(Of String, String)()
        Dim dt_temp As New DataTable
        Dim r As DataRow

        dt_temp.Columns.Add(New DataColumn("address"))
        dt_temp.Columns.Add(New DataColumn("timestamp"))
        dt_temp.Columns.Add(New DataColumn("IdlingTime"))
        dt_temp.Columns.Add(New DataColumn("StartAndEndDate"))
        dt_temp.Columns.Add(New DataColumn("x"))
        dt_temp.Columns.Add(New DataColumn("y"))

        Dim locObj As New Location(UID)

        Dim dt As DataTable = GetIdlingData(plateno, IdlingMin, startDT, endDT, UID)
        For Each xxx As DataRow In dt.Rows

            temp_lat = xxx("lat")
            temp_lon = xxx("lon")

            Dim strAdd As String = locObj.GetPublicGeofence(temp_lat, temp_lon)

            If strAdd <> "" Then

                'If temp1 <> strAdd Then
                r = dt_temp.NewRow()
                ''Green Color
                'Dim temp As String = "<a style='color:#04B404;text-decoration: none;' href='GoogleEarthMaps.aspx?x=" & Math.Round(temp_lon, 6) & "&amp;y= " & Math.Round(temp_lat, 6) & "' target=""_blank"">" & strAdd & "</a>"
                'Dim temp As String = "<a style='color:#04B404;text-decoration: none;' href=""http://maps.google.com/maps?f=q&hl=en&q=" & Math.Round(temp_lat, 6) & " + " & Math.Round(temp_lon, 4) & "&om=1&t=k"" target=""_blank"">" & strAdd & "</a>"
                'prevgeofencename += "<div style='color:#04B404;'><div style='float:left;width:500px;'><span>" & temp & "</span></div>" & "<div style='text-align:left;float:left'>(" & xxx("IdlingTime").ToString() & " min) - " & xxx("StartAndEndDate").ToString() & "</div></div>" & "<br/>"
                'r(0) = "<div style='color:#04B404;'><div style='float:left;width:500px;'><span>" & temp & "</span></div>" & "<div style='text-align:left;float:left'>(" & xxx("IdlingTime").ToString() & " min) - " & xxx("StartAndEndDate").ToString() & "</div></div>" & "<br/>"
                r(0) = "<span style='color:#04B404;'>" & strAdd & "</span>"
                r(1) = xxx("StartDate")
                r(2) = "<span style='color:#04B404;'>" & xxx("IdlingTime").ToString() & " min</span>"
                r(3) = "<span style='color:#04B404;'>" & xxx("StartAndEndDate").ToString() & "</span>"
                r(4) = Math.Round(temp_lat, 6)
                r(5) = Math.Round(temp_lon, 6)

                dt_temp.Rows.Add(r)
                'End If

                'temp1 = strAdd
            Else

                strAdd = locObj.GetPrivateGeofence(temp_lat, temp_lon)

                If strAdd <> "" Then

                    'If temp2 <> strAdd Then
                    r = dt_temp.NewRow()
                    ''Blue Color
                    'Dim temp As String = "<a style='color:#8181F7;text-decoration: none;' href='GoogleEarthMaps.aspx?x=" & Math.Round(temp_lon, 6) & "&amp;y= " & Math.Round(temp_lat, 6) & "' target=""_blank"">" & strAdd & "</a>"
                    r(0) = "<span style='color:#8181F7;'>" & strAdd & "</span>"
                    r(1) = xxx("StartDate")
                    r(2) = "<span style='color:#8181F7;'>" & xxx("IdlingTime").ToString() & " min</span>"
                    r(3) = "<span style='color:#8181F7;'>" & xxx("StartAndEndDate").ToString() & "</span>"
                    r(4) = Math.Round(temp_lat, 6)
                    r(5) = Math.Round(temp_lon, 6)

                    dt_temp.Rows.Add(r)
                    'End If

                    'temp2 = strAdd

                Else

                    strAdd = locObj.GetPoi(temp_lat, temp_lon)
                    If strAdd <> "" Then

                        'If temp3 <> strAdd Then
                        'Dim temp As String = "<a style='color:#FA5858;text-decoration: none;' href='GoogleEarthMaps.aspx?x=" & Math.Round(temp_lon, 6) & "&amp;y= " & Math.Round(temp_lat, 6) & "' target=""_blank"">" & strAdd & "</a>"
                        dictionary_POI(count_POI) = "<span style='color:#FA5858;'>" & strAdd & "</span>" & "!" & "<span style='color:#FA5858;'>" & xxx("IdlingTime").ToString() & " min</span>" & "!" & "<span style='color:#FA5858;'>" & xxx("StartAndEndDate").ToString() & "</span>" & "!" & xxx("StartDate") & " ! " & Math.Round(temp_lat, 6) & " ! " & Math.Round(temp_lon, 6)

                        count_POI += 1
                        'End If

                        'temp3 = strAdd

                    Else
                        strAdd = locObj.GetRoadName(temp_lat, temp_lon)

                        If strAdd <> "" Then

                            'If temp4 <> strAdd Then
                            'Dim temp As String = "<a style='color:black;text-decoration: none;' href='GoogleEarthMaps.aspx?x=" & Math.Round(temp_lon, 6) & "&amp;y= " & Math.Round(temp_lat, 6) & "' target=""_blank"">" & strAdd & "</a>"
                            dictionary(count_temp) = "<span style='color:black;'>" & strAdd & "</span>" & "!" & xxx("IdlingTime").ToString() & " min ! " & xxx("StartAndEndDate").ToString() & "!" & xxx("StartDate") & " ! " & Math.Round(temp_lat, 6) & " ! " & Math.Round(temp_lon, 6)
                            count_temp += 1
                            'End If

                            'temp4 = strAdd

                        Else
                            If temp_lat <> 0 And temp_lon <> 0 Then
                                r = dt_temp.NewRow()
                                'Dim temp As String = "<a href='GoogleEarthMaps.aspx?x=" & Math.Round(temp_lon, 6) & "&amp;y= " & Math.Round(temp_lat, 6) & "' target=""_blank"">" & Math.Round(temp_lat, 6) & " . " & Math.Round(temp_lon, 4) & "</a>"

                                r(0) = "<span style='color:#FF8000;'>" & temp_lat & "," & temp_lon & "</span>"
                                r(1) = xxx("StartDate")
                                r(2) = "<span style='color:#FF8000;'>" & xxx("IdlingTime").ToString() & " min</span>"
                                r(3) = "<span style='color:#FF8000;'>" & xxx("StartAndEndDate").ToString() & "</span>"
                                r(4) = Math.Round(temp_lat, 6)
                                r(5) = Math.Round(temp_lon, 6)

                                dt_temp.Rows.Add(r)
                            End If
                        End If

                    End If

                End If


            End If

        Next

        If dictionary_POI.Count > 0 Then

            For i As Int32 = 0 To dictionary_POI.Count - 1
                Dim split() As String = dictionary_POI(i).ToString().Split("!")
                r = dt_temp.NewRow()
                r(0) = split(0)
                r(1) = split(3)
                r(2) = split(1)
                r(3) = split(2)
                r(4) = split(4)
                r(5) = split(5)
                dt_temp.Rows.Add(r)
                'Try
                '    Dim split() As String = dictionary_POI(i).ToString().Split("!")
                '    Dim split2() As String = dictionary_POI(i + 2).ToString().Split("!")

                '    If split(0) <> split2(0) Then
                '        ''Red Color #FA5858
                '        r = dt_temp.NewRow()
                '        r(0) = split(0)
                '        r(1) = split(3)
                '        r(2) = split(1)
                '        r(3) = split(2)
                '        dt_temp.Rows.Add(r)
                '    End If
                'Catch ex As Exception
                '    r = dt_temp.NewRow()
                '    Dim split() As String = dictionary_POI(i).ToString().Split("!")
                '    r(0) = split(0)
                '    r(1) = split(3)
                '    r(2) = split(1)
                '    r(3) = split(2)
                '    dt_temp.Rows.Add(r)
                'End Try
            Next
        End If

        If dictionary.Count > 0 Then
            For i As Int32 = 0 To dictionary.Count - 1
                Dim split() As String = dictionary(i).ToString().Split("!")
                r = dt_temp.NewRow()
                r(0) = split(0)
                r(1) = split(3)
                r(2) = split(1)
                r(3) = split(2)
                r(4) = split(4)
                r(5) = split(5)
                dt_temp.Rows.Add(r)
                'Try
                '    If dictionary(i) <> dictionary(i + 2) Then
                '        r = dt_temp.NewRow()
                '        prevgeofencename += dictionary(i).ToString()
                '        Dim split() As String = dictionary(i).ToString().Split("_")
                '        r(0) = split(0)
                '        r(1) = split(1)
                '        dt_temp.Rows.Add(r)
                '    End If
                'Catch ex As Exception
                '    r = dt_temp.NewRow()
                '    Dim split() As String = dictionary(i).ToString().Split("_")
                '    r(0) = split(0)
                '    r(1) = split(1)
                '    dt_temp.Rows.Add(r)
                'End Try

            Next
        End If

        dt_temp.DefaultView.Sort = "timestamp ASC"

        Return dt_temp
    End Function

    Protected Sub btnSendEmail_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSendEmail.Click
        If ddlUsername.SelectedValue <> "--Select User Name--" And ddlGroupName.SelectedValue <> "--Group Name--" Then

            If txtEmail.Text <> "" Then

                Dim sw As New StringWriter()
                Dim htw As New HtmlTextWriter(sw)

                htw.Write("<b style='color:#005583'>Journal Top 10 Report</b>")
                htw.Write("<br><br>")
                htw.Write("User Name : " & ddlUsername.SelectedItem.ToString())
                htw.Write("<br>")
                htw.Write("Report Date : " & DateTime.Now)
                GridView4.Font.Name = "Verdana"

                GridView4.RenderControl(htw)

                Dim dt As String = DateTime.Now.ToString("_yyyyMMddHmmss")
                Dim ReportName As String = "JournalTop10Report" & dt

                Try

                    File.WriteAllText(Server.MapPath("tempSendEmail/" & ReportName & ".xls"), sw.ToString())
                    'sendEmail.SendMailMessage(txtEmail.Text, "", txtCC.Text, " Lafarge - Journal Top 10 Report", "This email attached <i>Jounal Top 10 Report</i> with excel file.<br/><br/><b><u>Jounal Top 10 Report Summary</u.></b><br/> Username: " & ddlUsername.SelectedItem.ToString() & "<br/> Group Name: " & ddlGroupName.SelectedValue & "<br/> Date : " & txtBeginDate.Value & " - " & txtEndDate.Value, ReportName)
                    File.Delete(Server.MapPath("tempSendEmail/" & ReportName & ".xls"))

                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, GetType(Page), "Script", "Sent();", True)
                Catch ex As Exception
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, GetType(Page), "Script", "alert('Email Send Failed, Please Try Again.');", True)
                End Try
            Else
                txtEmail.Focus()
            End If
        End If
    End Sub

    Protected Sub btnSaveExcel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSaveExcel.Click
        Response.ClearContent()
        Response.Buffer = True
        Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", "JournalTop10Report.xls"))
        Response.ContentType = "application/ms-excel"
        Dim sw As New StringWriter()
        Dim htw As New HtmlTextWriter(sw)

        htw.Write("<b style='color:#005583'>Journal Top 10 Report</b>")
        htw.Write("<br><br>")
        htw.Write("User Name : " & ddlUsername.SelectedItem.ToString())
        htw.Write("<br>")
        htw.Write("Report Date : " & DateTime.Now)
        GridView4.Font.Name = "Verdana"

        GridView4.RenderControl(htw)
        sw.ToString()

        Response.Write(sw.ToString())
        Response.[End]()

    End Sub

    Protected Sub ImageButton2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
        Top10Table.Visible = True
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Top10Table.Visible = False
    End Sub

End Class
