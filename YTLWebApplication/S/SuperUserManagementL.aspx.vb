Imports System.Data.SqlClient
Imports System.Data

Partial Class SuperUserManagementL
    Inherits System.Web.UI.Page

    Public message As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim userid As String = Request.Cookies("userinfo")("userid")
            ImageButton1.Attributes.Add("onclick", "return mysubmit()")

            If Page.IsPostBack = False Then

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim da As SqlDataAdapter
                Dim role = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")


                da = New SqlDataAdapter("select userid,username from userTBL where role='User' and companyname not like 'Gussmann%' order by username", conn)


                Dim ds As New DataSet()
                da.Fill(ds)

                If ds.Tables(0).Rows.Count > 0 Then
                    Dim i As Integer
                    For i = 0 To ds.Tables(0).Rows.Count - 1
                        ddluser.Items.Add(New ListItem(ds.Tables(0).Rows(i).Item("username").ToString().ToUpper(), ds.Tables(0).Rows(i).Item("userid")))
                    Next
                End If
            Else

            End If

        Catch ex As Exception
            message = ex.Message
        End Try
    End Sub

    Protected Sub ddluser_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddluser.SelectedIndexChanged
        btnselectall.Visible = True

        Fill()
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim dr As SqlDataReader
            conn.Open()
            For Each li As ListItem In lstsuperusers.Items
                If li.Selected Then
                    cmd = New SqlCommand("select userslist from userTBL where userid='" & li.Value & "'", conn)
                    dr = cmd.ExecuteReader()
                    Dim usersstring As String = ""
                    Dim uuserid As String = ddluser.SelectedValue
                    Dim flag As Boolean = False
                    If dr.Read() Then
                        flag = False
                        usersstring = dr("userslist")
                        Dim userslist() As String = usersstring.Split(",")
                        For j As Byte = 0 To userslist.Length - 1
                            If uuserid = userslist(j) Then
                                flag = True
                                Exit For
                            End If
                        Next
                        If Not flag Then
                            dr.Close()
                            usersstring = usersstring & "," & uuserid
                            cmd.CommandText = "update userTBL set userslist='" & usersstring & "'  where userid='" & li.Value & "'"
                            cmd.ExecuteNonQuery()
                        End If
                    End If
                End If

            Next
            conn.Close()
            message = "Successfully updated"
        Catch ex As Exception
            message = ex.Message
        End Try
    End Sub

    Protected Sub Fill()
        Try
            If ddluser.SelectedValue <> "--Select User--" Then
                lstsuperusers.Items.Clear()
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select username,userslist,userid from userTBL where role='SuperUser' and customrole in ('LFUser2','LFSuperUser') ", conn)
                Dim dr As SqlDataReader

                conn.Open()

                dr = cmd.ExecuteReader()
                Dim usersstring As String = ""
                Dim uuserid As String = ddluser.SelectedValue
                Dim flag As Boolean = False
                While dr.Read()
                    flag = False
                    Dim userslist() As String = dr("userslist").ToString().Split(",")
                    For j As Byte = 0 To userslist.Length - 1
                        If uuserid = userslist(j) Then
                            flag = True
                        End If
                    Next
                    If flag Then
                        lstsuperusers.Items.Add(New ListItem(dr("username").ToString().ToUpper(), dr("userid"), True))
                    Else
                        lstsuperusers.Items.Add(New ListItem(dr("username").ToString().ToUpper(), dr("userid"), False))
                    End If
                End While

                For Each li As ListItem In lstsuperusers.Items
                    li.Selected = False
                Next

                For Each li As ListItem In lstsuperusers.Items
                    If li.Enabled Then
                        li.Selected = True
                    Else
                        li.Selected = False
                        li.Enabled = True
                    End If
                Next



            End If
        Catch ex As Exception
            message = ex.Message
        End Try
    End Sub
    Protected Sub btnselectall_Click(sender As Object, e As EventArgs) Handles btnselectall.Click
        If btnselectall.Text = "Select All" Then
            For Each li As ListItem In lstsuperusers.Items
                li.Selected = True
            Next
            btnselectall.Text = "Deselect All"
        Else
            For Each li As ListItem In lstsuperusers.Items
                li.Selected = False
            Next
            btnselectall.Text = "Select All"
        End If
    End Sub

End Class
