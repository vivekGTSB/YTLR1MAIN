Imports System.Data.SqlClient
Partial Class AddShipToCode
    Inherits System.Web.UI.Page
    Public errormessage As String
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            If Page.IsPostBack = False Then
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
            End If
        Catch ex As Exception
            errormessage = ex.Message
        End Try
    End Sub

    Protected Sub ImageButton2_Click(sender As Object, e As System.EventArgs) Handles ImageButton2.Click
        Try

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmd As SqlCommand
            Dim result As Byte = 0
            Dim ShipToCode As String = txtShipToCode.Text
            Dim name As String = txtName.Text
            Dim address1 As String = txtaddress1.Text
            Dim address2 As String = txtaddress2.Text
            Dim address3 As String = txtaddress3.Text
            Dim address4 As String = txtaddress4.Text
            conn.Open()
            cmd = New SqlCommand("insert into oss_ship_to_code(shiptocode,uom,name,address1,address2,address3,address4 ) values('" & txtShipToCode.Text & "','0','" & txtName.Text & "','" & txtaddress1.Text.Trim() & "','" & txtaddress2.Text.Trim() & "','" & txtaddress3.Text.Trim() & "','" & txtaddress4.Text.Trim() & "')", conn)
            result = cmd.ExecuteNonQuery()
            conn.Close()
            txtShipToCode.Text = ""
            txtName.Text = ""
            txtaddress1.Text = ""
            txtaddress2.Text = ""
            txtaddress3.Text = ""
            txtaddress4.Text = ""
        Catch ex As Exception
            errormessage = "Please Enter Another Ship to Code "
            ' errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
        End Try
    End Sub

End Class
