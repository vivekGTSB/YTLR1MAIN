Imports System.Data.SqlClient
Imports System.Data
Imports System.Globalization
Imports System.Text

Partial Class AllVehiclesSummaryReportAClass
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Public test As StringBuilder = New StringBuilder()

    Dim sCon As String = System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")
    Dim suspectTime As String
    Dim GrantOdometer, GrantFuel, GrantPrice, GrandIdlingFuel, GrandIdlingPrice, GrantRefuelLitre, GrantRefuelPrice, TotOspeed, GrandIdleLkm, GrandIdleRmKm As Double
    Dim GrandIdlingTime, GrandTravellingTime, GrandStopTime As TimeSpan
    Public sb As New StringBuilder()
    Public sb1 As New StringBuilder()

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            Else
                hiduserid.Value = Request.Cookies("userinfo")("userid")
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Label2.Visible = False
            Label3.Visible = False
            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
            ImageButton1.Text = Literal22.Text
            ImageButton1.ToolTip = Literal22.Text
            rdcumulative.Text = Literal42.Text
            rddetailed.Text = Literal43.Text
            sb1.Length = 0
            sb1.Append(Literal1.Text)
            Dim userid As String = Request.Cookies("userinfo")("userid")
            If Page.IsPostBack = False Then
                sb.Length = 0
                '  sb.Append("<thead><tr align=""left""><th >SNo</th><th>User Name</th><th>Group Name</th><th>Plate No</th><th style='width:95px;'>Mileage (KM)</th><th>Usage (L)</th><th>Cost (RM)</th><th >Litre/KM</th><th>KM/Litre</th><th>RM/KM</th><th>Refuel (L)</th><th>Refuel (RM)</th><th>Idling Time</th><th>Idling (L)</th><th>Idling (RM)</th><th>Travelling Time</th><th>Stop Time</th></tr></thead>")
                sb.Append("<thead><tr align=""left""><th > " & Literal6.Text & "</th><th>" & Literal7.Text & "</th><th>" & Literal8.Text & "</th><th>" & Literal4.Text & "</th><th style='width:95px;'>" & Literal9.Text & " (KM)</th><th>" & Literal10.Text & " (L)</th><th>" & Literal11.Text & " (RM)</th><th >" & Literal12.Text & "</th><th>" & Literal13.Text & "</th><th>RM/KM</th><th>" & Literal14.Text & " (L)</th><th> " & Literal14.Text & " (RM)</th><th>" & Literal15.Text & "</th><th>" & Literal16.Text & " (L)</th><th>Idling (Ltr/KM)</th><th>" & Literal16.Text & " (RM)</th><th>Idling (RM/KM)</th><th>" & Literal17.Text & "</th><th>" & Literal18.Text & "</th>")
                If userid = "1140" Then
                    sb.Append("<th >OV Speed Cnt</th>")
                End If
                sb.Append("</tr></thead>")
                sb.Append("<tbody>")
                sb.Append("<tr>")
                For i As Integer = 0 To 18
                    sb.Append("<td>" & "--" & "</td>")
                Next
                If userid = "1140" Then
                    sb.Append("<td>--</td>")
                End If
                sb.Append("</tr>")
                sb.Append("</tbody>")
                'sb.Append("<tfoot><tr align=""left""><th >SNo</th><th>User Name</th><th>Group Name</th><th>Plate No</th><th style='width:95px;'>Mileage (KM)</th><th>Usage (L)</th><th>Cost (RM)</th><th >Litre/KM</th><th>KM/Litre</th><th>RM/KM</th><th>Refuel (L)</th><th>Refuel (RM)</th><th>Idling Time</th><th>Idling (L)</th><th>Idling (RM)</th><th>Travelling Time</th><th>Stop Time</th></tr></tfoot>")
                sb.Append("<tfoot><tr align=""left""><th > " & Literal6.Text & "</th><th>" & Literal7.Text & "</th><th>" & Literal8.Text & "</th><th>" & Literal4.Text & "</th><th style='width:95px;'>" & Literal9.Text & " (KM)</th><th>" & Literal10.Text & " (L)</th><th>" & Literal11.Text & " (RM)</th><th >" & Literal12.Text & "</th><th>" & Literal13.Text & "</th><th>RM/KM</th><th>" & Literal14.Text & " (L)</th><th> " & Literal14.Text & " (RM)</th><th>" & Literal15.Text & "</th><th>" & Literal16.Text & " (L)</th><th>Idling (Ltr/KM)</th><th>" & Literal16.Text & " (RM)</th><th>Idling (RM/KM)</th><th>" & Literal17.Text & "</th><th>" & Literal18.Text & "</th>")
                If userid = "1140" Then
                    sb.Append("<th >OV Speed Cnt</th>")
                End If
                sb.Append("</tr></tfoot>")
                txtBeginDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
                populateNode()
                If Request.Cookies("userinfo")("role") = "User" Then
                    tvPlateno.ExpandAll()
                End If
            End If
            If Request.Form("txtBeginDate") <> Nothing Then
                txtBeginDate.Value = Request.Form("txtBeginDate")
                txtEndDate.Value = Request.Form("txtEndDate")
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Sub populateNode()
        Try
            Dim ds As System.Data.DataSet = getTreeViewData()
            For Each masterRow As DataRow In ds.Tables("user").Rows
                Dim masterNode As New TreeNode(masterRow("username").ToString().ToUpper())
                tvPlateno.Nodes.Add(masterNode)
                For Each childRow As DataRow In masterRow.GetChildRows("Children")
                    Dim childNode As New TreeNode(childRow("plateno").ToString().ToUpper(), childRow("groupname").ToString())
                    masterNode.ChildNodes.Add(childNode)
                    If Request.Cookies("userinfo")("role") = "User" Then
                        masterNode.Checked = True
                        childNode.Checked = True
                    End If
                Next
            Next
        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Sub

    Function getTreeViewData() As System.Data.DataSet
        Try
            Dim conn As New SqlConnection(sCon)
            Dim daPlateno As SqlDataAdapter
            Dim daUser As SqlDataAdapter

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim ds As System.Data.DataSet = New System.Data.DataSet()

            If role = "Admin" Or role = "AdminViewer1" Then
                Dim dsRoute As DataSet = New DataSet()
                daUser = New SqlDataAdapter("select userid,username,dbip from userTBL where role='user' order by username", conn)
                daUser.Fill(dsRoute, "user")
                For x As Int32 = 0 To dsRoute.Tables("user").Rows.Count - 1
                    Dim uid As String = dsRoute.Tables("user").Rows(x)("userid").ToString()

                    Dim connRoute As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim daRoute As SqlDataAdapter = New SqlDataAdapter("select * from vehicleTBL where userid='" & uid & "' order by plateno", connRoute)
                    daRoute.Fill(dsRoute, "vehicle")
                Next
                dsRoute.Relations.Add("Children", dsRoute.Tables("user").Columns("userid"), dsRoute.Tables("vehicle").Columns("userid"))
                Return dsRoute
            ElseIf role = "SuperUser" Or role = "Operator" Then

                Dim connSuperUser As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                daPlateno = New SqlDataAdapter("select * from vehicleTBL where userid in(" & userslist & ") order by plateno", connSuperUser)
                daUser = New SqlDataAdapter("select * from userTBL where userid in (" & userslist & ") order by username", connSuperUser)
            Else

                Dim connUser As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                daPlateno = New SqlDataAdapter("select * from vehicleTBL where userid='" & userid & "' order by plateno", connUser)
                daUser = New SqlDataAdapter("select * from userTBL where userid='" & userid & "' order by username", connUser)
            End If

            daPlateno.Fill(ds, "vehicle")
            daUser.Fill(ds, "user")
            ds.Relations.Add("Children", ds.Tables("user").Columns("userid"), ds.Tables("vehicle").Columns("userid"))
            Return ds
        Catch ex As SystemException
            Response.Write("user" & ex.Message)
        End Try
    End Function



    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        ' DisplayFuelInformation()
        DisplayInformation()
    End Sub
    Function getTravellingMinutes(ByVal TravellingPlateNo As String, ByVal Travellingdate1 As String, ByVal Travellingdate2 As String)
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim da As SqlDataAdapter = New SqlDataAdapter("select distinct convert(varchar(19),timestamp,120) as datetime,lat,lon,speed,ignition from vehicle_history2 " &
        "where plateno ='" & TravellingPlateNo & "' and (gps_av='A' or (gps_av='V' and ignition='0')) and timestamp between '" & Convert.ToDateTime(Travellingdate1).ToString("yyyy/MM/dd HH:mm:ss") & "' and '" & Convert.ToDateTime(Travellingdate2).ToString("yyyy/MM/dd HH:mm:ss") & "' order by datetime", conn)

        Dim dsIdling As DataSet = New DataSet()
        da.Fill(dsIdling)

        Dim prevstatus As String = "Traveling"
        Dim presentstatus As String = "Traveling"

        Dim tempprevtime As DateTime
        Dim prevTime As DateTime
        Dim presenttime As DateTime
        Dim totaltravellingtime As TimeSpan

        If dsIdling.Tables(0).Rows.Count > 0 Then
            prevTime = dsIdling.Tables(0).Rows(0)("datetime")

            If (dsIdling.Tables(0).Rows(0)("ignition") = True) And (dsIdling.Tables(0).Rows(0)("speed") = 0) Then ' And dsIdling.Tables(0).Rows(0)("gps_av") = "A" Then
                prevstatus = "Ideling"
            ElseIf (dsIdling.Tables(0).Rows(0)("ignition") = False) Then
                prevstatus = "Stop"
            Else
                prevstatus = "Traveling"
            End If
        End If

        For i As Int32 = 1 To dsIdling.Tables(0).Rows.Count - 1

            presenttime = dsIdling.Tables(0).Rows(i)("datetime")

            If (dsIdling.Tables(0).Rows(i)("ignition") = True) And (dsIdling.Tables(0).Rows(i)("speed") = 0) Then ' And dsIdling.Tables(0).Rows(i)("gps_av") = "A" Then
                presentstatus = "Ideling"
            ElseIf (dsIdling.Tables(0).Rows(i)("ignition") = False) Then
                presentstatus = "Stop"
            Else
                presentstatus = "Traveling"
            End If
            tempprevtime = presenttime
            If prevstatus <> presentstatus Then
                If prevTime <> tempprevtime Then
                    Dim tfirstdatetime As DateTime = prevTime
                    Dim tseconddatetime As DateTime = tempprevtime 'presenttime
                    Dim temptime As TimeSpan = tseconddatetime - tfirstdatetime

                    If temptime.Minutes > 0 Then
                        If prevstatus = "Traveling" Then
                            totaltravellingtime += temptime
                        End If
                    End If
                End If
                prevTime = presenttime
                prevstatus = presentstatus
            End If


        Next

        If prevTime <> presenttime Then
            Dim tfirstdatetime As DateTime = prevTime
            Dim tseconddatetime As DateTime = presenttime
            Dim temptime As TimeSpan = tseconddatetime - tfirstdatetime
            If prevstatus = "Stop" Then

            ElseIf prevstatus = "Traveling" Then
                totaltravellingtime += temptime
            End If
        End If

        Return totaltravellingtime
    End Function

    Protected Sub DisplayInformation()
        Try
            Dim totalIdling As String
            Dim totalTravelling As String
            Dim totalStop As String
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim dr As SqlDataReader
            Dim opr As Int16
            If rdcumulative.Checked = True Then
                opr = 1
            ElseIf rddetailed.Checked = True Then
                opr = 0

            End If
            Dim t2 As New DataTable
            t2.Columns.Add(New DataColumn("S No"))

            If opr = 0 Then
                t2.Columns.Add(New DataColumn("timestamp"))
            End If
            t2.Columns.Add(New DataColumn("User Name1"))
            t2.Columns.Add(New DataColumn("GroupName"))
            t2.Columns.Add(New DataColumn("PlateNo"))
            t2.Columns.Add(New DataColumn("Mileage1"))
            t2.Columns.Add(New DataColumn("Fuel1"))
            t2.Columns.Add(New DataColumn("FuelCost"))
            t2.Columns.Add(New DataColumn("Liter/KM1"))
            t2.Columns.Add(New DataColumn("KM/Liter1"))
            t2.Columns.Add(New DataColumn("Cost/liter1"))
            t2.Columns.Add(New DataColumn("Refuel1"))
            t2.Columns.Add(New DataColumn("Cost1"))
            t2.Columns.Add(New DataColumn("IdlingTime"))
            t2.Columns.Add(New DataColumn("IdlingFuel"))
            t2.Columns.Add(New DataColumn("IdlingFperM"))
            t2.Columns.Add(New DataColumn("TotalIdlingCost"))
            t2.Columns.Add(New DataColumn("IdlingCperM"))

            t2.Columns.Add(New DataColumn("TravellingTime"))
            t2.Columns.Add(New DataColumn("StopTime"))
            t2.Columns.Add(New DataColumn("Over Speed Count"))

            Dim checkedNodes As TreeNodeCollection = tvPlateno.CheckedNodes
            Dim dPrice As DataTable = fuelPrice(userid)
            Dim dieselPrice As Double = 2.2
            Dim drPrice As DataRow() = dPrice.Select("StartDate <= #" & Convert.ToDateTime(txtBeginDate.Value).ToString("yyyy/MM/dd HH:mm:dd") & "# or EndDate >= #" & Convert.ToDateTime(txtEndDate.Value).ToString("yyyy/MM/dd HH:mm:dd") & "#")

            Try
                For Each row As DataRow In drPrice
                    If (Convert.ToDateTime(row(0)) <= Convert.ToDateTime(txtBeginDate.Value)) And (Convert.ToDateTime(row(1)) >= Convert.ToDateTime(txtEndDate.Value)) Then
                        dieselPrice = CDbl(row(2))
                        Exit For
                    End If
                Next
            Catch ex As Exception
            End Try


            Dim refuelCount As Int32 = 0
            Dim consumptionCount As Int32 = 0
            Dim idlingCount As Int32 = 0
            Dim eventCount As Int32 = 0
            Dim r As DataRow

            Dim begindatetime As String = ""
            Dim enddatetime As String = ""
            begindatetime = txtBeginDate.Value & " 00:00:00"
            Dim iSpan As TimeSpan
            If Convert.ToDateTime(txtEndDate.Value).ToString("yyyy/MM/dd") = Now.ToString("yyyy/MM/dd") Then
                enddatetime = Now.ToString("yyyy/MM/dd HH:mm:ss")
            Else
                enddatetime = txtEndDate.Value & " 23:59:59"
            End If
            sb = New StringBuilder()
            sb1.Length = 0            ' sb1.Append("Report generated from " & txtBeginDate.Value & " to " & txtEndDate.Value)
            sb1.Append(Literal5.Text & Literal2.Text & txtBeginDate.Value & Literal3.Text & txtEndDate.Value)
            Dim grandidilemin As Int32
            Dim grandtravelmin As Int32
            Dim grandstopmin As Int32
            conn.Open()


            For x As Int16 = 0 To checkedNodes.Count - 1
                If checkedNodes.Item(x).Checked = True Then
                    For y As Int16 = 0 To checkedNodes.Item(x).ChildNodes.Count - 1
                        Dim plateno As String = checkedNodes.Item(x).ChildNodes.Item(y).Text
                        cmd = New SqlCommand()
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_getFuelSummaryReport_test"
                        cmd.Parameters.AddWithValue("@fromdate", txtBeginDate.Value)
                        cmd.Parameters.AddWithValue("@todate", txtEndDate.Value)
                        cmd.Parameters.AddWithValue("@plateno", plateno)
                        cmd.Parameters.AddWithValue("@opr", opr)
                        cmd.Connection = conn

                        dr = cmd.ExecuteReader()
                        While dr.Read()

                            r = t2.NewRow
                            If opr = 0 Then
                                r(0) = consumptionCount
                                r(1) = Convert.ToDateTime(dr("timestamp")).ToString("dd/MM/yyyy")
                                r(2) = dr("username").ToString().ToUpper()
                                r(3) = dr("Groupname").ToString().ToUpper()
                                r(4) = dr("plateno")
                                r(5) = CDbl(dr("mileage")).ToString("0.00")
                                If r(5) >= 10 Then
                                    r(6) = CDbl(dr("usagefuel")).ToString("0.00")
                                    r(7) = CDbl(dr("usagefuelcost")).ToString("0.00")
                                    r(8) = CDbl(dr("ltrperkm")).ToString("0.00")
                                    r(9) = CDbl(dr("kmperltr")).ToString("0.00")
                                    r(10) = CDbl(dr("rmperkm")).ToString("0.00")
                                Else
                                    r(6) = 0
                                    r(7) = 0
                                    r(8) = 0
                                    r(9) = 0
                                    r(10) = 0
                                End If

                                r(11) = CDbl(dr("refuel")).ToString("0.00")
                                r(12) = CDbl(dr("refuelcost")).ToString("0.00")
                                iSpan = TimeSpan.FromMinutes(Math.Round(Convert.ToDouble(dr("idlingtime"))))
                                Dim totalidlingtime As String = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
                                iSpan = TimeSpan.FromMinutes(dr("trvelingtime"))
                                Dim totaltraveltime As String = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
                                iSpan = TimeSpan.FromMinutes(dr("stoptime"))
                                Dim totalstoptime As String = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
                                r(13) = totalidlingtime
                                r(14) = CDbl(dr("idelingfuel")).ToString("0.00")
                                If r(5) >= 10 Then
                                    r(15) = CDbl(CDbl(dr("idelingfuel")) / CDbl(dr("mileage"))).ToString("0.00")
                                Else
                                    r(15) = 0
                                End If
                                ' r(16) = CDbl(CDbl(dr("idelingfuel")) * dieselPrice).ToString("0.00")
                                r(16) = CDbl(CDbl(dr("idlingcost"))).ToString("0.00")
                                If r(5) >= 10 Then
                                    'r(17) = CDbl(CDbl(CDbl(dr("idlingcost")) * dieselPrice) / CDbl(dr("mileage"))).ToString("0.00")
                                    r(17) = CDbl(CDbl(CDbl(dr("idlingcost"))) / CDbl(dr("mileage"))).ToString("0.00")
                                Else
                                    r(17) = 0
                                End If
                                r(18) = totaltraveltime
                                r(19) = totalstoptime
                                r(20) = dr("ospeedcnt")

                                If dr("mileage") >= 10 Then
                                    GrantFuel = GrantFuel + CDbl(dr("usagefuel"))
                                    GrantPrice = GrantPrice + CDbl(dr("usagefuelcost"))
                                End If
                            Else
                                r(0) = consumptionCount
                                r(1) = dr("username").ToString().ToUpper()
                                r(2) = dr("Groupname").ToString().ToUpper()
                                r(3) = dr("plateno")
                                r(4) = CDbl(dr("mileage")).ToString("0.00")
                                If r(4) >= 50 Then
                                    r(5) = CDbl(dr("usagefuel")).ToString("0.00")
                                    r(6) = CDbl(dr("usagefuelcost")).ToString("0.00")
                                    r(7) = CDbl(dr("ltrperkm")).ToString("0.00")
                                    r(8) = CDbl(dr("kmperltr")).ToString("0.00")
                                    r(9) = CDbl(dr("rmperkm")).ToString("0.00")
                                Else
                                    r(5) = 0
                                    r(6) = 0
                                    r(7) = 0
                                    r(8) = 0
                                    r(9) = 0
                                End If

                                r(10) = CDbl(dr("refuel")).ToString("0.00")
                                r(11) = CDbl(dr("refuelcost")).ToString("0.00")
                                iSpan = TimeSpan.FromMinutes(Math.Round(Convert.ToDouble(dr("idlingtime"))))
                                Dim totalidlingtime As String
                                If iSpan.Days > 0 Then
                                    totalidlingtime = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"

                                Else
                                    totalidlingtime = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"

                                End If

                                iSpan = TimeSpan.FromMinutes(dr("trvelingtime"))
                                Dim totaltraveltime As String
                                If iSpan.Days > 0 Then
                                    totaltraveltime = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"

                                Else
                                    totaltraveltime = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"

                                End If

                                iSpan = TimeSpan.FromMinutes(dr("stoptime"))
                                Dim totalstoptime As String
                                If iSpan.Days > 0 Then
                                    totalstoptime = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"

                                Else
                                    totalstoptime = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"

                                End If

                                r(12) = totalidlingtime
                                r(13) = CDbl(dr("idelingfuel")).ToString("0.00")
                                If r(4) >= 10 Then
                                    r(14) = CDbl(CDbl(dr("idelingfuel")) / CDbl(dr("mileage"))).ToString("0.00")
                                Else
                                    r(14) = 0
                                End If

                                'r(15) = CDbl(CDbl(dr("idelingfuel")) * dieselPrice).ToString("0.00")
                                r(15) = CDbl(CDbl(dr("idlingcost"))).ToString("0.00")
                                If r(4) >= 10 Then
                                    'r(16) = CDbl(CDbl(CDbl(dr("idelingfuel")) * dieselPrice) / CDbl(dr("mileage"))).ToString("0.00")
                                    r(16) = CDbl(CDbl(CDbl(dr("idlingcost"))) / CDbl(dr("mileage"))).ToString("0.00")
                                Else
                                    r(16) = 0
                                End If
                                r(17) = totaltraveltime
                                r(18) = totalstoptime
                                r(19) = dr("ospeedcnt")

                                If dr("mileage") >= 50 Then
                                    GrantFuel = GrantFuel + CDbl(dr("usagefuel"))
                                    GrantPrice = GrantPrice + CDbl(dr("usagefuelcost"))
                                End If
                            End If



                            t2.Rows.Add(r)
                            GrantOdometer = GrantOdometer + CDbl(dr("mileage"))


                            TotOspeed = TotOspeed + Convert.ToInt16(dr("ospeedcnt"))
                            GrantRefuelLitre = GrantRefuelLitre + CDbl(dr("refuel"))
                            GrantRefuelPrice = GrantRefuelPrice + CDbl(dr("refuelcost"))
                            GrandIdlingFuel = GrandIdlingFuel + CDbl(dr("idelingfuel"))
                            GrandIdlingPrice = GrandIdlingPrice + CDbl(dr("idlingcost"))
                            grandidilemin = grandidilemin + dr("idlingtime")
                            grandtravelmin = grandtravelmin + dr("trvelingtime")
                            grandstopmin = grandstopmin + dr("stoptime")
                            consumptionCount += 1
                        End While

                        dr.Close()
                    Next
                End If

            Next
            iSpan = TimeSpan.FromMinutes(Math.Round(grandidilemin))
            Dim GrandIdlingTime As String
            If iSpan.Days > 0 Then
                GrandIdlingTime = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " &
               iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
            Else
                GrandIdlingTime = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " &
                iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
            End If
            iSpan = TimeSpan.FromMinutes(grandtravelmin)
            Dim GrandTravellingTime As String
            If iSpan.Days > 0 Then
                GrandTravellingTime = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " &
               iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
            Else
                GrandTravellingTime = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " &
                iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
            End If
            iSpan = TimeSpan.FromMinutes(grandstopmin)
            Dim GrandStopTime As String
            If iSpan.Days > 0 Then
                GrandStopTime = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " &
               iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
            Else
                GrandStopTime = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " &
                iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
            End If

            If t2.Rows.Count = 0 Then
                r = t2.NewRow
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
                r(18) = "--"
                r(19) = "--"
                If opr = 0 Then
                    r(20) = "--"
                End If
                t2.Rows.Add(r)
            End If

            Dim excelTable As DataTable = t2.Copy()

            sb.Append("<thead><tr align=""left""><th >" & Literal6.Text & "</th>")
            If opr = 0 Then
                sb.Append("<th >" & Literal41.Text & "</th>")
            End If
            sb.Append("<th>" & Literal7.Text & "</th><th>" & Literal8.Text & "</th><th>" & Literal4.Text & "</th><th style='width:95px;'>" & Literal9.Text & "(KM)</th><th>" & Literal10.Text & " (L)</th><th>" & Literal11.Text & " (RM)</th><th >" & Literal12.Text & "</th><th>" & Literal13.Text & "</th><th>RM/KM</th><th>" & Literal14.Text & " (L)</th><th> " & Literal14.Text & " (RM)</th><th>" & Literal15.Text & "</th><th>" & Literal16.Text & " (L)</th><th>Idling(Ltr/KM)</th><th>" & Literal16.Text & " (RM)</th><th>Idling(RM/KM)</th><th>" & Literal17.Text & "</th><th>" & Literal18.Text & "</th>")
            If userid = "1140" Then
                sb.Append("<th >OV Speed Cnt</th>")
            End If
            sb.Append("</tr></thead>")



            If opr = 0 Then

                excelTable.Columns(0).ColumnName = Literal6.Text
                excelTable.Columns(1).ColumnName = Literal41.Text
                excelTable.Columns(2).ColumnName = Literal7.Text
                excelTable.Columns(3).ColumnName = Literal8.Text
                excelTable.Columns(4).ColumnName = Literal4.Text
                excelTable.Columns(5).ColumnName = Literal9.Text & "(KM)"
                excelTable.Columns(6).ColumnName = Literal10.Text & " (L)"
                excelTable.Columns(7).ColumnName = Literal11.Text & "(RM)"
                excelTable.Columns(8).ColumnName = Literal12.Text
                excelTable.Columns(9).ColumnName = Literal13.Text
                excelTable.Columns(10).ColumnName = "RM/KM"
                excelTable.Columns(11).ColumnName = Literal14.Text & "  (L)"
                excelTable.Columns(12).ColumnName = Literal14.Text & " (RM)"
                excelTable.Columns(13).ColumnName = Literal15.Text
                excelTable.Columns(14).ColumnName = Literal16.Text & " (L)"
                excelTable.Columns(15).ColumnName = "Idling Ltr/KM"
                excelTable.Columns(16).ColumnName = Literal16.Text & "(RM)"
                excelTable.Columns(17).ColumnName = "Idling RM/KM"
                excelTable.Columns(18).ColumnName = Literal17.Text
                excelTable.Columns(19).ColumnName = Literal18.Text
                excelTable.Columns(20).ColumnName = "OV Speed Cnt"
            Else
                excelTable.Columns(0).ColumnName = Literal6.Text

                excelTable.Columns(1).ColumnName = Literal7.Text
                excelTable.Columns(2).ColumnName = Literal8.Text
                excelTable.Columns(3).ColumnName = Literal4.Text
                excelTable.Columns(4).ColumnName = Literal9.Text & "(KM)"
                excelTable.Columns(5).ColumnName = Literal10.Text & " (L)"
                excelTable.Columns(6).ColumnName = Literal11.Text & "(RM)"
                excelTable.Columns(7).ColumnName = Literal12.Text
                excelTable.Columns(8).ColumnName = Literal13.Text
                excelTable.Columns(9).ColumnName = "RM/KM"
                excelTable.Columns(10).ColumnName = Literal14.Text & "  (L)"
                excelTable.Columns(11).ColumnName = Literal14.Text & " (RM)"
                excelTable.Columns(12).ColumnName = Literal15.Text
                excelTable.Columns(13).ColumnName = Literal16.Text & " (L)"
                excelTable.Columns(14).ColumnName = "Idling Ltr/KM"
                excelTable.Columns(15).ColumnName = Literal16.Text & "(RM)"
                excelTable.Columns(16).ColumnName = "Idling RM/KM"
                excelTable.Columns(17).ColumnName = Literal17.Text
                excelTable.Columns(18).ColumnName = Literal18.Text
                excelTable.Columns(19).ColumnName = "OV Speed Cnt"
            End If



            If t2.Rows.Count > 0 Then
                r = excelTable.NewRow
                If opr = 0 Then
                    r(5) = CDbl(GrantOdometer).ToString("0.00") & "KM"
                    r(6) = CDbl(GrantFuel).ToString("0.00") & "L"
                    r(7) = "RM " & CDbl(GrantPrice).ToString("0.00")
                    r(8) = CDbl(CDbl(GrantFuel / GrantOdometer).ToString("0.000")).ToString("0.00")
                    r(9) = CDbl(CDbl(GrantOdometer / GrantFuel).ToString("0.000")).ToString("0.00")
                    r(10) = CDbl(CDbl(GrantPrice / GrantOdometer).ToString("0.000")).ToString("0.00")
                    If r(8) = "NaN" Then
                        r(8) = "0.00"
                    End If
                    If r(9) = "NaN" Then
                        r(9) = "0.00"
                    End If
                    If r(10) = "NaN" Then
                        r(10) = "0.00"
                    End If
                    r(11) = CDbl(GrantRefuelLitre).ToString("0.00") & "L"
                    r(12) = "RM " & CDbl(GrantRefuelPrice).ToString("0.00")
                    r(14) = CDbl(GrandIdlingFuel).ToString("0.00") & "L"
                    r(16) = "RM " & CDbl(GrandIdlingPrice).ToString("0.00")
                Else
                    r(4) = CDbl(GrantOdometer).ToString("0.00") & "KM"
                    r(5) = CDbl(GrantFuel).ToString("0.00") & "L"
                    r(6) = "RM " & CDbl(GrantPrice).ToString("0.00")
                    r(7) = CDbl(CDbl(GrantFuel / GrantOdometer).ToString("0.000")).ToString("0.00")
                    r(8) = CDbl(CDbl(GrantOdometer / GrantFuel).ToString("0.000")).ToString("0.00")
                    r(9) = CDbl(CDbl(GrantPrice / GrantOdometer).ToString("0.000")).ToString("0.00")
                    If r(7) = "NaN" Then
                        r(7) = "0.00"
                    End If
                    If r(8) = "NaN" Then
                        r(8) = "0.00"
                    End If
                    If r(9) = "NaN" Then
                        r(9) = "0.00"
                    End If
                    r(10) = CDbl(GrantRefuelLitre).ToString("0.00") & "L"
                    r(11) = "RM " & CDbl(GrantRefuelPrice).ToString("0.00")
                    r(13) = CDbl(GrandIdlingFuel).ToString("0.00") & "L"
                    r(15) = "RM " & CDbl(GrandIdlingPrice).ToString("0.00")
                End If


                excelTable.Rows.Add(r)
            End If

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session.Remove("exceltable3")


            Session("exceltable") = excelTable

            sb.Length = 0

            'sb.Append("<thead><tr align=""left""><th >SNo</th>")
            'If opr = 0 Then
            '    sb.Append("<th >TimeStamp</th>")
            'End If
            'sb.Append("<th>User Name</th><th>Group Name</th><th>Plate No</th><th style='width:95px;'>Mileage (KM)</th><th>Usage (L)</th><th>Cost (RM)</th><th >Litre/KM</th><th>KM/Litre</th><th>RM/KM</th><th>Refuel (L)</th><th>Refuel (RM)</th><th>Idling Time</th><th>Idling (L)</th><th>Idling (RM)</th><th>Travelling Time</th><th>Stop Time</th></tr></thead>")
            sb.Append("<thead><tr align=""left""><th >" & Literal6.Text & "</th>")
            If opr = 0 Then
                sb.Append("<th >" & Literal41.Text & "</th>")
            End If
            sb.Append("<th>" & Literal7.Text & "</th><th>" & Literal8.Text & "</th><th>" & Literal4.Text & "</th><th style='width:95px;'>" & Literal9.Text & " (KM)</th><th>" & Literal10.Text & " (L)</th><th>" & Literal11.Text & " (RM)</th><th >" & Literal12.Text & "</th><th>" & Literal13.Text & "</th><th>RM/KM</th><th>" & Literal14.Text & " (L)</th><th> " & Literal14.Text & " (RM)</th><th>" & Literal15.Text & "</th><th>" & Literal16.Text & " (L)</th><th>Idling(Ltr/KM)</th><th>" & Literal16.Text & " (RM)</th><th>Idling(RM/KM)</th><th>" & Literal17.Text & "</th><th>" & Literal18.Text & "</th>")
            If userid = "1140" Then
                sb.Append("<th >OV Speed Cnt</th>")
            End If
            sb.Append("</tr></thead>")

            sb.Append("<tbody>")
            Dim counter As Integer = 1
            For i As Integer = 0 To t2.Rows.Count - 1
                sb.Append("<tr>")
                sb.Append("<td>")
                sb.Append(t2.Rows(i).Item("S No"))
                If opr = 0 Then
                    sb.Append("</td><td>")
                    sb.Append(t2.Rows(i).Item("timestamp"))
                End If
                sb.Append("</td><td>")
                sb.Append(t2.Rows(i).Item("User Name1"))
                sb.Append("</td><td>")
                sb.Append(t2.Rows(i).Item("GroupName"))
                sb.Append("</td><td>")
                sb.Append(t2.Rows(i).Item("PlateNo"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Mileage1"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Fuel1"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("FuelCost"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Liter/KM1"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("KM/Liter1"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Cost/liter1"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Refuel1"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Cost1"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("IdlingTime"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("IdlingFuel"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("IdlingFperM"))
                If t2.Rows(i).Item("IdlingFperM") = "--" Then
                    GrandIdleLkm = GrandIdleLkm + 0
                Else
                    GrandIdleLkm = GrandIdleLkm + CDbl(t2.Rows(i).Item("IdlingFperM"))
                End If
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("TotalIdlingCost"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("IdlingCperM"))
                If t2.Rows(i).Item("IdlingCperM") = "--" Then
                    GrandIdleRmKm = GrandIdleRmKm + 0
                Else
                    GrandIdleRmKm = GrandIdleRmKm + CDbl(t2.Rows(i).Item("IdlingCperM"))
                End If

                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("TravellingTime"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("StopTime"))
                sb.Append("</td>")
                If userid = "1140" Then
                    sb.Append("<td align='left'>")
                    sb.Append(t2.Rows(i).Item("Over Speed Count"))
                    sb.Append("</td>")
                End If
                sb.Append("</tr>")
            Next
            sb.Append("</tbody>")


            Dim sevenval As Double = 0
            Dim eightval As Double = 0
            Dim nineval As Double = 0



            If CDbl(CDbl(GrantFuel / GrantOdometer).ToString("0.000")).ToString("0.00") = "NaN" Then
                sevenval = "0.00"
            Else
                sevenval = CDbl(CDbl(GrantFuel / GrantOdometer).ToString("0.000")).ToString("0.00")
            End If

            If CDbl(CDbl(GrantOdometer / GrantFuel).ToString("0.000")).ToString("0.00") = "NaN" Or CDbl(GrantFuel) = 0 Then
                eightval = "0.00"
            Else
                eightval = CDbl(CDbl(GrantOdometer / GrantFuel).ToString("0.000")).ToString("0.00")
            End If
            If CDbl(CDbl(GrantPrice / GrantOdometer).ToString("0.000")).ToString("0.00") = "NaN" Then
                nineval = "0.00"
            Else
                nineval = CDbl(CDbl(GrantPrice / GrantOdometer).ToString("0.000")).ToString("0.00")
            End If

            sb.Append("<tfoot id=""fut""><tr align=""left"" ><th ></th>")
            If opr = 0 Then
                sb.Append("<th ></th>")
            End If
            sb.Append("<th></th><th></th><th>" & Literal19.Text & "</th><th align='right'>" & CDbl(GrantOdometer).ToString("0.00") & "KM" & "</th><th align='right'>" & CDbl(GrantFuel).ToString("0.00") & "L" & "</th><th align='right'>" & "RM&nbsp;" & CDbl(GrantPrice).ToString("0.00") & "</th><th align='right'>" & sevenval & "</th><th align='right'>" & eightval & "</th><th>" & nineval & "</th><th align='right'>" & CDbl(GrantRefuelLitre).ToString("0.00") & "L" & "</th><th align='right'>RM&nbsp;" & CDbl(GrantRefuelPrice).ToString("0.00") & "</th><th>" & GrandIdlingTime.ToString() & "</th><th align='right'>" & CDbl(GrandIdlingFuel).ToString("0.00") & "L" & "</th><th align='right'>" & CDbl(GrandIdleLkm).ToString("0.00") & "" & "</th><th align='right'>RM&nbsp;" & CDbl(GrandIdlingPrice).ToString("0.00") & "</th><th align='right'>" & CDbl(GrandIdleRmKm).ToString("0.00") & "</th><th>" & GrandTravellingTime.ToString() & "</th><th>" & GrandStopTime.ToString() & "</th>")

            'sb.Append("<th></th><th></th><th>TOTAL</th><th align='right'>" & CDbl(GrantOdometer).ToString("0.00") & "KM" & "</th><th align='right'>" & CDbl(GrantFuel).ToString("0.00") & "L" & "</th><th align='right'>" & "RM&nbsp;" & CDbl(GrantPrice).ToString("0.00") & "</th><th align='right'>" & sevenval & "</th><th align='right'>" & eightval & "</th><th>" & nineval & "</th><th align='right'>" & CDbl(GrantRefuelLitre).ToString("0.00") & "L" & "</th><th align='right'>RM&nbsp;" & CDbl(GrantRefuelPrice).ToString("0.00") & "</th><th>" & GrandIdlingTime.ToString() & "</th><th align='right'>" & CDbl(GrandIdlingFuel).ToString("0.00") & "L" & "</th><th align='right'>RM&nbsp;" & CDbl(GrandIdlingPrice).ToString("0.00") & "</th><th>" & GrandTravellingTime.ToString() & "</th><th>" & GrandStopTime.ToString() & "</th></tr></tfoot>")

            ' sb.Append("<tfoot><tr align=""left""><th >SNo</th><th>User Name</th><th>Group Name</th><th>Plate No</th><th style='width:95px;'>Mileage (KM)</th><th>Usage (L)</th><th>Cost (RM)</th><th >Litre/KM</th><th>KM/Litre</th><th>RM/KM</th><th>Refuel (L)</th><th>Refuel (RM)</th><th>Idling Time</th><th>Idling (L)</th><th>Idling (RM)</th></tr></tfoot>")
            If userid = "1140" Then
                sb.Append("<th >" + TotOspeed.ToString() + "</th>")
            End If
            sb.Append("</tr></tfoot>")
            ec = "true"

            Label2.Visible = True
            Label3.Visible = True


        Catch ex As Exception
            'Response.Write(ex.Message)
        End Try
    End Sub

    Function fuelPrice(ByVal userid As String) As DataTable
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim dsFuelPrice As New DataSet
        Dim da As SqlDataAdapter
        da = New SqlDataAdapter("select * from fuel_price where countrycode=(select countrycode from userTBL where userid='" & userid & "') order by startdate desc ", conn)
        Dim priceTable As New DataTable
        Try
            da.Fill(dsFuelPrice)
            priceTable.Columns.Add(New DataColumn("StartDate", GetType(DateTime)))
            priceTable.Columns.Add(New DataColumn("EndDate", GetType(DateTime)))
            priceTable.Columns.Add(New DataColumn("FuelPrice"))
            Dim pRow As DataRow
            If dsFuelPrice.Tables(0).Rows.Count > 0 Then
                For i As Int32 = 0 To dsFuelPrice.Tables(0).Rows.Count - 1
                    pRow = priceTable.NewRow
                    pRow(0) = dsFuelPrice.Tables(0).Rows(i)("startdate")
                    pRow(1) = dsFuelPrice.Tables(0).Rows(i)("enddate")
                    pRow(2) = dsFuelPrice.Tables(0).Rows(i)("fuelprice")
                    priceTable.Rows.Add(pRow)
                Next
            Else
                pRow = priceTable.NewRow
                pRow(0) = Now.ToString("yyyy/MM/dd")
                pRow(1) = Now.ToString("yyyy/MM/dd")
                pRow(2) = 0
                priceTable.Rows.Add(pRow)
            End If

        Catch

        End Try
        Return priceTable
    End Function

    Public Sub New()

    End Sub
End Class
