Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports System.Linq

Public Class DeliveryReportYTL
    Inherits System.Web.UI.Page
    Public ec As String = "False"
    Public show As Boolean = False

    Public qureystr As String = "DeliveryReportYTL.aspx"
    Public sb1 As New StringBuilder()
    Public sb2 As New StringBuilder()
    Public Shared sb3 As New StringBuilder()
    Public adminusers As String
    Dim TrailerDict As New Dictionary(Of String, String)
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            ddlvehicletype.Items.Clear()
            ddlvehicletype.Items.Add(New ListItem("All", "All"))
            ddlvehicletype.Items.Add(New ListItem("CARGO", "CARGO"))
            ddlvehicletype.Items.Add(New ListItem("TANKER", "TANKER"))
            ddlvehicletype.Items.Add(New ListItem("TIPPER", "TIPPER"))
        Catch ex As Exception

        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
                Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
                Try
                    If Not Request.QueryString("bdt") = Nothing Then
                        begindatetime = Request.QueryString("bdt")
                        txtBeginDate.Value = begindatetime.Split(" ")(0)
                        ddlbh.SelectedValue = begindatetime.Split(" ")(1).Split(":")(0)
                        ddlbm.SelectedValue = begindatetime.Split(" ")(1).Split(":")(1)
                    End If
                    If Not Request.QueryString("edt") = Nothing Then
                        enddatetime = Request.QueryString("edt")
                        txtEndDate.Value = enddatetime.Split(" ")(0)
                        ddleh.SelectedValue = enddatetime.Split(" ")(1).Split(":")(0)
                        ddlem.SelectedValue = enddatetime.Split(" ")(1).Split(":")(1)
                    End If

                Catch ex As Exception

                End Try


                DisplayLogInformation(begindatetime, enddatetime)

            End If


        Catch ex As Exception

        End Try
    End Sub

    Protected Sub DisplayLogInformation(ByVal begindatetime As String, ByVal enddatetime As String)
        Try
            sb2.Clear()
            'Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            'Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            qureystr = "DeliveryReportYTL.aspx?bdt=" & begindatetime & "&edt=" & enddatetime & ""
            Dim vehicletype As String = ddlvehicletype.SelectedValue

            Dim uid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim smonth As String = Convert.ToDateTime(txtBeginDate.Value).Month
            Dim syear As String = Convert.ToDateTime(txtBeginDate.Value).Year
            Dim truckcmd As String = ""
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))

            Dim r As DataRow
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("SNo"))
            t.Columns.Add(New DataColumn("Plant Code"))
            t.Columns.Add(New DataColumn("Plant"))
            t.Columns.Add(New DataColumn("Bag External Transporter"))
            t.Columns.Add(New DataColumn("Bulk External Transporter"))
            t.Columns.Add(New DataColumn("Bag Own Transporter"))
            t.Columns.Add(New DataColumn("Bulk Own Transporter"))
            t.Columns.Add(New DataColumn("BagTotal"))
            t.Columns.Add(New DataColumn("BulkTotal"))
            t.Columns.Add(New DataColumn("Tipper Own Transporter"))
            t.Columns.Add(New DataColumn("Tipper External Transporter"))
            t.Columns.Add(New DataColumn("Total"))


            Dim excelt As New DataTable
            excelt.Columns.Add(New DataColumn("SNo"))
            excelt.Columns.Add(New DataColumn("Plant Code"))
            excelt.Columns.Add(New DataColumn("Plant"))
            excelt.Columns.Add(New DataColumn("Bag External Transporter"))
            excelt.Columns.Add(New DataColumn("Bulk External Transporter"))
            excelt.Columns.Add(New DataColumn("Bag Own Transporter"))
            excelt.Columns.Add(New DataColumn("Bulk Own Transporter"))
            excelt.Columns.Add(New DataColumn("BagTotal"))
            excelt.Columns.Add(New DataColumn("BulkTotal"))
            excelt.Columns.Add(New DataColumn("Tipper Own Transporter"))
            excelt.Columns.Add(New DataColumn("Tipper External Transporter"))
            excelt.Columns.Add(New DataColumn("Total"))

            Dim gpsplatenolist As New List(Of String)
            Dim ossplatenolist As New List(Of String)

            Dim YTLPlant As New List(Of String)
            Dim LafargePlant As New List(Of String)
            Dim FlyashPlant As New List(Of String)

            YTLPlant.Add("PR")
            YTLPlant.Add("BS")
            YTLPlant.Add("BC")
            YTLPlant.Add("WP")
            YTLPlant.Add("PG3")
            YTLPlant.Add("GPK")
            YTLPlant.Add("LM")


            LafargePlant.Add("KT")
            LafargePlant.Add("RW")
            LafargePlant.Add("LK")
            LafargePlant.Add("PG1")
            LafargePlant.Add("PG2")
            LafargePlant.Add("WP2")


            FlyashPlant.Add("KP")
            FlyashPlant.Add("TB")
            FlyashPlant.Add("MJG")
            FlyashPlant.Add("JEV")
            FlyashPlant.Add("JM")




            Dim planstreslist As New List(Of PlantRes)
            Dim Plantdict As New Dictionary(Of String, PlantRes)
            Dim cmd As SqlCommand
            Dim counter As Int16 = 0
            Dim platenocmd As String = ""
            Dim bagexternaltotal As Int32 = 0
            Dim baginternaltotal As Int32 = 0
            Dim bagTotal As Int32 = 0
            Dim bulkTotal As Int32 = 0
            Dim tipperTotal As Int32 = 0
            Dim tipperexternaltotal As Int32 = 0
            Dim tipperinternaltotal As Int32 = 0


            Dim bulkexternaltotal As Int32 = 0
            Dim bulkinternaltotal As Int32 = 0
            Dim eitotal As Int32 = 0

            Dim baggrandexternaltotal As Int32 = 0
            Dim baggrandinternaltotal As Int32 = 0

            Dim grandBagTotal As Int32 = 0
            Dim grandBulkTotal As Int32 = 0

            Dim bulkgrandexternaltotal As Int32 = 0
            Dim bulkgrandinternaltotal As Int32 = 0

            Dim tippergrandexternaltotal As Int32 = 0
            Dim tippergrandinternaltotal As Int32 = 0
            Dim grandeitotal As Int32 = 0


            For Each s As String In YTLPlant
                Dim p As New PlantRes()
                p.bagexternal = 0
                p.baginternal = 0
                p.bulkinternal = 0
                p.bulkexternal = 0
                p.tipperexternal = 0
                p.tipperinternal = 0

                Select Case (s)
                    Case "PR"
                        p.plantname = "PADANG RENGAS"
                    Case "BS"
                        p.plantname = "BUKIT SAGU"
                    Case "BC"
                        p.plantname = "BATU CAVES"
                    Case "WP"
                        p.plantname = "WESTPORT, PULAU INDAH"
                    Case "PG3"
                        p.plantname = "PASIR GUDANG"
                    Case "GPK"
                        p.plantname = "GPK"
                    Case "LM"
                        p.plantname = "LM"
                End Select
                Plantdict.Add(s, p)
            Next

            For Each s As String In LafargePlant
                Dim p As New PlantRes()
                p.bagexternal = 0
                p.baginternal = 0
                p.bulkinternal = 0
                p.bulkexternal = 0
                p.tipperexternal = 0
                p.tipperinternal = 0

                Select Case (s)
                    Case "KT"
                        p.plantname = "KANTHAN"
                    Case "RW"
                        p.plantname = "RAWANG"
                    Case "LK"
                        p.plantname = "LANGKAWI"
                    Case "PG1"
                        p.plantname = "PG1"
                    Case "PG2"
                        p.plantname = "PG2"
                    Case "WP2"
                        p.plantname = "WESTPORT2"
                End Select
                Plantdict.Add(s, p)
            Next

            For Each s As String In FlyashPlant
                Dim p As New PlantRes()
                p.bagexternal = 0
                p.baginternal = 0
                p.bulkinternal = 0
                p.bulkexternal = 0
                p.tipperexternal = 0
                p.tipperinternal = 0

                Select Case (s)
                    Case "KP"
                        p.plantname = "KAPAR"
                    Case "TB"
                        p.plantname = "TANJUNG BIN"
                    Case "MJG"
                        p.plantname = "MANJUNG"
                    Case "JEV"
                        p.plantname = "JEV"
                    Case "JM"
                        p.plantname = "JIMAH"
                End Select

                Plantdict.Add(s, p)
            Next

            If vehicletype = "CARGO" Then
                truckcmd = "  and pm.producttype=2 "
            ElseIf vehicletype = "TANKER" Then
                truckcmd = "  and pm.producttype=1 "
            ElseIf vehicletype = "TIPPER" Then
                truckcmd = "  and pm.producttype=3 "
            Else
                truckcmd = " "
            End If

            Try
                '**************    External
                cmd = New SqlCommand("select producttype,source_supply,count(*) as jobscount,t2.PV_DisplayName from oss_patch_out t1 left outer join oss_plant_master t2 on t1.source_supply=t2.PV_Plant left outer join oss_product_master pm on pm.productid=t1.productcode   where weight_outtime between '" & begindatetime & "' and '" & enddatetime & "' and transporter_id in (select transporterid from oss_transporter_master where internaltype=0)  " & truckcmd & " group by source_supply,t2.PV_DisplayName,producttype", conn2)

                conn2.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                While dr.Read()
                    If Plantdict.ContainsKey(dr("source_supply")) Then
                        Dim pr As PlantRes = Plantdict(dr("source_supply"))
                        If dr("producttype").ToString() = "2" Then
                            pr.bagexternal = dr("jobscount")
                        ElseIf dr("producttype").ToString() = "1" Then
                            pr.bulkexternal = dr("jobscount")
                        ElseIf dr("producttype").ToString() = "3" Then
                            pr.tipperexternal = dr("jobscount")
                        End If
                        pr.plantname = dr("PV_DisplayName")
                        ' Plantdict.Remove(dr("source_supply"))
                        Plantdict.Item(dr("source_supply")) = pr
                        '  Plantdict.Add(dr("source_supply"), pr)
                    Else
                        Dim pr As New PlantRes()
                        pr.plantname = dr("PV_DisplayName")
                        If dr("producttype").ToString() = "2" Then
                            pr.bagexternal = dr("jobscount")
                        ElseIf dr("producttype").ToString() = "1" Then
                            pr.bulkexternal = dr("jobscount")
                        ElseIf dr("producttype").ToString() = "3" Then
                            pr.tipperexternal = dr("jobscount")
                        End If
                        Plantdict.Add(dr("source_supply"), pr)
                    End If
                End While
                dr.Close()

                'Internal
                cmd.CommandText = "select producttype,source_supply,count(*) as jobscount,t2.PV_DisplayName from oss_patch_out t1 left outer join oss_plant_master t2 on t1.source_supply=t2.PV_Plant left outer join oss_product_master pm on pm.productid=t1.productcode where weight_outtime between '" & begindatetime & "' and '" & enddatetime & "' and transporter_id in (select transporterid from oss_transporter_master where internaltype=1) " & truckcmd & " group by source_supply,t2.PV_DisplayName,producttype"

                dr = cmd.ExecuteReader()
                While dr.Read()
                    If Plantdict.ContainsKey(dr("source_supply")) Then
                        Dim pr As PlantRes = Plantdict(dr("source_supply"))
                        If dr("producttype").ToString() = "2" Then
                            pr.baginternal = dr("jobscount")
                        ElseIf dr("producttype").ToString() = "1" Then
                            pr.bulkinternal = dr("jobscount")
                        ElseIf dr("producttype").ToString() = "3" Then
                            pr.tipperinternal = dr("jobscount")
                        End If
                        pr.plantname = dr("PV_DisplayName")
                        'Plantdict.Remove(dr("source_supply"))
                        'Plantdict.Add(dr("source_supply"), pr)
                        Plantdict.Item(dr("source_supply")) = pr
                    Else
                        Dim pr As New PlantRes()
                        pr.bulkinternal = 0
                        pr.plantname = dr("PV_DisplayName")
                        If dr("producttype").ToString() = "2" Then
                            pr.baginternal = dr("jobscount")
                        ElseIf dr("producttype").ToString() = "1" Then
                            pr.bulkinternal = dr("jobscount")
                        ElseIf dr("producttype").ToString() = "3" Then
                            pr.tipperinternal = dr("jobscount")
                        End If
                        Plantdict.Add(dr("source_supply"), pr)
                    End If
                End While
                dr.Close()

                counter = 1
                Dim kval As KeyValuePair(Of String, PlantRes)
                For Each kval In Plantdict
                    r = t.NewRow()
                    r(0) = counter
                    r(1) = kval.Key
                    Dim pr As PlantRes = kval.Value
                    r(2) = pr.plantname
                    r(3) = pr.bagexternal
                    r(4) = pr.bulkexternal
                    r(5) = pr.baginternal
                    r(6) = pr.bulkinternal
                    r(7) = pr.bagexternal + pr.baginternal
                    r(8) = pr.bulkexternal + pr.bulkinternal
                    r(9) = pr.tipperinternal
                    r(10) = pr.tipperexternal
                    r(11) = pr.tipperinternal + pr.tipperexternal 'pr.bagexternal + pr.baginternal + pr.bulkexternal + pr.bulkinternal + pr.tipperinternal + pr.tipperexternal
                    t.Rows.Add(r)
                    ' counter += 1
                Next



                sb2.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 13px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
                sb2.Append("<thead><tr><th>S No</th><th>Plant Code</th><th>Plant</th><th>Bag External</th><th>Bulk External</th><th>Bag Internal</th><th>Bulk Internal</th><th>Bag Total</th><th>Bulk Total</th><th>Tipper External</th><th>Tipper Internal</th><th>Total</th></tr></thead>")
                sb2.Append("<tbody>")

                'YTL Plant
                bagexternaltotal = 0
                baginternaltotal = 0
                eitotal = 0
                counter = 1
                For i As Integer = 0 To t.Rows.Count - 1
                    If YTLPlant.Contains(t.Rows(i)(1)) Then
                        sb2.Append("<tr>")
                        sb2.Append("<td>")
                        sb2.Append(counter)
                        sb2.Append("</td><td>")
                        sb2.Append(t.Rows(i)(1))
                        sb2.Append("</td><td>")
                        sb2.Append(t.Rows(i)(2))
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(3) & "</span>")
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(4) & "</span>")

                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(5) & "</span>")
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(6) & "</span>")

                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagall')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(7) & "</span>")
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkall')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(8) & "</span>")



                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','tipperex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(10) & "</span>")
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','tipperin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(9) & "</span>")


                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','0','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(11) & "</span>")
                        sb2.Append("</td>")
                        sb2.Append("</tr>")
                        bagexternaltotal += t.Rows(i)(3)
                        bulkexternaltotal += t.Rows(i)(4)
                        baginternaltotal += t.Rows(i)(5)
                        bulkinternaltotal += t.Rows(i)(6)

                        bagTotal = bagTotal + t.Rows(i)(3) + t.Rows(i)(5)
                        bulkTotal = bulkTotal + t.Rows(i)(4) + t.Rows(i)(6)

                        tipperinternaltotal += t.Rows(i)(9)
                        tipperexternaltotal += t.Rows(i)(10)

                        eitotal += t.Rows(i)(11)

                        r = excelt.NewRow()
                        r(0) = counter
                        r(1) = t.Rows(i)(1)
                        r(2) = t.Rows(i)(2)
                        r(3) = t.Rows(i)(3)
                        r(4) = t.Rows(i)(4)
                        r(5) = t.Rows(i)(5)
                        r(6) = t.Rows(i)(6)
                        r(7) = t.Rows(i)(7)
                        r(8) = t.Rows(i)(8)
                        r(9) = t.Rows(i)(9)
                        r(10) = t.Rows(i)(10)
                        r(11) = t.Rows(i)(11)
                        excelt.Rows.Add(r)
                        counter += 1
                    End If
                Next
                sb2.Append("<tr>")
                sb2.Append("<td>-")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('YTLALL','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bagexternaltotal & "</span>")
                'sb2.Append(bagexternaltotal)
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('YTLALL','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bulkexternaltotal & "</span>")


                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('YTLALL','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & baginternaltotal & "</span>")
                'sb2.Append(bagexternaltotal)
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('YTLALL','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bulkinternaltotal & "</span>")

                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('YTLALL','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagall')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bagTotal & "</span>")
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('YTLALL','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkall')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bulkTotal & "</span>")


                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('YTLALL','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','tipperex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & tipperexternaltotal & "</span>")

                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('YTLALL','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','tipperin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & tipperinternaltotal & "</span>")


                'sb2.Append(baginternaltotal)
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('YTLALL','" & syear & "','" & smonth & "','0','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & eitotal & "</span>")
                'sb2.Append(eitotal)
                sb2.Append("</td>")
                sb2.Append("</tr>")


                r = excelt.NewRow()
                r(0) = "-"
                r(1) = ""
                r(2) = ""
                r(3) = bagexternaltotal
                r(4) = bulkexternaltotal
                r(5) = baginternaltotal
                r(6) = bulkinternaltotal
                r(7) = bagTotal
                r(8) = bulkTotal
                r(9) = tipperinternaltotal
                r(10) = tipperexternaltotal
                r(11) = eitotal
                excelt.Rows.Add(r)

                sb2.Append("<tr>")
                sb2.Append("<td>-")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td>")
                sb2.Append("</tr>")

                r = excelt.NewRow()
                r(0) = "-"
                r(1) = ""
                r(2) = ""
                r(3) = ""
                r(4) = ""
                r(5) = ""
                r(6) = ""
                r(7) = ""
                r(8) = ""
                r(9) = ""
                r(10) = ""
                r(11) = ""
                excelt.Rows.Add(r)


                'Lafarge Plant
                bagexternaltotal = 0
                baginternaltotal = 0
                bulkexternaltotal = 0
                bulkinternaltotal = 0
                tipperinternaltotal = 0
                tipperexternaltotal = 0
                bagTotal = 0
                bulkTotal = 0
                tipperTotal = 0
                eitotal = 0
                For i As Integer = 0 To t.Rows.Count - 1
                    If LafargePlant.Contains(t.Rows(i)(1)) Then
                        sb2.Append("<tr>")
                        sb2.Append("<td>")
                        sb2.Append(counter)
                        sb2.Append("</td><td>")
                        sb2.Append(t.Rows(i)(1))
                        sb2.Append("</td><td>")
                        sb2.Append(t.Rows(i)(2))
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(3) & "</span>")
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(4) & "</span>")
                        sb2.Append("</td><td>")



                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(5) & "</span>")
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(6) & "</span>")


                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagall')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(7) & "</span>")
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkall')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(8) & "</span>")




                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','tipperin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(10) & "</span>")
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','tipperex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(9) & "</span>")
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','0','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(11) & "</span>")
                        sb2.Append("</td>")
                        sb2.Append("</tr>")
                        bagexternaltotal += t.Rows(i)(3)
                        bulkexternaltotal += t.Rows(i)(4)
                        baginternaltotal += t.Rows(i)(5)
                        bulkinternaltotal += t.Rows(i)(6)
                        bagTotal = bagTotal + t.Rows(i)(3) + t.Rows(i)(5)
                        bulkTotal = bulkTotal + t.Rows(i)(4) + t.Rows(i)(6)

                        tipperexternaltotal += t.Rows(i)(10)
                        tipperinternaltotal += t.Rows(i)(9)

                        eitotal += t.Rows(i)(11)


                        r = excelt.NewRow()
                        r(0) = counter
                        r(1) = t.Rows(i)(1)
                        r(2) = t.Rows(i)(2)
                        r(3) = t.Rows(i)(3)
                        r(4) = t.Rows(i)(4)
                        r(5) = t.Rows(i)(5)
                        r(6) = t.Rows(i)(6)
                        r(7) = t.Rows(i)(7)
                        r(8) = t.Rows(i)(8)
                        r(9) = t.Rows(i)(9)
                        r(10) = t.Rows(i)(10)
                        r(11) = t.Rows(i)(11)
                        excelt.Rows.Add(r)

                        counter += 1
                    End If
                Next
                sb2.Append("<tr>")
                sb2.Append("<td>-")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('LAFARGEE','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bagexternaltotal & "</span>")
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('LAFARGEE','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bulkexternaltotal & "</span>")
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('LAFARGEE','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & baginternaltotal & "</span>")
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('LAFARGEE','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bulkinternaltotal & "</span>")


                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('LAFARGEE','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagall')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bagTotal & "</span>")
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('LAFARGEE','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkall')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bulkTotal & "</span>")



                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('LAFARGEE','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','tipperin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & tipperinternaltotal & "</span>")

                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('LAFARGEE','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','tipperex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & tipperexternaltotal & "</span>")
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('LAFARGEE','" & syear & "','" & smonth & "','0','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & eitotal & "</span>")
                'sb2.Append(eitotal)
                sb2.Append("</td>")
                sb2.Append("</tr>")

                r = excelt.NewRow()
                r(0) = "-"
                r(1) = ""
                r(2) = ""
                r(3) = bagexternaltotal
                r(4) = bulkexternaltotal
                r(5) = baginternaltotal
                r(6) = bulkinternaltotal
                r(7) = bagTotal
                r(8) = bulkTotal
                r(9) = tipperinternaltotal
                r(10) = tipperexternaltotal
                r(11) = eitotal
                excelt.Rows.Add(r)


                sb2.Append("<tr>")
                sb2.Append("<td>-")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td>")
                sb2.Append("</tr>")

                r = excelt.NewRow()
                r(0) = "-"
                r(1) = ""
                r(2) = ""
                r(3) = ""
                r(4) = ""
                r(5) = ""
                r(6) = ""
                r(7) = ""
                r(8) = ""
                r(9) = ""
                r(10) = ""
                r(11) = ""
                excelt.Rows.Add(r)


                'Flyash
                bagexternaltotal = 0
                baginternaltotal = 0
                bulkexternaltotal = 0
                bulkinternaltotal = 0
                bagTotal = 0
                bulkTotal = 0
                tipperinternaltotal = 0
                tipperexternaltotal = 0
                eitotal = 0
                For i As Integer = 0 To t.Rows.Count - 1
                    If FlyashPlant.Contains(t.Rows(i)(1)) Then
                        sb2.Append("<tr>")
                        sb2.Append("<td>")
                        sb2.Append(counter)
                        sb2.Append("</td><td>")
                        sb2.Append(t.Rows(i)(1))
                        sb2.Append("</td><td>")
                        sb2.Append(t.Rows(i)(2))
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(3) & "</span>")
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(4) & "</span>")



                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(5) & "</span>")
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(6) & "</span>")

                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagall')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(7) & "</span>")
                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkall')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(8) & "</span>")






                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','tipperex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(10) & "</span>")

                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','tipperin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(9) & "</span>")


                        sb2.Append("</td><td>")
                        sb2.Append("<span onclick=""javascript:DrilldownToTransporter('" & t.Rows(i)(1) & "','" & syear & "','" & smonth & "','0','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & t.Rows(i)(11) & "</span>")
                        sb2.Append("</td>")
                        sb2.Append("</tr>")
                        bagexternaltotal += t.Rows(i)(3)
                        bulkexternaltotal += t.Rows(i)(4)
                        baginternaltotal += t.Rows(i)(5)
                        bulkinternaltotal += t.Rows(i)(6)

                        bagTotal = bagTotal + t.Rows(i)(3) + t.Rows(i)(5)
                        bulkTotal = bulkTotal + t.Rows(i)(4) + t.Rows(i)(6)

                        tipperexternaltotal += t.Rows(i)(10)
                        tipperinternaltotal += t.Rows(i)(9)

                        eitotal += t.Rows(i)(11)

                        r = excelt.NewRow()
                        r(0) = counter
                        r(1) = t.Rows(i)(1)
                        r(2) = t.Rows(i)(2)
                        r(3) = t.Rows(i)(3)
                        r(4) = t.Rows(i)(4)
                        r(5) = t.Rows(i)(5)
                        r(6) = t.Rows(i)(6)
                        r(7) = t.Rows(i)(7)
                        r(8) = t.Rows(i)(8)
                        r(9) = t.Rows(i)(9)
                        r(10) = t.Rows(i)(10)
                        r(11) = t.Rows(i)(11)
                        excelt.Rows.Add(r)

                        counter += 1
                    End If
                Next
                sb2.Append("<tr>")
                sb2.Append("<td>-")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ASHALL','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bagexternaltotal & "</span>")
                'sb2.Append(bagexternaltotal)
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ASHALL','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bulkexternaltotal & "</span>")
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ASHALL','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & baginternaltotal & "</span>")

                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ASHALL','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bulkinternaltotal & "</span>")

                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ASHALL','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','bagin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bagTotal & "</span>")

                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ASHALL','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','bulkin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bulkTotal & "</span>")



                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ASHALL','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','tipperex')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & tipperexternaltotal & "</span>")

                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ASHALL','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','tipperin')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & tipperinternaltotal & "</span>")



                'sb2.Append(baginternaltotal)
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ASHALL','" & syear & "','" & smonth & "','0','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & eitotal & "</span>")
                'sb2.Append(eitotal)
                sb2.Append("</td>")
                sb2.Append("</tr>")

                r = excelt.NewRow()
                r(0) = "-"
                r(1) = ""
                r(2) = ""
                r(3) = bagexternaltotal
                r(4) = bulkexternaltotal
                r(5) = baginternaltotal
                r(6) = bulkinternaltotal
                r(7) = bagTotal
                r(8) = bulkTotal
                r(9) = tipperinternaltotal
                r(10) = tipperexternaltotal
                r(11) = eitotal
                excelt.Rows.Add(r)



                sb2.Append("<tr>")
                sb2.Append("<td>-")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td>")
                sb2.Append("</tr>")

                r = excelt.NewRow()
                r(0) = "-"
                r(1) = ""
                r(2) = ""
                r(3) = ""
                r(4) = ""
                r(5) = ""
                r(6) = ""
                r(7) = ""
                r(8) = ""
                r(9) = ""
                r(10) = ""
                r(11) = ""
                excelt.Rows.Add(r)

                baggrandexternaltotal = 0
                baggrandinternaltotal = 0
                bulkgrandexternaltotal = 0
                bulkgrandinternaltotal = 0
                tippergrandexternaltotal = 0
                tippergrandinternaltotal = 0

                grandBagTotal = 0
                grandBulkTotal = 0

                grandeitotal = 0

                For i As Integer = 0 To t.Rows.Count - 1
                    baggrandexternaltotal += t.Rows(i)(3)
                    baggrandinternaltotal += t.Rows(i)(5)
                    bulkgrandexternaltotal += t.Rows(i)(4)
                    bulkgrandinternaltotal += t.Rows(i)(6)

                    grandBagTotal += t.Rows(i)(7)
                    grandBulkTotal += t.Rows(i)(8)

                    tippergrandinternaltotal += t.Rows(i)(9)
                    tippergrandexternaltotal += t.Rows(i)(10)

                    grandeitotal += t.Rows(i)(11)
                Next

                sb2.Append("<tr>")
                sb2.Append("<td>Grand Total")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")

                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ALL','" & syear & "','" & smonth & "','2','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & baggrandexternaltotal & "</span>")
                'sb2.Append(grandexternaltotal)
                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ALL','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bulkgrandexternaltotal & "</span>")



                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ALL','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & baggrandinternaltotal & "</span>")

                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ALL','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & bulkgrandinternaltotal & "</span>")

                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ALL','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & grandBagTotal & "</span>")

                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ALL','" & syear & "','" & smonth & "','1','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & grandBulkTotal & "</span>")


                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ALL','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & tippergrandexternaltotal & "</span>")

                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ALL','" & syear & "','" & smonth & "','3','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & tippergrandinternaltotal & "</span>")

                sb2.Append("</td><td>")
                sb2.Append("<span onclick=""javascript:DrilldownToTransporter('ALL','" & syear & "','" & smonth & "','0','" & ddlvehicletype.SelectedValue & "','all')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to transporter information."">" & grandeitotal & "</span>")

                sb2.Append("</td>")
                sb2.Append("</tr>")

                r = excelt.NewRow()
                r(0) = "Grand Total"
                r(1) = ""
                r(2) = ""
                r(3) = baggrandexternaltotal
                r(4) = bulkgrandexternaltotal
                r(5) = baggrandinternaltotal
                r(6) = bulkgrandinternaltotal
                r(7) = grandBagTotal
                r(8) = grandBulkTotal
                r(9) = tippergrandinternaltotal
                r(10) = tippergrandinternaltotal
                r(11) = grandeitotal
                excelt.Rows.Add(r)


                sb2.Append("<tr>")
                sb2.Append("<td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>Percentage(%)")
                sb2.Append("</td><td>")
                sb2.Append((CDbl(baggrandexternaltotal / grandBagTotal) * 100).ToString("0.00"))
                sb2.Append("</td><td>")
                sb2.Append((CDbl(bulkgrandexternaltotal / grandBulkTotal) * 100).ToString("0.00"))

                sb2.Append("</td><td>")
                sb2.Append((CDbl(baggrandinternaltotal / grandBagTotal) * 100).ToString("0.00"))
                sb2.Append("</td><td>")
                sb2.Append((CDbl(bulkgrandinternaltotal / grandBulkTotal) * 100).ToString("0.00"))

                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                sb2.Append("</td><td>")
                If grandeitotal > 0 Then
                    sb2.Append((CDbl(tippergrandexternaltotal / grandeitotal) * 100).ToString("0.00"))
                End If

                sb2.Append("</td><td>")
                If grandeitotal > 0 Then
                    sb2.Append((CDbl(tippergrandinternaltotal / grandeitotal) * 100).ToString("0.00"))
                End If

                sb2.Append("</td><td>")
                sb2.Append("</td>")
                sb2.Append("</tr>")

                r = excelt.NewRow()
                r(0) = ""
                r(1) = ""
                r(2) = ""
                r(3) = (CDbl(baggrandexternaltotal / grandBagTotal) * 100).ToString("0.00")
                r(4) = (CDbl(bulkgrandexternaltotal / grandBulkTotal) * 100).ToString("0.00")
                r(5) = (CDbl(baggrandinternaltotal / grandBagTotal) * 100).ToString("0.00")
                r(6) = (CDbl(bulkgrandinternaltotal / grandBulkTotal) * 100).ToString("0.00")
                r(7) = ""
                r(8) = ""
                If grandeitotal > 0 Then
                    r(9) = (CDbl(tippergrandinternaltotal / grandeitotal) * 100).ToString("0.00")
                    r(10) = (CDbl(tippergrandexternaltotal / grandeitotal) * 100).ToString("0.00")
                Else
                    r(9) = "0"
                    r(10) = "0"
                End If

                r(11) = ""
                excelt.Rows.Add(r)


                sb2.Append("</tbody>")
                sb2.Append("<tfoot><tr align=""left""><th>S No</th><th>Plant Code</th><th>Plant</th><th colspan=""2"">External Transporter</th><th colspan=""2"">Own Transporter</th><th colspan=""2""></th><th  colspan=""2"">Tippers</th><th>Total</th></tr></tfoot></table>")
            Catch ex As Exception
                Response.Write(ex.Message & "-" & ex.StackTrace)
            Finally
                If conn2.State = ConnectionState.Open Then
                    conn2.Close()
                End If

            End Try
            Session.Remove("exceltable")
            Session("exceltable") = excelt
            ec = "True"
            'End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub


    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetData(ByVal Plantid As String, ByVal year As String, ByVal month As String, ByVal type As String, ByVal vehicletype As String, ByVal protype As String, ByVal bh As String, ByVal bm As String, ByVal eh As String, ByVal em As String) As String
        Dim sbNew As New StringBuilder()
        Dim esbNew As New StringBuilder()
        Dim begindatetime As String = year & "/" & month & "/01 " & bh & ":" & bm & ":00"
        Dim enddatetime As String = year & "/" & month & "/" & DateTime.DaysInMonth(year, month) & " " & eh & ":" & em & ":59"
        Dim res As String = ""
        Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim errorString As String = ""

        Dim externalbit, internalbit As Boolean
        externalbit = False
        internalbit = False

        Try
            Select Case (Plantid.ToUpper())
                Case "YTLALL"
                    Plantid = "YTLALL"
                Case "LAFARGEE"
                    Plantid = "LAFARGEE"
                Case "ASHALL"
                    Plantid = "ASHALL"
                Case "ExALL"
                    Plantid = "ALL"
                Case "InALL"
                    Plantid = "ALL"
                Case "ALL"
                    Plantid = "ALL"
                    'Case Plantid.ToUpper().IndexOf("ALL") >= 0
                    '    Plantid = "ALL"
                Case Else
                    Plantid = Plantid
            End Select
            Dim da As New SqlDataAdapter()
            Dim ds As New DataSet
            Dim cmd2 As New SqlCommand("sp_apkDelivery_new", conn2)
            cmd2.CommandType = CommandType.StoredProcedure
            cmd2.Parameters.AddWithValue("@SS", Plantid)
            cmd2.Parameters.AddWithValue("@fromdate", begindatetime) 'Convert.ToDateTime(begindatetime).ToString("yyyy/MM/dd"))
            cmd2.Parameters.AddWithValue("@todate", enddatetime) 'Convert.ToDateTime(enddatetime).ToString("yyyy/MM/dd"))
            cmd2.Parameters.AddWithValue("@itype", "0")
            da.SelectCommand = cmd2
            da.Fill(ds, "External")
            If ds.Tables.Count > 0 Then
                externalbit = True
            End If

            Dim cmd As New SqlCommand("sp_apkDelivery_new", conn2)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@SS", Plantid)
            cmd.Parameters.AddWithValue("@fromdate", begindatetime) 'Convert.ToDateTime(begindatetime).ToString("yyyy/MM/dd"))
            cmd.Parameters.AddWithValue("@todate", enddatetime) ' Convert.ToDateTime(enddatetime).ToString("yyyy/MM/dd"))
            cmd.Parameters.AddWithValue("@itype", "1")
            da.SelectCommand = cmd
            da.Fill(ds, "Internal")
            If externalbit Then
                If ds.Tables.Count > 1 Then
                    internalbit = True
                Else
                    internalbit = False
                End If
            Else
                If ds.Tables.Count > 0 Then
                    internalbit = True
                Else
                    internalbit = False
                End If
            End If

            sbNew.Clear()
            sbNew.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples1"" style=""font-size: 13px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
            sbNew.Append("<thead><tr> ")

            esbNew.Clear()
            esbNew.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples2"" style=""font-size: 13px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
            esbNew.Append("<thead><tr> ")

            Try
                If externalbit Then
                    For i As Integer = 0 To ds.Tables("External").Columns.Count - 1
                        esbNew.Append("<th>")
                        If i > 1 Then
                            esbNew.Append(Convert.ToDateTime(ds.Tables("External").Columns(i).ColumnName).ToString("dd"))
                        Else
                            If i = 1 Then
                                esbNew.Append("Type")
                            Else
                                esbNew.Append(ds.Tables("External").Columns(i).ColumnName)
                            End If

                        End If

                        esbNew.Append("</th>")
                    Next
                Else
                    esbNew.Append("<th>SNo</th>")
                    esbNew.Append("<th>Type</th>")
                    For i As Int16 = 1 To DateTime.DaysInMonth(year, month)
                        esbNew.Append("<th>" & i & "</th>")
                    Next
                End If

            Catch ex As Exception
                '  sbNew.Append("Local:" & ex.Message & "-" & ex.StackTrace)
            End Try
            If internalbit Then
                For i As Integer = 0 To ds.Tables("Internal").Columns.Count - 1
                    sbNew.Append("<th>")
                    If i > 1 Then
                        sbNew.Append(Convert.ToDateTime(ds.Tables("Internal").Columns(i).ColumnName).ToString("dd"))
                    Else
                        If i = 1 Then
                            sbNew.Append("Type")
                        Else
                            sbNew.Append(ds.Tables("Internal").Columns(i).ColumnName)
                        End If

                    End If
                    sbNew.Append("</th>")
                Next
            Else
                sbNew.Append("<th>SNo</th>")
                sbNew.Append("<th>Type</th>")
                For i As Int16 = 1 To DateTime.DaysInMonth(year, month)
                    sbNew.Append("<th>" & i & "</th>")
                Next
            End If

            sbNew.Append("</thead><tbody>")
            esbNew.Append("</thead><tbody>")

            Dim isTipper As Boolean = False
            Dim isTanker As Boolean = False
            Dim isCargo As Boolean = False
            Dim enewr As Integer = 0
            Dim newr As Integer = 0
            Dim arr As New ArrayList
            arr.Add("Tanker")
            arr.Add("Cargo")
            arr.Add("Tipper")
            Dim internalDict As New Dictionary(Of String, String)
            Dim externalDict As New Dictionary(Of String, String)
            Try
                If externalbit Then
                    For r As Integer = 0 To ds.Tables("External").Rows.Count - 1
                        enewr = r
                        esbNew.Append("<tr>")
                        For c As Integer = 0 To ds.Tables("External").Columns.Count - 1
                            esbNew.Append("<td>")
                            If (r = 1 Or r = 2 Or r = 0) And c = 1 Then
                                If ds.Tables("External").Rows(r)(c).ToString() = "1" Then
                                    esbNew.Append("<span onclick="" javascript:DrilldownToVehicle('Tanker','Tanker')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to Vehicle information."">Tanker</span>")
                                    If Not externalDict.ContainsKey("Tanker") Then
                                        externalDict("Tanker") = "Tanker"
                                    End If
                                ElseIf ds.Tables("External").Rows(r)(c).ToString() = "2" Then
                                    esbNew.Append("<span onclick="" javascript:DrilldownToVehicle('Cargo','Cargo')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to Vehicle information."">Cargo</span>")
                                    If Not externalDict.ContainsKey("Cargo") Then
                                        externalDict("Cargo") = "Cargo"
                                    End If
                                ElseIf ds.Tables("External").Rows(r)(c).ToString() = "3" Then
                                    esbNew.Append("<span onclick="" javascript:DrilldownToVehicle('Tipper','Tipper')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to Vehicle information."">Tipper</span>")
                                    If Not externalDict.ContainsKey("Tipper") Then
                                        externalDict("Tipper") = "Tipper"
                                    End If
                                End If
                            Else
                                esbNew.Append(ds.Tables("External").Rows(r)(c).ToString())
                            End If
                            esbNew.Append("</td>")
                        Next
                        esbNew.Append("</tr>")
                    Next
                Else
                    For t As Int16 = 0 To 2
                        esbNew.Append("<tr>")
                        esbNew.Append("<td>" & t + 1 & "</td>")
                        If t = 0 Then
                            esbNew.Append("<td>Tanker</td>")
                        ElseIf t = 1 Then
                            esbNew.Append("<td>Cargo</td>")
                        Else
                            esbNew.Append("<td>Tipper</td>")
                        End If
                        For i As Int16 = 1 To DateTime.DaysInMonth(year, month)
                            esbNew.Append("<td>0</td>")
                        Next
                        esbNew.Append("</tr>")
                    Next

                End If

            Catch ex As Exception

            End Try

            If internalbit Then
                For r As Integer = 0 To ds.Tables("Internal").Rows.Count - 1
                    sbNew.Append("<tr>")
                    newr = r
                    For c As Integer = 0 To ds.Tables("Internal").Columns.Count - 1
                        sbNew.Append("<td>")
                        If (r = 1 Or r = 2 Or r = 0) And c = 1 Then
                            If ds.Tables("Internal").Rows(r)(c).ToString() = "1" Then
                                sbNew.Append("<span onclick="" javascript:DrilldownToVehicle('Tanker','Tanker')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to Vehicle information."">Tanker</span>")
                                If Not internalDict.ContainsKey("Tanker") Then
                                    internalDict("Tanker") = "Tanker"
                                End If
                            ElseIf ds.Tables("Internal").Rows(r)(c).ToString() = "2" Then
                                sbNew.Append("<span onclick="" javascript:DrilldownToVehicle('Cargo','Cargo')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to Vehicle information."">Cargo</span>")
                                If Not internalDict.ContainsKey("Cargo") Then
                                    internalDict("Cargo") = "Cargo"
                                End If
                            ElseIf ds.Tables("Internal").Rows(r)(c).ToString() = "3" Then
                                esbNew.Append("<span onclick="" javascript:DrilldownToVehicle('Tipper','Tipper')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to Vehicle information."">Tipper</span>")
                                If Not internalDict.ContainsKey("Tipper") Then
                                    internalDict("Tipper") = "Tipper"
                                End If
                            End If
                        Else
                            sbNew.Append(ds.Tables("Internal").Rows(r)(c).ToString())
                        End If

                        sbNew.Append("</td>")
                    Next
                    sbNew.Append("</tr>")
                Next
            Else
                For t As Int16 = 0 To 2
                    sbNew.Append("<tr>")
                    sbNew.Append("<td>" & t + 1 & "</td>")
                    If t = 0 Then
                        sbNew.Append("<td>Tanker</td>")
                    ElseIf t = 1 Then
                        sbNew.Append("<td>Cargo</td>")
                    Else
                        sbNew.Append("<td>Tipper</td>")
                    End If
                    For i As Int16 = 1 To DateTime.DaysInMonth(year, month)
                        sbNew.Append("<td>0</td>")
                    Next
                    sbNew.Append("</tr>")
                Next
            End If



            newr += 1
            enewr += 1
            For ii As Integer = 0 To arr.Count - 1
                If internalbit Then
                    If Not internalDict.Keys.Contains(arr(ii)) Then
                        sbNew.Append("<tr>")
                        sbNew.Append("<td>" & newr + 1 & "</td><td>")
                        sbNew.Append("<span onclick=""javascript: DrilldownToVehicle('" & arr(ii) & "','" & arr(ii) & "')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to Vehicle information."">" & arr(ii) & "</span>")
                        sbNew.Append("</td>")

                        For i As Int16 = 2 To ds.Tables("Internal").Columns.Count - 1
                            sbNew.Append("<td>0</td>")
                            'excelrow(excelrowcount) = 0
                            ' excelrowcount += 1
                        Next
                        '   excelrow(2) = transportertotal
                        sbNew.Append("</tr>")
                        'exceltable.Rows.Add(excelrow)
                        newr += 1
                    End If
                End If
                If externalbit Then
                    If Not externalDict.Keys.Contains(arr(ii)) Then
                        esbNew.Append("<tr>")
                        esbNew.Append("<td>" & enewr + 1 & "</td><td>")
                        esbNew.Append("<span onclick=""javascript: DrilldownToVehicle('" & arr(ii) & "','" & arr(ii) & "')"" style=""cursor:pointer;text-decoration: underline; color: #000080;font-weight:bold;"" title=""Drilldown to Vehicle information."">" & arr(ii) & "</span>")
                        esbNew.Append("</td>")

                        For i As Int16 = 2 To ds.Tables("External").Columns.Count - 1
                            esbNew.Append("<td>0</td>")
                        Next

                        esbNew.Append("</tr>")
                        enewr += 1
                    End If
                End If

            Next

            esbNew.Append("</tbody><tfoot><tr><td></td><td>Total</td>")
            sbNew.Append("</tbody><tfoot><tr><td></td><td>Total</td>")

            Dim total As Integer = 0
            If internalbit Then
                For c As Integer = 2 To ds.Tables("Internal").Columns.Count - 1
                    total = 0
                    For r As Integer = 0 To ds.Tables("Internal").Rows.Count - 1
                        If IsNumeric(ds.Tables("Internal").Rows(r)(c).ToString()) Then
                            total += Convert.ToInt32(ds.Tables("Internal").Rows(r)(c).ToString())
                        End If
                    Next
                    sbNew.Append("<td>")
                    sbNew.Append(total)
                    sbNew.Append("</td>")
                Next
            End If

            If externalbit Then
                For c As Integer = 2 To ds.Tables("External").Columns.Count - 1
                    total = 0
                    For r As Integer = 0 To ds.Tables("External").Rows.Count - 1
                        Try
                            If IsNumeric(ds.Tables("External").Rows(r)(c).ToString()) Then
                                total += Convert.ToInt32(ds.Tables("External").Rows(r)(c).ToString())
                            End If
                        Catch ex As Exception
                            esbNew.Append(ex.Message)
                        End Try
                    Next
                    esbNew.Append("<td>")
                    esbNew.Append(total)
                    esbNew.Append("</td>")
                Next
            End If


            esbNew.Append("</tr></tfoot></table>")
            sbNew.Append("</tr></tfoot></table>")
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session("exceltable") = ds.Tables(0)
        Catch ex As Exception
            ' sbNew.Append(ex.Message & "-" & ex.StackTrace)
        Finally
            If conn2.State = ConnectionState.Open Then
                conn2.Close()
            End If
        End Try

        res = sbNew.ToString() & "APK" & esbNew.ToString() ' rrorString & ressb.ToString() & "APK" & ressbExt.ToString()
        Return res
    End Function

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetDataTruck(ByVal Plantid As String, ByVal year As String, ByVal month As String, ByVal type As String, ByVal vehicletype As String, ByVal Transporterid As String, ByVal bh As String, ByVal bm As String, ByVal eh As String, ByVal em As String) As String
        Dim ressb As New StringBuilder()
        Dim begindatetime As String = year & "/" & month & "/01 " & bh & ":" & bm & ":00"
        Dim enddatetime As String = year & "/" & month & "/" & DateTime.DaysInMonth(year, month) & " " & eh & ":" & em & ":59"
        Dim res As String = ""
        Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim cmd As New SqlCommand()
        Dim selectioncmd As String = ""
        Dim plantcmd As String = ""
        Dim truckcmd As String = ""
        Dim errorstring As String = ""
        Dim trucktypecmd As String = ""
        Dim PlatenoDict As New Dictionary(Of Platekey, List(Of PlatenoRes))

        Dim TransporterDictTotal As New Dictionary(Of Int32, Int32)
        ressb.Append("No Data")
        Try
            Select Case (Plantid.ToUpper())
                Case "YTLALL"
                    Plantid = "YTLALL"
                Case "LAFARGEE"
                    Plantid = "LAFARGEE"
                Case "ASHALL"
                    Plantid = "ASHALL"
                Case "ExALL"
                    Plantid = "ALL"
                Case "InALL"
                    Plantid = "ALL"
                Case "ALL"
                    Plantid = "ALL"
                    'Case Plantid.ToUpper().IndexOf("ALL") >= 0
                    '    Plantid = "ALL"
                Case Else
                    Plantid = Plantid
            End Select

            Dim filterdata As String = ""
            cmd.Connection = conn2
            If Transporterid.ToUpper() = "TANKER" Then
                filterdata = "1"
            ElseIf Transporterid.ToUpper() = "CARGO" Then
                filterdata = "2"
            ElseIf Transporterid.ToUpper() = "TIPPER" Then
                filterdata = "3"
            Else
                filterdata = "4"
            End If

            Dim da As New SqlDataAdapter()
            Dim ds As New DataSet
            Dim cmd2 As New SqlCommand("sp_vvkDeliveryByTrucknew", conn2)
            cmd2.CommandType = CommandType.StoredProcedure
            cmd2.Parameters.AddWithValue("@SS", Plantid)
            cmd2.Parameters.AddWithValue("@fromdate", begindatetime)
            cmd2.Parameters.AddWithValue("@todate", enddatetime)
            cmd2.Parameters.AddWithValue("@producttype", filterdata)
            da.SelectCommand = cmd2

            da.Fill(ds, "External")
            Dim Tres As PlatenoRes
            Dim platekey As New Platekey
            Dim reslistt As List(Of PlatenoRes)
            Dim temprres As PlatenoRes

            For r As Integer = 0 To ds.Tables("External").Rows.Count - 1
                platekey = New Platekey
                platekey.plateno = ds.Tables("External").Rows(r)(2)
                platekey.transporter = ds.Tables("External").Rows(r)(4)
                platekey.username = ds.Tables("External").Rows(r)(3)
                platekey.pmid = ds.Tables("External").Rows(r)(5)
                'platekey.type = ds.Tables("External").Rows(r)(7)


                If ds.Tables("External").Rows(r)(6).ToString() = "1" Then
                    platekey.type = "Tanker"
                ElseIf ds.Tables("External").Rows(r)(6).ToString() = "2" Then
                    platekey.type = "Cargo"
                Else
                    platekey.type = "Tipper"
                End If

                If PlatenoDict.ContainsKey(platekey) Then
                    reslistt = PlatenoDict(platekey)
                    Tres = New PlatenoRes
                    Tres.count = ds.Tables("External").Rows(r)(0)
                    Tres.truck = ds.Tables("External").Rows(r)(2)
                    Tres.daydate = Convert.ToDateTime(ds.Tables("External").Rows(r)(1)).Day
                    reslistt.Add(Tres)
                    PlatenoDict.Remove(platekey)
                    PlatenoDict.Add(platekey, reslistt)
                Else
                    Tres = New PlatenoRes
                    Tres.count = ds.Tables("External").Rows(r)(0)
                    Tres.truck = ds.Tables("External").Rows(r)(2)
                    Tres.daydate = Convert.ToDateTime(ds.Tables("External").Rows(r)(1)).Day
                    reslistt = New List(Of PlatenoRes)
                    reslistt.Add(Tres)
                    PlatenoDict.Add(platekey, reslistt)
                End If
            Next










            Dim exceltable As New DataTable()
            exceltable.Columns.Add(New DataColumn("S No"))
            exceltable.Columns.Add(New DataColumn("Username"))
            exceltable.Columns.Add(New DataColumn("Transporter"))
            exceltable.Columns.Add(New DataColumn("Plateno"))
            exceltable.Columns.Add(New DataColumn("PMID"))
            exceltable.Columns.Add(New DataColumn("Type"))
            'exceltable.Columns.Add(New DataColumn("Date/Total"))
            Dim excelrow As DataRow
            ressb.Clear()
            ressb.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples3"" style=""font-size: 13px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
            ressb.Append("<thead><tr align=""left""><th>S No</th><th>Username</th><th>Transporter</th><th>Plateno</th><th>PM ID</th><th>Type</th>")

            Dim DatesDict As New Dictionary(Of String, String)

            'If DateTime.Now.Month = month Then
            '    For i As Int16 = 1 To DateTime.Now.Day
            '        ressb.Append("<th>" & i & "</th>")
            '        DatesDict.Add(i, i)
            '        exceltable.Columns.Add(New DataColumn("" & i & ""))
            '    Next
            'Else
            '    For i As Int16 = 1 To DateTime.DaysInMonth(year, month)
            '        ressb.Append("<th>" & i & "</th>")
            '        DatesDict.Add(i, i)
            '        exceltable.Columns.Add(New DataColumn("" & i & ""))
            '    Next
            'End If

            For i As Int16 = 1 To DateTime.DaysInMonth(year, month)
                ressb.Append("<th>" & i & "</th>")
                exceltable.Columns.Add(New DataColumn("" & i & ""))
            Next

            'For i As Integer = 6 To ds.Tables("External").Columns.Count - 1
            '    ressb.Append("<th>")
            '    ressb.Append(Convert.ToDateTime(ds.Tables("External").Columns(i).ColumnName).ToString("dd"))

            '    ressb.Append("</th>")
            '    exceltable.Columns.Add(New DataColumn("" & i & ""))
            'Next
            ressb.Append("</thead><tbody>")


            Dim counter As Int32 = 1
            Dim flag As Boolean = False

            Dim temptotal As Int32
            Dim excelrowcount As Int32 = 0
            Dim transportertotal As Int32 = 0
            For Each keyvl As KeyValuePair(Of Platekey, List(Of PlatenoRes)) In PlatenoDict
                ressb.Append("<tr>")

                ressb.Append("<td>" & counter & "</td>")
                ressb.Append("<td>" & keyvl.Key.username & "</td>")
                ressb.Append("<td>" & keyvl.Key.transporter & "</td>")
                ressb.Append("<td>" & keyvl.Key.plateno & "</td>")
                ressb.Append("<td>" & keyvl.Key.pmid & "</td>")
                ressb.Append("<td>" & keyvl.Key.type & "</td>")
                'ressb.Append("<td> </td>")
                excelrow = exceltable.NewRow()
                excelrow(0) = counter
                excelrow(1) = keyvl.Key.username
                excelrow(2) = keyvl.Key.transporter
                excelrow(3) = keyvl.Key.plateno
                excelrow(4) = keyvl.Key.pmid
                excelrow(5) = keyvl.Key.type
                reslistt = keyvl.Value
                excelrowcount = 6
                For i As Int16 = 1 To DateTime.DaysInMonth(year, month)
                    temprres = Nothing
                    For Each t As PlatenoRes In reslistt
                        If t.daydate = i Then
                            flag = True
                            temprres = t
                            Exit For
                        End If
                    Next
                    If flag Then
                        ressb.Append("<td>" & temprres.count & "</td>")
                        excelrow(excelrowcount) = temprres.count
                        transportertotal += temprres.count
                        If TransporterDictTotal.ContainsKey(temprres.daydate) Then
                            temptotal = TransporterDictTotal(temprres.daydate)
                            TransporterDictTotal.Remove(temprres.daydate)
                            TransporterDictTotal.Add(temprres.daydate, temprres.count + temptotal)
                        Else
                            TransporterDictTotal.Add(temprres.daydate, temprres.count)
                        End If
                        excelrowcount += 1
                    Else
                        ressb.Append("<td>0</td>")
                        excelrow(excelrowcount) = 0
                        excelrowcount += 1
                    End If
                Next
                ressb.Append("</tr>")
                exceltable.Rows.Add(excelrow)
                counter += 1
            Next




            'For r As Integer = 0 To ds.Tables("External").Rows.Count - 1
            '    ressb.Append("<tr>")
            '    excelrow = exceltable.NewRow()
            '    excelrow(0) = r
            '    excelrow(1) = ds.Tables("External").Rows(r)(1)
            '    excelrow(2) = ds.Tables("External").Rows(r)(2)
            '    excelrow(3) = ds.Tables("External").Rows(r)(3)
            '    excelrow(4) = ds.Tables("External").Rows(r)(4)

            '    For c As Integer = 0 To ds.Tables("External").Columns.Count - 1
            '        ressb.Append("<td>")
            '        If c = 5 Then
            '            If ds.Tables("External").Rows(r)(c).ToString() = "1" Then
            '                ressb.Append("Tanker")
            '                excelrow(5) = "Tanker"
            '            ElseIf ds.Tables("External").Rows(r)(c).ToString() = "2" Then
            '                ressb.Append("Cargo")
            '                excelrow(5) = "Cargo"
            '            ElseIf ds.Tables("External").Rows(r)(c).ToString() = "3" Then
            '                ressb.Append("Tipper")
            '                excelrow(5) = "Tipper"
            '            Else
            '                ressb.Append("--")
            '            End If
            '        Else
            '            ressb.Append(ds.Tables("External").Rows(r)(c).ToString())
            '        End If
            '        ressb.Append("</td>")
            '    Next
            '    ressb.Append("</tr>")

            'Next
            ressb.Append("</tbody>")
            ressb.Append("<tfoot><tr><td></td><td></td><td></td><td></td><td></td><td>Total</td>")
            Dim total As Integer = 0

            For i As Int16 = 1 To DateTime.DaysInMonth(year, month)
                If TransporterDictTotal.ContainsKey(i) Then
                    ressb.Append("<td>" & TransporterDictTotal(i) & "</td>")
                Else
                    ressb.Append("<td>0</td>")
                End If
            Next
            ressb.Append("</tr></tfoot></table>")
            HttpContext.Current.Session.Remove("exceltable")
            Try
                Dim dv As DataView = exceltable.DefaultView
                HttpContext.Current.Session("exceltable") = dv.ToTable()
            Catch ex As Exception
                HttpContext.Current.Session("exceltable") = exceltable
            End Try

        Catch ex As Exception
            ressb.Append("1." & ex.Message & ":" & ex.StackTrace)
        Finally
            If conn2.State = ConnectionState.Open Then
                conn2.Close()
            End If
        End Try

        res = ressb.ToString()
        Return res
    End Function

    '<System.Web.Services.WebMethod()>
    '<ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    'Public Shared Function GetDataTruck(ByVal Plantid As String, ByVal year As String, ByVal month As String, ByVal type As String, ByVal vehicletype As String, ByVal Transporterid As String, ByVal bh As String, ByVal bm As String, ByVal eh As String, ByVal em As String) As String
    '    Dim ressb As New StringBuilder()
    '    Dim begindatetime As String = year & "/" & month & "/01 " & bh & ":" & bm & ":00"
    '    Dim enddatetime As String = year & "/" & month & "/" & DateTime.DaysInMonth(year, month) & " " & eh & ":" & em & ":59"
    '    Dim res As String = ""
    '    Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
    '    Dim cmd As New SqlCommand()
    '    Dim selectioncmd As String = ""
    '    Dim plantcmd As String = ""
    '    Dim truckcmd As String = ""
    '    Dim errorstring As String = ""
    '    Dim trucktypecmd As String = ""
    '    Dim PlatenoDict As New Dictionary(Of String, List(Of PlatenoRes))
    '    Dim TransporterDictTotal As New Dictionary(Of Int32, Int32)
    '    ressb.Append("No Data")
    '    Try
    '        Dim filterdata As String = ""
    '        cmd.Connection = conn2
    '        If Transporterid.ToUpper() = "TANKER" Then
    '            filterdata = "1"
    '        ElseIf Transporterid.ToUpper() = "CARGO" Then
    '            filterdata = "2"
    '        ElseIf Transporterid.ToUpper() = "TIPPER" Then
    '            filterdata = "3"
    '        Else
    '            filterdata = "4"
    '        End If

    '        Dim exceltable As New DataTable()
    '        exceltable.Columns.Add(New DataColumn("S No"))
    '        exceltable.Columns.Add(New DataColumn("Username"))
    '        exceltable.Columns.Add(New DataColumn("Transporter"))
    '        exceltable.Columns.Add(New DataColumn("Plateno"))
    '        exceltable.Columns.Add(New DataColumn("PMID"))
    '        exceltable.Columns.Add(New DataColumn("Type"))
    '        exceltable.Columns.Add(New DataColumn("Date/Total"))
    '        Dim excelrow As DataRow

    '        Select Case (Plantid)
    '            Case "YTLALL"
    '                plantcmd = " and source_supply in('PR','BS','BC','WP','PG3','GPK','LM') "
    '            Case "LAFARGEE"
    '                plantcmd = " and source_supply in('KT','RW','LK','PG1','PG2','WP2')"
    '            Case "ASHALL"
    '                plantcmd = " and source_supply in('KP','TB','MJG','JEV')"
    '            Case "ExALL"
    '                plantcmd = ""
    '            Case "InALL"
    '                plantcmd = ""
    '            Case "ALL"
    '                plantcmd = ""
    '            Case Else
    '                plantcmd = " and source_supply ='" & Plantid & "'"
    '        End Select

    '        cmd.CommandText = "select *,IsNull(ytldb.dbo.fn_getPMIDByPlateno(plateno),'-') as PMID,IsNull(ytldb.dbo.fn_getVehicleTypefromPlateno(plateno),'-') as vType,IsNull(ytldb.dbo.getUsernameByPlatenoNew(plateno),'-') as Username from (select cast(IsNull(tm.internaltype,0) as bit)  internaltype,a.transporter,a.plateno,pm.producttype,count(*) as count,CAST(weight_outtime  AS DATE) as day,a.productcode from (select * from oss_patch_out where weight_outtime  between '" & begindatetime & "' and '" & enddatetime & "' " & plantcmd & " )a left outer join  oss_product_master pm on pm.productid=a.productcode inner join oss_transporter_master tm on tm.transporterid=a.transporter_id   group by CAST(weight_outtime  AS DATE),a.productcode,pm.producttype,tm.internaltype,a.plateno,a.transporter ) apk where producttype ='" & filterdata & "'  order by CAST(day AS DATE)"
    '        errorstring = cmd.CommandText
    '        ' cmd.CommandText = "select CAST(weight_outtime  AS DATE) as day,count(*) as count,plateno  from oss_patch_out where weight_outtime  between '" & begindatetime & "' and '" & enddatetime & "' " & plantcmd & " " & trucktypecmd & " " & selectioncmd & " " & truckcmd & " group by CAST(weight_outtime  AS DATE),plateno  order by CAST(weight_outtime  AS DATE),plateno"
    '        conn2.Open()
    '        Dim dr As SqlDataReader = cmd.ExecuteReader()
    '        Dim Tres As PlatenoRes
    '        Dim reslistt As List(Of PlatenoRes)
    '        While dr.Read()
    '            Try
    '                If PlatenoDict.ContainsKey(dr("plateno")) Then
    '                    reslistt = PlatenoDict(dr("plateno"))
    '                    Tres = New PlatenoRes
    '                    Tres.count = Convert.ToInt32(dr("count"))
    '                    Tres.transporterName = dr("transporter").ToString().ToUpper()
    '                    Tres.PMID = dr("pmid").ToString.ToUpper()
    '                    Tres.username = dr("username").ToString().ToUpper()
    '                    Tres.vType = Transporterid.ToString().ToUpper()
    '                    Tres.truck = dr("plateno").ToString().ToUpper()
    '                    Tres.daydate = Convert.ToDateTime(dr("day")).Day
    '                    reslistt.Add(Tres)
    '                    PlatenoDict.Remove(dr("plateno"))
    '                    PlatenoDict.Add(dr("plateno"), reslistt)
    '                Else
    '                    Tres = New PlatenoRes
    '                    Tres.count = dr("count")
    '                    Tres.truck = dr("plateno")
    '                    Tres.transporterName = dr("transporter").ToString().ToUpper()
    '                    Tres.PMID = dr("pmid").ToString.ToUpper()
    '                    Tres.username = dr("username").ToString().ToUpper()
    '                    Tres.vType = Transporterid.ToString().ToUpper()
    '                    Tres.daydate = Convert.ToDateTime(dr("day")).Day
    '                    reslistt = New List(Of PlatenoRes)
    '                    reslistt.Add(Tres)
    '                    PlatenoDict.Add(dr("plateno"), reslistt)
    '                End If
    '            Catch ex As Exception
    '                ressb.Append("2." & ex.Message)
    '            End Try


    '        End While
    '        ressb.Clear()
    '        ressb.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples3"" style=""font-size: 13px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
    '        ressb.Append("<thead><tr align=""left""><th>S No</th><th>Username</th><th>Transporter</th><th>Plateno</th><th>PM ID</th><th>Type</th>")
    '        ressb.Append("<th> </th>")
    '        For i As Int16 = 1 To DateTime.DaysInMonth(year, month)
    '            ressb.Append("<th>" & i & "</th>")
    '            exceltable.Columns.Add(New DataColumn("" & i & ""))
    '        Next
    '        ressb.Append("</tr></thead><tbody>")
    '        Dim counter As Int32 = 1
    '        Dim flag As Boolean = False
    '        Dim temprres As PlatenoRes
    '        Dim temptotal As Int32
    '        Dim excelrowcount As Int32 = 0
    '        Dim transportertotal As Int32 = 0
    '        For Each keyvl As KeyValuePair(Of String, List(Of PlatenoRes)) In PlatenoDict
    '            Try
    '                excelrow = exceltable.NewRow()
    '                excelrow(0) = counter
    '                transportertotal = 0
    '                ressb.Append("<tr>")
    '                ressb.Append("<td>" & counter & "</td>")
    '                ressb.Append("<td>" & keyvl.Value(0).username & "</td>")
    '                ressb.Append("<td>" & keyvl.Value(0).transporterName & "</td>")
    '                ressb.Append("<td>" & keyvl.Value(0).truck & "</td>")
    '                ressb.Append("<td>" & keyvl.Value(0).PMID & "</td>")
    '                ressb.Append("<td>" & keyvl.Value(0).vType & "</td>")

    '                excelrow(1) = keyvl.Value(0).username
    '                excelrow(2) = keyvl.Value(0).transporterName
    '                excelrow(3) = keyvl.Value(0).truck
    '                excelrow(4) = keyvl.Value(0).PMID
    '                excelrow(5) = keyvl.Value(0).vType
    '                ressb.Append("<td> </td>")


    '                reslistt = keyvl.Value
    '                excelrowcount = 6

    '                For i As Int16 = 1 To DateTime.DaysInMonth(year, month)
    '                    temprres = Nothing
    '                    For Each t As PlatenoRes In reslistt
    '                        If t.daydate = i Then
    '                            flag = True
    '                            temprres = t
    '                            Exit For
    '                        End If
    '                    Next
    '                    If flag Then
    '                        ressb.Append("<td>" & temprres.count & "</td>")
    '                        excelrow(excelrowcount) = temprres.count
    '                        transportertotal += temprres.count
    '                        excelrowcount += 1
    '                        If TransporterDictTotal.ContainsKey(temprres.daydate) Then
    '                            temptotal = TransporterDictTotal(temprres.daydate)
    '                            TransporterDictTotal.Remove(temprres.daydate)
    '                            TransporterDictTotal.Add(temprres.daydate, temprres.count + temptotal)
    '                        Else
    '                            TransporterDictTotal.Add(temprres.daydate, temprres.count)
    '                        End If

    '                    Else
    '                        ressb.Append("<td>0</td>")
    '                        excelrow(excelrowcount) = 0
    '                        excelrowcount += 1
    '                    End If

    '                Next
    '                ressb.Append("</tr>")
    '                excelrow(6) = transportertotal
    '                exceltable.Rows.Add(excelrow)
    '                counter += 1
    '            Catch ex As Exception
    '                ressb.Append("3." & ex.Message)
    '            End Try

    '        Next
    '        excelrow = exceltable.NewRow()
    '        ressb.Append("</tbody>")
    '        ressb.Append("<tfoot><tr>")
    '        excelrow(0) = ""
    '        ressb.Append("<td> </td>")
    '        ressb.Append("<td> </td>")
    '        ressb.Append("<td> </td>")
    '        ressb.Append("<td> </td>")
    '        ressb.Append("<td> </td>")
    '        excelrow(5) = "Total"
    '        ressb.Append("<td>Total</td>")
    '        excelrow(6) = ""
    '        ressb.Append("<td> </td>")
    '        excelrowcount = 6

    '        For i As Int16 = 1 To DateTime.DaysInMonth(year, month)
    '            Try
    '                If TransporterDictTotal.ContainsKey(i) Then
    '                    ressb.Append("<td>" & TransporterDictTotal(i) & "</td>")
    '                    excelrow(excelrowcount) = TransporterDictTotal(i)
    '                    excelrowcount += 1
    '                Else
    '                    ressb.Append("<td>0</td>")
    '                    excelrow(excelrowcount) = 0
    '                    excelrowcount += 1
    '                End If
    '            Catch ex As Exception
    '                ressb.Append("5." & ex.Message)
    '            End Try

    '        Next
    '        exceltable.Rows.Add(excelrow)
    '        ressb.Append("</tr>")

    '        ressb.Append("<tr align=""left""><th>S No</th><th>Username</th><th>Transporter</th><th>Plateno</th><th>PM ID</th><th>Type</th>")
    '        ressb.Append("<th> </th>")
    '        For i As Int16 = 1 To DateTime.DaysInMonth(year, month)
    '            ressb.Append("<th>" & i & "</th>")
    '        Next
    '        ressb.Append("</tr></tfoot></table>")
    '        HttpContext.Current.Session.Remove("exceltable")
    '        Try
    '            Dim dv As DataView = exceltable.DefaultView
    '            dv.Sort = "Transporter ASC"
    '            HttpContext.Current.Session("exceltable") = dv.ToTable()
    '        Catch ex As Exception
    '            HttpContext.Current.Session("exceltable") = exceltable
    '        End Try

    '    Catch ex As Exception
    '        ressb.Append("1." & ex.Message)
    '    Finally
    '        If conn2.State = ConnectionState.Open Then
    '            conn2.Close()
    '        End If
    '    End Try

    '    res = ressb.ToString()
    '    Return res
    'End Function

    Structure TransporterRes
        Dim count As Int32
        Dim Transporter As String
        Dim daydate As Int16
        Dim isInternal As Boolean
    End Structure

    Structure VehiclesTypeDetails
        Dim count As Int32
        Dim dateofthemonth As Int32
    End Structure
    Structure KeyStruct
        Dim vehicleType As String
        Dim dateofthemonth As Int32

    End Structure
    Structure Platekey
        Dim plateno As String
        Dim username As String
        Dim transporter As String
        Dim pmid As String
        Dim type As String

    End Structure
    Structure PlatenoRes
        Dim count As Int32
        Dim truck As String
        Dim daydate As Int16
        Dim transporterName As String
        Dim username As String
        Dim vType As String
        Dim PMID As String
    End Structure
    Structure PlantRes
        Dim baginternal As Int32
        Dim bulkinternal As Int32
        Dim plantname As String
        Dim bagexternal As Int32
        Dim bulkexternal As Int32
        Dim tipperexternal As Int32
        Dim tipperinternal As Int32
    End Structure



    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
        Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"


        DisplayLogInformation(begindatetime, enddatetime)
    End Sub
End Class