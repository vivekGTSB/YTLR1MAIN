Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic

Public Class GetKPIDetailsAPK
    Inherits System.Web.UI.Page

    Public sb1 As New StringBuilder()
    Public loggedinUID As String = ""
    
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate session
            If Not IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            loggedinUID = HttpUtility.HtmlEncode(GetCurrentUserId())
            FillGrid()
            
        Catch ex As Exception
            LogSecurityEvent("GetKPIDetailsAPK error", ex.Message)
            sb1.Append("<tbody><tr><td colspan='8'>Error loading data</td></tr></tbody>")
        End Try
    End Sub

    ' SECURITY FIX: Validate session authentication
    Private Function IsUserAuthenticated() As Boolean
        Try
            Return HttpContext.Current.Session("authenticated") IsNot Nothing AndAlso 
                   CBool(HttpContext.Current.Session("authenticated")) AndAlso
                   HttpContext.Current.Session("userid") IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ' SECURITY FIX: Get current user ID safely
    Private Function GetCurrentUserId() As String
        Try
            If HttpContext.Current.Session("userid") IsNot Nothing Then
                Return HttpContext.Current.Session("userid").ToString()
            End If
        Catch
        End Try
        Return ""
    End Function

    Public Sub FillGrid()
        Try
            ' SECURITY FIX: Validate and sanitize input parameters
            Dim tid As String = ValidateUserInput(Request.QueryString("u"))
            Dim tp As String = ValidateUserInput(Request.QueryString("tp"))
            Dim itype As String = ValidateUserInput(Request.QueryString("itype"))
            
            If String.IsNullOrEmpty(tid) OrElse String.IsNullOrEmpty(tp) OrElse String.IsNullOrEmpty(itype) Then
                sb1.Append("<tbody><tr><td colspan='8'>Invalid parameters</td></tr></tbody>")
                Return
            End If

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim maintenanceDict As New Dictionary(Of String, Maintenance)
            
            Try
                ' SECURITY FIX: Use parameterized query for maintenance data
                Using cmd2 As New SqlCommand("select plateno,timestamp,statusdate,status,officeremark,sourcename from maintenance where timestamp > @startDate order by timestamp desc", conn)
                    cmd2.Parameters.AddWithValue("@startDate", DateTime.Parse("2019/09/01"))
                    
                    conn.Open()
                    Using dr2 As SqlDataReader = cmd2.ExecuteReader()
                        While dr2.Read()
                            Try
                                Dim m As New Maintenance()
                                m.timestamp = DateTime.Parse(dr2("timestamp").ToString())
                                m.statusdate = DateTime.Parse(dr2("statusdate").ToString())
                                m.status = HttpUtility.HtmlEncode(dr2("status").ToString())
                                m.Remarks = If(IsDBNull(dr2("officeremark")), "", HttpUtility.HtmlEncode(dr2("officeremark").ToString()))
                                m.sourcename = If(IsDBNull(dr2("sourcename")), "", HttpUtility.HtmlEncode(dr2("sourcename").ToString().ToUpper()))
                                
                                Dim plateKey As String = HttpUtility.HtmlEncode(dr2("plateno").ToString().Trim().ToUpper())
                                If Not maintenanceDict.ContainsKey(plateKey) Then
                                    maintenanceDict.Add(plateKey, m)
                                End If
                            Catch ex As Exception
                                LogSecurityEvent("Maintenance data processing error", ex.Message)
                            End Try
                        End While
                    End Using
                Finally
                    conn.Close()
                End Try

                ' SECURITY FIX: Use parameterized queries for tracked data
                Dim trackedQuery As String
                If itype = "0" Then
                    trackedQuery = "select ytldb.dbo.fn_GetTransporterNameByPlateno(plateno) as TranspName,plateno,lat,lon,timestamp,dbo.fn_getusername_plateno(plateno) as username,isNull(roadname,'-') roadname from vehicle_tracked2 where plateno in (select plateno from VehicleTbl where userid=@tid)"
                Else
                    trackedQuery = "select ytldb.dbo.fn_GetTransporterNameByPlateno(plateno) as TranspName,plateno,lat,lon,timestamp,dbo.fn_getusername_plateno(plateno) as username,isNull(roadname,'-') roadname from vehicle_tracked2 where plateno in (select plateno from VehicleTbl where transporter_id=@tid)"
                End If

                Using cmd As New SqlCommand(trackedQuery, conn)
                    cmd.Parameters.AddWithValue("@tid", tid)
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim vehicleTrackedDict As New Dictionary(Of String, VehicleTrackedRecord)
                        
                        While dr.Read()
                            Try
                                Dim vtr As New VehicleTrackedRecord()
                                vtr.plateno = HttpUtility.HtmlEncode(dr("plateno").ToString().Trim())
                                vtr.timeStamp = DateTime.Parse(dr("timestamp").ToString())
                                vtr.lat = Convert.ToDouble(dr("lat"))
                                vtr.lon = Convert.ToDouble(dr("lon"))
                                vtr.TranspName = If(IsDBNull(dr("TranspName")) OrElse dr("TranspName").ToString().Trim() = "", "NA", HttpUtility.HtmlEncode(dr("TranspName").ToString()))
                                vtr.map = String.Format("<a href=""http://maps.google.com/maps?f=q&hl=en&q={0}+{1}&om=1&t=k"" target=""_blank"">Map</a>", dr("lat"), dr("lon"))
                                vtr.username = HttpUtility.HtmlEncode(dr("Username").ToString())
                                vtr.address = HttpUtility.HtmlEncode(dr("roadname").ToString())
                                vtr.status = String.Format("<a style='cursor:pointer;' onclick=""getData('{0}')""><img src='img/editicon.png' style='width: 16px;' alt='status' />Update</a>", HttpUtility.HtmlAttributeEncode(dr("plateno").ToString()))
                                
                                If Not vehicleTrackedDict.ContainsKey(vtr.plateno) Then
                                    vehicleTrackedDict.Add(vtr.plateno, vtr)
                                End If
                            Catch ex As Exception
                                LogSecurityEvent("Vehicle tracked data processing error", ex.Message)
                            End Try
                        End While
                    End Using
                    
                    ' Build the data table and HTML output
                    BuildDataTable(vehicleTrackedDict, maintenanceDict, tid, tp, itype)
                Finally
                    conn.Close()
                End Try

            Catch ex As Exception
                LogSecurityEvent("FillGrid database error", ex.Message)
                sb1.Append("<tbody><tr><td colspan='8'>Database error occurred</td></tr></tbody>")
            End Try

        Catch ex As Exception
            LogSecurityEvent("FillGrid general error", ex.Message)
            sb1.Append("<tbody><tr><td colspan='8'>Error loading data</td></tr></tbody>")
        End Try
    End Sub

    ' SECURITY FIX: Validate user input
    Private Function ValidateUserInput(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then
            Return ""
        End If

        ' Remove dangerous characters and limit length
        Dim sanitized As String = System.Text.RegularExpressions.Regex.Replace(input.Trim(), "[<>\"'%;()&+*/=]", "")
        
        ' Only allow alphanumeric characters
        If System.Text.RegularExpressions.Regex.IsMatch(sanitized, "^[A-Za-z0-9]{1,20}$") Then
            Return sanitized
        End If

        Return ""
    End Function

    ' SECURITY FIX: Build data table with proper encoding
    Private Sub BuildDataTable(vehicleTrackedDict As Dictionary(Of String, VehicleTrackedRecord), 
                              maintenanceDict As Dictionary(Of String, Maintenance), 
                              tid As String, tp As String, itype As String)
        Try
            sb1.Append("<thead><tr align=""left""><th>S No</th><th>Plate No</th><th>User Name</th><th>Transporter</th><th>Location</th><th>Last Updated Time</th><th>Map</th><th>Status</th></tr></thead>")
            sb1.Append("<tbody>")
            
            Dim counter As Integer = 1
            
            ' Process vehicle data safely
            For Each kvp In vehicleTrackedDict
                Try
                    Dim vr As VehicleTrackedRecord = kvp.Value
                    
                    sb1.Append("<tr>")
                    sb1.Append("<td>").Append(counter).Append("</td>")
                    sb1.Append("<td>").Append(vr.plateno).Append("</td>")
                    sb1.Append("<td>").Append(vr.username).Append("</td>")
                    sb1.Append("<td>").Append(vr.TranspName).Append("</td>")
                    sb1.Append("<td>").Append(vr.address).Append("</td>")
                    sb1.Append("<td>").Append(HttpUtility.HtmlEncode(vr.timeStamp.ToString("yyyy/MM/dd HH:mm:ss"))).Append("</td>")
                    sb1.Append("<td>").Append(vr.map).Append("</td>")
                    sb1.Append("<td>").Append(vr.status).Append("</td>")
                    sb1.Append("</tr>")
                    
                    counter += 1
                Catch ex As Exception
                    LogSecurityEvent("Data table row processing error", ex.Message)
                End Try
            Next
            
            If counter = 1 Then
                sb1.Append("<tr><td colspan='8'>No data available</td></tr>")
            End If
            
            sb1.Append("</tbody>")
            sb1.Append("<tfoot><tr style=""font-weight:bold;"" align=""left""><th>S No</th><th>Plate No</th><th>User Name</th><th>Transporter</th><th>Location</th><th>Last Updated Time</th><th>Map</th><th>Status</th></tr></tfoot>")
            
        Catch ex As Exception
            LogSecurityEvent("BuildDataTable error", ex.Message)
            sb1.Append("<tbody><tr><td colspan='8'>Error building data table</td></tr></tbody>")
        End Try
    End Sub

    ' SECURITY FIX: Secure logging
    Private Sub LogSecurityEvent(eventType As String, message As String)
        Try
            Dim logMessage As String = String.Format("{0}: {1} - User: {2}, IP: {3}", 
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                eventType, 
                If(HttpContext.Current.Session("userid"), "Unknown"), 
                HttpContext.Current.Request.UserHostAddress)
            
            System.Diagnostics.EventLog.WriteEntry("YTL_Security", logMessage, System.Diagnostics.EventLogEntryType.Warning)
        Catch
            ' Fail silently
        End Try
    End Sub

    Private Structure Maintenance
        Dim timestamp As DateTime
        Dim statusdate As DateTime
        Dim status As String
        Dim Remarks As String
        Dim sourcename As String
    End Structure

    Private Structure VehicleTrackedRecord
        Dim plateno As String
        Dim username As String
        Dim map As String
        Dim timeStamp As DateTime
        Dim TranspName As String
        Dim gpsAV As Char
        Dim lat As Double
        Dim lon As Double
        Dim speed As Double
        Dim status As String
        Dim direction As Double
        Dim odometer As Double
        Dim ignition As Boolean
        Dim overspeed As Boolean
        Dim power As Boolean
        Dim immobilizer As Boolean
        Dim fuel1 As Double
        Dim fuel2 As Double
        Dim fuel As Double
        Dim pto As Boolean
        Dim address As String
    End Structure
End Class