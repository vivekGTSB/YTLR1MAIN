Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class GetSoldToManagement
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
                Return
            End If
            
            Dim opr As String = Request.QueryString("op")
            
            ' SECURITY FIX: Validate operation parameter
            If Not SecurityHelper.ValidateInput(opr, "^[0-9]$") Then
                Response.Write("[]")
                Return
            End If
            
            Select Case opr
                Case "0"
                    GetSoldToData()
                Case "1"
                    AddOrUpdateSoldTo()
                Case "2"
                    DeleteSoldTo()
            End Select
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("Page_Load error", ex, Server)
            Response.Write("[]")
        End Try
    End Sub
    
    Private Sub GetSoldToData()
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Dim conn As SqlConnection = Nothing
        
        Try
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("SELECT id, customername, customerid, timestamp FROM oss_soldto_customer ORDER BY customername", conn)
            
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Dim i As Integer = 1
            
            While dr.Read()
                a = New ArrayList()
                a.Add(dr("id").ToString())
                a.Add(i)
                a.Add(dr("customername").ToString())
                a.Add(dr("customerid").ToString())
                a.Add(dr("timestamp").ToString())
                aa.Add(a)
                i += 1
            End While
            
            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("GetSoldToData error", ex, Server)
            Response.Write("[]")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    
    Private Sub AddOrUpdateSoldTo()
        Dim conn As SqlConnection = Nothing
        
        Try
            ' SECURITY FIX: Validate input parameters
            Dim pid As String = Request.QueryString("pid")
            Dim pname As String = Request.QueryString("pname")
            Dim tid As String = Request.QueryString("tid")
            Dim prevpid As String = Request.QueryString("prevpid")
            
            If String.IsNullOrEmpty(pid) OrElse String.IsNullOrEmpty(pname) Then
                Response.Write("0")
                Return
            End If
            
            ' SECURITY FIX: Validate input format
            If Not SecurityHelper.ValidateInput(pid, "^[A-Za-z0-9\-_]{1,50}$") OrElse
               Not SecurityHelper.ValidateInput(pname, "^[A-Za-z0-9\s\-_]{1,100}$") Then
                Response.Write("0")
                Return
            End If
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            conn.Open()
            
            ' Check if operation is add (0) or update (1)
            If Request.QueryString("opr") = "0" Then
                ' SECURITY FIX: Check if customer ID already exists
                Dim checkCmd As New SqlCommand("SELECT COUNT(*) FROM oss_soldto_customer WHERE customerid=@customerid", conn)
                checkCmd.Parameters.AddWithValue("@customerid", pid)
                
                Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
                
                If count > 0 Then
                    Response.Write("2") ' Customer ID already exists
                    Return
                End If
                
                ' SECURITY FIX: Use parameterized query for insert
                Dim cmd As New SqlCommand("INSERT INTO oss_soldto_customer (customerid, customername, timestamp) VALUES (@customerid, @customername, @timestamp)", conn)
                cmd.Parameters.AddWithValue("@customerid", pid)
                cmd.Parameters.AddWithValue("@customername", pname)
                cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                
                Dim result As Integer = cmd.ExecuteNonQuery()
                Response.Write(result.ToString())
                
            Else ' Update operation
                ' SECURITY FIX: Use parameterized query for update
                Dim cmd As New SqlCommand("UPDATE oss_soldto_customer SET customerid=@customerid, customername=@customername, timestamp=@timestamp WHERE id=@id", conn)
                cmd.Parameters.AddWithValue("@customerid", pid)
                cmd.Parameters.AddWithValue("@customername", pname)
                cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@id", prevpid)
                
                Dim result As Integer = cmd.ExecuteNonQuery()
                Response.Write(result.ToString())
            End If
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("AddOrUpdateSoldTo error", ex, Server)
            Response.Write("0")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    
    Private Sub DeleteSoldTo()
        Dim conn As SqlConnection = Nothing
        
        Try
            ' SECURITY FIX: Validate input parameters
            Dim ugData As String = Request.QueryString("ugData")
            
            If String.IsNullOrEmpty(ugData) Then
                Response.Write("0")
                Return
            End If
            
            ' Parse JSON array of IDs
            Dim ids As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(ugData)
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            conn.Open()
            
            Dim result As Integer = 0
            
            For Each id As String In ids
                ' SECURITY FIX: Validate ID format
                If Not SecurityHelper.ValidateInput(id, "^[0-9]+$") Then
                    Continue For
                End If
                
                ' SECURITY FIX: Use parameterized query
                Dim cmd As New SqlCommand("DELETE FROM oss_soldto_customer WHERE id=@id", conn)
                cmd.Parameters.AddWithValue("@id", id)
                
                result += cmd.ExecuteNonQuery()
            Next
            
            Response.Write(If(result > 0, "1", "0"))
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("DeleteSoldTo error", ex, Server)
            Response.Write("0")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
End Class