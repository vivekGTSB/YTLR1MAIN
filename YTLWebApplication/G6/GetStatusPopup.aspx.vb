Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.Script.Services

Public Class GetStatusPopup
    Inherits System.Web.UI.Page
    
    Public sb1 As New StringBuilder()
    Public loggedinUID As String = ""
    Public plateno As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Authentication check
            If Not IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            ' SECURITY FIX: Add security headers
            AddSecurityHeaders()

            ' SECURITY FIX: Get user ID from session
            loggedinUID = SecurityHelper.ValidateAndGetUserId(Request)
            
            ' SECURITY FIX: Validate plate number parameter
            Dim plateParam As String = Request.QueryString("p")
            If String.IsNullOrEmpty(plateParam) OrElse Not SecurityHelper.ValidatePlateNumber(plateParam) Then
                Response.Redirect("~/Error.aspx")
                Return
            End If
            
            plateno = plateParam
            FillGrid()

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("STATUS_POPUP_ERROR", ex.Message)
            Response.Redirect("~/Error.aspx")
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
    End Sub

    Public Sub FillGrid()
        Try
            Dim t As New DataTable()
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("Date Time"))
            t.Columns.Add(New DataColumn("Plateno"))
            t.Columns.Add(New DataColumn("Status"))
            t.Columns.Add(New DataColumn("Remarks"))
            t.Columns.Add(New DataColumn("Added By"))
            t.Columns.Add(New DataColumn("Edit"))

            ' SECURITY FIX: Use parameterized query
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@plateno", plateno},
                {"@minDate", New DateTime(2019, 9, 1)}
            }

            Dim query As String = "SELECT plateno, timestamp, statusdate, status, officeremark, sourcename FROM maintenance WHERE timestamp > @minDate AND plateno = @plateno ORDER BY timestamp DESC"
            
            Dim dataTable As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            Dim c As Integer = 1
            For Each row As DataRow In dataTable.Rows
                Try
                    Dim r As DataRow = t.NewRow()
                    r(0) = c
                    r(1) = DateTime.Parse(row("timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                    r(2) = SecurityHelper.HtmlEncode(row("plateno").ToString())
                    r(3) = SecurityHelper.HtmlEncode(row("status").ToString())
                    
                    Dim remarks As String = If(IsDBNull(row("officeremark")), "", SecurityHelper.HtmlEncode(row("officeremark").ToString()))
                    r(4) = $"<span class='clsremarks'>{remarks}</span>"
                    
                    Dim sourceName As String = If(IsDBNull(row("sourcename")), "", SecurityHelper.HtmlEncode(row("sourcename").ToString().ToUpper()))
                    r(5) = sourceName
                    
                    ' SECURITY FIX: Encode data attributes
                    Dim encodedPlateno As String = SecurityHelper.HtmlEncode(row("plateno").ToString())
                    Dim encodedStatusDate As String = SecurityHelper.HtmlEncode(row("statusdate").ToString())
                    
                    r(6) = $"<span class='btnedit' data-plateno='{encodedPlateno}' data-statusdate='{encodedStatusDate}'>Edit</span>" &
                           $"<span class='btnupdate hidenow' data-plateno='{encodedPlateno}' data-statusdate='{encodedStatusDate}'>Update</span>" &
                           $"<span class='btncancel hidenow' data-plateno='{encodedPlateno}' data-statusdate='{encodedStatusDate}'>Cancel</span>"
                    
                    t.Rows.Add(r)
                    c += 1
                Catch ex As Exception
                    SecurityHelper.LogSecurityEvent("FILL_GRID_ROW_ERROR", ex.Message)
                End Try
            Next

            If t.Rows.Count = 0 Then
                Dim r As DataRow = t.NewRow()
                For i As Integer = 0 To 6
                    r(i) = "--"
                Next
                t.Rows.Add(r)
            End If

            ' Build HTML table
            sb1.Append("<thead><tr align=""left""><th>S No</th><th>Date Time</th><th>Plate No</th><th>Status</th><th>Remarks</th><th>Added By</th><th>Edit</th></tr></thead>")
            sb1.Append("<tbody>")
            
            For i As Integer = 0 To t.Rows.Count - 1
                Try
                    sb1.Append("<tr>")
                    For j As Integer = 0 To 6
                        sb1.Append("<td>")
                        sb1.Append(t.Rows(i)(j).ToString())
                        sb1.Append("</td>")
                    Next
                    sb1.Append("</tr>")
                Catch ex As Exception
                    SecurityHelper.LogSecurityEvent("BUILD_HTML_ERROR", ex.Message)
                End Try
            Next
            
            sb1.Append("</tbody>")
            sb1.Append("<tfoot><tr><th>S No</th><th>Date Time</th><th>Plate No</th><th>Status</th><th>Remarks</th><th>Added By</th><th>Edit</th></tr></tfoot>")

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("FILL_GRID_ERROR", ex.Message)
            sb1.Append("<tbody><tr><td colspan='7'>Error loading data</td></tr></tbody>")
        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function saveData(ByVal plateno As String, ByVal sd As String, ByVal remarks As String) As String
        Dim res As String = ""
        Try
            ' SECURITY FIX: Validate input parameters
            If Not SecurityHelper.ValidatePlateNumber(plateno) Then
                Return "Invalid plate number"
            End If

            If Not SecurityHelper.ValidateDate(sd) Then
                Return "Invalid date"
            End If

            If String.IsNullOrEmpty(remarks) OrElse remarks.Length > 500 Then
                Return "Invalid remarks"
            End If

            If SecurityHelper.ContainsDangerousPatterns(remarks) Then
                Return "Invalid characters in remarks"
            End If

            ' SECURITY FIX: Get username from session
            Dim username As String = ""
            If HttpContext.Current.Session("username") IsNot Nothing Then
                username = HttpContext.Current.Session("username").ToString()
            Else
                Return "Session expired"
            End If

            ' SECURITY FIX: Use parameterized query
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@remarks", remarks},
                {"@username", username},
                {"@plateno", plateno},
                {"@statusdate", DateTime.Parse(sd).ToString("yyyy-MM-dd HH:mm:ss")}
            }

            Dim query As String = "UPDATE maintenance SET officeremark = @remarks, sourcename = @username WHERE plateno = @plateno AND statusdate = @statusdate"
            
            Dim result As Integer = SecurityHelper.ExecuteSecureNonQuery(query, parameters)
            
            If result > 0 Then
                SecurityHelper.LogSecurityEvent("MAINTENANCE_UPDATE", $"Plate: {plateno}, Date: {sd}")
                res = "Success"
            Else
                res = "No records updated"
            End If

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("SAVE_DATA_ERROR", ex.Message)
            res = "Error updating data"
        End Try

        Return res
    End Function

End Class