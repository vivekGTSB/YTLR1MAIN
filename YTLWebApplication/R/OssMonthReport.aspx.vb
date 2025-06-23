Imports System.Data.SqlClient
Imports System.Data
Imports ADODB
Imports AspMap
Imports System.IO
Imports System.Windows
Imports DocumentFormat.OpenXml.Drawing.Charts

Public Class OssMonthReport
    Inherits System.Web.UI.Page
    Public reportDateTime As String = ""
    Public noDataText As String = ""
    Public ec As String = "false"
    Public show As Boolean = False
    Public isD As String = "false"
    Public sb1 As New StringBuilder()
    Public sb2 As New StringBuilder()
    Public sb3 As New StringBuilder()
    Public strBeginDate As String = ""
    Public strEndDate As String = ""
    Dim bulkt As New System.Data.DataTable
    Dim bagt As New System.Data.DataTable

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim con As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand("select userid,username  from usertbl where companyname like 'YTL%' and role ='User' and username not like 'Returned%' order by username", con)
            con.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlPlants.Items.Clear()
            ddlPlants.Items.Add(New ListItem("ALL", "ALL"))
            While dr.Read()
                ddlPlants.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While
            con.Close()
            ddlyear.Items.Clear()
            Dim startyear As Integer = 2023
            While DateTime.Now.Year >= startyear
                ddlyear.Items.Add(New ListItem(startyear, startyear))
                startyear += 1
            End While
        Catch ex As Exception
            WriteLog("4" & ex.Message)
        Finally
            MyBase.OnInit(e)
        End Try
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
            WriteLog("3" & ex.Message)
        End Try
    End Sub



    Protected Sub DisplayLogInformation()
        Try
            reportonlbl.InnerText = Date.Now().ToString("yyyy/MM/dd HH:mm:dd")
            noDataText = ""
            Dim MainShipToCode As String = ""
            Dim shiptocode As String = ddlPlants.SelectedValue
            MainShipToCode = shiptocode
            Dim t As New System.Data.DataTable
            t.Columns.Add(New DataColumn("Transporters"))
            t.Columns.Add(New DataColumn("Bulk - Daily despatched"))
            t.Columns.Add(New DataColumn("Bulk - Cummulative"))
            t.Columns.Add(New DataColumn("Bag - Daily despatched"))
            t.Columns.Add(New DataColumn("Bag - Cummulative"))

            bulkt.Columns.Add(New DataColumn("Details of trips done (tonnes)"))
            bulkt.Columns.Add(New DataColumn("North"))
            bulkt.Columns.Add(New DataColumn("Centre"))
            bulkt.Columns.Add(New DataColumn("East"))
            bulkt.Columns.Add(New DataColumn("South"))
            bulkt.Columns.Add(New DataColumn("Total"))

            bagt.Columns.Add(New DataColumn("Details of trips done (tonnes)"))
            bagt.Columns.Add(New DataColumn("North"))
            bagt.Columns.Add(New DataColumn("Centre"))
            bagt.Columns.Add(New DataColumn("East"))
            bagt.Columns.Add(New DataColumn("South"))
            bagt.Columns.Add(New DataColumn("Total"))

            Dim r As DataRow
            Dim totalr As DataRow
            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmd As New SqlCommand()
            cmd.CommandText = "sp_GetPlantSummaryinfo"
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Connection = conn2

            cmd.Parameters.AddWithValue("@plant", ddlPlants.SelectedValue)

            cmd.Parameters.AddWithValue("@bdt", Convert.ToDateTime(txtBeginDate.Value).ToString("yyyy/MM/dd") & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00")
            cmd.Parameters.AddWithValue("@edt", Convert.ToDateTime(txtEndDate.Value).ToString("yyyy/MM/dd") & " " & ddlbh1.SelectedValue & ":" & ddlbm1.SelectedValue & ":00")
            Dim dates As Date = Convert.ToDateTime(txtBeginDate.Value)
            Dim cstart As String = "" & dates.Year & "/" & dates.Month & "/01 " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim cend As String = "" & dates.Year & "/" & dates.Month & "/" & Date.DaysInMonth(dates.Year, dates.Month).ToString() & " " & ddlbh1.SelectedValue & ":" & ddlbm1.SelectedValue & ":00"
            cmd.Parameters.AddWithValue("@cbdt", cstart)
            cmd.Parameters.AddWithValue("@cedt", cend)
            conn2.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            If dr.Read() Then
                totalr = t.NewRow
                r = t.NewRow()
                r(0) = "Internal"
                r(1) = dr("bulkin")
                r(2) = dr("bulkCin")
                r(3) = dr("bagin")
                r(4) = dr("bagCin")
                t.Rows.Add(r)
                totalr(1) = dr("bulkin")
                totalr(2) = dr("bulkCin")
                totalr(3) = dr("bagin")
                totalr(4) = dr("bagCin")
                r = t.NewRow()
                r(0) = "External"
                r(1) = dr("bulkex")
                r(2) = dr("bulkCex")
                r(3) = dr("bagex")
                r(4) = dr("bagCex")
                t.Rows.Add(r)
                totalr(1) = dr("bulkin") + dr("bulkex")
                totalr(2) = dr("bulkCin") + dr("bulkCex")
                totalr(3) = dr("bagin") + dr("bagex")
                totalr(4) = dr("bagCin") + dr("bagCex")
                r = t.NewRow()
                t.Rows.Add(r)
                totalr(0) = "Total"
                t.Rows.Add(totalr)
            End If
            conn2.Close()


            sb1.Length = 0
            ec = "True"
            If (t.Rows.Count > 0) Then
                sb1.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
                sb1.Append("<thead><tr><th class=""ui-state-default""></th><th class=""ui-state-default"">Transporters</th><th class=""ui-state-default"">Daily Dispatched - Bulk</th><th class=""ui-state-default"">Cummulative - Bulk</th><th class=""ui-state-default"">Daily Dispached - Bag</th><th class=""ui-state-default"">Cummulative - Bag</th></tr></thead>")
                sb1.Append("<tbody>")
                Dim counter As Integer = 1
                For i As Integer = 0 To t.Rows.Count - 1
                    Try
                        If i Mod 2 = 0 Then
                            sb1.Append("<tr class=""even"">")
                        Else
                            sb1.Append("<tr class=""odd"">")
                        End If
                        sb1.Append("<td></td><td class=""rightalign"">")
                        sb1.Append(t.DefaultView.Item(i)(0))
                        sb1.Append("</td><td class=""rightalign"">")
                        sb1.Append(t.DefaultView.Item(i)(1))
                        sb1.Append("</td><td class=""rightalign"">")
                        sb1.Append(t.DefaultView.Item(i)(2))
                        sb1.Append("</td><td class=""rightalign"">")
                        sb1.Append(t.DefaultView.Item(i)(3))
                        sb1.Append("</td><td class=""rightalign"">")
                        sb1.Append(t.DefaultView.Item(i)(4))
                        sb1.Append("</td></tr>")
                        counter += 1
                    Catch ex As Exception

                    End Try
                Next
                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr><th class=""ui-state-default""></th><th class=""ui-state-default"">Transporters</th><th class=""ui-state-default"">Daily Dispatched - Bulk</th><th class=""ui-state-default"">Cummulative - Bulk</th><th class=""ui-state-default"">Daily Dispached - Bag</th><th class=""ui-state-default"">Cummulative - Bag</th></tr></tfoot></table>")
            Else
                noDataText = ""
            End If
            Dim externdict As New Dictionary(Of transporterdata, regiondata)
            Dim internalregion As regiondata = getInteraldata(ddlPlants.SelectedValue, Convert.ToDateTime(txtBeginDate.Value).ToString("yyyy/MM/dd") & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00", Convert.ToDateTime(txtEndDate.Value).ToString("yyyy/MM/dd") & " " & ddlbh1.SelectedValue & ":" & ddlbm1.SelectedValue & ":00", 1)
            externdict = getTopExternaldata(ddlPlants.SelectedValue, Convert.ToDateTime(txtBeginDate.Value).ToString("yyyy/MM/dd") & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00", Convert.ToDateTime(txtEndDate.Value).ToString("yyyy/MM/dd") & " " & ddlbh1.SelectedValue & ":" & ddlbm1.SelectedValue & ":00", 1)
            sb2 = GenerateTable(internalregion, externdict, "examples1")
            internalregion = getInteraldata(ddlPlants.SelectedValue, Convert.ToDateTime(txtBeginDate.Value).ToString("yyyy/MM/dd") & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00", Convert.ToDateTime(txtEndDate.Value).ToString("yyyy/MM/dd") & " " & ddlbh1.SelectedValue & ":" & ddlbm1.SelectedValue & ":00", 2)
            externdict = getTopExternaldata(ddlPlants.SelectedValue, Convert.ToDateTime(txtBeginDate.Value).ToString("yyyy/MM/dd") & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00", Convert.ToDateTime(txtEndDate.Value).ToString("yyyy/MM/dd") & " " & ddlbh1.SelectedValue & ":" & ddlbm1.SelectedValue & ":00", 2)
            sb3 = GenerateTable(internalregion, externdict, "examples2")
            Session.Remove("exceltable")
            Session("exceltable") = t

            Session.Remove("exceltable2")
            Session("exceltable2") = bulkt

            Session.Remove("exceltable3")
            Session("exceltable3") = bagt
        Catch ex As Exception
            Response.Write("5" & ex.Message)
        End Try
    End Sub


    Protected Sub DisplayMonthInformation()
        Try
            reportonlbl.InnerText = Date.Now().ToString("yyyy/MM/dd HH:mm:dd")
            noDataText = ""
            Dim MainShipToCode As String = ""
            Dim username As String = ddlPlants.SelectedValue
            Dim months As String() = {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"}
            Dim t As New System.Data.DataTable
            t.Columns.Add(New DataColumn("Plateno"))
            t.Columns.Add(New DataColumn("January - Trips"))
            t.Columns.Add(New DataColumn("January - KM"))
            t.Columns.Add(New DataColumn("January - Inactive Days"))
            t.Columns.Add(New DataColumn("February - Trips"))
            t.Columns.Add(New DataColumn("February - KM"))
            t.Columns.Add(New DataColumn("February - Inactive Days"))
            t.Columns.Add(New DataColumn("March - Trips"))
            t.Columns.Add(New DataColumn("March - KM"))
            t.Columns.Add(New DataColumn("March - Inactive Days"))
            t.Columns.Add(New DataColumn("April - Trips"))
            t.Columns.Add(New DataColumn("April - KM"))
            t.Columns.Add(New DataColumn("April - Inactive Days"))
            t.Columns.Add(New DataColumn("May - Trips"))
            t.Columns.Add(New DataColumn("May - KM"))
            t.Columns.Add(New DataColumn("May - Inactive Days"))
            t.Columns.Add(New DataColumn("June - Trips"))
            t.Columns.Add(New DataColumn("June - KM"))
            t.Columns.Add(New DataColumn("June - Inactive Days"))
            t.Columns.Add(New DataColumn("July - Trips"))
            t.Columns.Add(New DataColumn("July - KM"))
            t.Columns.Add(New DataColumn("July - Inactive Days"))
            t.Columns.Add(New DataColumn("August - Trips"))
            t.Columns.Add(New DataColumn("August - KM"))
            t.Columns.Add(New DataColumn("August - Inactive Days"))
            t.Columns.Add(New DataColumn("September - Trips"))
            t.Columns.Add(New DataColumn("September - KM"))
            t.Columns.Add(New DataColumn("September - Inactive Days"))
            t.Columns.Add(New DataColumn("October - Trips"))
            t.Columns.Add(New DataColumn("October - KM"))
            t.Columns.Add(New DataColumn("October - Inactive Days"))
            t.Columns.Add(New DataColumn("November - Trips"))
            t.Columns.Add(New DataColumn("November - KM"))
            t.Columns.Add(New DataColumn("November - Inactive Days"))
            t.Columns.Add(New DataColumn("December - Trips"))
            t.Columns.Add(New DataColumn("December - KM"))
            t.Columns.Add(New DataColumn("December - Inactive Days"))
            t.Columns.Add(New DataColumn("Total - Trips"))
            t.Columns.Add(New DataColumn("Total - KM"))
            t.Columns.Add(New DataColumn("Total - Inactive Days"))


            Dim t1 As New System.Data.DataTable
            t1.Columns.Add(New DataColumn("Plateno"))
            t1.Columns.Add(New DataColumn("January - Trips"))
            t1.Columns.Add(New DataColumn("January - KM"))
            t1.Columns.Add(New DataColumn("January - Inactive Days"))
            t1.Columns.Add(New DataColumn("February - Trips"))
            t1.Columns.Add(New DataColumn("February - KM"))
            t1.Columns.Add(New DataColumn("February - Inactive Days"))
            t1.Columns.Add(New DataColumn("March - Trips"))
            t1.Columns.Add(New DataColumn("March - KM"))
            t1.Columns.Add(New DataColumn("March - Inactive Days"))
            t1.Columns.Add(New DataColumn("April - Trips"))
            t1.Columns.Add(New DataColumn("April - KM"))
            t1.Columns.Add(New DataColumn("April - Inactive Days"))
            t1.Columns.Add(New DataColumn("May - Trips"))
            t1.Columns.Add(New DataColumn("May - KM"))
            t1.Columns.Add(New DataColumn("May - Inactive Days"))
            t1.Columns.Add(New DataColumn("June - Trips"))
            t1.Columns.Add(New DataColumn("June - KM"))
            t1.Columns.Add(New DataColumn("June - Inactive Days"))
            t1.Columns.Add(New DataColumn("July - Trips"))
            t1.Columns.Add(New DataColumn("July - KM"))
            t1.Columns.Add(New DataColumn("July - Inactive Days"))
            t1.Columns.Add(New DataColumn("August - Trips"))
            t1.Columns.Add(New DataColumn("August - KM"))
            t1.Columns.Add(New DataColumn("August - Inactive Days"))
            t1.Columns.Add(New DataColumn("September - Trips"))
            t1.Columns.Add(New DataColumn("September - KM"))
            t1.Columns.Add(New DataColumn("September - Inactive Days"))
            t1.Columns.Add(New DataColumn("October - Trips"))
            t1.Columns.Add(New DataColumn("October - KM"))
            t1.Columns.Add(New DataColumn("October - Inactive Days"))
            t1.Columns.Add(New DataColumn("November - Trips"))
            t1.Columns.Add(New DataColumn("November - KM"))
            t1.Columns.Add(New DataColumn("November - Inactive Days"))
            t1.Columns.Add(New DataColumn("December - Trips"))
            t1.Columns.Add(New DataColumn("December - KM"))
            t1.Columns.Add(New DataColumn("December - Inactive Days"))
            t1.Columns.Add(New DataColumn("Total - Trips"))
            t1.Columns.Add(New DataColumn("Total - KM"))
            t1.Columns.Add(New DataColumn("Total - Inactive Days"))
            Dim platenodict As New Dictionary(Of String, List(Of Dictionary(Of Integer, reportdata)))
            Dim monthdict As Dictionary(Of Integer, reportdata)
            Dim monthlistdict As List(Of Dictionary(Of Integer, reportdata))
            Dim rdata As reportdata
            Dim r As DataRow
            Dim totalr As DataRow
            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand()
            If username = "ALL" Then
                cmd.CommandText = "select plateno, MONTH(dateadd(day,-1,timestamp)) as month,sum(km) as kmtravel,sum(trips) as tripscount,sum(inactivedays) as days from oss_result where timestamp between '" & ddlyear.SelectedValue & "/01/01' and '" & ddlyear.SelectedValue & "/12/31' group by plateno , MONTH(dateadd(day,-1,timestamp))  order by plateno ,MONTH(dateadd(day,-1,timestamp))"
            Else
                cmd.CommandText = "select t1.plateno, MONTH(dateadd(day,-1,timestamp)) as month,sum(km) as kmtravel,sum(trips) as tripscount,sum(inactivedays) as days from oss_result t1 inner join vehicleTBL t2 on t1.plateno=t2.plateno where t2.userid='" & username & "' and t1.timestamp between '" & ddlyear.SelectedValue & "/01/01' and '" & ddlyear.SelectedValue & "/12/31' group by t1.plateno , MONTH(dateadd(day,-1,timestamp))  order by plateno ,MONTH(dateadd(day,-1,timestamp))"
            End If


            cmd.Connection = conn2
            conn2.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            While dr.Read()
                If platenodict.ContainsKey(dr("plateno")) Then
                    monthlistdict = platenodict(dr("plateno"))
                    monthdict = New Dictionary(Of Integer, reportdata)
                    rdata = New reportdata
                    rdata.inactivedays = dr("days")
                    rdata.trips = dr("tripscount")
                    rdata.kmtravel = dr("kmtravel")
                    monthdict.Add(dr("month"), rdata)
                    monthlistdict.Add(monthdict)
                    platenodict(dr("plateno")) = monthlistdict
                Else
                    monthlistdict = New List(Of Dictionary(Of Integer, reportdata))
                    monthdict = New Dictionary(Of Integer, reportdata)
                    rdata = New reportdata
                    rdata.inactivedays = dr("days")
                    rdata.trips = dr("tripscount")
                    rdata.kmtravel = dr("kmtravel")
                    monthdict.Add(dr("month"), rdata)
                    monthlistdict.Add(monthdict)
                    platenodict.Add(dr("plateno"), monthlistdict)
                End If
            End While

            conn2.Close()
            Dim totaltrips, totalkm, totalinatcive As Integer

            For Each kval As KeyValuePair(Of String, List(Of Dictionary(Of Integer, reportdata))) In platenodict
                totalkm = 0
                totaltrips = 0
                totalinatcive = 0
                r = t.NewRow
                r(0) = kval.Key
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                r(5) = "-"
                r(6) = "-"
                r(7) = "-"
                r(8) = "-"
                r(9) = "-"
                r(10) = "-"
                r(11) = "-"
                r(12) = "-"
                r(13) = "-"
                r(14) = "-"
                r(15) = "-"
                r(16) = "-"
                r(17) = "-"
                r(18) = "-"
                r(19) = "-"
                r(20) = "-"
                r(21) = "-"
                r(22) = "-"
                r(23) = "-"
                r(24) = "-"
                r(25) = "-"
                r(26) = "-"
                r(27) = "-"
                r(28) = "-"
                r(29) = "-"
                r(30) = "-"
                r(31) = "-"
                r(32) = "-"
                r(33) = "-"
                r(34) = "-"
                r(35) = "-"
                r(36) = "-"
                r(37) = "-"
                r(38) = "-"
                r(39) = "-"
                monthlistdict = kval.Value
                If monthlistdict.Count > 0 Then
                    For Each kkval As Dictionary(Of Integer, reportdata) In monthlistdict
                        For i As Integer = 1 To 12
                            If (kkval.ContainsKey(i)) Then
                                totaltrips += kkval(i).trips
                                totalkm += kkval(i).kmtravel
                                totalinatcive += kkval(i).inactivedays
                            End If

                            'If (kkval.ContainsKey(i)) Then
                            '    r(i + 0) = kkval(i).trips
                            '    r(i + 1) = kkval(i).kmtravel
                            '    r(i + 2) = kkval(i).inactivedays
                            'End If

                        Next


                        If (kkval.ContainsKey(1)) Then
                            r(1) = kkval(1).trips
                            r(2) = kkval(1).kmtravel
                            r(3) = kkval(1).inactivedays
                            'Else
                            '    r(1) = "-"
                            '    r(2) = "-"
                            '    r(3) = "-"
                        End If

                        If (kkval.ContainsKey(2)) Then
                            r(4) = kkval(2).trips
                            r(5) = kkval(2).kmtravel
                            r(6) = kkval(2).inactivedays
                            'Else
                            '    r(4) = "-"
                            '    r(5) = "-"
                            '    r(6) = "-"
                        End If

                        If (kkval.ContainsKey(3)) Then
                            r(7) = kkval(3).trips
                            r(8) = kkval(3).kmtravel
                            r(9) = kkval(3).inactivedays
                            'Else
                            '    r(7) = "-"
                            '    r(8) = "-"
                            '    r(9) = "-"
                        End If

                        If (kkval.ContainsKey(4)) Then
                            r(10) = kkval(4).trips
                            r(11) = kkval(4).kmtravel
                            r(12) = kkval(4).inactivedays
                            'Else
                            '    r(10) = "-"
                            '    r(11) = "-"
                            '    r(12) = "-"
                        End If

                        If (kkval.ContainsKey(5)) Then
                            r(13) = kkval(5).trips
                            r(14) = kkval(5).kmtravel
                            r(15) = kkval(5).inactivedays
                            'Else
                            '    r(13) = "-"
                            '    r(14) = "-"
                            '    r(15) = "-"
                        End If

                        If (kkval.ContainsKey(6)) Then
                            r(16) = kkval(6).trips
                            r(17) = kkval(6).kmtravel
                            r(18) = kkval(6).inactivedays
                            'Else
                            '    r(16) = "-"
                            '    r(17) = "-"
                            '    r(18) = "-"
                        End If

                        If (kkval.ContainsKey(7)) Then
                            r(19) = kkval(7).trips
                            r(20) = kkval(7).kmtravel
                            r(21) = kkval(7).inactivedays
                        Else
                            'r(19) = "-"
                            'r(20) = "-"
                            'r(21) = "-"
                        End If

                        If (kkval.ContainsKey(8)) Then
                            r(22) = kkval(8).trips
                            r(23) = kkval(8).kmtravel
                            r(24) = kkval(8).inactivedays
                            'Else
                            '    r(22) = "-"
                            '    r(23) = "-"
                            '    r(24) = "-"
                        End If

                        If (kkval.ContainsKey(9)) Then
                            r(25) = kkval(9).trips
                            r(26) = kkval(9).kmtravel
                            r(27) = kkval(9).inactivedays
                            'Else
                            '    r(25) = "-"
                            '    r(26) = "-"
                            '    r(27) = "-"
                        End If

                        If (kkval.ContainsKey(10)) Then
                            r(28) = kkval(10).trips
                            r(29) = kkval(10).kmtravel
                            r(30) = kkval(10).inactivedays
                            'Else
                            '    r(28) = "-"
                            '    r(29) = "-"
                            '    r(30) = "-"
                        End If

                        If (kkval.ContainsKey(11)) Then
                            r(31) = kkval(11).trips
                            r(32) = kkval(11).kmtravel
                            r(33) = kkval(11).inactivedays
                            'Else
                            '    r(31) = "-"
                            '    r(32) = "-"
                            '    r(33) = "-"
                        End If

                        If (kkval.ContainsKey(12)) Then
                            r(35) = kkval(12).kmtravel
                            r(36) = kkval(12).inactivedays
                            r(34) = kkval(12).trips
                            'Else
                            '    r(34) = "-"
                            '    r(35) = "-"
                            '    r(36) = "-"
                        End If
                        r(37) = totaltrips
                        r(38) = totalkm
                        r(39) = totalinatcive
                    Next
                Else
                    r(1) = "-"
                    r(2) = "-"
                    r(3) = "-"
                    r(4) = "-"
                    r(5) = "-"
                    r(6) = "-"
                    r(7) = "-"
                    r(8) = "-"
                    r(9) = "-"
                    r(10) = "-"
                    r(11) = "-"
                    r(12) = "-"
                    r(13) = "-"
                    r(14) = "-"
                    r(15) = "-"
                    r(16) = "-"
                    r(17) = "-"
                    r(18) = "-"
                    r(19) = "-"
                    r(20) = "-"
                    r(21) = "-"
                    r(22) = "-"
                    r(23) = "-"
                    r(24) = "-"
                    r(25) = "-"
                    r(26) = "-"
                    r(27) = "-"
                    r(28) = "-"
                    r(29) = "-"
                    r(30) = "-"
                    r(31) = "-"
                    r(32) = "-"
                    r(33) = "-"
                    r(34) = "-"
                    r(35) = "-"
                    r(36) = "-"
                    r(37) = "-"
                    r(38) = "-"
                    r(39) = "-"
                End If

                t.Rows.Add(r)
            Next

            Dim total As Integer = 0
            If t.Rows.Count > 0 Then
                r = t.NewRow()
                For j As Integer = 1 To t.Columns.Count - 1
                    total = 0
                    For i As Integer = 1 To t.Rows.Count - 1
                        If Not IsDBNull(t.DefaultView.Item(i)(j)) Then
                            If Not t.DefaultView.Item(i)(j).ToString() = "-" Then
                                total += Convert.ToInt32(t.DefaultView.Item(i)(j))
                            End If
                        End If

                    Next
                    r(j) = total
                Next
            End If


            sb1.Length = 0
            ec = "True"
            If (t.Rows.Count > 0) Then
                sb1.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")

                Dim counter As Integer = 1
                Dim columns As String = ""
                'Try
                '    For i As Integer = 0 To t.Columns.Count - 1
                '        columns += "<th class=""ui-state-default"">" & t.Columns(i).ColumnName.Split("-")(1) & "</th>"
                '    Next
                'Catch ex As Exception
                '    Response.Write("Inside" & ex.StackTrace & " - " & ex.Message)
                'End Try

                Dim headcolumns As String = ""
                For i As Integer = 0 To months.Length - 1
                    columns += "<th class=""ui-state-default"">Trips</th><th class=""ui-state-default"">KM</th><th class=""ui-state-default"">Inactive</th>"
                    headcolumns += "<th  class=""ui-state-default"" colspan=3>" & months(i) & "</th>"
                Next
                sb1.Append("<thead><tr><th class=""ui-state-default"">" & ddlyear.SelectedValue & "</th>" & headcolumns & "<th  class=""ui-state-default"" colspan=3>Total</th></tr></thead>")
                sb1.Append("<thead><tr><th class=""ui-state-default"">Plateno</th>" & columns & "<th class=""ui-state-default"">Trips</th><th class=""ui-state-default"">KM</th><th class=""ui-state-default"">Inactive Days</th></tr></thead>")
                sb1.Append("<tbody>")
                For i As Integer = 0 To t.Rows.Count - 1
                    Try
                        If i Mod 2 = 0 Then
                            sb1.Append("<tr class=""even"">")
                        Else
                            sb1.Append("<tr class=""odd"">")
                        End If
                        For j As Integer = 0 To t.Columns.Count - 1
                            sb1.Append("<td class=""rightalign"">")
                            sb1.Append(t.DefaultView.Item(i)(j))
                            sb1.Append("</td>")
                        Next
                        sb1.Append("</td></tr>")

                        counter += 1
                    Catch ex As Exception

                    End Try
                Next

                sb1.Append("<tr class=""odd""><th>Total : </th>")
                For k As Integer = 1 To t.Columns.Count - 1
                    sb1.Append("<td class=""rightalign"">")
                    sb1.Append(r(k))
                    sb1.Append("</td>")
                Next
                sb1.Append("</tr>")



                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr><th class=""ui-state-default"">Plateno</th>" & columns & "<th class=""ui-state-default"">Trips</th><th class=""ui-state-default"">KM</th><th class=""ui-state-default"">Inactive</th></tr></tfoot></table>")
            Else
                noDataText = ""
            End If

            Session.Remove("exceltable")
            Session("exceltable") = t


        Catch ex As Exception
            Response.Write("5" & ex.Message & " - " & ex.StackTrace)
        End Try
    End Sub
    Protected Function getRegiondata(ByVal plant As String, ByVal bdt As String, ByVal edt As String, ByVal transporter As String, ByVal prodtype As Integer) As regiondata
        Dim region As New regiondata()
        Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Try
            Dim cmd As New SqlCommand("select  sum(qty) as tqty,region   from(select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0)   else round(dn_qty,0)  end qty,transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply in (@plant) and weight_outtime between @bdt and @edt and transporter_id in (select transporterid  from oss_transporter_master where internaltype =0)and productcode in (select productid from oss_product_master where producttype=@product)and transporter =@trans and region is not null) as T group by region  order by tqty desc", conn2)
            If plant = "PG" Then
                cmd.CommandText = "select  sum(qty) as tqty,region   from(select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0)   else round(dn_qty,0)  end qty,transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply in ('PG1','PG2','PG3') and weight_outtime between @bdt and @edt and transporter_id in (select transporterid  from oss_transporter_master where internaltype =0)and productcode in (select productid from oss_product_master where producttype=@product)and transporter =@trans and region is not null) as T group by region  order by tqty desc"
                cmd.Parameters.AddWithValue("@plant", "'PG1','PG2','PG3'")
            Else
                cmd.CommandText = "select  sum(qty) as tqty,region   from(select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0)   else round(dn_qty,0)  end qty,transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply in (@plant) and weight_outtime between @bdt and @edt and transporter_id in (select transporterid  from oss_transporter_master where internaltype =0)and productcode in (select productid from oss_product_master where producttype=@product)and transporter =@trans and region is not null) as T group by region  order by tqty desc"
                cmd.Parameters.AddWithValue("@plant", plant)
            End If
            cmd.Parameters.AddWithValue("@bdt", bdt)
            cmd.Parameters.AddWithValue("@edt", edt)
            cmd.Parameters.AddWithValue("@trans", transporter)
            cmd.Parameters.AddWithValue("@product", prodtype)
            conn2.Open()
            Dim dr2 As SqlDataReader = cmd.ExecuteReader()
            While (dr2.Read())
                If dr2("region") = "EAST" Then
                    region.east = dr2("tqty")
                ElseIf dr2("region") = "SOUTH" Then
                    region.south = dr2("tqty")
                ElseIf dr2("region") = "NORTH" Then
                    region.north = dr2("tqty")
                ElseIf dr2("region") = "CENTER" Then
                    region.centre = dr2("tqty")
                End If
            End While
        Catch ex As Exception

        End Try

        Return region
    End Function

    Protected Function getInteraldata(ByVal plant As String, ByVal bdt As String, ByVal edt As String, ByVal jobtype As Integer) As regiondata
        Dim internalregion As New regiondata()
        Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Try
            conn2.Open()
            Dim cmd2 As New SqlCommand()
            cmd2.Connection = conn2

            If ddlPlants.SelectedValue = "PG" Then
                cmd2.CommandText = "select  sum(qty) as tqty,region  from(select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0)   else round(dn_qty,0)  end qty,transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply in('PG1','PG2','PG3') and weight_outtime between @bdt and @edt and transporter_id in (select transporterid  from oss_transporter_master where internaltype =1)and productcode in (select productid from oss_product_master where producttype=@ptype) and t2.region is not null) as T group by region order by tqty desc"

            Else
                cmd2.CommandText = "select  sum(qty) as tqty,region  from(select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0)   else round(dn_qty,0)  end qty,transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply in(@plant) and weight_outtime between @bdt and @edt and transporter_id in (select transporterid  from oss_transporter_master where internaltype =1)and productcode in (select productid from oss_product_master where producttype=@ptype) and t2.region is not null) as T group by region order by tqty desc"
                cmd2.Parameters.AddWithValue("@plant", plant)
            End If
            cmd2.Parameters.AddWithValue("@bdt", bdt)
            cmd2.Parameters.AddWithValue("@edt", edt)
            cmd2.Parameters.AddWithValue("@ptype", jobtype)
            'cmd2.Parameters.AddWithValue("@internal", 1)
            'cmd2.Parameters.AddWithValue("@bulk", 1)
            Dim dr2 As SqlDataReader = cmd2.ExecuteReader()

            While dr2.Read()

                If dr2("region") = "EAST" Then
                    internalregion.east = dr2("tqty")
                ElseIf dr2("region") = "SOUTH" Then
                    internalregion.south = dr2("tqty")
                ElseIf dr2("region") = "NORTH" Then
                    internalregion.north = dr2("tqty")
                ElseIf dr2("region") = "CENTER" Then
                    internalregion.centre = dr2("tqty")
                End If


            End While
            conn2.Close()
        Catch ex As Exception

        End Try
        Return internalregion
    End Function
    Protected Function getTopExternaldata(ByVal plant As String, ByVal bdt As String, ByVal edt As String, ByVal jobtype As Integer) As Dictionary(Of transporterdata, regiondata)
        Dim externdict As New Dictionary(Of transporterdata, regiondata)
        Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Try
            Dim cmd3 As New SqlCommand()
            cmd3.CommandText = "select  sum(qty) as tqty,transporter   from (select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0)   else round(dn_qty,0)  end qty,transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply  in (@plant) and weight_outtime between @bdt and @edt and transporter_id in (select transporterid  from oss_transporter_master where internaltype =0)and productcode in (select productid from oss_product_master where producttype=@ptype)) as T group by transporter  order by tqty desc"
            cmd3.Connection = conn2
            If ddlPlants.SelectedValue = "PG" Then
                cmd3.CommandText = "select  sum(qty) as tqty,transporter   from (select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0)   else round(dn_qty,0)  end qty,transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply  in ('PG1','PG2','PG3') and weight_outtime between @bdt and @edt and transporter_id in (select transporterid  from oss_transporter_master where internaltype =0)and productcode in (select productid from oss_product_master where producttype=@ptype)) as T group by transporter  order by tqty desc"
                cmd3.Parameters.AddWithValue("@plant", "'PG1','PG2','PG3'")
            Else
                cmd3.CommandText = "select  sum(qty) as tqty,transporter   from (select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0)   else round(dn_qty,0)  end qty,transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply  in (@plant) and weight_outtime between @bdt and @edt and transporter_id in (select transporterid  from oss_transporter_master where internaltype =0)and productcode in (select productid from oss_product_master where producttype=@ptype)) as T group by transporter  order by tqty desc"
                cmd3.Parameters.AddWithValue("@plant", plant)
            End If
            cmd3.Parameters.AddWithValue("@bdt", bdt)
            cmd3.Parameters.AddWithValue("@edt", edt)
            cmd3.Parameters.AddWithValue("@ptype", jobtype)
            conn2.Open()
            Dim dr3 As SqlDataReader = cmd3.ExecuteReader()
            Dim region = New regiondata()
            Dim transdata As transporterdata
            While dr3.Read()
                transdata = New transporterdata
                transdata.transname = dr3("transporter")
                transdata.qty = dr3("tqty")
                region = getRegiondata(ddlPlants.SelectedValue, Convert.ToDateTime(txtBeginDate.Value).ToString("yyyy/MM/dd") & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00", Convert.ToDateTime(txtEndDate.Value).ToString("yyyy/MM/dd") & " " & ddlbh1.SelectedValue & ":" & ddlbm1.SelectedValue & ":00", transdata.transname, jobtype)
                externdict.Add(transdata, region)
            End While
        Catch ex As Exception

        End Try
        Return externdict
    End Function
    Protected Function GenerateTable(ByVal internalregion As regiondata, ByVal externdict As Dictionary(Of transporterdata, regiondata), ByVal tablename As String) As StringBuilder
        Dim internalsb As New StringBuilder()
        Try
            Dim r As DataRow
            internalsb.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""" & tablename & """ style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
            internalsb.Append("<thead><tr><th colspan=7>Details of trips done (tonnes)</th></tr>")
            internalsb.Append("<tr><th class=""ui-state-default""></th><th class=""ui-state-default"">North</th><th class=""ui-state-default"">Center</th><th class=""ui-state-default"">East</th><th class=""ui-state-default"">South</th><th class=""ui-state-default"">Total</th></tr>")
            internalsb.Append("<tbody>")
            If tablename = "examples1" Then
                r = bulkt.NewRow()
                r(0) = "Bulk"
                bulkt.Rows.Add(r)
            Else
                r = bagt.NewRow()
                r(0) = "Bag"
                bagt.Rows.Add(r)
            End If
            Dim total = internalregion.centre + internalregion.east + internalregion.north + internalregion.south
            If tablename = "examples1" Then
                r = bulkt.NewRow()
            Else
                r = bagt.NewRow()
            End If
            r(0) = "Internal"
            r(1) = internalregion.north
            r(2) = internalregion.centre
            r(3) = internalregion.east
            r(4) = internalregion.south
            r(5) = total
            If tablename = "examples1" Then
                bulkt.Rows.Add(r)
            Else
                bagt.Rows.Add(r)
            End If
            internalsb.Append("<tr><td>Internal</td><td class=""rightalign"">" & internalregion.north & "</td><td class=""rightalign"">" & internalregion.centre & "</td><td class=""rightalign"">" & internalregion.east & "</td><td class=""rightalign"">" & internalregion.south & "</td><td class=""rightalign"">" & total & "</td></tr>")
            internalsb.Append("<tr><td colspan=7></td></tr>")
            internalsb.Append("<tr><td>External (top3)</td><td colspan=6></td></tr>")
            If tablename = "examples1" Then
                r = bulkt.NewRow()
                bulkt.Rows.Add(r)
                r = bulkt.NewRow()
                r(0) = "External (top3)"
                bulkt.Rows.Add(r)
            Else
                r = bagt.NewRow()
                bagt.Rows.Add(r)
                r = bagt.NewRow()
                r(0) = "External (top3)"
                bagt.Rows.Add(r)
            End If
            Dim counter1 As Integer = 1
            Dim otherregion As New regiondata
            Dim externalregion As New regiondata
            For Each rdata As KeyValuePair(Of transporterdata, regiondata) In externdict

                If counter1 Mod 2 = 0 Then
                    internalsb.Append("<tr class=""even"">")
                Else
                    internalsb.Append("<tr class=""odd"">")
                End If
                If counter1 < 4 Then
                    If tablename = "examples1" Then
                        r = bulkt.NewRow()
                    Else
                        r = bagt.NewRow()
                    End If
                    internalsb.Append("<td>" & rdata.Key.transname & "</td><td class=""rightalign"">" & rdata.Value.north & "</td><td class=""rightalign"">" & rdata.Value.centre & "</td><td class=""rightalign"">" & rdata.Value.east & "</td><td class=""rightalign"">" & rdata.Value.south & "</td><td class=""rightalign"">" & rdata.Key.qty & "</td></tr>")
                    r(0) = rdata.Key.transname
                    r(1) = rdata.Value.north
                    r(2) = rdata.Value.centre
                    r(3) = rdata.Value.east
                    r(4) = rdata.Value.south
                    r(5) = rdata.Key.qty
                    If tablename = "examples1" Then
                        bulkt.Rows.Add(r)
                    Else
                        bagt.Rows.Add(r)
                    End If

                Else
                    otherregion.centre += rdata.Value.centre
                    otherregion.east += rdata.Value.east
                    otherregion.north += rdata.Value.north
                    otherregion.south += rdata.Value.south
                End If
                counter1 += 1
                externalregion.east += rdata.Value.east
                externalregion.north += rdata.Value.north
                externalregion.centre += rdata.Value.centre
                externalregion.south += rdata.Value.south
            Next
            Dim othertotal = otherregion.centre + otherregion.east + otherregion.north + otherregion.south
            internalsb.Append("<tr class=""odd""><td>Others</td><td class=""rightalign"">" & otherregion.north & "</td><td class=""rightalign"">" & otherregion.centre & "</td><td class=""rightalign"">" & otherregion.east & "</td><td class=""rightalign"">" & otherregion.south & "</td><td class=""rightalign"">" & othertotal & "</td></tr>")
            If tablename = "examples1" Then
                r = bulkt.NewRow()
            Else
                r = bagt.NewRow()
            End If
            r(0) = "Others"
            r(1) = otherregion.north
            r(2) = otherregion.centre
            r(3) = otherregion.east
            r(4) = otherregion.south
            r(5) = othertotal
            If tablename = "examples1" Then
                bulkt.Rows.Add(r)
            Else
                bagt.Rows.Add(r)
            End If
            Dim extotal = externalregion.centre + externalregion.east + externalregion.north + externalregion.south
            internalsb.Append("<tr class=""even""><td>Sub-total</td><td class=""rightalign"">" & externalregion.north & "</td><td class=""rightalign"">" & externalregion.centre & "</td><td class=""rightalign"">" & externalregion.east & "</td><td class=""rightalign"">" & externalregion.south & "</td><td class=""rightalign"">" & extotal & "</td></tr>")
            If tablename = "examples1" Then
                r = bulkt.NewRow()
            Else
                r = bagt.NewRow()
            End If
            r(0) = "Sub-total"
            r(1) = externalregion.north
            r(2) = externalregion.centre
            r(3) = externalregion.east
            r(4) = externalregion.south
            r(5) = extotal
            If tablename = "examples1" Then
                bulkt.Rows.Add(r)
            Else
                bagt.Rows.Add(r)
            End If
            internalsb.Append("<tr class=""odd""><td></td><td></td><td></td><td></td><td></td><td></td></tr>")
            If tablename = "examples1" Then
                r = bulkt.NewRow()
                bulkt.Rows.Add(r)
            Else
                r = bagt.NewRow()
                bagt.Rows.Add(r)
            End If
            internalsb.Append("<tr class=""even""><td>Total</td><td class=""rightalign"">" & internalregion.north + externalregion.north & "</td><td class=""rightalign"">" & internalregion.centre + externalregion.centre & "</td><td class=""rightalign"">" & internalregion.east + externalregion.east & "</td><td class=""rightalign"">" & internalregion.south + externalregion.south & "</td><td class=""rightalign"">" & total + extotal & "</td></tr>")
            If tablename = "examples1" Then
                r = bulkt.NewRow()
            Else
                r = bagt.NewRow()
            End If
            r(0) = "Total"
            r(1) = internalregion.north + externalregion.north
            r(2) = internalregion.centre + externalregion.centre
            r(3) = internalregion.east + externalregion.east
            r(4) = internalregion.south + externalregion.south
            r(5) = total + extotal
            If tablename = "examples1" Then
                bulkt.Rows.Add(r)
            Else
                bagt.Rows.Add(r)
            End If
            internalsb.Append("<tfoot><tr><th class=""ui-state-default""></th><th class=""ui-state-default"">North</th><th class=""ui-state-default"">Center</th><th class=""ui-state-default"">East</th><th class=""ui-state-default"">South</th><th class=""ui-state-default"">Total</th></tr></tfoot>")
            internalsb.Append("</table>")
        Catch ex As Exception

        End Try
        Return internalsb
    End Function
    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        DisplayMonthInformation()
    End Sub
    Protected Sub WriteLog(ByVal message As String)
        Try
            Response.Write(message)
        Catch ex As Exception

        End Try
    End Sub
    Class regiondata
        Public trans As String
        Public east As Integer
        Public south As Integer
        Public north As Integer
        Public centre As Integer
    End Class
    Class transporterdata
        Public transname As String
        Public qty As Integer
    End Class
    Class reportdata
        Public trips As Integer
        Public inactivedays As Integer
        Public kmtravel As Integer
    End Class
End Class