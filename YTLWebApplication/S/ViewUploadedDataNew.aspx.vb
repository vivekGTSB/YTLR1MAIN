Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Drawing




Partial Class ViewUploadedDataNew
    Inherits System.Web.UI.Page

    Private Sub ViewUploadedData_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Dim Patch_No As String = Request.QueryString("p")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmd As New SqlCommand
            Try

                cmd = New SqlCommand("select O.dn_driver,O.plateno,E.DivStatus,E.SubmittedBy,E.QRCode,E.FileData, O.Dn_No,O.destination_sitename  from  (select * from OSS_EXTENSION_TABLE where patch_no=@p ) E left outer join (select * from oss_patch_out where patch_no=@p ) O on e.Patch_no=E.Patch_No", conn)
                cmd.Parameters.AddWithValue("@p", Patch_No)
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                While dr.Read()
                    lblIC.Text = dr("SubmittedBy").ToString()
                    lblQR.Text = dr("QrCode").ToString()
                    Dim data As Byte() = DirectCast(dr("FileData"), Byte())
                    imgData.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(data)
                    Session.Remove("myImage")
                    Session("myImage") = Convert.ToBase64String(data)
                    ' Dim img As Bitmap = ToBlackAndWhite(New Bitmap(New MemoryStream(data)))
                    Dim img As Bitmap = ConvertToGrayscale(New Bitmap(New MemoryStream(data)))
                    imgBW.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(ConvertToByteArray(img))
                    Try
                        lblDnno.Text = dr("Dn_No").ToString()
                    Catch ex As Exception
                        ' lblDnno.Text = ex.Message
                    End Try


                    Try
                        lblPlateno.Text = dr("plateno").ToString()
                    Catch ex As Exception
                        ' lblDnno.Text = ex.Message
                    End Try

                    Try
                        lblDriverName.Text = dr("dn_driver").ToString()
                    Catch ex As Exception
                        ' lblDnno.Text = ex.Message
                    End Try

                    Try
                        lblShipTo.Text = dr("destination_sitename").ToString()
                    Catch ex As Exception
                        ' lblShipTo.Text = ex.Message
                    End Try
                    Try
                        IsDiversion.SelectedValue = Convert.ToInt32(dr("divstatus"))
                    Catch ex As Exception

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
    Public Shared Function ConvertToByteArray(ByVal value As Bitmap) As Byte()
        Dim bitmapBytes As Byte()
        Using stream As New System.IO.MemoryStream
            value.Save(stream, value.RawFormat)
            bitmapBytes = stream.ToArray
        End Using
        Return bitmapBytes
    End Function
    Public Function ToBlackAndWhite(ByVal bmp As Bitmap) As Bitmap
        Dim x As Integer
        Dim y As Integer
        Dim gem As Integer
        Dim r, g, b As Integer
        Dim col As Color

        For x = 0 To bmp.Width - 1
            For y = 0 To bmp.Height - 1

                col = bmp.GetPixel(x, y)
                r = col.R
                g = col.G
                b = col.B
                gem = (r + g + b) / 3

                If gem > 128 Then
                    bmp.SetPixel(x, y, Color.White)
                Else
                    bmp.SetPixel(x, y, Color.Gray)
                End If

            Next y
        Next x

        Return bmp
    End Function

    Public Shared Function ConvertToGrayscale(ByVal bmp As Bitmap) As Bitmap
        Dim grayscale As New Imaging.ColorMatrix(New Single()() _
        { _
            New Single() {0.299, 0.299, 0.299, 0, 0}, _
            New Single() {0.587, 0.587, 0.587, 0, 0}, _
            New Single() {0.114, 0.114, 0.114, 0, 0}, _
            New Single() {0, 0, 0, 1, 0}, _
            New Single() {0, 0, 0, 0, 1} _
        })
        
        Dim imgattr As New Imaging.ImageAttributes()
        imgattr.SetColorMatrix(grayscale)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.DrawImage(bmp, New Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imgattr)
        End Using
        Return bmp
    End Function

End Class
