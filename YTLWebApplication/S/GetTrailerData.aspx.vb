Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class GetTrailerData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
                Return
            End If
            
            Dim opr As String = Request.QueryString("opr")
            
            ' SECURITY FIX: Validate operation parameter
            If Not String.IsNullOrEmpty(opr) AndAlso Not SecurityHelper.ValidateInput(opr, "^[0-9]$") Then
                Response.Write("{""aaData"": []}")
                Return
            End If
            
            ' If no operation specified, default to get trailer data
            If String.IsNullOrEmpty(opr) Then
                GetTrailerData()
                Return
            End If
            
            Select Case opr
                Case "0", "1"
                    GetTrailerData()
                Case "2"
                    AddTrailer()
                Case "3"
                    UpdateTrailer()
                Case "4"
                    DeleteTrailer()
            End Select
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("Page_Load error", ex, Server)
            Response.Write("{""aaData"": []}")
        End Try
    End Sub
    
    Private Sub GetTrailerData()
        Dim aa As New ArrayList()
        Dim conn As SqlConnection = Nothing
        
        Try
            Dim u As String = Request.QueryString("u")
            Dim role As String = Request.QueryString("role")
            Dim userslist As String = Request.QueryString("userslist")
            
            ' SECURITY FIX: Validate input parameters
            If Not String.IsNullOrEmpty(u) AndAlso Not SecurityHelper.ValidateInput(u, "^[0-9]+$|^ALL USERS$|^SELECT USERNAME$") Then
                Response.Write("{""aaData"": []}")
                Return
            End If
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            Dim query As String = "SELECT t.id, t.trailerid, t.trailerno, t.timestamp, u.username, u.userid FROM trailer2 t LEFT JOIN userTBL u ON t.userid=u.userid"
            Dim whereClause As String = ""
            
            ' Build query based on user role and selection
            If role = "User" Then
                whereClause = " WHERE t.userid=@userid"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If u = "ALL USERS" Then
                    whereClause = " WHERE t.userid IN (" & userslist & ")"
                ElseIf u <> "SELECT USERNAME" AndAlso Not String.IsNullOrEmpty(u) Then
                    whereClause = " WHERE t.userid=@userid"
                Else
                    whereClause = " WHERE t.userid IN (" & userslist & ")"
                End If
            Else ' Admin
                If u = "ALL USERS" Then
                    ' No where clause needed - show all
                ElseIf u <> "SELECT USERNAME" AndAlso Not String.IsNullOrEmpty(u) Then
                    whereClause = " WHERE t.userid=@userid"
                End If
            End If
            
            query += whereClause + " ORDER BY t.timestamp DESC"
            
            Dim cmd As New SqlCommand(query, conn)
            
            ' Add parameters if needed
            If whereClause.Contains("@userid") Then
                cmd.Parameters.AddWithValue("@userid", u)
            End If
            
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Dim i As Integer = 1
            
            While dr.Read()
                Dim a As New ArrayList()
                a.Add(dr("id").ToString())
                a.Add(i.ToString())
                a.Add(HttpUtility.HtmlEncode(dr("trailerid").ToString()))
                a.Add(HttpUtility.HtmlEncode(dr("trailerno").ToString()))
                a.Add(Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd HH:mm:ss"))
                a.Add(HttpUtility.HtmlEncode(dr("username").ToString()))
                a.Add(dr("userid").ToString())
                aa.Add(a)
                i += 1
            End While
            
            Dim result As New Dictionary(Of String, Object)()
            result.Add("aaData", aa)
            
            Dim json As String = JsonConvert.SerializeObject(result, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("GetTrailerData error", ex, Server)
            Response.Write("{""aaData"": []}")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    
    Private Sub AddTrailer()
        Dim conn As SqlConnection = Nothing
        Dim result As New Dictionary(Of String, Object)()
        
        Try
            ' SECURITY FIX: Validate input parameters
            Dim u As String = Request.QueryString("u")
            Dim trailerid As String = Request.QueryString("trailerid")
            Dim trailerno As String = Request.QueryString("trailerno")
            
            If String.IsNullOrEmpty(u) OrElse String.IsNullOrEmpty(trailerid) OrElse String.IsNullOrEmpty(trailerno) Then
                result.Add("result", 0)
                Response.Write(JsonConvert.SerializeObject(result))
                Return
            End If
            
            ' SECURITY FIX: Validate input format
            If Not SecurityHelper.ValidateInput(u, "^[0-9]+$") OrElse
               Not SecurityHelper.ValidateInput(trailerid, "^[A-Za-z0-9\-_]{1,50}$") OrElse
               Not SecurityHelper.ValidateInput(trailerno, "^[A-Za-z0-9\-_]{1,50}$") Then
                result.Add("result", 0)
                Response.Write(JsonConvert.SerializeObject(result))
                Return
            End If
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Check if trailer ID already exists
            Dim checkCmd As New SqlCommand("SELECT COUNT(*) FROM trailer2 WHERE trailerid=@trailerid", conn)
            checkCmd.Parameters.AddWithValue("@trailerid", trailerid)
            
            conn.Open()
            Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
            
            If count > 0 Then
                result.Add("result", 2) ' Trailer ID already exists
                Response.Write(JsonConvert.SerializeObject(result))
                Return
            End If
            
            ' SECURITY FIX: Use parameterized query for insert
            Dim cmd As New SqlCommand("INSERT INTO trailer2 (trailerid, trailerno, userid, timestamp) VALUES (@trailerid, @trailerno, @userid, @timestamp)", conn)
            cmd.Parameters.AddWithValue("@trailerid", trailerid)
            cmd.Parameters.AddWithValue("@trailerno", trailerno)
            cmd.Parameters.AddWithValue("@userid", u)
            cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            
            Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
            result.Add("result", rowsAffected)
            
            Response.Write(JsonConvert.SerializeObject(result))
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("AddTrailer error", ex, Server)
            result.Add("result", 0)
            Response.Write(JsonConvert.SerializeObject(result))
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    
    Private Sub UpdateTrailer()
        Dim conn As SqlConnection = Nothing
        Dim result As New Dictionary(Of String, Object)()
        
        Try
            ' SECURITY FIX: Validate input parameters
            Dim tid As String = Request.QueryString("tid")
            Dim u As String = Request.QueryString("u")
            Dim trailerid As String = Request.QueryString("trailerid")
            Dim trailerno As String = Request.QueryString("trailerno")
            
            If String.IsNullOrEmpty(tid) OrElse String.IsNullOrEmpty(u) OrElse 
               String.IsNullOrEmpty(trailerid) OrElse String.IsNullOrEmpty(trailerno) Then
                result.Add("result", 0)
                Response.Write(JsonConvert.SerializeObject(result))
                Return
            End If
            
            ' SECURITY FIX: Validate input format
            If Not SecurityHelper.ValidateInput(tid, "^[0-9]+$") OrElse
               Not SecurityHelper.ValidateInput(u, "^[0-9]+$") OrElse
               Not SecurityHelper.ValidateInput(trailerid, "^[A-Za-z0-9\-_]{1,50}$") OrElse
               Not SecurityHelper.ValidateInput(trailerno, "^[A-Za-z0-9\-_]{1,50}$") Then
                result.Add("result", 0)
                Response.Write(JsonConvert.SerializeObject(result))
                Return
            End If
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Check if trailer ID already exists (except for this record)
            Dim checkCmd As New SqlCommand("SELECT COUNT(*) FROM trailer2 WHERE trailerid=@trailerid AND id<>@id", conn)
            checkCmd.Parameters.AddWithValue("@trailerid", trailerid)
            checkCmd.Parameters.AddWithValue("@id", tid)
            
            conn.Open()
            Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
            
            If count > 0 Then
                result.Add("result", 2) ' Trailer ID already exists
                Response.Write(JsonConvert.SerializeObject(result))
                Return
            End If
            
            ' SECURITY FIX: Use parameterized query for update
            Dim cmd As New SqlCommand("UPDATE trailer2 SET trailerid=@trailerid, trailerno=@trailerno, userid=@userid, timestamp=@timestamp WHERE id=@id", conn)
            cmd.Parameters.AddWithValue("@trailerid", trailerid)
            cmd.Parameters.AddWithValue("@trailerno", trailerno)
            cmd.Parameters.AddWithValue("@userid", u)
            cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@id", tid)
            
            Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
            result.Add("result", rowsAffected)
            
            Response.Write(JsonConvert.SerializeObject(result))
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("UpdateTrailer error", ex, Server)
            result.Add("result", 0)
            Response.Write(JsonConvert.SerializeObject(result))
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    
    Private Sub DeleteTrailer()
        Dim conn As SqlConnection = Nothing
        Dim result As New Dictionary(Of String, Object)()
        
        Try
            ' SECURITY FIX: Validate input parameters
            Dim ugData As String = Request.QueryString("ugData")
            
            If String.IsNullOrEmpty(ugData) Then
                result.Add("result", 0)
                Response.Write(JsonConvert.SerializeObject(result))
                Return
            End If
            
            ' Parse comma-separated list of IDs
            Dim ids As String() = ugData.Split(New Char() {","c})
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            conn.Open()
            
            Dim totalRowsAffected As Integer = 0
            
            For Each id As String In ids
                ' SECURITY FIX: Validate ID format
                If Not SecurityHelper.ValidateInput(id, "^[0-9]+$") Then
                    Continue For
                End If
                
                ' SECURITY FIX: Use parameterized query
                Dim cmd As New SqlCommand("DELETE FROM trailer2 WHERE id=@id", conn)
                cmd.Parameters.AddWithValue("@id", id)
                
                totalRowsAffected += cmd.ExecuteNonQuery()
            Next
            
            result.Add("result", totalRowsAffected)
            Response.Write(JsonConvert.SerializeObject(result))
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("DeleteTrailer error", ex, Server)
            result.Add("result", 0)
            Response.Write(JsonConvert.SerializeObject(result))
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
End Class