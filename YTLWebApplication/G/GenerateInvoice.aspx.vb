Imports System.Data.SqlClient
Imports ExcelLibrary
Imports ExcelLibrary.SpreadSheet

Partial Class GenerateInvoice
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Dim sql As String

            Dim wb As New Workbook()

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))


            sql = "select userid,username,companyname from userTBL where role=@role order by username"

            Dim cmd As New SqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@role", "User")
            Dim dr, tdr As SqlDataReader

            Dim userid As String
            Dim username As String


            Dim sheetrowcounter As Integer
            Dim tankcount As Byte

            Dim totalvehicles As Integer
            Dim total0tankvehicles As Integer
            Dim total1tankvehicles As Integer
            Dim total2tankvehicles As Integer
            Dim totalptovehicles As Integer
            Dim countExisting As Integer
            Dim countNew As Integer
            Dim countUnbill As Integer

            Try
                conn.Open()
                dr = cmd.ExecuteReader()

                While dr.Read()
                    Try

                        userid = dr("userid").ToString()
                        username = dr("username").ToString().ToUpper()

                        sheetrowcounter = 0

                        total0tankvehicles = 0
                        total1tankvehicles = 0
                        total2tankvehicles = 0
                        totalptovehicles = 0
                        countExisting = 0
                        countNew = 0
                        countUnbill = 0


                        Dim sheet As New ExcelLibrary.SpreadSheet.Worksheet(username.Replace("/", " "))

                        sheet.Cells(sheetrowcounter, 0) = New Cell("Company Name: " & dr("companyname").ToString().ToUpper() & "                                                                                                                          ")
                        sheetrowcounter += 1
                        sheet.Cells(sheetrowcounter, 0) = New Cell("For the Month of " & checkMonth(DateTime.Now.AddMonths(1).Month).ToString() & " " & DateTime.Now.Year & "                                                                                                                ")
                        sheetrowcounter += 1
                        sheet.Cells(sheetrowcounter, 0) = New Cell("Total Vehicles:")
                        sheetrowcounter += 1
                        sheet.Cells(sheetrowcounter, 0) = New Cell("Total 1 Tank :")
                        sheetrowcounter += 1
                        sheet.Cells(sheetrowcounter, 0) = New Cell("Total PTO:")
                        sheetrowcounter += 1
                        sheet.Cells(sheetrowcounter, 0) = New Cell("Total 2 Tank :")
                        sheetrowcounter += 1
                        sheet.Cells(sheetrowcounter, 0) = New Cell("Total Without Tank :")
                        sheetrowcounter += 1
                        sheet.Cells(sheetrowcounter, 0) = New Cell("Total Existing Vehicle :")
                        sheetrowcounter += 1
                        sheet.Cells(sheetrowcounter, 0) = New Cell("Total New Installation :")
                        sheetrowcounter += 1
                        sheet.Cells(sheetrowcounter, 0) = New Cell("Total Unbill Vehicle :")
                        sheetrowcounter += 1
                        sheetrowcounter += 1
                        sheetrowcounter += 1

                        Dim connection As New Redirect(userid)
                        Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))

                        cmd = New SqlCommand("select * from vehicleTBL where userid=@userid order by installationdate desc", conn2)
                        cmd.Parameters.AddWithValue("@userid", userid)
                        conn2.Open()
                        tdr = cmd.ExecuteReader()


                        sheet.Cells(sheetrowcounter, 0) = New Cell("No")
                        sheet.Cells.ColumnWidth(0) = 3000
                        sheet.Cells(sheetrowcounter, 1) = New Cell("Username")
                        sheet.Cells.ColumnWidth(1) = 6000
                        sheet.Cells(sheetrowcounter, 2) = New Cell("Group Name")
                        sheet.Cells.ColumnWidth(2) = 6000
                        sheet.Cells(sheetrowcounter, 3) = New Cell("Plate No")
                        sheet.Cells.ColumnWidth(3) = 5000
                        sheet.Cells(sheetrowcounter, 4) = New Cell("Date Installed")
                        sheet.Cells.ColumnWidth(4) = 3000
                        sheet.Cells(sheetrowcounter, 5) = New Cell("No of Tank")
                        sheet.Cells.ColumnWidth(5) = 3000
                        sheet.Cells(sheetrowcounter, 6) = New Cell("PTO")
                        sheet.Cells.ColumnWidth(6) = 3000
                        sheet.Cells(sheetrowcounter, 7) = New Cell("Billing Type")
                        sheet.Cells.ColumnWidth(6) = 3000
                        sheetrowcounter += 1

                        Dim rowcount As Integer = 1
                        While tdr.Read()
                            Try

                                sheet.Cells(sheetrowcounter, 0) = New Cell(rowcount.ToString())
                                sheet.Cells(sheetrowcounter, 1) = New Cell(username)
                                sheet.Cells(sheetrowcounter, 2) = New Cell(tdr("groupname").ToString().ToUpper())
                                sheet.Cells(sheetrowcounter, 3) = New Cell(tdr("plateno").ToString())

                                tankcount = 0

                                If (tdr("tank1size").ToString() <> "") Then
                                    tankcount += 1
                                End If

                                If (tdr("tank2size").ToString() <> "") Then
                                    tankcount += 1
                                End If

                                Select Case tankcount
                                    Case 0
                                        total0tankvehicles += 1
                                    Case 1
                                        total1tankvehicles += 1
                                    Case 2
                                        total2tankvehicles += 1
                                End Select


                                sheet.Cells(sheetrowcounter, 5) = New Cell(tankcount.ToString())

                                If (tdr("pto") = True) Then
                                    sheet.Cells(sheetrowcounter, 6) = New Cell("YES")
                                    totalptovehicles += 1
                                Else
                                    sheet.Cells(sheetrowcounter, 6) = New Cell("NO")
                                End If

                                Dim billingType As String
                                If IsDBNull(tdr("installationdate")) Then
                                    sheet.Cells(sheetrowcounter, 4) = New Cell("--")
                                Else

                                    If tdr("installationdate").ToString() <> "" And DateTime.Parse(tdr("installationdate")).ToString("yyyy-MM-dd") <> "1900-01-01" Then
                                        sheet.Cells(sheetrowcounter, 4) = New Cell(DateTime.Parse(tdr("installationdate")).ToString("yyyy-MM-dd"))
                                    Else
                                        sheet.Cells(sheetrowcounter, 4) = New Cell("--")
                                    End If

                                    Dim installationdate As DateTime = Convert.ToDateTime(tdr("installationdate")).ToString("yyyy/MM/dd")
                                    If (installationdate < DateTime.Now.AddMonths(-1).ToString("1/MMM/yyyy")) Then
                                        billingType = "Existing"
                                        countExisting += 1
                                    ElseIf (installationdate.Month = DateTime.Now.AddMonths(-1).Month) Then
                                        billingType = "New"
                                        countNew += 1
                                    ElseIf (installationdate.Month = DateTime.Now.Month) Then
                                        billingType = "Unbill"
                                        countUnbill += 1
                                    End If

                                End If
                                sheet.Cells(sheetrowcounter, 7) = New Cell(billingType)

                                rowcount += 1
                                sheetrowcounter += 1

                            Catch ex As Exception
                                Response.Write("aa" & ex.Message)
                            End Try
                        End While

                        conn2.Close()

                        totalvehicles = rowcount - 1

                        sheet.Cells(2, 0) = New Cell("Total Vehicles: " & totalvehicles.ToString() & "                                                                                                           ")
                        sheet.Cells(3, 0) = New Cell("Total 1 Tank : " & total1tankvehicles.ToString() & "                                                                                          ")
                        sheet.Cells(4, 0) = New Cell("Total 2 Tank : " & total2tankvehicles.ToString() & "                                                                                          ")
                        sheet.Cells(5, 0) = New Cell("Total Without Tank : " & total0tankvehicles.ToString() & "                                                                                          ")
                        sheet.Cells(6, 0) = New Cell("Total PTO : " & totalptovehicles.ToString() & "                                                                                              ")
                        sheet.Cells(7, 0) = New Cell("Total Existing Vehicle : " & countExisting.ToString() & "")
                        sheet.Cells(8, 0) = New Cell("Total New Installation : " & countNew.ToString() & "")
                        sheet.Cells(9, 0) = New Cell("Total Unbill Vehicle : " & countUnbill.ToString() & "")

                        wb.Worksheets.Add(sheet)
                    Catch ex As Exception
                        Response.Write("bb" & ex.Message)
                    End Try
                End While


            Catch ex As Exception
                Response.Write("cc" & ex.Message)
            Finally
                conn.Close()
            End Try

            Response.Clear()
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("content-disposition", "attachment;filename=LafargeInvoice.xls")

            Dim m As System.IO.MemoryStream = New System.IO.MemoryStream()
            wb.SaveToStream(m)
            m.WriteTo(Response.OutputStream)

        Catch ex As Exception
            Response.Write("dd" & ex.Message)
        End Try
    End Sub
    Protected Function checkMonth(ByVal para As String) As String
        Select Case para
            Case 1
                Return "January"
            Case 2
                Return "February"
            Case 3
                Return "March"
            Case 4
                Return "April"
            Case 5
                Return "May"
            Case 6
                Return "June"
            Case 7
                Return "July"
            Case 8
                Return "August"
            Case 9
                Return "September"
            Case 10
                Return "October"
            Case 11
                Return "November"
            Case 12
                Return "December"
        End Select
    End Function
End Class
