Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI
Imports System.Math
Imports System.Globalization
Imports System.Drawing
Partial Class OhsasVehicleWeeklysummary
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public show As Boolean = False
    Public sbplate As StringBuilder
#Region "Function"

    Protected Sub GetPlateNo(ByVal userid As String)
        '(d2 - d1).TotalMinutes
        Try
            'Response.Write("insert")
            Dim lguserlist As String = Request.Cookies("userinfo")("userslist")

            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ' Dim Rconn As New Redirect(userid)
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                Dim strSql As String
                If userid = "ALL" Then 
                    strSql = "select plateno from vehicleTBL where userid in (" & lguserlist & ") order by plateno"
                Else
                    strSql = "select plateno from vehicleTBL where userid='" & userid & "' order by plateno"
                End If
                Dim sqlCmd As New SqlCommand(strSql, conn)
                Dim dr As SqlDataReader
                ddlVehicle.Items.Clear()
                ' ddlVehicle.Items.Add("--All Vehicle--")
                conn.Open()
                dr = sqlCmd.ExecuteReader
                ' sbplate = New StringBuilder()
                'sbplate.Append(" <asp:ListBox ID='lstFruits' runat='server' SelectionMode='Multiple'>")
                While dr.Read
                    ddlVehicle.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                    'sbplate.Append("<asp:ListItem Text='" & dr("plateno") & "' Value='" & dr("plateno") & "' />")
                    ' ddlVehicle.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                End While
                'sbplate.Append("</asp:ListBox>")
                conn.Close()
            End If
        Catch ex As Exception
            Response.Write("GetPlateNo : " & ex.Message)
        End Try
    End Sub
    Protected Sub FillViolations()
        Dim platenos As String = ""
        For Each item As ListItem In ddlVehicle.Items
            If item.Selected Then
                If platenos = "" Then
                    platenos = "'" & item.Value & "'"
                Else
                    platenos += ",'" & item.Value & "'"
                End If
            End If
        Next
        Dim strBeginDateTime, strEndDateTime, strSql As String
        Dim currDate, prevDate As DateTime
        Dim ds As New DataSet
        Dim adpViolatinon As SqlDataAdapter
        Dim row As DataRow
        Dim speedException, harshDec, harshAcc, UnauthStop, UnauthRoad, BanHours, Idle, contDrive, DriveHour, WorkHour, TotalVio, j, unsafeWork, unsafeDrive, totalUnsafeWork, totalUnsafeDrive, longOSpeedDuration As Integer
        Dim grandspeedException, grandharshDec, grandharshAcc, grandUnauthStop, grandUnauthRoad, grandBanHours, grandIdle, grandcontDrive, grandDriveHour, grandWorkHour, grandTotalVio, grandj, grandunsafeWork, grandunsafeDrive, grandtotalUnsafeWork, grandtotalUnsafeDrive, grandlongOSpeedDuration As Integer
        Dim distance, totalDrive, totalWork, maxspeed, midnightcount As Double
        Dim granddistance, grandtotalDrive, grandtotalWork, grandmaxspeed, grandmidnightcount As Double
        '  Dim Rredirect As New Redirect(ddlUsername.SelectedValue)
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

        strBeginDateTime = ddlweek.SelectedValue.Split("-")(0) & " 00:00:00"
        strEndDateTime = ddlweek.SelectedValue.Split("-")(1) & "  23:59:59"
        j = 0
        Try
            Dim DbTable As New DataTable
            With DbTable.Columns
                .Add(New DataColumn("No"))
                .Add(New DataColumn("Vehicle No"))
                .Add(New DataColumn("Overspeeding"))
                .Add(New DataColumn("maxSpeed"))
                .Add(New DataColumn("longOverspeedDuration"))
                .Add(New DataColumn("Harsh Brake"))
                .Add(New DataColumn("Harsh acc"))
                .Add(New DataColumn("Unauth Stop"))
                .Add(New DataColumn("Unauth Road"))
                .Add(New DataColumn("Banned Hour"))
                .Add(New DataColumn("Idling"))
                .Add(New DataColumn("Cont Drive"))
                .Add(New DataColumn("Drive Hour"))
                .Add(New DataColumn("Work Hour"))
                .Add(New DataColumn("Total Violation"))
                .Add(New DataColumn("Total Drive Hour"))
                .Add(New DataColumn("Total Work Hour"))
                .Add(New DataColumn("more14Work"))
                .Add(New DataColumn("more10Drive"))
                .Add(New DataColumn("Distance Travel"))
                .Add(New DataColumn("MidNightCount"))
            End With
            Label1.ForeColor = Color.Blue
            Label4.ForeColor = Color.Blue
            Label2.ForeColor = Color.Red
            Label2.Font.Bold = True
            Label3.ForeColor = Color.Red
            Label3.Font.Bold = True
            Label2.Text = " " & strBeginDateTime
            Label3.Text = strEndDateTime
            Label4.Text = " To "
            Label1.Text = "Vehicles Violation Summary Report From "
            Label5.Enabled = True
            Label6.Enabled = True
            Label7.Enabled = True
            Label8.Enabled = True
            Label9.Enabled = True
            Label10.Enabled = True
            Label11.Enabled = True
            Label12.Enabled = True
            If ddlUsername.SelectedValue = "ALL" Then
                strSql = "SELECT * FROM ohsas_violation Where  plateno in (" & platenos & ") and timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' order by timestamp,plateno"
            Else
                strSql = "SELECT * FROM ohsas_violation Where userid='" & ddlUsername.SelectedValue & "' and plateno in (" & platenos & ") and timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' order by timestamp,plateno"
            End If


            conn.Open()
            adpViolatinon = New SqlDataAdapter(strSql, conn)
            adpViolatinon.Fill(ds)
            If ds.Tables(0).Rows.Count <> 0 Then

                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    currDate = ds.Tables(0).Rows(i)("timestamp")

                    If i = 0 Then
                        row = DbTable.NewRow
                        row(1) = "<font size=""2px"" color=""Blue""><b>" & Convert.ToDateTime(currDate).ToString("yyyy/MM/dd") & "</b></font>"
                        DbTable.Rows.Add(row)
                    Else


                        If prevDate <> currDate Then
                            row = DbTable.NewRow
                            row(1) = "Total"
                            row(2) = "<font color=""Red""><b>" & speedException & "</b></font>"
                            row(3) = "<font color=""Red""><b>" & maxspeed & "</b></font>"
                            row(4) = "<font color=""Red""><b>" & longOSpeedDuration & "</b></font>"
                            row(5) = "<font color=""Red""><b>" & harshDec & "</b></font>"
                            row(6) = "<font color=""Red""><b>" & harshAcc & "</b></font>"
                            row(7) = "<font color=""Red""><b>" & UnauthStop & "</b></font>"
                            row(8) = "<font color=""Red""><b>" & UnauthRoad & "</b></font>"
                            row(9) = "<font color=""Red""><b>" & BanHours & "</b></font>"
                            row(10) = "<font color=""Red""><b>" & Idle & "</b></font>"
                            row(11) = "<font color=""Red""><b>" & contDrive & "</b></font>"
                            row(12) = " <font color=""Red""><b>" & DriveHour & "</b></font>"
                            Dim driverWorkHour1 As New TimeSpan(0, 0, WorkHour)
                            row(13) = "<font color=""Red""><b>" & ConvertHours(driverWorkHour1.ToString) & "</b></font>"
                            row(14) = "<font color=""Red""><b>" & TotalVio & "</b></font>"
                            Dim totalcountDrive As New TimeSpan(0, totalDrive, 0)
                            Dim totalCountWork As New TimeSpan(0, totalWork, 0)
                            'Dim k1 As String
                            'Dim h2 As String
                            'k1 = totalcountDrive.ToString("HH:mm:ss")
                            'h2 = totalCountWork.ToString("HH:mm:ss")

                            row(15) = "<font color=""Red""><b>" & ConvertHours(totalcountDrive.ToString) & "</b></font>"
                            row(16) = "<font color=""Red""><b>" & ConvertHours(totalCountWork.ToString) & "</b></font>"
                            row(17) = "<font color=""Red""><b>" & ConvertHours(totalUnsafeWork.ToString) & "</b></font>"
                            row(18) = "<font color=""Red""><b>" & ConvertHours(totalUnsafeDrive.ToString) & "</b></font>"
                            row(19) = "<font color=""Red""><b>" & distance & "</b></font>"
                            row(20) = "<font color=""Red""><b>" & midnightcount & "</b></font>"
                            DbTable.Rows.Add(row)

                            grandspeedException = grandspeedException + speedException
                            grandharshDec = grandharshDec + harshDec
                            grandharshAcc = grandharshAcc + harshAcc
                            grandUnauthStop = grandUnauthStop + UnauthStop
                            grandUnauthRoad = grandUnauthRoad + UnauthRoad
                            grandBanHours = grandBanHours + BanHours
                            grandIdle = grandIdle + Idle
                            grandcontDrive = grandcontDrive + contDrive
                            grandDriveHour = grandDriveHour + DriveHour
                            grandWorkHour = grandWorkHour + WorkHour
                            grandTotalVio = grandTotalVio + TotalVio
                            granddistance = granddistance + distance
                            grandtotalDrive = grandtotalDrive + totalDrive
                            grandtotalWork = grandtotalWork + totalWork
                            grandmidnightcount = grandmidnightcount + midnightcount
                            grandtotalUnsafeWork = grandtotalUnsafeWork + totalUnsafeWork
                            grandtotalUnsafeDrive = grandtotalUnsafeDrive + totalUnsafeDrive
                            If (grandlongOSpeedDuration < longOSpeedDuration) Then
                                grandlongOSpeedDuration = longOSpeedDuration
                            End If
                            If (grandlongOSpeedDuration < maxspeed) Then
                                grandmaxspeed = maxspeed
                            End If

                            maxspeed = 0
                            longOSpeedDuration = 0
                            speedException = 0
                            harshDec = 0
                            harshAcc = 0
                            UnauthStop = 0
                            UnauthRoad = 0
                            BanHours = 0
                            Idle = 0
                            contDrive = 0
                            DriveHour = 0
                            WorkHour = 0
                            TotalVio = 0
                            distance = 0
                            totalDrive = 0
                            midnightcount = 0
                            totalWork = 0
                        End If
                        If prevDate <> currDate Then
                            row = DbTable.NewRow
                            DbTable.Rows.Add(row)
                        End If
                        If prevDate <> currDate Then
                            totalUnsafeDrive = 0
                            totalUnsafeWork = 0
                            row = DbTable.NewRow
                            row(1) = "<font size=""2px"" color=""Blue""><b>" & Convert.ToDateTime(currDate).ToString("yyyy/MM/dd") & "</b></font>"
                            DbTable.Rows.Add(row)
                        End If
                    End If

                    row = DbTable.NewRow
                    row(0) = i + 1
                    row(1) = ds.Tables(0).Rows(i)("plateno")
                    row(2) = CheckNullData(ds.Tables(0).Rows(i)("overspeed"))
                    row(3) = CheckNullData(ds.Tables(0).Rows(i)("maxspeed"))
                    row(4) = CheckNullData(ds.Tables(0).Rows(i)("longSpeedDuration"))
                    row(5) = CheckNullData(Round((ds.Tables(0).Rows(i)("hDec") / 22)))
                    row(6) = Round((ds.Tables(0).Rows(i)("hAcc") / 22))
                    row(7) = ds.Tables(0).Rows(i)("unstop")
                    row(8) = ds.Tables(0).Rows(i)("unroad")
                    row(9) = ds.Tables(0).Rows(i)("banhour")
                    row(10) = ds.Tables(0).Rows(i)("idling")
                    Dim cont2hrsDrive As New TimeSpan(0, 0, ds.Tables(0).Rows(i)("totalCont2HourDrive"))
                    Dim cont4hrsDrive As New TimeSpan(0, 0, ds.Tables(0).Rows(i)("totalCont4HourDrive"))
                    'Dim TotalContinuesDriveHour As New TimeSpan(0, 0, ds.Tables(0).Rows(z)("totalCont4HourDrive") + ds.Tables(0).Rows(z)("totalCont2HourDrive"))
                    Dim TotalContinuesDriveHour As New TimeSpan(0, 0, ds.Tables(0).Rows(i)("totalCont4HourDrive"))
                    row(11) = CheckNullData(ds.Tables(0).Rows(i)("totalCont2HourCount"))
                    row(12) = CheckNullData(ds.Tables(0).Rows(i)("totalCont4HourCount"))

                    row(13) = TotalContinuesDriveHour
                    'row(11) = ds.Tables(0).Rows(i)("contdrive")
                    'row(12) = ds.Tables(0).Rows(i)("drivehour")
                    'Dim driverworkhourValue As New TimeSpan(0, 0, ds.Tables(0).Rows(i)("workhour"))
                    'row(13) = driverworkhourValue.ToString
                    row(14) = Round((ds.Tables(0).Rows(i)("totalvio") / 22))

                    Dim hourValue As New TimeSpan(0, ds.Tables(0).Rows(i)("totaldriverhour"), 0)
                    row(15) = hourValue.ToString
                    Dim workHourValue As New TimeSpan(0, ds.Tables(0).Rows(i)("totalworkhour"), 0)
                    row(16) = workHourValue.ToString
                    If ds.Tables(0).Rows(i)("totalworkhour") > 840 Then
                        row(17) = unsafeWork + 1
                        totalUnsafeWork = totalUnsafeWork + 1

                    End If
                    If ds.Tables(0).Rows(i)("totaldriverhour") > 600 Then
                        row(18) = unsafeDrive + 1
                        totalUnsafeDrive = totalUnsafeDrive + 1
                    End If
                    row(19) = Math.Round(ds.Tables(0).Rows(i)("distance"), 2)
                    row(20) = ds.Tables(0).Rows(i)("midnightcount")
                    speedException = speedException + ds.Tables(0).Rows(i)("overspeed")
                    harshDec = harshDec + Round((ds.Tables(0).Rows(i)("hDec") / 22))
                    harshAcc = harshAcc + Round((ds.Tables(0).Rows(i)("hAcc") / 22))
                    UnauthStop = UnauthStop + ds.Tables(0).Rows(i)("unstop")
                    UnauthRoad = UnauthRoad + ds.Tables(0).Rows(i)("unroad")
                    BanHours = BanHours + ds.Tables(0).Rows(i)("banhour")
                    Idle = Idle + ds.Tables(0).Rows(i)("idling")
                    contDrive = contDrive + ds.Tables(0).Rows(i)("contdrive")
                    DriveHour = DriveHour + ds.Tables(0).Rows(i)("totalCont4HourCount")
                    WorkHour = WorkHour + ds.Tables(0).Rows(i)("totalCont4HourDrive")
                    TotalVio = TotalVio + Round((ds.Tables(0).Rows(i)("totalvio") / 22))
                    distance = distance + Math.Round(ds.Tables(0).Rows(i)("distance"), 2)
                    midnightcount = midnightcount + Convert.ToInt32(ds.Tables(0).Rows(i)("midnightcount"))
                    totalDrive = totalDrive + ds.Tables(0).Rows(i)("totaldriverhour")
                    totalWork = totalWork + ds.Tables(0).Rows(i)("totalworkhour")
                    If ds.Tables(0).Rows(i)("longSpeedDuration") > longOSpeedDuration Then
                        longOSpeedDuration = ds.Tables(0).Rows(i)("longSpeedDuration")
                    End If
                    If ds.Tables(0).Rows(i)("maxspeed") > maxspeed Then
                        maxspeed = ds.Tables(0).Rows(i)("maxspeed")
                    End If
                    DbTable.Rows.Add(row)
                    prevDate = currDate
                Next

                row = DbTable.NewRow
                row(1) = "<font color=""Red""><b> Total </b></font>"
                row(2) = "<font color=""Red""><b>" & speedException & "</b></font>"
                row(3) = "<font color=""Red""><b>" & maxspeed & "</b></font>"
                row(4) = "<font color=""Red""><b>" & longOSpeedDuration & "</b></font>"
                row(5) = "<font color=""Red""><b>" & Round((harshDec / 22)) & "</b></font>"
                row(6) = "<font color=""Red""><b>" & Round((harshAcc / 22)) & "</b></font>"
                row(7) = "<font color=""Red""><b>" & UnauthStop & "</b></font>"
                row(8) = "<font color=""Red""><b>" & UnauthRoad & "</b></font>"
                row(9) = "<font color=""Red""><b>" & BanHours & "</b></font>"
                row(10) = "<font color=""Red""><b>" & Idle & "</b></font>"
                row(11) = "<font color=""Red""><b>" & contDrive & "</b></font>"
                row(12) = "<font color=""Red""><b>" & DriveHour & "</b></font>"
                Dim driverWorkHour As New TimeSpan(0, 0, WorkHour)
                row(13) = "<font color=""Red""><b>" & ConvertHours(driverWorkHour.ToString) & "</b></font>"
                row(14) = "<font color=""Red""><b>" & TotalVio & "</b></font>"
                Dim totalcountDrive1 As New TimeSpan(0, totalDrive, 0)
                Dim totalCountWork1 As New TimeSpan(0, totalWork, 0)
                'Dim k As String
                'Dim h As String
                'k = totalcountDrive1.ToString("HH:mm:ss")
                'h = totalCountWork1.ToString("HH:mm:ss")

                row(15) = "<font color=""Red""><b>" & ConvertHours(totalcountDrive1.ToString) & "</b></font>"
                row(16) = "<font color=""Red""><b>" & ConvertHours(totalCountWork1.ToString) & "</b></font>"
                row(17) = "<font color=""Red""><b>" & ConvertHours(totalUnsafeWork.ToString) & "</b></font>"
                row(18) = "<font color=""Red""><b>" & ConvertHours(totalUnsafeDrive.ToString) & "</b></font>"
                row(19) = "<font color=""Red""><b>" & distance & "</b></font>"
                row(20) = "<font color=""Red""><b>" & midnightcount & "</b></font>"
                DbTable.Rows.Add(row)
                grandspeedException = grandspeedException + speedException
                grandharshDec = grandharshDec + harshDec
                grandharshAcc = grandharshAcc + harshAcc
                grandUnauthStop = grandUnauthStop + UnauthStop
                grandUnauthRoad = grandUnauthRoad + UnauthRoad
                grandBanHours = grandBanHours + BanHours
                grandIdle = grandIdle + Idle
                grandcontDrive = grandcontDrive + contDrive
                grandDriveHour = grandDriveHour + DriveHour
                grandWorkHour = grandWorkHour + WorkHour
                grandTotalVio = grandTotalVio + TotalVio
                granddistance = granddistance + distance
                grandtotalDrive = grandtotalDrive + totalDrive
                grandtotalWork = grandtotalWork + totalWork
                grandmidnightcount = grandmidnightcount + midnightcount
                grandtotalUnsafeWork = grandtotalUnsafeWork + totalUnsafeWork
                grandtotalUnsafeDrive = grandtotalUnsafeDrive + totalUnsafeDrive
                If (grandlongOSpeedDuration < longOSpeedDuration) Then
                    grandlongOSpeedDuration = longOSpeedDuration
                End If
                If (grandmaxspeed < maxspeed) Then
                    grandmaxspeed = maxspeed
                End If
                row = DbTable.NewRow
                DbTable.Rows.Add(row)
                row = DbTable.NewRow
                DbTable.Rows.Add(row)
                row = DbTable.NewRow
                row(1) = "<font color=""Red""><b> Grand Total </b></font>"
                row(2) = "<font color=""Red""><b>" & grandspeedException & "</b></font>"
                row(3) = "<font color=""Red""><b>" & grandmaxspeed & "</b></font>"
                row(4) = "<font color=""Red""><b>" & grandlongOSpeedDuration & "</b></font>"
                row(5) = "<font color=""Red""><b>" & grandharshDec & "</b></font>"
                row(6) = "<font color=""Red""><b>" & grandharshAcc & "</b></font>"
                row(7) = "<font color=""Red""><b>" & grandUnauthStop & "</b></font>"
                row(8) = "<font color=""Red""><b>" & grandUnauthRoad & "</b></font>"
                row(9) = "<font color=""Red""><b>" & grandBanHours & "</b></font>"
                row(10) = "<font color=""Red""><b>" & grandIdle & "</b></font>"
                row(11) = "<font color=""Red""><b>" & grandcontDrive & "</b></font>"
                row(12) = "<font color=""Red""><b>" & grandDriveHour & "</b></font>"
                Dim gdriverWorkHour As New TimeSpan(0, 0, grandWorkHour)
                row(13) = "<font color=""Red""><b>" & ConvertHours(gdriverWorkHour.ToString) & "</b></font>"
                row(14) = "<font color=""Red""><b>" & grandTotalVio & "</b></font>"
                Dim gtotalcountDrive1 As New TimeSpan(0, grandtotalDrive, 0)
                Dim gtotalCountWork1 As New TimeSpan(0, grandtotalWork, 0)
                'Dim k As String
                'Dim h As String
                'k = totalcountDrive1.ToString("HH:mm:ss")
                'h = totalCountWork1.ToString("HH:mm:ss")

                row(15) = "<font color=""Red""><b>" & ConvertHours(gtotalcountDrive1.ToString) & "</b></font>"
                row(16) = "<font color=""Red""><b>" & ConvertHours(gtotalCountWork1.ToString) & "</b></font>"
                row(17) = "<font color=""Red""><b>" & ConvertHours(grandtotalUnsafeWork.ToString) & "</b></font>"
                row(18) = "<font color=""Red""><b>" & ConvertHours(grandtotalUnsafeDrive.ToString) & "</b></font>"
                row(19) = "<font color=""Red""><b>" & granddistance & "</b></font>"
                row(20) = "<font color=""Red""><b>" & grandmidnightcount & "</b></font>"
                DbTable.Rows.Add(row)
                conn.Close()
            Else
                row = DbTable.NewRow
                row(0) = "-"
                row(1) = "-"
                row(2) = "-"
                row(3) = "-"
                row(4) = "-"
                row(5) = "-"
                row(6) = "-"
                row(7) = "-"
                row(8) = "-"
                row(9) = "-"
                row(10) = "-"
                row(11) = "-"
                row(12) = "-"
                row(13) = "-"
                row(14) = "-"
                row(15) = "-"
                row(16) = "-"
                row(17) = "-"
                row(18) = "-"
                row(19) = "-"
                row(20) = "-"
                DbTable.Rows.Add(row)
            End If

            With GridView1
                .DataSource = DbTable
                .DataBind()
                If .PageCount > 1 Then
                    show = True
                End If
            End With

            For i As Integer = 0 To GridView1.Rows.Count - 1
                If GridView1.Rows.Item(i).Cells(1).Text = "Total" Or GridView1.Rows.Item(i).Cells(1).Text = "Grand Total" Or Left(GridView1.Rows.Item(i).Cells(1).Text, 4) = "Date" Then
                    With GridView1.Rows.Item(i)
                        .Cells(1).BackColor = System.Drawing.Color.Yellow
                        .Cells(1).ForeColor = System.Drawing.Color.Red
                        .Cells(1).Font.Bold = True
                        .Cells(2).BackColor = System.Drawing.Color.Yellow
                        .Cells(3).BackColor = System.Drawing.Color.Yellow
                        .Cells(4).BackColor = System.Drawing.Color.Yellow
                        .Cells(5).BackColor = System.Drawing.Color.Yellow
                        .Cells(6).BackColor = System.Drawing.Color.Yellow
                        .Cells(7).BackColor = System.Drawing.Color.Yellow
                        .Cells(8).BackColor = System.Drawing.Color.Yellow
                        .Cells(9).BackColor = System.Drawing.Color.Yellow
                        .Cells(10).BackColor = System.Drawing.Color.Yellow
                        .Cells(11).BackColor = System.Drawing.Color.Yellow
                        .Cells(12).BackColor = System.Drawing.Color.Yellow
                        .Cells(13).BackColor = System.Drawing.Color.Yellow
                        .Cells(14).BackColor = System.Drawing.Color.Yellow
                        .Cells(15).BackColor = System.Drawing.Color.Yellow
                        .Cells(16).BackColor = System.Drawing.Color.Yellow
                        .Cells(17).BackColor = System.Drawing.Color.Yellow
                        .Cells(18).BackColor = System.Drawing.Color.Yellow
                        .Cells(19).BackColor = System.Drawing.Color.Yellow
                        .Cells(20).BackColor = System.Drawing.Color.Yellow
                    End With

                End If

                If GridView1.Rows.Item(i).Cells(1).Text <> "-" Then
                    With GridView1.Rows(DbTable.Rows.Count - 1)
                        .Cells(1).BackColor = System.Drawing.Color.Yellow
                        .Cells(1).ForeColor = System.Drawing.Color.Red
                        .Cells(1).Font.Bold = True
                        .Cells(2).BackColor = System.Drawing.Color.Yellow
                        .Cells(3).BackColor = System.Drawing.Color.Yellow
                        .Cells(4).BackColor = System.Drawing.Color.Yellow
                        .Cells(5).BackColor = System.Drawing.Color.Yellow
                        .Cells(6).BackColor = System.Drawing.Color.Yellow
                        .Cells(7).BackColor = System.Drawing.Color.Yellow
                        .Cells(8).BackColor = System.Drawing.Color.Yellow
                        .Cells(9).BackColor = System.Drawing.Color.Yellow
                        .Cells(10).BackColor = System.Drawing.Color.Yellow
                        .Cells(11).BackColor = System.Drawing.Color.Yellow
                        .Cells(12).BackColor = System.Drawing.Color.Yellow
                        .Cells(13).BackColor = System.Drawing.Color.Yellow
                        .Cells(14).BackColor = System.Drawing.Color.Yellow
                        .Cells(15).BackColor = System.Drawing.Color.Yellow
                        .Cells(16).BackColor = System.Drawing.Color.Yellow
                        .Cells(17).BackColor = System.Drawing.Color.Yellow
                        .Cells(18).BackColor = System.Drawing.Color.Yellow
                        .Cells(19).BackColor = System.Drawing.Color.Yellow
                        .Cells(20).BackColor = System.Drawing.Color.Yellow

                    End With
                Else

                End If

            Next



            Session("GridView1") = DbTable
            ec = "true"
            Session("exceltable") = DbTable
        Catch ex As Exception
            Response.Write("AA" & ex.Message)
        End Try
    End Sub
    Public Function CheckNullData(ByVal value As Double)
        If value = 0 Then
            Return ""
        Else
            Return value
        End If
    End Function
    Protected Sub FillViolationsGroupByPlateNo()
        Dim platenos As String = ""
        For Each item As ListItem In ddlVehicle.Items
            If item.Selected Then
                If platenos = "" Then
                    platenos = "'" & item.Value & "'"
                Else
                    platenos += ",'" & item.Value & "'"
                End If
            End If
        Next
        Dim strBeginDateTime, strEndDateTime, strSql, currPlateno, PrevPlateNo As String
        Dim ds As New DataSet
        Dim adpViolatinon As SqlDataAdapter
        Dim row As DataRow
        Dim midnightcount As Integer = 0
        Dim grandmidnightcount As Integer = 0
        Dim speedException, harshDec, harshAcc, UnauthStop, UnauthRoad, BanHours, Idle, contDrive, DriveHour, WorkHour, TotalVio, j, unsafeWork, unsafeDrive, totalUnsafeWork, totalUnsafeDrive, longOSpeedDuration As Integer
        Dim grandspeedException, grandharshDec, grandharshAcc, grandUnauthStop, grandUnauthRoad, grandBanHours, grandIdle, grandcontDrive, grandDriveHour, grandWorkHour, grandTotalVio, grandj, grandunsafeWork, grandunsafeDrive, grandtotalUnsafeWork, grandtotalUnsafeDrive, grandlongOSpeedDuration As Integer
        Dim distance, totalDrive, totalWork, maxspeed As Double
        Dim granddistance, grandtotalDrive, grandtotalWork, grandmaxspeed As Double
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        strBeginDateTime = ddlweek.SelectedValue.Split("-")(0) & " 00:00:00"
        strEndDateTime = ddlweek.SelectedValue.Split("-")(1) & "  23:59:59"
        j = 0
        Try
            Dim z, currentCount As Integer
            Dim DbTable As New DataTable
            With DbTable.Columns
                .Add(New DataColumn("No"))
                .Add(New DataColumn("Vehicle No"))
                .Add(New DataColumn("Overspeeding"))
                .Add(New DataColumn("maxSpeed"))
                .Add(New DataColumn("longOverspeedDuration"))

                .Add(New DataColumn("Harsh Brake"))
                .Add(New DataColumn("Harsh acc"))
                .Add(New DataColumn("Unauth Stop"))
                .Add(New DataColumn("Unauth Road"))
                .Add(New DataColumn("Banned Hour"))
                .Add(New DataColumn("Idling"))
                .Add(New DataColumn("Cont Drive"))
                .Add(New DataColumn("Drive Hour"))
                .Add(New DataColumn("Work Hour"))
                .Add(New DataColumn("Total Violation"))
                .Add(New DataColumn("Total Drive Hour"))
                .Add(New DataColumn("Total Work Hour"))
                .Add(New DataColumn("more14Work"))
                .Add(New DataColumn("more10Drive"))
                .Add(New DataColumn("Distance Travel"))
                .Add(New DataColumn("MidNightCount"))
            End With
            Label1.ForeColor = Color.Blue
            Label4.ForeColor = Color.Blue
            Label2.ForeColor = Color.Red
            Label2.Font.Bold = True
            Label3.ForeColor = Color.Red
            Label3.Font.Bold = True
            Label2.Text = " " & strBeginDateTime
            Label3.Text = strEndDateTime
            Label4.Text = " To "
            Label1.Text = "Vehicles Violation Summary Report From "
            Label5.Enabled = True
            Label6.Enabled = True
            Label7.Enabled = True
            Label8.Enabled = True
            Label9.Enabled = True
            Label10.Enabled = True
            Label11.Enabled = True
            Label12.Enabled = True
            If ddlUsername.SelectedValue = "ALL" Then
                strSql = "SELECT * FROM ohsas_violation Where  plateno in (" & platenos & ") and timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' order by plateno,timestamp"
            Else
                strSql = "SELECT * FROM ohsas_violation Where userid='" & ddlUsername.SelectedValue & "' and  plateno in (" & platenos & ") and timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' order by plateno,timestamp"
            End If


            conn.Open()
            adpViolatinon = New SqlDataAdapter(strSql, conn)
            adpViolatinon.Fill(ds)
            If ds.Tables(0).Rows.Count <> 0 Then
                PrevPlateNo = ""
                currPlateno = ""
                Dim sno As Integer = 1
                For z = 0 To ds.Tables(0).Rows.Count - 1
                    currPlateno = ds.Tables(0).Rows(z)("plateno")
                    If PrevPlateNo <> currPlateno And z > 1 Then
                        row = DbTable.NewRow
                        row(0) = sno
                        row(1) = "<font size=""2px"" color=""Blue""><b>" & PrevPlateNo & "</b></font>"
                        row(2) = speedException
                        row(3) = maxspeed
                        row(4) = longOSpeedDuration
                        row(5) = harshDec
                        row(6) = harshAcc
                        row(7) = UnauthStop
                        row(8) = UnauthRoad
                        row(9) = BanHours
                        row(10) = Idle
                        row(11) = contDrive
                        row(12) = DriveHour
                        Dim driverWorkHour1 As New TimeSpan(0, 0, WorkHour)
                        row(13) = ConvertHours(driverWorkHour1.ToString)
                        row(14) = TotalVio
                        Dim totalcountDrive As New TimeSpan(0, totalDrive, 0)
                        Dim totalCountWork As New TimeSpan(0, totalWork, 0)
                        'Dim k1 As String
                        'Dim h2 As String
                        'k1 = totalcountDrive.ToString("HH:mm:ss")
                        'h2 = totalCountWork.ToString("HH:mm:ss")

                        row(15) = ConvertHours(totalcountDrive.ToString)
                        row(16) = ConvertHours(totalCountWork.ToString)
                        row(17) = ConvertHours(totalUnsafeWork.ToString)
                        row(18) = ConvertHours(totalUnsafeDrive.ToString)
                        row(19) = distance
                        row(20) = midnightcount
                        DbTable.Rows.Add(row)
                        sno = sno + 1
                        grandspeedException = grandspeedException + speedException
                        grandharshDec = grandharshDec + harshDec
                        grandharshAcc = grandharshAcc + harshAcc
                        grandUnauthStop = grandUnauthStop + UnauthStop
                        grandUnauthRoad = grandUnauthRoad + UnauthRoad
                        grandBanHours = grandBanHours + BanHours
                        grandIdle = grandIdle + Idle
                        grandcontDrive = grandcontDrive + contDrive
                        grandDriveHour = grandDriveHour + DriveHour
                        grandWorkHour = grandWorkHour + WorkHour
                        grandTotalVio = grandTotalVio + TotalVio
                        granddistance = granddistance + distance
                        grandtotalDrive = grandtotalDrive + totalDrive
                        grandtotalWork = grandtotalWork + totalWork
                        grandmidnightcount = grandmidnightcount + midnightcount
                        grandtotalUnsafeWork = grandtotalUnsafeWork + totalUnsafeWork
                        grandtotalUnsafeDrive = grandtotalUnsafeDrive + totalUnsafeDrive
                        If (grandmaxspeed < maxspeed) Then
                            grandmaxspeed = maxspeed
                        End If
                        longOSpeedDuration = 0
                        maxspeed = 0
                        speedException = 0
                        harshDec = 0
                        harshAcc = 0
                        UnauthStop = 0
                        UnauthRoad = 0
                        BanHours = 0
                        Idle = 0
                        contDrive = 0
                        DriveHour = 0
                        WorkHour = 0
                        TotalVio = 0
                        distance = 0
                        totalDrive = 0
                        totalWork = 0
                        midnightcount = 0
                    End If
                    'If PrevPlateNo <> currPlateno Then
                    '    row = DbTable.NewRow
                    '    DbTable.Rows.Add(row)
                    'End If
                    'If PrevPlateNo <> currPlateno Then
                    '    row = DbTable.NewRow
                    '    row(1) = "<font size=""2px"" color=""Blue""><b>" & currPlateno & "</b></font>"
                    '    DbTable.Rows.Add(row)
                    'End If

                    If PrevPlateNo <> currPlateno Then
                        currentCount = 1
                        totalUnsafeDrive = 0
                        totalUnsafeWork = 0
                    Else
                        currentCount = currentCount + 1
                    End If
                    ' Response.Write(ds.Tables(0).Rows(z)("overspeed"))
                    row = DbTable.NewRow
                    row(0) = currentCount
                    row(1) = Convert.ToDateTime(ds.Tables(0).Rows(z)("timestamp")).ToString("yyyy/MM/dd")
                    row(2) = CheckNullData(ds.Tables(0).Rows(z)("overspeed"))
                    row(3) = CheckNullData(ds.Tables(0).Rows(z)("maxspeed"))
                    row(4) = CheckNullData(ds.Tables(0).Rows(z)("longSpeedDuration"))
                    row(5) = CheckNullData(Round((ds.Tables(0).Rows(z)("hDec") / 22)))
                    row(6) = Round((ds.Tables(0).Rows(z)("hAcc") / 22))
                    row(7) = ds.Tables(0).Rows(z)("unstop")
                    row(8) = ds.Tables(0).Rows(z)("unroad")
                    row(9) = ds.Tables(0).Rows(z)("banhour")
                    row(10) = ds.Tables(0).Rows(z)("idling")
                    'row(11) = ds.Tables(0).Rows(z)("contdrive")
                    'row(12) = ds.Tables(0).Rows(z)("drivehour")

                    Dim cont2hrsDrive As New TimeSpan(0, 0, ds.Tables(0).Rows(z)("totalCont2HourDrive"))
                    Dim cont4hrsDrive As New TimeSpan(0, 0, ds.Tables(0).Rows(z)("totalCont4HourDrive"))
                    'Dim TotalContinuesDriveHour As New TimeSpan(0, 0, ds.Tables(0).Rows(z)("totalCont4HourDrive") + ds.Tables(0).Rows(z)("totalCont2HourDrive"))
                    Dim TotalContinuesDriveHour As New TimeSpan(0, 0, ds.Tables(0).Rows(z)("totalCont4HourDrive"))
                    row(11) = CheckNullData(ds.Tables(0).Rows(z)("totalCont2HourCount"))
                    row(12) = CheckNullData(ds.Tables(0).Rows(z)("totalCont4HourCount"))

                    row(13) = TotalContinuesDriveHour
                    row(14) = CheckNullData(Round((ds.Tables(0).Rows(z)("totalvio") / 22)))

                    Dim hourValue As New TimeSpan(0, ds.Tables(0).Rows(z)("totaldriverhour"), 0)
                    row(15) = hourValue.ToString
                    Dim workHourValue As New TimeSpan(0, ds.Tables(0).Rows(z)("totalworkhour"), 0)
                    row(16) = workHourValue.ToString
                    'Response.Write(ds.Tables(0).Rows(z)("totalworkhour") & "<br/>")
                    If ds.Tables(0).Rows(z)("totalworkhour") > 840 Then
                        row(17) = unsafeWork + 1
                        totalUnsafeWork = totalUnsafeWork + 1
                    End If
                    If ds.Tables(0).Rows(z)("totaldriverhour") > 600 Then
                        row(18) = unsafeDrive + 1
                        totalUnsafeDrive = totalUnsafeDrive + 1
                    End If
                    row(19) = Math.Round(ds.Tables(0).Rows(z)("distance"), 2)
                    row(20) = ds.Tables(0).Rows(z)("midnightcount")

                    speedException = speedException + ds.Tables(0).Rows(z)("overspeed")
                    harshDec = harshDec + Round((ds.Tables(0).Rows(z)("hDec") / 22))
                    harshAcc = harshAcc + Round((ds.Tables(0).Rows(z)("hAcc") / 22))
                    UnauthStop = UnauthStop + ds.Tables(0).Rows(z)("unstop")
                    UnauthRoad = UnauthRoad + ds.Tables(0).Rows(z)("unroad")
                    BanHours = BanHours + ds.Tables(0).Rows(z)("banhour")
                    Idle = Idle + ds.Tables(0).Rows(z)("idling")
                    contDrive = contDrive + ds.Tables(0).Rows(z)("contdrive")
                    DriveHour = DriveHour + ds.Tables(0).Rows(z)("totalCont4HourCount")
                    WorkHour = WorkHour + ds.Tables(0).Rows(z)("totalCont4HourDrive")
                    TotalVio = TotalVio + Round((ds.Tables(0).Rows(z)("totalvio") / 22))
                    distance = distance + Math.Round(ds.Tables(0).Rows(z)("distance"), 2)
                    totalDrive = totalDrive + ds.Tables(0).Rows(z)("totaldriverhour")
                    totalWork = totalWork + ds.Tables(0).Rows(z)("totalworkhour")
                    midnightcount = midnightcount + Convert.ToInt32(ds.Tables(0).Rows(z)("midnightcount"))
                    If ds.Tables(0).Rows(z)("longSpeedDuration") > longOSpeedDuration Then
                        longOSpeedDuration = ds.Tables(0).Rows(z)("longSpeedDuration")
                    End If
                    If ds.Tables(0).Rows(z)("maxspeed") > maxspeed Then
                        maxspeed = ds.Tables(0).Rows(z)("maxspeed")
                    End If


                    ' DbTable.Rows.Add(row)
                    ' prevDate = currDate
                    PrevPlateNo = currPlateno
                Next

                row = DbTable.NewRow
                row(0) = sno
                row(1) = "<font size=""2px"" color=""Blue""><b>" & currPlateno & "</b></font>"
                row(2) = speedException
                row(3) = maxspeed
                row(4) = longOSpeedDuration
                row(5) = Round((harshDec))
                row(6) = Round((harshAcc))
                row(7) = UnauthStop
                row(8) = UnauthRoad
                row(9) = BanHours
                row(10) = Idle
                row(11) = contDrive
                row(12) = DriveHour
                Dim driverWorkHour As New TimeSpan(0, 0, WorkHour)
                row(13) = ConvertHours(driverWorkHour.ToString)
                row(14) = TotalVio
                Dim totalcountDrive1 As New TimeSpan(0, totalDrive, 0)
                Dim totalCountWork1 As New TimeSpan(0, totalWork, 0)
                'Dim k As String
                'Dim h As String
                'k = totalcountDrive1.ToString("HH:mm:ss")
                'h = totalCountWork1.ToString("HH:mm:ss")
                row(15) = ConvertHours(totalcountDrive1.ToString)
                row(16) = ConvertHours(totalCountWork1.ToString)
                row(17) = ConvertHours(totalUnsafeWork.ToString)
                row(18) = ConvertHours(totalUnsafeDrive.ToString)
                row(19) = distance
                row(20) = midnightcount
                grandspeedException = grandspeedException + speedException
                grandharshDec = grandharshDec + harshDec
                grandharshAcc = grandharshAcc + harshAcc
                grandUnauthStop = grandUnauthStop + UnauthStop
                grandUnauthRoad = grandUnauthRoad + UnauthRoad
                grandBanHours = grandBanHours + BanHours
                grandIdle = grandIdle + Idle
                grandcontDrive = grandcontDrive + contDrive
                grandDriveHour = grandDriveHour + DriveHour
                grandWorkHour = grandWorkHour + WorkHour
                grandTotalVio = grandTotalVio + TotalVio
                granddistance = granddistance + distance
                grandtotalDrive = grandtotalDrive + totalDrive
                grandtotalWork = grandtotalWork + totalWork
                grandmidnightcount = grandmidnightcount + midnightcount
                grandtotalUnsafeWork = grandtotalUnsafeWork + totalUnsafeWork
                grandtotalUnsafeDrive = grandtotalUnsafeDrive + totalUnsafeDrive
                If (grandmaxspeed < maxspeed) Then
                    grandmaxspeed = maxspeed
                End If
                DbTable.Rows.Add(row)
                conn.Close()
                row = DbTable.NewRow
                row(1) = "Total"
                row(2) = "<font color=""Red""><b>" & grandspeedException & "</b></font>"
                row(3) = "<font color=""Red""><b>" & grandmaxspeed & "</b></font>"
                row(4) = "<font color=""Red""><b>" & grandlongOSpeedDuration & "</b></font>"
                row(5) = "<font color=""Red""><b>" & grandharshDec & "</b></font>"
                row(6) = "<font color=""Red""><b>" & grandharshAcc & "</b></font>"
                row(7) = "<font color=""Red""><b>" & grandUnauthStop & "</b></font>"
                row(8) = "<font color=""Red""><b>" & grandUnauthRoad & "</b></font>"
                row(9) = "<font color=""Red""><b>" & grandBanHours & "</b></font>"
                row(10) = "<font color=""Red""><b>" & grandIdle & "</b></font>"
                row(11) = "<font color=""Red""><b>" & grandcontDrive & "</b></font>"
                row(12) = " <font color=""Red""><b>" & grandDriveHour & "</b></font>"
                Dim gdriverWorkHour1 As New TimeSpan(0, 0, grandWorkHour)
                row(13) = "<font color=""Red""><b>" & ConvertHours(gdriverWorkHour1.ToString) & "</b></font>"
                row(14) = "<font color=""Red""><b>" & grandTotalVio & "</b></font>"
                Dim gtotalcountDrive As New TimeSpan(0, grandtotalDrive, 0)
                Dim gtotalCountWork As New TimeSpan(0, grandtotalWork, 0)
                'Dim k1 As String
                'Dim h2 As String
                'k1 = totalcountDrive.ToString("HH:mm:ss")
                'h2 = totalCountWork.ToString("HH:mm:ss")

                row(15) = "<font color=""Red""><b>" & ConvertHours(gtotalcountDrive.ToString) & "</b></font>"
                row(16) = "<font color=""Red""><b>" & ConvertHours(gtotalCountWork.ToString) & "</b></font>"
                row(17) = "<font color=""Red""><b>" & ConvertHours(grandtotalUnsafeWork.ToString) & "</b></font>"
                row(18) = "<font color=""Red""><b>" & ConvertHours(grandtotalUnsafeDrive.ToString) & "</b></font>"
                row(19) = "<font color=""Red""><b>" & granddistance & "</b></font>"
                row(20) = "<font color=""Red""><b>" & grandmidnightcount & "</b></font>"
                DbTable.Rows.Add(row)
            Else
                row = DbTable.NewRow
                row(0) = "-"
                row(1) = "-"
                row(2) = "-"
                row(3) = "-"
                row(4) = "-"
                row(5) = "-"
                row(6) = "-"
                row(7) = "-"
                row(8) = "-"
                row(9) = "-"
                row(10) = "-"
                row(11) = "-"
                row(12) = "-"
                row(13) = "-"
                row(14) = "-"
                row(15) = "-"
                row(16) = "-"
                row(17) = "-"
                row(18) = "-"
                row(19) = "-"
                row(20) = "-"
                DbTable.Rows.Add(row)
            End If

            With GridView1
                .DataSource = DbTable
                .DataBind()
                If .PageCount > 1 Then
                    show = True
                End If
            End With

            For i As Integer = 0 To GridView1.Rows.Count - 1
                If GridView1.Rows.Item(i).Cells(1).Text = "Total" Or Left(GridView1.Rows.Item(i).Cells(1).Text, 8) = "Plate No" Then
                    With GridView1.Rows.Item(i)
                        .Cells(1).BackColor = System.Drawing.Color.Yellow
                        .Cells(1).ForeColor = System.Drawing.Color.Red
                        .Cells(1).Font.Bold = True
                        .Cells(2).BackColor = System.Drawing.Color.Yellow
                        .Cells(3).BackColor = System.Drawing.Color.Yellow
                        .Cells(4).BackColor = System.Drawing.Color.Yellow
                        .Cells(5).BackColor = System.Drawing.Color.Yellow
                        .Cells(6).BackColor = System.Drawing.Color.Yellow
                        .Cells(7).BackColor = System.Drawing.Color.Yellow
                        .Cells(8).BackColor = System.Drawing.Color.Yellow
                        .Cells(9).BackColor = System.Drawing.Color.Yellow
                        .Cells(10).BackColor = System.Drawing.Color.Yellow
                        .Cells(11).BackColor = System.Drawing.Color.Yellow
                        .Cells(12).BackColor = System.Drawing.Color.Yellow
                        .Cells(13).BackColor = System.Drawing.Color.Yellow
                        .Cells(14).BackColor = System.Drawing.Color.Yellow
                        .Cells(15).BackColor = System.Drawing.Color.Yellow
                        .Cells(16).BackColor = System.Drawing.Color.Yellow
                        .Cells(17).BackColor = System.Drawing.Color.Yellow
                        .Cells(18).BackColor = System.Drawing.Color.Yellow
                        .Cells(19).BackColor = System.Drawing.Color.Yellow
                        .Cells(20).BackColor = System.Drawing.Color.Yellow
                    End With

                End If

                'If GridView1.Rows.Item(i).Cells(1).Text <> "-" Then
                '    With GridView1.Rows(DbTable.Rows.Count - 1)
                '        .Cells(1).BackColor = System.Drawing.Color.Yellow
                '        .Cells(1).ForeColor = System.Drawing.Color.Red
                '        .Cells(1).Font.Bold = True
                '        .Cells(2).BackColor = System.Drawing.Color.Yellow
                '        .Cells(3).BackColor = System.Drawing.Color.Yellow
                '        .Cells(4).BackColor = System.Drawing.Color.Yellow
                '        .Cells(5).BackColor = System.Drawing.Color.Yellow
                '        .Cells(6).BackColor = System.Drawing.Color.Yellow
                '        .Cells(7).BackColor = System.Drawing.Color.Yellow
                '        .Cells(8).BackColor = System.Drawing.Color.Yellow
                '        .Cells(9).BackColor = System.Drawing.Color.Yellow
                '        .Cells(10).BackColor = System.Drawing.Color.Yellow
                '        .Cells(11).BackColor = System.Drawing.Color.Yellow
                '        .Cells(12).BackColor = System.Drawing.Color.Yellow
                '        .Cells(13).BackColor = System.Drawing.Color.Yellow
                '        .Cells(14).BackColor = System.Drawing.Color.Yellow
                '        .Cells(15).BackColor = System.Drawing.Color.Yellow
                '        .Cells(16).BackColor = System.Drawing.Color.Yellow
                '        .Cells(17).BackColor = System.Drawing.Color.Yellow
                '        .Cells(18).BackColor = System.Drawing.Color.Yellow
                '        .Cells(19).BackColor = System.Drawing.Color.Yellow
                '        .Cells(20).BackColor = System.Drawing.Color.Yellow
                '    End With
                'Else

                'End If

            Next



            Session("GridView1") = DbTable
            ec = "true"
            Session("exceltable") = DbTable
        Catch ex As Exception
            Response.Write("AA" & ex.Message)
        End Try
    End Sub
    Protected Function ConvertHours(ByVal p_hour As String) As String
        Dim sFLD() As String
        Dim sfld2() As String
        Dim hours As String
        Dim iPos As Integer
        hours = p_hour
        iPos = p_hour.IndexOf(".")
        If iPos > 0 Then
            sFLD = p_hour.Split(".")
            sfld2 = sFLD(1).Split(":")
            hours = CStr(CInt(sFLD(0)) * 24 + CInt(sfld2(0))) & ":" & sfld2(1) & ":" & sfld2(2)
        End If

        Return hours

    End Function
    'Public Function Getweeks(ByVal Monthyear As String) As ArrayList
    '    Dim a As ArrayList = New ArrayList()
    '    Try
    '        Dim dt As DateTime = Convert.ToDateTime(Monthyear & "/01")

    '        Dim weekOfMonth As Int16 = 1

    '        For k As Integer = dt.Day To k < 31
    '            Response.Write("in for")
    '            weekOfMonth = Convert.ToInt16((dt.Day + Convert.ToInt16(dt.DayOfWeek)) / 7 + 1)

    '            Response.Write("Week" & weekOfMonth & "(" & dt.Day & "-" & dt.Day + (7 - Convert.ToInt16(dt.DayOfWeek) + 1) & ")")
    '            a.Add("Week" & weekOfMonth & "(" & dt.Day & "-" & dt.Day + (7 - Convert.ToInt16(dt.DayOfWeek) + 1) & ")")
    '            dt = dt.AddDays((7 - Convert.ToInt16(dt.DayOfWeek) + 1) + 1)
    '            k = dt.Day
    '        Next

    '    Catch ex As Exception
    '        Response.Write("fffffff" & ex.Message)
    '    End Try
    '    Return a
    'End Function
    Public Function Getweeks(Monthyear As String) As ArrayList
        Dim a As New ArrayList()
        Dim a1 As New ArrayList()
        Dim aa As New ArrayList()
        Try
            Dim dt As DateTime = Convert.ToDateTime(Monthyear)

            Dim weekOfMonth As Int16 = 1
            Dim k As Integer = 1
            Dim ydays As Integer
            If DateTime.IsLeapYear(dt.Year) Then
                ydays = 366
            Else
                ydays = 365
            End If

            weekOfMonth = Math.Ceiling((Convert.ToInt16(dt.Day) + Convert.ToInt16(dt.DayOfWeek)) / 7)
            While dt <= Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd"))
                a.Add("Week" & weekOfMonth & "(" & dt.ToString("yyyy/MM/dd") & "-" & dt.AddDays(6).ToString("yyyy/MM/dd") & ")")
                a1.Add(dt.ToString("yyyy/MM/dd") & "-" & dt.AddDays(6).ToString("yyyy/MM/dd"))
                dt = dt.AddDays(7)
                weekOfMonth = weekOfMonth + 1
            End While
            aa.Add(a)
            aa.Add(a1)
        Catch ex As Exception
            Response.Write("fffffff" + ex.Message)
        End Try
        Return aa
    End Function
    Protected Sub Testing()
        Dim DbTable As New DataTable
        With DbTable.Columns
            .Add(New DataColumn("No"))
            .Add(New DataColumn("Vehicle No"))
            .Add(New DataColumn("Overspeeding"))
            .Add(New DataColumn("Harsh Brake"))
            .Add(New DataColumn("Harsh acc"))
            .Add(New DataColumn("Unauth Stop"))
            .Add(New DataColumn("Unauth Road"))
            .Add(New DataColumn("Banned Hour"))
            .Add(New DataColumn("Idling"))
            .Add(New DataColumn("Cont Drive"))
            .Add(New DataColumn("Drive Hour"))
            .Add(New DataColumn("Work Hour"))
            .Add(New DataColumn("Total Violation"))
            .Add(New DataColumn("Distance Travel"))
            .Add(New DataColumn("Total Drive Hour"))
            .Add(New DataColumn("Total Work Hour"))
        End With
        Try
            Dim row As DataRow
            row = DbTable.NewRow
            row(0) = "--"
            row(1) = "--"
            row(2) = "--"
            row(3) = "--"
            row(4) = "--"
            row(5) = "--"
            row(6) = "--"
            row(7) = "--"
            row(8) = "--"
            row(9) = "--"
            row(10) = "--"
            row(11) = "--"
            row(12) = "--"
            row(13) = "--"
            row(14) = "--"
            row(15) = "--"
            DbTable.Rows.Add(row)
            GridView1.DataSource = DbTable
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("Testing" & ex.Message)
        End Try
    End Sub
