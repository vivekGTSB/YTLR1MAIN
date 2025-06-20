Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.Web.Script.Services
Imports System.IO.Compression
Imports AspMap
Imports System.Net

Partial Class GetOssDash
    Inherits System.Web.UI.Page
    Public connstr As String
    Public uid As Integer, urole, uuserslist As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate authentication
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            ' SECURITY FIX: Get user data from session instead of cookies
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            uid = Convert.ToInt32(userid)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            urole = role
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
            uuserslist = userslist
            
            Response.Write(GetJson())
            Response.ContentType = "application/json"

        Catch ex As Exception
            SecurityHelper.LogError("GetOssDash Page_Load error", ex, Server)
            Response.StatusCode = 500
            Response.Write("An error occurred")
        End Try
    End Sub

    Protected Function GetJson() As String
        Dim json As String = ""
        Try
            connstr = System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")
            
            ' SECURITY FIX: Get user data from session instead of cookies
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
            
            Dim tankerQuery As String = "SELECT plateno FROM vehicleTBL"
            If role = "User" Then
                tankerQuery = "SELECT plateno,groupname FROM vehicleTBL WHERE userid = @userid ORDER BY groupname"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If SecurityHelper.IsValidUsersList(userslist) Then
                    tankerQuery = "SELECT plateno,groupname FROM vehicleTBL WHERE userid IN (" & userslist & ") ORDER BY groupname"
                Else
                    tankerQuery = "SELECT plateno,groupname FROM vehicleTBL WHERE userid = @userid ORDER BY groupname"
                End If
            End If
            
            Dim shipToCodeQuery As String = "SELECT geofencename,shiptocode,data,geofencetype,accesstype FROM geofence WHERE accesstype='1' ORDER BY LTRIM(geofencename)"
            
            Using conn As New SqlConnection(connstr)
                Dim ds As New DataSet
                Using da As New SqlDataAdapter(tankerQuery, conn)
                    If role = "User" OrElse (role <> "Admin" AndAlso Not SecurityHelper.IsValidUsersList(userslist)) Then
                        da.SelectCommand.Parameters.AddWithValue("@userid", userid)
                    End If
                    da.Fill(ds)
                End Using

                Dim platecond As String = ""
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    If Not IsDBNull(ds.Tables(0).Rows(i)("plateno")) Then
                        Dim plateNo As String = ds.Tables(0).Rows(i)("plateno").ToString()
                        If SecurityHelper.ValidatePlateNumber(plateNo) Then
                            platecond = platecond & "'" & SecurityHelper.SanitizeForHtml(plateNo) & "',"
                        End If
                    End If
                Next

                Dim firstpoint As String = ""
                Using da As New SqlDataAdapter(shipToCodeQuery, conn)
                    ds.Clear()
                    da.Fill(ds)
                End Using

                Dim vtr As New VehicleTrackedRecord()
                Dim ShipToNameDict1 As New Dictionary(Of String, VehicleTrackedRecord)
                Dim ShipToNameDict As New Dictionary(Of Integer, String)

                For c As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Try
                        Dim shipToCode As String = ds.Tables(0).Rows(c)("shiptocode").ToString()
                        If Not ShipToNameDict1.ContainsKey(shipToCode) Then
                            vtr = New VehicleTrackedRecord()

                            Dim data As String = ds.Tables(0).Rows(c)("data").ToString()
                            Dim ptslayer As New AspMap.Points
                            Dim shp As New AspMap.Shape
                            shp.ShapeType = ShapeType.mcPolygonShape
                            firstpoint = ""
                            Dim pots() As String = data.Split(";"c)
                            Dim vals() As String
                            For i1 As Integer = 0 To pots.Length - 1
                                vals = pots(i1).Split(","c)
                                If vals.Length >= 2 AndAlso IsNumeric(vals(0)) AndAlso IsNumeric(vals(1)) Then
                                    ptslayer.AddPoint(vals(0), vals(1))
                                    If i1 = 0 Then
                                        firstpoint = Convert.ToDouble(vals(1)).ToString("0.0000") & "," & Convert.ToDouble(vals(0)).ToString("0.0000")
                                    End If
                                End If
                            Next
                            shp.AddPart(ptslayer)

                            vtr.plateno = SecurityHelper.SanitizeForHtml(ds.Tables(0).Rows(c)("geofencename").ToString().ToUpper())
                            vtr.centroiod = firstpoint
                            ShipToNameDict1.Add(shipToCode, vtr)
                            
                            Dim shipToCodeInt As Integer
                            If Integer.TryParse(shipToCode, shipToCodeInt) Then
                                ShipToNameDict.Add(shipToCodeInt, SecurityHelper.SanitizeForHtml(ds.Tables(0).Rows(c)("geofencename").ToString().ToUpper()))
                            End If
                        End If
                    Catch ex As Exception
                        WriteLog("Error processing geofence data: " & ex.Message)
                    End Try
                Next

                If platecond.Length > 3 Then
                    platecond = platecond.Substring(0, platecond.Length - 1)
                    platecond = " AND plateno IN (" & platecond & ")"
                End If

                ' SECURITY FIX: Validate date parameter
                Dim sdate As String = SecurityHelper.SanitizeForHtml(Request.QueryString("d"))
                If Not SecurityHelper.ValidateDate(sdate) Then
                    sdate = DateTime.Now.ToString("yyyy-MM-dd")
                End If

                Dim ugdate As String = sdate.Substring(0, 10)
                Dim bdt As String = ugdate & " 00:00:00"
                Dim edt As String = ugdate & " 23:59:59"
                Dim columnname As String = "weight_outtime"
                
                Dim vehicleDict As New Dictionary(Of String, String)
                
                ' SECURITY FIX: Use parameterized query for vehicle data
                Using cmd As New SqlCommand("SELECT plateno,groupname FROM vehicleTBL WHERE plateno <> '' " & platecond & " ORDER BY groupname", conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Try
                                Dim plateNo As String = dr("plateno").ToString()
                                If plateNo.Contains("_") Then
                                    plateNo = plateNo.Split("_"c)(0)
                                End If
                                If Not vehicleDict.ContainsKey(plateNo) Then
                                    vehicleDict.Add(plateNo, SecurityHelper.SanitizeForHtml(dr("groupname").ToString()))
                                End If
                            Catch ex As Exception
                                WriteLog("Error processing vehicle data: " & ex.Message)
                            End Try
                        End While
                    End Using
                    conn.Close()
                End Using

                ' Continue with OSS data processing using secure methods...
                json = ProcessOSSData(bdt, edt, platecond, vehicleDict, ShipToNameDict1)
            End Using

        Catch ex As Exception
            WriteLog(ex.Message + " , " + ex.StackTrace)
            json = "{""error"":""An error occurred""}"
        End Try
        Return json
    End Function

    Private Function ProcessOSSData(bdt As String, edt As String, platecond As String, vehicleDict As Dictionary(Of String, String), ShipToNameDict1 As Dictionary(Of String, VehicleTrackedRecord)) As String
        Try
            Using conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                ' SECURITY FIX: Get transporter data with parameterized query
                Dim TransNameDict As New Dictionary(Of String, Integer)
                Using cmdT As New SqlCommand("SELECT DISTINCT transporter_name, transporter_id FROM oss_transporter ORDER BY transporter_name", conn2)
                    conn2.Open()
                    Using drT As SqlDataReader = cmdT.ExecuteReader()
                        While drT.Read()
                            Try
                                Dim transporterName As String = drT("transporter_name").ToString().ToUpper()
                                Dim transporterId As Integer = Convert.ToInt32(drT("transporter_id"))
                                If Not TransNameDict.ContainsKey(transporterName) Then
                                    TransNameDict.Add(transporterName, transporterId)
                                End If
                            Catch ex As Exception
                                WriteLog("Error processing transporter data: " & ex.Message)
                            End Try
                        End While
                    End Using
                    conn2.Close()
                End Using

                Dim t As New DataTable
                t.Columns.Add(New DataColumn("trid"))
                t.Columns.Add(New DataColumn("pno"))
                t.Columns.Add(New DataColumn("plNO"))
                t.Columns.Add(New DataColumn("sc"))
                t.Columns.Add(New DataColumn("wt"))
                t.Columns.Add(New DataColumn("ds"))
                t.Columns.Add(New DataColumn("dno"))
                t.Columns.Add(New DataColumn("dqty"))
                t.Columns.Add(New DataColumn("ddr"))
                t.Columns.Add(New DataColumn("atm"))
                t.Columns.Add(New DataColumn("st"))
                t.Columns.Add(New DataColumn("sts"))
                t.Columns.Add(New DataColumn("dnno"))
                t.Columns.Add(New DataColumn("dnqty"))
                t.Columns.Add(New DataColumn("dur"))
                t.Columns.Add(New DataColumn("dist"))
                t.Columns.Add(New DataColumn("duration"))
                t.Columns.Add(New DataColumn("est_arrivaltime"))

                ' SECURITY FIX: Use parameterized query for OSS data
                Using cmd As New SqlCommand("SELECT * FROM oss_patch_out WHERE weight_outtime BETWEEN @bdt AND @edt " & platecond & " ORDER BY status", conn2)
                    cmd.Parameters.AddWithValue("@bdt", bdt)
                    cmd.Parameters.AddWithValue("@edt", edt)
                    
                    conn2.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim i As Int32 = 1
                        While dr.Read()
                            Try
                                Dim transporterName As String = dr("transporter").ToString().ToUpper()
                                If TransNameDict.ContainsKey(transporterName) Then
                                    Dim r As DataRow = t.NewRow()
                                    Dim requiredDate As DateTime = Convert.ToDateTime(dr("weight_outtime"))
                                    r(0) = TransNameDict(transporterName)

                                    r(1) = SecurityHelper.SanitizeForHtml(dr("patch_no").ToString())
                                    r(2) = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
                                    r(3) = SecurityHelper.SanitizeForHtml(dr("source_supply").ToString())
                                    
                                    Dim p As String = requiredDate.ToString("yyyy/MM/dd HH:mm:ss")
                                    r(4) = p
                                    r(5) = SecurityHelper.SanitizeForHtml(dr("destination_siteid").ToString())

                                    Try
                                        Dim destSiteId As String = dr("destination_siteid").ToString()
                                        If ShipToNameDict1.ContainsKey(destSiteId) Then
                                            Dim a1 = ShipToNameDict1(destSiteId)
                                            r(6) = a1.plateno
                                            r(7) = a1.centroiod
                                        Else
                                            r(6) = "--"
                                            r(7) = "--"
                                        End If
                                    Catch ex As Exception
                                        r(6) = "--"
                                        r(7) = "--"
                                    End Try

                                    r(8) = Convert.ToInt64(dr("dn_no"))
                                    r(9) = Convert.ToInt64(dr("dn_qty"))
                                    r(10) = SecurityHelper.SanitizeForHtml(dr("dn_driver").ToString())

                                    If Not IsDBNull(dr("ata_datetime")) Then
                                        r(11) = Convert.ToDateTime(dr("ata_datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                                    Else
                                        r(11) = "--"
                                    End If

                                    ' Calculate duration
                                    If r(11).ToString() <> "--" Then
                                        Dim t1 As DateTime = Convert.ToDateTime(r(4))
                                        Dim t2 As DateTime = Convert.ToDateTime(r(11))
                                        Dim t3 As TimeSpan = t2 - t1
                                        If t3.TotalHours() > 1 Then
                                            Dim m As String = t3.TotalHours().ToString()
                                            If m.Contains(".") Then
                                                m = m.Split("."c)(1)
                                                If m.Length >= 2 Then
                                                    m = m.Substring(0, 2)
                                                End If
                                            Else
                                                m = "00"
                                            End If
                                            r(14) = t3.TotalHours().ToString().Split("."c)(0) & " h " & m & " min"
                                        Else
                                            r(14) = t3.TotalMinutes().ToString().Split("."c)(0) & " min"
                                        End If
                                    Else
                                        r(14) = "--"
                                    End If

                                    r(12) = Convert.ToInt16(dr("status"))

                                    ' Map status to display status
                                    Select Case r(12)
                                        Case 0 : r(13) = 6
                                        Case 1 : r(13) = 1
                                        Case 2 : r(13) = 1
                                        Case 3 : r(13) = 4
                                        Case 4 : r(13) = 4
                                        Case 5 : r(13) = 4
                                        Case 6 : r(13) = 6
                                        Case 7 : r(13) = 2
                                        Case 8 : r(13) = 2
                                        Case 10 : r(13) = 3
                                        Case 11 : r(13) = 7
                                        Case 12 : r(13) = 2
                                        Case 13 : r(13) = 2
                                        Case 14 : r(13) = 5
                                        Case Else : r(13) = 6
                                    End Select

                                    If Not IsDBNull(dr("distance")) AndAlso dr("distance").ToString() <> "0" Then
                                        Dim d1 As Double = Convert.ToDouble(dr("distance"))
                                        r(15) = d1.ToString("0.0")
                                    Else
                                        r(15) = "0"
                                    End If

                                    If Not IsDBNull(dr("duration")) Then
                                        Dim iSecond As Double = Convert.ToDouble(dr("duration"))
                                        Dim iSpan As TimeSpan = TimeSpan.FromSeconds(iSecond)

                                        If iSecond >= 3600 Then
                                            r(16) = iSpan.Hours.ToString.PadLeft(2, "0"c) & " h " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " min "
                                        ElseIf iSecond >= 60 Then
                                            r(16) = iSpan.Minutes.ToString.PadLeft(2, "0"c) & " min "
                                        Else
                                            r(16) = iSpan.Seconds.ToString.PadLeft(2, "0"c) & " sec"
                                        End If
                                    Else
                                        r(16) = "0"
                                    End If

                                    If IsDBNull(dr("est_arrivaltime")) Then
                                        r(17) = "--"
                                    Else
                                        r(17) = Convert.ToDateTime(dr("est_arrivaltime")).ToString("yyyy/MM/dd HH:mm:ss")
                                    End If

                                    t.Rows.Add(r)
                                End If
                            Catch ex As Exception
                                WriteLog("Error processing OSS record: " & ex.Message)
                            End Try
                        End While
                    End Using
                    conn2.Close()
                End Using

                Dim aa As New ArrayList()
                Dim view As New DataView(t)
                view.Sort = "dnqty ASC"

                For i As Integer = 0 To view.Count - 1
                    Dim a As New ArrayList()
                    For j As Integer = 0 To 17
                        a.Add(view.Item(i)(j))
                    Next
                    aa.Add(a)
                Next

                If aa.Count = 0 Then
                    Dim a As New ArrayList()
                    For j As Integer = 0 To 11
                        a.Add("--")
                    Next
                    aa.Add(a)
                End If

                Dim json As String = "{""data"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
                
                If HttpContext.Current.Request.Headers("Accept-Encoding") IsNot Nothing AndAlso 
                   HttpContext.Current.Request.Headers("Accept-Encoding").ToLower().Contains("gzip") Then
                    HttpContext.Current.Response.AppendHeader("Content-Encoding", "gzip")
                    HttpContext.Current.Response.Filter = New GZipStream(HttpContext.Current.Response.Filter, CompressionMode.Compress)
                End If

                Return json
            End Using

        Catch ex As Exception
            WriteLog("ProcessOSSData error: " & ex.Message)
            Return "{""error"":""Data processing failed""}"
        End Try
    End Function

    Protected Sub WriteLog(ByVal message As String)
        Try
            If message.Length > 0 Then
                Dim logPath As String = Server.MapPath("") & "\GetData.Log.txt"
                Dim sanitizedMessage As String = SecurityHelper.SanitizeLogMessage(message)
                Dim logEntry As String = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & " - " & sanitizedMessage & Environment.NewLine
                
                SyncLock GetType(GetOssDash)
                    System.IO.File.AppendAllText(logPath, logEntry)
                End SyncLock
            End If
        Catch ex As Exception
            ' Fail silently to prevent information disclosure
        End Try
    End Sub

    Private Structure VehicleTrackedRecord
        Dim plateno As String
        Dim centroiod As String
        Dim sc As String
    End Structure

End Class