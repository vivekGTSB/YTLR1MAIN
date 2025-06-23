Imports System.Data
Imports System.Data.SqlClient


Partial Class LafargePrivateGeofenceAlertManagement
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
        Try

            If Not Page.IsPostBack Then
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select geofenceid,geofencename from geofence where accesstype='2' and geofenceid not in (select geofenceid from lafarge_private_geofence_alert_settings) order by geofencename", conn)

                Dim dr As SqlDataReader
                Try
                    ddlGeofence.Items.Clear()
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    While dr.Read
                        ddlGeofence.Items.Add(New ListItem(dr("geofencename").ToString.ToUpper(), dr("geofenceid")))
                    End While
                    dr.Close()
                Catch ex As Exception
                    Response.Write(ex.Message)
                Finally
                    conn.Close()
                End Try
            End If

        Catch ex As Exception

        End Try

    End Sub

End Class
