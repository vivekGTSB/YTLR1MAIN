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
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim role As String = Request.Cookies("userinfo")("role")
        Dim userslist As String = Request.Cookies("userinfo")("userslist")
        Dim quserid As String = Request.QueryString("u")
        Dim opr As String = Request.QueryString("opr")
        Dim trailerid As String = Request.QueryString("trailerid")
        Dim trailerno As String = Request.QueryString("trailerno")
        Dim tid As String = Request.QueryString("tid")
        Dim ugdata As String = Request.QueryString("ugData")
        Dim ddata As String = Request.QueryString("ddata")
        Try
            If quserid.IndexOf(",") > 0 Then
                Dim sgroupname As String() = quserid.Split(",")
                quserid = sgroupname(0).ToString()
            End If
        Catch ex As Exception

        End Try

        If opr = "0" Then
            Response.Write(AddTrailer(trailerid, trailerno, quserid, opr))
            Response.ContentType = "text/plain"
        ElseIf opr = "1" Then
            Response.Write(updateTrailer(trailerid, trailerno, quserid, tid, opr))
            Response.ContentType = "text/plain"
        ElseIf ddata = "0" Then
            Response.Write(deleteTrailer(ugdata))
            Response.ContentType = "text/plain"
        Else
            Response.Write(GetJson())
            Response.ContentType = "text/plain"
        End If
        If (Request.Headers("Accept-Encoding").ToLower().Contains("gzip")) Then
            Response.AppendHeader("Content-Encoding", "gzip")
            Response.Filter = New GZipStream(Response.Filter, CompressionMode.Compress)
        End If

    End Sub

    Protected Function GetJson() As String
        Dim json As String = Nothing

        Try
            Dim suserid As String = Request.QueryString("u")
            Dim role As String = Request.QueryString("role")
            Dim userslist As String = Request.QueryString("userslist")
            Dim r As DataRow
            Dim j As Int32 = 1

            Dim trailertable As New DataTable
            trailertable.Columns.Add(New DataColumn("chk"))
            trailertable.Columns.Add(New DataColumn("No"))
            trailertable.Columns.Add(New DataColumn("Trailer ID"))
            trailertable.Columns.Add(New DataColumn("Trailer No"))
            trailertable.Columns.Add(New DataColumn("Modify DateTime"))
            If Not suserid = "SELECT USERNAME" Then
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select id,trailerid,trailerno,userid,createddatetime,modifieddatetime from  trailer2 where userid =  '" & suserid & "' order by trailerid asc", conn)
                Dim dr As SqlDataReader

                If suserid = "ALL" Then
                    cmd = New SqlCommand("select t.id,t.trailerid,t.trailerno,t.userid,t.createddatetime,t.modifieddatetime from trailer2 as t,userTBL as a where a.userid = t.userid and a.role='User' order by t.trailerid asc", conn)
                    If role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select t.id,t.trailerid,t.trailerno,t.userid,t.createddatetime,t.modifieddatetime from trailer2 as t,userTBL as a  where t.userid in (" & userslist & ") and a.userid = t.userid order by t.trailerid asc", conn)
                    End If

                End If
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read
                    r = trailertable.NewRow
                    r(0) = dr("trailerid")
                    r(1) = j.ToString()
                    r(2) = "<span style=""cursor:pointer;text-decoration:underline;"" onclick=""javascript:openUpdatePopup('" & dr("id").ToString & "','" & dr("trailerid").ToString & "','" & dr("trailerno").ToString & "','" & dr("userid").ToString & "')"">" & dr("trailerid").ToString & "</span>"
                    r(3) = dr("trailerno").ToString
                    If IsDBNull(dr("modifieddatetime")) = False Then
                        Dim p = DateTime.Parse(dr("modifieddatetime")).ToString("yyyy/MM/dd  HH:mm:ss")
                        p = p.Replace("-", "/")
                        r(4) = p
                    Else
                        r(4) = "--"
                    End If
                    trailertable.Rows.Add(r)
                    j = j + 1
                End While

                conn.Close()

            End If

            If trailertable.Rows.Count = 0 Then
                r = trailertable.NewRow
                r(0) = "--"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                trailertable.Rows.Add(r)
            End If

            Dim aa As New ArrayList
            Dim a As ArrayList

            For j1 As Integer = 0 To trailertable.Rows.Count - 1
                Try
                    a = New ArrayList
                    a.Add(trailertable.DefaultView.Item(j1)(0))
                    a.Add(trailertable.DefaultView.Item(j1)(1))
                    a.Add(trailertable.DefaultView.Item(j1)(2))
                    a.Add(trailertable.DefaultView.Item(j1)(3))
                    a.Add(trailertable.DefaultView.Item(j1)(4))
                    aa.Add(a)
                Catch ex As Exception

                End Try
            Next
            json = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
            ' Literal41--loac
            HttpContext.Current.Session.Remove("exceltable")
            HttpContext.Current.Session.Remove("exceltable2")
            HttpContext.Current.Session.Remove("exceltable3")
            trailertable.Columns.Remove("chk")
            HttpContext.Current.Session("exceltable") = trailertable
            HttpContext.Current.Session.Remove("tempTable")

        Catch ex As Exception

        End Try
        Return json
    End Function

    Protected Function AddTrailer(ByVal trid As String, ByVal trno As String, uid As String, ByVal opr As String) As String
        Dim json As String = ""
        Dim strqury As String = ""
        Dim result As Int16 = 0
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            If opr = 0 Then
                strqury = "Insert into trailer2 (userid,trailerid,trailerno,createddatetime,modifieddatetime) values ('" & uid & "','" & trid & "','" & trno & "','" & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & "','" & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & "')"
            End If
            Dim cmd As New SqlCommand(strqury, conn)
            conn.Open()
            result = cmd.ExecuteNonQuery()
            conn.Close()
            json = "{""result"":" & JsonConvert.SerializeObject(result, Formatting.None) & "}"
        Catch ex As Exception
            result = 2
            json = "{""result"":" & JsonConvert.SerializeObject(result, Formatting.None) & "}"
        End Try
        Return json
    End Function

    Protected Function updateTrailer(ByVal trid As String, ByVal trno As String, ByVal uid As String, ByVal tid As String, ByVal opr As String) As String
        Dim json As String = ""
        Dim strqury As String = ""
        Dim result As Int16 = 0
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            If opr = 1 Then
                strqury = "update  trailer2 set userid='" & uid & "',trailerid='" & trid & "',trailerno='" & trno & "',modifieddatetime='" & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & "' where id='" & tid & "' "
            End If
            Dim cmd As New SqlCommand(strqury, conn)
            conn.Open()
            result = cmd.ExecuteNonQuery()
            conn.Close()
            json = "{""result"":" & JsonConvert.SerializeObject(result, Formatting.None) & "}"
        Catch ex As Exception
            result = 2
            json = "{""result"":" & JsonConvert.SerializeObject(result, Formatting.None) & "}"
        End Try
        Return json
    End Function

    Protected Function deleteTrailer(ByVal udata As String) As String
        Dim result As Integer = 0
        Dim json As String = ""
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim trailers() As String = udata.Split(",")

            For i As Int32 = 0 To trailers.Length - 1
                If trailers(i) = "on" Then
                Else
                    cmd = New SqlCommand("delete from trailer2 where trailerid='" & trailers(i) & "'", conn)
                    conn.Open()
                    result = cmd.ExecuteNonQuery()
                End If
                conn.Close()
            Next
            json = "{""result"":" & JsonConvert.SerializeObject(result, Formatting.None) & "}"
        Catch ex As Exception
            result = 0
            json = "{""result"":" & JsonConvert.SerializeObject(result, Formatting.None) & "}"
        End Try
        Return result
    End Function
End Class
