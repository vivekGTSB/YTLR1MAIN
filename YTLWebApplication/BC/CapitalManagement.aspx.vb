Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class CapitalManagement
    Inherits SecureBasePage
    
    Public sb1 As New StringBuilder()
    Public opt As String
    Public sb As New StringBuilder()
    Public suserid As String
    Public userid As String
    
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' Validate authentication
            If CurrentUser Is Nothing Then
                Response.Redirect("Login.aspx")
                Return
            End If
            
            userid = CurrentUser.UserId
            Dim role As String = CurrentUser.Role
            Dim userslist As String = GetUsersList(CurrentUser.UserId)

            LoadUserDropdown(userid, role, userslist)
            
        Catch ex As Exception
            LogError(ex)
            Response.Redirect("Error.aspx")
        End Try
        
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            suserid = SafeGetHiddenValue(ss.Value)

            If Not Page.IsPostBack Then
                FillUserDropdown()
            End If
            
            FillGrid()
            
        Catch ex As Exception
            LogError(ex)
            sb1.Append("<tr><td colspan='3'>An error occurred while loading data</td></tr>")
        End Try
    End Sub
    
    Private Sub LoadUserDropdown(userid As String, role As String, userslist As String)
        Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Try
                Dim cmd As SqlCommand
                
                If role = "User" Then
                    cmd = New SqlCommand("SELECT userid, username, dbip FROM userTBL WHERE userid = @userid ORDER BY username", conn)
                    cmd.Parameters.Add("@userid", SqlDbType.VarChar, 50).Value = userid
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("SELECT userid, username, dbip FROM userTBL WHERE userid IN (SELECT value FROM STRING_SPLIT(@userslist, ',')) ORDER BY username", conn)
                    cmd.Parameters.Add("@userslist", SqlDbType.VarChar, 1000).Value = userslist
                Else
                    cmd = New SqlCommand("SELECT userid, username, dbip FROM userTBL WHERE role='User' ORDER BY username", conn)
                End If
                
                conn.Open()
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    sb = New StringBuilder()
                    sb.Append("<select id=""ddluser1"" onchange=""javascript: return refreshTable()"" data-placeholder=""Select User Group"" style=""width:250px;"" class=""chosen"" tabindex=""5"">")
                    sb.Append("<option id=""empty"" value=""""></option>")

                    If role = "SuperUser" Or role = "Admin" Then
                        sb.Append("<option selected=""selected"" value=""0"">SELECT USERNAME</option>")
                        sb.Append("<option value=""--AllUsers--"">--All Users--</option>")
                    End If

                    While dr.Read()
                        Dim userIdValue As String = SafeGetString(dr, "userid")
                        Dim usernameValue As String = SafeGetString(dr, "username").ToUpper()
                        sb.Append($"<option value=""{SafeOutput(userIdValue)}"">{SafeOutput(usernameValue)}</option>")
                    End While
                    
                    sb.Append("</select>")
                End Using
                
                opt = sb.ToString()
                
            Catch ex As Exception
                LogError(ex)
                opt = "<select><option>Error loading users</option></select>"
            End Try
        End Using
    End Sub
    
    Private Sub FillUserDropdown()
        Try
            Dim userid As String = CurrentUser.UserId
            Dim role As String = CurrentUser.Role
            Dim userslist As String = GetUsersList(CurrentUser.UserId)
            
            LoadUserDropdown(userid, role, userslist)
            
        Catch ex As Exception
            LogError(ex)
        End Try
    End Sub

    Private Sub FillGrid()
        Try
            Dim r As DataRow
            Dim j As Int32 = 1
            Dim t As New DataTable
            
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("User Name"))
            t.Columns.Add(New DataColumn("Capital"))
            t.Columns.Add(New DataColumn("userid"))

            If Not String.IsNullOrEmpty(suserid) AndAlso suserid <> "--Select User Name--" Then
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim cmd As SqlCommand
                    
                    If suserid = "--AllUsers--" Then
                        If CurrentUser.Role = "User" Then
                            cmd = New SqlCommand("SELECT userid, username, capital FROM userTBL WHERE userid = @userid ORDER BY username", conn)
                            cmd.Parameters.Add("@userid", SqlDbType.VarChar, 50).Value = CurrentUser.UserId
                        ElseIf CurrentUser.Role = "SuperUser" Or CurrentUser.Role = "Operator" Then
                            Dim userslist As String = GetUsersList(CurrentUser.UserId)
                            cmd = New SqlCommand("SELECT userid, username, capital FROM userTBL WHERE userid IN (SELECT value FROM STRING_SPLIT(@userslist, ',')) ORDER BY username", conn)
                            cmd.Parameters.Add("@userslist", SqlDbType.VarChar, 1000).Value = userslist
                        Else
                            cmd = New SqlCommand("SELECT userid, username, capital FROM userTBL WHERE role='User' ORDER BY username", conn)
                        End If
                    Else
                        cmd = New SqlCommand("SELECT userid, username, capital FROM userTBL WHERE userid = @userid ORDER BY username", conn)
                        cmd.Parameters.Add("@userid", SqlDbType.VarChar, 50).Value = suserid
                    End If

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            r = t.NewRow()
                            r(0) = j.ToString()
                            
                            Dim userIdValue As String = SafeGetString(dr, "userid")
                            Dim usernameValue As String = SafeGetString(dr, "username")
                            r(1) = $"<a href=""UpdateCapital.aspx?uid={SafeUrlEncode(userIdValue)}"">{SafeOutput(usernameValue)}</a>"
                            
                            If Not IsDBNull(dr("capital")) Then
                                r(2) = SafeOutput(dr("capital").ToString())
                            Else
                                r(2) = "-"
                            End If

                            t.Rows.Add(r)
                            j = j + 1
                        End While
                    End Using
                End Using
            End If

            If t.Rows.Count = 0 Then
                r = t.NewRow()
                r(0) = "-"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                t.Rows.Add(r)
            End If

            BuildGridHtml(t)
            
        Catch ex As Exception
            LogError(ex)
            sb1.Append("<tr><td colspan='3'>Error loading data</td></tr>")
        End Try
    End Sub
    
    Private Sub BuildGridHtml(t As DataTable)
        sb1.Clear()
        sb1.Append("<thead><tr><th style=""width:30px;"">S No</th><th>User Name</th><th>Capital Name</th></tr></thead>")
        sb1.Append("<tbody>")
        
        For i As Integer = 0 To t.Rows.Count - 1
            sb1.Append("<tr>")
            sb1.Append($"<td>{SafeOutput(t.DefaultView.Item(i)(0).ToString())}</td>")
            sb1.Append($"<td>{t.DefaultView.Item(i)(1).ToString()}</td>") ' Already encoded in FillGrid
            sb1.Append($"<td>{SafeOutput(t.DefaultView.Item(i)(2).ToString())}</td>")
            sb1.Append("</tr>")
        Next
        
        sb1.Append("</tbody>")
        sb1.Append("<tfoot><tr><th style=""width:30px;"">S No</th><th>User Name</th><th>Capital Name</th></tr></tfoot>")
    End Sub
    
    Private Function GetUsersList(userId As String) As String
        ' Implement secure method to get users list based on role
        ' This should be retrieved from database based on user's permissions
        Return userId ' Simplified for example
    End Function
    
    Private Function SafeGetString(dr As SqlDataReader, columnName As String) As String
        Return If(IsDBNull(dr(columnName)), String.Empty, dr(columnName).ToString())
    End Function
    
    Private Function SafeGetHiddenValue(value As String) As String
        If String.IsNullOrWhiteSpace(value) Then Return String.Empty
        Return value.Trim()
    End Function
    
    Private Function SafeUrlEncode(input As String) As String
        Return HttpUtility.UrlEncode(input)
    End Function
    
    Private Sub LogError(ex As Exception)
        System.Diagnostics.EventLog.WriteEntry("YTLWebApp", $"Error in CapitalManagement: {ex.Message}", System.Diagnostics.EventLogEntryType.Error)
    End Sub
End Class