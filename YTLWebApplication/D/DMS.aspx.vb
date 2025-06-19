Imports System.Data.SqlClient
Imports System.Web.Script.Services
Partial Class DMS
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            ' Validate userid is numeric to prevent injection
            Dim userIdInt As Integer
            If Not Integer.TryParse(userid, userIdInt) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            Dim cmd As SqlCommand
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim ds As New DataSet
            Dim da As SqlDataAdapter
            
            ddlGeofence.Items.Add(New ListItem("ALL SHIP-TO-CODE", "ALL GEOFENCES"))
            
            ' FIXED: Use parameterized query
            cmd = New SqlCommand("SELECT * FROM userTBL WHERE role='User' ORDER BY username", conn)

            ' FIXED: Use parameterized query with proper parameter handling
            da = New SqlDataAdapter("SELECT DISTINCT(shiptocode), geofencename, ISNULL(t2.geofenceid, 0) AS count FROM geofence t1 LEFT OUTER JOIN (SELECT * FROM user_geofence_favorite WHERE userid = @userid) AS t2 ON t1.geofenceid = t2.geofenceid WHERE accesstype='1' AND shiptocode <> '0' ORDER BY count DESC", conn2)
            da.SelectCommand.Parameters.AddWithValue("@userid", userIdInt)

            If role = "User" Then
                cmd = New SqlCommand("SELECT * FROM userTBL WHERE userid = @userid ORDER BY username", conn)
                cmd.Parameters.AddWithValue("@userid", userIdInt)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                ' Validate userslist contains only numbers and commas
                If Not IsValidUsersList(userslist) Then
                    Response.Redirect("Login.aspx")
                    Return
                End If
                cmd = New SqlCommand("SELECT * FROM userTBL WHERE role='User' AND userid IN (" & userslist & ") ORDER BY username", conn)
                DropDownList1.Items.Add(New ListItem("ALL USERS", "ALL USERS"))
            Else
                DropDownList1.Items.Add(New ListItem("ALL USERS", "ALL USERS"))
            End If
            
            da.Fill(ds)
            Dim count As Integer = 0
            For count = 0 To ds.Tables(0).Rows.Count - 1
                Dim ls As New ListItem(HttpUtility.HtmlEncode(ds.Tables(0).Rows(count)("geofencename").ToString().ToUpper() & "[ " & ds.Tables(0).Rows(count)("shiptocode").ToString().ToUpper() & " ]"), ds.Tables(0).Rows(count)("shiptocode"))
                If ds.Tables(0).Rows(count)("count") = 0 Then
                    ls.Attributes.Add("favorite", False)
                Else
                    ls.Attributes.Add("favorite", True)
                End If
                ddlGeofence.Items.Add(ls)
            Next
            
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            While (dr.Read())
                DropDownList1.Items.Add((New ListItem(HttpUtility.HtmlEncode(dr("username").ToString()), dr("userid").ToString())))
            End While
            DropDownList1.SelectedValue = Request.Form("DropDownList1")
            ddlplate.Items.Add(New ListItem("ALL PLATES", "ALL PLATES"))
            
            If role = "User" Then
                cmd = New SqlCommand("SELECT * FROM vehicleTBL WHERE userid = @userid ORDER BY plateno", conn)
                cmd.Parameters.AddWithValue("@userid", userIdInt)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If Not IsValidUsersList(userslist) Then
                    Response.Redirect("Login.aspx")
                    Return
                End If
                cmd = New SqlCommand("SELECT * FROM vehicleTBL WHERE userid IN (" & userslist & ") ORDER BY plateno", conn)
            Else
                cmd = New SqlCommand("SELECT * FROM vehicleTBL ORDER BY plateno", conn)
            End If

            dr = cmd.ExecuteReader()

            While (dr.Read())
                ddlplate.Items.Add((New ListItem(HttpUtility.HtmlEncode(dr("plateno")), HttpUtility.HtmlEncode(dr("plateno")))))
            End While

            ddlplate.SelectedValue = Request.Form("ddlplate")
            ddlGeofence.SelectedValue = Request.Form("ddlGeofence")
            conn.Close()
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Try
                Dim ds2 As New DataSet
                da = New SqlDataAdapter("SELECT DISTINCT (transporter) FROM oss_patch_out WHERE status=7 AND transporter <> 'NULL' ORDER BY transporter", conn)
                da.Fill(ds2)
                count = 0
                ddlTransporter.Items.Add(New ListItem("ALL TRANSPORTERS", "ALL TRANSPORTERS"))
                For count = 0 To ds2.Tables(0).Rows.Count - 1
                    ddlTransporter.Items.Add(New ListItem(HttpUtility.HtmlEncode(ds2.Tables(0).Rows(count)(0).ToString().ToUpper()), HttpUtility.HtmlEncode(ds2.Tables(0).Rows(count)(0))))
                Next
            Catch ex As Exception
                ' Log error but don't expose details
                System.Diagnostics.Debug.WriteLine("Error loading transporters: " & ex.Message)
            End Try
        Catch ex As Exception
            ' Log error but don't expose details
            System.Diagnostics.Debug.WriteLine("Error in OnInit: " & ex.Message)
            Response.Redirect("Login.aspx")
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now.AddDays(-2).ToString("yyyy/MM/dd")
                txtEndDate.Value = Now.AddDays(-1).ToString("yyyy/MM/dd")
            Else
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                
                ' Validate userid
                Dim userIdInt As Integer
                If Not Integer.TryParse(userid, userIdInt) Then
                    Response.Redirect("Login.aspx")
                    Return
                End If
                
                Dim dr As SqlDataReader
                Dim cmd As SqlCommand
                ddlplate.Items.Clear()
                
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                ddlplate.Items.Add(New ListItem("ALL PLATES", "ALL PLATES"))
                ddlGeofence.Items.Add(New ListItem("ALL SHIP-TO-CODE", "ALL GEOFENCES"))
                
                Dim da As SqlDataAdapter
                da = New SqlDataAdapter("SELECT DISTINCT(shiptocode), geofencename FROM geofence WHERE accesstype='1' AND shiptocode <> '0' ORDER BY shiptocode", conn2)

                ' FIXED: Use parameterized query for selected user
                Dim selectedUser As String = Request.Form("DropDownList1")
                If selectedUser = "ALL USERS" Then
                    If role = "SuperUser" Or role = "Operator" Then
                        If Not IsValidUsersList(userslist) Then
                            Response.Redirect("Login.aspx")
                            Return
                        End If
                        cmd = New SqlCommand("SELECT * FROM vehicleTBL WHERE userid IN (" & userslist & ") ORDER BY plateno", conn)
                    Else
                        cmd = New SqlCommand("SELECT * FROM vehicleTBL ORDER BY plateno", conn)
                    End If
                Else
                    ' Validate selected user ID
                    Dim selectedUserInt As Integer
                    If Integer.TryParse(selectedUser, selectedUserInt) Then
                        cmd = New SqlCommand("SELECT * FROM vehicleTBL WHERE userid = @userid ORDER BY plateno", conn)
                        cmd.Parameters.AddWithValue("@userid", selectedUserInt)
                    Else
                        Response.Redirect("Login.aspx")
                        Return
                    End If
                End If
                
                Dim ds As New DataSet
                Try
                    ddlGeofence.SelectedValue = Request.Form("ddlGeofence")
                    If conn.State <> ConnectionState.Open Then
                        conn.Open()
                    End If
                    dr = cmd.ExecuteReader()
                    While (dr.Read())
                        ddlplate.Items.Add((New ListItem(HttpUtility.HtmlEncode(dr("plateno")), HttpUtility.HtmlEncode(dr("plateno")))))
                    End While
                Catch ex As Exception
                    System.Diagnostics.Debug.WriteLine("Error loading vehicles: " & ex.Message)
                Finally
                    conn.Close()
                End Try
                ddlplate.SelectedValue = Request.Form("ddlplate")
                ddlTransporter.SelectedValue = Request.Form("ddlTransporter")
            End If

        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in Page_Load: " & ex.Message)
        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function ManageFavorite(ByVal geoid As String, ByVal op As Int16) As String
        Dim retval As String = "0"
        Dim userid As String = HttpContext.Current.Request.Cookies("userinfo")("userid")
        
        ' Validate inputs
        Dim userIdInt As Integer
        If Not Integer.TryParse(userid, userIdInt) Then
            Return "0"
        End If
        
        If String.IsNullOrEmpty(geoid) OrElse geoid.Length > 50 Then
            Return "0"
        End If
        
        ' Sanitize geoid - only allow alphanumeric characters
        If Not System.Text.RegularExpressions.Regex.IsMatch(geoid, "^[a-zA-Z0-9]+$") Then
            Return "0"
        End If
        
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try
            Dim cmd As New SqlCommand()
            cmd.Connection = conn
            
            ' FIXED: Use parameterized query
            cmd.CommandText = "SELECT geofenceid FROM geofence WHERE shiptocode = @geoid"
            cmd.Parameters.AddWithValue("@geoid", geoid)
            
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Dim geofenceid As String = "0"
            If dr.Read Then
                geofenceid = dr("geofenceid").ToString()
            End If
            dr.Close()
            
            ' Validate geofenceid is numeric
            Dim geofenceIdInt As Integer
            If Not Integer.TryParse(geofenceid, geofenceIdInt) Then
                Return "0"
            End If
            
            If op = 0 Then
                cmd.CommandText = "INSERT INTO user_geofence_favorite (userid, geofenceid) VALUES (@userid, @geofenceid)"
            Else
                cmd.CommandText = "DELETE FROM user_geofence_favorite WHERE userid = @userid AND geofenceid = @geofenceid"
            End If
            
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@userid", userIdInt)
            cmd.Parameters.AddWithValue("@geofenceid", geofenceIdInt)

            If cmd.ExecuteNonQuery() > 0 Then
                retval = "1"
            End If

        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in ManageFavorite: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return retval
    End Function
    
    ' Helper function to validate users list contains only numbers and commas
    Private Function IsValidUsersList(usersList As String) As Boolean
        If String.IsNullOrEmpty(usersList) Then
            Return False
        End If
        
        ' Check if usersList contains only numbers and commas
        Return System.Text.RegularExpressions.Regex.IsMatch(usersList, "^[0-9,]+$")
    End Function

End Class