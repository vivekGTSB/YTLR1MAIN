Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Partial Class GetSoldToManagement
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Dim op As String = Request.QueryString("op")
            Select Case op
                Case "0" :
                    Response.Write(FillGrid())
                Case "1" :
                    Dim pod As String = Request.QueryString("prevpid")
                    Dim cid As String = Request.QueryString("pid")
                    Dim cname As String = Request.QueryString("pname")
                    Response.Write(AddUpdateCustomer(pod, cid, cname))
            End Select

        Catch ex As Exception

        End Try
    End Sub
    Public Function FillGrid() As String
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim json As String = Nothing
        Try
            Dim aa As New ArrayList
            Dim a As ArrayList
            cmd.Connection = conn
            cmd.CommandText = "select id,customerid,customername,timestamp from EC_soldto"
            Dim soldtotable As New DataTable

            soldtotable.Columns.Add(New DataColumn("No"))
            soldtotable.Columns.Add(New DataColumn("Customer ID"))
            soldtotable.Columns.Add(New DataColumn("Customer Name"))
            soldtotable.Columns.Add(New DataColumn("Timestamp"))
            soldtotable.Columns.Add(New DataColumn("id"))
            Dim r As DataRow
            conn.Open()
            dr = cmd.ExecuteReader()
            Dim i As Integer = 1
            While dr.Read()
                r = soldtotable.NewRow()
                r(0) = i
                r(1) = dr("customerid")
                r(2) = dr("customername").ToString().ToUpper()
                r(3) = Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                r(4) = dr("id")
                i = i + 1
                soldtotable.Rows.Add(r)
            End While
            dr.Close()
            If soldtotable.Rows.Count = 0 Then
                r = soldtotable.NewRow()
                r(0) = "-"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                soldtotable.Rows.Add(r)
            End If
            For i = 0 To soldtotable.Rows.Count - 1
                a = New ArrayList
                a.Add(soldtotable.Rows(i)(0))
                a.Add(soldtotable.Rows(i)(2))
                a.Add(soldtotable.Rows(i)(1))
                a.Add(soldtotable.Rows(i)(3))
                a.Add(soldtotable.Rows(i)(4))
                aa.Add(a)
            Next
            json = JsonConvert.SerializeObject(aa, Formatting.None)
            ' Literal41--loac
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session.Remove("exceltable3")
            soldtotable.Columns.RemoveAt(4)
            HttpContext.Current.Session("exceltable") = soldtotable
            HttpContext.Current.Session.Remove("tempTable")
        Catch ex As Exception
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return json
    End Function


    Public Function AddUpdateCustomer(ByVal pid As String, ByVal cid As String, ByVal cname As String) As String
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As New SqlCommand

        Dim json As String = Nothing
        Try
            cmd.Connection = conn
            If pid = "0" Then
                cmd.CommandText = "insert into EC_soldto (customerid,customername) values (@cid,@cname)"
            Else
                cmd.CommandText = "update EC_soldto set customerid=@cid,customername=@cname where id=@pid"
            End If
            cmd.Parameters.AddWithValue("@cid", cid)
            cmd.Parameters.AddWithValue("@cname", cname)
            cmd.Parameters.AddWithValue("@pid", pid)
            conn.Open()
            If cmd.ExecuteNonQuery() > 0 Then
                json = "1"
            Else
                json = "0"
            End If
        Catch ex As Exception
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return json
    End Function
End Class

