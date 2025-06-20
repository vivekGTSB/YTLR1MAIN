Imports AspMap
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class GetSmartFleetAPK
    Inherits System.Web.UI.Page

    Public connstr As String
    Public suserid As String
    Public suser As String
    Public sgroup As String
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Response.Write(GetJson())
            Response.ContentType = "text/plain"
        Catch ex As Exception

        End Try
    End Sub

    Protected Function GetJson() As String
        Dim json As String = ""
        Try

            Dim userid As String = Request.Cookies("userinfo")("userid")

            Dim isLafarge As Boolean = False
            Dim ossDict As New Dictionary(Of String, OSSInfo)
            Dim distanceDict As New Dictionary(Of Int32, AspMap.Point)
            If Request.Cookies("userinfo")("companyname") = "LAFARGE" Then
                isLafarge = True
            End If
            Dim map As New AspMap.Map
            Dim centriodpoint As Point


            Dim allusers As Boolean = False
            Dim uu As String = ""
            suserid = Request.QueryString("u")
            Dim pno As String = ""
            If Request.QueryString("u") Is Nothing Then
                suserid = userid
                uu = suser
                If Request.Cookies("userinfo")("role") = "Admin" Then
                    suserid = "1990"
                End If
            End If

            If suserid.IndexOf(",") > 0 Then
                Dim sgroupname As String() = suserid.Split(",")
                suser = sgroupname(0)
                uu = suser
                sgroup = sgroupname(1)
            End If

            If suserid.IndexOf(";") > 0 Then
                Dim sgroupname As String() = suserid.Split(";")
                suser = sgroupname(0)
                uu = suser
                pno = sgroupname(1)
            End If


            If suserid = "All" Then
                allusers = True
            End If




            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            connstr = System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")
            Dim conn As New SqlConnection(connstr)
            '  Dim connOss As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))

            Dim cmd As New SqlCommand("select tr.*,tra.*,isnull(t1.remark,'') as remark,isnull(t1.status,'') as status from vehicle_tracked2 tr left join trailer2 tra on tr.trailerid=tra.trailerid left outer join vehicle_status_tracked2 t1 on tr.plateno =t1.plateno", conn)

            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim vehicleTrackedDict As New Dictionary(Of String, VehicleTrackedRecord)

                While dr.Read()
                    Try
                        Dim vtr As New VehicleTrackedRecord()
                        vtr.plateno = dr("plateno").ToString().Trim().ToUpper()
                        vtr.remarks = dr("remark")
                        vtr.recentStatus = dr("status")
                        If Not IsDBNull(dr("trailerno")) Then
                            vtr.trailerid = dr("trailerno").ToString().Trim().ToUpper()
                        Else
                            vtr.trailerid = "--"
                        End If
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

                        If IsDBNull(dr("nearesttown")) Then
                            vtr.nearesttown = "-"
                        Else
                            vtr.nearesttown = dr("nearesttown")
                        End If

                        If IsDBNull(dr("milepoint")) Then
                            vtr.milepoint = "-"
                        Else
                            vtr.milepoint = dr("milepoint")
                        End If

                        If IsDBNull(dr("lafargegeofence")) Then
                            vtr.lafargegeofence = "-"
                        Else
                            vtr.lafargegeofence = dr("lafargegeofence")
                        End If

                        If IsDBNull(dr("publicgeofence")) Then
                            vtr.publicgeofence = "-"
                        Else
                            vtr.publicgeofence = dr("publicgeofence")
                        End If

                        If IsDBNull(dr("privategeofence")) Then
                            vtr.privategeofence = "-"
                        Else
                            vtr.privategeofence = dr("privategeofence")

                        End If
                        If IsDBNull(dr("poiname")) Then
                            vtr.poiname = "-"
                        Else
                            vtr.poiname = dr("poiname")
                        End If
                        If IsDBNull(dr("roadname")) Then
                            vtr.roadname = "-"
                        Else
                            vtr.roadname = dr("roadname")
                        End If

                        If IsDBNull(dr("externalbatv")) Then
                            vtr.batVolt = "-"
                        Else
                            vtr.batVolt = dr("externalbatv")
                        End If
                        If IsDBNull(dr("publicgeofenceindatetime")) Then
                            vtr.geofenceintime = "-"
                        Else
                            vtr.geofenceintime = Convert.ToDateTime(dr("publicgeofenceindatetime")).ToString("yyyy/MM/dd HH:mm:ss")
                        End If


                        vehicleTrackedDict.Add(vtr.plateno, vtr)
                    Catch ex As Exception
                    End Try
                End While

                Dim idlingDict As New Dictionary(Of String, VehicleIdlingRecord)

                Try
                    cmd = New SqlCommand("select * from vehicle_idling where duration>0", conn)
                    dr = cmd.ExecuteReader()

                    While dr.Read()
                        Try
                            Dim vir As New VehicleIdlingRecord()
                            vir.plateno = dr("plateno").ToString().Trim().ToUpper()
                            vir.starttimeStamp = DateTime.Parse(dr("from"))
                            vir.endtimeStamp = DateTime.Parse(dr("to"))
                            vir.duration = dr("duration")
                            idlingDict.Add(vir.plateno, vir)
                        Catch ex As Exception

                        End Try
                    End While

                Catch ex As Exception
                    WriteLog("Ilding - " & suserid & " - " & ex.Message)
                End Try

                Try
                    dr.Close()
                    distanceDict.Clear()
                    cmd = New SqlCommand("select * from geofence where shiptocode in (select destination_siteid from ytloss.dbo.oss_patch_out where weight_outtime between '" & DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss") & "' and GetDate()  and destination_siteid<>'' and status in (3,5))", conn)
                    dr = cmd.ExecuteReader()
                    While dr.Read()
                        Dim polygonShape As New AspMap.Shape
                        polygonShape.ShapeType = ShapeType.mcPolygonShape

                        Dim shpPoints As New AspMap.Points()
                        Dim points() As String = dr("data").Split(";")
                        Dim values() As String

                        For i As Integer = 0 To points.Length - 1
                            values = points(i).Split(",")
                            If (values.Length = 2) Then
                                shpPoints.AddPoint(Convert.ToDouble(values(0)), Convert.ToDouble(values(1)))
                            End If
                        Next
                        polygonShape.AddPart(shpPoints)
                        centriodpoint = New Point()
                        centriodpoint.X = shpPoints.Centroid.X
                        centriodpoint.Y = shpPoints.Centroid.Y

                        If Not distanceDict.ContainsKey(dr("shiptocode")) Then
                            distanceDict.Add(dr("shiptocode"), centriodpoint)
                        Else
                            distanceDict(dr("shiptocode")) = centriodpoint
                        End If
                    End While

                Catch ex As Exception

                End Try


                Dim conn2 As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                Dim cmd2 As SqlCommand = New SqlCommand("select plateno,t2.PV_DisplayName,weight_outtime,destination_siteid,destination_sitename +' - '+area_code_name as destination,isnull(est_distance,0) as est_distance,est_arrivaltime,status,ata_datetime from oss_patch_out t1 left outer join oss_plant_master t2 on t1.source_supply=t2.pv_plant where weight_outtime between '" & DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss") & "' and GetDate()  and destination_siteid<>'' order by weight_outtime desc", conn2)
                Dim dr2 As SqlDataReader
                Dim ossinfor As OSSInfo
                Try
                    conn2.Open()
                    dr2 = cmd2.ExecuteReader()
                    While dr2.Read()
                        Try
                            '  ossDict.Add(dr2("plateno"), dr2("status"))
                            If ossDict.ContainsKey(dr2("plateno")) Then
                                ossinfor = ossDict(dr2("plateno"))
                                If Convert.ToDateTime(dr2("weight_outtime")) > Convert.ToDateTime(ossinfor.weightout) Then
                                    ossDict.Remove(dr2("plateno"))
                                    ossinfor = New OSSInfo()
                                    ossinfor.weightout = Convert.ToDateTime(dr2("weight_outtime")).ToString("yyyy/MM/dd HH:mm")
                                    If IsDBNull(dr2("est_arrivaltime")) Then
                                        ossinfor.esttimearrival = "-"
                                    Else
                                        ossinfor.esttimearrival = Convert.ToDateTime(dr2("est_arrivaltime")).ToString("yyyy/MM/dd HH:mm")
                                    End If
                                    If IsDBNull(dr2("ata_datetime")) Then
                                        ossinfor.atatimearrival = "-"
                                    Else
                                        ossinfor.atatimearrival = Convert.ToDateTime(dr2("ata_datetime")).ToString("yyyy/MM/dd HH:mm")
                                    End If
                                    ossinfor.destination = dr2("destination")
                                    ossinfor.currdistance = dr2("est_distance")
                                    ossinfor.status = dr2("status")
                                    ossinfor.distance = dr2("est_distance")
                                    ossinfor.source = dr2("PV_DisplayName")
                                    ossinfor.destinationsiteid = dr2("destination_siteid")
                                        ossDict.Add(dr2("plateno"), ossinfor)
                                    End If
                                Else
                                ossinfor = New OSSInfo()
                                ossinfor.weightout = Convert.ToDateTime(dr2("weight_outtime")).ToString("yyyy/MM/dd HH:mm")
                                If IsDBNull(dr2("est_arrivaltime")) Then
                                    ossinfor.esttimearrival = "-"
                                Else
                                    ossinfor.esttimearrival = Convert.ToDateTime(dr2("est_arrivaltime")).ToString("yyyy/MM/dd HH:mm")
                                End If
                                If IsDBNull(dr2("ata_datetime")) Then
                                    ossinfor.atatimearrival = "-"
                                Else
                                    ossinfor.atatimearrival = Convert.ToDateTime(dr2("ata_datetime")).ToString("yyyy/MM/dd HH:mm")
                                End If
                                ossinfor.destination = dr2("destination")
                                ossinfor.distance = dr2("est_distance")
                                ossinfor.status = dr2("status")
                                ossinfor.currdistance = dr2("est_distance")
                                ossinfor.source = dr2("PV_DisplayName")
                                ossinfor.destinationsiteid = dr2("destination_siteid")
                                ossDict.Add(dr2("plateno"), ossinfor)
                            End If

                        Catch ex As Exception

                        End Try
                    End While
                Catch ex As Exception

                Finally
                    conn2.Close()
                End Try


                Dim vehicleStartDict As New Dictionary(Of String, DateTime)
                Try
                    cmd = New SqlCommand("select * from vehicle_start", conn)
                    dr = cmd.ExecuteReader()

                    While dr.Read()
                        Try
                            vehicleStartDict.Add(dr("plateno").ToString().Trim().ToUpper(), dr("timestamp"))
                        Catch ex As Exception

                        End Try
                    End While
                Catch ex As Exception
                    WriteLog("Vehicle Strat - " & suserid & " - " & ex.Message)
                End Try

                Dim maintenanceDict As New Dictionary(Of String, Maintenance)

                Try
                    cmd = New SqlCommand("select plateno,timestamp,statusdate,status,officeremark,sourcename from maintenance where timestamp>'2016/09/01' order by timestamp desc", conn)
                    dr = cmd.ExecuteReader()
                    While dr.Read()
                        Try
                            Dim m As New Maintenance()

                            m.timestamp = DateTime.Parse(dr("timestamp"))
                            m.statusdate = DateTime.Parse(dr("statusdate"))
                            m.status = dr("status")
                            If IsDBNull(dr("officeremark")) Then
                                m.Remarks = ""
                            Else
                                m.Remarks = dr("officeremark").ToString()
                            End If
                            If IsDBNull(dr("sourcename")) Then
                                m.sourcename = ""
                            Else
                                m.sourcename = dr("sourcename").ToString().ToUpper()
                            End If
                            maintenanceDict.Add(dr("plateno").ToString().Trim().ToUpper(), m)
                        Catch ex As Exception

                        End Try
                    End While
                    If Not dr.IsClosed Then
                        dr.Close()
                    End If



                Catch ex As Exception
                    WriteLog("Maintenance - " & suserid & " - " & ex.Message)
                End Try

                If Request.QueryString("u") Is Nothing Then
                    If role = "User" Then
                        cmd = New SqlCommand("select immobilizer,type,plateno,unitid,g.groupname,v.userid,pto,modo,pmid from vehicleTBL v left join vehicle_group g on g.groupid=v.groupid  where v.userid='" & suserid & "' order by plateno", conn)
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select immobilizer,type,plateno,unitid,g.groupname,v.userid,pto,modo,pmid from vehicleTBL v left join vehicle_group g on g.groupid=v.groupid where v.userid in(" & userslist & ") order by plateno", conn)
                    ElseIf role = "Admin" Then
                        cmd = New SqlCommand("select immobilizer,type,plateno,unitid,g.groupname,v.userid,pto,modo,pmid from vehicleTBL v left join vehicle_group g on g.groupid=v.groupid where v.userid='" & suserid & "' order by plateno", conn)
                    End If
                ElseIf allusers Then
                    If role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select immobilizer,type,plateno,unitid,g.groupname,v.userid,pto,modo,pmid from vehicleTBL v left join vehicle_group g on g.groupid=v.groupid where v.userid in (" & userslist & ") order by plateno", conn)
                    Else
                        cmd = New SqlCommand("select immobilizer, type,plateno,unitid,g.groupname,v.userid,pto,modo,pmid from vehicleTBL v left join vehicle_group g on g.groupid=v.groupid order by plateno", conn)
                    End If
                ElseIf suserid.IndexOf(",") > 0 Then
                    cmd = New SqlCommand("select immobilizer,type,plateno,unitid,g.groupname,v.userid,pto,modo,pmid from vehicleTBL v left join vehicle_group g on g.groupid=v.groupid where v.userid='" & suser & "'  and  g.groupname='" & sgroup & "' order by plateno", conn)
                ElseIf pno <> "" Then
                    cmd = New SqlCommand("select immobilizer,type,plateno,unitid,g.groupname,v.userid,pto,modo,pmid from vehicleTBL v left join vehicle_group g on g.groupid=v.groupid where plateno='" & pno & "'", conn)
                Else
                    cmd = New SqlCommand("select immobilizer,type,plateno,unitid,g.groupname,v.userid,pto,modo,pmid from vehicleTBL v left join vehicle_group g on g.groupid=v.groupid where v.userid='" & suserid & "' order by plateno", conn)
                End If

                '   Return cmd.CommandText
                dr = cmd.ExecuteReader()

                'Dim vehiclepoint As New AspMap.Point()

                Dim rowcounter As Int32 = 1
                Dim plateno As String
                Dim vr As VehicleTrackedRecord
                Dim ir As VehicleIdlingRecord
                Dim mr As Maintenance

                Dim compassindex As Byte
                Dim compass() As String = {"N", "NE", "E", "SE", "S", "SW", "W", "NW", "N"}

                Dim jsonsb As New StringBuilder()

                Dim aa As New ArrayList()

                Dim rowno As Integer = 1
                Dim found As Boolean = False
                Dim bdt As String
                Dim edt As String
                Dim ossstatus As String = "0"
                While (dr.Read())
                    Try
                        Dim a As New ArrayList()

                        plateno = dr("plateno").ToString().Trim().ToUpper()

                        If (vehicleTrackedDict.ContainsKey(plateno)) Then
                            vr = vehicleTrackedDict.Item(plateno)

                            a.Add(rowcounter)

                            Dim astatus As New ArrayList()

                            If (maintenanceDict.ContainsKey(plateno)) Then
                                mr = maintenanceDict.Item(plateno)

                                If (mr.timestamp > vr.timeStamp) Then

                                    Dim amaintenance As New ArrayList()

                                    amaintenance.Add(mr.statusdate.ToString("yyyy/MM/dd HH:mm:ss"))
                                    amaintenance.Add(mr.status)
                                    amaintenance.Add(mr.sourcename)
                                    amaintenance.Add(mr.Remarks)
                                    astatus.Add(amaintenance)
                                Else
                                    If ((DateTime.Now - vr.timeStamp).TotalHours > 24) Then
                                        Dim amaintenance As New ArrayList()

                                        amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                                        amaintenance.Add("Data Not Coming")
                                        amaintenance.Add("System")
                                        amaintenance.Add("")
                                        astatus.Add(amaintenance)
                                    End If
                                End If
                            Else
                                If ((DateTime.Now - vr.timeStamp).TotalHours > 24) Then
                                    Dim amaintenance As New ArrayList()

                                    amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                                    amaintenance.Add("Data Not Coming")
                                    amaintenance.Add("System")
                                    amaintenance.Add("")
                                    astatus.Add(amaintenance)
                                End If

                            End If
                            Try
                                If dr("immobilizer") Then
                                    If (vr.immobilizer) Then
                                        Dim amaintenance As New ArrayList()
                                        amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                                        amaintenance.Add("Immobilizer Activated")
                                        amaintenance.Add("System")
                                        amaintenance.Add("")
                                        astatus.Add(amaintenance)
                                    End If
                                End If
                            Catch ex As Exception

                            End Try


                            If (vr.power) Then
                                Dim amaintenance As New ArrayList()
                                amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                                amaintenance.Add("Power Cut")
                                amaintenance.Add("System")
                                amaintenance.Add("")
                                astatus.Add(amaintenance)
                            End If

                            If (astatus.Count = 0) Then
                                astatus.Add(New String() {"--", "--"})
                            End If

                            a.Add(astatus)
                            If Not IsDBNull(dr("pmid")) Then
                                a.Add(dr("pmid").ToString())
                            Else
                                a.Add("--")
                            End If
                            a.Add(plateno)

                            a.Add(dr("groupname").ToString().ToUpper())
                            '********************New Column******5th (starts from 0)************************************************************
                            Try
                                If ossDict.ContainsKey(dr("plateno")) Then
                                    ossstatus = ossDict(dr("plateno")).ToString()
                                    If ossstatus = "7" Or ossstatus = "8" Or ossstatus = "12" Or ossstatus = "13" Then
                                        a.Add("E")
                                    Else
                                        If (dr("pto") = True) Then
                                            If (vr.pto) Then
                                                a.Add("E")
                                            Else
                                                a.Add("Y")
                                            End If
                                        Else
                                            a.Add("Y")
                                        End If
                                    End If
                                Else
                                    a.Add("E")
                                End If
                            Catch ex As Exception

                            End Try
                            '***************************************************************************************
                            a.Add(vr.trailerid.ToString)

                            a.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))


                            If (dr("pto") = True) Then
                                If (vr.pto) Then
                                    a.Add(1)
                                Else
                                    a.Add(0)
                                End If
                            Else
                                a.Add(-1)
                            End If


                            If (vr.ignition) Then
                                a.Add("ON")
                            Else
                                a.Add("OFF")
                            End If


                            If (vr.speed >= 100) Then
                                a.Add("<b style='background-color:#FF8800;color:#FFFFFF;' title='Overspeed = " & vr.speed.ToString("0") & " KM/H" & "'><blink>" & vr.speed.ToString("0") & "</blink></b>")
                            ElseIf (vr.speed >= 90) Then
                                a.Add("<b style='background-color:#FFFF00;color:red;' title='Overspeed = " & vr.speed.ToString("0") & " KM/H" & "'><blink>" & vr.speed.ToString("0") & "</blink></b>")
                            ElseIf (vr.speed >= 80) Then
                                a.Add("<b style='background-color:#FFFF00;color:red;' title='Overspeed = " & vr.speed.ToString("0") & " KM/H" & "'><blink>" & vr.speed.ToString("0") & "</blink></b>")
                            Else
                                a.Add(Convert.ToInt32(vr.speed.ToString("0")))
                            End If


                            If (vr.odometer > 0) Then
                                a.Add(Convert.ToInt32(vr.odometer))
                            Else
                                a.Add("--")
                            End If
                            If Not IsDBNull(dr("modo")) Then
                                a.Add(dr("modo"))
                            Else
                                a.Add("-")
                            End If
                            Dim fuel1 As Integer = Convert.ToInt32(vr.fuel1)
                            Dim fuel2 As Integer = Convert.ToInt32(vr.fuel2)

                            If (fuel1 = -1) Then
                                Try
                                    bdt = vr.timeStamp.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss")
                                    edt = vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss")
                                    cmd = New SqlCommand("select top 1 volume1,volume2 from  vehicle_history2 where plateno='" & vr.plateno & "' and timestamp between '" & bdt & "' and '" & edt & "' and volume1 > 0 order by timestamp desc", conn)
                                    Dim tdr As SqlDataReader = cmd.ExecuteReader()

                                    If tdr.Read() Then
                                        fuel1 = tdr("volume1")
                                        fuel2 = tdr("volume2")
                                    End If
                                Catch ex As Exception

                                End Try
                            End If
                            a.Add(Convert.ToInt32(fuel1))
                            a.Add(Convert.ToInt32(fuel2))
                            Dim aidling As New ArrayList()
                            Try
                                If (idlingDict.ContainsKey(plateno)) Then
                                    If (vr.ignition = True And vr.speed = 0) Then
                                        ir = idlingDict.Item(plateno)
                                        aidling.Add(vr.plateno)
                                        aidling.Add(ir.starttimeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                                        aidling.Add(ir.duration.ToString("0"))
                                        a.Add(aidling)
                                    Else
                                        a.Add(New String() {"--", "--", "--"})
                                    End If
                                Else
                                    If (vr.ignition = True And vr.speed = 0) Then
                                        aidling.Add(vr.plateno)
                                        aidling.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                                        aidling.Add("1")

                                        a.Add(aidling)
                                    Else
                                        a.Add(New String() {"--", "--", "--"})
                                    End If
                                End If
                            Catch ex As Exception
                                a.Add(New String() {"--", "--", "--"})
                            End Try
                            a.Add(Convert.ToDouble(vr.lat.ToString("0.000000")))
                            a.Add(Convert.ToDouble(vr.lon.ToString("0.000000")))
                            compassindex = ((vr.direction - 22.5 + 22.5) / 45)
                            a.Add(compass(compassindex))

                            Dim loctn As New ArrayList()

                            If isLafarge Then
                                If vr.lafargegeofence <> "-" Then
                                    loctn.Add(1)
                                    loctn.Add(vr.lafargegeofence)
                                ElseIf vr.publicgeofence <> "-" Then
                                    loctn.Add(1)
                                    loctn.Add(vr.publicgeofence)
                                ElseIf vr.roadname <> "-" Then
                                    loctn.Add(3)
                                    loctn.Add(vr.roadname)
                                Else
                                    loctn.Add(0)
                                    loctn.Add(Convert.ToDouble(vr.lat.ToString("0.000000")) & "," & Convert.ToDouble(vr.lon.ToString("0.000000")))
                                End If
                            Else
                                If vr.publicgeofence <> "-" Then
                                    If vr.privategeofence <> "-" Then
                                        loctn.Add(1)
                                        loctn.Add(vr.publicgeofence & ";" & vr.privategeofence)
                                    Else
                                        loctn.Add(1)
                                        loctn.Add(vr.publicgeofence)
                                    End If
                                ElseIf vr.privategeofence <> "-" Then
                                    loctn.Add(1)
                                    loctn.Add(vr.privategeofence)
                                ElseIf vr.poiname <> "-" Then
                                    loctn.Add(2)
                                    loctn.Add(vr.poiname)
                                ElseIf vr.roadname <> "-" Then
                                    loctn.Add(3)
                                    loctn.Add(vr.roadname)
                                Else
                                    loctn.Add(0)
                                    loctn.Add(Convert.ToDouble(vr.lat.ToString("0.000000")) & "," & Convert.ToDouble(vr.lon.ToString("0.000000")))
                                End If
                            End If


                            a.Add(loctn)


                            If vr.nearesttown <> "-" Then
                                a.Add(vr.nearesttown)
                            Else
                                a.Add("--")
                            End If

                            If vr.milepoint <> "-" Then
                                a.Add(vr.milepoint)
                            Else
                                a.Add("--")
                            End If

                            a.Add(plateno)

                            a.Add(Int32.Parse(dr("userid")))

                            a.Add(vr.timeStamp.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss"))

                            If Not IsDBNull(dr("type")) Then
                                a.Add(dr("type").ToString())
                            Else
                                a.Add("--")
                            End If

                            a.Add(vr.batVolt)
                            a.Add(plateno)
                            If ossDict.ContainsKey(plateno) Then
                                found = False
                                ossinfor = ossDict(plateno)
                                a.Add(ossinfor.source)
                                a.Add(ossinfor.destination)
                                a.Add(ossinfor.weightout)
                                If (vr.ignition) Then
                                    Dim vehiclepoint As New Point
                                    vehiclepoint.X = Convert.ToDouble(vr.lon)
                                    vehiclepoint.Y = Convert.ToDouble(vr.lat)
                                    If distanceDict.ContainsKey(ossinfor.destinationsiteid) Then
                                        Dim destcentriodpoint As AspMap.Point = distanceDict(ossinfor.destinationsiteid)
                                        Dim distance As Double = Map.ConvertDistance(Map.MeasureDistance(destcentriodpoint, vehiclepoint), 9102, 9036)
                                        ossinfor.currdistance = distance
                                        found = True
                                    Else
                                        found = False
                                    End If
                                    Dim hours As Double = 0
                                    If found Then
                                        Try
                                            If ossinfor.currdistance < 1 Then
                                                a.Add("Truck ARRIVE Soon")
                                            Else
                                                If vr.speed <= 30 Then
                                                    hours = ossinfor.currdistance / 30
                                                ElseIf vr.speed > 30 And vr.speed <= 60 Then
                                                    hours = ossinfor.currdistance / 45
                                                ElseIf vr.speed > 60 Then
                                                    hours = ossinfor.currdistance / 60
                                                Else
                                                    hours = 0
                                                End If
                                                a.Add(DateTime.Now().AddHours(hours).ToString("yyyy/MM/dd HH:mm:ss"))
                                            End If

                                        Catch ex As Exception

                                        End Try
                                        a.Add(ossinfor.distance)
                                        a.Add(ossinfor.currdistance)
                                        a.Add(ossinfor.destinationsiteid)
                                    Else
                                        If ossinfor.status = "7" Or ossinfor.status = "8" Or ossinfor.status = "12" Or ossinfor.status = "13" Then
                                            a.Add("Delivery Completed")
                                        Else
                                            'If ossinfor.distance = 0 Then
                                            a.Add("-")
                                                'Else
                                                '    Try
                                                '        If vr.speed <= 30 Then
                                                '            hours = ossinfor.distance / 30
                                                '        ElseIf vr.speed > 30 And vr.speed <= 60 Then
                                                '            hours = ossinfor.distance / 45
                                                '        ElseIf vr.speed > 60 Then
                                                '            hours = ossinfor.distance / 60
                                                '        Else
                                                '            hours = 0
                                                '        End If
                                                '    Catch ex As Exception

                                                '    End Try
                                                '    a.Add(Convert.ToDateTime(ossinfor.weightout).AddHours(hours).ToString("yyyy/MM/dd HH:mm:ss"))
                                                'End If
                                            End If
                                        a.Add(ossinfor.distance)
                                        a.Add(0)
                                        a.Add(ossinfor.destinationsiteid)
                                    End If
                                Else
                                    'Dim vehiclepoint As New Point
                                    'vehiclepoint.X = Convert.ToDouble(vr.lon)
                                    'vehiclepoint.Y = Convert.ToDouble(vr.lat)
                                    'If distanceDict.ContainsKey(ossinfor.destinationsiteid) Then
                                    '    Dim destcentriodpoint As AspMap.Point = distanceDict(ossinfor.destinationsiteid)
                                    '    Dim distance As Double = Map.ConvertDistance(Map.MeasureDistance(destcentriodpoint, vehiclepoint), 9102, 9036)
                                    '    ossinfor.currdistance = distance
                                    '    found = True
                                    'Else
                                    '    found = False
                                    'End If

                                    a.Add("Truck STOP")
                                    a.Add(ossinfor.distance)
                                    a.Add(ossinfor.currdistance)
                                    a.Add(ossinfor.destinationsiteid)
                                End If

                            Else
                                a.Add("--")
                                a.Add("--")
                                a.Add("--")
                                a.Add("--")
                                a.Add("--")
                                a.Add("--")
                                a.Add("--")
                            End If
                            a.Add(vr.recentStatus)
                            a.Add(vr.remarks)
                            a.Add(vr.geofenceintime)
                            rowcounter = rowcounter + 1
                        Else
                            a.Add("--")
                            a.Add(New String() {"--", "--"})
                            a.Add("--")
                            a.Add(plateno)
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add(New String() {"--", "--", "--"})
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                            a.Add("--")
                        End If

                        aa.Add(a)
                    Catch ex As Exception
                        WriteLog(ex.Message)
                    End Try
                End While


                If (aa.Count = 0) Then
                    Dim a As New ArrayList()

                    a.Add("--")
                    a.Add(New String() {"--", "--"})
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add(New String() {"--", "--", "--"})
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    a.Add("--")
                    aa.Add(a)
                End If

                Dim jss As New Newtonsoft.Json.JsonSerializer()


                json = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
            Catch ex As Exception
                WriteLog(ex.Message)
            Finally
                conn.Close()
            End Try

        Catch ex As Exception
            WriteLog(ex.Message)
        End Try

        Return json

    End Function

    Private Structure VehicleTrackedRecord
        Dim plateno As String
        Dim trailerid As String
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
        Dim nearesttown As String
        Dim milepoint As String
        Dim lafargegeofence As String
        Dim publicgeofence As String
        Dim privategeofence As String
        Dim poiname As String
        Dim roadname As String
        Dim batVolt As String
        Dim recentStatus As String
        Dim remarks As String
        Dim geofenceintime As String
    End Structure

    Private Structure VehicleIdlingRecord
        Dim plateno As String
        Dim starttimeStamp As DateTime
        Dim endtimeStamp As DateTime
        Dim duration As Integer
    End Structure

    Private Structure Maintenance
        Dim timestamp As DateTime
        Dim statusdate As DateTime
        Dim status As String
        Dim Remarks As String
        Dim sourcename As String
    End Structure

    Private Structure OSSInfo
        Dim weightout As String
        Dim esttimearrival As String
        Dim atatimearrival As String
        Dim source As String
        Dim destination As String
        Dim status As Int32
        Dim distance As Int32
        Dim currdistance As Int32
        Dim destinationsiteid As Int32
    End Structure

    Protected Sub WriteLog(ByVal message As String)
        Try
            If (message.Length > 0) Then
                Dim sw As New StreamWriter(Server.MapPath("") & "\GetDataNew.txt", FileMode.Append)
                sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & " - " & message)
                sw.Close()
            End If
        Catch ex As Exception

        End Try
    End Sub

End Class
