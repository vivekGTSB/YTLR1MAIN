Imports System.Data.SqlClient
Imports System.Data
Imports System.Globalization
Imports System.Text

Partial Class AllVehiclesSummaryReportOdo
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

            sb1.Length = 0
            sb1.Append(Literal1.Text)
            Dim userid As String = Request.Cookies("userinfo")("userid")
            If Page.IsPostBack = False Then
                sb.Length = 0
                '  sb.Append("<thead><tr align=""left""><th >SNo</th><th>User Name</th><th>Group Name</th><th>Plate No</th><th style='width:95px;'>Mileage (KM)</th><th>Usage (L)</th><th>Cost (RM)</th><th >Litre/KM</th><th>KM/Litre</th><th>RM/KM</th><th>Refuel (L)</th><th>Refuel (RM)</th><th>Idling Time</th><th>Idling (L)</th><th>Idling (RM)</th><th>Travelling Time</th><th>Stop Time</th></tr></thead>")
                sb.Append("<thead><tr align=""left""><th > " & Literal6.Text & "</th><th>" & Literal7.Text & "</th><th>" & Literal8.Text & "</th><th>" & Literal4.Text & "</th><th>Distance Travelled (KM)</th>")
                sb.Append("</tr></thead>")
                sb.Append("<tbody>")
                sb.Append("<tr>")
                For i As Integer = 0 To 4
                    sb.Append("<td>" & "--" & "</td>")
                Next

                sb.Append("</tr>")
                sb.Append("</tbody>")
                'sb.Append("<tfoot><tr align=""left""><th >SNo</th><th>User Name</th><th>Group Name</th><th>Plate No</th><th style='width:95px;'>Mileage (KM)</th><th>Usage (L)</th><th>Cost (RM)</th><th >Litre/KM</th><th>KM/Litre</th><th>RM/KM</th><th>Refuel (L)</th><th>Refuel (RM)</th><th>Idling Time</th><th>Idling (L)</th><th>Idling (RM)</th><th>Travelling Time</th><th>Stop Time</th></tr></tfoot>")
                sb.Append("<tfoot><tr align=""left""><th > " & Literal6.Text & "</th><th>" & Literal7.Text & "</th><th>" & Literal8.Text & "</th><th>" & Literal4.Text & "</th><th>Distance Travelled (KM)</th>")
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
            Dim t2 As New DataTable
            t2.Columns.Add(New DataColumn("S No"))
            t2.Columns.Add(New DataColumn("User Name1"))
            t2.Columns.Add(New DataColumn("GroupName"))
            t2.Columns.Add(New DataColumn("PlateNo"))
            t2.Columns.Add(New DataColumn("Mileage1"))
            Dim checkedNodes As TreeNodeCollection = tvPlateno.CheckedNodes



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
            conn.Open()
            For x As Int16 = 0 To checkedNodes.Count - 1
                If checkedNodes.Item(x).Checked = True Then
                    For y As Int16 = 0 To checkedNodes.Item(x).ChildNodes.Count - 1
                        Dim plateno As String = checkedNodes.Item(x).ChildNodes.Item(y).Text
                        cmd = New SqlCommand()
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.CommandText = "sp_getOdometerSummary"
                        cmd.Parameters.AddWithValue("@fromdate", txtBeginDate.Value)
                        cmd.Parameters.AddWithValue("@todate", txtEndDate.Value)
                        cmd.Parameters.AddWithValue("@plateno", plateno)
                        cmd.Connection = conn
                        dr = cmd.ExecuteReader()
                        While dr.Read()
                            r = t2.NewRow
                            r(0) = consumptionCount
                            r(1) = dr("uname").ToString().ToUpper()
                            r(2) = dr("grpname").ToString().ToUpper()
                            r(3) = dr("plateno")
                            r(4) = CDbl(dr("mileage")).ToString("0.00")
                            t2.Rows.Add(r)
                            GrantOdometer = GrantOdometer + CDbl(dr("mileage"))
                            consumptionCount += 1
                        End While
                        dr.Close()
                    Next
                End If

            Next


            If t2.Rows.Count = 0 Then
                r = t2.NewRow
                r(0) = "--"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"

                t2.Rows.Add(r)
            End If

            Dim excelTable As DataTable = t2.Copy()

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
            sb.Append("<thead><tr align=""left""><th > " & Literal6.Text & "</th><th>" & Literal7.Text & "</th><th>" & Literal8.Text & "</th><th>" & Literal4.Text & "</th><th>Distance Travelled (KM)</th>")
            sb.Append("</tr></thead>")
            sb.Append("<tbody>")

            Dim counter As Integer = 1
            For i As Integer = 0 To t2.Rows.Count - 1
                sb.Append("<tr>")
                sb.Append("<td>")
                sb.Append(t2.Rows(i).Item("S No"))
                sb.Append("</td><td>")
                sb.Append(t2.Rows(i).Item("User Name1"))
                sb.Append("</td><td>")
                sb.Append(t2.Rows(i).Item("GroupName"))
                sb.Append("</td><td>")
                sb.Append(t2.Rows(i).Item("PlateNo"))

                sb.Append("</td><td align='right'>")
                sb.Append(t2.Rows(i).Item("Mileage1"))
                sb.Append("</td>")
                sb.Append("</tr>")
            Next
            sb.Append("</tbody>")




            sb.Append("<tfoot id=""fut""><tr align=""left"" ><th ></th>")

            sb.Append("<th ></th>")

            sb.Append("<th></th><th></th><th align='right'>" & CDbl(GrantOdometer).ToString("0.00") & "KM" & "</th>")
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
