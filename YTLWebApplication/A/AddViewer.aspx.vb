Imports System.Data.SqlClient

Namespace AVLS

    Partial Class AddViewer
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
        Public lastviewerid As Int64

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return mysubmit()")

                    'Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("fuelmaster"))
                    Dim command As SqlCommand

                    'lastuserid = command.ExecuteScalar()

                    lastviewerid = 1001
                    Dim dr As SqlDataReader
                    conn.Open()
                    command = New SqlCommand("SELECT viewerid FROM view_user", conn)

                    dr = command.ExecuteReader()
                    While dr.Read()
                        If Convert.ToInt64(dr("viewerid")) > lastviewerid Then
                            lastviewerid = dr("viewerid")
                        End If
                    End While
                    dr.Close()
                    hfViewerID.Value = (System.Convert.ToInt64(lastviewerid) + 1).ToString()
                    conn.Close()

                    Dim userid As String = Request.Cookies("userinfo")("userid")

                    If userid = "1029" Or userid = "1077" Or userid = "1329" Then 'userid = "1031" Or userid = "1028" Then
                        ddlviewertype.Enabled = True
                    End If
                End If

            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub

        Private Sub ImageButton1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            insertUserIntoServer1()
        End Sub

        Protected Sub insertUserIntoServer1()
            Try
                Dim userid As String = Request.Cookies("userinfo")("userid").ToString()

                'Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("fuelmaster"))
                Dim connLocal As SqlConnection
                Dim cmd As SqlCommand
                Dim objConn As New MyConn

                Dim result As Byte = 0

                conn.Open()
                cmd = New SqlCommand("INSERT INTO view_user(userid,viewerid,viewername,viewerpwd,viewertype) VALUES ('" & userid & "','" & hfViewerID.Value.Trim() & "','" & viewername.Text.Trim() & "','" & viewerpwd.Text.Trim() & "'," & ddlviewertype.SelectedItem.Value & ")", conn)
                result = cmd.ExecuteNonQuery()
                conn.Close()
                conn.Dispose()

                connLocal = New SqlConnection(objConn.getConnectionString(userid, False))

                'Response.Write("--" & objConn.getConnectionString(userid, False))

                If objConn.bMaster = False Then
                    connLocal.Open()
                    cmd = New SqlCommand("INSERT INTO view_user(userid,viewerid,viewername,viewerpwd,viewertype) VALUES ('" & userid & "','" & hfViewerID.Value.Trim() & "','" & viewername.Text.Trim() & "','" & viewerpwd.Text.Trim() & "'," & ddlviewertype.SelectedItem.Value & ")", connLocal)
                    cmd.ExecuteNonQuery()
                    connLocal.Close()
                    connLocal.Dispose()

                    'Response.Write("<br>Insert into local")
                End If

                If result > 0 Then
                    Server.Transfer("ViewerManagement.aspx?viewerid=" & hfViewerID.Value.Trim())
                Else
                    errormessage = "Record Not Inserted"
                End If

            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub

    End Class

End Namespace
