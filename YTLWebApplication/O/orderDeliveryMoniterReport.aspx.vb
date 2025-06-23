Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports AspMap
Imports Newtonsoft.Json

Partial Class orderDeliveryMoniterReport
    Inherits System.Web.UI.Page
    Public Shared ec As String = "false"
    Public show As Boolean = False
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Check authentication
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in orderDeliveryMoniterReport OnInit: " & ex.Message)
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Check authentication
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("select orderid, id, customername, dbo.fn_getShiptoname(destination) as geofence from oss_sales_order_table order by orderid", conn)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlorder.Items.Clear()
            ddlorder.Items.Add(New ListItem("Select Order", "0"))
            While dr.Read()
                ' SECURITY FIX: Use HtmlEncode for output
                ddlorder.Items.Add(New ListItem(HttpUtility.HtmlEncode(dr("orderid") & " - " & dr("geofence")), HttpUtility.HtmlEncode(dr("id"))))
            End While
            conn.Close()
        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in orderDeliveryMoniterReport Page_Load: " & ex.Message)
        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function GetData(ByVal orderid As String) As String
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Dim query As String = ""
        Try
            ' SECURITY FIX: Validate input
            If Not ValidateOrderId(orderid) Then
                Throw New ArgumentException("Invalid order ID")
            End If
            
            Dim cmd As New SqlCommand()
            cmd.Connection = conn
            
            ' SECURITY FIX: Use parameterized query
            cmd.CommandText = "select * from fn_getOrderInfo(@oid)"
            cmd.Parameters.AddWithValue("@oid", orderid)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Dim i As Integer = 1
            While dr.Read()
                a = New ArrayList()
                a.Add(dr("tripsdone"))
                a.Add(dr("tripsinorder"))
                a.Add(dr("tonnagedone"))
                a.Add(dr("tonnageinorder"))
                a.Add(dr("deadline"))
                a.Add(HttpUtility.HtmlEncode(dr("itrucks")))
                a.Add(HttpUtility.HtmlEncode(dr("etrcuks")))
                a.Add(Convert.ToDateTime(dr("bdt")).ToString("yyyy/MM/dd HH:mm:ss") & " - " & Convert.ToDateTime(dr("edt")).ToString("yyyy/MM/dd HH:mm:ss"))
                aa.Add(a)
                i += 1
                HttpContext.Current.Session("internaltruck") = HttpUtility.HtmlEncode(dr("itrucks"))
                HttpContext.Current.Session("externaltruck") = HttpUtility.HtmlEncode(dr("etrcuks"))
            End While

        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in GetData: " & ex.Message)
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
    Public Shared Function GetTable(ByVal orderid As String) As String
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Try
            ' SECURITY FIX: Validate input
            If Not ValidateOrderId(orderid) Then
                Throw New ArgumentException("Invalid order ID")
            End If
            
            Dim shpfileDict As New Dictionary(Of String, AspMap.Shape)
            Dim map As New AspMap.Map
            Dim plantGeofenceLayer As New AspMap.DynamicLayer()
            plantGeofenceLayer.LayerType = LayerType.mcPolygonLayer
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("select shiptocode, data from YTLDB.dbo.geofence where shiptocode in (select PV_Plant from oss_plant_master)", conn)
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
            End While

            If Not dr.IsClosed() Then
                dr.Close()
            End If
            
            Dim loc As New Location()
            cmd.Connection = conn
            cmd.CommandText = "sp_getOrderInfo"
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@orderid", orderid)

            dr = cmd.ExecuteReader()
            While dr.Read()
                a = New ArrayList()
                a.Add(HttpUtility.HtmlEncode(dr("plateno")))
                a.Add(HttpUtility.HtmlEncode(dr("username")))
                a.Add(HttpUtility.HtmlEncode(dr("plateno")))
                a.Add(HttpUtility.HtmlEncode(dr("pmid")))
                a.Add(Convert.ToDateTime(dr("weight_outtime")).ToString("yyyy/MM/dd HH:mm:ss"))
                a.Add(HttpUtility.HtmlEncode(dr("source_supply")))
                a.Add(HttpUtility.HtmlEncode(dr("destination_sitename")))
                
                Dim status As String = dr("status").ToString()
                Select Case status
                    Case "0"
                        status = "Waiting for Ship To Code"
                    Case "1"
                        status = "No GPS Device"
                    Case "2"
                        status = "Pending Destination Set Up"
                    Case "3"
                        status = "In Progress"
                    Case "4"
                        status = "Geofence In"
                    Case "5"
                        status = "Inside Geofence"
                    Case "6"
                        status = "Geofence Out"
                    Case "7"
                        status = "Delivery Completed"
                    Case "8"
                        status = "Delivery Completed (E)"
                    Case "10"
                        status = "Timeout"
                    Case "11"
                        status = "Reprocess Job"
                    Case "12"
                        status = "Delivery Completed (D)"
                    Case "13"
                        status = "Delivery Completed (P)"
                    Case "14"
                        status = "No GPS Data"
                    Case "15"
                        status = "Diversion (Manual)"
                    Case Else
                        status = "Unknown"
                End Select
                
                a.Add(HttpUtility.HtmlEncode(status))
                a.Add(dr("dn_qty"))
                a.Add(HttpUtility.HtmlEncode(loc.GetLocation(dr("lat"), dr("lon"))))
                a.Add(dr("distance"))
                
                Dim vehiclepoint As New Point
                vehiclepoint.X = Convert.ToDouble(dr("lon"))
                vehiclepoint.Y = Convert.ToDouble(dr("lat"))
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
                    ' Log error securely
                    SecurityHelper.LogSecurityEvent("Error calculating distance: " & ex.Message)
                End Try
                
                a.Add(distance.ToString("0"))
                aa.Add(a)
            End While
            
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S NO"))
            t.Columns.Add(New DataColumn("Username"))
            t.Columns.Add(New DataColumn("Plate NO"))
            t.Columns.Add(New DataColumn("PM Id"))
            t.Columns.Add(New DataColumn("Weightout Datetime"))
            t.Columns.Add(New DataColumn("Plant"))
            t.Columns.Add(New DataColumn("Customer"))
            t.Columns.Add(New DataColumn("Job Status"))
            t.Columns.Add(New DataColumn("Dn Qty"))
            t.Columns.Add(New DataColumn("Current Lattest Loc"))
            t.Columns.Add(New DataColumn("Distance To Destination"))
            t.Columns.Add(New DataColumn("Distance From Plant"))

            Dim r As DataRow
            counter = 1
            For Each a In aa
                r = t.NewRow()
                r(0) = counter
                For i As Integer = 1 To 11
                    r(i) = a(i)
                Next
                counter += 1
                t.Rows.Add(r)
            Next
            
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session("exceltable") = t
            
        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in GetTable: " & ex.Message)
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

    ' SECURITY FIX: Validate order ID
    Private Shared Function ValidateOrderId(orderid As String) As Boolean
        If String.IsNullOrEmpty(orderid) Then
            Return False
        End If
        
        ' Check if it's numeric
        Dim orderId As Integer
        If Not Integer.TryParse(orderid, orderId) Then
            Return False
        End If
        
        ' Check if it's a reasonable value
        If orderId <= 0 Then
            Return False
        End If
        
        Return True
    End Function
End Class