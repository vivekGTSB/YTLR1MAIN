Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic

Public Class GetKPIDetailsAPK
    Inherits System.Web.UI.Page

    Public sb1 As New StringBuilder()
    Public loggedinUID As String = ""
    Public Sub FillGrid()
        Try
            Dim tid As String = Request.QueryString("u")
            Dim tp As String = Request.QueryString("tp")
            Dim itype As String = Request.QueryString("itype")
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

                Dim trackedQuery As String = "select ytldb.dbo.fn_GetTransporterNameByPlateno(plateno) as TranspName,plateno,lat,lon,timestamp,dbo.fn_getusername_plateno(plateno) as username,isNull(roadname,'-') roadname  from vehicle_tracked2 where plateno in (select plateno from VehicleTbl  where transporter_id='" & tid & "')"
                If itype = "0" Then
                    trackedQuery = "select ytldb.dbo.fn_GetTransporterNameByPlateno(plateno) as TranspName,plateno,lat,lon,timestamp,dbo.fn_getusername_plateno(plateno) as username,isNull(roadname,'-') roadname  from vehicle_tracked2 where plateno in (select plateno from VehicleTbl  where userid='" & tid & "')"
                End If
                ' Response.Write("<br/>1st Query = > " & trackedQuery & "<br/>")
                Dim cmd As New SqlCommand(trackedQuery, conn)
                'Response.Write(cmd.CommandText)
                Try

                    conn.Open()
                    Dim dr As SqlDataReader = cmd.ExecuteReader()

                    Dim vehicleTrackedDict As New Dictionary(Of String, VehicleTrackedRecord)
                    'Plateno, username,location,last updated datetime,Map
                    While dr.Read()
                        Try
                            Dim vtr As New VehicleTrackedRecord()
                            vtr.plateno = dr("plateno").ToString().Trim()
                            vtr.timeStamp = DateTime.Parse(dr("timestamp"))
                            vtr.lat = dr("lat")
                            vtr.lon = dr("lon")
                            If IsDBNull(dr("TranspName")) Then
                                vtr.TranspName = "NA"
                            Else
                                If dr("TranspName").ToString().Trim() = "" Then
                                    vtr.TranspName = "NA"
                                Else
                                    vtr.TranspName = dr("TranspName")
                                End If

                            End If

                            vtr.map = "<a href=""http://maps.google.com/maps?f=q&amp;hl=en&amp;q=" & dr("lat") & "+" & dr("lon") & "&amp;om=1&amp;t=k"" target=""_blank"">Map</a>"
                            vtr.username = dr("Username")
                            vtr.address = dr("roadname")
                            vtr.status = "<a style='cursor:pointer;' onclick=""getData('" & dr("plateno") & "')""><img src='img/editicon.png' style='width: 16px;' alt='status' />Update</a>"
                            vehicleTrackedDict.Add(vtr.plateno, vtr)
                        Catch ex As Exception
                            ' Response.Write("5." & ex.Message)
                        End Try
                    End While

                    Dim tdr As SqlDataReader

                    Dim t As New DataTable
                    t.Rows.Clear()
                    t.Columns.Add(New DataColumn("S No"))
                    t.Columns.Add(New DataColumn("Plateno"))
                    t.Columns.Add(New DataColumn("Username"))
                    t.Columns.Add(New DataColumn("TransporterName"))
                    t.Columns.Add(New DataColumn("Location"))
                    t.Columns.Add(New DataColumn("Last Updated At"))
                    t.Columns.Add(New DataColumn("Map"))
                    t.Columns.Add(New DataColumn("Status"))

                    Dim tSystem As New DataTable
                    tSystem.Rows.Clear()
                    tSystem.Columns.Add(New DataColumn("S No"))
                    tSystem.Columns.Add(New DataColumn("Plateno"))
                    tSystem.Columns.Add(New DataColumn("Username"))
                    tSystem.Columns.Add(New DataColumn("TransporterName"))
                    tSystem.Columns.Add(New DataColumn("Location"))
                    tSystem.Columns.Add(New DataColumn("Last Updated At"))
                    tSystem.Columns.Add(New DataColumn("Map"))
                    tSystem.Columns.Add(New DataColumn("Status"))


                    Dim counter As Integer = 1
                    Dim r As DataRow

                    Dim vr As VehicleTrackedRecord

                    Dim ossactive As Integer = 0
                    Dim ossinactive As Integer = 0
                    Dim osstotal As Integer = 0


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
                                ossDict.Add(dr2("plateno").ToString().Trim(), dr2("weight_outtime"))
                            Catch ex As Exception

                            End Try
                        End While
                    Catch ex As Exception
                        Response.Write("28." & ex.Message)
                    Finally
                        conn2.Close()
                    End Try
                    ' Response.Write("<br/>Tracked : " & vehicleTrackedDict.Keys.Count & " And OSS:" & ossDict.Keys.Count & "<br/>")
                    Dim vehicleQuery As String = "select plateno from vehicleTBL where  transporter_id='" & tid & "'"
                    Try
                        If itype = "0" Then
                            vehicleQuery = "select plateno from vehicleTBL where userid ='" & tid & "'"
                        End If
                        cmd = New SqlCommand(vehicleQuery, conn)
                        ' Response.Write("<br/>2nd Query = > " & vehicleQuery & "<br/>")
                        tdr = cmd.ExecuteReader()
                        ossactive = 0
                        ossinactive = 0
                        osstotal = 0

                        Dim mr As Maintenance
                        While tdr.Read()
                            Try
                                Try
                                    If (vehicleTrackedDict.ContainsKey(tdr("plateno"))) Then
                                        vr = vehicleTrackedDict.Item(tdr("plateno"))
                                        If (maintenanceDict.ContainsKey(tdr("plateno"))) Then
                                            mr = maintenanceDict.Item(tdr("plateno"))
                                            If (mr.timestamp > vr.timeStamp) Then
                                                If mr.status.ToUpper() = "WORKSHOP" Or mr.status.ToUpper() = "BATTERY TAKEN OUT" Or mr.status.ToUpper() = "POWER CUT" Or mr.status.ToUpper() = "SPARE TRUCK" Or mr.status.ToUpper() = "NOT IN OPERATION" Or mr.status.ToUpper() = "ACCIDENT" Then
                                                    If tp = "0" Or tp = "1" Then
                                                        r = tSystem.NewRow
                                                        r(0) = counter.ToString()
                                                        r(1) = tdr("plateno").ToString().ToUpper()
                                                        r(2) = vr.username
                                                        r(3) = vr.TranspName
                                                        r(4) = vr.address
                                                        r(5) = Convert.ToDateTime(vr.timeStamp).ToString("yyyy/MM/dd HH:mm:ss")
                                                        r(6) = vr.map
                                                        r(7) = "Maintenance"
                                                        tSystem.Rows.Add(r)
                                                    End If
                                                ElseIf mr.status.ToUpper() = "SERVICE SCHEDULED" Then
                                                    If tp = "0" Or tp = "2" Then
                                                        r = tSystem.NewRow
                                                        r(0) = counter.ToString()
                                                        r(1) = tdr("plateno").ToString().ToUpper()
                                                        r(2) = vr.username
                                                        r(3) = vr.TranspName
                                                        r(4) = vr.address
                                                        r(5) = Convert.ToDateTime(vr.timeStamp).ToString("yyyy/MM/dd HH:mm:ss")
                                                        r(6) = vr.map
                                                        r(7) = "Service Scheduled"
                                                        tSystem.Rows.Add(r)
                                                    End If
                                                Else
                                                    If tp = "0" Then
                                                        r = tSystem.NewRow
                                                        r(0) = counter.ToString()
                                                        r(1) = tdr("plateno").ToString().ToUpper()
                                                        r(2) = vr.username
                                                        r(3) = vr.TranspName
                                                        r(4) = vr.address
                                                        r(5) = Convert.ToDateTime(vr.timeStamp).ToString("yyyy/MM/dd HH:mm:ss")
                                                        r(6) = vr.map
                                                        r(7) = vr.status
                                                        tSystem.Rows.Add(r)
                                                    End If
                                                End If
                                            Else
                                                If ((DateTime.Now - vr.timeStamp).TotalHours > 24) Then
                                                    If tp = "0" Then
                                                        r = tSystem.NewRow
                                                        r(0) = counter.ToString()
                                                        r(1) = tdr("plateno").ToString().ToUpper()
                                                        r(2) = vr.username
                                                        r(3) = vr.TranspName
                                                        r(4) = vr.address
                                                        r(5) = Convert.ToDateTime(vr.timeStamp).ToString("yyyy/MM/dd HH:mm:ss")
                                                        r(6) = vr.map
                                                        r(7) = vr.status
                                                        tSystem.Rows.Add(r)
                                                    End If
                                                End If
                                            End If
                                        Else
                                            If ((DateTime.Now - vr.timeStamp).TotalHours < 24) Then

                                            Else
                                                If tp = "0" Then
                                                    r = tSystem.NewRow
                                                    r(0) = counter.ToString()
                                                    r(1) = tdr("plateno").ToString().ToUpper()
                                                    r(2) = vr.username
                                                    r(3) = vr.TranspName
                                                    r(4) = vr.address
                                                    r(5) = Convert.ToDateTime(vr.timeStamp).ToString("yyyy/MM/dd HH:mm:ss")
                                                    r(6) = vr.map
                                                    r(7) = vr.status
                                                    tSystem.Rows.Add(r)
                                                End If
                                            End If
                                        End If
                                    Else
                                        If tp = "0" Then
                                            r = tSystem.NewRow
                                            r(0) = counter.ToString()
                                            r(1) = tdr("plateno").ToString().ToUpper()
                                            r(2) = "-"
                                            r(3) = "-"
                                            r(4) = "-"
                                            r(5) = "-"
                                            r(6) = "-"
                                            r(7) = "New Installation"
                                            tSystem.Rows.Add(r)
                                        End If
                                    End If
                                Catch ex As Exception
                                    Response.Write("5161." & ex.Message)
                                End Try



                                Dim plateno As String() = tdr("plateno").ToString().Trim().Split("_")

                                Dim found As Boolean = False
                                For i As Byte = 0 To plateno.Length - 1
                                    If (ossDict.ContainsKey(plateno(i))) Then
                                        found = True
                                        Exit For
                                    End If
                                Next

                                If found = True And tp = "3" Then
                                    If (vehicleTrackedDict.ContainsKey(tdr("plateno"))) Then
                                        vr = vehicleTrackedDict.Item(tdr("plateno"))
                                        If ((DateTime.Now - vr.timeStamp).TotalHours < 24) Then

                                        Else
                                            r = t.NewRow
                                            r(0) = counter.ToString()
                                            r(1) = tdr("plateno").ToString().ToUpper()
                                            r(2) = vr.username
                                            r(3) = vr.TranspName
                                            r(4) = vr.address
                                            r(5) = Convert.ToDateTime(vr.timeStamp).ToString("yyyy/MM/dd HH:mm:ss")
                                            r(6) = vr.map
                                            r(7) = vr.status
                                            t.Rows.Add(r)
                                        End If
                                    Else
                                        r = t.NewRow
                                        r(0) = counter.ToString()
                                        r(1) = tdr("plateno").ToString().ToUpper()
                                        r(2) = "--"
                                        r(3) = "--"
                                        r(4) = "--"
                                        r(5) = "--"
                                        r(6) = "--"
                                        r(7) = "--"
                                        t.Rows.Add(r)
                                    End If
                                End If
                            Catch ex As Exception
                                Response.Write("26." & ex.Message)
                            End Try
                        End While
                    Catch ex As Exception
                        Response.Write("3." & ex.Message)
                    End Try
                    '  Response.Write("OSS " & t.Rows.Count)
                    '  Response.Write("System " & tSystem.Rows.Count)
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
                        t.Rows.Add(r)
                    End If
                    If tSystem.Rows.Count = 0 Then
                        r = tSystem.NewRow
                        r(0) = "--"
                        r(1) = "--"
                        r(2) = "--"
                        r(3) = "--"
                        r(4) = "--"
                        r(5) = "--"
                        r(6) = "--"
                        r(7) = "--"
                        tSystem.Rows.Add(r)
                    End If

                    sb1.Append("<thead><tr align=""left""><th>S No</th><th>Plate No</th><th>User Name</th><th>Transporter</th><th>Location</th><th>Last Updated Time</th><th>Map</th><th>Status</th></tr></thead>")

                    sb1.Append("<tbody>")
                    Dim finalTb As DataTable
                    If tp = "3" Then
                        finalTb = t.Copy()
                    Else
                        finalTb = tSystem.Copy()
                    End If
                    ' Response.Write("Whole " & finalTb.Rows.Count)
                    counter = 1
                    For i As Integer = 0 To finalTb.Rows.Count - 1
                        Try
                            sb1.Append("<tr>")
                            sb1.Append("<td>")
                            sb1.Append((i + 1).ToString())
                            sb1.Append("</td><td>")
                            sb1.Append(finalTb.DefaultView.Item(i)(1))
                            sb1.Append("</td><td>")
                            sb1.Append(finalTb.DefaultView.Item(i)(2))
                            sb1.Append("</td><td>")
                            sb1.Append(finalTb.DefaultView.Item(i)(3))
                            sb1.Append("</td><td>")
                            sb1.Append(finalTb.DefaultView.Item(i)(4))
                            sb1.Append("</td><td>")
                            sb1.Append(finalTb.DefaultView.Item(i)(5))
                            sb1.Append("</td><td>")
                            sb1.Append(finalTb.DefaultView.Item(i)(6))
                            sb1.Append("</td><td>")
                            sb1.Append(finalTb.DefaultView.Item(i)(7))
                            sb1.Append("</td></tr>")
                            counter += 1
                        Catch ex As Exception
                            Response.Write("1." & ex.Message)
                        End Try
                    Next
                    sb1.Append("</tbody>")
                    sb1.Append("<tfoot><tr style=""font-weight:bold;"" align=""left""><th>S No</th><th>Plate No</th><th>User Name</th><th>Transporter</th><th>Location</th><th>Last Updated Time</th><th>Map</th><th>Status</th></tr></tfoot>")



                Catch ex As Exception
                    Response.Write("2." & ex.Message)

                Finally

                    conn.Close()
                End Try

            Catch ex As Exception
                Response.Write("32." & ex.Message)
            End Try

        Catch ex As Exception
            Response.Write("24." & ex.Message)
        End Try
    End Sub

    Private Structure Maintenance
        Dim timestamp As DateTime
        Dim statusdate As DateTime
        Dim status As String
        Dim Remarks As String
        Dim sourcename As String
    End Structure
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        loggedinUID = Request.Cookies("userinfo")("userid")
        FillGrid()
    End Sub
    Private Structure VehicleTrackedRecord
        Dim plateno As String
        Dim username As String
        Dim map As String
        Dim timeStamp As DateTime
        Dim TranspName As String
        Dim gpsAV As Char
        Dim lat As Double
        Dim lon As Double
        Dim speed As Double
        Dim status As String
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
        Dim address As String
    End Structure
End Class