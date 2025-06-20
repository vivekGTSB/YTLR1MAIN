Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.Web.Script.Services
Imports System.IO.Compression
Imports AspMap
Imports System.Net
Partial Class GetOssT1
    Inherits System.Web.UI.Page
    Public connstr As String
    Public uid As Integer, urole, uuserslist As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
        Try
            connstr = System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")
            Dim conn As New SqlConnection(connstr)
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim tankerQuery As String = "select plateno from vehicleTBL"
            If role = "User" Then
                tankerQuery = "select plateno,groupname from vehicleTBL where userid='" & userid & "' order by groupname"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                tankerQuery = "select plateno,groupname from vehicleTBL where userid in (" & userslist & ") order by groupname"
            End If
            Dim shipToCodeQuery As String = "select geofencename,shiptocode,data,geofencetype,accesstype from geofence where accesstype='1' order by LTRIM(geofencename)"
            Dim ds As New DataSet
            Dim da As New SqlDataAdapter(tankerQuery, conn)
            da.Fill(ds)
            Dim platecond As String = ""
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If Not IsDBNull(ds.Tables(0).Rows(i)("plateno")) Then
                    platecond = platecond & "'" & ds.Tables(0).Rows(i)("plateno") & "',"
                End If
            Next
            Dim firstpoint As String = ""
            da = New SqlDataAdapter(shipToCodeQuery, conn)
            ds.Clear()
            da.Fill(ds)
            Dim vtr As New VehicleTrackedRecord()
            Dim ShipToNameDict1 As New Dictionary(Of String, VehicleTrackedRecord)
            Dim ShipToNameDict As New Dictionary(Of Integer, String)
            Dim tf As String = ""

            For c As Integer = 0 To ds.Tables(0).Rows.Count - 1
                Try
                    If Not ShipToNameDict1.ContainsKey(ds.Tables(0).Rows(c)("shiptocode")) Then
                        vtr = New VehicleTrackedRecord()

                        Dim data As String = ds.Tables(0).Rows(c)("data")
                        Dim ptslayer As New AspMap.Points
                        Dim shp As New AspMap.Shape
                        shp.ShapeType = ShapeType.mcPolygonShape
                        firstpoint = ""
                        Dim pots() As String = data.Split(";")
                        Dim vals() As String
                        For i1 As Integer = 0 To pots.Length - 1
                            vals = pots(i1).Split(",")
                            ptslayer.AddPoint(vals(0), vals(1))
                            If i1 = 0 Then
                                firstpoint = Convert.ToDouble(vals(1)).ToString("0.0000") & "," & Convert.ToDouble(vals(0)).ToString("0.0000")
                            End If
                        Next
                        shp.AddPart(ptslayer)

                        vtr.plateno = ds.Tables(0).Rows(c)("geofencename").ToString().ToUpper()
                        vtr.centroiod = firstpoint
                        ShipToNameDict1.Add(ds.Tables(0).Rows(c)("shiptocode"), vtr)
                        'ShipToNameDict.Add(ds.Tables(0).Rows(c)("shiptocode"), ds.Tables(0).Rows(c)("geofencename").ToString().ToUpper())
                    End If
                Catch ex As Exception

                End Try
            Next


            If platecond.Length > 3 Then
                platecond = platecond.Substring(0, platecond.Length - 1)
                platecond = " and plateno in (" & platecond & ")"
            End If
            Dim bdt As String = Now.AddDays(-2).ToString("yyyy/MM/dd")
            bdt = bdt & " 00:00:00"
            Dim edt As String = Now.ToString("yyyy/MM/dd")
            edt = edt & " 23:59:59"
            Dim columnname As String = "weight_outtime"
            Dim vehicleDict As New Dictionary(Of String, String)
            Dim cmd As New SqlCommand("select plateno,groupname from vehicleTBL where plateno <> '' " & platecond & " order by groupname", conn)
            Dim dr As SqlDataReader
            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    Try
                        vehicleDict.Add(dr("plateno").ToString().Split("_")(0), dr("groupname"))
                    Catch ex As Exception

                    End Try
                End While

            Catch ex As Exception

            Finally

            End Try
            conn.Close()
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmdT As New SqlCommand("select distinct transporter_name, transporter_id from oss_transporter order by transporter_name ", conn)
            Dim drT As SqlDataReader

            Dim TransNameDict As New Dictionary(Of String, Integer)
            Try
                conn.Open()
                drT = cmdT.ExecuteReader()
                While drT.Read()
                    Try
                        TransNameDict.Add(drT("transporter_name").ToString().ToUpper(), drT("transporter_id"))
                    Catch ex As Exception

                    End Try
                End While

            Catch ex As Exception

            Finally

            End Try
            conn.Close()

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("trid"))
            t.Columns.Add(New DataColumn("pno"))
            t.Columns.Add(New DataColumn("plNO"))
            t.Columns.Add(New DataColumn("sc"))
            t.Columns.Add(New DataColumn("wt"))
            t.Columns.Add(New DataColumn("ds"))
            t.Columns.Add(New DataColumn("dno"))
            t.Columns.Add(New DataColumn("dqty"))
            t.Columns.Add(New DataColumn("ddr"))
            t.Columns.Add(New DataColumn("atm"))
            t.Columns.Add(New DataColumn("st"))
            t.Columns.Add(New DataColumn("sts"))
            t.Columns.Add(New DataColumn("dnno"))
            t.Columns.Add(New DataColumn("dnqty"))
            t.Columns.Add(New DataColumn("dur"))
            t.Columns.Add(New DataColumn("dist"))
            t.Columns.Add(New DataColumn("duration"))
            t.Columns.Add(New DataColumn("est_arrivaltime"))
            Dim r As DataRow
            Dim d1 As Double
            Try
                cmd = New SqlCommand("select * from oss_patch_out where " & columnname & " between '" & bdt & "' and '" & edt & "'  " & platecond & "   order by status", conn)
                Dim dst As New DataSet
                Dim dat As SqlDataAdapter = New SqlDataAdapter(cmd)
                dat.Fill(dst)
                conn.Open()
                dr = cmd.ExecuteReader()
                Dim i As Int32 = 1
                While dr.Read()
                    Try
                        If TransNameDict.ContainsKey(dr("transporter").ToString().ToUpper()) Then
                            r = t.NewRow()
                            Dim requiredDate As DateTime = dr("weight_outtime")
                            r(0) = TransNameDict.Item(dr("transporter").ToString().ToUpper()) & requiredDate.ToString("dd")

                            r(1) = dr("patch_no")
                            r(2) = dr("plateno")

                            r(3) = dr("source_supply")
                            Dim p = DateTime.Parse(dr("weight_outtime")).ToString("yyyy/MM/dd  HH:mm:ss")
                            p = p.Replace("-", "/")

                            r(4) = p
                            r(5) = dr("destination_siteid")

                            Try
                                If ShipToNameDict1.ContainsKey(dr("destination_siteid")) Then
                                    Dim a1 = ShipToNameDict1.Item(dr("destination_siteid"))
                                    r(6) = a1.plateno
                                    r(7) = a1.centroiod
                                Else
                                    r(6) = "--"
                                    r(7) = "--"
                                End If
                            Catch ex As Exception

                            End Try
                            r(8) = Convert.ToInt64(dr("dn_no"))

                            r(9) = Convert.ToInt64(dr("dn_qty"))
                            r(10) = Server.HtmlEncode(dr("dn_driver").ToString())

                            If IsDBNull(dr("ata_datetime")) = False Then

                                r(11) = DateTime.Parse(dr("ata_datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                            Else
                                r(11) = "--"

                            End If



                            If (r(11) <> "--") Then
                                Dim t1 As DateTime = r(4)
                                Dim t2 As DateTime = r(11)
                                Dim t3 As TimeSpan = t2 - t1
                                If t3.TotalHours() > 1 Then
                                    Dim m As Int64 = t3.TotalHours().ToString().Split(".")(1)
                                    m = m.ToString().Substring(0, 2)
                                    r(14) = t3.TotalHours().ToString().Split(".")(0) & " h " & m & " min"
                                Else
                                    r(14) = t3.TotalMinutes().ToString().Split(".")(0) & " min"
                                End If
                            Else
                                r(14) = "--"
                            End If


                            r(12) = Convert.ToInt16(dr("status"))


                            Select Case r(12)
                                Case 0
                                    r(13) = 6
                                Case 1
                                    r(13) = 1
                                Case 2
                                    r(13) = 1
                                Case 3
                                    r(13) = 4
                                Case 4
                                    r(13) = 4
                                Case 5
                                    r(13) = 4
                                Case 6
                                    r(13) = 6
                                Case 7
                                    r(13) = 2
                                Case 8
                                    r(13) = 2
                                Case 10
                                    r(13) = 3
                                Case 11
                                    r(13) = 7
                                Case 12
                                    r(13) = 2
                                Case 13
                                    r(13) = 2
                                Case 14
                                    r(13) = 5
                                Case Else
                            End Select
                            If (IsDBNull(dr("distance")) = False) Then
                                If (dr("distance") <> "0") Then
                                    d1 = dr("distance")
                                    r(15) = d1.ToString("0.0")
                                End If

                            Else
                                r(15) = "0"
                            End If
                            If (IsDBNull(dr("duration")) = False) Then
                                Dim iSecond As Double = dr("duration") 'Total number of seconds
                                Dim iSpan As TimeSpan = TimeSpan.FromSeconds(iSecond)
                                If dr("duration") >= 3600 Then
                                    r(16) = iSpan.Hours.ToString.PadLeft(2, "0"c) & " h " & iSpan.Minutes.ToString.PadLeft(2, "0"c) & " min "
                                ElseIf dr("duration") >= 60 Then
                                    r(16) = iSpan.Minutes.ToString.PadLeft(2, "0"c) & " min "
                                Else
                                    r(16) = iSpan.Seconds.ToString.PadLeft(2, "0"c) & " sec"
                                End If
                            Else
                                r(16) = 0
                            End If
                            If IsDBNull(dr("est_arrivaltime")) Then
                                r(17) = "--"
                            Else
                                r(17) = Convert.ToDateTime(dr("est_arrivaltime")).ToString("yyyy/MM/dd HH:mm:ss")
                            End If
                            t.Rows.Add(r)
                        End If



                    Catch ex As Exception
                    Finally

                    End Try
                End While
            Catch ex As Exception
            End Try


            conn.Close()
            Dim a As New ArrayList()
            Dim aa As New ArrayList()



            Dim view As New DataView(t)

            view.Sort = "dnqty ASC"
            Dim jsonsb As New StringBuilder()

            For i As Integer = 0 To view.Count - 1
                a = New ArrayList()
                a.Add(view.Item(i)(0))
                a.Add(view.Item(i)(1))
                a.Add(view.Item(i)(2))
                a.Add(view.Item(i)(3))
                a.Add(view.Item(i)(4))
                a.Add(view.Item(i)(5))
                a.Add(view.Item(i)(6))
                a.Add(view.Item(i)(7))
                a.Add(view.Item(i)(8))
                a.Add(view.Item(i)(9))
                a.Add(view.Item(i)(10))
                a.Add(view.Item(i)(11))
                a.Add(view.Item(i)(12))
                a.Add(view.Item(i)(13))
                a.Add(view.Item(i)(14))
                a.Add(view.Item(i)(15))
                a.Add(view.Item(i)(16))
                a.Add(view.Item(i)(17))
                aa.Add(a)
            Next
            If (aa.Count = 0) Then
                a = New ArrayList()
                a.Add("--")
                a.Add("--")
                a.Add("--")
                a.Add("--")
                a.Add("--")
                a.Add("--")
                a.Add("--")
                a.Add("--")
                a.Add("--")
                a.Add("--")
                a.Add("--")
                a.Add("--")
                aa.Add(a)
            End If
            Dim jss As New Newtonsoft.Json.JsonSerializer()
            json = "{""data"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
            If (HttpContext.Current.Request.Headers("Accept-Encoding").ToLower().Contains("gzip")) Then
                HttpContext.Current.Response.AppendHeader("Content-Encoding", "gzip")
                HttpContext.Current.Response.Filter = New GZipStream(HttpContext.Current.Response.Filter, CompressionMode.Compress)
            End If
        Catch ex As Exception
            WriteLog(ex.Message + " , " + ex.StackTrace)
        End Try
        Return json
    End Function
    Protected Sub WriteLog(ByVal message As String)
        Try
            If (message.Length > 0) Then
                Dim sw As New StreamWriter(Server.MapPath("") & "\GetData.Log.txt", FileMode.Append)
                sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & " - " & message)
                sw.Close()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Structure VehicleTrackedRecord
        Dim plateno As String
        Dim centroiod As String
        Dim sc As String
    End Structure

End Class
