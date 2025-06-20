Imports System.Data
Imports System.Text
Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class GetSMSNotificationData
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
                FillData()
        End Select

    End Sub
    Private Sub FillData()
        Dim unitlist As ArrayList = New ArrayList
        Dim json As String = Nothing
        Dim aa As New ArrayList
        Dim a As ArrayList
        Try

            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim dr As SqlDataReader
            'Dim cmd As SqlCommand
            Dim cmd As SqlCommand = New SqlCommand("select geofencename,shiptocode from geofence where shiptocode<>'0' and  shiptocode not in (select shiptocode from oss_notification) order  by geofencename", conn)

            conn.Open()
            dr = cmd.ExecuteReader()
            unitlist.Clear()

            While dr.Read()
                a = New ArrayList()
                a.Add(dr("shiptocode") + " - " + dr("geofencename").ToString().ToUpper())
                a.Add(dr("shiptocode"))
                aa.Add(a)
                '  unitlist.Add(New ListItem(dr("shiptocode") + " - " + dr("geofencename").ToString().ToUpper(), dr("shiptocode").ToString()))
            End While
            conn.Close()
        Catch ex As Exception

        End Try
        json = JsonConvert.SerializeObject(aa, Formatting.None)
        Response.ContentType = "text/plain"
        Response.Write(json)
    End Sub
    Private Sub GetData()
        Dim json As String = Nothing
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try
            '  Dim suserid As String = ugData
            Dim userid As String = HttpContext.Current.Request.Cookies("userinfo")("userid")
            Dim role As String = HttpContext.Current.Request.Cookies("userinfo")("role")
            Dim userslist As String = HttpContext.Current.Request.Cookies("userinfo")("userslist")
            Dim aa As New ArrayList
            Dim a As ArrayList
            ' If Not suserid = "0" Then
            Dim poistable As New DataTable
            Dim r As DataRow

            poistable.Columns.Add(New DataColumn("S No"))
            poistable.Columns.Add(New DataColumn("Ship To Code"))
            poistable.Columns.Add(New DataColumn("Name"))
            poistable.Columns.Add(New DataColumn("Mobile List"))
            poistable.Columns.Add(New DataColumn("OTP"))

            poistable.Columns(3).DataType = System.Type.GetType("System.String")
            Dim cmd As SqlCommand = New SqlCommand("select oss.OtpFlag,g.geofencename,oss.shiptocode,oss.mobileno from oss_notification as oss join geofence g on g.shiptocode=oss.shiptocode", conn)
            Dim dr As SqlDataReader
            conn.Open()
            dr = cmd.ExecuteReader()
            Dim i As Integer = 0
            While dr.Read
                r = poistable.NewRow
                a = New ArrayList()
                a.Add(dr("shiptocode"))
                r(0) = i.ToString()
                a.Add(i)
                a.Add(dr("shiptocode"))
                r(1) = dr("shiptocode").ToString()
                a.Add(dr("geofencename"))
                r(2) = dr("geofencename").ToString()
                a.Add(dr("mobileno"))
                'r(3) = "Num: " & dr("mobileno").ToString()
                If dr("mobileno").ToString.Contains(",") Then
                    r(3) = dr("mobileno").Replace(",", ";")
                Else
                    r(3) = dr("mobileno").ToString
                End If

                If dr("OtpFlag") Then
                    a.Add("Enabled")
                    a.Add("1")
                    r(4) = "Enabled"
                Else
                    a.Add("Disabled")
                    a.Add("0")
                    r(4) = "Disabled"
                End If
                poistable.Rows.Add(r)
                aa.Add(a)
                i = i + 1
            End While
            If poistable.Rows.Count = 0 Then
                r = poistable.NewRow
                r(0) = "1"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                poistable.Rows.Add(r)
            End If
            Session.Remove("exceltable")
            Session("exceltable") = poistable
            '  End If
            json = JsonConvert.SerializeObject(aa, Formatting.None)
        Catch ex As Exception
            ' Return ex.Message
        Finally
            conn.Close()
        End Try
        'Return json

        Response.ContentType = "text/plain"
        Response.Write(json)
    End Sub

    Private Sub InsertData()
        Dim strqury As String = ""
        Dim res As Integer = 0
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

        Try
            Dim mobile1 As String = Request.QueryString("mobile1")
            Dim mobile2 As String = Request.QueryString("mobile2")
            Dim mobile3 As String = Request.QueryString("mobile3")
            Dim mobile4 As String = Request.QueryString("mobile4")
            Dim mobile5 As String = Request.QueryString("mobile5")
            Dim chkotp As String = Request.QueryString("chkotp")

            Dim scode As String = Request.QueryString("scode")

            Dim mob As String = Request.QueryString("mob")

            Dim strmobile As String = Nothing
            If mobile1 <> "" Then
                strmobile = mobile1 & ","
            End If
            If mobile2 <> "" Then
                strmobile += mobile2 & ","
            End If
            If mobile3 <> "" Then
                strmobile += mobile3 & ","
            End If
            If mobile4 <> "" Then
                strmobile += mobile4 & ","
            End If
            If mobile5 <> "" Then
                strmobile += mobile5 & ","
            End If
            If strmobile <> "" Then
                strmobile = strmobile.Remove(strmobile.Length - 1, 1)
            End If

            strqury = "insert into oss_notification(otpflag,shiptocode,mobileno,insert_datetime,update_datetime)"
            strqury = strqury + "values ('" & chkotp & "','" & scode & "','" & strmobile & "' ,'" & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "','" & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "')" ''" & statusname & "'
            'Else
            'strqury = "update oss_notification set mobileno='" & strmobile & "' where shiptocode='" & scode & "' "
            'End If

            Dim cmd As New SqlCommand(strqury, conn)
            conn.Open()
            res = cmd.ExecuteNonQuery()


        Catch ex As Exception
            res = ex.Message
        Finally
            conn.Close()
        End Try

        Response.ContentType = "text/plain"
        Response.Write(res)
    End Sub

    Private Sub UpdateData()
        Try


            Dim strqury As String = ""
            Dim res As Integer = 0
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Try
                Dim mobile1 As String = Request.QueryString("mobile1")
                Dim mobile2 As String = Request.QueryString("mobile2")
                Dim mobile3 As String = Request.QueryString("mobile3")
                Dim mobile4 As String = Request.QueryString("mobile4")
                Dim mobile5 As String = Request.QueryString("mobile5")
                Dim chkotp As String = Request.QueryString("chkotp")
                Dim scode As String = Request.QueryString("scode")

                Dim mob As String = Request.QueryString("mob")

                Dim strmobile As String = Nothing
                If mobile1 <> "" Then
                    strmobile = mobile1 & ","
                End If
                If mobile2 <> "" Then
                    strmobile += mobile2 & ","
                End If
                If mobile3 <> "" Then
                    strmobile += mobile3 & ","
                End If
                If mobile4 <> "" Then
                    strmobile += mobile4 & ","
                End If
                If mobile5 <> "" Then
                    strmobile += mobile5 & ","
                End If
                If strmobile <> "" Then
                    strmobile = strmobile.Remove(strmobile.Length - 1, 1)
                End If


                'Else
                strqury = "update oss_notification set  OTpFlag='" & chkotp & "', mobileno='" & strmobile & "',update_datetime='" & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "' where shiptocode='" & scode & "' "
                'End If

                Dim cmd As New SqlCommand(strqury, conn)
                conn.Open()
                res = cmd.ExecuteNonQuery()


            Catch ex As Exception
                res = ex.Message
            Finally
                conn.Close()
            End Try

            Response.ContentType = "text/plain"
            Response.Write(res)
        Catch ex As Exception
            Response.Write(" IMP Msg : " & ex.Message)
        End Try
    End Sub

    Private Sub DeleteData()
        Try
            Dim chekitems As String = Request.QueryString("geoid")
            chekitems = chekitems.Replace("[", "")
            chekitems = chekitems.Replace("]", "")
            chekitems.Replace("""", "")
            chekitems.Replace("""", "")
            Dim result As Integer
            Dim res As String = "0"
            Dim ids As String() = chekitems.Split(",")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand
            Try
                conn.Open()
                Dim cnt As Int64 = ids.Length()
                For i As Int64 = 0 To cnt - 1

                    Try
                        cmd = New SqlCommand("delete from oss_notification where shiptocode='" & ids(i).Replace("""", "") & "'", conn)
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
