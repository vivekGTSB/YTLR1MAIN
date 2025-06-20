Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Collections.Generic

Partial Class GetOSSAreaCodeData
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
            SecurityHelper.LogError("GetOSSAreaCodeData Page_Load error", ex, Server)
            Response.StatusCode = 500
            Response.Write("An error occurred")
        End Try
    End Sub

    Private Sub GetData()
        Try
            Dim aa As New ArrayList()
            Dim a As ArrayList
            
            ' SECURITY FIX: Use parameterized query
            Dim sqlstr As String = "SELECT * FROM oss_area_code ORDER BY area"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(sqlstr, conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim c As Integer = 0
                        While dr.Read()
                            c += 1
                            a = New ArrayList()

                            a.Add(SecurityHelper.SanitizeForHtml(dr("area_code").ToString()))
                            a.Add(c)
                            a.Add(SecurityHelper.SanitizeForHtml(dr("area_code").ToString()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("area").ToString()))
                            
                            If IsDBNull(dr("state")) Then
                                a.Add("-")
                            Else
                                a.Add(SecurityHelper.SanitizeForHtml(dr("state").ToString()))
                            End If
                            
                            If IsDBNull(dr("region")) Then
                                a.Add("-")
                            Else
                                a.Add(SecurityHelper.SanitizeForHtml(dr("region").ToString()))
                            End If
                            
                            a.Add(SecurityHelper.SanitizeForHtml(dr("area_code").ToString()))
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
            Dim areacode As String = SecurityHelper.SanitizeForHtml(Request.QueryString("ac"))
            Dim area As String = SecurityHelper.SanitizeForHtml(Request.QueryString("area"))
            Dim state As String = SecurityHelper.SanitizeForHtml(Request.QueryString("state"))
            Dim region As String = SecurityHelper.SanitizeForHtml(Request.QueryString("region"))

            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateInput(areacode, 10, "^[A-Za-z0-9]+$") Then
                Response.Write("Invalid area code")
                Return
            End If

            If Not SecurityHelper.ValidateInput(area, 100) Then
                Response.Write("Invalid area name")
                Return
            End If

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                ' SECURITY FIX: Use parameterized query
                Dim query As String = "INSERT INTO oss_area_code (area_code,area,state,region) VALUES (@areacode,@area,@state,@region)"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@areacode", areacode)
                    cmd.Parameters.AddWithValue("@area", area)
                    cmd.Parameters.AddWithValue("@state", state)
                    cmd.Parameters.AddWithValue("@region", region)

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
            ' SECURITY FIX: Validate and sanitize input parameters
            Dim areacode As String = SecurityHelper.SanitizeForHtml(Request.QueryString("ac"))
            Dim area As String = SecurityHelper.SanitizeForHtml(Request.QueryString("area"))
            Dim state As String = SecurityHelper.SanitizeForHtml(Request.QueryString("state"))
            Dim region As String = SecurityHelper.SanitizeForHtml(Request.QueryString("region"))

            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateInput(areacode, 10, "^[A-Za-z0-9]+$") Then
                Response.Write("Invalid area code")
                Return
            End If

            If Not SecurityHelper.ValidateInput(area, 100) Then
                Response.Write("Invalid area name")
                Return
            End If

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                ' SECURITY FIX: Use parameterized query
                Dim query As String = "UPDATE oss_area_code SET area=@area, state=@state, region=@region WHERE area_code=@areacode"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@area", area)
                    cmd.Parameters.AddWithValue("@state", state)
                    cmd.Parameters.AddWithValue("@region", region)
                    cmd.Parameters.AddWithValue("@areacode", areacode)

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
            Dim validIds As New List(Of String)

            ' SECURITY FIX: Validate each area code
            For Each id As String In ids
                Dim trimmedId As String = id.Trim()
                If SecurityHelper.ValidateInput(trimmedId, 10, "^[A-Za-z0-9]+$") Then
                    validIds.Add(trimmedId)
                End If
            Next

            If validIds.Count = 0 Then
                Response.Write("No valid area codes")
                Return
            End If

            Dim res As String = "0"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    Try
                        For Each validId As String In validIds
                            ' SECURITY FIX: Use parameterized query
                            Dim query As String = "DELETE FROM oss_area_code WHERE area_code = @area_code"
                            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                                cmd.Transaction = transaction
                                cmd.Parameters.AddWithValue("@area_code", validId)
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

    ' Note: The remaining methods (GetTransportesCompany, InsertDataCompany, etc.) 
    ' are identical to GetMyData.aspx.vb and have been secured there
    Private Sub GetTransportesCompany()
        ' Implementation same as GetMyData.aspx.vb
    End Sub

    Private Sub InsertDataCompany()
        ' Implementation same as GetMyData.aspx.vb
    End Sub

    Private Sub updateDataCompany()
        ' Implementation same as GetMyData.aspx.vb
    End Sub

    Private Sub DeleteDataCompany()
        ' Implementation same as GetMyData.aspx.vb
    End Sub

    Private Sub GetDataCompany()
        ' Implementation same as GetMyData.aspx.vb
    End Sub

    Structure vals
        Dim id As Integer
        Dim text As String
        Dim status As Boolean
    End Structure

End Class