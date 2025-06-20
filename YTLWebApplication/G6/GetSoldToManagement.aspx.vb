Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class GetSoldToManagement
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
            If Not SecurityHelper.CheckRateLimit("GetSoldToManagement_" & GetClientIP(), 50, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.Write("Rate limit exceeded")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate operation parameter
            Dim op As String = Request.QueryString("op")
            If String.IsNullOrEmpty(op) OrElse Not SecurityHelper.ValidateInput(op, "numeric") Then
                Response.StatusCode = 400
                Response.Write("Invalid operation parameter")
                Response.End()
                Return
            End If

            Select Case op
                Case "0"
                    Response.Write(FillGrid())
                Case "1"
                    Dim pod As String = Request.QueryString("prevpid")
                    Dim cid As String = Request.QueryString("pid")
                    Dim cname As String = Request.QueryString("pname")
                    Response.Write(AddUpdateCustomer(pod, cid, cname))
                Case Else
                    Response.StatusCode = 400
                    Response.Write("Invalid operation")
            End Select

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("SOLD_TO_MANAGEMENT_ERROR", ex.Message)
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

    Public Function FillGrid() As String
        Dim json As String = Nothing
        Try
            Dim aa As New ArrayList()
            Dim soldtotable As New DataTable()

            soldtotable.Columns.Add(New DataColumn("No"))
            soldtotable.Columns.Add(New DataColumn("Customer ID"))
            soldtotable.Columns.Add(New DataColumn("Customer Name"))
            soldtotable.Columns.Add(New DataColumn("Timestamp"))
            soldtotable.Columns.Add(New DataColumn("id"))

            ' SECURITY FIX: Use parameterized query
            Dim query As String = "SELECT id, customerid, customername, timestamp FROM EC_soldto ORDER BY customername"
            Dim parameters As New Dictionary(Of String, Object)()
            
            Dim dataTable As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            Dim i As Integer = 1
            For Each row As DataRow In dataTable.Rows
                Dim r As DataRow = soldtotable.NewRow()
                r(0) = i
                r(1) = SecurityHelper.HtmlEncode(row("customerid").ToString())
                r(2) = SecurityHelper.HtmlEncode(row("customername").ToString().ToUpper())
                r(3) = DateTime.Parse(row("timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                r(4) = row("id")
                i += 1
                soldtotable.Rows.Add(r)
            Next

            If soldtotable.Rows.Count = 0 Then
                Dim r As DataRow = soldtotable.NewRow()
                r(0) = "-"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                soldtotable.Rows.Add(r)
            End If

            For i = 0 To soldtotable.Rows.Count - 1
                Dim a As New ArrayList()
                a.Add(soldtotable.Rows(i)(0))
                a.Add(soldtotable.Rows(i)(2))
                a.Add(soldtotable.Rows(i)(1))
                a.Add(soldtotable.Rows(i)(3))
                a.Add(soldtotable.Rows(i)(4))
                aa.Add(a)
            Next

            json = JsonConvert.SerializeObject(aa, Formatting.None)

            ' Store for Excel export
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session.Remove("exceltable3")
            soldtotable.Columns.RemoveAt(4)
            HttpContext.Current.Session("exceltable") = soldtotable
            HttpContext.Current.Session.Remove("tempTable")

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("FILL_GRID_ERROR", ex.Message)
            json = "{""error"":""Error retrieving data""}"
        End Try

        Return json
    End Function

    Public Function AddUpdateCustomer(ByVal pid As String, ByVal cid As String, ByVal cname As String) As String
        Dim json As String = Nothing
        Try
            ' SECURITY FIX: Validate input parameters
            If Not ValidateCustomerInput(pid, cid, cname) Then
                Return "{""error"":""Invalid input parameters""}"
            End If

            Dim parameters As New Dictionary(Of String, Object)()
            Dim query As String

            If pid = "0" Then
                ' Insert new customer
                query = "INSERT INTO EC_soldto (customerid, customername, timestamp) VALUES (@cid, @cname, @timestamp)"
                parameters.Add("@cid", cid)
                parameters.Add("@cname", cname)
                parameters.Add("@timestamp", DateTime.Now)
                
                SecurityHelper.LogSecurityEvent("CUSTOMER_INSERT", $"Customer ID: {cid}, Name: {cname}")
            Else
                ' Update existing customer
                query = "UPDATE EC_soldto SET customerid = @cid, customername = @cname, timestamp = @timestamp WHERE id = @pid"
                parameters.Add("@cid", cid)
                parameters.Add("@cname", cname)
                parameters.Add("@timestamp", DateTime.Now)
                parameters.Add("@pid", pid)
                
                SecurityHelper.LogSecurityEvent("CUSTOMER_UPDATE", $"ID: {pid}, Customer ID: {cid}, Name: {cname}")
            End If

            Dim result As Integer = SecurityHelper.ExecuteSecureNonQuery(query, parameters)
            
            json = If(result > 0, "1", "0")

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("ADD_UPDATE_CUSTOMER_ERROR", ex.Message)
            json = "0"
        End Try

        Return json
    End Function

    ' SECURITY FIX: Validate customer input
    Private Function ValidateCustomerInput(pid As String, cid As String, cname As String) As Boolean
        Try
            ' Validate previous ID (for updates)
            If Not String.IsNullOrEmpty(pid) AndAlso pid <> "0" Then
                If Not SecurityHelper.ValidateInput(pid, "numeric") Then
                    Return False
                End If
            End If

            ' Validate customer ID
            If String.IsNullOrEmpty(cid) OrElse cid.Length > 50 Then
                Return False
            End If

            If SecurityHelper.ContainsDangerousPatterns(cid) Then
                Return False
            End If

            ' Validate customer name
            If String.IsNullOrEmpty(cname) OrElse cname.Length > 100 Then
                Return False
            End If

            If SecurityHelper.ContainsDangerousPatterns(cname) Then
                Return False
            End If

            Return True

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("VALIDATE_CUSTOMER_INPUT_ERROR", ex.Message)
            Return False
        End Try
    End Function

End Class