Imports System.Data.SqlClient

Namespace AVLS

    Partial Class AddUnit
        Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub


        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Public errormessage As String
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Request.Cookies("userinfo") Is Nothing Then
                    Response.Redirect("Login.aspx")
                End If

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return mysubmit()")

                    Dim userid As String = Request.Cookies("userinfo")("userid")
                    Dim role As String = Request.Cookies("userinfo")("role")
                    Dim userslist As String = Request.Cookies("userinfo")("userslist")


                    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim cmd As SqlCommand = New SqlCommand("select userid,username from userTBL where role='User' order by username", conn)
                    Dim dr As SqlDataReader


                    If role = "User" Then
                        cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "'", conn)
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ")", conn)
                    End If

                    conn.Open()
                    dr = cmd.ExecuteReader()
                    ddlusers.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))

                    While dr.Read()
                        ddlusers.Items.Add(New ListItem(dr("username"), dr("userid")))
                    End While
                    conn.Close()

                End If

            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub

        Private Sub ImageButton1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            Try
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("insert into unitLST values('" & versionid.Text.Trim() & "','" & unitid.Text.Trim() & "','" & ddlusers.SelectedValue & "','" & password.Text.Trim & "','" & simno.Text.Trim() & "')", conn)

                Dim result As Byte = 0

                conn.Open()
                result = cmd.ExecuteNonQuery()
                conn.Close()

                If result > 0 Then
                    Server.Transfer("UnitManagement.aspx?userid=" & ddlusers.SelectedValue & "")
                Else
                    errormessage = "Sorry,This unit not inserted."
                End If

            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub
    End Class

End Namespace
