Imports System.Data.SqlClient

Partial Class LafargeInstantAlertSettingsManagement
    Inherits System.Web.UI.Page
    Public addfuelpage As String = "AddFuel.aspx"
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Server.Transfer("Login.aspx")
            End If

            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim userid As String = Request.Cookies("userinfo")("userid")

            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim cmd As SqlCommand

            If role = "User" Then
                cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ") order by username", conn)
                ddluser.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                ddluser.Items.Add(New ListItem("--All Users--", "--All Users--"))
            Else
                cmd = New SqlCommand("select userid,username from userTBL where role='User' order by username", conn)
                ddluser.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                ddluser.Items.Add(New ListItem("--All Users--", "--All Users--"))
            End If

            conn.Open()

            Dim dr As SqlDataReader = cmd.ExecuteReader()


            While dr.Read()
                ddluser.Items.Add(New ListItem(dr("username").ToString().ToUpper(), dr("userid")))
            End While

            conn.Close()
        Catch ex As Exception

        End Try
        MyBase.OnInit(e)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Page.IsPostBack = False Then
                delete1.Attributes.Add("onclick", "return deleteconfirmation();")
                delete2.Attributes.Add("onclick", "return deleteconfirmation();")
            End If
            FillGrid()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub FillGrid()
        Try
            Dim userid As String = ddluser.SelectedValue

            Dim t As New DataTable

            Dim r As DataRow
            t.Rows.Clear()
            t.Columns.Add(New DataColumn("chk"))
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Mobiles List"))
            t.Columns.Add(New DataColumn("EMails List"))
            t.Columns.Add(New DataColumn("EMail"))
            t.Columns.Add(New DataColumn("Sms"))
            t.Columns.Add(New DataColumn("Popup"))

            Dim key() As String = {"Geofence In", "Geofence Out", "Idling", "Immobilizer", "Overspeed", "Panic", "Power Cut", "PTO On", "Jamming"}
            Dim value() As String = {"geofencein", "geofenceout", "idling", "immobilizer", "overspeed", "panic", "powercut", "alarm", "Jamming"}

            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            If Not userid = "--Select User Name--" Then
                Dim cmd As SqlCommand = New SqlCommand("select plateno,mobileno,emailid,alarm,geofencein,geofenceout,idling,ignitionon,ignitionoff,immobilizer,overspeed,overtimedrive,panic,powercut,unlock,signallose,Jamming from instant_alert_settings where userid='" & userid & "' order by plateno", conn)
                If userid = "--All Users--" Then
                    Dim role As String = Request.Cookies("userinfo")("role")
                    Dim userslist As String = Request.Cookies("userinfo")("userslist")
                    cmd = New SqlCommand("select plateno,mobileno,emailid,alarm,geofencein,geofenceout,idling,ignitionon,ignitionoff,immobilizer,overspeed,overtimedrive,panic,powercut,unlock,signallose,Jamming from instant_alert_settings order by plateno", conn)
                    If role = "User" Then
                        cmd = New SqlCommand("select plateno,mobileno,emailid,alarm,geofencein,geofenceout,idling,ignitionon,ignitionoff,immobilizer,overspeed,overtimedrive,panic,powercut,unlock,signallose,Jamming from instant_alert_settings where userid='" & userid & "' order by plateno", conn)
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select plateno,mobileno,emailid,alarm,geofencein,geofenceout,idling,ignitionon,ignitionoff,immobilizer,overspeed,overtimedrive,panic,powercut,unlock,signallose,Jamming from instant_alert_settings where userid in(" & userslist & ") order by plateno", conn)
                    End If
                End If

                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim i As Int32 = 1

                While dr.Read()
                    r = t.NewRow
                    r(0) = "<input type=""checkbox"" name=""chk"" value=""" & dr("plateno") & """/>"
                    r(1) = i.ToString()
                    r(2) = "<a href=""LafargeUpdateInstantAlertSettings.aspx?plateno=" & dr("plateno") & """/>" & dr("plateno") & "</a>"

                    Dim mnumber() As String = dr("mobileno").Split(",")
                    Array.Sort(mnumber)
                    Dim strmobile As String = ""
                    For k As Int32 = 0 To mnumber.Length - 1
                        If mnumber(k) <> "" Then
                            strmobile = strmobile & "<option value='" & k & "'>" & mnumber(k) & "</option>"
                        End If
                    Next
                    r(3) = "<select id= 'cbxmnumbers' style='width: 110px;'>" & strmobile & "</select>"


                    Dim email() As String = dr("emailid").Split(",")
                    Array.Sort(email)
                    Dim stremailid As String = ""
                    For k As Int32 = 0 To email.Length - 1
                        If email(k) <> "" Then
                            stremailid = stremailid & "<option value='" & k & "'>" & email(k) & "</option>"
                        End If
                    Next
                    r(4) = "<select id= 'cbxemail' style='width: 250px;'>" & stremailid & "</select>"

                    Dim emailactive As String = ""
                    Dim emailinactive As String = ""

                    Dim smsactive As String = ""
                    Dim smsinactive As String = ""

                    Dim popupactive As String = ""
                    Dim popupinactive As String = ""

                    Dim binvalue() As Char

                    For j As Byte = 0 To 8
                        binvalue = Convert.ToString(dr(value(j)), 2).PadLeft(3, "0")
                        If binvalue(0) = "1" Then
                            emailactive = emailactive & "<option>" & key(j) & "</option>"
                        Else
                            emailinactive = emailinactive & "<option>" & key(j) & "</option>"
                        End If

                        If binvalue(1) = "1" Then
                            smsactive = smsactive & "<option>" & key(j) & "</option>"
                        Else
                            smsinactive = smsinactive & "<option>" & key(j) & "</option>"
                        End If

                        If binvalue(2) = "1" Then
                            popupactive = popupactive & "<option>" & key(j) & "</option>"
                        Else
                            popupinactive = popupinactive & "<option>" & key(j) & "</option>"
                        End If
                    Next

                    If emailactive = "" Then
                        emailactive = "<option>None</option>"
                    End If
                    If emailinactive = "" Then
                        emailinactive = "<option>None</option>"
                    End If

                    If smsactive = "" Then
                        smsactive = "<option>None</option>"
                    End If
                    If smsinactive = "" Then
                        smsinactive = "<option>None</option>"
                    End If

                    If popupactive = "" Then
                        popupactive = "<option>None</option>"
                    End If
                    If popupinactive = "" Then
                        popupinactive = "<option>None</option>"
                    End If

                    r(5) = "<select id= 'cbxemail' style='width: 105px;'><optgroup label='Active' style='background-image :url(images/right_tick.gif);background-repeat:no-repeat;background-position:right top;'>" & emailactive & "</optgroup><optgroup label='InActive' style='background-image :url(images/cross_tick.gif);background-repeat:no-repeat;background-position:right top;'>" & emailinactive & "</optgroup></select>"
                    r(6) = "<select id= 'cbxsms' style='width: 105px;'><optgroup label='Active' style='background-image :url(images/right_tick.gif);background-repeat:no-repeat;background-position:right top;'>" & smsactive & "</optgroup><optgroup label='InActive' style='background-image :url(images/cross_tick.gif);background-repeat:no-repeat;background-position:right top;' >" & smsinactive & "</optgroup></select>"
                    r(7) = "<select id= 'cbxpopup' style='width: 105px;'><optgroup label='Active' style='background-image :url(images/right_tick.gif);background-repeat:no-repeat;background-position:right top;'>" & popupactive & "</optgroup><optgroup label='InActive' style='background-image :url(images/cross_tick.gif);background-repeat:no-repeat;background-position:right top;'>" & popupinactive & "</optgroup></select>"
                    t.Rows.Add(r)
                    i = i + 1
                End While

                conn.Close()
            End If
            If t.Rows.Count = 0 Then
                'No Records Found
                r = t.NewRow
                r(0) = "<input type=""checkbox"" name=""chk"" />"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                r(5) = "--"
                r(6) = "--"
                r(7) = "--"
                t.Rows.Add(r)
            End If

            GridView1.DataSource = t
            GridView1.DataBind()

        Catch ex As Exception
            ex.Message.ToString()
        End Try
    End Sub

    Protected Sub DeleteDriver()
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim command As SqlCommand
            Dim pno() As String = Request.Form("chk").Split(",")

            For i As Int32 = 0 To pno.Length - 1
                command = New SqlCommand("delete from instant_alert_settings where plateno='" & pno(i) & "'", conn)
                Try
                    conn.Open()
                    command.ExecuteNonQuery()
                Catch ex As Exception
                Finally
                    conn.Close()
                End Try
            Next
            FillGrid()
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub delete2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles delete2.Click
        DeleteDriver()
    End Sub

    Protected Sub delete1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles delete1.Click
        DeleteDriver()
    End Sub

    Protected Sub ddluser_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddluser.SelectedIndexChanged
        FillGrid()
    End Sub

End Class

