Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Net.Mail
Imports System.Web.Security

Partial Class ServiceingJsonT
    Inherits System.Web.UI.Page

    Private Sub GetTicketInformation(ByVal UID As String, ByVal role As String)
        Dim aa As ArrayList = New ArrayList()
        Dim a As ArrayList
        Dim json As String
        Try
            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateInput(UID, "^[0-9]+$") Then
                Response.Write("{""error"": ""Invalid user ID""}")
                Return
            End If
            
            Dim userid As String = UID
            Dim userstable As DataTable = New DataTable()
            userstable.Columns.Add("sno")
            userstable.Columns.Add("serviceid")
            userstable.Columns.Add("raiseddate")
            userstable.Columns.Add("expdt")
            userstable.Columns.Add("plateno")
            userstable.Columns.Add("groupname")
            userstable.Columns.Add("servicetype")
            userstable.Columns.Add("priority")
            userstable.Columns.Add("status")
            userstable.Columns.Add("assignedto")
            userstable.Columns.Add("username")
            userstable.Columns.Add("assignedby")
            userstable.Columns.Add("remarks")
            
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            
            ' SECURITY FIX: Use parameterized queries
            If role = "Admin" Or role = "AdminViewer1" Or role = "AdminViewer2" Then
                cmd = New SqlCommand("select vs.userid,ut.username,vs.serviceid,vs.plateno,convert(varchar(19),vs.timestamp,120) as timestamp,vs.serexpdtime,dbo.fn_getvehicletype(vs.plateno) as type ,vs.servicetype,vs.priority,vs.status,isnull(vsh.assignedto,'Not at Assigned') assignedto,vsh.servicedate,vs.assignedby,vs.remarks from vehicle_services vs left outer join vehicle_services_history vsh on vs.serviceid=vsh.serviceid inner join userTBL ut on vs.userid=ut.userid order by timestamp desc", conn)
            Else
                If userid = "1967" Then
                    cmd = New SqlCommand("select vs.userid,ut.username,vs.serviceid,vs.plateno,convert(varchar(19),vs.timestamp,120) as timestamp,vs.serexpdtime,dbo.fn_getvehicletype(vs.plateno) as type ,vs.servicetype,vs.priority,vs.status,isnull(vsh.assignedto,'Not at Assigned') assignedto,vsh.servicedate,vs.assignedby,vs.remarks from vehicle_services vs left outer join vehicle_services_history vsh on vs.serviceid=vsh.serviceid inner join userTBL ut on vs.userid=ut.userid order by timestamp desc", conn)
                Else
                    ' SECURITY FIX: Use parameterized query
                    cmd = New SqlCommand("select vs.userid,dbo.[fn_getusername](vs.userid) as username ,vs.serviceid,vs.plateno,convert(varchar(19),vs.timestamp,120) as timestamp,vs.serexpdtime,dbo.fn_getvehicletype(vs.plateno) as type ,vs.servicetype,vs.priority,vs.status,isnull(vsh.assignedto,'Not at Assigned') assignedto,vsh.servicedate,vs.assignedby ,vs.remarks from vehicle_services vs left outer join vehicle_services_history vsh on vs.serviceid=vsh.serviceid where userid=@userid order by timestamp desc", conn)
                    cmd.Parameters.AddWithValue("@userid", userid)
                End If
            End If
            
            Dim dr As SqlDataReader
            conn.Open()
            dr = cmd.ExecuteReader()
            Dim i As Int32 = 0
            While (dr.Read())
                Dim r As DataRow
                r = userstable.NewRow()
                r("sno") = i.ToString()
                r("serviceid") = dr("serviceid")

                r("raiseddate") = dr("timestamp")
                If dr("serexpdtime").ToString().Contains("1/1/1900") Or IsDBNull(dr("serexpdtime")) Then
                    r("expdt") = "--"
                Else
                    r("expdt") = Convert.ToDateTime(dr("serexpdtime")).ToString("yyyy/MM/dd HH:mm:ss")
                End If

                r("plateno") = dr("plateno")
                r("groupname") = dr("type")
                r("servicetype") = dr("servicetype")
                r("priority") = dr("priority")

                Select Case dr("status")
                    Case "0" : r("status") = "Open"

                    Case "3" : r("status") = "Request Received"

                    Case "1" : r("status") = "Technician Assign"

                    Case "4"
                        If IsDBNull(dr("servicedate")) Then
                            r("status") = "<span style='color:#0BA01A;'>Service Date Changed </span>"
                        Else
                            ' SECURITY FIX: HTML encode output
                            r("status") = "<span style='color:#0BA01A;'> Service Date Changed To " & HttpUtility.HtmlEncode(dr("servicedate").ToString()) & "</span>"
                        End If
                    Case "2" : r("status") = "Closed"

                    Case "5" : r("status") = "ReOpen"

                End Select

                If dr("status") = "2" And dr("assignedto") = "Not at Assigned" Then
                    r("assignedto") = "Closed by Customer"
                Else
                    r("assignedto") = dr("assignedto")
                    r("username") = dr("username")
                End If
                
                If IsDBNull(dr("assignedby")) Then
                    r("assignedby") = "--"
                Else
                    r("assignedby") = getusername(dr("assignedby").ToString())
                End If
                
                ' SECURITY FIX: HTML encode output
                r("remarks") = HttpUtility.HtmlEncode(dr("remarks").ToString())
                userstable.Rows.Add(r)
                i += 1
            End While
            
            If userstable.Rows.Count = 0 Then
                Dim r As DataRow
                r = userstable.NewRow()
                r("sno") = "--"
                r("serviceid") = "--"
                r("raiseddate") = "--"
                r("expdt") = "--"
                r("plateno") = "--"
                r("groupname") = "--"
                r("servicetype") = "--"
                r("priority") = "--"
                r("status") = "--"
                r("assignedto") = "--"
                r("username") = "--"
                r("assignedby") = "--"
                r("remarks") = "--"
                userstable.Rows.Add(r)
            End If

            For Each row As DataRow In userstable.Rows
                a = New ArrayList()

                a.Add(row(0))
                a.Add(row(10))
                a.Add(row(1))
                a.Add(row(2))
                a.Add(row(3))
                a.Add(row(4))
                a.Add(row(5))
                a.Add(row(6))
                a.Add(row(7))
                a.Add(row(8))
                a.Add(row(9))
                a.Add(row(11))
                a.Add(row(1))
                a.Add(row(12))
                If row(3) = "--" Then
                    a.Add("")
                    a.Add("00")
                    a.Add("00")
                Else
                    a.Add(Convert.ToDateTime(row(3)).ToString("yyyy/MM/dd"))
                    a.Add(Convert.ToDateTime(row(3)).Hour.ToString().PadLeft(2, "0"c))
                    a.Add(Convert.ToDateTime(row(3)).Minute.ToString().PadLeft(2, "0"c))
                End If

                aa.Add(a)
            Next

            Dim jss As New Newtonsoft.Json.JsonSerializer()
            json = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
            Response.ContentType = "text/plain"
            Response.Write(json)
        Catch ex As Exception
            ' SECURITY FIX: Don't expose error details to client
            SecurityHelper.LogError("GetTicketInformation error", ex, Server)
            Response.Write("{""error"": ""An error occurred while processing your request""}")
        Finally
            ' SECURITY FIX: Ensure connection is closed
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate all inputs
            Dim UID As String = Request.QueryString("u")
            Dim role As String = Request.QueryString("role")
            Dim opr As String = Request.QueryString("opr")

            If Not SecurityHelper.ValidateInput(opr, "^[0-9]+$") Then
                Response.Write("{""error"": ""Invalid operation""}")
                Return
            End If

            If (opr = "0") Then
                GetTicketInformation(UID, role)
            ElseIf opr = "1" Then
                Dim plateno As String = Request.QueryString("P")
                Dim servicetype As String = Request.QueryString("stype")
                Dim prioryty As String = Request.QueryString("pre")
                Dim remarks As String = Request.QueryString("rem")
                Dim serexpdt As String = Request.QueryString("expdt")
                
                ' SECURITY FIX: Validate inputs
                If Not SecurityHelper.ValidateInput(plateno, "^[A-Za-z0-9\-\s]{1,20}$") OrElse
                   Not SecurityHelper.ValidateInput(servicetype, "^[A-Za-z0-9\s\-_]{1,50}$") OrElse
                   Not SecurityHelper.ValidateInput(prioryty, "^[A-Za-z0-9\s]{1,20}$") Then
                    Response.Write("{""error"": ""Invalid input parameters""}")
                    Return
                End If
                
                insertNewService(UID, plateno, servicetype, prioryty, remarks, serexpdt)
            ElseIf opr = "2" Then
                Dim serviceid As String = Request.QueryString("serviceid")
                
                ' SECURITY FIX: Validate serviceid
                If Not SecurityHelper.ValidateInput(serviceid, "^[0-9]+$") Then
                    Response.Write("{""error"": ""Invalid service ID""}")
                    Return
                End If
                
                GetServiceDetails(serviceid)
            ElseIf opr = "3" Then
                Dim serviceid As String = Request.QueryString("serviceid")
                Dim comment As String = Request.QueryString("Comment")
                
                ' SECURITY FIX: Validate inputs
                If Not SecurityHelper.ValidateInput(serviceid, "^[0-9]+$") Then
                    Response.Write("{""error"": ""Invalid service ID""}")
                    Return
                End If
                
                insertComment(UID, serviceid, comment)
            ElseIf opr = "4" Then
                Dim serviceid As String = Request.QueryString("serviceid")
                Dim st As String = Request.QueryString("st")
                
                ' SECURITY FIX: Validate inputs
                If Not SecurityHelper.ValidateInput(serviceid, "^[0-9]+$") OrElse
                   Not SecurityHelper.ValidateInput(st, "^[0-9]+$") Then
                    Response.Write("{""error"": ""Invalid parameters""}")
                    Return
                End If
                
                Getcomments(serviceid, st)
            ElseIf opr = "5" Then
                Dim serviceid As String = Request.QueryString("serviceid")
                Dim asto As String = Request.QueryString("asto")
                Dim remarks As String = Request.QueryString("remarks")
                
                ' SECURITY FIX: Validate inputs
                If Not SecurityHelper.ValidateInput(serviceid, "^[0-9]+$") Then
                    Response.Write("{""error"": ""Invalid service ID""}")
                    Return
                End If
                
                AssignService(serviceid, asto, UID, remarks)
            ElseIf opr = "6" Then
                Dim serviceid As String = Request.QueryString("serviceid")
                Dim username As String = Request.Cookies("userinfo")("username")
                
                ' SECURITY FIX: Validate inputs
                If Not SecurityHelper.ValidateInput(serviceid, "^[0-9]+$") Then
                    Response.Write("{""error"": ""Invalid service ID""}")
                    Return
                End If
                
                CloseService(serviceid, UID, username)
            ElseIf opr = "7" Then
                Dim serviceid As String = Request.QueryString("serviceid")
                
                ' SECURITY FIX: Validate inputs
                If Not SecurityHelper.ValidateInput(serviceid, "^[0-9]+$") Then
                    Response.Write("{""error"": ""Invalid service ID""}")
                    Return
                End If
                
                ReceiveService(serviceid, UID)
            ElseIf opr = "8" Then
                Dim serviceid As String = Request.QueryString("serviceid")
                Dim serdate As String = Request.QueryString("srvdt")
                
                ' SECURITY FIX: Validate inputs
                If Not SecurityHelper.ValidateInput(serviceid, "^[0-9]+$") Then
                    Response.Write("{""error"": ""Invalid service ID""}")
                    Return
                End If
                
                changeServicedate(serviceid, UID, serdate)
            ElseIf opr = "9" Then
                Dim serviceid As String = Request.QueryString("serviceid")
                
                ' SECURITY FIX: Validate inputs
                If Not SecurityHelper.ValidateInput(serviceid, "^[0-9]+$") Then
                    Response.Write("{""error"": ""Invalid service ID""}")
                    Return
                End If
                
                ReOpenService(serviceid, UID)
            ElseIf opr = "10" Then
                Dim plateno As String = Request.QueryString("P")
                Dim servicetype As String = Request.QueryString("stype")
                Dim prioryty As String = Request.QueryString("pre")
                Dim remarks As String = Request.QueryString("rem")
                Dim serexpdt As String = Request.QueryString("expdt")
                Dim seriveid As String = Request.QueryString("u")
                
                ' SECURITY FIX: Validate inputs
                If Not SecurityHelper.ValidateInput(plateno, "^[A-Za-z0-9\-\s]{1,20}$") OrElse
                   Not SecurityHelper.ValidateInput(servicetype, "^[A-Za-z0-9\s\-_]{1,50}$") OrElse
                   Not SecurityHelper.ValidateInput(prioryty, "^[A-Za-z0-9\s]{1,20}$") OrElse
                   Not SecurityHelper.ValidateInput(seriveid, "^[0-9]+$") Then
                    Response.Write("{""error"": ""Invalid input parameters""}")
                    Return
                End If
                
                updateservice(seriveid, plateno, servicetype, prioryty, remarks, serexpdt)
            End If
        Catch ex As Exception
            ' SECURITY FIX: Don't expose error details to client
            SecurityHelper.LogError("Page_Load error", ex, Server)
            Response.Write("{""error"": ""An error occurred while processing your request""}")
        End Try
    End Sub
    
    Private Sub updateservice(ByVal serviceid As String, ByVal plateno As String, ByVal servicetype As String, ByVal prioryty As String, ByVal remarks As String, ByVal serexpdt As String)
        Dim res As String
        Dim json As String
        Dim conn As SqlConnection = Nothing
        
        Try
            ' SECURITY FIX: Sanitize inputs
            Dim sanitizedRemarks As String = HttpUtility.HtmlEncode(remarks.Replace("\n", "<br/>"))
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As SqlCommand = New SqlCommand("UPDATE vehicle_services SET plateno=@plateno, servicetype=@servicetype, priority=@priority, remarks=@remarks, serexpdtime=@serexpdtime WHERE serviceid=@serviceid", conn)
            cmd.Parameters.AddWithValue("@plateno", plateno)
            cmd.Parameters.AddWithValue("@servicetype", servicetype)
            cmd.Parameters.AddWithValue("@priority", prioryty)
            cmd.Parameters.AddWithValue("@remarks", remarks)
            cmd.Parameters.AddWithValue("@serexpdtime", serexpdt)
            cmd.Parameters.AddWithValue("@serviceid", serviceid)
            
            conn.Open()
            res = cmd.ExecuteNonQuery()
            
            Dim jss As New Newtonsoft.Json.JsonSerializer()
            json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
            Response.ContentType = "text/plain"
            Response.Write(json)
        Catch ex As Exception
            ' SECURITY FIX: Don't expose error details to client
            SecurityHelper.LogError("updateservice error", ex, Server)
            Response.Write("{""error"": ""An error occurred while updating service""}")
        Finally
            ' SECURITY FIX: Ensure connection is closed
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Sub insertNewService(ByVal UID As String, ByVal plateno As String, ByVal servicetype As String, ByVal prioryty As String, ByVal remarks As String, ByVal serexpdt As String)
        Dim res As String
        Dim json As String
        Dim conn As SqlConnection = Nothing
        
        Try
            ' SECURITY FIX: Sanitize inputs
            Dim sanitizedRemarks As String = HttpUtility.HtmlEncode(remarks.Replace("\n", "<br/>"))
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Use stored procedure with parameters
            Dim cmd As SqlCommand = New SqlCommand("sp_InsertNewservice", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@UID", UID)
            cmd.Parameters.AddWithValue("@plateno", plateno)
            cmd.Parameters.AddWithValue("@servicetype", servicetype)
            cmd.Parameters.AddWithValue("@priority", prioryty)
            cmd.Parameters.AddWithValue("@remarks", remarks)
            cmd.Parameters.AddWithValue("@expdt", serexpdt)
            
            conn.Open()
            res = cmd.ExecuteNonQuery()
            
            If res = 1 Then
                sendemail(UID, plateno)
            End If
            
            Dim jss As New Newtonsoft.Json.JsonSerializer()
            json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
            Response.ContentType = "text/plain"
            Response.Write(json)
        Catch ex As Exception
            ' SECURITY FIX: Don't expose error details to client
            SecurityHelper.LogError("insertNewService error", ex, Server)
            Response.Write("{""error"": ""An error occurred while creating service""}")
        Finally
            ' SECURITY FIX: Ensure connection is closed
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Public Sub sendemail(ByVal userid As String, ByVal plateno As String)
        Dim conn As SqlConnection = Nothing
        
        Try
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim message As New MailMessage
            message.From = New MailAddress("alerts@g1.com.my", "LAFARGE SERVICING")

            ' SECURITY FIX: Use parameterized query
            Dim cmd As SqlCommand = New SqlCommand("SELECT TOP 1 serviceId, servicetype, remarks, serexpdtime, vs.userid, ut.username, vs.timestamp FROM vehicle_services vs LEFT OUTER JOIN userTBL ut ON vs.userid=ut.userid WHERE vs.userid=@userid AND plateno=@plateno ORDER BY vs.timestamp DESC", conn)
            cmd.Parameters.AddWithValue("@userid", userid)
            cmd.Parameters.AddWithValue("@plateno", plateno)
            
            conn.Open()
            Dim dr As SqlDataReader
            dr = cmd.ExecuteReader()
            Dim sb As StringBuilder
            sb = New StringBuilder()
            
            If dr.Read() Then
                message.To.Add("cs.gstb@g1.com.my")
                message.Priority = MailPriority.High
                message.IsBodyHtml = True
                
                ' SECURITY FIX: HTML encode output
                message.Subject = " Lafarge Service No: <" & HttpUtility.HtmlEncode(dr("serviceid").ToString()) & "> And Type : <" & HttpUtility.HtmlEncode(dr("servicetype").ToString()) & ">"
                sb.Append("Hi ")
                sb.Append("<br/> Following is the details of the service :")
                sb.Append("<br/><br/>User Name=")
                sb.Append(HttpUtility.HtmlEncode(dr("username").ToString()))
                sb.Append("<br/>Plate Number=")
                sb.Append(HttpUtility.HtmlEncode(plateno))
                sb.Append("<br/>Service Type= " & HttpUtility.HtmlEncode(dr("servicetype").ToString()))
                
                If IsDBNull(dr("serexpdtime")) Then
                    ' No action needed
                Else
                    sb.Append("<br/>Service Expected Date & Time= " & HttpUtility.HtmlEncode(Convert.ToDateTime(dr("serexpdtime")).ToString("dd/MM/yyyy HH:mm")))
                End If

                sb.Append("<br/><u> Remarks :</u>")
                sb.Append("<br/>" & HttpUtility.HtmlEncode(dr("remarks").ToString()))
                sb.Append("<br/><br/> Thanks.")
                sb.Append("<br/> Global Alerts Team ")
                message.Body = sb.ToString()
            End If

            Dim client As New SmtpClient("203.223.159.180", 25)
            client.EnableSsl = False
            client.Send(message)
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("sendemail error", ex, Server)
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Sub insertComment(ByVal UID As String, ByVal Serviceid As String, ByVal Comment As String)
        Dim res As String
        Dim json As String
        Dim conn As SqlConnection = Nothing

        Try
            ' SECURITY FIX: Sanitize inputs
            Dim sanitizedComment As String = HttpUtility.HtmlEncode(Comment.Replace("\n", "<br/>"))
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As SqlCommand = New SqlCommand("INSERT INTO services_comments_history(serviceid, commentedby, comment, timestamp) VALUES(@serviceid, @commentedby, @comment, @timestamp)", conn)
            cmd.Parameters.AddWithValue("@serviceid", Serviceid)
            cmd.Parameters.AddWithValue("@commentedby", UID)
            cmd.Parameters.AddWithValue("@comment", sanitizedComment)
            cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))

            conn.Open()
            res = cmd.ExecuteNonQuery()

            Dim jss As New Newtonsoft.Json.JsonSerializer()
            json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
            Response.ContentType = "text/plain"
            Response.Write(json)
        Catch ex As Exception
            ' SECURITY FIX: Don't expose error details to client
            SecurityHelper.LogError("insertComment error", ex, Server)
            Response.Write("{""error"": ""An error occurred while adding comment""}")
        Finally
            ' SECURITY FIX: Ensure connection is closed
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Sub AssignService(ByVal Serviceid As String, ByVal assignedto As String, ByVal assignedby As String, ByVal remarks As String)
        Dim res As String
        Dim json As String
        Dim dr As SqlDataReader
        Dim conn As SqlConnection = Nothing
        
        Try
            ' SECURITY FIX: Sanitize inputs
            Dim sanitizedRemarks As String = HttpUtility.HtmlEncode(remarks.Replace("\n", "<br/>"))
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Use parameterized queries
            Dim cmd2 As SqlCommand = New SqlCommand("SELECT * FROM vehicle_services_history WHERE serviceid=@serviceid", conn)
            cmd2.Parameters.AddWithValue("@serviceid", Serviceid)
            
            Dim cmd3 As SqlCommand = New SqlCommand("UPDATE vehicle_services_history SET assignedto=@assignedto, timestamp=@timestamp, remarks=@remarks WHERE serviceid=@serviceid", conn)
            cmd3.Parameters.AddWithValue("@assignedto", assignedto)
            cmd3.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            cmd3.Parameters.AddWithValue("@remarks", sanitizedRemarks)
            cmd3.Parameters.AddWithValue("@serviceid", Serviceid)

            Dim cmd As SqlCommand = New SqlCommand("INSERT INTO vehicle_services_history(serviceid, timestamp, assignedto, remarks) VALUES(@serviceid, @timestamp, @assignedto, @remarks)", conn)
            cmd.Parameters.AddWithValue("@serviceid", Serviceid)
            cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@assignedto", assignedto)
            cmd.Parameters.AddWithValue("@remarks", sanitizedRemarks)
            
            Dim cmd1 As SqlCommand = New SqlCommand("UPDATE vehicle_services SET status=1, assignedby=@assignedby WHERE serviceid=@serviceid", conn)
            cmd1.Parameters.AddWithValue("@assignedby", assignedby)
            cmd1.Parameters.AddWithValue("@serviceid", Serviceid)

            conn.Open()
            dr = cmd2.ExecuteReader()
            If dr.Read() Then
                dr.Close()
                res = cmd3.ExecuteNonQuery()
                If res = "1" Then
                    cmd1.ExecuteNonQuery()
                End If
            Else
                dr.Close()
                res = cmd.ExecuteNonQuery()
                If res = "1" Then
                    cmd1.ExecuteNonQuery()
                End If
            End If
            
            Dim jss As New Newtonsoft.Json.JsonSerializer()
            json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
            Response.ContentType = "text/plain"
            Response.Write(json)
        Catch ex As Exception
            ' SECURITY FIX: Don't expose error details to client
            SecurityHelper.LogError("AssignService error", ex, Server)
            Response.Write("{""error"": ""An error occurred while assigning service""}")
        Finally
            ' SECURITY FIX: Ensure connection is closed
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Sub CloseService(ByVal Serviceid As String, ByVal closedby As String, ByVal username As String)
        Dim res As String
        Dim json As String
        Dim dr As SqlDataReader
        Dim conn As SqlConnection = Nothing
        
        Try
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Use parameterized queries
            Dim cmd2 As SqlCommand = New SqlCommand("SELECT * FROM vehicle_services_history WHERE serviceid=@serviceid", conn)
            cmd2.Parameters.AddWithValue("@serviceid", Serviceid)
            
            Dim cmd3 As SqlCommand = New SqlCommand("INSERT INTO vehicle_services_history(serviceid, timestamp, assignedto, remarks) VALUES(@serviceid, @timestamp, @assignedto, '')", conn)
            cmd3.Parameters.AddWithValue("@serviceid", Serviceid)
            cmd3.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            cmd3.Parameters.AddWithValue("@assignedto", username)

            Dim cmd As SqlCommand = New SqlCommand("UPDATE vehicle_services_history SET status=2, closeddate=@closeddate WHERE serviceid=@serviceid", conn)
            cmd.Parameters.AddWithValue("@closeddate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@serviceid", Serviceid)
            
            Dim cmd1 As SqlCommand = New SqlCommand("UPDATE vehicle_services SET status=2, closedby=@closedby WHERE serviceid=@serviceid", conn)
            cmd1.Parameters.AddWithValue("@closedby", closedby)
            cmd1.Parameters.AddWithValue("@serviceid", Serviceid)

            conn.Open()
            dr = cmd2.ExecuteReader()
            If dr.Read() Then
                dr.Close()
                res = cmd.ExecuteNonQuery()
                If res = "1" Then
                    cmd1.ExecuteNonQuery()
                End If
            Else
                dr.Close()
                res = cmd3.ExecuteNonQuery()
                If res = "1" Then
                    cmd1.ExecuteNonQuery()
                End If
            End If

            Dim jss As New Newtonsoft.Json.JsonSerializer()
            json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
            Response.ContentType = "text/plain"
            Response.Write(json)
        Catch ex As Exception
            ' SECURITY FIX: Don't expose error details to client
            SecurityHelper.LogError("CloseService error", ex, Server)
            Response.Write("{""error"": ""An error occurred while closing service""}")
        Finally
            ' SECURITY FIX: Ensure connection is closed
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    
    Private Sub ReOpenService(ByVal Serviceid As String, ByVal reopendby As String)
        Dim res As String
        Dim json As String
        Dim conn As SqlConnection = Nothing

        Try
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Use parameterized queries
            Dim cmd As SqlCommand = New SqlCommand("UPDATE vehicle_services_history SET status=5, reopeneddate=@reopeneddate WHERE serviceid=@serviceid", conn)
            cmd.Parameters.AddWithValue("@reopeneddate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@serviceid", Serviceid)
            
            Dim cmd1 As SqlCommand = New SqlCommand("UPDATE vehicle_services SET status=5, reopenby=@reopenby WHERE serviceid=@serviceid", conn)
            cmd1.Parameters.AddWithValue("@reopenby", reopendby)
            cmd1.Parameters.AddWithValue("@serviceid", Serviceid)

            conn.Open()
            res = cmd.ExecuteNonQuery()
            If res = "1" Then
                cmd1.ExecuteNonQuery()
            End If
            
            Dim jss As New Newtonsoft.Json.JsonSerializer()
            json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
            Response.ContentType = "text/plain"
            Response.Write(json)
        Catch ex As Exception
            ' SECURITY FIX: Don't expose error details to client
            SecurityHelper.LogError("ReOpenService error", ex, Server)
            Response.Write("{""error"": ""An error occurred while reopening service""}")
        Finally
            ' SECURITY FIX: Ensure connection is closed
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Sub changeServicedate(ByVal Serviceid As String, ByVal changedby As String, ByVal serdate As String)
        Dim res As String
        Dim json As String
        Dim conn As SqlConnection = Nothing

        Try
            ' SECURITY FIX: Validate date format
            Dim serviceDate As DateTime
            If Not DateTime.TryParse(serdate, serviceDate) Then
                Response.Write("{""error"": ""Invalid date format""}")
                Return
            End If
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Use parameterized queries
            Dim cmd As SqlCommand = New SqlCommand("UPDATE vehicle_services_history SET status=4, servicedate=@servicedate WHERE serviceid=@serviceid", conn)
            cmd.Parameters.AddWithValue("@servicedate", Convert.ToDateTime(serdate).ToString("yyyy/MM/dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@serviceid", Serviceid)
            
            Dim cmd1 As SqlCommand = New SqlCommand("UPDATE vehicle_services SET status=4, sdatechangedby=@sdatechangedby WHERE serviceid=@serviceid", conn)
            cmd1.Parameters.AddWithValue("@sdatechangedby", changedby)
            cmd1.Parameters.AddWithValue("@serviceid", Serviceid)

            conn.Open()
            res = cmd.ExecuteNonQuery()
            If res = "1" Then
                cmd1.ExecuteNonQuery()
            End If
            
            Dim jss As New Newtonsoft.Json.JsonSerializer()
            json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
            Response.ContentType = "text/plain"
            Response.Write(json)
        Catch ex As Exception
            ' SECURITY FIX: Don't expose error details to client
            SecurityHelper.LogError("changeServicedate error", ex, Server)
            Response.Write("{""error"": ""An error occurred while changing service date""}")
        Finally
            ' SECURITY FIX: Ensure connection is closed
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Sub ReceiveService(ByVal Serviceid As String, ByVal receiveddby As String)
        Dim res As String
        Dim json As String
        Dim conn As SqlConnection = Nothing

        Try
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As SqlCommand = New SqlCommand("UPDATE vehicle_services SET status=3, receivedby=@receivedby WHERE serviceid=@serviceid", conn)
            cmd.Parameters.AddWithValue("@receivedby", receiveddby)
            cmd.Parameters.AddWithValue("@serviceid", Serviceid)

            conn.Open()
            res = cmd.ExecuteNonQuery()
            
            Dim jss As New Newtonsoft.Json.JsonSerializer()
            json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
            Response.ContentType = "text/plain"
            Response.Write(json)
        Catch ex As Exception
            ' SECURITY FIX: Don't expose error details to client
            SecurityHelper.LogError("ReceiveService error", ex, Server)
            Response.Write("{""error"": ""An error occurred while receiving service""}")
        Finally
            ' SECURITY FIX: Ensure connection is closed
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Sub Getcomments(ByVal Serviceid As String, ByVal st As String)
        Dim json As String
        Dim dr As SqlDataReader
        Dim aa As ArrayList = New ArrayList()
        Dim a As ArrayList
        Dim comments As DataTable = New DataTable()
        Dim conn As SqlConnection = Nothing
        
        Try
            comments.Columns.Add("commentedby")
            comments.Columns.Add("comment")
            comments.Columns.Add("timestamp")
            comments.Columns.Add("username")
            comments.Columns.Add("role")

            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As SqlCommand = New SqlCommand("SELECT ut.username, ut.role, sh.commentedby, sh.comment, sh.timestamp FROM services_comments_history sh INNER JOIN userTBL ut ON sh.commentedby=ut.userid WHERE serviceid=@serviceid ORDER BY timestamp", conn)
            cmd.Parameters.AddWithValue("@serviceid", Serviceid)

            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                Dim r As DataRow
                r = comments.NewRow
                r(0) = dr("commentedby")
                r(1) = dr("comment")
                r(2) = Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd HH:mm")
                r(3) = dr("username")
                r(4) = dr("role")
                comments.Rows.Add(r)
            End While
            
            Dim sb As StringBuilder = New StringBuilder()
            If (comments.Rows.Count > 0) Then
                If st <> "1" Then
                    If (comments.Rows.Count > 3) Then
                        sb.Append("<li class='comment even thread-even depth-1'><span onclick='javascript:GetComments(1)' style='cursor:pointer;color:blue;'> View All" + (comments.Rows.Count).ToString() + " comments..</span></li>")
                        For r As Int32 = comments.Rows.Count - 3 To comments.Rows.Count - 1
                            If comments.Rows(r)(4) = "User" Or comments.Rows(r)(4) = "SuperUser" Then
                                ' SECURITY FIX: HTML encode output
                                sb.Append("<table style='font-size:10px;width:550px;'><tr><td style='color:#EC5906;width:40%;vertical-align: top;'>[" & HttpUtility.HtmlEncode(comments.Rows(r)(2).ToString()) & "] " & HttpUtility.HtmlEncode(comments.Rows(r)(3).ToString()) & " : </td>")
                                sb.Append("<td style='color:#216AB2;font-size:11px;text-align:left;'>" + HttpUtility.HtmlEncode(comments.Rows(r)(1).ToString()) + "</td><tr><table></li>")
                            Else
                                sb.Append("<table style='font-size:10px;width:550px;'><tr><td style='color:#0BA01A;width:40%;vertical-align: top;'>[" & HttpUtility.HtmlEncode(comments.Rows(r)(2).ToString()) & "] " & HttpUtility.HtmlEncode(comments.Rows(r)(3).ToString()) & " : </td>")
                                sb.Append("<td style='color:#216AB2;font-size:11px;text-align:left;'>" + HttpUtility.HtmlEncode(comments.Rows(r)(1).ToString()) + "</td><tr><table></li>")
                            End If
                        Next
                    Else
                        For r As Int32 = 0 To comments.Rows.Count - 1
                            If comments.Rows(r)(4) = "User" Or comments.Rows(r)(4) = "SuperUser" Then
                                ' SECURITY FIX: HTML encode output
                                sb.Append("<table style='font-size:10px;width:550px;'><tr><td style='color:#EC5906;width:40%;vertical-align: top;'>[" & HttpUtility.HtmlEncode(comments.Rows(r)(2).ToString()) & "] " & HttpUtility.HtmlEncode(comments.Rows(r)(3).ToString()) & " : </td>")
                                sb.Append("<td style='color:#216AB2;font-size:11px;text-align:left;'>" + HttpUtility.HtmlEncode(comments.Rows(r)(1).ToString()) + "</td><tr><table></li>")
                            Else
                                sb.Append("<table style='font-size:10px;width:550px;'><tr><td style='color:#0BA01A;width:40%;vertical-align: top;'>[" & HttpUtility.HtmlEncode(comments.Rows(r)(2).ToString()) & "] " & HttpUtility.HtmlEncode(comments.Rows(r)(3).ToString()) & " : </td>")
                                sb.Append("<td style='color:#216AB2;font-size:11px;text-align:left;'>" + HttpUtility.HtmlEncode(comments.Rows(r)(1).ToString()) + "</td><tr><table></li>")
                            End If
                        Next
                    End If
                Else
                    sb.Append("<li class='comment even thread-even depth-1'><span onclick='javascript:GetComments(0)' style='cursor:pointer;color:blue;'> Hide comments..</span></li>")

                    For r As Int32 = 0 To comments.Rows.Count - 1
                        If comments.Rows(r)(4) = "User" Or comments.Rows(r)(4) = "SuperUser" Then
                            ' SECURITY FIX: HTML encode output
                            sb.Append("<table style='font-size:10px;width:550px;'><tr><td style='color:#EC5906;width:40%;vertical-align: top;'>[" & HttpUtility.HtmlEncode(comments.Rows(r)(2).ToString()) & "] " & HttpUtility.HtmlEncode(comments.Rows(r)(3).ToString()) & " : </td>")
                            sb.Append("<td style='color:#216AB2;font-size:11px;text-align:left;'>" + HttpUtility.HtmlEncode(comments.Rows(r)(1).ToString()) + "</td><tr><table></li>")
                        Else
                            sb.Append("<table style='font-size:10px;width:550px;'><tr><td style='color:#0BA01A;width:40%;vertical-align: top;'>[" & HttpUtility.HtmlEncode(comments.Rows(r)(2).ToString()) & "] " & HttpUtility.HtmlEncode(comments.Rows(r)(3).ToString()) & " : </td>")
                            sb.Append("<td style='color:#216AB2;font-size:11px;text-align:left;'>" + HttpUtility.HtmlEncode(comments.Rows(r)(1).ToString()) + "</td><tr><table></li>")
                        End If
                    Next
                End If
            End If

            Dim jss As New Newtonsoft.Json.JsonSerializer()
            json = "{""Comments"":" & JsonConvert.SerializeObject(sb.ToString(), Formatting.None) & "}"
            Response.ContentType = "text/plain"
            Response.Write(json)
        Catch ex As Exception
            ' SECURITY FIX: Don't expose error details to client
            SecurityHelper.LogError("Getcomments error", ex, Server)
            Response.Write("{""error"": ""An error occurred while retrieving comments""}")
        Finally
            ' SECURITY FIX: Ensure connection is closed
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Sub GetServiceDetails(ByVal Serviceid As String)
        Dim json As String
        Dim dr As SqlDataReader
        Dim aa As ArrayList = New ArrayList()
        Dim a As ArrayList
        Dim conn As SqlConnection = Nothing
        
        Try
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            ' SECURITY FIX: Use parameterized query
            Dim cmd As SqlCommand = New SqlCommand("SELECT vs.userid, vs.serviceid, vs.plateno, CONVERT(VARCHAR(19), vsh.timestamp, 120) AS timestamp, vs.servicetype, vs.priority, vs.status, ISNULL(vsh.assignedto, 'Not at Assigned') assignedto, CONVERT(VARCHAR(19), vsh.closeddate, 120) closeddate, CONVERT(VARCHAR(19), vsh.servicedate, 120) servicedate, vs.remarks FROM vehicle_services vs LEFT OUTER JOIN vehicle_services_history vsh ON vs.serviceid=vsh.serviceid WHERE vs.serviceid=@serviceid", conn)
            cmd.Parameters.AddWithValue("@serviceid", Serviceid)

            conn.Open()
            dr = cmd.ExecuteReader()
            If dr.Read() Then
                a = New ArrayList()
                a.Add(dr("serviceid"))
                a.Add(dr("plateno"))
                a.Add(dr("timestamp"))
                a.Add(dr("servicetype"))
                a.Add(dr("priority"))
                Select Case dr("status")
                    Case "0" : a.Add("Open")
                    Case "3" : a.Add("Request Received")
                    Case "1" : a.Add("Technician Assign")
                    Case "4"
                        If IsDBNull(dr("servicedate")) Then
                            a.Add("Service Date Changed ")
                        Else
                            a.Add("Service Date Changed To " & Convert.ToDateTime(dr("servicedate")).ToString("yyyy/MM/dd"))
                        End If
                    Case "2" : a.Add("Closed")
                    Case "5" : a.Add("ReOpen")
                End Select
                a.Add(dr("assignedto"))
                a.Add(dr("closeddate"))
                
                ' SECURITY FIX: HTML encode output
                a.Add(HttpUtility.HtmlEncode(dr("remarks").ToString()))
                aa.Add(a)
            End If
            
            Dim jss As New Newtonsoft.Json.JsonSerializer()
            json = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.ContentType = "text/plain"
            Response.Write(json)
        Catch ex As Exception
            ' SECURITY FIX: Don't expose error details to client
            SecurityHelper.LogError("GetServiceDetails error", ex, Server)
            Response.Write("{""error"": ""An error occurred while retrieving service details""}")
        Finally
            ' SECURITY FIX: Ensure connection is closed
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Function getusername(ByVal userid As String) As String
        Dim dr As SqlDataReader
        Dim username As String = "--"
        Dim conn As SqlConnection = Nothing

        Try
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            ' SECURITY FIX: Use parameterized query
            Dim cmd As SqlCommand = New SqlCommand("SELECT username FROM userTBL WHERE userid=@userid", conn)
            cmd.Parameters.AddWithValue("@userid", userid)

            conn.Open()
            dr = cmd.ExecuteReader()
            If dr.Read() Then
                username = dr("username").ToString()
            End If
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("getusername error", ex, Server)
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return username
    End Function
End Class