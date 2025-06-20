Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Public Class GetShiptoData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Dim bdt As String = Request.QueryString("bdt")
            Dim edt As String = Request.QueryString("edt")
            Dim type As Int16 = Request.QueryString("type")
            Response.Write(FillGrid(bdt, edt, type))
            Response.ContentType = "text/json"

        Catch ex As Exception

        End Try
    End Sub
    Public Function FillGrid(ByVal bdt As String, ByVal edt As String, ByVal type As Int16) As String
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim json As String = Nothing
        Dim pendingdesti As New List(Of Int32)
        Try
            Dim aa As New ArrayList
            Dim a As ArrayList
            cmd.Connection = conn
            If type = 0 Then
                cmd.CommandText = "select  distinct destination_siteid from oss_patch_out where weight_outtime between  @bdt and @edt and status=2"
            ElseIf type = 1 Then
                cmd.CommandText = "select  distinct destination_siteid from oss_patch_out where weight_outtime between  @bdt and @edt and status=2 and productcode in (select productid from oss_product_master where producttype=1)"
            Else
                cmd.CommandText = "select  distinct destination_siteid from oss_patch_out where weight_outtime between  @bdt and @edt and status=2 and productcode in (select productid from oss_product_master where producttype=2)"
            End If

            cmd.Parameters.AddWithValue("@bdt", bdt)
            cmd.Parameters.AddWithValue("@edt", edt)
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                pendingdesti.Add(dr("destination_siteid"))
            End While
            dr.Close()
            If type = 0 Then
                cmd.CommandText = "select geofencename as shiptoname ,t1.shiptocode,t2.count  from YTLDB .dbo.geofence  as t1, (select distinct top 50000 destination_siteid as shiptocode,count(*) as count  from oss_patch_out where weight_outtime between @bdt and @edt group by destination_siteid  order by count desc) as t2  where t1.shiptocode=t2.shiptocode order by t1.geofencename"
            ElseIf type = 1 Then
                cmd.CommandText = "select t1.name as shiptoname,t2.shiptocode,t2.count from oss_ship_to_code as t1,(select top 50000 destination_siteid as shiptocode,count(*) as count from oss_patch_out where weight_outtime between @bdt and @edt and productcode in (select productid from oss_product_master where producttype=1) group by destination_siteid order by count desc) as t2 where t1.shiptocode=t2.shiptocode order by t1.name"
            Else
                cmd.CommandText = "select t1.name as shiptoname,t2.shiptocode,t2.count from oss_ship_to_code as t1,(select top 50000 destination_siteid as shiptocode,count(*) as count from oss_patch_out where weight_outtime between @bdt and @edt and productcode in (select productid from oss_product_master where producttype=1) group by destination_siteid order by count desc) as t2 where t1.shiptocode=t2.shiptocode order by t1.name"
            End If
            Dim ShiptoTable As New DataTable
            ShiptoTable.Columns.Add(New DataColumn("Code"))
            ShiptoTable.Columns.Add(New DataColumn("Name"))
            ShiptoTable.Columns.Add(New DataColumn("Status"))
            Dim r As DataRow
            dr = cmd.ExecuteReader()
            Dim i As Integer = 1
            While dr.Read()
                r = ShiptoTable.NewRow()
                r(0) = dr("shiptocode").ToString()
                r(1) = dr("shiptoname").ToString().ToUpper() & " @ " & dr("count").ToString() & " - " & dr("shiptocode").ToString()
                If pendingdesti.Contains(Convert.ToInt32(dr("shiptocode"))) Then
                    r(2) = "1"
                Else
                    r(2) = "0"
                End If

                ShiptoTable.Rows.Add(r)
            End While
            dr.Close()
            If ShiptoTable.Rows.Count = 0 Then
                r = ShiptoTable.NewRow()
                r(0) = "-"
                r(1) = "-"
                r(2) = "-"
                ShiptoTable.Rows.Add(r)
            End If
            For i = 0 To ShiptoTable.Rows.Count - 1
                a = New ArrayList
                a.Add(ShiptoTable.Rows(i)(0))
                a.Add(ShiptoTable.Rows(i)(1))
                a.Add(ShiptoTable.Rows(i)(2))
                aa.Add(a)
            Next
            json = JsonConvert.SerializeObject(aa, Formatting.None)

            'HttpContext.Current.Session.Remove("exceltable")
            'HttpContext.Current.Session.Remove("exceltable2")
            'HttpContext.Current.Session.Remove("exceltable3")
            'HttpContext.Current.Session.Remove("tempTable")
            'HttpContext.Current.Session("exceltable") = ShiptoTable

        Catch ex As Exception
            Return ex.Message
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return json
    End Function



End Class