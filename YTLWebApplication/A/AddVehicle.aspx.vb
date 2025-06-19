Imports System.Data.SqlClient
Imports System.Data
Imports System.Drawing

Namespace AVLS

    Partial Class AddVehicle
        Inherits System.Web.UI.Page


#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub


        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region
        Public errormessage As String

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Request.Cookies("userinfo") Is Nothing Then
                    Response.Redirect("Login.aspx")
                End If

                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")

                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                ' textplateno.Attributes.Add("OnKeyUp", "javascript:platenoUnit();") ' or OnChange can be used as well

                If Page.IsPostBack = False Then
                    texttype.Items.Clear()
                    FillPermit()
                    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                    Dim da As SqlDataAdapter = New SqlDataAdapter("select userid,username from userTBL where role='User' order by username", conn)

                    If role = "User" Then
                        da = New SqlDataAdapter("select userid,username from userTBL where userid='" & userid & "'", conn)
                    ElseIf role = "SuperUser" Then
                        da = New SqlDataAdapter("select userid,username from userTBL where userid in(" & userslist & ")", conn)
                    End If

                    Dim ds As New DataSet
                    da.Fill(ds)

                    ddluserid.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))

                    For i As Int32 = 0 To ds.Tables(0).Rows.Count - 1
                        ddluserid.Items.Add(New ListItem(ds.Tables(0).Rows(i).Item("username"), ds.Tables(0).Rows(i).Item("userid")))
                    Next

                    textInstallDate.Text = Convert.ToDateTime(Date.Now).ToString("yyyy/MM/dd")

                    Dim da1 As SqlDataAdapter = New SqlDataAdapter("select vehicletype from vehicle_type where status=1 order by vehicletype", conn)
                    Dim ds1 As New DataSet
                    da1.Fill(ds1)
                    texttype.Items.Add(New ListItem("--Select Vehicle Type--", "--Select Vehicle Type--"))
                    For i As Int32 = 0 To ds1.Tables(0).Rows.Count - 1
                        texttype.Items.Add(New ListItem(ds1.Tables(0).Rows(i).Item("vehicletype"), ds1.Tables(0).Rows(i).Item("vehicletype")))
                    Next

                Else

                End If
            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub
        Public Sub FillPermit()
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("select id,name  from customer", conn)
            Try
                ddlpermit.Items.Clear()
                ddlpermit.Items.Add(New ListItem("Select plant", "0"))
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                While dr.Read()
                    ddlpermit.Items.Add(New ListItem(dr("name"), dr("id")))
                End While
            Catch ex As Exception
            Finally
                If conn.State <> ConnectionState.Closed Then
                    conn.Close()
                End If
            End Try
        End Sub
        Private Sub ImageButton1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            Try
                Dim userid As String = ddluserid.SelectedValue
                Dim plateno As String = textplateno.Text
                Dim type As String = texttype.SelectedValue
                Dim color As String = textcolor.Text
                Dim model As String = textmodel.Text
                Dim brand As String = textbrand.Text
                Dim groupname As String = ddlgroupname.SelectedValue
                Dim trailerid As String = texttrailerid.Text
                Dim pmid As String = txtprimermoverid.Text
                Dim speedlimit As String = textspeedlimit.Text
                Dim unitid As String = textunitid.Text
                Dim versionid As String = textversionid.Text
                Dim smallimage As String = "smallvehicledefault.gif"
                Dim bigimage As String = "bigvehicle.gif"
                Dim immobilizer As String = ddlImmobilizer.SelectedValue
                Dim roaming As String = ddlroaming.SelectedValue
                Dim permit As String = ddlpermit.SelectedValue
                Dim portno As String = textPortNo.Text
                If textPortNo.Text = "" Or textPortNo.Text = "-" Or textPortNo.Text = "--" Then
                    portno = ""
                End If

                Dim installationdate As String = textInstallDate.Text
                If textInstallDate.Text = "" Or textInstallDate.Text = "-" Or textInstallDate.Text = "--" Then
                    installationdate = ""
                End If

                Dim tank1size As String = txttank1size.Text
                If txttank1size.Text = "" Or txttank1size.Text = "-" Or txttank1size.Text = "--" Then
                    tank1size = ""
                End If
                Dim tank2size As String = txttank2size.Text
                If txttank1size.Text = "" Or txttank1size.Text = "-" Or txttank1size.Text = "--" Then
                    tank2size = ""
                End If
                Dim tank1shape As String = ddltank1shape.SelectedValue
                If ddltank1shape.SelectedValue = "-- select shape --" Then
                    tank1shape = ""
                End If
                Dim tank2shape As String = ddltank2shape.SelectedValue
                If ddltank2shape.SelectedValue = "-- select shape --" Then
                    tank2shape = ""
                End If

                Dim inp As New IntPtr()
                Dim uploadedfilestream As System.IO.Stream
                Dim filecount As Byte = 0

                If (Request.Files(0).FileName <> "" And Request.Files(0).ContentLength <> 0) Then

                    filecount += 1
                    uploadedfilestream = Request.Files(0).InputStream
                    Dim img1 As New Bitmap(uploadedfilestream)
                    img1 = img1.GetThumbnailImage(20, 20, Nothing, inp)
                    smallimage = plateno & ".gif"
                    Dim filepath1 As String = Server.MapPath("vehiclesmallimages") & "\" & smallimage
                    img1.Save(filepath1, System.Drawing.Imaging.ImageFormat.Gif)

                End If

                If (Request.Files(1).FileName <> "" And Request.Files(1).ContentLength <> 0) Then

                    filecount += 1
                    uploadedfilestream = Request.Files(1).InputStream
                    Dim img2 As New Bitmap(uploadedfilestream)
                    img2 = img2.GetThumbnailImage(100, 100, Nothing, inp)
                    bigimage = plateno & ".gif"
                    Dim filepath2 As String = Server.MapPath("vehiclebigimages") & "\" & bigimage
                    img2.Save(filepath2, System.Drawing.Imaging.ImageFormat.Gif)

                End If

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand

                conn.Open()
                If permit = "0" Then
                    cmd = New SqlCommand("insert into vehicleTBL(pto,userid,plateno,type,color,model,brand,groupid,trailerid,speedlimit,unitid,versionid,gprsport,installationdate,tank1size,tank2size,tank1shape,tank2shape,smallimage,bigimage,immobilizer,roaming,groupname,pmid,companyid) values('" & btnPTO.SelectedValue & "','" & userid & "','" & plateno & "','" & type & "','" & color & "','" & model & "','" & brand & "','" & groupname & "','" & trailerid & "','" & speedlimit & "','" & unitid & "','" & versionid & "','" & portno & "','" & installationdate & "','" & tank1size & "','" & tank2size & "','" & tank1shape & "','" & tank2shape & "','" & smallimage & "','" & bigimage & "','" & immobilizer & "','" & roaming & "','','" & pmid & "',null)", conn)
                Else
                    cmd = New SqlCommand("insert into vehicleTBL(pto,userid,plateno,type,color,model,brand,groupid,trailerid,speedlimit,unitid,versionid,gprsport,installationdate,tank1size,tank2size,tank1shape,tank2shape,smallimage,bigimage,immobilizer,roaming,groupname,pmid,companyid) values('" & btnPTO.SelectedValue & "','" & userid & "','" & plateno & "','" & type & "','" & color & "','" & model & "','" & brand & "','" & groupname & "','" & trailerid & "','" & speedlimit & "','" & unitid & "','" & versionid & "','" & portno & "','" & installationdate & "','" & tank1size & "','" & tank2size & "','" & tank1shape & "','" & tank2shape & "','" & smallimage & "','" & bigimage & "','" & immobilizer & "','" & roaming & "','','" & pmid & "','" & permit & "')", conn)
                End If

                Dim result = cmd.ExecuteNonQuery()
                conn.Close()

                If result > 0 Then
                    Server.Transfer("VehicleManagement.aspx?userid=" & userid)
                Else
                    errormessage = "Record Not Inserted"
                End If

            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub

        Protected Sub ddluserid_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddluserid.SelectedIndexChanged
            Try
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                Dim da As SqlDataAdapter = New SqlDataAdapter("select * from vehicle_group where userid='" & ddluserid.SelectedValue & "'", conn)
                Dim ds As New DataSet
                da.Fill(ds)

                ddlgroupname.Items.Clear()

                ddlgroupname.Items.Add(New ListItem("--Select Group Name--", "--Select Group Name--"))

                For i As Int32 = 0 To ds.Tables(0).Rows.Count - 1
                    ddlgroupname.Items.Add(New ListItem(ds.Tables(0).Rows(i).Item("groupname"), ds.Tables(0).Rows(i).Item("groupid")))
                Next
            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub
    End Class

End Namespace
