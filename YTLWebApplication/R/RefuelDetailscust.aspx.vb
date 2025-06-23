Imports System.Data.SqlClient
Imports System.Web.Script.Services
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.IO
Imports ChartDirector
Imports ASPNetMultiLanguage


Partial Class RefuelDetailscust
    Inherits System.Web.UI.Page
    Dim plateno As String
    Dim userid As String
    Dim Username As String
    Dim Begintime As String
    Dim endtime As String
    Dim Beforevolume As String
    Dim aftervolume As String
    Dim tolalltr As String
    Dim type As String
    Dim duration As String
    Public sb2 As New StringBuilder()
    Public sb3 As New StringBuilder()
    Public sb6 As New StringBuilder()

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim lat As Double
        Dim lon As Double
        userid = Request.QueryString("uid")
        plateno = Request.QueryString("plno")
        Username = Request.QueryString("un")
        Begintime = Request.QueryString("fd")
        endtime = Request.QueryString("td")
        Beforevolume = Request.QueryString("vol1")
        aftervolume = Request.QueryString("vol2")
        tolalltr = Request.QueryString("totltrs")
        lat = Request.QueryString("lat")
        lon = Request.QueryString("lon")
        duration = Request.QueryString("due")
        type = "Refuel"
        lblst.Text = Literal9.Text
        Literal19.Text = Literal21.Text
        'If (Request.QueryString("type") = "0") Then
        '    type = "Refuel"
        '    lblst.Text = Literal9.Text
        '    Literal19.Text = Literal21.Text
        'ElseIf (Request.QueryString("type") = "1") Then
        '    type = "Fuel Theft"
        '    lblst.Text = Literal10.Text
        '    Literal19.Text = Literal20.Text
        'ElseIf (Request.QueryString("type") = "2") Then
        '    type = "Jumping"
        '    lblst.Text = Literal10.Text
        '    Literal19.Text = Literal49.Text
        'End If


        lblplate.Text = plateno
        lblusername.Text = Username
        lblfrom.Text = Begintime
        lblto.Text = endtime
        lblvol1.Text = Beforevolume
        lblvol2.Text = aftervolume
        lbltot.Text = tolalltr
        lblfuelst.Text = Request.QueryString("fuelst")
        lblduration.Text = duration & " Min"
        sb2.Append("userid=")
        sb2.Append(userid)
        sb2.Append("&username=")
        sb2.Append(Username)
        sb2.Append("&plateno=")
        sb2.Append(plateno)
        sb2.Append("&day=")
        sb2.Append(Convert.ToDateTime(Begintime).ToString("yyyy/MM/dd"))
        sb2.Append("&bdt=")
        sb2.Append(Convert.ToDateTime(Begintime).ToString("yyyy/MM/dd HH:mm:ss"))
        sb2.Append("&edt=")
        sb2.Append(Convert.ToDateTime(endtime).ToString("yyyy/MM/dd HH:mm:ss"))
        sb2.Append("&volume1=")
        sb2.Append(Convert.ToDouble(Beforevolume).ToString("0.000"))
        sb2.Append("&volume2=")
        sb2.Append(Convert.ToDouble(aftervolume).ToString("0.000"))
        sb2.Append("&totalliters=")
        sb2.Append(tolalltr)
        sb2.Append("&type=")
        sb2.Append(type)



        sb3.Append("x=")
        sb3.Append(lon.ToString("0.000000"))
        sb3.Append("&y=")
        sb3.Append(lat.ToString("0.000000"))
        sb3.Append("&z=15")
        sb3.Append("&plateno=")
        sb3.Append(plateno)
        sb3.Append("&bdt=")
        sb3.Append(Convert.ToDateTime(Begintime).ToString("yyyy/MM/dd HH:mm:ss"))
        sb3.Append("&edt=")
        sb3.Append(Convert.ToDateTime(endtime).ToString("yyyy/MM/dd HH:mm:ss"))
        sb3.Append("&volume1=")
        sb3.Append(Convert.ToDouble(Beforevolume).ToString("0.000"))
        sb3.Append("&volume2=")
        sb3.Append(Convert.ToDouble(aftervolume).ToString("0.000"))
        sb3.Append("&totalliters=")
        sb3.Append(tolalltr)
        sb3.Append("&type=")
        sb3.Append(type)

        DisplayLogData()

    End Sub

    Private Sub DisplayLogData()
        Try
            Dim begindatetime As String = Convert.ToDateTime(Begintime).AddMinutes(-5).ToString("yyyy/MM/dd HH:mm:ss")
            Dim enddatetime As String = Convert.ToDateTime(endtime).AddMinutes(5).ToString("yyyy/MM/dd HH:mm:ss")
            Dim t As New DataTable

            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("DateTime"))
            t.Columns.Add(New DataColumn("AV"))
            t.Columns.Add(New DataColumn("Latitude"))
            t.Columns.Add(New DataColumn("Longitude"))
            t.Columns.Add(New DataColumn("Speed"))
            t.Columns.Add(New DataColumn("Odometer"))
            t.Columns.Add(New DataColumn("Reset"))
            t.Columns.Add(New DataColumn("Ignition"))
            t.Columns.Add(New DataColumn("C Level 1"))
            t.Columns.Add(New DataColumn("C Level 2"))
            t.Columns.Add(New DataColumn("Level 1"))
            t.Columns.Add(New DataColumn("Level 2"))
            t.Columns.Add(New DataColumn("Volume 1"))
            t.Columns.Add(New DataColumn("Volume 2"))
            t.Columns.Add(New DataColumn("Volume"))
            t.Columns.Add(New DataColumn("DateTime1", System.Type.GetType("System.DateTime")))

            'Read data from database server


            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim cmd As SqlCommand = New SqlCommand("select timestamp as datetime1,insertdate as datetime2,ignition,gps_av,lat,lon,speed,odometer,panic,powercut,unlock,reset,clevel1,clevel2,level1,level2,volume1,volume2 from vehicle_history2 where plateno ='" & plateno & "' and timestamp between '" & begindatetime & "' and '" & enddatetime & "'", conn)

            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim r As DataRow
                Dim i As Int64 = 1

                Dim volume1 As Double
                Dim volume2 As Double
                Dim volume As Double

                cmd = New SqlCommand("select * from fuel_tank_check where plateno='" & plateno & "'", conn)
                Dim tankdr As SqlDataReader

                tankdr = cmd.ExecuteReader()
                Dim Is2Tank As Boolean = False

                While tankdr.Read()
                    Try
                        If (tankdr("tankno") = "2") Then
                            Is2Tank = True
                            Exit While
                        End If
                    Catch ex As Exception

                    End Try
                End While


                While dr.Read
                    Try

                        r = t.NewRow
                        r(0) = i.ToString()
                        If dr("gps_av") = "A" Then
                            r(1) = DateTime.Parse(dr("datetime1")).ToString("yyyy/MM/dd HH:mm:ss")
                            r(2) = dr("gps_av")
                        Else
                            r(1) = "<span style='color:red;'>" & DateTime.Parse(dr("datetime1")).ToString("yyyy/MM/dd HH:mm:ss") & "</span>"
                            r(2) = "<span style='color:red;'>" & dr("gps_av") & "</span>"
                        End If

                        r(3) = Convert.ToDouble(dr("lat")).ToString("0.0000")
                        r(4) = Convert.ToDouble(dr("lon")).ToString("0.0000")
                        r(5) = Convert.ToDouble(dr("speed")).ToString("0")
                        r(6) = Convert.ToDouble(dr("odometer")).ToString("0")
                        If dr("reset") = True Then
                            r(7) = "YES"
                        Else
                            r(7) = "NO"
                        End If


                        If dr("ignition") = True Then
                            r(8) = "ON"
                        Else
                            r(8) = "OFF"
                        End If


                        r(9) = dr("clevel1")
                        r(10) = dr("clevel2")

                        r(11) = Convert.ToDouble(dr("level1")).ToString("0.00")
                        r(12) = Convert.ToDouble(dr("level2")).ToString("0.00")

                        volume1 = Convert.ToDouble(dr("volume1"))
                        volume2 = Convert.ToDouble(dr("volume2"))



                        If Is2Tank Then
                            If volume1 > -1 And volume2 > -1 Then
                                volume = volume1 + volume2
                            ElseIf volume1 > -1 And volume2 <= -1 Then
                                volume = volume1
                            ElseIf volume1 <= -1 And volume2 > -1 Then
                                volume = volume2
                            End If
                        Else
                            volume = volume1
                        End If

                        r(13) = volume1.ToString("0.00")
                        r(14) = volume2.ToString("0.00")
                        r(15) = volume.ToString("0.00")
                        r(16) = DateTime.Parse(dr("datetime1"))

                        t.Rows.Add(r)

                        i = i + 1

                    Catch ex As Exception

                    Finally

                    End Try
                End While
                cmd = New SqlCommand("select * from fuel_alert where plateno ='" & plateno & "' and  begindatetime between '" & begindatetime & "' and '" & enddatetime & "'", conn)
                Dim tdr1 As SqlDataReader = cmd.ExecuteReader()

                Dim alertrows() As DataRow

                While tdr1.Read()
                    Try
                        alertrows = t.Select("DateTime1 >='" & DateTime.Parse(tdr1("begindatetime")).ToString("yyyy/MM/dd HH:mm:ss") & "' AND '" & DateTime.Parse(tdr1("enddatetime")).ToString("yyyy/MM/dd HH:mm:ss") & "' >= DateTime1")

                        For j As Integer = 0 To alertrows.Length - 1
                            Try
                                If (tdr1("type") = "0") Then
                                    alertrows(j)(15) = "<span style=""color:white;background-color:green;""><b>" & alertrows(j)(15) & "</b><span>"
                                ElseIf (tdr1("type") = "1") Then
                                    alertrows(j)(15) = "<span style=""color:white;background-color:red;""><b>" & alertrows(j)(15) & "</b><span>"
                                ElseIf (tdr1("type") = "2") Then
                                    alertrows(j)(15) = "<span style=""color:white;background-color:blue;""><b>" & alertrows(j)(15) & "</b><span>"
                                End If
                            Catch ex As Exception

                            End Try
                        Next
                    Catch ex As Exception
                    End Try
                End While
                cmd = New SqlCommand("select * from fuel_jumping where plateno ='" & plateno & "' and  begindatetime between '" & begindatetime & "' and '" & enddatetime & "'", conn)
                Dim tdr As SqlDataReader = cmd.ExecuteReader()

                While tdr.Read()
                    Try
                        alertrows = t.Select("DateTime1 >='" & DateTime.Parse(tdr("begindatetime")).ToString("yyyy/MM/dd HH:mm:ss") & "' AND '" & DateTime.Parse(tdr("enddatetime")).ToString("yyyy/MM/dd HH:mm:ss") & "' >= DateTime1")

                        For j As Integer = 0 To alertrows.Length - 1
                            Try
                                alertrows(j)(15) = "<span style=""color:white;background-color:blue;""><b>" & alertrows(j)(15) & "</b><span>"
                            Catch ex As Exception

                            End Try
                        Next
                    Catch ex As Exception

                    End Try
                End While

                If t.Rows.Count = 0 Then
                    r = t.NewRow
                    r(0) = "--"
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

                    t.Rows.Add(r)
                End If

            Catch ex As Exception
            Finally
                conn.Close()
            End Try

            sb6.Append("<table class=""hor-minimalist-b"">")

            sb6.Append("<thead><tr><th style=""width=2px;"" class=""tl"">" & Literal12.Text & "</th><th style=""width=200px;"">" & Literal13.Text & "</th><th>AV</th><th>Speed</th><th>Odometer</th><th>Ign</th><th>" & Literal15.Text & "1</th><th>" & Literal15.Text & "2</th><th>" & Literal16.Text & "1</th><th>" & Literal16.Text & "2</th><th style=""width=35px;"" class=""tr"">" & Literal17.Text & "</th></tr></thead>")

            Dim counter As Integer = 1
            For i As Integer = 0 To t.Rows.Count - 1
                Try
                    sb6.Append("<tr><td>" & counter.ToString() & "</td><td>" & t.DefaultView.Item(i)(1) & "</td><td>" & t.DefaultView.Item(i)(2) & "</td><td align=""right"">" & t.DefaultView.Item(i)(5) & "</td><td align=""right"">" & t.DefaultView.Item(i)(6) & "</td><td align=""left"">" & t.DefaultView.Item(i)(8) & "</td><td align=""right"">" & t.DefaultView.Item(i)(11) & "</td><td align=""right"">" & t.DefaultView.Item(i)(12) & "</td><td align=""right"">" & t.DefaultView.Item(i)(13) & "</td><td align=""right"">" & t.DefaultView.Item(i)(14) & "</td><td align=""right"">" & t.DefaultView.Item(i)(15) & "</td></tr>")
                    counter += 1
                Catch ex As Exception

                End Try
            Next

            sb6.Append("<tfoot><tr><td colspan=""10"" class=""bl""></td><td class=""br"">&nbsp;</td></tr></tfoot>")

        Catch ex As Exception
        End Try

    End Sub

End Class
