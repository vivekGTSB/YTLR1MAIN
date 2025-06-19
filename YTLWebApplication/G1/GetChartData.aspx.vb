Imports AspMap
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class GetChartData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Response.Write(GetJson())
            Response.ContentType = "text/plain"
        Catch ex As Exception

        End Try
    End Sub
    Protected Function GetJson() As String
        Dim json As String = ""
        Dim jss As New Newtonsoft.Json.JsonSerializer()
        Dim aa As New ArrayList()
        Dim a As New ArrayList()
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As SqlCommand = New SqlCommand("select * from plant_geofence_vehicle order by shiptoname", conn)
        conn.Open()
        Dim dr As SqlDataReader = cmd.ExecuteReader()
        While dr.Read()
            a = New ArrayList()
            a.Add(dr("short_name"))
            a.Add(dr("name"))
            a.Add(dr("one"))
            a.Add(dr("onehalf"))
            a.Add(dr("two"))
            a.Add(dr("twohalf"))
            a.Add(dr("three"))
            a.Add(dr("threehalf"))
            a.Add(dr("four"))
            a.Add(dr("fourhalf"))
            a.Add(dr("five"))
            a.Add(dr("fiveplus"))
            aa.Add(a)
        End While
        json = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
        Return json

    End Function


End Class
