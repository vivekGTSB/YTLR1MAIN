Imports System.Data.SqlClient

Namespace AVLS

    Partial Class AddUser
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
        Public lastuserid As Int64

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Request.Cookies("userinfo") Is Nothing Then
                    Response.Redirect("Login.aspx")
                End If

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return mysubmit()")

                    HiddenField1.Value = Convert.ToString(Request.Cookies("userinfo")("username")).ToUpper()

                    Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim command As SqlCommand

                    'lastuserid = command.ExecuteScalar()
                    
                    lastuserid = 0
                    Dim dr As SqlDataReader
                    conn.Open()
                    command = New SqlCommand("select userid from userTBL", conn)

                    dr = command.ExecuteReader()
                    While dr.Read()
                        If Convert.ToInt64(dr("userid")) > lastuserid Then
                            lastuserid = dr("userid")
                        End If
                    End While
                    dr.Close()
                    userid.Text = (System.Convert.ToInt64(lastuserid) + 1).ToString()
                    conn.Close()

                    If Convert.ToString(Request.Cookies("userinfo")("username")).ToUpper() = "SVWONG" Then
                        ddlrole.SelectedValue = "SuperUser"
                        ddlaccess.SelectedValue = "0"
                        ddllevel.SelectedValue = "0"
                        companyname.Text = "LAFARGE"

                    End If

                End If

            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub

        Private Sub ImageButton1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            insertUserIntoServer1()
            'insertUserIntoServer2()
            'insertUserIntoServer3()
        End Sub

        Protected Sub insertUserIntoServer1()
            Try

                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand

                Dim result As Byte = 0

                conn.Open()
                cmd = New SqlCommand("insert into userTBL(userid,username,pwd,companyname,phoneno,faxno,streetname,postcode,state,role,userslist,mobileno,emailid,access,timestamp, usertype,dbip,erp)values('" & userid.Text.Trim() & "','" & username.Text.Trim() & "','" & password.Text.Trim() & "','" & companyname.Text.Trim() & "','" & phonenumber.Text.Trim() & "','" & faxnumber.Text.Trim & "','" & streetname.Text.Trim() & "','" & postalcode.Text.Trim() & "','" & ddlstate.SelectedValue & "','" & ddlrole.SelectedValue & "','','" & mobilenumber.Text.Trim() & "','" & emailid.Text.Trim() & "','" & ddlaccess.SelectedValue & "','" & Convert.ToDateTime(DateTime.Now).ToString("yyyy/MM/dd HH:mm:ss") & "','" & ddllevel.SelectedValue & "','" & ddlServer.SelectedValue & "','" & ddlERP.SelectedValue & "')", conn)
                result = cmd.ExecuteNonQuery()
                conn.Close()
                'Response.Write(cmd.CommandText)
                If result > 0 Then
                    Server.Transfer("UserManagement.aspx?userid=" & userid.Text.Trim())
                Else
                    errormessage = "record not inserted in server 1."
                End If

            Catch ex As SystemException
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
                ' errormessage = cmd.CommandText
            End Try
        End Sub

    End Class

End Namespace
