Imports System.Data
Imports System.Text
Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetSMSNotificationData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
            If Not SecurityHelper.CheckRateLimit("GetSMSNotificationData_" & GetClientIP(), 50, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.Write("Rate limit exceeded")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate operation parameter
            Dim oper As String = Request.QueryString("opr")
            If String.IsNullOrEmpty(oper) OrElse Not SecurityHelper.ValidateInput(oper, "numeric") Then
                Response.StatusCode = 400
                Response.Write("Invalid operation parameter")
                Response.End()
                Return
            End If

            Select Case oper
                Case "0"
                    GetData()
                Case "1"
                    InsertData()
                Case "2"
                    UpdateData()
                Case "3"
                    DeleteData()
                Case "4"
                    FillData()
                Case Else
                    Response.StatusCode = 400
                    Response.Write("Invalid operation")
            End Select

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("SMS_NOTIFICATION_ERROR", ex.Message)
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

    Private Sub FillData()
        Try
            Dim aa As New ArrayList()
            
            ' SECURITY FIX: Use parameterized query
            Dim parameters As New Dictionary(Of String, Object)()
            Dim query As String = "SELECT geofencename, shiptocode FROM geofence WHERE shiptocode <> '0' AND shiptocode NOT IN (SELECT shiptocode FROM oss_notification) ORDER BY geofencename"
            
            Dim dataTable As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            For Each row As DataRow In dataTable.Rows
                Dim a As New ArrayList()
                a.Add(SecurityHelper.HtmlEncode(row("shiptocode").ToString()) & " - " & SecurityHelper.HtmlEncode(row("geofencename").ToString().ToUpper()))
                a.Add(SecurityHelper.HtmlEncode(row("shiptocode").ToString()))
                aa.Add(a)
            Next

            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("FILL_DATA_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Error retrieving data")
        End Try
    End Sub

    Private Sub GetData()
        Try
            ' SECURITY FIX: Get user data from session
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Dim aa As New ArrayList()
            Dim poistable As New DataTable()

            poistable.Columns.Add(New DataColumn("S No"))
            poistable.Columns.Add(New DataColumn("Ship To Code"))
            poistable.Columns.Add(New DataColumn("Name"))
            poistable.Columns.Add(New DataColumn("Mobile List"))
            poistable.Columns.Add(New DataColumn("OTP"))

            ' SECURITY FIX: Use parameterized query
            Dim query As String = "SELECT oss.OtpFlag, g.geofencename, oss.shiptocode, oss.mobileno FROM oss_notification AS oss JOIN geofence g ON g.shiptocode = oss.shiptocode"
            Dim parameters As New Dictionary(Of String, Object)()
            
            Dim dataTable As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            Dim i As Integer = 0
            For Each row As DataRow In dataTable.Rows
                Dim r As DataRow = poistable.NewRow()
                Dim a As New ArrayList()
                
                a.Add(SecurityHelper.HtmlEncode(row("shiptocode").ToString()))
                r(0) = i.ToString()
                a.Add(i)
                a.Add(SecurityHelper.HtmlEncode(row("shiptocode").ToString()))
                r(1) = SecurityHelper.HtmlEncode(row("shiptocode").ToString())
                a.Add(SecurityHelper.HtmlEncode(row("geofencename").ToString()))
                r(2) = SecurityHelper.HtmlEncode(row("geofencename").ToString())
                
                Dim mobileNo As String = SecurityHelper.HtmlEncode(row("mobileno").ToString())
                a.Add(mobileNo)
                If mobileNo.Contains(",") Then
                    r(3) = mobileNo.Replace(",", ";")
                Else
                    r(3) = mobileNo
                End If

                If CBool(row("OtpFlag")) Then
                    a.Add("Enabled")
                    a.Add("1")
                    r(4) = "Enabled"
                Else
                    a.Add("Disabled")
                    a.Add("0")
                    r(4) = "Disabled"
                End If
                
                poistable.Rows.Add(r)
                aa.Add(a)
                i += 1
            Next

            If poistable.Rows.Count = 0 Then
                Dim r As DataRow = poistable.NewRow()
                r(0) = "1"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                poistable.Rows.Add(r)
            End If

            Session.Remove("exceltable")
            Session("exceltable") = poistable

            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GET_DATA_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Error retrieving data")
        End Try
    End Sub

    Private Sub InsertData()
        Try
            ' SECURITY FIX: Validate all input parameters
            Dim mobile1 As String = ValidateMobileNumber(Request.QueryString("mobile1"))
            Dim mobile2 As String = ValidateMobileNumber(Request.QueryString("mobile2"))
            Dim mobile3 As String = ValidateMobileNumber(Request.QueryString("mobile3"))
            Dim mobile4 As String = ValidateMobileNumber(Request.QueryString("mobile4"))
            Dim mobile5 As String = ValidateMobileNumber(Request.QueryString("mobile5"))
            Dim chkotp As String = Request.QueryString("chkotp")
            Dim scode As String = Request.QueryString("scode")

            ' SECURITY FIX: Validate ship to code
            If Not SecurityHelper.ValidateInput(scode, "shiptocode") Then
                Response.StatusCode = 400
                Response.Write("Invalid ship to code")
                Return
            End If

            ' SECURITY FIX: Validate OTP flag
            If chkotp <> "0" AndAlso chkotp <> "1" Then
                Response.StatusCode = 400
                Response.Write("Invalid OTP flag")
                Return
            End If

            ' Build mobile numbers string
            Dim mobileNumbers As New List(Of String)()
            If Not String.IsNullOrEmpty(mobile1) Then mobileNumbers.Add(mobile1)
            If Not String.IsNullOrEmpty(mobile2) Then mobileNumbers.Add(mobile2)
            If Not String.IsNullOrEmpty(mobile3) Then mobileNumbers.Add(mobile3)
            If Not String.IsNullOrEmpty(mobile4) Then mobileNumbers.Add(mobile4)
            If Not String.IsNullOrEmpty(mobile5) Then mobileNumbers.Add(mobile5)

            Dim strmobile As String = String.Join(",", mobileNumbers)

            ' SECURITY FIX: Use parameterized query
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@otpflag", chkotp},
                {"@shiptocode", scode},
                {"@mobileno", strmobile},
                {"@datetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            }

            Dim query As String = "INSERT INTO oss_notification(otpflag, shiptocode, mobileno, insert_datetime, update_datetime) VALUES (@otpflag, @shiptocode, @mobileno, @datetime, @datetime)"
            
            Dim result As Integer = SecurityHelper.ExecuteSecureNonQuery(query, parameters)
            
            SecurityHelper.LogSecurityEvent("SMS_NOTIFICATION_INSERT", $"Ship to code: {scode}")
            
            Response.ContentType = "application/json"
            Response.Write(result.ToString())

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("INSERT_DATA_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Error inserting data")
        End Try
    End Sub

    Private Sub UpdateData()
        Try
            ' SECURITY FIX: Validate all input parameters
            Dim mobile1 As String = ValidateMobileNumber(Request.QueryString("mobile1"))
            Dim mobile2 As String = ValidateMobileNumber(Request.QueryString("mobile2"))
            Dim mobile3 As String = ValidateMobileNumber(Request.QueryString("mobile3"))
            Dim mobile4 As String = ValidateMobileNumber(Request.QueryString("mobile4"))
            Dim mobile5 As String = ValidateMobileNumber(Request.QueryString("mobile5"))
            Dim chkotp As String = Request.QueryString("chkotp")
            Dim scode As String = Request.QueryString("scode")

            ' SECURITY FIX: Validate ship to code
            If Not SecurityHelper.ValidateInput(scode, "shiptocode") Then
                Response.StatusCode = 400
                Response.Write("Invalid ship to code")
                Return
            End If

            ' SECURITY FIX: Validate OTP flag
            If chkotp <> "0" AndAlso chkotp <> "1" Then
                Response.StatusCode = 400
                Response.Write("Invalid OTP flag")
                Return
            End If

            ' Build mobile numbers string
            Dim mobileNumbers As New List(Of String)()
            If Not String.IsNullOrEmpty(mobile1) Then mobileNumbers.Add(mobile1)
            If Not String.IsNullOrEmpty(mobile2) Then mobileNumbers.Add(mobile2)
            If Not String.IsNullOrEmpty(mobile3) Then mobileNumbers.Add(mobile3)
            If Not String.IsNullOrEmpty(mobile4) Then mobileNumbers.Add(mobile4)
            If Not String.IsNullOrEmpty(mobile5) Then mobileNumbers.Add(mobile5)

            Dim strmobile As String = String.Join(",", mobileNumbers)

            ' SECURITY FIX: Use parameterized query
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@otpflag", chkotp},
                {"@mobileno", strmobile},
                {"@datetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                {"@shiptocode", scode}
            }

            Dim query As String = "UPDATE oss_notification SET OTpFlag = @otpflag, mobileno = @mobileno, update_datetime = @datetime WHERE shiptocode = @shiptocode"
            
            Dim result As Integer = SecurityHelper.ExecuteSecureNonQuery(query, parameters)
            
            SecurityHelper.LogSecurityEvent("SMS_NOTIFICATION_UPDATE", $"Ship to code: {scode}")
            
            Response.ContentType = "application/json"
            Response.Write(result.ToString())

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("UPDATE_DATA_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Error updating data")
        End Try
    End Sub

    Private Sub DeleteData()
        Try
            ' SECURITY FIX: Validate input parameter
            Dim chekitems As String = Request.QueryString("geoid")
            If String.IsNullOrEmpty(chekitems) Then
                Response.StatusCode = 400
                Response.Write("Missing geoid parameter")
                Return
            End If

            ' SECURITY FIX: Clean and validate the input
            chekitems = chekitems.Replace("[", "").Replace("]", "").Replace("""", "")
            Dim ids As String() = chekitems.Split(","c)

            Dim result As Integer = 0
            Dim deletedCount As Integer = 0

            For Each id As String In ids
                Dim cleanId As String = id.Trim().Replace("""", "")
                
                ' SECURITY FIX: Validate each ID
                If SecurityHelper.ValidateInput(cleanId, "shiptocode") Then
                    Try
                        Dim parameters As New Dictionary(Of String, Object) From {
                            {"@shiptocode", cleanId}
                        }
                        
                        Dim query As String = "DELETE FROM oss_notification WHERE shiptocode = @shiptocode"
                        Dim deleteResult As Integer = SecurityHelper.ExecuteSecureNonQuery(query, parameters)
                        
                        If deleteResult > 0 Then
                            deletedCount += 1
                            result = 1
                        End If
                        
                    Catch ex As Exception
                        SecurityHelper.LogSecurityEvent("DELETE_ITEM_ERROR", $"ID: {cleanId}, Error: {ex.Message}")
                    End Try
                End If
            Next

            SecurityHelper.LogSecurityEvent("SMS_NOTIFICATION_DELETE", $"Deleted {deletedCount} items")
            
            Response.ContentType = "application/json"
            Response.Write(result.ToString())

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("DELETE_DATA_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Error deleting data")
        End Try
    End Sub

    ' SECURITY FIX: Validate mobile number format
    Private Function ValidateMobileNumber(mobileNumber As String) As String
        If String.IsNullOrEmpty(mobileNumber) Then
            Return String.Empty
        End If

        If SecurityHelper.ValidateInput(mobileNumber, "mobile") Then
            Return mobileNumber.Trim()
        Else
            Throw New ArgumentException($"Invalid mobile number format: {mobileNumber}")
        End If
    End Function

End Class