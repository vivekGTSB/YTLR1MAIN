Imports System.Data.SqlClient
Imports System.Collections.Generic

Partial Class DriverAssignmentManagement
    Inherits System.Web.UI.Page
    Public message As String
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand

            If role = "User" Then
                cmd = New SqlCommand("select userid,plateno from vehicleTBL where userid=@userid order by plateno", conn)
                cmd.Parameters.AddWithValue("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid,plateno from vehicleTBL where userid in(" & userslist & ") order by plateno", conn)
            Else
                cmd = New SqlCommand("select userid,plateno from vehicleTBL order by plateno", conn)
            End If

            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            While dr.Read()
                ddlplate.Items.Add(New ListItem(dr("plateno"), dr("plateno") & "," & dr("userid")))
            End While
            conn.Close()


        Catch ex As Exception

        End Try
        MyBase.OnInit(e)


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
            If Not Page.IsPostBack Then
                Fill()
                FillGrid()
            End If

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ddlplate_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlplate.SelectedIndexChanged
        Try
            Fill()
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub Fill()
        Try
            If ddlplate.SelectedValue <> "--Select Plate No--" Then
                Dim value() As String = ddlplate.SelectedValue.Split(",")
                Dim plateno As String = value(0)
                Dim userid As String = value(1)
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select driverid,drivername from driver where userid=@userid and driverid not in (select driverid from vehicle_driver where plateno in (select plateno from vehicleTBL where userid=@userid)) order by drivername", conn)
                cmd.Parameters.AddWithValue("@userid", userid)
                Dim dr As SqlDataReader
                Try
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    While dr.Read()
                        ListBox1.Items.Add(New ListItem(dr("drivername"), dr("driverid")))
                    End While
                    cmd = New SqlCommand("select driverid,drivername from driver where userid=@userid and driverid in (select driverid from vehicle_driver where plateno=@plateno order by drivername", conn)
                    cmd.Parameters.AddWithValue("@userid", userid)
                    cmd.Parameters.AddWithValue("@plateno", plateno)
                    dr = cmd.ExecuteReader()

                    While dr.Read()
                        ListBox2.Items.Add(New ListItem(dr("drivername"), dr("driverid")))
                    End While

                Catch ex As Exception

                Finally
                    conn.Close()
                End Try

            End If
        Catch ex As Exception
            message = ex.Message
        End Try
    End Sub


    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        Try
            If ddlplate.SelectedValue <> "--Select Plate No--" Then

                Dim value() As String = ddlplate.SelectedValue.Split(",")
                Dim plateno As String = value(0)
                Dim userid As String = value(1)

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select drivername,driverid from driver where userid=@userid", conn)
                cmd.Parameters.AddWithValue("@userid", userid)
                Dim dr As SqlDataReader

                Dim totalgeofenceids() As Int32 = {}
                Dim olddisablegeofenceids() As Int32 = {}
                Dim oldenablegeofenceids() As Int32 = {}

                Dim newdisablegeofenceids() As Int32 = {}
                Dim newenablegeofenceids() As Int32 = {}

                Dim i As Int32 = 0

                Try
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    While dr.Read()
                        ReDim Preserve totalgeofenceids(i)
                        totalgeofenceids(i) = dr("driverid")
                        i = i + 1
                    End While

                    i = 0
                    cmd = New SqlCommand("select driverid,drivername from driver where userid=@userid and driverid not in (select driverid from vehicle_driver where plateno=@plateno) order by drivername", conn)
                    cmd.Parameters.AddWithValue("@userid", userid)
                    cmd.Parameters.AddWithValue("@plateno", plateno)
                    dr = cmd.ExecuteReader()

                    While dr.Read()
                        ReDim Preserve olddisablegeofenceids(i)
                        olddisablegeofenceids(i) = dr("driverid")
                        i = i + 1
                    End While

                    i = 0
                    cmd = New SqlCommand("select driverid,drivername from driver where userid=@userid and driverid in (select driverid from vehicle_driver where plateno=@plateno) order by drivername", conn)
                    cmd.Parameters.AddWithValue("@userid", userid)
                    cmd.Parameters.AddWithValue("@plateno", plateno)
                    dr = cmd.ExecuteReader()

                    While dr.Read()
                        ReDim Preserve oldenablegeofenceids(i)
                        oldenablegeofenceids(i) = dr("driverid")
                        i = i + 1
                    End While

                    If Not Request.Form("newdisablegeofenceids") = "" Then
                        Dim newdisablegeofenceidsstr() = Request.Form("newdisablegeofenceids").Split(",")
                        If newdisablegeofenceidsstr.Length > 0 Then
                            For j As Int32 = 0 To newdisablegeofenceidsstr.Length - 1
                                ReDim Preserve newdisablegeofenceids(j)
                                newdisablegeofenceids(j) = newdisablegeofenceidsstr(j)
                            Next
                        End If
                    End If

                    If Not Request.Form("newenablegeofenceids") = "" Then
                        Dim newenablegeofenceidsstr() = Request.Form("newenablegeofenceids").Split(",")
                        If newenablegeofenceidsstr.Length > 0 Then
                            For j As Int32 = 0 To newenablegeofenceidsstr.Length - 1
                                ReDim Preserve newenablegeofenceids(j)
                                newenablegeofenceids(j) = newenablegeofenceidsstr(j)
                            Next
                        End If
                    End If

                    'insert new records
                    Dim found As Boolean = False
                    For j As Int32 = 0 To newenablegeofenceids.Length - 1
                        found = False
                        For k As Int32 = 0 To oldenablegeofenceids.Length - 1
                            If newenablegeofenceids(j) = oldenablegeofenceids(k) Then
                                found = True
                                Exit For
                            End If
                        Next
                        If found = False Then
                            cmd = New SqlCommand("insert into vehicle_driver (plateno,driverid,createddatetime) values(@plateno,@newenablegeofenceids,'" & Now.ToString("yyyy/MM/dd HH:mm:ss") & "')", conn)
                            cmd.Parameters.AddWithValue("@plateno", plateno)
                            cmd.Parameters.AddWithValue("@newenablegeofenceids", newenablegeofenceids(j))
                            Try
                                cmd.ExecuteNonQuery()
                            Catch ex As Exception
                                message = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
                            End Try
                        End If
                    Next

                    'delete old records
                    found = False
                    For j As Int32 = 0 To newdisablegeofenceids.Length - 1
                        found = False
                        For k As Int32 = 0 To olddisablegeofenceids.Length - 1
                            If newdisablegeofenceids(j) = olddisablegeofenceids(k) Then
                                found = True
                                Exit For
                            End If
                        Next
                        If found = False Then
                            cmd = New SqlCommand("delete from vehicle_driver where driverid=@newenablegeofenceids", conn)
                            cmd.Parameters.AddWithValue("@newenablegeofenceids", newdisablegeofenceids(j))
                            Try
                                cmd.ExecuteNonQuery()
                            Catch ex As Exception
                                message = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
                            End Try
                        End If
                    Next
                    message = "Your selected drivers are successfully enabled."
                    Fill()

                Catch ex As Exception
                    message = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
                Finally
                    conn.Close()
                End Try
                FillGrid()
            End If

        Catch ex As Exception
            message = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
        End Try

    End Sub

    Private Sub FillGrid()
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Drivers"))

            Dim VehiclesDict As New Dictionary(Of String, String)

            Dim cmd As SqlCommand

            If role = "User" Then
                cmd = New SqlCommand("select plateno,dbo.fn_GetDriverName(plateno) as drivername from vehicle_driver where plateno in (select plateno from vehicleTBL where userid=@userid) order by plateno", conn)
                cmd.Parameters.AddWithValue("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select plateno,dbo.fn_GetDriverName(plateno) as drivername from vehicle_driver where plateno in (select plateno from vehicleTBL where userid in(" & userslist & ")) order by plateno", conn)
            Else
                cmd = New SqlCommand("select plateno,dbo.fn_GetDriverName(plateno) as drivername from vehicle_driver where plateno in (select plateno from vehicleTBL)order by plateno", conn)
            End If

            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            While dr.Read()
                If VehiclesDict.ContainsKey(dr("plateno")) Then
                    Dim str As String = VehiclesDict.Item(dr("plateno"))
                    str = str & " , " & dr("drivername").ToString().ToUpper()
                    VehiclesDict(dr("plateno")) = str
                Else
                    Dim str As String = dr("drivername").ToString().ToUpper()
                    VehiclesDict(dr("plateno")) = str
                End If
            End While

            Dim r As DataRow
            Dim counter As Integer = 0
            For Each key As String In VehiclesDict.Keys
                counter += 1
                r = t.NewRow()
                r(0) = counter
                r(1) = key.ToString.ToUpper()
                r(2) = VehiclesDict(key).ToUpper()
                t.Rows.Add(r)
            Next

            Gv1.DataSource = t
            Gv1.DataBind()
        Catch ex As Exception

        Finally
            conn.Close()
        End Try
    End Sub

End Class
