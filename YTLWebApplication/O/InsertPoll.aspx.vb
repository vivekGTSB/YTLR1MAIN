Imports System.Data.SqlClient
Imports System.Data
Imports System.Runtime

Partial Class InsertPoll
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim result As String = "No"
        Try
            Dim plateno As String = Request.QueryString("plateno").Trim()
            Dim cmdObj As SMSInsert.GlobalSimService = New SMSInsert.GlobalSimService()
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand("select v.versionid,v.userid,v.plateno,v.unitid,u.simno from vehicleTBL v,unitLST u where v.unitid=u.unitid and v.plateno='" & plateno & "'", conn)
            Dim dr As SqlDataReader
            Dim transid As String = ""
            Dim versionid As String = ""
            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                If (dr.Read()) Then
                    versionid = dr("versionid").ToString.ToUpper()
                    Dim type As String = "0"
                    Dim cmdstr As String = ""
                    If (versionid = "GT06" Or versionid = "GT07") And (type = "1" Or type = "2") Then
                        If type = "1" Then
                            cmdstr = "DYD,000000#"
                        ElseIf type = "2" Then
                            cmdstr = "HFYD,000000#"
                        End If
                    Else
                        If type = 1 Then
                            If versionid.Contains("MT") Then
                                cmdstr = "(1234;IMMO=ON)"
                            Else
                                cmdstr = "(PGUS;063)"
                            End If
                        ElseIf type = 2 Then
                            cmdstr = "(PGUS;060)"
                        Else
                            cmdstr = "(CGUS;01)"
                            If (versionid = "GT06" Or versionid = "GT07") Then
                                cmdstr = "WHERE#"
                            End If
                        End If
                    End If
                    Dim truresponse As Int16 = cmdObj.insertInboxData(dr("userid"), dr("plateno"), dr("simno"), "Data Received From Unit, Refer SmartFleet to Update", Now.ToString("yyyy/MM/dd HH:mm:ss"), "sqlserverconnectionLafarge")
                    If truresponse <> 0 Then
                        transid = truresponse.ToString()
                    End If
                    If (transid <> "") Then
                        result = "Yes"
                    End If
                End If
            Catch ex As Exception
                Response.Write(ex.Message & " - " & ex.StackTrace)
                Response.ContentType = "text/plain"
            Finally
                conn.Close()
            End Try

        Catch ex As Exception
            Response.Write(ex.Message & " - " & ex.StackTrace)
            Response.ContentType = "text/plain"
        End Try

        Response.Write(result)
        Response.ContentType = "text/plain"
    End Sub
End Class
