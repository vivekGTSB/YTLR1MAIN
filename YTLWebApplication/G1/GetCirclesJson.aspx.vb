Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetCirclesJson
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try

            Response.Write(GetJson())
            Response.ContentType = "text/plain"
        Catch ex As Exception

        End Try
    End Sub

    Private Function GetJson() As String
        Dim json As String = ""
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim la As String = Request.Cookies("userinfo")("LA")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim cmd As SqlCommand

            If role = "User" Then
                cmd = New SqlCommand("select * from geofence where geofencetype = '0' and userid='" & userid & "' or (accesstype='1' and geofencetype = '0')", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If la = "Y" Then
                    cmd = New SqlCommand("select geofencename,geofenceid,data,accesstype,status from geofence where geofencetype='0' and userid in (" & Request.Cookies("userinfo")("userslist") & ") or accesstype='2' or accesstype='1'", conn)
                Else
                    cmd = New SqlCommand("select * from geofence where geofencetype = '0' and  userid in(" & userslist & ") or (accesstype='1' and geofencetype = '0')", conn)
                End If
            Else
                cmd = New SqlCommand("select * from geofence where geofencetype = '0'", conn)
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
                        Dim values() As String = dr("data").ToString().Split(",")
                        a.Add(status)
                        a.Add(at)
                        a.Add(Convert.ToUInt32(dr("geofenceid")))
                        a.Add(dr("geofencename"))
                        a.Add(Convert.ToDouble(values(1)))
                        a.Add(Convert.ToDouble(values(0)))
                        a.Add(Convert.ToInt32(values(2)))

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
        Catch ex As Exception

        End Try

       
      
        Return json
    End Function

End Class
