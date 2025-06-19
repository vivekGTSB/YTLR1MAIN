Imports System.Data.SqlClient
Imports System.Text

Public Class buildcongeos
    Inherits SecureBasePage

    Public sb As String = ""
    Public ec As String = "false"
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' Validate and sanitize input parameters
            Dim plateno As String = ValidateInput(Request.QueryString("p"), "plateno")
            Dim weightout As String = ValidateInput(Request.QueryString("w"), "datetime")
            
            If String.IsNullOrEmpty(plateno) OrElse String.IsNullOrEmpty(weightout) Then
                sb = "<tr><td colspan='6'>Invalid parameters provided</td></tr>"
                Return
            End If
            
            LoadGeofenceData(plateno, weightout)
            
        Catch ex As Exception
            LogError(ex)
            sb = "<tr><td colspan='6'>An error occurred while loading data</td></tr>"
        End Try
    End Sub
    
    Private Sub LoadGeofenceData(plateno As String, weightout As String)
        Dim sbr As New StringBuilder()
        Dim visits As New List(Of GeofenceData)
        
        Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Try
                Using cmd As New SqlCommand("sp_GetbuildconGeofenceData", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    
                    ' Use parameterized queries to prevent SQL injection
                    cmd.Parameters.Add("@weightfromtimes", SqlDbType.DateTime).Value = Convert.ToDateTime(weightout)
                    cmd.Parameters.Add("@plateno", SqlDbType.VarChar, 50).Value = plateno
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim g As New GeofenceData()
                            g.dnno = SafeGetString(dr, "dnno")
                            g.duration = SafeGetInt(dr, "duration")
                            g.intime = SafeGetDateTime(dr, "intime").ToString("yyyy/MM/dd HH:mm:ss")
                            g.outtime = If(IsDBNull(dr("outtime")), "-", SafeGetDateTime(dr, "outtime").ToString("yyyy/MM/dd HH:mm:ss"))
                            g.name = SafeGetString(dr, "geofencename").ToUpper()
                            g.plate = SafeGetString(dr, "plateno").ToUpper()
                            visits.Add(g)
                        End While
                    End Using
                End Using
                
                ' Build safe HTML output
                For Each g In visits
                    sbr.Append("<tr>")
                    sbr.Append($"<td>{SafeOutput(g.dnno)}</td>")
                    sbr.Append($"<td>{SafeOutput(g.name)}</td>")
                    sbr.Append($"<td>{SafeOutput(g.plate)}</td>")
                    sbr.Append($"<td>{SafeOutput(g.intime)}</td>")
                    sbr.Append($"<td>{SafeOutput(g.outtime)}</td>")
                    sbr.Append($"<td>{g.duration} Mins</td>")
                    sbr.Append("</tr>")
                Next
                
                sb = sbr.ToString()
                
            Catch ex As Exception
                LogError(ex)
                Throw New ApplicationException("Database error occurred")
            End Try
        End Using
    End Sub
    
    Private Function ValidateInput(input As String, inputType As String) As String
        If String.IsNullOrWhiteSpace(input) Then Return String.Empty
        
        Select Case inputType.ToLower()
            Case "plateno"
                If Not System.Text.RegularExpressions.Regex.IsMatch(input, "^[A-Za-z0-9\-\s]{1,20}$") Then
                    Throw New ArgumentException("Invalid plate number format")
                End If
            Case "datetime"
                Dim dateValue As DateTime
                If Not DateTime.TryParse(input, dateValue) Then
                    Throw New ArgumentException("Invalid date format")
                End If
        End Select
        
        Return input.Trim()
    End Function
    
    Private Function SafeGetString(dr As SqlDataReader, columnName As String) As String
        Return If(IsDBNull(dr(columnName)), String.Empty, dr(columnName).ToString())
    End Function
    
    Private Function SafeGetInt(dr As SqlDataReader, columnName As String) As Integer
        Return If(IsDBNull(dr(columnName)), 0, Convert.ToInt32(dr(columnName)))
    End Function
    
    Private Function SafeGetDateTime(dr As SqlDataReader, columnName As String) As DateTime
        Return If(IsDBNull(dr(columnName)), DateTime.MinValue, Convert.ToDateTime(dr(columnName)))
    End Function
    
    Private Sub LogError(ex As Exception)
        ' Implement secure logging
        System.Diagnostics.EventLog.WriteEntry("YTLWebApp", $"Error in buildcongeos: {ex.Message}", System.Diagnostics.EventLogEntryType.Error)
    End Sub

    Public Class GeofenceData
        Public Property dnno As String
        Public Property name As String
        Public Property plate As String
        Public Property intime As String
        Public Property outtime As String
        Public Property duration As Integer
    End Class
End Class