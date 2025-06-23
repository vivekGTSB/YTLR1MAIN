Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data

Partial Class VehicleOdometerReport
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Dim suspectRefuel As Boolean = False
    Dim suspectTime As String
    Dim GrantTotalOdometer As Double

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
            cmd = New SqlCommand("select userid, username,dbip from userTBL where role='User' order by username", conn)
            If role = "User" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid='" & userid & "'", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
            End If
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While
            dr.Close()
            If role = "User" Then
                ddlUsername.Items.Remove("--Select User Name--")
                ddlUsername.SelectedValue = userid
                getPlateNo(userid)
            End If
            conn.Close()

        Catch ex As Exception

            Response.Write(ex.Message)
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub



    Protected Sub getPlateNo(ByVal uid As String)
        Try
            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select Plate No--")
                Dim cmd As SqlCommand
                Dim dr As SqlDataReader
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & uid & "' order by plateno", conn)
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlpleate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                End While
                dr.Close()

                conn.Close()
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Function getTableDataPost(ByVal begindatetime As String, ByVal enddatetime As String) As List(Of OdometerData)
        Dim odoObj As List(Of OdometerData) = New List(Of OdometerData)()
        Dim conn1 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try
            Dim query As String = "select plateno,timestamp ,beforeodometer,afterodometer ,milage from vehicle_odometer where plateno=@plateno and timestamp between @bdt and @edt  order by timestamp"
            Dim cmd1 As SqlCommand = New SqlCommand(query, conn1)
            cmd1.Parameters.Add(New SqlParameter("@plateno", ddlpleate.SelectedValue))
            cmd1.Parameters.Add(New SqlParameter("@bdt", begindatetime))
            cmd1.Parameters.Add(New SqlParameter("@edt", enddatetime))
            conn1.Open()
            Dim dr As SqlDataReader = cmd1.ExecuteReader
            If dr.HasRows Then
                While dr.Read
                    Dim odo As New OdometerData()
                    odo.Plateno = ddlpleate.SelectedValue
                    odo.StartDateTime = Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd 00:00:00")
                    odo.EndDateTime = Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd 23:59:59")
                    odo.StartOdometer = CDbl(dr("beforeodometer"))
                    odo.EndOdometer = CDbl(dr("afterodometer"))
                    odo.Mileage = CDbl(dr("milage"))
                    odoObj.Add(odo)
                End While
            Else
                Dim odo As New OdometerData()
                odo.Plateno = ddlpleate.SelectedValue
                odo.StartDateTime = "--"
                odo.EndDateTime = "--"
                odo.StartOdometer = 0
                odo.EndOdometer = 0
                odo.Mileage = 0
                odoObj.Add(odo)
            End If
        Catch ex As Exception
            Dim odo As New OdometerData()
            odo.Plateno = ddlpleate.SelectedValue
            odo.StartDateTime = "--"
            odo.EndDateTime = "--"
            odo.StartOdometer = 0
            odo.EndOdometer = 0
            odo.Mileage = 0
            odoObj.Add(odo)
        Finally
            conn1.Close()
        End Try
        Return odoObj
    End Function

    Protected Function getTableDataLive(ByVal begindatetime As String, ByVal enddatetime As String) As List(Of OdometerData)
        Dim odoObj As List(Of OdometerData) = New List(Of OdometerData)()
        Dim dateCounter As Int32 = DateDiff(DateInterval.Day, Convert.ToDateTime(begindatetime), Convert.ToDateTime(enddatetime))
        Dim rCounter As Int32 = 0
        Dim LostOdometer As Double
        Dim LostBeginDate As DateTime
        Dim startOdo, endOdo As Double
        For x As Int32 = 0 To dateCounter
            Dim fuelobj As New FuelMath1(ddlpleate.SelectedValue)
            Dim dFuel As New RefuelBeta3(ddlpleate.SelectedValue, begindatetime, enddatetime)
            Dim dTable As New DataTable
            Dim dPrice As New DataTable
            dTable = dFuel.rTable
            dPrice = dFuel.fuelPrice()
            Dim odo As New OdometerData()
            If dFuel.fuelstartdate <> "" Then
                If x = 0 Then
                    startOdo = CDbl(dFuel.dOdometerStart).ToString("0.00")
                    odo.Plateno = ddlpleate.SelectedValue
                    odo.StartDateTime = Convert.ToDateTime(dFuel.fuelstartdate).ToString("yyyy/MM/dd HH:mm:ss")
                    odo.EndDateTime = Convert.ToDateTime(dFuel.fuelenddate).ToString("yyyy/MM/dd HH:mm:ss")
                    If odo.EndDateTime Is DBNull.Value Then
                        odo.EndDateTime = "--"
                    End If
                    odo.StartOdometer = startOdo
                    If dFuel.dOdometerEnd - dFuel.dOdometerStart > 1200 Then '2012/10/12
                        odo.StartOdometer = CDbl(dFuel.dOdometerStart1).ToString("0.00")
                    End If
                    odo.EndOdometer = startOdo + CDbl(dFuel.fuelOdometerTotal).ToString("0.00")
                    odo.Mileage = CDbl(dFuel.fuelOdometerTotal).ToString("0.00")
                    endOdo = odo.EndOdometer
                Else
                    odo.Plateno = ddlpleate.SelectedValue
                    odo.StartDateTime = Convert.ToDateTime(dFuel.fuelstartdate).ToString("yyyy/MM/dd HH:mm:ss")
                    odo.EndDateTime = Convert.ToDateTime(dFuel.fuelenddate).ToString("yyyy/MM/dd HH:mm:ss")
                    If odo.EndDateTime Is DBNull.Value Then
                        odo.EndDateTime = "--"
                    End If
                    odo.StartOdometer = startOdo
                    If dFuel.dOdometerEnd - dFuel.dOdometerStart > 1200 Then '2012/10/12
                        odo.StartOdometer = CDbl(dFuel.dOdometerStart1).ToString("0.00")
                    End If
                    odo.Mileage = CDbl(dFuel.fuelOdometerTotal).ToString("0.00")
                    odo.EndOdometer = startOdo + odo.Mileage
                    endOdo = odo.EndOdometer
                End If
            Else
                odo.Plateno = "--"
                odo.StartDateTime = "--"
                odo.EndDateTime = "--"
                odo.StartOdometer = 0
                odo.EndOdometer = 0
                odo.Mileage = 0
            End If
            odoObj.Add(odo)
            If odo.StartDateTime <> "--" Then
                LostOdometer = CDbl(odo.EndOdometer).ToString("0.00")
                LostBeginDate = Convert.ToDateTime(dFuel.fuelenddate).ToString("yyyy/MM/dd HH:mm:ss")
            End If
            startOdo = endOdo
        Next
        Return odoObj
    End Function

    Protected Sub DisplaySummary()
        'Try
        'On Error Resume Next
        Try
            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            Dim t2 As New DataTable
            Dim r As DataRow
            t2.Columns.Add(New DataColumn("No"))
            t2.Columns.Add(New DataColumn("PlateNo"))
            t2.Columns.Add(New DataColumn("BeginDateTime"))
            t2.Columns.Add(New DataColumn("EndDateTime"))
            t2.Columns.Add(New DataColumn("startOdometer"))
            t2.Columns.Add(New DataColumn("endOdometer"))
            t2.Columns.Add(New DataColumn("Travelled"))

            Dim rCounter As Int32 = 0
            Dim currentDate As DateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd 00:00:00"))
            Dim bdt As DateTime = Convert.ToDateTime(begindatetime)
            Dim edt As DateTime = Convert.ToDateTime(enddatetime)
            Dim i As Integer = 1
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim odoObj As List(Of OdometerData) = New List(Of OdometerData)()
            Dim odoObj1 As List(Of OdometerData) = New List(Of OdometerData)()
            'odoObj = getTableDataLive(begindatetime, enddatetime)

            If currentDate.ToString("yyyy/MM/dd") = bdt.ToString("yyyy/MM/dd") And currentDate.ToString("yyyy/MM/dd") = edt.ToString("yyyy/MM/dd") Then
                odoObj = getTableDataLive(begindatetime, enddatetime)
            ElseIf currentDate > edt Then
                If CInt(ddlbh.SelectedValue) = 0 And CInt(ddlbm.SelectedValue) = 0 And CInt(ddleh.SelectedValue) = 23 And CInt(ddlem.SelectedValue) = 59 Then
                    odoObj = getTableDataPost(begindatetime, enddatetime)
                Else
                    Dim datediff As Integer = (Convert.ToDateTime(txtEndDate.Value) - Convert.ToDateTime(txtBeginDate.Value)).Days
                    If CInt(ddlbh.SelectedValue) > 0 Or CInt(ddlbm.SelectedValue) > 0 Then
                        Dim btime As String = ""
                        Dim etime As String = ""
                        btime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
                        If datediff = 0 And (CInt(ddleh.SelectedValue) > 0 Or CInt(ddlem.SelectedValue) > 0) Then
                            etime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
                        Else
                            etime = txtBeginDate.Value & " 23:59:59"
                        End If

                        odoObj1 = New List(Of OdometerData)()
                        odoObj1 = getTableDataLive(btime, etime)
                        For Each item As OdometerData In odoObj1
                            Dim odo As New OdometerData()
                            odo.Plateno = ddlpleate.SelectedValue
                            odo.StartDateTime = item.StartDateTime
                            odo.EndDateTime = item.EndDateTime
                            odo.StartOdometer = item.StartOdometer
                            odo.EndOdometer = item.EndOdometer
                            odo.Mileage = item.Mileage
                            odoObj.Add(odo)
                        Next
                    Else
                        Dim btime As String = txtBeginDate.Value & " 00:00:00"
                        Dim etime As String = ""
                        odoObj1 = New List(Of OdometerData)()
                        If datediff = 0 And (ddleh.SelectedValue > 0 Or ddlem.SelectedValue > 0) Then
                            etime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
                            odoObj1 = getTableDataLive(btime, etime)
                        Else
                            etime = txtBeginDate.Value & " 23:59:59"
                            odoObj1 = getTableDataPost(btime, etime)
                        End If
                        For Each item As OdometerData In odoObj1
                            Dim odo As New OdometerData()
                            odo.Plateno = ddlpleate.SelectedValue
                            odo.StartDateTime = item.StartDateTime
                            odo.EndDateTime = item.EndDateTime
                            odo.StartOdometer = item.StartOdometer
                            odo.EndOdometer = item.EndOdometer
                            odo.Mileage = item.Mileage
                            odoObj.Add(odo)
                        Next
                    End If

                    If datediff > 1 Then
                        Dim btime As String = (Convert.ToDateTime(txtBeginDate.Value).AddDays(1)).ToString("yyyy/MM/dd 00:00:00")
                        Dim etime As String = (Convert.ToDateTime(txtEndDate.Value).AddDays(-1)).ToString("yyyy/MM/dd 23:59:59")
                        odoObj1 = New List(Of OdometerData)()
                        odoObj1 = getTableDataPost(btime, etime)
                        For Each item As OdometerData In odoObj1
                            Dim odo As New OdometerData()
                            odo.Plateno = ddlpleate.SelectedValue
                            odo.StartDateTime = item.StartDateTime
                            odo.EndDateTime = item.EndDateTime
                            odo.StartOdometer = item.StartOdometer
                            odo.EndOdometer = item.EndOdometer
                            odo.Mileage = item.Mileage
                            odoObj.Add(odo)
                        Next
                    End If

                    If datediff > 0 Then
                        If CInt(ddleh.SelectedValue) > 0 Or CInt(ddlem.SelectedValue) > 0 Then
                            Dim btime As String = txtEndDate.Value & " 00:00:00"
                            Dim etime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":00"
                            odoObj1 = New List(Of OdometerData)()
                            odoObj1 = getTableDataLive(btime, etime)
                            For Each item As OdometerData In odoObj1
                                Dim odo As New OdometerData()
                                odo.Plateno = ddlpleate.SelectedValue
                                odo.StartDateTime = item.StartDateTime
                                odo.EndDateTime = item.EndDateTime
                                odo.StartOdometer = item.StartOdometer
                                odo.EndOdometer = item.EndOdometer
                                odo.Mileage = item.Mileage
                                odoObj.Add(odo)
                            Next
                        Else
                            Dim btime As String = txtEndDate.Value & " 00:00:00"
                            Dim etime As String = txtEndDate.Value & " 23:59:59"
                            odoObj1 = New List(Of OdometerData)()
                            odoObj1 = getTableDataPost(btime, etime)
                            For Each item As OdometerData In odoObj1
                                Dim odo As New OdometerData()
                                odo.Plateno = ddlpleate.SelectedValue
                                odo.StartDateTime = item.StartDateTime
                                odo.EndDateTime = item.EndDateTime
                                odo.StartOdometer = item.StartOdometer
                                odo.EndOdometer = item.EndOdometer
                                odo.Mileage = item.Mileage
                                odoObj.Add(odo)
                            Next
                        End If
                    End If
                End If
            ElseIf edt > currentDate And currentDate > bdt Then
                begindatetime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
                Dim prevEDate As String = currentDate.AddDays(-1).ToString("yyyy/MM/dd 23:59:59")
                Dim datediff As Integer = (Convert.ToDateTime(enddatetime) - Convert.ToDateTime(begindatetime)).Days
                If CInt(ddlbh.SelectedValue) > 0 Or CInt(ddlbm.SelectedValue) > 0 Then
                    Dim btime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
                    Dim etime As String = txtBeginDate.Value & " 23:59:59"
                    odoObj1 = New List(Of OdometerData)()
                    odoObj1 = getTableDataLive(btime, etime)
                    For Each item As OdometerData In odoObj1
                        Dim odo As New OdometerData()
                        odo.Plateno = ddlpleate.SelectedValue
                        odo.StartDateTime = item.StartDateTime
                        odo.EndDateTime = item.EndDateTime
                        odo.StartOdometer = item.StartOdometer
                        odo.EndOdometer = item.EndOdometer
                        odo.Mileage = item.Mileage
                        odoObj.Add(odo)
                    Next
                Else
                    Dim btime As String = txtBeginDate.Value & " 00:00:00"
                    Dim etime As String = txtBeginDate.Value & " 23:59:59"
                    odoObj1 = New List(Of OdometerData)()
                    odoObj1 = getTableDataPost(btime, etime)
                    For Each item As OdometerData In odoObj1
                        Dim odo As New OdometerData()
                        odo.Plateno = ddlpleate.SelectedValue
                        odo.StartDateTime = item.StartDateTime
                        odo.EndDateTime = item.EndDateTime
                        odo.StartOdometer = item.StartOdometer
                        odo.EndOdometer = item.EndOdometer
                        odo.Mileage = item.Mileage
                        odoObj.Add(odo)
                    Next
                End If

                If datediff > 1 Then
                    Dim btime As String = (Convert.ToDateTime(txtBeginDate.Value).AddDays(1)).ToString("yyyy/MM/dd 00:00:00")
                    Dim etime As String = prevEDate
                    odoObj1 = New List(Of OdometerData)()
                    odoObj1 = getTableDataPost(btime, etime)
                    For Each item As OdometerData In odoObj1
                        Dim odo As New OdometerData()
                        odo.Plateno = ddlpleate.SelectedValue
                        odo.StartDateTime = item.StartDateTime
                        odo.EndDateTime = item.EndDateTime
                        odo.StartOdometer = item.StartOdometer
                        odo.EndOdometer = item.EndOdometer
                        odo.Mileage = item.Mileage
                        odoObj.Add(odo)
                    Next
                End If

                odoObj1 = New List(Of OdometerData)()
                odoObj1 = getTableDataLive(currentDate.ToString("yyyy/MM/dd HH:mm:ss"), enddatetime)
                For Each item As OdometerData In odoObj1
                    Dim odo As New OdometerData()
                    odo.Plateno = ddlpleate.SelectedValue
                    odo.StartDateTime = item.StartDateTime
                    odo.EndDateTime = item.EndDateTime
                    odo.StartOdometer = item.StartOdometer
                    odo.EndOdometer = item.EndOdometer
                    odo.Mileage = item.Mileage
                    odoObj.Add(odo)
                Next
            Else
                odoObj = getTableDataLive(begindatetime, enddatetime)
            End If




            For Each item As OdometerData In odoObj
                r = t2.NewRow
                If item.StartDateTime = "" Then
                    r(0) = i
                    r(1) = item.Plateno
                    r(2) = item.StartDateTime
                    r(3) = item.EndDateTime
                    r(4) = item.StartOdometer
                    r(5) = item.EndOdometer
                    r(6) = item.Mileage
                    t2.Rows.Add(r)
                Else
                    r(0) = i
                    r(1) = item.Plateno
                    r(2) = item.StartDateTime
                    r(3) = item.EndDateTime
                    r(4) = item.StartOdometer.ToString("0.00")
                    r(5) = item.EndOdometer.ToString("0.00")
                    r(6) = item.Mileage.ToString("0.00")
                    GrantTotalOdometer = GrantTotalOdometer + item.Mileage
                    t2.Rows.Add(r)
                    i = i + 1
                End If
            Next

            Session.Remove("odometerreportexceltable")
            Session.Remove("exceltable2")
            Session.Remove("exceltable3")

            ec = "true"

            GridView2.DataSource = t2
            GridView2.DataBind()
            Session("odometerreportexceltable") = t2
        Catch ex As Exception
            Response.Write(ex.Message & " - " & ex.StackTrace)
        End Try
    End Sub

    'Protected Sub DisplaySummary()
    '    'Try
    '    On Error Resume Next
    '    Dim begindatetime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
    '    Dim enddatetime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

    '    Dim startOdo, endOdo As Double

    '    Dim t2 As New DataTable
    '    t2.Columns.Add(New DataColumn("No"))
    '    t2.Columns.Add(New DataColumn("PlateNo"))
    '    t2.Columns.Add(New DataColumn("BeginDateTime"))
    '    t2.Columns.Add(New DataColumn("EndDateTime"))
    '    t2.Columns.Add(New DataColumn("startOdometer"))
    '    t2.Columns.Add(New DataColumn("endOdometer"))
    '    t2.Columns.Add(New DataColumn("Travelled"))

    '    Dim dateCounter As Int32 = DateDiff(DateInterval.Day, Convert.ToDateTime(begindatetime), Convert.ToDateTime(enddatetime))
    '    Dim rCounter As Int32 = 0
    '    Dim LostOdometer As Double
    '    Dim LostBeginDate As DateTime
    '    For x As Int32 = 0 To dateCounter
    '        Dim fuelobj As New FuelMath1(ddlpleate.SelectedValue)
    '        Dim dFuel As New RefuelBeta3(ddlpleate.SelectedValue, Convert.ToDateTime(begindatetime).AddDays(x).ToString("yyyy/MM/dd 00:00:00"), Convert.ToDateTime(begindatetime).AddDays(x).ToString("yyyy/MM/dd 23:59:59"))
    '        Dim dTable As New DataTable
    '        Dim dPrice As New DataTable

    '        Dim r As DataRow
    '        dTable = dFuel.rTable
    '        dPrice = dFuel.fuelPrice()

    '        r = t2.NewRow
    '        If dFuel.fuelstartdate <> "" Then
    '            If x = 0 Then
    '                startOdo = CDbl(dFuel.dOdometerStart).ToString("0.00")
    '                r(0) = rCounter + 1
    '                r(1) = ddlpleate.SelectedValue
    '                r(2) = dFuel.fuelstartdate
    '                r(3) = dFuel.fuelenddate
    '                If r(3) Is DBNull.Value Then
    '                    r(3) = "--"
    '                End If
    '                r(4) = startOdo
    '                If dFuel.dOdometerEnd - dFuel.dOdometerStart > 1200 Then '2012/10/12
    '                    r(4) = CDbl(dFuel.dOdometerStart1).ToString("0.00")
    '                End If
    '                r(5) = startOdo + CDbl(dFuel.fuelOdometerTotal).ToString("0.00")
    '                r(6) = CDbl(dFuel.fuelOdometerTotal).ToString("0.00")

    '                endOdo = r(5)
    '            Else

    '                r(0) = rCounter + 1
    '                r(1) = ddlpleate.SelectedValue
    '                r(2) = dFuel.fuelstartdate
    '                r(3) = dFuel.fuelenddate
    '                If r(3) Is DBNull.Value Then
    '                    r(3) = "--"
    '                End If

    '                r(4) = startOdo


    '                '  r(4) = CDbl(dFuel.dOdometerStart).ToString("0.00")
    '                If dFuel.dOdometerEnd - dFuel.dOdometerStart > 1200 Then '2012/10/12
    '                    r(4) = CDbl(dFuel.dOdometerStart1).ToString("0.00")
    '                End If
    '                ' r(5) = CDbl(CDbl(dFuel.fuelOdometerTotal) + CDbl(dFuel.dOdometerStart)).ToString("0.00") '

    '                'If dFuel.fuelOdometerTotal >= 0 Then
    '                r(6) = CDbl(dFuel.fuelOdometerTotal).ToString("0.00")
    '                r(5) = startOdo + r(6)
    '                endOdo = r(5)
    '                'GrantTotalOdometer = GrantTotalOdometer + dFuel.fuelOdometerTotal
    '                'Else
    '                '    r(6) = "--"
    '                'End If

    '                'If x > 0 Then
    '                '    If t2.Rows(x - 1)(2) <> "--" Then
    '                '        LostOdometer = CDbl(r(5)).ToString("0.00")
    '                '        LostBeginDate = dFuel.fuelenddate
    '                '        Response.Write(LostOdometer)
    '                '    End If
    '                'End If
    '            End If
    '        Else
    '            r(0) = "--"
    '            r(1) = "--"
    '            r(2) = "--"
    '            r(3) = "--"
    '            r(4) = "--"
    '            r(5) = "--"
    '            r(6) = "--"
    '        End If

    '        t2.Rows.Add(r)

    '        'If x > 0 Then
    '        '    If t2.Rows(x - 1)(5) <> "--" And r(5) <> "--" And r(6) <> "--" Then
    '        '        t2.Rows(x - 1)(5) = CDbl(dFuel.dOdometerStart).ToString("0.00")
    '        '    End If
    '        'End If





    '        'If x > 0 Then
    '        '    If t2.Rows(x - 1)(5) <> "--" And r(5) <> "--" And r(6) <> "--" Then
    '        '        If t2.Rows(x - 1)(6) <> "--" Then
    '        '            t2.Rows(x - 1)(5) = CDbl(dFuel.dOdometerStart).ToString("0.00")
    '        '            If CDbl(CDbl(dFuel.dOdometerStart).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") < -50 Then
    '        '                If CDbl(CDbl(dFuel.dOdometerStart1).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") > 0 Then
    '        '                    t2.Rows(x - 1)(5) = CDbl(dFuel.dOdometerStart1).ToString("0.00")
    '        '                    t2.Rows(x - 1)(6) = CDbl(CDbl(dFuel.dOdometerStart1).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") '& "y"
    '        '                ElseIf CDbl(CDbl(dFuel.dOdometerStart2).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") > 0 Then
    '        '                    t2.Rows(x - 1)(5) = CDbl(dFuel.dOdometerStart2).ToString("0.00")
    '        '                    t2.Rows(x - 1)(6) = CDbl(CDbl(dFuel.dOdometerStart2).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") '& "y"
    '        '                Else
    '        '                    't2.Rows(x - 1)(5) = CDbl(dFuel.dOdometerStart).ToString("0.00") & "tt"
    '        '                End If
    '        '            ElseIf CDbl(CDbl(dFuel.dOdometerStart).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") > 1200 Then
    '        '                t2.Rows(x - 1)(5) = CDbl(dFuel.dOdometerStart1).ToString("0.00")
    '        '                t2.Rows(x - 1)(6) = CDbl(CDbl(dFuel.dOdometerStart1).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") '& "z"
    '        '            ElseIf CDbl(CDbl(dFuel.dOdometerStart).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") < 0 And CDbl(CDbl(dFuel.dOdometerStart).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") > -50 Then
    '        '                t2.Rows(x - 1)(6) = "0.00" '& "x"
    '        '            Else
    '        '                t2.Rows(x - 1)(6) = CDbl(CDbl(dFuel.dOdometerStart).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") '& "a"
    '        '            End If
    '        '        Else
    '        '            If CDbl(CDbl(dFuel.dOdometerStart).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") < -50 Then
    '        '                t2.Rows(x - 1)(6) = "0.00" '& "error 2"
    '        '            Else
    '        '                t2.Rows(x - 1)(5) = CDbl(dFuel.dOdometerStart).ToString("0.00")
    '        '                t2.Rows(x - 1)(6) = CDbl(CDbl(dFuel.dOdometerStart).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") '& "b"
    '        '            End If
    '        '        End If
    '        '    End If
    '        '    If r(5) = "--" And r(6) = "--" And t2.Rows(x - 1)(5) <> "--" And t2.Rows(x - 1)(6) <> "--" Then
    '        '        If LostOdometer <> 0 Then
    '        '            t2.Rows(x - 1)(5) = CDbl(LostOdometer).ToString("0.00")
    '        '            If CDbl(CDbl(LostOdometer).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") < -50 Then
    '        '                t2.Rows(x - 1)(6) = "0.00" '& "error 3"
    '        '            Else
    '        '                t2.Rows(x - 1)(6) = CDbl(CDbl(LostOdometer).ToString("0.00") - CDbl(t2.Rows(x - 1)(4)).ToString("0.00")).ToString("0.00") '& "c"
    '        '            End If
    '        '        End If


    '        '        'GrantTotalOdometer = GrantTotalOdometer + (CDbl(t2.Rows(x - 1)(6)).ToString("0.00"))
    '        '    End If
    '        '    If t2.Rows(x)(5) <> "--" And t2.Rows(x - 1)(5) = "--" And LostOdometer > 0 Then
    '        '        t2.Rows(x - 1)(1) = ddlpleate.SelectedValue
    '        '        t2.Rows(x - 1)(2) = LostBeginDate.AddDays(1).ToString("yyyy/MM/dd 00:00:00")
    '        '        t2.Rows(x - 1)(3) = Convert.ToDateTime(dFuel.fuelstartdate).AddDays(-1).ToString("yyyy/MM/dd 23:59:59")
    '        '        t2.Rows(x - 1)(4) = CDbl(LostOdometer).ToString("0.00")
    '        '        t2.Rows(x - 1)(5) = CDbl(dFuel.dOdometerStart).ToString("0.00")
    '        '        If CDbl(dFuel.dOdometerStart - LostOdometer) < -50 Then
    '        '            t2.Rows(x - 1)(6) = CDbl(dFuel.dOdometerStart).ToString("0.00") '& "d"
    '        '        ElseIf CDbl(dFuel.dOdometerStart - LostOdometer) < 0 And CDbl(dFuel.dOdometerStart - LostOdometer) > -50 Then
    '        '            t2.Rows(x - 1)(6) = "0.00" '& "h"
    '        '        Else
    '        '            t2.Rows(x - 1)(6) = CDbl(dFuel.dOdometerStart - LostOdometer).ToString("0.00") '& "e"
    '        '            'If t2.Rows(x - 1)(2) = "0001/01/02 00:00:00" Then
    '        '            '    t2.Rows(x - 1)(2) = Convert.ToDateTime(dFuel.fuelstartdate).AddDays(-1).ToString("yyyy/MM/dd 00:00:00")
    '        '            'End If
    '        '        End If
    '        '        't2.Rows(x - 1)(6) = CDbl(dFuel.dOdometerStart - LostOdometer).ToString("0.00")
    '        '        'GrantTotalOdometer = GrantTotalOdometer + (CDbl(dFuel.dOdometerStart - LostOdometer).ToString("0.00"))
    '        '    End If
    '        'End If

    '        If r(2) <> "--" Then
    '            LostOdometer = CDbl(r(5)).ToString("0.00")
    '            LostBeginDate = dFuel.fuelenddate
    '        End If


    '        rCounter = rCounter + 1
    '        startOdo = endOdo
    '    Next

    '    For tableCount As Int32 = 0 To t2.Rows.Count - 1
    '        If t2.Rows(tableCount)(6) <> "--" Then
    '            GrantTotalOdometer = GrantTotalOdometer + t2.Rows(tableCount)(6)
    '        End If
    '    Next

    '    Dim t3 As New DataTable
    '    t3.Columns.Add(New DataColumn("No"))
    '    t3.Columns.Add(New DataColumn("PlateNo"))
    '    t3.Columns.Add(New DataColumn("BeginDateTime"))
    '    t3.Columns.Add(New DataColumn("EndDateTime"))
    '    t3.Columns.Add(New DataColumn("startOdometer"))
    '    t3.Columns.Add(New DataColumn("endOdometer"))
    '    t3.Columns.Add(New DataColumn("Travelled"))

    '    Dim se As Int32 = 0

    '    For tCount As Int32 = 0 To t2.Rows.Count - 1
    '        If Not (t2.Rows(tCount)(1) = "--" And t2.Rows(tCount)(2) = "--" And t2.Rows(tCount)(3) = "--" And t2.Rows(tCount)(4) = "--" And t2.Rows(tCount)(5) = "--" And t2.Rows(tCount)(6) = "--") Then
    '            Dim r3 As DataRow
    '            r3 = t3.NewRow
    '            se = se + 1
    '            r3(0) = se
    '            r3(1) = t2.Rows(tCount)(1)
    '            r3(2) = t2.Rows(tCount)(2)
    '            r3(3) = t2.Rows(tCount)(3)
    '            r3(4) = t2.Rows(tCount)(4)
    '            r3(5) = t2.Rows(tCount)(5)
    '            r3(6) = t2.Rows(tCount)(6)
    '            t3.Rows.Add(r3)
    '        End If
    '    Next

    '    Session.Remove("exceltable")
    '    Session.Remove("exceltable2")
    '    Session.Remove("exceltable3")

    '    ec = "true"

    '    Session("exceltable") = t3
    '    GridView2.DataSource = t3
    '    GridView2.DataBind()

    '    'Catch ex As Exception
    '    '    Response.Write(ex.Message)
    '    'End Try

    'End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) ' Handles GridView2.RowDataBound
        If e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(5).Text = "TOTAL ODOMETER"
            e.Row.Cells(6).Text = CDbl(GrantTotalOdometer).ToString("0.00") & " KM"
        End If
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        getPlateNo(ddlUsername.SelectedValue)
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplaySummary()
    End Sub

    Public Structure OdometerData
        Public Plateno As String
        Public StartDateTime As String
        Public EndDateTime As String
        Public StartOdometer As Double
        Public EndOdometer As Double
        Public Mileage As Double
    End Structure
End Class