#End Region
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            Dim Userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userlist As String = Request.Cookies("userinfo")("userslist")


            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim dr As SqlDataReader

            Dim strSql As String = "select userid, username,dbip from userTBL where role='User' order by username"
            Dim sqlCmd As New SqlCommand(strSql, conn)
            If role = "User" Then
                strSql = "select userid, username, dbip from userTBL where userid='" & Userid & "'"
                sqlCmd = New SqlCommand(strSql, conn)
            ElseIf role = "Operator" Or role = "SuperUser" Then
                strSql = "select userid, username, dbip from userTBL where userid in (" & userlist & ") order by username"
                sqlCmd = New SqlCommand(strSql, conn)
            End If
            conn.Open()
            dr = sqlCmd.ExecuteReader
            If dr.HasRows Then
                If role = "SuperUser" Then
                    ddlUsername.Items.Add(New ListItem("ALL USERS", "ALL"))
                End If
                While dr.Read
                    ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
                End While
            End If
            conn.Close()

            If role = "User" Or role = "SuperUser" Or role = "Admin" Then
                ddlUsername.Items.Remove("--Select User Name--")
                'ddlVehicle.Items.Add("--All Vehicle--")
                GetPlateNo(ddlUsername.SelectedValue)
            End If

        Catch ex As Exception
            Response.Write("Oninit : " & ex.Message)
        End Try

        MyBase.OnInit(e)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            If Page.IsPostBack = False Then
                Dim startyear As Integer = Now.Date.AddYears(-1).Year
                Dim dt As DateTime = Convert.ToDateTime(startyear & "/01/01")
                While Now.Date.Year >= startyear
                    While dt.DayOfWeek <> DayOfWeek.Monday
                        dt = dt.AddDays(1)
                    End While
                    'Response.Write(dt.ToString("yyyy/MM/dd"))
                    DropDownList1.Items.Add(New ListItem((dt.Year).ToString(), dt.Year.ToString() & "/01/" & dt.Day))
                    startyear = startyear + 1
                    dt = Convert.ToDateTime(startyear & "/01/01")
                End While
                DropDownList1.SelectedIndex = 1
                Fillddlweek()
            End If
            '  Testing()
        End If
    End Sub
    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        'If radDate.Checked = True Then
        '    FillViolations()
        'Else
        FillViolationsGroupByPlateNo()
        ' End If

    End Sub
    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        ddlVehicle.Items.Clear()
        GetPlateNo(ddlUsername.SelectedValue)
    End Sub

    Protected Sub DropDownList1_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles DropDownList1.SelectedIndexChanged
        Fillddlweek()
    End Sub
    Public Sub Fillddlweek()
        Dim aa As ArrayList
        Try
            aa = Getweeks(DropDownList1.SelectedValue)

            Dim wn As Integer = 0
            ddlweek.Items.Clear()
            While wn < aa(0).Count
                ddlweek.Items.Add(New ListItem(aa(0)(wn), aa(1)(wn)))
                wn = wn + 1
            End While
        Catch ex As Exception
            Response.Write("1." & ex.Message & "Length")
        End Try
    End Sub

End Class