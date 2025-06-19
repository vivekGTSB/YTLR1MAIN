Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.Web.Security
Imports System.Web
Imports System.Linq

Partial Class DriverAlertMgmtJson
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Check authentication
        If Request.Cookies("userinfo") Is Nothing OrElse Request.Cookies("userinfo")("userid") Is Nothing Then
            Response.StatusCode = 403
            Response.End()
            Exit Sub
        End If

        Dim opr As String = Request.QueryString("opr")
        Dim role As String = Request.QueryString("r")

        If String.IsNullOrEmpty(role) OrElse Not {"User", "SuperUser", "Operator", "Admin"}.Contains(role) Then
            Response.StatusCode = 403
            Response.End()
            Exit Sub
        End If

        Select Case opr
            Case "1"
                FillVehiclesGrid(Sanitize(Request.QueryString("u")), role, Sanitize(Request.QueryString("lst")))
            Case "2"
                AddData(Sanitize(Request.QueryString("uid")), Sanitize(Request.QueryString("tname")),
                        Request.QueryString("drlexp"), Sanitize(Request.QueryString("em1")),
                        Sanitize(Request.QueryString("emlcc")), Request.QueryString("gdlexp"),
                        Request.QueryString("oth"), Sanitize(Request.QueryString("icno")))
            Case "3"
                UpdateData(Sanitize(Request.QueryString("id")), Request.QueryString("drlexp"),
                           Sanitize(Request.QueryString("tname")), Sanitize(Request.QueryString("uid")),
                           Sanitize(Request.QueryString("em1")), Sanitize(Request.QueryString("emlcc")),
                           Request.QueryString("gdlexp"), Request.QueryString("oth"),
                           Sanitize(Request.QueryString("icno")))
            Case "4"
                DeleteData(Request.QueryString("ugData"))
            Case Else
                Response.StatusCode = 400
                Response.End()
        End Select
    End Sub

    Private Function Sanitize(input As String) As String
        If input Is Nothing Then Return ""
        Return HttpUtility.HtmlEncode(input.Trim())
    End Function

    Public Function FillVehiclesGrid(ByVal ugData As String, ByVal role As String, ByVal userslist As String) As String
        Dim aa As New ArrayList()
        Dim a As ArrayList

        Dim r As DataRow
        Dim t As New DataTable
        t.Columns.Add(New DataColumn("chk"))
        t.Columns.Add(New DataColumn("S NO"))
        t.Columns.Add(New DataColumn("Name"))
        t.Columns.Add(New DataColumn("ICno"))
        t.Columns.Add(New DataColumn("Driver License Exp"))
        t.Columns.Add(New DataColumn("GDL Licence Exp"))
        t.Columns.Add(New DataColumn("Others"))
        t.Columns.Add(New DataColumn("unm"))
        t.Columns.Add(New DataColumn("eml1"))
        t.Columns.Add(New DataColumn("eml2"))

        Dim userid As String = HttpContext.Current.Request.Cookies("userinfo")("userid")
        Dim rle As String = HttpContext.Current.Request.Cookies("userinfo")("role")
        Dim ulist As String = HttpContext.Current.Request.Cookies("userinfo")("userslist")
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As SqlCommand
        Dim query As String

        Try
            If rle = "User" Then
                query = "SELECT emailid1,emailid2,id,drivername,icno,driverlicenseexp,GDLlicenseexp,others,u.username,u.userid FROM (SELECT * FROM driver_alerts WHERE userid=@userid) t LEFT OUTER JOIN userTBL u ON u.userid=t.userid"
                cmd = New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@userid", userid)
            ElseIf rle = "SuperUser" Or rle = "Operator" Then
                Dim userIds = userslist.Split(","c).Where(Function(u) IsNumeric(u.Trim())).ToArray()
                Dim placeholders As New List(Of String)
                cmd = New SqlCommand()
                For x As Integer = 0 To userIds.Length - 1
                    Dim param As String = "@id" & x
                    placeholders.Add(param)
                    cmd.Parameters.Add(param, SqlDbType.Int).Value = Convert.ToInt32(userIds(x).Trim())
                Next
                query = "SELECT emailid1,emailid2,id,drivername,icno,driverlicenseexp,GDLlicenseexp,others,u.username,u.userid FROM (SELECT * FROM driver_alerts WHERE userid IN (" & String.Join(",", placeholders) & ")) t LEFT OUTER JOIN userTBL u ON u.userid=t.userid"
                cmd.CommandText = query
                cmd.Connection = conn
            Else
                query = "SELECT emailid1,emailid2,id,drivername,icno,driverlicenseexp,GDLlicenseexp,others,u.username,u.userid FROM driver_alerts t LEFT OUTER JOIN userTBL u ON u.userid=t.userid"
                cmd = New SqlCommand(query, conn)
            End If

            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Dim i As Int32 = 1
            Dim insDateTime As String = ""
            While (dr.Read())
                Try
                    If Not IsDBNull(dr("driverlicenseexp")) Then
                        If dr("driverlicenseexp").ToString() = "1900-01-01 00:00:00.000" Then
                            insDateTime = ""
                        Else
                            insDateTime = Convert.ToDateTime(dr("driverlicenseexp")).ToString("yyyy/MM/dd")
                        End If
                    Else
                        insDateTime = ""
                    End If
                    r = t.NewRow
                    r(0) = "<input type='checkbox' name='chk' class='group1' value='" & HttpUtility.HtmlAttributeEncode(dr("id").ToString()) & "'/>"
                    r(1) = i.ToString()

                    r(8) = HttpUtility.HtmlEncode(dr("emailid1").ToString())
                    If Not IsDBNull(dr("emailid2")) Then
                        r(9) = HttpUtility.HtmlEncode(dr("emailid2").ToString())
                    Else
                        r(6) = ""
                    End If

                    r(4) = insDateTime
                    r(5) = If(Not IsDBNull(dr("GDLlicenseexp")) AndAlso dr("GDLlicenseexp").ToString() <> "1900-01-01 00:00:00.000", Convert.ToDateTime(dr("GDLlicenseexp")).ToString("yyyy/MM/dd"), "")
                    r(6) = If(Not IsDBNull(dr("others")) AndAlso dr("others").ToString() <> "1900-01-01 00:00:00.000", Convert.ToDateTime(dr("others")).ToString("yyyy/MM/dd"), "")
                    r(3) = If(Not IsDBNull(dr("icno")), HttpUtility.HtmlEncode(dr("icno").ToString()), "")

                    Dim popupCall As New StringBuilder()
                    popupCall.Append("<span style='cursor:pointer;text-decoration:underline;' onclick=""javascript:openPopup('")
                    popupCall.Append(HttpUtility.JavaScriptStringEncode(dr("id").ToString())).Append("','")
                    popupCall.Append(HttpUtility.JavaScriptStringEncode(dr("drivername").ToString())).Append("','")
                    popupCall.Append(HttpUtility.JavaScriptStringEncode(dr("icno").ToString())).Append("','")
                    popupCall.Append(insDateTime).Append("','")
                    popupCall.Append(r(5).ToString()).Append("','")
                    popupCall.Append(r(6).ToString()).Append("','")
                    popupCall.Append(HttpUtility.JavaScriptStringEncode(dr("userid").ToString())).Append("','")
                    popupCall.Append(r(8).ToString()).Append("','")
                    popupCall.Append(r(9).ToString()).Append("')"">")
                    popupCall.Append(HttpUtility.HtmlEncode(Convert.ToString(dr("drivername")).ToUpper())).Append("</span>")
                    r(2) = popupCall.ToString()
                    r(7) = HttpUtility.HtmlEncode(dr("username").ToString().ToUpper())

                    t.Rows.Add(r)
                    i += 1
                Catch ex As Exception
                    System.Diagnostics.Trace.WriteLine("Row processing error: " & ex.Message)
                End Try
            End While

            If t.Rows.Count = 0 Then
                r = t.NewRow
                For col = 0 To t.Columns.Count - 1
                    r(col) = "--"
                Next
                t.Rows.Add(r)
            End If

            Session("exceltable") = t
            For j As Integer = 0 To t.Rows.Count - 1
                Try
                    a = New ArrayList()
                    For col = 0 To t.Columns.Count - 1
                        a.Add(t.DefaultView.Item(j)(col))
                    Next
                    aa.Add(a)
                Catch ex As Exception
                    System.Diagnostics.Trace.WriteLine("Session serialization error: " & ex.Message)
                End Try
            Next

            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.Write(json)

        Catch ex As Exception
            System.Diagnostics.Trace.WriteLine("Query execution error: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Function
    Public Sub AddData(ByVal uid As String, ByVal tname As String, ByVal drliexp As String, ByVal eml1 As String, ByVal emlcc As String, ByVal gdlexp As String, ByVal others As String, ByVal icno As String)
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim result As String = "0"
        Dim cmd As SqlCommand
        Try
            conn.Open()
            If gdlexp <> "" Then
                gdlexp = Convert.ToDateTime(gdlexp).ToString("yyyy/MM/dd 00:00:00")
            End If
            If others <> "" Then
                others = Convert.ToDateTime(others).ToString("yyyy/MM/dd 00:00:00")
            End If
            If drliexp <> "" Then
                drliexp = Convert.ToDateTime(drliexp).ToString("yyyy/MM/dd 00:00:00")
            End If
            cmd = New SqlCommand("insert into driver_alerts(drivername,icno,driverlicenseexp,GDLlicenseexp,others,userid,emailid1,emailid2) 
                                 values(@tname,@icno,@drliexp,@gdlexp,@others ,@uid,@eml1,@emlcc)", conn)
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@tname", tname)
            cmd.Parameters.AddWithValue("@icno", icno)
            cmd.Parameters.AddWithValue("@drliexp", drliexp)
            cmd.Parameters.AddWithValue("@gdlexp", gdlexp)
            cmd.Parameters.AddWithValue("@others", others)
            cmd.Parameters.AddWithValue("@uid", uid)
            cmd.Parameters.AddWithValue("@eml1", eml1)
            cmd.Parameters.AddWithValue("@emlcc", emlcc)
            result = cmd.ExecuteNonQuery()
            If (result > 0) Then
                Response.Write("Yes")
            Else
                Response.Write("No")
            End If
        Catch ex As Exception
            Response.Write(cmd.CommandText)
        Finally
            conn.Close()
        End Try

    End Sub


    Public Sub UpdateData(ByVal id As String, ByVal drliexp As String, ByVal tname As String, ByVal uid As String, ByVal eml1 As String, ByVal emlcc As String, ByVal gdlexp As String, ByVal others As String, ByVal icno As String)
        Dim result As String = "0"
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            If gdlexp <> "" Then
                gdlexp = Convert.ToDateTime(gdlexp).ToString("yyyy/MM/dd 00:00:00")
            End If
            If others <> "" Then
                others = Convert.ToDateTime(others).ToString("yyyy/MM/dd 00:00:00")
            End If
            If drliexp <> "" Then
                drliexp = Convert.ToDateTime(drliexp).ToString("yyyy/MM/dd 00:00:00")
            End If
            Dim cmd As New SqlCommand("update  driver_alerts set drivername=@tname,
                           emailid1=@eml1,emailid2=@emlcc,driverlicenseexp=@drliexp,
                           GDLlicenseexp=@gdlexp, others=@others,icno=@icno, userid=@uid  where id=@id", conn)
            cmd.Parameters.AddWithValue("@tname", tname)
            cmd.Parameters.AddWithValue("@eml1", eml1)
            cmd.Parameters.AddWithValue("@emlcc", emlcc)
            cmd.Parameters.AddWithValue("@drliexp", drliexp)
            cmd.Parameters.AddWithValue("@gdlexp", gdlexp)
            cmd.Parameters.AddWithValue("@others", others)
            cmd.Parameters.AddWithValue("@icno", icno)
            cmd.Parameters.AddWithValue("@uid", uid)
            cmd.Parameters.AddWithValue("@id", id)
            Try
                conn.Open()
                result = cmd.ExecuteNonQuery().ToString()
                If (result > 0) Then
                    Response.Write("Yes")
                Else
                    Response.Write("No")
                End If
            Catch ex As Exception
                Response.Write(ex.Message)
            Finally
                conn.Close()
            End Try
        Catch ex As Exception

        End Try

    End Sub


    Public Sub DeleteData(ByVal ugData As String)
        Dim res As String = "0"
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim Tides() As String = ugData.Split(",")
            For i As Int32 = 0 To Tides.Length - 1
                Try
                    conn.Open()
                    If Tides(i) = "on" Then

                    Else
                        cmd = New SqlCommand("delete from driver_alerts where id=@Ids", conn)
                        cmd.Parameters.AddWithValue("@Ids", Tides(i))
                        res = cmd.ExecuteNonQuery().ToString()
                    End If
                Catch ex As Exception
                    res = ex.Message
                Finally
                    conn.Close()
                End Try
            Next
            Response.Write("Yes")
        Catch ex As Exception
            res = ex.Message
        End Try
    End Sub
End Class
