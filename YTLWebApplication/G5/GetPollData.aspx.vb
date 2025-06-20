Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Collections
Partial Class GetPollData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim plateno As String = Request.QueryString("plateno")
        If plateno <> "" Then
            plateno = plateno.Trim()
        End If
        Dim aa As New ArrayList
        Dim a As ArrayList
        Dim inboxLess As Boolean = False
        Dim t As New DataTable

        t.Columns.Add(New DataColumn("DateTime"))
        t.Columns.Add(New DataColumn("Message"))
        t.Columns.Add(New DataColumn("MobileNo"))
        t.Columns.Add(New DataColumn("Status"))

        ' plateno = "CP0005"
        Dim edt As DateTime = Now.ToString("yyyy/MM/dd HH:mm:ss")
        Dim bdt As DateTime = edt.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss")
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

        Dim cmd As New SqlCommand

        Dim drI As SqlDataReader
        Dim dro As SqlDataReader

        Dim condition As String = ""
        Dim r As DataRow
        Dim ii As Integer = 0
        cmd.Connection = conn
        condition = "and datetime between '" & bdt.ToString("yyyy/MM/dd HH:mm:ss") & "' and '" & edt.ToString("yyyy/MM/dd HH:mm:ss") & "'"
        cmd.CommandText = "select datetime, message,mobileno from sms_inbox where plateno='" & plateno & "' " & condition & " order by datetime  "
        Try
            conn.Open()
            drI = cmd.ExecuteReader()
            While drI.Read()
                r = t.NewRow()
                r(0) = DateTime.Parse(drI("datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                r(1) = drI("message")
                r(2) = drI("mobileno")
                r(3) = 0
                t.Rows.Add(r)
            End While

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            conn.Close()
            drI.Close()
        End Try

        
        cmd.CommandText = "select datetime, message,mobileno from sms_outbox where plateno='" & plateno & "' " & condition & " order by datetime "
        Try
            conn.Open()
            dro = cmd.ExecuteReader()
            While dro.Read()
                r = t.NewRow()
                r(0) = DateTime.Parse(dro("datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                r(1) = dro("message")
                r(2) = dro("mobileno")
                r(3) = 1
                t.Rows.Add(r)
            End While

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            conn.Close()
            dro.Close()
        End Try

        Dim message As String = ""
        For c As Integer = 0 To t.Rows.Count - 1
            a = New ArrayList()
            a.Add(c + 1)
            a.Add(t.DefaultView.Item(c)(0))
            a.Add(t.DefaultView.Item(c)(2))
            message = t.DefaultView.Item(c)(1)
            If message.StartsWith("CGUS") Then
                message = "Sent Polling Command"
            End If
            a.Add(message)

            a.Add(t.DefaultView.Item(c)(3))
            aa.Add(a)
        Next
        If t.Rows.Count = 0 Then
            a = New ArrayList()
            a.Add("--")
            a.Add("--")
            a.Add("--")
            a.Add("--")
            aa.Add(a)
        End If
        Dim jss As New Newtonsoft.Json.JsonSerializer()
        Dim json As String = ""
        json = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
        Response.Write(json)
        Response.ContentType = "text/plain"

    End Sub
End Class
