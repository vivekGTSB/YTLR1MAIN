Imports AspMap
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class GetViolationDailyReport
    Inherits System.Web.UI.Page

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
            If Not SecurityHelper.CheckRateLimit("GetViolationDailyReport_" & GetClientIP(), 50, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.Write("Rate limit exceeded")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate operation parameter
            Dim operation As String = Request.QueryString("op")
            If String.IsNullOrEmpty(operation) OrElse Not SecurityHelper.ValidateInput(operation, "numeric") Then
                Response.StatusCode = 400
                Response.Write("Invalid operation parameter")
                Response.End()
                Return
            End If

            Select Case operation
                Case "1"
                    Response.Write(LoadVehicleGroup(ValidateParameter("uid")))
                Case "2"
                    Response.Write(LoadVehicles(ValidateParameter("groupid"), ValidateParameter("uid")))
                Case "3"
                    Response.Write(FillGrid(ValidateParameter("pno"), ValidateParameter("bdt"), ValidateParameter("edt"), ValidateParameter("uid"), ValidateParameter("gid")))
                Case "4"
                    Response.Write(FillGridMonth(ValidateParameter("pno"), ValidateParameter("bdt"), ValidateParameter("edt"), ValidateParameter("uid"), ValidateParameter("gid")))
                Case Else
                    Response.StatusCode = 400
                    Response.Write("Invalid operation")
            End Select

            Response.ContentType = "application/json"

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("VIOLATION_REPORT_ERROR", ex.Message)
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

    ' SECURITY FIX: Validate query parameters
    Private Function ValidateParameter(paramName As String) As String
        Dim value As String = Request.QueryString(paramName)
        If String.IsNullOrEmpty(value) Then
            Return String.Empty
        End If

        Select Case paramName.ToLower()
            Case "uid", "groupid", "gid"
                If value = "ALL" OrElse SecurityHelper.ValidateInput(value, "username") Then
                    Return value
                End If
            Case "pno"
                If value = "ALL" OrElse SecurityHelper.ValidatePlateNumber(value) Then
                    Return value
                End If
            Case "bdt", "edt"
                If SecurityHelper.ValidateDate(value) Then
                    Return value
                End If
        End Select

        Return String.Empty
    End Function

    Public Function LoadVehicleGroup(ByVal userid As String) As String
        Dim aa As New ArrayList()
        Dim json As String = ""
        Try
            ' SECURITY FIX: Get user data from session
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
            
            Dim query As String
            Dim parameters As New Dictionary(Of String, Object)()

            If userid = "ALL" Then
                If SecurityHelper.ValidateUsersList(userslist) Then
                    query = $"SELECT groupname FROM vehicle_group WHERE userid IN ({userslist}) ORDER BY groupname"
                Else
                    query = "SELECT groupname FROM vehicle_group WHERE 1=0"
                End If
            Else
                query = "SELECT groupname FROM vehicle_group WHERE userid = @userid ORDER BY groupname"
                parameters.Add("@userid", userid)
            End If

            Dim dataTable As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            For Each row As DataRow In dataTable.Rows
                Dim a As New ArrayList()
                Dim groupName As String = SecurityHelper.HtmlEncode(row("groupname").ToString())
                a.Add(groupName)
                a.Add(groupName)
                aa.Add(a)
            Next

            json = JsonConvert.SerializeObject(aa, Formatting.None)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOAD_VEHICLE_GROUP_ERROR", ex.Message)
            json = "[]"
        End Try

        Return json
    End Function

    Public Function LoadVehicles(ByVal groupid As String, ByVal userid As String) As String
        Dim a As New ArrayList()
        Dim json As String = ""
        Try
            ' SECURITY FIX: Get user data from session
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim uid As String = SecurityHelper.ValidateAndGetUserId(Request)

            Dim query As String
            Dim parameters As New Dictionary(Of String, Object)()

            If groupid = "ALL" Then
                If role = "SuperUser" OrElse role = "Operator" Then
                    If SecurityHelper.ValidateUsersList(userslist) Then
                        query = $"SELECT plateno FROM vehicleTBL WHERE Userid IN ({userslist})"
                    Else
                        query = "SELECT plateno FROM vehicleTBL WHERE 1=0"
                    End If
                Else
                    query = "SELECT plateno FROM vehicleTBL WHERE Userid = @userid"
                    parameters.Add("@userid", userid)
                End If
            Else
                query = "SELECT plateno FROM vehicleTBL WHERE groupname = @groupid"
                parameters.Add("@groupid", groupid)
            End If

            Dim dataTable As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            For Each row As DataRow In dataTable.Rows
                a.Add(SecurityHelper.HtmlEncode(row("plateno").ToString()))
            Next

            json = JsonConvert.SerializeObject(a, Formatting.None)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOAD_VEHICLES_ERROR", ex.Message)
            json = "[]"
        End Try

        Return json
    End Function

    Public Function FillGrid(ByVal plateno As String, ByVal bdt As String, ByVal edt As String, ByVal uid As String, ByVal gid As String) As String
        Dim aa As New ArrayList()
        Dim json As String = ""
        Try
            ' SECURITY FIX: Validate date parameters
            If Not SecurityHelper.ValidateDate(bdt) OrElse Not SecurityHelper.ValidateDate(edt) Then
                Return JsonConvert.SerializeObject(aa, Formatting.None)
            End If

            ' SECURITY FIX: Get user data from session
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim usid As String = SecurityHelper.ValidateAndGetUserId(Request)

            ' Build filter condition
            Dim filter As String = BuildViolationFilter(plateno, gid, uid, role, userslist, usid)
            
            Dim query As String = $"SELECT plateno, timestamp, [dbo].[fn_Getgroupname](plateno) AS Grp, overspeed, idling, hdec, totalCont4HourCount, totaldriverhour, totalworkhour, distance, midnightcount, speed9095, speed95100, speed100 FROM ohsas_violation WHERE {filter} AND timestamp BETWEEN @startDate AND @endDate ORDER BY timestamp, plateno"
            
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@startDate", DateTime.Parse(bdt).ToString("yyyy-MM-dd") & " 00:00:00"},
                {"@endDate", DateTime.Parse(edt).ToString("yyyy-MM-dd") & " 23:59:59"}
            }

            ' Add user-specific parameters
            AddViolationParameters(parameters, plateno, gid, uid, role, userslist, usid)

            Dim dataTable As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            ' Process violation data
            ProcessViolationData(dataTable, aa, False)

            ' Store for Excel export
            StoreViolationDataForExport(dataTable, False)

            json = JsonConvert.SerializeObject(aa, Formatting.None)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("FILL_GRID_ERROR", ex.Message)
            json = "[]"
        End Try

        Return json
    End Function

    Public Function FillGridMonth(ByVal plateno As String, ByVal bdt As String, ByVal edt As String, ByVal uid As String, ByVal gid As String) As String
        Dim aa As New ArrayList()
        Dim json As String = ""
        Try
            ' SECURITY FIX: Validate date parameters
            If Not SecurityHelper.ValidateDate(bdt) OrElse Not SecurityHelper.ValidateDate(edt) Then
                Return JsonConvert.SerializeObject(aa, Formatting.None)
            End If

            ' SECURITY FIX: Get user data from session
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim usid As String = SecurityHelper.ValidateAndGetUserId(Request)

            ' Build filter condition
            Dim filter As String = BuildViolationFilter(plateno, gid, uid, role, userslist, usid)
            
            
            
            Dim query As String = $"SELECT plateno, [dbo].[fn_Getgroupname](plateno) AS Grp, SUM(overspeed) overspeed, SUM(speed9095) overspeed9095, SUM(speed95100) overspeed95100, SUM(speed100) overspeed100, SUM(idling) idling, SUM(hdec) hdec, SUM(totalCont4HourCount) contdrive, SUM(totaldriverhour) totaldriverhour, SUM(totalworkhour) totalworkhour, SUM(distance) distance, SUM(midnightcount) midnightcount, dbo.fnGetUnsafedrivingCount(plateno, @startDate, @endDate) unsafedrive, dbo.fnGetUnsafeWorkCount(plateno, @startDate, @endDate) unsafework FROM ohsas_violation WHERE {filter} AND timestamp BETWEEN @startDate AND @endDate GROUP BY plateno"
            
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@startDate", DateTime.Parse(bdt).ToString("yyyy-MM-dd")},
                {"@endDate", DateTime.Parse(edt).ToString("yyyy-MM-dd")}
            }

            ' Add user-specific parameters
            AddViolationParameters(parameters, plateno, gid, uid, role, userslist, usid)

            Dim dataTable As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            ' Process violation data
            ProcessViolationData(dataTable, aa, True)

            ' Store for Excel export
            StoreViolationDataForExport(dataTable, True)

            json = JsonConvert.SerializeObject(aa, Formatting.None)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("FILL_GRID_MONTH_ERROR", ex.Message)
            json = "[]"
        End Try

        Return json
    End Function

    ' SECURITY FIX: Build secure violation filter
    Private Function BuildViolationFilter(plateno As String, gid As String, uid As String, role As String, userslist As String, usid As String) As String
        If plateno <> "ALL" Then
            Return "plateno = @plateno"
        ElseIf gid <> "ALL" Then
            Return "plateno IN (SELECT plateno FROM vehicleTBL WHERE userid = @filterUserId AND groupname = @groupName)"
        ElseIf uid = "ALL" Then
            If role = "SuperUser" OrElse role = "Operator" Then
                If SecurityHelper.ValidateUsersList(userslist) Then
                    Return $"plateno IN (SELECT plateno FROM vehicleTBL WHERE userid IN ({userslist}))"
                Else
                    Return "1=0"
                End If
            Else
                Return "plateno IN (SELECT plateno FROM vehicleTBL WHERE userid = @currentUserId)"
            End If
        Else
            Return "plateno IN (SELECT plateno FROM vehicleTBL WHERE userid = @filterUserId)"
        End If
    End Function

    ' SECURITY FIX: Add violation parameters
    Private Sub AddViolationParameters(parameters As Dictionary(Of String, Object), plateno As String, gid As String, uid As String, role As String, userslist As String, usid As String)
        If plateno <> "ALL" Then
            parameters.Add("@plateno", plateno)
        ElseIf gid <> "ALL" Then
            parameters.Add("@filterUserId", uid)
            parameters.Add("@groupName", gid)
        ElseIf uid = "ALL" Then
            If Not (role = "SuperUser" OrElse role = "Operator") Then
                parameters.Add("@currentUserId", usid)
            End If
        Else
            parameters.Add("@filterUserId", uid)
        End If
    End Sub

    ' Process violation data
    Private Sub ProcessViolationData(dataTable As DataTable, aa As ArrayList, isMonthly As Boolean)
        Dim totals As New Dictionary(Of String, Double)
        InitializeTotals(totals)

        For Each row As DataRow In dataTable.Rows
            Try
                Dim a As New ArrayList()
                
                If Not isMonthly Then
                    a.Add(SecurityHelper.HtmlEncode(row("plateno").ToString()))
                    a.Add(DateTime.Parse(row("timestamp")).ToString("yyyy/MM/dd"))
                End If
                
                a.Add(SecurityHelper.HtmlEncode(row("plateno").ToString()))
                a.Add(SecurityHelper.HtmlEncode(row("Grp").ToString()))
                
                ' Process speed violations
                Dim overspeed As Integer = CInt(row("overspeed"))
                Dim speedAbove90 As Integer
                
                If isMonthly Then
                    speedAbove90 = CInt(row("overspeed9095")) + CInt(row("overspeed95100")) + CInt(row("overspeed100"))
                Else
                    speedAbove90 = CInt(row("speed9095")) + CInt(row("speed95100")) + CInt(row("speed100"))
                End If
                
                a.Add(overspeed)
                a.Add(speedAbove90)
                a.Add(CInt(row("idling")))
                a.Add(CInt(CDbl(row("hdec")) / 22))
                
                If isMonthly Then
                    a.Add(CInt(row("contdrive")))
                Else
                    a.Add(CInt(row("totalCont4HourCount")))
                End If
                
                ' Convert minutes to time format
                Dim drivingHours As TimeSpan = New TimeSpan(0, CInt(row("totaldriverhour")), 0)
                Dim workHours As TimeSpan = New TimeSpan(0, CInt(row("totalworkhour")), 0)
                
                a.Add(ConvertHours(drivingHours.ToString()))
                a.Add(ConvertHours(workHours.ToString()))
                
                If isMonthly Then
                    a.Add(CInt(row("unsafework")))
                    a.Add(CInt(row("unsafedrive")))
                Else
                    a.Add(If(CInt(row("totalworkhour")) > 840, 1, 0))
                    a.Add(If(CInt(row("totaldriverhour")) > 600, 1, 0))
                End If
                
                Dim totalViolations As Integer = overspeed + CInt(row("idling")) + CInt(CDbl(row("hdec")) / 22)
                a.Add(totalViolations)
                a.Add(CDbl(row("distance")).ToString("0.00"))
                a.Add(CInt(row("midnightcount")))
                
                ' Update totals
                UpdateTotals(totals, row, isMonthly)
                
                aa.Add(a)
                
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("PROCESS_VIOLATION_ROW_ERROR", ex.Message)
            End Try
        Next

        ' Add totals row if data exists
        If aa.Count > 0 Then
            AddTotalsRow(aa, totals, isMonthly)
        Else
            AddEmptyRow(aa, isMonthly)
        End If
    End Sub

    ' Initialize totals dictionary
    Private Sub InitializeTotals(totals As Dictionary(Of String, Double))
        totals.Add("overspeed", 0)
        totals.Add("speedAbove90", 0)
        totals.Add("idling", 0)
        totals.Add("harshBreak", 0)
        totals.Add("contDrive", 0)
        totals.Add("totalDrive", 0)
        totals.Add("totalWork", 0)
        totals.Add("unsafeWork", 0)
        totals.Add("unsafeDrive", 0)
        totals.Add("totalViolations", 0)
        totals.Add("distance", 0)
        totals.Add("midnight", 0)
    End Sub

    ' Update totals
    Private Sub UpdateTotals(totals As Dictionary(Of String, Double), row As DataRow, isMonthly As Boolean)
        totals("overspeed") += CInt(row("overspeed"))
        
        If isMonthly Then
            totals("speedAbove90") += CInt(row("overspeed9095")) + CInt(row("overspeed95100")) + CInt(row("overspeed100"))
            totals("contDrive") += CInt(row("contdrive"))
            totals("unsafeWork") += CInt(row("unsafework"))
            totals("unsafeDrive") += CInt(row("unsafedrive"))
        Else
            totals("speedAbove90") += CInt(row("speed9095")) + CInt(row("speed95100")) + CInt(row("speed100"))
            totals("contDrive") += CInt(row("totalCont4HourCount"))
            If CInt(row("totalworkhour")) > 840 Then totals("unsafeWork") += 1
            If CInt(row("totaldriverhour")) > 600 Then totals("unsafeDrive") += 1
        End If
        
        totals("idling") += CInt(row("idling"))
        totals("harshBreak") += CInt(CDbl(row("hdec")) / 22)
        totals("totalDrive") += CInt(row("totaldriverhour"))
        totals("totalWork") += CInt(row("totalworkhour"))
        totals("totalViolations") += CInt(row("overspeed")) + CInt(row("idling")) + CInt(CDbl(row("hdec")) / 22)
        totals("distance") += CDbl(row("distance"))
        totals("midnight") += CInt(row("midnightcount"))
    End Sub

    ' Add totals row
    Private Sub AddTotalsRow(aa As ArrayList, totals As Dictionary(Of String, Double), isMonthly As Boolean)
        Dim a As New ArrayList()
        
        a.Add(totals("overspeed"))
        a.Add(totals("speedAbove90"))
        a.Add(totals("idling"))
        a.Add(totals("harshBreak"))
        a.Add(totals("contDrive"))
        
        Dim drivingTime As TimeSpan = New TimeSpan(0, CInt(totals("totalDrive")), 0)
        Dim workTime As TimeSpan = New TimeSpan(0, CInt(totals("totalWork")), 0)
        
        a.Add(ConvertHours(drivingTime.ToString()))
        a.Add(ConvertHours(workTime.ToString()))
        a.Add(totals("unsafeWork"))
        a.Add(totals("unsafeDrive"))
        a.Add(totals("totalViolations"))
        a.Add(totals("distance").ToString("0.00"))
        a.Add(totals("midnight"))
        
        aa.Add(a)
    End Sub

    ' Add empty row
    Private Sub AddEmptyRow(aa As ArrayList, isMonthly As Boolean)
        Dim a As New ArrayList()
        For i As Integer = 0 To 11
            a.Add("0")
        Next
        aa.Add(a)
    End Sub

    ' Store violation data for Excel export
    Private Sub StoreViolationDataForExport(dataTable As DataTable, isMonthly As Boolean)
        Try
            Dim vehiclestable As New DataTable()
            
            If isMonthly Then
                vehiclestable.Columns.Add(New DataColumn("S No"))
                vehiclestable.Columns.Add(New DataColumn("Plate No"))
            Else
                vehiclestable.Columns.Add(New DataColumn("S No"))
                vehiclestable.Columns.Add(New DataColumn("Violation Date"))
                vehiclestable.Columns.Add(New DataColumn("Plate No"))
            End If
            
            vehiclestable.Columns.Add(New DataColumn("Group"))
            vehiclestable.Columns.Add(New DataColumn("Over Speed"))
            vehiclestable.Columns.Add(New DataColumn("speed >90"))
            vehiclestable.Columns.Add(New DataColumn("Idle"))
            vehiclestable.Columns.Add(New DataColumn("Harsh Break"))
            vehiclestable.Columns.Add(New DataColumn("CntDrv"))
            vehiclestable.Columns.Add(New DataColumn("TotDrv"))
            vehiclestable.Columns.Add(New DataColumn("TotWork"))
            vehiclestable.Columns.Add(New DataColumn("Frequency Work>14 Hrs"))
            vehiclestable.Columns.Add(New DataColumn("Frequency Driving>10 Hrs"))
            vehiclestable.Columns.Add(New DataColumn("TotVio"))
            vehiclestable.Columns.Add(New DataColumn("Distance"))
            vehiclestable.Columns.Add(New DataColumn("Mid-Night Count"))

            HttpContext.Current.Session.Remove("tempTable")
            HttpContext.Current.Session("exceltable") = vehiclestable

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("STORE_EXCEL_DATA_ERROR", ex.Message)
        End Try
    End Sub

    Protected Function ConvertHours(ByVal p_hour As String) As String
        Try
            Dim sFLD() As String
            Dim sfld2() As String
            Dim hours As String
            Dim iPos As Integer
            
            hours = p_hour
            iPos = p_hour.IndexOf(".")
            
            If iPos > 0 Then
                sFLD = p_hour.Split("."c)
                sfld2 = sFLD(1).Split(":"c)
                hours = CStr(CInt(sFLD(0)) * 24 + CInt(sfld2(0))) & ":" & sfld2(1) & ":" & sfld2(2)
            End If
            
            Return hours
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("CONVERT_HOURS_ERROR", ex.Message)
            Return "00:00:00"
        End Try
    End Function

End Class