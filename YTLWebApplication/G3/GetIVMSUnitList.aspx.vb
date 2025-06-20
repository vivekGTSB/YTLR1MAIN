Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Partial Class GetIVMSUnitList
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Dim version As String = Request.QueryString("v")
            Response.Write(FillGrid(version))
            Response.ContentType = "text/json"

        Catch ex As Exception

        End Try
    End Sub
    Public Function FillGrid(ByVal version As String) As String
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim json As String = Nothing
        Try
            Dim aa As New ArrayList
            Dim a As ArrayList
            cmd.Connection = conn
            If version = "All" Then
                cmd.CommandText = "select t2.username,t1.plateno,t3.versionid,t3.unitid,t3.simno,isnull( t4.groupname,'-') as groupname,dbo.GetTrailerStatus(t1.plateno) as status from vehicleTBL t1 left outer join  userTBL t2 on t1.userid=t2.userid inner join unitLST t3 on t1.unitid =t3.unitid  left outer join vehicle_group t4 on t1.groupid=t4.groupid  where t2.role ='User' and t2.companyname not like 'Gussmann%'"
            Else
                cmd.CommandText = "select t2.username,t1.plateno,t3.versionid,t3.unitid,t3.simno,isnull( t4.groupname,'-') as groupname,dbo.GetTrailerStatus(t1.plateno) as status from vehicleTBL t1 left outer join  userTBL t2 on t1.userid=t2.userid inner join unitLST t3 on t1.unitid =t3.unitid  left outer join vehicle_group t4 on t1.groupid=t4.groupid  where t2.role ='User' and t2.companyname not like 'Gussmann%' and t3.versionid=@vid"
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
                r(1) = dr("username")
                r(2) = dr("plateno").ToString().ToUpper()
                r(3) = dr("versionid")
                r(4) = dr("unitid")
                r(5) = dr("simno")
                r(6) = dr("groupname")
                If dr("status") = "0" Then
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
            ' Literal41--loac
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session.Remove("exceltable3")
            HttpContext.Current.Session.Remove("tempTable")
            HttpContext.Current.Session("exceltable") = unitlisttable

        Catch ex As Exception
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return json
    End Function



End Class

