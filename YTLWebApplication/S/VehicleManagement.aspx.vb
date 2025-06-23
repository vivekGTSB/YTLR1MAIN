Imports System.Data.SqlClient
Imports System.Data

Namespace AVLS

    Partial Class VehicleManagement
        Inherits System.Web.UI.Page
        Public ec As String = "false"
        Public ddlchange As Boolean = False

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub


        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Try

                'If Session("login") = Nothing Then
                '    Response.Redirect("Login.aspx")
                'End If

                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")

                Dim suserid As String = Request.QueryString("userid")


                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select userid,username from userTBL where role='User' order by username", conn)
                Dim dr As SqlDataReader

                If role = "User" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ") order by username", conn)
                End If

                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlusers.Items.Add(New ListItem(dr("username"), dr("userid")))
                End While
                conn.Close()

                If Not suserid = "" Then
                    ddlusers.SelectedValue = suserid
                End If


            Catch ex As Exception

            Finally

                MyBase.OnInit(e)

            End Try
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return deleteconfirmation();")
                    ImageButton2.Attributes.Add("onclick", "return deleteconfirmation();")

                    FillGrid()
                End If
            Catch ex As Exception
                Response.Write(ex.Message)
            End Try
        End Sub

        Private Sub FillGrid()
            Try

                Dim userid As String = ddlusers.SelectedValue
                Dim countTank1 As Integer = 0
                Dim countTank2 As Integer = 0
                Dim countTankNUll As Integer = 0
                Dim countVehicle As Integer = 0
                Dim countPTO As Integer = 0

                Dim vehiclestable As New DataTable

                vehiclestable.Columns.Add(New DataColumn("chk"))
                vehiclestable.Columns.Add(New DataColumn("sno"))
                vehiclestable.Columns.Add(New DataColumn("username"))
                vehiclestable.Columns.Add(New DataColumn("plateno"))
                vehiclestable.Columns.Add(New DataColumn("plate no"))
                vehiclestable.Columns.Add(New DataColumn("unitid"))
                vehiclestable.Columns.Add(New DataColumn("type"))
                vehiclestable.Columns.Add(New DataColumn("color"))
                vehiclestable.Columns.Add(New DataColumn("model"))
                vehiclestable.Columns.Add(New DataColumn("brand"))
                vehiclestable.Columns.Add(New DataColumn("groupname"))
                vehiclestable.Columns.Add(New DataColumn("speed"))
                vehiclestable.Columns.Add(New DataColumn("tank1"))
                vehiclestable.Columns.Add(New DataColumn("tank2"))
                vehiclestable.Columns.Add(New DataColumn("portno"))
                vehiclestable.Columns.Add(New DataColumn("Immobilizer"))
                vehiclestable.Columns.Add(New DataColumn("Roaming"))
                vehiclestable.Columns.Add(New DataColumn("installdate"))
                vehiclestable.Columns.Add(New DataColumn("Tank 1/2"))
                vehiclestable.Columns.Add(New DataColumn("PTO"))
                vehiclestable.Columns.Add(New DataColumn("WeightSensor"))
                vehiclestable.Columns.Add(New DataColumn("Permit"))
                'vehiclestable.Columns.Add(New DataColumn("icon"))

                Dim r As DataRow
                Dim connection As New Redirect(userid)

                If Not userid = "--Select User Name--" Then
                    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))
                    Dim cmd As SqlCommand = New SqlCommand("select c.name,v.userid,v.plateno,type,color,model,brand,g.groupname,trailerid,speedlimit,unitid,versionid,smallimage,bigimage,v.amount,tank1size,tank2size,tank1shape,tank2shape,odoOffset,gprsport,installationdate,immobilizer,fuelsensor,pto,WeightSensor,holcim,drivermobile,VehicleOdoRecDate,vehicleodometer,modo,H1Migration,tilt,mpob,mdt,temperature,roaming,drc,refuellimit,v.groupid,description,username,pwd,companyname,u.phoneno,u.faxno,streetname,postcode,state,role,u.userslist,u.mobileno,u.emailid,access,timestamp,usertype,dbip,remark,erp,countrycode,customrole,callback,pwdstatus,u.industry,salesagent,workhour,billedby,avlsamount,capital from vehicleTBL v left outer join customer c on v.companyid=c.id left join vehicle_group g on v.groupid=g.groupid , userTBL u where u.userid=v.userid and v.userid='" & userid & "' order by v.plateno", conn)
                    Dim dr As SqlDataReader
                    Dim role = Request.Cookies("userinfo")("role")
                    Dim userslist As String = Request.Cookies("userinfo")("userslist")
                    'Response.Write(userid)

                    If userid.Contains("Server") Then
                        cmd = New SqlCommand("select  c.name,v.userid,plateno,type,color,model,brand,g.groupname,trailerid,speedlimit,unitid,versionid,smallimage,bigimage,v.amount,tank1size,tank2size,tank1shape,tank2shape,odoOffset,gprsport,installationdate,immobilizer,fuelsensor,pto,WeightSensor,holcim,drivermobile,VehicleOdoRecDate,vehicleodometer,modo,H1Migration,tilt,mpob,mdt,temperature,roaming,drc,refuellimit,v.groupid,description,username,pwd,companyname,u.phoneno,u.faxno,streetname,postcode,state,role,u.userslist,u.mobileno,u.emailid,access,timestamp,usertype,dbip,remark,erp,countrycode,customrole,callback,pwdstatus,u.industry,salesagent,workhour,billedby,avlsamount,capital from userTBL u, vehicleTBL v left outer join customer c on v.companyid=c.id left join vehicle_group g on v.groupid=g.groupid where u.userid=v.userid and u.dbip='" & connection.dbip & "'", conn)
                    End If

                    If role = "User" Then
                        cmd = New SqlCommand("select  c.name,v.userid,plateno,type,color,model,brand,g.groupname,trailerid,speedlimit,unitid,versionid,smallimage,bigimage,v.amount,tank1size,tank2size,tank1shape,tank2shape,odoOffset,gprsport,installationdate,immobilizer,fuelsensor,pto,WeightSensor,holcim,drivermobile,VehicleOdoRecDate,vehicleodometer,modo,H1Migration,tilt,mpob,mdt,temperature,roaming,drc,refuellimit,v.groupid,description,username,pwd,companyname,u.phoneno,u.faxno,streetname,postcode,state,role,u.userslist,u.mobileno,u.emailid,access,timestamp,usertype,dbip,remark,erp,countrycode,customrole,callback,pwdstatus,u.industry,salesagent,workhour,billedby,avlsamount,capital from vehicleTBL v left outer join customer c on v.companyid=c.id left join vehicle_group g on v.groupid=g.groupid, userTBL u where u.userid=v.userid and v.userid='" & userid & "' order by v.plateno", conn)
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select  c.name,v.userid,plateno,type,color,model,brand,g.groupname,trailerid,speedlimit,unitid,versionid,smallimage,bigimage,v.amount,tank1size,tank2size,tank1shape,tank2shape,odoOffset,gprsport,installationdate,immobilizer,fuelsensor,pto,WeightSensor,holcim,drivermobile,VehicleOdoRecDate,vehicleodometer,modo,H1Migration,tilt,mpob,mdt,temperature,roaming,drc,refuellimit,v.groupid,description,username,pwd,companyname,u.phoneno,u.faxno,streetname,postcode,state,role,u.userslist,u.mobileno,u.emailid,access,timestamp,usertype,dbip,remark,erp,countrycode,customrole,callback,pwdstatus,u.industry,salesagent,workhour,billedby,avlsamount,capital from vehicleTBL v left outer join customer c on v.companyid=c.id left join vehicle_group g on v.groupid=g.groupid, userTBL u where u.userid=v.userid and v.userid in(" & userslist & ") order by v.plateno", conn)
                    End If
                    conn.Open()

                    dr = cmd.ExecuteReader()
                    Dim i As Int32 = 1
                    While dr.Read
                        r = vehiclestable.NewRow
                        r(0) = "<input type=""checkbox"" name=""chk"" value=""" & dr("plateno") & """/>"
                        r(1) = i.ToString()
                        r(2) = dr("username")
                        'r(2) = "<a href=""UpdateUnit.aspx?unitid=" & dr("unitid") & """ title='Update Unit Details'>" & dr("unitid") & "</a>"
                        r(3) = "<a rel=""balloon1""><img src=""vehiclesmallimages/" & dr("smallimage") & """ alt=""" & dr("plateno") & """ title=""" & dr("plateno") & """ width=""20px"" height=""20px"" onmouseover=""javascript:mouseover('" & dr("bigimage") & "');"" style=""vertical-align:middle;""/></a>&nbsp;<a href=""UpdateVehicle.aspx?pno=" & dr("plateno") & "&uid=" & dr("userid") & """>" & dr("plateno") & "</a>"
                        r(4) = dr("plateno")
                        r(5) = dr("unitid")
                        r(6) = dr("type")
                        r(7) = dr("color")
                        r(8) = dr("model")
                        r(9) = dr("brand")
                        r(10) = dr("groupname")
                        r(11) = dr("speedlimit")
                        If dr("tank1size") Is DBNull.Value Then
                            r(12) = "-"
                        ElseIf dr("tank1shape") Is DBNull.Value Then
                            r(12) = dr("tank1size")
                        ElseIf dr("tank1size") = "" And dr("tank1shape") = "" Then
                            r(12) = "-"
                        Else
                            r(12) = dr("tank1size") & " (" & dr("tank1shape") & ")"
                        End If
                        If dr("tank2size") Is DBNull.Value Then
                            r(13) = "-"
                        ElseIf dr("tank2shape") Is DBNull.Value Then
                            r(13) = dr("tank2size")
                        ElseIf dr("tank2size") = "" And dr("tank2shape") = "" Then
                            r(13) = "-"
                        Else
                            r(13) = dr("tank2size") & " (" & dr("tank2shape") & ")"
                        End If
                        If dr("gprsport") Is DBNull.Value Then
                            r(14) = "-"
                        ElseIf dr("gprsport") = "0" Then
                            r(14) = "-"
                        Else
                            r(14) = dr("gprsport")
                        End If
                        If dr("immobilizer") Is DBNull.Value Then
                            r(15) = "-"
                        ElseIf dr("immobilizer") = "0" Then
                            r(15) = "No"
                        Else
                            r(15) = "Yes"
                        End If
                        If dr("roaming") Is DBNull.Value Then
                            r(16) = "-"
                        ElseIf dr("roaming") = "0" Then
                            r(16) = "No"
                        Else
                            r(16) = "Yes"
                        End If
                        If dr("installationdate") Is DBNull.Value Then
                            r(17) = "-"
                        ElseIf dr("installationdate") = "1/1/1900" Then
                            r(17) = "-"
                        Else
                            r(17) = Convert.ToDateTime(dr("installationdate")).ToString("yyyy/MM/dd")
                        End If

                        If (r(12).ToString() <> "-" And r(13).ToString() <> "-") Then
                            r(18) = 2
                            countTank2 += 1
                        ElseIf (r(12).ToString() <> "-") Then
                            r(18) = 1
                            countTank1 += 1
                        Else
                            r(18) = 0
                            countTankNUll += 1
                        End If
                        If (dr("pto").ToString() = "False") Then
                            r(19) = "No"
                        Else
                            r(19) = "Yes"
                            countPTO += 1
                        End If
                        If (dr("WeightSensor").ToString() = "False") Then
                            r(20) = "No"
                        Else
                            r(20) = "Yes"
                        End If
                        r(21) = dr("name")
                        vehiclestable.Rows.Add(r)
                        i = i + 1
                        countVehicle = i
                    End While

                    conn.Close()
                End If

                If vehiclestable.Rows.Count = 0 Then
                    r = vehiclestable.NewRow
                    r(0) = "<input type=""checkbox"" name=""chk"" />"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    r(6) = "--"
                    r(7) = "--"
                    r(8) = "--"
                    r(9) = "--"
                    r(10) = "--"
                    r(11) = "--"
                    r(12) = "--"
                    r(13) = "--"
                    r(14) = "--"
                    r(15) = "--"
                    r(16) = "--"
                    r(17) = "--"
                    r(18) = "--"
                    r(19) = "--"
                    r(20) = "--"
                    r(21) = "--"
                    vehiclestable.Rows.Add(r)
                End If

                vehiclesgrid.DataSource = vehiclestable
                vehiclesgrid.DataBind()
                vehiclesgrid.Columns(2).Visible = False
                vehiclesgrid.Columns(4).Visible = False

                ec = "true"
                hdExceltitle.Value = "For the Month of " & checkMonth(DateTime.Now.Month).ToString()
                hdtotalTank1.Value = countTank1
                hdtotaltank2.Value = countTank2
                hdtotalTankNull.Value = countTankNUll
                hdtotalVehicle.Value = countVehicle - 1
                hdtotalPTO.Value = countPTO


                Session.Remove("exceltable")
                Session.Remove("exceltable2")
                Session.Remove("exceltable3")
                Session("exceltable") = vehiclestable
                Session("exceltable2") = vehiclestable

            Catch ex As Exception
                Response.Write(ex.Message & " - " & ex.StackTrace)
            End Try
        End Sub

        Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            DeleteVehicles()
        End Sub

        Protected Sub ImageButton2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
            DeleteVehicles()
        End Sub

        Protected Sub DeleteVehicles()
            Try
                Dim connection As New Redirect(ddlusers.SelectedValue)
                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))
                Dim command As SqlCommand
                Dim plateno() As String = Split(Request.Form("chk"), ",")

                For i As Int16 = 0 To plateno.Length - 1
                    conn.Open()
                    command = New SqlCommand("delete from vehicleTBL where plateno='" & plateno(i) & "'", conn)
                    command.ExecuteNonQuery()
                    conn.Close()
                Next
                FillGrid()
            Catch ex As Exception
                Response.Write(ex.Message)
            End Try
        End Sub

        Protected Function checkMonth(ByVal para As String) As String
            Select Case para
                Case 1
                    Return "January"
                Case 2
                    Return "February"
                Case 3
                    Return "March"
                Case 4
                    Return "April"
                Case 5
                    Return "May"
                Case 6
                    Return "June"
                Case 7
                    Return "July"
                Case 8
                    Return "August"
                Case 9
                    Return "September"
                Case 10
                    Return "October"
                Case 11
                    Return "November"
                Case 12
                    Return "December"
            End Select
        End Function

        Protected Sub ddlusers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlusers.SelectedIndexChanged
            Try
                FillGrid()
            Catch ex As SystemException
                Response.Write(ex.Message)
            End Try
        End Sub

        Protected Sub btnImage_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnImage.Click
            Response.Redirect("GenerateInvoice.aspx")
        End Sub
    End Class

End Namespace
