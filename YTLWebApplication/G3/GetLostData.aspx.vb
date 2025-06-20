Imports System.Data.SqlClient

Partial Class GetLostData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Dim plateno As String = Request.QueryString("p")
        Dim bdt As String = Request.QueryString("b")
        Dim edt As String = Request.QueryString("e")
        GridView1.DataSource = GetData(plateno, bdt, edt)
        GridView1.DataBind()
    End Sub

    Private Function GetData(plateno As String, bdt As String, edt As String) As DataTable
        Dim t As New DataTable
        t.Columns.Add(New DataColumn("Sl No"))
        t.Columns.Add(New DataColumn("Plate No"))
        t.Columns.Add(New DataColumn("From Time"))
        t.Columns.Add(New DataColumn("To Time"))
        t.Columns.Add(New DataColumn("From Location"))
        t.Columns.Add(New DataColumn("To Location"))
        t.Columns.Add(New DataColumn("Duration"))
        t.Columns.Add(New DataColumn("Type"))
        Dim r As DataRow

        Dim locObj As New Location()

        Dim counter As Integer = 0
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim ds As New DataSet
            Dim da As New SqlDataAdapter("select vh.timestamp,vh.gps_av,vh.ignition_sensor,vt.userid,vh.lat,vh.lon from (select lat,lon,timestamp,gps_av,ignition_sensor,plateno from vehicle_history where plateno='" & plateno & "' and timestamp between '" & Convert.ToDateTime(bdt).ToString("yyyy/MM/dd HH:mm:ss") & "' and '" & Convert.ToDateTime(edt).ToString("yyyy/MM/dd HH:mm:ss") & "') vh  left outer join vehicleTBL vt on vt.plateno=vh.plateno ", conn)
            da.Fill(ds)
            Dim prevstatus As String = "A"
            Dim prevdatetime As DateTime
            Dim currentstatus As String = "A"

            Dim CurrentLoc As AspMap.Point
            Dim prevLoc As AspMap.Point
            Dim tempPrevLoc As New AspMap.Point

            Dim prevlat As Double
            Dim prevlon As Double
            Dim currentlat As Double
            Dim currentlon As Double

            Dim currentdatetime As DateTime
            Dim totaldur As Integer = 0

            Dim tempprevtime As DateTime
            If ds.Tables(0).Rows.Count > 0 Then
                prevdatetime = Convert.ToDateTime(ds.Tables(0).Rows(0)(0))

                prevlon = Convert.ToDouble(ds.Tables(0).Rows(0)("lon"))
                prevlat = Convert.ToDouble(ds.Tables(0).Rows(0)("lat"))

                prevstatus = ds.Tables(0).Rows(0)(1).ToString.ToUpper()
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    currentdatetime = Convert.ToDateTime(ds.Tables(0).Rows(i)("timestamp"))
                    currentstatus = ds.Tables(0).Rows(i)("gps_av").ToString.ToUpper()

                    currentlon = Convert.ToDouble(ds.Tables(0).Rows(i)("lon"))
                    currentlat = Convert.ToDouble(ds.Tables(0).Rows(i)("lat"))

                    Try
                        If i > 1 Then
                            If ds.Tables(0).Rows(i)("ignition_sensor").ToString() = "1" And ds.Tables(0).Rows(i - 1)("ignition_sensor").ToString() = "1" And (currentdatetime - tempprevtime).Minutes > 15 Then
                                counter += 1
                                r = t.NewRow()
                                r(0) = counter
                                r(1) = plateno
                                r(2) = Convert.ToDateTime(tempprevtime).ToString("yyyy/MM/dd HH:mm:ss")
                                r(3) = Convert.ToDateTime(currentdatetime).ToString("yyyy/MM/dd HH:mm:ss")
                                r(4) = locObj.GetLocation(prevlat, prevlon)
                                r(5) = locObj.GetLocation(currentlat, currentlon)
                                'r(4) = prevlat & " - " & prevlon
                                'r(5) = currentlat & " - " & currentlon
                                r(6) = (Convert.ToDateTime(currentdatetime) - Convert.ToDateTime(tempprevtime)).TotalMinutes.ToString("0")
                                r(7) = "Data Loss (On)"
                                t.Rows.Add(r)
                            End If
                            If ds.Tables(0).Rows(i)("ignition_sensor").ToString() = "0" And ds.Tables(0).Rows(i - 1)("ignition_sensor").ToString() = "0" And (currentdatetime - tempprevtime).Minutes > 120 Then
                                counter += 1
                                r = t.NewRow()
                                r(0) = counter
                                r(1) = plateno
                                r(2) = Convert.ToDateTime(tempprevtime).ToString("yyyy/MM/dd HH:mm:ss")
                                r(3) = Convert.ToDateTime(currentdatetime).ToString("yyyy/MM/dd HH:mm:ss")
                                'r(4) = locObj.GetLocation(tempPrevLoc.Y, tempPrevLoc.X)
                                'r(5) = locObj.GetLocation(CurrentLoc.Y, CurrentLoc.X)
                                r(4) = locObj.GetLocation(prevlat, prevlon)
                                r(5) = locObj.GetLocation(currentlat, currentlon)
                                r(6) = (Convert.ToDateTime(currentdatetime) - Convert.ToDateTime(tempprevtime)).TotalMinutes.ToString("0")
                                r(7) = "Data Loss (Off)"
                                t.Rows.Add(r)
                            End If
                        End If

                    Catch ex As Exception
                        Response.Write("3" & ex.Message)
                    End Try

                    If prevstatus <> currentstatus Then
                        Dim temptime As TimeSpan = tempprevtime - prevdatetime  'currenttime - prevtime
                        Dim minutes As Int16 = temptime.TotalMinutes()

                        Select Case prevstatus
                            Case "V"
                                If temptime.Minutes > 15 Then
                                    Try
                                        counter += 1
                                        r = t.NewRow()
                                        r(0) = counter
                                        r(1) = plateno
                                        r(2) = Convert.ToDateTime(prevdatetime).ToString("yyyy/MM/dd HH:mm:ss")
                                        r(3) = Convert.ToDateTime(currentdatetime).ToString("yyyy/MM/dd HH:mm:ss")
                                        r(4) = prevlat & " - " & prevlon
                                        r(5) = currentlat & " - " & currentlon

                                        r(4) = locObj.GetLocation(prevlat, prevlon)
                                        r(5) = locObj.GetLocation(currentlat, currentlon)
                                        r(6) = (Convert.ToDateTime(currentdatetime) - Convert.ToDateTime(prevdatetime)).TotalMinutes.ToString("0")
                                        r(7) = "V - Data"
                                        t.Rows.Add(r)
                                    Catch ex As Exception
                                        Response.Write("2" & ex.Message)
                                    End Try

                                End If
                        End Select
                        prevdatetime = currentdatetime
                        prevstatus = currentstatus
                    End If

                    tempprevtime = currentdatetime
                    prevlat = currentlat
                    prevlon = currentlon
                Next

            End If
        Catch ex As Exception
            Response.Write("1" & ex.Message)
        End Try

        Return t
    End Function

End Class
