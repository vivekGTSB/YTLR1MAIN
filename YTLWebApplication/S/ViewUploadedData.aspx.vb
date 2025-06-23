Imports System
Imports System.Data
Imports System.Data.SqlClient
Partial Class ViewUploadedData
    Inherits System.Web.UI.Page

    Private Sub ViewUploadedData_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Dim Patch_No As String = Request.QueryString("p")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmd As New SqlCommand
            Try

                cmd = New SqlCommand("select E.SubmittedBy,E.QRCode,E.FileData, O.Dn_No,O.destination_sitename  from  (select * from OSS_EXTENSION_TABLE where patch_no=@p ) E left outer join (select * from oss_patch_out where patch_no=@p ) O on e.Patch_no=E.Patch_No", conn)
                cmd.Parameters.AddWithValue("@p", Patch_No)
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                While dr.Read()
                    lblIC.Text = dr("SubmittedBy").ToString()
                    lblQR.Text = dr("QrCode").ToString()
                    Dim data As Byte() = DirectCast(dr("FileData"), Byte())
                    imgData.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(data)
                    Try
                        lblDnno.Text = dr("Dn_No").ToString()
                    Catch ex As Exception
                        ' lblDnno.Text = ex.Message
                    End Try

                    Try
                        lblShipTo.Text = dr("destination_sitename").ToString()
                    Catch ex As Exception
                        ' lblShipTo.Text = ex.Message
                    End Try

                End While

            Catch ex As Exception
                ' Response.Write(ex.Message)
            Finally
                conn.Close()
            End Try
        Catch ex As Exception

        End Try
    End Sub
End Class
