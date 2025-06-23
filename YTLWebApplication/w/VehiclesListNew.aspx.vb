Imports System.Data.SqlClient
Partial Class VehiclesListNew
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public sb1 As New StringBuilder()
    Public sb As New StringBuilder()
    Public opt As String
    Public suserid As String
    Public spuserid As String
    Public suser As String
    Public sgroup As String
    Public isSvwong As String = "false"
    Public googleearthparameters As String = ""
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            suserid = ss.Value
            Dim transType As Integer = Convert.ToInt32(radioTransporterType.SelectedValue)
            If Page.IsPostBack = False Then
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                If userid = "1967" Then
                    isSvwong = "true"
                End If
                googleearthparameters = "userid=" & userid & "&role=" & role

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cond As String = ""
                If transType = 2 Then
                    cond = " "
                ElseIf transType = 1 Then
                    cond = " where transporter_id in (select transporterid from  ytloss.dbo.oss_transporter_master where internaltype='" & transType & "')"
                Else
                    cond = " where plateno not  in (select plateno from VehicleTbl where transporter_id in (select transporterid from  ytloss.dbo.oss_transporter_master where internaltype='1'))"
                End If


                Dim cmd As SqlCommand = New SqlCommand("select userid, username, dbip from  ytlDb.dbo.userTBL where userid in (select userid from VehicleTbl  " & cond & ") and  role='User' order by username", conn)
                If role = "User" Then
                    cmd = New SqlCommand("select userid, username, dbip from  ytlDb.dbo.userTBL where userid in (select userid from VehicleTbl " & cond & ") and userid='" & userid & "' order by username ", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid, username, dbip from  ytlDb.dbo.userTBL where userid in (select userid from VehicleTbl " & cond & ") and userid in (" & userslist & ") order by username", conn)
                End If
                ' Response.Write("3." & cmd.CommandText)
                ' Response.Write("<br/>")
                Dim dr As SqlDataReader
                sb = New StringBuilder()
                Try
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    sb.Append("<select  id=""ddluser1"" onchange=""javascript: return refreshTable()""  data-placeholder=""Select User Group"" style=""width:250px;"" class=""chosen""  tabindex=""5"">")
                    sb.Append("<option id=""epty"" value=""""></option>")
                    Dim ct As Integer = 0
                    Dim firstrecord As Boolean = True

                    If (role <> "User") Then
                        sb.Append("<option selected=""selected"" value=""0"">SELECT USERNAME</option>")
                        suserid = 0
                        ct = 1
                    End If

                    While dr.Read
                        userid = dr("userid")
                        If ct = 0 Then
                            sb.Append("<option selected=""selected""  value=")
                            sb.Append(dr("userid"))
                            sb.Append(">")
                            sb.Append(dr("username").ToString().ToUpper())
                            sb.Append("</option>")
                        Else
                            sb.Append("<option  value=")
                            sb.Append(dr("userid"))
                            sb.Append(">")
                            sb.Append(dr("username").ToString().ToUpper())
                            sb.Append("</option>")
                        End If
                        ct += 1

                        Dim cmd1 As SqlCommand = New SqlCommand("select  groupname from vehicle_group where userid='" & userid & "' order by groupname", conn)

                        Dim dr1 As SqlDataReader
                        dr1 = cmd1.ExecuteReader()
                        While dr1.Read()
                            Try
                                Dim group As String = dr1("groupname")
                                sb.Append("<option  value=")
                                sb.Append("""" & dr("userid") & "," & dr1("groupname") & """")
                                sb.Append(">")
                                sb.Append("" & System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dr1("groupname").ToString().ToLower()) & "")
                                sb.Append("</option>")

                            Catch ex As Exception

                            End Try

                        End While
                    End While
                    sb.Append("</select>")
                    dr.Close()
                Catch ex As Exception
                    '  Response.Write(ex.Message)
                Finally
                    conn.Close()
                End Try

                opt = sb.ToString()
            End If
            fillDrop(transType)
            FillVehiclesGrid()
            'Response.Write(opt)
        Catch ex As Exception
            ' Response.Write(ex.Message)
        End Try
    End Sub
    Private Sub fillDrop(ByVal transType As Int16)
        Try

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim cond As String = ""

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            If transType = 2 Then
                cond = " "
            ElseIf transType = 1 Then
                cond = " where transporter_id in (select transporterid from  ytloss.dbo.oss_transporter_master where internaltype='" & transType & "')"
            ElseIf transType = 0 Then
                cond = " where plateno not  in (select plateno from VehicleTbl where transporter_id in (select transporterid from  ytloss.dbo.oss_transporter_master where internaltype='1'))"
            Else
                cond = " where transporter_id='-9'"
            End If


            Dim cmd As SqlCommand = New SqlCommand("select userid, username, dbip from  ytlDb.dbo.userTBL where userid in (select userid from VehicleTbl  " & cond & ") and  role='User' order by username", conn)
            If role = "User" Then
                cmd = New SqlCommand("select userid, username, dbip from  ytlDb.dbo.userTBL where userid in (select userid from VehicleTbl " & cond & ") and userid='" & userid & "' order by username ", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid, username, dbip from  ytlDb.dbo.userTBL where userid in (select userid from VehicleTbl " & cond & ") and userid in (" & userslist & ") order by username", conn)
            End If

            Dim dr As SqlDataReader
            sb = New StringBuilder()
            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                suserid = ss.Value
                sb.Length = 0
                sb.Append("<select  id=""ddluser1"" onchange=""javascript: return refreshTable()""   data-placeholder=""Select User Group"" style=""width:250px;"" class=""chosen""  tabindex=""5"">")
                sb.Append("<option id=""epty"" value=""""></option>")
                If role <> "User" Then
                    sb.Append("<option selected=""selected"" value=""0"">SELECT USERNAME</option>")
                End If
                If suserid <> "--AllUsers--" Then

                    If role = "SuperUser" Or role = "Admin" Then
                        sb.Append("<option value=--AllUsers-->--All Users--</option>")
                    End If

                End If
                If suserid = "--AllUsers--" Then
                    If role = "SuperUser" Or role = "Admin" Then
                        sb.Append("<option selected=""selected"">--AllUsers--</option>")
                    End If
                End If

                Dim i As Integer = 0
                While dr.Read
                    userid = dr("userid")
                    If suserid.IndexOf(",") > 0 Then
                        Dim sgroupname As String() = suserid.Split(",")
                        suser = sgroupname(0)
                        sgroup = sgroupname(1)
                    End If

                    Dim ct As Integer = 0
                    userid = dr("userid")
                    If role = "User" Then
                        If suserid.IndexOf(",") > 0 Then
                            sb.Append("<option value=")
                            sb.Append(dr("userid"))
                            sb.Append(">")
                            sb.Append(dr("username").ToString().ToUpper())
                            sb.Append("</option>")
                        Else
                            Dim firstrecord As Boolean = True
                            If (firstrecord = True) Then
                                firstrecord = False
                                suserid = dr("userid")
                            End If
                            If ct = 0 Then
                                sb.Append("<option selected=""selected""  value=")
                                sb.Append(dr("userid"))
                                sb.Append(">")
                                sb.Append(dr("username").ToString().ToUpper())
                                sb.Append("</option>")

                            End If
                        End If
                        ct = +1
                    Else
                    End If


                    If role <> "User" Then

                        If ct = 0 Then
                            If userid = suserid Then
                                sb.Append("<option selected=""selected""  value=")
                                sb.Append(dr("userid"))
                                sb.Append(">")
                                sb.Append(dr("username").ToString().ToUpper())
                                sb.Append("</option>")
                                suserid = dr("userid")
                            Else
                                sb.Append("<option  value=")
                                sb.Append(dr("userid"))
                                sb.Append(">")
                                sb.Append(dr("username").ToString().ToUpper())
                                sb.Append("</option>")
                            End If
                        End If
                    End If

                    ct += 1


                    Dim cmd1 As SqlCommand = New SqlCommand("select  groupname from vehicle_group where userid='" & userid & "' order by groupname", conn)

                    Dim dr1 As SqlDataReader
                    dr1 = cmd1.ExecuteReader()
                    While dr1.Read()
                        Try
                            Dim group As String = dr1("groupname")
                            If suserid.IndexOf(",") > 0 Then
                                If group = sgroup Then
                                    sb.Append("<option selected=""selected""  value=")
                                    sb.Append("""" & dr("userid") & "," & dr1("groupname") & """")
                                    sb.Append(">")
                                    sb.Append("" & System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dr1("groupname").ToString().ToLower()) & "")
                                    sb.Append("</option>")
                                Else
                                    sb.Append("<option  value=")
                                    sb.Append("""" & dr("userid") & "," & dr1("groupname") & """")
                                    sb.Append(">")
                                    sb.Append("" & System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dr1("groupname").ToString().ToLower()) & "")
                                    sb.Append("</option>")
                                End If
                            Else
                                sb.Append("<option  value=")
                                sb.Append("""" & dr("userid") & "," & dr1("groupname") & """")
                                sb.Append(">")
                                sb.Append("" & System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dr1("groupname").ToString().ToLower()) & "")
                                sb.Append("</option>")
                            End If
                        Catch ex As Exception
                            '  Response.Write("1.1:" & ex.Message)
                        End Try

                    End While
                End While
                sb.Append("</select>")
                dr.Close()
            Catch ex As Exception
                ' Response.Write("1.2:" & ex.Message)
            Finally
                conn.Close()
            End Try

            opt = sb.ToString()
        Catch ex As Exception
            ' Response.Write("1.3:" & ex.Message)
        End Try
    End Sub
    Protected Sub FillVehiclesGrid()

        Dim cond As String = ""
        Dim transType As String = radioTransporterType.SelectedValue
        If transType = 2 Then
            cond = " "
        ElseIf transType = 1 Then
            cond = " and m.internaltype='" & transType & "'"
        ElseIf transType = 0 Then
            cond = " and m.internaltype='" & transType & "'"
        Else
            cond = " "
        End If

        Try
            Dim groupcondition As String = ""
            Dim vehiclestable As New DataTable
            vehiclestable.Columns.Add(New DataColumn("chk"))
            vehiclestable.Columns.Add(New DataColumn("Sno"))
            vehiclestable.Columns.Add(New DataColumn("Plateno"))
            vehiclestable.Columns.Add(New DataColumn("Platenumber"))
            vehiclestable.Columns.Add(New DataColumn("Type"))
            vehiclestable.Columns.Add(New DataColumn("GroupName"))
            vehiclestable.Columns.Add(New DataColumn("Brand"))
            vehiclestable.Columns.Add(New DataColumn("Model"))
            vehiclestable.Columns.Add(New DataColumn("Speedlimit"))
            vehiclestable.Columns.Add(New DataColumn("icon"))
            vehiclestable.Columns.Add(New DataColumn("VersionID"))
            vehiclestable.Columns.Add(New DataColumn("PTO"))
            vehiclestable.Columns.Add(New DataColumn("DriverMobile"))
            vehiclestable.Columns.Add(New DataColumn("Odometer"))
            vehiclestable.Columns.Add(New DataColumn("Recarded Date"))
            vehiclestable.Columns.Add(New DataColumn("Prime Mover ID"))
            vehiclestable.Columns.Add(New DataColumn("Username"))
            vehiclestable.Columns.Add(New DataColumn("TransporterName"))
            vehiclestable.Columns.Add(New DataColumn("BasePlant"))





            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            If suserid.IndexOf(",") > 0 Then
                Dim sgroupname As String() = suserid.Split(",")
                suser = sgroupname(0)
                sgroup = sgroupname(1)
            ElseIf (suserid = "--AllUsers--") Then
                suser = "--AllUsers--"
            End If
            Dim key As String = suser & ",User"
            If suser = "--AllUsers--" Then
                key = userid & "," & role
            End If


            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim cmd As SqlCommand = New SqlCommand("select   dbo.fn_getusername_plateno(plateno) as username,dbo.fn_GetTransporterNameByPlateno(plateno) as transportername,pto,v.userid,plateno,type,color,brand,model,speedlimit,smallimage,bigimage,VersionID,g.groupname,v.groupid,drivermobile,VehicleOdoRecDate,vehicleodometer,isnull(pmid,'-') as pmid,v.baseplant,isnull(v.companyid,0) as companyid,isnull(m.internaltype,0) as internaltype ,geo.shiptocode  from YTLDB.dbo.userTBL u left outer join (select distinct userid from YTLDB.dbo.vehicleTBL where plateno in(select distinct plateno from ytloss.dbo.oss_patch_in)) tt on tt.userid = u.userid inner join vehicleTBL v on v.userid = u.userid left outer join ytloss.dbo.oss_transporter_master m on v.transporter_id = m.transporterid left outer join vehicle_group g on g.groupid=v.groupid left outer join geofence geo on v.baseplant=geo.geofenceid where u.userid='" & suserid & "' and u.username <> 'JASA1' and u.username <> 'JASA2' and u.username <> 'JasaCargo' " & cond & " order by plateno ", conn)
            If suser = "--AllUsers--" Then
                If role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select   dbo.fn_getusername_plateno(plateno) as username,dbo.fn_GetTransporterNameByPlateno(plateno) as transportername,pto,v.userid,plateno,type,color,brand,model,speedlimit,smallimage,bigimage,VersionID,g.groupname,v.groupid,drivermobile,VehicleOdoRecDate,vehicleodometer,isnull(pmid,'-') as pmid,v.baseplant,isnull(v.companyid,0) as companyid,isnull(m.internaltype,0) as internaltype,geo.shiptocode  from YTLDB.dbo.userTBL u left outer join (select distinct userid from YTLDB.dbo.vehicleTBL where plateno in(select distinct plateno from ytloss.dbo.oss_patch_in)) tt on tt.userid = u.userid inner join vehicleTBL v on v.userid = u.userid left outer join ytloss.dbo.oss_transporter_master m on v.transporter_id = m.transporterid left outer join vehicle_group g on g.groupid=v.groupid left outer join geofence geo on v.baseplant=geo.geofenceid where u.userid in(" & userslist & ") and u.username <> 'JASA1' and u.username <> 'JASA2' and u.username <> 'JasaCargo' " & cond & " order by plateno", conn)
                Else
                    cmd = New SqlCommand("Select dbo.fn_getusername_plateno(plateno) As username,dbo.fn_GetTransporterNameByPlateno(plateno) As transportername,pto,v.userid,plateno,type,color,brand,model,speedlimit,smallimage,bigimage,VersionID,g.groupname,v.groupid,drivermobile,VehicleOdoRecDate,vehicleodometer,isnull(pmid,'-') as pmid,v.baseplant,isnull(v.companyid,0) as companyid,isnull(m.internaltype,0) as internaltype ,geo.shiptocode from vehicleTBL v left join vehicle_group g on g.groupid=v.groupid left outer join geofence geo on v.baseplant=geo.geofenceid order by plateno", conn)
                End If
            ElseIf suserid.IndexOf(",") > 0 Then
                cmd = New SqlCommand("select   dbo.fn_getusername_plateno(plateno) as username,dbo.fn_GetTransporterNameByPlateno(plateno) as transportername,pto,v.userid,plateno,type,color,brand,model,speedlimit,smallimage,bigimage,VersionID,g.groupname,v.groupid,drivermobile,VehicleOdoRecDate,vehicleodometer,isnull(pmid,'-') as pmid,v.baseplant,isnull(v.companyid,0) as companyid,isnull(m.internaltype,0) as internaltype ,geo.shiptocode  from YTLDB.dbo.userTBL u left outer join (select distinct userid from YTLDB.dbo.vehicleTBL where plateno in(select distinct plateno from ytloss.dbo.oss_patch_in)) tt on tt.userid = u.userid inner join vehicleTBL v on v.userid = u.userid left outer join ytloss.dbo.oss_transporter_master m on v.transporter_id = m.transporterid left outer join vehicle_group g on g.groupid=v.groupid left outer join geofence geo on v.baseplant=geo.geofenceid where u.userid='" & suser & "' and g.groupname='" & sgroup & "' and u.username <> 'JASA1' and u.username <> 'JASA2' and u.username <> 'JasaCargo' " & cond & " order by plateno ", conn)
            End If
            If transType = 3 Then
                cmd = New SqlCommand("SELECT a.plateno,b.transportername,a.[productname] FROM (select * from [YTLDB].[dbo].[a_temp_transporter_type] where plateno not in(select plateno from vehicletbl)) a  left outer join YtlOss.dbo.oss_transporter_master b on a.transporter_id=b.transporterid", conn)
            End If
            ' Response.Write(cmd.CommandText)
            Dim dr As SqlDataReader
            conn.Open()
            dr = cmd.ExecuteReader()
            Dim r As DataRow
            Dim i As Int32 = 1
            While dr.Read()
                Try
                    r = vehiclestable.NewRow

                    r(0) = "<input type=""checkbox"" name=""chk"" value=""" & dr("plateno") & """/>"
                    r(1) = i.ToString()
                    r(2) = ""
                    If transType = 3 Then
                        r(3) = dr("plateno").ToString()
                        r(4) = "--"
                        r(5) = dr("productname").ToString()
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
                        If IsDBNull(dr("transportername")) Then
                            r(17) = ""
                        Else
                            r(17) = dr("transportername").ToString()
                        End If
                        r(18) = "--"
                    Else
#Region "GPS"


                        If IsDBNull(dr("drivermobile")) Then
                            r(12) = "--"
                        Else
                            r(12) = dr("drivermobile")
                        End If
                        r(3) = "<span style='cursor:pointer;text-decoration:underline;' onclick=""javascript :openPopup('" & dr("userid") & "','" & dr("plateno") & "','" & dr("groupid") & "','" & dr("brand") & "','" & dr("model") & "','" & dr("speedlimit") & "','" & dr("type") & "','" & r(12) & "','" & dr("vehicleodometer") & "','" & dr("VehicleOdoRecDate") & "','" & dr("pmid") & "','" & dr("baseplant") & "','" & dr("internaltype") & "','" & dr("companyid") & "')"">" & dr("plateno") & "</span>"
                        Dim drivername As String = "Not Available"

                        drivername = ""
                        'r(4) = drivername
                        r(4) = dr("type").ToString().ToUpper()
                        r(5) = dr("groupname").ToString().ToUpper()
                        r(6) = dr("brand")
                        r(7) = dr("model").ToString().ToUpper()
                        r(8) = dr("speedlimit")
                        r(9) = "" 'HttpUtility.HtmlDecode("<a rel=""balloon1""><img src=""vehiclesmallimages/" & dr("smallimage") & """ alt=""" & dr("plateno") & """ title=""" & dr("plateno") & """ width=""20px"" height=""20px"" onmouseover=""javascript:mouseover('" & dr("bigimage") & "');"" onmouseout=""javascript:mouseout();""/></a>")
                        r(10) = dr("versionId")
                        If dr("pto") Then
                            r(11) = "Yes"
                        Else
                            r(11) = "No"
                        End If
                        If IsDBNull(dr("vehicleodometer")) Then
                            r(13) = ""
                        Else
                            r(13) = dr("vehicleodometer")
                        End If
                        If IsDBNull(dr("VehicleOdoRecDate")) Then
                            r(14) = ""
                        Else
                            r(14) = Convert.ToDateTime(dr("VehicleOdoRecDate")).ToString("yyyy/MM/dd HH:mm:ss")
                        End If
                        r(15) = dr("pmid")
                        If IsDBNull(dr("username")) Then
                            r(16) = ""
                        Else
                            r(16) = dr("username").ToString()
                        End If
                        If IsDBNull(dr("transportername")) Then
                            r(17) = ""
                        Else
                            r(17) = dr("transportername").ToString()
                        End If
                        If IsDBNull(dr("shiptocode")) Then
                            r(18) = ""
                        Else
                            r(18) = dr("shiptocode")
                        End If
#End Region
                    End If
                    vehiclestable.Rows.Add(r)
                    i = i + 1
                Catch ex As Exception
                    Response.Write(ex.Message & " - " & ex.StackTrace)
                End Try
            End While

            conn.Close()
            'Response.Write(vehiclestable.Rows.Count)
            If vehiclestable.Rows.Count = 0 Then
                r = vehiclestable.NewRow
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
                vehiclestable.Rows.Add(r)
            End If

            ec = "true"


            vehiclestable.Columns.Remove("Plateno")
            vehiclestable.Columns.Remove("icon")
            sb1.Length = 0

            sb1.Append("<thead><tr><th style=""width:44px;"">S No</th><th>Username</th><th>Transporter Name</th><th>Plate No</th><th>PM ID</th><th>Group Name</th><th>Type</th><th>Brand</th><th >Model</th><th >Speed</th><th >PTO</th><th>Driver Mobile No</th><th>Base Plant</th> ")
            '  If userid = "1997" Or userid = "2012" Or userid = "156" Then
            ' sb1.Append("<th style='padding-left:20px;' >Odometer</th><th style='padding-left:20px;width:120px' >Rec Date</th>")
            '  End If
            sb1.Append("</tr></thead>")
            Dim counter As Integer = 1
            sb1.Append("<tbody>")
            Try
                For j As Integer = 0 To vehiclestable.Rows.Count - 1
                    Try
                        sb1.Append("<tr>")
                        sb1.Append("<td>")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(1))
                        sb1.Append("</td>")
                        sb1.Append("<td>")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(14))
                        sb1.Append("</td>")
                        sb1.Append("<td>")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(15))
                        sb1.Append("</td>")
                        sb1.Append("<td>")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(2))
                        sb1.Append("</td>")
                        sb1.Append("<td>")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(13))
                        sb1.Append("</td>")
                        sb1.Append("<td>")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(4))
                        sb1.Append("</td>")
                        sb1.Append("<td>")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(3))
                        sb1.Append("</td>")
                        sb1.Append("<td>")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(5))
                        sb1.Append("</td>")
                        sb1.Append("<td>")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(6))
                        sb1.Append("</td>")
                        sb1.Append("<td align=""right"">")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(7))
                        sb1.Append("</td>")
                        sb1.Append("<td>")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(9))
                        sb1.Append("</td>")
                        sb1.Append("<td>")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(10))
                        sb1.Append("</td>")
                        sb1.Append("<td>")
                        sb1.Append(vehiclestable.DefaultView.Item(j)(16))
                        sb1.Append("</td>")
                        '    If userid = "1997" Or userid = "2012" Or userid = "156" Then
                        'sb1.Append("<td align=""right"">")
                        'sb1.Append(vehiclestable.DefaultView.Item(j)(11))
                        'sb1.Append("</td>")
                        'sb1.Append("<td >")
                        'sb1.Append(vehiclestable.DefaultView.Item(j)(12))
                        'sb1.Append("</td>")
                        '   End If
                        sb1.Append("</tr>")

                        counter += 1
                    Catch ex As Exception
                        Response.Write(ex.Message)
                    End Try
                Next
            Catch ex As Exception
                Response.Write(ex.Message)
            End Try



            sb1.Append("<tfoot><tr style=""font-weight:bold;"" align=""left""><th style='padding-left:20px;'>S No</th><th style='width:120px'>Username</th><th style='width:150px'>Transporter Name</th><th style='padding-left:34px;'>Plate No</th><th>PM ID</th><th style='padding-left:25px;'>Group Name</th><th style='padding-left:20px;'>Type</th><th  style='padding-left:20px;'>Brand</th><th  style='padding-left:20px;' >Model</th><th  style='padding-left:20px;' >Speed</th><th style='padding-left:20px;' >PTO</th><th style='padding-left:20px;' >Driver Mobile No</th><th style='padding-left:20px;'>Base Plant</th>")

            ' If userid = "1997" Or userid = "2012" Or userid = "156" Then
            'sb1.Append("<th style='padding-left:20px;' >Odometer</th><th style='padding-left:20px;width:120px' >Rec Date</th>")
            '  End If
            sb1.Append("</tr></tfoot>")
            sb1.Append("</tbody>")


            vehiclestable.Columns.Remove("chk")
            vehiclestable.Columns.Remove("VersionID")
            vehiclestable.Columns.Remove("Odometer")
            vehiclestable.Columns.Remove("Recarded Date")

            vehiclestable.Columns("Sno").SetOrdinal(0)
            vehiclestable.Columns("Username").SetOrdinal(1)
            vehiclestable.Columns("TransporterName").SetOrdinal(2)
            vehiclestable.Columns("Platenumber").SetOrdinal(3)
            vehiclestable.Columns("Prime Mover ID").SetOrdinal(4)
            vehiclestable.Columns("GroupName").SetOrdinal(5)
            vehiclestable.Columns("Type").SetOrdinal(6)
            vehiclestable.Columns("Brand").SetOrdinal(7)
            vehiclestable.Columns("Model").SetOrdinal(8)
            vehiclestable.Columns("Speedlimit").SetOrdinal(9)
            vehiclestable.Columns("PTO").SetOrdinal(10)
            vehiclestable.Columns("DriverMobile").SetOrdinal(11)
            vehiclestable.Columns("BasePlant").SetOrdinal(12)

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session.Remove("exceltable3")
            Session("exceltable") = vehiclestable

        Catch ex As Exception
            Response.Write("fill: " & ex.Message)
        End Try
    End Sub

    Protected Sub radioTransporterType_SelectedIndexChanged(sender As Object, e As EventArgs)
        fillDrop(radioTransporterType.SelectedValue)
        FillVehiclesGrid()
    End Sub
End Class
