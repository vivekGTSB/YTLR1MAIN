Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetAlertInbox
    Inherits System.Web.UI.Page

    Public Shared Function GetData(ByVal BeginDate As String, ByVal EndDate As String) As String
        Try
            Dim istep As Integer
            Dim aa As New ArrayList()
            Dim a As ArrayList

            Dim Gatewayconn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("smsgtw"))
            Try
                Dim t1 As New DataTable
                t1.Columns.Add(New DataColumn("Sno"))
                t1.Columns.Add(New DataColumn("source"))
                t1.Columns.Add(New DataColumn("sequence"))
                t1.Columns.Add(New DataColumn("data"))
                t1.Columns.Add(New DataColumn("username"))
                t1.Columns.Add(New DataColumn("mobileno"))
                t1.Columns.Add(New DataColumn("PhoneNo"))
                t1.Columns.Add(New DataColumn("Server"))
                t1.Columns.Add(New DataColumn("Action"))
                t1.Columns.Add(New DataColumn("Responsetime"))

                Dim query As String = "SELECT  *  FROM alert_inbox WHERE sequence BETWEEN '" & BeginDate & " 00:00:00" & "' AND '" & EndDate & " 23:59:59" & "' and actiontaken='1'  order by sequence desc"
                 '  Return query
                Dim cmd As SqlCommand = New SqlCommand(query, Gatewayconn)
                Try
                    Gatewayconn.Open()
                    Dim dr As SqlDataReader = cmd.ExecuteReader()

                    Dim r As DataRow
                    Dim i As Int64 = 0
                    Dim strData As String
                    Dim splitData As String

                    While dr.Read
                        Dim priority As Int16 = 0
                        strData = dr("data").ToString().Replace(vbCrLf, ";")
                        splitData = strData
                        splitData = Replace(Trim(splitData), " ", ",")
                        strData = strData.Replace(Chr(13), ";")
                        strData = strData.Replace(Chr(10), ";")
                        Dim plateNo() As String = Split(splitData, ",")
                        Dim data As String = CheckPlateNoSvrTest(plateNo(0))
                        Dim UnitData() As String = Split(data, ";")
                        If UnitData(0).ToString.ToUpper().Contains("BINTANG") Then
                            r = t1.NewRow
                            r(0) = ""
                            r(1) = dr("source").ToString()
                            r(2) = String.Format("{0:yyyy/MM/dd HH:mm:ss tt}", Date.Parse(dr("sequence").ToString()))
                            r(3) = strData
                            r(8) = dr("Action")
                            If IsDBNull(dr("insertdate")) Then
                                r(9) = "-"
                            Else
                                r(9) = DateDiff(DateInterval.Minute, Convert.ToDateTime(dr("insertdate")), Convert.ToDateTime(dr("actiondate"))).ToString()
                            End If
                            If data <> "" Then
                                r(4) = UnitData(0) & " - " & UnitData(3) 'dr("username").ToString()
                                r(5) = UnitData(1) 'dr("mobileno").ToString()
                                r(6) = UnitData(2) 'dr("phoneno").ToString()
                                r(7) = UnitData(4) 'dr("CompanyName").ToString()
                            End If
                            t1.Rows.Add(r)
                        End If
                    End While

                    If t1.Rows.Count = 0 Then
                        r = t1.NewRow
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
                        t1.Rows.Add(r)
                    End If

                Catch ex As Exception
                    Return "2." & ex.Message
                Finally
                    Gatewayconn.Close()
                End Try


                If (t1.Rows.Count > 0) Then
                    For i As Integer = 0 To t1.Rows.Count - 1
                        Try
                            a = New ArrayList
                            a.Add(t1.DefaultView.Item(i)(0))
                            a.Add(t1.DefaultView.Item(i)(1))
                            a.Add(t1.DefaultView.Item(i)(2))
                            a.Add(t1.DefaultView.Item(i)(3))
                            a.Add(t1.DefaultView.Item(i)(4))
                            a.Add(t1.DefaultView.Item(i)(5))
                            a.Add(t1.DefaultView.Item(i)(6))
                            a.Add(t1.DefaultView.Item(i)(7))
                            a.Add(t1.DefaultView.Item(i)(8))
                            a.Add(t1.DefaultView.Item(i)(9))
                            aa.Add(a)
                        Catch ex As Exception
                            Return "3." & ex.Message
                        End Try
                    Next
                Else

                End If


            Catch ex As Exception
                Return "4." & ex.Message
            End Try
            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Return json.ToString()
        Catch ex As Exception
            Return "5." & ex.Message
        End Try
    End Function
    Public Shared Function CheckPlateNoSvrTest(ByVal anPlateno As String) As String
        Dim value As String = ""
        If anPlateno = "Panic" Or anPlateno = "Alert" Or anPlateno = "BUKIT" Or anPlateno = "TMN" Or anPlateno = "KOLAM" Then
            value = "-"
            Return value
        End If
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim userid As String
        Dim conn As New SqlConnection
        Try
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            cmd = New SqlCommand("select userid from vehicleTBL where plateno like '" & anPlateno & "%'", conn)
            conn.Open()
            userid = cmd.ExecuteScalar
            If userid = "" Then
                conn.Close()
            Else
                cmd = New SqlCommand("select username, mobileno,companyname,phoneno from userTBL where userid='" & userid & "'", conn)
                dr = cmd.ExecuteReader
                While dr.Read
                    value = dr("username").ToString & ";" & dr("mobileno").ToString & ";" & dr("phoneno").ToString & ";" & dr("companyname").ToString & ";" & "Lafarge"
                End While
                conn.Close()
                Return value
            End If
        Catch ex As Exception
            'Response.Write("LAFARGE=" & ex.Message & "<br/>")
        End Try
    End Function

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Response.Write(GetData(Request.QueryString("b"), Request.QueryString("e")))
        Response.ContentType = "text/plain"
    End Sub
End Class
