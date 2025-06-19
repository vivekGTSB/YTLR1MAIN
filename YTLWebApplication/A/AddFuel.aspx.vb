Imports System.Data.SqlClient

Namespace AVLS

    Partial Class AddFuel
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

                If Page.IsPostBack = False Then

                    ImageButton2.Attributes.Add("onclick", "return mysubmit()")
                    ddloil.Attributes.Add("onchange", "fuelchange(this)")
                    tbxdatetime.Value = Now().ToString("yyyy/MM/dd")

                    Dim cmd As SqlCommand
                    Dim dr As SqlDataReader

                    Dim userid As String = Request.Cookies("userinfo")("userid")
                    Dim role As String = Request.Cookies("userinfo")("role")
                    Dim userslist As String = Request.Cookies("userinfo")("userslist")

                    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    'cmd = New SqlCommand("select userid, username,dbip from userTBL where role='User' order by username", conn)
                    'If role = "User" Then
                    '    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid='" & userid & "'", conn)
                    'ElseIf role = "SuperUser" Or role = "Operator" Then
                    '    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
                    'End If
                    If role = "User" Then
                        cmd = New SqlCommand("select userid, username, dbip from userTBL where userid = @userid", conn)
                        cmd.Parameters.AddWithValue("@userid", userid)
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        ' Validate userslist contains only numbers and commas
                        If Not IsValidUsersList(userslist) Then
                            Response.Redirect("Login.aspx")
                            Return
                        End If
                        cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
                    Else
                        cmd = New SqlCommand("select userid, username,dbip from userTBL where role=@role order by username", conn)
                        cmd.Parameters.AddWithValue("@role", "User")
                    End If
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    While dr.Read()
                        ddlusername.Items.Add(New ListItem(dr("username"), dr("userid")))
                    End While
                    dr.Close()
                    If role = "User" Then
                        ddlusername.Items.Remove("--Select User Name--")
                        ddlusername.SelectedValue = userid
                        getPlateNo(userid)
                    End If
                    conn.Close()

                End If

            Catch ex As Exception
                errormessage = ex.Message
            End Try
        End Sub
        ' Helper function to validate users list contains only numbers and commas
        Private Function IsValidUsersList(usersList As String) As Boolean
            If String.IsNullOrEmpty(usersList) Then
                Return False
            End If

            ' Check if usersList contains only numbers and commas
            Return System.Text.RegularExpressions.Regex.IsMatch(usersList, "^[0-9,]+$")
        End Function
        Private Sub ImageButton2_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
            Try

                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand

                Dim result As Byte = 0

                Dim struserid As String = ddlusername.SelectedValue
                Dim strplateno As String = ddlpleate.SelectedValue
                Dim strtimestamp As String = tbxdatetime.Value.Trim() & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
                Dim strstationcode As String
                If tbxstationcode.Text = "" Then
                    strstationcode = "NA"
                Else
                    strstationcode = tbxstationcode.Text
                End If
                Dim strfueltype As String = ddloil.SelectedItem.Text
                Dim strlitters As String = tbxlitters.Value.Trim()
                Dim strcost As String = tbxcost.Value.Trim()

                conn.Open()
                cmd = New SqlCommand("insert into fuel(userid,plateno,timestamp,stationcode,fueltype,liters,cost) values('" & struserid & "','" & strplateno & "','" & strtimestamp & "','" & strstationcode & "','" & strfueltype & "','" & strlitters & "','" & strcost & "')", conn)
                result = cmd.ExecuteNonQuery()
                conn.Close()

                If result > 0 Then
                    'Response.Redirect("FuelManagement.aspx")
                    Response.Redirect("FuelManagement.aspx?u=" & struserid & "&p=" & strplateno & "&s=" & strtimestamp)
                Else
                    errormessage = "Record Not Inserted"
                End If

            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub

        Protected Sub getPlateNo(ByVal uid As String)
            Try
                If ddlUsername.SelectedValue <> "--Select User Name--" Then
                    ddlpleate.Items.Clear()
                    ddlpleate.Items.Add("--Select Plate No--")
                    Dim cmd As SqlCommand
                    Dim dr As SqlDataReader
                    Dim connection As New Redirect(ddlUsername.SelectedValue)
                    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))

                    cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & uid & "' order by plateno", conn)
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    While dr.Read()
                        ddlpleate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                    End While
                    dr.Close()

                    conn.Close()
                Else
                    ddlpleate.Items.Clear()
                    ddlpleate.Items.Add("--Select User Name--")
                End If
            Catch ex As Exception
                Response.Write(ex.Message)
            End Try
        End Sub

        Protected Sub ddlusername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlusername.SelectedIndexChanged
            getPlateNo(ddlusername.SelectedValue)
        End Sub
    End Class

End Namespace
