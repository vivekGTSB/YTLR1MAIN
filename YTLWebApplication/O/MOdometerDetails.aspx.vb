Imports System.Data.SqlClient
Imports ASPNetMultiLanguage

Partial Class MOdometerDetails
    Inherits System.Web.UI.Page
    Public sb As StringBuilder
    Public Odometer As String
    Public Odometerdate As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim plateno As String = Request.QueryString("plateno")
        Odometer = Request.QueryString("odo")
        Odometerdate = Request.QueryString("date")
        Dim totalmilage As Double = 0
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim dr As SqlDataReader
        Dim cmd As SqlCommand
        Dim dt As New DataTable
        Dim modo As New Odometer
        Dim absolute As Double = Convert.ToDouble(Odometer)
        sb = New StringBuilder()
        Try
            dt = modo.ProcessData(plateno, Convert.ToDateTime(Odometerdate).ToString("yyyy/MM/dd HH:mm:ss"), Convert.ToDateTime(Odometerdate).ToString("yyyy/MM/dd") & " 23:59:59")
            If absolute = 0 And Odometerdate <> "2014/01/01 00:00:00" Then
                absolute = absolute + GetprocessstartOdoemter(plateno) 
            End If 
            If dt.Rows.Count > 0 Then 
                sb.Append("<tr><td align='left'>" & plateno & "</td>")
                sb.Append("<td align='left'>" & Convert.ToDateTime(dt.Rows(0)("Startdate")).ToString("yyyy/MM/dd  HH:mm:ss") & "</td>")
                sb.Append("<td align='right'>" & Convert.ToDouble(dt.Rows(0)("fromodemeter")).ToString("0.00") & "</td>")
                sb.Append("<td  align='right'>" & Convert.ToDouble(dt.Rows(0)("toodemeter")).ToString("0.00") & "</td>")
                sb.Append("<td  align='right'>" & Convert.ToDouble(dt.Rows(0)("net")).ToString("0.00") & "</td>")
                absolute = absolute + Convert.ToDouble(dt.Rows(0)("Absolute")).ToString("0.00")
                sb.Append("<td  align='right'>" & absolute & "</td></tr>")
                totalmilage = totalmilage + Convert.ToDouble(dt.Rows(0)("net"))
            Else
                sb.Append("<tr><td align='left' colspan='6'>" & plateno & "</td>")
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

        Try
            cmd = New SqlCommand("select plateno ,timestamp ,beforeodometer ,afterodometer ,milage ,absolute  from dbo.vehicle_odometer where timestamp >'" & Odometerdate & "'  and plateno ='" & plateno & "'", conn)
            conn.Open()
            dr = cmd.ExecuteReader()



            While (dr.Read())
                absolute = absolute + Convert.ToDouble(dr("milage")).ToString("0.00")
                sb.Append("<tr><td align='left'>" & dr("plateno") & "</td>")
                sb.Append("<td align='left'>" & Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd") & "</td>")
                sb.Append("<td align='right'>" & Convert.ToDouble(dr("beforeodometer")).ToString("0.00") & "</td>")
                sb.Append("<td  align='right'>" & Convert.ToDouble(dr("afterodometer")).ToString("0.00") & "</td>")
                sb.Append("<td  align='right'>" & Convert.ToDouble(dr("milage")).ToString("0.00") & "</td>")
                sb.Append("<td  align='right'>" & absolute.ToString("0.00") & "</td></tr>")
                totalmilage = totalmilage + Convert.ToDouble(dr("milage"))
            End While
            sb.Append("<tfoot><tr><th></th><th></th><th></th><th >TOTAL:</th><th  align='right'>" & Convert.ToDouble(totalmilage).ToString("0.00") & "</th><th  align='right'>" & Convert.ToDouble(absolute).ToString("0.00") & "</th></tr></tfoot>")
            conn.Close()
        Catch ex As Exception

        Finally
            conn.Close()
        End Try
    End Sub

    Public Function GetprocessstartOdoemter(ByVal plateno As String) As Double
        Dim resultodo As Double
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim drk As SqlDataReader
        Try
            Dim cmdm As SqlCommand = New SqlCommand("select top 1 * from vehicle_odometer  where plateno ='" & plateno & "'", conn)
            conn.Open()
            drk = cmdm.ExecuteReader()

            If drk.Read() Then
                resultodo = Convert.ToDouble(drk("beforeodometer"))
            Else
                resultodo = 0
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            drk.Close()
            conn.Close()
        End Try
        Return resultodo
    End Function
End Class
