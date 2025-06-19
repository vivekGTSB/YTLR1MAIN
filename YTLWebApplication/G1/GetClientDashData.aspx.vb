Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Collections.Generic
Partial Class GetClientDashData
    Inherits System.Web.UI.Page
    Public collected_Names As String = ""
    Public json As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim paramValue As String = Request.QueryString("p")
        Server.ScriptTimeout = Integer.MaxValue - 1
         GetTop5Data(paramValue)
       
    End Sub

    Private Sub GetTop5Data(ByVal ParamValue As Int32 )
        Try
            ' 1--> Transporter Happy 
            '2 --> Plant happy
            '3 --> Customer happy
            ' 4--> Transporter Passive 
            '5 --> Plant Passive
            '6 --> Customer Passive
            ' 7--> Transporter Unhappy
            '8 --> Plant Unhappy
            '9 --> Customer unhappy
            collected_Names = "["
            Dim mysqlstring As String = "select  top 1 SourceSupply,sum(csd) csd, sum(cq) cq , sum(dt) dt , sum(db) db, sum(csd)+sum(t.cq)+sum(dt)+sum(db) as tot  from (select  dbo.fn_getTransporterNameFromPlateno(0) as SourceSupply ,case when CSD=2 then 1 else 0 end csd  ,case when cq=2 then 1 else 0 end cq,case when dt=2 then 1 else 0 end dt ,case when db=2 then 1 else 0 end db   from Feedback) t  group by SourceSupply order by tot desc "
            Select Case ParamValue
                Case 1
                    mysqlstring = "select  top 5 SourceSupply,sum(csd) csd, sum(cq) cq , sum(dt) dt , sum(db) db, sum(csd)+sum(t.cq)+sum(dt)+sum(db) as tot  from (select  dbo.fn_getTransporterNameFromPlateno(plateno) as SourceSupply ,case when CSD=2 then 1 else 0 end csd  ,0 as  cq,case when dt=2 then 1 else 0 end dt ,case when db=2 then 1 else 0 end db   from Feedback) t  group by SourceSupply order by tot desc "
                Case 2
                    mysqlstring = "select  top 5 SourceSupply,sum(csd) csd, sum(cq) cq , sum(dt) dt , sum(db) db, sum(csd)+sum(t.cq)+sum(dt)+sum(db) as tot  from (select  SourceSupply ,0 as csd  ,0 as dt ,0 as db  ,case when cq=2 then 1 else 0 end cq   from Feedback) t  group by SourceSupply order by tot desc "
                Case 3
                    mysqlstring = "select  top 5 SourceSupply,sum(csd) csd, sum(cq) cq , sum(dt) dt , sum(db) db, sum(csd)+sum(t.cq)+sum(dt)+sum(db) as tot  from (select  dbo.fn_GetClientName(TUserId) as SourceSupply ,case when CSD=2 then 1 else 0 end csd  ,case when cq=2 then 1 else 0 end cq ,case when dt=2 then 1 else 0 end dt ,case when db=2 then 1 else 0 end db   from Feedback) t  group by SourceSupply order by tot desc "
                Case 4
                    mysqlstring = "select  top 5 SourceSupply,sum(csd) csd, sum(cq) cq , sum(dt) dt , sum(db) db, sum(csd)+sum(t.cq)+sum(dt)+sum(db) as tot  from (select  dbo.fn_getTransporterNameFromPlateno(plateno) as SourceSupply ,case when CSD=1 then 1 else 0 end csd  ,0 as  cq,case when dt=1 then 1 else 0 end dt ,case when db=1 then 1 else 0 end db   from Feedback) t  group by SourceSupply order by tot desc "
                Case 5
                    mysqlstring = "select  top 5 SourceSupply,sum(csd) csd, sum(cq) cq , sum(dt) dt , sum(db) db, sum(csd)+sum(t.cq)+sum(dt)+sum(db) as tot  from (select  SourceSupply ,0 as csd  ,0 as dt ,0 as db  ,case when cq=1 then 1 else 0 end cq   from Feedback) t  group by SourceSupply order by tot desc "
                Case 6
                    mysqlstring = "select  top 5 SourceSupply,sum(csd) csd, sum(cq) cq , sum(dt) dt , sum(db) db, sum(csd)+sum(t.cq)+sum(dt)+sum(db) as tot  from (select  dbo.fn_GetClientName(TUserId) as SourceSupply ,case when CSD=1 then 1 else 0 end csd  ,case when cq=1 then 1 else 0 end cq ,case when dt=1 then 1 else 0 end dt ,case when db=1 then 1 else 0 end db   from Feedback) t  group by SourceSupply order by tot desc "
                Case 7
                    mysqlstring = "select  top 5 SourceSupply,sum(csd) csd, sum(cq) cq , sum(dt) dt , sum(db) db, sum(csd)+sum(t.cq)+sum(dt)+sum(db) as tot  from (select  dbo.fn_getTransporterNameFromPlateno(plateno) as SourceSupply ,case when CSD=0 then 1 else 0 end csd  ,0 as  cq,case when dt=0 then 1 else 0 end dt ,case when db=0 then 1 else 0 end db   from Feedback) t  group by SourceSupply order by tot desc "
                Case 8
                    mysqlstring = "select  top 5 SourceSupply,sum(csd) csd, sum(cq) cq , sum(dt) dt , sum(db) db, sum(csd)+sum(t.cq)+sum(dt)+sum(db) as tot  from (select  SourceSupply ,0 as csd  ,0 as dt ,0 as db  ,case when cq=0 then 1 else 0 end cq   from Feedback) t  group by SourceSupply order by tot desc "
                Case 9
                    mysqlstring = "select  top 5 SourceSupply,sum(csd) csd, sum(cq) cq , sum(dt) dt , sum(db) db, sum(csd)+sum(t.cq)+sum(dt)+sum(db) as tot  from (select  dbo.fn_GetClientName(TUserId) as SourceSupply ,case when CSD=0 then 1 else 0 end csd  ,case when cq=0 then 1 else 0 end cq ,case when dt=0 then 1 else 0 end dt ,case when db=0 then 1 else 0 end db   from Feedback) t  group by SourceSupply order by tot desc "
                Case Else
                    mysqlstring = "select   '' as SourceSupply,sum(csd) csd, sum(cq) cq , sum(dt) dt , sum(db) db, sum(csd)+sum(t.cq)+sum(dt)+sum(db) as tot  from (select case when CSD=2 then 1 else 0 end csd  ,case when cq=2 then 1 else 0 end cq,case when dt=2 then 1 else 0 end dt ,case when db=2 then 1 else 0 end db   from Feedback) t  order by tot desc"
            End Select


            Dim connJob As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Try
                Dim cmd2 As New SqlCommand(mysqlstring, connJob)
                Try
                    connJob.Open()
                    Dim drJob As SqlDataReader = cmd2.ExecuteReader()
                    While drJob.Read()
                        collected_Names += "{""" & "name"":""" & drJob("SourceSupply").ToString.ToUpper() & """,""csd"":""" & drJob("csd").ToString() & """,""cq"":""" & drJob("cq").ToString() & """,""dt"":""" & drJob("dt").ToString() & """,""db"":""" & drJob("db").ToString() & """},"
                     End While
                Catch ex As Exception
                    Response.Write(ex.message)
                Finally
                    connJob.Close()
                End Try
            Catch ex As Exception
                ' collected_Names += "1" & ex.Message
            End Try
        Catch ex As Exception
            'collected_Names += "2" & ex.Message
        End Try
        If (collected_Names.Length > 1) Then
            collected_Names = collected_Names.Remove(collected_Names.Length - 1, 1)
        End If

        collected_Names += "]"
        Response.Write(collected_Names)
    End Sub
    

    
End Class
