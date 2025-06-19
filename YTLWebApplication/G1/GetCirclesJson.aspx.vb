Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetCirclesJson
    Inherits SecurePageBase

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.Write("Unauthorized")
                Response.End()
                Return
            End If

            Response.Write(GetSecureGeofenceData())
            Response.ContentType = "application/json"
            
        Catch ex As Exception
            SecurityHelper.LogError("GetCirclesJson error", ex, Server)
            Response.StatusCode = 500
            Response.Write("{""error"":""An error occurred""}")
        End Try
    End Sub

    ' SECURITY FIX: Secure geofence data retrieval
    Private Function GetSecureGeofenceData() As String
        Try
            Dim userId As String = SessionManager.GetCurrentUserId()
            Dim userRole As String = SessionManager.GetCurrentUserRole()
            Dim userslist As String = If(Session("userslist"), "").ToString()
            Dim la As String = If(Session("LA"), "N").ToString()
            
            ' SECURITY FIX: Build secure query based on role
            Dim query As String
            Dim parameters As New Dictionary(Of String, Object)
            
            Select Case userRole
                Case "User"
                    query = "SELECT geofencename, geofenceid, data, accesstype, status " &
                           "FROM geofence WHERE geofencetype = '0' AND (userid = @userId OR accesstype = '1') AND active = 1"
                    parameters.Add("@userId", userId)
                    
                Case "SuperUser", "Operator"
                    If la = "Y" Then
                        query = "SELECT geofencename, geofenceid, data, accesstype, status " &
                               "FROM geofence WHERE geofencetype = '0' AND (accesstype IN ('1', '2') OR userid IN (SELECT value FROM STRING_SPLIT(@usersList, ','))) AND active = 1"
                        parameters.Add("@usersList", SecurityHelper.ValidateAndGetUsersList(Request))
                    Else
                        query = "SELECT geofencename, geofenceid, data, accesstype, status " &
                               "FROM geofence WHERE geofencetype = '0' AND (accesstype = '1' OR userid IN (SELECT value FROM STRING_SPLIT(@usersList, ','))) AND active = 1"
                        parameters.Add("@usersList", SecurityHelper.ValidateAndGetUsersList(Request))
                    End If
                    
                Case "Admin"
                    query = "SELECT geofencename, geofenceid, data, accesstype, status " &
                           "FROM geofence WHERE geofencetype = '0' AND active = 1"
                    
                Case Else
                    Return "{""error"":""Invalid user role""}"
            End Select
            
            Dim dataTable As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)
            Dim resultList As New List(Of List(Of Object))
            
            For Each row As DataRow In dataTable.Rows
                Try
                    Dim accessType As Integer = If(row("accesstype").ToString() = "1", 1, If(row("accesstype").ToString() = "0", 0, 2))
                    Dim status As Integer = If(CBool(row("status")), 1, 0)
                    
                    ' SECURITY FIX: Validate and parse coordinates
                    Dim coordinates As String() = row("data").ToString().Split(","c)
                    If coordinates.Length >= 3 Then
                        Dim latitude As Double
                        Dim longitude As Double
                        Dim radius As Integer
                        
                        If Double.TryParse(coordinates(1), latitude) AndAlso 
                           Double.TryParse(coordinates(0), longitude) AndAlso 
                           Integer.TryParse(coordinates(2), radius) Then
                            
                            ' Validate coordinate ranges
                            If latitude >= -90 AndAlso latitude <= 90 AndAlso 
                               longitude >= -180 AndAlso longitude <= 180 AndAlso 
                               radius > 0 AndAlso radius <= 50000 Then
                                
                                Dim rowData As New List(Of Object) From {
                                    status,
                                    accessType,
                                    Convert.ToUInt32(row("geofenceid")),
                                    SecurityHelper.SanitizeForHtml(row("geofencename").ToString()),
                                    latitude,
                                    longitude,
                                    radius
                                }
                                resultList.Add(rowData)
                            End If
                        End If
                    End If
                    
                Catch ex As Exception
                    SecurityHelper.LogError("Error processing geofence row", ex, Server)
                    Continue For
                End Try
            Next
            
            Return JsonConvert.SerializeObject(resultList)
            
        Catch ex As Exception
            SecurityHelper.LogError("GetSecureGeofenceData failed", ex, Server)
            Return "{""error"":""Geofence data retrieval failed""}"
        End Try
    End Function
End Class