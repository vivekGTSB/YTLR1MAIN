Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports AspMap
Imports Newtonsoft.Json

Partial Class FleetMoniterReport
    Inherits System.Web.UI.Page
    Public Shared ec As String = "false"
    Public show As Boolean = False
    
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' Check authentication using secure helper
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("INIT_ERROR", "Error in OnInit: " & ex.Message)
            Response.Redirect("~/Login.aspx")
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' Validate session
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If
            
            LoadTransporterNames()
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("PAGE_LOAD_ERROR", "Error in Page_Load: " & ex.Message)
        End Try
    End Sub
    
    Private Sub LoadTransporterNames()
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = "SELECT userid, username FROM YTLDB.dbo.userTBL WHERE userid NOT IN (7144,7145,7146,7147,7148,7099,7180) AND companyname LIKE 'ytl%' AND role = 'User' ORDER BY username"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        ddltransportername.Items.Clear()
                        ddltransportername.Items.Add(New ListItem("ALL", "ALL"))
                        While dr.Read()
                            ddltransportername.Items.Add(New ListItem(SecurityHelper.SanitizeForHtml(dr("username").ToString()), dr("userid").ToString()))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOAD_TRANSPORTERS_ERROR", "Error loading transporters: " & ex.Message)
        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function GetData(ByVal type As String, ByVal username As String) As String
        Try
            ' Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Return "{""error"":""Unauthorized""}"
            End If
            
            ' Validate input parameters
            If Not SecurityHelper.ValidateInput(type, "alphanumeric") OrElse 
               Not SecurityHelper.ValidateInput(username, "alphanumeric") Then
                SecurityHelper.LogSecurityEvent("INVALID_INPUT", "Invalid parameters in GetData: type=" & type & ", username=" & username)
                Return "{""error"":""Invalid parameters""}"
            End If
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim aa As New ArrayList()
                Dim query As String = ""
                
                Using cmd As New SqlCommand()
                    cmd.Connection = conn
                    
                    If username = "ALL" Then
                        If type = "ALL" Then
                            query = "SELECT t1.plateno, ISNULL(pmid,'-') AS pmid, ISNULL(remark,'') AS remark, ISNULL(status,'Running') AS status FROM vehicletbl t1 LEFT OUTER JOIN vehicle_status_tracked2 t2 ON t1.plateno = t2.plateno ORDER BY t1.plateno"
                        ElseIf type = "1" Then
                            query = "SELECT t1.plateno, ISNULL(pmid,'-') AS pmid, ISNULL(remark,'') AS remark, ISNULL(status,'Running') AS status FROM vehicletbl t1 LEFT OUTER JOIN vehicle_status_tracked2 t2 ON t1.plateno = t2.plateno WHERE t1.userid IN (SELECT userid FROM YTLDB.dbo.userTBL WHERE companyname LIKE 'ytl%' AND role = 'User') ORDER BY t1.plateno"
                        Else
                            query = "SELECT t1.plateno, ISNULL(pmid,'-') AS pmid, ISNULL(remark,'') AS remark, ISNULL(status,'Running') AS status FROM vehicletbl t1 LEFT OUTER JOIN vehicle_status_tracked2 t2 ON t1.plateno = t2.plateno WHERE t1.userid IN (SELECT userid FROM YTLDB.dbo.userTBL WHERE companyname NOT LIKE 'ytl%' AND role = 'User') ORDER BY t1.plateno"
                        End If
                    Else
                        query = "SELECT t1.plateno, ISNULL(pmid,'-') AS pmid, ISNULL(remark,'') AS remark, ISNULL(status,'Running') AS status FROM vehicletbl t1 LEFT OUTER JOIN vehicle_status_tracked2 t2 ON t1.plateno = t2.plateno WHERE t1.userid = @username ORDER BY t1.plateno"
                        cmd.Parameters.AddWithValue("@username", username)
                    End If
                    
                    cmd.CommandText = query
                    conn.Open()
                    
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim i As Integer = 1
                        While dr.Read()
                            Dim a As New ArrayList()
                            a.Add(i)
                            a.Add(SecurityHelper.SanitizeForHtml(dr("pmid").ToString()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("plateno").ToString()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("status").ToString()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("remark").ToString()))
                            aa.Add(a)
                            i += 1
                        End While
                    End Using
                End Using
                
                ' Create data table for Excel export
                Dim t As New DataTable
                t.Columns.Add(New DataColumn("S NO"))
                t.Columns.Add(New DataColumn("PM Id"))
                t.Columns.Add(New DataColumn("Plate NO"))
                t.Columns.Add(New DataColumn("Status"))
                t.Columns.Add(New DataColumn("Remarks"))
                
                Dim r As DataRow
                For Each a As ArrayList In aa
                    r = t.NewRow()
                    For i As Integer = 0 To 4
                        r(i) = a(i)
                    Next
                    t.Rows.Add(r)
                Next
                
                HttpContext.Current.Session.Remove("exceltable")
                HttpContext.Current.Session.Remove("exceltable2")
                HttpContext.Current.Session("exceltable") = t
                
                Dim json As String = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
                Return json
            End Using
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GETDATA_ERROR", "Error in GetData: " & ex.Message)
            Return "{""error"":""An error occurred""}"
        End Try
    End Function
    
    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function GetUsers(ByVal type As String) As String
        Try
            ' Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Return "{""error"":""Unauthorized""}"
            End If
            
            ' Validate input
            If Not SecurityHelper.ValidateInput(type, "alphanumeric") Then
                SecurityHelper.LogSecurityEvent("INVALID_TYPE_INPUT", "Invalid type parameter: " & type)
                Return "{""error"":""Invalid type""}"
            End If
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim aa As New ArrayList()
                Dim query As String = ""
                
                Select Case type
                    Case "ALL"
                        query = "SELECT userid, username FROM usertbl WHERE role='User' ORDER BY username"
                    Case "1"
                        query = "SELECT userid, username FROM YTLDB.dbo.userTBL WHERE userid NOT IN (7144,7145,7146,7147,7148,7099,7180) AND companyname LIKE 'ytl%' AND role = 'User' ORDER BY username"
                    Case "2"
                        query = "SELECT userid, username FROM userTBL WHERE companyname NOT LIKE 'ytl%' AND role = 'User' ORDER BY username"
                    Case Else
                        Return "{""error"":""Invalid type""}"
                End Select
                
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim a As New ArrayList()
                            a.Add(dr("userid"))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("username").ToString()))
                            aa.Add(a)
                        End While
                    End Using
                End Using
                
                Dim json As String = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
                Return json
            End Using
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GETUSERS_ERROR", "Error in GetUsers: " & ex.Message)
            Return "{""error"":""An error occurred""}"
        End Try
    End Function

End Class