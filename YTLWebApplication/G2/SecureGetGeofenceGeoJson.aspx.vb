Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports AspMap

Public Class SecureGetGeofenceGeoJson
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
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
            If Not G2SecurityHelper.CheckRateLimit("GeofenceGeoJson_" & clientIP, 60, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.Write("Too Many Requests")
                Response.End()
                Return
            End If
            
            ' SECURITY: Validate geofence ID parameter
            Dim geofenceid As String = Request.QueryString("gid")
            If Not G2SecurityHelper.ValidateG2Input(geofenceid, G2InputType.GeofenceId) Then
                Response.StatusCode = 400
                Response.Write("Invalid geofence ID")
                Response.End()
                Return
            End If
            
            Dim json As String = GetSecureGeofenceGeoJson(geofenceid)
            Response.Write(json)
            Response.ContentType = "application/json"
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("GEOFENCE_GEOJSON_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("Internal Server Error")
        End Try
    End Sub
    
    Private Function GetSecureGeofenceGeoJson(geofenceid As String) As String
        Try
            Dim aa As New ArrayList()
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ' SECURITY: Use parameterized query
                Dim query As String = "SELECT * FROM geofence WHERE geofenceid = @geofenceid"
                Using cmd As SqlCommand = G2SecurityHelper.CreateSecureCommand(query, conn)
                    cmd.Parameters.AddWithValue("@geofenceid", geofenceid)
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Try
                                Dim a As New ArrayList()
                                
                                ' SECURITY: Validate and sanitize access type
                                Dim at As Integer = 0
                                Dim accessType As String = dr("accesstype").ToString()
                                Select Case accessType
                                    Case "1"
                                        at = 1
                                    Case "0"
                                        at = 0
                                    Case Else
                                        at = 2
                                End Select
                                
                                ' SECURITY: Validate status
                                Dim status As Byte = 0
                                If Not IsDBNull(dr("status")) AndAlso CBool(dr("status")) Then
                                    status = 1
                                End If
                                
                                a.Add(status)
                                a.Add(at)
                                a.Add(Convert.ToUInt32(dr("geofenceid")))
                                a.Add(G2SecurityHelper.SanitizeForHtml(dr("geofencename").ToString()))
                                
                                ' SECURITY: Validate and process geofence data
                                Dim geofenceData As String = dr("data").ToString()
                                If ValidateGeofenceData(geofenceData) Then
                                    a.Add(G2SecurityHelper.SanitizeForHtml(geofenceData))
                                    
                                    ' Calculate centroid safely
                                    Dim centroid As PointF = CalculateSafeCentroid(geofenceData)
                                    a.Add(centroid.Y)
                                    a.Add(centroid.X)
                                Else
                                    ' Invalid geofence data
                                    a.Add("")
                                    a.Add(0)
                                    a.Add(0)
                                End If
                                
                                aa.Add(a)
                                
                            Catch ex As Exception
                                G2SecurityHelper.LogSecurityEvent("PROCESS_GEOFENCE_RECORD_ERROR", ex.Message)
                                ' Continue with next record
                            End Try
                        End While
                    End Using
                End Using
            End Using
            
            Return JsonConvert.SerializeObject(aa, Formatting.None)
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("GET_SECURE_GEOFENCE_GEOJSON_ERROR", ex.Message)
            Return JsonConvert.SerializeObject(New ArrayList())
        End Try
    End Function
    
    Private Function ValidateGeofenceData(geofenceData As String) As Boolean
        Try
            If String.IsNullOrEmpty(geofenceData) Then
                Return False
            End If
            
            ' SECURITY: Validate geofence data format
            Dim points() As String = geofenceData.Split(";"c)
            
            For Each point As String In points
                If Not String.IsNullOrEmpty(point) Then
                    Dim coords() As String = point.Split(","c)
                    If coords.Length <> 2 Then
                        Return False
                    End If
                    
                    ' Validate coordinates are numeric and within valid ranges
                    Dim lat, lon As Double
                    If Not Double.TryParse(coords(0), lat) OrElse Not Double.TryParse(coords(1), lon) Then
                        Return False
                    End If
                    
                    ' Validate coordinate ranges (basic validation)
                    If lat < -90 OrElse lat > 90 OrElse lon < -180 OrElse lon > 180 Then
                        Return False
                    End If
                End If
            Next
            
            Return True
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("VALIDATE_GEOFENCE_DATA_ERROR", ex.Message)
            Return False
        End Try
    End Function
    
    Private Function CalculateSafeCentroid(geofenceData As String) As PointF
        Try
            Dim polygonShape As New AspMap.Shape
            polygonShape.ShapeType = ShapeType.mcPolygonShape
            
            Dim shpPoints As New AspMap.Points()
            Dim points() As String = geofenceData.Split(";"c)
            
            For Each point As String In points
                If Not String.IsNullOrEmpty(point) Then
                    Dim coords() As String = point.Split(","c)
                    If coords.Length = 2 Then
                        Dim lat, lon As Double
                        If Double.TryParse(coords(0), lat) AndAlso Double.TryParse(coords(1), lon) Then
                            shpPoints.AddPoint(lat, lon)
                        End If
                    End If
                End If
            Next
            
            If shpPoints.Count > 0 Then
                Return New PointF(CSng(shpPoints.Centroid.X), CSng(shpPoints.Centroid.Y))
            Else
                Return New PointF(0, 0)
            End If
            
        Catch ex As Exception
            G2SecurityHelper.LogSecurityEvent("CALCULATE_CENTROID_ERROR", ex.Message)
            Return New PointF(0, 0)
        End Try
    End Function

End Class