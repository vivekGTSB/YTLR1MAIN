Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class smsoutboxjson
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
                Return
            End If
            
            Dim ddlu As String = Request.QueryString("u")
            Dim ddlp As String = Request.QueryString("ddlp")
            Dim bdt As String = Request.QueryString("bdt")
            Dim edt As String = Request.QueryString("edt")
            Dim luid As String = Request.QueryString("luid")
            Dim role As String = Request.QueryString("role")
            Dim userlist As String = Request.QueryString("userslist")

            ' SECURITY FIX: Validate input parameters
            If Not String.IsNullOrEmpty(ddlu) AndAlso 
               ddlu <> "--All Users--" AndAlso 
               Not SecurityHelper.ValidateInput(ddlu, "^[0-9]+$") Then
                Response.Write("[]")
                Return
            End If
            
            If Not String.IsNullOrEmpty(luid) AndAlso Not SecurityHelper.ValidateInput(luid, "^[0-9]+$") Then
                Response.Write("[]")
                Return
            End If
            
            ' SECURITY FIX: Validate date formats
            Dim beginDate As DateTime
            Dim endDate As DateTime
            
            If Not DateTime.TryParse(bdt, beginDate) OrElse Not DateTime.TryParse(edt, endDate) Then
                Response.Write("[]")
                Return
            End If

            DisplayLogInformation(ddlu, ddlp, bdt, edt, luid, role, userlist)
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("Page_Load error", ex, Server)
            Response.Write("[]")
        End Try
    End Sub

    Public Sub DisplayLogInformation(ByVal ddlu As String, ByVal ddlp As String, ByVal bdt As String, ByVal edt As String, ByVal luid As String, ByVal role As String, ByVal userslist As String)
        Dim json As String
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Dim conn As SqlConnection = Nothing
        
        Try
            Dim begindatetime As String = bdt
            Dim enddatetime As String = edt
            Dim userid As String = ddlu
            Dim plateno As String = ddlp

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Date Time"))
            t.Columns.Add(New DataColumn("Message"))
            t.Columns.Add(New DataColumn("Mobile No"))
            t.Columns.Add(New DataColumn("Server"))
            t.Columns.Add(New DataColumn("Cost"))

            Dim query As String = ""
            Dim parameters As New List(Of SqlParameter)()
            
            ' Build query based on user selection
            If ddlu <> "--All Users--" Then
                If ddlp = "--All Plate No--" Then
                    query = "SELECT m.plateno, m.datetime, m.message, m.mobileno, m.server FROM sms_outbox m WHERE m.datetime BETWEEN @beginDateTime AND @endDateTime AND m.userid=@userid"
                    parameters.Add(New SqlParameter("@userid", ddlu))
                Else
                    query = "SELECT m.plateno, m.datetime, m.message, m.mobileno, m.server FROM sms_outbox m WHERE m.datetime BETWEEN @beginDateTime AND @endDateTime AND m.plateno=@plateno"
                    parameters.Add(New SqlParameter("@plateno", ddlp))
                End If
            Else
                If ddlp = "--All Plate No--" Then
                    If role = "SuperUser" Or role = "Operator" Then
                        query = "SELECT m.plateno, m.datetime, m.message, m.mobileno, m.server FROM sms_outbox m WHERE m.datetime BETWEEN @beginDateTime AND @endDateTime AND m.userid IN (" & userslist & ")"
                    ElseIf role = "Admin" Then
                        query = "SELECT m.plateno, m.datetime, m.message, m.mobileno, m.server FROM sms_outbox m WHERE m.datetime BETWEEN @beginDateTime AND @endDateTime"
                    End If
                Else
                    query = "SELECT m.plateno, m.datetime, m.message, m.mobileno, m.server FROM sms_outbox m WHERE m.datetime BETWEEN @beginDateTime AND @endDateTime AND m.plateno=@plateno"
                    parameters.Add(New SqlParameter("@plateno", ddlp))
                End If
            End If
            
            parameters.Add(New SqlParameter("@beginDateTime", begindatetime))
            parameters.Add(New SqlParameter("@endDateTime", enddatetime))
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand(query, conn)
            
            ' Add parameters to command
            For Each param As SqlParameter In parameters
                cmd.Parameters.Add(param)
            Next
            
            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim r As DataRow
                Dim i As Int64 = 1
                Dim cost As Single = 0.2

                While dr.Read
                    r = t.NewRow
                    r(0) = i.ToString()
                    r(1) = HttpUtility.HtmlEncode(dr("plateno").ToString())
                    r(2) = Convert.ToDateTime(dr("datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                    r(3) = HttpUtility.HtmlEncode(dr("message").ToString())
                    r(4) = HttpUtility.HtmlEncode(dr("mobileno").ToString())
                    r(5) = HttpUtility.HtmlEncode(dr("server").ToString())
                    r(6) = cost.ToString("0.00")

                    t.Rows.Add(r)
                    i = i + 1
                End While

                If t.Rows.Count = 0 Then
                    r = t.NewRow
                    r(0) = "--"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    r(6) = "--"

                    t.Rows.Add(r)
                End If

            Catch ex As Exception
                ' SECURITY FIX: Log error but don't expose details
                SecurityHelper.LogError("DisplayLogInformation query error", ex, Server)
            Finally
                conn.Close()
            End Try

            If (t.Rows.Count > 0) Then
                For i As Integer = 0 To t.Rows.Count - 1
                    Try
                        a = New ArrayList
                        a.Add(t.DefaultView.Item(i)(0))
                        a.Add(t.DefaultView.Item(i)(1))
                        a.Add(t.DefaultView.Item(i)(2))
                        a.Add(t.DefaultView.Item(i)(3))
                        a.Add(t.DefaultView.Item(i)(4))
                        a.Add(t.DefaultView.Item(i)(5))
                        a.Add(Convert.ToSingle(t.DefaultView.Item(i)(6)))
                        aa.Add(a)
                    Catch ex As Exception
                        ' SECURITY FIX: Log error but don't expose details
                        SecurityHelper.LogError("DisplayLogInformation array processing error", ex, Server)
                    End Try
                Next
            End If

            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session.Remove("exceltable3")
            HttpContext.Current.Session.Remove("tempTable")

            HttpContext.Current.Session("exceltable") = t
            json = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("DisplayLogInformation error", ex, Server)
            Response.Write("[]")
        End Try
    End Sub
End Class