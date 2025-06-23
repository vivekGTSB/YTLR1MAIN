Imports System.Data.SqlClient
Partial Class ViewDetails
    Inherits System.Web.UI.Page
    Public sb As StringBuilder 
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim plateno As String = Request.QueryString("pno")
        Dim bdt As String = Request.QueryString("bdt")
        Dim edt As String = Request.QueryString("edt")
        Dim gid As String = Request.QueryString("gid")
      
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("sp_GetGeofencereportprivate", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@plateno", plateno)
            cmd.Parameters.AddWithValue("@gid", gid)
            cmd.Parameters.AddWithValue("@frmdtae", bdt)
            cmd.Parameters.AddWithValue("@todtae", edt)
            cmd.Parameters.AddWithValue("@userid", "")
          
            Dim da As New SqlDataAdapter(cmd)
            Dim ds As New DataSet
            da.Fill(ds)

            sb = New StringBuilder()
            sb.Append("<thead align='left' ><tr  ><td colspan='5' style='color:rgb(35, 135, 228)' >Plate No: " & plateno & "</td></tr>")
            sb.Append("<tr  ><td colspan='5' style='color:rgb(35, 135, 228)' >From: " & DateTime.Parse(bdt).ToString("yyyy/MM/dd HH:mm:ss") & " To: " & DateTime.Parse(edt).ToString("yyyy/MM/dd HH:mm:ss") & " </td></tr>")
            sb.Append("<tr ><th scope='col' style='width: 30px;'>S No</th><th  scope='col' style='width: 80px;'>Plateno</th><th   scope='col' style='width: 100px; '>Username</th><th  scope='col' style='width: 100px;'>Group Name</th><th   scope='col' style='width: 200px; '>Geofence Name</th><th   scope='col' style='width: 130px; '>In Time</th><th   scope='col' style='width: 130px; '>Out Time</th><th   scope='col' style='width: 30px; '>Mins</th></tr></thead>")
            sb.Append("<tbody>")
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                sb.Append("<tr ><td>" & i + 1 & "</td>")
                sb.Append("<td>" & ds.Tables(0).Rows(i)("plateno") & "</td>")
                sb.Append("<td>" & ds.Tables(0).Rows(i)("username") & "</td>")
                sb.Append("<td>" & ds.Tables(0).Rows(i)("groupname") & "</td>")
                sb.Append("<td>" & ds.Tables(0).Rows(i)("geofencename") & "</td>")
                sb.Append("<td>" & Convert.ToDateTime(ds.Tables(0).Rows(i)("intime")).ToString("yyyy/MM/dd HH:mm:ss") & "</td>")


                If IsDBNull(ds.Tables(0).Rows(i)("outtime")) Then
                    sb.Append("<td></td>")
                Else
                    sb.Append("<td>" & Convert.ToDateTime(ds.Tables(0).Rows(i)("outtime")).ToString("yyyy/MM/dd HH:mm:ss") & "</td>")
                End If


                sb.Append("<td>" & ds.Tables(0).Rows(i)("duration") & "</td>")
            Next
            sb.Append("</tbody>")
            sb.Append("<tfoot align='left'><tr ><th scope='col' style='width: 30px;'>S No</th><th  scope='col' style='width: 80px;'>Plateno</th><th   scope='col' style='width: 100px; '>Username</th><th  scope='col' style='width: 100px;'>Group Name</th><th   scope='col' style='width: 200px; '>Geofence Name</th><th   scope='col' style='width: 130px; '>In Time</th><th   scope='col' style='width: 130px; '>Out Time</th><th   scope='col' style='width: 30px; '>Mins</th></tr></tfoot>")


        Catch ex As Exception
            Response.Write(ex.Message)
        End Try


    End Sub
End Class
