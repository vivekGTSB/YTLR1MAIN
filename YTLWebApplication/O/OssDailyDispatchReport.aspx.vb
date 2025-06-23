Imports System.Data.SqlClient
Imports System.Data
Imports ADODB
Imports AspMap
Imports System.IO
Imports System.Windows
Imports DocumentFormat.OpenXml.Drawing.Charts

Public Class OssDailyDispatchReport
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
            ' SECURITY FIX: Check authentication
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim con As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("select * from YTLOSS.dbo.fn_GetAssignedPlants(@userid) order by PV_Plant", con)
            cmd.Parameters.AddWithValue("@userid", userid)
            
            con.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlPlants.Items.Clear()
            While dr.Read()
                ' SECURITY FIX: Use HtmlEncode for output
                ddlPlants.Items.Add(New ListItem(HttpUtility.HtmlEncode(dr("PV_Plant") & " - " & dr("PV_DisplayName")), HttpUtility.HtmlEncode(dr("PV_Plant"))))
            End While
            con.Close()
            
            ddlPlants.Items.Add(New ListItem("PG - PG1 & PG2 & PG3", "PG"))

        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in OssDailyDispatchReport OnInit: " & ex.Message)
            WriteLog("4" & ex.Message)
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Check authentication
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            End If

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in OssDailyDispatchReport Page_Load: " & ex.Message)
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

            ' SECURITY FIX: Use parameterized query
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
                        sb1.Append(HttpUtility.HtmlEncode(t.DefaultView.Item(i)(0)))
                        sb1.Append("</td><td class=""rightalign"">")
                        sb1.Append(HttpUtility.HtmlEncode(t.DefaultView.Item(i)(1)))
                        sb1.Append("</td><td class=""rightalign"">")
                        sb1.Append(HttpUtility.HtmlEncode(t.DefaultView.Item(i)(2)))
                        sb1.Append("</td><td class=""rightalign"">")
                        sb1.Append(HttpUtility.HtmlEncode(t.DefaultView.Item(i)(3)))
                        sb1.Append("</td><td class=""rightalign"">")
                        sb1.Append(HttpUtility.HtmlEncode(t.DefaultView.Item(i)(4)))
                        sb1.Append("</td></tr>")
                        counter += 1
                    Catch ex As Exception
                        ' Log error securely
                        SecurityHelper.LogSecurityEvent("Error in DisplayLogInformation: " & ex.Message)
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
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in DisplayLogInformation: " & ex.Message)
            Response.Write("5" & ex.Message)
        End Try
    End Sub
    
    Protected Function getRegiondata(ByVal plant As String, ByVal bdt As String, ByVal edt As String, ByVal transporter As String, ByVal prodtype As Integer) As regiondata
        Dim region As New regiondata()
        Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Try
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand()
            
            If plant = "PG" Then
                cmd.CommandText = "select sum(qty) as tqty, region from (select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0) else round(dn_qty,0) end qty, transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply in ('PG1','PG2','PG3') and weight_outtime between @bdt and @edt and transporter_id in (select transporterid from oss_transporter_master where internaltype=0) and productcode in (select productid from oss_product_master where producttype=@product) and transporter=@trans and region is not null) as T group by region order by tqty desc"
            Else
                cmd.CommandText = "select sum(qty) as tqty, region from (select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0) else round(dn_qty,0) end qty, transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply=@plant and weight_outtime between @bdt and @edt and transporter_id in (select transporterid from oss_transporter_master where internaltype=0) and productcode in (select productid from oss_product_master where producttype=@product) and transporter=@trans and region is not null) as T group by region order by tqty desc"
                cmd.Parameters.AddWithValue("@plant", plant)
            End If
            
            cmd.Parameters.AddWithValue("@bdt", bdt)
            cmd.Parameters.AddWithValue("@edt", edt)
            cmd.Parameters.AddWithValue("@trans", transporter)
            cmd.Parameters.AddWithValue("@product", prodtype)
            
            cmd.Connection = conn2
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
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in getRegiondata: " & ex.Message)
        Finally
            If conn2.State = ConnectionState.Open Then
                conn2.Close()
            End If
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

            ' SECURITY FIX: Use parameterized query
            If ddlPlants.SelectedValue = "PG" Then
                cmd2.CommandText = "select sum(qty) as tqty, region from (select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0) else round(dn_qty,0) end qty, transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply in('PG1','PG2','PG3') and weight_outtime between @bdt and @edt and transporter_id in (select transporterid from oss_transporter_master where internaltype=1) and productcode in (select productid from oss_product_master where producttype=@ptype) and t2.region is not null) as T group by region order by tqty desc"
            Else
                cmd2.CommandText = "select sum(qty) as tqty, region from (select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0) else round(dn_qty,0) end qty, transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply=@plant and weight_outtime between @bdt and @edt and transporter_id in (select transporterid from oss_transporter_master where internaltype=1) and productcode in (select productid from oss_product_master where producttype=@ptype) and t2.region is not null) as T group by region order by tqty desc"
                cmd2.Parameters.AddWithValue("@plant", plant)
            End If
            
            cmd2.Parameters.AddWithValue("@bdt", bdt)
            cmd2.Parameters.AddWithValue("@edt", edt)
            cmd2.Parameters.AddWithValue("@ptype", jobtype)
            
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
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in getInteraldata: " & ex.Message)
        Finally
            If conn2.State = ConnectionState.Open Then
                conn2.Close()
            End If
        End Try
        Return internalregion
    End Function
    
    Protected Function getTopExternaldata(ByVal plant As String, ByVal bdt As String, ByVal edt As String, ByVal jobtype As Integer) As Dictionary(Of transporterdata, regiondata)
        Dim externdict As New Dictionary(Of transporterdata, regiondata)
        Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Try
            Dim cmd3 As New SqlCommand()
            
            ' SECURITY FIX: Use parameterized query
            If ddlPlants.SelectedValue = "PG" Then
                cmd3.CommandText = "select sum(qty) as tqty, transporter from (select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0) else round(dn_qty,0) end qty, transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply in ('PG1','PG2','PG3') and weight_outtime between @bdt and @edt and transporter_id in (select transporterid from oss_transporter_master where internaltype=0) and productcode in (select productid from oss_product_master where producttype=@ptype)) as T group by transporter order by tqty desc"
            Else
                cmd3.CommandText = "select sum(qty) as tqty, transporter from (select t2.region, case when dn_qty>=100 then round(IIF(productcode ='P3',dn_qty*0.02,dn_qty*0.05),0) else round(dn_qty,0) end qty, transporter from oss_patch_out t1 left outer join oss_area_code t2 on t1.area_code = t2.area_code where source_supply=@plant and weight_outtime between @bdt and @edt and transporter_id in (select transporterid from oss_transporter_master where internaltype=0) and productcode in (select productid from oss_product_master where producttype=@ptype)) as T group by transporter order by tqty desc"
                cmd3.Parameters.AddWithValue("@plant", plant)
            End If
            
            cmd3.Parameters.AddWithValue("@bdt", bdt)
            cmd3.Parameters.AddWithValue("@edt", edt)
            cmd3.Parameters.AddWithValue("@ptype", jobtype)
            
            cmd3.Connection = conn2
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
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in getTopExternaldata: " & ex.Message)
        Finally
            If conn2.State = ConnectionState.Open Then
                conn2.Close()
            End If
        End Try
        Return externdict
    End Function
    
    Protected Function GenerateTable(ByVal internalregion As regiondata, ByVal externdict As Dictionary(Of transporterdata, regiondata), ByVal tablename As String) As StringBuilder
        Dim internalsb As New StringBuilder()
        Try
            Dim r As DataRow
            internalsb.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""" & HttpUtility.HtmlEncode(tablename) & """ style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
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
            
            ' SECURITY FIX: Use HtmlEncode for output
            internalsb.Append("<tr><td>" & HttpUtility.HtmlEncode("Internal") & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(internalregion.north) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(internalregion.centre) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(internalregion.east) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(internalregion.south) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(total) & "</td></tr>")
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
                    
                    ' SECURITY FIX: Use HtmlEncode for output
                    internalsb.Append("<td>" & HttpUtility.HtmlEncode(rdata.Key.transname) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(rdata.Value.north) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(rdata.Value.centre) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(rdata.Value.east) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(rdata.Value.south) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(rdata.Key.qty) & "</td></tr>")
                    
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
            
            ' SECURITY FIX: Use HtmlEncode for output
            internalsb.Append("<tr class=""odd""><td>Others</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(otherregion.north) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(otherregion.centre) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(otherregion.east) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(otherregion.south) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(othertotal) & "</td></tr>")
            
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
            
            ' SECURITY FIX: Use HtmlEncode for output
            internalsb.Append("<tr class=""even""><td>Sub-total</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(externalregion.north) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(externalregion.centre) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(externalregion.east) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(externalregion.south) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(extotal) & "</td></tr>")
            
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
            
            ' SECURITY FIX: Use HtmlEncode for output
            internalsb.Append("<tr class=""even""><td>Total</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(internalregion.north + externalregion.north) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(internalregion.centre + externalregion.centre) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(internalregion.east + externalregion.east) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(internalregion.south + externalregion.south) & "</td><td class=""rightalign"">" & HttpUtility.HtmlEncode(total + extotal) & "</td></tr>")
            
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
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in GenerateTable: " & ex.Message)
        End Try
        Return internalsb
    End Function
    
    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub
    
    Protected Sub WriteLog(ByVal message As String)
        Try
            ' SECURITY FIX: Use secure logging
            SecurityHelper.LogSecurityEvent("OssDailyDispatchReport: " & message)
        Catch ex As Exception
            ' Fail silently
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
End Class