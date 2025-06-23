Imports System.Data.SqlClient
Partial Class VehiclePopup
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As New SqlCommand("select groupname,groupid from vehicle_group where userid='" & Request.QueryString("userid") & "'", conn)
        Dim dr As SqlDataReader
        Try
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlgroupname.Items.Add(New ListItem(dr("groupname"), dr("groupid")))
            End While
            dr.Close()
            cmd.CommandText = "SELECT shiptocode ,geofenceid  from geofence where geofenceid in ('24916','4457','24914','23582','17364','24915','14194','19585','25125','24912','4395','29564','29562','23581','29687') order by shiptocode "
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlbaseplant.Items.Add(New ListItem(dr("shiptocode"), dr("geofenceid")))
            End While
            dr.Close()
            ddlpermit.Items.Clear()
            cmd.CommandText = "select id,name  from customer "
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlpermit.Items.Add(New ListItem(dr("name"), dr("id")))
            End While
            dr.Close()
            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select vehicletype from vehicle_type where status=1 order by vehicletype", conn)
            Dim ds1 As New DataSet
            da1.Fill(ds1)
            txtType.Items.Add(New ListItem("--Select Vehicle Type--", "--Select Vehicle Type--"))
            For i As Int32 = 0 To ds1.Tables(0).Rows.Count - 1
                txtType.Items.Add(New ListItem(ds1.Tables(0).Rows(i).Item("vehicletype"), ds1.Tables(0).Rows(i).Item("vehicletype")))
            Next

        Catch ex As Exception

        Finally
            conn.Close()
        End Try
        ddlgroupname.SelectedValue = Request.QueryString("grpid")
        txtBrand.Text = Request.QueryString("brand")
        lblPlateno.Text = Request.QueryString("plateno")
        txtModel.Text = Request.QueryString("model")
        txtType.SelectedValue = Request.QueryString("type")
        txtSpeed.Text = Request.QueryString("speedlimit")
        txtphone.Text = Request.QueryString("mob")
        txtodometer.Text = Request.QueryString("odometer")
        txtpmid.Text = Request.QueryString("pmid")
        ddlpermit.SelectedValue = Request.QueryString("permit")
        ddlbaseplant.SelectedValue = Request.QueryString("baseplant")
        If Convert.ToBoolean(Request.QueryString("internal")) Then
            ddlbaseplant.Enabled = True
        Else
            ddlbaseplant.Enabled = False
        End If
        Dim MydateTime As DateTime
        Try
            MydateTime = Convert.ToDateTime(Request.QueryString("recdate"))
            txtRecDate.Text = MydateTime.ToString("yyyy/MM/dd")
            ddlbh.SelectedValue = MydateTime.ToString("HH")
            ddlbm.SelectedValue = MydateTime.ToString("mm")
        Catch ex As Exception

        End Try


    End Sub
End Class
