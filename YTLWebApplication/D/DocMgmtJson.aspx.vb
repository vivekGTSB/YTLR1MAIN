Imports System.Data.SqlClient 
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports ASPNetMultiLanguage
Partial Class DocMgmtJson
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim opr As String = Request.QueryString("opr")
        Dim userid As String = Request.QueryString("u")
        Dim role As String = Request.QueryString("r")
        Dim ulist As String = Request.QueryString("lst")
        Dim plateno As String = Request.QueryString("p")
        Dim rtax As String = Request.QueryString("rtax")
        Dim ptest As String = Request.QueryString("pt")
        Dim ins As String = Request.QueryString("Insu")
        Dim e1 As String = Request.QueryString("e1")
        Dim oe1 As String = Request.QueryString("oe1")
        Dim emailid As String = Request.QueryString("em1")
        Dim emailid2 As String = Request.QueryString("em2")
        Dim reamrks As String = Request.QueryString("rem")
        Dim ugData As String = Request.QueryString("ugData")
        
        ' Validate operation parameter
        Dim oprInt As Integer
        If Not Integer.TryParse(opr, oprInt) OrElse oprInt < 1 OrElse oprInt > 3 Then
            Response.Write("Invalid operation")
            Return
        End If
        
        If oprInt = 1 Then
            FillVehiclesGrid(userid, role, ulist)
        ElseIf oprInt = 2 Then
            DeleteGroup(ugData)
        ElseIf oprInt = 3 Then
            UpdateData(plateno, userid, rtax, ptest, ins, e1, oe1, emailid, emailid2, reamrks)
        End If
    End Sub

    Public Sub FillVehiclesGrid(ByVal ugData As String, ByVal role As String, ByVal userslist As String)
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Try
            ' Validate userid
            Dim userIdInt As Integer
            If Not Integer.TryParse(ugData, userIdInt) Then
                Response.Write("[]")
                Return
            End If

            Dim documentsdatetable As New DataTable
            documentsdatetable.Rows.Clear()
            documentsdatetable.Columns.Add(New DataColumn("chk"))
            documentsdatetable.Columns.Add(New DataColumn("sno"))
            documentsdatetable.Columns.Add(New DataColumn("plateno"))
            documentsdatetable.Columns.Add(New DataColumn("roadtax"))
            documentsdatetable.Columns.Add(New DataColumn("puspakomtest"))
            documentsdatetable.Columns.Add(New DataColumn("insurance"))
            documentsdatetable.Columns.Add(New DataColumn("permitexpiry"))
            documentsdatetable.Columns.Add(New DataColumn("otherdoc"))
            documentsdatetable.Columns.Add(New DataColumn("email1"))
            documentsdatetable.Columns.Add(New DataColumn("email2"))
            documentsdatetable.Columns.Add(New DataColumn("remarks"))
            documentsdatetable.Columns.Add(New DataColumn("mob1"))
            documentsdatetable.Columns.Add(New DataColumn("mob2"))
            documentsdatetable.Columns.Add(New DataColumn("plateid"))
            documentsdatetable.Columns.Add(New DataColumn("pmid"))
            Dim r As DataRow

            If ugData <> "--Select User Name--" Then
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                
                ' FIXED: Use parameterized query
                Dim cmd As SqlCommand = New SqlCommand("SELECT vt.plateno, vt.pmid, vt.userid, dt.roadtax, puspakomtest, insurance, permitexpiry, otherdocexpiry, pma, emailid, simno, btm, bdm, emailid2, simno, simno1, remarks FROM (SELECT * FROM vehicleTBL WHERE userid = @userid) vt LEFT OUTER JOIN documents_date dt ON dt.plateno = vt.plateno ORDER BY dt.plateno", conn)
                cmd.Parameters.AddWithValue("@userid", userIdInt)
                
                Dim dr As SqlDataReader

                If ugData = "--AllUsers--" Then
                    If role = "User" Then
                        cmd = New SqlCommand("SELECT vt.plateno, vt.pmid, vt.userid, dt.roadtax, puspakomtest, insurance, permitexpiry, otherdocexpiry, pma, emailid, simno, btm, bdm, emailid2, simno, simno1, remarks FROM (SELECT * FROM vehicleTBL WHERE userid = @userid) vt LEFT OUTER JOIN documents_date dt ON dt.plateno = vt.plateno ORDER BY dt.plateno", conn)
                        cmd.Parameters.AddWithValue("@userid", userIdInt)
                    ElseIf role = "SuperUser" Then
                        ' Validate userslist
                        If Not IsValidUsersList(userslist) Then
                            Response.Write("[]")
                            Return
                        End If
                        cmd = New SqlCommand("SELECT vt.plateno, vt.pmid, vt.userid, dt.roadtax, puspakomtest, insurance, permitexpiry, otherdocexpiry, pma, emailid, simno, btm, bdm, emailid2, simno, simno1, remarks FROM (SELECT * FROM vehicleTBL WHERE userid IN (" & userslist & ")) vt LEFT OUTER JOIN documents_date dt ON dt.plateno = vt.plateno ORDER BY dt.plateno", conn)
                    Else
                        cmd = New SqlCommand("SELECT vt.plateno, vt.pmid, vt.userid, dt.roadtax, puspakomtest, insurance, permitexpiry, otherdocexpiry, pma, emailid, simno, btm, bdm, emailid2, simno, simno1, remarks FROM vehicleTBL vt LEFT OUTER JOIN documents_date dt ON dt.plateno = vt.plateno ORDER BY dt.plateno", conn)
                    End If
                End If
                
                conn.Open()
                Dim i As Int32 = 1
                Dim tempdatetime As String = ""
                Dim tempdatetime1 As String = ""
                Dim tempdatetime2 As String = ""
                Dim tempdatetime3 As String = ""
                Dim tempdatetime4 As String = ""
                Dim tempdatetime5 As String = ""
                Dim tval As String = ""
                Dim tval1 As String = ""
                Dim tval2 As String = ""
                Dim tval3 As String = ""
                Dim tval4 As String = ""
                Dim tval5 As String = ""
                Dim emailid As String = ""
                Dim emailid2 As String = ""
                Dim remarks As String = ""
                Dim mobile1 As String = ""
                Dim mobile2 As String = ""
                dr = cmd.ExecuteReader()
                
                While dr.Read()
                    r = documentsdatetable.NewRow
                    
                    ' FIXED: HTML encode output to prevent XSS
                    r(0) = "<input type=""checkbox"" name=""chk"" class=""group1"" value=""" & HttpUtility.HtmlEncode(dr("plateno")) & """/>"
                    r(1) = i.ToString()

                    If Not IsDBNull(dr("emailid")) Then
                        emailid = HttpUtility.HtmlEncode(dr("emailid").ToString())
                    Else
                        emailid = ""
                    End If
                    If Not IsDBNull(dr("emailid2")) Then
                        emailid2 = HttpUtility.HtmlEncode(dr("emailid2").ToString())
                    Else
                        emailid2 = ""
                    End If
                    If Not IsDBNull(dr("simno")) Then
                        mobile1 = HttpUtility.HtmlEncode(dr("simno"))
                    Else
                        mobile1 = ""
                    End If
                    If Not IsDBNull(dr("simno1")) Then
                        mobile2 = HttpUtility.HtmlEncode(dr("simno1"))
                    Else
                        mobile2 = ""
                    End If

                    If Not IsDBNull(dr("remarks")) Then
                        remarks = HttpUtility.HtmlEncode(dr("remarks").ToString())
                    Else
                        remarks = ""
                    End If
                    
                    ' Process dates with proper validation
                    If Not IsDBNull(dr("roadtax")) Then
                        Dim roadTaxDate As DateTime
                        If DateTime.TryParse(dr("roadtax").ToString(), roadTaxDate) Then
                            tval = roadTaxDate.ToString("yyyy/MM/dd")
                            Dim diffdays As Int64 = (roadTaxDate.Date - DateTime.Now.Date).Days
                            If diffdays > 0 Then
                                If diffdays > 30 Then
                                    tempdatetime = "<span style='color :green;'>" & HttpUtility.HtmlEncode(tval) & "</span>"
                                ElseIf diffdays > 7 And diffdays <= 30 Then
                                    tempdatetime = "<span style='color :blue;'>" & HttpUtility.HtmlEncode(tval) & "</span>"
                                ElseIf diffdays <= 7 Then
                                    tempdatetime = "<span style='color :Red;font-weight:bold;'>" & HttpUtility.HtmlEncode(tval) & "</span>"
                                End If
                            Else
                                tempdatetime = "<span style='color :Red;text-decoration:line-through;'>" & HttpUtility.HtmlEncode(tval) & "</span>"
                            End If
                        End If
                    Else
                        tval = ""
                        tempdatetime = ""
                    End If

                    ' Similar processing for other dates...
                    If Not IsDBNull(dr("puspakomtest")) Then
                        Dim puspakomDate As DateTime
                        If DateTime.TryParse(dr("puspakomtest").ToString(), puspakomDate) Then
                            tval1 = puspakomDate.ToString("yyyy/MM/dd")
                            Dim diffdays As Int64 = (puspakomDate.Date - DateTime.Now.Date).Days
                            If diffdays > 0 Then
                                If diffdays > 30 Then
                                    tempdatetime1 = "<span style='color :green;'>" & HttpUtility.HtmlEncode(tval1) & "</span>"
                                ElseIf diffdays > 7 And diffdays <= 30 Then
                                    tempdatetime1 = "<span style='color :blue;'>" & HttpUtility.HtmlEncode(tval1) & "</span>"
                                ElseIf diffdays <= 7 Then
                                    tempdatetime1 = "<span style='color :Red;font-weight:bold;'>" & HttpUtility.HtmlEncode(tval1) & "</span>"
                                End If
                            Else
                                tempdatetime1 = "<span style='color :Red;text-decoration:line-through;'>" & HttpUtility.HtmlEncode(tval1) & "</span>"
                            End If
                        End If
                    Else
                        tval1 = ""
                        tempdatetime1 = ""
                    End If

                    ' Continue with other date fields...
                    If Not IsDBNull(dr("insurance")) Then
                        Dim insuranceDate As DateTime
                        If DateTime.TryParse(dr("insurance").ToString(), insuranceDate) Then
                            tval2 = insuranceDate.ToString("yyyy/MM/dd")
                            Dim diffdays As Int64 = (insuranceDate.Date - DateTime.Now.Date).Days
                            If diffdays > 0 Then
                                If diffdays > 30 Then
                                    tempdatetime2 = "<span style='color :green;'>" & HttpUtility.HtmlEncode(tval2) & "</span>"
                                ElseIf diffdays > 7 And diffdays <= 30 Then
                                    tempdatetime2 = "<span style='color :blue;'>" & HttpUtility.HtmlEncode(tval2) & "</span>"
                                ElseIf diffdays <= 7 Then
                                    tempdatetime2 = "<span style='color :Red;font-weight:bold;'>" & HttpUtility.HtmlEncode(tval2) & "</span>"
                                End If
                            Else
                                tempdatetime2 = "<span style='color :Red;text-decoration:line-through;'>" & HttpUtility.HtmlEncode(tval2) & "</span>"
                            End If
                        End If
                    Else
                        tval2 = ""
                        tempdatetime2 = ""
                    End If

                    If Not IsDBNull(dr("permitexpiry")) Then
                        Dim permitDate As DateTime
                        If DateTime.TryParse(dr("permitexpiry").ToString(), permitDate) Then
                            tval3 = permitDate.ToString("yyyy/MM/dd")
                            Dim diffdays As Int64 = (permitDate.Date - DateTime.Now.Date).Days
                            If diffdays > 0 Then
                                If diffdays > 30 Then
                                    tempdatetime3 = "<span style='color :green;'>" & HttpUtility.HtmlEncode(tval3) & "</span>"
                                ElseIf diffdays > 7 And diffdays <= 30 Then
                                    tempdatetime3 = "<span style='color :blue;'>" & HttpUtility.HtmlEncode(tval3) & "</span>"
                                ElseIf diffdays <= 7 Then
                                    tempdatetime3 = "<span style='color :Red;font-weight:bold;'>" & HttpUtility.HtmlEncode(tval3) & "</span>"
                                End If
                            Else
                                tempdatetime3 = "<span style='color :Red;text-decoration:line-through;'>" & HttpUtility.HtmlEncode(tval3) & "</span>"
                            End If
                        End If
                    Else
                        tval3 = ""
                        tempdatetime3 = ""
                    End If

                    If Not IsDBNull(dr("otherdocexpiry")) Then
                        Dim otherDocDate As DateTime
                        If DateTime.TryParse(dr("otherdocexpiry").ToString(), otherDocDate) Then
                            tval5 = otherDocDate.ToString("yyyy/MM/dd")
                            If tval5 <> "1900/01/01" Then
                                Dim diffdays As Int64 = (otherDocDate.Date - DateTime.Now.Date).Days
                                If diffdays > 0 Then
                                    If diffdays > 30 Then
                                        tempdatetime5 = "<span style='color :green;'>" & HttpUtility.HtmlEncode(tval5) & "</span>"
                                    ElseIf diffdays > 7 And diffdays <= 30 Then
                                        tempdatetime5 = "<span style='color :blue;'>" & HttpUtility.HtmlEncode(tval5) & "</span>"
                                    ElseIf diffdays <= 7 Then
                                        tempdatetime5 = "<span style='color :Red;font-weight:bold;'>" & HttpUtility.HtmlEncode(tval5) & "</span>"
                                    End If
                                Else
                                    tempdatetime5 = "<span style='color :Red;text-decoration:line-through;'>" & HttpUtility.HtmlEncode(tval5) & "</span>"
                                End If
                            Else
                                tval5 = ""
                                tempdatetime5 = ""
                            End If
                        End If
                    Else
                        tval5 = ""
                        tempdatetime5 = ""
                    End If

                    If Not IsDBNull(dr("pma")) Then
                        Dim pmaDate As DateTime
                        If DateTime.TryParse(dr("pma").ToString(), pmaDate) Then
                            tval4 = pmaDate.ToString("yyyy/MM/dd")
                            Dim diffdays As Int64 = (pmaDate.Date - DateTime.Now.Date).Days
                            If diffdays > 0 Then
                                If diffdays > 30 Then
                                    tempdatetime4 = "<span style='color :green;'>" & HttpUtility.HtmlEncode(tval4) & "</span>"
                                ElseIf diffdays > 7 And diffdays <= 30 Then
                                    tempdatetime4 = "<span style='color :blue;'>" & HttpUtility.HtmlEncode(tval4) & "</span>"
                                ElseIf diffdays <= 7 Then
                                    tempdatetime4 = "<span style='color :Red;font-weight:bold;'>" & HttpUtility.HtmlEncode(tval4) & "</span>"
                                End If
                            Else
                                tempdatetime4 = "<span style='color :Red;text-decoration:line-through;'>" & HttpUtility.HtmlEncode(tval4) & "</span>"
                            End If
                        End If
                    Else
                        tval4 = ""
                        tempdatetime4 = ""
                    End If

                    ' Get username with parameterized query
                    Dim uname As String = ""
                    Dim cmd1 As SqlCommand = New SqlCommand("SELECT username FROM userTBL WHERE userid = @userid", conn)
                    cmd1.Parameters.AddWithValue("@userid", dr("userid"))
                    Try
                        uname = HttpUtility.HtmlEncode(cmd1.ExecuteScalar()?.ToString())
                    Catch ex As Exception
                        uname = "Unknown"
                    End Try
                    
                    ' FIXED: Properly encode all JavaScript parameters to prevent XSS
                    Dim plateNo As String = HttpUtility.HtmlEncode(dr("plateno").ToString.ToUpper())
                    Dim userIdParam As String = HttpUtility.HtmlEncode(dr("userid").ToString())
                    Dim unameParam As String = HttpUtility.JavaScriptStringEncode(uname)
                    Dim plateParam As String = HttpUtility.JavaScriptStringEncode(plateNo)
                    Dim tvalParam As String = HttpUtility.JavaScriptStringEncode(tval)
                    Dim tval1Param As String = HttpUtility.JavaScriptStringEncode(tval1)
                    Dim tval2Param As String = HttpUtility.JavaScriptStringEncode(tval2)
                    Dim tval3Param As String = HttpUtility.JavaScriptStringEncode(tval3)
                    Dim tval5Param As String = HttpUtility.JavaScriptStringEncode(tval5)
                    Dim tval4Param As String = HttpUtility.JavaScriptStringEncode(tval4)
                    Dim btmParam As String = HttpUtility.JavaScriptStringEncode(dr("btm")?.ToString())
                    Dim bdmParam As String = HttpUtility.JavaScriptStringEncode(dr("bdm")?.ToString())
                    Dim emailidParam As String = HttpUtility.JavaScriptStringEncode(emailid)
                    Dim emailid2Param As String = HttpUtility.JavaScriptStringEncode(emailid2)
                    Dim mobile1Param As String = HttpUtility.JavaScriptStringEncode(mobile1)
                    Dim mobile2Param As String = HttpUtility.JavaScriptStringEncode(mobile2)
                    Dim remarksParam As String = HttpUtility.JavaScriptStringEncode(remarks)
                    
                    r(2) = "<span style='cursor:pointer;text-decoration:underline;' onclick=""javascript :openPopup('" & userIdParam & "','" & unameParam & "','" & plateParam & "','" & tvalParam & "','" & tval1Param & "','" & tval2Param & "','" & tval3Param & "','" & tval5Param & "','" & tval4Param & "','" & btmParam & "','" & bdmParam & "','" & emailidParam & "','" & emailid2Param & "','" & mobile1Param & "','" & mobile2Param & "','" & remarksParam & "')"">" & plateNo & "</span>"

                    r(3) = tempdatetime
                    r(4) = tempdatetime1
                    r(5) = tempdatetime2
                    r(6) = tempdatetime3
                    r(7) = tempdatetime5
                    r(8) = emailid
                    r(9) = emailid2
                    r(10) = remarks
                    r(11) = mobile1
                    r(12) = mobile2
                    r(13) = HttpUtility.HtmlEncode(dr("plateno"))
                    r(14) = HttpUtility.HtmlEncode(dr("pmid"))
                    documentsdatetable.Rows.Add(r)
                    i = i + 1
                End While
            End If

            If documentsdatetable.Rows.Count = 0 Then
                r = documentsdatetable.NewRow
                r(0) = "--"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                r(5) = "--"
                r(6) = "--"
                r(7) = "--"
                r(8) = "--"
                r(9) = "--"
                r(10) = "--"
                r(11) = "--"
                r(12) = "--"
                r(13) = "--"
                r(14) = "--"
                documentsdatetable.Rows.Add(r)
            End If
            
            Session("exceltable") = documentsdatetable
            For j As Integer = 0 To documentsdatetable.Rows.Count - 1
                Try
                    a = New ArrayList
                    a.Add(documentsdatetable.DefaultView.Item(j)(0))
                    a.Add(documentsdatetable.DefaultView.Item(j)(1))
                    a.Add(documentsdatetable.DefaultView.Item(j)(2))
                    a.Add(documentsdatetable.DefaultView.Item(j)(14))
                    a.Add(documentsdatetable.DefaultView.Item(j)(3))
                    a.Add(documentsdatetable.DefaultView.Item(j)(4))
                    a.Add(documentsdatetable.DefaultView.Item(j)(5))
                    a.Add(documentsdatetable.DefaultView.Item(j)(6))
                    a.Add(documentsdatetable.DefaultView.Item(j)(7))
                    a.Add(documentsdatetable.DefaultView.Item(j)(8))
                    a.Add(documentsdatetable.DefaultView.Item(j)(9))
                    a.Add(documentsdatetable.DefaultView.Item(j)(10))
                    aa.Add(a)
                Catch ex As Exception
                    System.Diagnostics.Debug.WriteLine("Error processing row: " & ex.Message)
                End Try
            Next
            
            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.Write(json)
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in FillVehiclesGrid: " & ex.Message)
            Response.Write("[]")
        End Try
    End Sub

    Public Sub DeleteGroup(ByVal ugData As String)
        Dim str As Integer = 0
        Try
            ' Validate and sanitize input
            If String.IsNullOrEmpty(ugData) Then
                Response.Write("N")
                Return
            End If

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand

            Dim groupides() As String = ugData.Split(",")

            For i As Int32 = 0 To groupides.Length - 1
                If groupides(i) = "on" Then
                    Continue For
                End If
                
                ' Validate plate number format (alphanumeric only)
                If Not System.Text.RegularExpressions.Regex.IsMatch(groupides(i), "^[a-zA-Z0-9]+$") Then
                    Continue For
                End If
                
                ' FIXED: Use parameterized query
                cmd = New SqlCommand("DELETE FROM documents_date WHERE plateno = @plateno", conn)
                cmd.Parameters.AddWithValue("@plateno", groupides(i))
                
                conn.Open()
                str = cmd.ExecuteNonQuery()
                conn.Close()
                
                If str > 0 Then
                    Response.Write("Yes")
                Else
                    Response.Write("N")
                End If
            Next
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in DeleteGroup: " & ex.Message)
            Response.Write("N")
        End Try
    End Sub
    
    Public Sub UpdateData(ByVal plateno As String, ByVal userid As String, ByVal rtax As String, ByVal ptest As String, ByVal ins As String, ByVal e1 As String, ByVal oe1 As String, ByVal emailid As String, ByVal emailid2 As String, ByVal Remarks As String)
        Dim s As String = ""
        Try
            ' Validate inputs
            If String.IsNullOrEmpty(plateno) OrElse String.IsNullOrEmpty(userid) Then
                Response.Write("N")
                Return
            End If
            
            ' Validate userid is numeric
            Dim userIdInt As Integer
            If Not Integer.TryParse(userid, userIdInt) Then
                Response.Write("N")
                Return
            End If
            
            ' Validate plate number format
            If Not System.Text.RegularExpressions.Regex.IsMatch(plateno, "^[a-zA-Z0-9]+$") Then
                Response.Write("N")
                Return
            End If
            
            ' Validate email format if provided
            If Not String.IsNullOrEmpty(emailid) AndAlso Not IsValidEmail(emailid) Then
                Response.Write("N")
                Return
            End If
            
            If Not String.IsNullOrEmpty(emailid2) AndAlso Not IsValidEmail(emailid2) Then
                Response.Write("N")
                Return
            End If
            
            ' Validate and parse dates
            Dim roadTaxDate As DateTime?
            Dim puspakomDate As DateTime?
            Dim insuranceDate As DateTime?
            Dim permitDate As DateTime?
            Dim otherDocDate As DateTime?
            
            If Not String.IsNullOrEmpty(rtax) AndAlso Not DateTime.TryParse(rtax, roadTaxDate) Then
                Response.Write("N")
                Return
            End If
            
            If Not String.IsNullOrEmpty(ptest) AndAlso Not DateTime.TryParse(ptest, puspakomDate) Then
                Response.Write("N")
                Return
            End If
            
            If Not String.IsNullOrEmpty(ins) AndAlso Not DateTime.TryParse(ins, insuranceDate) Then
                Response.Write("N")
                Return
            End If
            
            If Not String.IsNullOrEmpty(e1) AndAlso Not DateTime.TryParse(e1, permitDate) Then
                Response.Write("N")
                Return
            End If
            
            If Not String.IsNullOrEmpty(oe1) AndAlso Not DateTime.TryParse(oe1, otherDocDate) Then
                Response.Write("N")
                Return
            End If

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' FIXED: Use parameterized query for update
            Dim updateCmd As New SqlCommand("UPDATE documents_date SET pma = @pma, btm = '0', bdm = '0', emailid = @emailid, emailid2 = @emailid2, updateddatetime = @updatetime, remarks = @remarks", conn)
            
            ' Add conditional date updates
            If roadTaxDate.HasValue Then
                updateCmd.CommandText += ", roadtax = @roadtax"
                updateCmd.Parameters.AddWithValue("@roadtax", roadTaxDate.Value)
            End If
            
            If puspakomDate.HasValue Then
                updateCmd.CommandText += ", puspakomtest = @puspakomtest"
                updateCmd.Parameters.AddWithValue("@puspakomtest", puspakomDate.Value)
            End If
            
            If insuranceDate.HasValue Then
                updateCmd.CommandText += ", insurance = @insurance"
                updateCmd.Parameters.AddWithValue("@insurance", insuranceDate.Value)
            End If
            
            If permitDate.HasValue Then
                updateCmd.CommandText += ", permitexpiry = @permitexpiry"
                updateCmd.Parameters.AddWithValue("@permitexpiry", permitDate.Value)
            End If
            
            If otherDocDate.HasValue Then
                updateCmd.CommandText += ", otherdocexpiry = @otherdocexpiry"
                updateCmd.Parameters.AddWithValue("@otherdocexpiry", otherDocDate.Value)
            End If
            
            updateCmd.CommandText += " WHERE plateno = @plateno"
            
            updateCmd.Parameters.AddWithValue("@pma", DateTime.Now)
            updateCmd.Parameters.AddWithValue("@emailid", emailid)
            updateCmd.Parameters.AddWithValue("@emailid2", emailid2)
            updateCmd.Parameters.AddWithValue("@updatetime", DateTime.Now)
            updateCmd.Parameters.AddWithValue("@remarks", Remarks)
            updateCmd.Parameters.AddWithValue("@plateno", plateno)

            Try
                conn.Open()
                s = updateCmd.ExecuteNonQuery().ToString()
                
                If s = "0" Then
                    ' Insert new record if update affected 0 rows
                    Dim insertCmd As New SqlCommand("INSERT INTO documents_date (plateno, userid, pma, btm, bdm, emailid, updateddatetime, emailid2, remarks", conn)
                    Dim valuesClause As String = "VALUES (@plateno, @userid, @pma, '0', '0', @emailid, @updatetime, @emailid2, @remarks"
                    
                    If roadTaxDate.HasValue Then
                        insertCmd.CommandText += ", roadtax"
                        valuesClause += ", @roadtax"
                        insertCmd.Parameters.AddWithValue("@roadtax", roadTaxDate.Value)
                    End If
                    
                    If puspakomDate.HasValue Then
                        insertCmd.CommandText += ", puspakomtest"
                        valuesClause += ", @puspakomtest"
                        insertCmd.Parameters.AddWithValue("@puspakomtest", puspakomDate.Value)
                    End If
                    
                    If insuranceDate.HasValue Then
                        insertCmd.CommandText += ", insurance"
                        valuesClause += ", @insurance"
                        insertCmd.Parameters.AddWithValue("@insurance", insuranceDate.Value)
                    End If
                    
                    If permitDate.HasValue Then
                        insertCmd.CommandText += ", permitexpiry"
                        valuesClause += ", @permitexpiry"
                        insertCmd.Parameters.AddWithValue("@permitexpiry", permitDate.Value)
                    End If
                    
                    If otherDocDate.HasValue Then
                        insertCmd.CommandText += ", otherdocexpiry"
                        valuesClause += ", @otherdocexpiry"
                        insertCmd.Parameters.AddWithValue("@otherdocexpiry", otherDocDate.Value)
                    End If
                    
                    insertCmd.CommandText += ") " & valuesClause & ")"
                    
                    insertCmd.Parameters.AddWithValue("@plateno", plateno)
                    insertCmd.Parameters.AddWithValue("@userid", userIdInt)
                    insertCmd.Parameters.AddWithValue("@pma", DateTime.Now)
                    insertCmd.Parameters.AddWithValue("@emailid", emailid)
                    insertCmd.Parameters.AddWithValue("@updatetime", DateTime.Now)
                    insertCmd.Parameters.AddWithValue("@emailid2", emailid2)
                    insertCmd.Parameters.AddWithValue("@remarks", Remarks)
                    
                    s = insertCmd.ExecuteNonQuery().ToString()
                End If
                
                If Integer.Parse(s) > 0 Then
                    Response.Write("Yes")
                Else
                    Response.Write("N")
                End If
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine("Error in UpdateData: " & ex.Message)
                Response.Write("N")
            Finally
                conn.Close()
            End Try
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in UpdateData: " & ex.Message)
            Response.Write("N")
        End Try
    End Sub

    Public Sub AddGroup(ByVal plateno As String, ByVal userid As String, ByVal rtax As String, ByVal ptest As String, ByVal ins As String, ByVal e1 As String, ByVal pma As String)
        Try
            ' Validate inputs
            If String.IsNullOrEmpty(plateno) OrElse String.IsNullOrEmpty(userid) Then
                Response.Write("N")
                Return
            End If
            
            Dim userIdInt As Integer
            If Not Integer.TryParse(userid, userIdInt) Then
                Response.Write("N")
                Return
            End If
            
            ' Validate dates
            Dim roadTaxDate, puspakomDate, insuranceDate, permitDate, pmaDate As DateTime
            If Not String.IsNullOrEmpty(rtax) AndAlso Not DateTime.TryParse(rtax, roadTaxDate) Then
                Response.Write("N")
                Return
            End If
            
            If Not String.IsNullOrEmpty(ptest) AndAlso Not DateTime.TryParse(ptest, puspakomDate) Then
                Response.Write("N")
                Return
            End If
            
            If Not String.IsNullOrEmpty(ins) AndAlso Not DateTime.TryParse(ins, insuranceDate) Then
                Response.Write("N")
                Return
            End If
            
            If Not String.IsNullOrEmpty(e1) AndAlso Not DateTime.TryParse(e1, permitDate) Then
                Response.Write("N")
                Return
            End If
            
            If Not String.IsNullOrEmpty(pma) AndAlso Not DateTime.TryParse(pma, pmaDate) Then
                Response.Write("N")
                Return
            End If

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            conn.Open()

            ' FIXED: Use parameterized query
            Dim cmd As SqlCommand = New SqlCommand("INSERT INTO documents_date(plateno, userid, roadtax, puspakomtest, insurance, permitexpiry, pma) VALUES (@plateno, @userid, @roadtax, @puspakomtest, @insurance, @permitexpiry, @pma)", conn)
            cmd.Parameters.AddWithValue("@plateno", plateno)
            cmd.Parameters.AddWithValue("@userid", userIdInt)
            cmd.Parameters.AddWithValue("@roadtax", If(String.IsNullOrEmpty(rtax), DBNull.Value, roadTaxDate))
            cmd.Parameters.AddWithValue("@puspakomtest", If(String.IsNullOrEmpty(ptest), DBNull.Value, puspakomDate))
            cmd.Parameters.AddWithValue("@insurance", If(String.IsNullOrEmpty(ins), DBNull.Value, insuranceDate))
            cmd.Parameters.AddWithValue("@permitexpiry", If(String.IsNullOrEmpty(e1), DBNull.Value, permitDate))
            cmd.Parameters.AddWithValue("@pma", If(String.IsNullOrEmpty(pma), DBNull.Value, pmaDate))
            
            Dim result As Int16 = cmd.ExecuteNonQuery()
            conn.Close()
            
            If result > 0 Then
                Response.Write("Yes")
            Else
                Response.Write("N")
            End If

        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error in AddGroup: " & ex.Message)
            Response.Write("N")
        End Try
    End Sub
    
    ' Helper function to validate email format
    Private Function IsValidEmail(email As String) As Boolean
        Try
            Dim addr As New System.Net.Mail.MailAddress(email)
            Return addr.Address = email
        Catch
            Return False
        End Try
    End Function
    
    ' Helper function to validate users list
    Private Function IsValidUsersList(usersList As String) As Boolean
        If String.IsNullOrEmpty(usersList) Then
            Return False
        End If
        
        ' Check if usersList contains only numbers and commas
        Return System.Text.RegularExpressions.Regex.IsMatch(usersList, "^[0-9,]+$")
    End Function
End Class