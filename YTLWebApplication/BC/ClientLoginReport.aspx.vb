Imports System.Data.SqlClient
Imports System.Data
Imports System.Collections.Generic

Partial Class ClientLoginReport
    Inherits SecureBasePage
    
    Public show As Boolean = False
    Public addShiptocode As String = "AddShipToCode.aspx"
    Public divgrid As Boolean = False
    Public ec As String = "false"
    Public sb1 As New StringBuilder()
    Public strBeginDate As String = ""
    Public strEndDate As String = ""
    
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            If Page.IsPostBack = False Then
                InitializePage()
                LoadUserDropdown()
            End If
            
            ' Set default dates
            strBeginDate = Now().ToString("yyyy/MM/dd")
            strEndDate = Now().ToString("yyyy/MM/dd")
            
        Catch ex As Exception
            LogError(ex)
            sb1.Append("<tr><td colspan='6'>An error occurred while loading the page</td></tr>")
        End Try
    End Sub
    
    Private Sub InitializePage()
        Try
            txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
            txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            
        Catch ex As Exception
            LogError(ex)
        End Try
    End Sub
    
    Private Sub LoadUserDropdown()
        Try
            Dim userid As String = CurrentUser.UserId
            Dim role As String = CurrentUser.Role
            Dim userslist As String = GetUsersList(CurrentUser.UserId)

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand
                
                ' Use parameterized queries based on user role
                If role = "User" Then
                    cmd = New SqlCommand("SELECT CUserName, CUserId FROM EC_client_user WHERE CUserId = @userid AND status = 1 ORDER BY CUserName", conn)
                    cmd.Parameters.Add("@userid", SqlDbType.VarChar, 50).Value = userid
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("SELECT CUserName, CUserId FROM EC_client_user WHERE CUserId IN (SELECT value FROM STRING_SPLIT(@userslist, ',')) AND status = 1 ORDER BY CUserName", conn)
                    cmd.Parameters.Add("@userslist", SqlDbType.VarChar, 1000).Value = userslist
                Else
                    cmd = New SqlCommand("SELECT CUserName, CUserId FROM EC_client_user WHERE status = 1 ORDER BY CUserName", conn)
                End If

                conn.Open()
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    ddlUsername.Items.Clear()
                    ddlUsername.Items.Add(New ListItem("ALL", "0"))
                    
                    While dr.Read()
                        Dim userName As String = SafeGetString(dr, "CUserName")
                        userid = SafeGetString(dr, "CUserId")
                        ddlUsername.Items.Add(New ListItem(userName, userId))
                    End While
                End Using
            End Using
            
        Catch ex As Exception
            LogError(ex)
            ddlUsername.Items.Clear()
            ddlUsername.Items.Add(New ListItem("Error loading users", "0"))
        End Try
    End Sub
    
    Public Sub FillGrid()
        Try
            Dim userstable As New DataTable
            Dim ok As String = "no"
            Dim condition As String = ""

            ' Validate and sanitize date inputs
            Dim beginDate As String = ValidateInput(txtBeginDate.Value, "date")
            Dim endDate As String = ValidateInput(txtEndDate.Value, "date")
            Dim beginHour As String = ValidateInput(ddlbh.SelectedValue, "hour")
            Dim beginMin As String = ValidateInput(ddlbm.SelectedValue, "minute")
            Dim endHour As String = ValidateInput(ddleh.SelectedValue, "hour")
            Dim endMin As String = ValidateInput(ddlem.SelectedValue, "minute")

            Dim strBeginDateTime As String = $"{beginDate} {beginHour}:{beginMin}:00"
            Dim strEndDateTime As String = $"{endDate} {endHour}:{endMin}:59"

            ' Validate date range
            If Not IsValidDateRange(strBeginDateTime, strEndDateTime) Then
                Throw New ArgumentException("Invalid date range selected")
            End If

            ' Initialize data table
            InitializeDataTable(userstable)

            ' Build condition based on user selection
            If ddlUsername.SelectedValue <> "0" Then
                condition = " AND userid = @selectedUserId"
            End If

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' Use parameterized query to prevent SQL injection
                Dim sql As String = "SELECT t1.userid, t2.CUserName, t1.logintime, browser, applicationversion, ISNULL(t1.mobile, 0) as mobile " &
                                   "FROM EC_user_log t1 " &
                                   "INNER JOIN EC_client_user t2 ON t1.userid = t2.CUserId " &
                                   "WHERE logintime BETWEEN @beginDateTime AND @endDateTime" & condition &
                                   " ORDER BY logintime"

                Using cmd As New SqlCommand(sql, conn)
                    ' Add parameters to prevent SQL injection
                    cmd.Parameters.Add("@beginDateTime", SqlDbType.DateTime).Value = Convert.ToDateTime(strBeginDateTime)
                    cmd.Parameters.Add("@endDateTime", SqlDbType.DateTime).Value = Convert.ToDateTime(strEndDateTime)
                    
                    If ddlUsername.SelectedValue <> "0" Then
                        cmd.Parameters.Add("@selectedUserId", SqlDbType.VarChar, 50).Value = ddlUsername.SelectedValue
                    End If

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim i As Integer = 1
                        
                        While dr.Read()
                            Dim r As DataRow = userstable.NewRow()
                            r(0) = i.ToString()
                            r(1) = SafeGetString(dr, "CUserName")
                            r(2) = SafeGetDateTime(dr, "logintime").ToString("yyyy/MM/dd HH:mm:ss")
                            r(3) = SafeGetString(dr, "browser")
                            r(4) = SafeGetString(dr, "applicationversion")
                            
                            ' Handle mobile platform
                            Select Case SafeGetString(dr, "mobile")
                                Case "1"
                                    r(5) = "Mobile"
                                Case "0"
                                    r(5) = "Desktop"
                                Case Else
                                    r(5) = "-"
                            End Select
                            
                            userstable.Rows.Add(r)
                            i += 1
                            ok = "yes"
                        End While
                    End Using
                End Using
            End Using

            ' Add empty row if no data found
            If ok = "no" Then
                AddEmptyRow(userstable)
            End If

            ' Store data for Excel export
            Session.Remove("exceltable")
            Session("exceltable") = userstable
            ec = "true"
            
            ' Build HTML output
            BuildGridHtml(userstable)
            
        Catch ex As Exception
            LogError(ex)
            sb1.Clear()
            sb1.Append("<tr><td colspan='6'>An error occurred while loading data</td></tr>")
        End Try
    End Sub
    
    Private Sub InitializeDataTable(userstable As DataTable)
        userstable.Columns.Add(New DataColumn("S No"))
        userstable.Columns.Add(New DataColumn("Client Name"))
        userstable.Columns.Add(New DataColumn("Login Time"))
        userstable.Columns.Add(New DataColumn("Browser"))
        userstable.Columns.Add(New DataColumn("Applicationversion"))
        userstable.Columns.Add(New DataColumn("Platform"))
    End Sub
    
    Private Sub AddEmptyRow(userstable As DataTable)
        Dim r As DataRow = userstable.NewRow()
        r(0) = "-"
        r(1) = "-"
        r(2) = "-"
        r(3) = "-"
        r(4) = "-"
        r(5) = "-"
        userstable.Rows.Add(r)
    End Sub
    
    Private Sub BuildGridHtml(userstable As DataTable)
        sb1.Clear()
        sb1.Append("<thead><tr><th style=""width:35px;"">S No</th><th style=""width:200px;"">Client User Name</th><th style=""width:220px;"">Login Time</th><th>Browser</th><th>Application Version</th><th>Platform</th></tr></thead>")
        sb1.Append("<tbody>")
        
        For i As Integer = 0 To userstable.Rows.Count - 1
            sb1.Append("<tr>")
            sb1.Append($"<td>{SafeOutput(userstable.DefaultView.Item(i)(0).ToString())}</td>")
            sb1.Append($"<td>{SafeOutput(userstable.DefaultView.Item(i)(1).ToString())}</td>")
            sb1.Append($"<td>{SafeOutput(userstable.DefaultView.Item(i)(2).ToString())}</td>")
            sb1.Append($"<td>{SafeOutput(userstable.DefaultView.Item(i)(3).ToString())}</td>")
            sb1.Append($"<td>{SafeOutput(userstable.DefaultView.Item(i)(4).ToString())}</td>")
            sb1.Append($"<td>{SafeOutput(userstable.DefaultView.Item(i)(5).ToString())}</td>")
            sb1.Append("</tr>")
        Next
        
        sb1.Append("</tbody>")
        sb1.Append("<tfoot><tr><th>S No</th><th style=""width:200px;"">Client User Name</th><th style=""width:220px;"">Login Time</th><th>Browser</th><th>Application Version</th><th>Platform</th></tr></tfoot>")
    End Sub
    
    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton.Click
        Try
            ' Validate CSRF token is handled by SecureBasePage
            FillGrid()
        Catch ex As Exception
            LogError(ex)
            sb1.Clear()
            sb1.Append("<tr><td colspan='6'>An error occurred while processing your request</td></tr>")
        End Try
    End Sub
    
    Private Function ValidateInput(input As String, inputType As String) As String
        If String.IsNullOrWhiteSpace(input) Then
            Throw New ArgumentException($"Invalid {inputType} value")
        End If
        
        Select Case inputType.ToLower()
            Case "date"
                Dim dateValue As DateTime
                If Not DateTime.TryParse(input, dateValue) Then
                    Throw New ArgumentException("Invalid date format")
                End If
                Return input.Trim()
                
            Case "hour"
                Dim hourValue As Integer
                If Not Integer.TryParse(input, hourValue) OrElse hourValue < 0 OrElse hourValue > 23 Then
                    Throw New ArgumentException("Invalid hour value")
                End If
                Return input.Trim()
                
            Case "minute"
                Dim minuteValue As Integer
                If Not Integer.TryParse(input, minuteValue) OrElse minuteValue < 0 OrElse minuteValue > 59 Then
                    Throw New ArgumentException("Invalid minute value")
                End If
                Return input.Trim()
                
            Case Else
                Return SafeOutput(input.Trim())
        End Select
    End Function
    
    Private Function IsValidDateRange(beginDateTime As String, endDateTime As String) As Boolean
        Try
            Dim startDate As DateTime = Convert.ToDateTime(beginDateTime)
            Dim endDate As DateTime = Convert.ToDateTime(endDateTime)
            
            ' Check if end date is after start date
            If endDate < startDate Then
                Return False
            End If
            
            ' Check if date range is not too large (e.g., max 90 days)
            Dim daysDifference As Integer = (endDate - startDate).Days
            If daysDifference > 90 Then
                Return False
            End If
            
            ' Check if dates are not in the future
            If startDate > DateTime.Now OrElse endDate > DateTime.Now Then
                Return False
            End If
            
            Return True
            
        Catch ex As Exception
            Return False
        End Try
    End Function
    
    Private Function GetUsersList(userId As String) As String
        ' Implement secure method to get users list based on role
        ' This should be retrieved from database based on user's permissions
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As New SqlCommand("SELECT userslist FROM userTBL WHERE userid = @userid", conn)
                cmd.Parameters.Add("@userid", SqlDbType.VarChar, 50).Value = userId
                
                conn.Open()
                Dim result As Object = cmd.ExecuteScalar()
                Return If(result IsNot Nothing, result.ToString(), userId)
            End Using
        Catch ex As Exception
            LogError(ex)
            Return userId
        End Try
    End Function
    
    Private Function SafeGetString(dr As SqlDataReader, columnName As String) As String
        Return If(IsDBNull(dr(columnName)), String.Empty, dr(columnName).ToString())
    End Function
    
    Private Function SafeGetDateTime(dr As SqlDataReader, columnName As String) As DateTime
        Return If(IsDBNull(dr(columnName)), DateTime.MinValue, Convert.ToDateTime(dr(columnName)))
    End Function
    
    Private Sub LogError(ex As Exception)
        ' Implement secure logging
        System.Diagnostics.EventLog.WriteEntry("YTLWebApp", $"Error in ClientLoginReport: {ex.Message}", System.Diagnostics.EventLogEntryType.Error)
    End Sub
End Class