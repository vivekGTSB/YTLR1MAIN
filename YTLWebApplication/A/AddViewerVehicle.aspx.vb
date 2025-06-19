Imports System.Data.SqlClient

Namespace AVLS

    Partial Class AddViewerVehicle
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
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")

                If Page.IsPostBack = False Then
                    txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                    txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                    Dim userid As String = Request.Cookies("userinfo")("userid")
                    ImageButton1.Attributes.Add("onclick", "return mysubmit()")

                    Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("fuelmaster"))
                    'Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim command As SqlCommand
                    Dim dr As SqlDataReader
                    conn.Open()
                    command = New SqlCommand("SELECT viewerid,viewername FROM view_user WHERE userid='" & userid & "' ORDER BY viewername", conn)

                    dr = command.ExecuteReader()
                    While dr.Read()
                        ddlViewerName.Items.Add(New ListItem(dr("viewername"), dr("viewerid")))
                    End While
                    dr.Close()
                    conn.Close()

                    Dim objConn As New MyConn
                    conn.ConnectionString = objConn.getConnectionString(userid, False)
                    'conn.ConnectionString = "Server=desktop-kf29t7e\sqlexpress;Database=avlsdev;User ID=sa;Password=sa123;MultipleActiveResultSets=True;"
                    conn.Open()
                    If role = "SuperUser" Then
                        command = New SqlCommand("SELECT plateno FROM vehicleTBL WHERE userid in (" & userslist & ") ORDER BY plateno", conn)
                    Else
                        command = New SqlCommand("SELECT plateno FROM vehicleTBL WHERE userid='" & userid & "' ORDER BY plateno", conn)
                    End If
                    dr = command.ExecuteReader()
                    While dr.Read()
                        ddlPlateno.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                    End While
                    dr.Close()

                    conn.Close()

                End If

            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub

        Private Sub ImageButton1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            AddViewerVehicle()
        End Sub

        Protected Sub AddViewerVehicle()
            Try

                Dim begindatetime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
                Dim enddatetime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("fuelmaster"))
                Dim cmd As SqlCommand
                Dim strquery As String = Nothing

                Dim result As Byte = 0
                For n As Int32 = 0 To ddlPlateno.Items.Count - 1
                    If ddlPlateno.Items(n).Selected = True Then
                        Dim plateno As String = ddlPlateno.Items(n).Text
                        strquery = strquery & "INSERT INTO view_vehicle(viewerid,plateno,starttime,endtime,note,remarks) VALUES ('" & ddlViewerName.SelectedValue & "','" & plateno & "','" & begindatetime & "','" & enddatetime & "','" & txtConsignNote.Text & "','" & txtRemarks.Text & "');"
                    End If
                Next

                conn.Open()
                cmd = New SqlCommand(strquery, conn)
                result = cmd.ExecuteNonQuery()
                conn.Close()

                If result > 0 Then
                    Response.Redirect("ViewerVehicleManagement.aspx?viewerid=" & ddlViewerName.SelectedValue)
                Else
                    errormessage = "Record Not Inserted"
                End If

            Catch sqlError As SqlException
                If sqlError.Number = 2627 Then
                    errormessage = "This vehicle is already assigned to the user.\nPlease try another vehicle again."
                End If

            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "").Substring(0, 25)
            End Try
        End Sub

    End Class

End Namespace
