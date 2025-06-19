Imports System.Data.SqlClient
Imports System.Data
Imports System.Globalization
Imports System.Text

Partial Class AllVehiclesSummaryReportA
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Public test As StringBuilder = New StringBuilder()
    Public uid As String = ""

    Dim sCon As String = System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")
    Dim suspectTime As String
    Dim GrantOdometer, GrantFuel, GrantPrice, GrandIdlingFuel, GrandIdlingPrice, GrantRefuelLitre, GrantRefuelPrice As Double
    Dim GrandIdlingTime, GrandTravellingTime, GrandStopTime As TimeSpan
    Public sb As New StringBuilder()
    Public sb1 As New StringBuilder()

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            uid = Request.Cookies("userinfo")("userid")
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
            sb1 = New StringBuilder()
            sb1.Append("All Vehicles Summary Report")

            If Page.IsPostBack = False Then
                sb = New StringBuilder()
                sb.Append("<thead><tr align=""left""><th >SNo</th><th>User Name</th><th>Group Name</th><th>Plate No</th><th style='width:95px;'>Mileage (KM)</th><th>Usage (L)</th><th>Cost (RM)</th><th >Litre/KM</th><th>KM/Litre</th><th>RM/KM</th><th>Refuel (L)</th><th>Refuel (RM)</th><th>Idling Time</th><th>Idling (L)</th><th>Idling (RM)</th><th>Travelling Time</th><th>Stop Time</th></tr></thead>")
                sb.Append("<tbody>")
                sb.Append("<tr>")
                For i As Integer = 0 To 16
                    sb.Append("<td>" & "--" & "</td>")
                Next
                sb.Append("</tr>")
                sb.Append("</tbody>")
                sb.Append("<tfoot><tr align=""left""><th >SNo</th><th>User Name</th><th>Group Name</th><th>Plate No</th><th style='width:95px;'>Mileage (KM)</th><th>Usage (L)</th><th>Cost (RM)</th><th >Litre/KM</th><th>KM/Litre</th><th>RM/KM</th><th>Refuel (L)</th><th>Refuel (RM)</th><th>Idling Time</th><th>Idling (L)</th><th>Idling (RM)</th><th>Travelling Time</th><th>Stop Time</th></tr></tfoot>")

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

            If role = "Admin" Then
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

    'Protected Sub DisplayFuelInformation()

    '    Try


    '        Dim totalIdling As String
    '        Dim totalTravelling As String
    '        Dim totalStop As String

    '        Dim userid As String = Request.Cookies("userinfo")("userid")

    '        Dim t2 As New DataTable
    '        t2.Columns.Add(New DataColumn("S No"))
    '        t2.Columns.Add(New DataColumn("User Name"))
    '        t2.Columns.Add(New DataColumn("Plate No"))
    '        t2.Columns.Add(New DataColumn("Mileage"))
    '        t2.Columns.Add(New DataColumn("Fuel"))
    '        t2.Columns.Add(New DataColumn("Fuel Cost"))
    '        t2.Columns.Add(New DataColumn("Liter/KM"))
    '        t2.Columns.Add(New DataColumn("KM/Liter"))
    '        t2.Columns.Add(New DataColumn("Cost/liter"))
    '        t2.Columns.Add(New DataColumn("Refuel"))
    '        t2.Columns.Add(New DataColumn("Cost"))
    '        t2.Columns.Add(New DataColumn("Idling Time"))
    '        t2.Columns.Add(New DataColumn("Idling Fuel"))
    '        t2.Columns.Add(New DataColumn("Total Idling Cost"))
    '        t2.Columns.Add(New DataColumn("Group Name"))
    '        t2.Columns.Add(New DataColumn("Travelling Time"))
    '        t2.Columns.Add(New DataColumn("Stop Time"))
    '        Dim checkedNodes As TreeNodeCollection = tvPlateno.CheckedNodes

    '        Dim refuelCount As Int32 = 0
    '        Dim consumptionCount As Int32 = 0
    '        Dim idlingCount As Int32 = 0
    '        Dim eventCount As Int32 = 0
    '        Dim r As DataRow
    '        Dim dieselPrice As Double = 0
    '        Dim begindatetime As String = ""
    '        Dim enddatetime As String = ""
    '        begindatetime = txtBeginDate.Value & " 00:00:00"

    '        If Convert.ToDateTime(txtEndDate.Value).ToString("yyyy/MM/dd") = Now.ToString("yyyy/MM/dd") Then
    '            enddatetime = Now.ToString("yyyy/MM/dd HH:mm:ss")
    '        Else
    '            enddatetime = txtEndDate.Value & " 23:59:59"
    '        End If


    '        sb1.Clear()



    '        sb1.Append("Report generated from " & txtBeginDate.Value & " to " & txtEndDate.Value)

    '        For x As Int16 = 0 To checkedNodes.Count - 1
    '            If checkedNodes.Item(x).Checked = True Then
    '                Dim plateno As String = checkedNodes.Item(x).Text
    '                '$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
    '                Dim dFuel As New RefuelAnil(plateno, begindatetime, enddatetime)
    '                Dim dTable As New DataTable
    '                Dim dPrice As New DataTable

    '                Dim selectionDateTime As String = " timestamp between '" & begindatetime & "' and '" & enddatetime & "'"
    '                Dim yesterdayDateTime As String = " timestamp between '" & Convert.ToDateTime(begindatetime).AddDays(-1).ToString("yyyy/MM/dd") & " 00:00:00" & "' and '" & Convert.ToDateTime(begindatetime).AddDays(-1).ToString("yyyy/MM/dd") & " 23:59:59" & "'"
    '                dTable = dFuel.RefuelOnly(userid, plateno, selectionDateTime, yesterdayDateTime, 18)
    '                dPrice = dFuel.fuelPrice()

    '                '     Response.Write(dFuel.ErrMsg & "<br/>")
    '                '###### Fuel Consumption Table ###############################################################################
    '                r = t2.NewRow
    '                If dFuel.fuelstartdate <> "" And dFuel.fuelenddate <> "" Then
    '                    Dim drPrice2 As DataRow() = dPrice.Select("StartDate <= #" & dFuel.fuelstartdate & "# And EndDate >= #" & dFuel.fuelstartdate & "#")
    '                    r(0) = consumptionCount + 1
    '                    r(1) = checkedNodes.Item(x).Parent.Text.ToString().ToUpper()
    '                    r(14) = checkedNodes.Item(x).Value.ToString().ToUpper()
    '                    r(2) = plateno
    '                    r(3) = CDbl(dFuel.fuelOdometerTotal).ToString("0.00")
    '                    GrantOdometer = GrantOdometer + CDbl(dFuel.fuelOdometerTotal)

    '                    If dFuel.fuelOdometerTotal > 50 Then
    '                        r(4) = CDbl(dFuel.fuelConsumptionTotal).ToString("0.00")
    '                        r(5) = CDbl(dFuel.fuelPriceTotal).ToString("0.00")
    '                        r(6) = CDbl(CDbl(dFuel.fuelConsumptionTotal) / CDbl(dFuel.fuelOdometerTotal)).ToString("0.00")
    '                        r(7) = CDbl(CDbl(dFuel.fuelOdometerTotal) / CDbl(dFuel.fuelConsumptionTotal)).ToString("0.00")
    '                        r(8) = CDbl(CDbl(dFuel.fuelPriceTotal) / CDbl(dFuel.fuelOdometerTotal)).ToString("0.00")
    '                        GrantFuel = GrantFuel + CDbl(dFuel.fuelConsumptionTotal)
    '                        GrantPrice = GrantPrice + CDbl(dFuel.fuelPriceTotal)
    '                    Else
    '                        r(4) = ""
    '                        r(5) = ""
    '                        r(6) = ""
    '                        r(7) = ""
    '                        r(8) = ""
    '                    End If
    '                    ' Response.Write(r(6) & " -- " & r(7) & " -- " & r(8) & "<br/>")
    '                    '###### Refueling Table #####################################################################################
    '                    Dim refuelLitre As Double = 0
    '                    Dim refuelPrice As Double = 0
    '                    Dim subRefuelLitre As Double = 0
    '                    Dim subRefuelPrice As Double = 0
    '                    For i As Int32 = 0 To dTable.Rows.Count - 1
    '                        dieselPrice = 0
    '                        Dim drPrice As DataRow() = dPrice.Select("StartDate <= #" & Convert.ToDateTime(dTable.Rows(i)("refuelstoptime")).ToString("yyyy/MM/dd HH:mm:ss") & "# And EndDate >= #" & Convert.ToDateTime(dTable.Rows(i)("refueltime")).ToString("yyyy/MM/dd HH:mm:ss") & "#")
    '                        For Each row In drPrice
    '                            If (Convert.ToDateTime(row(0)) <= Convert.ToDateTime(dTable.Rows(i)("refuelstoptime"))) And (Convert.ToDateTime(row(1)) >= Convert.ToDateTime(dTable.Rows(i)("refuelstoptime"))) Then
    '                                dieselPrice = CDbl(row(2))
    '                                Exit For
    '                            End If
    '                        Next
    '                        refuelLitre = dTable.Rows(i)("refuelvolumetotal")
    '                        refuelPrice = CDbl(refuelLitre * dieselPrice).ToString("0.00")
    '                        If refuelLitre > 20 Then
    '                            subRefuelLitre = subRefuelLitre + refuelLitre
    '                            subRefuelPrice = subRefuelPrice + refuelPrice
    '                        End If
    '                    Next

    '                    If dFuel.fuelstartdate <> "" And dFuel.fuelenddate <> "" Then
    '                        If subRefuelLitre = 0 Then
    '                            r(9) = ""
    '                            r(10) = ""
    '                        Else
    '                            r(9) = CDbl(subRefuelLitre).ToString("0.00")
    '                            r(10) = CDbl(subRefuelPrice).ToString("0.00")
    '                            GrantRefuelLitre = GrantRefuelLitre + subRefuelLitre
    '                            GrantRefuelPrice = GrantRefuelPrice + subRefuelPrice
    '                        End If
    '                    End If

    '                    '###### End Refueling Table #################################################################################

    '                    '### new idling table ##########################################################################################
    '                    Dim idlingStruct = dFuel.getidlingInfo(plateno, begindatetime, enddatetime)
    '                    test.Append(dFuel.test)
    '                    Dim totalidlingtime As TimeSpan = idlingStruct.totITime

    '                    Dim totalidlingCost As Double = idlingStruct.totICost
    '                    Dim totaltravellingtime As TimeSpan = getTravellingMinutes(plateno, begindatetime, enddatetime)
    '                    Dim totaltimespan As TimeSpan = Convert.ToDateTime(enddatetime) - Convert.ToDateTime(begindatetime)
    '                    Dim totalstoptime As TimeSpan = totaltimespan - (totalidlingtime + totaltravellingtime)
    '                    Dim displaytext As String = "-"
    '                    Dim totalTravellingTimeText As String = "-"
    '                    Dim totalStopTimeText As String = "-"
    '                    Dim displayMinute As Double = 0

    '                    GrandIdlingTime = GrandIdlingTime.Add(TimeSpan.FromMinutes((totalidlingtime.Days * 24 * 60) + (totalidlingtime.Hours * 60) + totalidlingtime.Minutes))
    '                    GrandTravellingTime = GrandTravellingTime.Add(TimeSpan.FromMinutes((totaltravellingtime.Days * 24 * 60) + (totaltravellingtime.Hours * 60) + totaltravellingtime.Minutes))
    '                    GrandStopTime = GrandStopTime.Add(TimeSpan.FromMinutes((totalstoptime.Days * 24 * 60) + (totalstoptime.Hours * 60) + totalstoptime.Minutes))
    '                    If totalidlingtime.Days > 0 Then
    '                        displaytext = totalidlingtime.Days & " Days " & totalidlingtime.Hours & " Hrs " & totalidlingtime.Minutes & " Mins"
    '                        displayMinute = (totalidlingtime.Days * 24 * 60) + (totalidlingtime.Hours * 60) + totalidlingtime.Minutes
    '                    Else
    '                        displaytext = totalidlingtime.Hours & " Hrs " & totalidlingtime.Minutes & " Mins"
    '                        displayMinute = (totalidlingtime.Hours * 60) + totalidlingtime.Minutes
    '                    End If

    '                    If totaltravellingtime.Days > 0 Then
    '                        totalTravellingTimeText = totaltravellingtime.Days & " Days " & totaltravellingtime.Hours & " Hrs " & totaltravellingtime.Minutes & " Mins"
    '                    Else
    '                        totalTravellingTimeText = totaltravellingtime.Hours & " Hrs " & totaltravellingtime.Minutes & " Mins"
    '                    End If
    '                    If totalstoptime.Days > 0 Then
    '                        totalStopTimeText = totalstoptime.Days & " Days " & totalstoptime.Hours & " Hrs " & totalstoptime.Minutes & " Mins"
    '                    Else
    '                        totalStopTimeText = totalstoptime.Hours & " Hrs " & totalstoptime.Minutes & " Mins"
    '                    End If

    '                    If displayMinute > 0 Then
    '                        Try


    '                            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

    '                            '##########################################################################

    '                            Dim fuelstartdate As String
    '                            If dFuel.fuelstartdate = "" Then
    '                                fuelstartdate = begindatetime
    '                            Else
    '                                fuelstartdate = dFuel.fuelstartdate
    '                            End If

    '                            dieselPrice = 0
    '                            Dim drPrice3 As DataRow() = dPrice.Select("StartDate <= #" & fuelstartdate & "# And EndDate >= #" & fuelstartdate & "#")
    '                            For Each row In drPrice3
    '                                If (Convert.ToDateTime(row(0)) <= Convert.ToDateTime(fuelstartdate) And (Convert.ToDateTime(row(1)) >= Convert.ToDateTime(fuelstartdate))) Then
    '                                    dieselPrice = CDbl(row(2))
    '                                    Exit For
    '                                End If
    '                            Next

    '                            If idlingStruct.errMsg <> "" Then
    '                                Response.Write(idlingStruct.errMsg)
    '                            End If


    '                            'Dim drPrice3 As DataRow() = dPrice.Select("StartDate <= #" & fuelstartdate & "# And EndDate >= #" & fuelstartdate & "#")

    '                            r(11) = displaytext
    '                            r(12) = CDbl(displayMinute / 60 * idlingStruct.idlingVal).ToString("0.00")
    '                            r(13) = CDbl(totalidlingCost).ToString("0.00")
    '                            r(14) = checkedNodes.Item(x).Value.ToString.ToUpper()
    '                            r(15) = totalTravellingTimeText
    '                            r(16) = totalStopTimeText
    '                            GrandIdlingFuel = GrandIdlingFuel + r(12)
    '                            GrandIdlingPrice = GrandIdlingPrice + r(13)
    '                        Catch ex As Exception
    '                            Response.Write("Here " & ex.Message)
    '                        End Try
    '                    End If
    '                    '#######################################################################################################


    '                    t2.Rows.Add(r)
    '                    consumptionCount = consumptionCount + 1
    '                ElseIf False Then
    '                    consumptionCount = consumptionCount + 1
    '                    r = t2.NewRow
    '                    r(0) = consumptionCount
    '                    r(1) = checkedNodes.Item(x).Parent.Text
    '                    r(2) = plateno
    '                    r(3) = "--"
    '                    r(4) = "--"
    '                    r(5) = "--"
    '                    r(6) = "--"
    '                    r(7) = "--"
    '                    r(8) = "--"
    '                    r(9) = "--"
    '                    r(10) = "--"
    '                    r(11) = "--"
    '                    r(12) = "--"
    '                    r(13) = "--"
    '                    r(14) = "--"
    '                    r(15) = "--"
    '                    r(16) = "--"
    '                    t2.Rows.Add(r)
    '                End If
    '                '#####################################################################################################
    '                '$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

    '            End If
    '        Next

    '        If t2.Rows.Count = 0 Then
    '            r = t2.NewRow
    '            r(0) = "--"
    '            r(1) = "--"
    '            r(2) = "--"
    '            r(3) = "--"
    '            r(4) = "--"
    '            r(5) = "--"
    '            r(6) = "--"
    '            r(7) = "--"
    '            r(8) = "--"
    '            r(9) = "--"
    '            r(10) = "--"
    '            r(11) = "--"
    '            r(12) = "--"
    '            r(13) = "--"
    '            r(14) = "--"
    '            r(15) = "--"
    '            r(16) = "--"
    '            t2.Rows.Add(r)
    '        End If

    '        Dim excelTable As DataTable = t2.Copy()
    '        If t2.Rows.Count > 0 Then
    '            r = excelTable.NewRow

    '            r(3) = CDbl(GrantOdometer).ToString("0.00") & "KM"
    '            r(4) = CDbl(GrantFuel).ToString("0.00") & "L"
    '            r(5) = "RM " & CDbl(GrantPrice).ToString("0.00")
    '            r(6) = CDbl(CDbl(GrantFuel / GrantOdometer).ToString("0.000")).ToString("0.00")
    '            r(7) = CDbl(CDbl(GrantOdometer / GrantFuel).ToString("0.000")).ToString("0.00")
    '            r(8) = CDbl(CDbl(GrantPrice / GrantOdometer).ToString("0.000")).ToString("0.00")
    '            If r(6) = "NaN" Then
    '                r(6) = "0.00"
    '            End If
    '            If r(7) = "NaN" Then
    '                r(7) = "0.00"
    '            End If
    '            If r(8) = "NaN" Then
    '                r(8) = "0.00"
    '            End If
    '            r(9) = CDbl(GrantRefuelLitre).ToString("0.00") & "L"
    '            r(10) = "RM " & CDbl(GrantRefuelPrice).ToString("0.00")
    '            r(12) = CDbl(GrandIdlingFuel).ToString("0.00") & "L"
    '            r(13) = "RM " & CDbl(GrandIdlingPrice).ToString("0.00")

    '            excelTable.Rows.Add(r)
    '        End If

    '        Session.Remove("exceltable")
    '        Session.Remove("exceltable2")
    '        Session.Remove("exceltable3")
    '        If GrandIdlingTime.Days > 0 Then
    '            totalIdling = GrandIdlingTime.Days & " Days " & GrandIdlingTime.Hours & " Hrs " & GrandIdlingTime.Minutes & " Mins"
    '        Else
    '            totalIdling = GrandIdlingTime.Hours & " Hrs " & GrandIdlingTime.Minutes & " Mins"
    '        End If

    '        If GrandTravellingTime.Days > 0 Then
    '            totalTravelling = GrandTravellingTime.Days & " Days " & GrandTravellingTime.Hours & " Hrs " & GrandTravellingTime.Minutes & " Mins"
    '        Else
    '            totalTravelling = GrandTravellingTime.Hours & " Hrs " & GrandTravellingTime.Minutes & " Mins"
    '        End If

    '        If GrandStopTime.Days > 0 Then
    '            totalStop = GrandStopTime.Days & " Days " & GrandStopTime.Hours & " Hrs " & GrandStopTime.Minutes & " Mins"
    '        Else
    '            totalStop = GrandStopTime.Hours & " Hrs " & GrandStopTime.Minutes & " Mins"
    '        End If

    '        Session("exceltable") = excelTable

    '        sb.Clear()

    '        sb.Append("<thead><tr align=""left""><th >SNo</th><th>User Name</th><th>Group Name</th><th>Plate No</th><th style='width:95px;'>Mileage (KM)</th><th>Usage (L)</th><th>Cost (RM)</th><th >Litre/KM</th><th>KM/Litre</th><th>RM/KM</th><th>Refuel (L)</th><th>Refuel (RM)</th><th>Idling Time</th><th>Idling (L)</th><th>Idling (RM)</th><th>Travelling Time</th><th>Stop Time</th></tr></thead>")

    '        sb.Append("<tbody>")
    '        Dim counter As Integer = 1
    '        For i As Integer = 0 To t2.Rows.Count - 1
    '            sb.Append("<tr>")
    '            sb.Append("<td>")
    '            sb.Append(t2.Rows(i).Item("S No"))
    '            sb.Append("</td><td>")
    '            sb.Append(t2.Rows(i).Item("User Name"))
    '            sb.Append("</td><td>")
    '            sb.Append(t2.Rows(i).Item("Group Name"))
    '            sb.Append("</td><td>")
    '            sb.Append(t2.Rows(i).Item("Plate No"))
    '            sb.Append("</td><td align='right'>")
    '            sb.Append(t2.Rows(i).Item("Mileage"))
    '            sb.Append("</td><td align='right'>")
    '            sb.Append(t2.Rows(i).Item("Fuel"))
    '            sb.Append("</td><td align='right'>")
    '            sb.Append(t2.Rows(i).Item("Fuel Cost"))
    '            sb.Append("</td><td align='right'>")
    '            sb.Append(t2.Rows(i).Item("Liter/KM"))
    '            sb.Append("</td><td align='right'>")
    '            sb.Append(t2.Rows(i).Item("KM/Liter"))
    '            sb.Append("</td><td align='right'>")
    '            sb.Append(t2.Rows(i).Item("Cost/liter"))
    '            sb.Append("</td><td align='right'>")
    '            sb.Append(t2.Rows(i).Item("Refuel"))
    '            sb.Append("</td><td align='right'>")
    '            sb.Append(t2.Rows(i).Item("Cost"))
    '            sb.Append("</td><td align='left'>")
    '            sb.Append(t2.Rows(i).Item("Idling Time"))
    '            sb.Append("</td><td align='right'>")
    '            sb.Append(t2.Rows(i).Item("Idling Fuel"))
    '            sb.Append("</td><td align='right'>")
    '            sb.Append(t2.Rows(i).Item("Total Idling Cost"))
    '            sb.Append("</td><td align='left'>")
    '            sb.Append(t2.Rows(i).Item("Travelling Time"))
    '            sb.Append("</td><td align='left'>")
    '            sb.Append(t2.Rows(i).Item("Stop Time"))
    '            sb.Append("</td>")
    '            sb.Append("</tr>")
    '        Next
    '        sb.Append("</tbody>")


    '        Dim sevenval As Double = 0
    '        Dim eightval As Double = 0
    '        Dim nineval As Double = 0



    '        If CDbl(CDbl(GrantFuel / GrantOdometer).ToString("0.000")).ToString("0.00") = "NaN" Then
    '            sevenval = "0.00"
    '        Else
    '            sevenval = CDbl(CDbl(GrantFuel / GrantOdometer).ToString("0.000")).ToString("0.00")
    '        End If

    '        If CDbl(CDbl(GrantOdometer / GrantFuel).ToString("0.000")).ToString("0.00") = "NaN" Or CDbl(GrantFuel) = 0 Then
    '            eightval = "0.00"
    '        Else
    '            eightval = CDbl(CDbl(GrantOdometer / GrantFuel).ToString("0.000")).ToString("0.00")
    '        End If
    '        If CDbl(CDbl(GrantPrice / GrantOdometer).ToString("0.000")).ToString("0.00") = "NaN" Then
    '            nineval = "0.00"
    '        Else
    '            nineval = CDbl(CDbl(GrantPrice / GrantOdometer).ToString("0.000")).ToString("0.00")
    '        End If

    '        sb.Append("<tfoot id=""fut""><tr align=""left"" ><th ></th><th></th><th></th><th>TOTAL</th><th align='right'>" & CDbl(GrantOdometer).ToString("0.00") & "KM" & "</th><th align='right'>" & CDbl(GrantFuel).ToString("0.00") & "L" & "</th><th align='right'>" & "RM&nbsp;" & CDbl(GrantPrice).ToString("0.00") & "</th><th align='right'>" & sevenval & "</th><th align='right'>" & eightval & "</th><th>" & nineval & "</th><th align='right'>" & CDbl(GrantRefuelLitre).ToString("0.00") & "L" & "</th><th align='right'>RM&nbsp;" & CDbl(GrantRefuelPrice).ToString("0.00") & "</th><th>" & totalIdling & "</th><th align='right'>" & CDbl(GrandIdlingFuel).ToString("0.00") & "L" & "</th><th align='right'>RM&nbsp;" & CDbl(GrandIdlingPrice).ToString("0.00") & "</th><th>" & totalTravelling & "</th><th>" & totalStop & "</th></tr></tfoot>")

    '        ' sb.Append("<tfoot><tr align=""left""><th >SNo</th><th>User Name</th><th>Group Name</th><th>Plate No</th><th style='width:95px;'>Mileage (KM)</th><th>Usage (L)</th><th>Cost (RM)</th><th >Litre/KM</th><th>KM/Litre</th><th>RM/KM</th><th>Refuel (L)</th><th>Refuel (RM)</th><th>Idling Time</th><th>Idling (L)</th><th>Idling (RM)</th></tr></tfoot>")


    '        ec = "true"

    '        Label2.Visible = True
    '        Label3.Visible = True

    '    Catch ex As Exception
    '        Response.Write(ex.Message)
    '    End Try
    'End Sub

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
            t2.Columns.Add(New DataColumn("User Name"))
            t2.Columns.Add(New DataColumn("Group Name"))
            t2.Columns.Add(New DataColumn("Plate No"))
            t2.Columns.Add(New DataColumn("Mileage"))
            t2.Columns.Add(New DataColumn("Fuel"))
            t2.Columns.Add(New DataColumn("Fuel Cost"))
            t2.Columns.Add(New DataColumn("Liter/KM"))
            t2.Columns.Add(New DataColumn("KM/Liter"))
            t2.Columns.Add(New DataColumn("Cost/liter"))
            t2.Columns.Add(New DataColumn("Refuel"))
            t2.Columns.Add(New DataColumn("Cost"))
            t2.Columns.Add(New DataColumn("Idling Time"))
            t2.Columns.Add(New DataColumn("Idling Fuel"))
            t2.Columns.Add(New DataColumn("Total Idling Cost"))

            t2.Columns.Add(New DataColumn("Travelling Time"))
            t2.Columns.Add(New DataColumn("Stop Time"))
          
            Dim checkedNodes As TreeNodeCollection = tvPlateno.CheckedNodes

            Dim refuelCount As Int32 = 0
            Dim consumptionCount As Int32 = 0
            Dim idlingCount As Int32 = 0
            Dim eventCount As Int32 = 0
            Dim r As DataRow
            Dim dieselPrice As Double = 0
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

            sb.Append("Report generated from " & txtBeginDate.Value & " to " & txtEndDate.Value)
            Dim grandidilemin As Int32
            Dim grandtravelmin As Int32
            Dim grandstopmin As Int32
            conn.Open()

          
            For x As Int16 = 0 To checkedNodes.Count - 1
                If checkedNodes.Item(x).Checked = True Then
                    Dim plateno As String = checkedNodes.Item(x).Text
                    cmd = New SqlCommand()
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "sp_getFuelSummaryReport"
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
                            iSpan = TimeSpan.FromMinutes(dr("idlingtime"))
                            Dim totalidlingtime As String = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
                            iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
                            iSpan = TimeSpan.FromMinutes(dr("trvelingtime"))
                            Dim totaltraveltime As String = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
                            iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
                            iSpan = TimeSpan.FromMinutes(dr("stoptime"))
                            Dim totalstoptime As String = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
                            iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"

                            r(13) = totalidlingtime
                            r(14) = CDbl(dr("idelingfuel")).ToString("0.00")
                            r(15) = CDbl(dr("idlingcost")).ToString("0.00")
                            r(16) = totaltraveltime
                            r(17) = totalstoptime
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
                            iSpan = TimeSpan.FromMinutes(dr("idlingtime"))
                            Dim totalidlingtime As String
                            If iSpan.Days > 0 Then
                                totalidlingtime = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
                            iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
                            Else
                                totalidlingtime = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
                               iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
                            End If

                            iSpan = TimeSpan.FromMinutes(dr("trvelingtime"))
                            Dim totaltraveltime As String
                            If iSpan.Days > 0 Then
                                totaltraveltime = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
                                 iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
                            Else
                                totaltraveltime = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
                                 iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
                            End If
                          
                            iSpan = TimeSpan.FromMinutes(dr("stoptime"))
                            Dim totalstoptime As String
                            If iSpan.Days > 0 Then
                                totalstoptime = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
                         iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
                            Else
                                totalstoptime = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
                         iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
                            End If
                         
                            r(12) = totalidlingtime
                            r(13) = CDbl(dr("idelingfuel")).ToString("0.00")
                            r(14) = CDbl(dr("idlingcost")).ToString("0.00")
                            r(15) = totaltraveltime
                            r(16) = totalstoptime

                            If dr("mileage") >= 50 Then

                                GrantFuel = GrantFuel + CDbl(dr("usagefuel"))
                                GrantPrice = GrantPrice + CDbl(dr("usagefuelcost"))
                            End If
                        End If
                       


                        t2.Rows.Add(r)
                        GrantOdometer = GrantOdometer + CDbl(dr("mileage"))

                     

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
                End If
              
            Next
            iSpan = TimeSpan.FromMinutes(grandidilemin)
            Dim GrandIdlingTime As String
            If iSpan.Days > 0 Then
                GrandIdlingTime = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
               iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
            Else
                GrandIdlingTime = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
                iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
            End If
            iSpan = TimeSpan.FromMinutes(grandtravelmin)
            Dim GrandTravellingTime As String
            If iSpan.Days > 0 Then
                GrandTravellingTime = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
               iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
            Else
                GrandTravellingTime = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
                iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
            End If
            iSpan = TimeSpan.FromMinutes(grandstopmin)
            Dim GrandStopTime As String
            If iSpan.Days > 0 Then
                GrandStopTime = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
               iSpan.Minutes.ToString.PadLeft(2, "0"c) & " Min"
            Else
                GrandStopTime = iSpan.Hours.ToString.PadLeft(2, "0"c) & " Hrs " & _
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
                If opr = 0 Then
                    r(17) = "--"
                End If
                t2.Rows.Add(r)
            End If
            Dim excelTable As DataTable = t2.Copy()
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
                    r(15) = "RM " & CDbl(GrandIdlingPrice).ToString("0.00")
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
                    r(14) = "RM " & CDbl(GrandIdlingPrice).ToString("0.00")
                End If
              

                excelTable.Rows.Add(r)
            End If

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session.Remove("exceltable3")


            Session("exceltable") = excelTable

            sb = New StringBuilder()


            sb.Append("<thead><tr align=""left""><th >SNo</th>")
            If opr = 0 Then
                sb.Append("<th >TimeStamp</th>")
            End If
            sb.Append("<th>User Name</th><th>Group Name</th><th>Plate No</th><th style='width:95px;'>Mileage (KM)</th><th>Usage (L)</th><th>Cost (RM)</th><th >Litre/KM</th><th>KM/Litre</th><th>RM/KM</th><th>Refuel (L)</th><th>Refuel (RM)</th><th>Idling Time</th><th>Idling (L)</th><th>Idling (RM)</th><th>Travelling Time</th><th>Stop Time</th></tr></thead>")

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
                sb.Append(t2.Rows(i).Item("User Name"))
                sb.Append("</td><td>")
                sb.Append(t2.Rows(i).Item("Group Name"))
                sb.Append("</td><td>")
                sb.Append(t2.Rows(i).Item("Plate No"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Mileage"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Fuel"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Fuel Cost"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Liter/KM"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("KM/Liter"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Cost/liter"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Refuel"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Cost"))
                sb.Append("</td><td align='left'>")
                sb.Append(t2.Rows(i).Item("Idling Time"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Idling Fuel"))
                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Total Idling Cost"))
                sb.Append("</td><td align='left'>")
                sb.Append(t2.Rows(i).Item("Travelling Time"))
                sb.Append("</td><td align='left'>")
                sb.Append(t2.Rows(i).Item("Stop Time"))
                sb.Append("</td>")
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
            sb.Append("<th></th><th></th><th>TOTAL</th><th align='right'>" & CDbl(GrantOdometer).ToString("0.00") & "KM" & "</th><th align='right'>" & CDbl(GrantFuel).ToString("0.00") & "L" & "</th><th align='right'>" & "RM&nbsp;" & CDbl(GrantPrice).ToString("0.00") & "</th><th align='right'>" & sevenval & "</th><th align='right'>" & eightval & "</th><th>" & nineval & "</th><th align='right'>" & CDbl(GrantRefuelLitre).ToString("0.00") & "L" & "</th><th align='right'>RM&nbsp;" & CDbl(GrantRefuelPrice).ToString("0.00") & "</th><th>" & GrandIdlingTime.ToString() & "</th><th align='right'>" & CDbl(GrandIdlingFuel).ToString("0.00") & "L" & "</th><th align='right'>RM&nbsp;" & CDbl(GrandIdlingPrice).ToString("0.00") & "</th><th>" & GrandTravellingTime.ToString() & "</th><th>" & GrandStopTime.ToString() & "</th></tr></tfoot>")

            ' sb.Append("<tfoot><tr align=""left""><th >SNo</th><th>User Name</th><th>Group Name</th><th>Plate No</th><th style='width:95px;'>Mileage (KM)</th><th>Usage (L)</th><th>Cost (RM)</th><th >Litre/KM</th><th>KM/Litre</th><th>RM/KM</th><th>Refuel (L)</th><th>Refuel (RM)</th><th>Idling Time</th><th>Idling (L)</th><th>Idling (RM)</th></tr></tfoot>")


            ec = "true"

            Label2.Visible = True
            Label3.Visible = True


        Catch ex As Exception

        End Try
    End Sub


End Class
