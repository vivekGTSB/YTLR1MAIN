Imports System.Data.SqlClient
Imports ChartDirector


Partial Class VehicleMileageChart
    Inherits System.Web.UI.Page
    Public xyvalues As String
    Public ilat, ilon As Double
    Public ec As String = "false"
    Dim begindatetime As String
    Dim enddatetime As String
    Public suser As String
    Public sgroup As String

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
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


        End Try
        MyBase.OnInit(e)
          
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        ImageButton1.Attributes.Add("onclick", "return mysubmit()")
        Try
            If Page.IsPostBack = False Then


                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                Dim userid As String = Request.QueryString("u")

                If userid.IndexOf(",") > 0 Then
                    Dim sgroupname As String() = userid.Split(",")
                    suser = sgroupname(0)
                    sgroup = sgroupname(1)
                End If
                Dim plateno As String = Request.QueryString("p")

                Dim cmd As SqlCommand
                Dim dr As SqlDataReader

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                If suser <> "" Then
                    cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & suser & "' order by plateno", conn)
                Else
                    cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & userid & "' order by plateno", conn)
                End If

                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlpleate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                End While
                dr.Close()
                If suser <> "" Then
                    ddlUsername.SelectedValue = suser
                Else
                    ddlUsername.SelectedValue = userid
                End If

                ddlpleate.SelectedValue = Request.QueryString("p")

                conn.Close()

                If (plateno <> "") Then

                    Dim begindatetime As String = Request.QueryString("bdt")
                    Dim enddatetime As String = Request.QueryString("edt")
                   
                    txtBeginDate.Value = DateTime.Parse(begindatetime).ToString("yyyy/MM/dd")
                    txtEndDate.Value = DateTime.Parse(enddatetime).ToString("yyyy/MM/dd")
                    ddlUsername.SelectedValue = userid
                    ddlpleate.SelectedValue = plateno
                    displaychart(plateno, ddlUsername.SelectedValue)

                End If

            End If
        Catch ex As Exception
        End Try


    End Sub


    Sub displaychart(ByVal plateno As String, ByVal userid As String)
        Try

            begindatetime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            enddatetime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            Dim tempdate As DateTime = begindatetime
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim da As New SqlDataAdapter("select plateno,convert(varchar(10),timestamp,120) as datetime,odometer as odometer from vehicle_history2 where plateno='" & plateno & "' and timestamp between '" & begindatetime & "' and '" & enddatetime & "' and gps_av='A' and odometer>0 ", conn)
            Dim ds As New System.Data.DataSet
            da.Fill(ds, "history")
            Dim historytable As DataTable = ds.Tables("history")
            ' Response.Write(da.SelectCommand.CommandText)
            Dim datavalues() As Double = {}
            Dim labelsvalues() As String = {}
            Dim colorsvalues() As Integer = {}
            Dim dif As Double = 0
            Dim imagestatus As String = "no"
            Dim i As Integer = 0
            While (tempdate <= txtEndDate.Value)

                Dim totalmileage As Double = 0

                historytable.DefaultView.RowFilter = "datetime='" & tempdate.ToString("yyyy-MM-dd") & "'"
                If historytable.DefaultView.Count > 0 Then
                    imagestatus = "yes"
                    For j As Int16 = 0 To historytable.DefaultView.Count - 2
                        dif = ((Convert.ToDouble(historytable.DefaultView.Item(j + 1)("odometer")) - Convert.ToDouble(historytable.DefaultView.Item(j)("odometer")))).ToString("0.00")
                        If dif > 0 And dif < 500 Then
                            totalmileage += dif
                            dif = 0
                        End If
                    Next
                End If
                Try
                    ReDim Preserve datavalues(i)
                    ReDim Preserve labelsvalues(i)
                    ReDim Preserve colorsvalues(i)
                    datavalues(i) = Math.Round(totalmileage, 2).ToString("0.00")
                    labelsvalues(i) = tempdate.ToString("yyyy/MM/dd ")
                Catch ex As Exception

                Finally
                    conn.Close()
                End Try
                tempdate = tempdate.AddDays(1)
                i += 1
            End While

            If imagestatus = "no" Then
                WebChartViewer1.Visible = False
                Image1.Visible = True
                Image1.ImageUrl = "~/images/NoDataWide.jpg"

            End If
            Array.Reverse(datavalues)
            Array.Reverse(labelsvalues)
            Dim chight As Int64 = datavalues.Length * 30

            'Create a XYChart object of size 600 x 250 pixels
            Dim c As XYChart = New XYChart(600, chight + 90, &HFFFFFF, 0, 0)

            'Add a title to the chart using Arial Bold Italic font
            c.addTitle(plateno & " Mileage Chart", "Verdana", 10) '.setBackground(&H9999FF)


            'Set the plotarea at (180, 30) and of size 400 x 200 pixels. Set the plotarea
            'border, background and grid lines to Transparent
            'c.setPlotArea(120, 40, 410, chight, Chart.Transparent, Chart.Transparent, _
            '    Chart.Transparent, Chart.Transparent, Chart.Transparent)
            c.setPlotArea(120, 50, 410, chight)

            'Add a bar chart layer using the given data. Use a gradient color for the bars,
            'where the gradient is from dark green (0x008000) to white (0xffffff)

            Dim layer As BarLayer = c.addBarLayer3(datavalues) 'c.addBarLayer(datavalues, colorsvalues)

            layer.set3D(6)
            'Set bar shape to circular (cylinder)
            ' layer.setBarShape(Chart.CircleShape)

            'Swap the axis so that the bars are drawn horizontally
            c.swapXY(True)

            ''Set the bar gap to 10%
            layer.setBarGap(0.3)

            'Use the format "US$ xxx millions" as the bar label
            layer.setAggregateLabelFormat(" {value|2,.}")

            'Set the bar label font to 10 pts Times Bold Italic/dark red (0x663300)
            layer.setAggregateLabelStyle("Verdana", 8)

            'Set the labels on the x axis
            Dim textbox As ChartDirector.TextBox = c.xAxis().setLabels(labelsvalues)

            'Set the x axis label font to 10pt Arial Bold Italic
            textbox.setFontStyle("Verdana")
            textbox.setFontSize(8)

            ''Set the x axis to Transparent, with labels in dark red (0x663300)
            'c.xAxis().setColors(Chart.Transparent, 0)

            ''Set the y axis and labels to Transparent
            'c.yAxis().setColors(Chart.Transparent, Chart.Transparent)
            ''Dim yMark As Mark = c.yAxis().addMark(maxspeed, &HCC0000, "Max Speed Limit " & maxspeed & " Km/h")
            ''yMark.setLineWidth(2)


            'Add a title to the x axis
            c.xAxis().setTitle("Date")
            'Add a title to the y axis
            c.yAxis().setTitle("Mileage (Kilometers)")


            'output the chart
            WebChartViewer1.Image = c.makeWebImage(Chart.PNG)


            'Client side Javascript to show detail information "onmouseover"

            'include tool tip for the chart     

            WebChartViewer1.ImageMap = c.getHTMLImageMap("", "", _
                "title='{xLabel}: {value} KM'")



            If imagestatus = "yes" Then
                Image1.Visible = False
                WebChartViewer1.Visible = True
                Session("Chart") = c.makeChart2(0)
                ec = "true"
            End If
        Catch ex As Exception
            Response.Write(ex.Message.ToUpper())
        End Try
        ddlpleate.SelectedValue = platenotemp.Value
    End Sub
    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        begindatetime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
        enddatetime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
        Dim userid As String = ddlUsername.SelectedValue
        Dim plateno As String = platenotemp.Value

        displaychart(plateno, userid)
    End Sub
    Protected Sub getPlateNo(ByVal uid As String)
        Try
            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select Plate No--")
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
