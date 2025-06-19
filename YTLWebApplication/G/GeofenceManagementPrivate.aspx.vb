Imports System.Data.SqlClient
Imports System.Data
Imports AspMap
Imports System.Text

Partial Class GeofenceManagementPrivate
    Inherits System.Web.UI.Page
    Public googlemapsparameters As String
    Public googleearthparameters As String
    Public tf As String = ""
    Public sb1 As New StringBuilder()


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' Enhanced authentication check
            If Not IsUserAuthenticated() Then
                Response.Redirect("Login.aspx", True)
                Return
            End If

            ' Validate user permissions
            If Not HasRequiredPermissions() Then
                Response.Redirect("AccessDenied.aspx", True)
                Return
            End If

        Catch ex As Exception
            LogSecurityEvent("Authentication Error", ex.Message)
            Response.Redirect("Login.aspx", True)
        End Try
        MyBase.OnInit(e)
    End Sub
    Private Function GetUserId() As String
        Try
            If Request.Cookies("userinfo") IsNot Nothing Then
                Return Request.Cookies("userinfo")("userid")
            End If
            Return String.Empty
        Catch
            Return String.Empty
        End Try
    End Function

    Private Function GetUserRole() As String
        Try
            If Request.Cookies("userinfo") IsNot Nothing Then
                Return Request.Cookies("userinfo")("role")
            End If
            Return String.Empty
        Catch
            Return String.Empty
        End Try
    End Function
    Private Sub LogSecurityEvent(ByVal eventType As String, ByVal message As String)
        Try
            ' Implement proper security logging
            ' For now, just log to system event log or file
            System.Diagnostics.EventLog.WriteEntry("YTLWebApp",
                String.Format("{0}: {1} - User: {2} - IP: {3}",
                eventType, message, GetUserId(), Request.UserHostAddress),
                System.Diagnostics.EventLogEntryType.Warning)
        Catch
            ' Fail silently for logging errors
        End Try
    End Sub
    Private Function HasRequiredPermissions() As Boolean
        Try
            Dim role As String = GetUserRole()
            Return Not String.IsNullOrEmpty(role) AndAlso
                   (role = "Admin" OrElse role = "SuperUser" OrElse role = "LFSuperUser")
        Catch
            Return False
        End Try
    End Function
    Private Function IsUserAuthenticated() As Boolean
        Try
            Return Request.Cookies("userinfo") IsNot Nothing AndAlso
                   Not String.IsNullOrEmpty(Request.Cookies("userinfo")("userid"))
        Catch
            Return False
        End Try
    End Function
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return activeconfirmation();")
                ImageButton2.Attributes.Add("onclick", "return activeconfirmation();")
                ImageButton3.Attributes.Add("onclick", "return activeconfirmation();")
                ImageButton4.Attributes.Add("onclick", "return activeconfirmation();")
            End If
            FillGrid()
        Catch ex As Exception
            LogSecurityEvent("Page Load Error", ex.Message)
            ShowErrorMessage("An error occurred while loading the page.")
        End Try
    End Sub
    Private Sub ShowErrorMessage(ByVal message As String)
        Try
            ' Implement user-friendly error display
            Response.Write(String.Format("<script>alert('{0}');</script>", HttpUtility.JavaScriptStringEncode(message)))
        Catch
            ' Fail silently
        End Try
    End Sub
    Private Function GetUsersList() As String
        Try
            If Request.Cookies("userinfo") IsNot Nothing Then
                Return Request.Cookies("userinfo")("userslist")
            End If
            Return String.Empty
        Catch
            Return String.Empty
        End Try
    End Function
    Private Sub FillGrid()
        Try
            Dim geofencetable As New DataTable
            geofencetable.Rows.Clear()
            geofencetable.Columns.Add(New DataColumn("chk"))
            geofencetable.Columns.Add(New DataColumn("S No"))
            geofencetable.Columns.Add(New DataColumn("Geofence Name"))
            geofencetable.Columns.Add(New DataColumn("Username"))
            geofencetable.Columns.Add(New DataColumn("Transporter"))
            geofencetable.Columns.Add(New DataColumn("Centroid"))
            geofencetable.Columns.Add(New DataColumn("Modify Date Time"))

            Dim r As DataRow

            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand



            Dim role As String = GetUserRole()
            Dim userslist As String = GetUsersList()
            Dim userid As String = GetUserId()

            If role = "User" Then
                cmd = New SqlCommand("select  t1.data,t1.geofenceid,t1.geofencename,t1.status,t1.geofencetype,t1.modifieddatetime,t2.username,t2.companyname from geofence as t1,userTBL as t2 where accesstype=@accessType and t1.userid=@userid and t1.userid=t2.userid order by geofencename", conn)
                cmd.Parameters.AddWithValue("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select  t1.data,t1.geofenceid,t1.geofencename,t1.status,t1.geofencetype,t1.modifieddatetime,t2.username,t2.companyname from geofence as t1,userTBL as t2 where accesstype=@accessType and t1.userid in (" & userslist & ") and t1.userid=t2.userid order by geofencename", conn)
            Else
                cmd = New SqlCommand("select  t1.data,t1.geofenceid,t1.geofencename,t1.status,t1.geofencetype,t1.modifieddatetime,t2.username,t2.companyname from geofence as t1,userTBL as t2 where accesstype=@accessType and t1.userid=t2.userid order by geofencename", conn)
            End If
            cmd.Parameters.AddWithValue("@accessType", 0)

            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()

                Dim i As Int32 = 1
                While (dr.Read)
                    Try

                        r = geofencetable.NewRow

                        r(0) = String.Format("<input type=""checkbox"" name=""chk"" value=""{0}""/>",
                                HttpUtility.HtmlEncode(dr("geofenceid").ToString()))
                        r(1) = i.ToString()
                        Dim geofenceId As String = HttpUtility.HtmlEncode(dr("geofenceid").ToString())
                        Dim geofenceName As String = HttpUtility.HtmlEncode(dr("geofencename").ToString().ToUpper())
                        If dr("status") Then
                            r(2) = String.Format("<a href='UpdateGeofencePrivate.aspx?geofenceid={0}'> {1} </a>",
                                                 Server.UrlEncode(geofenceId), geofenceName)
                        Else
                            r(2) = String.Format("<a href='UpdateGeofencePrivate.aspx?geofenceid={0}'><span style=""text-decoration: line-through; color: #FF0000"">{1}</span></a>",
                                                               Server.UrlEncode(geofenceId), geofenceName)
                        End If

                        r(3) = dr("username").ToString().ToUpper()
                        r(4) = dr("companyname").ToString().ToUpper()
                        Dim data As String = dr("data")

                        If dr("geofencetype") Then
                            Dim ptslayer As New AspMap.Points
                            Dim shp As New AspMap.Shape
                            shp.ShapeType = ShapeType.mcPolygonShape

                            Dim pots() As String = data.Split(";")
                            Dim vals() As String
                            For i1 As Integer = 0 To pots.Length - 1
                                vals = pots(i1).Split(",")
                                ptslayer.AddPoint(vals(0), vals(1))
                            Next
                            shp.AddPart(ptslayer)
                            r(5) = shp.Centroid.Y.ToString("0.0000") & "," & shp.Centroid.X.ToString("0.0000")
                        Else
                            Dim pots() As String = data.Split(",")
                            r(5) = Convert.ToDouble(pots(1).ToString()).ToString("0.0000") & "," & Convert.ToDouble(pots(0).ToString()).ToString("0.0000")
                        End If
                        r(6) = DateTime.Parse(dr("modifieddatetime")).ToString("yyyy/MM/dd  HH:mm:ss")

                        geofencetable.Rows.Add(r)
                        i = i + 1

                    Catch ex As Exception

                    End Try
                End While
            Catch ex As Exception

            Finally
                conn.Close()
            End Try
            If geofencetable.Rows.Count = 0 Then
                r = geofencetable.NewRow

                r(0) = "<input type=""checkbox"" name=""chk"" />"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                r(5) = "-"
                r(6) = "-"
                geofencetable.Rows.Add(r)
            End If

            Session("exceltable") = geofencetable
            BuildGridHtml(geofencetable)


        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub
    Private Sub BuildGridHtml(ByVal geofencetable As DataTable)
        Try
            sb1.Length = 0
            sb1.Append("<thead><tr><th align=""center""  style=""width:20px;""><input type='checkbox' onclick=""javascript:checkall(this);""/></th><th style=""width:35px;"" >S No</th><th style=""width:250px;"">Geofence Name</th><th style=""width:90px;"">Username</th><th style=""width:200px;"">Transporter Name</th><th style=""width:130px;"">Centroid</th><th style=""width:150px;"">Modify Date Time</th></tr></thead>")
            Dim counter As Integer = 1
            sb1.Append("<tbody>")
            For i As Integer = 0 To geofencetable.Rows.Count - 1
                Try
                    sb1.Append("<tr>")
                    For j As Integer = 0 To 6 ' Only include necessary columns
                        sb1.Append("<td>")
                        sb1.Append(HttpUtility.HtmlEncode(geofencetable.DefaultView.Item(i)(j).ToString()))
                        sb1.Append("</td>")
                    Next
                    sb1.Append("</tr>")
                Catch ex As Exception
                    LogSecurityEvent("Grid HTML Building Error", ex.Message)
                    Continue For
                End Try
            Next
            sb1.Append("</tbody>")
            sb1.Append("<tfoot><tr><th><input type='checkbox' onclick=""javascript:checkall(this);""/></th><th>S No</th><th>Geofence Name</th><th>Username</th><th>Transporter Name</th><th>Centroid</th><th>Modify DateTime</th></tr></tfoot>")

        Catch ex As Exception
            LogSecurityEvent("BuildGridHtml Error", ex.Message)
        End Try
    End Sub
    Protected Sub DeleteGeofence()
        Try
            Dim geofenceIds As String() = GetSelectedGeofenceIds()
            If geofenceIds Is Nothing OrElse geofenceIds.Length = 0 Then
                ShowErrorMessage("No items selected for deletion.")
                Return
            End If

            Dim conn As SqlConnection = New SqlConnection(GetSecureConnectionString())

            Try
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    Try
                        For Each geofenceId As String In geofenceIds
                            Dim cmd As SqlCommand = New SqlCommand("DELETE FROM geofence WHERE geofenceid = @geofenceId", conn, transaction)
                            cmd.Parameters.AddWithValue("@geofenceId", geofenceId)
                            cmd.ExecuteNonQuery()
                            LogSecurityEvent("Geofence Deleted", String.Format("GeofenceId: {0}, User: {1}", geofenceId, GetUserId()))

                        Next

                        transaction.Commit()
                        ShowSuccessMessage("Selected geofences deleted successfully.")

                    Catch ex As Exception
                        transaction.Rollback()
                        LogSecurityEvent("Delete Transaction Error", ex.Message)
                        ShowErrorMessage("An error occurred during deletion.")
                    End Try
                End Using

            Finally
                If conn.State = ConnectionState.Open Then
                    conn.Close()
                End If
            End Try

            FillGrid()

        Catch ex As Exception
            LogSecurityEvent("DeleteGeofence Error", ex.Message)
            ShowErrorMessage("An error occurred while deleting geofences.")
        End Try
    End Sub
    Private Function GetSecureConnectionString() As String
        Try
            Return System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")
        Catch
            Throw New Exception("Database connection configuration error")
        End Try
    End Function
    Private Sub ShowSuccessMessage(ByVal message As String)
        Try
            ' Implement user-friendly success display
            Response.Write(String.Format("<script>alert('{0}');</script>", HttpUtility.JavaScriptStringEncode(message)))
        Catch
            ' Fail silently
        End Try
    End Sub
    Private Function GetSelectedGeofenceIds() As String()
        Try
            If Not String.IsNullOrEmpty(Request.Form("chk")) Then
                Return Request.Form("chk").Split(","c)
            End If
            Return New String() {}
        Catch
            Return New String() {}
        End Try
    End Function
    Private Sub Activate()
        Try
            UpdateGeofenceStatus(True)
        Catch ex As Exception
            LogSecurityEvent("Activate Error", ex.Message)
            ShowErrorMessage("An error occurred while activating geofences.")
        End Try
    End Sub
    Private Sub UpdateGeofenceStatus(ByVal status As Boolean)
        Try
            Dim geofenceIds As String() = GetSelectedGeofenceIds()
            If geofenceIds Is Nothing OrElse geofenceIds.Length = 0 Then
                ShowErrorMessage("No items selected.")
                Return
            End If

            Dim conn As SqlConnection = New SqlConnection(GetSecureConnectionString())

            Try
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    Try
                        For Each geofenceId As String In geofenceIds
                            Dim cmd As SqlCommand = New SqlCommand("UPDATE geofence SET status = @status WHERE geofenceid = @geofenceId", conn, transaction)
                            cmd.Parameters.AddWithValue("@status", status)
                            cmd.Parameters.AddWithValue("@geofenceId", geofenceId)
                            cmd.ExecuteNonQuery()
                            LogSecurityEvent("Geofence Status Updated", String.Format("GeofenceId: {0}, Status: {1}, User: {2}", geofenceId, status, GetUserId()))

                        Next

                        transaction.Commit()
                        ShowSuccessMessage(String.Format("Selected geofences {0} successfully.", If(status, "activated", "deactivated")))

                    Catch ex As Exception
                        transaction.Rollback()
                        LogSecurityEvent("Status Update Transaction Error", ex.Message)
                        ShowErrorMessage("An error occurred during status update.")
                    End Try
                End Using

            Finally
                If conn.State = ConnectionState.Open Then
                    conn.Close()
                End If
            End Try

            FillGrid()

        Catch ex As Exception
            LogSecurityEvent("UpdateGeofenceStatus Error", ex.Message)
            ShowErrorMessage("An error occurred while updating geofence status.")
        End Try
    End Sub
    Private Sub DeActivate()
        Try
            UpdateGeofenceStatus(False)
        Catch ex As Exception
            LogSecurityEvent("DeActivate Error", ex.Message)
            ShowErrorMessage("An error occurred while deactivating geofences.")
        End Try
    End Sub

    Protected Sub ImageButton3_Click(sender As Object, e As System.EventArgs) Handles ImageButton3.Click
        Activate()
    End Sub

    Protected Sub ImageButton4_Click(sender As Object, e As System.EventArgs) Handles ImageButton4.Click
        DeActivate()
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        Activate()
    End Sub

    Protected Sub ImageButton2_Click(sender As Object, e As System.EventArgs) Handles ImageButton2.Click
        DeActivate()
    End Sub


    Protected Sub ImageButton5_Click(sender As Object, e As System.EventArgs) Handles ImageButton5.Click
        DeleteGeofence()
    End Sub


    Protected Sub ImageButton6_Click(sender As Object, e As System.EventArgs) Handles ImageButton6.Click
        DeleteGeofence()
    End Sub
End Class
