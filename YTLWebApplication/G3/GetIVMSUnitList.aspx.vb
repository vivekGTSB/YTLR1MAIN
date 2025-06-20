Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class GetIVMSUnitList
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate session
            If Not IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.Write("{""error"":""Unauthorized""}")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate and sanitize input
            Dim version As String = ValidateVersionInput(Request.QueryString("v"))
            If String.IsNullOrEmpty(version) Then
                Response.StatusCode = 400
                Response.Write("{""error"":""Invalid version parameter""}")
                Response.End()
                Return
            End If

            Response.Write(FillGrid(version))
            Response.ContentType = "application/json"

        Catch ex As Exception
            ' SECURITY FIX: Log error securely without exposing details
            LogSecurityEvent("GetIVMSUnitList error", ex.Message)
            Response.StatusCode = 500
            Response.Write("{""error"":""Internal server error""}")
        End Try
    End Sub

    ' SECURITY FIX: Validate session authentication
    Private Function IsUserAuthenticated() As Boolean
        Try
            Return HttpContext.Current.Session("authenticated") IsNot Nothing AndAlso 
                   CBool(HttpContext.Current.Session("authenticated")) AndAlso
                   HttpContext.Current.Session("userid") IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ' SECURITY FIX: Validate version input
    Private Function ValidateVersionInput(version As String) As String
        If String.IsNullOrWhiteSpace(version) Then
            Return ""
        End If

        ' Only allow "All" or numeric values
        If version.Equals("All", StringComparison.OrdinalIgnoreCase) Then
            Return "All"
        End If

        ' Validate numeric version
        If System.Text.RegularExpressions.Regex.IsMatch(version, "^[0-9]+$") AndAlso version.Length <= 10 Then
            Return version
        End If

        Return ""
    End Function

    Public Function FillGrid(ByVal version As String) As String
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim json As String = Nothing
        
        Try
            Dim aa As New ArrayList
            Dim a As ArrayList
            cmd.Connection = conn
            
            ' SECURITY FIX: Use parameterized queries
            If version = "All" Then
                cmd.CommandText = "select t2.username,t1.plateno,t3.versionid,t3.unitid,t3.simno,isnull(t4.groupname,'-') as groupname,dbo.GetTrailerStatus(t1.plateno) as status from vehicleTBL t1 left outer join userTBL t2 on t1.userid=t2.userid inner join unitLST t3 on t1.unitid =t3.unitid left outer join vehicle_group t4 on t1.groupid=t4.groupid where t2.role ='User' and t2.companyname not like 'Gussmann%'"
            Else
                cmd.CommandText = "select t2.username,t1.plateno,t3.versionid,t3.unitid,t3.simno,isnull(t4.groupname,'-') as groupname,dbo.GetTrailerStatus(t1.plateno) as status from vehicleTBL t1 left outer join userTBL t2 on t1.userid=t2.userid inner join unitLST t3 on t1.unitid =t3.unitid left outer join vehicle_group t4 on t1.groupid=t4.groupid where t2.role ='User' and t2.companyname not like 'Gussmann%' and t3.versionid=@vid"
                cmd.Parameters.AddWithValue("@vid", version)
            End If

            Dim unitlisttable As New DataTable
            unitlisttable.Columns.Add(New DataColumn("No"))
            unitlisttable.Columns.Add(New DataColumn("Transporter Name"))
            unitlisttable.Columns.Add(New DataColumn("Plateno"))
            unitlisttable.Columns.Add(New DataColumn("Version"))
            unitlisttable.Columns.Add(New DataColumn("Unitid"))
            unitlisttable.Columns.Add(New DataColumn("Simno"))
            unitlisttable.Columns.Add(New DataColumn("Type"))
            unitlisttable.Columns.Add(New DataColumn("TrailerID"))
            
            Dim r As DataRow
            conn.Open()
            dr = cmd.ExecuteReader()
            Dim i As Integer = 1
            
            While dr.Read()
                r = unitlisttable.NewRow()
                r(0) = i
                ' SECURITY FIX: HTML encode output to prevent XSS
                r(1) = HttpUtility.HtmlEncode(dr("username").ToString())
                r(2) = HttpUtility.HtmlEncode(dr("plateno").ToString().ToUpper())
                r(3) = HttpUtility.HtmlEncode(dr("versionid").ToString())
                r(4) = HttpUtility.HtmlEncode(dr("unitid").ToString())
                r(5) = HttpUtility.HtmlEncode(dr("simno").ToString())
                r(6) = HttpUtility.HtmlEncode(dr("groupname").ToString())
                
                If dr("status").ToString() = "0" Then
                    r(7) = "No"
                Else
                    r(7) = "Yes"
                End If

                i = i + 1
                unitlisttable.Rows.Add(r)
            End While
            dr.Close()
            
            If unitlisttable.Rows.Count = 0 Then
                r = unitlisttable.NewRow()
                r(0) = "-"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                r(5) = "-"
                r(6) = "-"
                r(7) = "-"
                unitlisttable.Rows.Add(r)
            End If
            
            For i = 0 To unitlisttable.Rows.Count - 1
                a = New ArrayList
                a.Add(unitlisttable.Rows(i)(0))
                a.Add(unitlisttable.Rows(i)(1))
                a.Add(unitlisttable.Rows(i)(2))
                a.Add(unitlisttable.Rows(i)(3))
                a.Add(unitlisttable.Rows(i)(4))
                a.Add(unitlisttable.Rows(i)(5))
                a.Add(unitlisttable.Rows(i)(6))
                a.Add(unitlisttable.Rows(i)(7))
                aa.Add(a)
            Next
            
            json = JsonConvert.SerializeObject(aa, Formatting.None)
            
            ' Clear session variables
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session.Remove("exceltable3")
            HttpContext.Current.Session.Remove("tempTable")
            HttpContext.Current.Session("exceltable") = unitlisttable

        Catch ex As Exception
            LogSecurityEvent("FillGrid error", ex.Message)
            json = "{""error"":""Data retrieval failed""}"
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        
        Return json
    End Function

    ' SECURITY FIX: Secure logging
    Private Sub LogSecurityEvent(eventType As String, message As String)
        Try
            Dim logMessage As String = String.Format("{0}: {1} - User: {2}, IP: {3}", 
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                eventType, 
                If(HttpContext.Current.Session("userid"), "Unknown"), 
                HttpContext.Current.Request.UserHostAddress)
            
            System.Diagnostics.EventLog.WriteEntry("YTL_Security", logMessage, System.Diagnostics.EventLogEntryType.Warning)
        Catch
            ' Fail silently
        End Try
    End Sub
End Class