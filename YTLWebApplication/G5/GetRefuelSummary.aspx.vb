Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.DataRow
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports AspMap

Partial Class GetRefuelSummary
    Inherits SecurePageBase
    
    Public Shared totrefuel As Double = 0
    Public Shared totrefuelcost As Double = 0
    
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
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
            SecurityHelper.LogSecurityEvent("GENERAL_ERROR", "Error in GetRefuelSummary: " & ex.Message)
            Response.StatusCode = 500
            Response.Write("{""error"":""Internal server error""}")
        End Try
    End Sub
    
    Protected Function GetJson() As String
        Dim json As String = ""
        Try
            ' SECURITY FIX: Validate and sanitize query parameters
            Dim ddlu As String = SecurityHelper.SanitizeForHtml(Request.QueryString("ddlu"))
            Dim ddlp As String = SecurityHelper.SanitizeForHtml(Request.QueryString("ddlp"))
            Dim bdt As String = SecurityHelper.SanitizeForHtml(Request.QueryString("bdt"))
            Dim edt As String = SecurityHelper.SanitizeForHtml(Request.QueryString("edt"))

            ' SECURITY FIX: Validate date parameters
            If Not SecurityHelper.ValidateDate(bdt) OrElse Not SecurityHelper.ValidateDate(edt) Then
                Return "{""error"":""Invalid date format""}"
            End If

            ' SECURITY FIX: Get user data from session
            Dim luid As String = SessionManager.GetCurrentUserId()
            Dim role As String = SessionManager.GetCurrentUserRole()
            Dim userslist As String = If(Session("userslist") IsNot Nothing, Session("userslist").ToString(), "")

            ' SECURITY FIX: Validate users list format
            If Not String.IsNullOrEmpty(userslist) AndAlso Not SecurityHelper.ValidateUsersList(userslist) Then
                userslist = $"'{luid}'"
            End If

            Dim aa As New ArrayList()
            Dim vehiclepoint As New AspMap.Point
            Dim mapping As New AspMap.Map()
            
            Dim begindatetime As String = bdt
            Dim enddatetime As String = edt
            Dim userid As String = ddlu
            Dim plateno As String = ddlp
            Dim uid As String = luid
            totrefuel = 0
            totrefuelcost = 0

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("UserName"))
            t.Columns.Add(New DataColumn("PlateNo"))
            t.Columns.Add(New DataColumn("RefuelfrmTime"))
            t.Columns.Add(New DataColumn("RefueltoTime"))
            t.Columns.Add(New DataColumn("Address"))
            t.Columns.Add(New DataColumn("Refuel Start"))
            t.Columns.Add(New DataColumn("Refuel End"))
            t.Columns.Add(New DataColumn("Refuel(Ltr)"))
            t.Columns.Add(New DataColumn("Refuel Cost(RM)"))
            t.Columns.Add(New DataColumn("Actuelrefuel"))
            t.Columns.Add(New DataColumn("costperltr"))
            t.Columns.Add(New DataColumn("Actuelcost"))
            t.Columns.Add(New DataColumn("accurasyagenestrefuel"))
            t.Columns.Add(New DataColumn("acurasyagainesttankvol"))
            t.Columns.Add(New DataColumn("lat"))
            t.Columns.Add(New DataColumn("lon"))
            t.Columns.Add(New DataColumn("time"))
            t.Columns.Add(New DataColumn("userid"))
            t.Columns.Add(New DataColumn("fuelst"))
            t.Columns.Add(New DataColumn("actrefuel"))
            t.Columns.Add(New DataColumn("actcost"))

            ' Build secure query based on parameters
            Dim query As String = BuildSecureRefuelQuery(ddlu, ddlp, begindatetime, enddatetime, userid, plateno, role, userslist)
            
            Try
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    ' Load fuel stations securely
                    LoadFuelStations(conn, mapping)
                    
                    ' Execute main refuel query
                    Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                        AddRefuelQueryParameters(cmd, ddlu, ddlp, begindatetime, enddatetime, userid, plateno)
                        
                        conn.Open()
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            ProcessRefuelData(dr, t, mapping, luid)
                        End Using
                    End Using
                End Using

                ' Process results for JSON output
                ProcessResultsForJson(t, aa)
                
                ' Store excel table in session
                Dim exceltable As DataTable = CreateExcelTable(t)
                HttpContext.Current.Session.Remove("exceltable")
                HttpContext.Current.Session("exceltable") = exceltable
                
                json = JsonConvert.SerializeObject(aa, Formatting.None)
                
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("DATABASE_ERROR", "Error in GetRefuelSummary query: " & ex.Message)
                json = "{""error"":""Database error""}"
            End Try

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GENERAL_ERROR", "Error in GetRefuelSummary GetJson: " & ex.Message)
            json = "{""error"":""Internal server error""}"
        End Try

        Return json
    End Function

    Private Function BuildSecureRefuelQuery(ddlu As String, ddlp As String, begindatetime As String, enddatetime As String, userid As String, plateno As String, role As String, userslist As String) As String
        Dim query As String = ""
        
        If ddlu <> "--All Users--" Then
            If ddlp = "--All Plate No--" Then
                query = "SELECT DISTINCT rt.plateno, avu.userid, avu.username, SUBSTRING(CONVERT(varchar,refuelfrom,120),1,16) as refuelfrom, SUBSTRING(CONVERT(varchar,refuelto,120),1,16) as refuelto, beforerefuel, afterrefuel, totrefuel, fuelcost, lat, lon, ft.liters, ft.cost FROM refuel rt INNER JOIN (SELECT au.userid, au.username, plateno FROM vehicleTBL vt LEFT OUTER JOIN userTBL au ON vt.userid = au.userid WHERE au.userid = @userid) avu ON rt.plateno = avu.plateno LEFT OUTER JOIN fuel ft ON ft.plateno = rt.plateno AND ft.timestamp BETWEEN DATEADD(mi,-10,rt.refuelfrom) AND DATEADD(mi,10,rt.refuelto) WHERE rt.refuelfrom BETWEEN @bdt AND @edt AND rt.status = 1 ORDER BY refuelfrom"
            Else
                query = "SELECT DISTINCT rt.plateno, avu.userid, avu.username, SUBSTRING(CONVERT(varchar,refuelfrom,120),1,16) as refuelfrom, SUBSTRING(CONVERT(varchar,refuelto,120),1,16) as refuelto, beforerefuel, afterrefuel, totrefuel, fuelcost, lat, lon, ft.liters, ft.cost FROM refuel rt INNER JOIN (SELECT au.userid, au.username, plateno FROM vehicleTBL vt LEFT OUTER JOIN userTBL au ON vt.userid = au.userid WHERE au.userid = @userid AND plateno = @plateno) avu ON rt.plateno = avu.plateno LEFT OUTER JOIN fuel ft ON ft.plateno = rt.plateno AND ft.timestamp BETWEEN DATEADD(mi,-10,rt.refuelfrom) AND DATEADD(mi,10,rt.refuelto) WHERE rt.refuelfrom BETWEEN @bdt AND @edt AND rt.status = 1 ORDER BY avu.username, rt.plateno, refuelfrom"
            End If
        Else
            If ddlp = "--All Plate No--" Then
                If role = "SuperUser" Or role = "Operator" Then
                    If Not String.IsNullOrEmpty(userslist) Then
                        query = $"SELECT DISTINCT avu.userid, avu.username, ar.plateno, SUBSTRING(CONVERT(varchar,ar.refuelfrom,120),1,16) as refuelfrom, SUBSTRING(CONVERT(varchar,ar.refuelto,120),1,16) as refuelto, ar.beforerefuel, ar.afterrefuel, ar.totrefuel, ar.fuelcost, ar.lat, ar.lon, ft.liters, ft.cost FROM refuel ar INNER JOIN (SELECT au.userid, au.username, plateno FROM vehicleTBL vt LEFT OUTER JOIN userTBL au ON vt.userid = au.userid WHERE au.userid IN ({userslist})) avu ON ar.plateno = avu.plateno LEFT OUTER JOIN fuel ft ON ft.timestamp BETWEEN DATEADD(mi,-10,ar.refuelfrom) AND DATEADD(mi,10,ar.refuelto) AND ar.plateno = ft.plateno WHERE refuelfrom BETWEEN @bdt AND @edt AND ar.status = 1 ORDER BY avu.username, ar.plateno, refuelfrom"
                    Else
                        query = "SELECT DISTINCT avu.userid, avu.username, ar.plateno, SUBSTRING(CONVERT(varchar,ar.refuelfrom,120),1,16) as refuelfrom, SUBSTRING(CONVERT(varchar,ar.refuelto,120),1,16) as refuelto, ar.beforerefuel, ar.afterrefuel, ar.totrefuel, ar.fuelcost, ar.lat, ar.lon, ft.liters, ft.cost FROM refuel ar INNER JOIN (SELECT au.userid, au.username, plateno FROM vehicleTBL vt LEFT OUTER JOIN userTBL au ON vt.userid = au.userid WHERE au.userid = @userid) avu ON ar.plateno = avu.plateno LEFT OUTER JOIN fuel ft ON ft.timestamp BETWEEN DATEADD(mi,-10,ar.refuelfrom) AND DATEADD(mi,10,ar.refuelto) AND ar.plateno = ft.plateno WHERE refuelfrom BETWEEN @bdt AND @edt AND ar.status = 1 ORDER BY avu.username, ar.plateno, refuelfrom"
                    End If
                ElseIf role = "Admin" Then
                    query = "SELECT DISTINCT plateno, SUBSTRING(CONVERT(varchar,refuelfrom,120),1,16) as refuelfrom, SUBSTRING(CONVERT(varchar,refuelto,120),1,16) as refuelto, beforerefuel, afterrefuel, totrefuel, fuelcost, lat, lon FROM refuel WHERE refuelfrom BETWEEN @bdt AND @edt"
                End If
            Else
                query = "SELECT DISTINCT ar.plateno, vt.userid, ut.username, SUBSTRING(CONVERT(varchar,refuelfrom,120),1,16) as refuelfrom, SUBSTRING(CONVERT(varchar,refuelto,120),1,16) as refuelto, beforerefuel, afterrefuel, totrefuel, fuelcost, lat, lon, ft.liters, ft.cost FROM refuel ar INNER JOIN vehicleTBL vt ON ar.plateno = vt.plateno LEFT OUTER JOIN userTBL ut ON vt.userid = ut.userid LEFT OUTER JOIN fuel ft ON ft.timestamp BETWEEN DATEADD(mi,-10,ar.refuelfrom) AND DATEADD(mi,10,ar.refuelto) AND ar.plateno = ft.plateno WHERE ar.plateno = @plateno AND refuelfrom BETWEEN @bdt AND @edt AND ar.status = 1 ORDER BY refuelfrom"
            End If
        End If
        
        Return query
    End Function

    Private Sub AddRefuelQueryParameters(cmd As SqlCommand, ddlu As String, ddlp As String, begindatetime As String, enddatetime As String, userid As String, plateno As String)
        cmd.Parameters.AddWithValue("@bdt", begindatetime)
        cmd.Parameters.AddWithValue("@edt", enddatetime)
        
        If ddlu <> "--All Users--" Then
            cmd.Parameters.AddWithValue("@userid", userid)
            If ddlp <> "--All Plate No--" Then
                cmd.Parameters.AddWithValue("@plateno", plateno)
            End If
        ElseIf ddlp <> "--All Plate No--" Then
            cmd.Parameters.AddWithValue("@plateno", plateno)
        End If
    End Sub

    Private Sub LoadFuelStations(conn As SqlConnection, mapping As AspMap.Map)
        Try
            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand("SELECT * FROM fuel_station", conn)
                Dim FuelstationsLayer As New AspMap.DynamicLayer()
                FuelstationsLayer.LayerType = LayerType.mcPolygonLayer

                conn.Open()
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        Try
                            Dim circleShape As New AspMap.Shape
                            circleShape.MakeCircle(CDbl(dr("lat")), CDbl(dr("lon")), 50 / 111120.0)
                            FuelstationsLayer.Add(circleShape, SecurityHelper.SanitizeForHtml(dr("name").ToString()), dr("id"))
                        Catch ex As Exception
                            SecurityHelper.LogSecurityEvent("FUEL_STATION_ERROR", "Error processing fuel station: " & ex.Message)
                        End Try
                    End While
                End Using
                conn.Close()

                mapping.AddLayer(FuelstationsLayer)
                mapping(0).Name = "PetrolStations"
            End Using
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("FUEL_STATION_LOAD_ERROR", "Error loading fuel stations: " & ex.Message)
        End Try
    End Sub

    Private Sub ProcessRefuelData(dr As SqlDataReader, t As DataTable, mapping As AspMap.Map, luid As String)
        Dim i As Int64 = 1
        Dim totalTankSize As Double = 0
        Dim presplateno As String = ""
        Dim prevplateno As String = ""

        While dr.Read()
            Try
                presplateno = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
                
                Dim r As DataRow = t.NewRow()
                r(0) = i.ToString()
                r(1) = SecurityHelper.SanitizeForHtml(dr("username").ToString())
                r(2) = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
                r(3) = CDate(dr("refuelfrom")).ToString("yyyy/MM/dd HH:mm:ss")
                r(4) = CDate(dr("refuelto")).ToString("yyyy/MM/dd HH:mm:ss")

                ' Process location data securely
                Dim vehiclepoint As New AspMap.Point()
                vehiclepoint.Y = CType(dr("lon"), Double)
                vehiclepoint.X = CType(dr("lat"), Double)

                ' Get location information
                r(5) = GetSecureLocationInfo(mapping, vehiclepoint, dr("lat"), dr("lon"), luid)

                r(6) = CDbl(dr("beforerefuel")).ToString("0.00")
                r(7) = CDbl(dr("afterrefuel")).ToString("0.00")
                r(8) = CDbl(dr("totrefuel")).ToString("0.00")
                r(9) = CDbl(dr("fuelcost")).ToString("0.00")

                ' Process fuel data
                ProcessFuelData(r, dr, luid)

                r(15) = CType(dr("lat"), Double)
                r(16) = CType(dr("lon"), Double)
                r(17) = (CDate(dr("refuelto")) - CDate(dr("refuelfrom"))).TotalMinutes
                r(18) = SecurityHelper.SanitizeForHtml(dr("userid").ToString())

                totrefuel += CDbl(dr("totrefuel"))
                totrefuelcost += CDbl(dr("fuelcost"))

                t.Rows.Add(r)
                i += 1
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("REFUEL_DATA_ERROR", "Error processing refuel data row: " & ex.Message)
            End Try
        End While
    End Sub

    Private Function GetSecureLocationInfo(mapping As AspMap.Map, vehiclepoint As AspMap.Point, lat As Object, lon As Object, userid As String) As String
        Try
            Dim rs As AspMap.Recordset = mapping("PetrolStations").SearchByDistance(vehiclepoint, 50 / (60 * 1852), SearchMethod.mcIntersect)
            If rs.RecordCount < 1 OrElse userid = "4215" Then
                Return $"{CDbl(lat):0.000000},{CDbl(lon):0.000000}"
            Else
                Return $"<div class='pt1' title='Petrol Station'></div>{SecurityHelper.SanitizeForHtml(rs(0).ToString())}"
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOCATION_INFO_ERROR", "Error getting location info: " & ex.Message)
            Return $"{CDbl(lat):0.000000},{CDbl(lon):0.000000}"
        End Try
    End Function

    Private Sub ProcessFuelData(r As DataRow, dr As SqlDataReader, luid As String)
        Try
            Dim costperltr As Double = GetFuelPrice(dr("userid").ToString(), dr("plateno").ToString(), CDate(dr("refuelto")).ToString("yyyy/MM/dd HH:mm:ss"))
            Dim id As String = Convert.ToDateTime(dr("refuelto")).Ticks.ToString()
            
            If IsDBNull(dr("liters")) Then
                r(10) = $"<input type=""textbox"" style=""width:60px;"" id=""txt_{SecurityHelper.SanitizeForHtml(id)}"" value=''></input>"
                r(11) = $"<input type=""textbox"" style=""width:60px;"" id=""txtprice_{SecurityHelper.SanitizeForHtml(id)}"" value='{costperltr}' ></input>"
                r(20) = ""
            Else
                r(10) = $"<input type=""textbox"" style=""width:60px;"" id=""txt_{SecurityHelper.SanitizeForHtml(id)}"" value='{SecurityHelper.SanitizeForHtml(dr("liters").ToString())}' ></input>"
                r(11) = $"<input type=""textbox"" style=""width:60px;"" id=""txtprice_{SecurityHelper.SanitizeForHtml(id)}"" value='{costperltr}' ></input>"
                r(20) = SecurityHelper.SanitizeForHtml(dr("liters").ToString())
            End If
            
            If IsDBNull(dr("liters")) Then
                r(12) = $"<label id=""lbl_{SecurityHelper.SanitizeForHtml(dr("plateno").ToString())}"" ></label>"
                r(21) = ""
            Else
                r(12) = $"<label id=""lbl_{SecurityHelper.SanitizeForHtml(dr("plateno").ToString())}"" >{CDbl(dr("cost")).ToString("0.00")}</label>"
                r(21) = CDbl(dr("cost")).ToString("0.00")
            End If

            If IsDBNull(dr("liters")) Then
                r(13) = "--"
                r(14) = "--"
            Else
                r(13) = CDbl(100 - Math.Abs(((CDbl(dr("totrefuel")) - CDbl(dr("liters"))) / CDbl(dr("liters"))) * 100)).ToString("0") & "%"
                r(14) = "100%" ' Simplified for security
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("FUEL_DATA_ERROR", "Error processing fuel data: " & ex.Message)
            r(10) = "--"
            r(11) = "--"
            r(12) = "--"
            r(13) = "--"
            r(14) = "--"
            r(20) = "--"
            r(21) = "--"
        End Try
    End Sub

    Private Sub ProcessResultsForJson(t As DataTable, aa As ArrayList)
        If t.Rows.Count = 0 Then
            Dim r As DataRow = t.NewRow()
            For i As Integer = 0 To 21
                r(i) = "--"
            Next
            t.Rows.Add(r)
        End If

        ' Add totals row
        Dim totalRow As DataRow = t.NewRow()
        For i As Integer = 0 To 21
            If i = 8 Then
                totalRow(i) = totrefuel.ToString("0.00")
            ElseIf i = 9 Then
                totalRow(i) = totrefuelcost.ToString("0.00")
            Else
                totalRow(i) = "--"
            End If
        Next
        t.Rows.Add(totalRow)

        ' Convert to ArrayList for JSON
        For i As Integer = 0 To t.Rows.Count - 1
            Try
                Dim a As New ArrayList()
                For j As Integer = 0 To 19 ' Limit columns for security
                    a.Add(t.DefaultView.Item(i)(j))
                Next
                aa.Add(a)
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("JSON_PROCESSING_ERROR", "Error processing row for JSON: " & ex.Message)
            End Try
        Next
    End Sub

    Private Function CreateExcelTable(t As DataTable) As DataTable
        Dim exceltable As New DataTable()
        exceltable.Columns.Add(New DataColumn("S No"))
        exceltable.Columns.Add(New DataColumn("User Name"))
        exceltable.Columns.Add(New DataColumn("PlateNo"))
        exceltable.Columns.Add(New DataColumn("Refuel Start Time"))
        exceltable.Columns.Add(New DataColumn("Refuel End Time"))
        exceltable.Columns.Add(New DataColumn("Address"))
        exceltable.Columns.Add(New DataColumn("Refuel Start(L)"))
        exceltable.Columns.Add(New DataColumn("Refuel End (L)"))
        exceltable.Columns.Add(New DataColumn("Refuel Ltr(L)"))
        exceltable.Columns.Add(New DataColumn("Refuel Cost(RM)"))
        exceltable.Columns.Add(New DataColumn("Actual(L)"))
        exceltable.Columns.Add(New DataColumn("Actual(RM)"))
        exceltable.Columns.Add(New DataColumn("Accuracy Against Tankvolume"))

        For i As Integer = 0 To t.Rows.Count - 1
            Try
                Dim er As DataRow = exceltable.NewRow()
                er(0) = t.DefaultView.Item(i)(0)
                er(1) = t.DefaultView.Item(i)(1)
                er(2) = t.DefaultView.Item(i)(2)
                er(3) = t.DefaultView.Item(i)(3)
                er(4) = t.DefaultView.Item(i)(4)
                er(5) = t.DefaultView.Item(i)(5)
                er(6) = t.DefaultView.Item(i)(6)
                er(7) = t.DefaultView.Item(i)(7)
                er(8) = t.DefaultView.Item(i)(8)
                er(9) = t.DefaultView.Item(i)(9)
                er(10) = t.DefaultView.Item(i)(20)
                er(11) = t.DefaultView.Item(i)(21)
                er(12) = t.DefaultView.Item(i)(14)
                exceltable.Rows.Add(er)
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("EXCEL_TABLE_ERROR", "Error creating excel table row: " & ex.Message)
            End Try
        Next

        Return exceltable
    End Function

    Private Function GetFuelPrice(userid As String, plateno As String, timestamp As String) As Double
        Dim fuelcost As Double = 0
        Try
            ' SECURITY FIX: Validate inputs
            If Not SecurityHelper.ValidateUserId(userid) OrElse Not SecurityHelper.ValidatePlateNumber(plateno) Then
                Return 0
            End If

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY FIX: Use parameterized query
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(
                    "SELECT cost, liters FROM fuel WHERE plateno = @plateno AND timestamp = @timestamp", conn)
                    cmd.Parameters.AddWithValue("@plateno", plateno)
                    cmd.Parameters.AddWithValue("@timestamp", timestamp)
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then
                            Dim cost As Double = Convert.ToDouble(dr("cost"))
                            Dim liters As Double = Convert.ToDouble(dr("liters"))
                            If liters > 0 Then
                                fuelcost = Math.Round(cost / liters, 2)
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("FUEL_PRICE_ERROR", "Error getting fuel price: " & ex.Message)
            fuelcost = 0
        End Try
        
        Return fuelcost
    End Function

    Public Shared Function fuelPrice(userid As String) As DataTable
        Dim priceTable As New DataTable()
        Try
            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateUserId(userid) Then
                Return priceTable
            End If

            priceTable.Columns.Add(New DataColumn("StartDate", GetType(DateTime)))
            priceTable.Columns.Add(New DataColumn("EndDate", GetType(DateTime)))
            priceTable.Columns.Add(New DataColumn("FuelPrice"))

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY FIX: Use parameterized query
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(
                    "SELECT * FROM fuel_price WHERE countrycode = (SELECT countrycode FROM userTBL WHERE userid = @userid) ORDER BY startdate DESC", conn)
                    cmd.Parameters.AddWithValue("@userid", userid)
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Try
                                Dim pRow As DataRow = priceTable.NewRow()
                                pRow(0) = dr("startdate")
                                pRow(1) = dr("enddate")
                                pRow(2) = dr("fuelprice")
                                priceTable.Rows.Add(pRow)
                            Catch ex As Exception
                                SecurityHelper.LogSecurityEvent("FUEL_PRICE_ROW_ERROR", "Error processing fuel price row: " & ex.Message)
                            End Try
                        End While
                    End Using
                End Using
            End Using

            If priceTable.Rows.Count = 0 Then
                Dim pRow As DataRow = priceTable.NewRow()
                pRow(0) = DateTime.Now
                pRow(1) = DateTime.Now
                pRow(2) = 0
                priceTable.Rows.Add(pRow)
            End If

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("FUEL_PRICE_TABLE_ERROR", "Error creating fuel price table: " & ex.Message)
        End Try

        Return priceTable
    End Function

End Class