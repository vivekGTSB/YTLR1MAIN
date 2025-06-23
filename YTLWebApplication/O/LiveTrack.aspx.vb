Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports Newtonsoft.Json

Partial Class LiveTrack
    Inherits System.Web.UI.Page
    Public sb As StringBuilder
    Public bdt As String = ""
    Public edt As String = ""
    Public role As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.Cookies("userinfo") Is Nothing Then
            Server.Transfer("Login.aspx")
        Else
            If Page.IsPostBack = False Then
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                role = Request.Cookies("userinfo")("role")
                sb = New StringBuilder

                Dim conn1 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim ds As New DataSet
                Dim da As SqlDataAdapter
                If role = "User" Then
                    da = New SqlDataAdapter("select userid,username from userTBL where userid='" & userid & "' order by username desc", conn1)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    da = New SqlDataAdapter("select userid,username from userTBL where  userid in(" & userslist & ") order by username desc", conn1)
                Else
                    da = New SqlDataAdapter("select userid,username from userTBL where role='User' order by username desc", conn1)
                End If
                da.Fill(ds)
                sb.Append("<select style=""width:500px;"" id=""ddlplate""  runat=""server"" data-placeholder=""Select Plate Number"" style=""width:350px;"" class=""chzn-select"" tabindex=""5"">")
                sb.Append("<option value=""0""></option>")

                Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                Dim dr As SqlDataReader
                Dim statusstate As String = "3" ' For In-Process

                Dim OSSDict As New Dictionary(Of String, OssRecord)

                Dim cmd As New SqlCommand("select s.name,p.plateno,p.source_supply,p.status from (select * from oss_patch_out where weight_outtime between '" & Now.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss") & "' and '" & Now.ToString("yyyy/MM/dd HH:mm:ss") & "'  and status='" & statusstate & "')  p  Left Outer Join  oss_ship_to_code s On s.shiptocode=p.destination_siteid   order by p.weight_outtime desc", conn2)
                Try
                    conn2.Open()
                    dr = cmd.ExecuteReader()
                    While dr.Read()
                        Try
                            Dim ossr As New OssRecord()
                            ossr.source = dr("source_supply")
                            ossr.destination = dr("name")
                            ossr.status = dr("status")
                            OSSDict.Add(dr("plateno"), ossr)
                        Catch ex As Exception

                        End Try
                    End While

                Catch ex As Exception

                Finally
                    conn2.Close()
                End Try
                Dim ossRec As New OssRecord()

                Dim vcmd As SqlCommand
                Dim i As Integer
                For i = 0 To ds.Tables(0).Rows.Count - 1
                    vcmd = New SqlCommand("select plateno,isnull(pmid,'-') as pmid from vehicleTBL where userid='" & ds.Tables(0).Rows(i)(0) & "' order by pmid", conn1)
                    conn1.Open()
                    sb.Append("<optgroup label=""" & ds.Tables(0).Rows(i)(1).ToString().ToUpper() & """>")
                    Dim dr2 As SqlDataReader = vcmd.ExecuteReader()
                    While dr2.Read()
                        If dr2("pmid") = "-" Then
                            If OSSDict.ContainsKey(dr2("plateno")) Then
                                ossRec = OSSDict.Item(dr2("plateno"))
                                sb.Append("<option value=""" & dr2("plateno") & """>" & dr2("plateno") & " [" & ossRec.source & " To  " & ossRec.destination & "]</option>")
                            Else
                                sb.Append("<option value=""" & dr2("plateno") & """>" & dr2("plateno") & "</option>")
                            End If
                        Else
                            If OSSDict.ContainsKey(dr2("plateno")) Then
                                ossRec = OSSDict.Item(dr2("plateno"))
                                sb.Append("<option value=""" & dr2("plateno") & """>" & dr2("pmid") & " - " & dr2("plateno") & " [" & ossRec.source & " To  " & ossRec.destination & "]</option>")
                            Else
                                sb.Append("<option value=""" & dr2("plateno") & """>" & dr2("pmid") & " - " & dr2("plateno") & "</option>")
                            End If
                        End If


                    End While
                    sb.Append("</optgroup>")
                    conn1.Close()
                    dr2.Close()
                Next
                sb.Append("</select>")
            End If
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function RefreshJobs() As String
        Dim sbopts As New StringBuilder()
        Dim json As String = ""
        Try
            Dim userid As String = HttpContext.Current.Request.Cookies("userinfo")("userid")
            Dim userslist As String = HttpContext.Current.Request.Cookies("userinfo")("userslist")
            Dim role As String = HttpContext.Current.Request.Cookies("userinfo")("role")
            sbopts = New StringBuilder
            Dim conn1 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim ds As New DataSet
            Dim da As SqlDataAdapter
            If role = "User" Then
                da = New SqlDataAdapter("select userid,username from userTBL where userid='" & userid & "' order by username desc", conn1)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                da = New SqlDataAdapter("select userid,username from userTBL where  userid in(" & userslist & ") order by username desc", conn1)
            Else
                da = New SqlDataAdapter("select userid,username from userTBL where role='User' order by username desc", conn1)
            End If
            da.Fill(ds)
            'sb.Append("<select style=""width:500px;"" id=""ddlplate""  runat=""server"" data-placeholder=""Select Plate Number"" style=""width:350px;"" class=""chzn-select"" tabindex=""5"">")
            sbopts.Append("<option value=""0""></option>")

            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim dr As SqlDataReader
            Dim statusstate As String = "3" ' For In-Process

            Dim OSSDict As New Dictionary(Of String, OssRecord)

            Dim cmd As New SqlCommand("select s.name,p.plateno,p.source_supply,p.status from (select * from oss_patch_out where weight_outtime between '" & Now.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss") & "' and '" & Now.ToString("yyyy/MM/dd HH:mm:ss") & "'  and status='" & statusstate & "')  p  Left Outer Join  oss_ship_to_code s On s.shiptocode=p.destination_siteid   order by p.weight_outtime desc", conn2)
            Try
                conn2.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    Try
                        Dim ossr As New OssRecord()
                        ossr.source = dr("source_supply")
                        ossr.destination = dr("name")
                        ossr.status = dr("status")
                        OSSDict.Add(dr("plateno"), ossr)
                    Catch ex As Exception

                    End Try
                End While

            Catch ex As Exception

            Finally
                conn2.Close()
            End Try
            Dim ossRec As New OssRecord()

            Dim vcmd As SqlCommand
            Dim i As Integer
            For i = 0 To ds.Tables(0).Rows.Count - 1
                vcmd = New SqlCommand("select plateno,isnull(pmid,'-') as pmid from vehicleTBL where userid='" & ds.Tables(0).Rows(i)(0) & "' order by pmid", conn1)
                conn1.Open()
                sbopts.Append("<optgroup label=""" & ds.Tables(0).Rows(i)(1).ToString().ToUpper() & """>")
                Dim dr2 As SqlDataReader = vcmd.ExecuteReader()
                While dr2.Read()
                    If dr2("pmid") = "-" Then
                        If OSSDict.ContainsKey(dr2("plateno")) Then
                            ossRec = OSSDict.Item(dr2("plateno"))
                            sbopts.Append("<option value=""" & dr2("plateno") & """>" & dr2("plateno") & " [" & ossRec.source & " To  " & ossRec.destination & "]</option>")
                        Else
                            sbopts.Append("<option value=""" & dr2("plateno") & """>" & dr2("plateno") & "</option>")
                        End If
                    Else
                        If OSSDict.ContainsKey(dr2("plateno")) Then
                            ossRec = OSSDict.Item(dr2("plateno"))
                            sbopts.Append("<option value=""" & dr2("plateno") & """>" & dr2("pmid") & " - " & dr2("plateno") & " [" & ossRec.source & " To  " & ossRec.destination & "]</option>")
                        Else
                            sbopts.Append("<option value=""" & dr2("plateno") & """>" & dr2("pmid") & " - " & dr2("plateno") & "</option>")
                        End If
                    End If
                End While
                sbopts.Append("</optgroup>")
                conn1.Close()
                dr2.Close()
            Next
            sbopts.Append("</select>")
            json = JsonConvert.SerializeObject(sbopts, Formatting.None)
        Catch ex As Exception
            Return ex.Message
        End Try
        Return json

    End Function


    Private Structure OssRecord
        Dim source As String
        Dim destination As String
        Dim status As String
        Dim plateno As String
    End Structure
End Class
