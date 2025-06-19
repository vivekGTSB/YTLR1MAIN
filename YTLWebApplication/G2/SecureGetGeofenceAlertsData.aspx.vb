Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class SecureGetGeofenceAlertsData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY: Validate session
            If Not G2SecurityHelper.ValidateSession() Then
                Response.StatusCode = 401
                Response.Write("Unauthorized")
                Response.End()
                Return
            End If
            
            ' SECURITY: Check authorization
            If Not G2SecurityHelper.HasRequiredRole("OPERATOR") Then
                Response.StatusCode = 403
                Response.Write("Forbidden")
                Response.End()
                Return
            End If
            
            ' SECURITY: Rate limiting
            Dim clientIP As String = Request.UserHostAddress
            If Not G2SecurityHelper.CheckRateLimit("GeofenceAlerts_" & clientIP, 30, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.Write("Too Many Requests")
                Response.End()
                Return
            End If
            
            ' SECURITY: Validate operation parameter
            Dim oper As String = Request.QueryString("opr")
            If Not G2SecurityHelper.ValidateG2Input(oper, G2InputType.AlphaNumeric, 1) Then
                Response.StatusCode = 400
                Response.Write("Invalid operation")
                Response.End()
                Return
            End If

            Select Case oper.ToUpper()
                Case "0"
                    GetSecureData()
                Case "1"
                    InsertSecureData()
                Case "2"
                    UpdateSecureData()
                Case "3"
                    DeleteSecureData()
                Case Else
                    Response.StatusCode = 400
                    Response.Write("Invalid operation")
                    Response.End()
            End Select

        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("GEOFENCE_ALERTS_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Internal Server Error")
        End Try
    End Sub

    Private Sub GetSecureData()
        Try
            Dim aa As New ArrayList()
            
            ' SECURITY: Use parameterized query
            Dim sqlstr As String = "SELECT gt.geofenceid, at.id, at.emaillist, at.mobileno, gt.geofencename FROM (SELECT geofencename, geofenceid FROM geofence WHERE accesstype = @accessType) as gt LEFT OUTER JOIN lafarge_private_geofence_alert_settings at ON at.geofenceid = gt.geofenceid"
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(sqlstr, conn)
                    cmd.Parameters.AddWithValue("@accessType", 2)
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim c As Integer = 0
                        While dr.Read()
                            c += 1
                            Dim a As New ArrayList()

                            ' SECURITY: Sanitize all output data
                            a.Add(If(IsDBNull(dr("id")), "0", G2SecurityHelper.SanitizeForHtml(dr("id").ToString())))
                            a.Add(c)
                            a.Add(G2SecurityHelper.SanitizeForHtml(dr("geofencename").ToString()))
                            a.Add(If(IsDBNull(dr("emaillist")), "", G2SecurityHelper.SanitizeForHtml(dr("emaillist").ToString())))
                            a.Add(If(IsDBNull(dr("mobileno")), "", G2SecurityHelper.SanitizeForHtml(dr("mobileno").ToString())))
                            a.Add(G2SecurityHelper.SanitizeForHtml(dr("geofenceid").ToString()))
                            a.Add(If(IsDBNull(dr("id")), "0", G2SecurityHelper.SanitizeForHtml(dr("id").ToString())))
                            
                            aa.Add(a)
                        End While
                    End Using
                End Using
            End Using
            
            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("GET_SECURE_DATA_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Data retrieval failed")
        End Try
    End Sub

    Private Sub InsertSecureData()
        Dim res As String = "0"
        
        Try
            ' SECURITY: Validate input parameters
            Dim geoid As String = Request.QueryString("geoid")
            Dim email As String = Request.QueryString("eml")
            Dim mob As String = Request.QueryString("mob")
            
            If Not G2SecurityHelper.ValidateG2Input(geoid, G2InputType.GeofenceId) Then
                Response.Write("Invalid geofence ID")
                Return
            End If
            
            If Not String.IsNullOrEmpty(email) AndAlso Not G2SecurityHelper.ValidateG2Input(email, G2InputType.Email) Then
                Response.Write("Invalid email format")
                Return
            End If
            
            If Not String.IsNullOrEmpty(mob) AndAlso Not G2SecurityHelper.ValidateG2Input(mob, G2InputType.PhoneNumber) Then
                Response.Write("Invalid phone number format")
                Return
            End If
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = "INSERT INTO lafarge_private_geofence_alert_settings (geofenceid, emaillist, mobileno, updateddatetime) VALUES (@geofenceid, @emaillist, @mobileno, @updateddatetime)"
                Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, conn)
                    cmd.Parameters.AddWithValue("@geofenceid", geoid)
                    cmd.Parameters.AddWithValue("@emaillist", If(String.IsNullOrEmpty(email), DBNull.Value, email))
                    cmd.Parameters.AddWithValue("@mobileno", If(String.IsNullOrEmpty(mob), DBNull.Value, mob))
                    cmd.Parameters.AddWithValue("@updateddatetime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                    
                    conn.Open()
                    res = cmd.ExecuteNonQuery().ToString()
                End Using
            End Using
            
            G2SecurityHelper.LogSecurityEvent("GEOFENCE_ALERT_INSERT", $"GeofenceId: {geoid}, Result: {res}")
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("INSERT_SECURE_DATA_ERROR", ex.Message)
            res = "Error occurred"
        End Try

        Response.ContentType = "text/plain"
        Response.Write(res)
    End Sub

    Private Sub UpdateSecureData()
        Dim res As String = "0"
        
        Try
            ' SECURITY: Validate input parameters
            Dim geoid As String = Request.QueryString("geoid")
            Dim email As String = Request.QueryString("eml")
            Dim mob As String = Request.QueryString("mob")
            
            If Not G2SecurityHelper.ValidateG2Input(geoid, G2InputType.GeofenceId) Then
                Response.Write("Invalid ID")
                Return
            End If
            
            If Not String.IsNullOrEmpty(email) AndAlso Not G2SecurityHelper.ValidateG2Input(email, G2InputType.Email) Then
                Response.Write("Invalid email format")
                Return
            End If
            
            If Not String.IsNullOrEmpty(mob) AndAlso Not G2SecurityHelper.ValidateG2Input(mob, G2InputType.PhoneNumber) Then
                Response.Write("Invalid phone number format")
                Return
            End If
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = "UPDATE lafarge_private_geofence_alert_settings SET emaillist = @emaillist, mobileno = @mobileno, updateddatetime = @updateddatetime WHERE id = @id"
                Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, conn)
                    cmd.Parameters.AddWithValue("@emaillist", If(String.IsNullOrEmpty(email), DBNull.Value, email))
                    cmd.Parameters.AddWithValue("@mobileno", If(String.IsNullOrEmpty(mob), DBNull.Value, mob))
                    cmd.Parameters.AddWithValue("@updateddatetime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@id", geoid)
                    
                    conn.Open()
                    res = cmd.ExecuteNonQuery().ToString()
                End Using
            End Using
            
            G2SecurityHelper.LogSecurityEvent("GEOFENCE_ALERT_UPDATE", $"Id: {geoid}, Result: {res}")
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("UPDATE_SECURE_DATA_ERROR", ex.Message)
            res = "Error occurred"
        End Try

        Response.ContentType = "text/plain"
        Response.Write(res)
    End Sub

    Private Sub DeleteSecureData()
        Try
            Dim chekitems As String = Request.QueryString("geoid")
            If String.IsNullOrEmpty(chekitems) Then
                Response.Write("0")
                Return
            End If
            
            ' SECURITY: Validate and sanitize IDs
            Dim ids As String() = chekitems.Split(","c)
            Dim validIds As New List(Of String)
            
            For Each id As String In ids
                If G2SecurityHelper.ValidateG2Input(id.Trim(), G2InputType.GeofenceId) Then
                    validIds.Add(id.Trim())
                End If
            Next
            
            If validIds.Count = 0 Then
                Response.Write("0")
                Return
            End If
            
            Dim result As Integer = 0
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                conn.Open()
                
                For Each validId As String In validIds
                    Try
                        Dim query As String = "DELETE FROM lafarge_private_geofence_alert_settings WHERE id = @id"
                        Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, conn)
                            cmd.Parameters.AddWithValue("@id", validId)
                            Dim deleteResult As Integer = cmd.ExecuteNonQuery()
                            If deleteResult > 0 Then
                                result += deleteResult
                            End If
                        End Using
                    Catch ex As Exception
                        G2SecurityHelper.LogSecurityEvent("DELETE_INDIVIDUAL_RECORD_ERROR", $"Id: {validId}, Error: {ex.Message}")
                    End Try
                Next
            End Using
            
            G2SecurityHelper.LogSecurityEvent("GEOFENCE_ALERT_DELETE", $"Count: {validIds.Count}, Result: {result}")
            Response.ContentType = "text/plain"
            Response.Write(If(result > 0, "1", "0"))
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("DELETE_SECURE_DATA_ERROR", ex.Message)
            Response.Write("0")
        End Try
    End Sub

End Class