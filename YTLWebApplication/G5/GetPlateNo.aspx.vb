Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.DataRow
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports AspMap
Partial Class GetPlateNo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Dim userId As String = Request.QueryString("userId")
        ' Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim role As String = Request.Cookies("userinfo")("role")
        Dim userslist As String = Request.Cookies("userinfo")("userslist")
        Dim list As ArrayList = New ArrayList
        Dim l As ArrayList
        Try
            Dim cmd As New SqlCommand
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            cmd.Connection = conn
            If userId <> "--All Users--" Then
                cmd.CommandText = "select plateno from vehicleTBL where userid='" & userId & "' order by plateno"
            Else
                If role = "SuperUser" Or role = "Operator" Then
                    cmd.CommandText = "select plateno from vehicleTBL where userid in (" & userslist & ") order by plateno"
                ElseIf role = "User" Then
                    cmd.CommandText = "select plateno from vehicleTBL where userid='" & userId & "' order by plateno"
                Else
                    cmd.CommandText = "select plateno from vehicleTBL order by plateno"
                End If
            End If

            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader
                While dr.Read
                    l = New ArrayList()
                    l.Add(dr("plateno").ToString().ToUpper())
                    list.Add(l)
                End While
            Catch ex As Exception

            Finally
                conn.Close()
            End Try

        Catch ex As Exception

        End Try
        Dim json As String = JsonConvert.SerializeObject(list, Formatting.None)

        Response.Write(json)
        Response.ContentType = "text/plain"

        '  Return json

    End Sub

End Class
