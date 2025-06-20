Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Collections

Public Class GetOssData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim plateno As String = Request.QueryString("plateno")
        If plateno <> "" Then
            plateno = plateno.Trim()
        End If
        Dim aa As New ArrayList
        Dim a As ArrayList
        Dim t As New DataTable

        t.Columns.Add(New DataColumn("SNo"))
        t.Columns.Add(New DataColumn("Source"))
        t.Columns.Add(New DataColumn("DN ID"))
        t.Columns.Add(New DataColumn("Product Type"))
        t.Columns.Add(New DataColumn("Area Code"))
        t.Columns.Add(New DataColumn("ShiptoCode"))
        t.Columns.Add(New DataColumn("ShiptoName"))
        t.Columns.Add(New DataColumn("WOT"))
        t.Columns.Add(New DataColumn("ATA"))


        Dim edt As DateTime = Now.ToString("yyyy/MM/dd HH:mm:ss")
        Dim bdt As DateTime = edt.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss")
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim cmd As New SqlCommand("select *,isnull(area_code_name,'-') as areaname,isnull(area_code,'0') as areacode from oss_patch_out where weight_outtime between  '" & bdt.ToString("yyyy/MM/dd HH:mm:ss") & "' and '" & edt.ToString("yyyy/MM/dd HH:mm:ss") & "' and plateno='" & plateno & "'   order by weight_outtime", conn2)
        Dim drI As SqlDataReader

        Dim r As DataRow
        Dim ii As Integer = 1

        Dim shipToCodeQuery As String = "select geofencename,shiptocode from geofence where accesstype='1' order by LTRIM(geofencename)"

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter(shipToCodeQuery, conn)
        ds.Clear()
        da.Fill(ds)
        Dim ShipToNameDict As New Dictionary(Of Integer, String)
        For c As Integer = 0 To ds.Tables(0).Rows.Count - 1
            Try
                If Not ShipToNameDict.ContainsKey(ds.Tables(0).Rows(c)("shiptocode")) Then
                    ShipToNameDict.Add(ds.Tables(0).Rows(c)("shiptocode"), ds.Tables(0).Rows(c)("geofencename").ToString().ToUpper())
                End If
            Catch ex As Exception

            End Try
        Next

        Try
            conn2.Open()
            drI = cmd.ExecuteReader()
            While drI.Read()
                r = t.NewRow()
                r(0) = ii.ToString()
                r(1) = drI("source_supply").ToString().ToUpper()
                r(2) = drI("dn_id").ToString().ToUpper()
                r(3) = drI("productname").ToString().ToUpper()
                r(4) = drI("areaname").ToString().ToUpper() & " - " & drI("areacode").ToString().ToUpper()
                r(5) = drI("destination_siteid").ToString().ToUpper()

                If IsDBNull(drI("destination_sitename")) Then
                    If ShipToNameDict.ContainsKey(drI("destination_siteid")) Then
                        r(6) = ShipToNameDict.Item(drI("destination_siteid")).ToUpper()
                    Else
                        r(6) = "--"
                    End If
                Else
                    If drI("status").ToString() <> "2" Then
                        r(6) = "<span style=''>" & drI("destination_sitename").ToString.ToUpper() & "</span>"
                    Else
                        r(6) = "<span style='color:red;'>" & drI("destination_sitename").ToString.ToUpper() & "</span>"
                    End If
                End If

                If IsDBNull(drI("weight_outtime")) = False Then
                    r(7) = DateTime.Parse(drI("weight_outtime")).ToString("yyyy/MM/dd HH:mm:ss")
                Else
                    r(7) = "--"
                End If
                If IsDBNull(drI("ata_datetime")) = False Then
                    r(8) = DateTime.Parse(drI("ata_datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                Else
                    r(8) = "--"
                End If
                t.Rows.Add(r)
                ii += 1
            End While

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            conn2.Close()
        End Try




        Dim message As String = ""
        For c As Integer = 0 To t.Rows.Count - 1
            a = New ArrayList()
            a.Add(t.DefaultView.Item(c)(0))
            a.Add(t.DefaultView.Item(c)(1))
            a.Add(t.DefaultView.Item(c)(2))
            a.Add(t.DefaultView.Item(c)(3))
            a.Add(t.DefaultView.Item(c)(4))
            a.Add(t.DefaultView.Item(c)(5))
            a.Add(t.DefaultView.Item(c)(6))
            a.Add(t.DefaultView.Item(c)(7))
            a.Add(t.DefaultView.Item(c)(8))
            aa.Add(a)
        Next
        If t.Rows.Count = 0 Then
            a = New ArrayList()
            a.Add("--")
            a.Add("--")
            a.Add("--")
            a.Add("--")
            a.Add("--")
            a.Add("--")
            a.Add("--")
            a.Add("--")
            a.Add("--")
            aa.Add(a)
        End If
        Dim jss As New Newtonsoft.Json.JsonSerializer()
        Dim json As String = ""
        json = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
        Response.Write(json)
        Response.ContentType = "text/plain"

    End Sub

End Class