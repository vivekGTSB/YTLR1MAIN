Imports AspMap
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class GetSmartFleetsek
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

            If Request.Cookies("userinfo")("companyname") = "LAFARGE" Then
                isLafarge = True
            End If


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
            Dim cmd As New SqlCommand("select * from vehicle_tracked2", conn)

            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim vehicleTrackedDict As New Dictionary(Of String, VehicleTrackedRecord)

                While dr.Read()
                    Try
                        Dim vtr As New VehicleTrackedRecord()
                        vtr.plateno = dr("plateno").ToString().Trim().ToUpper()
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
                        ' Return "camehere"
                        vehicleTrackedDict.Add(vtr.plateno, vtr)
                    Catch ex As Exception
                        ' Return ex.Message
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
                    cmd = New SqlCommand("select plateno,timestamp,statusdate,status from maintenance where timestamp>'2012/09/01' order by timestamp desc", conn)
                    dr = cmd.ExecuteReader()
                    While dr.Read()
                        Try
                            Dim m As New Maintenance()

                            m.timestamp = DateTime.Parse(dr("timestamp"))
                            m.statusdate = DateTime.Parse(dr("statusdate"))
                            m.status = dr("status")
                            maintenanceDict.Add(dr("plateno").ToString().Trim().ToUpper(), m)
                        Catch ex As Exception

                        End Try
                    End While
                Catch ex As Exception
                    WriteLog("Maintenance - " & suserid & " - " & ex.Message)
                End Try

                If Request.QueryString("u") Is Nothing Then
                    If role = "User" Then
                        cmd = New SqlCommand("select immobilizer,brand,plateno,unitid,groupname,userid,pto,modo,vehicleodometer,VehicleOdoRecDate from vehicleTBL where userid='" & suserid & "' order by plateno", conn)
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select immobilizer,brand,plateno,unitid,groupname,userid,pto,modo,vehicleodometer,VehicleOdoRecDate from vehicleTBL where userid in(" & userslist & ") order by plateno", conn)
                    ElseIf role = "Admin" Then
                        cmd = New SqlCommand("select immobilizer,brand,plateno,unitid,groupname,userid,pto,modo,vehicleodometer,VehicleOdoRecDate from vehicleTBL where userid='" & suserid & "' order by plateno", conn)
                    End If
                ElseIf allusers Then
                    If role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select immobilizer,brand,plateno,unitid,groupname,userid,pto,modo,vehicleodometer,VehicleOdoRecDate from vehicleTBL where userid in (" & userslist & ") order by plateno", conn)
                    Else
                        cmd = New SqlCommand("select immobilizer, brand,plateno,unitid,groupname,userid,pto,modo,vehicleodometer,VehicleOdoRecDate from vehicleTBL order by plateno", conn)
                    End If
                ElseIf suserid.IndexOf(",") > 0 Then
                    cmd = New SqlCommand("select immobilizer,brand,plateno,unitid,groupname,userid,pto,modo,vehicleodometer,VehicleOdoRecDate from vehicleTBL where userid='" & suser & "'  and  groupname='" & sgroup & "' order by plateno", conn)
                ElseIf pno <> "" Then
                    cmd = New SqlCommand("select immobilizer,brand,plateno,unitid,groupname,userid,pto,modo,vehicleodometer,VehicleOdoRecDate from vehicleTBL where plateno='" & pno & "'", conn)
                Else
                    cmd = New SqlCommand("select immobilizer,brand,plateno,unitid,groupname,userid,pto,modo,vehicleodometer,VehicleOdoRecDate from vehicleTBL where userid='" & suserid & "' order by plateno", conn)
                End If

                '   Return cmd.CommandText
                dr = cmd.ExecuteReader()

                Dim vehiclepoint As New AspMap.Point()

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

                Dim bdt As String
                Dim edt As String

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

                                    astatus.Add(amaintenance)
                                Else
                                    If ((DateTime.Now - vr.timeStamp).TotalHours > 24) Then
                                        Dim amaintenance As New ArrayList()

                                        amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                                        amaintenance.Add("Data Not Coming")

                                        astatus.Add(amaintenance)
                                    End If
                                End If
                            Else
                                If ((DateTime.Now - vr.timeStamp).TotalHours > 24) Then
                                    Dim amaintenance As New ArrayList()

                                    amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                                    amaintenance.Add("Data Not Coming")

                                    astatus.Add(amaintenance)
                                End If

                            End If
                            Try
                                If dr("immobilizer") Then
                                    If (vr.immobilizer) Then
                                        Dim amaintenance As New ArrayList()
                                        amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                                        amaintenance.Add("Immobilizer Activated")
                                        astatus.Add(amaintenance)
                                    End If
                                End If
                            Catch ex As Exception

                            End Try


                            If (vr.power) Then
                                Dim amaintenance As New ArrayList()
                                amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                                amaintenance.Add("Power Cut")

                                astatus.Add(amaintenance)
                            End If

                            If (astatus.Count = 0) Then
                                astatus.Add(New String() {"--", "--"})
                            End If

                            a.Add(astatus)
                            a.Add(plateno)

                            a.Add(dr("groupname").ToString().ToUpper())

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

                            Dim Odometerdetails As New ArrayList()
                            Dim lastodometer As Double = 0
                            If IsDBNull(dr("vehicleodometer")) Then
                                Odometerdetails.Add(0)
                            Else
                                Odometerdetails.Add(dr("vehicleodometer"))
                            End If
                            If IsDBNull(dr("vehicleodometer")) Then
                                Odometerdetails.Add("")
                            Else
                                Odometerdetails.Add(Convert.ToDateTime(dr("VehicleOdoRecDate")).ToString("yyyy/MM/dd HH:mm:ss"))
                            End If
                            If IsDBNull(dr("vehicleodometer")) Then
                                Odometerdetails.Add(0)
                            Else
                                Odometerdetails.Add(Convert.ToInt32(dr("modo")))
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
                            a.Add(Odometerdetails)
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

                            If Not IsDBNull(dr("brand")) Then
                                a.Add(dr("brand").ToString())
                            Else
                                a.Add("--")
                            End If
                            rowcounter = rowcounter + 1
                        Else
                            'a.Add("1")
                            'a.Add(New String() {"--", "--"})
                            'a.Add(plateno)
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add(New String() {"--", "--", "--"})
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            'a.Add("--")
                            a.Add("--")
                            a.Add(New String() {"--", "--"})
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
                            a.Add(New String() {"--", "--", "--"})
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
                    a.Add(New String() {"--", "--", "--"})
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
    End Structure

    Protected Sub WriteLog(ByVal message As String)
        Try
            If (message.Length > 0) Then
                Dim sw As New StreamWriter(Server.MapPath("") & "\GetData.Log.txt", FileMode.Append)
                sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & " - " & message)
                sw.Close()
            End If
        Catch ex As Exception

        End Try
    End Sub

End Class
