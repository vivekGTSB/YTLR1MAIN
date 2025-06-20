Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.Net

Partial Class GetServiceData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Dim code As Integer = 0
        Try
            code = Convert.ToInt32(Request.QueryString("i"))
        Catch ex As Exception

        End Try
        'Request.SaveAs("D:\wwwroot\Lafarge\LafargeBeta\ReqSave.txt", True)

        Select Case code
            Case 1
                Dim userid As String = Request.Form("ugData")
                Dim role As String = Request.Form("role")
                Dim userslist As String = Request.Form("userslist")
                Response.Write(GetData(userid, role, userslist))
                Response.ContentType = "text/plain"
            Case 2
                Dim userslist As String = Request.Form("ugData")
                Response.Write(DeleteRecord(userslist))
            Case 3
                Dim userid As String = Request.Form("userid")
                Dim plateno As String = Request.Form("plateno")
                Dim hodo As String = Request.Form("hodo")
                Dim htime As String = Request.Form("htime")
                Dim odolimit As String = Request.Form("odolimit")
                Dim timelimit As String = Request.Form("timelimit")
                Dim enginelimit As String = Request.Form("enginelimit")
                Dim emailid1 As String = Request.Form("emailid1")
                Dim emailid2 As String = Request.Form("emailid2")
                Dim mobile1 As String = Request.Form("mobile1")
                Dim mobile2 As String = Request.Form("mobile2")
                Dim id As String = Request.Form("id")
                Dim remarks As String = Request.Form("remarks")
                Response.Write(UpdateData(userid, plateno, hodo, htime, odolimit, timelimit, enginelimit, emailid1, emailid2, mobile1, mobile2, id, remarks))
            Case 4
                Dim userid As String = Request.Form("userId")
                Response.Write(LoadGrouplist(userid))
            Case 5
                Dim userid As String = Request.Form("userId")
                Response.Write(LoadGrouplist1(userid))
            Case 6
                Dim userid As String = Request.Form("userId")
                Response.Write(LoadGrouplist2(userid))
            Case Else

        End Select
    End Sub

    Private Function GetData(ByVal ugData As String, ByVal role As String, ByVal userslist As String) As String
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Try
            Dim suserid As String = ugData

            Dim servicingTable As New DataTable
            servicingTable.Columns.Add(New DataColumn("serviceid"))
            servicingTable.Columns.Add(New DataColumn("plateno"))
            servicingTable.Columns.Add(New DataColumn("userid"))
            servicingTable.Columns.Add(New DataColumn("lastservicingdate"))
            servicingTable.Columns.Add(New DataColumn("lastservicingodometer"))

            servicingTable.Columns.Add(New DataColumn("OdometerLimit"))
            servicingTable.Columns.Add(New DataColumn("TimeLimit"))
            servicingTable.Columns.Add(New DataColumn("EngineLimit"))

            servicingTable.Columns.Add(New DataColumn("emailid1"))
            servicingTable.Columns.Add(New DataColumn("emailid2"))
            servicingTable.Columns.Add(New DataColumn("mobile1"))
            servicingTable.Columns.Add(New DataColumn("mobile2"))
            servicingTable.Columns.Add(New DataColumn("remarks"))

            Dim r As DataRow
            If Not suserid = "--Select User Name--" Then
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select vt.userid,vt.plateno,serviceid,engineLimit, lastservicingdate,lastservicingodometer,odometerlimit,timelimit,emailid1,emailid2,mobile1,mobile2,case when CHARINDEX(CHAR(10), remarks)>0 then  REPLACE(remarks, char(10), ' ') else remarks end as remarks from (select * from vehicleTBL where userid='" & suserid & "') vt left outer join (select * from  servicing ) st on st.plateno=vt.plateno   order by plateno,lastservicingdate desc", conn)
                Dim dr As SqlDataReader
                If suserid = "--AllUsers--" Then
                    cmd = New SqlCommand("select vt.userid,vt.plateno,serviceid, lastservicingdate,engineLimit,lastservicingodometer,odometerlimit,timelimit,emailid1,emailid2,mobile1,mobile2,case when CHARINDEX(CHAR(10), remarks)>0 then  REPLACE(remarks, char(10), ' ') else remarks end as remarks from (select * from vehicleTBL where userid in (select userid from userTBL where role='User')) vt left outer join (select * from  servicing ) st on st.plateno=vt.plateno   order by plateno,lastservicingdate desc", conn)
                    If role = "User" Then
                        cmd = New SqlCommand("select vt.userid,vt.plateno,serviceid, lastservicingdate,engineLimit,lastservicingodometer,odometerlimit,timelimit,emailid1,emailid2,mobile1,mobile2,case when CHARINDEX(CHAR(10), remarks)>0 then  REPLACE(remarks, char(10), ' ') else remarks end as remarks from (select * from vehicleTBL where userid='" & suserid & "') vt left outer join (select * from  servicing ) st on st.plateno=vt.plateno   order by plateno,lastservicingdate desc", conn)
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select vt.userid,vt.plateno,serviceid, lastservicingdate,engineLimit,lastservicingodometer,odometerlimit,timelimit,emailid1,emailid2,mobile1,mobile2,case when CHARINDEX(CHAR(10), remarks)>0 then  REPLACE(remarks, char(10), ' ') else remarks end as remarks from (select * from vehicleTBL where userid in (" & userslist & ")) vt left outer join (select * from  servicing ) st on st.plateno=vt.plateno   order by plateno,lastservicingdate desc", conn)
                    End If
                End If

                conn.Open()
                dr = cmd.ExecuteReader()

                Dim drivername As String = ""
                While dr.Read()
                    r = servicingTable.NewRow()
                    If IsDBNull(dr("serviceid")) = False Then
                        r(0) = dr("serviceid")
                        r(1) = dr("plateno")
                        r(2) = dr("userid")

                        r(3) = Convert.ToDateTime(dr("lastservicingdate")).ToString("yyyy/MM/dd")
                        r(4) = System.Convert.ToDouble(dr("lastservicingodometer")).ToString("0.00")


                        If IsDBNull(dr("odometerlimit")) Then
                            r(5) = ""
                        Else
                            r(5) = dr("odometerlimit")
                        End If

                        If IsDBNull(dr("timeLimit")) Then
                            r(6) = ""
                        Else
                            r(6) = Convert.ToDateTime(dr("timeLimit")).ToString("yyyy/MM/dd")
                        End If


                        If IsDBNull(dr("engineLimit")) Then
                            r(7) = ""
                        Else
                            r(7) = dr("engineLimit")
                        End If


                        r(8) = dr("emailid1")
                        r(9) = dr("emailid2")
                        r(10) = dr("mobile1")
                        r(11) = dr("mobile2")
                        Try
                            r(12) = dr("remarks")
                        Catch ex As Exception
                            r(12) = ""
                        End Try

                    Else
                        r(0) = "--"
                        r(1) = dr("plateno")
                        r(2) = dr("userid")
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


                    End If
                    servicingTable.Rows.Add(r)


                End While
                conn.Close()
            End If

            If servicingTable.Rows.Count = 0 Then
                r = servicingTable.NewRow
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
                r(11) = "--"
                servicingTable.Rows.Add(r)
            End If
            Session("exceltable") = servicingTable
            For j As Integer = 0 To servicingTable.Rows.Count - 1
                Try
                    a = New ArrayList

                    If servicingTable.DefaultView.Item(j)("serviceid").ToString() <> "--" Then
                        a.Add("<input type=""checkbox"" name=""chk"" class=""group1"" value=""" & servicingTable.DefaultView.Item(j)("serviceid") & """/>")
                        a.Add(j + 1)
                        a.Add("<span style='cursor:pointer;text-decoration:underline;' onclick=""javascript :openPopup('" & servicingTable.DefaultView.Item(j)("userid") & "','" & servicingTable.DefaultView.Item(j)("plateno") & "','" & Convert.ToDateTime(servicingTable.DefaultView.Item(j)("lastservicingdate")).ToString("yyyy/MM/dd") & "','" & servicingTable.DefaultView.Item(j)("lastservicingodometer") & "','" & servicingTable.DefaultView.Item(j)("OdometerLimit") & "','" & servicingTable.DefaultView.Item(j)("TimeLimit") & "','" & servicingTable.DefaultView.Item(j)("EngineLimit") & "','" & servicingTable.DefaultView.Item(j)("emailid1") & "','" & servicingTable.DefaultView.Item(j)("emailid2") & "','" & servicingTable.DefaultView.Item(j)("mobile1") & "','" & servicingTable.DefaultView.Item(j)("mobile2") & "','" & servicingTable.DefaultView.Item(j)("serviceid") & "','" & servicingTable.DefaultView.Item(j)("remarks").ToString.Replace("'", "\'") & "')"">" & servicingTable.DefaultView.Item(j)("plateno").ToString.ToUpper() & "</span>")
                    Else
                        a.Add("")
                        a.Add(j + 1)
                        If servicingTable.DefaultView.Item(j)("userid") <> "--" Then
                            a.Add("<span style='cursor:pointer;text-decoration:underline;' onclick=""javascript :openPopup('" & servicingTable.DefaultView.Item(j)("userid") & "','" & servicingTable.DefaultView.Item(j)("plateno") & "','','','','','','','','','','0','')"">" & servicingTable.DefaultView.Item(j)("plateno").ToString.ToUpper() & "</span>")
                        Else
                            a.Add("--")
                        End If
                    End If
                    a.Add(servicingTable.DefaultView.Item(j)(3))
                    a.Add(servicingTable.DefaultView.Item(j)(4))
                    a.Add(servicingTable.DefaultView.Item(j)(5))

                    a.Add(servicingTable.DefaultView.Item(j)(6))
                    a.Add(servicingTable.DefaultView.Item(j)(7))
                    a.Add(servicingTable.DefaultView.Item(j)(8))
                    a.Add(servicingTable.DefaultView.Item(j)(9))
                    a.Add(servicingTable.DefaultView.Item(j)(12))
                    aa.Add(a)
                Catch ex As Exception

                End Try
            Next

            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Return json


        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Private Function DeleteRecord(ByVal ugData As String) As String
        'Return ugData
        Dim result As String = "0"
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim unitid As String = ""
            Dim groupides() As String = ugData.Split(",")
            For i As Int32 = 0 To groupides.Length - 1
                If groupides(i) = "on" Then
                Else
                    cmd = New SqlCommand("delete from servicing where serviceid='" & groupides(i) & "'", conn)
                    conn.Open()
                    result = cmd.ExecuteNonQuery().ToString()
                End If
                conn.Close()
            Next
        Catch ex As Exception
            result = ex.Message
        End Try
        Return result
    End Function

    Public Function UpdateData(ByVal userid As String, ByVal plateno As String, ByVal hodo As String, ByVal htime As String, ByVal odolimit As String, ByVal timelimit As String, ByVal enginelimit As String, ByVal emailid1 As String, ByVal emailid2 As String, ByVal mobile1 As String, ByVal mobile2 As String, ByVal id As String, ByVal remarks As String) As String
        Dim ret As String = ""
        Dim odo As Double = Convert.ToDouble(GetOdometer(Convert.ToDateTime(htime).ToString("yyyy/MM/dd 00:00:00"), plateno)).ToString("0.00")
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim updatequery As String = ""
            Dim insertquery As String = ""

            remarks = remarks.Replace("'", "''")
            If odolimit <> "" And timelimit <> "" And enginelimit <> "" Then
                insertquery = "insert into servicing(userid,plateno,lastservicingdate,lastservicingodometer,odometerlimit,timelimit,enginelimit,emailid1,emailid2,mobile1,mobile2,remarks) values('" & userid & "','" & plateno & "','" & Convert.ToDateTime(htime).ToString("yyyy/MM/dd 23:59:59") & "','" & odo & "','" & odolimit & "','" & Convert.ToDateTime(timelimit).ToString("yyyy/MM/dd 23:59:59") & "','" & enginelimit & "','" & emailid1 & "','" & emailid2 & "','" & mobile1 & "','" & mobile2 & "','" & remarks & "')"
            ElseIf odolimit <> "" And timelimit <> "" Then
                insertquery = "insert into servicing(userid,plateno,lastservicingdate,lastservicingodometer,odometerlimit,timelimit,emailid1,emailid2,mobile1,mobile2,remarks) values('" & userid & "','" & plateno & "','" & Convert.ToDateTime(htime).ToString("yyyy/MM/dd 23:59:59") & "','" & odo & "','" & odolimit & "','" & Convert.ToDateTime(timelimit).ToString("yyyy/MM/dd 23:59:59") & "','" & emailid1 & "','" & emailid2 & "','" & mobile1 & "','" & mobile2 & "','" & remarks & "')"
            ElseIf timelimit <> "" And enginelimit <> "" Then
                insertquery = "insert into servicing(userid,plateno,lastservicingdate,lastservicingodometer,timelimit,enginelimit,emailid1,emailid2,mobile1,mobile2,remarks) values('" & userid & "','" & plateno & "','" & Convert.ToDateTime(htime).ToString("yyyy/MM/dd 23:59:59") & "','" & odo & "','" & Convert.ToDateTime(timelimit).ToString("yyyy/MM/dd 23:59:59") & "','" & enginelimit & "','" & emailid1 & "','" & emailid2 & "','" & mobile1 & "','" & mobile2 & "','" & remarks & "')"
            ElseIf odolimit <> "" And enginelimit <> "" Then
                insertquery = "insert into servicing(userid,plateno,lastservicingdate,lastservicingodometer,odometerlimit,enginelimit,emailid1,emailid2,mobile1,mobile2,remarks) values('" & userid & "','" & plateno & "','" & Convert.ToDateTime(htime).ToString("yyyy/MM/dd 23:59:59") & "','" & odo & "','" & odolimit & "','" & enginelimit & "','" & emailid1 & "','" & emailid2 & "','" & mobile1 & "','" & mobile2 & "','" & remarks & "')"
            ElseIf odolimit <> "" Then
                insertquery = "insert into servicing(userid,plateno,lastservicingdate,lastservicingodometer,odometerlimit,emailid1,emailid2,mobile1,mobile2,remarks) values('" & userid & "','" & plateno & "','" & Convert.ToDateTime(htime).ToString("yyyy/MM/dd 23:59:59") & "','" & odo & "','" & odolimit & "','" & emailid1 & "','" & emailid2 & "','" & mobile1 & "','" & mobile2 & "','" & remarks & "')"
            ElseIf timelimit <> "" Then
                insertquery = "insert into servicing(userid,plateno,lastservicingdate,lastservicingodometer,timelimit,emailid1,emailid2,mobile1,mobile2,remarks) values('" & userid & "','" & plateno & "','" & Convert.ToDateTime(htime).ToString("yyyy/MM/dd 23:59:59") & "','" & odo & "','" & Convert.ToDateTime(timelimit).ToString("yyyy/MM/dd 23:59:59") & "','" & emailid1 & "','" & emailid2 & "','" & mobile1 & "','" & mobile2 & "','" & remarks & "')"
            ElseIf enginelimit <> "" Then
                insertquery = "insert into servicing(userid,plateno,lastservicingdate,lastservicingodometer,enginelimit,emailid1,emailid2,mobile1,mobile2,remarks) values('" & userid & "','" & plateno & "','" & Convert.ToDateTime(htime).ToString("yyyy/MM/dd 23:59:59") & "','" & odo & "','" & enginelimit & "','" & emailid1 & "','" & emailid2 & "','" & mobile1 & "','" & mobile2 & "','" & remarks & "')"
            End If



            If odolimit <> "" Then
                updatequery = updatequery & ",odometerlimit='" & odolimit & "'"
            End If
            If timelimit <> "" Then
                updatequery = updatequery & ",timelimit='" & timelimit & "'"
            End If
            If enginelimit <> "" Then
                updatequery = updatequery & ",enginelimit='" & enginelimit & "'"
            End If


            Dim cmd As SqlCommand = New SqlCommand("update servicing set lastservicingdate='" & Convert.ToDateTime(htime).ToString("yyyy/MM/dd 23:59:59") & "',lastservicingodometer='" & odo & "',emailid1='" & emailid1 & "',emailid2='" & emailid2 & "',mobile1='" & mobile1 & "',mobile2='" & mobile2 & "' ,remarks='" & remarks & "'" & updatequery & "   where serviceid='" & id & "'", conn)
            Try
                conn.Open()
                ret = cmd.ExecuteNonQuery().ToString()
                If ret = "0" Then
                    cmd.CommandText = insertquery
                    ret = cmd.ExecuteNonQuery().ToString()
                End If
            Catch ex As Exception
                ret = ex.Message
            Finally
                conn.Close()
            End Try
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function

    Public Shared Function GetOdometer(ByVal bdt As String, ByVal plateno As String) As Double
        Dim odometer As Double = 0.0
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("select top 1 absolute   from  vehicle_odometer where plateno like '" & plateno & "' and timestamp ='" & Convert.ToDateTime(bdt).ToString("yyyy/MM/dd") & " 00:00:00'and absolute > -1 order by timestamp ", conn)
            Try
                conn.Open()
                odometer = Convert.ToDouble(cmd.ExecuteScalar().ToString())
            Catch ex As Exception
                odometer = 0
            Finally
                conn.Close()
            End Try
        Catch ex As Exception
            odometer = 0
        End Try
        Return odometer
    End Function

    Public Function LoadGrouplist(ByVal userId As String) As String
        Dim grouplist As ArrayList = New ArrayList

        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand
            Dim dr As SqlDataReader
            If userId = 0 Then
                cmd = New SqlCommand("select plateno,userid from vehicleTBL ", conn)
            Else

                cmd = New SqlCommand("select plateno,userid from vehicleTBL where userid='" & userId & "'", conn)
            End If

            conn.Open()

            dr = cmd.ExecuteReader()

            grouplist.Clear()


            While dr.Read()
                grouplist.Add(New ListItem(dr("plateno").ToString().ToUpper(), dr("plateno").ToString()))
            End While


            conn.Close()

        Catch ex As Exception

        End Try
        Dim json As String = JsonConvert.SerializeObject(grouplist, Formatting.None)
        Return json
    End Function

    Public Function LoadGrouplist1(ByVal userId As String) As ArrayList
        Dim grouplist As ArrayList = New ArrayList
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand
            Dim dr As SqlDataReader
            If userId = 0 Then
                cmd = New SqlCommand("select plateno,userid from vehicleTBL ", conn)
            Else

                cmd = New SqlCommand("select plateno,userid from vehicleTBL where userid='" & userId & "'", conn)
            End If

            conn.Open()

            dr = cmd.ExecuteReader()

            grouplist.Clear()


            While dr.Read()
                grouplist.Add(New ListItem(dr("plateno").ToString().ToUpper(), dr("plateno").ToString()))
            End While


            conn.Close()

        Catch ex As Exception

        End Try
        Return grouplist
    End Function

    Public Shared Function LoadGrouplist2(ByVal userId As String) As String
        Dim grouplist As ArrayList
        Dim aa As New ArrayList
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand
            Dim dr As SqlDataReader
            If userId = 0 Then
                cmd = New SqlCommand("select plateno,userid from vehicleTBL ", conn)
            Else
                cmd = New SqlCommand("select plateno,userid from vehicleTBL where userid='" & userId & "'", conn)
            End If

            conn.Open()

            dr = cmd.ExecuteReader()
            While dr.Read()
                grouplist = New ArrayList()
                grouplist.Add(New ListItem(dr("plateno").ToString().ToUpper(), dr("plateno").ToString()))
                aa.Add(grouplist)
            End While


            conn.Close()

        Catch ex As Exception

        End Try
        Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
        Return json
    End Function
End Class
