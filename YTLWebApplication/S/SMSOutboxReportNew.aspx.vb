Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Script.Services
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class SMSOutboxReportNew
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public show As Boolean = False
    Public sb1 As New StringBuilder()
    Public lng As String
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Not IsPostBack Then
                If Request.Cookies("userinfo") Is Nothing Then
                    Response.Redirect("Login.aspx")
                End If
                Dim userid As String = Request.Cookies("userinfo")("userid")
                luid.Value = userid
                Dim role As String = Request.Cookies("userinfo")("role")
                rle.Value = role
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                ulist.Value = userslist
                Dim cmd As SqlCommand
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim ds As New DataSet


                cmd = New SqlCommand("select * from userTBL where role='User' order by username", conn)

                If role = "User" Then
                    cmd = New SqlCommand("select * from userTBL where userid='" & userid & "' order by username", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select * from userTBL where role='User' and userid in (" & userslist & ") order by username", conn)
                    DropDownList1.Items.Add(New ListItem("All Users", "--All Users--"))
                Else
                    DropDownList1.Items.Add(New ListItem("All Users", "--All Users--"))
                End If

                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                While (dr.Read())
                    DropDownList1.Items.Add((New ListItem(dr("username").ToString(), dr("userid").ToString())))
                End While

                ddlplate.Items.Add(New ListItem("All Plates", "--All Plate No--"))
                If role = "User" Then
                    cmd = New SqlCommand("select * from vehicleTBL where userid='" & userid & "' order by plateno", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select * from vehicleTBL where userid in (" & userslist & ") order by plateno", conn)
                Else
                    cmd = New SqlCommand("select * from vehicleTBL order by plateno", conn)
                End If

                dr = cmd.ExecuteReader()

                While (dr.Read())
                    ddlplate.Items.Add((New ListItem(dr("plateno").ToString().ToUpper(), dr("plateno").ToString().ToUpper())))
                End While
                conn.Close()
            End If


        Catch ex As Exception

        Finally
            MyBase.OnInit(e)
        End Try

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            End If


        Catch ex As Exception

        End Try
    End Sub



    'Public Sub LoadVehicles(ByVal userId As String, ByVal luid As String, ByVal role As String, ByVal userslist As String)
    '    Dim list As ArrayList = New ArrayList
    '    Dim l As ArrayList
    '    Try
    '        Dim cmd As New SqlCommand
    '        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
    '        cmd.Connection = conn
    '        If userId <> "--All Users--" Then
    '            cmd.CommandText = "select plateno from vehicleTBL where userid='" & userId & "' order by plateno"
    '        Else
    '            If role = "SuperUser" Or role = "Operator" Then
    '                cmd.CommandText = "select plateno from vehicleTBL where userid in (" & userslist & ") order by plateno"
    '            ElseIf role = "User" Then
    '                cmd.CommandText = "select plateno from vehicleTBL where userid='" & userId & "' order by plateno"
    '            Else
    '                cmd.CommandText = "select plateno from vehicleTBL order by plateno"
    '            End If
    '        End If

    '        Try
    '            conn.Open()
    '            Dim dr As SqlDataReader = cmd.ExecuteReader
    '            While dr.Read
    '                l = New ArrayList()
    '                l.Add(dr("plateno").ToString().ToUpper())
    '                list.Add(l)
    '            End While
    '        Catch ex As Exception

    '        Finally
    '            conn.Close()
    '        End Try

    '    Catch ex As Exception

    '    End Try
    '    Dim json As String = JsonConvert.SerializeObject(list, Formatting.None)


    'End Sub


    'Public Shared Function GetSMSDetails(ByVal fromdate As String, ByVal todate As String, ByVal plateno As String, ByVal userid As String, ByVal atype As String) As String
    '    Dim aa As New ArrayList()
    '    Dim a As ArrayList
    '    Dim json As String = ""

    '    Dim i As Int16 = 0

    '    Dim sb As StringBuilder
    '    Try


    '        Dim groupcondition As String = ""
    '        Dim vehiclestable As New DataTable

    '        vehiclestable.Columns.Add(New DataColumn("Sno"))
    '        vehiclestable.Columns.Add(New DataColumn("Username"))
    '        vehiclestable.Columns.Add(New DataColumn("Plateno"))
    '        vehiclestable.Columns.Add(New DataColumn("Date"))
    '        vehiclestable.Columns.Add(New DataColumn("Alert"))
    '        vehiclestable.Columns.Add(New DataColumn("mobile"))
    '        vehiclestable.Columns.Add(New DataColumn("Message"))

    '        Dim cmd As New SqlCommand
    '        Dim r As DataRow
    '        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
    '        If atype <> "ALL" Then
    '            cmd = New SqlCommand("select ot.plateno,ot.userid,ut.username,ot.alert_type,ot.datetime,ot.message,ot.mobileno from sms_outbox ot inner join userTBL  ut on ot.userid=ut.userid where ot.userid='" & userid & "' and ot.plateno='" & plateno & "' and ot.alert_type='" + atype + "' and datetime between '" & fromdate & "' and '" & todate & "' order by datetime", conn)
    '        Else
    '            cmd = New SqlCommand("select ot.plateno,ot.userid,ut.username,ot.alert_type,ot.datetime,ot.message,ot.mobileno from sms_outbox ot inner join userTBL  ut on ot.userid=ut.userid where ot.userid='" & userid & "' and ot.plateno='" & plateno & "' and datetime between '" & fromdate & "' and '" & todate & "' order by datetime", conn)
    '        End If

    '        Try
    '            conn.Open()
    '            Dim dr As SqlDataReader = cmd.ExecuteReader
    '            Dim sno As Int16 = 1

    '            While dr.Read
    '                r = vehiclestable.NewRow
    '                r(0) = sno
    '                r(1) = dr("username")
    '                r(2) = dr("plateno")
    '                r(3) = Convert.ToDateTime(dr("datetime")).ToString("yyyy/MM/dd HH:mm:ss")
    '                Select Case dr("alert_type")
    '                    Case "0" : r(4) = "Pto On"
    '                    Case "1" : r(4) = "Immobilizer"
    '                    Case "2" : r(4) = "Overspeed"
    '                    Case "3" : r(4) = "Panic"
    '                    Case "4" : r(4) = "PowerCut"
    '                    Case "5" : r(4) = "Unlock"
    '                    Case "6" : r(4) = "Idling"
    '                    Case "7" : r(4) = "IgnitionOff"
    '                    Case "8" : r(4) = "IgnitionOn"
    '                    Case "9" : r(4) = "Overtime"
    '                    Case "10" : r(4) = "GeofenceIn"
    '                    Case "11" : r(4) = "GeofenceOut"
    '                    Case "12" : r(4) = "SignalLoss"
    '                    Case "13" : r(4) = "HarshBreaak"
    '                    Case "14" : r(4) = "TollPlaza"
    '                    Case "15" : r(4) = "Refuel"
    '                    Case "16" : r(4) = "FuelTheft"
    '                    Case "17" : r(4) = "Stop"
    '                    Case Else
    '                        r(4) = "Nothing"
    '                End Select
    '                r(5) = dr("message")
    '                r(6) = dr("mobileno")
    '                vehiclestable.Rows.Add(r)
    '                sno = sno + 1
    '            End While

    '            sb = New StringBuilder()
    '            sb.Clear()

    '            If vehiclestable.Rows.Count > 0 Then

    '                sb.Append("<thead align='left' ><tr  ><td colspan='5' style='color:rgb(35, 135, 228)' >Plate No: " & plateno & "</td></tr>")
    '                sb.Append("<tr  ><td colspan='5' style='color:rgb(35, 135, 228)' >From: " & Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd HH:mm:ss") & " " & "To: " & Convert.ToDateTime(todate).ToString("yyyy-MM-dd HH:mm:ss") & " </td></tr>")
    '                sb.Append("<tr ><th scope='col' style='width: 30px;'>S No</th><th   scope='col' style='width: 100px; '>User Name</th><th  scope='col' style='width: 30px;'>Plateno</th><th   scope='col' style='width: 100px; '>Date Time</th><th   scope='col' style='width: 70px; '>Alert Type</th><th   scope='col' style='width: 70px; '>Mobileno</th><th   scope='col'>Message</th></tr></thead>")

    '                sb.Append("<tbody>")
    '                For j As Integer = 0 To vehiclestable.Rows.Count - 1
    '                    Try
    '                        sb.Append("<tr ><td style='width:30px;'>" & vehiclestable.DefaultView.Item(j)(0) & "</td>")
    '                        sb.Append("<td style='width: 80px;'>" & vehiclestable.DefaultView.Item(j)(1) & "</td>")
    '                        sb.Append("<td style='width: 70px;'>" & vehiclestable.DefaultView.Item(j)(2) & "</td>")
    '                        sb.Append("<td style='width: 100px;'>" & vehiclestable.DefaultView.Item(j)(3) & "</td>")
    '                        sb.Append("<td style='width:70px;'>" & vehiclestable.DefaultView.Item(j)(4) & "</td>")
    '                        sb.Append("<td style='width:70px;'>" & vehiclestable.DefaultView.Item(j)(6) & "</td>")
    '                        sb.Append("<td>" & vehiclestable.DefaultView.Item(j)(5) & "</td></tr>")

    '                    Catch ex As Exception

    '                    End Try
    '                Next
    '                sb.Append("</tbody>")
    '                sb.Append("<tfoot align='left'><tr ><th style='width: 30px;'>S No</th><th   scope='col' style='width: 100px; '>User Name</th><th  scope='col' style='width: 30px;'>Plateno</th><th   scope='col' style='width: 100px; '>Date Time</th><th   scope='col' style='width: 70px; '>Alert Type</th><th   scope='col' style='width: 70px; '>Mobileno</th><th   scope='col' style='width: 70px; '>Message</th></tr></tfoot>")

    '            Else
    '                sb.Append(cmd.CommandText)
    '            End If
    '            HttpContext.Current.Session("deatails") = vehiclestable
    '        Catch ex As Exception

    '        End Try
    '    Catch ex As Exception

    '    End Try
    '    'json = JsonConvert.SerializeObject(aa, Formatting.None)

    '    Return sb.ToString()
    'End Function


    Protected Sub DropDownList1_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles DropDownList1.SelectedIndexChanged
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim role As String = Request.Cookies("userinfo")("role")
        Dim userslist As String = Request.Cookies("userinfo")("userslist")
        Try
            Dim cmd As New SqlCommand
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            cmd.Connection = conn
            If userid <> "--All Users--" Then
                cmd.CommandText = "select plateno from vehicleTBL where userid='" & userid & "' order by plateno"
            Else
                If role = "SuperUser" Or role = "Operator" Then
                    cmd.CommandText = "select plateno from vehicleTBL where userid in (" & userslist & ") order by plateno"
                ElseIf role = "User" Then
                    cmd.CommandText = "select plateno from vehicleTBL where userid='" & userid & "' order by plateno"
                Else
                    cmd.CommandText = "select plateno from vehicleTBL order by plateno"
                End If
            End If

            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader
                While dr.Read
                    ddlplate.Items.Add((New ListItem(dr("plateno").ToString().ToUpper(), dr("plateno").ToString().ToUpper())))
                End While
            Catch ex As Exception

            Finally
                conn.Close()
            End Try

        Catch ex As Exception

        End Try

    End Sub
End Class
