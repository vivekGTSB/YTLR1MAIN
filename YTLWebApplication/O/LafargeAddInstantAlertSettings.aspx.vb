Imports System.Data.SqlClient
Imports System.IO

Partial Class LafargeAddInstantAlertSettings
    Inherits System.Web.UI.Page

    Dim errorsb As New StringBuilder()
    Public errormessage As String = ""
    Public strcount As Int16 = 0
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Page.IsPostBack = False Then

                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                Dim userid As String = Request.Cookies("userinfo")("userid")

                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select userid,username from userTBL where role='User' order by username", conn)

                If role = "User" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ") order by username", conn)
                End If

                conn.Open()
                ddluser.Items.Clear()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                If role <> "User" Then
                    ddluser.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                End If
                While dr.Read()
                    ddluser.Items.Add(New ListItem(dr("username").ToString().ToUpper(), dr("userid")))
                End While

                cmd = New SqlCommand("select plateno from vehicleTBL where  userid='" & ddluser.SelectedValue & "' and plateno not in (select plateno from instant_alert_settings where userid='" & ddluser.SelectedValue & "') and versionid='MT' order by plateno", conn)
                dr = cmd.ExecuteReader()
                While dr.Read()
                    CheckBoxList1.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                    strcount += 1
                End While

            End If
        Catch ex As Exception

        End Try

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Server.Transfer("Login.aspx")
            End If

            If strcount >= 13 Then
                panplate.Height = 120
                panplate.ScrollBars = ScrollBars.Vertical
            End If
            ImageButton2.Attributes.Add("onclick", "return mysubmit()")
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ImageButton2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
        Try

            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim userid As String = ddluser.SelectedItem.Value

            Dim strmobile As String = Nothing
            Dim stremail As String = Nothing

            If mobile1.Text <> "" Then
                strmobile = mobile1.Text & ","
            End If
            If mobile2.Text <> "" Then
                strmobile += mobile2.Text & ","
            End If
            If mobile3.Text <> "" Then
                strmobile += mobile3.Text & ","
            End If
            If mobile4.Text <> "" Then
                strmobile += mobile4.Text & ","
            End If
            If mobile5.Text <> "" Then
                strmobile += mobile5.Text & ","
            End If
            If strmobile <> "" Then
                strmobile = strmobile.Remove(strmobile.Length - 1, 1)
            End If

            If email1.Text <> "" Then
                stremail += email1.Text & ","
            End If
            If email2.Text <> "" Then
                stremail += email2.Text & ","
            End If
            If email3.Text <> "" Then
                stremail += email3.Text & ","
            End If
            If email4.Text <> "" Then
                stremail += email4.Text & ","
            End If
            If email5.Text <> "" Then
                stremail += email5.Text & ","
            End If
            If stremail <> "" Then
                stremail = stremail.Remove(stremail.Length - 1, 1)
            End If

            Dim strbin As String = Nothing
            Dim splitstrbin() As String


            Dim geofencein As Int16
            Dim geofenceout As Int16
            Dim Idling As Int16
            Dim Immobilizer As Int16
            Dim Overspeed As Int16
            Dim Panic As Int16
            Dim Powercut As Int16
            Dim Jamming As Int16

            Dim ptoon As Int16

            Dim bits As String = ""

            If Request.Form("chk") <> Nothing Then

                Dim chkno() As String = Request.Form("chk").Split(",")
                Dim j As Int32 = 0

                For i As Int32 = 0 To 26
                    If chkno(j) = i Then
                        bits = bits & "1"
                        'strbin = strbin & "1"
                        If j < chkno.Length - 1 Then
                            j = j + 1
                        End If
                    Else
                        bits = bits & "0"
                        'strbin = strbin & "0"
                    End If

                    If i Mod 3 = 2 Then
                        bits = bits(0) & bits(2) & bits(1)
                        strbin = strbin & bits & ","
                        bits = ""
                    End If
                Next


                strbin = strbin.Remove(strbin.Length - 1, 1)
                splitstrbin = strbin.Split(",")

                geofencein = Convert.ToByte(splitstrbin(0), 2)
                geofenceout = Convert.ToByte(splitstrbin(1), 2)
                Idling = Convert.ToByte(splitstrbin(2), 2)
                Immobilizer = Convert.ToByte(splitstrbin(3), 2)
                Overspeed = Convert.ToByte(splitstrbin(4), 2)
                Panic = Convert.ToByte(splitstrbin(5), 2)
                Powercut = Convert.ToByte(splitstrbin(6), 2)
                ptoon = Convert.ToByte(splitstrbin(7), 2)
                Jamming = Convert.ToByte(splitstrbin(8), 2)
            End If
            conn.Open()
            Try
                For i As Int32 = 0 To CheckBoxList1.Items.Count - 1
                    Try

                        If CheckBoxList1.Items.Item(i).Selected = True Then
                            cmd = New SqlCommand("delete from instant_alert_settings where plateno='" & CheckBoxList1.Items.Item(i).Text & "'", conn)
                            cmd.ExecuteNonQuery()

                            cmd = New SqlCommand("insert into instant_alert_settings(userid,plateno,mobileno,emailid,alarm,geofencein,geofenceout,idling,immobilizer,overspeed,panic,powercut,jamming,idlingtime) values('" & userid & "','" & CheckBoxList1.Items.Item(i).Text & "','" & strmobile & "','" & stremail & "'," & ptoon & "," & geofencein & "," & geofenceout & "," & Idling & "," & Immobilizer & "," & Overspeed & "," & Panic & "," & Powercut & "," & Jamming & ",'" & txtIdlingTime.Value & "')", conn)
                            cmd.ExecuteNonQuery()
                            ' Response.Write(cmd.CommandText)
                        End If

                    Catch ex As Exception
                        'Response.Write(ex.Message)
                        errorsb.Append(ex.Message)
                    Finally
                        WriteErrors(errorsb.ToString())
                    End Try
                Next

                Response.Redirect("LafargeInstantAlertSettingsManagement.aspx")


            Catch ex As Exception

            Finally
                conn.Close()
            End Try

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ddluser_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddluser.SelectedIndexChanged
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("select plateno from vehicleTBL where  userid='" & ddluser.SelectedValue & "' and plateno not in (select plateno from instant_alert_settings where userid='" & ddluser.SelectedValue & "') and versionid='MT' order by plateno", conn)
            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                CheckBoxList1.Items.Clear()
                While dr.Read()
                    CheckBoxList1.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                    strcount += 1
                End While
            Catch ex As Exception

            Finally
                conn.Close()
            End Try

            If strcount >= 13 Then
                panplate.Height = 120
                panplate.ScrollBars = ScrollBars.Vertical
            End If

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub WriteErrors(ByVal message As String)
        Try
            If (message.Length > 0) Then
                Dim sw As New StreamWriter("D:\wwwroot\Lafarge\LafargeBeta\AddInstantAlertSettings.txt", FileMode.Append)
                sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & " - " & message)
                sw.Close()
            End If
        Catch ex As Exception

        End Try
    End Sub
End Class
