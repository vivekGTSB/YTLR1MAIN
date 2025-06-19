Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Data

Namespace AVLS

    Partial Class AddFuelFormula
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

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Try

                If Request.Cookies("userinfo") Is Nothing Then
                    Response.Redirect("Login.aspx")
                End If

                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")

                ImageButton1.Attributes.Add("onclick", "return mysubmit()")

                If Page.IsPostBack = False Then

                    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                    Dim da As SqlDataAdapter = New SqlDataAdapter("select userid,username from userTBL where role='User' order by username", conn)

                    If role = "User" Then
                        da = New SqlDataAdapter("select userid,username from userTBL where userid='" & userid & "'", conn)
                    ElseIf role = "SuperUser" Then
                        da = New SqlDataAdapter("select userid,username from userTBL where userid in(" & userslist & ")", conn)
                    End If

                    Dim ds As New DataSet
                    da.Fill(ds)

                    For i As Int32 = 0 To ds.Tables(0).Rows.Count - 1
                        ddluserid.Items.Add(New ListItem(ds.Tables(0).Rows(i).Item("username"), ds.Tables(0).Rows(i).Item("userid")))
                    Next

                Else

                End If
            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                End If

            Catch ex As Exception
                errormessage = ex.Message
            End Try
        End Sub

        Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            Try
                If cbUpdateOld.Checked = False Then
                    AddFuelFormula()
                Else
                    AddExistingFormula()
                End If
            Catch ex As SystemException
                Response.Write(ex.Message)
            End Try

        End Sub

        Protected Sub AddFuelFormula()
            Try
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                conn.Open()
                Dim cmd As SqlCommand = New SqlCommand("insert into fuel_tank_check(plateno,formulaname,tankno,remark)values('" & ddlPlateNo.SelectedValue & "','" & textVolumeName.Text & "','" & ddlTankNo.SelectedValue & "','" & textRemarks.Text & "')", conn)
                Dim result = cmd.ExecuteNonQuery()
                Dim cmd2 As SqlCommand = New SqlCommand("insert into fuel_tank_check(plateno,formulaname,tankno,remark)values('" & ddlPlateNo.SelectedValue & "','" & textCalibrationName.Text & "','" & ddlTankNo.SelectedValue & "','" & "" & "')", conn)
                Dim result2 = cmd2.ExecuteNonQuery()
                Dim cmd3 As SqlCommand = New SqlCommand("insert into fuel_tank_formula(formulaname,value,formula,formulatype)values('" & textVolumeName.Text & "','" & textVolumeOffset.Text & "','" & textVolumeFormula.Text & "','" & ddlVolumeType.SelectedValue & "')", conn)
                Dim result3 = cmd3.ExecuteNonQuery()
                Dim cmd4 As SqlCommand = New SqlCommand("insert into fuel_tank_formula(formulaname,value,formula,formulatype)values('" & textCalibrationName.Text & "','" & "0.0000" & "','" & "0" & "','" & ddlCalibrationType.SelectedValue & "')", conn)
                Dim result4 = cmd4.ExecuteNonQuery()

                Dim Heights(,,) As Integer = HeightArray()
                Dim cmd5 As SqlCommand
                For x As Int32 = 0 To (Heights.Length / 4) - 1
                    If Heights(x, 1, 1) <> 0 And Heights(x, 1, 0) <> 0 Then
                        cmd5 = New SqlCommand("insert into fuel_calibration(formulaname,height,minvalue,maxvalue)values('" & textCalibrationName.Text & "','" & Heights(x, 0, 1) & "','" & Heights(x, 1, 0) & "','" & Heights(x, 1, 1) & "')", conn)
                        cmd5.ExecuteNonQuery()
                    End If
                Next
                conn.Close()

                If result > 0 And result2 > 0 And result3 > 0 And result4 > 0 Then
                    Server.Transfer("fuelformulamanagement.aspx?userid=" & ddlUserid.SelectedValue)
                Else
                    errormessage = "Record Not Inserted"
                End If

            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub

        Protected Sub AddExistingFormula()
            Try
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                conn.Open()
                Dim cmd As SqlCommand = New SqlCommand("insert into fuel_tank_check(plateno,formulaname,tankno,remark)values('" & ddlPlateNo.SelectedValue & "','" & textVolumeName.Text & "','" & ddlTankNo.SelectedValue & "','" & textRemarks.Text & "')", conn)
                Dim result = cmd.ExecuteNonQuery()
                Dim cmd2 As SqlCommand = New SqlCommand("insert into fuel_tank_check(plateno,formulaname,tankno,remark)values('" & ddlPlateNo.SelectedValue & "','" & textCalibrationName.Text & "','" & ddlTankNo.SelectedValue & "','" & "" & "')", conn)
                Dim result2 = cmd2.ExecuteNonQuery()
                conn.Close()
                If result > 0 And result2 > 0 Then
                    Server.Transfer("fuelformulamanagement.aspx?userid=" & ddlUserid.SelectedValue)
                Else
                    errormessage = "Record Not Inserted"
                End If
            Catch ex As SystemException
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try

        End Sub

        Function HeightArray() As Integer(,,)
            Dim i As Int16 = 0
            Dim height(10, 1, 1) As Integer

            If (txtN0b.Text <> "" And txtX0.Text <> "") Then
                height(i, 0, 1) = 0
                height(i, 1, 0) = txtN0b.Text
                height(i, 1, 1) = txtX0.Text
                i = i + 1
            End If
            If (txtN1b.Text <> "" And txtX1.Text <> "") Then
                height(i, 0, 1) = 100
                height(i, 1, 0) = txtN1b.Text
                height(i, 1, 1) = txtX1.Text
                i = i + 1
            End If
            If (txtN2b.Text <> "" And txtX2.Text <> "") Then
                height(i, 0, 1) = 200
                height(i, 1, 0) = txtN2b.Text
                height(i, 1, 1) = txtX2.Text
                i = i + 1
            End If
            If (txtN3b.Text <> "" And txtX3.Text <> "") Then
                height(i, 0, 1) = 300
                height(i, 1, 0) = txtN3b.Text
                height(i, 1, 1) = txtX3.Text
                i = i + 1
            End If
            If (txtN4b.Text <> "" And txtX4.Text <> "") Then
                height(i, 0, 1) = 400
                height(i, 1, 0) = txtN4b.Text
                height(i, 1, 1) = txtX4.Text
                i = i + 1
            End If
            If (txtN5b.Text <> "" And txtX5.Text <> "") Then
                height(i, 0, 1) = 500
                height(i, 1, 0) = txtN5b.Text
                height(i, 1, 1) = txtX5.Text
                i = i + 1
            End If
            If (txtN6b.Text <> "" And txtX6.Text <> "") Then
                height(i, 0, 1) = 600
                height(i, 1, 0) = txtN6b.Text
                height(i, 1, 1) = txtX6.Text
                i = i + 1
            End If
            If (txtN7b.Text <> "" And txtX7.Text <> "") Then
                height(i, 0, 1) = 700
                height(i, 1, 0) = txtN7b.Text
                height(i, 1, 1) = txtX7.Text
                i = i + 1
            End If
            If (txtN8b.Text <> "" And txtX8.Text <> "") Then
                height(i, 0, 1) = 800
                height(i, 1, 0) = txtN8b.Text
                height(i, 1, 1) = txtX8.Text
                i = i + 1
            End If
            If (txtN9b.Text <> "" And txtX9.Text <> "") Then
                height(i, 0, 1) = 900
                height(i, 1, 0) = txtN9b.Text
                height(i, 1, 1) = txtX9.Text
                i = i + 1
            End If

            Return height
        End Function

        Protected Sub ddlUserid_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUserid.SelectedIndexChanged
            Try
               Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim da As SqlDataAdapter = New SqlDataAdapter("select * from vehicleTBL where userid='" & ddlUserid.SelectedValue & "'", conn)
                Dim ds As New DataSet
                da.Fill(ds)

                ddlPlateNo.Items.Clear()

                ddlPlateNo.Items.Add(New ListItem("-- select plateno --", "-- select plateno --"))

                For i As Int32 = 0 To ds.Tables(0).Rows.Count - 1
                    ddlPlateNo.Items.Add(New ListItem(ds.Tables(0).Rows(i).Item("plateno"), ds.Tables(0).Rows(i).Item("plateno")))
                Next
            Catch ex As Exception
                errormessage = ex.Message.ToString.Replace("'", "\'").Replace(ControlChars.CrLf, "")
            End Try
        End Sub
    End Class

End Namespace
