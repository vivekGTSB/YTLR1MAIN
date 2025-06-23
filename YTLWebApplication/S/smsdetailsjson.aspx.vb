Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class smsdetailsjson
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
                Return
            End If
            
            Dim fromdate As String = Request.QueryString("fromdate")
            Dim todate As String = Request.QueryString("todate")
            Dim plateno As String = Request.QueryString("plateno")
            Dim userid As String = Request.QueryString("userid")
            Dim atype As String = Request.QueryString("atype")
            
            ' SECURITY FIX: Validate input parameters
            If Not SecurityHelper.ValidateInput(userid, "^[0-9]+$") Then
                Response.Write("[]")
                Return
            End If
            
            If Not String.IsNullOrEmpty(plateno) AndAlso Not SecurityHelper.ValidateInput(plateno, "^[A-Za-z0-9\-\s]{1,20}$") Then
                Response.Write("[]")
                Return
            End If
            
            If Not String.IsNullOrEmpty(atype) AndAlso atype <> "ALL" AndAlso Not SecurityHelper.ValidateInput(atype, "^[0-9]+$") Then
                Response.Write("[]")
                Return
            End If
            
            ' SECURITY FIX: Validate date formats
            Dim beginDate As DateTime
            Dim endDate As DateTime
            
            If Not DateTime.TryParse(fromdate, beginDate) OrElse Not DateTime.TryParse(todate, endDate) Then
                Response.Write("[]")
                Return
            End If
            
            GetSMSDetails(fromdate, todate, plateno, userid, atype)
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("Page_Load error", ex, Server)
            Response.Write("[]")
        End Try
    End Sub

    Public Sub GetSMSDetails(ByVal fromdate As String, ByVal todate As String, ByVal plateno As String, ByVal userid As String, ByVal atype As String)
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Dim json As String = ""
        Dim conn As SqlConnection = Nothing

        Try
            Dim vehiclestable As New DataTable
            vehiclestable.Columns.Add(New DataColumn("Sno"))
            vehiclestable.Columns.Add(New DataColumn("Username"))
            vehiclestable.Columns.Add(New DataColumn("Plateno"))
            vehiclestable.Columns.Add(New DataColumn("Date"))
            vehiclestable.Columns.Add(New DataColumn("Alert"))
            vehiclestable.Columns.Add(New DataColumn("mobile"))
            vehiclestable.Columns.Add(New DataColumn("Message"))

            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand()
            cmd.Connection = conn
            
            ' SECURITY FIX: Use parameterized query
            If atype <> "ALL" Then
                cmd.CommandText = "SELECT ot.plateno, ot.userid, ut.username, alert_type, ot.datetime, ot.message, ot.mobileno FROM sms_outbox ot INNER JOIN userTBL ut ON ot.userid=ut.userid WHERE ot.userid=@userid AND ot.plateno=@plateno AND alert_type=@alertType AND datetime BETWEEN @fromDate AND @toDate ORDER BY datetime"
                cmd.Parameters.AddWithValue("@alertType", atype)
            Else
                cmd.CommandText = "SELECT ot.plateno, ot.userid, ut.username, alert_type, ot.datetime, ot.message, ot.mobileno FROM sms_outbox ot INNER JOIN userTBL ut ON ot.userid=ut.userid WHERE ot.userid=@userid AND ot.plateno=@plateno AND datetime BETWEEN @fromDate AND @toDate ORDER BY datetime"
            End If
            
            cmd.Parameters.AddWithValue("@userid", userid)
            cmd.Parameters.AddWithValue("@plateno", plateno)
            cmd.Parameters.AddWithValue("@fromDate", fromdate)
            cmd.Parameters.AddWithValue("@toDate", todate)

            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader
                Dim sno As Int16 = 1

                While dr.Read
                    Dim r As DataRow = vehiclestable.NewRow
                    r(0) = sno
                    r(1) = HttpUtility.HtmlEncode(dr("username").ToString())
                    r(2) = HttpUtility.HtmlEncode(dr("plateno").ToString())
                    r(3) = Convert.ToDateTime(dr("datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                    
                    Select Case dr("alert_type")
                        Case "0" : r(4) = "Pto On"
                        Case "1" : r(4) = "Immobilizer"
                        Case "2" : r(4) = "Overspeed"
                        Case "3" : r(4) = "Panic"
                        Case "4" : r(4) = "PowerCut"
                        Case "5" : r(4) = "Unlock"
                        Case "6" : r(4) = "Idling"
                        Case "7" : r(4) = "IgnitionOff"
                        Case "8" : r(4) = "IgnitionOn"
                        Case "9" : r(4) = "Overtime"
                        Case "10" : r(4) = "GeofenceIn"
                        Case "11" : r(4) = "GeofenceOut"
                        Case "12" : r(4) = "SignalLoss"
                        Case "13" : r(4) = "HarshBreaak"
                        Case "14" : r(4) = "TollPlaza"
                        Case "15" : r(4) = "Refuel"
                        Case "16" : r(4) = "FuelTheft"
                        Case "17" : r(4) = "Stop"
                        Case Else
                            r(4) = "Others"
                    End Select
                    
                    r(5) = HttpUtility.HtmlEncode(dr("message").ToString())
                    r(6) = HttpUtility.HtmlEncode(dr("mobileno").ToString())
                    vehiclestable.Rows.Add(r)
                    sno = sno + 1
                End While

                For j As Integer = 0 To vehiclestable.Rows.Count - 1
                    Try
                        a = New ArrayList
                        a.Add(vehiclestable.DefaultView.Item(j)(0))
                        a.Add(vehiclestable.DefaultView.Item(j)(1))
                        a.Add(vehiclestable.DefaultView.Item(j)(2))
                        a.Add(vehiclestable.DefaultView.Item(j)(3))
                        a.Add(vehiclestable.DefaultView.Item(j)(4))
                        a.Add(vehiclestable.DefaultView.Item(j)(5))
                        a.Add(vehiclestable.DefaultView.Item(j)(6))
                        aa.Add(a)
                    Catch ex As Exception
                        ' SECURITY FIX: Log error but don't expose details
                        SecurityHelper.LogError("GetSMSDetails array processing error", ex, Server)
                    End Try
                Next
                
            Catch ex As Exception
                ' SECURITY FIX: Log error but don't expose details
                SecurityHelper.LogError("GetSMSDetails query error", ex, Server)
            End Try

        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("GetSMSDetails error", ex, Server)
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        
        json = JsonConvert.SerializeObject(aa, Formatting.None)
        Response.ContentType = "application/json"
        Response.Write(json)
    End Sub
End Class