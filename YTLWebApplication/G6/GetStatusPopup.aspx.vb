Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.Script.Services

Public Class GetStatusPopup
    Inherits System.Web.UI.Page
    Public sb1 As New StringBuilder()
    Public loggedinUID As String = ""
    Public plateno As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        loggedinUID = Request.Cookies("userinfo")("userid")
        plateno = Request.QueryString("p")
        FillGrid()
    End Sub
    Public Sub FillGrid()
        Try
            Try

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim t As New DataTable
                t.Rows.Clear()
                t.Columns.Add(New DataColumn("S No"))
                t.Columns.Add(New DataColumn("Date Time"))
                t.Columns.Add(New DataColumn("Plateno"))
                t.Columns.Add(New DataColumn("Status"))
                t.Columns.Add(New DataColumn("Remarks"))
                t.Columns.Add(New DataColumn("Added By"))
                t.Columns.Add(New DataColumn("Edit"))

                Dim r As DataRow
                Try
                    Dim cmd2 As New SqlCommand("select plateno,timestamp,statusdate,status,officeremark,sourcename from maintenance where timestamp>'2019/09/01' and plateno='" & plateno & "' order by timestamp desc", conn)
                    '  Response.Write(cmd2.CommandText)

                    conn.Open()
                    Dim dr2 As SqlDataReader = cmd2.ExecuteReader()
                    Dim c As Integer = 1
                    While dr2.Read()
                        Try
                            r = t.NewRow()
                            r(0) = c
                            r(1) = DateTime.Parse(dr2("statusdate")).ToString("yyyy/MM/dd HH:mm:ss")
                            r(2) = dr2("plateno").ToString()
                            r(3) = dr2("status").ToString()
                            If IsDBNull(dr2("officeremark")) Then
                                r(4) = "<span class='clsremarks'></span>"
                            Else
                                r(4) = "<span class='clsremarks'>" & dr2("officeremark").ToString() & "</span>"
                            End If

                            If IsDBNull(dr2("sourcename")) Then
                                r(5) = ""
                            Else
                                r(5) = dr2("sourcename").ToString().ToUpper()
                            End If
                            r(6) = "<span class='btnedit' data-plateno='" & dr2("plateno") & "'  data-statusdate='" & dr2("statusdate") & "'  > Edit </span><span class='btnupdate hidenow' data-plateno='" & dr2("plateno") & "'  data-statusdate='" & dr2("statusdate") & "'  > Update </span><span class='btncancel hidenow' data-plateno='" & dr2("plateno") & "'  data-statusdate='" & dr2("statusdate") & "'  >Cancel </span>"
                            t.Rows.Add(r)
                            c += 1
                        Catch ex As Exception

                        End Try
                    End While
                Catch ex As Exception

                Finally
                    conn.Close()
                End Try
                If t.Rows.Count = 0 Then
                    r = t.NewRow
                    r(0) = "--"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    r(6) = "--"
                    t.Rows.Add(r)
                End If


                sb1.Append("<thead><tr align=""left""><th>S No</th><th>Date Time</th><th>Plate No</th><th>Status</th><th>Remarks</th><th>Added By</th><th>Edit</th></tr></thead>")

                sb1.Append("<tbody>")
                Dim finalTb As DataTable
                finalTb = t.Copy()
                For i As Integer = 0 To finalTb.Rows.Count - 1
                    Try
                        sb1.Append("<tr>")
                        sb1.Append("<td>")
                        sb1.Append((i + 1).ToString())
                        sb1.Append("</td><td>")
                        sb1.Append(finalTb.DefaultView.Item(i)(1))
                        sb1.Append("</td><td>")
                        sb1.Append(finalTb.DefaultView.Item(i)(2))
                        sb1.Append("</td><td>")
                        sb1.Append(finalTb.DefaultView.Item(i)(3))
                        sb1.Append("</td><td>")
                        sb1.Append(finalTb.DefaultView.Item(i)(4))
                        sb1.Append("</td><td>")
                        sb1.Append(finalTb.DefaultView.Item(i)(5))
                        sb1.Append("</td><td>")
                        sb1.Append(finalTb.DefaultView.Item(i)(6))
                        sb1.Append("</td></tr>")
                    Catch ex As Exception
                        Response.Write("1." & ex.Message)
                    End Try
                Next
                sb1.Append("</tbody>")
                sb1.Append("<tfoot><th>S No</th><th>Date Time</th><th>Plate No</th><th>Status</th><th>Remarks</th><th>Added By</th><th>Edit</th></tr></tfoot>")
            Catch ex As Exception
                Response.Write("2." & ex.Message)
            End Try

        Catch ex As Exception
            Response.Write("32." & ex.Message)
        End Try

    End Sub

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function saveData(ByVal plateno As String, ByVal sd As String, ByVal remarks As String) As String
        Dim res As String = ""
        Dim username As String = HttpContext.Current.Request.Cookies("userinfo")("username")
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As New SqlCommand("update maintenance set officeremark=@remarks,sourcename=@snm where plateno=@plateno and statusdate=@sd", conn)
        cmd.Parameters.AddWithValue("@plateno", plateno)
        cmd.Parameters.AddWithValue("@remarks", remarks)
        cmd.Parameters.AddWithValue("@snm", username)
        cmd.Parameters.AddWithValue("@sd", Convert.ToDateTime(sd).ToString("yyyy/MM/dd HH:mm:ss"))

        Try
            conn.Open()
            res = cmd.ExecuteNonQuery()
        Catch ex As Exception

        Finally
            conn.Close()
        End Try

        Return res
    End Function
End Class