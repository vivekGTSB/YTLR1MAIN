Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Collections.Generic

Partial Class GetOSSAreaCodeData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim oper As String = Request.QueryString("opr")
        If Request.Cookies("userinfo") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If
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
                GetTransportesCompany()
            Case "5"
                InsertDataCompany()
            Case "6"
                updateDataCompany()
            Case "7"
                DeleteDataCompany()
            Case "8"
                GetDataCompany()

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


        'If role = "User" Then
        '    cond = " where createdby ='" & userid & "'"
        'ElseIf role = "SuperUser" Or role = "Operator" Then
        '    cond = " where (createdby in( " & userslist & ") Or createdby ='" & userid & "')"
        'End If


        Dim sqlstr As String = "select * from oss_area_code order by area "
        'response.write(sqlstr)
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
        Dim cmd As SqlCommand = New SqlCommand(sqlstr, conn)
        Dim dr As SqlDataReader
        Try
            Dim c As Integer = 0
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                c += 1
                a = New ArrayList()

                a.Add(dr("area_code"))

                a.Add(c)
                a.Add(dr("area_code"))
                a.Add(dr("area"))
                If IsDBNull(dr("state")) Then
                    a.Add("-")
                Else
                    a.Add(dr("state"))
                End If
                If IsDBNull(dr("region")) Then
                    a.Add("-")
                Else
                    a.Add(dr("region"))
                End If
                a.Add(dr("area_code"))
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
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim areacode As String = Request.QueryString("ac")
            Dim area As String = Request.QueryString("area")
            Dim state As String = Request.QueryString("state")
            Dim region As String = Request.QueryString("region")

            Dim cmd As New SqlCommand("insert into oss_area_code (area_code,area,state,region) values ('" & areacode & "','" & area & "','" & state & "','" & region & "')", conn)
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


    Private Sub UpdateData()
        Dim res As String = "0"
        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim areacode As String = Request.QueryString("ac")
            Dim area As String = Request.QueryString("area")
            Dim state As String = Request.QueryString("state")
            Dim region As String = Request.QueryString("region")
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim cmd As New SqlCommand("update oss_area_code set area='" & area & "',state='" & state & "',region='" & region & "' where area_code='" & areacode & "'", conn)
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
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmd As New SqlCommand("delete from oss_area_code where area_code=0", conn)
            Try
                conn.Open()
                Dim cnt As Int16 = ids.Length()
                For i As Int16 = 0 To cnt - 1
                    Try
                        cmd = New SqlCommand("delete from oss_area_code where area_code=" & ids(i).ToString(), conn)
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

    Private Sub GetTransportesCompany()
        Dim aa As New ArrayList()
        Dim a As ArrayList
        Dim sqlstr As String = "select companyid,companyname,[dbo].[fnGetUserNames](companyid) as transporters,[dbo].fnGetGeofenceNames(companyid) as geofences,TransporterList,GeofenceList from ec_company"
        'response.write(sqlstr)
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As SqlCommand = New SqlCommand(sqlstr, conn)
        Dim dr As SqlDataReader
        Try
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()

                a = New ArrayList()
                a.Add(dr("companyid"))
                a.Add(dr("companyid"))
                a.Add(dr("companyname").ToString().ToUpper())
                a.Add(dr("transporters").ToString().ToUpper())
                a.Add(dr("geofences").ToString().ToUpper())
                a.Add(dr("companyid"))
                a.Add(dr("TransporterList"))
                a.Add(dr("GeofenceList"))
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

    Private Sub InsertDataCompany()
        Dim res As String = "0"

        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim companyname As String = Request.QueryString("name")
            Dim transporters As String = Request.QueryString("trans")
            Dim geofences As String = Request.QueryString("geof")

            Dim cmd As New SqlCommand("insert into [ec_company] (CompanyName,TransporterList,GeofenceList) values (@comp,@transpo,@geos)", conn)
            cmd.Parameters.AddWithValue("@comp", companyname)
            cmd.Parameters.AddWithValue("@transpo", transporters)
            cmd.Parameters.AddWithValue("@geos", geofences)
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


    Private Sub updateDataCompany()
        Dim res As String = "0"

        Try
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim companyname As String = Request.QueryString("name")
            Dim transporters As String = Request.QueryString("trans")
            Dim geofences As String = Request.QueryString("geof")

            Dim compid As String = Request.QueryString("compid")

            Dim cmd As New SqlCommand("update [ec_company] set CompanyName=@comp,TransporterList=@transpo,GeofenceList=@geos where CompanyId=" & compid & "", conn)
            cmd.Parameters.AddWithValue("@comp", companyname)
            cmd.Parameters.AddWithValue("@transpo", transporters)
            cmd.Parameters.AddWithValue("@geos", geofences)
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

    Private Sub DeleteDataCompany()
        Try
            Dim chekitems As String = Request.QueryString("geoid")
            Dim result As Integer
            Dim res As String = "0"
            Dim ids As String() = chekitems.Split(",")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Try
                conn.Open()
                Dim cnt As Int16 = ids.Length()
                For i As Int16 = 0 To cnt - 1
                    Try
                        cmd = New SqlCommand("delete from ec_company where companyid =" & ids(i).ToString(), conn)
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

    Private Sub GetDataCompany()
        Dim aa As New ArrayList()
        Dim userslist As New List(Of vals)
        Dim geoslist As New List(Of vals)
        Dim v As vals
        Dim sqlstr As String = "select userid,username,(case when exists (select item from fn_getcompanylist(0) where item=userid) then 0 else 1 end) as status  from userTBL where role='User' order by username"
        'response.write(sqlstr)
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As SqlCommand = New SqlCommand(sqlstr, conn)
        Dim dr As SqlDataReader
        Try
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                v = New vals
                v.id = dr("userid")
                v.text = dr("username").ToString().ToUpper()
                v.status = dr("status")
                userslist.Add(v)

            End While
            dr.Close()
            sqlstr = "select IsNull(shiptocode,'') shiptocode ,geofenceid ,geofencename,(case when exists (select item from fn_getgeofencelist(0) where item=geofenceid) then 0 else 1 end) as status from geofence where accesstype=1 and shiptocode not like '000%' order by geofencename"
            cmd.CommandText = sqlstr
            dr = cmd.ExecuteReader()
            While dr.Read()
                v = New vals
                v.id = dr("geofenceid")
                v.text = dr("geofencename").ToString().ToUpper() & " - " & dr("shiptocode").ToString().ToUpper()
                v.status = dr("status")
                geoslist.Add(v)
            End While
            dr.Close()
            aa.Add(userslist)
            aa.Add(geoslist)
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

    Structure vals
        Dim id As Integer
        Dim text As String
        Dim status As Boolean
    End Structure

End Class
