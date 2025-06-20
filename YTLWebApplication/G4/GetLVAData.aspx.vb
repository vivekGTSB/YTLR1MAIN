Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetLVAData
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
            If String.IsNullOrEmpty(oper) OrElse Not SecurityHelper.ValidateInput(oper, 1, "^[0-5]$") Then
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
                    MultiInsert()
                Case "5"
                    MultiInsertALL()
                Case Else
                    Response.StatusCode = 400
                    Response.Write("Invalid operation")
            End Select

        Catch ex As Exception
            SecurityHelper.LogError("GetLVAData Page_Load error", ex, Server)
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
            
            ' SECURITY FIX: Validate and sanitize sUserid parameter
            Dim sUserid As String = SecurityHelper.SanitizeForHtml(Request.QueryString("suid"))
            If Not SecurityHelper.ValidateInput(sUserid, 10, "^[A-Za-z0-9]+$") Then
                Response.StatusCode = 400
                Response.Write("Invalid user ID")
                Return
            End If

            Dim cond As String = ""

            If sUserid = "ALL" Then
                If role = "User" Then
                    cond = " where userid = @userid"
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    If SecurityHelper.IsValidUsersList(userslist) Then
                        cond = " where userid in( " & userslist & ")"
                    Else
                        cond = " where userid = @userid"
                    End If
                End If
            Else
                cond = " where userid = @sUserid"
            End If

            ' SECURITY FIX: Use parameterized query
            Dim sqlstr As String = "select gt.plateno,at.id,at.emaillist,at.mobileno from (select plateno from vehicleTBL " & cond & ") as gt left outer join void_alert_settings at on at.plateno=gt.plateno"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(sqlstr, conn)
                    ' Add parameters based on condition
                    If sUserid = "ALL" Then
                        If role = "User" OrElse Not SecurityHelper.IsValidUsersList(userslist) Then
                            cmd.Parameters.AddWithValue("@userid", userid)
                        End If
                    Else
                        cmd.Parameters.AddWithValue("@sUserid", sUserid)
                    End If

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim c As Integer = 0
                        While dr.Read()
                            c += 1
                            a = New ArrayList()

                            If IsDBNull(dr("id")) Then
                                a.Add("0")
                            Else
                                a.Add(SecurityHelper.SanitizeForHtml(dr("id").ToString()))
                            End If

                            a.Add(c)
                            a.Add(SecurityHelper.SanitizeForHtml(dr("plateno").ToString()))

                            If IsDBNull(dr("emaillist")) Then
                                a.Add("")
                            Else
                                a.Add(SecurityHelper.SanitizeForHtml(dr("emaillist").ToString()))
                            End If

                            If IsDBNull(dr("mobileno")) Then
                                a.Add("")
                            Else
                                a.Add(SecurityHelper.SanitizeForHtml(dr("mobileno").ToString()))
                            End If

                            a.Add(SecurityHelper.SanitizeForHtml(dr("plateno").ToString()))

                            If IsDBNull(dr("id")) Then
                                a.Add("0")
                            Else
                                a.Add(SecurityHelper.SanitizeForHtml(dr("id").ToString()))
                            End If
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
            ' SECURITY FIX: Validate and sanitize input parameters
            Dim plateno As String = SecurityHelper.SanitizeForHtml(Request.QueryString("geoid"))
            Dim email As String = SecurityHelper.SanitizeForHtml(Request.QueryString("eml"))
            Dim mob As String = SecurityHelper.SanitizeForHtml(Request.QueryString("mob"))

            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidatePlateNumber(plateno) Then
                Response.Write("Invalid plate number")
                Return
            End If

            If Not String.IsNullOrEmpty(email) AndAlso Not SecurityHelper.ValidateInput(email, 100, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$") Then
                Response.Write("Invalid email format")
                Return
            End If

            If Not String.IsNullOrEmpty(mob) AndAlso Not SecurityHelper.ValidateInput(mob, 20, "^[0-9+\-\s()]+$") Then
                Response.Write("Invalid mobile number")
                Return
            End If

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY FIX: Use parameterized query
                Dim query As String = "INSERT INTO void_alert_settings (plateno,emaillist,mobileno,updateddatetime) VALUES (@plateno,@email,@mob,@datetime)"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@plateno", plateno)
                    cmd.Parameters.AddWithValue("@email", email)
                    cmd.Parameters.AddWithValue("@mob", mob)
                    cmd.Parameters.AddWithValue("@datetime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))

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

    Private Sub MultiInsert()
        Try
            ' SECURITY FIX: Validate and sanitize input
            Dim geoidParam As String = SecurityHelper.SanitizeForHtml(Request.QueryString("geoid"))
            Dim email As String = SecurityHelper.SanitizeForHtml(Request.QueryString("eml"))
            Dim mob As String = SecurityHelper.SanitizeForHtml(Request.QueryString("mob"))

            If String.IsNullOrEmpty(geoidParam) Then
                Response.Write("Invalid input")
                Return
            End If

            Dim platenos() As String = geoidParam.Split(","c)
            Dim validPlateNumbers As New List(Of String)

            ' SECURITY FIX: Validate each plate number
            For Each plateNo As String In platenos
                If SecurityHelper.ValidatePlateNumber(plateNo.Trim()) Then
                    validPlateNumbers.Add(plateNo.Trim())
                End If
            Next

            If validPlateNumbers.Count = 0 Then
                Response.Write("No valid plate numbers")
                Return
            End If

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    Try
                        ' SECURITY FIX: Delete existing records with parameterized query
                        For Each plateNo As String In validPlateNumbers
                            Dim deleteQuery As String = "DELETE FROM void_alert_settings WHERE plateno = @plateno"
                            Using deleteCmd As SqlCommand = SecurityHelper.CreateSafeCommand(deleteQuery, conn)
                                deleteCmd.Transaction = transaction
                                deleteCmd.Parameters.AddWithValue("@plateno", plateNo)
                                deleteCmd.ExecuteNonQuery()
                            End Using
                        Next

                        ' SECURITY FIX: Insert new records with parameterized query
                        For Each plateNo As String In validPlateNumbers
                            Dim insertQuery As String = "INSERT INTO void_alert_settings (plateno,emaillist,mobileno,updateddatetime) VALUES (@plateno,@email,@mob,@datetime)"
                            Using insertCmd As SqlCommand = SecurityHelper.CreateSafeCommand(insertQuery, conn)
                                insertCmd.Transaction = transaction
                                insertCmd.Parameters.AddWithValue("@plateno", plateNo)
                                insertCmd.Parameters.AddWithValue("@email", email)
                                insertCmd.Parameters.AddWithValue("@mob", mob)
                                insertCmd.Parameters.AddWithValue("@datetime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                                res = insertCmd.ExecuteNonQuery().ToString()
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
            SecurityHelper.LogError("MultiInsert error", ex, Server)
            Response.Write("Error occurred")
        End Try
    End Sub

    Private Sub MultiInsertALL()
        Try
            ' SECURITY FIX: Validate input
            Dim email As String = SecurityHelper.SanitizeForHtml(Request.QueryString("eml"))
            Dim mob As String = SecurityHelper.SanitizeForHtml(Request.QueryString("mob"))

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY FIX: Use parameterized query to get plate numbers
                Dim query As String = "SELECT plateno FROM vehicleTBL WHERE userid IN (SELECT userid FROM userTBL WHERE role='User')"
                Dim plateNumbers As New List(Of String)

                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            If Not IsDBNull(dr("plateno")) Then
                                Dim plateNo As String = dr("plateno").ToString().Trim()
                                If SecurityHelper.ValidatePlateNumber(plateNo) Then
                                    plateNumbers.Add(plateNo)
                                End If
                            End If
                        End While
                    End Using
                End Using

                If plateNumbers.Count > 0 Then
                    Using transaction As SqlTransaction = conn.BeginTransaction()
                        Try
                            ' SECURITY FIX: Clear existing records safely
                            Dim deleteQuery As String = "DELETE FROM void_alert_settings"
                            Using deleteCmd As SqlCommand = SecurityHelper.CreateSafeCommand(deleteQuery, conn)
                                deleteCmd.Transaction = transaction
                                deleteCmd.ExecuteNonQuery()
                            End Using

                            ' SECURITY FIX: Insert new records with parameterized queries
                            For Each plateNo As String In plateNumbers
                                Dim insertQuery As String = "INSERT INTO void_alert_settings (plateno,emaillist,mobileno,updateddatetime) VALUES (@plateno,@email,@mob,@datetime)"
                                Using insertCmd As SqlCommand = SecurityHelper.CreateSafeCommand(insertQuery, conn)
                                    insertCmd.Transaction = transaction
                                    insertCmd.Parameters.AddWithValue("@plateno", plateNo)
                                    insertCmd.Parameters.AddWithValue("@email", email)
                                    insertCmd.Parameters.AddWithValue("@mob", mob)
                                    insertCmd.Parameters.AddWithValue("@datetime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                                    res = insertCmd.ExecuteNonQuery().ToString()
                                End Using
                            Next

                            transaction.Commit()
                        Catch ex As Exception
                            transaction.Rollback()
                            Throw
                        End Try
                    End Using
                End If
            End Using

            Response.ContentType = "text/plain"
            Response.Write(res)

        Catch ex As Exception
            SecurityHelper.LogError("MultiInsertALL error", ex, Server)
            Response.Write("Error occurred")
        End Try
    End Sub

    Private Sub UpdateData()
        Try
            ' SECURITY FIX: Validate and sanitize input
            Dim geoid As String = SecurityHelper.SanitizeForHtml(Request.QueryString("geoid"))
            Dim email As String = SecurityHelper.SanitizeForHtml(Request.QueryString("eml"))
            Dim mob As String = SecurityHelper.SanitizeForHtml(Request.QueryString("mob"))

            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateNumeric(geoid, 1, Integer.MaxValue) Then
                Response.Write("Invalid ID")
                Return
            End If

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY FIX: Use parameterized query
                Dim query As String = "UPDATE void_alert_settings SET emaillist=@email, mobileno=@mob, updateddatetime=@datetime WHERE id=@id"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@email", email)
                    cmd.Parameters.AddWithValue("@mob", mob)
                    cmd.Parameters.AddWithValue("@datetime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(geoid))

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
                            Dim query As String = "DELETE FROM void_alert_settings WHERE id = @id"
                            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                                cmd.Transaction = transaction
                                cmd.Parameters.AddWithValue("@id", validId)
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

End Class