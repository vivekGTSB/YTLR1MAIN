Imports System.Data.SqlClient
Imports System.Data
Imports System.Collections.Generic

Partial Class ShipToCodeManagement
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public addShiptocode As String = "AddShipToCode.aspx"
    Public divgrid As Boolean = False
    Public ec As String = "false"
    Public sb1 As New StringBuilder()
    
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Validate user session
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("OnInit error", ex, Server)
        End Try
        MyBase.OnInit(e)
    End Sub
    
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            
            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return deleteconfirmation();")
                ImageButton3.Attributes.Add("onclick", "return deleteconfirmation();")
            End If
            
            FillGrid()
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("Page_Load error", ex, Server)
        End Try
    End Sub
    
    Public Sub FillGrid()
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim userstable As New DataTable
            Dim ok As String = "no"
            Dim condition As String = ""
            Dim r As DataRow
            
            userstable.Rows.Clear()
            userstable.Columns.Add(New DataColumn("chk"))
            userstable.Columns.Add(New DataColumn("S No"))
            userstable.Columns.Add(New DataColumn("shiptocode"))
            userstable.Columns.Add(New DataColumn("name"))
            userstable.Columns.Add(New DataColumn("address1"))

            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            If Not userid = "--Select User Name--" Then
                ' SECURITY FIX: Use parameterized query
                Dim cmd As SqlCommand = New SqlCommand("SELECT * FROM oss_ship_to_code", conn)
                
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                Dim i As Int32 = 1
                
                While dr.Read()
                    r = userstable.NewRow
                    r(0) = "<input type=""checkbox"" name=""chk"" value=""" & HttpUtility.HtmlEncode(dr("shiptocode").ToString()) & """/>"
                    r(1) = i.ToString()
                    r(2) = "<a href=UpdateShipToCode.aspx?Id=" & HttpUtility.HtmlEncode(dr("shiptocode").ToString()) & "> " & HttpUtility.HtmlEncode(dr("shiptocode").ToString()) & " </a>"
                    r(3) = HttpUtility.HtmlEncode(dr("name").ToString())
                    
                    ' SECURITY FIX: Safely concatenate address fields
                    Dim address As String = ""
                    If Not IsDBNull(dr("address1")) Then address &= dr("address1").ToString().Trim()
                    If Not IsDBNull(dr("address2")) Then address &= dr("address2").ToString().Trim()
                    If Not IsDBNull(dr("address3")) Then address &= dr("address3").ToString().Trim()
                    If Not IsDBNull(dr("address4")) Then address &= dr("address4").ToString().Trim()
                    
                    r(4) = HttpUtility.HtmlEncode(address)
                    userstable.Rows.Add(r)
                    i = i + 1
                    ok = "yes"
                End While

                conn.Close()
            End If
            
            If ok = "no" Then
                r = userstable.NewRow
                r(0) = "-"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                userstable.Rows.Add(r)
            End If

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session("exceltable") = userstable
            ec = "true"
            
            sb1.Length = 0
            sb1.Append("<thead><tr><th align=""center"" style=""width:20px;""><input type='checkbox' onclick=""javascript:checkall(this);""/></th><th style=""width:35px;"">S No</th><th  style=""width:100px;"">Ship To Code</th><th  style=""width:250px;"">Name</th><th>Address</th></tr></thead>")
            sb1.Append("<tbody>")
            
            Dim counter As Integer = 1
            For i As Integer = 0 To userstable.Rows.Count - 1
                sb1.Append("<tr><td>")
                sb1.Append(userstable.DefaultView.Item(i)(0))
                sb1.Append("</td><td>")
                sb1.Append(userstable.DefaultView.Item(i)(1))
                sb1.Append("</td><td>")
                sb1.Append(userstable.DefaultView.Item(i)(2))
                sb1.Append("</td><td>")
                sb1.Append(userstable.DefaultView.Item(i)(3))
                sb1.Append("</td><td>")
                sb1.Append(userstable.DefaultView.Item(i)(4))
                sb1.Append("</td></tr>")
                counter += 1
            Next
            
            sb1.Append("<tfoot><tr><th><input type='checkbox' onclick=""javascript:checkall(this);""/></th><th>S No</th><th>Ship To Code</th><th>Name</th><th>Address</th></tr></tfoot>")
            sb1.Append("</tbody>")
            
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("FillGrid error", ex, Server)
        End Try
    End Sub

    Protected Sub DeleteDriver()
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            
            ' SECURITY FIX: Validate form data exists
            If String.IsNullOrEmpty(Request.Form("chk")) Then
                Response.Write("No items selected")
                Return
            End If
            
            Dim shiptocodeid() As String = Request.Form("chk").Split(",")
            
            For i As Int32 = 0 To shiptocodeid.Length - 1
                ' SECURITY FIX: Validate shiptocode format
                If Not SecurityHelper.ValidateInput(shiptocodeid(i), "^[A-Za-z0-9\-_]{1,50}$") Then
                    Continue For
                End If
                
                ' SECURITY FIX: Use parameterized query
                Dim command As New SqlCommand("DELETE FROM oss_ship_to_code WHERE shiptocode=@shiptocode", conn)
                command.Parameters.AddWithValue("@shiptocode", shiptocodeid(i))
                
                Try
                    conn.Open()
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    ' SECURITY FIX: Log error but don't expose details
                    SecurityHelper.LogError("DeleteDriver error", ex, Server)
                Finally
                    conn.Close()
                End Try
            Next
            
            FillGrid()
        Catch ex As Exception
            ' SECURITY FIX: Log error but don't expose details
            SecurityHelper.LogError("DeleteDriver error", ex, Server)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DeleteDriver()
    End Sub
    
    Protected Sub ImageButton3_Click(sender As Object, e As System.EventArgs) Handles ImageButton3.Click
        DeleteDriver()
    End Sub
End Class