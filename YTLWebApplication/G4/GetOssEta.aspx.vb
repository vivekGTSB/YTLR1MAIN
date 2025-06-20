Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.Web.Script.Services
Imports System.IO.Compression
Imports AspMap
Imports System.Net
Partial Class GetOssEta
    Inherits System.Web.UI.Page
    Public connstr As String
    Public uid As Integer, urole, uuserslist As String
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            uid = userid
            Dim role As String = Request.Cookies("userinfo")("role")
            urole = role
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            uuserslist = userslist
            Response.Write(GetJson())
            Response.ContentType = "text/plain"


        Catch ex As Exception
        End Try
    End Sub
    Protected Function GetJson() As String
        Dim json As String = ""
        Dim aa As New ArrayList()
        Dim a As ArrayList

        Dim d1 As Double
        Try

            Dim groupcondition As String = ""
            Dim unitstable As New DataTable
            unitstable.Columns.Add(New DataColumn("Sno"))
            unitstable.Columns.Add(New DataColumn("DN NO"))
            unitstable.Columns.Add(New DataColumn("Plate NO"))
            unitstable.Columns.Add(New DataColumn("Unit ID"))
            unitstable.Columns.Add(New DataColumn("Group Name"))
            unitstable.Columns.Add(New DataColumn("Source"))
            unitstable.Columns.Add(New DataColumn("Weight Out Time"))
            unitstable.Columns.Add(New DataColumn("Ship To Code"))
            unitstable.Columns.Add(New DataColumn("Ship To Name"))
            unitstable.Columns.Add(New DataColumn("Distance"))
            unitstable.Columns.Add(New DataColumn("duration"))
            unitstable.Columns.Add(New DataColumn("ETA"))
            unitstable.Columns.Add(New DataColumn("Pduration"))


            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))




            Dim userid As String = HttpContext.Current.Request.Cookies("userinfo")("userid")
            Dim role As String = HttpContext.Current.Request.Cookies("userinfo")("role")
            Dim userslist As String = HttpContext.Current.Request.Cookies("userinfo")("userslist")


            Dim tankerQuery As String = "select plateno from vehicleTBL"

            '  Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            If role = "User" Then
                tankerQuery = "select plateno from vehicleTBL where userid='" & userid & "'"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                tankerQuery = "select plateno from vehicleTBL where userid in (" & userslist & ")"
            End If
            Dim shipToCodeQuery As String = "select geofencename,shiptocode from geofence where accesstype='1' order by LTRIM(geofencename)"

            Dim ds As New DataSet
            Dim da As New SqlDataAdapter(tankerQuery, conn)
            da.Fill(ds)
            Dim platecond As String = ""
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If Not IsDBNull(ds.Tables(0).Rows(i)("plateno")) Then
                    platecond = platecond & "'" & ds.Tables(0).Rows(i)("plateno") & "',"
                End If
            Next

            da = New SqlDataAdapter(shipToCodeQuery, conn)
            ds.Clear()
            da.Fill(ds)

            Dim ShipToNameDict As New Dictionary(Of Integer, String)

            For c As Integer = 0 To ds.Tables(0).Rows.Count - 1
                Try
                    If Not ShipToNameDict.ContainsKey(ds.Tables(0).Rows(c)("shiptocode")) Then
                        ShipToNameDict.Add(ds.Tables(0).Rows(c)("shiptocode"), ds.Tables(0).Rows(c)("geofencename").ToString().ToUpper())
                    End If
                Catch ex As Exception
                    ' Response.Write(ex.Message)
                End Try
            Next
            If platecond.Length > 3 Then
                platecond = platecond.Substring(0, platecond.Length - 1)
                platecond = " and plateno in (" & platecond & ")"
            End If


            Dim vehicleDict As New Dictionary(Of String, String)


            Dim cmd As New SqlCommand("select plateno,groupname from vehicleTBL where plateno <> '' " & platecond, conn)
            Dim dr As SqlDataReader

            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    Try
                        vehicleDict.Add(dr("plateno").ToString().Split("_")(0), dr("groupname"))
                    Catch ex As Exception
                        '  Response.Write(ex.Message)
                    End Try
                End While

            Catch ex As Exception
                Response.Write(ex.Message)
            Finally
                conn.Close()
            End Try



            Dim conn2 = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmdT As New SqlCommand("select distinct transporter_name, transporter_id from oss_transporter order by transporter_name ", conn2)
            Dim drT As SqlDataReader

            Dim TransNameDict As New Dictionary(Of String, Integer)
            Try
                conn2.Open()
                drT = cmdT.ExecuteReader()
                While drT.Read()
                    Try
                        TransNameDict.Add(drT("transporter_name").ToString().ToUpper(), drT("transporter_id"))
                    Catch ex As Exception
                        Response.Write(ex.Message)
                    End Try
                End While

            Catch ex As Exception
                Response.Write(ex.Message)
            Finally
                conn2.Close()
            End Try









            'Dim dr As SqlDataReader
            'Dim cmd As New SqlCommand()


            Dim bdt As String = DateTime.Now.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss")
            ' bdt = bdt & " 00:00:00"
            Dim edt As String = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            'edt = edt & " 23:59:59"

            cmd.CommandText = "select * from oss_patch_out where weight_outtime between '" & bdt & "' and '" & edt & "' and status='3'"
            cmd.Connection = conn2

            Dim r As DataRow
            Try
                conn2.Open()
                dr = cmd.ExecuteReader()
                Dim i As Int32 = 1
                Dim d11 As DateTime
                Dim d2 As DateTime
                Dim duration As Double
                While dr.Read()
                    If TransNameDict.ContainsKey(dr("transporter").ToString().ToUpper()) Then
                        r = unitstable.NewRow

                        r(0) = i.ToString()

                        r(1) = dr("dn_no")


                        r(2) = dr("plateno")
                        r(3) = dr("unitid")

                        r(4) = ""

                        If vehicleDict.ContainsKey(dr("plateno")) Then
                            r(4) = vehicleDict(dr("plateno"))
                        End If

                        r(5) = dr("source_supply")


                        Dim p = DateTime.Parse(dr("weight_outtime")).ToString("yyyy/MM/dd  HH:mm:ss")
                        p = p.Replace("-", "/")

                        r(6) = p
                        r(7) = dr("destination_siteid")


                        If ShipToNameDict.ContainsKey(dr("destination_siteid")) Then
                            r(8) = ShipToNameDict.Item(dr("destination_siteid")).ToUpper()
                        Else
                            r(8) = "--"
                        End If

                        If (IsDBNull(dr("distance")) = False) Then
                            If (dr("distance") <> "0") Then
                                d1 = dr("distance")
                                r(9) = d1.ToString("0.0")
                            End If

                        Else
                            r(9) = 0
                        End If
                        If (IsDBNull(dr("duration")) = False) Then
                            Dim iSecond As Double = dr("duration") 'Total number of seconds
                            Dim iSpan As TimeSpan = TimeSpan.FromSeconds(iSecond)

                            If dr("duration") > 3600 Then

                                r(10) = iSpan.Hours.ToString.PadLeft(2, "0"c) & " h " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " min "
                            ElseIf dr("duration") > 60 Then
                                r(10) = iSpan.Minutes.ToString.PadLeft(2, "0"c) & " min "
                            Else
                                'r(10) = iSpan.Seconds.ToString.PadLeft(2, "0"c) & " sec"
                                r(10) = 0
                            End If



                        Else
                            r(10) = 0
                        End If

                        If IsDBNull(dr("est_arrivaltime")) Then
                            r(11) = "--"
                            r(12) = "--"
                        Else
                            r(11) = Convert.ToDateTime(dr("est_arrivaltime")).ToString("yyyy/MM/dd HH:mm:ss")
                            d11 = DateTime.Parse(dr("weight_outtime"))
                            d2 = DateTime.Parse(dr("est_arrivaltime"))

                            duration = (d2 - d11).TotalSeconds
                            Dim iSpan As TimeSpan = TimeSpan.FromSeconds(duration)
                            If duration > 3600 Then

                                r(12) = iSpan.Hours.ToString.PadLeft(2, "0"c) & " h " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " min "
                            ElseIf duration > 60 Then
                                r(12) = iSpan.Minutes.ToString.PadLeft(2, "0"c) & " min "
                            Else
                                ' r(12) = iSpan.Seconds.ToString.PadLeft(2, "0"c) & " sec"
                                r(12) = 0
                            End If
                        End If










                        unitstable.Rows.Add(r)

                        i = i + 1


                    End If


                End While

            Catch ex As Exception
                WriteLog(ex.Message)
                Response.Write(ex.Message)

            Finally
                conn2.Close()
            End Try

            If unitstable.Rows.Count = 0 Then
                r = unitstable.NewRow
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
                unitstable.Rows.Add(r)
            End If



            For j As Integer = 0 To unitstable.Rows.Count - 1
                Try
                    a = New ArrayList()

                    a.Add(unitstable.DefaultView.Item(j)(0))
                    a.Add(unitstable.DefaultView.Item(j)(1))
                    a.Add(unitstable.DefaultView.Item(j)(2))
                    a.Add(unitstable.DefaultView.Item(j)(3))
                    a.Add(unitstable.DefaultView.Item(j)(4))
                    a.Add(unitstable.DefaultView.Item(j)(5))
                    a.Add(unitstable.DefaultView.Item(j)(6))
                    a.Add(unitstable.DefaultView.Item(j)(7))
                    a.Add(unitstable.DefaultView.Item(j)(8))
                    a.Add(unitstable.DefaultView.Item(j)(9))
                    a.Add(unitstable.DefaultView.Item(j)(10))
                    a.Add(unitstable.DefaultView.Item(j)(11))
                    a.Add(unitstable.DefaultView.Item(j)(12))

                    aa.Add(a)
                Catch ex As Exception
                    Response.Write(ex.Message)
                End Try
            Next


            Dim jss As New Newtonsoft.Json.JsonSerializer()
            ' json = "{""data"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
            json = JsonConvert.SerializeObject(aa, Formatting.None)
            If (HttpContext.Current.Request.Headers("Accept-Encoding").ToLower().Contains("gzip")) Then
                HttpContext.Current.Response.AppendHeader("Content-Encoding", "gzip")
                HttpContext.Current.Response.Filter = New GZipStream(HttpContext.Current.Response.Filter, CompressionMode.Compress)
            End If
        Catch ex As Exception
            WriteLog(ex.Message + " , " + ex.Message)
        End Try
        Return json
    End Function
    Protected Sub WriteLog(ByVal message As String)
        Try
            If (message.Length > 0) Then
                Dim sw As New StreamWriter(Server.MapPath("") & "\Getoss.Log.txt", FileMode.Append)
                sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & " - " & message)
                sw.Close()
            End If
        Catch ex As Exception

        End Try
    End Sub

End Class
