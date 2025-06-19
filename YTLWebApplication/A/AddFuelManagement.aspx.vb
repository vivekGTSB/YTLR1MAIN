Imports System.Data.SqlClient

Namespace AVLS

    Partial Class AddFuelManagement
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
        Public DieselCost As String = "0"
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Request.Cookies("userinfo") Is Nothing Then
                    Response.Redirect("Login.aspx")
                End If

                If Page.IsPostBack = False Then
                    DieselCost = Request.QueryString("fc")
                    Dim userid As String = Request.QueryString("u")
                    Dim plateno As String = Request.QueryString("p")
                    Dim refueldate As DateTime = DateTime.Now()
                    Dim litre As Double = 0
                    If Request.QueryString("r") <> "" Then
                        refueldate = Request.QueryString("r")
                    End If
                    If Request.QueryString("l") <> "" Then
                        litre = CDbl(Request.QueryString("l"))
                         imgHead.InnerText = "Update Refuel Receipt"
                    End If

                    ImageButton2.Attributes.Add("onclick", "return mysubmit()")
                    ddloil.Attributes.Add("onchange", "fuelchange(this)")

                    Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim cmd As SqlCommand = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)

                    conn.Open()
                    Dim dr As SqlDataReader = cmd.ExecuteReader()
                    If dr.Read() Then
                        ddluser.Items.Add(New ListItem(dr("username"), dr("userid")))
                    End If
                    conn.Close()

                    txtonelitercost.Value = DieselCost

                    ddlplatenumber.Items.Add(New ListItem(plateno, plateno))
                    tbxdatetime.Value = refueldate.ToString("yyyy/MM/dd")
                    tbxlitters.Value = litre
                    tbxcost.Value = CDbl(litre * txtonelitercost.Value).ToString("0.00")

                    If Request.QueryString("r") <> "" Then
                        Dim hourTime As String = Convert.ToString(refueldate.Hour)
                        If hourTime.Length = 1 Then
                            hourTime = "0" & hourTime
                        End If
                        ddlbh.SelectedValue = hourTime
                        ddlbm.SelectedValue = refueldate.Minute
                    End If
                End If

            Catch ex As SystemException
                errormessage = ex.Message
            End Try
        End Sub

        Private Sub ImageButton2_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
            Try
                If Request.QueryString("l") = "" Then
                    AddNewReceipt()
                Else
                    UpdateReceipt()
                End If
            Catch ex As Exception
                Response.Write(ex.Message)
            End Try
        End Sub

        Protected Sub AddNewReceipt()
            Try
                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand

                Dim result As Byte = 0

                Dim struserid As String = ddluser.SelectedValue
                Dim strplateno As String = ddlplatenumber.SelectedValue
                Dim strtimestamp As String = tbxdatetime.Value.Trim() & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
                Dim strfueltype As String = ddloil.SelectedItem.Text
                Dim strlitters As String = tbxlitters.Value.Trim()
                Dim strcost As String = tbxcost.Value.Trim()

                conn.Open()
                cmd = New SqlCommand("insert into fuel(userid,plateno,timestamp,fueltype,liters,cost) values('" & struserid & "','" & strplateno & "','" & strtimestamp & "','" & strfueltype & "','" & strlitters & "','" & strcost & "')", conn)
                result = cmd.ExecuteNonQuery()
                conn.Close()

                If result > 0 Then
                    'Response.Redirect("RefuelComparisonReport.aspx")
                    errormessage = "Record updated successfully."
                Else
                    errormessage = "Record Not Inserted."
                End If

            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub

        Protected Sub UpdateReceipt()
            Try

                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand

                Dim result As Byte = 0

                Dim struserid As String = ddluser.SelectedValue
                Dim strplateno As String = ddlplatenumber.SelectedValue
                Dim strtimestamp As String = tbxdatetime.Value.Trim() & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
                Dim strfueltype As String = ddloil.SelectedItem.Text
                Dim strlitters As String = tbxlitters.Value.Trim()
                Dim strcost As String = tbxcost.Value.Trim()
                'Response.Write("update fuel set timestamp='" & strtimestamp & "', fueltype='" & strfueltype & "', liters='" & strlitters & "', cost='" & strcost & "' where plateno='" & strplateno & "' and timestamp='" & Request.QueryString("r") & "'")
                conn.Open()
                cmd = New SqlCommand("update fuel set timestamp='" & strtimestamp & "', fueltype='" & strfueltype & "', liters='" & strlitters & "', cost='" & strcost & "' where plateno='" & strplateno & "' and timestamp='" & Request.QueryString("r") & "'", conn)
                result = cmd.ExecuteNonQuery()
                conn.Close()

                If result > 0 Then
                    'Response.Redirect("RefuelComparisonReport.aspx")
                    errormessage = "Record updated successfully."
                Else
                    errormessage = "Record Not Inserted"
                End If

            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub

    End Class

End Namespace
