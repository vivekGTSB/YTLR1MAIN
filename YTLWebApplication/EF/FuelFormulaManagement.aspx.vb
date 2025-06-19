Imports System.Data.SqlClient
Imports System.Data

Namespace AVLS

    Partial Class FuelFormulaManagement
        Inherits System.Web.UI.Page
        Public ddlchange As Boolean = False
        Public ec As String = "false"

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Try
                ' Check authentication using secure helper
                If Not AuthenticationHelper.IsUserAuthenticated() Then
                    Response.Redirect("~/Login.aspx")
                    Return
                End If
                
                ' Get user info from session
                Dim userid As String = HttpContext.Current.Session("userid").ToString()
                Dim role As String = HttpContext.Current.Session("role").ToString()
                Dim userslist As String = HttpContext.Current.Session("userslist").ToString()
                
                LoadUsers(userid, role, userslist)
                
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("INIT_ERROR", "Error in OnInit: " & ex.Message)
                Response.Redirect("~/Login.aspx")
            Finally
                MyBase.OnInit(e)
            End Try
        End Sub
        
        Private Sub LoadUsers(userid As String, role As String, userslist As String)
            Try
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim query As String = "SELECT userid, username FROM userTBL WHERE role='User' ORDER BY username"
                    
                    If role = "User" Then
                        query = "SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username"
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        query = "SELECT userid, username FROM userTBL WHERE userid IN (" & userslist & ") ORDER BY username"
                    End If
                    
                    Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                        If role = "User" Then
                            cmd.Parameters.AddWithValue("@userid", userid)
                        End If
                        
                        conn.Open()
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            ddlusers.Items.Clear()
                            ddlusers.Items.Add(New ListItem("--Select User Name--", ""))
                            
                            While dr.Read()
                                ddlusers.Items.Add(New ListItem(SecurityHelper.SanitizeForHtml(dr("username").ToString()), dr("userid").ToString()))
                            End While
                        End Using
                    End Using
                End Using
                
                ' Auto-select user if single user role
                If role = "User" AndAlso ddlusers.Items.Count > 1 Then
                    ddlusers.SelectedValue = userid
                End If
                
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("LOAD_USERS_ERROR", "Error loading users: " & ex.Message)
            End Try
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load
            Try
                ' Validate session
                If Not AuthenticationHelper.IsUserAuthenticated() Then
                    Response.Redirect("~/Login.aspx")
                    Return
                End If
                
                If Page.IsPostBack = False Then
                    ' Generate CSRF token
                    hdnCSRFToken.Value = SecurityHelper.GenerateCSRFToken()
                    
                    ImageButton1.Attributes.Add("onclick", "return validateForm();")
                    ImageButton2.Attributes.Add("onclick", "return deleteconfirmation();")
                    
                    FillGrid()
                End If
                
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("PAGE_LOAD_ERROR", "Error in Page_Load: " & ex.Message)
            End Try
        End Sub

        Private Sub FillGrid()
            Try
                Dim userid As String = ddlusers.SelectedValue
                
                Dim t As New DataTable
                t.Columns.Add(New DataColumn("chk"))
                t.Columns.Add(New DataColumn("S No"))
                t.Columns.Add(New DataColumn("Plate No"))
                t.Columns.Add(New DataColumn("Level ID"))
                t.Columns.Add(New DataColumn("Formula"))
                t.Columns.Add(New DataColumn("Offset Value"))
                t.Columns.Add(New DataColumn("Type"))
                t.Columns.Add(New DataColumn("Tank"))
                t.Columns.Add(New DataColumn("Remarks"))

                If Not String.IsNullOrEmpty(userid) AndAlso SecurityHelper.ValidateInput(userid, "numeric") Then
                    Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                        Dim query As String = "SELECT v.plateno, ISNULL(c.formulaname, '-') AS formulaname, ISNULL(f.formula, '-') AS formula, ISNULL(f.value, '-') AS value, ISNULL(f.formulatype, '-') AS formulatype, ISNULL(c.tankno, '-') AS tankno, ISNULL(f.remark, '-') AS remark FROM vehicleTBL v LEFT JOIN fuel_tank_check c ON v.plateno = c.plateno LEFT JOIN fuel_tank_formula f ON c.formulaname = f.formulaname WHERE v.userid = @userid AND (f.formulatype = 'Tank Volume' OR f.formulatype = 'Tank Cylinder' OR f.formulatype IS NULL) ORDER BY v.plateno, c.tankno, c.formulaname"
                        
                        Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                            cmd.Parameters.AddWithValue("@userid", userid)
                            
                            conn.Open()
                            Using dr As SqlDataReader = cmd.ExecuteReader()
                                Dim i As Integer = 1
                                While dr.Read()
                                    Dim r As DataRow = t.NewRow()
                                    
                                    r("chk") = ""
                                    r("S No") = i.ToString()
                                    
                                    Dim plateno As String = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
                                    Dim formulaname As String = SecurityHelper.SanitizeForHtml(dr("formulaname").ToString())
                                    
                                    If formulaname <> "-" Then
                                        r("Plate No") = String.Format("<a href=""UpdateFuelFormula.aspx?uno={0}&pno={1}&fno={2}"">{3}</a>", 
                                                                    HttpUtility.UrlEncode(userid), 
                                                                    HttpUtility.UrlEncode(plateno), 
                                                                    HttpUtility.UrlEncode(formulaname), 
                                                                    plateno)
                                    Else
                                        r("Plate No") = plateno
                                    End If
                                    
                                    r("Level ID") = SecurityHelper.SanitizeForHtml(dr("formulaname").ToString())
                                    r("Formula") = SecurityHelper.SanitizeForHtml(dr("formula").ToString())
                                    r("Offset Value") = SecurityHelper.SanitizeForHtml(dr("value").ToString())
                                    
                                    Dim formulaType As String = SecurityHelper.SanitizeForHtml(dr("formulatype").ToString())
                                    If formulaType = "Tank Volume" Then
                                        formulaType = "Volume"
                                    ElseIf formulaType = "Tank Cylinder" Then
                                        formulaType = "Cylinder"
                                    End If
                                    r("Type") = formulaType
                                    
                                    r("Tank") = SecurityHelper.SanitizeForHtml(dr("tankno").ToString())
                                    r("Remarks") = SecurityHelper.SanitizeForHtml(dr("remark").ToString())
                                    
                                    t.Rows.Add(r)
                                    i += 1
                                End While
                            End Using
                        End Using
                    End Using
                End If

                If t.Rows.Count = 0 Then
                    Dim r As DataRow = t.NewRow()
                    For j As Integer = 0 To t.Columns.Count - 1
                        r(j) = "--"
                    Next
                    t.Rows.Add(r)
                End If

                ec = "true"
                vehiclesgrid.DataSource = t
                vehiclesgrid.DataBind()

            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("FILL_GRID_ERROR", "Error filling grid: " & ex.Message)
            End Try
        End Sub

        Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            Try
                ' Validate CSRF token
                If Not SecurityHelper.ValidateCSRFToken(hdnCSRFToken.Value) Then
                    SecurityHelper.LogSecurityEvent("CSRF_VIOLATION", "Invalid CSRF token in ImageButton1_Click")
                    Return
                End If
                
                FillGrid()
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("IMAGEBUTTON1_CLICK_ERROR", "Error in ImageButton1_Click: " & ex.Message)
            End Try
        End Sub

        Protected Sub ImageButton2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
            Try
                ' Validate CSRF token
                If Not SecurityHelper.ValidateCSRFToken(hdnCSRFToken.Value) Then
                    SecurityHelper.LogSecurityEvent("CSRF_VIOLATION", "Invalid CSRF token in ImageButton2_Click")
                    Return
                End If
                
                DeleteVehicles()
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("IMAGEBUTTON2_CLICK_ERROR", "Error in ImageButton2_Click: " & ex.Message)
            End Try
        End Sub

        Protected Sub DeleteVehicles()
            Try
                Dim selectedItems As New List(Of String)
                
                ' Collect selected items safely
                For Each row As GridViewRow In vehiclesgrid.Rows
                    Dim chkSelect As CheckBox = TryCast(row.FindControl("chkSelect"), CheckBox)
                    Dim hdnValue As HiddenField = TryCast(row.FindControl("hdnValue"), HiddenField)
                    
                    If chkSelect IsNot Nothing AndAlso chkSelect.Checked AndAlso hdnValue IsNot Nothing Then
                        selectedItems.Add(hdnValue.Value)
                    End If
                Next
                
                If selectedItems.Count > 0 Then
                    Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                        For Each item As String In selectedItems
                            Dim parts() As String = item.Split(";"c)
                            If parts.Length = 2 Then
                                Dim plateno As String = parts(0)
                                Dim tankno As String = parts(1)
                                
                                ' Validate inputs
                                If SecurityHelper.ValidateInput(plateno, "plateno") AndAlso 
                                   SecurityHelper.ValidateInput(tankno, "numeric") Then
                                    
                                    Dim query As String = "DELETE FROM fuel_tank_check WHERE plateno = @plateno AND tankno = @tankno"
                                    Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                                        cmd.Parameters.AddWithValue("@plateno", plateno)
                                        cmd.Parameters.AddWithValue("@tankno", tankno)
                                        
                                        conn.Open()
                                        cmd.ExecuteNonQuery()
                                        conn.Close()
                                    End Using
                                End If
                            End If
                        Next
                    End Using
                    
                    FillGrid()
                End If
                
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("DELETE_VEHICLES_ERROR", "Error deleting vehicles: " & ex.Message)
            End Try
        End Sub

        Protected Sub ddlusers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlusers.SelectedIndexChanged
            Try
                FillGrid()
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("DDL_USERS_CHANGED_ERROR", "Error in ddlusers_SelectedIndexChanged: " & ex.Message)
            End Try
        End Sub

    End Class

End Namespace