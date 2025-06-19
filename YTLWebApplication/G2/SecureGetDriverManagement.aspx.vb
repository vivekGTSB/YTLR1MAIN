Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class SecureGetDriverManagement
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY: Validate session
            If Not G2SecurityHelper.ValidateSession() Then
                Response.StatusCode = 401
                Response.Write("Unauthorized")
                Response.End()
                Return
            End If
            
            ' SECURITY: Check authorization
            If Not G2SecurityHelper.HasRequiredRole("OPERATOR") Then
                Response.StatusCode = 403
                Response.Write("Forbidden")
                Response.End()
                Return
            End If
            
            ' SECURITY: Rate limiting
            Dim clientIP As String = Request.UserHostAddress
            If Not G2SecurityHelper.CheckRateLimit("DriverMgmt_" & clientIP, 30, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.Write("Too Many Requests")
                Response.End()
                Return
            End If
            
            ' SECURITY: Validate operation parameter
            Dim operation As String = Request.QueryString("op")
            If Not G2SecurityHelper.ValidateG2Input(operation, G2InputType.AlphaNumeric, 1) Then
                Response.StatusCode = 400
                Response.Write("Invalid operation")
                Response.End()
                Return
            End If
            
            Select Case operation
                Case "1"
                    Response.Write(SecureFillGrid())
                Case "2"
                    Response.Write(SecureInsertUpdateDriver())
                Case "3"
                    Response.Write(SecureActivate())
                Case "4"
                    Response.Write(SecureInActivate())
                Case Else
                    Response.StatusCode = 400
                    Response.Write("Invalid operation")
                    Response.End()
                    Return
            End Select
            
            Response.ContentType = "application/json"
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("DRIVER_MANAGEMENT_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Internal Server Error")
        End Try
    End Sub
    
    Public Function SecureFillGrid() As String
        Dim json As String = Nothing
        
        Try
            ' SECURITY: Get and validate user data from session
            Dim userid As String = G2SecurityHelper.GetCurrentUserId()
            Dim role As String = G2SecurityHelper.GetCurrentUserRole()
            
            If String.IsNullOrEmpty(userid) OrElse String.IsNullOrEmpty(role) Then
                Throw New UnauthorizedAccessException("Invalid session data")
            End If
            
            ' SECURITY: Validate ugData parameter
            Dim ugData As String = Request.QueryString("ugData")
            If Not G2SecurityHelper.ValidateG2Input(ugData, G2InputType.AlphaNumeric, 50) Then
                Throw New ArgumentException("Invalid ugData parameter")
            End If
            
            Dim userslist As String = HttpContext.Current.Session("userslist").ToString()
            If Not G2SecurityHelper.ValidateUsersList(userslist) Then
                userslist = userid ' Fallback to current user only
            End If
            
            Dim drivertable As New DataTable
            InitializeDriverTable(drivertable)
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = BuildSecureDriverQuery(ugData, role, userid, userslist)
                Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, conn)
                    
                    ' SECURITY: Add parameters based on query type
                    If ugData <> "ALL" AndAlso ugData <> "SELECT USERNAME" Then
                        cmd.Parameters.AddWithValue("@ugData", ugData)
                    End If
                    
                    If role = "SuperUser" Or role = "Operator" Then
                        If ugData <> "ALL" Then
                            cmd.Parameters.AddWithValue("@userid", ugData)
                        End If
                    ElseIf role = "User" Then
                        cmd.Parameters.AddWithValue("@userid", userid)
                    End If
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        PopulateDriverTable(drivertable, dr)
                    End Using
                End Using
            End Using
            
            ' SECURITY: Store sanitized data in session for Excel export
            StoreExcelData(drivertable)
            
            json = ConvertDriverTableToJson(drivertable)
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("SECURE_FILL_GRID_ERROR", ex.Message)
            Return JsonConvert.SerializeObject(New With {.error = "Data retrieval failed"})
        End Try
        
        Return json
    End Function
    
    Private Sub InitializeDriverTable(drivertable As DataTable)
        drivertable.Columns.Add(New DataColumn("chk"))
        drivertable.Columns.Add(New DataColumn("sno"))
        drivertable.Columns.Add(New DataColumn("drivername"))
        drivertable.Columns.Add(New DataColumn("Licence No"))
        drivertable.Columns.Add(New DataColumn("Exp Date"))
        drivertable.Columns.Add(New DataColumn("Phone No"))
        drivertable.Columns.Add(New DataColumn("Address"))
        drivertable.Columns.Add(New DataColumn("Fuel Card No"))
        drivertable.Columns.Add(New DataColumn("rfid"))
        drivertable.Columns.Add(New DataColumn("dob"))
        drivertable.Columns.Add(New DataColumn("Issuingdate"))
        drivertable.Columns.Add(New DataColumn("userid"))
        drivertable.Columns.Add(New DataColumn("status"))
        drivertable.Columns.Add(New DataColumn("driveric"))
        drivertable.Columns.Add(New DataColumn("pwd"))
        drivertable.Columns.Add(New DataColumn("driverrole"))
    End Sub
    
    Private Function BuildSecureDriverQuery(ugData As String, role As String, userid As String, userslist As String) As String
        Dim query As String = "SELECT * FROM driver"
        
        If ugData = "SELECT USERNAME" Then
            Return "SELECT TOP 0 * FROM driver" ' Return empty result
        ElseIf ugData = "ALL" Then
            If role = "SuperUser" Or role = "Operator" Then
                query &= " WHERE userid IN (" & userslist & ")"
            ElseIf role = "User" Then
                query &= " WHERE userid = @userid"
            End If
        Else
            query &= " WHERE userid = @ugData"
        End If
        
        query &= " ORDER BY drivername"
        Return query
    End Function
    
    Private Sub PopulateDriverTable(drivertable As DataTable, dr As SqlDataReader)
        Dim j As Integer = 1
        
        While dr.Read()
            Dim r As DataRow = drivertable.NewRow()
            
            ' SECURITY: Sanitize all data before adding to table
            r(0) = G2SecurityHelper.SanitizeForHtml(dr("driverid").ToString())
            r(1) = j.ToString()
            r(2) = G2SecurityHelper.SanitizeForHtml(dr("drivername").ToString().ToUpper())
            r(3) = G2SecurityHelper.SanitizeForHtml(dr("licenceno").ToString().ToUpper())
            
            ' Safe date handling
            r(4) = SafeFormatDate(dr("expirydate"))
            r(5) = G2SecurityHelper.SanitizeForHtml(dr("phoneno").ToString())
            r(6) = G2SecurityHelper.SanitizeForHtml(dr("address").ToString())
            r(7) = G2SecurityHelper.SanitizeForHtml(dr("fuelcardno").ToString())
            r(8) = G2SecurityHelper.SanitizeForHtml(dr("rfid").ToString())
            r(9) = SafeFormatDate(dr("dateofbirth"))
            r(10) = SafeFormatDate(dr("issuingdate"))
            r(11) = G2SecurityHelper.SanitizeForHtml(dr("userid").ToString())
            
            ' Status handling
            r(12) = "1"
            If Not IsDBNull(dr("status")) AndAlso dr("status") = False Then
                r(12) = "0"
            End If
            
            r(13) = If(IsDBNull(dr("driver_ic")), "-", G2SecurityHelper.SanitizeForHtml(dr("driver_ic").ToString()))
            r(14) = If(IsDBNull(dr("password")), "-", "***") ' Don't expose passwords
            r(15) = If(CBool(dr("isowner")), "OWNER", "DRIVER")
            
            drivertable.Rows.Add(r)
            j += 1
        End While
        
        ' Add empty row if no data
        If drivertable.Rows.Count = 0 Then
            Dim r As DataRow = drivertable.NewRow()
            For i As Integer = 0 To 15
                r(i) = "--"
            Next
            drivertable.Rows.Add(r)
        End If
    End Sub
    
    Private Function SafeFormatDate(dateValue As Object) As String
        Try
            If IsDBNull(dateValue) Then
                Return ""
            End If
            
            Dim dateTime As DateTime = Convert.ToDateTime(dateValue)
            If dateTime.Year = 1900 Then
                Return ""
            End If
            
            Return dateTime.ToString("yyyy/MM/dd")
        Catch
            Return ""
        End Try
    End Function
    
    Private Sub StoreExcelData(drivertable As DataTable)
        Try
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable1")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session.Remove("exceltable3")
            
            Dim exceltable As DataTable = drivertable.Copy()
            exceltable.Columns.Remove("userid")
            exceltable.Columns.Remove("chk")
            exceltable.Columns.Remove("sno")
            
            HttpContext.Current.Session("exceltable") = exceltable
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("STORE_EXCEL_DATA_ERROR", ex.Message)
        End Try
    End Sub
    
    Private Function ConvertDriverTableToJson(drivertable As DataTable) As String
        Try
            Dim aa As New ArrayList()
            
            For j1 As Integer = 0 To drivertable.Rows.Count - 1
                Dim a As New ArrayList()
                For col As Integer = 0 To drivertable.Columns.Count - 1
                    a.Add(drivertable.DefaultView.Item(j1)(col))
                Next
                ' Add additional computed fields
                a.Add(drivertable.DefaultView.Item(j1)(0)) ' Duplicate ID for actions
                aa.Add(a)
            Next
            
            Return JsonConvert.SerializeObject(aa, Formatting.None)
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("CONVERT_TO_JSON_ERROR", ex.Message)
            Return JsonConvert.SerializeObject(New ArrayList())
        End Try
    End Function
    
    Public Function SecureInsertUpdateDriver() As String
        Try
            ' SECURITY: Validate all input parameters
            Dim userId As String = ValidateParameter("userId", G2InputType.UserId)
            Dim poiname As String = ValidateParameter("poiname", G2InputType.AlphaNumeric, 100)
            Dim txtDOB As String = ValidateParameter("txtDOB", G2InputType.DateTime)
            Dim txtrfid As String = ValidateParameter("txtrfid", G2InputType.AlphaNumeric, 50)
            Dim txtPhone As String = ValidateParameter("txtPhone", G2InputType.PhoneNumber)
            Dim txtAddress As String = ValidateParameter("txtAddress", G2InputType.AlphaNumeric, 200)
            Dim txtLicenceno As String = ValidateParameter("txtLicenceno", G2InputType.AlphaNumeric, 50)
            Dim txtIssuingdate As String = ValidateParameter("txtIssuingdate", G2InputType.DateTime)
            Dim txtExpiryDate As String = ValidateParameter("txtExpiryDate", G2InputType.DateTime)
            Dim txtFuelCardNo As String = ValidateParameter("txtFuelCardNo", G2InputType.AlphaNumeric, 50)
            Dim poiid As String = ValidateParameter("poiid", G2InputType.UserId)
            Dim opr As String = ValidateParameter("opr", G2InputType.AlphaNumeric, 1)
            Dim ic As String = ValidateParameter("ic", G2InputType.AlphaNumeric, 20)
            Dim pwd As String = ValidateParameter("pwd", G2InputType.AlphaNumeric, 50)
            Dim dvrole As String = ValidateParameter("driverrole", G2InputType.AlphaNumeric, 1)
            
            Return InsertUpdateDriverSecure(userId, poiname, txtDOB, txtrfid, txtPhone, txtAddress, txtLicenceno, txtIssuingdate, txtExpiryDate, txtFuelCardNo, poiid, CInt(opr), ic, pwd, CInt(dvrole))
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("SECURE_INSERT_UPDATE_DRIVER_ERROR", ex.Message)
            Return "0"
        End Try
    End Function
    
    Private Function ValidateParameter(paramName As String, inputType As G2InputType, Optional maxLength As Integer = 255) As String
        Dim value As String = Request.QueryString(paramName)
        
        If String.IsNullOrEmpty(value) Then
            Return ""
        End If
        
        If Not G2SecurityHelper.ValidateG2Input(value, inputType, maxLength) Then
            Throw New ArgumentException($"Invalid {paramName} parameter")
        End If
        
        Return G2SecurityHelper.SanitizeForHtml(value)
    End Function
    
    Public Function InsertUpdateDriverSecure(userId As String, poiname As String, txtDOB As String, txtrfid As String, txtPhone As String, txtAddress As String, txtLicenceno As String, txtIssuingdate As String, txtExpiryDate As String, txtFuelCardNo As String, poiid As String, opr As Integer, ic As String, pwd As String, driverrole As Integer) As String
        Dim result As Integer = 0
        
        Try
            ' SECURITY: Normalize date inputs
            If String.IsNullOrEmpty(txtDOB) Then txtDOB = "1900/01/01"
            If String.IsNullOrEmpty(txtIssuingdate) Then txtIssuingdate = "1900/01/01"
            If String.IsNullOrEmpty(txtExpiryDate) Then txtExpiryDate = "1900/01/01"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                If opr = 0 Then
                    ' Insert operation
                    result = PerformInsertDriver(conn, userId, poiname, txtDOB, txtrfid, txtPhone, txtAddress, txtLicenceno, txtIssuingdate, txtExpiryDate, txtFuelCardNo, ic, pwd, driverrole)
                Else
                    ' Update operation
                    result = PerformUpdateDriver(conn, userId, poiname, txtDOB, txtrfid, txtPhone, txtAddress, txtLicenceno, txtIssuingdate, txtExpiryDate, txtFuelCardNo, poiid, ic, pwd, driverrole)
                End If
            End Using
            
            G2SecurityHelper.LogSecurityEvent("DRIVER_OPERATION", $"Operation: {If(opr = 0, "INSERT", "UPDATE")}, Result: {result}")
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("INSERT_UPDATE_DRIVER_ERROR", ex.Message)
            Return "0"
        End Try
        
        Return result.ToString()
    End Function
    
    Private Function PerformInsertDriver(conn As SqlConnection, userId As String, poiname As String, txtDOB As String, txtrfid As String, txtPhone As String, txtAddress As String, txtLicenceno As String, txtIssuingdate As String, txtExpiryDate As String, txtFuelCardNo As String, ic As String, pwd As String, driverrole As Integer) As Integer
        ' SECURITY: Check for duplicate phone number first
        Dim checkQuery As String = "SELECT COUNT(*) FROM driver WHERE phoneno = @phoneno"
        Using checkCmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(checkQuery, conn)
            checkCmd.Parameters.AddWithValue("@phoneno", txtPhone)
            
            conn.Open()
            Dim count As Integer = CInt(checkCmd.ExecuteScalar())
            If count > 0 Then
                Return 99 ' Duplicate phone number
            End If
        End Using
        
        ' SECURITY: Insert with parameterized query
        Dim insertQuery As String = "INSERT INTO driver(rfid, userid, drivername, dateofbirth, phoneno, address, licenceno, issuingdate, expirydate, fuelcardno, driver_ic, password, isowner) VALUES (@rfid, @userid, @drivername, @dateofbirth, @phoneno, @address, @licenceno, @issuingdate, @expirydate, @fuelcardno, @driver_ic, @password, @isowner)"
        
        Using insertCmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(insertQuery, conn)
            insertCmd.Parameters.AddWithValue("@rfid", txtrfid)
            insertCmd.Parameters.AddWithValue("@userid", userId)
            insertCmd.Parameters.AddWithValue("@drivername", poiname)
            insertCmd.Parameters.AddWithValue("@dateofbirth", txtDOB)
            insertCmd.Parameters.AddWithValue("@phoneno", txtPhone)
            insertCmd.Parameters.AddWithValue("@address", txtAddress)
            insertCmd.Parameters.AddWithValue("@licenceno", txtLicenceno)
            insertCmd.Parameters.AddWithValue("@issuingdate", txtIssuingdate)
            insertCmd.Parameters.AddWithValue("@expirydate", txtExpiryDate)
            insertCmd.Parameters.AddWithValue("@fuelcardno", txtFuelCardNo)
            insertCmd.Parameters.AddWithValue("@driver_ic", ic)
            insertCmd.Parameters.AddWithValue("@password", pwd)
            insertCmd.Parameters.AddWithValue("@isowner", driverrole)
            
            Return insertCmd.ExecuteNonQuery()
        End Using
    End Function
    
    Private Function PerformUpdateDriver(conn As SqlConnection, userId As String, poiname As String, txtDOB As String, txtrfid As String, txtPhone As String, txtAddress As String, txtLicenceno As String, txtIssuingdate As String, txtExpiryDate As String, txtFuelCardNo As String, poiid As String, ic As String, pwd As String, driverrole As Integer) As Integer
        ' SECURITY: Update with parameterized query
        Dim updateQuery As String = "UPDATE driver SET userid = @userid, drivername = @drivername, rfid = @rfid, dateofbirth = @dateofbirth, phoneno = @phoneno, address = @address, licenceno = @licenceno, issuingdate = @issuingdate, expirydate = @expirydate, fuelcardno = @fuelcardno, password = @password, driver_ic = @driver_ic, isowner = @isowner WHERE driverid = @driverid"
        
        Using updateCmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(updateQuery, conn)
            updateCmd.Parameters.AddWithValue("@userid", userId)
            updateCmd.Parameters.AddWithValue("@drivername", poiname)
            updateCmd.Parameters.AddWithValue("@rfid", txtrfid)
            updateCmd.Parameters.AddWithValue("@dateofbirth", Convert.ToDateTime(txtDOB).ToString("yyyy/MM/dd"))
            updateCmd.Parameters.AddWithValue("@phoneno", txtPhone)
            updateCmd.Parameters.AddWithValue("@address", txtAddress)
            updateCmd.Parameters.AddWithValue("@licenceno", txtLicenceno)
            updateCmd.Parameters.AddWithValue("@issuingdate", Convert.ToDateTime(txtIssuingdate).ToString("yyyy/MM/dd"))
            updateCmd.Parameters.AddWithValue("@expirydate", Convert.ToDateTime(txtExpiryDate).ToString("yyyy/MM/dd"))
            updateCmd.Parameters.AddWithValue("@fuelcardno", txtFuelCardNo)
            updateCmd.Parameters.AddWithValue("@password", pwd)
            updateCmd.Parameters.AddWithValue("@driver_ic", ic)
            updateCmd.Parameters.AddWithValue("@isowner", driverrole)
            updateCmd.Parameters.AddWithValue("@driverid", poiid)
            
            conn.Open()
            Return updateCmd.ExecuteNonQuery()
        End Using
    End Function
    
    Public Function SecureActivate() As String
        Return PerformStatusUpdate(True)
    End Function
    
    Public Function SecureInActivate() As String
        Return PerformStatusUpdate(False)
    End Function
    
    Private Function PerformStatusUpdate(status As Boolean) As String
        Try
            Dim chekitemsParam As String = Request.QueryString("chekitems")
            If String.IsNullOrEmpty(chekitemsParam) Then
                Return "0"
            End If
            
            ' SECURITY: Validate and sanitize driver IDs
            Dim chekitems() As String = chekitemsParam.Split(","c)
            Dim validIds As New List(Of String)
            
            For Each item As String In chekitems
                If G2SecurityHelper.ValidateG2Input(item.Trim(), G2InputType.UserId) Then
                    validIds.Add(item.Trim())
                End If
            Next
            
            If validIds.Count = 0 Then
                Return "0"
            End If
            
            Dim result As Integer = 0
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY: Use parameterized query with IN clause
                Dim placeholders As String = String.Join(",", validIds.Select(Function(id, index) "@id" & index))
                Dim query As String = $"UPDATE driver SET status = @status WHERE driverid IN ({placeholders})"
                
                Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, conn)
                    cmd.Parameters.AddWithValue("@status", status)
                    
                    For i As Integer = 0 To validIds.Count - 1
                        cmd.Parameters.AddWithValue("@id" & i, validIds(i))
                    Next
                    
                    conn.Open()
                    result = cmd.ExecuteNonQuery()
                End Using
            End Using
            
            G2SecurityHelper.LogSecurityEvent("DRIVER_STATUS_UPDATE", $"Status: {status}, Count: {validIds.Count}, Result: {result}")
            Return result.ToString()
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("STATUS_UPDATE_ERROR", ex.Message)
            Return "0"
        End Try
    End Function
    
End Class