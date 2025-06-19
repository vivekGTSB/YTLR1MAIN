Imports System.Data.SqlClient
Imports System.IO
Imports System.Data
Imports Microsoft.VisualBasic.ApplicationServices

Partial Class CustomerCertificateNew
    Inherits System.Web.UI.Page

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            Dim cmd As SqlCommand
            Dim dr As SqlDataReader

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            ddlPlate.Items.Add(New ListItem("--Select Plate No--", "--Select Plate No--"))
            ddlUsers.Items.Add(New ListItem("--Select Transporter--", "--Select Transporter--"))
            cmd = New SqlCommand("select id,name from customer order by name", conn)
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlUsers.Items.Add(New ListItem(dr("name").ToString().ToUpper(), dr("id")))
            End While
            dr.Close()

            'If role = "User" Then
            '    ddlUsers.Items.Remove(New ListItem("--Select User Name--", "--Select User Name--"))
            '    ddlUsers.SelectedValue = userid
            '    getPlateNo(userid)
            'End If

            conn.Close()
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
        MyBase.OnInit(e)
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub getPlateNo(ByVal uid As String)
        Try
            If ddlUsers.SelectedValue <> "--Select Transporter--" Then
                ddlPlate.Items.Clear()
                ddlPlate.Items.Add(New ListItem("--Select Plate No--", "--Select Plate No--"))
                Dim cmd As SqlCommand
                Dim dr As SqlDataReader

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                cmd = New SqlCommand("select plateno from vehicleTBL where companyid=@uuid order by plateno", conn)
                cmd.Parameters.AddWithValue("@uuid", uid)
                conn.Open()
                dr = cmd.ExecuteReader()

                ddlPlate.Items.Add(New ListItem("ALL", "ALL"))

                While dr.Read()
                    ddlPlate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                End While
                dr.Close()
                conn.Close()
            Else
                ddlPlate.Items.Clear()
                ddlPlate.Items.Add(New ListItem("--Select Plate No--", "--Select Plate No--"))
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub


    Protected Sub ddlUsers_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlUsers.SelectedIndexChanged
        getPlateNo(ddlUsers.SelectedValue)
    End Sub
End Class
