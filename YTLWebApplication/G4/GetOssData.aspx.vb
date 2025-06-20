Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Collections

Public Class GetOssData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate authentication
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate and sanitize plate number parameter
            Dim plateno As String = SecurityHelper.SanitizeForHtml(Request.QueryString("plateno"))
            If Not String.IsNullOrEmpty(plateno) Then
                plateno = plateno.Trim()
                If Not SecurityHelper.ValidatePlateNumber(plateno) Then
                    Response.StatusCode = 400
                    Response.Write("{""error"":""Invalid plate number""}")
                    Return
                End If
            Else
                Response.StatusCode = 400
                Response.Write("{""error"":""Plate number required""}")
                Return
            End If

            ProcessOssData(plateno)

        Catch ex As Exception
            SecurityHelper.LogError("GetOssData Page_Load error", ex, Server)
            Response.StatusCode = 500
            Response.Write("{""error"":""An error occurred""}")
        End Try
    End Sub

    Private Sub ProcessOssData(plateno As String)
        Try
            Dim aa As New ArrayList
            Dim t As New DataTable

            t.Columns.Add(New DataColumn("SNo"))
            t.Columns.Add(New DataColumn("Source"))
            t.Columns.Add(New DataColumn("DN ID"))
            t.Columns.Add(New DataColumn("Product Type"))
            t.Columns.Add(New DataColumn("Area Code"))
            t.Columns.Add(New DataColumn("ShiptoCode"))
            t.Columns.Add(New DataColumn("ShiptoName"))
            t.Columns.Add(New DataColumn("WOT"))
            t.Columns.Add(New DataColumn("ATA"))

            Dim edt As DateTime = DateTime.Now
            Dim bdt As DateTime = edt.AddHours(-48)

            ' SECURITY FIX: Get geofence data with parameterized query
            Dim ShipToNameDict As New Dictionary(Of Integer, String)
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim shipToCodeQuery As String = "SELECT geofencename,shiptocode FROM geofence WHERE accesstype='1' ORDER BY LTRIM(geofencename)"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(shipToCodeQuery, conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Try
                                Dim shipToCode As Integer
                                If Integer.TryParse(dr("shiptocode").ToString(), shipToCode) AndAlso Not ShipToNameDict.ContainsKey(shipToCode) Then
                                    ShipToNameDict.Add(shipToCode, SecurityHelper.SanitizeForHtml(dr("geofencename").ToString().ToUpper()))
                                End If
                            Catch ex As Exception
                                SecurityHelper.LogError("Error processing geofence data", ex, Server)
                            End Try
                        End While
                    End Using
                    conn.Close()
                End Using
            End Using

            ' SECURITY FIX: Get OSS data with parameterized query
            Using conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                Dim query As String = "SELECT *,ISNULL(area_code_name,'-') as areaname,ISNULL(area_code,'0') as areacode FROM oss_patch_out WHERE weight_outtime BETWEEN @bdt AND @edt AND plateno = @plateno ORDER BY weight_outtime"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn2)
                    cmd.Parameters.AddWithValue("@bdt", bdt.ToString("yyyy/MM/dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@edt", edt.ToString("yyyy/MM/dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@plateno", plateno)

                    conn2.Open()
                    Using drI As SqlDataReader = cmd.ExecuteReader()
                        Dim ii As Integer = 1
                        While drI.Read()
                            Try
                                Dim r As DataRow = t.NewRow()
                                r(0) = ii.ToString()
                                r(1) = SecurityHelper.SanitizeForHtml(drI("source_supply").ToString().ToUpper())
                                r(2) = SecurityHelper.SanitizeForHtml(drI("dn_id").ToString().ToUpper())
                                r(3) = SecurityHelper.SanitizeForHtml(drI("productname").ToString().ToUpper())
                                r(4) = SecurityHelper.SanitizeForHtml(drI("areaname").ToString().ToUpper()) & " - " & SecurityHelper.SanitizeForHtml(drI("areacode").ToString().ToUpper())
                                r(5) = SecurityHelper.SanitizeForHtml(drI("destination_siteid").ToString().ToUpper())

                                If IsDBNull(drI("destination_sitename")) Then
                                    Dim destSiteId As Integer
                                    If Integer.TryParse(drI("destination_siteid").ToString(), destSiteId) AndAlso ShipToNameDict.ContainsKey(destSiteId) Then
                                        r(6) = ShipToNameDict(destSiteId).ToUpper()
                                    Else
                                        r(6) = "--"
                                    End If
                                Else
                                    Dim siteName As String = SecurityHelper.SanitizeForHtml(drI("destination_sitename").ToString().ToUpper())
                                    If drI("status").ToString() <> "2" Then
                                        r(6) = "<span style=''>" & siteName & "</span>"
                                    Else
                                        r(6) = "<span style='color:red;'>" & siteName & "</span>"
                                    End If
                                End If

                                If Not IsDBNull(drI("weight_outtime")) Then
                                    r(7) = Convert.ToDateTime(drI("weight_outtime")).ToString("yyyy/MM/dd HH:mm:ss")
                                Else
                                    r(7) = "--"
                                End If

                                If Not IsDBNull(drI("ata_datetime")) Then
                                    r(8) = Convert.ToDateTime(drI("ata_datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                                Else
                                    r(8) = "--"
                                End If

                                t.Rows.Add(r)
                                ii += 1
                            Catch ex As Exception
                                SecurityHelper.LogError("Error processing OSS record", ex, Server)
                            End Try
                        End While
                    End Using
                    conn2.Close()
                End Using
            End Using

            ' Convert DataTable to ArrayList for JSON serialization
            For c As Integer = 0 To t.Rows.Count - 1
                Dim a As New ArrayList()
                For i As Integer = 0 To 8
                    a.Add(t.DefaultView.Item(c)(i))
                Next
                aa.Add(a)
            Next

            If t.Rows.Count = 0 Then
                Dim a As New ArrayList()
                For i As Integer = 0 To 8
                    a.Add("--")
                Next
                aa.Add(a)
            End If

            Dim json As String = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
            Response.ContentType = "application/json"
            Response.Write(json)

        Catch ex As Exception
            SecurityHelper.LogError("ProcessOssData error", ex, Server)
            Response.StatusCode = 500
            Response.Write("{""error"":""Data processing failed""}")
        End Try
    End Sub

End Class