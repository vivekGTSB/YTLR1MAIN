Imports System.Data.SqlClient
Imports System.Data
Imports ADODB
Imports AspMap
Imports System.IO
Imports System.Windows
Imports DocumentFormat.OpenXml.Drawing.Charts

Public Class OssDayReport
    Inherits System.Web.UI.Page
    Public reportDateTime As String = ""
    Public noDataText As String = ""
    Public ec As String = "false"
    Public show As Boolean = False
    Public isD As String = "false"
    Public sb1 As New StringBuilder()
    Public sb2 As New StringBuilder()
    Public sb3 As New StringBuilder()
    Public strBeginDate As String = ""
    Public strEndDate As String = ""
    Dim bulkt As New System.Data.DataTable
    Dim bagt As New System.Data.DataTable

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Check authentication
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim con As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("select userid, username from usertbl where companyname like @companyname and role=@role and username not like @username order by username", con)
            cmd.Parameters.AddWithValue("@companyname", "YTL%")
            cmd.Parameters.AddWithValue("@role", "User")
            cmd.Parameters.AddWithValue("@username", "Returned%")
            
            con.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlPlants.Items.Clear()
            ddlPlants.Items.Add(New ListItem("ALL", "ALL"))
            While dr.Read()
                ' SECURITY FIX: Use HtmlEncode for output
                ddlPlants.Items.Add(New ListItem(HttpUtility.HtmlEncode(dr("username")), HttpUtility.HtmlEncode(dr("userid"))))
            End While
            con.Close()

        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in OssDayReport OnInit: " & ex.Message)
            WriteLog("4" & ex.Message)
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Check authentication
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                ddlyear.Items.Clear()
                Dim startyear As Integer = 2023
                While DateTime.Now.Year >= startyear
                    ddlyear.Items.Add(New ListItem(startyear, startyear))
                    startyear += 1
                End While
                ddlyear.SelectedValue = DateTime.Now.Year
            End If

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in OssDayReport Page_Load: " & ex.Message)
            WriteLog("3" & ex.Message)
        End Try
    End Sub

    Protected Sub DisplayMonthInformation()
        Try
            reportonlbl.InnerText = Date.Now().ToString("yyyy/MM/dd HH:mm:dd")
            noDataText = ""
            Dim MainShipToCode As String = ""
            Dim username As String = ddlPlants.SelectedValue
            Dim t As New System.Data.DataTable
            t.Columns.Add(New DataColumn("Plateno"))
            
            For i As Integer = 1 To DateTime.DaysInMonth(ddlyear.SelectedValue, ddlmonth.SelectedValue)
                t.Columns.Add(New DataColumn("" & i & " - Trips"))
                t.Columns.Add(New DataColumn("" & i & " - KM"))
                t.Columns.Add(New DataColumn("" & i & " - Inactive"))
            Next
            
            t.Columns.Add(New DataColumn("Total - Trips"))
            t.Columns.Add(New DataColumn("Total - KM"))
            t.Columns.Add(New DataColumn("Total - Inactive"))

            Dim platenodict As New Dictionary(Of String, List(Of Dictionary(Of Integer, reportdata)))
            Dim monthdict As Dictionary(Of Integer, reportdata)
            Dim monthlistdict As List(Of Dictionary(Of Integer, reportdata))
            Dim rdata As reportdata
            Dim r As DataRow
            Dim totalr As DataRow
            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand()
            
            ' SECURITY FIX: Use parameterized query
            If username = "ALL" Then
                cmd.CommandText = "select t1.plateno, DAY(dateadd(day,-1,timestamp)) as day, sum(km) as kmtravel, sum(trips) as tripscount, sum(inactivedays) as days from oss_result t1 inner join vehicleTBL t2 on t1.plateno=t2.plateno where t1.timestamp between @startdate and @enddate group by t1.plateno, DAY(dateadd(day,-1,timestamp))"
            Else
                cmd.CommandText = "select t1.plateno, DAY(dateadd(day,-1,timestamp)) as day, sum(km) as kmtravel, sum(trips) as tripscount, sum(inactivedays) as days from oss_result t1 inner join vehicleTBL t2 on t1.plateno=t2.plateno where t2.userid=@userid and t1.timestamp between @startdate and @enddate group by t1.plateno, DAY(dateadd(day,-1,timestamp))"
                cmd.Parameters.AddWithValue("@userid", username)
            End If
            
            cmd.Parameters.AddWithValue("@startdate", ddlyear.SelectedValue & "/" & ddlmonth.SelectedValue & "/01")
            cmd.Parameters.AddWithValue("@enddate", ddlyear.SelectedValue & "/" & ddlmonth.SelectedValue & "/" & DateTime.DaysInMonth(ddlyear.SelectedValue, ddlmonth.SelectedValue))
            
            cmd.Connection = conn2
            conn2.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            
            While dr.Read()
                If platenodict.ContainsKey(dr("plateno")) Then
                    monthlistdict = platenodict(dr("plateno"))
                    monthdict = New Dictionary(Of Integer, reportdata)
                    rdata = New reportdata
                    rdata.inactivedays = dr("days")
                    rdata.trips = dr("tripscount")
                    rdata.kmtravel = dr("kmtravel")
                    monthdict.Add(dr("day"), rdata)
                    monthlistdict.Add(monthdict)
                    platenodict(dr("plateno")) = monthlistdict
                Else
                    monthlistdict = New List(Of Dictionary(Of Integer, reportdata))
                    monthdict = New Dictionary(Of Integer, reportdata)
                    rdata = New reportdata
                    rdata.inactivedays = dr("days")
                    rdata.trips = dr("tripscount")
                    rdata.kmtravel = dr("kmtravel")
                    monthdict.Add(dr("day"), rdata)
                    monthlistdict.Add(monthdict)
                    platenodict.Add(dr("plateno"), monthlistdict)
                End If
            End While

            conn2.Close()
            Dim totaltrips, totalkm, totalinatcive As Integer

            For Each kval As KeyValuePair(Of String, List(Of Dictionary(Of Integer, reportdata))) In platenodict
                totalkm = 0
                totaltrips = 0
                totalinatcive = 0
                r = t.NewRow
                r(0) = HttpUtility.HtmlEncode(kval.Key)
                Dim j As Integer = 1
                For j = 1 To DateTime.DaysInMonth(ddlyear.SelectedValue, ddlmonth.SelectedValue) - 1
                    r(j) = "-"
                    r(j) = "-"
                    r(j) = "-"
                Next
                r(j + 1) = "-"
                r(j + 1) = "-"
                r(j + 1) = "-"

                monthlistdict = kval.Value
                If monthlistdict.Count > 0 Then
                    For Each kkval As Dictionary(Of Integer, reportdata) In monthlistdict
                        Dim i As Integer = 1
                        For i = 1 To DateTime.DaysInMonth(ddlyear.SelectedValue, ddlmonth.SelectedValue) - 1
                            If (kkval.ContainsKey(i)) Then
                                totaltrips += kkval(i).trips
                                totalkm += kkval(i).kmtravel
                                totalinatcive += kkval(i).inactivedays
                            End If

                            If (kkval.ContainsKey(i)) Then
                                r((3 * i) - 2) = kkval(i).trips
                                r((3 * i) - 1) = kkval(i).kmtravel
                                r((3 * i)) = kkval(i).inactivedays
                            End If
                        Next
                        r((3 * (i + 1)) - 2) = totaltrips
                        r((3 * (i + 1)) - 1) = totalkm
                        r((3 * (i + 1))) = totalinatcive
                    Next
                Else
                    For j = 1 To DateTime.DaysInMonth(ddlyear.SelectedValue, ddlmonth.SelectedValue) - 1
                        r(j) = "-"
                        r(j) = "-"
                        r(j) = "-"
                    Next
                    r(j + 1) = "-"
                    r(j + 1) = "-"
                    r(j + 1) = "-"
                End If

                t.Rows.Add(r)
            Next

            Dim total As Integer = 0
            If t.Rows.Count > 0 Then
                r = t.NewRow()
                For j As Integer = 1 To t.Columns.Count - 1
                    total = 0
                    For i As Integer = 1 To t.Rows.Count - 1
                        If Not IsDBNull(t.DefaultView.Item(i)(j)) Then
                            If Not t.DefaultView.Item(i)(j).ToString() = "-" Then
                                total += Convert.ToInt32(t.DefaultView.Item(i)(j))
                            End If
                        End If
                    Next
                    r(j) = total
                Next
            End If

            sb1.Length = 0
            ec = "True"
            If (t.Rows.Count > 0) Then
                sb1.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")

                Dim counter As Integer = 1
                Dim columns As String = ""
                Dim headcolumns As String = ""
                
                For i As Integer = 0 To DateTime.DaysInMonth(ddlyear.SelectedValue, ddlmonth.SelectedValue) - 1
                    ' SECURITY FIX: Use HtmlEncode for output
                    columns += "<th class=""ui-state-default"">" & HttpUtility.HtmlEncode("Trips") & "</th><th class=""ui-state-default"">" & HttpUtility.HtmlEncode("KM") & "</th><th class=""ui-state-default"">" & HttpUtility.HtmlEncode("Inactive") & "</th>"
                    headcolumns += "<th class=""ui-state-default"" colspan=3>" & HttpUtility.HtmlEncode(i + 1) & "</th>"
                Next
                
                ' SECURITY FIX: Use HtmlEncode for output
                sb1.Append("<thead><tr><th class=""ui-state-default"">" & HttpUtility.HtmlEncode(ddlmonth.SelectedValue & " - " & ddlyear.SelectedValue) & "</th>" & headcolumns & "<th class=""ui-state-default"" colspan=3>" & HttpUtility.HtmlEncode("Total") & "</th></tr></thead>")
                sb1.Append("<thead><tr><th class=""ui-state-default"">" & HttpUtility.HtmlEncode("Plateno") & "</th>" & columns & "<th class=""ui-state-default"">" & HttpUtility.HtmlEncode("Trips") & "</th><th class=""ui-state-default"">" & HttpUtility.HtmlEncode("KM") & "</th><th class=""ui-state-default"">" & HttpUtility.HtmlEncode("Inactive") & "</th></tr></thead>")
                sb1.Append("<tbody>")
                
                For i As Integer = 0 To t.Rows.Count - 1
                    Try
                        If i Mod 2 = 0 Then
                            sb1.Append("<tr class=""even"">")
                        Else
                            sb1.Append("<tr class=""odd"">")
                        End If
                        
                        For j As Integer = 0 To t.Columns.Count - 1
                            sb1.Append("<td class=""rightalign"">")
                            ' SECURITY FIX: Use HtmlEncode for output
                            sb1.Append(HttpUtility.HtmlEncode(t.DefaultView.Item(i)(j)))
                            sb1.Append("</td>")
                        Next
                        
                        sb1.Append("</td></tr>")
                        counter += 1
                    Catch ex As Exception
                        ' Log error securely
                        SecurityHelper.LogSecurityEvent("Error in DisplayMonthInformation: " & ex.Message)
                    End Try
                Next

                sb1.Append("<tr class=""odd""><th>" & HttpUtility.HtmlEncode("Total : ") & "</th>")
                For k As Integer = 1 To t.Columns.Count - 1
                    sb1.Append("<td class=""rightalign"">")
                    ' SECURITY FIX: Use HtmlEncode for output
                    sb1.Append(HttpUtility.HtmlEncode(r(k)))
                    sb1.Append("</td>")
                Next
                sb1.Append("</tr>")

                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr><th class=""ui-state-default"">" & HttpUtility.HtmlEncode("Plateno") & "</th>" & columns & "<th class=""ui-state-default"">" & HttpUtility.HtmlEncode("Trips") & "</th><th class=""ui-state-default"">" & HttpUtility.HtmlEncode("KM") & "</th><th class=""ui-state-default"">" & HttpUtility.HtmlEncode("Inactive Days") & "</th></tr></tfoot></table>")
            Else
                noDataText = ""
            End If

            Session.Remove("exceltable")
            Session("exceltable") = t

        Catch ex As Exception
            ' Log error securely
            SecurityHelper.LogSecurityEvent("Error in DisplayMonthInformation: " & ex.Message)
            Response.Write("5" & ex.Message & " - " & ex.StackTrace)
        End Try
    End Sub
    
    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        DisplayMonthInformation()
    End Sub
    
    Protected Sub WriteLog(ByVal message As String)
        Try
            ' SECURITY FIX: Use secure logging
            SecurityHelper.LogSecurityEvent("OssDayReport: " & message)
        Catch ex As Exception
            ' Fail silently
        End Try
    End Sub
    
    Class regiondata
        Public trans As String
        Public east As Integer
        Public south As Integer
        Public north As Integer
        Public centre As Integer
    End Class
    
    Class transporterdata
        Public transname As String
        Public qty As Integer
    End Class
    
    Class reportdata
        Public trips As Integer
        Public inactivedays As Integer
        Public kmtravel As Integer
    End Class
End Class