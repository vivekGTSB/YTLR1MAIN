Imports AspMap
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Partial Class GetViolationDailyReport
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim operation As String = Request.QueryString("op")
        Select Case operation
            Case "1"
                Response.Write(LoadVehicleGroup(Request.QueryString("uid")))
            Case "2"
                Response.Write(LoadVehicles(Request.QueryString("groupid"), Request.QueryString("uid")))
            Case "3"
                Response.Write(FillGrid(Request.QueryString("pno"), Request.QueryString("bdt"), Request.QueryString("edt"), Request.QueryString("uid"), Request.QueryString("gid")))
            Case "4"
                Response.Write(FillGridMonth(Request.QueryString("pno"), Request.QueryString("bdt"), Request.QueryString("edt"), Request.QueryString("uid"), Request.QueryString("gid")))
        End Select
        Response.ContentType = "text/plain"
    End Sub

    Public Function LoadVehicleGroup(ByVal userid As String) As String
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Dim json As String = ""
        Try
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand()
            cmd.Connection = conn
            If userid = "ALL" Then
                cmd.CommandText = "select groupname  from vehicle_group where userid in (" & userslist & ") order by groupname"

            Else
                cmd.CommandText = "select groupname  from vehicle_group where userid ='" & userid & "' order by groupname"
            End If


            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader
                While dr.Read
                    a = New ArrayList
                    a.Add(dr("groupname"))
                    a.Add(dr("groupname"))
                    aa.Add(a)
                End While
            Catch ex As Exception

            Finally
                conn.Close()
            End Try

        Catch ex As Exception

        End Try
        json = JsonConvert.SerializeObject(aa, Formatting.None)
        Return json
    End Function


    Public Function LoadVehicles(ByVal groupid As String, ByVal userid As String) As String
        Dim a As New ArrayList
        Dim json As String = ""
        Try
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim uid As String = Request.Cookies("userinfo")("userid")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand("select plateno from vehicleTBL where groupname=@groupID", conn)
            If groupid = "ALL" Then
                cmd.CommandText = "select plateno from vehicleTBL where Userid=" & userid & ""
                If role = "SuperUser" Or role = "Operator" Then
                    cmd.CommandText = "select plateno from vehicleTBL where Userid in (" & userslist & ")"
                End If
            Else
                cmd.Parameters.AddWithValue("@groupID", groupid)
            End If
            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader
                While dr.Read
                    a.Add(dr("plateno"))
                End While
            Catch ex As Exception

            Finally
                conn.Close()
            End Try

        Catch ex As Exception

        End Try
        json = JsonConvert.SerializeObject(a, Formatting.None)
        Return json
    End Function

    Public Function FillGrid(ByVal plateno As String, ByVal bdt As String, ByVal edt As String, ByVal uid As String, ByVal gid As String) As String
        Dim a As ArrayList
        Dim aa As New ArrayList()
        Dim json As String = ""
        Dim unsafeWork, unsafeDrive, totalmidnight, totalunsavework, totalunsafedriving As Integer
        Dim ovtotal, idlingtot, hbreaktot, cntdrvtot, totdrvtot, totworktot, totvio, distot, spnintytotal As Double
        Try
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim usid As String = Request.Cookies("userinfo")("userid")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand()
            cmd.Connection = conn

            Dim filter As String = ""

            If plateno = "ALL" Then
                If gid = "ALL" Then
                    If uid = "ALL" Then
                        If role = "SuperUser" Or role = "Operator" Then
                            filter = " plateno in (select plateno from vehicleTBL where userid in (" & userslist & "))"
                        Else
                            filter = " plateno in (select plateno from vehicleTBL where userid='" & uid & "')"
                        End If
                    Else
                        filter = " plateno in (select plateno from vehicleTBL where userid='" & uid & "')"
                    End If
                Else
                    filter = " plateno in (select plateno from vehicleTBL where userid='" & uid & "' and  groupname='" & gid & "') "
                End If
            Else
                filter = " plateno ='" & plateno & "' "
            End If

            cmd.CommandText = "Select plateno,timestamp ,[dbo].[fn_Getgroupname](plateno) As Grp, overspeed,idling,hdec,totalCont4HourCount,totaldriverhour,totalworkhour,distance,midnightcount,speed9095,speed95100,speed100   FROM ohsas_violation Where " & filter & "  and timestamp between '" & bdt & " 00:00:00' and '" & edt & " 23:59:59' order by timestamp,plateno"

            Dim tempts As TimeSpan
            Try

                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader
                While dr.Read
                    Dim speedaboveNinty As Integer = 0
                    speedaboveNinty = CInt(dr("speed9095")) + CInt(dr("speed95100")) + CInt(dr("speed100"))
                    a = New ArrayList()
                    a.Add(dr("plateno"))
                    a.Add(Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd"))
                    a.Add(dr("plateno"))
                    a.Add(dr("Grp"))
                    a.Add(dr("overspeed"))
                    a.Add(speedaboveNinty)
                    a.Add(dr("idling"))
                    a.Add(Convert.ToInt32(dr("hdec") / 22))

                    a.Add(dr("totalCont4HourCount"))
                    tempts = New TimeSpan(0, dr("totaldriverhour"), 0)
                    a.Add(ConvertHours(tempts.ToString()))
                    tempts = New TimeSpan(0, dr("totalworkhour"), 0)
                    a.Add(ConvertHours(tempts.ToString()))
                    If dr("totalworkhour") > 840 Then
                        a.Add(unsafeWork + 1)
                        totalunsavework += 1
                    Else
                        a.Add(0)
                    End If
                    If dr("totaldriverhour") > 600 Then
                        a.Add(unsafeDrive + 1)
                        totalunsafedriving += 1
                    Else
                        a.Add(0)
                    End If
                    a.Add(dr("overspeed") + dr("idling") + Convert.ToInt32(dr("hdec") / 22))
                    a.Add(CDbl(dr("distance")).ToString("0.00"))
                    a.Add(dr("midnightcount"))
                    ovtotal += dr("overspeed")
                    spnintytotal += speedaboveNinty
                    idlingtot += dr("idling")
                    hbreaktot += Convert.ToInt32(dr("hdec") / 22)
                    cntdrvtot += dr("totalCont4HourCount")
                    totdrvtot += dr("totaldriverhour")
                    totworktot += dr("totalworkhour")
                    totalmidnight += dr("midnightcount")

                    totvio += a(13)
                    distot += CDbl(CDbl(dr("distance")).ToString("0.00"))
                    aa.Add(a)
                End While

            Catch ex As Exception
                Response.Write(ex.Message)
                Response.Write(ex.StackTrace)
            Finally
                conn.Close()
            End Try
            If aa.Count > 0 Then
                a = New ArrayList
                a.Add(ovtotal)
                a.Add(spnintytotal)
                a.Add(idlingtot)
                a.Add(hbreaktot)

                a.Add(cntdrvtot)

                tempts = New TimeSpan(0, totdrvtot, 0)
                a.Add(ConvertHours(tempts.ToString()))

                tempts = New TimeSpan(0, totworktot, 0)
                a.Add(ConvertHours(tempts.ToString()))

                a.Add(totalunsavework)
                a.Add(totalunsafedriving)

                a.Add(totvio)
                a.Add(CDbl(distot).ToString("0.00"))
                a.Add(totalmidnight)
                aa.Add(a)

            Else
                a = New ArrayList
                a.Add("0")
                a.Add("0")
                a.Add("0")
                a.Add("0")
                a.Add("00:00:00")
                a.Add("00:00:00")
                a.Add("00:00:00")
                a.Add("0")
                a.Add("0")
                a.Add("0")
                a.Add("0")
                a.Add("0")

                aa.Add(a)
            End If
            Dim vehiclestable As New DataTable
            vehiclestable.Columns.Add(New DataColumn("S No"))
            vehiclestable.Columns.Add(New DataColumn("Violation Date"))
            vehiclestable.Columns.Add(New DataColumn("Plate No"))
            vehiclestable.Columns.Add(New DataColumn("Group"))
            vehiclestable.Columns.Add(New DataColumn("Over Speed"))
            vehiclestable.Columns.Add(New DataColumn("speed >90"))
            vehiclestable.Columns.Add(New DataColumn("Idle"))
            vehiclestable.Columns.Add(New DataColumn("Harsh Break"))
            vehiclestable.Columns.Add(New DataColumn("CntDrv"))
            vehiclestable.Columns.Add(New DataColumn("TotDrv"))
            vehiclestable.Columns.Add(New DataColumn("TotWork"))
            vehiclestable.Columns.Add(New DataColumn("Frequency Work>14 Hrs"))
            vehiclestable.Columns.Add(New DataColumn("Frequency Driving>10 Hrs"))
            vehiclestable.Columns.Add(New DataColumn("TotVio"))
            vehiclestable.Columns.Add(New DataColumn("Distance"))
            vehiclestable.Columns.Add(New DataColumn("Mid-Night Count"))
            Dim r As DataRow
            Dim count As Integer = 1
            Dim tempa As ArrayList
            For tco As Integer = 0 To aa.Count - 2
                tempa = aa(tco)
                r = vehiclestable.NewRow()
                r(0) = count
                For tempcount As Integer = 1 To 15
                    r(tempcount) = tempa(tempcount)
                Next
                vehiclestable.Rows.Add(r)
                count += 1
            Next

            HttpContext.Current.Session.Remove("tempTable")

            HttpContext.Current.Session("exceltable") = vehiclestable

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
        json = JsonConvert.SerializeObject(aa, Formatting.None)
        Return json
    End Function

    Public Function FillGridMonth(ByVal plateno As String, ByVal bdt As String, ByVal edt As String, ByVal uid As String, ByVal gid As String) As String
        Dim a As ArrayList
        Dim aa As New ArrayList()
        Dim json As String = ""
        Dim totalunsafework, totalunsafedrive, totmidnight As Integer
        Dim ovtotal, idlingtot, hbreaktot, cntdrvtot, totdrvtot, totworktot, totvio, distot, spnintytotal As Double
        Try
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim usid As String = Request.Cookies("userinfo")("userid")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand()
            cmd.Connection = conn

            Dim filter As String = ""

            If plateno = "ALL" Then
                If gid = "ALL" Then
                    If uid = "ALL" Then
                        If role = "SuperUser" Or role = "Operator" Then
                            filter = " plateno in (select plateno from vehicleTBL where userid in (" & userslist & "))"
                        Else
                            filter = " plateno in (select plateno from vehicleTBL where userid='" & uid & "')"
                        End If
                    Else
                        filter = " plateno in (select plateno from vehicleTBL where userid='" & uid & "')"
                    End If
                Else
                    filter = " plateno in (select plateno from vehicleTBL where userid='" & uid & "' and  groupname='" & gid & "') "
                End If
            Else
                filter = " plateno ='" & plateno & "' "
            End If

            cmd.CommandText = "Select plateno,[dbo].[fn_Getgroupname](plateno) As Grp, sum(overspeed) overspeed,sum(speed9095) overspeed9095,sum(speed95100) overspeed95100,sum(speed100) overspeed100,sum(idling) idling,sum(hdec) hdec,sum(totalCont4HourCount) contdrive,sum(totaldriverhour) totaldriverhour,sum(totalworkhour) totalworkhour,sum(distance) distance,sum(midnightcount) midnightcount,dbo.fnGetUnsafedrivingCount(plateno,'" & bdt & "','" & edt & "') unsafedrive,dbo.fnGetUnsafeWorkCount(plateno,'" & bdt & "','" & edt & "') unsafework   FROM ohsas_violation Where " & filter & "  and timestamp between '" & bdt & "' and '" & edt & "' group by plateno"
            ' Response.Write(cmd.CommandText)
            Dim tempts As TimeSpan
            Try

                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader
                While dr.Read
                    Dim speedaboveNinty As Integer = 0
                    speedaboveNinty = CInt(dr("overspeed9095")) + CInt(dr("overspeed95100")) + CInt(dr("overspeed100"))
                    a = New ArrayList()
                    a.Add(dr("plateno"))
                    a.Add(dr("plateno"))
                    a.Add(dr("Grp"))
                    a.Add(dr("overspeed"))
                    a.Add(speedaboveNinty)
                    a.Add(dr("idling"))
                    a.Add(Convert.ToInt32(dr("hdec") / 22))

                    a.Add(dr("contdrive"))
                    tempts = New TimeSpan(0, dr("totaldriverhour"), 0)
                    a.Add(ConvertHours(tempts.ToString()))
                    tempts = New TimeSpan(0, dr("totalworkhour"), 0)
                    a.Add(ConvertHours(tempts.ToString()))
                    a.Add(dr("unsafework"))
                    totalunsafework += dr("unsafework")
                    a.Add(dr("unsafedrive"))
                    totalunsafedrive += dr("unsafedrive")
                    a.Add(dr("overspeed") + dr("idling") + Convert.ToInt32(dr("hdec") / 22))
                    a.Add(CDbl(dr("distance")).ToString("0.00"))
                    a.Add(dr("midnightcount"))
                    ovtotal += dr("overspeed")
                    spnintytotal += speedaboveNinty
                    idlingtot += dr("idling")
                    hbreaktot += Convert.ToInt32(dr("hdec") / 22)
                    cntdrvtot += dr("contdrive")
                    totdrvtot += dr("totaldriverhour")
                    totworktot += dr("totalworkhour")
                    totmidnight += dr("midnightcount")
                    totvio += a(12)
                    distot += CDbl(CDbl(dr("distance")).ToString("0.00"))
                    aa.Add(a)
                End While

            Catch ex As Exception
                Response.Write(ex.Message)
                Response.Write(ex.StackTrace)
            Finally
                conn.Close()
            End Try
            If aa.Count > 0 Then
                a = New ArrayList
                a.Add(ovtotal)
                a.Add(spnintytotal)
                a.Add(idlingtot)
                a.Add(hbreaktot)

                a.Add(cntdrvtot)

                tempts = New TimeSpan(0, totdrvtot, 0)
                a.Add(ConvertHours(tempts.ToString()))

                tempts = New TimeSpan(0, totworktot, 0)
                a.Add(ConvertHours(tempts.ToString()))

                a.Add(totalunsafework)
                a.Add(totalunsafedrive)

                a.Add(totvio)
                a.Add(CDbl(distot).ToString("0.00"))
                a.Add(totmidnight)
                aa.Add(a)

            Else
                a = New ArrayList
                a.Add("0")
                a.Add("0")
                a.Add("0")
                a.Add("0")
                a.Add("00:00:00")
                a.Add("00:00:00")
                a.Add("00:00:00")
                a.Add("0")
                a.Add("0")
                a.Add("0")
                a.Add("0")
                a.Add("0")
                aa.Add(a)
            End If
            Dim vehiclestable As New DataTable
            vehiclestable.Columns.Add(New DataColumn("S No"))
            vehiclestable.Columns.Add(New DataColumn("Plate No"))
            vehiclestable.Columns.Add(New DataColumn("Group"))
            vehiclestable.Columns.Add(New DataColumn("Over Speed"))
            vehiclestable.Columns.Add(New DataColumn("speed >90"))
            vehiclestable.Columns.Add(New DataColumn("Idle"))
            vehiclestable.Columns.Add(New DataColumn("Harsh Break"))
            vehiclestable.Columns.Add(New DataColumn("CntDrv"))
            vehiclestable.Columns.Add(New DataColumn("TotDrv"))
            vehiclestable.Columns.Add(New DataColumn("TotWork"))
            vehiclestable.Columns.Add(New DataColumn("Frequency Work>14 Hrs"))
            vehiclestable.Columns.Add(New DataColumn("Frequency Driving>10 Hrs"))
            vehiclestable.Columns.Add(New DataColumn("TotVio"))
            vehiclestable.Columns.Add(New DataColumn("Distance"))
            vehiclestable.Columns.Add(New DataColumn("Mid-Night Count"))
            Dim r As DataRow
            Dim count As Integer = 1
            Dim tempa As ArrayList
            For tco As Integer = 0 To aa.Count - 2
                tempa = aa(tco)
                r = vehiclestable.NewRow()
                r(0) = count
                For tempcount As Integer = 1 To 14
                    r(tempcount) = tempa(tempcount)
                Next
                vehiclestable.Rows.Add(r)
                count += 1
            Next

            HttpContext.Current.Session.Remove("tempTable")

            HttpContext.Current.Session("exceltable") = vehiclestable

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
        json = JsonConvert.SerializeObject(aa, Formatting.None)
        Return json
    End Function

    Protected Function ConvertHours(ByVal p_hour As String) As String
        Dim sFLD() As String
        Dim sfld2() As String
        Dim hours As String
        Dim iPos As Integer
        hours = p_hour
        iPos = p_hour.IndexOf(".")
        If iPos > 0 Then
            sFLD = p_hour.Split(".")
            sfld2 = sFLD(1).Split(":")
            hours = CStr(CInt(sFLD(0)) * 24 + CInt(sfld2(0))) & ":" & sfld2(1) & ":" & sfld2(2)
        End If
        Return hours
    End Function
End Class
