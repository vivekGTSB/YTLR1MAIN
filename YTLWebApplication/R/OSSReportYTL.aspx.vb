Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports AspMap
Imports Newtonsoft.Json

Partial Class OSSReportYTL
    Inherits System.Web.UI.Page
    Public Shared ec As String = "false"
    Public show As Boolean = False
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
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
            uid.Value = Request.Cookies("userinfo")("userid")
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            End If
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand("select userid ,username  from YTLDB .dbo .userTBL where userid  not in (7144,7145,7146,7147,7148,7099,7180) and companyname  like 'ytl%' and role ='User' order by username", conn)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddltransportername.Items.Clear()
            ddltransportername.Items.Add(New ListItem("ALL", "ALL"))
            While dr.Read()
                ddltransportername.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While
            conn.Close()
        Catch ex As Exception

        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function GetData(ByVal fromd As String, ByVal tod As String, ByVal type As String, ByVal username As String, ByVal merge As Boolean) As String
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Dim userid As String = HttpContext.Current.Request.Cookies("userinfo")("userid")
        Dim role As String = HttpContext.Current.Request.Cookies("userinfo")("role")
        Dim uname As String = HttpContext.Current.Request.Cookies("userinfo")("username")
        Dim userslist As String = HttpContext.Current.Request.Cookies("userinfo")("userslist")
        Dim query As String = ""
        Try
            Dim shpfileDict As New Dictionary(Of String, AspMap.Shape)
            Dim map As New AspMap.Map
            Dim plantGeofenceLayer As New AspMap.DynamicLayer()
            plantGeofenceLayer.LayerType = LayerType.mcPolygonLayer
            Dim cmd As New SqlCommand("select shiptocode ,data  from YTLDB .dbo.geofence where shiptocode in (select PV_Plant  from oss_plant_master)", conn)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Dim counter As Integer = 0
            While dr.Read()

                Dim polygonShape As New AspMap.Shape
                polygonShape.ShapeType = ShapeType.mcPolygonShape

                Dim shpPoints As New AspMap.Points()
                Dim points() As String = dr("data").Split(";")
                Dim values() As String

                For i As Integer = 0 To points.Length - 1
                    values = points(i).Split(",")
                    If (values.Length = 2) Then
                        shpPoints.AddPoint(Convert.ToDouble(values(0)), Convert.ToDouble(values(1)))
                    End If
                Next
                polygonShape.AddPart(shpPoints)
                shpfileDict.Add(dr("shiptocode").ToString().ToUpper(), polygonShape)
                'plantGeofenceLayer.AddShape(polygonShape, dr("shiptocode").ToString().ToUpper(), dr("shiptocode"))
                'If (map.AddLayer(plantGeofenceLayer)) Then
                '    map(0).Name = dr("shiptocode").ToString().ToUpper()
                'End If

            End While

            If Not dr.IsClosed() Then
                dr.Close()
            End If
            Dim loc As New Location()
            Dim tankerQuery As String = ""
            Dim tankerQuery1 As String = ""
            If username = "ALL" Then
                'If role = "User" Then
                '    tankerQuery = "select plateno from vehicleTBL where userid='" & userid & "' and "
                'ElseIf role = "SuperUser" Or role = "Operator" Then
                '    tankerQuery = "select plateno from vehicleTBL where userid in (" & userslist & ") and "
                'Else
                tankerQuery = ""
                If type = "ALL" Then
                    tankerQuery1 = " where vt.userid in (select userid from usertbl where role='User') "
                ElseIf type = "1" Then
                    tankerQuery1 = " where vt.userid in (select userid  from YTLDB .dbo .userTBL where userid  not in (7144,7145,7146,7147,7148,7099,7180) and companyname  like 'ytl%' and role ='User') "
                Else
                    tankerQuery1 = " where vt.userid in (select userid  from userTBL where companyname not like 'ytl%' and role ='User') "
                End If


                ' End If
            Else
                tankerQuery = "t1.plateno in (select plateno from YTLDB.dbo.vehicletbl where userid='" & username & "') and "
                tankerQuery1 = " where vt.userid='" & username & "' "
            End If


            If type = "ALL" Then
                query = "select *,dbo.fn_getLastlon(t1.plateno) as lon,dbo.fn_getLastlat(t1.plateno) as lst,ytldb.dbo.fn_GetLattestRemakrs(t1.plateno) as remakrs from (select plateno,pmid ,g.shiptocode as baseplant from YTLDB.dbo.vehicletbl vt left outer join ytldb.dbo.geofence g on vt.baseplant=g.geofenceid " & tankerQuery1 & ") as T1 left outer join (select t1.plateno ,source_supply,count(t1.plateno) as counts,t2.pmid,dbo.fn_getLastTrip(t1.plateno,t1.source_supply) as lasttrip,dbo.fn_getLastDestination(t1.plateno,t1.source_supply ) as lastdestination,dbo.fn_getLastArea(t1.plateno,t1.source_supply ) as area    from oss_patch_out t1 left outer join YTLDB.dbo.vehicletbl t2 on t1.plateno =t2.plateno left outer join oss_transporter_master t3 on t1.transporter_id=t3.transporterid where " & tankerQuery & " weight_outtime between @bdt and @edt  group by t1.plateno,source_supply,t2.pmid) as T2 on t1.plateno =t2.plateno order by t1.plateno,t2.lasttrip"
            ElseIf type = "1" Then
                query = "select *,dbo.fn_getLastlon(t1.plateno) as lon,dbo.fn_getLastlat(t1.plateno) as lst,ytldb.dbo.fn_GetLattestRemakrs(t1.plateno) as remakrs from (select plateno,pmid,g.shiptocode as baseplant  from YTLDB.dbo.vehicletbl vt left outer join ytldb.dbo.geofence g on vt.baseplant=g.geofenceid " & tankerQuery1 & ") as T1 left outer join (select t1.plateno ,source_supply,count(t1.plateno) as counts,t2.pmid,dbo.fn_getLastTrip(t1.plateno,t1.source_supply) as lasttrip,dbo.fn_getLastDestination(t1.plateno,t1.source_supply ) as lastdestination,dbo.fn_getLastArea(t1.plateno,t1.source_supply ) as area    from oss_patch_out t1 left outer join YTLDB.dbo.vehicletbl t2 on t1.plateno =t2.plateno left outer join oss_transporter_master t3 on t1.transporter_id=t3.transporterid where " & tankerQuery & " weight_outtime between @bdt and @edt  and t3.internaltype =1  group by t1.plateno,source_supply,t2.pmid) as T2 on t1.plateno =t2.plateno order by t1.plateno,t2.lasttrip"
            Else
                query = "select *,dbo.fn_getLastlon(t1.plateno) as lon,dbo.fn_getLastlat(t1.plateno) as lst,ytldb.dbo.fn_GetLattestRemakrs(t1.plateno) as remakrs from (select plateno,pmid,g.shiptocode as baseplant  from YTLDB.dbo.vehicletbl vt left outer join ytldb.dbo.geofence g on vt.baseplant=g.geofenceid " & tankerQuery1 & ") as T1 left outer join (select t1.plateno ,source_supply,count(t1.plateno) as counts,t2.pmid,dbo.fn_getLastTrip(t1.plateno,t1.source_supply) as lasttrip,dbo.fn_getLastDestination(t1.plateno,t1.source_supply ) as lastdestination,dbo.fn_getLastArea(t1.plateno,t1.source_supply ) as area    from oss_patch_out t1 left outer join YTLDB.dbo.vehicletbl t2 on t1.plateno =t2.plateno left outer join oss_transporter_master t3 on t1.transporter_id=t3.transporterid where " & tankerQuery & " weight_outtime between @bdt and @edt  and t3.internaltype =0  group by t1.plateno,source_supply,t2.pmid) as T2 on t1.plateno =t2.plateno  order by t1.plateno,t2.lasttrip"
            End If
            cmd.CommandText = query
            cmd.Parameters.AddWithValue("@bdt", Convert.ToDateTime(fromd).ToString("yyyy/MM/dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@edt", Convert.ToDateTime(tod).ToString("yyyy/MM/dd HH:mm:ss"))
            dr = cmd.ExecuteReader()
            Dim icounter As Int32 = 0
            Dim enddate As DateTime = Convert.ToDateTime(tod)
            Dim ts As TimeSpan

            While dr.Read()
                a = New ArrayList()
                a.Add(icounter)
                a.Add(dr("plateno").ToString().ToUpper())
                a.Add(dr("pmid"))
                If IsDBNull(dr("source_supply")) Then
                    a.Add("-")
                Else
                    a.Add(dr("source_supply"))
                End If
                If IsDBNull(dr("counts")) Then
                    a.Add("-")
                Else
                    a.Add(dr("counts"))
                End If


                If IsDBNull(dr("lasttrip")) Then
                    a.Add("-")
                    a.Add("-")
                Else
                    a.Add(Convert.ToDateTime(dr("lasttrip")).ToString("MMMM dd"))
                    ts = enddate - Convert.ToDateTime(dr("lasttrip"))
                    a.Add(ts.TotalDays.ToString("0.0"))
                End If
                If IsDBNull(dr("lastdestination")) Then
                    a.Add("-")
                Else
                    a.Add(dr("lastdestination"))
                End If
                If IsDBNull(dr("area")) Then
                    a.Add("-")
                Else
                    a.Add(dr("area"))
                End If
                a.Add(loc.GetLocation(dr("lst"), dr("lon")))
                a.Add(loc.GetNearestTown(dr("lst"), dr("lon")))
                Dim vehiclepoint As New Point
                vehiclepoint.X = Convert.ToDouble(dr("lon"))
                vehiclepoint.Y = Convert.ToDouble(dr("lst"))
                Dim distance As Double = 0
                Try
                    If Not IsDBNull(dr("source_supply")) Then
                        If shpfileDict.ContainsKey(dr("source_supply").ToString().ToUpper()) Then
                            Dim shp As AspMap.Shape = shpfileDict(dr("source_supply").ToString().ToUpper())
                            Dim addresspoint As AspMap.Point = New AspMap.Point
                            addresspoint.X = shp.Centroid.X
                            addresspoint.Y = shp.Centroid.Y
                            distance = map.ConvertDistance(map.MeasureDistance(addresspoint, vehiclepoint), 9102, 9036)
                        End If

                    End If

                Catch ex As Exception

                End Try

                a.Add(distance.ToString("0"))
                distance = 0
                Try
                    If Not IsDBNull(dr("baseplant")) And vehiclepoint.X > 0 Then
                        If shpfileDict.ContainsKey(dr("baseplant").ToString().ToUpper()) Then
                            Dim shp As AspMap.Shape = shpfileDict(dr("baseplant").ToString().ToUpper())
                            Dim addresspoint As AspMap.Point = New AspMap.Point
                            addresspoint.X = shp.Centroid.X
                            addresspoint.Y = shp.Centroid.Y
                            distance = map.ConvertDistance(map.MeasureDistance(addresspoint, vehiclepoint), 9102, 9036)
                        End If

                    End If

                Catch ex As Exception

                End Try

                a.Add(distance.ToString("0"))

                a.Add(dr("remakrs"))
                aa.Add(a)
                icounter += 1
            End While
            If merge Then
                Dim tempaa As New ArrayList()
                Dim prevaa As ArrayList
                Dim first As Boolean = True
                Try
                    For Each a In aa
                        If first Then
                            prevaa = a
                            first = False
                            tempaa.Add(a)
                        Else
                            If a(1) = prevaa(1) Then
                                a(3) = a(3) & " & " & prevaa(3)
                                a(4) = a(4) & " & " & prevaa(4)
                            End If
                            prevaa = a
                            tempaa.Remove(prevaa)
                            tempaa.Add(a)
                        End If
                    Next
                Catch ex As Exception
                    a = New ArrayList()
                    a.Add("Tempaa Formation " & ex.Message)
                    a.Add(ex.StackTrace)
                    tempaa.Add(a)
                End Try
                aa = tempaa
            End If


            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S NO"))
            t.Columns.Add(New DataColumn("Plate NO"))
            t.Columns.Add(New DataColumn("PM Id"))
            t.Columns.Add(New DataColumn("Based"))
            t.Columns.Add(New DataColumn("Trip Done"))
            t.Columns.Add(New DataColumn("Last Trip"))
            t.Columns.Add(New DataColumn("Day"))
            t.Columns.Add(New DataColumn("Last Customer"))
            t.Columns.Add(New DataColumn("Location"))
            t.Columns.Add(New DataColumn("Current Lattest Loc"))
            t.Columns.Add(New DataColumn("Distance From Maintown"))
            t.Columns.Add(New DataColumn("Distance From Plant"))
            t.Columns.Add(New DataColumn("Distance From Base Plant"))
            t.Columns.Add(New DataColumn("Vehicle Status"))
            Try
                Dim r As DataRow
                For Each a In aa
                    r = t.NewRow()
                    For i As Integer = 0 To 12
                        r(i) = a(i)
                    Next
                    t.Rows.Add(r)
                Next
                HttpContext.Current.Session.Remove("exceltable")
                HttpContext.Current.Session.Remove("exceltable2")
                HttpContext.Current.Session("exceltable") = t

            Catch ex As Exception
                a = New ArrayList()
                a.Add(ex.Message)
                a.Add(ex.StackTrace)
                aa.Add(a)
            End Try


        Catch ex As Exception
            a = New ArrayList()
            a.Add(ex.Message)
            a.Add(ex.StackTrace)
            aa.Add(a)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Dim jss As New Newtonsoft.Json.JsonSerializer()
        Dim json As String = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
        Return json
    End Function
    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function GetUsers(ByVal type As String) As String
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Try
            Dim cmd As New SqlCommand()
            cmd.Connection = conn
            If type = "ALL" Then
                cmd.CommandText = "select userid,username from usertbl where role='User' order by username"
            ElseIf type = "1" Then
                cmd.CommandText = "select userid ,username  from userTBL where userid  not in (7144,7145,7146,7147,7148,7099,7180) and companyname  like 'ytl%' and role ='User' order by username"
            Else
                cmd.CommandText = "select userid ,username  from userTBL where companyname not like 'ytl%' and role ='User' order by username"
            End If
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            While dr.Read()
                a = New ArrayList()
                a.Add(dr("userid"))
                a.Add(dr("username"))
                aa.Add(a)
            End While
        Catch ex As Exception
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Dim jss As New Newtonsoft.Json.JsonSerializer()
        Dim json As String = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
        Return json
    End Function
    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function GetRecentRemarks(ByVal plateno As String) As String
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Try
            Dim cmd As New SqlCommand("select top 5 timestamp ,sourcename ,officeremark  from maintenance where plateno =@plateno and  status ='OSS' order by timestamp desc  ", conn)
            cmd.Parameters.AddWithValue("@plateno", plateno)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Dim counter As Integer = 1
            While dr.Read()
                a = New ArrayList()
                a.Add(counter)
                a.Add(Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd HH:mm:ss"))
                a.Add(dr("sourcename"))
                a.Add(dr("officeremark"))
                aa.Add(a)
                counter += 1
            End While
        Catch ex As Exception
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Dim jss As New Newtonsoft.Json.JsonSerializer()
        Dim json As String = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
        Return json
    End Function





End Class
