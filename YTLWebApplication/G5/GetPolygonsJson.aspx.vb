Imports System.Data.SqlClient
Imports Newtonsoft.Json
Partial Class GetPolygonsJson
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim LA As String = Request.Cookies("userinfo")("LA").ToString.ToUpper()
        Dim role As String = Request.Cookies("userinfo")("role")
        Dim userslist As String = Request.Cookies("userinfo")("userslist")
        Dim json As String = ""
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As SqlCommand
            If role = "User" Then
                cmd = New SqlCommand("select geofencename,geofenceid,data,accesstype,status,shiptocode from geofence where geofencetype = '1' and userid='" & userid & "' or (accesstype='1' and geofencetype = '1')", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If LA = "Y" Then
                    cmd = New SqlCommand("select geofencename,geofenceid,data,accesstype,status,shiptocode from geofence where geofencetype='1' and userid in (" & Request.Cookies("userinfo")("userslist") & ") or accesstype='2' or accesstype='1'", conn)
                Else
                    cmd = New SqlCommand("select geofencename,geofenceid,data,accesstype,status,shiptocode from geofence where geofencetype = '1' and  userid in(" & userslist & ") or (accesstype='1' and geofencetype = '1')", conn)
                End If
            Else
                cmd = New SqlCommand("select geofencename,geofenceid,data,accesstype,status,shiptocode from geofence where geofencetype = '1'", conn)
            End If
        Dim dr As SqlDataReader


        Dim aa As New ArrayList()

        Try
            conn.Open()
            dr = cmd.ExecuteReader()

            While dr.Read()
                Try
                        Dim at As Integer = 0
                    Dim status As Byte = 0
                    Dim a As New ArrayList()
                        If dr("accesstype") = "1" Then
                            at = 1
                        ElseIf dr("accesstype") = "0" Then
                            at = 0
                        Else
                            at = 2
                        End If
                    If dr("status") Then
                        status = 1
                    Else
                        status = 0
                    End If

                    a.Add(status)
                    a.Add(at)
                    a.Add(Convert.ToUInt32(dr("geofenceid")))
                    a.Add(dr("geofencename"))
                        a.Add(dr("data").ToString())
                        a.Add(dr("shiptocode"))
                        aa.Add(a)
                Catch ex As Exception

                End Try
            End While

            Dim jss As New Newtonsoft.Json.JsonSerializer()


            json = JsonConvert.SerializeObject(aa, Formatting.None)
        Catch ex As Exception

        Finally
            conn.Close()
        End Try
        Response.Write(json)
        Response.ContentType = "text/plain"
        Catch ex As Exception

        End Try

    End Sub
End Class
