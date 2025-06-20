Imports System.Data.SqlClient
Imports System.Collections.Generic

Partial Class GetOssDetails
    Inherits System.Web.UI.Page
    Public sb1 As New StringBuilder()

    Public Sub FillGrid()
        Try
            Dim dnid As String = Request.QueryString("p")
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim tankerQuery As String = "select plateno from vehicleTBL"

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

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

                End Try
            Next


            If platecond.Length > 3 Then
                platecond = platecond.Substring(0, platecond.Length - 1)
                platecond = " and plateno in (" & platecond & ")"
            End If

            Dim columnname As String = "weight_outtime"
            Dim r As DataRow

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("chk"))
            t.Columns.Add(New DataColumn("S NO"))
            t.Columns.Add(New DataColumn("Plate NO"))
            t.Columns.Add(New DataColumn("Unit ID"))
            t.Columns.Add(New DataColumn("Transporter"))
            t.Columns.Add(New DataColumn("Source"))
            t.Columns.Add(New DataColumn("DN NO"))
            t.Columns.Add(New DataColumn("Weight Outtime"))
            t.Columns.Add(New DataColumn("Ship To Code"))
            t.Columns.Add(New DataColumn("Ship To Name"))
            t.Columns.Add(New DataColumn("ATA"))
            t.Columns.Add(New DataColumn("GPS Point"))
            t.Columns.Add(New DataColumn("Status"))
            t.Columns.Add(New DataColumn("Remarks"))
            t.Columns.Add(New DataColumn("DN ID"))
            t.Columns.Add(New DataColumn("Group Name"))


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

                    End Try
                End While

            Catch ex As Exception

            Finally
                conn.Close()
            End Try

            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))

            cmd = New SqlCommand("select * from oss_patch_out where dn_id='" & dnid & "' and status=7 ", conn)

            'Response.Write(cmd.CommandText) 
            Try
                conn.Open()

                dr = cmd.ExecuteReader()

                Dim i As Int32 = 1
                While dr.Read()
                    Try
                        '  If dr("destination_siteid").ToString().Trim().Length <> 3 Then
                        r = t.NewRow

                        r(0) = dr("patch_no")
                        r(1) = i.ToString()
                        r(2) = dr("plateno")

                        r(3) = dr("unitid")
                        If IsDBNull(dr("transporter")) Then
                            r(4) = "--"
                        Else
                            r(4) = dr("transporter")
                        End If

                        r(5) = dr("source_supply")
                        r(6) = dr("dn_no")


                        Dim p = DateTime.Parse(dr("weight_outtime")).ToString("yyyy/MM/dd  HH:mm:ss")
                        p = p.Replace("-", "/")

                        r(7) = p

                        r(8) = dr("destination_siteid")


                        If ShipToNameDict.ContainsKey(dr("destination_siteid")) Then
                            r(9) = ShipToNameDict.Item(dr("destination_siteid")).ToUpper()
                        Else
                            r(9) = "--"
                        End If


                        Dim lat As Double = 0
                        Dim lon As Double = 0

                        If IsDBNull(dr("ata_datetime")) = False Then
                            r(10) = DateTime.Parse(dr("ata_datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                        Else
                            r(10) = "--"
                        End If

                        If IsDBNull(dr("lat")) = False Then
                            r(11) = Convert.ToDouble(dr("lat")).ToString("0.0000") & "," & Convert.ToDouble(dr("lon")).ToString("0.0000")
                        Else
                            r(11) = "--"
                        End If

                        Dim status As String = dr("status").ToString()

                        Select Case status
                            Case 0
                                If dr("destination_siteid") <> Nothing Then
                                    status = "Waiting To Process"
                                Else
                                    status = "Waiting for Ship To Code"
                                End If
                            Case 1
                                status = "No GPS Device"
                            Case 2
                                status = "Pending Destination Set Up"
                            Case 3
                                status = "In Progress"
                            Case 4
                                status = "Geofence In"
                            Case 5
                                status = "Inside Geofence"
                            Case 6
                                status = "Geofence Out"
                            Case 7
                                status = "Delivery Completed"
                            Case 8
                                status = "Delivery Completed (E)"
                            Case 10
                                status = "Timeout"
                            Case 11
                                status = "Reprocess Job"
                            Case 12
                                status = "Delivery Completed (D)"
                            Case 13
                                status = "Delivery Completed (P)"
                            Case 14
                                status = "No GPS Data"
                            Case Else

                        End Select

                        Try
                            r(12) = status
                            If IsDBNull(dr("remarks")) = False Then
                                r(13) = dr("remarks")
                            Else
                                r(13) = "--"
                            End If
                            r(14) = dr("dn_id")

                            r(15) = ""

                            If vehicleDict.ContainsKey(dr("plateno")) Then
                                r(15) = vehicleDict(dr("plateno"))
                            End If

                        Catch ex As Exception
                            Response.Write(ex.Message)
                        End Try
                        t.Rows.Add(r)

                        i = i + 1
                        ' End If
                    Catch ex As Exception

                    End Try

                End While

            Catch ex As Exception

            Finally
                conn.Close()
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

                t.Rows.Add(r)
            End If
            sb1.Append("<thead><tr align=""left""><th>S No</th><th>Plate NO</th><th>Unit ID</th><th>Group Name</th><th>Transporter</th><th>Source</th><th>DN ID</th><th>DN NO</th><th>Weight Out Time</th><th>Ship To Code</th><th>Ship To Name</th><th>ATA</th><th>GPS Point</th><th>Status</th></tr></thead>")

            sb1.Append("<tbody>")
            Dim counter As Integer = 1
            For i As Integer = 0 To t.Rows.Count - 1
                Try
                    sb1.Append("<tr>")
                    sb1.Append("<td>")
                    sb1.Append(t.DefaultView.Item(i)(1))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(2))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(3))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(15))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(4))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(5))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(14))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(6))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(7))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(8))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(9))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(10))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(11))
                    sb1.Append("</td><td>")
                    sb1.Append(t.DefaultView.Item(i)(12))
                    sb1.Append("</td></tr>")
                    counter += 1
                Catch ex As Exception

                End Try
            Next
            sb1.Append("</tbody>")
            sb1.Append("<tfoot><tr style=""font-weight:bold;"" align=""left""><th>S No</th><th>Plate NO</th><th>Unit ID</th><th>Group Name</th><th>Transporter</th><th>Source</th><th>DN ID</th><th>DN NO</th><th>Weight Out Time</th><th>Ship To Code</th><th>Ship To Name</th><th>ATA</th><th>GPS Point</th><th>Status</th></tr></tfoot>")
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        FillGrid()
    End Sub
End Class
