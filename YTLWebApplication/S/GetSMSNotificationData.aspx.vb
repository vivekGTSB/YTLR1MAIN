Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class GetSMSNotificationData
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
            If Not SecurityHelper.ValidateInput(opr, "^[0-4]$") Then
                Response.Write("[]")
                Return
            End If
            
            Select Case opr
                Case "0"
                    GetNotificationData()
                Case "1"
                    AddNotification()
                Case "2"
                    UpdateNotification()
                Case "3"
                    DeleteNotification()
                Case "4"
                    GetShipToCodes()
            End Select
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("Page_Load error", ex, Server)
            Response.Write("[]")
        End Try
    End Sub
    
    Private Sub GetNotificationData()
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Dim conn As SqlConnection = Nothing
        
        Try
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("SELECT n.id, n.shiptocode, s.name, n.mobilelist, n.otpflag FROM oss_sms_notification n LEFT JOIN oss_ship_to_code s ON n.shiptocode = s.shiptocode", conn)
            
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Dim i As Integer = 1
            
            While dr.Read()
                a = New ArrayList()
                a.Add(dr("id").ToString())
                a.Add(i)
                a.Add(dr("shiptocode").ToString())
                a.Add(dr("name").ToString())
                a.Add(dr("mobilelist").ToString())
                a.Add(dr("otpflag").ToString())
                aa.Add(a)
                i += 1
            End While
            
            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("GetNotificationData error", ex, Server)
            Response.Write("[]")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    
    Private Sub AddNotification()
        Dim conn As SqlConnection = Nothing
        
        Try
            ' SECURITY FIX: Validate input parameters
            Dim scode As String = Request.QueryString("scode")
            Dim chkotp As String = Request.QueryString("chkotp")
            Dim mobile1 As String = Request.QueryString("mobile1")
            Dim mobile2 As String = Request.QueryString("mobile2")
            Dim mobile3 As String = Request.QueryString("mobile3")
            Dim mobile4 As String = Request.QueryString("mobile4")
            Dim mobile5 As String = Request.QueryString("mobile5")
            
            If Not SecurityHelper.ValidateInput(scode, "^[A-Za-z0-9\-_]{1,50}$") Then
                Response.Write("0")
                Return
            End If
            
            ' Build mobile list
            Dim mobileList As String = mobile1
            If Not String.IsNullOrEmpty(mobile2) Then mobileList &= "," & mobile2
            If Not String.IsNullOrEmpty(mobile3) Then mobileList &= "," & mobile3
            If Not String.IsNullOrEmpty(mobile4) Then mobileList &= "," & mobile4
            If Not String.IsNullOrEmpty(mobile5) Then mobileList &= "," & mobile5
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("INSERT INTO oss_sms_notification (shiptocode, mobilelist, otpflag) VALUES (@shiptocode, @mobilelist, @otpflag)", conn)
            cmd.Parameters.AddWithValue("@shiptocode", scode)
            cmd.Parameters.AddWithValue("@mobilelist", mobileList)
            cmd.Parameters.AddWithValue("@otpflag", chkotp)
            
            conn.Open()
            Dim result As Integer = cmd.ExecuteNonQuery()
            
            Response.Write(result.ToString())
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("AddNotification error", ex, Server)
            Response.Write("0")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    
    Private Sub UpdateNotification()
        Dim conn As SqlConnection = Nothing
        
        Try
            ' SECURITY FIX: Validate input parameters
            Dim scode As String = Request.QueryString("scode")
            Dim chkotp As String = Request.QueryString("chkotp")
            Dim mobile1 As String = Request.QueryString("mobile1")
            Dim mobile2 As String = Request.QueryString("mobile2")
            Dim mobile3 As String = Request.QueryString("mobile3")
            Dim mobile4 As String = Request.QueryString("mobile4")
            Dim mobile5 As String = Request.QueryString("mobile5")
            
            If Not SecurityHelper.ValidateInput(scode, "^[A-Za-z0-9\-_]{1,50}$") Then
                Response.Write("0")
                Return
            End If
            
            ' Build mobile list
            Dim mobileList As String = mobile1
            If Not String.IsNullOrEmpty(mobile2) Then mobileList &= "," & mobile2
            If Not String.IsNullOrEmpty(mobile3) Then mobileList &= "," & mobile3
            If Not String.IsNullOrEmpty(mobile4) Then mobileList &= "," & mobile4
            If Not String.IsNullOrEmpty(mobile5) Then mobileList &= "," & mobile5
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("UPDATE oss_sms_notification SET mobilelist=@mobilelist, otpflag=@otpflag WHERE shiptocode=@shiptocode", conn)
            cmd.Parameters.AddWithValue("@shiptocode", scode)
            cmd.Parameters.AddWithValue("@mobilelist", mobileList)
            cmd.Parameters.AddWithValue("@otpflag", chkotp)
            
            conn.Open()
            Dim result As Integer = cmd.ExecuteNonQuery()
            
            Response.Write(result.ToString())
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("UpdateNotification error", ex, Server)
            Response.Write("0")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    
    Private Sub DeleteNotification()
        Dim conn As SqlConnection = Nothing
        
        Try
            ' SECURITY FIX: Validate input parameters
            Dim geoid As String = Request.QueryString("geoid")
            
            If String.IsNullOrEmpty(geoid) Then
                Response.Write("0")
                Return
            End If
            
            ' Parse JSON array of IDs
            Dim ids As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(geoid)
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            conn.Open()
            
            Dim result As Integer = 0
            
            For Each id As String In ids
                ' SECURITY FIX: Validate ID format
                If Not SecurityHelper.ValidateInput(id, "^[0-9]+$") Then
                    Continue For
                End If
                
                ' SECURITY FIX: Use parameterized query
                Dim cmd As New SqlCommand("DELETE FROM oss_sms_notification WHERE id=@id", conn)
                cmd.Parameters.AddWithValue("@id", id)
                
                result += cmd.ExecuteNonQuery()
            Next
            
            Response.Write(If(result > 0, "1", "0"))
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("DeleteNotification error", ex, Server)
            Response.Write("0")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    
    Private Sub GetShipToCodes()
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Dim conn As SqlConnection = Nothing
        
        Try
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("SELECT name, shiptocode FROM oss_ship_to_code ORDER BY name", conn)
            
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            
            While dr.Read()
                a = New ArrayList()
                a.Add(dr("name").ToString() & " (" & dr("shiptocode").ToString() & ")")
                a.Add(dr("shiptocode").ToString())
                aa.Add(a)
            End While
            
            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("GetShipToCodes error", ex, Server)
            Response.Write("[]")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
End Class