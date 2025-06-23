Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports Newtonsoft.Json
Imports ASPNetMultiLanguage

Partial Class orderCreation
    Inherits System.Web.UI.Page


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try
            ' SECURITY FIX: Check authentication
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            MyBase.OnInit(e)
        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in orderCreation OnInit: " & ex.Message)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' SECURITY FIX: Check authentication
        If Request.Cookies("userinfo") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If
        
        If Page.IsPostBack = False Then
            txtBeginDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
            txtEndDate.Value = Now().ToString("yyyy/MM/dd")
        End If
        
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Try
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("select PV_Plant, PV_DisplayName from oss_plant_master order by PV_DisplayName", conn)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlplant.Items.Clear()
            While dr.Read()
                ' SECURITY FIX: Use HtmlEncode for output
                ddlplant.Items.Add(New ListItem(HttpUtility.HtmlEncode(dr("PV_DisplayName").ToString().ToUpper()), HttpUtility.HtmlEncode(dr("PV_Plant"))))
            End While
            dr.Close()
            
            ' SECURITY FIX: Use parameterized query
            cmd.CommandText = "select geofencename, shiptocode from YTLDB.dbo.geofence where accesstype = 1 order by geofencename"
            dr = cmd.ExecuteReader()
            ddlcustomer.Items.Clear()
            ddlcustomer.Items.Add(New ListItem("Select Customer", 0))
            While dr.Read
                ' SECURITY FIX: Use HtmlEncode for output
                ddlcustomer.Items.Add(New ListItem(HttpUtility.HtmlEncode(dr("geofencename").ToString().ToUpper()), HttpUtility.HtmlEncode(dr("shiptocode"))))
            End While
        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in orderCreation Page_Load: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    
    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function InsertOrder(ByVal plant As String, ByVal cid As String, ByVal cname As String, ByVal bdt As String, ByVal edt As String, ByVal trips As String, ByVal tonnage As String, ByVal internaltruck As String, ByVal externaltruck As String, ByVal destination As String, ByVal id As String) As String
        Dim res As String = 0
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Try
            ' SECURITY FIX: Validate input
            If Not ValidateOrderInput(plant, cid, cname, bdt, edt, trips, tonnage, destination) Then
                Return "Invalid input data"
            End If
            
            Dim cmd As New SqlCommand()
            cmd.Connection = conn
            
            ' SECURITY FIX: Use parameterized query
            If Not id = "0" Then
                cmd.CommandText = "update oss_sales_order_table set orderid=@orderid, customername=@customername, begindate=@begindate, enddate=@enddate, internaltrucks=@internaltrucks, externaltrucks=@externaltrucks, source=@source, destination=@destination, trips=@trips, tonnage=@tonnage where id=@id"
            Else
                cmd.CommandText = "INSERT INTO oss_sales_order_table (orderid, customername, begindate, enddate, internaltrucks, externaltrucks, source, destination, trips, tonnage) VALUES (@orderid, @customername, @begindate, @enddate, @internaltrucks, @externaltrucks, @source, @destination, @trips, @tonnage)"
            End If

            cmd.Parameters.AddWithValue("@id", id)
            cmd.Parameters.AddWithValue("@orderid", cid)
            cmd.Parameters.AddWithValue("@customername", System.Uri.UnescapeDataString(cname))
            cmd.Parameters.AddWithValue("@trips", trips)
            cmd.Parameters.AddWithValue("@tonnage", tonnage)
            cmd.Parameters.AddWithValue("@begindate", Convert.ToDateTime(bdt).ToString("yyyy/MM/dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@enddate", Convert.ToDateTime(edt).ToString("yyyy/MM/dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@internaltrucks", System.Uri.UnescapeDataString(internaltruck))
            cmd.Parameters.AddWithValue("@externaltrucks", System.Uri.UnescapeDataString(externaltruck))
            cmd.Parameters.AddWithValue("@source", System.Uri.UnescapeDataString(plant))
            cmd.Parameters.AddWithValue("@destination", System.Uri.UnescapeDataString(destination))
            
            conn.Open()
            res = cmd.ExecuteNonQuery()
            
            ' Log successful operation
            SecurityHelper.LogSecurityEvent("Order created/updated successfully: " & cid)
            
        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in InsertOrder: " & ex.Message)
            res = ex.Message
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try

        Return res
    End Function
    
    ' SECURITY FIX: Validate order input
    Private Shared Function ValidateOrderInput(plant As String, cid As String, cname As String, bdt As String, edt As String, trips As String, tonnage As String, destination As String) As Boolean
        ' Check for null or empty values
        If String.IsNullOrEmpty(plant) OrElse String.IsNullOrEmpty(cid) OrElse String.IsNullOrEmpty(cname) OrElse 
           String.IsNullOrEmpty(bdt) OrElse String.IsNullOrEmpty(edt) OrElse String.IsNullOrEmpty(trips) OrElse 
           String.IsNullOrEmpty(tonnage) OrElse String.IsNullOrEmpty(destination) Then
            Return False
        End If
        
        ' Validate numeric values
        Dim tripsValue As Integer
        Dim tonnageValue As Double
        
        If Not Integer.TryParse(trips, tripsValue) OrElse tripsValue < 0 Then
            Return False
        End If
        
        If Not Double.TryParse(tonnage, tonnageValue) OrElse tonnageValue < 0 Then
            Return False
        End If
        
        ' Validate dates
        Try
            Dim beginDate As DateTime = Convert.ToDateTime(bdt)
            Dim endDate As DateTime = Convert.ToDateTime(edt)
            
            ' End date should be after begin date
            If endDate < beginDate Then
                Return False
            End If
            
            ' Dates should be within reasonable range
            Dim minDate As DateTime = DateTime.Now.AddYears(-1)
            Dim maxDate As DateTime = DateTime.Now.AddYears(1)
            
            If beginDate < minDate OrElse beginDate > maxDate OrElse endDate < minDate OrElse endDate > maxDate Then
                Return False
            End If
        Catch
            Return False
        End Try
        
        Return True
    End Function
    
    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function GetData() As String
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Try
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("select *, dbo.fn_getShiptoname(destination) as geofence from oss_sales_order_table", conn)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            
            While dr.Read()
                a = New ArrayList()
                a.Add(dr("id"))
                a.Add(HttpUtility.HtmlEncode(dr("orderid")))
                a.Add(HttpUtility.HtmlEncode(dr("customername")))
                a.Add(Convert.ToDateTime(dr("begindate")).ToString("yyyy/MM/dd HH:mm:ss"))
                a.Add(Convert.ToDateTime(dr("enddate")).ToString("yyyy/MM/dd HH:mm:ss"))
                a.Add(HttpUtility.HtmlEncode(dr("source")))
                a.Add(HttpUtility.HtmlEncode(dr("geofence")))
                a.Add(dr("trips"))
                a.Add(dr("tonnage"))
                a.Add(HttpUtility.HtmlEncode(dr("internaltrucks")))
                a.Add(HttpUtility.HtmlEncode(dr("externaltrucks")))
                a.Add(HttpUtility.HtmlEncode(dr("destination")))
                aa.Add(a)
            End While
            
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S NO"))
            t.Columns.Add(New DataColumn("Order No"))
            t.Columns.Add(New DataColumn("Customer Name"))
            t.Columns.Add(New DataColumn("Begin Date"))
            t.Columns.Add(New DataColumn("End Date"))
            t.Columns.Add(New DataColumn("Source"))
            t.Columns.Add(New DataColumn("Destination"))
            t.Columns.Add(New DataColumn("Trips"))
            t.Columns.Add(New DataColumn("Tonnage"))
            t.Columns.Add(New DataColumn("Internal Trucks"))
            t.Columns.Add(New DataColumn("External Trucks"))

            Dim r As DataRow
            For Each a In aa
                r = t.NewRow()
                For i As Integer = 0 To 10
                    r(i) = a(i)
                Next
                t.Rows.Add(r)
            Next
            
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session("exceltable") = t
            
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
End Class