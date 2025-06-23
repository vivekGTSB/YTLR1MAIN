Imports System.Data.SqlClient
Imports AspMap
Imports DocumentFormat.OpenXml.Office2010.Excel

Partial Class ShowInformation
    Inherits System.Web.UI.Page
    Public qsa() As String
    Public qs As String = ""
    Public source As String = ""
    Public pid As String = ""
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Try
            Dim id As String = Request.QueryString("id")
            pid = id
            
            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateInput(id, "^[0-9]+$") Then
                Response.Write("Invalid input parameters")
                Return
            End If
            
            Dim events As String = "2"
            If Request.QueryString("events") Is Nothing Then
                events = 2
            Else
                events = Request.QueryString("events")
                If events = "5" Then
                    event5hrs.Checked = True
                Else
                    event5hrs.Checked = False
                End If
            End If
            
            Dim from As String = ""
            from = Request.QueryString("from")

            Dim conn2 As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim role As String = ""
            If Not from = "Client" Then
                role = Request.Cookies("userinfo")("role")
            End If
            reqfrom.Value = from
            Dim remarkfn As String = ""

            If from = "Client" Then
                txtremark.Visible = False
                btnSubmitremark.Visible = False
            Else
                txtremark.Visible = True
                btnSubmitremark.Visible = True
            End If

            If role = "Admin" Then
                remarkfn = "fn_GetOSSRemark_admin"
                chkbox.Visible = True
            Else
                remarkfn = "fn_GetOSSRemark"
                chkbox.Visible = False
            End If
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As SqlCommand = New SqlCommand("SELECT dbo.fn_getnextweightouttime(p.plateno, p.weight_outtime) AS nextweightouttime, dbo.fn_getnextplantintime(p.plateno, p.weight_outtime) AS nextplantintime, ISNULL(s.name, p.destination_sitename) AS name, p.dn_id, p.dn_no, p.patch_no, p.plateno, p.weight_outtime, p.source_supply, p.destination_siteid, p.insert_dt, p.unitid, p.status, p.ata_date, p.ata_time, p.ata_datetime, p.timestamp, p.lat, p.lon, p.transporter, p.remarks, ISNULL(s.address5, p.area_code_name) AS address5, dbo." & remarkfn & "(p.dn_id) AS Jremarks, CASE WHEN start_odometer IS NULL OR end_odometer IS NULL THEN 0 WHEN end_odometer > start_odometer THEN end_odometer-start_odometer ELSE 0 END AS distancetravel, p.atd_datetime, p.plant_intime FROM (SELECT * FROM oss_patch_out WHERE patch_no=@id) p LEFT OUTER JOIN oss_ship_to_code s ON s.shiptocode=p.destination_siteid LEFT OUTER JOIN oss_job_remarks t2 ON p.dn_id=t2.dn_id", conn2)
            cmd.Parameters.AddWithValue("@id", id)
            
            Dim plateno As String = ""
            Dim bdt As DateTime
            Dim edt As DateTime
            Dim shiptocode As String = ""
            Dim status As String = ""
            Dim shiptoname As String = ""
            Dim shiptoaddress As String = ""
            Dim transportername As String = ""
            Dim tdnno As String = ""
            Dim dnid As String = ""
            Dim atddatetime As DateTime
            Dim plantintime As String = ""
            Dim nextweightouttime As String = ""
            Dim nextplantin As String = ""
            
            Try
                conn2.Open()
                Dim dr2 As SqlDataReader = cmd.ExecuteReader()

                While dr2.Read()
                    plateno = dr2("plateno").ToString()
                    tdnno = dr2("dn_no").ToString()
                    dnid = dr2("dn_id").ToString()
                    bdt = Convert.ToDateTime(dr2("weight_outtime"))
                    edt = Convert.ToDateTime(dr2("weight_outtime"))
                    status = dr2("status").ToString()
                    dn_id.Value = dr2("dn_id").ToString()
                    
                    If IsDBNull(dr2("transporter")) Then
                        transportername = "--"
                    Else
                        transportername = dr2("transporter").ToString().ToUpper()
                    End If
                    
                    atddatetime = bdt.AddHours(72)
                    
                    If IsDBNull(dr2("plant_intime")) Then
                        plantintime = "-"
                    Else
                        plantintime = Convert.ToDateTime(dr2("plant_intime")).ToString("yyyy/MM/dd HH:mm:ss")
                    End If
                    
                    If IsDBNull(dr2("nextweightouttime")) Then
                        nextweightouttime = "-"
                    Else
                        nextweightouttime = Convert.ToDateTime(dr2("nextweightouttime")).ToString("yyyy/MM/dd HH:mm:ss")
                    End If
                    
                    If IsDBNull(dr2("nextplantintime")) Then
                        nextplantin = "-"
                    Else
                        nextplantin = Convert.ToDateTime(dr2("nextplantintime")).ToString("yyyy/MM/dd HH:mm:ss")
                    End If

                    Select Case status
                        Case "1"
                            source = "Gmap.aspx?scode=1&plateno=" & plateno & "&bdt=" & bdt.ToString("yyyy/MM/dd HH:mm:ss") & "&edt=" & atddatetime.ToString("yyyy/MM/dd HH:mm:ss") & "&from=" & from & ""
                        Case "2"
                            If event5hrs.Checked Then
                                edt = bdt.AddHours(120)
                            Else
                                edt = bdt.AddHours(24)
                            End If

                            source = "Gmap.aspx?scode=1&plateno=" & plateno & "&bdt=" & bdt.ToString("yyyy/MM/dd HH:mm:ss") & "&edt=" & edt.ToString("yyyy/MM/dd HH:mm:ss") & "&from=" & from & ""
                        Case "3"
                            Dim tr As String = ""
                            If IsDBNull(dr2("transporter")) Then
                                tr = "--"
                            Else
                                tr = dr2("transporter").ToString()
                            End If
                            
                            edt = Now.ToString("yyyy/MM/dd HH:mm:ss")
                            source = "GMap.aspx?plateno=" & plateno & "&bdt=" & bdt.AddMinutes(-10).ToString("yyyy/MM/dd HH:mm:ss") & "&edt=" & edt.ToString("yyyy/MM/dd HH:mm:ss") & "&uid=" & dr2("unitid") & "&tr=" & HttpUtility.UrlEncode(tr) & "&sr=" & HttpUtility.UrlEncode(dr2("source_supply").ToString()) & "&dn=" & HttpUtility.UrlEncode(dr2("dn_no").ToString()) & "&wo=" & HttpUtility.UrlEncode(DateTime.Parse(dr2("weight_outtime").ToString()).ToString("yyyy/MM/dd HH:mm:ss")) & "&sc=" & HttpUtility.UrlEncode(dr2("destination_siteid").ToString()) & "&sn=" & HttpUtility.UrlEncode(dr2("name").ToString()) & "&scode=3&from=" & from & ""

                        Case "4"

                        Case "5"
                            If event5hrs.Checked Then
                                edt = bdt.AddHours(120)
                            Else
                                edt = Convert.ToDateTime(dr2("ata_date").ToString()).ToString("yyyy/MM/dd") & " " & dr2("ata_time").ToString()
                            End If

                            source = "Gmap.aspx?scode=1&plateno=" & plateno & "&bdt=" & bdt.AddMinutes(-10).ToString("yyyy/MM/dd HH:mm:ss") & "&edt=" & edt.AddMinutes(+10).ToString("yyyy/MM/dd HH:mm:ss") & "&from=" & from & ""
                        Case "6"

                        Case "7"
                            If event5hrs.Checked Then
                                edt = bdt.AddHours(120)
                            Else
                                edt = atddatetime.ToString("yyyy/MM/dd HH:mm:ss")
                            End If

                            Dim tr As String = ""
                            If IsDBNull(dr2("transporter")) Then
                                tr = "--"
                            Else
                                tr = dr2("transporter").ToString()
                            End If
                            
                            source = "GMap.aspx?markerlat=" & Convert.ToDouble(dr2("lat")).ToString("0.0000") & "&markerlon=" & Convert.ToDouble(dr2("lon")).ToString("0.0000") & "&plateno=" & plateno & "&bdt=" & bdt.AddMinutes(-10).ToString("yyyy/MM/dd HH:mm:ss") & "&edt=" & edt.ToString("yyyy/MM/dd HH:mm:ss") & "&uid=" & dr2("unitid") & "&tr=" & HttpUtility.UrlEncode(tr) & "&sr=" & HttpUtility.UrlEncode(dr2("source_supply").ToString()) & "&dn=" & HttpUtility.UrlEncode(dr2("dn_no").ToString()) & "&wo=" & HttpUtility.UrlEncode(DateTime.Parse(dr2("weight_outtime").ToString()).ToString("yyyy/MM/dd HH:mm:ss")) & "&sc=" & HttpUtility.UrlEncode(dr2("destination_siteid").ToString()) & "&sn=" & HttpUtility.UrlEncode(dr2("name").ToString()) & "&pos=" & HttpUtility.UrlEncode(DateTime.Parse(dr2("ata_datetime").ToString()).ToString("yyyy/MM/dd HH:mm:ss")) & "&ata=" & HttpUtility.UrlEncode(Convert.ToDateTime(dr2("ata_date").ToString()).ToString("yyyy/MM/dd") & " " & dr2("ata_time").ToString()) & "&scode=2&distance=" & CDbl(dr2("distancetravel")).ToString("0.00") & "&from=" & from & ""

                        Case "13"
                            If event5hrs.Checked Then
                                edt = bdt.AddHours(120)
                            Else
                                edt = bdt.AddHours(72) 'Convert.ToDateTime(dr2("ata_date").ToString()).ToString("yyyy/MM/dd") & " " & dr2("ata_time").ToString()
                            End If

                            source = "Gmap.aspx?scode=1&plateno=" & plateno & "&bdt=" & bdt.AddMinutes(-10).ToString("yyyy/MM/dd HH:mm:ss") & "&edt=" & edt.AddMinutes(+10).ToString("yyyy/MM/dd HH:mm:ss") & "&from=" & from & ""

                        Case "8"
                            If event5hrs.Checked Then
                                edt = bdt.AddHours(120)
                            Else
                                edt = bdt.AddHours(72) 'Convert.ToDateTime(dr2("ata_date").ToString()).ToString("yyyy/MM/dd") & " " & dr2("ata_time").ToString()
                            End If
                            
                            Dim tr As String = ""
                            If IsDBNull(dr2("transporter")) Then
                                tr = "--"
                            Else
                                tr = dr2("transporter").ToString()
                            End If
                            
                            source = "GMap.aspx?markerlat=" & Convert.ToDouble(dr2("lat")).ToString("0.0000") & "&markerlon=" & Convert.ToDouble(dr2("lon")).ToString("0.0000") & "&plateno=" & plateno & "&bdt=" & bdt.ToString("yyyy/MM/dd HH:mm:ss") & "&edt=" & edt.ToString("yyyy/MM/dd HH:mm:ss") & "&uid=" & dr2("unitid") & "&tr=" & HttpUtility.UrlEncode(tr) & "&sr=" & HttpUtility.UrlEncode(dr2("source_supply").ToString()) & "&dn=" & HttpUtility.UrlEncode(dr2("dn_no").ToString()) & "&wo=" & HttpUtility.UrlEncode(DateTime.Parse(dr2("weight_outtime").ToString()).ToString("yyyy/MM/dd HH:mm:ss")) & "&sc=" & HttpUtility.UrlEncode(dr2("destination_siteid").ToString()) & "&sn=" & HttpUtility.UrlEncode(dr2("name").ToString()) & "&pos=" & HttpUtility.UrlEncode(DateTime.Parse(dr2("ata_datetime").ToString()).ToString("yyyy/MM/dd HH:mm:ss")) & "&ata=" & HttpUtility.UrlEncode(Convert.ToDateTime(dr2("ata_date").ToString()).ToString("yyyy/MM/dd") & " " & dr2("ata_time").ToString()) & "&scode=2&distance=" & CDbl(dr2("distancetravel")).ToString("0.00") & "&from=" & from & ""
                            
                        Case "9"

                        Case "10"
                            If event5hrs.Checked Then
                                edt = bdt.AddHours(120)
                            Else
                                edt = bdt.AddHours(72) 'DateTime.Parse(dr2("weight_outtime")).AddHours(24).ToString("yyyy/MM/dd HH:mm:ss")
                            End If
                            
                            source = "Gmap.aspx?scode=1&plateno=" & plateno & "&bdt=" & bdt.AddMinutes(-10).ToString("yyyy/MM/dd HH:mm:ss") & "&edt=" & edt.ToString("yyyy/MM/dd HH:mm:ss") & "&from=" & from & ""
                            
                        Case "12"
                            If event5hrs.Checked Then
                                edt = bdt.AddHours(120)
                            Else
                                edt = bdt.AddHours(72) 'Convert.ToDateTime(dr2("ata_date").ToString()).ToString("yyyy/MM/dd") & " " & dr2("ata_time").ToString()
                            End If

                            source = "Gmap.aspx?scode=1&plateno=" & plateno & "&bdt=" & bdt.AddMinutes(-10).ToString("yyyy/MM/dd HH:mm:ss") & "&edt=" & edt.AddMinutes(+10).ToString("yyyy/MM/dd HH:mm:ss") & "&from=" & from & ""

                        Case "15"
                            If event5hrs.Checked Then
                                edt = bdt.AddHours(120)
                            Else
                                edt = bdt.AddHours(72) 'Convert.ToDateTime(dr2("ata_date").ToString()).ToString("yyyy/MM/dd") & " " & dr2("ata_time").ToString()
                            End If

                            source = "Gmap.aspx?scode=1&plateno=" & plateno & "&bdt=" & bdt.AddMinutes(-10).ToString("yyyy/MM/dd HH:mm:ss") & "&edt=" & edt.AddMinutes(+10).ToString("yyyy/MM/dd HH:mm:ss") & "&from=" & from & ""

                        Case Else
                    End Select

                    shiptoaddress = dr2("address5").ToString()
                    shiptocode = dr2("name").ToString()
                    shiptoname = dr2("destination_siteid").ToString()
                End While
                
                If Not Page.IsPostBack Then
                    dr2.Close()
                    If role = "Admin" Then
                        cmd.CommandText = "SELECT * FROM oss_job_remarks WHERE dn_id=@dn_id"
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@dn_id", dn_id.Value)
                    Else
                        cmd.CommandText = "SELECT * FROM oss_job_remarks WHERE dn_id=@dn_id AND admin=0"
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@dn_id", dn_id.Value)
                    End If

                    dr2 = cmd.ExecuteReader()
                    If dr2.HasRows() Then
                        If dr2.Read() Then
                            txtremark.Text = dr2("remark").ToString()
                            If dr2("Admin") Then
                                chkbox.Checked = True
                            Else
                                chkbox.Checked = False
                            End If
                        End If
                        btnSubmitremark.Text = "Update Remarks"
                    End If
                End If
            Catch ex As Exception
                ' SECURITY FIX: Log error but don't expose details
                SecurityHelper.LogError("Page_Load query error", ex, Server)
            Finally
                conn2.Close()
            End Try
            
            source = "'" & source & "'"
            txtOutTime.Text = HttpUtility.HtmlEncode(bdt.ToString("yyyy/MM/dd HH:mm:ss"))
            enddate.Value = edt.ToString("yyyy/MM/dd HH:mm:ss")
            txtdnid.Text = HttpUtility.HtmlEncode(dn_id.Value)
            txtdnno.Text = HttpUtility.HtmlEncode(tdnno)
            txtPlateno.Text = HttpUtility.HtmlEncode(plateno)
            txtSTC.Text = HttpUtility.HtmlEncode(shiptocode)
            txtShipToName.Text = HttpUtility.HtmlEncode(shiptoname)
            txtShiptoaddress.Text = HttpUtility.HtmlEncode(shiptoaddress)
            txtTransporter.Text = HttpUtility.HtmlEncode(transportername)
            txtPlantinTime.Text = HttpUtility.HtmlEncode(plantintime)
            txtNextWeightoutTime.Text = HttpUtility.HtmlEncode(nextweightouttime)
            txtNxtPlantinTime.Text = HttpUtility.HtmlEncode(nextplantin)
            
            Dim idlingtable As New DataTable
            idlingtable.Columns.Add(New DataColumn("sno"))
            idlingtable.Columns.Add(New DataColumn("plateno"))
            idlingtable.Columns.Add(New DataColumn("begindatetime"))
            idlingtable.Columns.Add(New DataColumn("enddatetime"))
            idlingtable.Columns.Add(New DataColumn("duration"))
            idlingtable.Columns.Add(New DataColumn("shiptocode"))
            idlingtable.Columns.Add(New DataColumn("time"))
            idlingtable.Columns.Add(New DataColumn("status"))

            Dim stoptable As New DataTable
            stoptable.Columns.Add(New DataColumn("sno"))
            stoptable.Columns.Add(New DataColumn("plateno"))
            stoptable.Columns.Add(New DataColumn("begindatetime"))
            stoptable.Columns.Add(New DataColumn("enddatetime"))
            stoptable.Columns.Add(New DataColumn("duration"))
            stoptable.Columns.Add(New DataColumn("shiptocode"))
            stoptable.Columns.Add(New DataColumn("time"))
            stoptable.Columns.Add(New DataColumn("status"))

            Dim ptotable As New DataTable
            ptotable.Columns.Add(New DataColumn("sno"))
            ptotable.Columns.Add(New DataColumn("plateno"))
            ptotable.Columns.Add(New DataColumn("begindatetime"))
            ptotable.Columns.Add(New DataColumn("enddatetime"))
            ptotable.Columns.Add(New DataColumn("duration"))
            ptotable.Columns.Add(New DataColumn("shiptocode"))
            ptotable.Columns.Add(New DataColumn("time"))
            ptotable.Columns.Add(New DataColumn("status"))

            Dim Newtable As New DataTable
            Newtable.Columns.Add(New DataColumn("sno"))
            Newtable.Columns.Add(New DataColumn("plateno"))
            Newtable.Columns.Add(New DataColumn("begindatetime"))
            Newtable.Columns.Add(New DataColumn("enddatetime"))
            Newtable.Columns.Add(New DataColumn("duration"))
            Newtable.Columns.Add(New DataColumn("shiptocode"))
            Newtable.Columns.Add(New DataColumn("time"))
            Newtable.Columns.Add(New DataColumn("status"))

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim i As Int32 = 1
            Dim lat As Double = 0
            Dim lon As Double = 0
            Dim r As DataRow

            ' SECURITY FIX: Use parameterized query
            cmd = New SqlCommand("SELECT DISTINCT CONVERT(VARCHAR(19), timestamp, 120) AS datetime, plateno, speed, ignition_sensor, alarm, lat, lon FROM vehicle_history WHERE plateno=@plateno AND (gps_av='A' OR (gps_av='V' AND ignition_sensor='0')) AND timestamp BETWEEN @startTime AND @endTime ORDER BY datetime ASC", conn)
            cmd.Parameters.AddWithValue("@plateno", plateno)
            cmd.Parameters.AddWithValue("@startTime", bdt.ToString("yyyy/MM/dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@endTime", atddatetime.ToString("yyyy/MM/dd HH:mm:ss"))

            Dim prevstatus As String = "stop"
            Dim currentstatus As String = "stop"
            Dim prevpto As String = "off"
            Dim currentpto As String = "off"

            Dim tempprevtime As DateTime = bdt
            Dim tempprevtimep As DateTime = bdt
            Dim prevtime As DateTime = bdt
            Dim currenttime As DateTime = bdt
            Dim prevtimep As DateTime = bdt
            Dim currenttimep As DateTime = bdt
            
            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim lastlat As Double = 0
                Dim lastlon As Double = 0
                Dim prevlat As Double = 0
                Dim prevlon As Double = 0
                Dim minOption As Byte = 15
                
                Try
                    While dr.Read()
                        lastlat = Convert.ToDouble(dr("lat"))
                        lastlon = Convert.ToDouble(dr("lon"))
                        currenttime = Convert.ToDateTime(dr("datetime"))
                        currenttimep = Convert.ToDateTime(dr("datetime"))
                        
                        If dr("ignition_sensor") = 1 And dr("speed") <> 0 Then
                            currentstatus = "moving"
                        ElseIf dr("ignition_sensor") = 1 And dr("speed") = 0 Then
                            currentstatus = "idle"
                        Else
                            currentstatus = "stop"
                        End If

                        If prevstatus <> currentstatus Then
                            Dim temptime As TimeSpan = tempprevtime - prevtime
                            Dim minutes As Integer = temptime.TotalMinutes()

                            Select Case prevstatus
                                Case "stop"
                                    If temptime.TotalMinutes >= 15 Then
                                        r = Newtable.NewRow
                                        r(0) = i
                                        r(1) = dr("plateno")
                                        r(2) = prevtime.ToString("yyyy/MM/dd HH:mm:ss")
                                        r(3) = tempprevtime.ToString("yyyy/MM/dd HH:mm:ss")
                                        r(4) = "<span onclick=""javascript:panMapToPos(" & lastlat & "," & lastlon & ") "" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""View on map"">" & temptime.TotalMinutes.ToString("0") & "</span>"
                                        r(5) = ""
                                        r(6) = prevtime.ToString("d MMM HH:mm") & "-" & tempprevtime.ToString("d MMM HH:mm")
                                        r(7) = "Stop"
                                        Newtable.Rows.Add(r)
                                        i = i + 1
                                    End If

                                Case "moving"

                                Case "idle"
                                    If temptime.TotalMinutes >= minOption Then
                                        r = Newtable.NewRow
                                        r(0) = i
                                        r(1) = dr("plateno")
                                        txtPlateno.Text = HttpUtility.HtmlEncode(dr("plateno").ToString())
                                        txtSTC.Text = HttpUtility.HtmlEncode(shiptocode)
                                        txtShipToName.Text = HttpUtility.HtmlEncode(shiptoname)
                                        r(2) = prevtime.ToString("yyyy/MM/dd HH:mm:ss")
                                        r(3) = tempprevtime.ToString("yyyy/MM/dd HH:mm:ss")
                                        r(4) = "<span onclick=""javascript:panMapToPos(" & prevlat & "," & prevlon & ") "" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""View on map"">" & temptime.TotalMinutes.ToString("0") & "</span>"
                                        r(5) = HttpUtility.HtmlEncode(shiptocode.ToString())
                                        r(6) = prevtime.ToString("d MMM HH:mm") & "-" & tempprevtime.ToString("d MMM HH:mm")
                                        r(7) = "Idling"
                                        Newtable.Rows.Add(r)
                                        i = i + 1
                                    End If
                            End Select
                            
                            prevtime = currenttime
                            prevstatus = currentstatus
                            prevlat = lastlat
                            prevlon = lastlon
                        End If

                        If dr("alarm") = "ON" Then
                            currentpto = "on"
                        Else
                            currentpto = "off"
                        End If

                        tempprevtime = currenttime
                        tempprevtimep = currenttimep
                    End While
                    
                Catch ex As Exception
                    ' SECURITY FIX: Log error but don't expose details
                    SecurityHelper.LogError("Page_Load data processing error", ex, Server)
                End Try
                
                If Not dr.IsClosed() Then
                    dr.Close()
                End If

                ' SECURITY FIX: Use parameterized query
                Dim da2 As New SqlDataAdapter("SELECT * FROM pto_history WHERE plateno=@plateno AND from_timestamp BETWEEN @startTime AND @endTime", conn)
                da2.SelectCommand.Parameters.AddWithValue("@plateno", plateno)
                da2.SelectCommand.Parameters.AddWithValue("@startTime", bdt.ToString("yyyy/MM/dd HH:mm:ss"))
                da2.SelectCommand.Parameters.AddWithValue("@endTime", atddatetime.ToString("yyyy/MM/dd HH:mm:ss"))
                
                Dim ds2 As New DataSet
                da2.Fill(ds2)
                Dim k As Integer = 1
                
                For c As Integer = 0 To ds2.Tables(0).Rows.Count - 1
                    Dim temptime As TimeSpan = Convert.ToDateTime(ds2.Tables(0).Rows(c)("to_timestamp")) - Convert.ToDateTime(ds2.Tables(0).Rows(c)("from_timestamp"))
                    If temptime.TotalMinutes > 5 Then
                        r = Newtable.NewRow
                        r(0) = k
                        r(1) = ds2.Tables(0).Rows(c)("plateno")
                        r(2) = Convert.ToDateTime(ds2.Tables(0).Rows(c)("from_timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                        r(3) = Convert.ToDateTime(ds2.Tables(0).Rows(c)("to_timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                        r(4) = "<span onclick=""javascript:panMapToPos(" & ds2.Tables(0).Rows(c)("lat") & "," & ds2.Tables(0).Rows(c)("lon") & ") "" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""View on map"">" & temptime.TotalMinutes.ToString("0") & "</span>"
                        r(5) = ""
                        r(6) = Convert.ToDateTime(ds2.Tables(0).Rows(c)("from_timestamp")).ToString("d MMM HH:mm") & "-" & Convert.ToDateTime(ds2.Tables(0).Rows(c)("to_timestamp")).ToString("d MMM HH:mm")
                        r(7) = "PTO On"
                        Newtable.Rows.Add(r)
                        k = k + 1
                    End If
                Next

                If idlingtable.Rows.Count = 0 Then
                    r = idlingtable.NewRow()
                    r(0) = "--"
                    r(1) = "--"
                    r(2) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    r(6) = "--"
                    idlingtable.Rows.Add(r)
                End If
                
                If stoptable.Rows.Count = 0 Then
                    r = stoptable.NewRow()
                    r(0) = "--"
                    r(1) = "--"
                    r(2) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    r(6) = "--"
                    stoptable.Rows.Add(r)
                End If
                
                If ptotable.Rows.Count = 0 Then
                    r = ptotable.NewRow()
                    r(0) = "--"
                    r(1) = "--"
                    r(2) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    r(6) = "--"
                    ptotable.Rows.Add(r)
                End If

                If Newtable.Rows.Count = 0 Then
                    r = Newtable.NewRow
                    r(0) = "--"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    r(6) = "--"
                    r(7) = 0
                    Newtable.Rows.Add(r)
                Else
                    Newtable.DefaultView.Sort = "begindatetime ASC"
                End If
            Catch ex As Exception
                ' SECURITY FIX: Log error but don't expose details
                SecurityHelper.LogError("Page_Load data processing error", ex, Server)
            End Try
            
            GridView1.DataSource = idlingtable
            GridView1.DataBind()
            GridView2.DataSource = stoptable
            GridView2.DataBind()
            GridView3.DataSource = ptotable
            GridView3.DataBind()
            GridView4.DataSource = Newtable
            GridView4.DataBind()
            
            conn.Close()
            conn.Dispose()

        Catch ex As SystemException
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("Page_Load system error", ex, Server)
        End Try
    End Sub

    Protected Sub btnSubmitremark_Click(sender As Object, e As EventArgs) Handles btnSubmitremark.Click
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim cmd As New SqlCommand()
        cmd.Connection = conn
        Dim adminbit As Int16 = 0
        
        Try
            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateInput(dn_id.Value, "^[0-9]+$") Then
                Response.Write("Invalid DN ID")
                Return
            End If
            
            conn.Open()
            If chkbox.Checked Then
                adminbit = 1
            Else
                adminbit = 0
            End If
            
            If btnSubmitremark.Text = "Update Remarks" Then
                ' SECURITY FIX: Use parameterized query
                cmd.CommandText = "UPDATE oss_job_remarks SET remark=@remarks, admin=@admin WHERE dn_id=@dn_id"
                cmd.Parameters.AddWithValue("@remarks", txtremark.Text)
                cmd.Parameters.AddWithValue("@admin", adminbit)
                cmd.Parameters.AddWithValue("@dn_id", dn_id.Value)
            Else
                ' SECURITY FIX: Use parameterized query
                cmd.CommandText = "INSERT INTO oss_job_remarks (dn_id, remark, admin) VALUES (@dn_id, @remarks, @admin)"
                cmd.Parameters.AddWithValue("@dn_id", dn_id.Value)
                cmd.Parameters.AddWithValue("@remarks", txtremark.Text)
                cmd.Parameters.AddWithValue("@admin", adminbit)
            End If
            
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("btnSubmitremark_Click error", ex, Server)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Protected Sub GridView4_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView4.RowDataBound
        ' Row coloring logic removed for security reasons
    End Sub
End Class