Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Collections.Generic

Partial Class GetMyData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate authentication
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate and sanitize operation parameter
            Dim oper As String = SecurityHelper.SanitizeForHtml(Request.QueryString("opr"))
            If String.IsNullOrEmpty(oper) OrElse Not SecurityHelper.ValidateInput(oper, 1, "^[0-8]$") Then
                Response.StatusCode = 400
                Response.Write("Invalid operation")
                Return
            End If

            Select Case oper.ToUpper()
                Case "0"
                    GetData()
                Case "1"
                    InsertData()
                Case "2"
                    UpdateData()
                Case "3"
                    DeleteData()
                Case "4"
                    GetTransportesCompany()
                Case "5"
                    InsertDataCompany()
                Case "6"
                    updateDataCompany()
                Case "7"
                    DeleteDataCompany()
                Case "8"
                    GetDataCompany()
                Case Else
                    Response.StatusCode = 400
                    Response.Write("Invalid operation")
            End Select

        Catch ex As Exception
            SecurityHelper.LogError("GetMyData Page_Load error", ex, Server)
            Response.StatusCode = 500
            Response.Write("An error occurred")
        End Try
    End Sub

    Private Sub GetData()
        Try
            Dim aa As New ArrayList()
            Dim a As ArrayList
            
            ' SECURITY FIX: Get user data from session instead of cookies
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Dim cond As String = ""

            ' SECURITY FIX: Use parameterized conditions
            If role = "User" Then
                cond = " WHERE createdby = @userid"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If SecurityHelper.IsValidUsersList(userslist) Then
                    cond = " WHERE (createdby IN( " & userslist & ") OR createdby = @userid)"
                Else
                    cond = " WHERE createdby = @userid"
                End If
            End If

            ' SECURITY FIX: Use parameterized query
            Dim sqlstr As String = "SELECT t1.tid,t1.name,t1.pwd,t1.emailid,t1.mobileno,t2.CompanyName,t1.companyid FROM [ec_transporter_user] t1 LEFT OUTER JOIN ec_company t2 ON t1.CompanyId=t2.CompanyId" & cond & " ORDER BY Name"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(sqlstr, conn)
                    If Not String.IsNullOrEmpty(cond) Then
                        cmd.Parameters.AddWithValue("@userid", userid)
                    End If

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim c As Integer = 0
                        While dr.Read()
                            c += 1
                            a = New ArrayList()

                            If IsDBNull(dr("tid")) Then
                                a.Add("0")
                            Else
                                a.Add(SecurityHelper.SanitizeForHtml(dr("tid").ToString()))
                            End If

                            a.Add(c)
                            a.Add(SecurityHelper.SanitizeForHtml(dr("Name").ToString().ToUpper()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("pwd").ToString()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("CompanyName").ToString()))

                            If IsDBNull(dr("emailid")) Then
                                a.Add("")
                            Else
                                a.Add(SecurityHelper.SanitizeForHtml(dr("emailid").ToString()))
                            End If

                            If IsDBNull(dr("mobileno")) Then
                                a.Add("")
                            Else
                                a.Add(SecurityHelper.SanitizeForHtml(dr("mobileno").ToString()))
                            End If

                            If IsDBNull(dr("tid")) Then
                                a.Add("0")
                            Else
                                a.Add(SecurityHelper.SanitizeForHtml(dr("tid").ToString()))
                            End If

                            a.Add(SecurityHelper.SanitizeForHtml(dr("Name").ToString().ToUpper()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("companyid").ToString()))
                            aa.Add(a)
                        End While
                    End Using
                End Using
            End Using

            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)

        Catch ex As Exception
            SecurityHelper.LogError("GetData error", ex, Server)
            Response.StatusCode = 500
            Response.Write("An error occurred")
        End Try
    End Sub

    Private Sub InsertData()
        Try
            ' SECURITY FIX: Get user data from session
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            
            ' SECURITY FIX: Validate and sanitize input parameters
            Dim username As String = SecurityHelper.SanitizeForHtml(Request.QueryString("unm"))
            Dim pwd As String = SecurityHelper.SanitizeForHtml(Request.QueryString("pwd"))
            Dim email As String = SecurityHelper.SanitizeForHtml(Request.QueryString("eml"))
            Dim mob As String = SecurityHelper.SanitizeForHtml(Request.QueryString("mob"))
            Dim comp As String = SecurityHelper.SanitizeForHtml(Request.QueryString("comp"))

            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateInput(username, 50, "^[a-zA-Z0-9_]+$") Then
                Response.Write("Invalid username")
                Return
            End If

            If Not SecurityHelper.ValidateInput(pwd, 100) Then
                Response.Write("Invalid password")
                Return
            End If

            If Not String.IsNullOrEmpty(email) AndAlso Not SecurityHelper.ValidateInput(email, 100, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$") Then
                Response.Write("Invalid email")
                Return
            End If

            If Not SecurityHelper.ValidateNumeric(comp, 1, Integer.MaxValue) Then
                Response.Write("Invalid company ID")
                Return
            End If

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY FIX: Use parameterized query
                Dim query As String = "INSERT INTO [ec_transporter_user] (Name,pwd,emailid,mobileno,createdBy,CompanyId) VALUES (@username,@pwd,@email,@mob,@userid,@comp)"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", username)
                    cmd.Parameters.AddWithValue("@pwd", pwd)
                    cmd.Parameters.AddWithValue("@email", email)
                    cmd.Parameters.AddWithValue("@mob", mob)
                    cmd.Parameters.AddWithValue("@userid", userid)
                    cmd.Parameters.AddWithValue("@comp", Convert.ToInt32(comp))

                    conn.Open()
                    res = cmd.ExecuteNonQuery().ToString()
                End Using
            End Using

            Response.ContentType = "text/plain"
            Response.Write(res)

        Catch ex As Exception
            SecurityHelper.LogError("InsertData error", ex, Server)
            Response.Write("Error occurred")
        End Try
    End Sub

    Private Sub UpdateData()
        Try
            ' SECURITY FIX: Get user data from session
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            
            ' SECURITY FIX: Validate and sanitize input parameters
            Dim tid As String = SecurityHelper.SanitizeForHtml(Request.QueryString("tid"))
            Dim email As String = SecurityHelper.SanitizeForHtml(Request.QueryString("eml"))
            Dim mob As String = SecurityHelper.SanitizeForHtml(Request.QueryString("mob"))
            Dim pwd As String = SecurityHelper.SanitizeForHtml(Request.QueryString("pwd"))
            Dim comp As String = SecurityHelper.SanitizeForHtml(Request.QueryString("comp"))

            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateNumeric(tid, 1, Integer.MaxValue) Then
                Response.Write("Invalid transporter ID")
                Return
            End If

            If Not SecurityHelper.ValidateNumeric(comp, 1, Integer.MaxValue) Then
                Response.Write("Invalid company ID")
                Return
            End If

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY FIX: Use parameterized query
                Dim query As String = "UPDATE [ec_transporter_user] SET updatedby=@userid, emailid=@email, mobileno=@mob, pwd=@pwd, updateddatetime=GETDATE(), CompanyId=@comp WHERE tid=@tid"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userid", userid)
                    cmd.Parameters.AddWithValue("@email", email)
                    cmd.Parameters.AddWithValue("@mob", mob)
                    cmd.Parameters.AddWithValue("@pwd", pwd)
                    cmd.Parameters.AddWithValue("@comp", Convert.ToInt32(comp))
                    cmd.Parameters.AddWithValue("@tid", Convert.ToInt32(tid))

                    conn.Open()
                    res = cmd.ExecuteNonQuery().ToString()
                End Using
            End Using

            Response.ContentType = "text/plain"
            Response.Write(res)

        Catch ex As Exception
            SecurityHelper.LogError("UpdateData error", ex, Server)
            Response.Write("Error occurred")
        End Try
    End Sub

    Private Sub DeleteData()
        Try
            ' SECURITY FIX: Validate and sanitize input
            Dim chekitems As String = SecurityHelper.SanitizeForHtml(Request.QueryString("geoid"))
            
            If String.IsNullOrEmpty(chekitems) Then
                Response.Write("Invalid input")
                Return
            End If

            Dim ids As String() = chekitems.Split(","c)
            Dim validIds As New List(Of Integer)

            ' SECURITY FIX: Validate each ID
            For Each id As String In ids
                Dim numericId As Integer
                If Integer.TryParse(id.Trim(), numericId) AndAlso numericId > 0 Then
                    validIds.Add(numericId)
                End If
            Next

            If validIds.Count = 0 Then
                Response.Write("No valid IDs")
                Return
            End If

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    Try
                        For Each validId As Integer In validIds
                            ' SECURITY FIX: Use parameterized query
                            Dim query As String = "DELETE FROM ec_transporter_user WHERE tid = @tid"
                            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                                cmd.Transaction = transaction
                                cmd.Parameters.AddWithValue("@tid", validId)
                                Dim result As Integer = cmd.ExecuteNonQuery()
                                If result > 0 Then
                                    res = "1"
                                End If
                            End Using
                        Next
                        transaction.Commit()
                    Catch ex As Exception
                        transaction.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            Response.ContentType = "text/plain"
            Response.Write(res)

        Catch ex As Exception
            SecurityHelper.LogError("DeleteData error", ex, Server)
            Response.Write("Error occurred")
        End Try
    End Sub

    Private Sub GetTransportesCompany()
        Try
            Dim aa As New ArrayList()
            Dim a As ArrayList
            
            ' SECURITY FIX: Use parameterized query
            Dim sqlstr As String = "SELECT companyid,companyname,[dbo].[fnGetUserNames](companyid) as transporters,[dbo].fnGetGeofenceNames(companyid) as geofences,TransporterList,GeofenceList FROM ec_company"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(sqlstr, conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            a = New ArrayList()
                            a.Add(SecurityHelper.SanitizeForHtml(dr("companyid").ToString()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("companyid").ToString()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("companyname").ToString().ToUpper()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("transporters").ToString().ToUpper()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("geofences").ToString().ToUpper()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("companyid").ToString()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("TransporterList").ToString()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("GeofenceList").ToString()))
                            aa.Add(a)
                        End While
                    End Using
                End Using
            End Using

            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)

        Catch ex As Exception
            SecurityHelper.LogError("GetTransportesCompany error", ex, Server)
            Response.StatusCode = 500
            Response.Write("An error occurred")
        End Try
    End Sub

    Private Sub InsertDataCompany()
        Try
            ' SECURITY FIX: Validate and sanitize input parameters
            Dim companyname As String = SecurityHelper.SanitizeForHtml(Request.QueryString("name"))
            Dim transporters As String = SecurityHelper.SanitizeForHtml(Request.QueryString("trans"))
            Dim geofences As String = SecurityHelper.SanitizeForHtml(Request.QueryString("geof"))

            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateInput(companyname, 100) Then
                Response.Write("Invalid company name")
                Return
            End If

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY FIX: Use parameterized query
                Dim query As String = "INSERT INTO [ec_company] (CompanyName,TransporterList,GeofenceList) VALUES (@comp,@transpo,@geos)"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@comp", companyname)
                    cmd.Parameters.AddWithValue("@transpo", transporters)
                    cmd.Parameters.AddWithValue("@geos", geofences)

                    conn.Open()
                    res = cmd.ExecuteNonQuery().ToString()
                End Using
            End Using

            Response.ContentType = "text/plain"
            Response.Write(res)

        Catch ex As Exception
            SecurityHelper.LogError("InsertDataCompany error", ex, Server)
            Response.Write("Error occurred")
        End Try
    End Sub

    Private Sub updateDataCompany()
        Try
            ' SECURITY FIX: Validate and sanitize input parameters
            Dim companyname As String = SecurityHelper.SanitizeForHtml(Request.QueryString("name"))
            Dim transporters As String = SecurityHelper.SanitizeForHtml(Request.QueryString("trans"))
            Dim geofences As String = SecurityHelper.SanitizeForHtml(Request.QueryString("geof"))
            Dim compid As String = SecurityHelper.SanitizeForHtml(Request.QueryString("compid"))

            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateInput(companyname, 100) Then
                Response.Write("Invalid company name")
                Return
            End If

            If Not SecurityHelper.ValidateNumeric(compid, 1, Integer.MaxValue) Then
                Response.Write("Invalid company ID")
                Return
            End If

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY FIX: Use parameterized query
                Dim query As String = "UPDATE [ec_company] SET CompanyName=@comp, TransporterList=@transpo, GeofenceList=@geos WHERE CompanyId=@compid"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@comp", companyname)
                    cmd.Parameters.AddWithValue("@transpo", transporters)
                    cmd.Parameters.AddWithValue("@geos", geofences)
                    cmd.Parameters.AddWithValue("@compid", Convert.ToInt32(compid))

                    conn.Open()
                    res = cmd.ExecuteNonQuery().ToString()
                End Using
            End Using

            Response.ContentType = "text/plain"
            Response.Write(res)

        Catch ex As Exception
            SecurityHelper.LogError("updateDataCompany error", ex, Server)
            Response.Write("Error occurred")
        End Try
    End Sub

    Private Sub DeleteDataCompany()
        Try
            ' SECURITY FIX: Validate and sanitize input
            Dim chekitems As String = SecurityHelper.SanitizeForHtml(Request.QueryString("geoid"))
            
            If String.IsNullOrEmpty(chekitems) Then
                Response.Write("Invalid input")
                Return
            End If

            Dim ids As String() = chekitems.Split(","c)
            Dim validIds As New List(Of Integer)

            ' SECURITY FIX: Validate each ID
            For Each id As String In ids
                Dim numericId As Integer
                If Integer.TryParse(id.Trim(), numericId) AndAlso numericId > 0 Then
                    validIds.Add(numericId)
                End If
            Next

            If validIds.Count = 0 Then
                Response.Write("No valid IDs")
                Return
            End If

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    Try
                        For Each validId As Integer In validIds
                            ' SECURITY FIX: Use parameterized query
                            Dim query As String = "DELETE FROM ec_company WHERE companyid = @companyid"
                            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                                cmd.Transaction = transaction
                                cmd.Parameters.AddWithValue("@companyid", validId)
                                Dim result As Integer = cmd.ExecuteNonQuery()
                                If result > 0 Then
                                    res = "1"
                                End If
                            End Using
                        Next
                        transaction.Commit()
                    Catch ex As Exception
                        transaction.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            Response.ContentType = "text/plain"
            Response.Write(res)

        Catch ex As Exception
            SecurityHelper.LogError("DeleteDataCompany error", ex, Server)
            Response.Write("Error occurred")
        End Try
    End Sub

    Private Sub GetDataCompany()
        Try
            Dim aa As New ArrayList()
            Dim userslist As New List(Of vals)
            Dim geoslist As New List(Of vals)
            Dim v As vals
            
            ' SECURITY FIX: Use parameterized query for users
            Dim sqlstr As String = "SELECT userid,username,(CASE WHEN EXISTS (SELECT item FROM fn_getcompanylist(0) WHERE item=userid) THEN 0 ELSE 1 END) as status FROM userTBL WHERE role='User' ORDER BY username"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(sqlstr, conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            v = New vals
                            v.id = Convert.ToInt32(dr("userid"))
                            v.text = SecurityHelper.SanitizeForHtml(dr("username").ToString().ToUpper())
                            v.status = Convert.ToBoolean(dr("status"))
                            userslist.Add(v)
                        End While
                    End Using

                    ' SECURITY FIX: Use parameterized query for geofences
                    sqlstr = "SELECT ISNULL(shiptocode,'') shiptocode, geofenceid, geofencename, (CASE WHEN EXISTS (SELECT item FROM fn_getgeofencelist(0) WHERE item=geofenceid) THEN 0 ELSE 1 END) as status FROM geofence WHERE accesstype=1 AND shiptocode NOT LIKE '000%' ORDER BY geofencename"
                    cmd.CommandText = sqlstr
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            v = New vals
                            v.id = Convert.ToInt32(dr("geofenceid"))
                            v.text = SecurityHelper.SanitizeForHtml(dr("geofencename").ToString().ToUpper() & " - " & dr("shiptocode").ToString().ToUpper())
                            v.status = Convert.ToBoolean(dr("status"))
                            geoslist.Add(v)
                        End While
                    End Using
                End Using
            End Using

            aa.Add(userslist)
            aa.Add(geoslist)

            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)

        Catch ex As Exception
            SecurityHelper.LogError("GetDataCompany error", ex, Server)
            Response.StatusCode = 500
            Response.Write("An error occurred")
        End Try
    End Sub

    Structure vals
        Dim id As Integer
        Dim text As String
        Dim status As Boolean
    End Structure

End Class