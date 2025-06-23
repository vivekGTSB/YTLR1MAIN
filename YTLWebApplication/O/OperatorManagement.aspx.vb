Imports System.Data.SqlClient
Imports System.Data

Partial Class OperatorManagement
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

                da = New SqlDataAdapter("select userid,username from userTBL where role='Operator' order by username", conn)

                Dim ds As New DataSet()
                da.Fill(ds)

                If ds.Tables(0).Rows.Count > 0 Then
                    Dim i As Integer
                    For i = 0 To ds.Tables(0).Rows.Count - 1
                        ddluser.Items.Add(New ListItem(ds.Tables(0).Rows(i).Item("username"), ds.Tables(0).Rows(i).Item("userid")))
                    Next
                End If
            Else

            End If

        Catch ex As Exception
            message = ex.Message
        End Try
    End Sub

    Protected Sub ddluser_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddluser.SelectedIndexChanged
        Fill()
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim userslist As String = Request.Form("userslist")

            Dim cmd As SqlCommand = New SqlCommand("update userTBL set userslist='" & userslist & "' where userid='" & ddluser.SelectedValue & "'", conn)
            conn.Open()
            cmd.ExecuteNonQuery()
            conn.Close()

            Dim conn4 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection4"))
            cmd = New SqlCommand("update userTBL set userslist='" & userslist & "' where userid='" & ddluser.SelectedValue & "'", conn4)
            conn4.Open()
            cmd.ExecuteNonQuery()
            conn4.Close()

            Dim conn8 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection8"))
            cmd = New SqlCommand("update userTBL set userslist='" & userslist & "' where userid='" & ddluser.SelectedValue & "'", conn8)
            conn8.Open()
            cmd.ExecuteNonQuery()
            conn8.Close()

            Fill()

            message = "Successfully updated"

        Catch ex As Exception
            message = ex.Message
        End Try
    End Sub

    Protected Sub Fill()
        Try
            If ddluser.SelectedValue <> "--Select Super User--" Then

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select username,userslist from userTBL where userid='" & ddluser.SelectedValue & "'", conn)
                Dim dr As SqlDataReader

                conn.Open()

                dr = cmd.ExecuteReader()
                Dim usersstring As String = ""

                If dr.Read() Then
                    Dim userslist() As String = dr("userslist").ToString().Split(",")

                    For j As Int64 = 0 To userslist.Length - 1
                        usersstring &= "'" & userslist(j) & "',"
                    Next
                    usersstring = usersstring.Remove(usersstring.Length - 1, 1)
                    usersstring = usersstring
                End If
                dr.Close()


                cmd = New SqlCommand("select userid,username from userTBL where role='User' and userid not in(" & usersstring & ") order by username", conn)
                dr = cmd.ExecuteReader()

                While dr.Read()
                    ListBox1.Items.Add(New ListItem(dr("username"), dr("userid")))
                End While
                dr.Close()


                cmd = New SqlCommand("select userid,username from userTBL where userid in(" & usersstring & ") order by username", conn)
                dr = cmd.ExecuteReader()

                While dr.Read()
                    ListBox2.Items.Add(New ListItem(dr("username"), dr("userid")))
                End While
                dr.Close()

            End If
        Catch ex As Exception
            message = ex.Message
        End Try
    End Sub
End Class
