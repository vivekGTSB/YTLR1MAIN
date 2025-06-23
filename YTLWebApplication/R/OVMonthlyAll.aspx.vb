Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI
Imports System.Math
Imports System.Globalization
Imports System.Drawing
Partial Class Lafarge_OVMonthlyAll
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
        Dim currentCount, speedException, harshDec, Idle, contDrive, DriveHour, WorkHour, TotalVio, unsafeWork, unsafeDrive, totalUnsafeWork, totalUnsafeDrive, midnightcount As Integer
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

                .Add(New DataColumn("Total Drive Hour")) '13
                .Add(New DataColumn("Total Work Hour")) '14
                .Add(New DataColumn("more14Work")) '15
                .Add(New DataColumn("more10Drive")) '16
                .Add(New DataColumn("Distance Travel")) '17
                .Add(New DataColumn("midnightcount"))
            End With
            Label5.Visible = True
            Label6.Visible = True
            Label7.Visible = True
            Label8.Visible = False
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
            ' Response.Write(Convert.ToDateTime(strBeginDateTime).Month & "--" & Now.Year & "--" & Now.Month)
            If Not (Convert.ToDateTime(strBeginDateTime).Year = Now.Year And Convert.ToDateTime(strBeginDateTime).Month = Now.Month) Then
                Label3.Text = Convert.ToDateTime(strEndDateTime).ToString("yyyy/MM/dd") & " 23:59:59"
            End If

            Label4.Text = " To "
            Label1.Text = "Driver Behavior Report(Monthly) From "



            strSql = "select t1.username,t1.plateno,t1.overspeed + t2.overspeed as overspeed,t1.harshDec+ t2.harshDec as harshDec,t1.harchAcc + t2.harchAcc as harchAcc," &
            "t1.idling + t2.idling as idling,t1.unstop + t2.unstop as unstop,t1.unroad + t2.unroad as unroad,t1.banhour + t2.banhour as banhour," &
            "t1.totalCont4HourCount + t2.totalCont4HourCount as totalCont4HourCount,t1.contdrive + t2.contdrive as contdrive,t1.drivehour + t2.drivehour as drivehour,t1.totalCont4HourDrive + t2.totalCont4HourDrive as totalCont4HourDrive," &
            "t1.totalvio + t2.totalvio as totalvio,t1.distance + t2.distance as distance,t1.totaldrivehour + t2.totaldrivehour as totaldrivehour,t1.totalworkhour + t2.totalworkhour as totalworkhour" &
            " from (SELECT username,plateno,overspeed,harshDec,harchAcc,idling,midnightcount,unstop,unroad,banhour,totalCont4HourCount,contDrive, DriveHour, totalCont4HourDrive, TotalVio, distance, totaldrivehour, totalworkhour" &
            " FROM (select u.username, substring(o.plateno,CHARINDEX(o.plateno,'_'),CHARINDEX('_',o.plateno))as plateno,sum(o.overspeed) as overspeed,sum(o.midnightcount) as midnightcount ,sum(o.hdec) as harshDec,sum(o.hacc) as harchAcc,sum(o.idling) as idling,sum(o.unstop) as unstop,sum(o.unroad) as unroad,sum(o.banhour) as banhour,sum(o.totalCont4HourCount) as totalCont4HourCount," &
            "sum(o.contdrive) as contdrive,sum(o.drivehour) as drivehour,sum(o.totalCont4HourDrive) as totalCont4HourDrive,sum(o.totalvio) as totalvio,sum(o.distance) as distance,sum(o.totaldriverhour) as totaldrivehour,sum(o.totalworkhour) as totalworkhour, ROW_NUMBER() OVER (PARTITION BY u.username ORDER BY o.plateno) AS RN" &
             " FROM ohsas_violation o,vehicleTBL v, userTBL u WHERE substring(v.plateno,CHARINDEX(v.plateno,'_'),CHARINDEX('_',v.plateno))=substring(o.plateno,CHARINDEX(o.plateno,'_'),CHARINDEX('_',o.plateno))" &
            " and v.userid = '" & ddlUsername.SelectedValue & "' and o.timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' group by  o.plateno,u.username) AS t WHERE RN = 1) as t1," &
            "(SELECT username,plateno,overspeed,harshDec,harchAcc,idling,midnightcount,unstop,unroad,banhour,totalCont4HourCount,contDrive, DriveHour, totalCont4HourDrive, TotalVio, distance, totaldrivehour, totalworkhour" &
            " FROM (select u.username, substring(o.plateno,CHARINDEX(o.plateno,'_'),CHARINDEX('_',o.plateno))as plateno,sum(o.overspeed) as overspeed,sum(o.midnightcount) as midnightcount,sum(o.hdec) as harshDec,sum(o.hacc) as harchAcc,sum(o.idling) as idling,sum(o.unstop) as unstop,sum(o.unroad) as unroad,sum(o.banhour) as banhour,sum(o.totalCont4HourCount) as totalCont4HourCount," &
            "sum(o.contdrive) as contdrive,sum(o.drivehour) as drivehour,sum(o.totalCont4HourDrive) as totalCont4HourDrive,sum(o.totalvio) as totalvio,sum(o.distance) as distance,sum(o.totaldriverhour) as totaldrivehour,sum(o.totalworkhour) as totalworkhour," &
            "ROW_NUMBER() OVER (PARTITION BY u.username ORDER BY o.plateno) AS RN" &
            " FROM ohsas_violation o,vehicleTBL v, userTBL u" &
            " WHERE substring(v.plateno,CHARINDEX(v.plateno,'_'),CHARINDEX('_',v.plateno))=substring(o.plateno,CHARINDEX(o.plateno,'_'),CHARINDEX('_',o.plateno))" &
            " and v.userid ='" & ddlUsername.SelectedValue & "' and o.timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "'" &
            " group by  o.plateno,u.username) AS t WHERE RN = 2) as t2"


            If ddlUsername.Text = "--ALL User--" Then
                strSql = "SELECT u.username,o.plateno, sum(o.overspeed) as overspeed,sum(o.midnightcount) as midnightcount,sum(o.hdec) as harshDec,sum(o.hacc) as harchAcc,sum(o.idling) as idling,sum(o.unstop) as unstop,sum(o.unroad) as unroad,sum(o.banhour) as banhour,sum(o.totalCont4HourCount) as totalCont4HourCount," &
                   "sum(o.contdrive) as contdrive,sum(o.drivehour) as drivehour,sum(o.totalCont4HourDrive) as totalCont4HourDrive,sum(o.totalvio) as totalvio,sum(o.distance) as distance,sum(o.totaldriverhour) as totaldrivehour,sum(o.totalworkhour) as totalworkhour" &
                   " FROM userTBL u inner join vehicleTBL v on u.userid=v.userid inner join ohsas_violation o on v.plateno=o.plateno Where o.timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' group by o.plateno,u.username order by u.username,o.plateno"
            Else
                '        strSql = "select t1.username,t1.plateno,t1.overspeed + t2.overspeed as overspeed,t1.harshDec+ t2.harshDec as harshDec,t1.harchAcc + t2.harchAcc as harchAcc," & _
                '  "t1.idling + t2.idling as idling,t1.unstop + t2.unstop as unstop,t1.unroad + t2.unroad as unroad,t1.banhour + t2.banhour as banhour," & _
                '  "t1.totalCont4HourCount + t2.totalCont4HourCount as totalCont4HourCount,t1.contdrive + t2.contdrive as contdrive,t1.drivehour + t2.drivehour as drivehour,t1.totalCont4HourDrive + t2.totalCont4HourDrive as totalCont4HourDrive," & _
                '  "t1.totalvio + t2.totalvio as totalvio,t1.distance + t2.distance as distance,t1.totaldrivehour + t2.totaldrivehour as totaldrivehour,t1.totalworkhour + t2.totalworkhour as totalworkhour" & _
                '  " from (SELECT username,plateno,overspeed,harshDec,harchAcc,idling,unstop,unroad,banhour,totalCont4HourCount,contDrive, DriveHour, totalCont4HourDrive, TotalVio, distance, totaldrivehour, totalworkhour" & _
                '  " FROM (select u.username, substring(o.plateno,CHARINDEX(o.plateno,'_'),CHARINDEX('_',o.plateno))as plateno,sum(o.overspeed) as overspeed,sum(o.hdec) as harshDec,sum(o.hacc) as harchAcc,sum(o.idling) as idling,sum(o.unstop) as unstop,sum(o.unroad) as unroad,sum(o.banhour) as banhour,sum(o.totalCont4HourCount) as totalCont4HourCount," & _
                '  "sum(o.contdrive) as contdrive,sum(o.drivehour) as drivehour,sum(o.totalCont4HourDrive) as totalCont4HourDrive,sum(o.totalvio) as totalvio,sum(o.distance) as distance,sum(o.totaldriverhour) as totaldrivehour,sum(o.totalworkhour) as totalworkhour, ROW_NUMBER() OVER (PARTITION BY u.username ORDER BY o.plateno) AS RN" & _
                '   " FROM userTBL u inner join vehicleTBL v on u.userid=v.userid " & _
                '" inner join ohsas_violation o on substring(v.plateno,CHARINDEX(v.plateno,'_'),CHARINDEX('_',v.plateno))=substring(o.plateno,CHARINDEX(o.plateno,'_'),CHARINDEX('_',o.plateno)) " & _
                '"  WHERE v.userid = '" & ddlUsername.SelectedValue & "' and o.timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' group by  o.plateno,u.username) AS t WHERE RN = 1) as t1," & _
                '"(SELECT username,plateno,overspeed,harshDec,harchAcc,idling,unstop,unroad,banhour,totalCont4HourCount,contDrive, DriveHour, totalCont4HourDrive, TotalVio, distance, totaldrivehour, totalworkhour" & _
                '" FROM (select u.username, substring(o.plateno,CHARINDEX(o.plateno,'_'),CHARINDEX('_',o.plateno))as plateno,sum(o.overspeed) as overspeed,sum(o.hdec) as harshDec,sum(o.hacc) as harchAcc,sum(o.idling) as idling,sum(o.unstop) as unstop,sum(o.unroad) as unroad,sum(o.banhour) as banhour,sum(o.totalCont4HourCount) as totalCont4HourCount," & _
                '"sum(o.contdrive) as contdrive,sum(o.drivehour) as drivehour,sum(o.totalCont4HourDrive) as totalCont4HourDrive,sum(o.totalvio) as totalvio,sum(o.distance) as distance,sum(o.totaldriverhour) as totaldrivehour,sum(o.totalworkhour) as totalworkhour," & _
                '"ROW_NUMBER() OVER (PARTITION BY u.username ORDER BY o.plateno) AS RN" & _
                ' " FROM userTBL u inner join vehicleTBL v on u.userid=v.userid " & _
                '        " inner join ohsas_violation o on substring(v.plateno,CHARINDEX(v.plateno,'_'),CHARINDEX('_',v.plateno))=substring(o.plateno,CHARINDEX(o.plateno,'_'),CHARINDEX('_',o.plateno)) " & _
                '        " where v.userid ='" & ddlUsername.SelectedValue & "' and o.timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "'" & _
                '        " group by  o.plateno,u.username) AS t WHERE RN = 2) as t2"
                strSql = "SELECT u.username,o.plateno, sum(o.overspeed) as overspeed,sum(o.midnightcount) as midnightcount,sum(o.hdec) as harshDec,sum(o.hacc) as harchAcc,sum(o.idling) as idling,sum(o.unstop) as unstop,sum(o.unroad) as unroad,sum(o.banhour) as banhour,sum(o.totalCont4HourCount) as totalCont4HourCount," &
                   "sum(o.contdrive) as contdrive,sum(o.drivehour) as drivehour,sum(o.totalCont4HourDrive) as totalCont4HourDrive,sum(o.totalvio) as totalvio,sum(o.distance) as distance,sum(o.totaldriverhour) as totaldrivehour,sum(o.totalworkhour) as totalworkhour" &
                   " FROM userTBL u inner join vehicleTBL v on u.userid=v.userid inner join ohsas_violation o on v.plateno=o.plateno Where u.userid='" & ddlUsername.SelectedValue & "' and o.timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' group by o.plateno,u.username order by u.username,o.plateno"
                'Response.Write(strSql)
                'Exit Sub

            End If

            adpViolatinon = New SqlDataAdapter(strSql, conn)
            adpViolatinon.Fill(ds)
            If ds.Tables(0).Rows.Count <> 0 Then
                Label9.Visible = False
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    'strSql = "Select username from userTBL u inner join vehicleTBL v on u.userid=v.userid Where v.plateno='" & ds.Tables(0).Rows(i)("plateno") & "'"
                    'sqlCmd = New SqlCommand(strSql, conn)
                    'username = sqlCmd.ExecuteScalar
                    ' Response.Write(username & "<br />")
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
                            row(8) = "<font color=""Red""><b>" & Idle & "</b></font>"
                            row(9) = "<font color=""Red""><b>" & contDrive & "</b></font>"
                            row(10) = " <font color=""Red""><b>" & DriveHour & "</b></font>"
                            Dim driverWorkHour1 As New TimeSpan(0, 0, WorkHour)
                            row(11) = "<font color=""Red""><b>" & ConvertHours(driverWorkHour1.ToString) & "</b></font>"
                            row(12) = "<font color=""Red""><b>" & TotalVio & "</b></font>"
                            Dim totalcountDrive As New TimeSpan(0, totalDrive, 0)
                            Dim totalCountWork As New TimeSpan(0, totalWork, 0)
                            row(13) = "<font color=""Red""><b>" & ConvertHours(totalcountDrive.ToString) & "</b></font>"
                            row(14) = "<font color=""Red""><b>" & ConvertHours(totalCountWork.ToString) & "</b></font>"
                            row(15) = "<font color=""Red""><b>" & ConvertHours(totalUnsafeWork.ToString) & "</b></font>"
                            row(16) = "<font color=""Red""><b>" & ConvertHours(totalUnsafeDrive.ToString) & "</b></font>"
                            row(17) = "<font color=""Red""><b>" & distance & "</b></font>"
                            row(18) = "<font color=""Red""><b>" & midnightcount & "</b></font>"
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
                        'strSql = "select COUNT(*) from ohsas_violation where plateno='" & ds.Tables(0).Rows(i)("plateno") & "' and " & _
                        '      "timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' and totalworkhour >=50400"
                        '' Response.Write(strSql & "<br/>")

                        'sqlCmd2 = New SqlCommand(strSql, conn)
                        'unsafeWork = sqlCmd2.ExecuteScalar
                        'row(15) = unsafeWork
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
                    row(4) = Round((ds.Tables(0).Rows(i)("harchAcc") / 22))
                    row(5) = ds.Tables(0).Rows(i)("unstop")
                    row(6) = ds.Tables(0).Rows(i)("unroad")
                    row(7) = ds.Tables(0).Rows(i)("banhour")
                    row(8) = ds.Tables(0).Rows(i)("idling")
                    row(9) = ds.Tables(0).Rows(i)("contdrive")
                    row(10) = ds.Tables(0).Rows(i)("totalCont4HourCount")
                    ' Dim driverworkhourValue As New TimeSpan(0, 0, ds.Tables(0).Rows(i)("workhour"))
                    Dim TotalContinuesDriveHour As New TimeSpan(0, 0, ds.Tables(0).Rows(i)("totalCont4HourDrive"))
                    row(11) = ConvertHours(TotalContinuesDriveHour.ToString)
                    row(12) = Round((ds.Tables(0).Rows(i)("harshDec") / 22)) + Round((ds.Tables(0).Rows(i)("harchAcc") / 22)) + ds.Tables(0).Rows(i)("unstop") + ds.Tables(0).Rows(i)("unroad") + ds.Tables(0).Rows(i)("banhour") + ds.Tables(0).Rows(i)("idling") + ds.Tables(0).Rows(i)("drivehour") + ds.Tables(0).Rows(i)("contdrive") + ds.Tables(0).Rows(i)("overspeed")
                    Dim hourValue As New TimeSpan(0, ds.Tables(0).Rows(i)("totaldrivehour"), 0)
                    row(13) = ConvertHours(hourValue.ToString)
                    Dim workHourValue As New TimeSpan(0, ds.Tables(0).Rows(i)("totalworkhour"), 0)
                    row(14) = ConvertHours(workHourValue.ToString)
                    strSql = "select COUNT(*) from ohsas_violation where plateno='" & ds.Tables(0).Rows(i)("plateno") & "' and " &
                         "timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' and totalworkhour >=840"
                    sqlCmd2 = New SqlCommand(strSql, conn)
                    unsafeWork = sqlCmd2.ExecuteScalar
                    row(15) = unsafeWork

                    strSql = "select COUNT(*) from ohsas_violation where plateno='" & ds.Tables(0).Rows(i)("plateno") & "' and " &
                        "timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "' and totaldriverhour >=600"
                    sqlCmd3 = New SqlCommand(strSql, conn)
                    unsafeDrive = sqlCmd3.ExecuteScalar
                    row(16) = unsafeDrive

                    row(17) = Format(ds.Tables(0).Rows(i)("distance"), "0.00")
                    row(18) = ds.Tables(0).Rows(i)("midnightcount")

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
                    totalUnsafeWork = totalUnsafeWork + unsafeWork
                    totalUnsafeDrive = totalUnsafeDrive + unsafeDrive
                    DbTable.Rows.Add(row)
                    PrevUsername = CurrUserName

                Next
                row = DbTable.NewRow
                row(1) = "<font color=""Red""><b> Total :</b></font>"
                row(2) = "<font color=""Red""><b>" & speedException & "</b></font>"
                row(3) = "<font color=""Red""><b>" & Round((harshDec)) & "</b></font>"
                row(8) = "<font color=""Red""><b>" & Idle & "</b></font>"
                row(9) = "<font color=""Red""><b>" & contDrive & "</b></font>"
                row(10) = "<font color=""Red""><b>" & DriveHour & "</b></font>"
                Dim driverWorkHour As New TimeSpan(0, 0, WorkHour)
                row(11) = "<font color=""Red""><b>" & ConvertHours(driverWorkHour.ToString) & "</b></font>"
                row(12) = "<font color=""Red""><b>" & TotalVio & "</b></font>"
                Dim totalcountDrive1 As New TimeSpan(0, totalDrive, 0)
                Dim totalCountWork1 As New TimeSpan(0, totalWork, 0)
                row(13) = "<font color=""Red""><b>" & ConvertHours(totalcountDrive1.ToString) & "</b></font>"
                row(14) = "<font color=""Red""><b>" & ConvertHours(totalCountWork1.ToString) & "</b></font>"
                row(15) = "<font color=""Red""><b>" & ConvertHours(totalUnsafeWork.ToString) & "</b></font>"
                row(16) = "<font color=""Red""><b>" & ConvertHours(totalUnsafeDrive.ToString) & "</b></font>"
                row(17) = "<font color=""Red""><b>" & distance & "</b></font>"
                row(18) = "<font color=""Red""><b>" & midnightcount & "</b></font>"

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
                row(18) = "-"
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
                        .Cells(18).BackColor = System.Drawing.Color.Yellow
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
            Dim strSql As String = "select userid, username,dbip from userTBL where role='User' order by username"
            Dim sqlCmd As New SqlCommand(strSql, conn)
            If role = "User" Then
                strSql = "select userid, username, dbip from userTBL where userid='" & Userid & "'"
                sqlCmd = New SqlCommand(strSql, conn)
            ElseIf role = "Operator" Or role = "SuperUser" Then
                strSql = "select userid, username, dbip from userTBL where userid in (" & userlist & ") order by username"
                sqlCmd = New SqlCommand(strSql, conn)
            End If
            If Userid <> "1967" And Userid <> "1948" And Userid <> "742" And Userid <> "1968" And Userid <> "1943" Then
                ddlUsername.Items.Remove("--ALL User--")
            End If

            conn.Open()
            dr = sqlCmd.ExecuteReader
            While dr.Read
                ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While
            conn.Close()

            If role = "User" Or role = "Superuser" Then
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
