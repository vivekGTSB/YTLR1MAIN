Imports System.Data.SqlClient
Imports System.IO.Stream
Imports System.IO
Imports System.Drawing.Imaging.ImageFormat
Imports System.Drawing
Imports System.IO.Path
Imports System.Drawing.Imaging
Namespace AVLS
    Partial Class AddPOI
        Inherits System.Web.UI.Page
        Public backpage As String = "PrivatePOIManagement.aspx"
        Public errormessage As String
        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Try

                If Request.Cookies("userinfo") Is Nothing Then
                    Response.Redirect("Login.aspx")
                End If

                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")

                Dim suserid As String = Request.QueryString("userid")

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select userid,username from userTBL where role='User' order by username", conn)
                Dim dr As SqlDataReader

                If role = "User" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ") order by username", conn)
                End If

                conn.Open()
                dr = cmd.ExecuteReader()

                If role = "Admin" Or role = "SuperUser" Then
                    ddlusers.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                End If

                While dr.Read()
                    ddlusers.Items.Add(New ListItem(dr("username"), dr("userid")))
                End While
                conn.Close()

            Catch ex As Exception


            End Try
            MyBase.OnInit(e)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try
                backpage = Request.Headers("Referer")
                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return mysubmit();")
                    RadioButton1.Attributes.Add("onclick", "radioclick();")
                    RadioButton2.Attributes.Add("onclick", "radioclick();")
                End If

            Catch ex As Exception

            End Try
        End Sub

        Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            Try
                Dim userid As String = ddlusers.SelectedValue
                Dim accesstype As Byte = 0

                If RadioButton1.Checked = True Then
                    If Request.Cookies("userinfo")("role") = "Admin" Then
                        accesstype = "2"
                    Else
                        accesstype = "1"
                    End If
                End If
              
                Dim len As Integer = FileUpload1.PostedFile.ContentLength
                Dim pic As Byte() = {}
                Dim imgname = ""

                Dim poitype As Int16 = poitypevalue.Value
                Dim poiname As String = poinametextbox.Text
                Dim lat As String = latitudetextbox.Text
                Dim lon As String = longitudetextbox.Text
                Dim minzoom As String = ddlmin.Value
                Dim maxzoom As String = ddlmax.Value
                Dim createddatetime As String = Now.ToString("yyyy/MM/dd HH:mm:ss")
                Dim modifieddatetime As String = Now.ToString("yyyy/MM/dd HH:mm:ss")

                'Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
               Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand
                Try
                    conn.Open()
                    If len = 0 Then
                        cmd = New SqlCommand("insert into poi_new(userid,accesstype,poitype,poiname,lat,lon,createddatetime,modifieddatetime,minzoomlevel,maxzoomlevel) values('" & userid & "'," & accesstype & "," & poitype & ",'" & poiname & "'," & lat & "," & lon & ",'" & createddatetime & "','" & modifieddatetime & "'," & minzoom & "," & maxzoom & ")", conn)
                        If cmd.ExecuteNonQuery() > 0 Then
                            If Request.Cookies("userinfo")("role") = "Admin" Then
                                If accesstype = 0 Then
                                    Response.Redirect("POIManagement.aspx?userid=" & userid)
                                Else
                                    Response.Redirect("POIManagement.aspx?userid=" & userid)
                                End If
                            Else
                                If accesstype = 0 Then
                                    Response.Redirect("POIManagement.aspx?userid=" & userid)
                                Else
                                    Response.Redirect("POIManagement.aspx?userid=" & userid)
                                End If
                            End If
                        End If
                    Else
                        '-
                        pic = New Byte(len - 1) {}

                        FileUpload1.PostedFile.InputStream.Read(pic, 0, len)

                        Const PROD_IMG_MAX_WIDTH As Integer = 256
                        Const PROD_IMG_HEIGHT As Integer = 256

                        'Create an image object from the uploaded file

                        Dim UploadedImage As System.Drawing.Image = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream)

                        If UploadedImage.Width > PROD_IMG_MAX_WIDTH Or UploadedImage.Height > PROD_IMG_HEIGHT Then
                            'whatever fail code you need
                            errormessage = "Upload file dimension should be less then 256px*256px"
                        Else
                            '-
                            If len > 102400 Then
                                errormessage = "Upload file size should be less then 100 KB"
                            Else
                                ' Dim imgsize As Double = Convert.ToDouble(UploadedImage.Size)
                                cmd = New SqlCommand("insert into poi_new(userid,accesstype,poitype,poiname,lat,lon,createddatetime,modifieddatetime,minzoomlevel,maxzoomlevel,image) values(@userid,@accesstype,@poitype,@poiname,@lat,@lon,@createddatetime,@modifieddatetime,@minzoomlevel,@maxzoomlevel,@previewimage)", conn)

                                cmd.Parameters.AddWithValue("@userid", userid)
                                cmd.Parameters.AddWithValue("@accesstype", accesstype)
                                cmd.Parameters.AddWithValue("@poitype", poitype)

                                cmd.Parameters.AddWithValue("@poiname", poiname)

                                cmd.Parameters.AddWithValue("@lat", lat)

                                cmd.Parameters.AddWithValue("@lon", lon)

                                cmd.Parameters.AddWithValue("@createddatetime", createddatetime)

                                cmd.Parameters.AddWithValue("@modifieddatetime", modifieddatetime)

                                cmd.Parameters.AddWithValue("@minzoomlevel", minzoom)
                                cmd.Parameters.AddWithValue("@maxzoomlevel", maxzoom)
                                cmd.Parameters.AddWithValue("@previewimage", pic)
                                If cmd.ExecuteNonQuery() > 0 Then
                                    If Request.Cookies("userinfo")("role") = "Admin" Then
                                        If accesstype = 0 Then
                                            Response.Redirect("POIManagement.aspx?userid=" & userid)
                                        Else
                                            Response.Redirect("POIManagement.aspx?userid=" & userid)
                                        End If
                                    Else
                                        If accesstype = 0 Then
                                            Response.Redirect("POIManagement.aspx?userid=" & userid)
                                        Else
                                            Response.Redirect("POIManagement.aspx?userid=" & userid)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If


                Catch ex As Exception

                Finally
                    conn.Close()
                End Try

            Catch ex As Exception

            End Try
        End Sub
    End Class
End Namespace
