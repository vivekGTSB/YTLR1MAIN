Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI
Imports System.Math
Imports System.Globalization
Imports System.Drawing




Partial Class OhsasVehicleMonthlySpeedratings
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public show As Boolean = False
#Region "Function"

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
    Protected Sub FillViolations()
        Dim strBeginDateTime, strEndDateTime, getProcessEndDate, strSql, CurrUserName, PrevUsername, username As String
        Dim currentCount, speedException, harshDec, Idle, contDrive, DriveHour, WorkHour, TotalVio, unsafeWork, unsafeDrive, totalUnsafeWork, totalUnsafeDrive, midnightcount, speed8085, speed8590, speed9095, speed95100, speed100 As Integer
        Dim distance, totalDrive, totalWork As Double
        Dim ds As New DataSet
        Dim adpViolatinon As SqlDataAdapter
        Dim row As DataRow
        Dim sqlCmd As SqlCommand
        Dim str() As String = DropDownList1.SelectedValue.Split("/")
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        strBeginDateTime = str(0) & "-" & str(1) & "-" & "01" & " " & "00:00:00"
        Dim lastDay As Integer
        Dim sqlCmd2 As SqlCommand
        Dim sqlCmd3 As SqlCommand
        lastDay = DateAdd(DateInterval.Day, -1, DateSerial(str(0), str(1) + 1, 1)).Day
        strEndDateTime = str(0) & "-" & str(1) & "-" & lastDay & " " & "23:59:59"
        Try

            Dim DbTable As New DataTable
            With DbTable.Columns
                .Add(New DataColumn("No"))
                .Add(New DataColumn("Vehicle No"))
                .Add(New DataColumn("Speeding Freequency"))
                .Add(New DataColumn("Harsh Braking Frequency"))
                .Add(New DataColumn("Excessive Idle"))
                .Add(New DataColumn("Continuous Driving > 4 Hrs"))
                .Add(New DataColumn("Total Continuous Driving Hours"))
                .Add(New DataColumn("Total Driving Hour"))
                .Add(New DataColumn("Total Working Hour"))
                .Add(New DataColumn("Frequency of > 14 Hrs Work"))
                .Add(New DataColumn("Frequency of > 10 Hrs Drive"))
                .Add(New DataColumn("Distance Travel"))
                .Add(New DataColumn("Mid-Night Count"))
                .Add(New DataColumn("Rating 1 (80 to 84.99)"))
                .Add(New DataColumn("Rating 2 (85 to 89.99)"))
                .Add(New DataColumn("Rating 3 (90 to 94.99)"))
                .Add(New DataColumn("Rating 4 (95 to 99.99)"))
                .Add(New DataColumn("Rating 5 (>=100)"))
            End With
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim role As String = Request.Cookies("userinfo")("role")
            Label5.Visible = True
            Label6.Visible = True
            Label7.Visible = True
            ' Label8.Visible = False
            Label1.ForeColor = Color.Blue
            Label4.ForeColor = Color.Blue
            Label2.ForeColor = Color.Red
            Label2.Font.Bold = True
            Label3.ForeColor = Color.Red
            Label3.Font.Bold = True
            Label2.Text = " " & Convert.ToDateTime(strBeginDateTime).ToString("yyyy/MM/dd HH:mm:ss")
            strSql = "Select top 1 timestamp from ohsas_violation order by timestamp desc"
            sqlCmd = New SqlCommand(strSql, conn)
            conn.Open()
            getProcessEndDate = sqlCmd.ExecuteScalar
            sqlCmd.Dispose()
            Label3.Text = Convert.ToDateTime(getProcessEndDate).ToString("yyyy/MM/dd") & " 23:59:59"
            If Not (Convert.ToDateTime(strBeginDateTime).Year = Now.Year And Convert.ToDateTime(strBeginDateTime).Month = Now.Month) Then
                Label3.Text = Convert.ToDateTime(strEndDateTime).ToString("yyyy/MM/dd") & " 23:59:59"
            End If

            Label4.Text = " To "
            Label1.Text = "Driver Behavior Report(Monthly) From "

            If ddlUsername.Text = "--ALL User--" Then
                If role = "Admin" Then
                    strSql = "SELECT u.username,o.plateno, sum(o.overspeed) as overspeed,sum(o.midnightcount) as midnightcount,sum(o.hdec) as harshDec,sum(o.hacc) as harchAcc,sum(o.idling) as idling,sum(o.unstop) as unstop,sum(o.unroad) as unroad,sum(o.banhour) as banhour,sum(o.totalCont4HourCount) as totalCont4HourCount," &
                       "sum(o.contdrive) as contdrive,sum(o.drivehour) as drivehour,sum(o.totalCont4HourDrive) as totalCont4HourDrive,sum(o.totalvio) as totalvio,sum(o.distance) as distance,sum(o.totaldriverhour) as totaldrivehour,sum(o.totalworkhour) as totalworkhour ,sum(o.speed8085) as speed8085 ,sum(o.speed8590) as speed8590,sum(o.speed9095) as speed9095 ,sum(o.speed95100) as speed95100 ,sum(o.speed100) as speed100" &
                       " FROM userTBL u inner join vehicleTBL v on u.userid=v.userid and u.customrole='transporter' inner join ohsas_violation o on v.plateno=o.plateno Where   o.timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' group by o.plateno,u.username order by u.username,o.plateno"
                ElseIf role = "SuperUser" Then
                    strSql = "SELECT u.username,o.plateno, sum(o.overspeed) as overspeed,sum(o.midnightcount) as midnightcount,sum(o.hdec) as harshDec,sum(o.hacc) as harchAcc,sum(o.idling) as idling,sum(o.unstop) as unstop,sum(o.unroad) as unroad,sum(o.banhour) as banhour,sum(o.totalCont4HourCount) as totalCont4HourCount," &
                       "sum(o.contdrive) as contdrive,sum(o.drivehour) as drivehour,sum(o.totalCont4HourDrive) as totalCont4HourDrive,sum(o.totalvio) as totalvio,sum(o.distance) as distance,sum(o.totaldriverhour) as totaldrivehour,sum(o.totalworkhour) as totalworkhour ,sum(o.speed8085) as speed8085 ,sum(o.speed8590) as speed8590,sum(o.speed9095) as speed9095 ,sum(o.speed95100) as speed95100 ,sum(o.speed100) as speed100" &
                       " FROM userTBL u inner join vehicleTBL v on u.userid=v.userid and u.customrole='transporter' inner join ohsas_violation o on v.plateno=o.plateno Where u.userid in (" & userslist & ") and  o.timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' group by o.plateno,u.username order by u.username,o.plateno"
                End If
            ElseIf ddlUsername.Text = "--ALL Trans--" Then
                If role = "Admin" Then
                    strSql = "SELECT u.username,o.plateno, sum(o.overspeed) as overspeed,sum(o.midnightcount) as midnightcount,sum(o.hdec) as harshDec,sum(o.hacc) as harchAcc,sum(o.idling) as idling,sum(o.unstop) as unstop,sum(o.unroad) as unroad,sum(o.banhour) as banhour,sum(o.totalCont4HourCount) as totalCont4HourCount," &
                       "sum(o.contdrive) as contdrive,sum(o.drivehour) as drivehour,sum(o.totalCont4HourDrive) as totalCont4HourDrive,sum(o.totalvio) as totalvio,sum(o.distance) as distance,sum(o.totaldriverhour) as totaldrivehour,sum(o.totalworkhour) as totalworkhour ,sum(o.speed8085) as speed8085 ,sum(o.speed8590) as speed8590,sum(o.speed9095) as speed9095 ,sum(o.speed95100) as speed95100 ,sum(o.speed100) as speed100" &
                       " FROM userTBL u inner join vehicleTBL v on u.userid=v.userid and u.customrole='transporter' inner join ohsas_violation o on v.plateno=o.plateno Where   o.timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' group by o.plateno,u.username order by u.username,o.plateno"
                ElseIf role = "SuperUser" Then
                    strSql = "SELECT u.username,o.plateno, sum(o.overspeed) as overspeed,sum(o.midnightcount) as midnightcount,sum(o.hdec) as harshDec,sum(o.hacc) as harchAcc,sum(o.idling) as idling,sum(o.unstop) as unstop,sum(o.unroad) as unroad,sum(o.banhour) as banhour,sum(o.totalCont4HourCount) as totalCont4HourCount," &
                       "sum(o.contdrive) as contdrive,sum(o.drivehour) as drivehour,sum(o.totalCont4HourDrive) as totalCont4HourDrive,sum(o.totalvio) as totalvio,sum(o.distance) as distance,sum(o.totaldriverhour) as totaldrivehour,sum(o.totalworkhour) as totalworkhour ,sum(o.speed8085) as speed8085 ,sum(o.speed8590) as speed8590,sum(o.speed9095) as speed9095 ,sum(o.speed95100) as speed95100 ,sum(o.speed100) as speed100" &
                       " FROM userTBL u inner join vehicleTBL v on u.userid=v.userid and u.customrole='transporter' inner join ohsas_violation o on v.plateno=o.plateno Where u.userid in (" & userslist & ") and  o.timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' group by o.plateno,u.username order by u.username,o.plateno"
                End If
            Else
                strSql = "SELECT u.username,o.plateno, sum(o.overspeed) as overspeed,sum(o.midnightcount) as midnightcount,sum(o.hdec) as harshDec,sum(o.hacc) as harchAcc,sum(o.idling) as idling,sum(o.unstop) as unstop,sum(o.unroad) as unroad,sum(o.banhour) as banhour,sum(o.totalCont4HourCount) as totalCont4HourCount," &
         "sum(o.contdrive) as contdrive,sum(o.drivehour) as drivehour,sum(o.totalCont4HourDrive) as totalCont4HourDrive,sum(o.totalvio) as totalvio,sum(o.distance) as distance,sum(o.totaldriverhour) as totaldrivehour,sum(o.totalworkhour) as totalworkhour ,sum(o.speed8085) as speed8085 ,sum(o.speed8590) as speed8590,sum(o.speed9095) as speed9095 ,sum(o.speed95100) as speed95100 ,sum(o.speed100) as speed100" &
         " FROM userTBL u inner join vehicleTBL v on u.userid=v.userid and  u.customrole='transporter' inner join ohsas_violation o on v.plateno=o.plateno Where u.userid='" & ddlUsername.SelectedValue & "' and o.timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' group by o.plateno,u.username order by u.username,o.plateno"

            End If

            adpViolatinon = New SqlDataAdapter(strSql, conn)
            adpViolatinon.Fill(ds)
            If ds.Tables(0).Rows.Count <> 0 Then
                Label9.Visible = False
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    If i = 0 Then
                        CurrUserName = ds.Tables(0).Rows(i)("username")
                        row = DbTable.NewRow
                        row(1) = "Username : <font color=""Blue""><b>" & CurrUserName & "</b></font>"
                        DbTable.Rows.Add(row)

                    Else
                        CurrUserName = ds.Tables(0).Rows(i)("username")

                        If PrevUsername <> CurrUserName And i > 1 Then
                            row = DbTable.NewRow
                            row(1) = "<font color=""Red""><b> Total :</b></font>"
                            row(2) = "<font color=""Red""><b>" & speedException & "</b></font>"
                            row(3) = "<font color=""Red""><b>" & harshDec & "</b></font>"
                            row(4) = "<font color=""Red""><b>" & Idle & "</b></font>"
                            row(5) = " <font color=""Red""><b>" & DriveHour & "</b></font>"

                            Dim driverWorkHour1 As New TimeSpan(0, 0, WorkHour)
                            row(6) = "<font color=""Red""><b>" & ConvertHours(driverWorkHour1.ToString) & "</b></font>"

                            Dim totalcountDrive As New TimeSpan(0, totalDrive, 0)
                            Dim totalCountWork As New TimeSpan(0, totalWork, 0)
                            row(7) = "<font color=""Red""><b>" & ConvertHours(totalcountDrive.ToString) & "</b></font>"
                            row(8) = "<font color=""Red""><b>" & ConvertHours(totalCountWork.ToString) & "</b></font>"
                            row(9) = "<font color=""Red""><b>" & ConvertHours(totalUnsafeWork.ToString) & "</b></font>"
                            row(10) = "<font color=""Red""><b>" & ConvertHours(totalUnsafeDrive.ToString) & "</b></font>"
                            row(11) = "<font color=""Red""><b>" & distance & "</b></font>"
                            row(12) = "<font color=""Red""><b>" & midnightcount & "</b></font>"
                            row(13) = "<font color=""Red""><b>" & speed8085 & "</b></font>"
                            row(14) = "<font color=""Red""><b>" & speed8590 & "</b></font>"
                            row(15) = "<font color=""Red""><b>" & speed9095 & "</b></font>"
                            row(16) = "<font color=""Red""><b>" & speed95100 & "</b></font>"
                            row(17) = "<font color=""Red""><b>" & speed100 & "</b></font>"
                            DbTable.Rows.Add(row)

                            speedException = 0
                            harshDec = 0
                            Idle = 0
                            contDrive = 0
                            DriveHour = 0
                            WorkHour = 0
                            TotalVio = 0
                            distance = 0
                            totalDrive = 0
                            totalWork = 0
                            midnightcount = 0
                            speed8085 = 0
                            speed8590 = 0
                            speed9095 = 0
                            speed95100 = 0
                            speed100 = 0
                        End If
                        If CurrUserName <> PrevUsername Then

                            row = DbTable.NewRow
                            DbTable.Rows.Add(row)
                        End If
                        If CurrUserName <> PrevUsername Then
                            totalUnsafeDrive = 0
                            totalUnsafeWork = 0
                            row = DbTable.NewRow
                            row(1) = "Username : <font color=""Blue""><b>" & CurrUserName & "</b></font>"
                            DbTable.Rows.Add(row)
                        End If

                    End If

                    If PrevUsername <> CurrUserName Then
                        currentCount = 1
                    Else
                        currentCount = currentCount + 1
                    End If





                    row = DbTable.NewRow


                    row(0) = currentCount
                    row(1) = ds.Tables(0).Rows(i)("plateno")
                    row(2) = ds.Tables(0).Rows(i)("overspeed")
                    row(3) = Round((ds.Tables(0).Rows(i)("harshDec") / 22))
                    row(4) = ds.Tables(0).Rows(i)("idling")

                    row(5) = ds.Tables(0).Rows(i)("totalCont4HourCount")
                    Dim TotalContinuesDriveHour As New TimeSpan(0, 0, ds.Tables(0).Rows(i)("totalCont4HourDrive"))
                    row(6) = ConvertHours(TotalContinuesDriveHour.ToString)
                    Dim hourValue As New TimeSpan(0, ds.Tables(0).Rows(i)("totaldrivehour"), 0)
                    row(7) = ConvertHours(hourValue.ToString)
                    Dim workHourValue As New TimeSpan(0, ds.Tables(0).Rows(i)("totalworkhour"), 0)
                    row(8) = ConvertHours(workHourValue.ToString)
                    strSql = "select COUNT(*) from ohsas_violation where plateno='" & ds.Tables(0).Rows(i)("plateno") & "' and " &
                         "timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' and totalworkhour >=840"
                    sqlCmd2 = New SqlCommand(strSql, conn)
                    unsafeWork = sqlCmd2.ExecuteScalar
                    row(9) = unsafeWork

                    strSql = "select COUNT(*) from ohsas_violation where plateno='" & ds.Tables(0).Rows(i)("plateno") & "' and " &
                        "timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' and totaldriverhour >=600"
                    sqlCmd3 = New SqlCommand(strSql, conn)
                    unsafeDrive = sqlCmd3.ExecuteScalar
                    row(10) = unsafeDrive

                    row(11) = Format(ds.Tables(0).Rows(i)("distance"), "0.00")
                    row(12) = ds.Tables(0).Rows(i)("midnightcount")
                    row(13) = ds.Tables(0).Rows(i)("speed8085")
                    row(14) = ds.Tables(0).Rows(i)("speed8590")
                    row(15) = ds.Tables(0).Rows(i)("speed9095")
                    row(16) = ds.Tables(0).Rows(i)("speed95100")
                    row(17) = ds.Tables(0).Rows(i)("speed100")

                    speedException = speedException + ds.Tables(0).Rows(i)("overspeed")
                    midnightcount = midnightcount + ds.Tables(0).Rows(i)("midnightcount")
                    harshDec = harshDec + Convert.ToInt16(row(3))
                    Idle = Idle + ds.Tables(0).Rows(i)("idling")
                    contDrive = contDrive + ds.Tables(0).Rows(i)("contdrive")
                    DriveHour = DriveHour + ds.Tables(0).Rows(i)("totalCont4HourCount")
                    WorkHour = WorkHour + ds.Tables(0).Rows(i)("totalCont4HourDrive")
                    TotalVio = TotalVio + Convert.ToInt16(row(12))
                    distance = distance + Math.Round(ds.Tables(0).Rows(i)("distance"), 2)
                    totalDrive = totalDrive + ds.Tables(0).Rows(i)("totaldrivehour")
                    totalWork = totalWork + ds.Tables(0).Rows(i)("totalworkhour")
                    speed8085 = speed8085 + Convert.ToInt32(ds.Tables(0).Rows(i)("speed8085"))
                    speed8590 = speed8590 + Convert.ToInt32(ds.Tables(0).Rows(i)("speed8590"))
                    speed9095 = speed9095 + Convert.ToInt32(ds.Tables(0).Rows(i)("speed9095"))
                    speed95100 = speed95100 + Convert.ToInt32(ds.Tables(0).Rows(i)("speed95100"))
                    speed100 = speed100 + Convert.ToInt32(ds.Tables(0).Rows(i)("speed100"))
                    totalUnsafeWork = totalUnsafeWork + unsafeWork
                    totalUnsafeDrive = totalUnsafeDrive + unsafeDrive
                    DbTable.Rows.Add(row)
                    PrevUsername = CurrUserName

                Next
                row = DbTable.NewRow
                row(1) = "<font color=""Red""><b> Total :</b></font>"
                row(2) = "<font color=""Red""><b>" & speedException & "</b></font>"
                row(3) = "<font color=""Red""><b>" & Round((harshDec)) & "</b></font>"
                row(4) = "<font color=""Red""><b>" & Idle & "</b></font>"
                row(5) = "<font color=""Red""><b>" & DriveHour & "</b></font>"
                Dim driverWorkHour As New TimeSpan(0, 0, WorkHour)
                row(6) = "<font color=""Red""><b>" & ConvertHours(driverWorkHour.ToString) & "</b></font>"
                Dim totalcountDrive1 As New TimeSpan(0, totalDrive, 0)
                Dim totalCountWork1 As New TimeSpan(0, totalWork, 0)
                row(7) = "<font color=""Red""><b>" & ConvertHours(totalcountDrive1.ToString) & "</b></font>"
                row(8) = "<font color=""Red""><b>" & ConvertHours(totalCountWork1.ToString) & "</b></font>"
                row(9) = "<font color=""Red""><b>" & ConvertHours(totalUnsafeWork.ToString) & "</b></font>"
                row(10) = "<font color=""Red""><b>" & ConvertHours(totalUnsafeDrive.ToString) & "</b></font>"
                row(11) = "<font color=""Red""><b>" & distance & "</b></font>"
                row(12) = "<font color=""Red""><b>" & midnightcount & "</b></font>"
                row(13) = "<font color=""Red""><b>" & speed8085 & "</b></font>"
                row(14) = "<font color=""Red""><b>" & speed8590 & "</b></font>"
                row(15) = "<font color=""Red""><b>" & speed9095 & "</b></font>"
                row(16) = "<font color=""Red""><b>" & speed95100 & "</b></font>"
                row(17) = "<font color=""Red""><b>" & speed100 & "</b></font>"
                DbTable.Rows.Add(row)

            Else
                Label9.Visible = True
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
                DbTable.Rows.Add(row)
            End If
            conn.Close()
            With GridView1
                .PageSize = noofrecords.SelectedValue
                .DataSource = DbTable
                .DataBind()
                If .PageCount > 1 Then
                    show = True
                End If
            End With
            For i As Integer = 0 To GridView1.Rows.Count - 1
                ' Response.Write(Left(GridView1.Rows.Item(i).Cells(1).Text, 8))
                If Left(GridView1.Rows.Item(i).Cells(1).Text, 8) = "Username" Then
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
                    End With
                End If
            Next
            Session("gridview1") = DbTable
            ec = "true"
            Session("exceltable") = DbTable
            Session("OVMonthlyAllAng") = Label1.Text & Label2.Text & Label4.Text & Label3.Text
        Catch ex As Exception
            Response.Write(ex.Message)
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
            Dim strSql As String = "select userid, username,dbip from userTBL where role='User' and customrole='transporter'  order by username"
            Dim sqlCmd As New SqlCommand(strSql, conn)
            If role = "User" Then
                strSql = "select userid, username, dbip from userTBL where userid='" & Userid & "' and customrole='transporter' "
                sqlCmd = New SqlCommand(strSql, conn)
            ElseIf role = "Operator" Or role = "SuperUser" Then
                strSql = "select userid, username, dbip from userTBL where userid in (" & userlist & ") and customrole='transporter'  order by username"
                sqlCmd = New SqlCommand(strSql, conn)
            End If
            'If Userid <> "1967" And Userid <> "1948" And Userid <> "742" And Userid <> "3340" And Userid <> "1968" And Userid <> "1943" Then
            '    ddlUsername.Items.Remove("--ALL User--")
            'End If

            conn.Open()
            dr = sqlCmd.ExecuteReader
            While dr.Read
                ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While
            conn.Close()

            If role = "User" Then
                ddlUsername.Items.Remove("--ALL User--")
                ddlUsername.Items.Remove("--ALL Transporters--")
                ddlUsername.Items.Remove("--Select User Name--")
            End If
        Catch ex As Exception
            Response.Write("Oninit : " & ex.Message)
        End Try
        MyBase.OnInit(e)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            Dim nowdate As DateTime = Now.Date
            Dim oneYearBackDate As DateTime = Now.Date.AddMonths(-12)
            While nowdate >= oneYearBackDate
                DropDownList1.Items.Add(New ListItem((CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(oneYearBackDate.Month).ToString().ToUpper() & " - " & oneYearBackDate.Year).ToString(), (oneYearBackDate.Year.ToString() & "/" & oneYearBackDate.Month.ToString())))
                oneYearBackDate = oneYearBackDate.AddMonths(1)
            End While
            DropDownList1.SelectedIndex = 12
        End If
    End Sub
    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        FillViolations()
    End Sub
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        Try
            With GridView1
                .PageSize = noofrecords.SelectedValue
                .DataSource = Session("gridview1")
                .PageIndex = e.NewPageIndex
                .DataBind()
            End With
            show = True
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub
End Class
