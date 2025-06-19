Imports System.Data.SqlClient

Public Class SecureGetExtVoltage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY: Validate session
            If Not G2SecurityHelper.ValidateSession() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If
            
            ' SECURITY: Check authorization
            If Not G2SecurityHelper.HasRequiredRole("USER") Then
                Response.Redirect("~/AccessDenied.aspx")
                Return
            End If
            
            ' SECURITY: Rate limiting
            Dim clientIP As String = Request.UserHostAddress
            If Not G2SecurityHelper.CheckRateLimit("ExtVoltage_" & clientIP, 30, TimeSpan.FromMinutes(1)) Then
                Response.Write("Rate limit exceeded")
                Response.End()
                Return
            End If
            
            LoadSecureExtVoltageData()
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("EXT_VOLTAGE_ERROR", ex.Message)
            Response.Redirect("~/Error.aspx")
        End Try
    End Sub
    
    Private Sub LoadSecureExtVoltageData()
        Try
            ' SECURITY: Validate input parameters
            Dim dttm As String = Request.QueryString("d")
            Dim plateno As String = Request.QueryString("plateno")
            
            If Not G2SecurityHelper.ValidateG2Input(dttm, G2InputType.DateTime) Then
                ShowErrorMessage("Invalid date parameter")
                Return
            End If
            
            If Not G2SecurityHelper.ValidateG2Input(plateno, G2InputType.PlateNumber) Then
                ShowErrorMessage("Invalid plate number")
                Return
            End If
            
            ' SECURITY: Parse and validate date range
            Dim targetDate As DateTime
            If Not DateTime.TryParse(dttm, targetDate) Then
                ShowErrorMessage("Invalid date format")
                Return
            End If
            
            Dim bdt As String = targetDate.AddMinutes(-10).ToString("yyyy/MM/dd HH:mm:ss")
            Dim edt As String = targetDate.AddMinutes(10).ToString("yyyy/MM/dd HH:mm:ss")
            
            Dim t As New DataTable
            InitializeVoltageTable(t)
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY: Use parameterized query
                Dim query As String = "SELECT DISTINCT CONVERT(varchar(19), timestamp, 120) as datetime, externalbatv, gps_av, speed FROM vehicle_history vht JOIN vehicleTBL vt ON vt.plateno = vht.plateno AND vt.plateno = @plateno AND timestamp BETWEEN @startDate AND @endDate"
                
                Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, conn)
                    cmd.Parameters.AddWithValue("@plateno", plateno)
                    cmd.Parameters.AddWithValue("@startDate", bdt)
                    cmd.Parameters.AddWithValue("@endDate", edt)
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        PopulateVoltageTable(t, dr, plateno)
                    End Using
                End Using
            End Using
            
            ' Bind to GridView
            gv1.DataSource = t
            gv1.DataBind()
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("LOAD_EXT_VOLTAGE_DATA_ERROR", ex.Message)
            ShowErrorMessage("Error loading voltage data")
        End Try
    End Sub
    
    Private Sub InitializeVoltageTable(t As DataTable)
        t.Columns.Add(New DataColumn("No"))
        t.Columns.Add(New DataColumn("Plateno"))
        t.Columns.Add(New DataColumn("Date Time"))
        t.Columns.Add(New DataColumn("GPS"))
        t.Columns.Add(New DataColumn("Speed"))
        t.Columns.Add(New DataColumn("External Voltage"))
    End Sub
    
    Private Sub PopulateVoltageTable(t As DataTable, dr As SqlDataReader, plateno As String)
        Dim i As Long = 1
        
        While dr.Read()
            Dim r As DataRow = t.NewRow()
            
            ' SECURITY: Sanitize all output data
            r(0) = i.ToString()
            r(1) = G2SecurityHelper.SanitizeForHtml(plateno)
            r(2) = G2SecurityHelper.SanitizeForHtml(dr("datetime").ToString())
            r(3) = G2SecurityHelper.SanitizeForHtml(dr("gps_av").ToString())
            
            ' SECURITY: Validate and format speed
            Dim speed As Double = 0
            If Not IsDBNull(dr("speed")) AndAlso Double.TryParse(dr("speed").ToString(), speed) Then
                r(4) = speed.ToString("0.00")
            Else
                r(4) = "0.00"
            End If
            
            ' SECURITY: Handle external voltage safely
            If IsDBNull(dr("externalbatv")) Then
                r(5) = "-"
            Else
                r(5) = G2SecurityHelper.SanitizeForHtml(dr("externalbatv").ToString())
            End If
            
            t.Rows.Add(r)
            i += 1
        End While
        
        ' Add empty row if no data
        If t.Rows.Count = 0 Then
            Dim r As DataRow = t.NewRow()
            For j As Integer = 0 To 5
                r(j) = "--"
            Next
            t.Rows.Add(r)
        End If
    End Sub
    
    Private Sub ShowErrorMessage(message As String)
        Dim t As New DataTable
        InitializeVoltageTable(t)
        
        Dim r As DataRow = t.NewRow()
        r(0) = "Error"
        r(1) = G2SecurityHelper.SanitizeForHtml(message)
        For i As Integer = 2 To 5
            r(i) = "--"
        Next
        t.Rows.Add(r)
        
        gv1.DataSource = t
        gv1.DataBind()
    End Sub

End Class