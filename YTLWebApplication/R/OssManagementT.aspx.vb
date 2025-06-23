Imports System.Collections.Generic
Imports System.Data.SqlClient
Partial Class OssManagementT
    Inherits System.Web.UI.Page
    Public userid As String
    Public plateno As String
    Public plantsCustom As Boolean
    Public lat As Double
    Public lon As Double
    Public sb1 As New StringBuilder()
    Public show As Boolean = False
    Public addosspatchoutpage As String = "AddOssPatchOut.aspx"
    Public divgrid As Boolean = False
    Public ec As String = "false"
    Public tpton As String = "1"
    Public title As String = ""

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            Dim userid As String = Request.Cookies("userinfo")("userid")

            Dim con As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmd As New SqlCommand("select * from YTLOSS.dbo.fn_GetAssignedPlants(" & userid & ")", con)
            con.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlSource.Items.Clear()
            ddlSource.Items.Add(New ListItem("ALL SOURCES", "ALL SOURCES"))
            Dim myid As String = ""
            plantsCustom = False
            Dim pgplant As Boolean = False

            While dr.Read()
                'myid = dr("id")
                'If userid = "7110" Then
                '    If myid = "2" Then
                '        ddlSource.Items.Add(New ListItem(dr("PV_Plant") & " - " & dr("PV_DisplayName"), dr("PV_Plant")))
                '    End If
                'ElseIf userid = "7111" Then
                '    If myid = "8" Or myid = "9" Or myid = "5" Or myid = "14" Then
                '        ddlSource.Items.Add(New ListItem(dr("PV_Plant") & " - " & dr("PV_DisplayName"), dr("PV_Plant")))
                '    End If
                'Else
                ddlSource.Items.Add(New ListItem(dr("PV_Plant") & " - " & dr("PV_DisplayName"), dr("PV_Plant")))
                If (dr("PV_Plant").ToString() = "PG1" Or dr("PV_Plant").ToString() = "PG2" Or dr("PV_Plant").ToString() = "PG3") And pgplant = False Then
                    pgplant = True
                End If
                If dr("custom") = "1" Then
                    plantsCustom = True
                Else
                    plantsCustom = False
                End If
                'End If
            End While
            If pgplant Then
                ddlSource.Items.Add(New ListItem("PG - PG1 & PG2 & PG3", "PG"))
            End If
            con.Close()


        Catch ex As Exception

        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim role As String = Request.Cookies("userinfo")("role")

            If role <> "SuperUser" Then
                tpton = "0"
            End If


            If Page.IsPostBack = False Then
                Dim bdt As String = ""
                bdt = Request.QueryString("bdt")
                Dim edt As String = ""
                edt = Request.QueryString("edt")
                title = Request.QueryString("text")
                If Not title = "" Then
                    'reprocesslbl.Text = title
                    'reprocesslbl.Visible = True
                End If
                If bdt = "" Then
                    txtBeginDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
                    txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                Else
                    txtBeginDate.Value = Convert.ToDateTime(bdt).ToString("yyyy/MM/dd")
                    txtEndDate.Value = Convert.ToDateTime(edt).ToString("yyyy/MM/dd")
                End If


                ' FillTransporters()
                FillUser(0)
                'Dim ssource As String = ""
                'Dim sshipto As String = ""
                'Dim sstatus As String = ""

                'ssource = System.Uri.UnescapeDataString(Request.QueryString("source"))
                'sshipto = System.Uri.UnescapeDataString(Request.QueryString("shipto"))
                'sstatus = System.Uri.UnescapeDataString(Request.QueryString("status"))

                'If Not ssource = "" Then
                '    ddlSource.SelectedValue = ssource
                'End If
                'If Not sstatus = "" Then
                '    ddlstatus.SelectedValue = sstatus
                'End If
                'If Not sshipto = "" Then
                '    ddlShipToCode.SelectedValue = sshipto
                'End If

                FillGrid()
            End If

        Catch ex As Exception

        End Try
    End Sub

    Sub FillTransporters()
        Try


            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim tankerQuery As String = "select plateno from vehicleTBL"

            Dim con As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            If role = "User" Then
                tankerQuery = "select plateno from vehicleTBL where userid='" & userid & "'"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                tankerQuery = "select plateno from vehicleTBL where userid in (" & userslist & ")"
            End If
            Dim ds As New DataSet
            Dim da As New SqlDataAdapter(tankerQuery, con)
            da.Fill(ds)
            Dim platecond As String = ""
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If Not IsDBNull(ds.Tables(0).Rows(i)("plateno")) Then
                    platecond = platecond & "'" & ds.Tables(0).Rows(i)("plateno") & "',"
                End If
            Next


            If platecond.Length > 3 Then
                platecond = platecond.Substring(0, platecond.Length - 1)
                platecond = " where plateno in (" & platecond & ")  "
            End If


            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmd As New SqlCommand("select transporter_name,transporter_id from oss_transporter order by transporter_name ", conn)

            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            ddlTransport.Items.Add("ALL TRANSPORTERS")

            While (dr.Read())
                Try
                    ddlTransport.Items.Add(New ListItem(dr("transporter_name"), dr("transporter_id")))
                Catch ex As Exception

                End Try
            End While
            dr.Close()
            'cmd.CommandText = "select  PV_Plant,PV_DisplayName from oss_plant_master order by PV_DisplayName"
            'dr = cmd.ExecuteReader()
            'ddlSource.Items.Clear()
            'ddlSource.Items.Add(New ListItem("ALL SOURCES", "ALL SOURCES"))
            'While dr.Read()
            '    ddlSource.Items.Add(New ListItem(dr("PV_Plant") & " - " & dr("PV_DisplayName"), dr("PV_Plant")))
            'End While
            conn.Close()

        Catch ex As Exception

        End Try
    End Sub

    Public Sub FillGrid()

        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim uname As String = Request.Cookies("userinfo")("username")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim tankerQuery As String = "select plateno from vehicleTBL"
            ddlshipto.Value = Request.Form("ddlShipToCode")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            'If role = "User" Then
            '    tankerQuery = "select plateno from vehicleTBL where userid='" & userid & "'"
            'ElseIf role = "SuperUser" Or role = "Operator" Then
            '    tankerQuery = "select plateno from vehicleTBL where userid in (" & userslist & ")"
            'End If
            Dim shipToCodeQuery As String = "select geofencename,shiptocode from geofence where accesstype='1' order by LTRIM(geofencename)"

            Dim ds As New DataSet
            Dim da As New SqlDataAdapter(tankerQuery, conn)
            da.Fill(ds)
            Dim platecond As String = ""
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If Not IsDBNull(ds.Tables(0).Rows(i)("plateno")) Then
                    platecond = platecond & "'" & ds.Tables(0).Rows(i)("plateno") & "',"
                End If
            Next

            da = New SqlDataAdapter(shipToCodeQuery, conn)
            ds.Clear()
            da.Fill(ds)

            Dim ShipToNameDict As New Dictionary(Of Integer, String)

            For c As Integer = 0 To ds.Tables(0).Rows.Count - 1
                Try
                    If Not ShipToNameDict.ContainsKey(ds.Tables(0).Rows(c)("shiptocode")) Then
                        ShipToNameDict.Add(ds.Tables(0).Rows(c)("shiptocode"), ds.Tables(0).Rows(c)("geofencename").ToString().ToUpper())
                    End If
                Catch ex As Exception

                End Try
            Next

            platecond = " "
            'If platecond.Length > 3 Then
            '    platecond = platecond.Substring(0, platecond.Length - 1)
            '    platecond = " and plateno in (" & platecond & ")"
            'End If

            'If role = "Admin" Or (userid = "7110" Or userid = "7111" Or userid = "1967" Or userid = "1968" Or userid = "7050" Or userid = "7060" Or userid = "2068") Then
            '    platecond = " "
            'End If



            Dim begintimestamp As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim endtimestamp As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            Dim statusdrop As String = ddlstatus.SelectedValue

            Dim condition As String = ""
            Dim totaldnqty As Double = 0

            If (ddlstatus.SelectedValue <> "--ALL STATUS--") Then
                condition = " and status='" & ddlstatus.SelectedValue & "'"
            End If



            If (ddlSource.SelectedValue = "PG") Then
                'If userid = "7110" Then
                '    condition = condition & " and source_supply='BS' "
                'ElseIf userid = "7111" Then
                '    condition = condition & " and source_supply in ('PG1','PG2','PG3','TB')"
                'Else
                condition = condition & " and source_supply in ('PG1','PG2','PG3')"

                'End If
            ElseIf ddlSource.SelectedValue <> "ALL SOURCES" Then
                condition = condition & " and source_supply='" & ddlSource.SelectedValue & "'"
            Else
                'If userid = "7110" Then
                '    condition = condition & " and source_supply='BS' "
                'ElseIf userid = "7111" Then
                'condition = condition & " and source_supply in (select pv_plant from YTLOSS.dbo.fn_GetAssignedPlants(" & userid & "))"
                'Else
                '    ' condition = ""
                'End If

                If plantsCustom Then
                    condition = condition & " and source_supply in (select pv_plant from YTLOSS.dbo.fn_GetAssignedPlants(" & userid & "))"
                End If
            End If

            If (ddlJobType.SelectedValue = "0") Then

            ElseIf ddlJobType.SelectedValue = "1" Then
                condition = condition & " and productcode in (select productid from oss_product_master where producttype=1) "
            Else
                condition = condition & " and productcode in (select productid from oss_product_master where producttype=2) "
            End If
            Dim remarkfn As String = ""
            If role = "Admin" Then
                remarkfn = "fn_GetOSSRemark_admin"
            Else
                remarkfn = "fn_GetOSSRemark"
            End If


            Try
                If (Request.Form("ddlShipToCode") <> "ALL") Then
                    condition = condition & " and destination_siteid='" & Request.Form("ddlShipToCode") & "'"
                End If
            Catch ex As Exception

            End Try

            If ddlfiltertype.SelectedValue = 0 Then
                If (ddlTransport.SelectedValue <> "ALL TRANSPORTERS") Then
                    condition = condition & " and transporter_id='" & ddlTransport.SelectedValue & "'"
                Else
                    If ddltranstype.SelectedValue = 0 Then
                    ElseIf ddltranstype.SelectedValue = 1 Then
                        condition = condition & " and transporter_id in (select transporterid  from oss_transporter_master where internaltype =1)"
                    Else
                        condition = condition & " and transporter_id in (select transporterid  from oss_transporter_master where internaltype =0)"
                    End If
                End If
            Else
                If (ddlTransport.SelectedValue <> "ALL USERS") Then
                    condition = condition & " and plateno in (select plateno from YTLDB.dbo.vehicleTBL where userid='" & ddlTransport.SelectedValue & "')"
                Else
                    If ddltranstype.SelectedValue = 0 Then
                    ElseIf ddltranstype.SelectedValue = 1 Then
                        condition = condition & " and plateno in (select plateno from YTLDB.dbo.vehicleTBL where userid in (select userid from YTLDB.dbo.usertbl where role='User' and companyname like 'YTL%'))"
                    Else
                        condition = condition & " and plateno in (select plateno from YTLDB.dbo.vehicleTBL where userid in (select userid from YTLDB.dbo.usertbl where role='User' and userid not in (select userid from YTLDB.dbo.usertbl where role='User' and companyname like 'YTL%')))"
                    End If
                End If
            End If

            'If (ddlTransport.SelectedValue <> "ALL TRANSPORTERS") Then
            '    condition = condition & " and transporter_id='" & ddlTransport.SelectedValue & "'"
            'End If

            Dim columnname As String = "weight_outtime"

            If (RadioButton2.Checked = True) Then
                columnname = "ata_datetime"
            End If

            Dim r As DataRow

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("chk"))
            t.Columns.Add(New DataColumn("S NO"))
            t.Columns.Add(New DataColumn("Plate NO"))
            t.Columns.Add(New DataColumn("Unit ID"))
            t.Columns.Add(New DataColumn("Transporter"))
            t.Columns.Add(New DataColumn("Source"))
            t.Columns.Add(New DataColumn("DN NO"))
            t.Columns.Add(New DataColumn("Weight Outtime"))
            t.Columns.Add(New DataColumn("Ship To Code"))
            t.Columns.Add(New DataColumn("Ship To Name"))
            t.Columns.Add(New DataColumn("ATA"))
            t.Columns.Add(New DataColumn("GPS Point"))
            t.Columns.Add(New DataColumn("Status"))
            t.Columns.Add(New DataColumn("Remarks"))
            t.Columns.Add(New DataColumn("DN ID"))
            t.Columns.Add(New DataColumn("Group Name"))
            t.Columns.Add(New DataColumn("PTO On Time"))
            t.Columns.Add(New DataColumn("Wait Start Time"))
            t.Columns.Add(New DataColumn("Diversion"))
            t.Columns.Add(New DataColumn("Product Type"))
            t.Columns.Add(New DataColumn("Tonnage"))
            t.Columns.Add(New DataColumn("Reprocess"))
            t.Columns.Add(New DataColumn("Area Code"))
            t.Columns.Add(New DataColumn("ETA"))
            t.Columns.Add(New DataColumn("ETA-ATA"))
            t.Columns.Add(New DataColumn("Est Distance"))
            t.Columns.Add(New DataColumn("Code"))
            t.Columns.Add(New DataColumn("Base"))
            t.Columns.Add(New DataColumn("Plateno"))
            t.Columns.Add(New DataColumn("Date"))
            t.Columns.Add(New DataColumn("Transporter Type"))
            t.Columns.Add(New DataColumn("State"))
            t.Columns.Add(New DataColumn("Patchno"))
            t.Columns.Add(New DataColumn("Product Type1"))
            t.Columns.Add(New DataColumn("Region"))

            Dim vehicleDict As New Dictionary(Of String, Vinfo)
            Dim vinfo As Vinfo
            Dim cmd As New SqlCommand("select plateno,dbo.fn_getGroupNameFromPlateno(plateno) as groupname, isnull ( pmid,'-') as pmid,t2.username  from vehicleTBL t1 left outer join userTBL t2 on t1.userid =t2.userid  where plateno <>'' " & platecond, conn)
            Dim dr As SqlDataReader

            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    Try
                        vinfo = New Vinfo()
                        vinfo.Groupname = dr("groupname")
                        vinfo.pmid = dr("pmid")
                        Dim strarr() As String = dr("username").ToString().Split("_")
                        If strarr.Length > 1 Then
                            vinfo.base = strarr(1)
                        Else
                            vinfo.base = ""
                        End If

                        vehicleDict.Add(dr("plateno").ToString().Split("_")(0), vinfo)
                    Catch ex As Exception

                    End Try
                End While

            Catch ex As Exception

            Finally
                conn.Close()
            End Try

            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))

            cmd = New SqlCommand("select *,isnull(t1.area_code_name,'-') as areaname,isnull(t1.area_code,'0') as areacode,dbo." & remarkfn & "(dn_id) as jremark,isnull(t2.internaltype,0) as ttype,isnull(t3.state,'-') as state,dbo.fn_GetProductType(t1.productcode ) as producttype,isnull(t3.region,'-') as region from oss_patch_out t1 left outer join oss_transporter_master t2 on t1.transporter_id=t2.transporterid  left outer join oss_area_code t3 on t1.area_code =t3.area_code where " & columnname & " between '" & begintimestamp & "' and '" & endtimestamp & "'" & condition & "  " & platecond & "   order by " & columnname & "", conn)
            'Response.Write(cmd.CommandText)
            Try
                conn.Open()

                dr = cmd.ExecuteReader()
                Dim time As DateTime
                Dim checktime As DateTime
                Dim endchecktime As DateTime
                Dim i As Int32 = 1
                While dr.Read()
                    Try
                        '  If dr("destination_siteid").ToString().Trim().Length <> 3 Then
                        r = t.NewRow
                        r(0) = dr("patch_no")
                        r(1) = i.ToString()
                        r(2) = "<a href=  UpdateOssNew.aspx?Id=" & dr("patch_no") & "&bdt=" & txtBeginDate.Value & "&edt=" & txtEndDate.Value & " title='Update'> " & dr("plateno") & " </a>"

                        r(3) = dr("unitid")
                        If IsDBNull(dr("transporter")) Then
                            r(4) = "--"
                        Else
                            r(4) = dr("transporter")
                        End If

                        r(5) = dr("source_supply")
                        r(6) = dr("dn_no")


                        Dim p = DateTime.Parse(dr("weight_outtime")).ToString("yyyy/MM/dd  HH:mm:ss")
                        p = p.Replace("-", "/")

                        r(7) = p

                        r(8) = dr("destination_siteid")

                        If IsDBNull(dr("destination_sitename")) Then
                            If ShipToNameDict.ContainsKey(dr("destination_siteid")) Then
                                r(9) = ShipToNameDict.Item(dr("destination_siteid")).ToUpper()
                            Else
                                r(9) = "--"
                            End If
                        Else
                            If dr("destination_sitename").ToString() = "Buildcon Concrete Sdn Bhd" Then
                                r(9) = "<span onclick="" javascript:OpenGeoInfo('" & dr("plateno") & "','" & Convert.ToDateTime(dr("weight_outtime")).ToString("yyyy/MM/dd HH:mm:ss") & "')"" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""Show Buildcon Geofences"">" & dr("destination_sitename").ToString.ToUpper() & "</span>"
                            Else
                                If dr("status").ToString() <> "2" Then
                                    r(9) = "<span style=''>" & dr("destination_sitename").ToString.ToUpper() & "</span>"
                                Else
                                    r(9) = "<span style='color:red;'>" & dr("destination_sitename").ToString.ToUpper() & "</span>"
                                End If
                            End If

                        End If

                            Dim lat As Double = 0
                        Dim lon As Double = 0

                        If IsDBNull(dr("ata_datetime")) = False Then
                            r(10) = DateTime.Parse(dr("ata_datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                        Else
                            r(10) = "--"
                        End If

                        If IsDBNull(dr("lat")) = False Then
                            r(11) = Convert.ToDouble(dr("lat")).ToString("0.0000") & "," & Convert.ToDouble(dr("lon")).ToString("0.0000")
                        Else
                            r(11) = "--"
                        End If

                        Dim status As String = dr("status").ToString()
                        Dim diversionstatus As String = ""
                        Select Case status
                            Case 0
                                If dr("destination_siteid") <> Nothing Then
                                    status = "Waiting To Process"
                                Else
                                    status = "Waiting for Ship To Code"
                                End If
                            Case 1
                                status = "No GPS Device"
                            Case 2
                                status = "Pending Destination Set Up"
                            Case 3
                                status = "In Progress"
                            Case 4
                                status = "Geofence In"
                            Case 5
                                status = "Inside Geofence"
                            Case 6
                                status = "Geofence Out"
                            Case 7
                                status = "Delivery Completed"
                            Case 8
                                status = "Delivery Completed (E)"
                            Case 10
                                status = "Timeout"
                            Case 11
                                status = "Reprocess Job"
                            Case 12
                                status = "Delivery Completed (D)"
                            Case 13
                                status = "Delivery Completed (P)"
                            Case 14
                                status = "No GPS Data"
                            Case 15
                                status = "Diversion"
                            Case Else

                        End Select

                        Try


                            If status = "Pending Destination Set Up" Then
                                r(12) = "<span onclick=""javascript:openpage('" & dr("patch_no") & "')"" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""Show idling information."">" & status & "</span>"
                            ElseIf status = "Inside Geofence" Then
                                r(12) = "<span onclick=""javascript:openpage('" & dr("patch_no") & "')"" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""Show idling information."">" & status & "</span>"
                            ElseIf status = "Timeout" Then
                                r(12) = "<span onclick=""javascript:openpage('" & dr("patch_no") & "')"" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""Show idling information."">" & status & "</span>"
                            ElseIf status = "Delivery Completed" Or status = "Delivery Completed (P)" Or status = "Delivery Completed (E)" Or status = "Delivery Completed (D)" Then
                                r(12) = "<span onclick=""javascript:openpage('" & dr("patch_no") & "')"" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""Show idling information."">" & status & "</span>"
                            ElseIf status = "In Progress" Then
                                r(12) = "<span onclick=""javascript:openpage('" & dr("patch_no") & "')"" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""Show idling information."">" & status & "</span>"
                            ElseIf status = "No GPS Data" Or status = "No GPS Device" Or status = "Diversion" Then
                                r(12) = "<span onclick=""javascript:openpage('" & dr("patch_no") & "')"" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""Show idling information."">" & status & "</span>"
                            Else
                                r(12) = status
                            End If
                            If IsDBNull(dr("remarks")) = False Then
                                r(13) = dr("remarks")
                            Else
                                r(13) = "--"
                            End If
                            r(14) = dr("dn_id")

                            r(15) = ""

                            If vehicleDict.ContainsKey(dr("plateno")) Then
                                r(15) = vehicleDict(dr("plateno")).Groupname
                            End If

                            If IsDBNull(dr("pto1_datetime")) = False Then
                                r(16) = DateTime.Parse(dr("pto1_datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                            Else
                                r(16) = "--"
                            End If


                            If IsDBNull(dr("wait_start_time")) = False Then
                                r(17) = DateTime.Parse(dr("wait_start_time")).ToString("yyyy/MM/dd HH:mm:ss")
                            Else
                                r(17) = "--"
                            End If
                            '  If dr("status").ToString() <> "7" And dr("status").ToString() <> "8" And dr("status").ToString() <> "12" And dr("status").ToString() <> "13" Then
                            r(18) = "<span onclick=""javascript:OpenDiversion('" & dr("patch_no") & "')"" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""Divert"">Diversion</span>"
                            ' Else
                            '     r(18) = "NA"
                            ' End If
                            r(19) = dr("productname")
                            r(20) = dr("dn_qty")

                            If dr("dn_qty") >= 100 Then
                                If dr("productcode").ToString().ToUpper() = "P3" Then
                                    totaldnqty += dr("dn_qty") * 0.02
                                Else
                                    totaldnqty += dr("dn_qty") * 0.05
                                End If
                            Else
                                totaldnqty += dr("dn_qty")
                            End If

                            If dr("jremark") = "-" Then
                                r(21) = "<span onclick=""javascript:ReprocessJob('" & dr("patch_no") & "')"" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""Reprocess"">Reprocess</span>"
                            Else
                                r(21) = "<span onclick=""javascript:ReprocessJob('" & dr("patch_no") & "')"" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""Reprocess"">Reprocess</span> <img src='images/information_balloon.png' style='cursor:pointer' title='" & dr("jremark") & "' />"
                            End If


                            r(22) = dr("areaname") & " - " & dr("areacode") & ""
                            If Not IsDBNull(dr("est_arrivaltime")) Then
                                r(23) = Convert.ToDateTime(dr("est_arrivaltime")).ToString("yyyy/MM/dd HH:mm:ss")
                                If (IsDBNull(dr("ata_datetime"))) Then
                                    r(24) = "-"
                                Else
                                    r(24) = Convert.ToInt32((Convert.ToDateTime(dr("est_arrivaltime")) - Convert.ToDateTime(dr("ata_datetime"))).TotalMinutes)
                                End If
                            Else
                                r(23) = "-"
                                r(24) = "-"
                            End If
                            r(25) = dr("est_distance")
                            If vehicleDict.ContainsKey(dr("plateno")) Then
                                r(26) = vehicleDict(dr("plateno")).pmid
                                r(27) = vehicleDict(dr("plateno")).base
                            Else
                                r(26) = "-"
                                r(27) = "-"
                            End If
                            r(28) = dr("plateno")
                            time = Convert.ToDateTime(dr("weight_outtime"))
                            checktime = Convert.ToDateTime(Convert.ToDateTime(dr("weight_outtime")).ToString("yyyy/MM/dd 07:00:00"))
                            endchecktime = Convert.ToDateTime(Convert.ToDateTime(dr("weight_outtime")).AddDays(1).ToString("yyyy/MM/dd 07:00:00"))
                            If time > checktime And time < endchecktime Then
                                r(29) = DateTime.Parse(dr("weight_outtime")).ToString("yyyy/MM/dd")
                            ElseIf time < checktime Then
                                r(29) = time.AddDays(-1).ToString("yyyy/MM/dd")
                            Else
                                r(29) = endchecktime.ToString("yyyy/MM/dd")
                            End If
                            If dr("ttype") Then
                                r(30) = "Internal"
                            Else
                                r(30) = "External"
                            End If
                            r(31) = dr("state")
                            r(32) = dr("patch_no")
                            r(33) = dr("producttype")
                            r(34) = dr("region")
                        Catch ex As Exception
                            Response.Write(ex.Message & "-" & ex.StackTrace)
                        End Try
                        t.Rows.Add(r)

                        i = i + 1
                        ' End If
                    Catch ex As Exception

                    End Try

                End While

            Catch ex As Exception
                Response.Write(ex.Message & " - " & ex.StackTrace)
            Finally
                conn.Close()
            End Try


            If t.Rows.Count = 0 Then
                r = t.NewRow
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
                r(20) = "--"
                r(21) = "--"
                r(22) = "--"
                r(23) = "--"
                r(24) = "--"
                r(25) = "--"
                r(26) = "--"
                r(27) = "--"
                r(28) = "--"
                r(29) = "--"
                r(30) = "--"
                r(31) = "--"
                r(32) = "--"
                r(33) = "--"
                r(34) = "--"
                t.Rows.Add(r)
            End If



            If statusdrop = "4" Or statusdrop = "5" Or statusdrop = "6" Or statusdrop = "7" Or statusdrop = "8" Or statusdrop = "12" Or statusdrop = "13" Then
                'sb1.Append("<thead><tr align=""left""><th>S No</th><th>Plate NO</th><th>Unit ID</th><th>Group Name</th><th>Transporter</th><th>Source</th><th>DN ID</th><th>DN NO</th><th>Product Type</th><th>DN Qty</th><th>Weight Out Time</th><th>Area Code</th><th>Ship To Code</th><th>Ship To Name</th><th>PTO On Time</th><th>ATA</th><th>GPS Point</th><th>Wait Start Time</th><th>Status</th><th>Divert</th><th>Reprocess</th></tr></thead>")

                sb1.Append("<thead><tr align=""left""><th>S No</th><th>Plate NO</th><th>Unit ID</th><th>Group Name</th><th>Transporter</th><th>Source</th><th>DN ID</th><th>DN NO</th><th>Product Type</th><th>Product Type1</th><th>DN Qty</th><th>Weight Out Time</th><th>Area Code</th><th>Region</th><th>Ship To Code</th><th>Ship To Name</th><th>PTO On Time</th><th>ATA</th><th>Wait Start Time</th><th>Status</th><th>Divert</th><th>Reprocess</th><th></th></tr></thead>")
                '<th>ETA</th><th>ETA-ATA(Mins)</th><th>Est Distance</th>
            ElseIf statusdrop = "11" Then
                sb1.Append("<thead><tr align=""left""><th >S No</th><th>Plate NO</th><th>Unit ID</th><th>Group Name</th><th>Transporter</th><th>Source</th><th>DN ID</th><th>DN NO</th><th>Product Type</th><th>Product Type1</th><th>DN Qty</th><th>Weight Out Time</th><th>Area Code</th><th>Region</th><th>Ship To Code</th><th>Ship To Name</th><th>Status</th><th>Wait Start Time</th><th>Remarks</th><th>Divert</th><th>Reprocess</th><th></th></tr></thead>")
            Else
                sb1.Append("<thead><tr align=""left""><th >S No</th><th>Plate NO</th><th>Unit ID</th><th>Group Name</th><th>Transporter</th><th>Source</th><th>DN ID</th><th>DN NO</th><th>Product Type</th><th>Product Type1</th><th>DN Qty</th><th>Weight Out Time</th><th>Area Code</th><th>Region</th><th>Ship To Code</th><th>Ship To Name</th><th>PTO On Time</th><th>ATA</th><th>Wait Start Time</th><th>Status</th><th>Divert</th><th>Reprocess</th><th></th></tr></thead>")
                '<th>ETA</th><th>ETA-ATA(Mins)</th><th>Est Distance</th>
            End If

            sb1.Append("<tbody>")
            Dim counter As Integer = 1
            For i As Integer = 0 To t.Rows.Count - 1
                Try
                    If statusdrop = "4" Or statusdrop = "5" Or statusdrop = "6" Or statusdrop = "7" Or statusdrop = "8" Or statusdrop = "12" Or statusdrop = "13" Then

                        sb1.Append("<tr>")
                        sb1.Append("<td>")
                        sb1.Append(t.DefaultView.Item(i)(1))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(2))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(3))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(15))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(4))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(5))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(14))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(6))

                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(19))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(33))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(20))


                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(7))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(22))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(34))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(8))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(9))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(16))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(10))
                        'sb1.Append("</td><td>")
                        'sb1.Append(t.DefaultView.Item(i)(23))
                        'sb1.Append("</td><td>")
                        'sb1.Append(t.DefaultView.Item(i)(24))
                        'sb1.Append("</td><td>")
                        'sb1.Append(t.DefaultView.Item(i)(25))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(17))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(12))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(18))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(21))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(32))
                        sb1.Append("</td></tr>")

                    ElseIf statusdrop = "11" Then
                        sb1.Append("<tr>")
                        sb1.Append("<td>")
                        sb1.Append(t.DefaultView.Item(i)(1))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(2))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(3))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(15))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(4))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(5))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(14))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(6))

                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(19))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(33))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(20))

                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(7))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(22))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(34))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(8))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(9))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(12))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(17))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(13))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(18))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(21))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(32))
                        sb1.Append("</tr>")
                    Else

                        sb1.Append("<tr>")
                        sb1.Append("<td>")
                        sb1.Append(t.DefaultView.Item(i)(1))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(2))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(3))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(15))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(4))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(5))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(14))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(6))

                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(19))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(33))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(20))

                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(7))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(22))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(34))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(8))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(9))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(16))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(10))
                        'sb1.Append("</td><td>")
                        'sb1.Append(t.DefaultView.Item(i)(23))
                        'sb1.Append("</td><td>")
                        'sb1.Append(t.DefaultView.Item(i)(24))
                        'sb1.Append("</td><td>")
                        'sb1.Append(t.DefaultView.Item(i)(25))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(17))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(12))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(18))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(21))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(32))
                        sb1.Append("</td></tr>")
                    End If

                    counter += 1
                Catch ex As Exception

                End Try
            Next
            sb1.Append("</tbody>")

            If statusdrop = "4" Or statusdrop = "5" Or statusdrop = "6" Or statusdrop = "7" Or statusdrop = "8" Or statusdrop = "12" Or statusdrop = "13" Then
                sb1.Append("<tfoot><tr style=""font-weight:bold;"" align=""left""><th>S No</th><th>Plate NO</th><th>Unit ID</th><th>Group Name</th><th>Transporter</th><th>Source</th><th>DN ID</th><th>DN NO</th><th>Product Type</th><th>Product Type1</th><th>" & CDbl(totaldnqty).ToString("0.00") & " Ton</th><th>Weight Out Time</th><th>Area Code</th><th>Region</th><th>Ship To Code</th><th>Ship To Name</th><th>PTO On Time</th><th>ATA</th><th>Wait Start Time</th><th>Status</th><th>Divert</th><th>Reprocess</th><th></th></tr></tfoot>")
                '<th>ETA</th><th>ETA-ATA(Mins)</th><th>Est Distance</th>

                'sb1.Append("<tfoot><tr style=""font-weight:bold;"" align=""left""><th>S No</th><th>Plate NO</th><th>Unit ID</th><th>Group Name</th><th>Transporter</th><th>Source</th><th>DN ID</th><th>DN NO</th><th>Product Type</th><th>DN Qty</th><th>Weight Out Time</th><th>Area Code</th><th>Ship To Code</th><th>Ship To Name</th><th>PTO On Time</th><th>ATA</th><th>GPS Point</th><th>Wait Start Time</th><th>Status</th><th>Divert</th><th>Reprocess</th></tr></tfoot>")

            ElseIf statusdrop = "11" Then
                sb1.Append("<tfoot><tr style=""font-weight:bold;"" align=""left""><th>S No</th><th>Plate NO</th><th>Unit ID</th><th>Group Name</th><th>Transporter</th><th>Source</th><th>DN ID</th><th>DN NO</th><th>Product Type</th><th>Product Type1</th><th>" & CDbl(totaldnqty).ToString("0.00") & " Ton</th><th>Weight Out Time</th><th>Area Code</th><th>Region</th><th>Ship To Code</th><th>Ship To Name</th><th>Status</th><th>Wait Start Time</th><th>Remarks</th><th>Divert</th><th>Reprocess</th><th></th></tr></tfoot>")
            Else
                sb1.Append("<tfoot><tr style=""font-weight:bold;"" align=""left""><th>S No</th><th>Plate NO</th><th>Unit ID</th><th>Group Name</th><th>Transporter</th><th>Source</th><th>DN ID</th><th>DN NO</th><th>Product Type</th><th>Product Type1</th><th>" & CDbl(totaldnqty).ToString("0.00") & " Ton</th><th>Weight Out Time</th><th>Area Code</th><th>Region></th><th>Ship To Code</th><th>Ship To Name</th><th>PTO On Time</th><th>ATA</th><th>Wait Start Time</th><th>Status</th><th>Divert</th><th>Reprocess</th><th></th></tr></tfoot>")
                '<th>ETA</th><th>ETA-ATA(Mins)</th><th>Est Distance</th>
            End If

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session.Remove("exceltable3")
            't.Columns.RemoveAt(23)
            't.Columns.RemoveAt(24)
            't.Columns.RemoveAt(25)
            t.Columns.RemoveAt(0)
            t.Columns.RemoveAt(1)
            t.Columns.RemoveAt(9)
            t.Columns.RemoveAt(10)
            t.Columns.RemoveAt(14)
            t.Columns.RemoveAt(16)
            t.Columns.RemoveAt(17)
            t.Columns.RemoveAt(17)
            t.Columns.RemoveAt(17)

            t.Columns(18).SetOrdinal(1)
            t.Columns(18).SetOrdinal(1)
            t.Columns(19).SetOrdinal(1)

            Session("exceltable") = t

            ec = "true"
        Catch ex As Exception
            Response.Write(ex.Message & "-" & ex.StackTrace)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        Try
            FillGrid()
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ddlfiltertype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlfiltertype.SelectedIndexChanged
        If ddlfiltertype.SelectedValue = 0 Then
            idlbl.InnerText = "Transporter"
            FillTransporter(ddltranstype.SelectedValue)
        Else
            idlbl.InnerText = "Username"
            FillUser(ddltranstype.SelectedValue)
        End If
    End Sub

    Public Sub FillTransporter(ByVal type As Int16)
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Try
            Dim cmd As New SqlCommand("select transporterid,transportername from oss_transporter_master order by transportername ", conn)
            If type = 0 Then
                cmd.CommandText = "select transporterid,transportername from oss_transporter_master order by transportername "
            ElseIf type = 1 Then
                cmd.CommandText = "select transporterid,transportername from oss_transporter_master where internaltype=1 order by transportername "
            Else
                cmd.CommandText = "select transporterid,transportername from oss_transporter_master where internaltype=0 order by transportername "
            End If
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlTransport.Items.Clear()
            ddlTransport.Items.Add("ALL TRANSPORTERS")
            While (dr.Read())
                Try
                    ddlTransport.Items.Add(New ListItem(dr("transportername"), dr("transporterid")))
                Catch ex As Exception

                End Try
            End While
            dr.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    Public Sub FillUser(ByVal type As Int16)
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try

            Dim cmd As New SqlCommand()
            cmd.Connection = conn
            If type = 0 Then
                cmd.CommandText = "select username,userid from usertbl where role='User' order by username "
            ElseIf type = 1 Then
                cmd.CommandText = "select username,userid from usertbl where role='User' and companyname like 'YTL%' order by username "
            Else
                cmd.CommandText = "select username,userid from usertbl where role='User' and companyname not like 'YTL%' order by username "
            End If
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlTransport.Items.Clear()
            ddlTransport.Items.Add("ALL USERS")
            While (dr.Read())
                Try
                    ddlTransport.Items.Add(New ListItem(dr("username").ToString().ToUpper(), dr("userid")))
                Catch ex As Exception

                End Try
            End While
            dr.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Protected Sub ddltranstype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlfiltertype.SelectedIndexChanged
        If ddltranstype.SelectedValue = 0 Then
            If ddlfiltertype.SelectedValue = 0 Then
                idlbl.InnerText = "Transporter"
                FillTransporter(0)
            Else
                idlbl.InnerText = "Username"
                FillUser(0)
            End If
        ElseIf ddltranstype.SelectedValue = 1 Then
            If ddlfiltertype.SelectedValue = 0 Then
                idlbl.InnerText = "Transporter"
                FillTransporter(1)
            Else
                idlbl.InnerText = "Username"
                FillUser(1)
            End If
        Else
            If ddlfiltertype.SelectedValue = 0 Then
                idlbl.InnerText = "Transporter"
                FillTransporter(2)
            Else
                idlbl.InnerText = "Username"
                FillUser(2)
            End If
        End If
    End Sub
    Private Structure Vinfo
        Dim Groupname As String
        Dim pmid As String
        Dim base As String
    End Structure
End Class
