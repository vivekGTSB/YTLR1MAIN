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
            ' SECURITY FIX: Authentication check
            If Not IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.Write("Unauthorized")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Add security headers
            AddSecurityHeaders()

            ' SECURITY FIX: Rate limiting
            If Not SecurityHelper.CheckRateLimit("GetSmartFleetsek_" & GetClientIP(), 100, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.Write("Rate limit exceeded")
                Response.End()
                Return
            End If

            Response.Write(GetJson())
            Response.ContentType = "application/json"
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("PAGE_LOAD_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Internal server error")
        End Try
    End Sub

    ' SECURITY FIX: Authentication check
    Private Function IsUserAuthenticated() As Boolean
        Try
            Return SecurityHelper.ValidateSession() AndAlso
                   HttpContext.Current.Session("userid") IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ' SECURITY FIX: Add security headers
    Private Sub AddSecurityHeaders()
        Response.Headers.Add("X-Content-Type-Options", "nosniff")
        Response.Headers.Add("X-Frame-Options", "DENY")
        Response.Headers.Add("X-XSS-Protection", "1; mode=block")
        Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate")
        Response.Headers.Add("Pragma", "no-cache")
        Response.Headers.Add("Expires", "0")
    End Sub

    ' SECURITY FIX: Get client IP safely
    Private Function GetClientIP() As String
        Try
            Dim ip As String = Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If String.IsNullOrEmpty(ip) Then
                ip = Request.ServerVariables("REMOTE_ADDR")
            End If
            Return ip
        Catch
            Return "Unknown"
        End Try
    End Function

    Protected Function GetJson() As String
        Dim json As String = ""
        Try
            ' SECURITY FIX: Get user data from session instead of cookies
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            If String.IsNullOrEmpty(userid) Then
                Throw New SecurityException("Invalid user session")
            End If

            ' SECURITY FIX: Validate user role
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Dim isLafarge As Boolean = False
            If HttpContext.Current.Session("companyname") IsNot Nothing Then
                If HttpContext.Current.Session("companyname").ToString() = "LAFARGE" Then
                    isLafarge = True
                End If
            End If

            Dim allusers As Boolean = False
            Dim uu As String = ""
            
            ' SECURITY FIX: Validate query string parameter
            Dim queryUserId As String = Request.QueryString("u")
            If Not String.IsNullOrEmpty(queryUserId) Then
                If Not SecurityHelper.ValidateInput(queryUserId, "username") Then
                    Throw New ArgumentException("Invalid user parameter")
                End If
                suserid = queryUserId
            Else
                suserid = userid
                uu = suser
                If role = "Admin" Then
                    suserid = "1990"
                End If
            End If

            Dim pno As String = ""
            If suserid.IndexOf(",") > 0 Then
                Dim sgroupname As String() = suserid.Split(",")
                If sgroupname.Length >= 2 Then
                    suser = sgroupname(0)
                    uu = suser
                    sgroup = sgroupname(1)
                    
                    ' SECURITY FIX: Validate split values
                    If Not SecurityHelper.ValidateInput(suser, "username") OrElse
                       Not SecurityHelper.ValidateInput(sgroup, "username") Then
                        Throw New ArgumentException("Invalid user group parameter")
                    End If
                End If
            End If

            If suserid.IndexOf(";") > 0 Then
                Dim sgroupname As String() = suserid.Split(";")
                If sgroupname.Length >= 2 Then
                    suser = sgroupname(0)
                    uu = suser
                    pno = sgroupname(1)
                    
                    ' SECURITY FIX: Validate split values
                    If Not SecurityHelper.ValidateInput(suser, "username") OrElse
                       Not SecurityHelper.ValidatePlateNumber(pno) Then
                        Throw New ArgumentException("Invalid user plate parameter")
                    End If
                End If
            End If

            If suserid = "All" Then
                allusers = True
            End If

            connstr = System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")
            
            ' SECURITY FIX: Use parameterized queries
            Using conn As New SqlConnection(connstr)
                ' Get vehicle tracked data
                Dim vehicleTrackedDict As New Dictionary(Of String, VehicleTrackedRecord)
                Using cmd As New SqlCommand("SELECT * FROM vehicle_tracked2", conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Try
                                Dim vtr As New VehicleTrackedRecord()
                                vtr.plateno = SecurityHelper.HtmlEncode(dr("plateno").ToString().Trim().ToUpper())
                                vtr.timeStamp = DateTime.Parse(dr("timestamp"))
                                vtr.lat = CDbl(dr("lat"))
                                vtr.lon = CDbl(dr("lon"))
                                vtr.speed = CDbl(dr("speed"))
                                vtr.direction = CDbl(dr("bearing"))
                                vtr.odometer = CDbl(dr("odometer"))

                                vtr.ignition = CBool(dr("ignition"))
                                If Not vtr.ignition Then
                                    vtr.speed = 0
                                End If

                                vtr.overspeed = CBool(dr("overspeed"))
                                vtr.power = CBool(dr("powercut"))
                                vtr.immobilizer = CBool(dr("immobilizer"))
                                vtr.pto = CBool(dr("alarm"))

                                vtr.fuel1 = CDbl(dr("volume1"))
                                vtr.fuel2 = CDbl(dr("volume2"))

                                ' SECURITY FIX: Safely handle null values
                                vtr.nearesttown = If(IsDBNull(dr("nearesttown")), "-", SecurityHelper.HtmlEncode(dr("nearesttown").ToString()))
                                vtr.milepoint = If(IsDBNull(dr("milepoint")), "-", SecurityHelper.HtmlEncode(dr("milepoint").ToString()))
                                vtr.lafargegeofence = If(IsDBNull(dr("lafargegeofence")), "-", SecurityHelper.HtmlEncode(dr("lafargegeofence").ToString()))
                                vtr.publicgeofence = If(IsDBNull(dr("publicgeofence")), "-", SecurityHelper.HtmlEncode(dr("publicgeofence").ToString()))
                                vtr.privategeofence = If(IsDBNull(dr("privategeofence")), "-", SecurityHelper.HtmlEncode(dr("privategeofence").ToString()))
                                vtr.poiname = If(IsDBNull(dr("poiname")), "-", SecurityHelper.HtmlEncode(dr("poiname").ToString()))
                                vtr.roadname = If(IsDBNull(dr("roadname")), "-", SecurityHelper.HtmlEncode(dr("roadname").ToString()))

                                vehicleTrackedDict.Add(vtr.plateno, vtr)
                            Catch ex As Exception
                                SecurityHelper.LogSecurityEvent("VEHICLE_DATA_ERROR", ex.Message)
                            End Try
                        End While
                    End Using
                End Using

                ' Get idling data
                Dim idlingDict As New Dictionary(Of String, VehicleIdlingRecord)
                Try
                    Using cmd As New SqlCommand("SELECT * FROM vehicle_idling WHERE duration > 0", conn)
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            While dr.Read()
                                Try
                                    Dim vir As New VehicleIdlingRecord()
                                    vir.plateno = SecurityHelper.HtmlEncode(dr("plateno").ToString().Trim().ToUpper())
                                    vir.starttimeStamp = DateTime.Parse(dr("from"))
                                    vir.endtimeStamp = DateTime.Parse(dr("to"))
                                    vir.duration = CInt(dr("duration"))

                                    idlingDict.Add(vir.plateno, vir)
                                Catch ex As Exception
                                    SecurityHelper.LogSecurityEvent("IDLING_DATA_ERROR", ex.Message)
                                End Try
                            End While
                        End Using
                    End Using
                Catch ex As Exception
                    SecurityHelper.LogSecurityEvent("IDLING_QUERY_ERROR", ex.Message)
                End Try

                ' Get vehicle start data
                Dim vehicleStartDict As New Dictionary(Of String, DateTime)
                Try
                    Using cmd As New SqlCommand("SELECT * FROM vehicle_start", conn)
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            While dr.Read()
                                Try
                                    vehicleStartDict.Add(SecurityHelper.HtmlEncode(dr("plateno").ToString().Trim().ToUpper()), DateTime.Parse(dr("timestamp")))
                                Catch ex As Exception
                                    SecurityHelper.LogSecurityEvent("VEHICLE_START_ERROR", ex.Message)
                                End Try
                            End While
                        End Using
                    End Using
                Catch ex As Exception
                    SecurityHelper.LogSecurityEvent("VEHICLE_START_QUERY_ERROR", ex.Message)
                End Try

                ' Get maintenance data
                Dim maintenanceDict As New Dictionary(Of String, Maintenance)
                Try
                    Using cmd As New SqlCommand("SELECT plateno, timestamp, statusdate, status FROM maintenance WHERE timestamp > @minDate ORDER BY timestamp DESC", conn)
                        cmd.Parameters.AddWithValue("@minDate", New DateTime(2012, 9, 1))
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            While dr.Read()
                                Try
                                    Dim m As New Maintenance()
                                    m.timestamp = DateTime.Parse(dr("timestamp"))
                                    m.statusdate = DateTime.Parse(dr("statusdate"))
                                    m.status = SecurityHelper.HtmlEncode(dr("status").ToString())
                                    maintenanceDict.Add(SecurityHelper.HtmlEncode(dr("plateno").ToString().Trim().ToUpper()), m)
                                Catch ex As Exception
                                    SecurityHelper.LogSecurityEvent("MAINTENANCE_DATA_ERROR", ex.Message)
                                End Try
                            End While
                        End Using
                    End Using
                Catch ex As Exception
                    SecurityHelper.LogSecurityEvent("MAINTENANCE_QUERY_ERROR", ex.Message)
                End Try

                ' Build vehicle query based on role and parameters
                Dim vehicleQuery As String = BuildVehicleQuery(role, userslist, suserid, allusers, suser, sgroup, pno)
                
                Using cmd As New SqlCommand(vehicleQuery, conn)
                    ' Add parameters based on query type
                    AddVehicleQueryParameters(cmd, role, userslist, suserid, suser, sgroup, pno)
                    
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim aa As New ArrayList()
                        Dim rowcounter As Integer = 1

                        While dr.Read()
                            Try
                                Dim a As New ArrayList()
                                Dim plateno As String = SecurityHelper.HtmlEncode(dr("plateno").ToString().Trim().ToUpper())

                                If vehicleTrackedDict.ContainsKey(plateno) Then
                                    Dim vr As VehicleTrackedRecord = vehicleTrackedDict(plateno)
                                    
                                    ' Build vehicle data array with proper encoding
                                    BuildVehicleDataArray(a, vr, dr, maintenanceDict, idlingDict, rowcounter, isLafarge)
                                    rowcounter += 1
                                Else
                                    ' Add placeholder data for vehicles without tracking
                                    BuildPlaceholderDataArray(a)
                                End If

                                aa.Add(a)
                            Catch ex As Exception
                                SecurityHelper.LogSecurityEvent("VEHICLE_PROCESSING_ERROR", ex.Message)
                            End Try
                        End While

                        If aa.Count = 0 Then
                            Dim a As New ArrayList()
                            BuildPlaceholderDataArray(a)
                            aa.Add(a)
                        End If

                        json = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
                    End Using
                End Using
            End Using

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GET_JSON_ERROR", ex.Message)
            json = "{""error"":""An error occurred while retrieving data""}"
        End Try

        Return json
    End Function

    ' SECURITY FIX: Build secure vehicle query
    Private Function BuildVehicleQuery(role As String, userslist As String, suserid As String, allusers As Boolean, suser As String, sgroup As String, pno As String) As String
        Dim baseQuery As String = "SELECT immobilizer, brand, plateno, unitid, groupname, userid, pto, modo, vehicleodometer, VehicleOdoRecDate FROM vehicleTBL"
        
        If Not String.IsNullOrEmpty(pno) Then
            Return baseQuery & " WHERE plateno = @plateno"
        ElseIf Not String.IsNullOrEmpty(suser) AndAlso Not String.IsNullOrEmpty(sgroup) Then
            Return baseQuery & " WHERE userid = @userid AND groupname = @groupname ORDER BY plateno"
        ElseIf allusers Then
            If role = "SuperUser" OrElse role = "Operator" Then
                Return baseQuery & " WHERE userid IN (" & userslist & ") ORDER BY plateno"
            Else
                Return baseQuery & " ORDER BY plateno"
            End If
        Else
            Select Case role
                Case "User"
                    Return baseQuery & " WHERE userid = @userid ORDER BY plateno"
                Case "SuperUser", "Operator"
                    Return baseQuery & " WHERE userid IN (" & userslist & ") ORDER BY plateno"
                Case "Admin"
                    Return baseQuery & " WHERE userid = @userid ORDER BY plateno"
                Case Else
                    Return baseQuery & " WHERE userid = @userid ORDER BY plateno"
            End Select
        End If
    End Function

    ' SECURITY FIX: Add parameters to vehicle query
    Private Sub AddVehicleQueryParameters(cmd As SqlCommand, role As String, userslist As String, suserid As String, suser As String, sgroup As String, pno As String)
        If Not String.IsNullOrEmpty(pno) Then
            cmd.Parameters.AddWithValue("@plateno", pno)
        ElseIf Not String.IsNullOrEmpty(suser) AndAlso Not String.IsNullOrEmpty(sgroup) Then
            cmd.Parameters.AddWithValue("@userid", suser)
            cmd.Parameters.AddWithValue("@groupname", sgroup)
        ElseIf Not (role = "SuperUser" OrElse role = "Operator") OrElse Not String.IsNullOrEmpty(suserid) Then
            cmd.Parameters.AddWithValue("@userid", suserid)
        End If
    End Sub

    ' SECURITY FIX: Build vehicle data array with proper encoding
    Private Sub BuildVehicleDataArray(a As ArrayList, vr As VehicleTrackedRecord, dr As SqlDataReader, maintenanceDict As Dictionary(Of String, Maintenance), idlingDict As Dictionary(Of String, VehicleIdlingRecord), rowcounter As Integer, isLafarge As Boolean)
        a.Add(rowcounter)

        ' Status information
        Dim astatus As New ArrayList()
        If maintenanceDict.ContainsKey(vr.plateno) Then
            Dim mr As Maintenance = maintenanceDict(vr.plateno)
            If mr.timestamp > vr.timeStamp Then
                Dim amaintenance As New ArrayList()
                amaintenance.Add(mr.statusdate.ToString("yyyy/MM/dd HH:mm:ss"))
                amaintenance.Add(mr.status)
                astatus.Add(amaintenance)
            ElseIf (DateTime.Now - vr.timeStamp).TotalHours > 24 Then
                Dim amaintenance As New ArrayList()
                amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                amaintenance.Add("Data Not Coming")
                astatus.Add(amaintenance)
            End If
        ElseIf (DateTime.Now - vr.timeStamp).TotalHours > 24 Then
            Dim amaintenance As New ArrayList()
            amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
            amaintenance.Add("Data Not Coming")
            astatus.Add(amaintenance)
        End If

        ' Check immobilizer status
        Try
            If CBool(dr("immobilizer")) AndAlso vr.immobilizer Then
                Dim amaintenance As New ArrayList()
                amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
                amaintenance.Add("Immobilizer Activated")
                astatus.Add(amaintenance)
            End If
        Catch
        End Try

        ' Check power status
        If vr.power Then
            Dim amaintenance As New ArrayList()
            amaintenance.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
            amaintenance.Add("Power Cut")
            astatus.Add(amaintenance)
        End If

        If astatus.Count = 0 Then
            astatus.Add(New String() {"--", "--"})
        End If

        a.Add(astatus)
        a.Add(vr.plateno)
        a.Add(SecurityHelper.HtmlEncode(dr("groupname").ToString().ToUpper()))
        a.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))

        ' PTO status
        If CBool(dr("pto")) Then
            a.Add(If(vr.pto, 1, 0))
        Else
            a.Add(-1)
        End If

        ' Ignition status
        a.Add(If(vr.ignition, "ON", "OFF"))

        ' Speed with overspeed highlighting
        If vr.speed >= 100 Then
            a.Add($"<b style='background-color:#FF8800;color:#FFFFFF;' title='Overspeed = {vr.speed:0} KM/H'><blink>{vr.speed:0}</blink></b>")
        ElseIf vr.speed >= 90 Then
            a.Add($"<b style='background-color:#FFFF00;color:red;' title='Overspeed = {vr.speed:0} KM/H'><blink>{vr.speed:0}</blink></b>")
        ElseIf vr.speed >= 80 Then
            a.Add($"<b style='background-color:#FFFF00;color:red;' title='Overspeed = {vr.speed:0} KM/H'><blink>{vr.speed:0}</blink></b>")
        Else
            a.Add(CInt(vr.speed))
        End If

        ' Odometer
        a.Add(If(vr.odometer > 0, CInt(vr.odometer), "--"))

        ' Odometer details
        Dim odometerDetails As New ArrayList()
        odometerDetails.Add(If(IsDBNull(dr("vehicleodometer")), 0, dr("vehicleodometer")))
        odometerDetails.Add(If(IsDBNull(dr("VehicleOdoRecDate")), "", DateTime.Parse(dr("VehicleOdoRecDate")).ToString("yyyy/MM/dd HH:mm:ss")))
        odometerDetails.Add(If(IsDBNull(dr("modo")), 0, CInt(dr("modo"))))
        a.Add(odometerDetails)

        ' Fuel levels
        a.Add(CInt(vr.fuel1))
        a.Add(CInt(vr.fuel2))

        ' Idling information
        Dim aidling As New ArrayList()
        If idlingDict.ContainsKey(vr.plateno) AndAlso vr.ignition AndAlso vr.speed = 0 Then
            Dim ir As VehicleIdlingRecord = idlingDict(vr.plateno)
            aidling.Add(vr.plateno)
            aidling.Add(ir.starttimeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
            aidling.Add(ir.duration.ToString("0"))
        ElseIf vr.ignition AndAlso vr.speed = 0 Then
            aidling.Add(vr.plateno)
            aidling.Add(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))
            aidling.Add("1")
        Else
            aidling.Add("--")
            aidling.Add("--")
            aidling.Add("--")
        End If
        a.Add(aidling)

        ' Coordinates
        a.Add(Math.Round(vr.lat, 6))
        a.Add(Math.Round(vr.lon, 6))

        ' Direction
        Dim compass() As String = {"N", "NE", "E", "SE", "S", "SW", "W", "NW", "N"}
        Dim compassindex As Integer = CInt((vr.direction - 22.5 + 22.5) / 45)
        If compassindex >= 0 AndAlso compassindex < compass.Length Then
            a.Add(compass(compassindex))
        Else
            a.Add("N")
        End If

        ' Location information
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
                loctn.Add($"{Math.Round(vr.lat, 6)},{Math.Round(vr.lon, 6)}")
            End If
        Else
            If vr.publicgeofence <> "-" Then
                If vr.privategeofence <> "-" Then
                    loctn.Add(1)
                    loctn.Add($"{vr.publicgeofence};{vr.privategeofence}")
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
                loctn.Add($"{Math.Round(vr.lat, 6)},{Math.Round(vr.lon, 6)}")
            End If
        End If
        a.Add(loctn)

        ' Additional location data
        a.Add(If(vr.nearesttown <> "-", vr.nearesttown, "--"))
        a.Add(If(vr.milepoint <> "-", vr.milepoint, "--"))
        a.Add(vr.plateno)
        a.Add(CInt(dr("userid")))
        a.Add(vr.timeStamp.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss"))
        a.Add(If(IsDBNull(dr("brand")), "--", SecurityHelper.HtmlEncode(dr("brand").ToString())))
    End Sub

    ' SECURITY FIX: Build placeholder data array
    Private Sub BuildPlaceholderDataArray(a As ArrayList)
        For i As Integer = 0 To 22
            a.Add("--")
        Next
    End Sub

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
            If message.Length > 0 Then
                SecurityHelper.LogSecurityEvent("WRITE_LOG", SecurityHelper.SanitizeLogMessage(message))
            End If
        Catch ex As Exception
            ' Fail silently
        End Try
    End Sub

End Class