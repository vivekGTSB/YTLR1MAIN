Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data

Partial Class VehicleOverspeedReport
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Dim suspectRefuel As Boolean = False
    Dim suspectTime As String
    Dim GrantTotalOdometer As Double

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim cmd As SqlCommand
            Dim dr As SqlDataReader

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            cmd = New SqlCommand("select userid, username,dbip from userTBL where role='User' order by username", conn)
            If role = "User" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid='" & userid & "'", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
            End If
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While
            dr.Close()
            If role = "User" Then
                ddlUsername.Items.Remove("--Select User Name--")
                ddlUsername.SelectedValue = userid
                getPlateNo(userid)
            End If
            conn.Close()

        Catch ex As Exception

            Response.Write(ex.Message)
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub



    Protected Sub getPlateNo(ByVal uid As String)
        Try
            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("All")
                Dim cmd As SqlCommand
                Dim dr As SqlDataReader
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & uid & "' order by plateno", conn)
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlpleate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                End While
                dr.Close()

                conn.Close()
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub DisplaySummary()
        Try

            Dim begindatetime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            Dim t2 As New DataTable
            t2.Columns.Add(New DataColumn("No"))
            t2.Columns.Add(New DataColumn("Username"))
            t2.Columns.Add(New DataColumn("VehicleGroup"))
            t2.Columns.Add(New DataColumn("PlateNo"))
            t2.Columns.Add(New DataColumn("DateTime"))
            t2.Columns.Add(New DataColumn("Speed"))
            t2.Columns.Add(New DataColumn("Address"))
            t2.Columns.Add(New DataColumn("Latitute"))
            t2.Columns.Add(New DataColumn("Longitude"))

            Dim cmd As SqlCommand
            Dim dr As SqlDataReader
            Dim r As DataRow
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim counter As Integer = 1
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            cmd = New SqlCommand("select userid, username,dbip from userTBL where role='User' order by username", conn)

            If ddlpleate.SelectedValue = "All" Then
                cmd = New SqlCommand("Select t4.username,isnull( t3.groupname,'') as groupname,t1.plateno ,t1.fromtime,t1.speed ,t1.address,t1.lat ,t1.lon     from vehicle_safty_details t1 left outer join vehicleTBL t2 on t1.plateno =t2.plateno  left outer join vehicle_group  t3 on t2.groupid =t3.groupid left outer join userTBL t4 on t4.userid =t2.userid where t1.plateno in (select plateno from vehicleTBL where userid='" & ddlUsername.SelectedValue & "') And t1.timesatmp between @bdt and @edt and t1.viotype in (55,56) order by plateno ,fromtime ,username ", conn)
            Else
                cmd = New SqlCommand("Select t4.username,isnull( t3.groupname,'') as groupname,t1.plateno ,t1.fromtime,t1.speed ,t1.address,t1.lat ,t1.lon     from vehicle_safty_details t1 left outer join vehicleTBL t2 on t1.plateno =t2.plateno  left outer join vehicle_group  t3 on t2.groupid =t3.groupid left outer join userTBL t4 on t4.userid =t2.userid where t1.plateno =@plateno And t1.timesatmp between @bdt and @edt and t1.viotype in (55,56) order by plateno ,fromtime ,username ", conn)
            End If

            cmd.Parameters.AddWithValue("@bdt", begindatetime)
            cmd.Parameters.AddWithValue("@edt", enddatetime)
            cmd.Parameters.AddWithValue("@plateno", ddlpleate.SelectedValue)
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                r = t2.NewRow()
                r(0) = counter
                r(1) = dr("username")
                r(2) = dr("groupname")
                r(3) = dr("plateno")
                r(4) = Convert.ToDateTime(dr("fromtime")).ToString("yyyy/MM/dd HH:mm:ss")
                r(5) = Convert.ToDouble(dr("speed")).ToString("0.00")
                r(6) = dr("address").ToString().Replace("<div class='r1' title='street name'></div>", "")
                r(7) = Convert.ToDouble(dr("lat")).ToString("0.0000")
                r(8) = Convert.ToDouble(dr("lon")).ToString("0.0000")
                counter += 1
                t2.Rows.Add(r)
            End While

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session.Remove("exceltable3")

            ec = "true"

            Session("exceltable") = t2
            GridView2.DataSource = t2
            GridView2.DataBind()

        Catch ex As Exception
            Response.Write(ex.Message & " - " & ex.StackTrace)
        End Try

    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) ' Handles GridView2.RowDataBound

    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        getPlateNo(ddlUsername.SelectedValue)
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplaySummary()
    End Sub
End Class
