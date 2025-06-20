Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetLVAData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim oper As String = Request.QueryString("opr")

        Select Case oper.ToUpper()
            Case "0"
                GetData()
            Case "1"
                InsertData()
            Case "2"
                UpdateData()
            Case "3"
                DeleteData()
            Case "4"
                MultiInsert()
            Case "5"
                MultiInsertALL()
        End Select

    End Sub

    Private Sub GetData()
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Dim sUserid As String = Request.QueryString("suid")
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim role As String = Request.Cookies("userinfo")("role")
        Dim userslist As String = Request.Cookies("userinfo")("userslist")
        Dim cond As String = ""


        If sUserid = "ALL" Then
            If role = "User" Then
                cond = " where userid ='" & userid & "'"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cond = " where userid in( " & userslist & ")"
            End If
        Else
            cond = " where userid ='" & sUserid & "'"
        End If



        Dim sqlstr As String = "select gt.plateno,at.id,at.emaillist,at.mobileno from (select plateno from vehicleTBL " & cond & ") as gt left outer join  void_alert_settings at on at.plateno=gt.plateno"
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As SqlCommand = New SqlCommand(sqlstr, conn)
        Dim dr As SqlDataReader
        Try
            Dim c As Integer = 0
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                c += 1
                a = New ArrayList()

                If IsDBNull(dr("id")) Then
                    a.Add("0")
                Else
                    a.Add(dr("id"))
                End If

                a.Add(c)

                a.Add(dr("plateno"))

                If IsDBNull(dr("emaillist")) Then
                    a.Add("")
                Else
                    a.Add(dr("emaillist"))
                End If

                If IsDBNull(dr("mobileno")) Then
                    a.Add("")
                Else
                    a.Add(dr("mobileno"))
                End If

                a.Add(dr("plateno"))

                If IsDBNull(dr("id")) Then
                    a.Add("0")
                Else
                    a.Add(dr("id"))
                End If
                aa.Add(a)
            End While
            dr.Close()
        Catch ex As Exception

        Finally
            conn.Close()
        End Try
        Dim json As String = ""
        Dim jss As New Newtonsoft.Json.JsonSerializer()
        json = JsonConvert.SerializeObject(aa, Formatting.None)
        Response.ContentType = "text/plain"
        Response.Write(json)
    End Sub

    Private Sub InsertData()
        Dim res As String = "0"
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim plateno As String = Request.QueryString("geoid")
            Dim email As String = Request.QueryString("eml")

            Dim mob As String = Request.QueryString("mob")
            Dim cmd As New SqlCommand("insert into void_alert_settings (plateno,emaillist,mobileno,updateddatetime) values ('" & plateno & "','" & email & "','" & mob & "','" & Now.ToString("yyyy/MM/dd HH:mm:ss") & "')", conn)
            Try
                conn.Open()
                res = cmd.ExecuteNonQuery().ToString()

            Catch ex As Exception
                res = ex.Message
            Finally
                conn.Close()
            End Try
        Catch ex As Exception
            res = ex.Message
        End Try

        Response.ContentType = "text/plain"
        Response.Write(res)
    End Sub

    Private Sub MultiInsert()
        Dim res As String = "0"
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim platenos() As String = Request.QueryString("geoid").Split(",")
            Dim email As String = Request.QueryString("eml")
            Dim mob As String = Request.QueryString("mob")
            Dim plateno As String = ""

            For i As Integer = 0 To platenos.Length - 1
                plateno = plateno & "'" & platenos(i) & "',"
            Next

            If plateno.Length > 0 Then
                plateno = plateno.Substring(0, plateno.Length - 1)
            End If


            Dim cmd As New SqlCommand("delete from  void_alert_settings where plateno in (" & plateno & ")", conn)

            Try
                conn.Open()
                res = cmd.ExecuteNonQuery().ToString()
            Catch ex As Exception

            Finally
                conn.Close()
            End Try

            For i As Integer = 0 To platenos.Length - 1
                cmd = New SqlCommand("insert into void_alert_settings (plateno,emaillist,mobileno,updateddatetime) values ('" & platenos(i) & "','" & email & "','" & mob & "','" & Now.ToString("yyyy/MM/dd HH:mm:ss") & "')", conn)
                Try
                    conn.Open()
                    res = cmd.ExecuteNonQuery().ToString()
                Catch ex As Exception
                    res = ex.Message
                Finally
                    conn.Close()
                End Try
            Next


        Catch ex As Exception
            res = ex.Message
        End Try

        Response.ContentType = "text/plain"
        Response.Write(res)
    End Sub

    Private Sub MultiInsertALL()
        Dim res As String = "0"
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim email As String = Request.QueryString("eml")
            Dim mob As String = Request.QueryString("mob")
            Dim plateno As String = ""
            Dim ds As New DataSet
            Dim da As New SqlDataAdapter("select plateno from vehicleTBL where userid in (select userid from userTBL where role='User')", conn)
            da.Fill(ds)

            If ds.Tables(0).Rows.Count > 0 Then
                For counter As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    plateno = plateno & " '" & ds.Tables(0).Rows(counter)("plateno").ToString() & "',"
                Next
            End If

            If plateno.Length > 0 Then
                plateno = plateno.Substring(0, plateno.Length - 1)
            End If

            Dim platenos() As String = plateno.Split(",")

            Dim cmd As New SqlCommand("delete from  void_alert_settings ", conn)

            Try
                conn.Open()
                res = cmd.ExecuteNonQuery().ToString()
            Catch ex As Exception
                Response.Write("Here1 " & ex.Message)
            Finally
                conn.Close()
            End Try

            For i As Integer = 0 To platenos.Length - 1
                cmd = New SqlCommand("insert into void_alert_settings (plateno,emaillist,mobileno,updateddatetime) values (" & platenos(i) & ",'" & email & "','" & mob & "','" & Now.ToString("yyyy/MM/dd HH:mm:ss") & "')", conn)
                Try
                    conn.Open()
                    res = cmd.ExecuteNonQuery().ToString()
                    ' If Convert.ToInt32(res) = 0 Then
                    ' Response.Write("<br/>" & cmd.CommandText & "<br/>")
                    ' End If

                Catch ex As Exception
                    Response.Write("Here2 " & ex.Message & "<br/>" & cmd.CommandText)
                Finally
                    conn.Close()
                End Try
            Next
        Catch ex As Exception
            res = ex.Message
        End Try

        Response.ContentType = "text/plain"
        Response.Write(res)
    End Sub
    Private Sub UpdateData()
        Dim res As String = "0"
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim geoid As String = Request.QueryString("geoid")
            Dim email As String = Request.QueryString("eml")

            Dim mob As String = Request.QueryString("mob")
            Dim cmd As New SqlCommand("update void_alert_settings set emaillist='" & email & "',mobileno='" & mob & "',updateddatetime='" & Now.ToString("yyyy/MM/dd HH:mm:ss") & "' where id='" & geoid & "'", conn)
            Try
                conn.Open()
                res = cmd.ExecuteNonQuery.ToString()

            Catch ex As Exception
                res = ex.Message
            Finally
                conn.Close()
            End Try
        Catch ex As Exception
            res = ex.Message
        End Try

        Response.ContentType = "text/plain"
        Response.Write(res)
    End Sub

    Private Sub DeleteData()
        Try
            Dim chekitems As String = Request.QueryString("geoid")
            Dim result As Integer
            Dim res As String = "0"
            Dim ids As String() = chekitems.Split(",")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand("delete from void_alert_settings where id=0", conn)
            Try
                conn.Open()
                Dim cnt As Int16 = ids.Length()
                For i As Int16 = 0 To cnt - 1
                    Try
                        cmd = New SqlCommand("delete from void_alert_settings where id =" & ids(i).ToString(), conn)
                        result = cmd.ExecuteNonQuery
                        If result > 0 Then
                            res = "1"
                        End If
                    Catch ex As Exception
                        Response.Write("@@" & ex.Message)
                    End Try
                Next
            Catch ex As Exception
                Response.Write("@@@" & ex.Message)
            Finally
                conn.Close()
            End Try

            Response.ContentType = "text/plain"
            Response.Write(res)
        Catch ex As Exception
            Response.Write("@" & ex.Message)
        End Try

    End Sub

End Class
