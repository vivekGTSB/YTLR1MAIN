Imports AspMap
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class GetSmartFleetAPK
    Inherits SecurePageBase

    Public connstr As String
    Public suserid As String
    Public suser As String
    Public sgroup As String

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.Write("{""error"":""Unauthorized""}")
                Response.End()
                Return
            End If

            Response.ContentType = "application/json"
            Response.Write(GetJson())
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GENERAL_ERROR", "Error in GetSmartFleetApk: " & ex.Message)
            Response.StatusCode = 500
            Response.Write("{""error"":""Internal server error""}")
        End Try
    End Sub

    Protected Function GetJson() As String
        Dim json As String = ""
        Try
            ' SECURITY FIX: Get user data from session
            Dim userid As String = SessionManager.GetCurrentUserId()
            Dim role As String = SessionManager.GetCurrentUserRole()
            Dim userslist As String = If(Session("userslist") IsNot Nothing, Session("userslist").ToString(), "")
            Dim companyName As String = If(Session("companyname") IsNot Nothing, Session("companyname").ToString(), "")

            Dim isLafarge As Boolean = companyName = "LAFARGE"
            Dim ossDict As New Dictionary(Of String, OSSInfo)
            Dim distanceDict As New Dictionary(Of Int32, AspMap.Point)
            Dim map As New AspMap.Map
            Dim centriodpoint As Point

            Dim allusers As Boolean = False
            Dim uu As String = ""
            
            ' SECURITY FIX: Validate and sanitize query parameter
            suserid = SecurityHelper.SanitizeForHtml(Request.QueryString("u"))
            If String.IsNullOrEmpty(suserid) Then
                suserid = userid
                uu = suser
                If role = "Admin" Then
                    suserid = "1990"
                End If
            End If

            ' SECURITY FIX: Parse user and group parameters safely
            ParseUserParameters(suserid, uu)

            If suserid = "All" Then
                allusers = True
            End If

            ' SECURITY FIX: Validate users list format
            If Not String.IsNullOrEmpty(userslist) AndAlso Not SecurityHelper.ValidateUsersList(userslist) Then
                userslist = $"'{userid}'"
            End If

            connstr = System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")

            Try
                Using conn As New SqlConnection(connstr)
                    ' Load vehicle tracking data
                    Dim vehicleTrackedDict As Dictionary(Of String, VehicleTrackedRecord) = LoadVehicleTrackingData(conn)
                    Dim idlingDict As Dictionary(Of String, VehicleIdlingRecord) = LoadIdlingData(conn)
                    Dim maintenanceDict As Dictionary(Of String, Maintenance) = LoadMaintenanceData(conn)
                    Dim vehicleStartDict As Dictionary(Of String, DateTime) = LoadVehicleStartData(conn)
                    
                    ' Load OSS data
                    LoadOSSData(ossDict, distanceDict, map)
                    
                    ' Process vehicle data
                    Dim aa As New ArrayList()
                    ProcessVehicleData(conn, vehicleTrackedDict, idlingDict, maintenanceDict, vehicleStartDict, ossDict, distanceDict, map, aa, role, userslist, allusers, isLafarge)
                    
                    json = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
                End Using

            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("DATABASE_ERROR", "Database error in GetSmartFleetApk: " & ex.Message)
                json = "{""error"":""Database error""}"
            End Try

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("JSON_ERROR", "Error generating JSON in GetSmartFleetApk: " & ex.Message)
            json = "{""error"":""Internal server error""}"
        End Try

        Return json
    End Function

    Private Sub ParseUserParameters(ByRef suserid As String, ByRef uu As String)
        Try
            Dim pno As String = ""
            
            If suserid.IndexOf(",") > 0 Then
                Dim sgroupname As String() = suserid.Split(","c)
                suser = SecurityHelper.SanitizeForHtml(sgroupname(0))
                uu = suser
                sgroup = SecurityHelper.SanitizeForHtml(sgroupname(1))
            End If

            If suserid.IndexOf(";") > 0 Then
                Dim sgroupname As String() = suserid.Split(";"c)
                suser = SecurityHelper.SanitizeForHtml(sgroupname(0))
                uu = suser
                pno = SecurityHelper.SanitizeForHtml(sgroupname(1))
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("PARSE_USER_PARAMS_ERROR", "Error parsing user parameters: " & ex.Message)
        End Try
    End Sub

    Private Function LoadVehicleTrackingData(conn As SqlConnection) As Dictionary(Of String, VehicleTrackedRecord)
        Dim vehicleTrackedDict As New Dictionary(Of String, VehicleTrackedRecord)
        
        Try
            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(
                "SELECT tr.*, tra.*, ISNULL(t1.remark,'') as remark, ISNULL(t1.status,'') as status FROM vehicle_tracked2 tr LEFT JOIN trailer2 tra ON tr.trailerid = tra.trailerid LEFT OUTER JOIN vehicle_status_tracked2 t1 ON tr.plateno = t1.plateno", conn)
                
                conn.Open()
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        Try
                            Dim vtr As New VehicleTrackedRecord()
                            PopulateVehicleRecord(vtr, dr)
                            vehicleTrackedDict.Add(vtr.plateno, vtr)
                        Catch ex As Exception
                            SecurityHelper.LogSecurityEvent("VEHICLE_RECORD_ERROR", "Error processing vehicle record: " & ex.Message)
                        End Try
                    End While
                End Using
                conn.Close()
            End Using
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("VEHICLE_TRACKING_ERROR", "Error loading vehicle tracking data: " & ex.Message)
        End Try
        
        Return vehicleTrackedDict
    End Function

    Private Sub PopulateVehicleRecord(vtr As VehicleTrackedRecord, dr As SqlDataReader)
        vtr.plateno = SecurityHelper.SanitizeForHtml(dr("plateno").ToString().Trim().ToUpper())
        vtr.remarks = SecurityHelper.SanitizeForHtml(dr("remark").ToString())
        vtr.recentStatus = SecurityHelper.SanitizeForHtml(dr("status").ToString())
        
        If Not IsDBNull(dr("trailerno")) Then
            vtr.trailerid = SecurityHelper.SanitizeForHtml(dr("trailerno").ToString().Trim().ToUpper())
        Else
            vtr.trailerid = "--"
        End If
        
        vtr.timeStamp = DateTime.Parse(dr("timestamp").ToString())
        vtr.lat = CDbl(dr("lat"))
        vtr.lon = CDbl(dr("lon"))
        vtr.speed = CDbl(dr("speed"))
        vtr.direction = CDbl(dr("bearing"))
        vtr.odometer = CDbl(dr("odometer"))
        vtr.ignition = CBool(dr("ignition"))
        vtr.overspeed = CBool(dr("overspeed"))
        vtr.power =  CBool(dr("powercut"))
        vtr.immobilizer = CBool(dr("immobilizer"))
        vtr.pto = CBool(dr("alarm"))
        vtr.fuel1 = CDbl(dr("volume1"))
        vtr.fuel2 = CDbl(dr("volume2"))
        
        ' Handle nullable fields safely
        vtr.nearesttown = If(IsDBNull(dr("nearesttown")), "-", SecurityHelper.SanitizeForHtml(dr("nearesttown").ToString()))
        vtr.milepoint = If(IsDBNull(dr("milepoint")), "-", SecurityHelper.SanitizeForHtml(dr("milepoint").ToString()))
        vtr.lafargegeofence = If(IsDBNull(dr("lafargegeofence")), "-", SecurityHelper.SanitizeForHtml(dr("lafargegeofence").ToString()))
        vtr.publicgeofence = If(IsDBNull(dr("publicgeofence")), "-", SecurityHelper.SanitizeForHtml(dr("publicgeofence").ToString()))
        vtr.privategeofence = If(IsDBNull(dr("privategeofence")), "-", SecurityHelper.SanitizeForHtml(dr("privategeofence").ToString()))
        vtr.poiname = If(IsDBNull(dr("poiname")), "-", SecurityHelper.SanitizeForHtml(dr("poiname").ToString()))
        vtr.roadname = If(IsDBNull(dr("roadname")), "-", SecurityHelper.SanitizeForHtml(dr("roadname").ToString()))
        vtr.batVolt = If(IsDBNull(dr("externalbatv")), "-", SecurityHelper.SanitizeForHtml(dr("externalbatv").ToString()))
        vtr.geofenceintime = If(IsDBNull(dr("publicgeofenceindatetime")), "-", Convert.ToDateTime(dr("publicgeofenceindatetime")).ToString("yyyy/MM/dd HH:mm:ss"))
        
        If Not vtr.ignition Then
            vtr.speed = 0
        End If
    End Sub

    Private Function LoadIdlingData(conn As SqlConnection) As Dictionary(Of String, VehicleIdlingRecord)
        Dim idlingDict As New Dictionary(Of String, VehicleIdlingRecord)
        
        Try
            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand("SELECT * FROM vehicle_idling WHERE duration > 0", conn)
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        Try
                            Dim vir As New VehicleIdlingRecord()
                            vir.plateno = SecurityHelper.SanitizeForHtml(dr("plateno").ToString().Trim().ToUpper())
                            vir.starttimeStamp = DateTime.Parse(dr("from").ToString())
                            vir.endtimeStamp = DateTime.Parse(dr("to").ToString())
                            vir.duration = CInt(dr("duration"))
                            idlingDict.Add(vir.plateno, vir)
                        Catch ex As Exception
                            SecurityHelper.LogSecurityEvent("IDLING_RECORD_ERROR", "Error processing idling record: " & ex.Message)
                        End Try
                    End While
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("IDLING_DATA_ERROR", "Error loading idling data: " & ex.Message)
        End Try
        
        Return idlingDict
    End Function

    Private Function LoadMaintenanceData(conn As SqlConnection) As Dictionary(Of String, Maintenance)
        Dim maintenanceDict As New Dictionary(Of String, Maintenance)
        
        Try
            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(
                "SELECT plateno, timestamp, statusdate, status, officeremark, sourcename FROM maintenance WHERE timestamp > '2016/09/01' ORDER BY timestamp DESC", conn)
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        Try
                            Dim m As New Maintenance()
                            m.timestamp = DateTime.Parse(dr("timestamp").ToString())
                            m.statusdate = DateTime.Parse(dr("statusdate").ToString())
                            m.status = SecurityHelper.SanitizeForHtml(dr("status").ToString())
                            m.Remarks = If(IsDBNull(dr("officeremark")), "", SecurityHelper.SanitizeForHtml(dr("officeremark").ToString()))
                            m.sourcename = If(IsDBNull(dr("sourcename")), "", SecurityHelper.SanitizeForHtml(dr("sourcename").ToString().ToUpper()))
                            maintenanceDict.Add(SecurityHelper.SanitizeForHtml(dr("plateno").ToString().Trim().ToUpper()), m)
                        Catch ex As Exception
                            SecurityHelper.LogSecurityEvent("MAINTENANCE_RECORD_ERROR", "Error processing maintenance record: " & ex.Message)
                        End Try
                    End While
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("MAINTENANCE_DATA_ERROR", "Error loading maintenance data: " & ex.Message)
        End Try
        
        Return maintenanceDict
    End Function

    Private Function LoadVehicleStartData(conn As SqlConnection) As Dictionary(Of String, DateTime)
        Dim vehicleStartDict As New Dictionary(Of String, DateTime)
        
        Try
            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand("SELECT * FROM vehicle_start", conn)
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        Try
                            vehicleStartDict.Add(SecurityHelper.SanitizeForHtml(dr("plateno").ToString().Trim().ToUpper()), CDate(dr("timestamp")))
                        Catch ex As Exception
                            SecurityHelper.LogSecurityEvent("VEHICLE_START_ERROR", "Error processing vehicle start record: " & ex.Message)
                        End Try
                    End While
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("VEHICLE_START_DATA_ERROR", "Error loading vehicle start data: " & ex.Message)
        End Try
        
        Return vehicleStartDict
    End Function

    Private Sub LoadOSSData(ossDict As Dictionary(Of String, OSSInfo), distanceDict As Dictionary(Of Int32, AspMap.Point), map As AspMap.Map)
        Try
            ' Load geofence data for distance calculations
            Using conn As New SqlConnection(connstr)
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(
                    "SELECT * FROM geofence WHERE shiptocode IN (SELECT destination_siteid FROM ytloss.dbo.oss_patch_out WHERE weight_outtime BETWEEN @startTime AND GETDATE() AND destination_siteid <> '' AND status IN (3,5))", conn)
                    cmd.Parameters.AddWithValue("@startTime", DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss"))
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Try
                                Dim polygonShape As New AspMap.Shape
                                polygonShape.ShapeType = ShapeType.mcPolygonShape
                                
                                Dim shpPoints As New AspMap.Points()
                                Dim points() As String = dr("data").ToString().Split(";"c)
                                
                                For Each pointStr As String In points
                                    Dim values() As String = pointStr.Split(","c)
                                    If values.Length = 2 Then
                                        Dim lat, lon As Double
                                        If Double.TryParse(values(0), lat) AndAlso Double.TryParse(values(1), lon) Then
                                            shpPoints.AddPoint(lat, lon)
                                        End If
                                    End If
                                Next
                                
                                polygonShape.AddPart(shpPoints)
                                Dim centriodpoint As New Point()
                                centriodpoint.X = shpPoints.Centroid.X
                                centriodpoint.Y = shpPoints.Centroid.Y
                                
                                Dim shiptoCode As Integer
                                If Integer.TryParse(dr("shiptocode").ToString(), shiptoCode) Then
                                    If Not distanceDict.ContainsKey(shiptoCode) Then
                                        distanceDict.Add(shiptoCode, centriodpoint)
                                    Else
                                        distanceDict(shiptoCode) = centriodpoint
                                    End If
                                End If
                            Catch ex As Exception
                                SecurityHelper.LogSecurityEvent("GEOFENCE_PROCESSING_ERROR", "Error processing geofence: " & ex.Message)
                            End Try
                        End While
                    End Using
                    conn.Close()
                End Using
            End Using

            ' Load OSS patch data
            Using conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                Using cmd2 As SqlCommand = SecurityHelper.CreateSafeCommand(
                    "SELECT plateno, t2.PV_DisplayName, weight_outtime, destination_siteid, destination_sitename + ' - ' + area_code_name as destination, ISNULL(est_distance,0) as est_distance, est_arrivaltime, status, ata_datetime FROM oss_patch_out t1 LEFT OUTER JOIN oss_plant_master t2 ON t1.source_supply = t2.pv_plant WHERE weight_outtime BETWEEN @startTime AND GETDATE() AND destination_siteid <> '' ORDER BY weight_outtime DESC", conn2)
                    cmd2.Parameters.AddWithValue("@startTime", DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss"))
                    
                    conn2.Open()
                    Using dr2 As SqlDataReader = cmd2.ExecuteReader()
                        While dr2.Read()
                            Try
                                ProcessOSSRecord(dr2, ossDict)
                            Catch ex As Exception
                                SecurityHelper.LogSecurityEvent("OSS_RECORD_ERROR", "Error processing OSS record: " & ex.Message)
                            End Try
                        End While
                    End Using
                End Using
            End Using

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("OSS_DATA_ERROR", "Error loading OSS data: " & ex.Message)
        End Try
    End Sub

    Private Sub ProcessOSSRecord(dr As SqlDataReader, ossDict As Dictionary(Of String, OSSInfo))
        Dim plateno As String = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
        
        If ossDict.ContainsKey(plateno) Then
            Dim ossinfor As OSSInfo = ossDict(plateno)
            If Convert.ToDateTime(dr("weight_outtime")) > Convert.ToDateTime(ossinfor.weightout) Then
                ossDict.Remove(plateno)
                ossinfor = CreateOSSInfo(dr)
                ossDict.Add(plateno, ossinfor)
            End If
        Else
            Dim ossinfor As OSSInfo = CreateOSSInfo(dr)
            ossDict.Add(plateno, ossinfor)
        End If
    End Sub

    Private Function CreateOSSInfo(dr As SqlDataReader) As OSSInfo
        Dim ossinfor As New OSSInfo()
        ossinfor.weightout = Convert.ToDateTime(dr("weight_outtime")).ToString("yyyy/MM/dd HH:mm")
        ossinfor.esttimearrival = If(IsDBNull(dr("est_arrivaltime")), "-", Convert.ToDateTime(dr("est_arrivaltime")).ToString("yyyy/MM/dd HH:mm"))
        ossinfor.atatimearrival = If(IsDBNull(dr("ata_datetime")), "-", Convert.ToDateTime(dr("ata_datetime")).ToString("yyyy/MM/dd HH:mm"))
        ossinfor.destination = SecurityHelper.SanitizeForHtml(dr("destination").ToString())
        ossinfor.currdistance = CInt(dr("est_distance"))
        ossinfor.status = CInt(dr("status"))
        ossinfor.distance = CInt(dr("est_distance"))
        ossinfor.source = SecurityHelper.SanitizeForHtml(dr("PV_DisplayName").ToString())
        ossinfor.destinationsiteid = CInt(dr("destination_siteid"))
        Return ossinfor
    End Function

    Private Sub ProcessVehicleData(conn As SqlConnection, vehicleTrackedDict As Dictionary(Of String, VehicleTrackedRecord), idlingDict As Dictionary(Of String, VehicleIdlingRecord), maintenanceDict As Dictionary(Of String, Maintenance), vehicleStartDict As Dictionary(Of String, DateTime), ossDict As Dictionary(Of String, OSSInfo), distanceDict As Dictionary(Of Int32, AspMap.Point), map As AspMap.Map, aa As ArrayList, role As String, userslist As String, allusers As Boolean, isLafarge As Boolean)
        Try
            ' Build secure vehicle query
            Dim query As String = BuildVehicleQuery(role, userslist, allusers)
            
            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                AddVehicleQueryParameters(cmd, role, userslist, allusers)
                
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    Dim rowcounter As Integer = 1
                    Dim compass() As String = {"N", "NE", "E", "SE", "S", "SW", "W", "NW", "N"}
                    
                    While dr.Read()
                        Try
                            Dim plateno As String = SecurityHelper.SanitizeForHtml(dr("plateno").ToString().Trim().ToUpper())
                            
                            If vehicleTrackedDict.ContainsKey(plateno) Then
                                Dim a As New ArrayList()
                                ProcessVehicleRow(dr, vehicleTrackedDict(plateno), idlingDict, maintenanceDict, ossDict, distanceDict, map, a, rowcounter, compass, isLafarge)
                                aa.Add(a)
                                rowcounter += 1
                            Else
                                ' Add empty row for vehicles without tracking data
                                Dim a As New ArrayList()
                                AddEmptyVehicleRow(a, plateno)
                                aa.Add(a)
                            End If
                        Catch ex As Exception
                            SecurityHelper.LogSecurityEvent("VEHICLE_ROW_ERROR", "Error processing vehicle row: " & ex.Message)
                        End Try
                    End While
                End Using
            End Using

            If aa.Count = 0 Then
                Dim a As New ArrayList()
                AddEmptyVehicleRow(a, "--")
                aa.Add(a)
            End If

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("VEHICLE_DATA_ERROR", "Error processing vehicle data: " & ex.Message)
        End Try
    End Sub

    Private Function BuildVehicleQuery(role As String, userslist As String, allusers As Boolean) As String
        Dim query As String = ""
        
        If Request.QueryString("u") Is Nothing Then
            If role = "User" Then
                query = "SELECT immobilizer, type, plateno, unitid, g.groupname, v.userid, pto, modo, pmid FROM vehicleTBL v LEFT JOIN vehicle_group g ON g.groupid = v.groupid WHERE v.userid = @userid ORDER BY plateno"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If Not String.IsNullOrEmpty(userslist) Then
                    query = $"SELECT immobilizer, type, plateno, unitid, g.groupname, v.userid, pto, modo, pmid FROM vehicleTBL v LEFT JOIN vehicle_group g ON g.groupid = v.groupid WHERE v.userid IN ({userslist}) ORDER BY plateno"
                Else
                    query = "SELECT immobilizer, type, plateno, unitid, g.groupname, v.userid, pto, modo, pmid FROM vehicleTBL v LEFT JOIN vehicle_group g ON g.groupid = v.groupid WHERE v.userid = @userid ORDER BY plateno"
                End If
            ElseIf role = "Admin" Then
                query = "SELECT immobilizer, type, plateno, unitid, g.groupname, v.userid, pto, modo, pmid FROM vehicleTBL v LEFT JOIN vehicle_group g ON g.groupid = v.groupid WHERE v.userid = @userid ORDER BY plateno"
            End If
        ElseIf allusers Then
            If role = "SuperUser" Or role = "Operator" Then
                If Not String.IsNullOrEmpty(userslist) Then
                    query = $"SELECT immobilizer, type, plateno, unitid, g.groupname, v.userid, pto, modo, pmid FROM vehicleTBL v LEFT JOIN vehicle_group g ON g.groupid = v.groupid WHERE v.userid IN ({userslist}) ORDER BY plateno"
                Else
                    query = "SELECT immobilizer, type, plateno, unitid, g.groupname, v.userid, pto, modo, pmid FROM vehicleTBL v LEFT JOIN vehicle_group g ON g.groupid = v.groupid ORDER BY plateno"
                End If
            Else
                query = "SELECT immobilizer, type, plateno, unitid, g.groupname, v.userid, pto, modo, pmid FROM vehicleTBL v LEFT JOIN vehicle_group g ON g.groupid = v.groupid ORDER BY plateno"
            End If
        Else
            ' Handle specific user/group/plate queries
            If suserid.IndexOf(",") > 0 Then
                query = "SELECT immobilizer, type, plateno, unitid, g.groupname, v.userid, pto, modo, pmid FROM vehicleTBL v LEFT JOIN vehicle_group g ON g.groupid = v.groupid WHERE v.userid = @suser AND g.groupname = @sgroup ORDER BY plateno"
            ElseIf Not String.IsNullOrEmpty(sgroup) Then
                query = "SELECT immobilizer, type, plateno, unitid, g.groupname, v.userid, pto, modo, pmid FROM vehicleTBL v LEFT JOIN vehicle_group g ON g.groupid = v.groupid WHERE plateno = @plateno"
            Else
                query = "SELECT immobilizer, type, plateno, unitid, g.groupname, v.userid, pto, modo, pmid FROM vehicleTBL v LEFT JOIN vehicle_group g ON g.groupid = v.groupid WHERE v.userid = @userid ORDER BY plateno"
            End If
        End If
        
        Return query
    End Function

    Private Sub AddVehicleQueryParameters(cmd As SqlCommand, role As String, userslist As String, allusers As Boolean)
        If Request.QueryString("u") Is Nothing Then
            If role = "User" OrElse role = "Admin" OrElse (role = "SuperUser" AndAlso String.IsNullOrEmpty(userslist)) OrElse (role = "Operator" AndAlso String.IsNullOrEmpty(userslist)) Then
                cmd.Parameters.AddWithValue("@userid", SessionManager.GetCurrentUserId())
            End If
        ElseIf Not allusers Then
            If suserid.IndexOf(",") > 0 Then
                cmd.Parameters.AddWithValue("@suser", suser)
                cmd.Parameters.AddWithValue("@sgroup", sgroup)
            ElseIf Not String.IsNullOrEmpty(sgroup) Then
                cmd.Parameters.AddWithValue("@plateno", sgroup)
            Else
                cmd.Parameters.AddWithValue("@userid", suserid)
            End If
        End If
    End Sub

    Private Sub ProcessVehicleRow(dr As SqlDataReader, vr As VehicleTrackedRecord, idlingDict As Dictionary(Of String, VehicleIdlingRecord), maintenanceDict As Dictionary(Of String, Maintenance), ossDict As Dictionary(Of String, OSSInfo), distanceDict As Dictionary(Of Int32, AspMap.Point), map As AspMap.Map, a As ArrayList, rowcounter As Integer, compass() As String, isLafarge As Boolean)
        ' Add row counter
        a.Add(rowcounter)
        
        ' Add status information
        Dim astatus As New ArrayList()
        ProcessStatusInfo(vr, maintenanceDict, dr, astatus)
        a.Add(astatus)
        
        ' Add PMID
        a.Add(If(IsDBNull(dr("pmid")), "--", SecurityHelper.SanitizeForHtml(dr("pmid").ToString())))
        
        ' Add plate number
        a.Add(SecurityHelper.SanitizeForHtml(vr.plateno))
        
        ' Add group name
        a.Add(SecurityHelper.SanitizeForHtml(dr("groupname").ToString().ToUpper()))
        
        ' Add PTO status
        ProcessPTOStatus(vr, dr, ossDict, a)
        
        ' Add trailer ID
        a.Add(SecurityHelper.SanitizeForHtml(vr.trailerid))
        
        ' Add timestamp
        a.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
        
        ' Add PTO value
        If CBool(dr("pto")) Then
            a.Add(If(vr.pto, 1, 0))
        Else
            a.Add(-1)
        End If
        
        ' Add ignition status
        a.Add(If(vr.ignition, "ON", "OFF"))
        
        ' Add speed with overspeed formatting
        ProcessSpeedInfo(vr, a)
        
        ' Add odometer
        a.Add(If(vr.odometer > 0, Convert.ToInt32(vr.odometer), "--"))
        
        ' Add modo
        a.Add(If(IsDBNull(dr("modo")), "-", SecurityHelper.SanitizeForHtml(dr("modo").ToString())))
        
        ' Add fuel levels
        ProcessFuelLevels(vr, a)
        
        ' Add idling information
        ProcessIdlingInfo(vr, idlingDict, a)
        
        ' Add coordinates
        a.Add(Convert.ToDouble(vr.lat.ToString("0.000000")))
        a.Add(Convert.ToDouble(vr.lon.ToString("0.000000")))
        
        ' Add direction
        Dim compassindex As Byte = CByte((vr.direction - 22.5 + 22.5) / 45)
        a.Add(compass(compassindex))
        
        ' Add location information
        ProcessLocationInfo(vr, isLafarge, a)
        
        ' Add additional location data
        a.Add(If(vr.nearesttown <> "-", SecurityHelper.SanitizeForHtml(vr.nearesttown), "--"))
        a.Add(If(vr.milepoint <> "-", SecurityHelper.SanitizeForHtml(vr.milepoint), "--"))
        a.Add(SecurityHelper.SanitizeForHtml(vr.plateno))
        a.Add(Integer.Parse(dr("userid").ToString()))
        a.Add(vr.timeStamp.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss"))
        a.Add(If(IsDBNull(dr("type")), "--", SecurityHelper.SanitizeForHtml(dr("type").ToString())))
        a.Add(SecurityHelper.SanitizeForHtml(vr.batVolt))
        a.Add(SecurityHelper.SanitizeForHtml(vr.plateno))
        
        ' Add OSS information
        ProcessOSSInfo(vr, ossDict, distanceDict, map, a)
        
        ' Add status and remarks
        a.Add(SecurityHelper.SanitizeForHtml(vr.recentStatus))
        a.Add(SecurityHelper.SanitizeForHtml(vr.remarks))
        a.Add(SecurityHelper.SanitizeForHtml(vr.geofenceintime))
    End Sub

    Private Sub ProcessStatusInfo(vr As VehicleTrackedRecord, maintenanceDict As Dictionary(Of String, Maintenance), dr As SqlDataReader, astatus As ArrayList)
        Try
            If maintenanceDict.ContainsKey(vr.plateno) Then
                Dim mr As Maintenance = maintenanceDict(vr.plateno)
                If mr.timestamp > vr.timeStamp Then
                    Dim amaintenance As New ArrayList()
                    amaintenance.Add(mr.statusdate.ToString("yyyy/MM/dd HH:mm:ss"))
                    amaintenance.Add(SecurityHelper.SanitizeForHtml(mr.status))
                    amaintenance.Add(SecurityHelper.SanitizeForHtml(mr.sourcename))
                    amaintenance.Add(SecurityHelper.SanitizeForHtml(mr.Remarks))
                    astatus.Add(amaintenance)
                Else
                    If (DateTime.Now - vr.timeStamp).TotalHours > 24 Then
                        AddDataNotComingStatus(vr, astatus)
                    End If
                End If
            Else
                If (DateTime.Now - vr.timeStamp).TotalHours > 24 Then
                    AddDataNotComingStatus(vr, astatus)
                End If
            End If

            ' Check immobilizer status
            Try
                If CBool(dr("immobilizer")) AndAlso vr.immobilizer Then
                    Dim amaintenance As New ArrayList()
                    amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                    amaintenance.Add("Immobilizer Activated")
                    amaintenance.Add("System")
                    amaintenance.Add("")
                    astatus.Add(amaintenance)
                End If
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("IMMOBILIZER_STATUS_ERROR", "Error checking immobilizer status: " & ex.Message)
            End Try

            ' Check power cut status
            If vr.power Then
                Dim amaintenance As New ArrayList()
                amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                amaintenance.Add("Power Cut")
                amaintenance.Add("System")
                amaintenance.Add("")
                astatus.Add(amaintenance)
            End If

            If astatus.Count = 0 Then
                astatus.Add(New String() {"--", "--"})
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("STATUS_INFO_ERROR", "Error processing status info: " & ex.Message)
            astatus.Add(New String() {"--", "--"})
        End Try
    End Sub

    Private Sub AddDataNotComingStatus(vr As VehicleTrackedRecord, astatus As ArrayList)
        Dim amaintenance As New ArrayList()
        amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
        amaintenance.Add("Data Not Coming")
        amaintenance.Add("System")
        amaintenance.Add("")
        astatus.Add(amaintenance)
    End Sub

    Private Sub ProcessPTOStatus(vr As VehicleTrackedRecord, dr As SqlDataReader, ossDict As Dictionary(Of String, OSSInfo), a As ArrayList)
        Try
            If ossDict.ContainsKey(vr.plateno) Then
                Dim ossstatus As String = ossDict(vr.plateno).status.ToString()
                If ossstatus = "7" Or ossstatus = "8" Or ossstatus = "12" Or ossstatus = "13" Then
                    a.Add("E")
                Else
                    If CBool(dr("pto")) Then
                        a.Add(If(vr.pto, "E", "Y"))
                    Else
                        a.Add("Y")
                    End If
                End If
            Else
                a.Add("E")
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("PTO_STATUS_ERROR", "Error processing PTO status: " & ex.Message)
            a.Add("E")
        End Try
    End Sub

    Private Sub ProcessSpeedInfo(vr As VehicleTrackedRecord, a As ArrayList)
        If vr.speed >= 100 Then
            a.Add($"<b style='background-color:#FF8800;color:#FFFFFF;' title='Overspeed = {vr.speed:0} KM/H'><blink>{vr.speed:0}</blink></b>")
        ElseIf vr.speed >= 90 Then
            a.Add($"<b style='background-color:#FFFF00;color:red;' title='Overspeed = {vr.speed:0} KM/H'><blink>{vr.speed:0}</blink></b>")
        ElseIf vr.speed >= 80 Then
            a.Add($"<b style='background-color:#FFFF00;color:red;' title='Overspeed = {vr.speed:0} KM/H'><blink>{vr.speed:0}</blink></b>")
        Else
            a.Add(Convert.ToInt32(vr.speed))
        End If
    End Sub

    Private Sub ProcessFuelLevels(vr As VehicleTrackedRecord, a As ArrayList)
        Dim fuel1 As Integer = Convert.ToInt32(vr.fuel1)
        Dim fuel2 As Integer = Convert.ToInt32(vr.fuel2)

        ' If fuel data is invalid, try to get recent data
        If fuel1 = -1 Then
            Try
                Using conn As New SqlConnection(connstr)
                    Dim bdt As String = vr.timeStamp.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss")
                    Dim edt As String = vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss")
                    
                    Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(
                        "SELECT TOP 1 volume1, volume2 FROM vehicle_history2 WHERE plateno = @plateno AND timestamp BETWEEN @bdt AND @edt AND volume1 > 0 ORDER BY timestamp DESC", conn)
                        cmd.Parameters.AddWithValue("@plateno", vr.plateno)
                        cmd.Parameters.AddWithValue("@bdt", bdt)
                        cmd.Parameters.AddWithValue("@edt", edt)
                        
                        conn.Open()
                        Using tdr As SqlDataReader = cmd.ExecuteReader()
                            If tdr.Read() Then
                                fuel1 = CInt(tdr("volume1"))
                                fuel2 = CInt(tdr("volume2"))
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("FUEL_HISTORY_ERROR", "Error getting fuel history: " & ex.Message)
            End Try
        End If

        a.Add(Convert.ToInt32(fuel1))
        a.Add(Convert.ToInt32(fuel2))
    End Sub

    Private Sub ProcessIdlingInfo(vr As VehicleTrackedRecord, idlingDict As Dictionary(Of String, VehicleIdlingRecord), a As ArrayList)
        Try
            Dim aidling As New ArrayList()
            
            If idlingDict.ContainsKey(vr.plateno) Then
                If vr.ignition AndAlso vr.speed = 0 Then
                    Dim ir As VehicleIdlingRecord = idlingDict(vr.plateno)
                    aidling.Add(SecurityHelper.SanitizeForHtml(vr.plateno))
                    aidling.Add(ir.starttimeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                    aidling.Add(ir.duration.ToString("0"))
                    a.Add(aidling)
                Else
                    a.Add(New String() {"--", "--", "--"})
                End If
            Else
                If vr.ignition AndAlso vr.speed = 0 Then
                    aidling.Add(SecurityHelper.SanitizeForHtml(vr.plateno))
                    aidling.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                    aidling.Add("1")
                    a.Add(aidling)
                Else
                    a.Add(New String() {"--", "--", "--"})
                End If
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("IDLING_INFO_ERROR", "Error processing idling info: " & ex.Message)
            a.Add(New String() {"--", "--", "--"})
        End Try
    End Sub

    Private Sub ProcessLocationInfo(vr As VehicleTrackedRecord, isLafarge As Boolean, a As ArrayList)
        Try
            Dim loctn As New ArrayList()

            If isLafarge Then
                If vr.lafargegeofence <> "-" Then
                    loctn.Add(1)
                    loctn.Add(SecurityHelper.SanitizeForHtml(vr.lafargegeofence))
                ElseIf vr.publicgeofence <> "-" Then
                    loctn.Add(1)
                    loctn.Add(SecurityHelper.SanitizeForHtml(vr.publicgeofence))
                ElseIf vr.roadname <> "-" Then
                    loctn.Add(3)
                    loctn.Add(SecurityHelper.SanitizeForHtml(vr.roadname))
                Else
                    loctn.Add(0)
                    loctn.Add($"{vr.lat:0.000000},{vr.lon:0.000000}")
                End If
            Else
                If vr.publicgeofence <> "-" Then
                    If vr.privategeofence <> "-" Then
                        loctn.Add(1)
                        loctn.Add($"{SecurityHelper.SanitizeForHtml(vr.publicgeofence)};{SecurityHelper.SanitizeForHtml(vr.privategeofence)}")
                    Else
                        loctn.Add(1)
                        loctn.Add(SecurityHelper.SanitizeForHtml(vr.publicgeofence))
                    End If
                ElseIf vr.privategeofence <> "-" Then
                    loctn.Add(1)
                    loctn.Add(SecurityHelper.SanitizeForHtml(vr.privategeofence))
                ElseIf vr.poiname <> "-" Then
                    loctn.Add(2)
                    loctn.Add(SecurityHelper.SanitizeForHtml(vr.poiname))
                ElseIf vr.roadname <> "-" Then
                    loctn.Add(3)
                    loctn.Add(SecurityHelper.SanitizeForHtml(vr.roadname))
                Else
                    loctn.Add(0)
                    loctn.Add($"{vr.lat:0.000000},{vr.lon:0.000000}")
                End If
            End If

            a.Add(loctn)
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOCATION_INFO_ERROR", "Error processing location info: " & ex.Message)
            Dim loctn As New ArrayList()
            loctn.Add(0)
            loctn.Add($"{vr.lat:0.000000},{vr.lon:0.000000}")
            a.Add(loctn)
        End Try
    End Sub

    Private Sub ProcessOSSInfo(vr As VehicleTrackedRecord, ossDict As Dictionary(Of String, OSSInfo), distanceDict As Dictionary(Of Int32, AspMap.Point), map As AspMap.Map, a As ArrayList)
        Try
            If ossDict.ContainsKey(vr.plateno) Then
                Dim ossinfor As OSSInfo = ossDict(vr.plateno)
                a.Add(SecurityHelper.SanitizeForHtml(ossinfor.source))
                a.Add(SecurityHelper.SanitizeForHtml(ossinfor.destination))
                a.Add(SecurityHelper.SanitizeForHtml(ossinfor.weightout))
                
                If vr.ignition Then
                    Dim vehiclepoint As New Point()
                    vehiclepoint.X = vr.lon
                    vehiclepoint.Y = vr.lat
                    
                    If distanceDict.ContainsKey(ossinfor.destinationsiteid) Then
                        Dim destcentriodpoint As AspMap.Point = distanceDict(ossinfor.destinationsiteid)
                        Dim distance As Double = Map.ConvertDistance(Map.MeasureDistance(destcentriodpoint, vehiclepoint), 9102, 9036)
                        ossinfor.currdistance = CInt(distance)
                        
                        If ossinfor.currdistance < 1 Then
                            a.Add("Truck ARRIVE Soon")
                        Else
                            Dim hours As Double = CalculateETA(vr.speed, ossinfor.currdistance)
                            a.Add(DateTime.Now().AddHours(hours).ToString("yyyy/MM/dd HH:mm:ss"))
                        End If
                        
                        a.Add(ossinfor.distance)
                        a.Add(ossinfor.currdistance)
                        a.Add(ossinfor.destinationsiteid)
                    Else
                        ProcessOSSStatusWithoutDistance(ossinfor, a)
                    End If
                Else
                    a.Add("Truck STOP")
                    a.Add(ossinfor.distance)
                    a.Add(ossinfor.currdistance)
                    a.Add(ossinfor.destinationsiteid)
                End If
            Else
                ' No OSS data
                For i As Integer = 1 To 7
                    a.Add("--")
                Next
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("OSS_INFO_ERROR", "Error processing OSS info: " & ex.Message)
            For i As Integer = 1 To 7
                a.Add("--")
            Next
        End Try
    End Sub

    Private Sub ProcessOSSStatusWithoutDistance(ossinfor As OSSInfo, a As ArrayList)
        If ossinfor.status = 7 OrElse ossinfor.status = 8 OrElse ossinfor.status = 12 OrElse ossinfor.status = 13 Then
            a.Add("Delivery Completed")
        Else
            a.Add("-")
        End If
        a.Add(ossinfor.distance)
        a.Add(0)
        a.Add(ossinfor.destinationsiteid)
    End Sub

    Private Function CalculateETA(speed As Double, distance As Integer) As Double
        If speed <= 30 Then
            Return distance / 30
        ElseIf speed > 30 AndAlso speed <= 60 Then
            Return distance / 45
        ElseIf speed > 60 Then
            Return distance / 60
        Else
            Return 0
        End If
    End Function

    Private Sub AddEmptyVehicleRow(a As ArrayList, plateno As String)
        For i As Integer = 1 To 42
            If i = 4 Then
                a.Add(SecurityHelper.SanitizeForHtml(plateno))
            Else
                a.Add("--")
            End If
        Next
    End Sub

    Protected Sub WriteLog(message As String)
        Try
            If message.Length > 0 Then
                Dim logPath As String = Server.MapPath("") & "\GetDataNew.txt"
                Using sw As New StreamWriter(logPath, True)
                    sw.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss} - {SecurityHelper.SanitizeLogMessage(message)}")
                End Using
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("WRITE_LOG_ERROR", "Error writing to log: " & ex.Message)
        End Try
    End Sub

    ' Data structures
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
        Dim status As Integer
        Dim distance As Integer
        Dim currdistance As Integer
        Dim destinationsiteid As Integer
    End Structure

End Class