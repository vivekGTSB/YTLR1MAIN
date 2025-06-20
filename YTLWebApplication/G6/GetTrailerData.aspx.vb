Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.IO.Compression
Imports System.Web.Caching

Partial Class GetTrailerData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Authentication check
            If Not IsUserAuthenticated() Then
                Response.StatusCode = 401
                Response.Write("Unauthorized")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Add security headers
            AddSecurityHeaders()

            ' SECURITY FIX: Rate limiting
            If Not SecurityHelper.CheckRateLimit("GetTrailerData_" & GetClientIP(), 50, TimeSpan.FromMinutes(1)) Then
                Response.StatusCode = 429
                Response.Write("Rate limit exceeded")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Get and validate parameters
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
            
            Dim quserid As String = ValidateQueryParameter("u")
            Dim opr As String = ValidateQueryParameter("opr")
            Dim trailerid As String = ValidateQueryParameter("trailerid")
            Dim trailerno As String = ValidateQueryParameter("trailerno")
            Dim tid As String = ValidateQueryParameter("tid")
            Dim ugdata As String = ValidateQueryParameter("ugData")
            Dim ddata As String = ValidateQueryParameter("ddata")

            ' Parse user ID if it contains comma
            If Not String.IsNullOrEmpty(quserid) AndAlso quserid.IndexOf(",") > 0 Then
                Dim sgroupname As String() = quserid.Split(","c)
                If sgroupname.Length > 0 AndAlso SecurityHelper.ValidateInput(sgroupname(0), "username") Then
                    quserid = sgroupname(0)
                Else
                    quserid = userid
                End If
            End If

            ' Route to appropriate operation
            If opr = "0" Then
                Response.Write(AddTrailer(trailerid, trailerno, quserid, opr))
            ElseIf opr = "1" Then
                Response.Write(UpdateTrailer(trailerid, trailerno, quserid, tid, opr))
            ElseIf ddata = "0" Then
                Response.Write(DeleteTrailer(ugdata))
            Else
                Response.Write(GetJson(quserid, role, userslist))
            End If

            Response.ContentType = "application/json"

            ' Add compression if supported
            If Request.Headers("Accept-Encoding") IsNot Nothing AndAlso Request.Headers("Accept-Encoding").ToLower().Contains("gzip") Then
                Response.AppendHeader("Content-Encoding", "gzip")
                Response.Filter = New GZipStream(Response.Filter, CompressionMode.Compress)
            End If

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("TRAILER_DATA_ERROR", ex.Message)
            Response.StatusCode = 500
            Response.Write("{""error"":""Internal server error""}")
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

    ' SECURITY FIX: Validate query parameters
    Private Function ValidateQueryParameter(paramName As String) As String
        Dim value As String = Request.QueryString(paramName)
        If String.IsNullOrEmpty(value) Then
            Return String.Empty
        End If

        ' Basic validation based on parameter type
        Select Case paramName.ToLower()
            Case "u", "userid"
                If SecurityHelper.ValidateInput(value, "username") Then
                    Return value
                End If
            Case "opr", "tid"
                If SecurityHelper.ValidateInput(value, "numeric") Then
                    Return value
                End If
            Case "trailerid", "trailerno"
                If value.Length <= 50 AndAlso Not SecurityHelper.ContainsDangerousPatterns(value) Then
                    Return value
                End If
            Case "ugdata", "ddata"
                If value.Length <= 1000 AndAlso Not SecurityHelper.ContainsDangerousPatterns(value) Then
                    Return value
                End If
        End Select

        Return String.Empty
    End Function

    Protected Function GetJson(suserid As String, role As String, userslist As String) As String
        Dim json As String = Nothing
        Try
            Dim trailertable As New DataTable()
            trailertable.Columns.Add(New DataColumn("chk"))
            trailertable.Columns.Add(New DataColumn("No"))
            trailertable.Columns.Add(New DataColumn("Trailer ID"))
            trailertable.Columns.Add(New DataColumn("Trailer No"))
            trailertable.Columns.Add(New DataColumn("Modify DateTime"))

            If suserid <> "SELECT USERNAME" Then
                ' SECURITY FIX: Build secure query
                Dim query As String = BuildTrailerQuery(suserid, role, userslist)
                Dim parameters As New Dictionary(Of String, Object)()
                
                ' Add parameters based on query type
                If suserid <> "ALL" AndAlso (role = "User" OrElse role = "Admin") Then
                    parameters.Add("@userid", suserid)
                End If

                Dim dataTable As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
                
                Dim j As Integer = 1
                For Each row As DataRow In dataTable.Rows
                    Dim r As DataRow = trailertable.NewRow()
                    r(0) = SecurityHelper.HtmlEncode(row("trailerid").ToString())
                    r(1) = j.ToString()
                    
                    ' SECURITY FIX: Encode data for JavaScript
                    Dim encodedId As String = SecurityHelper.SanitizeForJavaScript(row("id").ToString())
                    Dim encodedTrailerId As String = SecurityHelper.SanitizeForJavaScript(row("trailerid").ToString())
                    Dim encodedTrailerNo As String = SecurityHelper.SanitizeForJavaScript(row("trailerno").ToString())
                    Dim encodedUserId As String = SecurityHelper.SanitizeForJavaScript(row("userid").ToString())
                    
                    r(2) = $"<span style=""cursor:pointer;text-decoration:underline;"" onclick=""javascript:openUpdatePopup('{encodedId}','{encodedTrailerId}','{encodedTrailerNo}','{encodedUserId}')"">{SecurityHelper.HtmlEncode(row("trailerid").ToString())}</span>"
                    r(3) = SecurityHelper.HtmlEncode(row("trailerno").ToString())
                    
                    If Not IsDBNull(row("modifieddatetime")) Then
                        Dim modifiedDate As DateTime = DateTime.Parse(row("modifieddatetime"))
                        r(4) = modifiedDate.ToString("yyyy/MM/dd HH:mm:ss")
                    Else
                        r(4) = "--"
                    End If
                    
                    trailertable.Rows.Add(r)
                    j += 1
                Next
            End If

            If trailertable.Rows.Count = 0 Then
                Dim r As DataRow = trailertable.NewRow()
                For i As Integer = 0 To 4
                    r(i) = "--"
                Next
                trailertable.Rows.Add(r)
            End If

            Dim aa As New ArrayList()
            For j1 As Integer = 0 To trailertable.Rows.Count - 1
                Try
                    Dim a As New ArrayList()
                    For col As Integer = 0 To 4
                        a.Add(trailertable.Rows(j1)(col))
                    Next
                    aa.Add(a)
                Catch ex As Exception
                    SecurityHelper.LogSecurityEvent("TRAILER_ROW_ERROR", ex.Message)
                End Try
            Next

            json = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"

            ' Store for Excel export
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session.Remove("exceltable3")
            trailertable.Columns.Remove("chk")
            HttpContext.Current.Session("exceltable") = trailertable
            HttpContext.Current.Session.Remove("tempTable")

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GET_JSON_ERROR", ex.Message)
            json = "{""error"":""Error retrieving data""}"
        End Try

        Return json
    End Function

    ' SECURITY FIX: Build secure trailer query
    Private Function BuildTrailerQuery(suserid As String, role As String, userslist As String) As String
        Dim baseQuery As String = "SELECT t.id, t.trailerid, t.trailerno, t.userid, t.createddatetime, t.modifieddatetime FROM trailer2 AS t"
        
        If suserid = "ALL" Then
            If role = "SuperUser" OrElse role = "Operator" Then
                Return baseQuery & " WHERE t.userid IN (" & userslist & ") ORDER BY t.trailerid ASC"
            Else
                Return baseQuery & " JOIN userTBL AS a ON a.userid = t.userid WHERE a.role = 'User' ORDER BY t.trailerid ASC"
            End If
        Else
            Return baseQuery & " WHERE t.userid = @userid ORDER BY t.trailerid ASC"
        End If
    End Function

    Protected Function AddTrailer(ByVal trid As String, ByVal trno As String, uid As String, ByVal opr As String) As String
        Dim json As String = ""
        Dim result As Integer = 0
        
        Try
            ' SECURITY FIX: Validate input parameters
            If Not ValidateTrailerInput(trid, trno, uid) Then
                Return "{""result"":2,""error"":""Invalid input parameters""}"
            End If

            If opr = "0" Then
                Dim parameters As New Dictionary(Of String, Object) From {
                    {"@userid", uid},
                    {"@trailerid", trid},
                    {"@trailerno", trno},
                    {"@datetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
                }

                Dim query As String = "INSERT INTO trailer2 (userid, trailerid, trailerno, createddatetime, modifieddatetime) VALUES (@userid, @trailerid, @trailerno, @datetime, @datetime)"
                
                result = SecurityHelper.ExecuteSecureNonQuery(query, parameters)
                
                SecurityHelper.LogSecurityEvent("TRAILER_INSERT", $"Trailer ID: {trid}, User: {uid}")
            End If

            json = "{""result"":" & JsonConvert.SerializeObject(result, Formatting.None) & "}"

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("ADD_TRAILER_ERROR", ex.Message)
            result = 2
            json = "{""result"":" & JsonConvert.SerializeObject(result, Formatting.None) & ",""error"":""Database error""}"
        End Try

        Return json
    End Function

    Protected Function UpdateTrailer(ByVal trid As String, ByVal trno As String, ByVal uid As String, ByVal tid As String, ByVal opr As String) As String
        Dim json As String = ""
        Dim result As Integer = 0
        
        Try
            ' SECURITY FIX: Validate input parameters
            If Not ValidateTrailerInput(trid, trno, uid) OrElse Not SecurityHelper.ValidateInput(tid, "numeric") Then
                Return "{""result"":2,""error"":""Invalid input parameters""}"
            End If

            If opr = "1" Then
                Dim parameters As New Dictionary(Of String, Object) From {
                    {"@userid", uid},
                    {"@trailerid", trid},
                    {"@trailerno", trno},
                    {"@datetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                    {"@id", tid}
                }

                Dim query As String = "UPDATE trailer2 SET userid = @userid, trailerid = @trailerid, trailerno = @trailerno, modifieddatetime = @datetime WHERE id = @id"
                
                result = SecurityHelper.ExecuteSecureNonQuery(query, parameters)
                
                SecurityHelper.LogSecurityEvent("TRAILER_UPDATE", $"ID: {tid}, Trailer ID: {trid}, User: {uid}")
            End If

            json = "{""result"":" & JsonConvert.SerializeObject(result, Formatting.None) & "}"

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("UPDATE_TRAILER_ERROR", ex.Message)
            result = 2
            json = "{""result"":" & JsonConvert.SerializeObject(result, Formatting.None) & ",""error"":""Database error""}"
        End Try

        Return json
    End Function

    Protected Function DeleteTrailer(ByVal udata As String) As String
        Dim result As Integer = 0
        Dim json As String = ""
        
        Try
            ' SECURITY FIX: Validate and sanitize input
            If String.IsNullOrEmpty(udata) OrElse udata.Length > 1000 Then
                Return "{""result"":0,""error"":""Invalid input""}"
            End If

            Dim trailers() As String = udata.Split(","c)
            Dim deletedCount As Integer = 0

            For Each trailer As String In trailers
                Dim cleanTrailer As String = trailer.Trim()
                
                If cleanTrailer <> "on" AndAlso Not String.IsNullOrEmpty(cleanTrailer) Then
                    ' SECURITY FIX: Validate trailer ID
                    If SecurityHelper.ValidateInput(cleanTrailer, "username") Then
                        Try
                            Dim parameters As New Dictionary(Of String, Object) From {
                                {"@trailerid", cleanTrailer}
                            }

                            Dim query As String = "DELETE FROM trailer2 WHERE trailerid = @trailerid"
                            Dim deleteResult As Integer = SecurityHelper.ExecuteSecureNonQuery(query, parameters)
                            
                            If deleteResult > 0 Then
                                deletedCount += 1
                                result = 1
                            End If
                            
                        Catch ex As Exception
                            SecurityHelper.LogSecurityEvent("DELETE_TRAILER_ITEM_ERROR", $"Trailer: {cleanTrailer}, Error: {ex.Message}")
                        End Try
                    End If
                End If
            Next

            SecurityHelper.LogSecurityEvent("TRAILER_DELETE", $"Deleted {deletedCount} trailers")
            
            json = "{""result"":" & JsonConvert.SerializeObject(result, Formatting.None) & "}"

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("DELETE_TRAILER_ERROR", ex.Message)
            result = 0
            json = "{""result"":" & JsonConvert.SerializeObject(result, Formatting.None) & ",""error"":""Delete operation failed""}"
        End Try

        Return json
    End Function

    ' SECURITY FIX: Validate trailer input
    Private Function ValidateTrailerInput(trid As String, trno As String, uid As String) As Boolean
        Try
            ' Validate trailer ID
            If String.IsNullOrEmpty(trid) OrElse trid.Length > 50 Then
                Return False
            End If

            If SecurityHelper.ContainsDangerousPatterns(trid) Then
                Return False
            End If

            ' Validate trailer number
            If String.IsNullOrEmpty(trno) OrElse trno.Length > 50 Then
                Return False
            End If

            If SecurityHelper.ContainsDangerousPatterns(trno) Then
                Return False
            End If

            ' Validate user ID
            If Not SecurityHelper.ValidateInput(uid, "username") Then
                Return False
            End If

            Return True

        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("VALIDATE_TRAILER_INPUT_ERROR", ex.Message)
            Return False
        End Try
    End Function

End Class