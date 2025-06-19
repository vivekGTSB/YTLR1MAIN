Imports System.Data.SqlClient
Partial Class AddVehToTankProfile
    Inherits System.Web.UI.Page
    Public errormessage As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If Request.Cookies("userinfo") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If

        If Page.IsPostBack = False Then

            btn_Add.Attributes.Add("onclick", "return mysubmit()")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim dr As SqlDataReader

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            cmd = New SqlCommand("select plateno from vehicleTBL order by plateno", conn)

            If role = "User" Then
                cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & userid & "' order by plateno", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select plateno from vehicleTBL where userid in(" & userslist & ") order by plateno", conn)
            End If

            conn.Open()

            dr = cmd.ExecuteReader()
            ddlplatenumber.Items.Add(New ListItem("--Select Plate Number--", "--Select Plate Number--"))
            While dr.Read()
                ddlplatenumber.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
            End While
        End If
    End Sub

    Protected Sub btn_Add_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btn_Add.Click
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim result As Byte = 0


            If Not ddlplatenumber.SelectedItem.Text = "--Select Plate Number--" Then
                cmd = New SqlCommand("insert into fuel_tank_profile(plateno,tanklevel,tankvolume) values('" & ddlplatenumber.SelectedItem.Text & "','" & TankLevel.Text & "','" & TankVolume.Text & "')", conn)
                conn.Open()
                cmd.ExecuteNonQuery()
                Label1.Text = "One Row Inserted !"
                conn.Close()
            End If



                'If result > 0 Then
                '    Response.Redirect("AdminManagement.html")
                'Else
                '    errormessage = "Record Not Inserted"
                'End If

        Catch ex As Exception
            'errormessage = ex.Message
        End Try

        TankLevel.Text = ""
        TankVolume.Text = ""
    End Sub
End Class
