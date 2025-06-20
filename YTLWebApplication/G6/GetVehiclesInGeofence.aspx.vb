Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Collections.Generic

Public Class GetVehiclesInGeofence
    Inherits System.Web.UI.Page

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enhanced authentication check
            If Not IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GEOFENCE_VEHICLES_INIT_ERROR", ex.Message)
            Response.Redirect("~/Login.aspx")
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Add security headers
            AddSecurityHeaders()

            ' SECURITY FIX: Rate limiting
            If Not SecurityHelper.CheckRateLimit("GetVehiclesInGeofence_" & GetClientIP(), 50, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.ContentType = "text/plain"
                Response.Write("Rate limit exceeded")
                Response.End()
                Return
            End If

            Dim json As String = GetVehiclesData()
            
            Response.ContentType = "application/json"
            Response.Write(json)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GEOFENCE_VEHICLES_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.ContentType = "text/plain"
            Response.Write("Internal server error")
        End Try
    End Sub

    ' SECURITY FIX: Authentication check
    Private Function IsUserAuthenticated() As Boolean
        Try
            Return SecurityHelper.ValidateSession() AndAlso
                   HttpContext.Current.Session("userid") IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ' SECURITY FIX: Add security headers
    Private Sub AddSecurityHeaders()
        Response.Headers.Add("X-Content-Type-Options", "nosniff")
        Response.Headers.Add("X-Frame-Options", "DENY")
        Response.Headers.Add("X-XSS-Protection", "1; mode=block")
        Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate")
    End Sub

    ' SECURITY FIX: Get client IP safely
    Private Function GetClientIP() As String
        Try
            Dim ip As String = Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If String.IsNullOrEmpty(ip) Then
                ip = Request.ServerVariables("REMOTE_ADDR")
            End If
            Return ip
        Catch
            Return "Unknown"
        End Try
    End Function

    Private Function GetVehiclesData() As String
        Dim json As String = ""
        Try
            Dim aa As New ArrayList()
            
            ' SECURITY FIX: Validate and sanitize geofence IDs
            Dim gidParam As String = Request.QueryString("gid")
            If String.IsNullOrEmpty(gidParam) Then
                Return JsonConvert.SerializeObject(aa, Formatting.None)
            End If

            ' SECURITY FIX: Decode and validate geofence IDs
            Dim decodedGids As String = System.Uri.UnescapeDataString(gidParam)
            Dim gid As String() = decodedGids.Split(","c)
            
            ' Validate each geofence ID
            Dim validGids As New List(Of String)()
            For Each g As String In gid
                If SecurityHelper.ValidateInput(g.Trim(), "numeric") Then
                    validGids.Add(g.Trim())
                End If
            Next

            If validGids.Count = 0 Then
                Return JsonConvert.SerializeObject(aa, Formatting.None)
            End If

            ' SECURITY FIX: Get user data from session
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            ' Build secure query
            Dim query As String = BuildGeofenceQuery(role, userslist, validGids)
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@startTime", DateTime.Now.AddHours(-48).ToString("yyyy/MM/dd HH:mm:ss")},
                {"@endTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}
            }

            ' Add user-specific parameters
            If role = "User" Then
                parameters.Add("@userid", userid)
            End If

            Dim dataTable As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            For Each row As DataRow In dataTable.Rows
                Try
                    Dim a As New ArrayList()
                    a.Add(row("id"))
                    
                    ' SECURITY FIX: Safely handle PMID
                    Dim pmid As String = If(IsDBNull(row("pmid")) OrElse row("pmid").ToString() = "-", "", SecurityHelper.HtmlEncode(row("pmid").ToString()))
                    Dim plateno As String = SecurityHelper.HtmlEncode(row("plateno").ToString())
                    
                    If String.IsNullOrEmpty(pmid) Then
                        a.Add(plateno)
                    Else
                        a.Add($"{pmid}-{plateno}")
                    End If
                    
                    a.Add(DateTime.Parse(row("timestamp")).ToString("yyyy/MM/dd HH:mm:ss"))
                    a.Add(CDbl(row("lat")))
                    a.Add(CDbl(row("lon")))
                    aa.Add(a)
                    
                Catch ex As Exception
                    SecurityHelper.LogSecurityEvent("GEOFENCE_VEHICLE_ROW_ERROR", ex.Message)
                End Try
            Next

            json = JsonConvert.SerializeObject(aa, Formatting.None)

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GET_VEHICLES_DATA_ERROR", ex.Message)
            json = "[]"
        End Try

        Return json
    End Function

    ' SECURITY FIX: Build secure geofence query
    Private Function BuildGeofenceQuery(role As String, userslist As String, validGids As List(Of String)) As String
        ' Create safe IN clause for geofence IDs
        Dim geoInClause As String = String.Join(",", validGids)
        
        Dim baseQuery As String = "SELECT id, t1.plateno, ISNULL(t2.pmid, '-') AS pmid, intimestamp AS timestamp, inlat AS lat, inlon AS lon " &
                                 "FROM public_geofence_History t1 " &
                                 "LEFT OUTER JOIN vehicletbl t2 ON t1.plateno = t2.plateno " &
                                 $"WHERE id IN ({geoInClause}) " &
                                 "AND intimestamp BETWEEN @startTime AND @endTime " &
                                 "AND outtimestamp IS NULL"

        Select Case role
            Case "Admin"
                Return baseQuery
            Case "User"
                Return baseQuery & " AND t2.userid = @userid"
            Case "SuperUser", "Operator"
                If SecurityHelper.ValidateUsersList(userslist) Then
                    Return baseQuery & $" AND t2.userid IN ({userslist})"
                Else
                    ' Fallback to no results if userslist is invalid
                    Return baseQuery & " AND 1=0"
                End If
            Case Else
                ' Default to no results for unknown roles
                Return baseQuery & " AND 1=0"
        End Select
    End Function

End Class