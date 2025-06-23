Imports System.Data.SqlClient
Imports System.Data

Namespace AVLS

    Partial Class ViewerManagement
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

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Try
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")

                Dim suserid As String = Request.QueryString("userid")

                'Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("fuelmaster"))
                Dim cmd As SqlCommand = New SqlCommand("select * from view_user where userid='" & userid & "' order by viewername", conn)
                Dim dr As SqlDataReader

                conn.Open()
                dr = cmd.ExecuteReader()

                While dr.Read()
                    ddlViewers.Items.Add(New ListItem(dr("viewername"), dr("viewerid")))
                End While
                conn.Close()

                If Not Request.QueryString("viewerid") = "" Then
                    ddlViewers.SelectedValue = Request.QueryString("viewerid")
                End If


            Catch ex As Exception


            End Try
            MyBase.OnInit(e)
        End Sub


        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return deleteconfirmation();")
                    ImageButton2.Attributes.Add("onclick", "return deleteconfirmation();")
                    FillGrid()
                End If

            Catch ex As Exception

            End Try

        End Sub

        Private Sub FillGrid()
            Try

                Dim viewerid As String = ddlViewers.SelectedValue

                Dim userstable As New DataTable
                userstable.Columns.Add(New DataColumn("chk"))
                userstable.Columns.Add(New DataColumn("sno"))
                userstable.Columns.Add(New DataColumn("username"))
                userstable.Columns.Add(New DataColumn("password"))
                userstable.Columns.Add(New DataColumn("type"))
                Dim r As DataRow

                'Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("fuelmaster"))
                Dim cmd As SqlCommand

                If viewerid = "-- All Viewers --" Then
                    cmd = New SqlCommand("select * from view_user where userid='" & Request.Cookies("userinfo")("userid") & "' order by viewername", conn)
                Else 'If Not viewerid = "--Select User Name--" Then
                    cmd = New SqlCommand("select * from view_user where viewerid='" & viewerid & "'", conn)
                End If

                Dim dr As SqlDataReader

                conn.Open()
                dr = cmd.ExecuteReader()
                Dim i As Int32 = 1
                While dr.Read()
                    r = userstable.NewRow
                    r(0) = "<input type=""checkbox"" name=""chk"" value=""" & dr("viewerid") & """/>"
                    r(1) = i.ToString()
                    r(2) = "<a href= UpdateViewer.aspx?viewerid=" & dr("viewerid") & "> " & dr("viewername") & " </a>"
                    r(3) = dr("viewerpwd")
                    If IsDBNull(dr("viewertype")) Then
                        r(4) = "Basic"
                    Else
                        If Convert.ToInt16(dr("viewertype")) = 0 Then
                            r(4) = "Basic"
                        Else
                            r(4) = "Advance"
                        End If
                    End If

                    userstable.Rows.Add(r)
                    i = i + 1
                End While
                conn.Close()

                If userstable.Rows.Count = 0 Then
                    r = userstable.NewRow
                    r(0) = "<input type=""checkbox"" name=""chk"" />"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    userstable.Rows.Add(r)
                End If

                usersgrid.DataSource = userstable
                usersgrid.DataBind()

            Catch ex As Exception
                Response.Write(ex.Message)
            End Try
        End Sub

        Private Sub ImageButton1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            DeleteViewer()
        End Sub

        Protected Sub ImageButton2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
            DeleteViewer()
        End Sub

        Protected Sub DeleteViewer()
            Try
                'Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("fuelmaster"))
                Dim connLocal As SqlConnection
                Dim cmd As SqlCommand
                Dim cmdLocal As SqlCommand
                Dim strSQL As String
                Dim strSQL2 As String

                Dim objConn As MyConn
                Dim viewerids() As String = Split(Request.Form("chk"), ",")

                conn.Open()
                cmd = New SqlCommand()
                cmd.Connection = conn

                objConn = New MyConn
                connLocal = New SqlConnection(objConn.getConnectionString(Request.Cookies("userinfo")("userid"), False))
                If objConn.bMaster = False Then
                    connLocal.Open()
                    cmdLocal = New SqlCommand()
                    cmdLocal.Connection = connLocal
                End If

                For i As Int16 = 0 To viewerids.Length - 1

                    'cmd = New SqlCommand("delete from view_user where viewerid='" & viewerids(i) & "'", conn)
                    'cmd.ExecuteNonQuery()
                    'cmd = New SqlCommand("delete from view_vehicle where viewerid='" & viewerids(i) & "'", conn)
                    'cmd.ExecuteNonQuery()
                    strSQL = "DELETE FROM view_user WHERE viewerid='" & viewerids(i) & "'"
                    cmd.CommandText = strSQL

                    'Response.Write(strSQL)
                    cmd.ExecuteNonQuery()

                    strSQL2 = "DELETE FROM view_vehicle WHERE viewerid='" & viewerids(i) & "'"
                    cmd.CommandText = strSQL2

                    'Response.Write(strSQL2)
                    cmd.ExecuteNonQuery()

                    If objConn.bMaster = False Then
                        cmdLocal.CommandText = strSQL
                        cmdLocal.ExecuteNonQuery()

                        cmdLocal.CommandText = strSQL2
                        cmdLocal.ExecuteNonQuery()
                    End If

                Next
                conn.Close()
                If objConn.bMaster = False Then
                    connLocal.Close()
                    cmdLocal.Dispose()
                    cmdLocal.Dispose()
                End If

                FillGrid()
            Catch ex As Exception
                Response.Write(ex.Message)
            End Try
        End Sub

        Protected Sub ddlusers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlViewers.SelectedIndexChanged
            FillGrid()
        End Sub
    End Class

End Namespace
