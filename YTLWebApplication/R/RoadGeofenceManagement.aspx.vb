Imports System.Data.SqlClient
Imports System.Data
Imports AspMap
Imports System.Text

Partial Class RoadGeofenceManagement
    Inherits System.Web.UI.Page
    Public googlemapsparameters As String
    Public googleearthparameters As String
    Public tf As String = ""
    Public sb1 As New StringBuilder()
    Public addgeo As String = "UploadKml.aspx"

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Server.Transfer("Login.aspx")
            End If
        Catch ex As Exception

        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Page.IsPostBack = False Then
               
            End If
            FillGrid()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub FillGrid()
        Try
            Dim geofencetable As New DataTable
            geofencetable.Rows.Clear()
            geofencetable.Columns.Add(New DataColumn("chk"))
            geofencetable.Columns.Add(New DataColumn("S No"))
            geofencetable.Columns.Add(New DataColumn("Geofence Name"))
            geofencetable.Columns.Add(New DataColumn("Username"))
            geofencetable.Columns.Add(New DataColumn("Transporter"))
            geofencetable.Columns.Add(New DataColumn("Modify Date Time"))

            Dim r As DataRow

            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            'Dim cmd As SqlCommand = New SqlCommand("select  t1.geofenceid,t1.geofencename,t1.status,t1.geofencetype,t1.modifieddatetime,t2.username,t2.companyname from geofence as t1,userTBL as t2 where accesstype='0' and t1.userid=t2.userid order by geofencename", conn)
            Dim cmd As SqlCommand = New SqlCommand("select  t1.geofenceid,t1.geofencename,t1.modifieddatetime,t2.username,t2.companyname from road_geofence as t1,userTBL as t2 where  t1.userid=t2.userid order by geofencename", conn)

            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim userid As String = Request.Cookies("userinfo")("userid")

            If role = "User" Then
                cmd = New SqlCommand("select  t1.geofenceid,t1.geofencename,t1.modifieddatetime,t2.username,t2.companyname from road_geofence as t1,userTBL as t2 where  t1.userid='" & userid & "' and t1.userid=t2.userid order by geofencename", conn)

            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select  t1.geofenceid,t1.geofencename,t1.modifieddatetime,t2.username,t2.companyname from road_geofence as t1,userTBL as t2 where t1.userid in (" & userslist & ") and t1.userid=t2.userid order by geofencename", conn)

            End If

            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim i As Int32 = 1
                While (dr.Read)
                    Try

                        r = geofencetable.NewRow

                        r(0) = "<input type=""checkbox"" name=""chk"" value=""" & dr("geofenceid") & """/>"
                        r(1) = i.ToString()

                        r(2) = "<a href='UpdateRoadGeofenceT.aspx?geofenceid=" & dr("geofenceid") & "'> " & dr("geofencename").ToString.ToUpper() & " </a>"



                        r(3) = dr("username").ToString().ToUpper()
                        r(4) = dr("companyname").ToString().ToUpper()

                        r(5) = DateTime.Parse(dr("modifieddatetime")).ToString("yyyy/MM/dd  HH:mm:ss")

                        geofencetable.Rows.Add(r)
                        i = i + 1

                    Catch ex As Exception

                    End Try
                End While
            Catch ex As Exception

            Finally
                conn.Close()
            End Try
            If geofencetable.Rows.Count = 0 Then
                r = geofencetable.NewRow

                r(0) = "<input type=""checkbox"" name=""chk"" />"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                r(5) = "-"
                geofencetable.Rows.Add(r)
            End If

            Session("exceltable") = geofencetable

            sb1.Length = 0
            sb1.Append("<thead><tr><th align=""center""  style=""width:20px;""><input type='checkbox' onclick=""javascript:checkall(this);""/></th><th style=""width:35px;"" >S No</th><th>Geofence Name</th><th>Username</th><th>Transporter Name</th><th style=""width:130px;"">Modify Date Time</th></tr></thead>")
            Dim counter As Integer = 1
            sb1.Append("<tbody>")
            For i As Integer = 0 To geofencetable.Rows.Count - 1
                Try
                    sb1.Append("<tr>")
                    sb1.Append("<td>")
                    sb1.Append(geofencetable.DefaultView.Item(i)(0))
                    sb1.Append("</td>")
                    sb1.Append("<td>")
                    sb1.Append(geofencetable.DefaultView.Item(i)(1))
                    sb1.Append("</td>")
                    sb1.Append("<td>")
                    sb1.Append(geofencetable.DefaultView.Item(i)(2))
                    sb1.Append("</td>")
                    sb1.Append("<td>")
                    sb1.Append(geofencetable.DefaultView.Item(i)(3))
                    sb1.Append("</td>")
                    sb1.Append("<td>")
                    sb1.Append(geofencetable.DefaultView.Item(i)(4))
                    sb1.Append("</td>")
                    sb1.Append("<td>")
                    sb1.Append(geofencetable.DefaultView.Item(i)(5))
                    sb1.Append("</td>")

                    sb1.Append("</tr>")

                    counter += 1
                Catch ex As Exception

                End Try
            Next
            sb1.Append("</tbody>")


            sb1.Append("<tfoot><tr><th><input type='checkbox' onclick=""javascript:checkall(this);""/></th><th>S No</th><th>Geofence Name</th><th>Username</th><th>Transporter Name</th><th>Modify DateTime</th></tr></tfoot>")


        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub
    Protected Sub DeleteGeofence()
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand

            Dim geofenceids() As String = Request.Form("chk").Split(",")
            conn.Open()
            For i As Int32 = 0 To geofenceids.Length - 1
                cmd = New SqlCommand("delete from road_geofence where geofenceid='" & geofenceids(i) & "'", conn)
                Try
                    cmd.ExecuteNonQuery()
                Catch ex As Exception

                End Try
            Next
            conn.Close()
            FillGrid()
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub AddGeofence()
        Response.Redirect("AddVehicleToGF.aspx")
    End Sub


    Function getUserLevel() As String
        Try
            Dim cmd As SqlCommand
            Dim Userlevel As String
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            cmd = New SqlCommand("select usertype from userTBL where userid='" & Request.Cookies("userinfo")("userid") & "'", conn)
            conn.Open()
            Userlevel = cmd.ExecuteScalar()
            conn.Close()

            Return Userlevel
        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Function

    Function LimitUserAccess() As Boolean
        If getUserLevel() = "7" Then

            Return True
        Else
            Return False
        End If
    End Function



    Private Sub Activate()
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand

            Dim geofenceids() As String = Request.Form("chk").Split(",")
            conn.Open()
            For i As Int32 = 0 To geofenceids.Length - 1
                cmd = New SqlCommand("update geofence set status='1' where geofenceid='" & geofenceids(i) & "'", conn)
                Try
                    cmd.ExecuteNonQuery()
                Catch ex As Exception

                End Try
            Next
            conn.Close()
            FillGrid()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub DeActivate()
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand

            Dim geofenceids() As String = Request.Form("chk").Split(",")
            conn.Open()
            For i As Int32 = 0 To geofenceids.Length - 1
                cmd = New SqlCommand("update geofence set status='0' where geofenceid='" & geofenceids(i) & "'", conn)
                Try
                    cmd.ExecuteNonQuery()
                Catch ex As Exception

                End Try
            Next
            conn.Close()
            FillGrid()
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ImageButton5_Click(sender As Object, e As System.EventArgs) Handles ImageButton5.Click
        DeleteGeofence()
    End Sub


    Protected Sub ImageButton6_Click(sender As Object, e As System.EventArgs) Handles ImageButton6.Click
        DeleteGeofence()
    End Sub
End Class
