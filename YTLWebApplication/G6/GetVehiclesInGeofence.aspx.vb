Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Collections.Generic
Public Class GetVehiclesInGeofence
    Inherits System.Web.UI.Page
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
        Catch ex As Exception

        End Try
        MyBase.OnInit(e)
    End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Dim jss As New Newtonsoft.Json.JsonSerializer()
        Dim json As String = ""
        Try
            Dim dr As SqlDataReader
            Dim gid As String() = System.Uri.UnescapeDataString(Request.QueryString("gid")).ToString().Split(",")
            Dim geos As String = ""
            For Each g As String In gid
                geos = geos & g & ","

            Next
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim role As String = Request.Cookies("userinfo")("role")
            geos = geos.Substring(0, geos.Length - 1)
            Dim cmd As New SqlCommand()
            cmd.Connection = conn
            conn.Open()
            If Not gid Is Nothing Then
                If role = "Admin" Then
                    cmd.CommandText = "select id,t1.plateno,isnull(t2.pmid,'-') as pmid,intimestamp as timestamp, inlat as lat, inlon as lon from public_geofence_History t1 left outer join vehicletbl t2 on t1.plateno=t2.plateno   where id in (" & geos & ")  and intimestamp between '" & DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss") & "' and GetDate() and outtimestamp is null"
                ElseIf role = "User" Then
                    cmd.CommandText = "select id,t1.plateno,isnull(t2.pmid,'-') as pmid,intimestamp as timestamp, inlat as lat, inlon as lon from public_geofence_History t1 left outer join vehicletbl t2 on t1.plateno=t2.plateno   where id in (" & geos & ")  and intimestamp between '" & DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss") & "' and GetDate() and outtimestamp is null and t2.userid='" & userid & "'"
                Else
                    cmd.CommandText = "select id,t1.plateno,isnull(t2.pmid,'-') as pmid,intimestamp as timestamp, inlat as lat, inlon as lon from public_geofence_History t1 left outer join vehicletbl t2 on t1.plateno=t2.plateno   where id in (" & geos & ")  and intimestamp between '" & DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss") & "' and GetDate() and outtimestamp is null and t2.userid in (" & userslist & ")"
                End If


                Try
                    dr = cmd.ExecuteReader()
                    While dr.Read()
                        Try
                            a = New ArrayList
                            a.Add(dr("id"))
                            If dr("pmid") = "-" Then
                                a.Add(dr("plateno"))
                            Else
                                a.Add(dr("pmid") & "-" & dr("plateno"))
                            End If
                            a.Add(dr("timestamp"))
                            a.Add(dr("lat"))
                            a.Add(dr("lon"))
                            aa.Add(a)
                        Catch ex As Exception

                        End Try
                    End While

                Catch ex As Exception
                    Response.ContentType = "text/plain"
                    Response.Write(ex.Message)
                Finally

                End Try
            End If
        Catch ex As Exception
            Response.ContentType = "text/plain"
            Response.Write(ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        json = JsonConvert.SerializeObject(aa, Formatting.None)
        Response.ContentType = "text/plain"
        Response.Write(json)
    End Sub

End Class