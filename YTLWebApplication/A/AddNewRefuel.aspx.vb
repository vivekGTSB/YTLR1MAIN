Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.DataRow

'Imports System.Web.Script.Services
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports AspMap
Partial Class AddNewRefuel
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Try
            Response.Write(GetJson())

        Catch ex As Exception

        End Try


    End Sub
    Protected Function GetJson() As String



        Dim userid As String = Request.QueryString("userid")
        Dim plateno As String = Request.QueryString("plateno")
        Dim timestamp As String = Request.QueryString("timestamp")
        Dim ltrs As String = Request.QueryString("ltrs")
        Dim fc As String = Request.QueryString("fc")
        Dim receiptno As String = Request.QueryString("receiptno")

        Dim result As Int16
        Dim price As Double
        Dim cmd As SqlCommand
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim cmd1 As SqlCommand
            Dim dr As SqlDataReader
            Dim fuelcost As Double
            If fc <> "" Then
                fuelcost = Convert.ToDouble(fc)
            Else
                Dim dPrice As New DataTable
                dPrice = fuelPrice(userid)
                Dim drPrice As DataRow() = dPrice.Select("StartDate <= #" & Convert.ToDateTime(timestamp).ToString("yyyy/MM/dd HH:mm:ss") & "# And EndDate >= #" & Convert.ToDateTime(timestamp).ToString("yyyy/MM/dd HH:mm:ss") & "#")
                For pr As Integer = 0 To drPrice.Length - 1
                    If (Convert.ToDateTime(drPrice(pr)(0)) <= Convert.ToDateTime(timestamp)) And (Convert.ToDateTime(drPrice(pr)(1)) >= Convert.ToDateTime(timestamp)) Then
                        fuelcost = CDbl(drPrice(pr)(2))
                        Exit For
                    End If
                Next
                'For Each row In drPrice
                '    If (Convert.ToDateTime(row(0)) <= Convert.ToDateTime(timestamp)) And (Convert.ToDateTime(row(1)) >= Convert.ToDateTime(timestamp)) Then
                '        fuelcost = CDbl(row(2))
                '        Exit For
                '    End If
                'Next
            End If
            cmd1 = New SqlCommand("select * from fuel where plateno='" & plateno & "' and timestamp='" & timestamp & "'", conn)
            conn.Open()
            dr = cmd1.ExecuteReader()
            If dr.Read() Then
                cmd = New SqlCommand("update fuel set liters='" & ltrs & "',receiptno='" & receiptno & "',cost='" & (Convert.ToDouble(ltrs) * fuelcost).ToString("0.00") & "' where plateno='" & plateno & "' and timestamp='" & Convert.ToDateTime(timestamp).ToString("yyyy/MM/dd HH:mm:ss") & "'", conn)
            Else
                cmd = New SqlCommand("insert into fuel(userid,plateno,timestamp,stationcode,fueltype,liters,cost,receiptno)values('" & userid & "','" & plateno & "','" & timestamp & "','','Diesel','" & ltrs & "','" & (Convert.ToDouble(ltrs) * fuelcost).ToString("0.00") & "','" & receiptno & "')", conn)
            End If
            result = cmd.ExecuteNonQuery()
            If result >= 1 Then
                price = (Convert.ToDouble(ltrs) * fuelcost).ToString("0.00")
            End If
            conn.Close()


        Catch ex As Exception
            HttpContext.Current.Response.Write(ex.Message)
        End Try
        Return price
    End Function
    Protected Function fuelPrice(ByVal userid As String) As DataTable
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim dsFuelPrice As New DataSet
        Dim da As SqlDataAdapter
        da = New SqlDataAdapter("select * from fuel_price where countrycode=(select countrycode from userTBL where userid='" & userid & "') order by startdate desc ", conn)
        Dim priceTable As New DataTable
        Try
            da.Fill(dsFuelPrice)
            priceTable.Columns.Add(New DataColumn("StartDate", GetType(DateTime)))
            priceTable.Columns.Add(New DataColumn("EndDate", GetType(DateTime)))
            priceTable.Columns.Add(New DataColumn("FuelPrice"))
            Dim pRow As DataRow
            If dsFuelPrice.Tables(0).Rows.Count > 0 Then
                For i As Int32 = 0 To dsFuelPrice.Tables(0).Rows.Count - 1
                    pRow = priceTable.NewRow
                    pRow(0) = dsFuelPrice.Tables(0).Rows(i)("startdate")
                    pRow(1) = dsFuelPrice.Tables(0).Rows(i)("enddate")
                    pRow(2) = dsFuelPrice.Tables(0).Rows(i)("fuelprice")
                    priceTable.Rows.Add(pRow)
                Next
            Else
                pRow = priceTable.NewRow
                pRow(0) = Now.ToString("yyyy/MM/dd")
                pRow(1) = Now.ToString("yyyy/MM/dd")
                pRow(2) = 0
                priceTable.Rows.Add(pRow)
            End If

        Catch

        End Try
        Return priceTable
    End Function
End Class
