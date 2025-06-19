Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class SecureGetDMSLafargeT
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY: Validate session
            If Not G2SecurityHelper.ValidateSession() Then
                Response.StatusCode = 401
                Response.Write("Unauthorized")
                Response.End()
                Return
            End If
            
            ' SECURITY: Check authorization
            If Not G2SecurityHelper.HasRequiredRole("USER") Then
                Response.StatusCode = 403
                Response.Write("Forbidden")
                Response.End()
                Return
            End If
            
            ' SECURITY: Rate limiting
            Dim clientIP As String = Request.UserHostAddress
            If Not G2SecurityHelper.CheckRateLimit("GetDMSLafarge_" & clientIP, 60, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.Write("Too Many Requests")
                Response.End()
                Return
            End If
            
            Response.Write(GetSecureJson())
            Response.ContentType = "application/json"
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("GET_DMS_LAFARGE_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Internal Server Error")
        End Try
    End Sub

    Protected Function GetSecureJson() As String
        Dim json As String = ""
        
        Try
            ' SECURITY: Validate and sanitize input parameters
            Dim sdate As String = ValidateAndSanitizeDateInput(Request.QueryString("fdt"))
            Dim edate As String = ValidateAndSanitizeDateInput(Request.QueryString("tdt"))
            
            If String.IsNullOrEmpty(sdate) OrElse String.IsNullOrEmpty(edate) Then
                sdate = Date.Now.AddDays(-2).ToString("yyyy-MM-dd")
                edate = Date.Now.AddDays(-1).ToString("yyyy-MM-dd")
            End If
            
            Dim shipToCodeDict As New Dictionary(Of String, String)
            
            ' SECURITY: Use parameterized queries
            Using connlafarge As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = "SELECT DISTINCT shiptocode, geofencename FROM geofence WHERE accesstype = @accessType"
                Using cmdlafarge As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, connlafarge)
                    cmdlafarge.Parameters.AddWithValue("@accessType", 1)
                    
                    Try
                        connlafarge.Open()
                        Using drlafarge As SqlDataReader = cmdlafarge.ExecuteReader()
                            While drlafarge.Read()
                                Dim shipToCode As String = G2SecurityHelper.SanitizeForHtml(drlafarge("shiptocode").ToString())
                                Dim geofenceName As String = G2SecurityHelper.SanitizeForHtml(drlafarge("geofencename").ToString())
                                
                                If Not shipToCodeDict.ContainsKey(shipToCode) Then
                                    shipToCodeDict.Add(shipToCode, geofenceName)
                                End If
                            End While
                        End Using
                    Catch ex As Exception
                        G2SecurityHelper.LogSecurityEvent("SHIP_TO_CODE_QUERY_ERROR", ex.Message)
                        Throw New ApplicationException("Database query failed")
                    End Try
                End Using
            End Using
            
            Dim aa As New ArrayList()
            Dim driverNameDict As New Dictionary(Of String, String)
            
            ' SECURITY: Load driver names with parameterized query
            LoadDriverNames(driverNameDict, sdate, edate)
            
            ' SECURITY: Load main data with parameterized query
            LoadMainData(aa, shipToCodeDict, driverNameDict, sdate, edate)
            
            json = JsonConvert.SerializeObject(aa, Formatting.None)
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("GET_SECURE_JSON_ERROR", ex.Message)
            Throw New ApplicationException("Data retrieval failed")
        End Try
        
        Return json
    End Function
    
    Private Function ValidateAndSanitizeDateInput(dateInput As String) As String
        Try
            If String.IsNullOrEmpty(dateInput) Then
                Return String.Empty
            End If
            
            ' SECURITY: Validate date format
            If Not G2SecurityHelper.ValidateG2Input(dateInput, G2InputType.DateTime) Then
                Return String.Empty
            End If
            
            Dim parsedDate As DateTime
            If DateTime.TryParse(dateInput, parsedDate) Then
                ' SECURITY: Ensure date is within reasonable range
                If parsedDate >= DateTime.Now.AddYears(-5) AndAlso parsedDate <= DateTime.Now.AddDays(1) Then
                    Return parsedDate.ToString("yyyy-MM-dd")
                End If
            End If
            
            Return String.Empty
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("DATE_VALIDATION_ERROR", ex.Message)
            Return String.Empty
        End Try
    End Function
    
    Private Sub LoadDriverNames(driverNameDict As Dictionary(Of String, String), sdate As String, edate As String)
        Try
            Using connJob As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                Dim query As String = "SELECT dn_no, dn_driver, dn_qty FROM oss_patch_in WHERE weight_outtime BETWEEN @startDate AND @endDate AND dn_driver IS NOT NULL"
                Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, connJob)
                    cmd.Parameters.AddWithValue("@startDate", sdate)
                    cmd.Parameters.AddWithValue("@endDate", edate)
                    
                    connJob.Open()
                    Using drJob As SqlDataReader = cmd.ExecuteReader()
                        While drJob.Read()
                            Dim dnNo As String = G2SecurityHelper.SanitizeForHtml(drJob("dn_no").ToString())
                            Dim driverName As String = G2SecurityHelper.SanitizeForHtml(drJob("dn_driver").ToString())
                            
                            If Not driverNameDict.ContainsKey(dnNo) Then
                                driverNameDict.Add(dnNo, driverName)
                            End If
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("LOAD_DRIVER_NAMES_ERROR", ex.Message)
            ' Continue execution with empty dictionary
        End Try
    End Sub
    
    Private Sub LoadMainData(aa As ArrayList, shipToCodeDict As Dictionary(Of String, String), driverNameDict As Dictionary(Of String, String), sdate As String, edate As String)
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                Dim query As String = "SELECT * FROM oss_patch_out WHERE weight_outtime BETWEEN @startDate AND @endDate AND status IN ('7','8','12','13')"
                Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, conn)
                    cmd.Parameters.AddWithValue("@startDate", sdate)
                    cmd.Parameters.AddWithValue("@endDate", edate)
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Try
                                Dim a As New ArrayList()
                                
                                ' SECURITY: Sanitize all output data
                                a.Add("ShipToName")
                                a.Add(G2SecurityHelper.SanitizeForHtml(dr("dn_no").ToString()))
                                a.Add(If(IsDBNull(dr("transporter")), "--", G2SecurityHelper.SanitizeForHtml(dr("transporter").ToString())))
                                a.Add(G2SecurityHelper.SanitizeForHtml(dr("plateno").ToString()))
                                
                                ' Add driver name safely
                                Dim dnNo As String = dr("dn_no").ToString()
                                If driverNameDict.ContainsKey(dnNo) Then
                                    a.Add(driverNameDict(dnNo))
                                Else
                                    a.Add("--")
                                End If
                                
                                a.Add(G2SecurityHelper.SanitizeForHtml(dr("source_supply").ToString()))
                                
                                ' Add ship to code safely
                                Dim destinationSiteId As String = dr("destination_siteid").ToString()
                                If shipToCodeDict.ContainsKey(destinationSiteId) Then
                                    a.Add(shipToCodeDict(destinationSiteId))
                                Else
                                    a.Add("--")
                                End If
                                
                                a.Add(G2SecurityHelper.SanitizeForHtml(destinationSiteId))
                                
                                ' Process time data safely
                                ProcessTimeData(a, dr)
                                
                                aa.Add(a)
                            Catch ex As Exception
                                G2SecurityHelper.LogSecurityEvent("PROCESS_RECORD_ERROR", ex.Message)
                                ' Continue with next record
                            End Try
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("LOAD_MAIN_DATA_ERROR", ex.Message)
            Throw New ApplicationException("Main data loading failed")
        End Try
    End Sub
    
    Private Sub ProcessTimeData(a As ArrayList, dr As SqlDataReader)
        Try
            ' Loading time processing
            If IsDBNull(dr("plant_intime")) Then
                a.Add("--")
                a.Add("--")
                a.Add(If(IsDBNull(dr("weight_outtime")), "--", Convert.ToDateTime(dr("weight_outtime")).ToString("yyyy/MM/dd")))
                a.Add(If(IsDBNull(dr("weight_outtime")), "--", Convert.ToDateTime(dr("weight_outtime")).ToString("HH:mm:ss")))
                a.Add("--")
            Else
                Dim plantintime As DateTime = Convert.ToDateTime(dr("plant_intime"))
                a.Add(plantintime.ToString("yyyy/MM/dd"))
                a.Add(plantintime.ToString("HH:mm:ss"))
                a.Add(Convert.ToDateTime(dr("weight_outtime")).ToString("yyyy/MM/dd"))
                a.Add(Convert.ToDateTime(dr("weight_outtime")).ToString("HH:mm:ss"))
                
                Dim tim As TimeSpan = (Convert.ToDateTime(dr("weight_outtime")) - plantintime)
                a.Add(tim.TotalMinutes.ToString("0"))
            End If
            
            ' Travelling time processing
            If Not (IsDBNull(dr("ata_date")) And IsDBNull(dr("ata_time"))) Then
                Dim tsss As String = (Convert.ToDateTime(dr("ata_date")).ToString("yyyy/MM/dd") & " " & dr("ata_time").ToString()).ToString()
                Dim atatimess As DateTime = Convert.ToDateTime(tsss)
                Dim tim As TimeSpan = (atatimess - Convert.ToDateTime(dr("weight_outtime")))
                a.Add(tim.TotalMinutes.ToString("0"))
            Else
                a.Add("--")
            End If
            
            ' Distance processing
            If IsDBNull(dr("distance")) Then
                a.Add("0.00")
            Else
                a.Add(Convert.ToDouble(dr("distance")).ToString("0.00"))
            End If
            
            ' Waiting time processing
            ProcessWaitingTime(a, dr)
            
            ' Unloading time processing
            ProcessUnloadingTime(a, dr)
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("PROCESS_TIME_DATA_ERROR", ex.Message)
            ' Add default values on error
            For i As Integer = 0 To 10
                a.Add("--")
            Next
        End Try
    End Sub
    
    Private Sub ProcessWaitingTime(a As ArrayList, dr As SqlDataReader)
        Try
            If IsDBNull(dr("pto1_datetime")) Then
                If Not IsDBNull(dr("wait_start_time")) Then
                    If Not (IsDBNull(dr("ata_date")) And IsDBNull(dr("ata_time"))) Then
                        Dim tsss As String = (Convert.ToDateTime(dr("ata_date")).ToString("yyyy/MM/dd") & " " & dr("ata_time").ToString()).ToString()
                        Dim atatimess As DateTime = Convert.ToDateTime(tsss)
                        a.Add(Convert.ToDateTime(dr("wait_start_time")).ToString("yyyy/MM/dd"))
                        a.Add(Convert.ToDateTime(dr("wait_start_time")).ToString("HH:mm:ss"))
                        a.Add(atatimess.ToString("yyyy/MM/dd"))
                        a.Add(atatimess.ToString("HH:mm:ss"))
                        Dim tim As TimeSpan = (atatimess - Convert.ToDateTime(dr("wait_start_time")))
                        a.Add(tim.TotalMinutes.ToString("0"))
                    Else
                        For i As Integer = 0 To 4
                            a.Add("--")
                        Next
                    End If
                Else
                    For i As Integer = 0 To 4
                        a.Add("--")
                    Next
                End If
            ElseIf IsDBNull(dr("wait_start_time")) Then
                a.Add("--")
                a.Add("--")
                a.Add(Convert.ToDateTime(dr("pto1_datetime")).ToString("yyyy/MM/dd"))
                a.Add(Convert.ToDateTime(dr("pto1_datetime")).ToString("HH:mm:ss"))
                a.Add("--")
            Else
                Dim atatimess As DateTime = Convert.ToDateTime(dr("pto1_datetime").ToString())
                a.Add(Convert.ToDateTime(dr("wait_start_time")).ToString("yyyy/MM/dd"))
                a.Add(Convert.ToDateTime(dr("wait_start_time")).ToString("HH:mm:ss"))
                a.Add(atatimess.ToString("yyyy/MM/dd"))
                a.Add(atatimess.ToString("HH:mm:ss"))
                Dim tim As TimeSpan = (atatimess - Convert.ToDateTime(dr("wait_start_time")))
                a.Add(tim.TotalMinutes.ToString("0"))
            End If
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("PROCESS_WAITING_TIME_ERROR", ex.Message)
            For i As Integer = 0 To 4
                a.Add("--")
            Next
        End Try
    End Sub
    
    Private Sub ProcessUnloadingTime(a As ArrayList, dr As SqlDataReader)
        Try
            If IsDBNull(dr("pto1_datetime")) Or IsDBNull(dr("pto2_datetime")) Then
                For i As Integer = 0 To 4
                    a.Add("--")
                Next
            Else
                Dim atatimess As DateTime = Convert.ToDateTime(dr("pto1_datetime").ToString())
                a.Add(atatimess.ToString("yyyy/MM/dd"))
                a.Add(atatimess.ToString("HH:mm:ss"))
                a.Add(Convert.ToDateTime(dr("pto2_datetime")).ToString("yyyy/MM/dd"))
                a.Add(Convert.ToDateTime(dr("pto2_datetime")).ToString("HH:mm:ss"))
                Dim tim As TimeSpan = (Convert.ToDateTime(dr("pto2_datetime")) - atatimess)
                a.Add(tim.TotalMinutes.ToString("0"))
            End If
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("PROCESS_UNLOADING_TIME_ERROR", ex.Message)
            For i As Integer = 0 To 4
                a.Add("--")
            Next
        End Try
    End Sub
    
End Class