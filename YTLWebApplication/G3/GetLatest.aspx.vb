Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetLatest
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim aa As New ArrayList
        Dim a As ArrayList
        Dim id As String = Request.QueryString("id")
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim role As String = Request.Cookies("userinfo")("role")
        Dim userslist As String = Request.Cookies("userinfo")("userslist")
        Dim condition As String = ""
        Dim count As Integer = 0
        If role = "User" Then
            condition = " and userid='" & userid & "'"
        ElseIf role = "SuperUser" Or role = "Operator" Then
            condition = " and userid in(" & userslist & ")"
        End If
        Dim cmd As New SqlCommand("select  * from alert_notification where timestamp between'" & Now.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss") & "' and '" & Now.ToString("yyyy/MM/dd HH:mm:ss") & "' " & condition & " and id > " & id & " order by timestamp", conn)

        Try
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            While dr.Read()
                a = New ArrayList()

                a.Add(dr("id"))

                a.Add(dr("plateno"))

                Dim diff As Integer = (DateTime.Parse(Now.ToString("yyyy/MM/dd HH:mm:ss")) - DateTime.Parse(dr("timestamp"))).TotalMinutes
                Dim timestatus As String = ""
                If diff > 0 Then
                    If diff = 1 Then
                        timestatus = diff & " min ago"
                    Else
                        timestatus = diff & " mins ago"
                    End If
                Else
                    timestatus = "Just Now"
                End If

                a.Add(timestatus)

                Select Case dr("alert_type").ToString()
                    Case "0"
                        a.Add("PTO ON")
                    Case "1"
                        a.Add("IMMOBILIZER")
                    Case "2"
                        a.Add("OVERSPEED")
                    Case "3"
                        a.Add("PANIC")
                    Case "4"
                        a.Add("POWERCUT")
                    Case "5"
                        a.Add("UNLOCK")
                    Case "6"
                        a.Add("IDLING")
                    Case "7"
                        a.Add("IGNITION OFF")
                    Case "8"
                        a.Add("IGNITION ON")
                    Case "9"
                        a.Add("OVERTIME")
                    Case "10"
                        a.Add("Geofence In")
                    Case "11"
                        a.Add("Geofence out")
                    Case Else
                End Select
                If dr("alert_type").ToString() = "2" Then
                    a.Add(dr("speed"))
                Else
                    If Not IsDBNull(dr("extra_info")) Then
                        a.Add(dr("extra_info"))
                    Else
                        a.Add("--")
                    End If
                End If


               
                aa.Add(a)
            End While

        Catch ex As Exception
            'a = New ArrayList()
            'a.Add(1)
            'a.Add(ex.Message.ToString())
            'a.Add("")
            'a.Add("")
            'aa.Add(a)
        Finally
            conn.Close()
        End Try

        Dim jss As New Newtonsoft.Json.JsonSerializer()
        Response.Write(JsonConvert.SerializeObject(aa, Formatting.None))
        Response.ContentType = "text/plain"
    End Sub

End Class
