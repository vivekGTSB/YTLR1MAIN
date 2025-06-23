Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class loadplatejson
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim userid As String = Request.QueryString("userId")
        Dim luid As String = Request.QueryString("luid")
        Dim role As String = Request.QueryString("role")
        Dim userlist As String = Request.QueryString("userslist")

        LoadVehicles(userid, luid, role, userlist)

    End Sub

    Public Sub LoadVehicles(ByVal userId As String, ByVal luid As String, ByVal role As String, ByVal userslist As String)
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

    End Sub
End Class
