Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.DataRow
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports AspMap

Partial Class GetRefuelSummary
    Inherits System.Web.UI.Page
    Public Shared totrefuel As Double = 0
    Public Shared totrefuelcost As Double = 0
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Try
            Response.Write(GetJson())
            Response.ContentType = "text/plain"
        Catch ex As Exception

        End Try


    End Sub
    Protected Function GetJson() As String
        Dim json As String = ""
        Try
            Dim ddlu As String = Request.QueryString("ddlu")
            Dim ddlp As String = Request.QueryString("ddlp")
            Dim bdt As String = Request.QueryString("bdt")
            Dim edt As String = Request.QueryString("edt")
            Dim luid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim aa As New ArrayList()
            Dim a As ArrayList

            Dim vehiclepoint As New AspMap.Point
            Dim mapping As New AspMap.Map()
            Dim circle As New AspMap.Symbol
            Dim begindatetime As String = bdt
            Dim enddatetime As String = edt
            Dim userid As String = ddlu
            Dim plateno As String = ddlp
            Dim uid As String = luid
            totrefuel = 0
            totrefuelcost = 0

            Dim locObj As New Location(luid)
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("UserName"))
            t.Columns.Add(New DataColumn("PlateNo"))
            t.Columns.Add(New DataColumn("RefuelfrmTime"))
            t.Columns.Add(New DataColumn("RefueltoTime"))
            t.Columns.Add(New DataColumn("Address"))
            t.Columns.Add(New DataColumn("Refuel Start"))
            t.Columns.Add(New DataColumn("Refuel End"))
            t.Columns.Add(New DataColumn("Refuel(Ltr)"))
            t.Columns.Add(New DataColumn("Refuel Cost(RM)"))
            t.Columns.Add(New DataColumn("Actuelrefuel"))
            t.Columns.Add(New DataColumn("costperltr"))
            t.Columns.Add(New DataColumn("Actuelcost"))
            t.Columns.Add(New DataColumn("accurasyagenestrefuel"))
            t.Columns.Add(New DataColumn("acurasyagainesttankvol"))
            t.Columns.Add(New DataColumn("lat"))
            t.Columns.Add(New DataColumn("lon"))
            t.Columns.Add(New DataColumn("time"))
            t.Columns.Add(New DataColumn("userid"))
            t.Columns.Add(New DataColumn("fuelst"))
            t.Columns.Add(New DataColumn("actrefuel"))
            t.Columns.Add(New DataColumn("actcost"))
            Dim query As String = ""
            Dim condition As String = ""
            Dim query1 As String = ""

            If ddlu <> "--All Users--" Then
                If ddlp = "--All Plate No--" Then
                    query = "select distinct rt.plateno,avu.userid,avu.username,substring(convert(varchar,refuelfrom,120),1,16) as refuelfrom,substring(convert(varchar,refuelto,120),1,16) as refuelto,beforerefuel, afterrefuel,totrefuel,fuelcost,lat,lon,ft.liters ,ft.cost from refuel rt inner join (select au.userid,au.username,plateno from vehicleTBL vt left outer join userTBL au on vt.userid=au.userid where  au.userid='" & userid & "' ) avu  on  rt.plateno=avu.plateno left outer join fuel ft on ft.plateno=rt.plateno and ft.timestamp between dateadd(mi,-10,rt.refuelfrom) and dateadd(mi,10,rt.refuelto) where  rt.refuelfrom between   '" & begindatetime & "' and '" & enddatetime & "'   and rt.status=1 order by refuelfrom"
                    query1 = "select distinct au.username,ft.plateno,substring(convert(varchar,ft.timestamp,120),1,16) as timestamp,ft.stationcode,ft.liters,ft.cost from (select * from fuel where userid ='" & userid & "' ) ft left outer join   ( select ft.plateno,ft.timestamp from fuel ft inner join refuel rt on ft.plateno=rt.plateno and ft.timestamp between  dateadd(mi,-10,rt.refuelfrom) and dateadd(mi,10,rt.refuelto)  where ft.timestamp between '" & begindatetime & "' and '" & enddatetime & "' and userid<>0 ) t on ft.plateno=t.plateno and ft.timestamp=t.timestamp left outer join userTBL au on ft.userid=au.userid where ft.timestamp between '" & begindatetime & "' and '" & enddatetime & "' and ft.userid<>0  and t.plateno is NULL and ft.userid='" & userid & "'"
                Else
                    query = "select distinct rt.plateno,avu.userid,avu.username,substring(convert(varchar,refuelfrom,120),1,16) as refuelfrom,substring(convert(varchar,refuelto,120),1,16) as refuelto,beforerefuel, afterrefuel,totrefuel,fuelcost,lat,lon,ft.liters ,ft.cost from refuel rt inner join (select au.userid,au.username,plateno from vehicleTBL vt left outer join userTBL au on vt.userid=au.userid where  au.userid='" & userid & "' and plateno='" & plateno & "') avu  on rt.plateno=avu.plateno left outer join fuel ft on ft.plateno=rt.plateno and ft.timestamp between dateadd(mi,-10,rt.refuelfrom) and dateadd(mi,10,rt.refuelto) where  rt.refuelfrom between   '" & begindatetime & "' and '" & enddatetime & "'  and rt.status=1   order by avu.username,rt.plateno,refuelfrom"
                    query1 = "select distinct au.username,ft.plateno,substring(convert(varchar,ft.timestamp,120),1,16) as timestamp,ft.stationcode,ft.liters,ft.cost from (select * from fuel where userid ='" & userid & "' and plateno='" & plateno & "') ft left outer join   ( select ft.plateno,ft.timestamp from fuel ft inner join refuel rt on ft.plateno=rt.plateno and ft.timestamp between  dateadd(mi,-10,rt.refuelfrom) and dateadd(mi,10,rt.refuelto)  where ft.timestamp between '" & begindatetime & "' and '" & enddatetime & "' and userid<>0 ) t on ft.plateno=t.plateno and ft.timestamp=t.timestamp left outer join userTBL au on ft.userid=au.userid where ft.timestamp between '" & begindatetime & "' and '" & enddatetime & "' and ft.userid<>0  and t.plateno is NULL and ft.plateno='" & plateno & "'"
                End If
            Else
                If ddlp = "--All Plate No--" Then
                    If role = "SuperUser" Or role = "Operator" Then
                        query = "select  distinct avu.userid,avu.username,ar.plateno,substring(convert(varchar,ar.refuelfrom,120),1,16) as refuelfrom,substring(convert(varchar,ar.refuelto,120),1,16) as refuelto,ar.beforerefuel, ar.afterrefuel,ar.totrefuel,ar.fuelcost,ar.lat,ar.lon,ft.liters ,ft.cost from refuel ar inner join (select au.userid,au.username,plateno from vehicleTBL vt left outer join userTBL au on vt.userid=au.userid where  au.userid in (" & userslist & ")) avu on ar.plateno=avu.plateno  left outer join fuel ft on ft.timestamp between dateadd(mi,-10,ar.refuelfrom) and dateadd(mi,10,ar.refuelto)  and ar.plateno =ft.plateno    where  refuelfrom between '" & begindatetime & "' and '" & enddatetime & "' and ar.status=1   order by avu.username,ar.plateno, refuelfrom "
                        query1 = "select distinct au.username,ft.plateno,substring(convert(varchar,ft.timestamp,120),1,16) as timestamp,ft.stationcode,ft.liters,ft.cost from  (select * from fuel where userid in (" & userslist & ") ) ft left outer join   ( select ft.plateno,ft.timestamp from fuel ft inner join refuel rt on ft.plateno=rt.plateno and ft.timestamp  between dateadd(mi,-10,rt.refuelfrom) and dateadd(mi,10,rt.refuelto)  where ft.timestamp between '" & begindatetime & "' and '" & enddatetime & "' and userid<>0 ) t on ft.plateno=t.plateno and ft.timestamp=t.timestamp left outer join userTBL au on ft.userid=au.userid where ft.timestamp between '" & begindatetime & "' and '" & enddatetime & "' and ft.userid<>0  and t.plateno is NULL "

                    ElseIf role = "Admin" Then
                        query = "select distinct plateno,substring(convert(varchar,refuelfrom,120),1,16) as refuelfrom,substring(convert(varchar,refuelto,120),1,16) as refuelto,beforerefuel, afterrefuel,totrefuel,fuelcost,lat,lon from refuel where refuelfrom between '" & begindatetime & "' and '" & enddatetime & "' "
                    End If
                Else
                    query = "select distinct ar.plateno,vt.userid,ut.username,substring(convert(varchar,refuelfrom,120),1,16) as refuelfrom,substring(convert(varchar,refuelto,120),1,16) as refuelto,beforerefuel, afterrefuel,totrefuel,fuelcost,lat,lon,ft.liters ,ft.cost from refuel ar inner join vehicleTBL vt  on ar.plateno=vt.plateno left outer join userTBL ut on vt.userid=ut.userid left outer join fuel ft on ft.timestamp between dateadd(mi,-10,ar.refuelfrom) and dateadd(mi,10,ar.refuelto)  and ar.plateno =ft.plateno  where  ar.plateno='" & plateno & "' and refuelfrom between '" & begindatetime & "' and '" & enddatetime & "' and ar.status=1  order by refuelfrom "
                    query1 = "select distinct au.username,ft.plateno,substring(convert(varchar,ft.timestamp,120),1,16) as timestamp,ft.stationcode,ft.liters,ft.cost from (select * from fuel where plateno='" & plateno & "') ft left outer join   ( select ft.plateno,ft.timestamp from fuel ft inner join refuel rt on ft.plateno=rt.plateno and ft.timestamp between  dateadd(mi,-10,rt.refuelfrom) and dateadd(mi,10,rt.refuelto)  where ft.timestamp between '" & begindatetime & "' and '" & enddatetime & "' and userid<>0 ) t on ft.plateno=t.plateno and ft.timestamp=t.timestamp left outer join userTBL au on ft.userid=au.userid where ft.timestamp between '" & begindatetime & "' and '" & enddatetime & "' and ft.userid<>0  and t.plateno is NULL  and ft.plateno='" & plateno & "' "

                End If
            End If
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            'Return query1
            '#LOAD PETROL STATIONS
            conn.Open()
            Dim cmd1 As SqlCommand
            Dim dr1 As SqlDataReader
            cmd1 = New SqlCommand("select * from fuel_station", conn)


            Dim FuelstationsLayer As New AspMap.DynamicLayer()
            FuelstationsLayer.LayerType = LayerType.mcPolygonLayer

            Try
                dr1 = cmd1.ExecuteReader()
                While dr1.Read()
                    Try
                        Dim circleShape As New AspMap.Shape
                        circleShape.MakeCircle(dr1("lat"), dr1("lon"), 50 / 111120.0)
                        FuelstationsLayer.Add(circleShape, dr1("name").ToString(), dr1("id"))
                    Catch ex As Exception

                    End Try
                End While

            Catch ex As Exception

            Finally
                conn.Close()
                dr1.Close()
            End Try
            mapping.AddLayer(FuelstationsLayer)
            mapping(0).Name = "PetrolStations"
            '#END


            Dim cmd As SqlCommand = New SqlCommand(query, conn)
            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim r As DataRow
                Dim i As Int64 = 1
                Dim totalTankSize As Double = 0
                Dim presplateno As String = ""
                Dim prevplateno As String = ""
                While dr.Read
                    presplateno = dr("plateno")
                    If prevplateno <> presplateno Then
                        Dim fuelobj As New FuelMath1(dr("plateno"))
                        totalTankSize = calculateTotalTank(fuelobj.formulaText1, fuelobj.formulaShape1)
                        If fuelobj.f2Found = True Then
                            totalTankSize = totalTankSize + calculateTotalTank(fuelobj.formulaText2, fuelobj.formulaShape2)
                        End If
                    End If

                    r = t.NewRow
                    r(0) = i.ToString()
                    r(1) = dr("username")
                    r(2) = dr("plateno")
                    r(3) = CDate(dr("refuelfrom")).ToString("yyyy/MM/dd HH:mm:ss")
                    r(4) = CDate(dr("refuelto")).ToString("yyyy/MM/dd HH:mm:ss")

                    vehiclepoint.Y = CType(dr("lon"), Double)
                    vehiclepoint.X = CType(dr("lat"), Double)

                    Dim rs As AspMap.Recordset
                    rs = mapping("PetrolStations").SearchByDistance(vehiclepoint, 50 / (60 * 1852), SearchMethod.mcIntersect)
                    If rs.RecordCount < 1 Or userid = "4215" Then
                        r(5) = locObj.GetLocation(dr("lat"), dr("lon"))
                    Else
                        r(5) = "<div class='pt1' title='Petrol Station'></div>" & rs(0)
                        r(18) = rs(0)
                    End If

                    r(6) = CDbl(dr("beforerefuel")).ToString("0.00")
                    r(7) = CDbl(dr("afterrefuel")).ToString("0.00")
                    r(8) = CDbl(dr("totrefuel")).ToString("0.00")
                    r(9) = CDbl(dr("fuelcost")).ToString("0.00")
                    Dim costperltr As Double = GetFuelPrice(dr("userid"), dr("plateno"), CDate(dr("refuelto")).ToString("yyyy/MM/dd HH:mm:ss"))
                    Dim id As String = Convert.ToDateTime(dr("refuelto")).Ticks.ToString()
                    If IsDBNull(dr("liters")) Then
                        r(10) = "<input type=""textbox"" style=""width:60px;"" id=""txt_" & id & """ onchange=""javascript:insertrefuel('" & dr("userid") & "','" & dr("plateno") & "','" & Convert.ToDateTime(dr("refuelto")).ToString("yyyy/MM/dd HH:mm:ss") & "',this.value,1,'" & costperltr & "')"" value=''></input>"
                        r(11) = "<input type=""textbox"" style=""width:60px;"" id=""txtprice_" & id & """ onchange=""javascript:insertrefuelcpl('" & dr("userid") & "','" & dr("plateno") & "','" & Convert.ToDateTime(dr("refuelto")).ToString("yyyy/MM/dd HH:mm:ss") & "',this.value,1,'" & id & "')"" value='" & costperltr & "' ></input>"

                        r(20) = ""
                    Else
                        r(10) = "<input type=""textbox"" style=""width:60px;"" id=""txt_" & id & """ onchange=""javascript:insertrefuel('" & dr("userid") & "','" & dr("plateno") & "','" & Convert.ToDateTime(dr("refuelto")).ToString("yyyy/MM/dd HH:mm:ss") & "',this.value,1,'" & costperltr & "')"" value='" & dr("liters") & "' ></input>"
                        r(11) = "<input type=""textbox"" style=""width:60px;"" id=""txtprice_" & id & """ onchange=""javascript:insertrefuelcpl('" & dr("userid") & "','" & dr("plateno") & "','" & Convert.ToDateTime(dr("refuelto")).ToString("yyyy/MM/dd HH:mm:ss") & "',this.value,1,'" & id & "')"" value='" & costperltr & "' ></input>"
                        r(20) = dr("liters")
                    End If
                    If IsDBNull(dr("liters")) Then
                        r(12) = "<label id=""lbl_" + dr("plateno") + """ ></lable>"
                        r(21) = ""
                    Else
                        r(12) = "<label id=""lbl_" + dr("plateno") + """ >" & CDbl(dr("cost")).ToString("0.00") & "</lable>"
                        r(21) = CDbl(dr("cost")).ToString("0.00")
                    End If

                    If IsDBNull(dr("liters")) Then
                        r(13) = "--"
                    Else
                        r(13) = CDbl(100 - Math.Abs(((CDbl(dr("totrefuel")) - CDbl(dr("liters"))) / CDbl(dr("liters"))) * 100)).ToString("0") & "%"
                    End If

                    If IsDBNull(dr("liters")) Then
                        r(14) = "--"
                    Else
                        Dim val As String = (CDbl(dr("totrefuel")) - CDbl(dr("liters"))).ToString("0.00")
                        Dim checkTanksize As String = CDbl(100 - ((Math.Abs(CDbl(val))) / totalTankSize) * 100).ToString("0") & "%"
                        If (checkTanksize <> "-Infinity%") Then
                            r(14) = checkTanksize
                        Else
                            r(14) = "-"
                        End If
                    End If
                    r(15) = CType(dr("lat"), Double)
                    r(16) = CType(dr("lon"), Double)
                    r(17) = (CDate(dr("refuelto")) - CDate(dr("refuelfrom"))).TotalMinutes
                    r(18) = dr("userid")


                    totrefuel = totrefuel + CDbl(dr("totrefuel")).ToString("0.00")
                    totrefuelcost = totrefuelcost + CDbl(dr("fuelcost")).ToString("0.00")


                    t.Rows.Add(r)
                    i = i + 1
                    'Try

                    '    If Convert.ToDateTime(edt).ToString("yyyy/MM/dd") = DateTime.Now.ToString("yyyy/MM/dd") Then
                    '        Dim dt1 As New DataTable
                    '        dt1 = GetLiveData(dr("plateno"), dr("userid"))
                    '        If dt1.Rows.Count > 0 Then
                    '            For row As Int32 = 0 To dt1.Rows.Count - 1
                    '                r = t.NewRow
                    '                r(0) = i.ToString()
                    '                r(1) = dt1.Rows(row)("Plate No")
                    '                r(2) = CDate(dt1.Rows(row)("Refuelfrm Time")).ToString("yyyy/MM/dd HH:mm:ss")
                    '                r(3) = CDate(dt1.Rows(row)("Refuelto Time")).ToString("yyyy/MM/dd HH:mm:ss")
                    '                r(4) = dt1.Rows(row)("Address")
                    '                r(5) = CDbl(dt1.Rows(row)("Refuel Start")).ToString("0.00")
                    '                r(6) = CDbl(dt1.Rows(row)("Refuel End")).ToString("0.00")
                    '                r(7) = CDbl(dt1.Rows(row)("Refuel(Ltr)")).ToString("0.00")
                    '                r(8) = CDbl(dt1.Rows(row)("Refuel Cost(RM)")).ToString("0.00")
                    '                totrefuel = totrefuel + CDbl(dt1.Rows(row)("Refuel(Ltr)")).ToString("0.00")
                    '                totrefuelcost = totrefuelcost + CDbl(dt1.Rows(row)("Refuel Cost(RM)")).ToString("0.00")
                    '                t.Rows.Add(r)
                    '            Next
                    '        End If

                    '    End If
                    'Catch ex As Exception
                    '    r = t.NewRow
                    '    r(0) = "--"
                    '    r(1) = ex.Message
                    '    r(2) = "--"
                    '    r(3) = "--"
                    '    r(4) = "--"
                    '    r(5) = "--"
                    '    r(6) = "--"
                    '    r(7) = "--"
                    '    r(8) = "--"
                    '    t.Rows.Add(r)
                    'End Try



                End While

                cmd = New SqlCommand(query1, conn)
                Try

                    Dim dr2 As SqlDataReader = cmd.ExecuteReader()
                    While dr2.Read
                        Try
                            Dim foundRows() As DataRow
                            foundRows = t.Select("RefueltoTime = #" & Convert.ToDateTime(dr2("timestamp")).ToString("yyyy/MM/dd HH:mm") & "# And PlateNo ='" & dr2("plateno") & "'")
                            If foundRows.Length = 0 Then
                                r = t.NewRow
                                r(0) = i.ToString()
                                r(1) = dr2("username")
                                r(2) = dr2("plateno")
                                r(3) = CDate(dr2("timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                                r(4) = "--"
                                r(5) = "--"
                                r(6) = "--"
                                r(7) = "--"
                                r(8) = "--"
                                r(9) = "--"
                                Dim rid As String = Convert.ToDateTime(dr2("timestamp")).Ticks.ToString()
                                Dim costperltr As Double = GetFuelPrice(userid, dr2("plateno"), CDate(dr2("timestamp")).ToString("yyyy/MM/dd HH:mm:ss"))
                                r(10) = "<input type=""textbox"" style=""width:60px;"" id=""txt_" & rid & """ onchange=""javascript:insertrefuel('" & userid & "','" & dr2("plateno") & "','" & Convert.ToDateTime(dr2("timestamp")).ToString("yyyy/MM/dd HH:mm:ss") & "',this.value,1,'" & costperltr & "')"" value='" & dr2("liters") & "' ></input>"

                                r(11) = "<input type=""textbox"" style=""width:60px;"" id=""txtprice_" & rid & """ value='" & costperltr & "' ></input>"
                                r(12) = CDbl(dr2("cost")).ToString("0.00")
                                ' r(11) = "<label id=""lbl_" + dr("plateno") + """ >" & CDbl(dr("cost")).ToString("0.00") & "</lable>"
                                r(13) = "--"
                                r(14) = "--"
                                r(15) = "--"
                                r(16) = "--"
                                r(17) = "--"
                                r(18) = "--"
                                r(19) = "--"
                                r(20) = dr2("liters")
                                r(21) = CDbl(dr2("cost")).ToString("0.00")
                                t.Rows.Add(r)
                                i = i + 1
                            End If
                        Catch ex As Exception

                        End Try

                    End While


                Catch ex As Exception

                End Try


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
                    r(18) = "--"
                    r(19) = "--"
                    r(20) = "--"
                    r(21) = "--"
                    t.Rows.Add(r)
                End If

                r = t.NewRow
                r(0) = "--"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                r(5) = "--"
                r(6) = "--"
                r(7) = "--"
                r(8) = totrefuel.ToString("0.00")
                r(9) = totrefuelcost.ToString("0.00")
                r(10) = "--"
                r(11) = "--"
                r(12) = "--"
                r(13) = "--"
                r(14) = "--"
                r(15) = "--"
                r(16) = "--"
                r(18) = "--"
                r(19) = "--"
                r(20) = "--"
                r(21) = "--"
                t.Rows.Add(r)

            Catch ex As Exception

            Finally
                conn.Close()
            End Try

            Dim exceltable As New DataTable
            exceltable.Columns.Add(New DataColumn("S No"))
            exceltable.Columns.Add(New DataColumn("User Name"))
            exceltable.Columns.Add(New DataColumn("PlateNo"))
            exceltable.Columns.Add(New DataColumn("Refuel Start Time"))
            exceltable.Columns.Add(New DataColumn("Refuel End Time"))
            exceltable.Columns.Add(New DataColumn("Address"))
            exceltable.Columns.Add(New DataColumn("Refuel Start(L)"))
            exceltable.Columns.Add(New DataColumn("Refuel End (L)"))
            exceltable.Columns.Add(New DataColumn("Refuel Ltr(L)"))
            exceltable.Columns.Add(New DataColumn("Refuel Cost(RM)"))
            exceltable.Columns.Add(New DataColumn("Actual(L)"))
            exceltable.Columns.Add(New DataColumn("Actual(RM)"))
            ' exceltable.Columns.Add(New DataColumn("Accuracy Agenest Refuel"))
            exceltable.Columns.Add(New DataColumn("Acuracy Againest Tankvolume"))

            Dim er As DataRow
            For i As Int32 = 0 To t.Rows.Count - 1
                er = exceltable.NewRow
                er(0) = t.DefaultView.Item(i)(0)
                er(1) = t.DefaultView.Item(i)(1)
                er(2) = t.DefaultView.Item(i)(2)
                er(3) = t.DefaultView.Item(i)(3)
                er(4) = t.DefaultView.Item(i)(4)
                er(5) = t.DefaultView.Item(i)(5)
                er(6) = t.DefaultView.Item(i)(6)
                er(7) = t.DefaultView.Item(i)(7)
                er(8) = t.DefaultView.Item(i)(8)
                er(9) = t.DefaultView.Item(i)(9)
                er(10) = t.DefaultView.Item(i)(20)
                er(11) = t.DefaultView.Item(i)(21)
                ' er(12) = t.DefaultView.Item(i)(12)
                er(12) = t.DefaultView.Item(i)(14)
                exceltable.Rows.Add(er)
            Next
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session.Remove("exceltable3")
            HttpContext.Current.Session("exceltable") = exceltable
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
                        aa.Add(a)
                    Catch ex As Exception

                    End Try
                Next

                'a = New ArrayList
                'a.Add(0)
                'a.Add(0)
                'a.Add(0)
                'a.Add(0)
                'a.Add(0)
                'a.Add(0)
                'a.Add(0)
                'a.Add(totrefuel.ToString("0.00"))
                'a.Add(totrefuelcost.ToString("0.00"))
                'aa.Add(a)
            Else

            End If
            json = JsonConvert.SerializeObject(aa, Formatting.None)
            'HttpContext.Current.Response.Write(query & " :::::" & query1)
            Return json
        Catch ex As Exception

        End Try

        Return json

    End Function
    Public Function calculateTotalTank(ByVal formulatext As String, ByVal formulashape As String) As Double
        Try
            Dim formulaArray() As String = Split(formulatext, " ")
            Dim tanksize As String = ""
            Dim finalTankSize() As String
            Dim TankActualSize() As Integer = New Integer() {}
            Dim parseTankSize As Integer
            Dim w As Integer = 0

            For i As Integer = 0 To formulaArray.Length - 1


                If (formulaArray(i).Contains("x") Or formulaArray(i).Contains("X")) And formulaArray(i).Length > 7 Then
                    tanksize = formulaArray(i)
                    finalTankSize = Split(CStr(tanksize).ToLower(), "x")

                    For x As Integer = 0 To finalTankSize.Length - 1
                        'Response.Write(x & "-" & finalTankSize(x))
                        If finalTankSize(x).Contains("(") Then
                            Dim tempTankSize As String = finalTankSize(x)
                            Dim sizesize() As String = Split(tempTankSize, "(")
                            finalTankSize(x) = sizesize(0)
                        End If
                        If Integer.TryParse(finalTankSize(x), 0) Then
                            ReDim Preserve TankActualSize(w)
                            TankActualSize(w) = finalTankSize(x)
                            w += 1
                        End If

                    Next

                End If
            Next

            Dim tanktank As Double = 0
            If formulashape = "Tank Volume" Then
                tanktank = TankActualSize(0) * TankActualSize(1) * TankActualSize(2) / 1000
            Else
                tanktank = 3.142 * (TankActualSize(0) / 2) * (TankActualSize(0) / 2) * TankActualSize(2) / 1000
            End If


            'lblInfo2.Text = lblInfo2.Text & " Maximum=" & tanktank & " Litre"

            Return tanktank

        Catch ex As SystemException
            'lblInfo2.Text = lblInfo2.Text & " !!! " & ex.Message & "!!!!"
        End Try
    End Function

    Private Function GetFuelPrice(ByVal userid As String, ByVal plateno As String, ByVal timestamp As String) As Double
        Dim result As Int16
        Dim price As Double
        Dim fuelcost As Double
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim cmd1 As SqlCommand
            Dim dr As SqlDataReader

            Dim dPrice As New DataTable
            dPrice = fuelPrice(userid)
            Dim drPrice As DataRow() = dPrice.Select("StartDate <= #" & Convert.ToDateTime(timestamp).ToString("yyyy/MM/dd HH:mm:ss") & "# And EndDate >= #" & Convert.ToDateTime(timestamp).ToString("yyyy/MM/dd HH:mm:ss") & "#")

            For pr As Integer = 0 To drPrice.Length - 1
                If (Convert.ToDateTime(drPrice(pr)(0)) <= Convert.ToDateTime(timestamp)) And (Convert.ToDateTime(drPrice(pr)(1)) >= Convert.ToDateTime(timestamp)) Then
                    fuelcost = CDbl(drPrice(pr)(2))
                    Exit For
                End If
            Next
            ''For Each Row In drPrice
            ''    If (Convert.ToDateTime(Row(0)) <= Convert.ToDateTime(timestamp)) And (Convert.ToDateTime(Row(1)) >= Convert.ToDateTime(timestamp)) Then
            ''        fuelcost = CDbl(Row(2))
            ''        Exit For
            ''    End If
            ''Next
            cmd1 = New SqlCommand("select * from fuel where plateno='" & plateno & "' and timestamp='" & timestamp & "'", conn)
            conn.Open()
            dr = cmd1.ExecuteReader()
            If dr.Read() Then
                fuelcost = Math.Round(Convert.ToDouble(dr("cost")) / Convert.ToDouble(dr("liters")), 2)
            End If

            conn.Close()

        Catch ex As Exception

        End Try
        Return fuelcost
    End Function

    Public Shared Function fuelPrice(ByVal userid As String) As DataTable
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim dsFuelPrice As New DataSet
        Dim da As SqlDataAdapter
        da = New SqlDataAdapter("select * from fuel_price where countrycode=(select countrycode from userTBL where userid='" & userid & "') order by startdate desc ", conn)
        Dim priceTable As New DataTable
        Try
            da.Fill(dsFuelPrice)
            priceTable.Columns.Add(New DataColumn("StartDate", GetType(DateTime)))
            priceTable.Columns.Add(New DataColumn("EndDate", GetType(DateTime)))
            priceTable.Columns.Add(New DataColumn("FuelPrice"))
            Dim pRow As DataRow
            If dsFuelPrice.Tables(0).Rows.Count > 0 Then
                For i As Int32 = 0 To dsFuelPrice.Tables(0).Rows.Count - 1
                    pRow = priceTable.NewRow
                    pRow(0) = dsFuelPrice.Tables(0).Rows(i)("startdate")
                    pRow(1) = dsFuelPrice.Tables(0).Rows(i)("enddate")
                    pRow(2) = dsFuelPrice.Tables(0).Rows(i)("fuelprice")
                    priceTable.Rows.Add(pRow)
                Next
            Else
                pRow = priceTable.NewRow
                pRow(0) = Now.ToString("yyyy/MM/dd")
                pRow(1) = Now.ToString("yyyy/MM/dd")
                pRow(2) = 0
                priceTable.Rows.Add(pRow)
            End If

        Catch

        End Try
        Return priceTable
    End Function
End Class

