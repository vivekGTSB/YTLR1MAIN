Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data

Partial Class HistoryReport
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

            If Request.Cookies("accesslevel") Is Nothing Then
                Response.Redirect("hLogin.aspx")
            End If

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
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While
            dr.Close()
            If role = "User" Then
                ddlUsername.Items.Remove("--Select User Name--")
                ddlUsername.SelectedValue = userid
                getPlateNo(userid)
            End If
            conn.Close()

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            End If

        Catch ex As Exception
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        DisplayRecords()
    End Sub

    Protected Sub DisplayRecords()
        Try

            Dim begindatetime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("No"))
            t.Columns.Add(New DataColumn("PlateNo"))
            t.Columns.Add(New DataColumn("TimeStamp"))
            t.Columns.Add(New DataColumn("InsertDate"))
            t.Columns.Add(New DataColumn("Odometer"))
            t.Columns.Add(New DataColumn("Level1"))
            t.Columns.Add(New DataColumn("Level2"))
            t.Columns.Add(New DataColumn("Edit"))

            'Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim connection As New Redirect(ddlUsername.SelectedValue)
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))

            Dim da As SqlDataAdapter

            da = New SqlDataAdapter("select distinct convert(varchar(19),timestamp,120) as datetime,plateno,insertdate,gps_odometer,oil_tank_level1,oil_tank_level2 from vehicle_history where plateno ='" & ddlpleate.SelectedValue & "' and timestamp between '" & begindatetime & "' and '" & enddatetime & "' Order By datetime", conn)

            Dim ds As New DataSet

            'Try

            da.Fill(ds)

            Dim r As DataRow

            For i As Int32 = 0 To ds.Tables(0).Rows.Count - 1

                r = t.NewRow
                r(0) = (i + 1).ToString()
                r(1) = ds.Tables(0).Rows(i)("plateno")
                r(2) = ds.Tables(0).Rows(i)("datetime")
                r(3) = Convert.ToDateTime(ds.Tables(0).Rows(i)("insertdate")).ToString("yyyy-MM-dd HH:mm:ss")
                r(4) = ds.Tables(0).Rows(i)("gps_odometer")
                r(5) = ds.Tables(0).Rows(i)("oil_tank_level1")
                r(6) = ds.Tables(0).Rows(i)("oil_tank_level2")
                Dim dDate As String = Convert.ToString(r(2)).Substring(0, 10)
                Dim dTime As String = Convert.ToString(r(2)).Substring(11, 8)
                Dim iDate As String = Convert.ToString(r(3)).Substring(0, 10)
                Dim iTime As String = Convert.ToString(r(3)).Substring(11, 8)
                r(7) = "<a href= UpdateHistoryReport.aspx?userid=" & ddlUsername.SelectedValue & "&pno=" & r(1) & "&tsd=" & dDate & "&tst=" & dTime & "&idd=" & iDate & "&idt=" & iTime & "&odo=" & r(4) & "&o1=" & r(5) & "&o2=" & r(6) & " target=_Blank> " & "edit" & " </a>"

                t.Rows.Add(r)

            Next

            If t.Rows.Count = 0 Then
                r = t.NewRow
                r(0) = "--"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                r(5) = "--"
                r(6) = "--"
                r(7) = "--"
                t.Rows.Add(r)

            End If

            'Catch ex As Exception
            'Response.Write(ex.Message)
            'Finally
            conn.Close()
            'End Try

            Session("exceltable") = t

            GridView1.DataSource = t
            GridView1.DataBind()
            ec = "true"

            If GridView1.PageCount > 1 Then
                show = True
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        Try

            GridView1.DataSource = Session("exceltable")
            GridView1.PageIndex = e.NewPageIndex
            GridView1.DataBind()

            ec = "true"
            show = True
        Catch ex As Exception
        End Try

    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound
        Try
            'If e.Row.RowType = DataControlRowType.DataRow Then
            '    If CStr(e.Row.Cells(7).Text) = "9876" Then
            '        e.Row.Cells(7).ColumnSpan = 2
            '        e.Row.Cells(8).Visible = False
            '        e.Row.Cells(7).Text = "Unit Defected"
            '        e.Row.Cells(7).Font.Bold = True
            '        e.Row.Style.Add("background-color", "#ECE5FA") 'C4D46B
            '        e.Row.Cells(7).HorizontalAlign = HorizontalAlign.Center
            '        'e.Row.Cells(7).Width = 200
            '    ElseIf CStr(e.Row.Cells(7).Text) = "9998" Then
            '        e.Row.Cells(7).ColumnSpan = 2
            '        e.Row.Cells(8).Visible = False
            '        e.Row.Cells(7).Text = "Fuel Sensor Disconnected"
            '        e.Row.Cells(7).Font.Bold = True
            '        e.Row.Style.Add("background-color", "#F0FAE5") 'C4D46B
            '        e.Row.Cells(7).HorizontalAlign = HorizontalAlign.Center
            '        'e.Row.Cells(7).Width = 200
            '    ElseIf CStr(e.Row.Cells(7).Text) = "9999" Then
            '        e.Row.Cells(7).ColumnSpan = 2
            '        e.Row.Cells(8).Visible = False
            '        e.Row.Cells(7).Text = "Fuel Sensor Reading Error"
            '        e.Row.Cells(7).Font.Bold = True
            '        e.Row.Style.Add("background-color", "#FAF0E5") 'C4D46B
            '        e.Row.Cells(7).HorizontalAlign = HorizontalAlign.Center
            '        'e.Row.Cells(7).Width = 200
            '    End If
            '    If CStr(e.Row.Cells(9).Text) = "9876" Then
            '        e.Row.Cells(9).ColumnSpan = 2
            '        e.Row.Cells(10).Visible = False
            '        e.Row.Cells(9).Text = "Unit Defected"
            '        e.Row.Cells(9).Font.Bold = True
            '        e.Row.Style.Add("background-color", "#ECE5FA") 'C4D46B
            '        e.Row.Cells(9).HorizontalAlign = HorizontalAlign.Center
            '        'e.Row.Cells(9).Width = 200
            '    ElseIf CStr(e.Row.Cells(9).Text) = "9998" Then
            '        e.Row.Cells(9).ColumnSpan = 2
            '        e.Row.Cells(10).Visible = False
            '        e.Row.Cells(9).Text = "Fuel Sensor Disconnected"
            '        e.Row.Cells(9).Font.Bold = True
            '        e.Row.Style.Add("background-color", "#F0FAE5") 'C4D46B
            '        e.Row.Cells(9).HorizontalAlign = HorizontalAlign.Center
            '        'e.Row.Cells(9).Width = 200
            '    ElseIf CStr(e.Row.Cells(9).Text) = "9999" Then
            '        e.Row.Cells(9).ColumnSpan = 2
            '        e.Row.Cells(10).Visible = False
            '        e.Row.Cells(9).Text = "Fuel Sensor Reading Error"
            '        e.Row.Cells(9).Font.Bold = True
            '        e.Row.Style.Add("background-color", "#FAF0E5") 'C4D46B
            '        e.Row.Cells(9).HorizontalAlign = HorizontalAlign.Center
            '        'e.Row.Cells(9).Width = 200
            '    End If
            'End If
        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub getPlateNo(ByVal uid As String)
        Try
            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select Plate No--")
                Dim cmd As SqlCommand
                Dim dr As SqlDataReader
                Dim connection As New Redirect(ddlUsername.SelectedValue)
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))

                cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & uid & "' order by plateno", conn)
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlpleate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                End While
                dr.Close()

                conn.Close()
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        getPlateNo(ddlUsername.SelectedValue)
    End Sub
End Class
