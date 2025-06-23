Imports System.Data.SqlClient

Partial Class MapSettingsManagement
    Inherits System.Web.UI.Page
    Public errormessage As String
    Public backpage As String = "Management.aspx"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            If role = "Admin" Then
                backpage = "AdminManagement.html"
            End If

            If Page.IsPostBack = False Then

                ImageButton2.Attributes.Add("onclick", "return mysubmit()")


                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select userid,username from userTBL order by username", conn)

                If role = "User" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    userslist = userslist & "," & userid
                    cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ") order by username", conn)
                End If

                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                If role <> "User" Then
                    ddluser.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                End If

                While dr.Read()
                    ddluser.Items.Add(New ListItem(dr("username"), dr("userid")))
                End While

                conn.Close()

                FillValues()

            End If

        Catch ex As Exception
            'errormessage = ex.Message
        End Try
    End Sub

    Protected Sub ddluser_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddluser.SelectedIndexChanged
        Try
            FillValues()
        Catch ex As Exception
            errormessage = ex.Message
        End Try
    End Sub

    Protected Sub FillValues()
        Try
            Dim userid As String = ddluser.SelectedValue
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim dr As SqlDataReader

            cmd = New SqlCommand("select * from map_settings where userid='" & userid & "'", conn)
            conn.Open()
            dr = cmd.ExecuteReader()

            If dr.Read() Then
                ddlzoomlevel.SelectedValue = dr("zoomlevel")
                latitude.Text = dr("lat")
                longitude.Text = dr("lon")
            End If

            conn.Close()

        Catch ex As Exception
            errormessage = ex.Message
        End Try
    End Sub

    Protected Sub ImageButton2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
        Try
            Dim userid As String = ddluser.SelectedValue
            Dim zoomlevel As String = ddlzoomlevel.SelectedValue
            Dim lat As Double = latitude.Text
            Dim lon As Double = longitude.Text
            Dim result As Byte = 0
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim dr As SqlDataReader


            cmd = New SqlCommand("select * from map_settings where userid='" & userid & "'", conn)
            conn.Open()
            dr = cmd.ExecuteReader()

            If dr.Read() Then
                cmd = New SqlCommand("update map_settings set userid='" & userid & "',zoomlevel='" & zoomlevel & "',lat='" & lat & "',lon='" & lon & "' where userid='" & userid & "'", conn)
            Else
                cmd = New SqlCommand("insert into map_settings values('" & userid & "','" & zoomlevel & "','" & lat & "','" & lon & "')", conn)
            End If

            result = cmd.ExecuteNonQuery()
            conn.Close()

            If result > 0 Then
                errormessage = "Your map settings are successfully updated."
            Else
                errormessage = "Your map settings are not updated."
            End If

        Catch ex As Exception
            errormessage = ex.Message
        End Try
    End Sub
End Class
