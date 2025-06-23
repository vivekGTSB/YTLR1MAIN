Imports System.Data.SqlClient 
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class TrailerMgmtJson
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
                Return
            End If
            
            Dim opr As String = Request.QueryString("opr")
            
            ' SECURITY FIX: Validate operation parameter
            If Not SecurityHelper.ValidateInput(opr, "^[1-4]$") Then
                Response.Write("[]")
                Return
            End If
            
            Select Case opr
                Case "1"
                    Dim userid As String = Request.QueryString("u")
                    Dim role As String = Request.QueryString("r")
                    Dim ulist As String = Request.QueryString("lst")
                    
                    ' SECURITY FIX: Validate input parameters
                    If Not String.IsNullOrEmpty(userid) AndAlso Not SecurityHelper.ValidateInput(userid, "^[0-9]+$") Then
                        Response.Write("[]")
                        Return
                    End If
                    
                    FillVehiclesGrid(userid, role, ulist)
                    
                Case "2"
                    Dim uid As String = Request.QueryString("uid")
                    Dim trilerno As String = Request.QueryString("tname")
                    Dim inspection As String = Request.QueryString("insdatetime")
                    Dim rtax As String = Request.QueryString("rtax")
                    Dim ptest As String = Request.QueryString("pt")
                    Dim ins As String = Request.QueryString("Insu") 
                    Dim emailid As String = Request.QueryString("em1")
                    Dim emailid2 As String = Request.QueryString("emlcc")
                    
                    ' SECURITY FIX: Validate input parameters
                    If Not SecurityHelper.ValidateInput(uid, "^[0-9]+$") OrElse
                       Not SecurityHelper.ValidateInput(trilerno, "^[A-Za-z0-9\-_\s]{1,50}$") Then
                        Response.Write("No")
                        Return
                    End If
                    
                    AddData(uid, trilerno, inspection, emailid, emailid2, rtax, ptest, ins)
                    
                Case "3"
                    Dim id As String = Request.QueryString("id")
                    Dim inspection As String = Request.QueryString("insdatetime")
                    Dim trilerno As String = Request.QueryString("tname")
                    Dim uid As String = Request.QueryString("uid")
                    Dim emailid As String = Request.QueryString("em1")
                    Dim emailid2 As String = Request.QueryString("emlcc")
                    Dim rtax As String = Request.QueryString("rtax")
                    Dim ptest As String = Request.QueryString("pt")
                    Dim ins As String = Request.QueryString("Insu")
                    
                    ' SECURITY FIX: Validate input parameters
                    If Not SecurityHelper.ValidateInput(id, "^[0-9]+$") OrElse
                       Not SecurityHelper.ValidateInput(uid, "^[0-9]+$") OrElse
                       Not SecurityHelper.ValidateInput(trilerno, "^[A-Za-z0-9\-_\s]{1,50}$") Then
                        Response.Write("No")
                        Return
                    End If
                    
                    UpdateData(id, inspection, trilerno, uid, emailid, emailid2, rtax, ptest, ins)
                    
                Case "4"
                    Dim ugData As String = Request.QueryString("ugData")
                    
                    ' SECURITY FIX: Validate input parameters
                    If String.IsNullOrEmpty(ugData) Then
                        Response.Write("No")
                        Return
                    End If
                    
                    DeleteData(ugData)
            End Select
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("Page_Load error", ex, Server)
            Response.Write("[]")
        End Try
    End Sub

    Public Function FillVehiclesGrid(ByVal ugData As String, ByVal role As String, ByVal userslist As String) As String
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Dim conn As SqlConnection = Nothing

        Try
            Dim r As DataRow
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("chk"))
            t.Columns.Add(New DataColumn("S NO"))
            t.Columns.Add(New DataColumn("Name"))
            t.Columns.Add(New DataColumn("insdate"))
            t.Columns.Add(New DataColumn("taxdate"))
            t.Columns.Add(New DataColumn("pusdate"))
            t.Columns.Add(New DataColumn("insudate"))
            t.Columns.Add(New DataColumn("unm"))
            t.Columns.Add(New DataColumn("eml1"))
            t.Columns.Add(New DataColumn("eml2"))
            
            Dim userid As String = HttpContext.Current.Request.Cookies("userinfo")("userid")
            Dim rle As String = HttpContext.Current.Request.Cookies("userinfo")("role")
            Dim ulist As String = HttpContext.Current.Request.Cookies("userinfo")("userslist")

            Dim query As String = "SELECT emailid1, emailid2, id, trailerNo, inspectionDate, roadtax, puspakom, insurance, u.username, u.userid FROM trailer t LEFT OUTER JOIN userTBL u ON u.userid=t.userid"
            
            ' Build query based on user role
            If rle = "User" Then
                query = "SELECT emailid1, emailid2, id, trailerNo, inspectionDate, roadtax, puspakom, insurance, u.username, u.userid FROM (SELECT * FROM trailer WHERE userid=@userid) t LEFT OUTER JOIN userTBL u ON u.userid=t.userid"
            ElseIf rle = "SuperUser" Or rle = "Operator" Then
                query = "SELECT emailid1, emailid2, id, trailerNo, inspectionDate, roadtax, puspakom, insurance, u.username, u.userid FROM (SELECT * FROM trailer WHERE userid IN (" & ulist & ")) t LEFT OUTER JOIN userTBL u ON u.userid=t.userid"
            End If

            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand(query, conn)
            
            ' Add parameters if needed
            If rle = "User" Then
                cmd.Parameters.AddWithValue("@userid", userid)
            End If
            
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Dim i As Int32 = 1
            Dim insDateTime As String = ""
            
            While dr.Read()
                Try
                    insDateTime = Convert.ToDateTime(dr("inspectionDate")).ToString("yyyy/MM/dd")
                    r = t.NewRow
                    r(0) = "<input type=""checkbox"" name=""chk"" class=""group1"" value=""" & HttpUtility.HtmlEncode(dr("id").ToString()) & """/>"
                    r(1) = i.ToString()

                    r(8) = HttpUtility.HtmlEncode(dr("emailid1").ToString())
                    If Not IsDBNull(dr("emailid2")) Then
                        r(9) = HttpUtility.HtmlEncode(dr("emailid2").ToString())
                    Else
                        r(6) = ""
                    End If

                    r(3) = insDateTime
                    If Not IsDBNull(dr("roadtax")) Then
                        If dr("roadtax").ToString() = "1900-01-01 00:00:00.000" Then
                            r(4) = ""
                        Else
                            r(4) = Convert.ToDateTime(dr("roadtax")).ToString("yyyy/MM/dd")
                        End If
                    Else
                        r(4) = ""
                    End If
                    
                    If Not IsDBNull(dr("puspakom")) Then
                        If dr("puspakom").ToString() = "1900-01-01 00:00:00.000" Then
                            r(5) = ""
                        Else
                            r(5) = Convert.ToDateTime(dr("puspakom")).ToString("yyyy/MM/dd")
                        End If
                    Else
                        r(5) = ""
                    End If
                    
                    If Not IsDBNull(dr("insurance")) Then
                        If dr("insurance").ToString() = "1900-01-01 00:00:00.000" Then
                            r(6) = ""
                        Else
                            r(6) = Convert.ToDateTime(dr("insurance")).ToString("yyyy/MM/dd")
                        End If
                    Else
                        r(6) = ""
                    End If
                    
                    ' SECURITY FIX: HTML encode output
                    r(2) = "<span style='cursor:pointer;text-decoration:underline;' onclick=""javascript:openPopup('" & 
                           HttpUtility.HtmlEncode(dr("id").ToString()) & "','" & 
                           HttpUtility.HtmlEncode(dr("trailerNo").ToString()) & "','" & 
                           HttpUtility.HtmlEncode(insDateTime) & "','" & 
                           HttpUtility.HtmlEncode(r(4).ToString()) & "','" & 
                           HttpUtility.HtmlEncode(r(5).ToString()) & "','" & 
                           HttpUtility.HtmlEncode(r(6).ToString()) & "','" & 
                           HttpUtility.HtmlEncode(dr("userid").ToString()) & "','" & 
                           HttpUtility.HtmlEncode(r(8).ToString()) & "','" & 
                           HttpUtility.HtmlEncode(r(9).ToString()) & "')"">" & 
                           HttpUtility.HtmlEncode(dr("trailerNo").ToString().ToUpper()) & "</span>"

                    r(7) = HttpUtility.HtmlEncode(dr("username").ToString().ToUpper())

                    t.Rows.Add(r)
                    i = i + 1
                Catch ex As Exception
                    ' SECURITY FIX: Log error but don't expose details
                    SecurityHelper.LogError("FillVehiclesGrid data processing error", ex, Server)
                End Try
            End While
            
            If t.Rows.Count = 0 Then
                r = t.NewRow
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
                t.Rows.Add(r)
            End If
            
            Session("exceltable") = t
            
            For j As Integer = 0 To t.Rows.Count - 1
                Try
                    a = New ArrayList
                    a.Add(t.DefaultView.Item(j)(0))
                    a.Add(t.DefaultView.Item(j)(1))
                    a.Add(t.DefaultView.Item(j)(2))
                    a.Add(t.DefaultView.Item(j)(3))
                    a.Add(t.DefaultView.Item(j)(4))
                    a.Add(t.DefaultView.Item(j)(5))
                    a.Add(t.DefaultView.Item(j)(6))
                    a.Add(t.DefaultView.Item(j)(7))
                    a.Add(t.DefaultView.Item(j)(8))
                    a.Add(t.DefaultView.Item(j)(9))
                    aa.Add(a)
                Catch ex As Exception
                    ' SECURITY FIX: Log error but don't expose details
                    SecurityHelper.LogError("FillVehiclesGrid array processing error", ex, Server)
                End Try
            Next

            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)
            
            Return json
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("FillVehiclesGrid error", ex, Server)
            Return "[]"
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Function

    Public Sub AddData(ByVal uid As String, ByVal tname As String, ByVal insdatetime As String, ByVal eml1 As String, ByVal emlcc As String, ByVal raodtax As String, ByVal puspakam As String, ByVal insurence As String)
        Dim conn As SqlConnection = Nothing
        
        Try
            ' SECURITY FIX: Validate date formats
            Dim inspectionDate As DateTime
            Dim roadTaxDate As DateTime
            Dim puspakomDate As DateTime
            Dim insuranceDate As DateTime
            
            If Not DateTime.TryParse(insdatetime, inspectionDate) Then
                Response.Write("No")
                Return
            End If
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' Process date values
            Dim roadTaxValue As String = "NULL"
            If Not String.IsNullOrEmpty(raodtax) AndAlso DateTime.TryParse(raodtax, roadTaxDate) Then
                roadTaxValue = "'" + roadTaxDate.ToString("yyyy/MM/dd 00:00:00") + "'"
            End If
            
            Dim puspakomValue As String = "NULL"
            If Not String.IsNullOrEmpty(puspakam) AndAlso DateTime.TryParse(puspakam, puspakomDate) Then
                puspakomValue = "'" + puspakomDate.ToString("yyyy/MM/dd 00:00:00") + "'"
            End If
            
            Dim insuranceValue As String = "NULL"
            If Not String.IsNullOrEmpty(insurence) AndAlso DateTime.TryParse(insurence, insuranceDate) Then
                insuranceValue = "'" + insuranceDate.ToString("yyyy/MM/dd 00:00:00") + "'"
            End If
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("INSERT INTO trailer (trailerNo, inspectionDate, RoadTax, Puspakom, Insurance, userid, emailid1, emailid2) VALUES (@trailerNo, @inspectionDate, @roadTax, @puspakom, @insurance, @userid, @emailid1, @emailid2)", conn)
            cmd.Parameters.AddWithValue("@trailerNo", tname)
            cmd.Parameters.AddWithValue("@inspectionDate", inspectionDate.ToString("yyyy/MM/dd 00:00:00"))
            
            If roadTaxValue = "NULL" Then
                cmd.Parameters.AddWithValue("@roadTax", DBNull.Value)
            Else
                cmd.Parameters.AddWithValue("@roadTax", roadTaxDate.ToString("yyyy/MM/dd 00:00:00"))
            End If
            
            If puspakomValue = "NULL" Then
                cmd.Parameters.AddWithValue("@puspakom", DBNull.Value)
            Else
                cmd.Parameters.AddWithValue("@puspakom", puspakomDate.ToString("yyyy/MM/dd 00:00:00"))
            End If
            
            If insuranceValue = "NULL" Then
                cmd.Parameters.AddWithValue("@insurance", DBNull.Value)
            Else
                cmd.Parameters.AddWithValue("@insurance", insuranceDate.ToString("yyyy/MM/dd 00:00:00"))
            End If
            
            cmd.Parameters.AddWithValue("@userid", uid)
            cmd.Parameters.AddWithValue("@emailid1", eml1)
            cmd.Parameters.AddWithValue("@emailid2", If(String.IsNullOrEmpty(emlcc), DBNull.Value, CType(emlcc, Object)))
            
            conn.Open()
            Dim result As Integer = cmd.ExecuteNonQuery()
            
            Response.Write(If(result > 0, "Yes", "No"))
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("AddData error", ex, Server)
            Response.Write("No")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Public Sub UpdateData(ByVal id As String, ByVal insdatetime As String, ByVal tname As String, ByVal uid As String, ByVal eml1 As String, ByVal emlcc As String, ByVal raodtax As String, ByVal puspakam As String, ByVal insurence As String)
        Dim conn As SqlConnection = Nothing
        
        Try
            ' SECURITY FIX: Validate date formats
            Dim inspectionDate As DateTime
            Dim roadTaxDate As DateTime
            Dim puspakomDate As DateTime
            Dim insuranceDate As DateTime
            
            If Not DateTime.TryParse(insdatetime, inspectionDate) Then
                Response.Write("No")
                Return
            End If
            
            ' Process date values
            Dim roadTaxValue As String = "NULL"
            If Not String.IsNullOrEmpty(raodtax) AndAlso DateTime.TryParse(raodtax, roadTaxDate) Then
                roadTaxValue = "'" + roadTaxDate.ToString("yyyy/MM/dd 00:00:00") + "'"
            End If
            
            Dim puspakomValue As String = "NULL"
            If Not String.IsNullOrEmpty(puspakam) AndAlso DateTime.TryParse(puspakam, puspakomDate) Then
                puspakomValue = "'" + puspakomDate.ToString("yyyy/MM/dd 00:00:00") + "'"
            End If
            
            Dim insuranceValue As String = "NULL"
            If Not String.IsNullOrEmpty(insurence) AndAlso DateTime.TryParse(insurence, insuranceDate) Then
                insuranceValue = "'" + insuranceDate.ToString("yyyy/MM/dd 00:00:00") + "'"
            End If
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            
            ' SECURITY FIX: Use parameterized query
            Dim cmd As New SqlCommand("UPDATE trailer SET trailerNo=@trailerNo, emailid1=@emailid1, emailid2=@emailid2, inspectionDate=@inspectionDate, RoadTax=@roadTax, Puspakom=@puspakom, Insurance=@insurance, userid=@userid WHERE id=@id", conn)
            cmd.Parameters.AddWithValue("@trailerNo", tname)
            cmd.Parameters.AddWithValue("@inspectionDate", inspectionDate.ToString("yyyy/MM/dd 00:00:00"))
            
            If roadTaxValue = "NULL" Then
                cmd.Parameters.AddWithValue("@roadTax", DBNull.Value)
            Else
                cmd.Parameters.AddWithValue("@roadTax", roadTaxDate.ToString("yyyy/MM/dd 00:00:00"))
            End If
            
            If puspakomValue = "NULL" Then
                cmd.Parameters.AddWithValue("@puspakom", DBNull.Value)
            Else
                cmd.Parameters.AddWithValue("@puspakom", puspakomDate.ToString("yyyy/MM/dd 00:00:00"))
            End If
            
            If insuranceValue = "NULL" Then
                cmd.Parameters.AddWithValue("@insurance", DBNull.Value)
            Else
                cmd.Parameters.AddWithValue("@insurance", insuranceDate.ToString("yyyy/MM/dd 00:00:00"))
            End If
            
            cmd.Parameters.AddWithValue("@userid", uid)
            cmd.Parameters.AddWithValue("@emailid1", eml1)
            cmd.Parameters.AddWithValue("@emailid2", If(String.IsNullOrEmpty(emlcc), DBNull.Value, CType(emlcc, Object)))
            cmd.Parameters.AddWithValue("@id", id)
            
            conn.Open()
            Dim result As Integer = cmd.ExecuteNonQuery()
            
            Response.Write(If(result > 0, "Yes", "No"))
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("UpdateData error", ex, Server)
            Response.Write("No")
        Finally
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Public Sub DeleteData(ByVal ugData As String)
        Dim conn As SqlConnection = Nothing
        
        Try
            ' SECURITY FIX: Validate input
            If String.IsNullOrEmpty(ugData) Then
                Response.Write("No")
                Return
            End If
            
            ' Parse comma-separated list of IDs
            Dim Tides As String() = ugData.Split(New Char() {","c})
            
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim totalRowsAffected As Integer = 0
            
            For i As Int32 = 0 To Tides.Length - 1
                Try
                    ' SECURITY FIX: Skip "on" value (from checkall checkbox)
                    If Tides(i) = "on" Then
                        Continue For
                    End If
                    
                    ' SECURITY FIX: Validate ID format
                    If Not SecurityHelper.ValidateInput(Tides(i), "^[0-9]+$") Then
                        Continue For
                    End If
                    
                    conn.Open()
                    
                    ' SECURITY FIX: Use parameterized query
                    Dim cmd As New SqlCommand("DELETE FROM trailer WHERE id=@id", conn)
                    cmd.Parameters.AddWithValue("@id", Tides(i))
                    
                    totalRowsAffected += cmd.ExecuteNonQuery()
                    
                Catch ex As Exception
                    ' SECURITY FIX: Log error but don't expose details
                    SecurityHelper.LogError("DeleteData processing error", ex, Server)
                Finally
                    If conn.State = ConnectionState.Open Then
                        conn.Close()
                    End If
                End Try
            Next
            
            Response.Write(If(totalRowsAffected > 0, "Yes", "No"))
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("DeleteData error", ex, Server)
            Response.Write("No")
        End Try
    End Sub
End Class