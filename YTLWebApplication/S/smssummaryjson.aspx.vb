Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class smssummaryjson
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim ddlu As String = Request.QueryString("u")
        Dim ddlp As String = Request.QueryString("ddlp")
        Dim bdt As String = Request.QueryString("bdt")
        Dim edt As String = Request.QueryString("edt")

        Dim luid As String = Request.QueryString("luid")
        Dim role As String = Request.QueryString("role")
        Dim userlist As String = Request.QueryString("userslist")
        Dim nu As String = Request.QueryString("nu")
        Dim pno As String = Request.QueryString("pno")
        Dim gna As String = Request.QueryString("gna")
        Dim una As String = Request.QueryString("una")
        Dim mdt As String = Request.QueryString("mdt")
        Dim lit As String = Request.QueryString("lit")
        Dim dur As String = Request.QueryString("dur")
        Dim gru As String = Request.QueryString("gru")
        DisplayLogInformation(ddlu, ddlp, bdt, edt, luid, role, userlist, nu, pno, gna, una, mdt, lit, dur, gru)

    End Sub

    Public Sub DisplayLogInformation(ByVal ddlu As String, ByVal ddlp As String, ByVal bdt As String, ByVal edt As String, ByVal luid As String, ByVal role As String, ByVal userslist As String, ByVal nu As String, ByVal pno As String, ByVal gna As String, ByVal una As String, ByVal mdt As String, ByVal lit As String, ByVal dur As String, ByVal gru As String)
        Dim json As String
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Try

            Dim begindatetime As String = bdt
            Dim enddatetime As String = edt
            Dim userid As String = ddlu
            Dim plateno As String = ddlp
            Dim uid As String = luid
            Dim totalcost As Double = 0
            Dim totalsms As Integer = 0
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S Nu"))
            t.Columns.Add(New DataColumn("PlateNo"))
            t.Columns.Add(New DataColumn("username"))
            t.Columns.Add(New DataColumn("PO"))
            t.Columns.Add(New DataColumn("IMMO"))
            t.Columns.Add(New DataColumn("OS"))
            t.Columns.Add(New DataColumn("PA"))
            t.Columns.Add(New DataColumn("PC"))
            t.Columns.Add(New DataColumn("UL"))
            t.Columns.Add(New DataColumn("ID"))
            t.Columns.Add(New DataColumn("IO"))
            t.Columns.Add(New DataColumn("ION"))
            t.Columns.Add(New DataColumn("OT"))
            t.Columns.Add(New DataColumn("GI"))
            t.Columns.Add(New DataColumn("GO"))
            t.Columns.Add(New DataColumn("SL"))
            t.Columns.Add(New DataColumn("HB"))
            t.Columns.Add(New DataColumn("TP"))
            t.Columns.Add(New DataColumn("RFL"))
            t.Columns.Add(New DataColumn("FD"))
            t.Columns.Add(New DataColumn("ST"))
            t.Columns.Add(New DataColumn("Others"))
            t.Columns.Add(New DataColumn("Total"))
            t.Columns.Add(New DataColumn("Cost Per SMS"))
            t.Columns.Add(New DataColumn("Total Cost"))
            t.Columns.Add(New DataColumn("userid"))
            Dim query As String = ""


            If ddlu <> "--All Users--" Then
                If ddlp = "--All Plate No--" Then
                    query = "select ot.plateno,ot.userid,ut.username, alert_type,count(message ) as count from sms_outbox ot inner join userTBL  ut on ot.userid=ut.userid where ot.userid='" & userid & "' and datetime between '" & begindatetime & "' and '" & enddatetime & "'  group by ot.plateno,ot.userid,ut.username,alert_type"
                Else
                    query = "select ot.plateno,ot.userid,ut.username, alert_type,count(message ) as count from sms_outbox ot inner join userTBL  ut on ot.userid=ut.userid where ot.userid='" & userid & "' and plateno='" & plateno & "' and datetime between '" & begindatetime & "' and '" & enddatetime & "' group by ot.plateno,ot.userid,ut.username,alert_type"
                End If
            Else
                If ddlp = "--All Plate No--" Then
                    If role = "SuperUser" Or role = "Operator" Then
                        query = "select ot.plateno,ot.userid,ut.username, alert_type,count(message ) as count from sms_outbox ot inner join userTBL  ut on ot.userid=ut.userid where ot.userid in (" & userslist & ") and datetime between '" & begindatetime & "' and '" & enddatetime & "' group by ot.plateno,ot.userid,ut.username,alert_type"
                    ElseIf role = "Admin" Then
                        query = "select ot.plateno,ot.userid,ut.username, alert_type,count(message ) as count from sms_outbox ot inner join userTBL  ut on ot.userid=ut.userid where  datetime between '" & begindatetime & "' and '" & enddatetime & "' group by ot.plateno,ot.userid,ut.username,alert_type"
                    End If
                Else
                    query = "select ot.plateno,ot.userid,ut.username, alert_type,count(message ) as count from sms_outbox ot inner join userTBL  ut on ot.userid=ut.userid where ot.plateno='" & plateno & "' and datetime between '" & begindatetime & "' and '" & enddatetime & "' group by ot.plateno,ot.userid,ut.username,alert_type"
                End If
            End If
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand(query, conn)
            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim r As DataRow
                Dim i As Int64 = 1

                Dim drow() As DataRow

                While dr.Read
                    Try

                        drow = t.Select("PlateNo='" & dr("plateno") & "'")
                        If drow.Length > 0 Then
                            If IsDBNull(dr("alert_type")) Then
                                drow(0)(21) = dr("count")
                            Else
                                Select Case dr("alert_type")
                                    Case "0" : drow(0)(3) = dr("count")
                                    Case "1" : drow(0)(4) = dr("count")
                                    Case "2" : drow(0)(5) = dr("count")
                                    Case "3" : drow(0)(6) = dr("count")
                                    Case "4" : drow(0)(7) = dr("count")
                                    Case "5" : drow(0)(8) = dr("count")
                                    Case "6" : drow(0)(9) = dr("count")
                                    Case "7" : drow(0)(10) = dr("count")
                                    Case "8" : drow(0)(11) = dr("count")
                                    Case "9" : drow(0)(12) = dr("count")
                                    Case "10" : drow(0)(13) = dr("count")
                                    Case "11" : drow(0)(14) = dr("count")
                                    Case "12" : drow(0)(15) = dr("count")
                                    Case "13" : drow(0)(16) = dr("count")
                                    Case "14" : drow(0)(17) = dr("count")
                                    Case "15" : drow(0)(18) = dr("count")
                                    Case "16" : drow(0)(19) = dr("count")
                                    Case "17" : drow(0)(20) = dr("count")
                                    Case Else
                                        drow(0)(21) = dr("count")
                                End Select
                            End If
                            totalsms = totalsms + Convert.ToInt32(dr("count"))
                            totalcost = totalcost + (Convert.ToInt32(dr("count")) * 0.2).ToString("0.00")
                            drow(0)(22) = Convert.ToInt32(drow(0)(22)) + Convert.ToInt32(dr("count"))
                            drow(0)(24) = "RM " & (Convert.ToInt32(drow(0)(22)) * 0.2).ToString("0.00")
                        Else
                            r = t.NewRow
                            r(0) = i.ToString()
                            r(1) = dr("plateno")
                            r(2) = dr("username").ToString.ToUpper()

                            If IsDBNull(dr("alert_type")) Then
                                r(21) = dr("count")
                            Else
                                Select Case dr("alert_type")
                                    Case "0" : r(3) = "" + dr("count")
                                    Case "1" : r(4) = dr("count")
                                    Case "2" : r(5) = dr("count")
                                    Case "3" : r(6) = dr("count")
                                    Case "4" : r(7) = dr("count")
                                    Case "5" : r(8) = dr("count")
                                    Case "6" : r(9) = dr("count")
                                    Case "7" : r(10) = dr("count")
                                    Case "8" : r(11) = dr("count")
                                    Case "9" : r(12) = dr("count")
                                    Case "10" : r(13) = dr("count")
                                    Case "11" : r(14) = dr("count")
                                    Case "12" : r(15) = dr("count")
                                    Case "13" : r(16) = dr("count")
                                    Case "14" : r(17) = dr("count")
                                    Case "15" : r(18) = dr("count")
                                    Case "16" : r(19) = dr("count")
                                    Case "17" : r(20) = dr("count")
                                    Case Else
                                        r(21) = dr("count")
                                End Select
                            End If

                            r(22) = dr("count")
                            r(23) = "RM 0.20"
                            r(24) = "RM " & (Convert.ToInt32(r(22)) * 0.2).ToString("0.00")
                            r(25) = dr("userid")
                            totalsms = totalsms + Convert.ToInt32(dr("count"))
                            totalcost = totalcost + (Convert.ToInt32(dr("count")) * 0.2).ToString("0.00")
                            t.Rows.Add(r)
                            i = i + 1
                        End If
                    Catch ex As Exception
                        json = "Error 1 : " & ex.Message
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
                    r(16) = "--"
                    r(17) = "--"
                    r(18) = "--"
                    r(19) = "--"
                    r(20) = "--"
                    r(21) = "--"
                    r(22) = "--"
                    r(23) = "--"
                    r(24) = "--"
                    r(25) = "--"
                    t.Rows.Add(r)
                Else
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
                    r(16) = "--"
                    r(17) = "--"
                    r(18) = "--"
                    r(19) = "--"
                    r(20) = "--"
                    r(21) = "--"
                    r(22) = totalsms
                    r(23) = "Total Cost"
                    r(24) = "RM " & totalcost.ToString("0.00")
                    r(25) = "--"
                    t.Rows.Add(r)
                End If

            Catch ex As Exception
                json = ex.Message
            Finally
                conn.Close()
            End Try


            If (t.Rows.Count > 0) Then
                For i As Integer = 0 To t.Rows.Count - 1
                    Try
                        a = New ArrayList
                        a.Add(t.DefaultView.Item(i)(0))
                        a.Add(t.DefaultView.Item(i)(1))
                        a.Add(t.DefaultView.Item(i)(2))
                        a.Add(t.DefaultView.Item(i)(3))
                        a.Add(t.DefaultView.Item(i)(4))
                        a.Add(t.DefaultView.Item(i)(5))
                        a.Add(t.DefaultView.Item(i)(6))
                        a.Add(t.DefaultView.Item(i)(7))
                        a.Add(t.DefaultView.Item(i)(8))
                        a.Add(t.DefaultView.Item(i)(9))
                        a.Add(t.DefaultView.Item(i)(10))
                        a.Add(t.DefaultView.Item(i)(11))
                        a.Add(t.DefaultView.Item(i)(12))
                        a.Add(t.DefaultView.Item(i)(13))
                        a.Add(t.DefaultView.Item(i)(14))
                        a.Add(t.DefaultView.Item(i)(15))
                        a.Add(t.DefaultView.Item(i)(16))
                        a.Add(t.DefaultView.Item(i)(17))
                        a.Add(t.DefaultView.Item(i)(18))
                        a.Add(t.DefaultView.Item(i)(19))
                        a.Add(t.DefaultView.Item(i)(20))
                        a.Add(t.DefaultView.Item(i)(21))
                        a.Add(t.DefaultView.Item(i)(22))
                        a.Add(t.DefaultView.Item(i)(23))
                        a.Add(t.DefaultView.Item(i)(24))
                        a.Add(t.DefaultView.Item(i)(25))
                        aa.Add(a)
                    Catch ex As Exception
                        json = ex.Message
                    End Try
                Next
            Else

            End If
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session.Remove("exceltable3")
            HttpContext.Current.Session.Remove("tempTable")

            HttpContext.Current.Session("exceltable") = t
            json = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.Write(json)
        Catch ex As Exception
            json = ex.Message
        End Try


    End Sub

End Class
