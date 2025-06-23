Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI
Imports System.Math
Imports System.Globalization

Partial Class SpeedSummaryMonthly
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public show As Boolean = False
#Region "Function"

    Protected Function ConvertHours(ByVal p_hour As String) As String
        Dim sFLD() As String
        Dim sfld2() As String
        Dim hours As String
        Dim iPos As Integer
        hours = p_hour
        iPos = p_hour.IndexOf(".")
        If iPos > 0 Then
            sFLD = p_hour.Split(".")
            sfld2 = sFLD(1).Split(":")
            hours = CStr(CInt(sFLD(0)) * 24 + CInt(sfld2(0))) & ":" & sfld2(1) & ":" & sfld2(2)
        End If
        Return hours
    End Function
    Protected Sub FillViolations()
        Dim strBeginDateTime, strEndDateTime, strSql, CurrUserName, PrevUsername As String
        Dim currentCount As Integer
        Dim ds As New DataSet
        Dim ts80, ts85, ts90, ts100, tAll As Integer
        Dim adpViolatinon As SqlDataAdapter
        Dim row As DataRow
        Try
            Dim bdtt As String = Now.AddDays(-180).ToString("yyyy/MM/dd")
            Dim str() As String = bdtt.Split("/")

            Dim str1() As String = DropDownList1.SelectedValue.Split("/")
            strBeginDateTime = str(0) & "-" & str(1) & "-" & "01" & " " & "00:00:00"
            Dim lastDay As Integer
            lastDay = DateAdd(DateInterval.Day, -1, DateSerial(str1(0), str1(1) + 1, 1)).Day
            strEndDateTime = str1(0) & "-" & str1(1) & "-" & lastDay & " " & "23:59:59"
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
      

        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        
        Try

            Dim DbTable As New DataTable
            With DbTable.Columns
                .Add(New DataColumn("No"))
                .Add(New DataColumn("Month"))
                .Add(New DataColumn("Speed > 80KM"))
                .Add(New DataColumn("Speed > 85KM"))
                .Add(New DataColumn("Speed > 90KM"))
                .Add(New DataColumn("Speed > 100KM"))
                .Add(New DataColumn("Total"))
            End With

            strSql = "select userid,plateno,SUM(s80) as s80 ,SUM(s85) as s85, SUM(s90) as s90, SUM(s100) as s100,(SUM(s80)+SUM(s85)+SUM(s90)+SUM(s100)) as Total, substring(DateName( month , DateAdd( month , Month(timestamp) , 0 ) - 1 ),1,3) as monthName, YEAR(timestamp) as yearno,MONTH(timestamp) as monthno FROM speed_summary where userid = '" & ddlUsername.SelectedValue & "' and timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "'   GROUP BY plateno,substring(DateName( month , DateAdd( month , Month(timestamp) , 0 ) - 1 ),1,3), YEAR(timestamp), userid,MONTH(timestamp) order by plateno,monthno ,yearno"
            '   strSql = "select userid,plateno,SUM(s80) as s80 ,SUM(s85) as s85, SUM(s90) as s90, SUM(s100) as s100,(SUM(s80)+SUM(s85)+SUM(s90)+SUM(s100)) as Total, substring(DateName( month , DateAdd( month , Month(timestamp) , 0 ) - 1 ),1,3) as monthName, YEAR(timestamp) as yearno FROM speed_summary where userid = '" & ddlUsername.SelectedValue & "' and timestamp between '" & strBeginDateTime & "' and '" & strEndDateTime & "'   GROUP BY plateno,substring(DateName( month , DateAdd( month , Month(timestamp) , 0 ) - 1 ),1,3), YEAR(timestamp),userid"


            adpViolatinon = New SqlDataAdapter(strSql, conn)
            adpViolatinon.Fill(ds)
            If ds.Tables(0).Rows.Count <> 0 Then
                ts80 = 0
                ts85 = 0
                ts90 = 0
                ts100 = 0
                tAll = 0
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    If i = 0 Then
                        CurrUserName = ds.Tables(0).Rows(i)("plateno")
                        row = DbTable.NewRow
                        row(1) = "Plateno : <font color=""Blue""><b>" & CurrUserName & "</b></font>"
                        DbTable.Rows.Add(row)
                    Else
                        CurrUserName = ds.Tables(0).Rows(i)("plateno")
                        If CurrUserName <> PrevUsername Then
                            row = DbTable.NewRow
                            DbTable.Rows.Add(row)
                        End If

                        If CurrUserName <> PrevUsername Then
                            row = DbTable.NewRow
                            row(1) = "Plateno : <font color=""Blue""><b>" & CurrUserName & "</b></font>"
                            DbTable.Rows.Add(row)
                        End If

                    End If

                    If PrevUsername <> CurrUserName Then
                        currentCount = 1
                    Else
                        currentCount = currentCount + 1
                    End If

                    ts80 += ds.Tables(0).Rows(i)("s80")
                    ts85 += ds.Tables(0).Rows(i)("s85")
                    ts90 += ds.Tables(0).Rows(i)("s90")
                    ts100 += ds.Tables(0).Rows(i)("s100")
                    tAll += ds.Tables(0).Rows(i)("Total")

                    row = DbTable.NewRow
                    row(0) = currentCount
                    row(1) = ds.Tables(0).Rows(i)("monthName")
                    row(2) = ds.Tables(0).Rows(i)("s80")
                    row(3) = ds.Tables(0).Rows(i)("s85")
                    row(4) = ds.Tables(0).Rows(i)("s90")
                    row(5) = ds.Tables(0).Rows(i)("s100")
                    row(6) = ds.Tables(0).Rows(i)("Total")
                    DbTable.Rows.Add(row)
                    PrevUsername = CurrUserName

                Next
                row = DbTable.NewRow
                row(1) = "<font color=""Red""><b> Total :</b></font>"
                row(2) = "<font color=""Red""><b>" & ts80 & "</b></font>"
                row(3) = "<font color=""Red""><b>" & ts85 & "</b></font>"
                row(4) = "<font color=""Red""><b>" & ts90 & "</b></font>"
                row(5) = "<font color=""Red""><b>" & ts100 & "</b></font>"
                row(6) = "<font color=""Red""><b>" & tAll & "</b></font>"
                DbTable.Rows.Add(row)

            Else
                row = DbTable.NewRow
                row(0) = "-"
                row(1) = "-"
                row(2) = "-"
                row(3) = "-"
                row(4) = "-"
                row(5) = "-"
                row(6) = "-"
                DbTable.Rows.Add(row)
            End If
            conn.Close()
            With GridView1
                .PageSize = noofrecords.SelectedValue
                .DataSource = DbTable
                .DataBind()
                If .PageCount > 1 Then
                    show = True
                End If
            End With
            For i As Integer = 0 To GridView1.Rows.Count - 1
                If Left(GridView1.Rows.Item(i).Cells(1).Text, 8).Trim() = "Plateno" Then
                    With GridView1.Rows.Item(i)
                        .Cells(1).BackColor = System.Drawing.Color.Yellow
                        .Cells(1).ForeColor = System.Drawing.Color.Red
                        .Cells(1).Font.Bold = True
                        .Cells(2).BackColor = System.Drawing.Color.Yellow
                        .Cells(3).BackColor = System.Drawing.Color.Yellow
                        .Cells(4).BackColor = System.Drawing.Color.Yellow
                        .Cells(5).BackColor = System.Drawing.Color.Yellow
                        .Cells(6).BackColor = System.Drawing.Color.Yellow
                    End With
                End If
            Next
            Session("gridview1") = DbTable
            ec = "true"
            Session("exceltable") = DbTable
            Session("OVMonthlyAllAng") = Label1.Text & Label2.Text & Label4.Text & Label3.Text
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub
#End Region
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            Dim Userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userlist As String = Request.Cookies("userinfo")("userslist")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim dr As SqlDataReader
            Dim strSql As String = "select userid, username,dbip from userTBL where role='User' order by username"
            Dim sqlCmd As New SqlCommand(strSql, conn)
            If role = "User" Then
                strSql = "select userid, username, dbip from userTBL where userid='" & Userid & "'"
                sqlCmd = New SqlCommand(strSql, conn)
            ElseIf role = "Operator" Or role = "SuperUser" Then
                strSql = "select userid, username, dbip from userTBL where userid in (" & userlist & ") order by username"
                sqlCmd = New SqlCommand(strSql, conn)
            End If


            conn.Open()
            dr = sqlCmd.ExecuteReader
            While dr.Read
                ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While
            conn.Close()


        Catch ex As Exception
            Response.Write("Oninit : " & ex.Message)
        End Try
        MyBase.OnInit(e)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            Dim nowdate As DateTime = Now.Date
            Dim oneYearBackDate As DateTime = Now.Date.AddMonths(-12)
            While nowdate >= oneYearBackDate
                DropDownList1.Items.Add(New ListItem((CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(oneYearBackDate.Month).ToString().ToUpper() & " - " & oneYearBackDate.Year).ToString(), (oneYearBackDate.Year.ToString() & "/" & oneYearBackDate.Month.ToString())))
                oneYearBackDate = oneYearBackDate.AddMonths(1)
            End While
            DropDownList1.SelectedIndex = 12
        End If
    End Sub
    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        FillViolations()
    End Sub
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        Try
            With GridView1
                .PageSize = noofrecords.SelectedValue
                .DataSource = Session("gridview1")
                .PageIndex = e.NewPageIndex
                .DataBind()
            End With
            show = True
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub
End Class
