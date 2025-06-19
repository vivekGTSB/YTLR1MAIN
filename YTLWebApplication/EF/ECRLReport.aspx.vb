Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports AspMap
Imports Newtonsoft.Json

Partial Class ECRLReport
    Inherits System.Web.UI.Page
    Public Shared ec As String = "false"
    Public show As Boolean = False
    
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' Check authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("INIT_ERROR", "Error in OnInit: " & ex.Message)
            Response.Redirect("~/Login.aspx")
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' Validate session
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Response.Redirect("~/Login.aspx")
                Return
            End If
            
            ' Set user ID from session
            If HttpContext.Current.Session("userid") IsNot Nothing Then
                uid.Value = HttpContext.Current.Session("userid").ToString()
            End If
            
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                LoadTransporterNames()
            End If
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("PAGE_LOAD_ERROR", "Error in Page_Load: " & ex.Message)
        End Try
    End Sub
    
    Private Sub LoadTransporterNames()
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim query As String = "SELECT userid, username FROM YTLDB.dbo.userTBL WHERE userid NOT IN (7144,7145,7146,7147,7148,7099,7180) AND companyname LIKE 'ytl%' AND role = 'User' ORDER BY username"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        ddltransportername.Items.Clear()
                        ddltransportername.Items.Add(New ListItem("ALL", "ALL"))
                        While dr.Read()
                            ddltransportername.Items.Add(New ListItem(SecurityHelper.SanitizeForHtml(dr("username").ToString()), dr("userid").ToString()))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("LOAD_TRANSPORTERS_ERROR", "Error loading transporters: " & ex.Message)
        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function GetData(ByVal fromd As String) As String
        Try
            ' Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Return "{""error"":""Unauthorized""}"
            End If
            
            ' Validate input
            If Not SecurityHelper.ValidateInput(fromd, "date") Then
                SecurityHelper.LogSecurityEvent("INVALID_DATE_INPUT", "Invalid date format: " & fromd)
                Return "{""error"":""Invalid date format""}"
            End If
            
            Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim aa As New ArrayList()
            Dim a As ArrayList
            Dim userid As String = HttpContext.Current.Session("userid").ToString()
            Dim role As String = HttpContext.Current.Session("role").ToString()
            Dim uname As String = HttpContext.Current.Session("username").ToString()
            Dim userslist As String = HttpContext.Current.Session("userslist").ToString()
            
            Try
                Using cmd As New SqlCommand("SELECT * FROM dbo.fn_getecrlsummary(@date) ORDER BY shiptocode", conn)
                    cmd.Parameters.AddWithValue("@date", fromd)
                    conn.Open()
                    
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim datastruct As New Dictionary(Of String, shiptocode)
                        Dim statedict As New Dictionary(Of String, String)
                        Dim shipcd As shiptocode
                        
                        ' Initialize state dictionary with sanitized data
                        InitializeStateDict(statedict)
                        
                        ' Process main data
                        While dr.Read()
                            ProcessShipToData(dr, datastruct)
                        End While
                        
                        If Not dr.IsClosed() Then
                            dr.Close()
                        End If
                        
                        ' Process ATA data
                        cmd.CommandText = "SELECT * FROM dbo.fn_getecrlatasummary(@date) ORDER BY shiptocode"
                        Using dr2 As SqlDataReader = cmd.ExecuteReader()
                            While dr2.Read()
                                ProcessATAData(dr2, datastruct)
                            End While
                        End Using
                        
                        ' Build result
                        BuildResult(datastruct, statedict, aa)
                    End Using
                End Using
                
            Catch ex As Exception
                SecurityHelper.LogSecurityEvent("GETDATA_ERROR", "Error in GetData: " & ex.Message)
                a = New ArrayList()
                a.Add("Error occurred while processing data")
                aa.Add(a)
            Finally
                If conn.State = ConnectionState.Open Then
                    conn.Close()
                End If
            End Try
            
            Dim json As String = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
            Return json
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GETDATA_EXCEPTION", "Exception in GetData: " & ex.Message)
            Return "{""error"":""An error occurred""}"
        End Try
    End Function
    
    Private Shared Sub InitializeStateDict(statedict As Dictionary(Of String, String))
        ' PAHANG locations
        statedict.Add("76940", "PAHANG;6-1-SGL;B.I.MAHKOTA")
        statedict.Add("111683", "PAHANG;6-1-CH;PANCHING SELATAN331")
        statedict.Add("100031", "PAHANG;6-1-DE;PANCHING TIMURSA")
        statedict.Add("77922", "PAHANG;6-1-KG;KG BELIMBING, MARANB")
        statedict.Add("100123", "PAHANG;6-2-CH;AUR GADING, MARAN373")
        statedict.Add("77631", "PAHANG;6-2-BTG;KG BENTUNG, MARAN")
        statedict.Add("111006", "PAHANG;6-2-CH;PEKAN TAJAU392")
        statedict.Add("100419", "PAHANG;6-3-CH;KG JENGKA BATU 13406")
        statedict.Add("112203", "PAHANG;6-3-CH;KG KETAM, MARAN372")
        statedict.Add("77033", "PAHANG;6-3-KGT;CHENOR")
        statedict.Add("78380", "PAHANG;6-3-TML;KG TUALANG, TEMERLOH")
        statedict.Add("112017", "PAHANG;6-3-CH;PEKAN AWAH, TEMERLOH409")
        statedict.Add("78704", "PAHANG;6-1-GMB;BATU 16, JALAN GAMBANG")
        statedict.Add("111511", "PAHANG;6-1-CH;KG MELAYU, GAMBANG340")

        ' TERENGGANU locations
        statedict.Add("78011", "TERENGGANU;5-3-BSR;PLANT 3, JABOR")
        statedict.Add("75847", "TERENGGANU;5-4-JBO;PLANT 4, JABOR")
        statedict.Add("75916", "TERENGGANU;4-3-CM3;DUNGUN")
        statedict.Add("99484", "TERENGGANU;4-7-T11;BRIDGE T118,DUNGUN8")
        statedict.Add("79138", "TERENGGANU;4-8-DSM;CHRONG SERDANG, PAKA PAKA")
        statedict.Add("111980", "TERENGGAN;5-1-CH2;CH237,KERTEH37U")
        statedict.Add("78317", "TERENGGANU;5-1-KER;KERTEH")
        statedict.Add("78747", "TERENGGANU;3-1-CH1;CHALOK02")
        statedict.Add("111918", "TERENGGAN;5-2-CH2;CUKAI69U")
        statedict.Add("79458", "TERENGGANU;5-2-MLG;MAK LAGAM, CUKAI")
        statedict.Add("111294", "TERENGGAN;5-2-CH2;KIJAL59U")
        statedict.Add("99729", "TERENGGANU;3-2-CH1;BELARA, SETIU23")
        statedict.Add("100239", "TERENGGAN;4-2-CM2;BKT PAK MERIAH, MARANGU")
        statedict.Add("100163", "TERENGGAN;3-4-CH1;BKT TINGGI SERDANG38U")
        statedict.Add("99748", "TERENGGANU;4-1-CM1;MARANG")
        statedict.Add("79185", "TERENGGANU;2-4-WGL;TEMBILA")

        ' KELANTAN locations
        statedict.Add("110818", "KELANTAN;1-3-JRS;BUKIT JAWA")
        statedict.Add("100543", "KELANTAN;2-1-PSP;PASIR PUTIH")
        statedict.Add("99485", "KELANTAN;1-1-TJG;TUNJONG")
    End Sub
    
    Private Shared Sub ProcessShipToData(dr As SqlDataReader, datastruct As Dictionary(Of String, shiptocode))
        Dim shiptoCode As String = dr("shiptocode").ToString()
        Dim shifts As String = dr("shifts").ToString()
        Dim counts As Integer = Convert.ToInt32(dr("counts"))
        
        Dim shipcd As shiptocode
        If datastruct.ContainsKey(shiptoCode) Then
            shipcd = datastruct(shiptoCode)
        Else
            shipcd = New shiptocode()
            shipcd.shiptoid = shiptoCode
        End If
        
        Select Case shifts
            Case "11"
                shipcd.s11 = counts
            Case "12"
                shipcd.s12 = counts
            Case "21"
                shipcd.s21 = counts
            Case "22"
                shipcd.s22 = counts
            Case "31"
                shipcd.s31 = counts
            Case "32"
                shipcd.s32 = counts
        End Select
        
        datastruct(shiptoCode) = shipcd
    End Sub
    
    Private Shared Sub ProcessATAData(dr As SqlDataReader, datastruct As Dictionary(Of String, shiptocode))
        Dim shiptoCode As String = dr("shiptocode").ToString()
        Dim shifts As String = dr("shifts").ToString()
        Dim counts As Integer = Convert.ToInt32(dr("counts"))
        
        Dim shipcd As shiptocode
        If datastruct.ContainsKey(shiptoCode) Then
            shipcd = datastruct(shiptoCode)
        Else
            shipcd = New shiptocode()
            shipcd.shiptoid = shiptoCode
        End If
        
        Select Case shifts
            Case "11"
                shipcd.a11 = counts
            Case "12"
                shipcd.a12 = counts
            Case "21"
                shipcd.a21 = counts
            Case "22"
                shipcd.a22 = counts
            Case "31"
                shipcd.a31 = counts
            Case "32"
                shipcd.a32 = counts
        End Select
        
        datastruct(shiptoCode) = shipcd
    End Sub
    
    Private Shared Sub BuildResult(datastruct As Dictionary(Of String, shiptocode), statedict As Dictionary(Of String, String), aa As ArrayList)
        ' Build result for each state
        BuildStateResult("PAHANG", datastruct, statedict, aa)
        BuildStateResult("TERENGGANU", datastruct, statedict, aa)
        BuildStateResult("KELANTAN", datastruct, statedict, aa)
    End Sub
    
    Private Shared Sub BuildStateResult(stateName As String, datastruct As Dictionary(Of String, shiptocode), statedict As Dictionary(Of String, String), aa As ArrayList)
        Dim a As ArrayList
        
        ' Add state header
        a = New ArrayList()
        a.Add("")
        a.Add(stateName)
        For i As Integer = 2 To 14
            a.Add("")
        Next
        aa.Add(a)
        
        Dim ts11, ts12, ts1, ts21, ts22, ts2, ts31, ts32, ts3, gtotal As Integer
        ts11 = 0 : ts12 = 0 : ts1 = 0 : ts21 = 0 : ts22 = 0 : ts2 = 0
        ts31 = 0 : ts32 = 0 : ts3 = 0 : gtotal = 0
        
        For Each data1 In datastruct
            If statedict.ContainsKey(data1.Key) Then
                Dim infostr As String = statedict(data1.Key)
                Dim infos() As String = infostr.Split(";"c)
                
                If infos(0) = stateName Then
                    ' Add weightout data
                    a = New ArrayList()
                    a.Add("")
                    If infos.Length >= 2 Then
                        a.Add(SecurityHelper.SanitizeForHtml(infos(1)))
                        a.Add(SecurityHelper.SanitizeForHtml(infos(2)))
                    End If
                    a.Add(data1.Key)
                    a.Add("Weightout")
                    a.Add(data1.Value.s11)
                    a.Add(data1.Value.s12)
                    a.Add(data1.Value.s11 + data1.Value.s12)
                    a.Add(data1.Value.s21)
                    a.Add(data1.Value.s22)
                    a.Add(data1.Value.s21 + data1.Value.s22)
                    a.Add(data1.Value.s31)
                    a.Add(data1.Value.s32)
                    a.Add(data1.Value.s31 + data1.Value.s32)
                    a.Add(data1.Value.s11 + data1.Value.s12 + data1.Value.s21 + data1.Value.s22 + data1.Value.s31 + data1.Value.s32)
                    
                    ' Update totals
                    ts11 += data1.Value.s11
                    ts12 += data1.Value.s12
                    ts1 += data1.Value.s11 + data1.Value.s12
                    ts21 += data1.Value.s21
                    ts22 += data1.Value.s22
                    ts2 += data1.Value.s21 + data1.Value.s22
                    ts31 += data1.Value.s31
                    ts32 += data1.Value.s32
                    ts3 += data1.Value.s31 + data1.Value.s32
                    gtotal += data1.Value.s11 + data1.Value.s12 + data1.Value.s21 + data1.Value.s22 + data1.Value.s31 + data1.Value.s32
                    
                    aa.Add(a)
                    
                    ' Add ATA trips data
                    a = New ArrayList()
                    a.Add("")
                    a.Add("")
                    a.Add("")
                    a.Add("")
                    a.Add("ATA Trips")
                    a.Add(data1.Value.a11)
                    a.Add(data1.Value.a12)
                    a.Add(data1.Value.a11 + data1.Value.a12)
                    a.Add(data1.Value.a21)
                    a.Add(data1.Value.a22)
                    a.Add(data1.Value.a21 + data1.Value.a22)
                    a.Add(data1.Value.a31)
                    a.Add(data1.Value.a32)
                    a.Add(data1.Value.a31 + data1.Value.a32)
                    a.Add(data1.Value.a11 + data1.Value.a12 + data1.Value.a21 + data1.Value.a22 + data1.Value.a31 + data1.Value.a32)
                    
                    aa.Add(a)
                End If
            End If
        Next
        
        ' Add total row
        a = New ArrayList()
        a.Add("")
        a.Add("")
        a.Add("")
        a.Add("")
        a.Add("Total")
        a.Add(ts11)
        a.Add(ts12)
        a.Add(ts1)
        a.Add(ts21)
        a.Add(ts22)
        a.Add(ts2)
        a.Add(ts31)
        a.Add(ts32)
        a.Add(ts3)
        a.Add(gtotal)
        aa.Add(a)
    End Sub

    Structure shiptocode
        Dim shiptoid As String
        Dim area As String
        Dim location As String
        Dim s11 As Int32
        Dim s12 As Int32
        Dim total1 As Int32
        Dim s21 As Int32
        Dim s22 As Int32
        Dim total2 As Int32
        Dim s31 As Int32
        Dim s32 As Int32
        Dim total3 As Int32
        Dim a11 As Int32
        Dim a12 As Int32
        Dim totalata1 As Int32
        Dim a21 As Int32
        Dim a22 As Int32
        Dim totalata2 As Int32
        Dim a31 As Int32
        Dim a32 As Int32
        Dim totalata3 As Int32
    End Structure

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function GetUsers(ByVal type As String) As String
        Try
            ' Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Return "{""error"":""Unauthorized""}"
            End If
            
            ' Validate input
            If String.IsNullOrEmpty(type) OrElse Not SecurityHelper.ValidateInput(type, "alphanumeric") Then
                SecurityHelper.LogSecurityEvent("INVALID_TYPE_INPUT", "Invalid type parameter: " & type)
                Return "{""error"":""Invalid type""}"
            End If
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim aa As New ArrayList()
                Dim query As String = ""
                
                Select Case type
                    Case "ALL"
                        query = "SELECT userid, username FROM usertbl WHERE role='User' ORDER BY username"
                    Case "1"
                        query = "SELECT userid, username FROM YTLDB.dbo.userTBL WHERE userid NOT IN (7144,7145,7146,7147,7148,7099,7180) AND companyname LIKE 'ytl%' AND role = 'User' ORDER BY username"
                    Case "2"
                        query = "SELECT userid, username FROM userTBL WHERE companyname NOT LIKE 'ytl%' AND role = 'User' ORDER BY username"
                    Case Else
                        Return "{""error"":""Invalid type""}"
                End Select
                
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim a As New ArrayList()
                            a.Add(dr("userid"))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("username").ToString()))
                            aa.Add(a)
                        End While
                    End Using
                End Using
                
                Dim json As String = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
                Return json
            End Using
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GETUSERS_ERROR", "Error in GetUsers: " & ex.Message)
            Return "{""error"":""An error occurred""}"
        End Try
    End Function
    
    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    Public Shared Function GetRecentRemarks(ByVal plateno As String) As String
        Try
            ' Validate authentication
            If Not AuthenticationHelper.IsUserAuthenticated() Then
                Return "{""error"":""Unauthorized""}"
            End If
            
            ' Validate input
            If Not SecurityHelper.ValidateInput(plateno, "plateno") Then
                SecurityHelper.LogSecurityEvent("INVALID_PLATENO_INPUT", "Invalid plate number: " & plateno)
                Return "{""error"":""Invalid plate number""}"
            End If
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim aa As New ArrayList()
                
                Dim query As String = "SELECT TOP 5 timestamp, sourcename, officeremark FROM maintenance WHERE plateno = @plateno AND status = 'OSS' ORDER BY timestamp DESC"
                Using cmd As SqlCommand = SecurityHelper.CreateSafeCommand(query, conn)
                    cmd.Parameters.AddWithValue("@plateno", plateno)
                    conn.Open()
                    
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        Dim counter As Integer = 1
                        While dr.Read()
                            Dim a As New ArrayList()
                            a.Add(counter)
                            a.Add(Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd HH:mm:ss"))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("sourcename").ToString()))
                            a.Add(SecurityHelper.SanitizeForHtml(dr("officeremark").ToString()))
                            aa.Add(a)
                            counter += 1
                        End While
                    End Using
                End Using
                
                Dim json As String = "{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}"
                Return json
            End Using
            
        Catch ex As Exception
            SecurityHelper.LogSecurityEvent("GETREMARKS_ERROR", "Error in GetRecentRemarks: " & ex.Message)
            Return "{""error"":""An error occurred""}"
        End Try
    End Function

End Class