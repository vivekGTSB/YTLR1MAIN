Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Partial Class GetClientSoldToManagement
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Dim op As String = Request.QueryString("op")
            Select Case op
                Case "0"
                    Response.Write(FillGrid())
                Case "1"
                    Dim pod As String = Request.QueryString("prevpid")
                    Dim cname As String = Request.QueryString("cid")
                    Dim cpwd As String = Request.QueryString("cpwd")
                    Dim cemail As String = Request.QueryString("email")
                    Dim cmobie As String = Request.QueryString("mobile")
                    Dim soldto As String = Request.QueryString("soldto")

                    Response.Write(AddUpdateClient(pod, cname, cpwd, cemail, cmobie, soldto))
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
            cmd.CommandText = "select cuserid,CUsername,pwd,isnull(emaillist,'-') as emaillist,isnull( mobileList,'-') as mobileList   ,isnull ( t2.customername,'-') as soldto,status,soldtoid from EC_client_user t1 left outer join ec_soldto t2 on t1.soldtoid=t2.customerid where t1.soldtoid<>'0'"
            conn.Open()

            Dim customertable As New DataTable
            customertable.Columns.Add(New DataColumn("sno"))
            customertable.Columns.Add(New DataColumn("CUsername"))
            customertable.Columns.Add(New DataColumn("CPwd"))
            customertable.Columns.Add(New DataColumn("emaillist"))
            customertable.Columns.Add(New DataColumn("mobileList"))
            customertable.Columns.Add(New DataColumn("Customer Name"))
            customertable.Columns.Add(New DataColumn("status"))
            customertable.Columns.Add(New DataColumn("cuserid"))
            customertable.Columns.Add(New DataColumn("soldtoid"))
            Dim r As DataRow
            Dim i As Integer = 1
            dr = cmd.ExecuteReader()
            While dr.Read()
                r = customertable.NewRow()
                r(0) = i
                r(1) = dr("CUsername")
                r(2) = dr("pwd")
                r(3) = dr("emaillist")
                r(4) = dr("mobileList")
                r(5) = dr("soldto")
                r(6) = dr("status")
                r(7) = dr("cuserid")
                r(8) = dr("soldtoid")
                customertable.Rows.Add(r)
                i = i + 1

            End While
            If customertable.Rows.Count = 0 Then
                r = customertable.NewRow()
                r(0) = "-"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                r(5) = "-"
                r(6) = "-"
                r(7) = "-"
                r(8) = "-"
                customertable.Rows.Add(r)
            End If

            For i = 0 To customertable.Rows.Count - 1
                a = New ArrayList
                a.Add(customertable.Rows(i)(0))
                a.Add(customertable.Rows(i)(0))
                a.Add(customertable.Rows(i)(1))
                a.Add(customertable.Rows(i)(2))
                a.Add(customertable.Rows(i)(3))
                a.Add(customertable.Rows(i)(4))
                a.Add(customertable.Rows(i)(5))
                a.Add(customertable.Rows(i)(6))
                a.Add(customertable.Rows(i)(7))
                a.Add(customertable.Rows(i)(8))
                aa.Add(a)
            Next

            json = JsonConvert.SerializeObject(aa, Formatting.None)
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session.Remove("exceltable3")
            customertable.Columns.RemoveAt(6)
            customertable.Columns.RemoveAt(6)
            customertable.Columns.RemoveAt(6)
            HttpContext.Current.Session("exceltable") = customertable
            HttpContext.Current.Session.Remove("tempTable")

        Catch ex As Exception
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return json
    End Function


    Public Function AddUpdateClient(ByVal id As String, ByVal cname As String, ByVal cpwd As String, ByVal email As String, ByVal mobile As String, ByVal soldto As String) As String
        Dim json As String = Nothing
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As New SqlCommand

        Try
            cmd.Connection = conn
            conn.Open()

            cmd.CommandText = "select * from EC_client_user where Cusername='" + cname + "'"
            Dim dr As SqlDataReader = cmd.ExecuteReader()



            If (dr.HasRows() And id = "0") Then
                json = "2"
            Else
                If Not dr.IsClosed() Then
                    dr.Close()
                End If

                If id = "0" Then
                    cmd.CommandText = "insert into EC_client_user (CUsername,pwd,emaillist,mobileList,soldtoid) values (@CUsername,@pwd,@emaillist,@mobileList,@soldtoid)"
                Else
                    cmd.CommandText = "update EC_client_user set CUsername=@CUsername,pwd=@pwd,emaillist=@emaillist,mobileList=@mobileList,soldtoid=@soldtoid where cuserid=@id"
                End If
                cmd.Parameters.AddWithValue("@CUsername", cname)
                cmd.Parameters.AddWithValue("@pwd", cpwd)
                cmd.Parameters.AddWithValue("@emaillist", email)
                cmd.Parameters.AddWithValue("@mobileList", mobile)
                cmd.Parameters.AddWithValue("@soldtoid", soldto)
                cmd.Parameters.AddWithValue("@id", id)


                If cmd.ExecuteNonQuery() > 0 Then
                    json = "1"
                Else
                    json = "0"
                End If
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
