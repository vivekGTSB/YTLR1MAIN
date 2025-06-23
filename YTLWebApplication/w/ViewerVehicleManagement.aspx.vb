Imports System.Data.SqlClient
Imports System.Data

Namespace AVLS

    Partial Class ViewerVehicleManagement
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
               
                Dim objConn As New MyConn
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")

                Dim suserid As String = Request.QueryString("userid")

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' Dim conn As New SqlConnection(objConn.getConnectionString(userid, False))
                Dim cmd As SqlCommand = New SqlCommand("select * from view_user where userid='" & userid & "' order by viewername", conn)
                Dim dr As SqlDataReader

                conn.Open()
                dr = cmd.ExecuteReader()

                While dr.Read()
                    ddlViewers.Items.Add(New ListItem(dr("viewername"), dr("viewerid")))
                End While
                conn.Close()

                If Request.QueryString("viewerid") <> Nothing Then
                    ddlViewers.SelectedValue = Request.QueryString("viewerid")
                End If


            Catch ex As Exception
                Response.Write(ex.Message)
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
                Response.Write(ex.Message)
            End Try

        End Sub

        Private Sub FillGrid()
            Try
                Dim viewerid As String = ddlViewers.SelectedValue

                Dim viewerVehicleTable As New DataTable
                viewerVehicleTable.Columns.Add(New DataColumn("chk"))
                viewerVehicleTable.Columns.Add(New DataColumn("sno"))
                viewerVehicleTable.Columns.Add(New DataColumn("viewername"))
                viewerVehicleTable.Columns.Add(New DataColumn("vehicle"))
                viewerVehicleTable.Columns.Add(New DataColumn("startdatetime"))
                viewerVehicleTable.Columns.Add(New DataColumn("enddatetime"))
                viewerVehicleTable.Columns.Add(New DataColumn("consignnote"))
                viewerVehicleTable.Columns.Add(New DataColumn("remarks"))
                Dim r As DataRow

                ' Dim objConn As New MyConn
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                'Dim conn As New SqlConnection(objConn.getConnectionString(Request.Cookies("userinfo")("userid"), False))
                Dim cmd As SqlCommand

                If viewerid <> "--select viewer --" Then
                    If viewerid = "-- All Viewers --" Then
                        Dim userid As String = Request.Cookies("userinfo")("userid")
                        cmd = New SqlCommand("SELECT * FROM view_vehicle v, view_user u WHERE u.viewerid=v.viewerid AND u.userid='" & userid & "' ORDER BY v.viewerid,v.plateno", conn)
                    Else
                        cmd = New SqlCommand("SELECT * FROM view_vehicle v, view_user u WHERE u.viewerid=v.viewerid AND v.viewerid='" & viewerid & "' ORDER BY v.viewerid,v.plateno", conn)
                    End If

                    Dim dr As SqlDataReader

                    conn.Open()
                    dr = cmd.ExecuteReader()
                    Dim i As Int32 = 1
                    While dr.Read()
                        r = viewerVehicleTable.NewRow
                        r(0) = "<input type=""checkbox"" name=""chk"" value=""" & dr("plateno") & "&" & dr("viewerid") & """/>"
                        r(1) = i.ToString()
                        r(2) = dr("viewername")
                        r(3) = "<a href= UpdateViewerVehicle.aspx?plateno=" & dr("plateno") & "&vid=" & dr("viewerid") & "> " & dr("plateno") & " </a>"
                        r(4) = Convert.ToDateTime(dr("starttime")).ToString("yyyy/MM/dd HH:mm:ss")
                        r(5) = Convert.ToDateTime(dr("endtime")).ToString("yyyy/MM/dd HH:mm:ss")
                        r(6) = dr("note")
                        r(7) = dr("remarks")
                        viewerVehicleTable.Rows.Add(r)
                        i = i + 1
                    End While
                    conn.Close()
                ElseIf viewerid = "-- select viewer --" Then
                    r = viewerVehicleTable.NewRow
                    r(0) = "<input type=""checkbox"" name=""chk"" />"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    r(6) = "--"
                    r(7) = "--"
                    viewerVehicleTable.Rows.Add(r)
                End If


                If viewerVehicleTable.Rows.Count = 0 Then
                    r = viewerVehicleTable.NewRow
                    r(0) = "<input type=""checkbox"" name=""chk"" />"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    r(6) = "--"
                    r(7) = "--"
                    viewerVehicleTable.Rows.Add(r)
                End If

                usersgrid.DataSource = viewerVehicleTable
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
                ' Dim objConn As New MyConn
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' Dim conn As New SqlConnection(objConn.getConnectionString(Request.Cookies("userinfo")("userid"), False))
                Dim cmd As SqlCommand
                Dim platenos() As String = Split(Request.Form("chk"), ",")

                For i As Int16 = 0 To platenos.Length - 1
                    Dim primaryString() As String = platenos(i).Split("&")
                    conn.Open()
                    cmd = New SqlCommand("delete from view_vehicle where plateno='" & primaryString(0) & "' and viewerid='" & primaryString(1) & "'", conn)
                    cmd.ExecuteNonQuery()
                    conn.Close()
                Next
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
