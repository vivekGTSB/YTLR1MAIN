Imports System.Data.SqlClient
Imports System.Data
Imports ADODB
Imports AspMap
Imports System.IO
Public Class OssGeofenceDiffReportNew
    Inherits System.Web.UI.Page
    Public reportDateTime As String = ""
    Public noDataText As String = ""
    Public ec As String = "false"
    Public show As Boolean = False
    Public isD As String = "false"
    Public sb1 As New StringBuilder()
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim con As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmd As New SqlCommand("select * from YTLOSS.dbo.fn_GetAssignedPlants(" & userid & ") order by PV_Plant", con)
            con.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlPlants.Items.Clear()
            ddlPlants.Items.Add(New ListItem("ALL SOURCES", "ALL"))
            While dr.Read()
                ddlPlants.Items.Add(New ListItem(dr("PV_Plant") & " - " & dr("PV_DisplayName"), dr("PV_Plant")))
            End While
            con.Close()
            'ddlPlants.SelectedValue = Request.Form("ddlPlants")

        Catch ex As Exception
            WriteLog("4" & ex.Message)
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
            If Page.IsPostBack Then
                ddlPlants.SelectedValue = Request.Form("ddlPlants")
            End If

            Dim callfromDashBoard As String = ""
            Try
                callfromDashBoard = Request.QueryString("d")
            Catch ex As Exception
                callfromDashBoard = ""
            End Try

            If callfromDashBoard = "1" Then
                isD = "true"
                DisplayLogInformation()
            End If
        Catch ex As Exception
            WriteLog("3" & ex.Message)
        End Try
    End Sub



    Protected Sub DisplayLogInformation()
        Try
            noDataText = ""
            Dim MainShipToCode As String = ""
            Dim shiptocode As String = ddlPlants.SelectedValue
            MainShipToCode = shiptocode
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Vehicle Type"))
            t.Columns.Add(New DataColumn("Transporter Name"))
            t.Columns.Add(New DataColumn("Plant In Time"))
            t.Columns.Add(New DataColumn("In Plant"))
            t.Columns.Add(New DataColumn("In Oss"))
            t.Columns.Add(New DataColumn("DN"))
            t.Columns.Add(New DataColumn("Source"))
            t.Columns.Add(New DataColumn("Map"))
            t.Columns.Add(New DataColumn("GeoName"))
            t.Columns.Add(New DataColumn("Truck Code"))
            Dim query As String = ""
            Dim condition As String = ""
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim userid As String = Request.Cookies("userinfo")("userid")
            reportDateTime = "Report Generated Date Time:" & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            query = "select distinct t1.plateno,t1.intimestamp as intime,t1.id,t1.inlat,t1.inlon,t2.pmid,dbo.fn_GetGeofenceName(t1.id) as geoname,isnull(t2.type,'-') as type,t3.username  from (select * from public_geofence_History   where id in (select geofenceid from geofence where shiptocode='" & shiptocode & "')  and intimestamp between '" & DateTime.Now.AddHours(-720).ToString("yyyy/MM/dd HH:mm:ss") & "' and GetDate() and outtimestamp is null) t1 left outer join vehicletbl t2 on t1.plateno=t2.plateno left outer join usertbl t3 on t2.userid=t3.userid  order by t1.plateno "
            If MainShipToCode = "ALL" Then
                query = "select distinct t1.plateno,t1.intimestamp as intime,t1.id,t1.inlat,t1.inlon,t2.pmid,dbo.fn_GetGeofenceName(t1.id) as geoname,isnull(t2.type,'-') as type,t3.username  from (select * from public_geofence_History   where id in (select geofenceid from geofence where shiptocode in ( select pv_plant from YTLOSS.dbo.fn_GetAssignedPlants(" & userid & ")))  and intimestamp between '" & DateTime.Now.AddHours(-720).ToString("yyyy/MM/dd HH:mm:ss") & "' and GetDate() and outtimestamp is null) t1 left outer join vehicletbl t2 on t1.plateno=t2.plateno left outer join usertbl t3 on t2.userid=t3.userid  order by t1.plateno "
            End If
            ' Response.Write(query)
            Dim ds As New DataSet
            Dim da As New SqlDataAdapter(query, conn)
            da.Fill(ds)
            Dim cmd As SqlCommand
            conn2.Open()
            For counter As Integer = 0 To ds.Tables(0).Rows.Count - 1
                If MainShipToCode = "ALL" Then
                    Select Case ds.Tables(0).Rows(counter)("id").ToString()
                        'Case "4395"
                        '    shiptocode = "KP"
                        'Case "4457"
                        '    shiptocode = "TB"
                        'Case "14194"
                        '    shiptocode = "PG1"
                        'Case "23581"
                        '    shiptocode = "BC"
                        'Case "23582"
                        '    shiptocode = "PR"
                        'Case "24912"
                        '    shiptocode = "KT"
                        'Case "24914"
                        '    shiptocode = "RW"
                        'Case "24915"
                        '    shiptocode = "PG2"
                        'Case "24916"
                        '    shiptocode = "WP"
                        'Case "25125"
                        '    shiptocode = "LK"
                        'Case "29562"
                        '    shiptocode = "BS"
                        'Case "29563"
                        '    shiptocode = "WP2"
                        Case "29563"
                            shiptocode = "WP2"
                        Case "24916"
                            shiptocode = "WP"
                        Case "4457"
                            shiptocode = "TB"
                        Case "24914"
                            shiptocode = "RW"
                        Case "23582"
                            shiptocode = "PR"
                        Case "17364"
                            shiptocode = "PG3"
                        Case "24915"
                            shiptocode = "PG2"
                        Case "14194"
                            shiptocode = "PG1"
                        Case "19585"
                            shiptocode = "LM"
                        Case "25125"
                            shiptocode = "LK"
                        Case "24912"
                            shiptocode = "KT"
                        Case "4395"
                            shiptocode = "KP"
                        Case "29564"
                            shiptocode = "GPK"
                        Case "29562"
                            shiptocode = "BS"
                        Case "23581"
                            shiptocode = "BC"
                        Case Else
                    End Select
                End If
                query = "select * from oss_patch_out where weight_outtime  between '" & DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss") & "' and GetDate()  and plateno ='" & ds.Tables(0).Rows(counter)("plateno") & "'  and source_supply='" & shiptocode & "'"
                cmd = New SqlCommand(query, conn2)
                ' Response.Write("Oss 2 : " & query & "<br/>")
                Try

                    Dim dr As SqlDataReader = cmd.ExecuteReader()
                    Dim r As DataRow
                    r = t.NewRow
                    r(0) = (counter + 1).ToString()
                    r(1) = ds.Tables(0).Rows(counter)("plateno").ToString().ToUpper()
                    r(2) = ds.Tables(0).Rows(counter)("type").ToString().ToUpper()
                    r(3) = ds.Tables(0).Rows(counter)("username").ToString().ToUpper()
                    r(4) = Convert.ToDateTime(ds.Tables(0).Rows(counter)("intime")).ToString("yyyy/MM/dd HH:mm:ss")
                    r(5) = "YES"
                    If dr.Read() Then
                        r(6) = "YES"
                        r(7) = dr("dn_no").ToString().ToUpper()
                        r(8) = dr("source_supply").ToString().ToUpper()
                    Else
                        dr.Close()
                        cmd.CommandText = "select * from oss_patch_out where weight_outtime  between '" & DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss") & "' and GetDate()  and plateno ='" & ds.Tables(0).Rows(counter)("plateno") & "'  and source_supply in ('TB','KP') order by weight_outtime desc"
                        dr = cmd.ExecuteReader()
                        If dr.Read() Then
                            r(6) = "YES"
                            r(7) = dr("dn_no").ToString().ToUpper()
                            r(8) = dr("source_supply").ToString().ToUpper()
                        Else
                            r(6) = "NO"
                            r(7) = "--"
                            r(8) = "--"
                        End If

                    End If
                    r(9) = "<a href='http://maps.google.com/maps?f=q&amp;hl=en&amp;q=" & ds.Tables(0).Rows(counter)("inlat") & " + " & ds.Tables(0).Rows(counter)("inlon") & "&amp;om=1&amp;t=k' target='_blank' title='View map in Google Maps'>View on map</a>"
                    r(10) = ds.Tables(0).Rows(counter)("geoname").ToString().ToUpper()
                    r(11) = ds.Tables(0).Rows(counter)("pmid").ToString().ToUpper()
                    t.Rows.Add(r)

                Catch ex As Exception
                    WriteLog("1. " & ex.Message)
                Finally

                End Try
            Next
            conn2.Close()
            Session.Remove("exceltable")
            Session("exceltable") = t
            sb1.Length = 0
            ec = "True"
            If (t.Rows.Count > 0) Then

                sb1.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")

                sb1.Append("<thead><tr><th>S No</th><th>Plate NO</th><th>Truck Code</th><th>Vehicle Type</th><th>Transporter Name</th><th>Plant In Time</th><th>In Plant</th><th>In Oss</th><th>DN Number</th><th>Source</th><th>Map</th></tr></thead>")

                sb1.Append("<tbody>")
                Dim counter As Integer = 1
                For i As Integer = 0 To t.Rows.Count - 1
                    Try
                        sb1.Append("<tr>")
                        sb1.Append("<td>")
                        sb1.Append(t.DefaultView.Item(i)(0))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(1))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(11))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(2))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(3))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(4))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(5))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(6))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(7))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(8))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(9))
                        sb1.Append("</td></tr>")
                        counter += 1
                    Catch ex As Exception

                    End Try
                Next
                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr><th>S No</th><th>Plate NO</th><th>Truck Code</th><th>Vehicle Type</th><th>Transporter Name</th><th>Plant In Time</th><th>In Plant</th><th>In Oss</th><th>DN Number</th><th>Source</th><th>Map</th></tr></tfoot>")
            Else
                noDataText = "No truck inside the plant waiting for order as of 'submit timestamp' "
            End If

        Catch ex As Exception
            Response.Write("5" & ex.Message)
        End Try
    End Sub
    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub
    Protected Sub WriteLog(ByVal message As String)
        Try
            Response.Write(message)
        Catch ex As Exception

        End Try
    End Sub

End Class