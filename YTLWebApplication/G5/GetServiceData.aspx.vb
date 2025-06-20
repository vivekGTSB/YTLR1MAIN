Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.Net

Partial Class GetServiceData
    Inherits SecurePageBase

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.Write("{""error"":""Unauthorized""}")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate and sanitize input parameter
            Dim code As Integer = 0
            Dim codeParam As String = SecurityHelper.SanitizeForHtml(Request.QueryString("i"))
            If Not Integer.TryParse(codeParam, code) Then
                Response.StatusCode = 400
                Response.Write("{""error"":""Invalid parameter""}")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate CSRF token for POST requests
            If Request.HttpMethod.ToUpper() = "POST" Then
                Dim csrfToken As String = Request.Form("__CSRFToken")
                If Not SecurityHelper.ValidateCSRFToken(csrfToken) Then
                    Response.StatusCode = 403
                    Response.Write("{""error"":""Invalid CSRF token""}")
                    Response.End()
                    Return
                End If
            End If

            Select Case code
                Case 1
                    Dim userid As String = SecurityHelper.SanitizeForHtml(Request.Form("ugData"))
                    Dim role As String = SecurityHelper.SanitizeForHtml(Request.Form("role"))
                    Dim userslist As String = SecurityHelper.SanitizeForHtml(Request.Form("userslist"))
                    Response.Write(GetData(userid, role, userslist))
                    Response.ContentType = "application/json"
                Case 2
                    Dim userslist As String = SecurityHelper.SanitizeForHtml(Request.Form("ugData"))
                    Response.Write(DeleteRecord(userslist))
                Case 3
                    Response.Write(UpdateServiceData())
                Case 4
                    Dim userid As String = SecurityHelper.SanitizeForHtml(Request.Form("userId"))
                    Response.Write(LoadGrouplist(userid))
                Case 5
                    Dim userid As String = SecurityHelper.SanitizeForHtml(Request.Form("userId"))
                    Response.Write(LoadGrouplist1(userid))
                Case 6
                    Dim userid As String = SecurityHelper.SanitizeForHtml(Request.Form("userId"))
                    Response.Write(LoadGrouplist2(userid))
                Case Else
                    Response.StatusCode = 400
                    Response.Write("{""error"":""Invalid operation""}")
            End Select

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GENERAL_ERROR", "Error in GetServiceData: " & ex.Message)
            Response.StatusCode = 500
            Response.Write("{""error"":""Internal server error""}")
        End Try
    End Sub

    Private Function GetData(ugData As String, role As String, userslist As String) As String
        Try
            ' SECURITY FIX: Validate inputs
            If String.IsNullOrEmpty(ugData) OrElse String.IsNullOrEmpty(role) Then
                Return "{""error"":""Invalid parameters""}"
            End If

            ' SECURITY FIX: Validate users list format
            If Not String.IsNullOrEmpty(userslist) AndAlso Not SecurityHelper.ValidateUsersList(userslist) Then
                userslist = ""
            End If

            Dim aa As New ArrayList()
            Dim servicingTable As New DataTable()
            
            ' Initialize table structure
            InitializeServiceTable(servicingTable)

            Try
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    ' SECURITY FIX: Build secure query with parameters
                    Dim query As String = BuildSecureServiceQuery(ugData, role, userslist)
                    Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                        AddServiceQueryParameters(cmd, ugData, role, userslist)
                        
                        conn.Open()
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            ProcessServiceData(dr, servicingTable)
                        End Using
                    End Using
                End Using

                ' Process results for JSON output
                ProcessServiceResults(servicingTable, aa)
                
                ' Store in session for Excel export
                Session("exceltable") = servicingTable
                
                Return JsonConvert.SerializeObject(aa, Formatting.None)

            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("DATABASE_ERROR", "Error in GetData: " & ex.Message)
                Return "{""error"":""Database error""}"
            End Try

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GENERAL_ERROR", "Error in GetData: " & ex.Message)
            Return "{""error"":""Internal server error""}"
        End Try
    End Function

    Private Sub InitializeServiceTable(servicingTable As DataTable)
        servicingTable.Columns.Add(New DataColumn("serviceid"))
        servicingTable.Columns.Add(New DataColumn("plateno"))
        servicingTable.Columns.Add(New DataColumn("userid"))
        servicingTable.Columns.Add(New DataColumn("lastservicingdate"))
        servicingTable.Columns.Add(New DataColumn("lastservicingodometer"))
        servicingTable.Columns.Add(New DataColumn("OdometerLimit"))
        servicingTable.Columns.Add(New DataColumn("TimeLimit"))
        servicingTable.Columns.Add(New DataColumn("EngineLimit"))
        servicingTable.Columns.Add(New DataColumn("emailid1"))
        servicingTable.Columns.Add(New DataColumn("emailid2"))
        servicingTable.Columns.Add(New DataColumn("mobile1"))
        servicingTable.Columns.Add(New DataColumn("mobile2"))
        servicingTable.Columns.Add(New DataColumn("remarks"))
    End Sub

    Private Function BuildSecureServiceQuery(ugData As String, role As String, userslist As String) As String
        Dim query As String = ""
        
        If ugData <> "--Select User Name--" Then
            If ugData = "--AllUsers--" Then
                If role = "User" Then
                    query = "SELECT vt.userid, vt.plateno, serviceid, lastservicingdate, engineLimit, lastservicingodometer, odometerlimit, timelimit, emailid1, emailid2, mobile1, mobile2, CASE WHEN CHARINDEX(CHAR(10), remarks) > 0 THEN REPLACE(remarks, CHAR(10), ' ') ELSE remarks END as remarks FROM (SELECT * FROM vehicleTBL WHERE userid = @userid) vt LEFT OUTER JOIN (SELECT * FROM servicing) st ON st.plateno = vt.plateno ORDER BY plateno, lastservicingdate DESC"
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    If Not String.IsNullOrEmpty(userslist) Then
                        query = $"SELECT vt.userid, vt.plateno, serviceid, lastservicingdate, engineLimit, lastservicingodometer, odometerlimit, timelimit, emailid1, emailid2, mobile1, mobile2, CASE WHEN CHARINDEX(CHAR(10), remarks) > 0 THEN REPLACE(remarks, CHAR(10), ' ') ELSE remarks END as remarks FROM (SELECT * FROM vehicleTBL WHERE userid IN ({userslist})) vt LEFT OUTER JOIN (SELECT * FROM servicing) st ON st.plateno = vt.plateno ORDER BY plateno, lastservicingdate DESC"
                    Else
                        query = "SELECT vt.userid, vt.plateno, serviceid, lastservicingdate, engineLimit, lastservicingodometer, odometerlimit, timelimit, emailid1, emailid2, mobile1, mobile2, CASE WHEN CHARINDEX(CHAR(10), remarks) > 0 THEN REPLACE(remarks, CHAR(10), ' ') ELSE remarks END as remarks FROM (SELECT * FROM vehicleTBL WHERE userid IN (SELECT userid FROM userTBL WHERE role = 'User')) vt LEFT OUTER JOIN (SELECT * FROM servicing) st ON st.plateno = vt.plateno ORDER BY plateno, lastservicingdate DESC"
                    End If
                Else
                    query = "SELECT vt.userid, vt.plateno, serviceid, lastservicingdate, engineLimit, lastservicingodometer, odometerlimit, timelimit, emailid1, emailid2, mobile1, mobile2, CASE WHEN CHARINDEX(CHAR(10), remarks) > 0 THEN REPLACE(remarks, CHAR(10), ' ') ELSE remarks END as remarks FROM (SELECT * FROM vehicleTBL WHERE userid IN (SELECT userid FROM userTBL WHERE role = 'User')) vt LEFT OUTER JOIN (SELECT * FROM servicing) st ON st.plateno = vt.plateno ORDER BY plateno, lastservicingdate DESC"
                End If
            Else
                query = "SELECT vt.userid, vt.plateno, serviceid, engineLimit, lastservicingdate, lastservicingodometer, odometerlimit, timelimit, emailid1, emailid2, mobile1, mobile2, CASE WHEN CHARINDEX(CHAR(10), remarks) > 0 THEN REPLACE(remarks, CHAR(10), ' ') ELSE remarks END as remarks FROM (SELECT * FROM vehicleTBL WHERE userid = @userid) vt LEFT OUTER JOIN (SELECT * FROM servicing) st ON st.plateno = vt.plateno ORDER BY plateno, lastservicingdate DESC"
            End If
        End If
        
        Return query
    End Function

    Private Sub AddServiceQueryParameters(cmd As SqlCommand, ugData As String, role As String, userslist As String)
        If ugData <> "--Select User Name--" AndAlso ugData <> "--AllUsers--" Then
            cmd.Parameters.AddWithValue("@userid", ugData)
        ElseIf ugData = "--AllUsers--" AndAlso (role = "User" OrElse String.IsNullOrEmpty(userslist)) Then
            cmd.Parameters.AddWithValue("@userid", SessionManager.GetCurrentUserId())
        End If
    End Sub

    Private Sub ProcessServiceData(dr As SqlDataReader, servicingTable As DataTable)
        While dr.Read()
            Try
                Dim r As DataRow = servicingTable.NewRow()
                
                If Not IsDBNull(dr("serviceid")) Then
                    r(0) = dr("serviceid")
                    r(1) = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
                    r(2) = SecurityHelper.SanitizeForHtml(dr("userid").ToString())
                    r(3) = Convert.ToDateTime(dr("lastservicingdate")).ToString("yyyy/MM/dd")
                    r(4) = Convert.ToDouble(dr("lastservicingodometer")).ToString("0.00")
                    r(5) = If(IsDBNull(dr("odometerlimit")), "", SecurityHelper.SanitizeForHtml(dr("odometerlimit").ToString()))
                    r(6) = If(IsDBNull(dr("timeLimit")), "", Convert.ToDateTime(dr("timeLimit")).ToString("yyyy/MM/dd"))
                    r(7) = If(IsDBNull(dr("engineLimit")), "", SecurityHelper.SanitizeForHtml(dr("engineLimit").ToString()))
                    r(8) = SecurityHelper.SanitizeForHtml(dr("emailid1").ToString())
                    r(9) = SecurityHelper.SanitizeForHtml(dr("emailid2").ToString())
                    r(10) = SecurityHelper.SanitizeForHtml(dr("mobile1").ToString())
                    r(11) = SecurityHelper.SanitizeForHtml(dr("mobile2").ToString())
                    r(12) = SecurityHelper.SanitizeForHtml(dr("remarks").ToString())
                Else
                    For i As Integer = 0 To 12
                        If i = 1 Then
                            r(i) = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
                        ElseIf i = 2 Then
                            r(i) = SecurityHelper.SanitizeForHtml(dr("userid").ToString())
                        Else
                            r(i) = "--"
                        End If
                    Next
                End If
                
                servicingTable.Rows.Add(r)
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("ROW_PROCESSING_ERROR", "Error processing service data row: " & ex.Message)
            End Try
        End While
    End Sub

    Private Sub ProcessServiceResults(servicingTable As DataTable, aa As ArrayList)
        If servicingTable.Rows.Count = 0 Then
            Dim r As DataRow = servicingTable.NewRow()
            For i As Integer = 0 To 12
                r(i) = "--"
            Next
            servicingTable.Rows.Add(r)
        End If

        For j As Integer = 0 To servicingTable.Rows.Count - 1
            Try
                Dim a As New ArrayList()
                
                If servicingTable.DefaultView.Item(j)("serviceid").ToString() <> "--" Then
                    a.Add($"<input type=""checkbox"" name=""chk"" class=""group1"" value=""{SecurityHelper.SanitizeForHtml(servicingTable.DefaultView.Item(j)("serviceid").ToString())}""/>")
                    a.Add(j + 1)
                    a.Add($"<span style='cursor:pointer;text-decoration:underline;' onclick=""javascript:openPopup('{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("userid").ToString())}','{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("plateno").ToString())}','{SecurityHelper.SanitizeForJavaScript(Convert.ToDateTime(servicingTable.DefaultView.Item(j)("lastservicingdate")).ToString("yyyy/MM/dd"))}','{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("lastservicingodometer").ToString())}','{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("OdometerLimit").ToString())}','{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("TimeLimit").ToString())}','{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("EngineLimit").ToString())}','{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("emailid1").ToString())}','{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("emailid2").ToString())}','{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("mobile1").ToString())}','{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("mobile2").ToString())}','{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("serviceid").ToString())}','{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("remarks").ToString().Replace("'", "\'"))}')"">{SecurityHelper.SanitizeForHtml(servicingTable.DefaultView.Item(j)("plateno").ToString().ToUpper())}</span>")
                Else
                    a.Add("")
                    a.Add(j + 1)
                    If servicingTable.DefaultView.Item(j)("userid").ToString() <> "--" Then
                        a.Add($"<span style='cursor:pointer;text-decoration:underline;' onclick=""javascript:openPopup('{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("userid").ToString())}','{SecurityHelper.SanitizeForJavaScript(servicingTable.DefaultView.Item(j)("plateno").ToString())}','','','','','','','','','','0','')"">{SecurityHelper.SanitizeForHtml(servicingTable.DefaultView.Item(j)("plateno").ToString().ToUpper())}</span>")
                    Else
                        a.Add("--")
                    End If
                End If
                
                For i As Integer = 3 To 12
                    a.Add(servicingTable.DefaultView.Item(j)(i))
                Next
                
                aa.Add(a)
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("RESULT_PROCESSING_ERROR", "Error processing service result: " & ex.Message)
            End Try
        Next
    End Sub

    Private Function DeleteRecord(ugData As String) As String
        Try
            ' SECURITY FIX: Validate input
            If String.IsNullOrEmpty(ugData) Then
                Return "{""error"":""Invalid data""}"
            End If

            Dim result As String = "0"
            Dim groupides() As String = ugData.Split(","c)
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                For i As Integer = 0 To groupides.Length - 1
                    If groupides(i) <> "on" AndAlso Not String.IsNullOrEmpty(groupides(i)) Then
                        ' SECURITY FIX: Validate service ID
                        Dim serviceId As Integer
                        If Integer.TryParse(groupides(i), serviceId) AndAlso serviceId > 0 Then
                            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(
                                "DELETE FROM servicing WHERE serviceid = @serviceid", conn)
                                cmd.Parameters.AddWithValue("@serviceid", serviceId)
                                
                                conn.Open()
                                result = cmd.ExecuteNonQuery().ToString()
                                conn.Close()
                            End Using
                        End If
                    End If
                Next
            End Using
            
            Return result
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("DELETE_ERROR", "Error deleting service record: " & ex.Message)
            Return "{""error"":""Delete failed""}"
        End Try
    End Function

    Private Function UpdateServiceData() As String
        Try
            ' SECURITY FIX: Validate and sanitize all inputs
            Dim userid As String = SecurityHelper.SanitizeForHtml(Request.Form("userid"))
            Dim plateno As String = SecurityHelper.SanitizeForHtml(Request.Form("plateno"))
            Dim hodo As String = SecurityHelper.SanitizeForHtml(Request.Form("hodo"))
            Dim htime As String = SecurityHelper.SanitizeForHtml(Request.Form("htime"))
            Dim odolimit As String = SecurityHelper.SanitizeForHtml(Request.Form("odolimit"))
            Dim timelimit As String = SecurityHelper.SanitizeForHtml(Request.Form("timelimit"))
            Dim enginelimit As String = SecurityHelper.SanitizeForHtml(Request.Form("enginelimit"))
            Dim emailid1 As String = SecurityHelper.SanitizeForHtml(Request.Form("emailid1"))
            Dim emailid2 As String = SecurityHelper.SanitizeForHtml(Request.Form("emailid2"))
            Dim mobile1 As String = SecurityHelper.SanitizeForHtml(Request.Form("mobile1"))
            Dim mobile2 As String = SecurityHelper.SanitizeForHtml(Request.Form("mobile2"))
            Dim id As String = SecurityHelper.SanitizeForHtml(Request.Form("id"))
            Dim remarks As String = SecurityHelper.SanitizeForHtml(Request.Form("remarks"))

            ' SECURITY FIX: Validate required fields
            If Not SecurityHelper.ValidateUserId(userid) OrElse Not SecurityHelper.ValidatePlateNumber(plateno) Then
                Return "{""error"":""Invalid user ID or plate number""}"
            End If

            If Not SecurityHelper.ValidateDate(htime) Then
                Return "{""error"":""Invalid date format""}"
            End If

            Return UpdateData(userid, plateno, hodo, htime, odolimit, timelimit, enginelimit, emailid1, emailid2, mobile1, mobile2, id, remarks)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("UPDATE_SERVICE_ERROR", "Error updating service data: " & ex.Message)
            Return "{""error"":""Update failed""}"
        End Try
    End Function

    Public Function UpdateData(userid As String, plateno As String, hodo As String, htime As String, odolimit As String, timelimit As String, enginelimit As String, emailid1 As String, emailid2 As String, mobile1 As String, mobile2 As String, id As String, remarks As String) As String
        Try
            Dim odo As Double = Convert.ToDouble(GetOdometer(Convert.ToDateTime(htime).ToString("yyyy/MM/dd 00:00:00"), plateno))
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY FIX: Escape single quotes in remarks
                remarks = remarks.Replace("'", "''")
                
                ' Build secure update/insert query
                Dim updateQuery As String = BuildUpdateQuery(userid, plateno, htime, odo, odolimit, timelimit, enginelimit, emailid1, emailid2, mobile1, mobile2, id, remarks)
                Dim insertQuery As String = BuildInsertQuery(userid, plateno, htime, odo, odolimit, timelimit, enginelimit, emailid1, emailid2, mobile1, mobile2, remarks)
                
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(updateQuery, conn)
                    AddUpdateParameters(cmd, userid, plateno, htime, odo, odolimit, timelimit, enginelimit, emailid1, emailid2, mobile1, mobile2, id, remarks)
                    
                    conn.Open()
                    Dim result As String = cmd.ExecuteNonQuery().ToString()
                    
                    If result = "0" Then
                        cmd.CommandText = insertQuery
                        cmd.Parameters.Clear()
                        AddInsertParameters(cmd, userid, plateno, htime, odo, odolimit, timelimit, enginelimit, emailid1, emailid2, mobile1, mobile2, remarks)
                        result = cmd.ExecuteNonQuery().ToString()
                    End If
                    
                    Return result
                End Using
            End Using
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("UPDATE_DATA_ERROR", "Error in UpdateData: " & ex.Message)
            Return "{""error"":""Update failed""}"
        End Try
    End Function

    Private Function BuildUpdateQuery(userid As String, plateno As String, htime As String, odo As Double, odolimit As String, timelimit As String, enginelimit As String, emailid1 As String, emailid2 As String, mobile1 As String, mobile2 As String, id As String, remarks As String) As String
        Dim query As String = "UPDATE servicing SET lastservicingdate = @htime, lastservicingodometer = @odo, emailid1 = @emailid1, emailid2 = @emailid2, mobile1 = @mobile1, mobile2 = @mobile2, remarks = @remarks"
        
        If Not String.IsNullOrEmpty(odolimit) Then
            query &= ", odometerlimit = @odolimit"
        End If
        If Not String.IsNullOrEmpty(timelimit) Then
            query &= ", timelimit = @timelimit"
        End If
        If Not String.IsNullOrEmpty(enginelimit) Then
            query &= ", enginelimit = @enginelimit"
        End If
        
        query &= " WHERE serviceid = @id"
        Return query
    End Function

    Private Function BuildInsertQuery(userid As String, plateno As String, htime As String, odo As Double, odolimit As String, timelimit As String, enginelimit As String, emailid1 As String, emailid2 As String, mobile1 As String, mobile2 As String, remarks As String) As String
        Dim columns As String = "userid, plateno, lastservicingdate, lastservicingodometer, emailid1, emailid2, mobile1, mobile2, remarks"
        Dim values As String = "@userid, @plateno, @htime, @odo, @emailid1, @emailid2, @mobile1, @mobile2, @remarks"
        
        If Not String.IsNullOrEmpty(odolimit) Then
            columns &= ", odometerlimit"
            values &= ", @odolimit"
        End If
        If Not String.IsNullOrEmpty(timelimit) Then
            columns &= ", timelimit"
            values &= ", @timelimit"
        End If
        If Not String.IsNullOrEmpty(enginelimit) Then
            columns &= ", enginelimit"
            values &= ", @enginelimit"
        End If
        
        Return $"INSERT INTO servicing ({columns}) VALUES ({values})"
    End Function

    Private Sub AddUpdateParameters(cmd As SqlCommand, userid As String, plateno As String, htime As String, odo As Double, odolimit As String, timelimit As String, enginelimit As String, emailid1 As String, emailid2 As String, mobile1 As String, mobile2 As String, id As String, remarks As String)
        cmd.Parameters.AddWithValue("@htime", Convert.ToDateTime(htime).ToString("yyyy/MM/dd 23:59:59"))
        cmd.Parameters.AddWithValue("@odo", odo)
        cmd.Parameters.AddWithValue("@emailid1", emailid1)
        cmd.Parameters.AddWithValue("@emailid2", emailid2)
        cmd.Parameters.AddWithValue("@mobile1", mobile1)
        cmd.Parameters.AddWithValue("@mobile2", mobile2)
        cmd.Parameters.AddWithValue("@remarks", remarks)
        cmd.Parameters.AddWithValue("@id", id)
        
        If Not String.IsNullOrEmpty(odolimit) Then
            cmd.Parameters.AddWithValue("@odolimit", odolimit)
        End If
        If Not String.IsNullOrEmpty(timelimit) Then
            cmd.Parameters.AddWithValue("@timelimit", Convert.ToDateTime(timelimit).ToString("yyyy/MM/dd 23:59:59"))
        End If
        If Not String.IsNullOrEmpty(enginelimit) Then
            cmd.Parameters.AddWithValue("@enginelimit", enginelimit)
        End If
    End Sub

    Private Sub AddInsertParameters(cmd As SqlCommand, userid As String, plateno As String, htime As String, odo As Double, odolimit As String, timelimit As String, enginelimit As String, emailid1 As String, emailid2 As String, mobile1 As String, mobile2 As String, remarks As String)
        cmd.Parameters.AddWithValue("@userid", userid)
        cmd.Parameters.AddWithValue("@plateno", plateno)
        cmd.Parameters.AddWithValue("@htime", Convert.ToDateTime(htime).ToString("yyyy/MM/dd 23:59:59"))
        cmd.Parameters.AddWithValue("@odo", odo)
        cmd.Parameters.AddWithValue("@emailid1", emailid1)
        cmd.Parameters.AddWithValue("@emailid2", emailid2)
        cmd.Parameters.AddWithValue("@mobile1", mobile1)
        cmd.Parameters.AddWithValue("@mobile2", mobile2)
        cmd.Parameters.AddWithValue("@remarks", remarks)
        
        If Not String.IsNullOrEmpty(odolimit) Then
            cmd.Parameters.AddWithValue("@odolimit", odolimit)
        End If
        If Not String.IsNullOrEmpty(timelimit) Then
            cmd.Parameters.AddWithValue("@timelimit", Convert.ToDateTime(timelimit).ToString("yyyy/MM/dd 23:59:59"))
        End If
        If Not String.IsNullOrEmpty(enginelimit) Then
            cmd.Parameters.AddWithValue("@enginelimit", enginelimit)
        End If
    End Sub

    Public Shared Function GetOdometer(bdt As String, plateno As String) As Double
        Dim odometer As Double = 0.0
        Try
            ' SECURITY FIX: Validate inputs
            If Not SecurityHelper.ValidateDate(bdt) OrElse Not SecurityHelper.ValidatePlateNumber(plateno) Then
                Return 0.0
            End If

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(
                    "SELECT TOP 1 absolute FROM vehicle_odometer WHERE plateno = @plateno AND timestamp = @timestamp AND absolute > -1 ORDER BY timestamp", conn)
                    cmd.Parameters.AddWithValue("@plateno", plateno)
                    cmd.Parameters.AddWithValue("@timestamp", Convert.ToDateTime(bdt).ToString("yyyy/MM/dd 00:00:00"))
                    
                    conn.Open()
                    Dim result As Object = cmd.ExecuteScalar()
                    If result IsNot Nothing Then
                        odometer = Convert.ToDouble(result)
                    End If
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("ODOMETER_ERROR", "Error getting odometer: " & ex.Message)
            odometer = 0
        End Try
        
        Return odometer
    End Function

    Public Function LoadGrouplist(userId As String) As String
        Try
            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateUserId(userId) AndAlso userId <> "0" Then
                Return "{""error"":""Invalid user ID""}"
            End If

            Dim grouplist As New ArrayList()
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = If(userId = "0", 
                    "SELECT plateno, userid FROM vehicleTBL", 
                    "SELECT plateno, userid FROM vehicleTBL WHERE userid = @userid")
                
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    If userId <> "0" Then
                        cmd.Parameters.AddWithValue("@userid", userId)
                    End If
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            grouplist.Add(New With {
                                .Text = SecurityHelper.SanitizeForHtml(dr("plateno").ToString().ToUpper()),
                                .Value = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
                            })
                        End While
                    End Using
                End Using
            End Using
            
            Return JsonConvert.SerializeObject(grouplist, Formatting.None)
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOAD_GROUPLIST_ERROR", "Error loading group list: " & ex.Message)
            Return "{""error"":""Failed to load data""}"
        End Try
    End Function

    Public Function LoadGrouplist1(userId As String) As ArrayList
        Dim grouplist As New ArrayList()
        Try
            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateUserId(userId) AndAlso userId <> "0" Then
                Return grouplist
            End If

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = If(userId = "0", 
                    "SELECT plateno, userid FROM vehicleTBL", 
                    "SELECT plateno, userid FROM vehicleTBL WHERE userid = @userid")
                
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    If userId <> "0" Then
                        cmd.Parameters.AddWithValue("@userid", userId)
                    End If
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            grouplist.Add(New With {
                                .Text = SecurityHelper.SanitizeForHtml(dr("plateno").ToString().ToUpper()),
                                .Value = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
                            })
                        End While
                    End Using
                End Using
            End Using
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOAD_GROUPLIST1_ERROR", "Error loading group list 1: " & ex.Message)
        End Try
        
        Return grouplist
    End Function

    Public Shared Function LoadGrouplist2(userId As String) As String
        Try
            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateUserId(userId) AndAlso userId <> "0" Then
                Return "{""error"":""Invalid user ID""}"
            End If

            Dim aa As New ArrayList()
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = If(userId = "0", 
                    "SELECT plateno, userid FROM vehicleTBL", 
                    "SELECT plateno, userid FROM vehicleTBL WHERE userid = @userid")
                
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    If userId <> "0" Then
                        cmd.Parameters.AddWithValue("@userid", userId)
                    End If
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim grouplist As New ArrayList()
                            grouplist.Add(New With {
                                .Text = SecurityHelper.SanitizeForHtml(dr("plateno").ToString().ToUpper()),
                                .Value = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
                            })
                            aa.Add(grouplist)
                        End While
                    End Using
                End Using
            End Using
            
            Return JsonConvert.SerializeObject(aa, Formatting.None)
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOAD_GROUPLIST2_ERROR", "Error loading group list 2: " & ex.Message)
            Return "{""error"":""Failed to load data""}"
        End Try
    End Function

End Class