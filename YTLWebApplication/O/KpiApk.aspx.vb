Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic

Public Class KpiApk
    Inherits System.Web.UI.Page

    Public tactive As Integer = 0
    Public tinactive As Integer = 0
    Public ttotal As Double = 0

    Public totalActive As Integer = 0
    Public totalInActive As Integer = 0
    Public totalTotal As Integer = 0

    Public Total_bag_cargo As Integer = 0
    Public Total_bulk_tanker As Integer = 0
    Public Total_tipper As Integer = 0

    Public tworkshop As Integer = 0
    Public tService As Integer = 0

    Public e_tactive As Integer = 0
    Public e_tinactive As Integer = 0
    Public e_ttotal As Double = 0
    Public e_totalActive As Integer = 0
    Public e_totalInActive As Integer = 0
    Public e_totalTotal As Integer = 0
    Public e_tworkshop As Integer = 0
    Public e_tService As Integer = 0
    Public e_Total_bag_cargo As Integer = 0
    Public e_Total_bulk_tanker As Integer = 0
    Public e_Total_tipper As Integer = 0

    Public isInternal As String = "false"
    Dim TransporterDict As New Dictionary(Of String, KPIStruct)
    Dim e_TransporterDict As New Dictionary(Of String, KPIStruct)
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim maintenanceDict As New Dictionary(Of String, Maintenance)
            Try
                Dim cmd2 As New SqlCommand("select plateno,timestamp,statusdate,status,officeremark,sourcename from maintenance where timestamp>'2019/09/01' order by timestamp desc", conn)
                conn.Open()
                Dim dr2 As SqlDataReader = cmd2.ExecuteReader()
                While dr2.Read()
                    Try
                        Dim m As New Maintenance()
                        m.timestamp = DateTime.Parse(dr2("timestamp"))
                        m.statusdate = DateTime.Parse(dr2("statusdate"))
                        m.status = dr2("status")
                        If IsDBNull(dr2("officeremark")) Then
                            m.Remarks = ""
                        Else
                            m.Remarks = dr2("officeremark").ToString()
                        End If
                        If IsDBNull(dr2("sourcename")) Then
                            m.sourcename = ""
                        Else
                            m.sourcename = dr2("sourcename").ToString().ToUpper()
                        End If
                        maintenanceDict.Add(dr2("plateno").ToString().Trim().ToUpper(), m)
                    Catch ex As Exception

                    End Try
                End While
            Catch ex As Exception

            Finally
                conn.Close()
            End Try

            Dim cmd As New SqlCommand("select * from vehicle_tracked2", conn)

            Try
                Dim uid As String = Request.Cookies("userinfo")("userid")
                ' If uid = "0002" Or uid = "1931" Or uid = "6806" Then
                isInternal = "true"
                ' End If

                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim vehicleTrackedDict As New Dictionary(Of String, VehicleTrackedRecord)

                While dr.Read()
                    Try
                        Dim vtr As New VehicleTrackedRecord()
                        vtr.plateno = dr("plateno")
                        vtr.timeStamp = DateTime.Parse(dr("timestamp"))
                        vtr.lat = dr("lat")
                        vtr.lon = dr("lon")
                        vtr.speed = dr("speed")
                        vtr.direction = dr("bearing")
                        vtr.odometer = dr("odometer")

                        If (dr("ignition") = True) Then
                            vtr.ignition = True
                        Else
                            vtr.ignition = False
                            vtr.speed = 0
                        End If

                        If (dr("overspeed") = True) Then
                            vtr.overspeed = True
                        Else
                            vtr.overspeed = False
                        End If

                        If (dr("powercut") = True) Then
                            vtr.power = True
                        Else
                            vtr.power = False
                        End If

                        If (dr("immobilizer") = True) Then
                            vtr.immobilizer = True
                        Else
                            vtr.immobilizer = False
                        End If

                        If (dr("alarm") = True) Then
                            vtr.pto = True
                        Else
                            vtr.pto = False
                        End If

                        vtr.fuel1 = dr("volume1")
                        vtr.fuel2 = dr("volume2")

                        vehicleTrackedDict.Add(vtr.plateno, vtr)
                    Catch ex As Exception

                    End Try
                End While


                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")


                cmd = New SqlCommand("select v.type,v.plateno,cast(IsNull(m.internaltype,0) as bit)  internaltype,v.userid, u.username,u.companyname,IsNull(m.transportername,'N/A') as transportername,cast(IsNull(v.transporter_id,0) as int)  transporter_id,v.installationdate from YTLDB.dbo.userTBL u inner join (select distinct userid from YTLDB.dbo.vehicleTBL where plateno in(select distinct plateno from ytloss.dbo.oss_patch_in)) tt on tt.userid = u.userid inner join vehicleTBL v on v.userid = u.userid left outer join ytloss.dbo.oss_transporter_master m on v.transporter_id = m.transporterid  where u.username <> 'JASA1' and u.username <> 'JASA2' and u.username <> 'JasaCargo' ", conn)
                If role = "User" Then
                    cmd = New SqlCommand("select v.type,v.plateno,cast(IsNull(m.internaltype,0) as bit)  internaltype,v.userid, u.username,u.companyname,IsNull(m.transportername,'N/A') as transportername,cast(IsNull(v.transporter_id,0) as int)  transporter_id,v.installationdate from YTLDB.dbo.userTBL u inner join (select distinct userid from YTLDB.dbo.vehicleTBL where plateno in(select distinct plateno from ytloss.dbo.oss_patch_in) and userid='" & userid & "') tt on tt.userid = u.userid inner join vehicleTBL v on v.userid = u.userid left outer join ytloss.dbo.oss_transporter_master m on v.transporter_id = m.transporterid  where u.username <> 'JASA1' and u.username <> 'JASA2' and u.username <> 'JasaCargo' ", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select v.type,v.plateno,cast(IsNull(m.internaltype,0) as bit)  internaltype,v.userid, u.username,u.companyname,IsNull(m.transportername,'N/A') as transportername,cast(IsNull(v.transporter_id,0) as int)  transporter_id,v.installationdate from YTLDB.dbo.userTBL u inner join (select distinct userid from YTLDB.dbo.vehicleTBL where plateno in(select distinct plateno from ytloss.dbo.oss_patch_in) and userid in(" & userslist & ")) tt on tt.userid = u.userid inner join vehicleTBL v on v.userid = u.userid left outer join ytloss.dbo.oss_transporter_master m on v.transporter_id = m.transporterid  where u.username <> 'JASA1' and u.username <> 'JASA2' and u.username <> 'JasaCargo' ", conn)
                End If



                dr = cmd.ExecuteReader()

                ' Dim tdr As SqlDataReader

                Dim t As New DataTable
                t.Rows.Clear()
                t.Columns.Add(New DataColumn("S No"))
                t.Columns.Add(New DataColumn("Username"))
                t.Columns.Add(New DataColumn("Transporter"))
                t.Columns.Add(New DataColumn("Active"))
                t.Columns.Add(New DataColumn("Inactive"))
                t.Columns.Add(New DataColumn("Total"))
                t.Columns.Add(New DataColumn("Active %"))
                t.Columns.Add(New DataColumn("Inactive %"))
                t.Columns.Add(New DataColumn("Oss Total"))
                t.Columns.Add(New DataColumn("Oss Active"))
                t.Columns.Add(New DataColumn("Oss Inactive"))
                t.Columns.Add(New DataColumn("Oss Active %"))
                t.Columns.Add(New DataColumn("Oss Inactive %"))
                t.Columns.Add(New DataColumn("Workshop"))
                t.Columns.Add(New DataColumn("ServiceScheduled"))
                t.Columns.Add(New DataColumn("Pending"))
                t.Columns.Add(New DataColumn("TransType"))
                t.Columns.Add(New DataColumn("Bag"))
                t.Columns.Add(New DataColumn("Bulk"))
                t.Columns.Add(New DataColumn("Tipper"))




                Dim e_t As New DataTable
                e_t.Rows.Clear()
                e_t.Columns.Add(New DataColumn("S No"))
                e_t.Columns.Add(New DataColumn("Username"))
                e_t.Columns.Add(New DataColumn("Transporter"))
                e_t.Columns.Add(New DataColumn("Active"))
                e_t.Columns.Add(New DataColumn("Inactive"))
                e_t.Columns.Add(New DataColumn("Total"))
                e_t.Columns.Add(New DataColumn("Active %"))
                e_t.Columns.Add(New DataColumn("Inactive %"))
                e_t.Columns.Add(New DataColumn("Oss Total"))
                e_t.Columns.Add(New DataColumn("Oss Active"))
                e_t.Columns.Add(New DataColumn("Oss Inactive"))
                e_t.Columns.Add(New DataColumn("Oss Active %"))
                e_t.Columns.Add(New DataColumn("Oss Inactive %"))
                e_t.Columns.Add(New DataColumn("Workshop"))
                e_t.Columns.Add(New DataColumn("ServiceScheduled"))
                e_t.Columns.Add(New DataColumn("Pending"))
                e_t.Columns.Add(New DataColumn("TransType"))

                e_t.Columns.Add(New DataColumn("Bag"))
                e_t.Columns.Add(New DataColumn("Bulk"))
                e_t.Columns.Add(New DataColumn("Tipper"))

                Dim counter As Integer = 1
                Dim r As DataRow

                Dim vr As VehicleTrackedRecord


                Dim active As Integer = 0
                Dim inactive As Integer = 0
                Dim workshop As Integer = 0
                Dim serviceScheduled As Integer = 0
                Dim total As Double = 0
                Dim bag_cargo As Integer = 0
                Dim bulk_tanker As Integer = 0
                Dim tipper As Integer = 0




                Dim ossactive As Integer = 0
                Dim ossinactive As Integer = 0
                Dim osstotal As Integer = 0
                Dim ossworkshop As Integer = 0
                Dim ossserviceScheduled As Integer = 0

                Dim tossactive As Integer = 0
                Dim tossinactive As Integer = 0
                Dim tosstotal As Integer = 0
                Dim tossworkshop As Integer = 0
                Dim tossserviceScheduled As Integer = 0



                Dim e_active As Integer = 0

                Dim e_bag_cargo As Integer = 0
                Dim e_bulk_tanker As Integer = 0
                Dim e_tipper As Integer = 0


                Dim e_inactive As Integer = 0
                Dim e_workshop As Integer = 0
                Dim e_serviceScheduled As Integer = 0
                Dim e_total As Double = 0
                Dim e_ossactive As Integer = 0
                Dim e_ossinactive As Integer = 0
                Dim e_osstotal As Integer = 0
                Dim e_ossworkshop As Integer = 0
                Dim e_ossserviceScheduled As Integer = 0
                Dim e_tossactive As Integer = 0
                Dim e_tossinactive As Integer = 0
                Dim e_tosstotal As Integer = 0
                Dim e_tossworkshop As Integer = 0
                Dim e_tossserviceScheduled As Integer = 0

                Dim bdt As String = DateTime.Now.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss")
                Dim edt As String = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

                Dim ossDict As New Dictionary(Of String, DateTime)

                Dim conn2 As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                Dim cmd2 As SqlCommand = New SqlCommand("select plateno,weight_outtime from oss_patch_out where weight_outtime between '" & bdt & "' and '" & edt & "' and destination_siteid<>'' order by weight_outtime desc", conn2)
                Dim dr2 As SqlDataReader

                Try
                    conn2.Open()
                    dr2 = cmd2.ExecuteReader()
                    While dr2.Read()
                        Try
                            ossDict.Add(dr2("plateno"), dr2("weight_outtime"))
                        Catch ex As Exception

                        End Try
                    End While
                Catch ex As Exception

                Finally
                    conn2.Close()
                End Try

                While dr.Read()
                    Try
                        If Not IsDBNull(dr("internaltype")) Then
                            If Convert.ToBoolean(dr("internaltype")) Then
                                Try
                                    active = 0
                                    bag_cargo = 0
                                    bulk_tanker = 0
                                    tipper = 0
                                    inactive = 0
                                    total = 0
                                    workshop = 0
                                    serviceScheduled = 0

                                    ossactive = 0
                                    ossinactive = 0
                                    osstotal = 0
                                    ossworkshop = 0
                                    ossserviceScheduled = 0

                                    Dim mr As Maintenance

                                    Try
                                        If (vehicleTrackedDict.ContainsKey(dr("plateno"))) Then
                                            vr = vehicleTrackedDict.Item(dr("plateno"))
                                            Try
                                                If dr("type").ToString().ToUpper() = "CARGO" Then
                                                    bag_cargo += 1
                                                    Total_bag_cargo += 1
                                                ElseIf dr("type").ToString().ToUpper() = "TIPPER" Then
                                                    tipper += 1
                                                    Total_tipper += 1
                                                ElseIf dr("type").ToString().ToUpper() = "TANKER" Then
                                                    bulk_tanker += 1
                                                    Total_bulk_tanker += 1
                                                End If
                                            Catch ex As Exception

                                            End Try

                                            If (maintenanceDict.ContainsKey(dr("plateno"))) Then
                                                mr = maintenanceDict.Item(dr("plateno"))

                                                If (mr.timestamp > vr.timeStamp) Then
                                                    inactive = inactive + 1
                                                    totalInActive = totalInActive + 1
                                                    If mr.status.ToUpper() = "WORKSHOP" Or mr.status.ToUpper() = "BATTERY TAKEN OUT" Or mr.status.ToUpper() = "POWER CUT" Or mr.status.ToUpper() = "SPARE TRUCK" Or mr.status.ToUpper() = "NOT IN OPERATION" Or mr.status.ToUpper() = "ACCIDENT" Then
                                                        workshop = workshop + 1
                                                        tworkshop += 1
                                                    ElseIf mr.status.ToUpper() = "SERVICE SCHEDULED" Then
                                                        serviceScheduled = serviceScheduled + 1
                                                        tService += 1
                                                    Else
                                                        ' Response.Write(dr("plateno") & "_4<br/>")
                                                        ''''inactive = inactive + 1
                                                        ''''totalInActive += 1
                                                    End If
                                                Else
                                                    If ((DateTime.Now - vr.timeStamp).TotalHours > 24) Then
                                                        inactive = inactive + 1
                                                        totalInActive = totalInActive + 1
                                                    Else
                                                        active = active + 1
                                                        totalActive += 1
                                                        ' tactive += 1
                                                    End If
                                                End If
                                            Else
                                                If ((DateTime.Now - vr.timeStamp).TotalHours < 24) Then
                                                    active = active + 1
                                                    totalActive += 1
                                                Else
                                                    inactive = inactive + 1
                                                    totalInActive = totalInActive + 1
                                                End If
                                            End If
                                        Else
                                            inactive = inactive + 1
                                            totalInActive = totalInActive + 1
                                        End If

                                        Dim plateno As String() = dr("plateno").ToString().Split("_")
                                        Dim found As Boolean = False

                                        For i As Byte = 0 To plateno.Length - 1
                                            If (ossDict.ContainsKey(plateno(i))) Then
                                                found = True
                                                Exit For
                                            End If
                                        Next

                                        If found = True Then
                                            If (vehicleTrackedDict.ContainsKey(dr("plateno"))) Then
                                                vr = vehicleTrackedDict.Item(dr("plateno"))
                                                If ((DateTime.Now - vr.timeStamp).TotalHours < 24) Then
                                                    ossactive = ossactive + 1
                                                    tossactive = tossactive + 1
                                                    tactive = tactive + 1
                                                Else
                                                    ossinactive = ossinactive + 1
                                                    tossinactive = tossinactive + 1
                                                    tinactive = tinactive + 1
                                                End If
                                            Else
                                                ossinactive = ossinactive + 1
                                                tossinactive = tossinactive + 1
                                                tinactive = tinactive + 1
                                            End If

                                            osstotal = osstotal + 1
                                            tosstotal = tosstotal + 1
                                        End If

                                        total = total + 1
                                        ttotal = ttotal + 1
                                        totalTotal += 1
                                    Catch ex As Exception
                                        Response.Write("46546" & ex.Message)
                                    End Try

                                    'If total > inactive Then
                                    If Not TransporterDict.ContainsKey(dr("username").ToString.ToUpper()) Then
                                        Dim KPIStructNew As KPIStruct
                                        KPIStructNew.SYstem_Active = active
                                        KPIStructNew.bulk_tanker = bulk_tanker
                                        KPIStructNew.bag_cargo = bag_cargo
                                        KPIStructNew.tipper = tipper
                                        KPIStructNew.SYstem_InActive = inactive
                                        'Response.Write(dr("username").ToString.ToUpper() & "=" & inactive)
                                        KPIStructNew.SYstem_Workshop = workshop
                                        KPIStructNew.SYstem_ServiceScheduled = serviceScheduled

                                        KPIStructNew.SYstem_Total = total
                                        KPIStructNew.IsInternalType = Convert.ToBoolean(dr("internaltype"))
                                        KPIStructNew.Oss_Active = ossactive
                                        KPIStructNew.Oss_InActive = ossinactive
                                        KPIStructNew.Oss_Total = osstotal

                                        KPIStructNew.username = dr("username").ToString().ToUpper()
                                        KPIStructNew.userid = Convert.ToInt32(dr("userid"))
                                        KPIStructNew.transporterId = Convert.ToInt32(dr("transporter_id"))
                                        TransporterDict(dr("username").ToString.ToUpper()) = KPIStructNew
                                    Else
                                        Dim KPIStruct As KPIStruct = TransporterDict.Item(dr("username").ToString.ToUpper())
                                        KPIStruct.SYstem_Active = active + KPIStruct.SYstem_Active
                                        KPIStruct.SYstem_InActive = inactive + KPIStruct.SYstem_InActive
                                        KPIStruct.SYstem_Total = total + KPIStruct.SYstem_Total
                                        'Response.Write(dr("username").ToString.ToUpper() & "=" & KPIStruct.SYstem_InActive)
                                        KPIStruct.bulk_tanker += bulk_tanker
                                        KPIStruct.bag_cargo += bag_cargo
                                        KPIStruct.tipper += tipper

                                        KPIStruct.SYstem_Workshop = workshop + KPIStruct.SYstem_Workshop
                                        KPIStruct.SYstem_ServiceScheduled = serviceScheduled + KPIStruct.SYstem_ServiceScheduled

                                        KPIStruct.Oss_Active = ossactive + KPIStruct.Oss_Active
                                        KPIStruct.Oss_InActive = ossinactive + KPIStruct.Oss_InActive
                                        KPIStruct.Oss_Total = osstotal + KPIStruct.Oss_Total
                                        TransporterDict.Item(dr("username").ToString.ToUpper()) = KPIStruct
                                    End If
                                    ' End If
                                Catch ex As Exception
                                    Response.Write("654" & ex.Message)
                                End Try
                            Else
                                ' Response.Write("Ok2")
                                Try
                                    e_active = 0

                                    e_bag_cargo = 0
                                    e_bulk_tanker = 0
                                    e_tipper = 0

                                    e_inactive = 0
                                    e_total = 0
                                    e_workshop = 0
                                    e_serviceScheduled = 0
                                    e_ossactive = 0
                                    e_ossinactive = 0
                                    e_osstotal = 0
                                    e_ossworkshop = 0
                                    e_ossserviceScheduled = 0

                                    Dim mr As Maintenance

                                    Try
                                        If (vehicleTrackedDict.ContainsKey(dr("plateno"))) Then
                                            vr = vehicleTrackedDict.Item(dr("plateno"))
                                            If dr("type").ToString().ToUpper() = "CARGO" Then
                                                e_bag_cargo += 1
                                                e_Total_bag_cargo += 1
                                            ElseIf dr("type").ToString().ToUpper() = "TIPPER" Then
                                                e_tipper += 1
                                                e_Total_tipper += 1
                                            ElseIf dr("type").ToString().ToUpper() = "TANKER" Then
                                                e_bulk_tanker += 1
                                                e_Total_bulk_tanker += 1
                                            End If
                                            If (maintenanceDict.ContainsKey(dr("plateno"))) Then
                                                mr = maintenanceDict.Item(dr("plateno"))
                                                If (mr.timestamp > vr.timeStamp) Then
                                                    e_inactive = e_inactive + 1
                                                    e_totalInActive += 1
                                                    If mr.status.ToUpper() = "WORKSHOP" Or mr.status.ToUpper() = "BATTERY TAKEN OUT" Or mr.status.ToUpper() = "POWER CUT" Or mr.status.ToUpper() = "SPARE TRUCK" Or mr.status.ToUpper() = "NOT IN OPERATION" Or mr.status.ToUpper() = "ACCIDENT" Then
                                                        e_workshop = e_workshop + 1
                                                        e_tworkshop += 1
                                                    ElseIf mr.status.ToUpper() = "SERVICE SCHEDULED" Then
                                                        e_serviceScheduled = e_serviceScheduled + 1
                                                        e_tService += 1
                                                    Else
                                                        ''''e_inactive = e_inactive + 1
                                                        ''''e_totalInActive += 1
                                                    End If
                                                Else
                                                    If ((DateTime.Now - vr.timeStamp).TotalHours > 24) Then
                                                        e_inactive = e_inactive + 1
                                                        e_totalInActive += 1
                                                    Else
                                                        e_active = e_active + 1
                                                        e_totalActive += 1
                                                    End If
                                                End If
                                            Else
                                                If ((DateTime.Now - vr.timeStamp).TotalHours < 24) Then
                                                    e_active = e_active + 1
                                                    e_totalActive += 1
                                                Else
                                                    e_inactive = e_inactive + 1
                                                    e_totalInActive += 1
                                                    ' tinactive += 1
                                                End If
                                            End If
                                        Else
                                            e_inactive = e_inactive + 1
                                            'tinactive += 1
                                        End If

                                        Dim plateno As String() = dr("plateno").ToString().Split("_")
                                        Dim found As Boolean = False

                                        For i As Byte = 0 To plateno.Length - 1
                                            If (ossDict.ContainsKey(plateno(i))) Then
                                                found = True
                                                Exit For
                                            End If
                                        Next

                                        If found = True Then
                                            If (vehicleTrackedDict.ContainsKey(dr("plateno"))) Then
                                                vr = vehicleTrackedDict.Item(dr("plateno"))
                                                If ((DateTime.Now - vr.timeStamp).TotalHours < 24) Then
                                                    e_ossactive = e_ossactive + 1
                                                    e_tossactive = e_tossactive + 1
                                                    e_tactive = e_tactive + 1
                                                Else
                                                    e_ossinactive = e_ossinactive + 1
                                                    e_tossinactive = e_tossinactive + 1
                                                    e_tinactive = e_tinactive + 1
                                                End If
                                            Else
                                                e_ossinactive = e_ossinactive + 1
                                                e_tossinactive = e_tossinactive + 1
                                                e_tinactive = e_tinactive + 1
                                            End If

                                            e_osstotal = e_osstotal + 1
                                            e_tosstotal = e_tosstotal + 1
                                        End If

                                        e_total = e_total + 1
                                        e_ttotal = e_ttotal + 1
                                        e_totalTotal += 1
                                        '    Response.Write(e_totalTotal.ToString() & "<br/>")
                                    Catch ex As Exception
                                        Response.Write("7866" & ex.Message)
                                    End Try

                                    '  If e_total > e_inactive Then
                                    If Not e_TransporterDict.ContainsKey(dr("transportername").ToString.ToUpper()) Then
                                        Dim KPIStructNew As KPIStruct
                                        KPIStructNew.SYstem_Active = e_active
                                        KPIStructNew.bulk_tanker = e_bulk_tanker
                                        KPIStructNew.bag_cargo = e_bag_cargo
                                        KPIStructNew.tipper = e_tipper
                                        KPIStructNew.SYstem_InActive = e_inactive
                                        KPIStructNew.SYstem_Workshop = e_workshop
                                        KPIStructNew.SYstem_ServiceScheduled = e_serviceScheduled

                                        'KPIStructNew.bulk_tanker = bulk_tanker
                                        'KPIStructNew.bag_cargo = bag_cargo
                                        'KPIStructNew.tipper = tipper

                                        KPIStructNew.SYstem_Total = e_total
                                        KPIStructNew.IsInternalType = Convert.ToBoolean(dr("internaltype"))
                                        KPIStructNew.Oss_Active = e_ossactive
                                        KPIStructNew.Oss_InActive = e_ossinactive
                                        KPIStructNew.Oss_Total = e_osstotal

                                        KPIStructNew.username = dr("username").ToString().ToUpper()
                                        KPIStructNew.userid = Convert.ToInt32(dr("userid"))
                                        KPIStructNew.transporterId = Convert.ToInt32(dr("transporter_id"))
                                        e_TransporterDict(dr("transportername").ToString.ToUpper()) = KPIStructNew


                                    Else
                                        Dim KPIStruct As KPIStruct = e_TransporterDict.Item(dr("transportername").ToString.ToUpper())
                                        KPIStruct.SYstem_Active = e_active + KPIStruct.SYstem_Active
                                        KPIStruct.SYstem_InActive = e_inactive + KPIStruct.SYstem_InActive
                                        KPIStruct.SYstem_Total = e_total + KPIStruct.SYstem_Total

                                        KPIStruct.bulk_tanker += e_bulk_tanker
                                        KPIStruct.bag_cargo += e_bag_cargo
                                        KPIStruct.tipper += e_tipper

                                        KPIStruct.SYstem_Workshop = e_workshop + KPIStruct.SYstem_Workshop
                                        KPIStruct.SYstem_ServiceScheduled = e_serviceScheduled + KPIStruct.SYstem_ServiceScheduled

                                        KPIStruct.Oss_Active = e_ossactive + KPIStruct.Oss_Active
                                        KPIStruct.Oss_InActive = e_ossinactive + KPIStruct.Oss_InActive
                                        KPIStruct.Oss_Total = e_osstotal + KPIStruct.Oss_Total
                                        e_TransporterDict.Item(dr("transportername").ToString.ToUpper()) = KPIStruct
                                    End If
                                    '  End If
                                Catch ex As Exception
                                    Response.Write("244" & ex.Message)
                                End Try
                            End If
                        Else
                            '  Response.Write(dr("plateno").ToString() & "<br/>")
                        End If

                    Catch ex As Exception
                        Response.Write("577" & ex.Message)
                    End Try


                End While
                counter = 1
                Dim FinalTable As KPIStruct
                Dim prevKey As String = ""

                For Each key As String In TransporterDict.Keys
                    FinalTable = TransporterDict.Item(key)
                    r = t.NewRow()
                    r(0) = counter.ToString()
                    r(1) = FinalTable.username
                    If key.ToString() = "" Then
                        r(2) = "N/A"
                    Else
                        r(2) = key.ToUpper()
                    End If
                    ' Response.Write("key is " & key & " and inactive is :" & FinalTable.SYstem_InActive.ToString())
                    r(3) = FinalTable.SYstem_Active.ToString()
                    r(4) = "<span style='color:blue;cursor:pointer;text-decoration: underline;' onclick='openDetails(0," & FinalTable.userid & ",0)' >" & FinalTable.SYstem_InActive.ToString() & "</span>"
                    r(5) = FinalTable.SYstem_Total.ToString("0")
                    r(6) = (((FinalTable.SYstem_Active + FinalTable.SYstem_Workshop + FinalTable.SYstem_ServiceScheduled) / FinalTable.SYstem_Total) * 100).ToString("0.00")
                    r(7) = ""
                    r(8) = FinalTable.Oss_Total.ToString("0")
                    r(9) = FinalTable.Oss_Active.ToString("0")
                    r(10) = "<span style='color:blue;cursor:pointer;text-decoration: underline;' onclick='openDetails(0," & FinalTable.userid & ",3)' >" & FinalTable.Oss_InActive.ToString() & "</span>"
                    If (FinalTable.Oss_Total = 0) Then
                        r(11) = "--"
                        r(12) = "--"
                    Else
                        r(11) = ((FinalTable.Oss_Active / FinalTable.Oss_Total) * 100).ToString("0.00")
                        r(12) = ((FinalTable.Oss_InActive / FinalTable.Oss_Total) * 100).ToString("0.00")
                    End If
                    r(13) = "<span style='color:blue;cursor:pointer;text-decoration: underline;' onclick='openDetails(0," & FinalTable.userid & ",1)' >" & FinalTable.SYstem_Workshop & "</span>"
                    r(14) = "<span style='color:blue;cursor:pointer;text-decoration: underline;' onclick='openDetails(0," & FinalTable.userid & ",2)' >" & FinalTable.SYstem_ServiceScheduled & "</span>"
                    r(15) = (FinalTable.SYstem_InActive - (FinalTable.SYstem_Workshop + FinalTable.SYstem_ServiceScheduled)).ToString()
                    If FinalTable.IsInternalType Then
                        r(16) = "Internal"
                    Else
                        r(16) = "External"
                    End If

                    r(17) = FinalTable.bag_cargo.ToString()
                    r(18) = FinalTable.bulk_tanker.ToString()
                    r(19) = FinalTable.tipper.ToString()

                    t.Rows.Add(r)
                    counter = counter + 1
                Next

                For Each key As String In e_TransporterDict.Keys
                    FinalTable = e_TransporterDict.Item(key)
                    r = e_t.NewRow()
                    r(0) = counter.ToString()
                    r(1) = FinalTable.username
                    If key.ToString() = "" Then
                        r(2) = "N/A"
                    Else
                        r(2) = key.ToString.ToUpper()
                    End If
                    r(3) = FinalTable.SYstem_Active.ToString()
                    r(4) = "<span style='color:blue;cursor:pointer;text-decoration: underline;' onclick='openDetails(1," & FinalTable.transporterId & ",0)' >" & FinalTable.SYstem_InActive.ToString() & "</span>"
                    r(5) = FinalTable.SYstem_Total.ToString("0")
                    r(6) = (((FinalTable.SYstem_Active + FinalTable.SYstem_Workshop + FinalTable.SYstem_ServiceScheduled) / FinalTable.SYstem_Total) * 100).ToString("0.00")
                    r(7) = ""
                    r(8) = FinalTable.Oss_Total.ToString("0")
                    r(9) = FinalTable.Oss_Active.ToString("0")
                    r(10) = "<span style='color:blue;cursor:pointer;text-decoration: underline;' onclick='openDetails(1," & FinalTable.transporterId & ",3)' >" & FinalTable.Oss_InActive.ToString() & "</span>"
                    If (FinalTable.Oss_Total = 0) Then
                        r(11) = "--"
                        r(12) = "--"
                    Else
                        r(11) = ((FinalTable.Oss_Active / FinalTable.Oss_Total) * 100).ToString("0.00")
                        r(12) = ((FinalTable.Oss_InActive / FinalTable.Oss_Total) * 100).ToString("0.00")
                    End If
                    r(13) = "<span style='color:blue;cursor:pointer;text-decoration: underline;' onclick='openDetails(1," & FinalTable.transporterId & ",1)' >" & FinalTable.SYstem_Workshop & "</span>"
                    r(14) = "<span style='color:blue;cursor:pointer;text-decoration: underline;' onclick='openDetails(1," & FinalTable.transporterId & ",2)' >" & FinalTable.SYstem_ServiceScheduled & "</span>"
                    r(15) = (FinalTable.SYstem_InActive - (FinalTable.SYstem_Workshop + FinalTable.SYstem_ServiceScheduled)).ToString()
                    If FinalTable.IsInternalType Then
                        r(16) = "Internal"
                    Else
                        r(16) = "External"
                    End If

                    r(17) = FinalTable.bag_cargo.ToString()
                    r(18) = FinalTable.bulk_tanker.ToString()
                    r(19) = FinalTable.tipper.ToString()
                    e_t.Rows.Add(r)
                    counter = counter + 1
                Next

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
                    t.Rows.Add(r)
                Else
                    r = t.NewRow
                    r(0) = ""
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
                    r(12) = ""
                    r(13) = ""
                    r(14) = ""
                    r(15) = ""
                    r(16) = ""
                    r(17) = ""
                    r(18) = ""
                    r(19) = ""
                    t.Rows.Add(r)

                    ' r = t.NewRow

                    'r(0) = ""
                    'r(1) = "TOTAL"
                    'r(2) = "TOTAL"
                    'r(3) = totalActive.ToString()
                    'r(4) = totalInActive.ToString()
                    'r(5) = totalTotal.ToString("0")
                    ''r(6) = ((tactive / ttotal) * 100).ToString("0.00")
                    ''r(7) = ((tinactive / ttotal) * 100).ToString("0.00")
                    'r(6) = (((totalActive + tworkshop + tService) / totalTotal) * 100).ToString("0.00")
                    'r(7) = ""
                    'r(8) = tosstotal.ToString()
                    'r(9) = tossactive.ToString()
                    'r(10) = tossinactive.ToString()
                    'r(11) = ((tossactive / tosstotal) * 100).ToString("0.00")
                    'r(12) = ((tossinactive / tosstotal) * 100).ToString("0.00")
                    'r(13) = tworkshop.ToString()
                    'r(14) = tService.ToString()
                    'r(15) = (totalInActive - (tworkshop + tService)).ToString()
                    'r(17) = Total_bag_cargo.ToString()
                    'r(18) = Total_bulk_tanker.ToString()
                    'r(19) = Total_tipper.ToString()

                    't.Rows.Add(r)

                    'Commentedd first Total row
                    r = t.NewRow
                    r(0) = ""
                    r(1) = "TOTAL"
                    r(2) = "TOTAL"
                    r(3) = totalActive.ToString()
                    r(4) = totalInActive.ToString()
                    r(5) = totalTotal.ToString("0")
                    r(6) = (((totalActive + tworkshop + tService) / totalTotal) * 100).ToString("0.00")
                    'r(7) = ((tinactive / ttotal) * 100).ToString("0.00")
                    r(7) = ""
                    r(8) = tosstotal.ToString()
                    r(9) = tossactive.ToString()
                    r(10) = tossinactive.ToString()
                    r(11) = ((tossactive / tosstotal) * 100).ToString("0.00")
                    r(12) = ((tossinactive / tosstotal) * 100).ToString("0.00")
                    r(13) = tworkshop.ToString()
                    r(14) = tService.ToString()
                    r(15) = (totalInActive - (tworkshop + tService)).ToString()
                    r(17) = Total_bag_cargo.ToString()
                    r(18) = Total_bulk_tanker.ToString()
                    r(19) = Total_tipper.ToString()
                    t.Rows.InsertAt(r, 0)

                    r = t.NewRow
                    r(0) = ""
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
                    r(12) = ""
                    r(13) = ""
                    r(14) = ""
                    r(15) = ""
                    r(16) = ""
                    r(17) = ""
                    r(18) = ""
                    r(19) = ""
                    t.Rows.InsertAt(r, 1)


                End If


                If e_t.Rows.Count = 0 Then
                    r = e_t.NewRow
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
                    e_t.Rows.Add(r)
                Else
                    r = e_t.NewRow
                    r(0) = ""
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
                    r(12) = ""
                    r(13) = ""
                    r(14) = ""
                    r(15) = ""
                    r(16) = ""
                    r(17) = ""
                    r(18) = ""
                    r(19) = ""
                    e_t.Rows.Add(r)

                    'r = e_t.NewRow

                    'r(0) = ""
                    'r(1) = "TOTAL"
                    'r(2) = "TOTAL"
                    'r(3) = e_totalActive.ToString()
                    'r(4) = e_totalInActive.ToString()
                    'r(5) = e_totalTotal.ToString("0")
                    'r(6) = (((e_totalActive + e_tworkshop + e_tService) / e_totalTotal) * 100).ToString("0.00")
                    'r(7) = ""
                    'r(8) = e_tosstotal.ToString()
                    'r(9) = e_tossactive.ToString()
                    'r(10) = e_tossinactive.ToString()
                    'r(11) = ((e_tossactive / e_tosstotal) * 100).ToString("0.00")
                    'r(12) = ((e_tossinactive / e_tosstotal) * 100).ToString("0.00")
                    'r(13) = e_tworkshop.ToString()
                    'r(14) = e_tService.ToString()
                    'r(15) = (e_totalInActive - (e_tworkshop + e_tService)).ToString()

                    'r(17) = e_Total_bag_cargo.ToString()
                    'r(18) = e_Total_bulk_tanker.ToString()
                    'r(19) = e_Total_tipper.ToString()

                    'e_t.Rows.Add(r)

                    'Commentedd first Total row
                    r = e_t.NewRow
                    r(0) = ""
                    r(1) = "TOTAL"
                    r(2) = "TOTAL"
                    r(3) = e_totalActive.ToString()
                    r(4) = e_totalInActive.ToString()
                    r(5) = e_totalTotal.ToString("0")
                    r(6) = (((e_totalActive + e_tworkshop + e_tService) / e_totalTotal) * 100).ToString("0.00")
                    r(7) = ""
                    r(8) = e_tosstotal.ToString()
                    r(9) = e_tossactive.ToString()
                    r(10) = e_tossinactive.ToString()
                    r(11) = ((e_tossactive / e_tosstotal) * 100).ToString("0.00")
                    r(12) = ((e_tossinactive / e_tosstotal) * 100).ToString("0.00")
                    r(13) = e_tworkshop.ToString()
                    r(14) = e_tService.ToString()
                    r(15) = (e_totalInActive - (e_tworkshop + e_tService)).ToString()
                    r(17) = e_Total_bag_cargo.ToString()
                    r(18) = e_Total_bulk_tanker.ToString()
                    r(19) = e_Total_tipper.ToString()
                    e_t.Rows.InsertAt(r, 0)

                    r = e_t.NewRow
                    r(0) = ""
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
                    r(12) = ""
                    r(13) = ""
                    r(14) = ""
                    r(15) = ""
                    r(16) = ""
                    r(17) = ""
                    r(18) = ""
                    r(19) = ""
                    e_t.Rows.InsertAt(r, 1)

                    r = e_t.NewRow
                    r(0) = ""
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
                    r(12) = ""
                    r(13) = ""
                    r(14) = ""
                    r(15) = ""
                    r(16) = ""
                    r(17) = ""
                    r(18) = ""
                    r(19) = ""
                    e_t.Rows.Add(r)
                    r = e_t.NewRow
                    r(0) = ""
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
                    r(12) = ""
                    r(13) = ""
                    r(14) = ""
                    r(15) = ""
                    r(16) = ""
                    r(17) = ""
                    r(18) = ""
                    r(19) = ""
                    e_t.Rows.Add(r)


                    r = e_t.NewRow
                    r(0) = ""
                    r(1) = "GRAND TOTAL"
                    r(2) = "GRAND TOTAL"
                    r(3) = (totalActive + e_totalActive).ToString()
                    r(4) = (totalInActive + e_totalInActive).ToString()
                    r(5) = (totalTotal + e_totalTotal).ToString("0")
                    r(6) = ((((totalActive + e_totalActive) + (tworkshop + e_tworkshop) + (tService + e_tService)) / (totalTotal + e_totalTotal)) * 100).ToString("0.00")
                    r(7) = ""
                    r(8) = (tosstotal + e_tosstotal).ToString()
                    r(9) = (tossactive + e_tossactive).ToString()
                    r(10) = (tossinactive + e_tossinactive).ToString()
                    r(11) = (((tossactive + e_tossactive) / (tosstotal + e_tosstotal)) * 100).ToString("0.00")
                    r(12) = (((tossinactive + e_tossinactive) / (tosstotal + e_tosstotal)) * 100).ToString("0.00")
                    r(13) = (tworkshop + e_tworkshop).ToString()
                    r(14) = (tService + e_tService).ToString()
                    r(15) = ((totalInActive + e_totalInActive) - ((tworkshop + e_tworkshop) + (tService + e_tService))).ToString()
                    r(17) = (Total_bag_cargo + e_Total_bag_cargo).ToString()
                    r(18) = (Total_bulk_tanker + e_Total_bulk_tanker).ToString()
                    r(19) = (Total_tipper + e_Total_tipper).ToString()
                    e_t.Rows.Add(r)
                End If


                GridView1.Columns(1).Visible = False
                GridView2.Columns(1).Visible = False

                If isInternal = "false" Then
                    GridView1.Columns(3).Visible = False
                    GridView1.Columns(4).Visible = False
                    GridView1.Columns(5).Visible = False
                    GridView1.Columns(6).Visible = False
                    GridView1.Columns(7).Visible = False
                    GridView1.Columns(8).Visible = False

                    GridView2.Columns(3).Visible = False
                    GridView2.Columns(4).Visible = False
                    GridView2.Columns(5).Visible = False
                    GridView2.Columns(6).Visible = False
                    GridView2.Columns(7).Visible = False
                    GridView2.Columns(8).Visible = False
                End If
                If tactive = 0 And tinactive = 0 Then
                    tinactive = 100
                End If
                If e_tactive = 0 And e_tinactive = 0 Then
                    e_tinactive = 100
                End If

                GridView1.DataSource = t
                GridView1.DataBind()

                GridView2.DataSource = e_t
                GridView2.DataBind()

            Catch ex As Exception

            Finally

                conn.Close()
            End Try

        Catch ex As Exception

        End Try
    End Sub

    Private Structure Maintenance
        Dim timestamp As DateTime
        Dim statusdate As DateTime
        Dim status As String
        Dim Remarks As String
        Dim sourcename As String
    End Structure
    Private Structure KPIStruct
        Dim SYstem_Total As Integer
        Dim SYstem_Active As Integer
        Dim SYstem_InActive As Integer
        Dim SYstem_Workshop As Integer
        Dim SYstem_ServiceScheduled As Integer
        Dim SYstem_ActivePercent As Double
        Dim SYstem_InactivePercent As Double

        Dim Oss_Total As Integer
        Dim Oss_Active As Integer
        Dim Oss_InActive As Integer
        Dim Oss_ActivePercent As Double
        Dim Oss_InactivePercent As Double

        Dim userid As Integer
        Dim username As String
        Dim transporterId As Integer
        Dim IsInternalType As Boolean

        Dim bag_cargo As Integer
        Dim tipper As Integer
        Dim bulk_tanker As Integer
    End Structure

    Private Structure VehicleTrackedRecord
        Dim plateno As String
        Dim timeStamp As DateTime
        Dim gpsAV As Char
        Dim lat As Double
        Dim lon As Double
        Dim speed As Double
        Dim direction As Double
        Dim odometer As Double
        Dim ignition As Boolean
        Dim overspeed As Boolean
        Dim power As Boolean
        Dim immobilizer As Boolean
        Dim fuel1 As Double
        Dim fuel2 As Double
        Dim fuel As Double
        Dim pto As Boolean
    End Structure
End Class