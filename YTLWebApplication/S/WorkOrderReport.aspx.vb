Imports System.Data.SqlClient
Imports System.Data

Partial Class WorkOrderReport
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Public plateno As String
    Public statisticstable As New DataTable
    Public path As String
    Public lng As String
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            path = "http://" & Request.Url.Host & Request.ApplicationPath

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim cmd As SqlCommand
            Dim dr As SqlDataReader
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            cmd = New SqlCommand("select userid, username,dbip from userTBL where role='User' order by username", conn)
            If role = "User" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid='" & userid & "'", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
            End If
            conn.Open()
            ddlpleate.Items.Add(New ListItem("--Select Plate No--", "--Select Plate No--"))
            ddlUsername.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While
            dr.Close()
            If role = "User" Then
                ddlUsername.Items.Remove(New ListItem("--Select User Name--", "--Select User Name--"))
                ddlUsername.SelectedValue = userid
                getPlateNo(userid)
            Else
                ddlUsername.SelectedIndex = 0

            End If

            conn.Close()
            conn.Dispose()
        Catch ex As Exception

        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now.AddDays(-1).ToString("yyyy/MM/dd")
                txtEndDate.Value = Now.AddDays(-1).ToString("yyyy/MM/dd")
            End If
            ImageButton1.Text = "Submit"
            ImageButton1.ToolTip = "Submit"
            ImageButton1.Attributes.Add("onclick", "return mysubmit()")

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub getPlateNo(ByVal uid As String)
        Try
            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add(New ListItem("--Select Plate No--", "--Select Plate No--"))
                ddlpleate.Items.Add(New ListItem("ALL PLATES", "ALL PLATES"))
                Dim cmd As SqlCommand
                Dim dr As SqlDataReader

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & uid & "' order by plateno", conn)
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlpleate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                End While
                dr.Close()
                conn.Close()
                conn.Dispose()
                cmd.Dispose()
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add(New ListItem("--Select Plate No--", "--Select Plate No--"))
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub
    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplayIdlingInformation()
    End Sub
    Protected Sub DisplayIdlingInformation()
        Try
            Dim plateno As String = ddlpleate.SelectedValue
            Dim begindatetime As String = Date.Parse(txtBeginDate.Value).ToString("yyyy-MM-dd") & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = Date.Parse(txtEndDate.Value).ToString("yyyy-MM-dd") & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("SNo"))
            t.Columns.Add(New DataColumn("PlateNo"))
            t.Columns.Add(New DataColumn("Timestamp"))
            t.Columns.Add(New DataColumn("IgnON"))
            t.Columns.Add(New DataColumn("IgnOff"))
            t.Columns.Add(New DataColumn("WorkHour"))

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim cmd As SqlCommand
            Dim i As Int32 = 1
            Dim lat As Double = 0
            Dim lon As Double = 0
            Dim r As DataRow
            If ddlpleate.SelectedValue = "ALL PLATES" Then
                cmd = New SqlCommand("select  workhour,plateno,ignontime,ignofftime,timestamp from work_hour where userid ='" & ddlUsername.SelectedValue & "' and timestamp between '" & begindatetime & "' and '" & enddatetime & "' order by plateno,timestamp ", conn)
            Else
                cmd = New SqlCommand("select  workhour,plateno,ignontime,ignofftime,timestamp from work_hour where plateno ='" & ddlpleate.SelectedValue & "' and timestamp between '" & begindatetime & "' and '" & enddatetime & "' order by plateno,timestamp ", conn)
            End If
            '  Response.Write(cmd.CommandText)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Dim lastlat As Double = 0
            Dim lastlon As Double = 0
            Dim iSpan As TimeSpan
            Dim dutyhours As String
            Dim userid As String = ddlUsername.SelectedValue
            While dr.Read()
                r = t.NewRow
                r(0) = i
                r(1) = dr("plateno").ToString.ToUpper()
                r(2) = Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd")
                r(3) = Convert.ToDateTime(dr("ignontime")).ToString("HH:mm:ss")
                r(4) = Convert.ToDateTime(dr("ignofftime")).ToString("HH:mm:ss")

                If Convert.ToInt32(dr("workhour")) < 0 Then
                    iSpan = TimeSpan.FromMinutes(0)
                Else
                    iSpan = TimeSpan.FromMinutes(dr("workhour"))
                End If
                dutyhours = iSpan.Hours.ToString.PadLeft(2, "0"c) & ":" & _
                iSpan.Minutes.ToString.PadLeft(2, "0"c) & ":" & _
                iSpan.Seconds.ToString.PadLeft(2, "0"c)

                r(5) = dutyhours

                t.Rows.Add(r)
                i = i + 1
            End While

            conn.Close()
            conn.Dispose()
            If t.Rows.Count = 0 Then
                r = t.NewRow
                r(0) = i
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                r(5) = "--"
                t.Rows.Add(r)
            End If

            GridView1.DataSource = t
            GridView1.DataBind()
            ec = "true"
            If GridView1.PageCount > 1 Then
                show = True
            End If
            Dim t4 As New DataTable
            t4 = t.Copy()
            Session("exceltable4") = t4
            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session.Remove("exceltable3")
            Session.Remove("excelchart")

            Session("exceltable") = t
            Try
                t.Dispose()
                dr.Close()
                cmd.Dispose()
                GridView1.Dispose()
            Catch ex As Exception
            End Try
        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Sub
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging

        GridView1.DataSource = Session("exceltable4")
        GridView1.PageIndex = e.NewPageIndex
        GridView1.DataBind()
        ec = "true"
        show = True
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                If Double.TryParse(e.Row.Cells(0).Text, 0) = False Then
                    e.Row.Style.Add("background-color", "darkseagreen")
                    e.Row.Style.Add("color", "BLACK")
                    e.Row.Style.Add("font-weight", "Bold")
                    e.Row.Style.Add("BORDER-TOP", "BLACK 3px solid")
                    e.Row.Style.Add("BORDER-BOTTOM", "BLACK 3px solid")
                End If
            End If
            If e.Row.RowType = DataControlRowType.Footer Then
                e.Row.Style.Add("BORDER-BOTTOM", "BLACK 5px double")

            End If
        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Sub



    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        getPlateNo(ddlUsername.SelectedValue)
    End Sub


End Class


