Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetChartVehicle
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim list As ArrayList = New ArrayList
        Dim userId As Integer = Request.QueryString("u")
        Try

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand("select plateno+'-'+convert(varchar,pto) as plateno,UPPER(plateno) as unitid from vehicleTBL where userid=@userID", conn)

            cmd.Parameters.AddWithValue("@userID", userId)

            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader
                While dr.Read
                    list.Add(New ListItem(dr("unitid").ToString, dr("plateno").ToString))
                End While
            Catch ex As Exception

            Finally
                conn.Close()
            End Try

        Catch ex As Exception

        End Try
        Dim jss As New Newtonsoft.Json.JsonSerializer()
        Dim json As String = JsonConvert.SerializeObject(list, Formatting.None)
        Response.Write(json)
        Response.ContentType = "text/plain"
    End Sub
End Class
