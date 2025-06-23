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

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If


            MyBase.OnInit(e)
        Catch ex As Exception

        End Try

    End Sub



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.Cookies("userinfo") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If
        If Page.IsPostBack = False Then
            txtBeginDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
            txtEndDate.Value = Now().ToString("yyyy/MM/dd")
        End If
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Try
            Dim cmd As New SqlCommand("select PV_Plant ,PV_DisplayName   from oss_plant_master order by PV_DisplayName", conn)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlplant.Items.Clear()
            While dr.Read()
                ddlplant.Items.Add(New ListItem(dr("PV_DisplayName").ToString().ToUpper(), dr("PV_Plant")))
            End While
            dr.Close()
            cmd.CommandText = "select geofencename ,shiptocode  from YTLDB.dbo.geofence where accesstype =1 order by geofencename"
            dr = cmd.ExecuteReader()
            ddlcustomer.Items.Clear()
            ddlcustomer.Items.Add(New ListItem("Select Customer", 0))
            While dr.Read
                ddlcustomer.Items.Add(New ListItem(dr("geofencename").ToString().ToUpper(), dr("shiptocode")))
            End While
        Catch ex As Exception
            Response.Write(ex.Message & " - " & ex.StackTrace)
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
            Dim cmd As New SqlCommand()
            cmd.Connection = conn
            If Not id = "0" Then
                cmd.CommandText = "update oss_sales_order_table set orderid=@orderid,customername=@customername,begindate=@begindate,enddate=@enddate,internaltrucks=@internaltrucks,externaltrucks=@externaltrucks,source=@source,destination=@destination,trips=@trips,tonnage=@tonnage where id=@id"
            Else
                cmd.CommandText = "INSERT INTO oss_sales_order_table
           (orderid,customername,begindate,enddate,internaltrucks,externaltrucks,source,destination,trips,tonnage) VALUES (@orderid,@customername,@begindate,@enddate,@internaltrucks,@externaltrucks,@source,@destination,@trips,@tonnage)"
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
        Catch ex As Exception
            res = ex.Message
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try


        Return res
    End Function
    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function GetData() As String
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Try
            Dim cmd As New SqlCommand("select *,dbo.fn_getShiptoname(destination) as geofence from oss_sales_order_table ", conn)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            '  Dim i As Integer = 0
            While dr.Read()
                a = New ArrayList()
                a.Add(dr("id"))
                a.Add(dr("orderid"))
                a.Add(dr("customername"))
                a.Add(Convert.ToDateTime(dr("begindate")).ToString("yyyy/MM/dd HH:mm:ss"))
                a.Add(Convert.ToDateTime(dr("enddate")).ToString("yyyy/MM/dd HH:mm:ss"))
                a.Add(dr("source"))
                a.Add(dr("geofence"))
                a.Add(dr("trips"))
                a.Add(dr("tonnage"))
                a.Add(dr("internaltrucks"))
                a.Add(dr("externaltrucks"))
                a.Add(dr("destination"))
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
