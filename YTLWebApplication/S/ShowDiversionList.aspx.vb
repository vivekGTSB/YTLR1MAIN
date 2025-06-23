Imports System.Data.SqlClient
Imports AspMap
Partial Class ShowDiversionList
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                tblDisplay.Visible = False
                Dim id As String = Request.QueryString("id")
                
                ' SECURITY FIX: Validate input
                If Not SecurityHelper.ValidateInput(id, "^[0-9]+$") Then
                    Response.Write("Invalid input parameters")
                    Return
                End If
                
                Dim conn2 As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                
                ' SECURITY FIX: Use parameterized query
                Dim cmd As SqlCommand = New SqlCommand("Select top 1 o.dn_no,o.source_supply,isnull(L.Item_Name,'') as Item_Name,isnull(L.Item_Name,'') as Item_Name,l.UOM,o.dn_qty,o.plateno,o.weight_outtime,o.productcode,o.productname,o.div_remark from ((select * from oss_patch_out where patch_no=@id) o left outer join O2D_LoadTableData l on o.dn_no=l.DN_No )", conn2)
                cmd.Parameters.AddWithValue("@id", id)
                
                Dim plateno As String = ""
                Dim shiptocode As String = ""
                Dim status As String = ""
                Dim shiptoname As String = ""
                Dim DiversionRemarks As String = ""
                Try
                    conn2.Open()
                    Dim dr2 As SqlDataReader = cmd.ExecuteReader()

                    While dr2.Read()
                        plateno = dr2("plateno").ToString()
                        txtDnNo1.Text = dr2("dn_no").ToString()
                        shiptocode = dr2("Item_Name").ToString()
                        shiptoname = dr2("productcode").ToString()
                        DiversionRemarks = dr2("div_remark").ToString()
                    End While

                Catch ex As Exception
                    ' SECURITY FIX: Log error but don't expose details
                    SecurityHelper.LogError("Page Load error", ex, Server)
                Finally
                    conn2.Close()
                End Try

                txtPlateno.Text = HttpUtility.HtmlEncode(plateno)
                txtSTC.Text = HttpUtility.HtmlEncode(shiptocode)
                txtShipToName.Text = HttpUtility.HtmlEncode(shiptoname)
                txtremarksarea.Value = HttpUtility.HtmlEncode(DiversionRemarks)
                
                Dim driverIc As String = ""
                Dim deviceid As String = ""
                m.Value = "You job has been diverted. Please check your new diverted location."
                t.Value = "Diversion Notification Alert"
                skey.Value = "AAAAYk9xI5w:APA91bHcEWZlDNW9qvnzRnysep87KV2li1W8dMQ5JE-_2ER5SRjioR76ayMT810Isvs39qLv1DshkVxZU8vv_u9r5C0-nk7iSibsSECzVv1f6w9BpaoPROx4cIXy9kwUjMKpurou5GEa"
                sid.Value = "apktest089"

                Try
                    Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                    
                    ' SECURITY FIX: Use parameterized query
                    Dim cmd2 As SqlCommand = New SqlCommand("select driver_ic from oss_patch_out where dn_no=@dn", conn)
                    cmd2.Parameters.AddWithValue("@dn", txtDnNo1.Text)
                    
                    Try
                        conn.Open()
                        driverIc = cmd2.ExecuteScalar().ToString()
                    Catch Sqex As SqlException
                        ' SECURITY FIX: Log error but don't expose details
                        SecurityHelper.LogError("SQL error retrieving driver_ic", Sqex, Server)
                    Catch ex As Exception
                        ' SECURITY FIX: Log error but don't expose details
                        SecurityHelper.LogError("Error retrieving driver_ic", ex, Server)
                    Finally
                        conn2.Close()
                    End Try
                Catch ex As SystemException
                    ' SECURITY FIX: Log error but don't expose details
                    SecurityHelper.LogError("System error", ex, Server)
                End Try
                
                Try
                    Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    
                    ' SECURITY FIX: Use parameterized query
                    Dim cmd2 As SqlCommand = New SqlCommand("select IsNull(deviceId,'') as deviceId from driver where driver_ic=@dic", conn)
                    cmd2.Parameters.AddWithValue("@dic", driverIc)
                    
                    Try
                        conn.Open()
                        deviceid = cmd2.ExecuteScalar().ToString()
                        d.Value = deviceid
                    Catch Sqex As SqlException
                        ' SECURITY FIX: Log error but don't expose details
                        SecurityHelper.LogError("SQL error retrieving deviceId", Sqex, Server)
                    Catch ex As Exception
                        ' SECURITY FIX: Log error but don't expose details
                        SecurityHelper.LogError("Error retrieving deviceId", ex, Server)
                    Finally
                        conn.Close()
                    End Try
                Catch ex As SystemException
                    ' SECURITY FIX: Log error but don't expose details
                    SecurityHelper.LogError("System error", ex, Server)
                End Try
            End If

        Catch ex As SystemException
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("Page_Load error", ex, Server)
        End Try
    End Sub


    Protected Sub btnSearchDn_Click(sender As Object, e As EventArgs) Handles btnSearchDn.Click
        tblDisplay.Visible = True
        Try
            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateInput(txtDnNo.Text, "^[0-9]+$") Then
                Response.Write("Invalid load ID")
                Return
            End If

            Dim conn2 As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Use parameterized stored procedure
            Dim cmd As SqlCommand = New SqlCommand("sp_LoadDivertedJobs", conn2)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@loadid", txtDnNo.Text)
            hdnLid.Value = txtDnNo.Text

            Try
                conn2.Open()
                Dim dr2 As SqlDataReader = cmd.ExecuteReader()

                If dr2.Read() Then
                    ' SECURITY FIX: HTML encode output
                    lblCo.Text = HttpUtility.HtmlEncode(dr2("co").ToString())
                    lblDnNo.Text = HttpUtility.HtmlEncode(dr2("dn_no").ToString())
                    lblProduct.Text = HttpUtility.HtmlEncode(dr2("productname").ToString())
                    lblTonnage.Text = HttpUtility.HtmlEncode(dr2("dn_qty").ToString() & " " & dr2("uom").ToString())
                    lblOpenLoad.Text = HttpUtility.HtmlEncode(dr2("OpenLoadCount").ToString())
                    lblONo.Text = HttpUtility.HtmlEncode(dr2("Order_No").ToString())
                    txtOrderNo.Text = HttpUtility.HtmlEncode(dr2("Order_No").ToString())
                    lblCustomerPO.Text = HttpUtility.HtmlEncode(dr2("Cust_PO").ToString())
                    lblCustomer.Text = HttpUtility.HtmlEncode(dr2("Cust_Name").ToString())
                    lblDelTo.Text = HttpUtility.HtmlEncode(dr2("DelTo").ToString())
                    lblDestination.Text = HttpUtility.HtmlEncode(dr2("Destination").ToString())
                    lblSplIns.Text = HttpUtility.HtmlEncode(dr2("sp_ins").ToString())
                    lblSiteId.Text = HttpUtility.HtmlEncode(dr2("shipto_id").ToString())
                End If

            Catch ex As Exception
                ' SECURITY FIX: Log error but don't expose details
                SecurityHelper.LogError("btnSearchDn_Click error", ex, Server)
            Finally
                conn2.Close()
            End Try

        Catch ex As SystemException
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("btnSearchDn_Click system error", ex, Server)
        End Try
    End Sub
    
    Protected Sub btnDivert_Click(sender As Object, e As EventArgs) Handles btnDivert.Click
        Dim result As Int16 = 0
        Try
            ' SECURITY FIX: Validate inputs
            If String.IsNullOrEmpty(txtRemarks.Text) Then
                Response.Write("<script>alert('Please enter remarks');</script>")
                Return
            End If
            
            If Not SecurityHelper.ValidateInput(txtDnNo1.Text, "^[0-9A-Za-z\-]+$") OrElse
               Not SecurityHelper.ValidateInput(lblDnNo.Text, "^[0-9A-Za-z\-]+$") OrElse
               Not SecurityHelper.ValidateInput(txtOrderNo.Text, "^[0-9A-Za-z\-]+$") OrElse
               Not SecurityHelper.ValidateInput(lblSiteId.Text, "^[0-9A-Za-z\-]+$") OrElse
               Not SecurityHelper.ValidateInput(hdnLid.Value, "^[0-9]+$") Then
                Response.Write("<script>alert('Invalid input parameters');</script>")
                Return
            End If

            Dim conn2 As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As SqlCommand = New SqlCommand("INSERT INTO oss_diversion_log (DivSiteId, OrderNo, remarks, FromLoadDN, ToLoadDn, userid, timestamp) VALUES (@DivSiteId, @OrderNo, @remarks, @FromLoadDN, @ToLoadDn, @userid, @timestamp)", conn2)
            cmd.Parameters.AddWithValue("@FromLoadDN", txtDnNo1.Text)
            cmd.Parameters.AddWithValue("@ToLoadDn", lblDnNo.Text)
            cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text)
            cmd.Parameters.AddWithValue("@OrderNo", txtOrderNo.Text)
            cmd.Parameters.AddWithValue("@DivSiteId", lblSiteId.Text)
            cmd.Parameters.AddWithValue("@userid", Request.Cookies("userinfo")("userid"))
            cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
            
            Try
                conn2.Open()
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                ' SECURITY FIX: Log error but don't expose details
                SecurityHelper.LogError("btnDivert_Click insert error", ex, Server)
            Finally
                conn2.Close()
            End Try

        Catch ex As SystemException
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("btnDivert_Click system error", ex, Server)
        End Try

        Try
            Dim conn2 As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Use parameterized stored procedure
            Dim cmd As SqlCommand = New SqlCommand("sp_UpdateDiversionData", conn2)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@dsid", lblSiteId.Text)
            cmd.Parameters.AddWithValue("@remark", txtRemarks.Text)
            cmd.Parameters.AddWithValue("@dn_no", txtDnNo1.Text)
            cmd.Parameters.AddWithValue("@OrderNo", txtOrderNo.Text)
            cmd.Parameters.AddWithValue("@loadid", hdnLid.Value)
            
            Try
                conn2.Open()
                result = cmd.ExecuteNonQuery()
                If result > 0 Then
                    Response.Write("<script>sendNoti(); alert('Job has been diverted successfully.'); this.parent.parent.closePopup();</script>")
                End If
            Catch Sqex As SqlException
                ' SECURITY FIX: Log error but don't expose details
                SecurityHelper.LogError("btnDivert_Click SQL error", Sqex, Server)
            Catch ex As Exception
                ' SECURITY FIX: Log error but don't expose details
                SecurityHelper.LogError("btnDivert_Click error", ex, Server)
            Finally
                conn2.Close()
            End Try

        Catch ex As SystemException
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("btnDivert_Click system error", ex, Server)
        End Try
    End Sub

    Protected Sub btndivertjob_Click(sender As Object, e As EventArgs) Handles btndivertjob.Click
        Dim result As Int16 = 0
        Try
            ' SECURITY FIX: Validate inputs
            If String.IsNullOrEmpty(txtremarksarea.Value) Then
                Response.Write("<script>alert('Please enter remarks');</script>")
                Return
            End If
            
            If Not SecurityHelper.ValidateInput(txtDnNo1.Text, "^[0-9A-Za-z\-]+$") Then
                Response.Write("<script>alert('Invalid DN number');</script>")
                Return
            End If
            
            Dim conn2 As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As SqlCommand = New SqlCommand("UPDATE oss_patch_out SET div_remark=@remark, status=15 WHERE dn_no=@dn_no", conn2)
            cmd.Parameters.AddWithValue("@remark", txtremarksarea.Value)
            cmd.Parameters.AddWithValue("@dn_no", txtDnNo1.Text)
            
            Try
                conn2.Open()
                Result = cmd.ExecuteNonQuery()
                If Result > 0 Then
                    Response.Write("<script>alert('Job has been diverted successfully.'); this.parent.closePopup();</script>")
                End If
            Catch Sqex As SqlException
                ' SECURITY FIX: Log error but don't expose details
                SecurityHelper.LogError("btndivertjob_Click SQL error", Sqex, Server)
            Catch ex As Exception
                ' SECURITY FIX: Log error but don't expose details
                SecurityHelper.LogError("btndivertjob_Click error", ex, Server)
            Finally
                conn2.Close()
            End Try

        Catch ex As SystemException
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("btndivertjob_Click system error", ex, Server)
        End Try
    End Sub

    Protected Sub btncanceldivertjob_Click(sender As Object, e As EventArgs) Handles btncanceldivertjob.Click
        Dim result As Int16 = 0
        Try
            ' SECURITY FIX: Validate input
            If Not SecurityHelper.ValidateInput(txtDnNo1.Text, "^[0-9A-Za-z\-]+$") Then
                Response.Write("<script>alert('Invalid DN number');</script>")
                Return
            End If
            
            Dim conn2 As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Use parameterized stored procedure
            Dim cmd As SqlCommand = New SqlCommand("sp_UpdateCancelDivert", conn2)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@dn_no", txtDnNo1.Text)
            
            Try
                conn2.Open()
                result = cmd.ExecuteNonQuery()
                If result > 0 Then
                    Response.Write("<script>alert('Job diversion has been cancelled successfully.'); this.parent.closePopup();</script>")
                End If
            Catch Sqex As SqlException
                ' SECURITY FIX: Log error but don't expose details
                SecurityHelper.LogError("btncanceldivertjob_Click SQL error", Sqex, Server)
            Catch ex As Exception
                ' SECURITY FIX: Log error but don't expose details
                SecurityHelper.LogError("btncanceldivertjob_Click error", ex, Server)
            Finally
                conn2.Close()
            End Try

        Catch ex As SystemException
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("btncanceldivertjob_Click system error", ex, Server)
        End Try
    End Sub
End Class