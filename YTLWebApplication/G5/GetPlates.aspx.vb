Imports Newtonsoft.Json
Imports System.Data.SqlClient

Partial Class GetPlates
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim qs As String = ""
            qs = Request.QueryString("userid")
            Dim gid As String = ""
            gid = Request.QueryString("groupid")

            Dim json As String = ""
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            If role = "User" Then
                If gid <> "ALLGROUPS" Then
                    cmd = New SqlCommand("select vt.plateno,vtt.ignition,vtt.speed,isnull(vt.pmid,'-') as pmid from (select plateno,pmid from vehicleTBL where groupid = '" & gid & "') vt left outer join  vehicle_tracked2 vtt on vtt.plateno=vt.plateno", conn)
                Else
                    cmd = New SqlCommand("select vt.plateno,vtt.ignition,vtt.speed,isnull(vt.pmid,'-') as pmid from (select plateno,pmid from vehicleTBL where userid='" & userid & "') vt left outer join  vehicle_tracked2 vtt on vtt.plateno=vt.plateno", conn)
                End If
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If gid <> "ALLGROUPS" Then
                    cmd = New SqlCommand("select vt.plateno,vtt.ignition,vtt.speed,isnull(vt.pmid,'-') as pmid from (select plateno,pmid from vehicleTBL where groupid = '" & gid & "') vt left outer join  vehicle_tracked2 vtt on vtt.plateno=vt.plateno", conn)
                Else
                    If qs <> "ALLUSERS" Then
                        cmd = New SqlCommand("select vt.plateno,vtt.ignition,vtt.speed,isnull(vt.pmid,'-') as pmid from (select plateno,pmid from vehicleTBL where  userid ='" & qs & "') vt left outer join  vehicle_tracked2 vtt on vtt.plateno=vt.plateno", conn)
                    Else
                        cmd = New SqlCommand("select vt.plateno,vtt.ignition,vtt.speed,isnull(vt.pmid,'-') as pmid from (select plateno,pmid from vehicleTBL where userid in(" & userslist & ")) vt left outer join  vehicle_tracked2 vtt on vtt.plateno=vt.plateno", conn)
                    End If
                End If
            Else
                If gid <> "ALLGROUPS" Then
                    cmd = New SqlCommand("select vt.plateno,vtt.ignition,vtt.speed,isnull(vt.pmid,'-') as pmid from (select plateno,pmid from vehicleTBL where groupid = '" & gid & "') vt left outer join  vehicle_tracked2 vtt on vtt.plateno=vt.plateno", conn)
                Else
                    If qs <> "ALLUSERS" Then
                        cmd = New SqlCommand("select vt.plateno,vtt.ignition,vtt.speed,isnull(vt.pmid,'-') as pmid from (select plateno,pmid from vehicleTBL where userid ='" & qs & "') vt left outer join  vehicle_tracked2 vtt on vtt.plateno=vt.plateno", conn)
                    Else
                        cmd = New SqlCommand("select vt.plateno,vtt.ignition,vtt.speed,isnull(vt.pmid,'-') as pmid from (select plateno,pmid from vehicleTBL) vt left outer join  vehicle_tracked2 vtt on vtt.plateno=vt.plateno", conn)
                    End If
                End If
            End If

            Dim dr As SqlDataReader


            Dim aa As New ArrayList()

            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                Dim i As Integer = 0
                While dr.Read()
                    Try
                        i += 1
                        Dim a As New ArrayList()
                        Dim status As Integer = 0

                        a.Add(i)

                        a.Add(dr("plateno").ToString.ToUpper())

                        If Not IsDBNull(dr("ignition")) Then
                            If dr("ignition") Then
                                status = 0
                                If Not IsDBNull(dr("speed")) Then
                                    If dr("speed") > 0 Then
                                        status = 2
                                    Else
                                        status = 1
                                    End If
                                Else
                                    status = 1
                                End If
                            End If
                        End If

                        a.Add(status)
                        a.Add(dr("pmid"))

                        aa.Add(a)
                    Catch ex As Exception
                        Response.Write("In while Loop :" & ex.Message)
                    End Try
                End While

                Dim jss As New Newtonsoft.Json.JsonSerializer()


                json = JsonConvert.SerializeObject(aa, Formatting.None)
            Catch ex As Exception
                Response.Write("After while Loop :" & ex.Message)
            Finally
                conn.Close()
            End Try
            Response.Write(json)
            Response.ContentType = "text/plain"
        Catch ex As Exception
            Response.Write("In Last :" & ex.Message)
        End Try

    End Sub
End Class
