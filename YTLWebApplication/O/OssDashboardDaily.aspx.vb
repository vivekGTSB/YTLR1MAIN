Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.Web.Services
Imports System.Web.Script.Services

Partial Class OssDashboardDaily
    Inherits System.Web.UI.Page
    Public htmlsb As New StringBuilder()
    Public connstr As String
    Public Shared disdate As String, uid, urole, uuserslist As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            If Not Page.IsPostBack Then
                If Request.Cookies("userinfo") Is Nothing Then
                    Response.Redirect("Login.aspx")
                End If
            End If
            Dim d As DateTime = Now
            Dim curdate As String
            curdate = d.ToString("yyyy/MM/dd")
            curdate = curdate.Replace("-", "/")
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim tankerQuery As String = "select plateno from vehicleTBL"
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Try
                Dim cmd As SqlCommand
                Dim dr As SqlDataReader
                cmd = New SqlCommand("select distinct transporter_name, transporter_id from oss_transporter order by transporter_name", conn)
                conn.Open()
                dr = cmd.ExecuteReader()
                Dim prevarea As String = ""
                Dim currentarea As String = ""
                Dim team As String = ""
                Dim teamid As String = ""
                Dim day1 As String = Now.AddDays(-2).ToString("dd")
                Dim day2 As String = Now.AddDays(-1).ToString("dd")
                Dim day3 As String = Now.ToString("dd")
                Dim day1h As String = Now.AddDays(-2).ToString("ddd, MMM dd")
                Dim day2h As String = "Yesterday"
                Dim day3h As String = "Today"
                htmlsb.Append("<thead><tr><th colspan=""4"" align=""right""> <span id='sp1' style='float:Left'>Date : &nbsp</span> <input type='text' class='datepick'   id='history' value='" + curdate + "' onchange='javascript:refreshTable();'/><input type=""button"" style=""width: 80px;float:left;margin-left: 10px;"" value=""Refresh"" class=""btn btn-success btn-xs"" onclick=""showdiv();refreshTable()""/><div id='loader'>Loading....</div><span class=""j11"" style=""width: 254px;height: 13px;"">No GPS Device / Pending To Destination Setup </span><span class=""j15"" style=""width: 80px;height: 13px;color:white;"">No GPS Data</span><span class=""j12"" style=""width: 113px;height: 13px;color:black;"">Delivery Completed</span><span class=""j13"" style=""width: 80px;height: 13px;"">Time Out</span><span class=""j14"" style=""width: 80px;height: 13px;"">In Progress</span><span class=""j16"" style=""width: 120px;height: 13px;"">Waiting To Process</span><span class=""j17"" style=""width: 80px;height: 13px;color:white;"">Re Process</span></th></tr></thead>")
                htmlsb.Append("<thead><tr>")
                htmlsb.Append("<th scope=""col"" width=""16%"">Transporter Name</td>")
                htmlsb.Append("<th scope=""col"" width=""84%"">")
                htmlsb.Append("Data")
                htmlsb.Append("</th>")
                htmlsb.Append("</tr></thead>")
                htmlsb.Append("</tbody><tbody>")
                Dim plateno As String = ""
                While dr.Read()
                    Try
                        plateno = ""
                        plateno = dr("transporter_name").ToString().ToUpper()
                        Dim answer As String
                        If plateno.Length > 30 Then
                            answer = plateno.Substring(0, 30)
                        Else
                            answer = plateno
                        End If
                        answer = "<span title='" & plateno & "'>" & answer & "</span>"
                        teamid = dr("transporter_id")
                        htmlsb.Append("<tr><th scope=""row"">")
                        htmlsb.Append("<span class='jobids'>")
                        htmlsb.Append(answer)
                        htmlsb.Append("</span></th>")
                        htmlsb.Append("<td id=""" & teamid & """></td>")
                        htmlsb.Append("</tr>")
                    Catch ex As Exception
                    End Try
                End While
                htmlsb.Append("<thead><tr>")
                htmlsb.Append("<th scope=""col"" width=""16%"" align=""centre"">Transporter Name</td>")
                htmlsb.Append("<th scope=""col"" width=""84%"">")
                htmlsb.Append("Data")
                htmlsb.Append("</th>")
                htmlsb.Append("</tr></thead>")
                htmlsb.Append("<thead><tr><th colspan=""4"" align=""right""><span class=""j11"" style=""width: 254px;height: 13px;"">No GPS Device / Pending To Destination Setup </span><span class=""j15"" style=""width: 80px;height: 13px;color:white;"">No GPS Data</span><span class=""j12"" style=""width: 113px;height: 13px;color:black;"">Delivery Completed</span><span class=""j13"" style=""width: 80px;height: 13px;"">Time Out</span><span class=""j14"" style=""width: 80px;height: 13px;"">In Progress</span><span class=""j16"" style=""width: 120px;height: 13px;"">Waiting To Process</span><span class=""j17"" style=""width: 80px;height: 13px;color:white;"">Re Process</span></th></tr></thead>")

            Catch ex As Exception
            Finally
                conn.Close()
            End Try
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub
End Class
