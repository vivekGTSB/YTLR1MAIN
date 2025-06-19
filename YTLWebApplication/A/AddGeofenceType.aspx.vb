Imports System.Data.SqlClient
Imports System.Data

Partial Class AddGeofenceType
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Request.Cookies("userinfo") Is Nothing Then
            Server.Transfer("Login.aspx")
        End If

        If Page.IsPostBack = False Then
            ImageButton1.Attributes.Add("onclick", "return mysubmit();")

        End If
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click


        Dim groupname As String = txtgtype.Text
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim role As String = Request.Cookies("userinfo")("role")
        Dim userslist As String = Request.Cookies("userinfo")("userslist")
        conn.Open()
        Dim cmd As SqlCommand = New SqlCommand("insert into geofence_type(userid,GeofenceType) values('" & userid & "','" & groupname & "')", conn)
        Dim result As Int16 = cmd.ExecuteNonQuery()
        conn.Close()
        If result = 1 Then
            Server.Transfer("GeofenceTypeManagment.aspx?userid=" & userid)
        End If

    End Sub
End Class
