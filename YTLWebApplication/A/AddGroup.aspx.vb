Imports System.Data.SqlClient
Imports System.Data

Partial Class AddNewGroup
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Request.Cookies("userinfo") Is Nothing Then
            Server.Transfer("Login.aspx")
        End If

        If Page.IsPostBack = False Then
            ImageButton1.Attributes.Add("onclick", "return mysubmit();")

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim da As SqlDataAdapter = New SqlDataAdapter("select userid,username from userTBL where role='User' order by username", conn)

            If role = "User" Then
                da = New SqlDataAdapter("select userid,username from userTBL where userid='" & userid & "'", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                da = New SqlDataAdapter("select userid,username from userTBL where userid in (" & userslist & ")", conn)
            End If

            Dim ds As New DataSet
            da.Fill(ds)
            ddlusers.Items.Add(New ListItem("-- Select User Name --", "-- Select User Name --"))
            For i As Int16 = 0 To ds.Tables(0).Rows.Count - 1
                ddlusers.Items.Add(New ListItem(ds.Tables(0).Rows(i)("username"), ds.Tables(0).Rows(i)("userid")))
            Next
        End If
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click

        Dim userid As String = ddlusers.SelectedValue
        Dim groupname As String = groupnametextbox.Text
        Dim description As String = descriptiontextbox.Text

      Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

        conn.Open()
        Dim cmd As SqlCommand = New SqlCommand("insert into vehicle_group(userid,groupname,description) values('" & userid & "','" & groupname & "','" & description & "')", conn)
        Dim result As Int16 = cmd.ExecuteNonQuery()
        conn.Close()
        If result = 1 Then
            Server.Transfer("GroupManagement.aspx?userid=" & userid)
        End If

    End Sub
End Class
