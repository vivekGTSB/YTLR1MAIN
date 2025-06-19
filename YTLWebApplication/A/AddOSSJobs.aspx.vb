Imports System.Data.SqlClient
Imports System.Web.Script.Services
Imports System.Web.Services

Public Class AddOSSJobs
    Inherits System.Web.UI.Page
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

        Catch ex As Exception

        End Try
        MyBase.OnInit(e)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim role As String = Request.Cookies("userinfo")("role")
        Dim userslist As String = Request.Cookies("userinfo")("userslist")
        If userid = "0002" Or userid = "7050" Or userid = "7120" Then
            'allow
        Else
            Response.Redirect("Login.aspx")
        End If
        If Page.IsPostBack = False Then
            alertbox.Visible = False
            Dim con As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmd As New SqlCommand("select transporterid ,transportername  from oss_transporter_master order by transportername", con)
            Dim dr As SqlDataReader
            Try
                con.Open()
                dr = cmd.ExecuteReader()
                ddltransporter.Items.Clear()
                ddltransporter.Items.Add(New ListItem("Select Transporter", 0))
                While dr.Read()
                    ddltransporter.Items.Add(New ListItem(dr("transportername").ToString().ToUpper(), dr("transporterid")))
                End While
                dr.Close()
                cmd.CommandText = "select productid  ,productname   from oss_product_master where productname<>'' order by productname"
                dr = cmd.ExecuteReader()
                ddlproduct.Items.Clear()
                ddlproduct.Items.Add(New ListItem("Select Product", 0))
                While dr.Read()
                    ddlproduct.Items.Add(New ListItem(dr("productname").ToString().ToUpper(), dr("productid")))
                End While
            Catch ex As Exception
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End If

    End Sub
    <WebMethod>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function AddJob(ByVal dnno As String, ByVal dnid As String, ByVal weightouttime As String, ByVal plateno As String, ByVal plant As String, ByVal destinationsiteid As String, ByVal destinationsite As String, ByVal trnasporterid As String, ByVal trnasporter As String, ByVal drivername As String, ByVal driveric As String, ByVal dnqty As String, ByVal areaid As String, ByVal area As String, ByVal productid As String, ByVal product As String, ByVal orderno As String) As Resposne
        Dim res As New Resposne()
        res.Code = 50
        res.Response = "Basic"
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim cmd As New SqlCommand()
        Dim dr As SqlDataReader
        Try
            cmd.CommandText = "select dbo.fn_CheckDNID(@dnno,@dnid) as res"
            cmd.Parameters.AddWithValue("@dnno", dnno)
            cmd.Parameters.AddWithValue("@dnid", dnid)
            cmd.Connection = conn
            conn.Open()
            dr = cmd.ExecuteReader()
            Dim Re As String = ""
            If dr.Read() Then
                Re = dr("res")
            End If
            dr.Close()
            If Re = "1" Then
                res.Response = "DN Number already Exists"
                res.Code = 1
                Return res
            ElseIf Re = "2" Then
                res.Response = "DN ID already Exists"
                res.Code = 2
                Return res
            Else
                cmd.CommandText = "INSERT INTO dbo.oss_patch_in(dn_no,dn_id,weight_outtime,plateno,source_supply,insert_dt,readed,destination_siteid,transporter,mt,dn_qty,dn_driver,driver_ic,div_bit,div_remark,transporter_id,area_code,area_code_name,productcode,productname,destination_sitename,orderno,sold_to_id,sold_to_name) values(@dn_no,@dn_id,@weight_outtime,@plateno,@source_supply,@insert_dt,@readed,@destination_siteid,@transporter,@mt,@dn_qty,@dn_driver,@driver_ic,@div_bit,@div_remark,@transporter_id,@area_code,@area_code_name,@productcode,@productname,@destination_sitename,@orderno,@sold_to_id,@sold_to_name)"
                cmd.Parameters.AddWithValue("@dn_no", System.Uri.UnescapeDataString(dnno))
                cmd.Parameters.AddWithValue("@dn_id", System.Uri.UnescapeDataString(dnid))
                cmd.Parameters.AddWithValue("@weight_outtime", Convert.ToDateTime(weightouttime).ToString("yyyy/MM/dd HH:mm:ss"))
                cmd.Parameters.AddWithValue("@plateno", System.Uri.UnescapeDataString(plateno))
                cmd.Parameters.AddWithValue("@source_supply", plant)
                cmd.Parameters.AddWithValue("@insert_dt", DateTime.Now().ToString("yyyy/MM/dd HH:mm:sss"))
                cmd.Parameters.AddWithValue("@readed", 0)
                cmd.Parameters.AddWithValue("@destination_siteid", destinationsiteid)
                cmd.Parameters.AddWithValue("@transporter", System.Uri.UnescapeDataString(trnasporter))
                cmd.Parameters.AddWithValue("@mt", " ")
                cmd.Parameters.AddWithValue("@dn_qty", dnqty)
                cmd.Parameters.AddWithValue("@dn_driver", System.Uri.UnescapeDataString(drivername))
                cmd.Parameters.AddWithValue("@driver_ic", System.Uri.UnescapeDataString(driveric))
                cmd.Parameters.AddWithValue("@div_bit", 0)
                cmd.Parameters.AddWithValue("@div_remark", " ")
                cmd.Parameters.AddWithValue("@transporter_id", trnasporterid)
                cmd.Parameters.AddWithValue("@area_code", areaid)
                cmd.Parameters.AddWithValue("@area_code_name", System.Uri.UnescapeDataString(area))
                cmd.Parameters.AddWithValue("@productcode", productid)
                cmd.Parameters.AddWithValue("@productname", System.Uri.UnescapeDataString(product))
                cmd.Parameters.AddWithValue("@destination_sitename", System.Uri.UnescapeDataString(destinationsite))
                cmd.Parameters.AddWithValue("@orderno", System.Uri.UnescapeDataString(orderno))
                cmd.Parameters.AddWithValue("@sold_to_id", " ")
                cmd.Parameters.AddWithValue("@sold_to_name", " ")
                If cmd.ExecuteNonQuery() > 0 Then
                    res.Code = 3
                    res.Response = "Job Added Successfully"
                End If
            End If
        Catch ex As Exception
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return res
    End Function

    Structure Resposne
        Public Code As Integer
        Public Response As String
    End Structure

    'Protected Sub btnsubmit_Click(sender As Object, e As EventArgs) Handles btnsubmit.Click
    '    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
    '    Dim cmd As New SqlCommand()
    '    Dim dr As SqlDataReader
    '    Try
    '        cmd.CommandText = "select dbo.fn_CheckDNID(@dnno,@dnid) as res"
    '        cmd.Parameters.AddWithValue("@dnno", txtdn_no.Text)
    '        cmd.Parameters.AddWithValue("@dnid", txtdn_id.Text)
    '        cmd.Connection = conn
    '        dr = cmd.ExecuteReader()
    '        Dim Res As Integer = 0
    '        If dr.Read() Then
    '            Res = dr("res")
    '        End If
    '        dr.Close()
    '        If Res = "1" Then
    '            alertbox.Text = "DN Number already Exists"
    '            alertbox.Visible = True
    '            Return
    '        ElseIf Res = "2" Then
    '            alertbox.Text = "DN ID already Exists"
    '            alertbox.Visible = True
    '            Return
    '        Else

    '        End If
    '    Catch ex As Exception

    '    End Try

    'End Sub

    'Protected Sub txtdn_id_TextChanged(sender As Object, e As EventArgs) Handles txtdn_id.TextChanged
    '    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
    '    Dim dr As SqlDataReader
    '    Try
    '        Dim cmd As New SqlCommand("select * from oss_patch_in where dn_id=@dn_id", conn)
    '        cmd.Parameters.AddWithValue("@dn_id", txtdn_id.Text)
    '        conn.Open()
    '        dr = cmd.ExecuteReader()
    '        If dr.HasRows() Then
    '            alertbox.Text = "DN ID already Exists"
    '            alertbox.Visible = True
    '        Else

    '        End If
    '    Catch ex As Exception
    '        If conn.State = ConnectionState.Open Then
    '            conn.Close()
    '        End If
    '    End Try
    'End Sub

    'Protected Sub txtdn_no_TextChanged(sender As Object, e As EventArgs) Handles txtdn_no.TextChanged
    '    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
    '    Dim dr As SqlDataReader
    '    Try
    '        Dim cmd As New SqlCommand("select * from oss_patch_in where dn_no=@dn_no", conn)
    '        cmd.Parameters.AddWithValue("@dn_no", txtdn_no.Text)
    '        conn.Open()
    '        dr = cmd.ExecuteReader()
    '        If dr.HasRows() Then
    '            alertbox.Text = "DN Number already Exists"
    '            alertbox.Visible = True
    '        Else

    '        End If
    '    Catch ex As Exception
    '        If conn.State = ConnectionState.Open Then
    '            conn.Close()
    '        End If
    '    End Try
    'End Sub
End Class