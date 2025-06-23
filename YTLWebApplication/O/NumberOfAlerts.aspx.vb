Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class NumberOfAlerts
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim role As String = Request.Cookies("userinfo")("role")
        Dim userslist As String = Request.Cookies("userinfo")("userslist")
        Dim condition As String = ""
       
        Dim count As String = "0"
        If role = "User" Then
            condition = " and userid='" & userid & "'"
        ElseIf role = "SuperUser" Or role = "Operator" Then
            condition = " and userid in(" & userslist & ")"
        End If
        'Dim cmd As New SqlCommand("select count(*) from alert_notification where resolved='0'
        'and timestamp between '" & Now.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss") & "' and '"
        '& Now.ToString("yyyy/MM/dd HH:mm:ss") & "' "
        '& condition, conn)
        Dim query As String = "SELECT COUNT(*) FROM alert_notification " &
                      "WHERE resolved = @resolved " &
                      "AND timestamp BETWEEN @startTime AND @endTime " & condition

        Dim cmd As New SqlCommand(query, conn)
        cmd.Parameters.AddWithValue("@resolved", "0")
        cmd.Parameters.AddWithValue("@startTime", Now.AddHours(-24))
        cmd.Parameters.AddWithValue("@endTime", Now)

        Try
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            If dr.Read() Then
                count = dr(0).ToString()
            End If
        Catch ex As Exception
            count = "0"
        Finally
            conn.Close()
        End Try
        Response.Write(count)

    End Sub
End Class
