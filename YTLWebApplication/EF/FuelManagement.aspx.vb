Imports System.Data.SqlClient
Imports System.Data

Partial Class FuelManagement
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public addfuelpage As String = "AddFuel.aspx"
    Public divgrid As Boolean = False
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
                        ddlusername.Items.Clear()
                        ddlusername.Items.Add(New ListItem("--Select User Name--", ""))
                        
                        While dr.Read()
                            ddlusername.Items.Add(New ListItem(SecurityHelper.SanitizeForHtml(dr("username").ToString()), dr("userid").ToString()))
                        End While
                    End Using
                End Using
            End Using
            
            ' Auto-select user if single user role
            If role = "User" AndAlso ddlusername.Items.Count > 1 Then
                ddlusername.SelectedValue = userid
                GetPlateNumbers(userid)
            End If
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOAD_USERS_ERROR", "Error loading users: " & ex.Message)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' Validate session
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If
            
            ' Get user info from session
            Dim userid As String = HttpContext.Current.Session("userid").ToString()
            
            ' Set add fuel page based on user permissions
            If userid = "0214" Then
                addfuelpage = "AddFuelToAll.aspx"
            End If

            If Page.IsPostBack = False Then
                ' Generate CSRF token
                hdnCSRFToken.Value = SecurityHelper.GenerateCSRFToken()
                
                txtBeginDate.Text = DateTime.Now.ToString("yyyy-MM-dd")
                txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd")
                
                ImageButton1.Attributes.Add("onclick", "return mysubmit();")
                delete1.Attributes.Add("onclick", "return deleteconfirmation();")
                delete2.Attributes.Add("onclick", "return deleteconfirmation();")
                btnDownload.Attributes.Add("onclick", "return validateDownload();")

                ' Handle query string parameters for updates
                If Not String.IsNullOrEmpty(Request.QueryString("s")) Then
                    CompleteUpdateFuel()
                End If
            End If

            LimitUserAccess()
            FillGrid()
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("PAGE_LOAD_ERROR", "Error in Page_Load: " & ex.Message)
        End Try
    End Sub

    Public Sub FillGrid()
        Try
            Dim userid As String = ddlusername.SelectedValue
            Dim plateno As String = ddlpleate.SelectedValue
            
            ' Validate inputs
            If Not SecurityHelper.ValidateInput(userid, "numeric") OrElse 
               Not SecurityHelper.ValidateInput(plateno, "plateno") Then
                Return
            End If
            
            Dim beginTimestamp As String = txtBeginDate.Text & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim endTimestamp As String = txtEndDate.Text & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            
            ' Validate dates
            If Not SecurityHelper.ValidateInput(txtBeginDate.Text, "date") OrElse 
               Not SecurityHelper.ValidateInput(txtEndDate.Text, "date") Then
                Return
            End If

            Dim usersTable As New DataTable
            usersTable.Columns.Add(New DataColumn("chk"))
            usersTable.Columns.Add(New DataColumn("S No"))
            usersTable.Columns.Add(New DataColumn("Plate No"))
            usersTable.Columns.Add(New DataColumn("Date Time"))
            usersTable.Columns.Add(New DataColumn("Fuel Id"))
            usersTable.Columns.Add(New DataColumn("Fuel Station Code"))
            usersTable.Columns.Add(New DataColumn("Fuel Type"))
            usersTable.Columns.Add(New DataColumn("Liters"))
            usersTable.Columns.Add(New DataColumn("Cost"))

            If Not String.IsNullOrEmpty(userid) AndAlso Not String.IsNullOrEmpty(plateno) Then
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim query As String = "SELECT fuelid, userid, plateno, CONVERT(varchar(19), timestamp, 120) AS timestamp, stationcode, fueltype, liters, cost FROM fuel WHERE plateno = @plateno AND timestamp BETWEEN @beginTime AND @endTime ORDER BY timestamp DESC"
                    
                    Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                        cmd.Parameters.AddWithValue("@plateno", plateno)
                        cmd.Parameters.AddWithValue("@beginTime", beginTimestamp)
                        cmd.Parameters.AddWithValue("@endTime", endTimestamp)
                        
                        conn.Open()
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            Dim i As Integer = 1
                            While dr.Read()
                                Dim r As DataRow = usersTable.NewRow()
                                
                                If LimitUserAccess() Then
                                    r("chk") = ""
                                    r("Plate No") = SecurityHelper.SanitizeForHtml(dr("plateno").ToString())
                                Else
                                    r("chk") = ""
                                    r("Plate No") = String.Format("<a href=""UpdateFuel.aspx?fuelid={0}"" title=""Update"">{1}</a>", 
                                                                HttpUtility.UrlEncode(dr("fuelid").ToString()), 
                                                                SecurityHelper.SanitizeForHtml(dr("plateno").ToString()))
                                End If
                                
                                r("S No") = i.ToString()
                                r("Date Time") = SecurityHelper.SanitizeForHtml(dr("timestamp").ToString())
                                r("Fuel Id") = SecurityHelper.SanitizeForHtml(dr("fuelid").ToString())
                                r("Fuel Station Code") = SecurityHelper.SanitizeForHtml(dr("stationcode").ToString())
                                r("Fuel Type") = SecurityHelper.SanitizeForHtml(dr("fueltype").ToString())
                                r("Liters") = Convert.ToDouble(dr("liters")).ToString("0.00")
                                r("Cost") = Convert.ToDouble(dr("cost")).ToString("0.00")
                                
                                usersTable.Rows.Add(r)
                                i += 1
                            End While
                        End Using
                    End Using
                End Using
            End If

            If usersTable.Rows.Count = 0 Then
                Dim r As DataRow = usersTable.NewRow()
                For j As Integer = 0 To usersTable.Columns.Count - 1
                    r(j) = "-"
                Next
                usersTable.Rows.Add(r)
            End If

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            
            fuelgrid.PageSize = Convert.ToInt32(noofrecords.SelectedValue)
            Session("exceltable") = usersTable
            fuelgrid.DataSource = usersTable
            fuelgrid.DataBind()
            
            ec = "true"
            
            If fuelgrid.PageCount > 1 Then
                show = True
            End If

            If LimitUserAccess() Then
                fuelgrid.Columns(0).Visible = False
            End If

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("FILL_GRID_ERROR", "Error filling grid: " & ex.Message)
        End Try
    End Sub

    Protected Sub DeleteFuelRecords()
        Try
            ' Validate CSRF token
            If Not SecurityHelper.ValidateCSRFToken(hdnCSRFToken.Value) Then
                SecurityHelper.LogSecurityEvent("CSRF_VIOLATION", "Invalid CSRF token in DeleteFuelRecords")
                Return
            End If
            
            Dim selectedItems As New List(Of String)
            
            ' Collect selected items safely
            For Each row As GridViewRow In fuelgrid.Rows
                Dim chkSelect As CheckBox = TryCast(row.FindControl("chkSelect"), CheckBox)
                Dim hdnFuelId As HiddenField = TryCast(row.FindControl("hdnFuelId"), HiddenField)
                
                If chkSelect IsNot Nothing AndAlso chkSelect.Checked AndAlso hdnFuelId IsNot Nothing Then
                    selectedItems.Add(hdnFuelId.Value)
                End If
            Next
            
            If selectedItems.Count > 0 Then
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    For Each fuelId As String In selectedItems
                        If SecurityHelper.ValidateInput(fuelId, "numeric") Then
                            Dim query As String = "DELETE FROM fuel WHERE fuelid = @fuelid"
                            Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                                cmd.Parameters.AddWithValue("@fuelid", fuelId)
                                
                                conn.Open()
                                cmd.ExecuteNonQuery()
                                conn.Close()
                            End Using
                        End If
                    Next
                End Using
                
                FillGrid()
            End If
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("DELETE_FUEL_RECORDS_ERROR", "Error deleting fuel records: " & ex.Message)
        End Try
    End Sub

    Protected Sub fuelgrid_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles fuelgrid.PageIndexChanging
        Try
            ec = "true"
            fuelgrid.PageSize = Convert.ToInt32(noofrecords.SelectedValue)
            fuelgrid.DataSource = Session("exceltable")
            fuelgrid.PageIndex = e.NewPageIndex
            fuelgrid.DataBind()
            
            If LimitUserAccess() Then
                fuelgrid.Columns(0).Visible = False
            End If
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("PAGE_INDEX_CHANGING_ERROR", "Error in page index changing: " & ex.Message)
        End Try
    End Sub

    Protected Sub delete2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles delete2.Click
        DeleteFuelRecords()
    End Sub

    Protected Sub delete1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles delete1.Click
        DeleteFuelRecords()
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

    Protected Sub ddlusername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlusername.SelectedIndexChanged
        Try
            GetPlateNumbers(ddlusername.SelectedValue)
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("DDL_USERNAME_CHANGED_ERROR", "Error in ddlusername_SelectedIndexChanged: " & ex.Message)
        End Try
    End Sub

    Protected Sub GetPlateNumbers(ByVal uid As String)
        Try
            ddlpleate.Items.Clear()
            ddlpleate.Items.Add(New ListItem("--Select Plate No--", ""))
            
            If Not String.IsNullOrEmpty(uid) AndAlso SecurityHelper.ValidateInput(uid, "numeric") Then
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim query As String = "SELECT plateno FROM vehicleTBL WHERE userid = @userid ORDER BY plateno"
                    Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                        cmd.Parameters.AddWithValue("@userid", uid)
                        
                        conn.Open()
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            While dr.Read()
                                ddlpleate.Items.Add(New ListItem(SecurityHelper.SanitizeForHtml(dr("plateno").ToString()), dr("plateno").ToString()))
                            End While
                        End Using
                    End Using
                End Using
            End If
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GET_PLATE_NUMBERS_ERROR", "Error getting plate numbers: " & ex.Message)
        End Try
    End Sub

    Protected Sub CompleteUpdateFuel()
        Try
            Dim startDate As String = Request.QueryString("s")
            Dim userid As String = Request.QueryString("u")
            Dim plateno As String = Request.QueryString("p")
            
            ' Validate inputs
            If SecurityHelper.ValidateInput(startDate, "date") AndAlso 
               SecurityHelper.ValidateInput(userid, "numeric") AndAlso 
               SecurityHelper.ValidateInput(plateno, "plateno") Then
                
                txtBeginDate.Text = Convert.ToDateTime(startDate).ToString("yyyy-MM-dd")
                txtEndDate.Text = Convert.ToDateTime(startDate).ToString("yyyy-MM-dd")
                
                ddlusername.SelectedValue = userid
                GetPlateNumbers(userid)
                ddlpleate.SelectedValue = plateno
                FillGrid()
            End If
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("COMPLETE_UPDATE_FUEL_ERROR", "Error in CompleteUpdateFuel: " & ex.Message)
        End Try
    End Sub

    Function GetUserLevel() As String
        Try
            Dim userid As String = HttpContext.Current.Session("userid").ToString()
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = "SELECT usertype FROM userTBL WHERE userid = @userid"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userid", userid)
                    
                    conn.Open()
                    Dim userLevel As Object = cmd.ExecuteScalar()
                    Return If(userLevel IsNot Nothing, userLevel.ToString(), "")
                End Using
            End Using
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GET_USER_LEVEL_ERROR", "Error getting user level: " & ex.Message)
            Return ""
        End Try
    End Function

    Function LimitUserAccess() As Boolean
        Try
            If GetUserLevel() = "7" Then
                delete1.Visible = False
                delete2.Visible = False
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    Protected Sub btnDownload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDownload.Click
        Try
            ' Validate CSRF token
            If Not SecurityHelper.ValidateCSRFToken(hdnCSRFToken.Value) Then
                SecurityHelper.LogSecurityEvent("CSRF_VIOLATION", "Invalid CSRF token in btnDownload_Click")
                Return
            End If
            
            If ec = "true" Then
                Response.Redirect("ExcelReport.aspx?title=Fuel Management Report&plateno=" & HttpUtility.UrlEncode(ddlpleate.SelectedValue))
            Else
                ' Show error message
            End If
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("DOWNLOAD_ERROR", "Error in btnDownload_Click: " & ex.Message)
        End Try
    End Sub

End Class