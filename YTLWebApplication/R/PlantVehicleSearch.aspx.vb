Imports System.Data.SqlClient
Imports System.Web.Script.Services
Imports AspMap
Imports Newtonsoft.Json

Partial Class PlantVehicleSearch
    Inherits System.Web.UI.Page

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            'Dim userid As String = Request.Cookies("userinfo")("userid")
            'Dim role As String = Request.Cookies("userinfo")("role")
            'Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim cmd As SqlCommand

            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))

            cmd = New SqlCommand("select * from oss_plant_master order by PV_DisplayName", conn)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlplant.Items.Clear()
            ddlplant.Items.Add(New ListItem("Select Plant", "0"))
            While (dr.Read())
                ddlplant.Items.Add(New ListItem(dr("PV_DisplayName").ToString(), dr("PV_Physical")))
            End While
            conn.Close()
        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function GetData(ByVal Plantid As String, ByVal Type As String, ByVal kmdist As String, ByVal vtype As String) As String
        Dim res As String = ""
        Dim vlist As New List(Of DistancRes)
        Dim Distres As DistancRes
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim conn2 As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim map As New AspMap.Map
        Dim cmd, cmd2 As SqlCommand
        Dim plantGeofenceLayer As New AspMap.DynamicLayer()
        plantGeofenceLayer.LayerType = LayerType.mcPolygonLayer
        Dim ossDict As New Dictionary(Of String, OSSPlateRes)
        Dim ossplatre As OSSPlateRes
        Dim platecond As String = ""
        Dim transcond As String = ""
        Try
            cmd = New SqlCommand("select * from geofence where shiptocode='" & Plantid & "'", conn2)
            conn2.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            If dr.Read() Then

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

                Distres = New DistancRes
                Distres.distance = 0
                Distres.plateno = "Plant"
                Distres.lat = shpPoints.Centroid.Y
                Distres.lon = shpPoints.Centroid.X
                ' Distres.mark = 50
                vlist.Add(Distres)

                plantGeofenceLayer.AddShape(polygonShape, dr("geofencename").ToString().ToUpper(), dr("geofenceid"))
            End If
            If (map.AddLayer(plantGeofenceLayer)) Then
                map(0).Name = "PlantGeofenceLayer"
            End If
            dr.Close()
            conn2.Close()
            cmd.Connection = conn
            If Type = "ALL" Then
                cmd.CommandText = "select plateno,weight_outtime,status,t2.transportername,t2.internaltype from oss_patch_out  t1 left outer join oss_transporter_master t2 on t1.transporter_id=t2.transporterid where weight_outtime between '" & DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss") & "' and GetDate()  and destination_siteid<>''  order by weight_outtime desc"
            ElseIf Type = "2" Then 'External Transporters
                cmd.CommandText = "select plateno,weight_outtime,status,t2.transportername,t2.internaltype from oss_patch_out  t1 left outer join oss_transporter_master t2 on t1.transporter_id=t2.transporterid where weight_outtime between '" & DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss") & "' and GetDate()  and destination_siteid<>'' and t2.internaltype=0  order by weight_outtime desc"
            Else
                cmd.CommandText = "select plateno,weight_outtime,status,t2.transportername,t2.internaltype from oss_patch_out  t1 left outer join oss_transporter_master t2 on t1.transporter_id=t2.transporterid where weight_outtime between '" & DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss") & "' and GetDate()  and destination_siteid<>'' and t2.internaltype=1  order by weight_outtime desc"
            End If
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                Try
                    If Not ossDict.ContainsKey(dr("plateno")) Then
                        ossplatre = New OSSPlateRes()
                        ossplatre.trnsportername = dr("transportername")
                        ossplatre.status = dr("status")
                        ossplatre.type = dr("internaltype")
                        ossDict.Add(dr("plateno"), ossplatre)
                    End If
                Catch ex As Exception

                End Try
            End While

            'For Each kval As KeyValuePair(Of String, Int16) In ossDict

            'Next

            'While dr.Read()
            '    platecond = platecond & "'" & dr("plateno") & "',"
            'End While
            'If platecond.Length > 3 Then
            '    platecond = platecond.Substring(0, platecond.Length - 1)
            '    platecond = " and plateno not in (" & platecond & ")"
            'End If
            dr.Close()
        Catch ex As Exception
            Distres = New DistancRes
            Distres.distance = 0
            Distres.plateno = ex.Message
            Distres.lat = 0
            Distres.lon = 0
            ' Distres.mark = 50
            vlist.Add(Distres)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
            If conn2.State = ConnectionState.Open Then
                conn2.Close()
            End If
        End Try

        Try
            Dim loc As New Location()
            If vtype = "0" Then
                cmd2 = New SqlCommand("select lat,lon,plateno,ignition,speed,roadname,dbo.fn_GetVehicleStatus(plateno) as status,alarm,bearing,dbo.fn_GetVehicleTransporter(plateno) as transporter,dbo.fn_GetVehicleTransporterType(plateno) as ttype from vehicle_tracked2 where gps_av ='A' and plateno in (select distinct  plateno from vehicletbl where userid in(select userid from usertbl where role='User')) " & platecond & "", conn2)
            Else
                cmd2 = New SqlCommand("select lat,lon,plateno,ignition,speed,roadname,dbo.fn_GetVehicleStatus(plateno) as status,alarm,bearing,dbo.fn_GetVehicleTransporter(plateno) as transporter,dbo.fn_GetVehicleTransporterType(plateno) as ttype from vehicle_tracked2 where gps_av ='A' and plateno in (select distinct  plateno from vehicletbl where userid in(select userid from usertbl where role='User') and type =@vtype) " & platecond & "", conn2)
                cmd2.Parameters.AddWithValue("@vtype", vtype)
            End If

            conn2.Open()
            Dim dr As SqlDataReader = cmd2.ExecuteReader()
            Dim truckpoints As New AspMap.DynamicPoints
            truckpoints.Type = LayerType.mcPointLayer
            Dim ossstatus As Int16 = 0
            Dim Transportername As String = ""
            Dim Transportertype As String = ""
            Dim vehiclestatus As Boolean = False
            While dr.Read()
                vehiclestatus = False
                Try
                    If ossDict.ContainsKey(dr("plateno")) Then
                        ossplatre = ossDict(dr("plateno"))
                        ossstatus = ossplatre.status
                        Transportername = ossplatre.trnsportername.ToString().ToUpper()
                        ' If ossplatre.type Then
                        Transportertype = dr("ttype")
                        'Else
                        '    Transportertype = "External"
                        'End If
                        If ossstatus = "7" Or ossstatus = "8" Or ossstatus = "12" Or ossstatus = "13" Then
                            vehiclestatus = True
                        End If
                        If dr("alarm") = True Then
                            vehiclestatus = True
                        Else
                            vehiclestatus = False
                        End If
                    Else
                        vehiclestatus = True
                        Transportername = dr("transporter").ToString().ToUpper()
                        Transportertype = dr("ttype")

                    End If

                    If vehiclestatus Then
                        If Not (dr("status").ToString() = "True") Then
                            truckpoints.AddPoint(Convert.ToDouble(dr("lon")), Convert.ToDouble(dr("lat")), dr("plateno").ToString().ToUpper())
                            Dim vehiclepoint As New Point
                            vehiclepoint.X = Convert.ToDouble(dr("lon"))
                            vehiclepoint.Y = Convert.ToDouble(dr("lat"))
                            ' 50KM 50000 / 111120 = 0.449
                            ' 100KM 100000 / 111120 = 0.899
                            ' 200KM 200000 / 111120 = 1.799
                            ' 300KM 300000 / 111120 = 2.699
                            Dim dist As Double = (kmdist * 1000) / 111120
                            Dim rs As AspMap.Recordset = map("PlantGeofenceLayer").SearchByDistanceEx(vehiclepoint, dist, SearchMethod.mcInside, "", True)
                            If (rs.RecordCount > 0) Then
                                Dim addresspoint As AspMap.Point = New AspMap.Point
                                addresspoint.X = rs.Shape.Centroid.X
                                addresspoint.Y = rs.Shape.Centroid.Y
                                Dim distance As Double = map.ConvertDistance(map.MeasureDistance(addresspoint, vehiclepoint), 9102, 9036)
                                Distres = New DistancRes
                                Distres.distance = distance.ToString("0")
                                Distres.trnsportername = Transportername
                                Distres.trnsportertype = Transportertype
                                Distres.plateno = dr("plateno").ToString().ToUpper()
                                Distres.ignition = dr("ignition")
                                Distres.speed = dr("speed")
                                Distres.address = loc.GetLocation(dr("lat"), dr("lon")) 'dr("roadname")
                                Distres.lat = vehiclepoint.Y
                                Distres.lon = vehiclepoint.X
                                Distres.bearing = Convert.ToInt32(dr("bearing"))
                                ' Distres.mark = 50
                                vlist.Add(Distres)
                            End If

                            'Dim rs1 As AspMap.Recordset = map("PlantGeofenceLayer").SearchByDistanceEx(vehiclepoint, 0.499, SearchMethod.mcInside, "", True)
                            'If (rs1.RecordCount > 0) Then
                            '    Dim addresspoint As AspMap.Point = New AspMap.Point
                            '    addresspoint.X = rs1.Shape.Centroid.X
                            '    addresspoint.Y = rs1.Shape.Centroid.Y

                            '    Dim distance As Double = map.ConvertDistance(map.MeasureDistance(addresspoint, vehiclepoint), 9102, 9036)
                            '    Distres = New DistancRes
                            '    Distres.distance = distance.ToString("0")
                            '    Distres.trnsportername = Transportername
                            '    Distres.trnsportertype = Transportertype
                            '    Distres.plateno = dr("plateno").ToString().ToUpper()
                            '    Distres.ignition = dr("ignition")
                            '    Distres.speed = dr("speed")
                            '    Distres.address = dr("roadname")
                            '    Distres.lat = vehiclepoint.Y
                            '    Distres.lon = vehiclepoint.X
                            '    Distres.bearing = Convert.ToInt32(dr("bearing"))
                            '    'Distres.mark = 50
                            '    If Not (vlist.Contains(Distres)) Then
                            '        'Distres.mark = 100
                            '        vlist.Add(Distres)
                            '    End If
                            'End If

                            'rs = map("PlantGeofenceLayer").SearchByDistanceEx(vehiclepoint, 1.799, SearchMethod.mcInside, "", True)
                            'If (rs1.RecordCount > 0) Then
                            '    Dim addresspoint As AspMap.Point = New AspMap.Point
                            '    addresspoint.X = rs1.Shape.Centroid.X
                            '    addresspoint.Y = rs1.Shape.Centroid.Y
                            '    Dim distance As Double = map.ConvertDistance(map.MeasureDistance(addresspoint, vehiclepoint), 9102, 9036)
                            '    Distres = New DistancRes
                            '    Distres.distance = distance.ToString("0")
                            '    Distres.trnsportername = Transportername
                            '    Distres.trnsportertype = Transportertype
                            '    Distres.plateno = dr("plateno").ToString().ToUpper()
                            '    Distres.ignition = dr("ignition")
                            '    Distres.speed = dr("speed")
                            '    Distres.address = dr("roadname")
                            '    Distres.lat = vehiclepoint.Y
                            '    Distres.lon = vehiclepoint.X
                            '    Distres.bearing = Convert.ToInt32(dr("bearing"))
                            '    'Distres.mark = 50
                            '    If Not (vlist.Contains(Distres)) Then
                            '        'Distres.mark = 100
                            '        vlist.Add(Distres)
                            '    End If
                            'End If

                            'rs = map("PlantGeofenceLayer").SearchByDistanceEx(vehiclepoint, 2.699, SearchMethod.mcInside, "", True)
                            'If (rs1.RecordCount > 0) Then
                            '    Dim addresspoint As AspMap.Point = New AspMap.Point
                            '    addresspoint.X = rs1.Shape.Centroid.X
                            '    addresspoint.Y = rs1.Shape.Centroid.Y
                            '    Dim distance As Double = map.ConvertDistance(map.MeasureDistance(addresspoint, vehiclepoint), 9102, 9036)
                            '    Distres = New DistancRes
                            '    Distres.distance = distance.ToString("0")
                            '    Distres.trnsportername = Transportername
                            '    Distres.trnsportertype = Transportertype
                            '    Distres.plateno = dr("plateno").ToString().ToUpper()
                            '    Distres.ignition = dr("ignition")
                            '    Distres.speed = dr("speed")
                            '    Distres.address = dr("roadname")
                            '    Distres.lat = vehiclepoint.Y
                            '    Distres.lon = vehiclepoint.X
                            '    Distres.bearing = Convert.ToInt32(dr("bearing"))
                            '    'Distres.mark = 50
                            '    If Not (vlist.Contains(Distres)) Then
                            '        'Distres.mark = 100
                            '        vlist.Add(Distres)
                            '    End If
                            'End If
                        End If
                    End If
                Catch ex As Exception
                    Distres = New DistancRes
                    Distres.distance = 0
                    Distres.plateno = ex.Message
                    Distres.lat = 0
                    Distres.lon = 0
                    ' Distres.mark = 50
                    vlist.Add(Distres)
                End Try

            End While

            If (map.AddLayer(truckpoints)) Then
                map(0).Name = "TruckPosition"
            End If
        Catch ex As Exception
            Distres = New DistancRes
            Distres.distance = 0
            Distres.plateno = ex.Message
            Distres.lat = 0
            Distres.lon = 0
            ' Distres.mark = 50
            vlist.Add(Distres)
        Finally
            If conn2.State = ConnectionState.Open Then
                conn2.Close()
            End If
        End Try

        Dim jss As New Newtonsoft.Json.JsonSerializer()
        Dim json As String = "{""aaData"":" & JsonConvert.SerializeObject(vlist, Formatting.None) & "}"
        Return json
    End Function

    Private Shared Function Direction(ByVal p1 As AspMap.Point, ByVal p2 As AspMap.Point) As Double
        Dim d As Double = 0

        Try
            Dim dlon As Double = p2.X - p1.X

            Dim y As Double = Math.Sin(dlon) * Math.Cos(p2.Y)
            Dim x As Double = Math.Cos(p1.X) * Math.Sin(p2.Y) - Math.Sin(p1.Y) * Math.Cos(p2.Y) * Math.Cos(dlon)

            d = Math.Atan2(y, x) * (180 / Math.PI)

            d = (d + 360) Mod 360
        Catch ex As Exception

        End Try

        Return d
    End Function

    Structure OSSPlateRes
        Public trnsportername As String
        Public status As Int16
        Public type As Boolean
    End Structure

    Structure DistancRes
        Public plateno As String
        Public trnsportername As String
        Public trnsportertype As String
        Public jobinfo As String
        Public distance As Double
        Public mark As Int16
        Public lat As Double
        Public lon As Double
        Public ignition As Boolean
        Public speed As Double
        Public idingtime As Int32
        Public address As String
        Public bearing As Int32
    End Structure

End Class