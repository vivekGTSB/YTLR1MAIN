Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.Web.Script.Services
Imports System.IO.Compression
Imports AspMap
Imports System.Net

Partial Class OssEta
    Inherits System.Web.UI.Page
    Public cnt As String
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If


        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub
    '<System.Web.Services.WebMethod()> _
    '<ScriptMethod(ResponseFormat:=ResponseFormat.Xml)> _
    'Public Shared Function FillGrid() As String
    '    Dim aa As New ArrayList()
    '    Dim a As ArrayList
    '    Dim json As String = ""

    '    Dim d1 As Double
    '    Try

    '        Dim groupcondition As String = ""
    '        Dim unitstable As New DataTable
    '        unitstable.Columns.Add(New DataColumn("Sno"))
    '        unitstable.Columns.Add(New DataColumn("DN NO"))
    '        unitstable.Columns.Add(New DataColumn("Plate NO"))
    '        unitstable.Columns.Add(New DataColumn("Unit ID"))
    '        unitstable.Columns.Add(New DataColumn("Group Name"))
    '        unitstable.Columns.Add(New DataColumn("Source"))
    '        unitstable.Columns.Add(New DataColumn("Weight Out Time"))
    '        unitstable.Columns.Add(New DataColumn("Ship To Code"))
    '        unitstable.Columns.Add(New DataColumn("Ship To Name"))
    '        unitstable.Columns.Add(New DataColumn("Distance"))
    '        unitstable.Columns.Add(New DataColumn("ETA"))


    '        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))




    '        Dim userid As String = HttpContext.Current.Request.Cookies("userinfo")("userid")
    '        Dim role As String = HttpContext.Current.Request.Cookies("userinfo")("role")
    '        Dim userslist As String = HttpContext.Current.Request.Cookies("userinfo")("userslist")


    '        Dim tankerQuery As String = "select plateno from vehicleTBL"

    '        '  Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

    '        If role = "User" Then
    '            tankerQuery = "select plateno from vehicleTBL where userid='" & userid & "'"
    '        ElseIf role = "SuperUser" Or role = "Operator" Then
    '            tankerQuery = "select plateno from vehicleTBL where userid in (" & userslist & ")"
    '        End If
    '        Dim shipToCodeQuery As String = "select geofencename,shiptocode from geofence where accesstype='1' order by LTRIM(geofencename)"

    '        Dim ds As New DataSet
    '        Dim da As New SqlDataAdapter(tankerQuery, conn)
    '        da.Fill(ds)
    '        Dim platecond As String = ""
    '        For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
    '            If Not IsDBNull(ds.Tables(0).Rows(i)("plateno")) Then
    '                platecond = platecond & "'" & ds.Tables(0).Rows(i)("plateno") & "',"
    '            End If
    '        Next

    '        da = New SqlDataAdapter(shipToCodeQuery, conn)
    '        ds.Clear()
    '        da.Fill(ds)

    '        Dim ShipToNameDict As New Dictionary(Of Integer, String)

    '        For c As Integer = 0 To ds.Tables(0).Rows.Count - 1
    '            Try
    '                If Not ShipToNameDict.ContainsKey(ds.Tables(0).Rows(c)("shiptocode")) Then
    '                    ShipToNameDict.Add(ds.Tables(0).Rows(c)("shiptocode"), ds.Tables(0).Rows(c)("geofencename").ToString().ToUpper())
    '                End If
    '            Catch ex As Exception

    '            End Try
    '        Next
    '        If platecond.Length > 3 Then
    '            platecond = platecond.Substring(0, platecond.Length - 1)
    '            platecond = " and plateno in (" & platecond & ")"
    '        End If


    '        Dim vehicleDict As New Dictionary(Of String, String)


    '        Dim cmd As New SqlCommand("select plateno,groupname from vehicleTBL where plateno <> '' " & platecond, conn)
    '        Dim dr As SqlDataReader

    '        Try
    '            conn.Open()
    '            dr = cmd.ExecuteReader()
    '            While dr.Read()
    '                Try
    '                    vehicleDict.Add(dr("plateno").ToString().Split("_")(0), dr("groupname"))
    '                Catch ex As Exception

    '                End Try
    '            End While

    '        Catch ex As Exception

    '        Finally
    '            conn.Close()
    '        End Try



    '        Dim conn2 = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
    '        Dim cmdT As New SqlCommand("select distinct transporter_name, transporter_id from oss_transporter order by transporter_name ", conn2)
    '        Dim drT As SqlDataReader

    '        Dim TransNameDict As New Dictionary(Of String, Integer)
    '        Try
    '            conn.Open()
    '            drT = cmdT.ExecuteReader()
    '            While drT.Read()
    '                Try
    '                    TransNameDict.Add(drT("transporter_name").ToString().ToUpper(), drT("transporter_id"))
    '                Catch ex As Exception

    '                End Try
    '            End While

    '        Catch ex As Exception

    '        Finally
    '            conn2.Close()
    '        End Try









    '        'Dim dr As SqlDataReader
    '        'Dim cmd As New SqlCommand()


    '        Dim bdt As String = Now.Date.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss")
    '        ' bdt = bdt & " 00:00:00"
    '        Dim edt As String = Now.Date.ToString("yyyy/MM/dd HH:mm:ss")
    '        'edt = edt & " 23:59:59"

    '        cmd.CommandText = "select * from oss_patch_out where weight_outtime between '" & bdt & "' and '" & edt & "' and status='3'"
    '        cmd.Connection = conn
    '        Dim r As DataRow
    '        Try
    '            conn.Open()
    '            dr = cmd.ExecuteReader()
    '            Dim i As Int32 = 1
    '            While dr.Read()
    '                If TransNameDict.ContainsKey(dr("transporter").ToString().ToUpper()) Then
    '                    r = unitstable.NewRow

    '                    r(0) = i.ToString()

    '                    r(1) = dr("dn_no")

    '                    r(1) = TransNameDict.Item(dr("transporter").ToString().ToUpper())
    '                    r(2) = dr("plateno")
    '                    r(3) = dr("unitid")

    '                    r(4) = ""

    '                    If vehicleDict.ContainsKey(dr("plateno")) Then
    '                        r(4) = vehicleDict(dr("plateno"))
    '                    End If

    '                    r(5) = dr("source_supply")


    '                    Dim p = DateTime.Parse(dr("weight_outtime")).ToString("yyyy/MM/dd  HH:mm:ss")
    '                    p = p.Replace("-", "/")

    '                    r(6) = p
    '                    r(7) = dr("destination_siteid")


    '                    If ShipToNameDict.ContainsKey(dr("destination_siteid")) Then
    '                        r(8) = ShipToNameDict.Item(dr("destination_siteid")).ToUpper()
    '                    Else
    '                        r(8) = "--"
    '                    End If

    '                    If (IsDBNull(dr("distance")) = False) Then
    '                        If (dr("distance") <> "0") Then
    '                            d1 = dr("distance")
    '                            r(9) = d1.ToString("0.0")
    '                        End If

    '                    Else
    '                        r(9) = 0
    '                    End If

    '                    If IsDBNull(dr("est_arrivaltime")) Then
    '                        r(10) = "--"
    '                    Else
    '                        r(10) = Convert.ToDateTime(dr("est_arrivaltime")).ToString("yyyy/MM/dd HH:mm:ss")
    '                    End If










    '                    unitstable.Rows.Add(r)

    '                    i = i + 1


    '                End If


    '            End While

    '        Catch ex As Exception

    '        Finally
    '            conn.Close()
    '        End Try

    '        If unitstable.Rows.Count = 0 Then
    '            r = unitstable.NewRow
    '            r(0) = "--"
    '            r(1) = "--"
    '            r(2) = "--"
    '            r(3) = "--"
    '            r(4) = "--"
    '            r(5) = "--"
    '            r(6) = "--"
    '            r(7) = "--"
    '            r(8) = "--"
    '            r(9) = "--"
    '            r(10) = "--"

    '            unitstable.Rows.Add(r)
    '        End If



    '        For j As Integer = 0 To unitstable.Rows.Count - 1
    '            Try
    '                a = New ArrayList()

    '                a.Add(unitstable.DefaultView.Item(j)(0))
    '                a.Add(unitstable.DefaultView.Item(j)(1))
    '                a.Add(unitstable.DefaultView.Item(j)(2))
    '                a.Add(unitstable.DefaultView.Item(j)(3))
    '                a.Add(unitstable.DefaultView.Item(j)(4))
    '                a.Add(unitstable.DefaultView.Item(j)(5))
    '                a.Add(unitstable.DefaultView.Item(j)(6))
    '                a.Add(unitstable.DefaultView.Item(j)(7))
    '                a.Add(unitstable.DefaultView.Item(j)(8))
    '                a.Add(unitstable.DefaultView.Item(j)(9))
    '                a.Add(unitstable.DefaultView.Item(j)(10))


    '                aa.Add(a)
    '            Catch ex As Exception

    '            End Try
    '        Next



    '        json = JsonConvert.SerializeObject(aa, Formatting.None)

    '    Catch ex As SystemException

    '    End Try
    '    Return json

    'End Function
End Class
