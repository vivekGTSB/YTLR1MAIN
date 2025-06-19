Imports System.Data.SqlClient

Public Class DatabaseHelper
    
    ' Execute parameterized query safely
    Public Shared Function ExecuteQuery(query As String, parameters As Dictionary(Of String, Object)) As DataTable
        Dim dt As New DataTable()
        
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Using cmd As New SqlCommand(query, conn)
                    ' Add parameters
                    If parameters IsNot Nothing Then
                        For Each param In parameters
                            cmd.Parameters.AddWithValue(param.Key, If(param.Value, DBNull.Value))
                        Next
                    End If
                    
                    Using adapter As New SqlDataAdapter(cmd)
                        adapter.Fill(dt)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Database error: " & ex.Message)
            Throw New ApplicationException("Database operation failed", ex)
        End Try
        
        Return dt
    End Function
    
    ' Execute non-query with parameters
    Public Shared Function ExecuteNonQuery(query As String, parameters As Dictionary(Of String, Object)) As Integer
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Using cmd As New SqlCommand(query, conn)
                    ' Add parameters
                    If parameters IsNot Nothing Then
                        For Each param In parameters
                            cmd.Parameters.AddWithValue(param.Key, If(param.Value, DBNull.Value))
                        Next
                    End If
                    
                    conn.Open()
                    Return cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Database error: " & ex.Message)
            Throw New ApplicationException("Database operation failed", ex)
        End Try
    End Function
    
    ' Execute scalar query with parameters
    Public Shared Function ExecuteScalar(query As String, parameters As Dictionary(Of String, Object)) As Object
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Using cmd As New SqlCommand(query, conn)
                    ' Add parameters
                    If parameters IsNot Nothing Then
                        For Each param In parameters
                            cmd.Parameters.AddWithValue(param.Key, If(param.Value, DBNull.Value))
                        Next
                    End If
                    
                    conn.Open()
                    Return cmd.ExecuteScalar()
                End Using
            End Using
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Database error: " & ex.Message)
            Throw New ApplicationException("Database operation failed", ex)
        End Try
    End Function
    
    ' Get user vehicles safely
    Public Shared Function GetUserVehicles(userInfo As UserInfo) As DataTable
        Dim query As String
        Dim parameters As New Dictionary(Of String, Object)
        
        If userInfo.Role = "User" Then
            query = "SELECT * FROM vehicleTBL WHERE userid = @userid ORDER BY plateno"
            parameters.Add("@userid", userInfo.UserId)
        ElseIf userInfo.Role = "SuperUser" OrElse userInfo.Role = "Operator" Then
            If SecurityHelper.IsValidUsersList(userInfo.UsersList) Then
                query = "SELECT * FROM vehicleTBL WHERE userid IN (" & userInfo.UsersList & ") ORDER BY plateno"
            Else
                query = "SELECT * FROM vehicleTBL WHERE 1=0" ' Return empty result for invalid users list
            End If
        Else
            query = "SELECT * FROM vehicleTBL ORDER BY plateno"
        End If
        
        Return ExecuteQuery(query, parameters)
    End Function
    
    ' Get users safely
    Public Shared Function GetUsers(userInfo As UserInfo) As DataTable
        Dim query As String
        Dim parameters As New Dictionary(Of String, Object)
        
        If userInfo.Role = "User" Then
            query = "SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username"
            parameters.Add("@userid", userInfo.UserId)
        ElseIf userInfo.Role = "SuperUser" OrElse userInfo.Role = "Operator" Then
            If SecurityHelper.IsValidUsersList(userInfo.UsersList) Then
                query = "SELECT userid, username FROM userTBL WHERE userid IN (" & userInfo.UsersList & ") ORDER BY username"
            Else
                query = "SELECT userid, username FROM userTBL WHERE 1=0" ' Return empty result
            End If
        Else
            query = "SELECT userid, username FROM userTBL WHERE role='User' ORDER BY username"
        End If
        
        Return ExecuteQuery(query, parameters)
    End Function
    
    ' Get geofences safely
    Public Shared Function GetGeofences(userId As Integer) As DataTable
        Dim query As String = "SELECT DISTINCT(shiptocode), geofencename, ISNULL(t2.geofenceid, 0) AS count " &
                             "FROM geofence t1 " &
                             "LEFT OUTER JOIN (SELECT * FROM user_geofence_favorite WHERE userid = @userid) AS t2 " &
                             "ON t1.geofenceid = t2.geofenceid " &
                             "WHERE accesstype='1' AND shiptocode <> '0' " &
                             "ORDER BY count DESC"
        
        Dim parameters As New Dictionary(Of String, Object)
        parameters.Add("@userid", userId)
        
        Return ExecuteQuery(query, parameters)
    End Function
    
    ' Update document data safely
    Public Shared Function UpdateDocumentData(plateno As String, userId As Integer, roadTax As DateTime?, puspakom As DateTime?, insurance As DateTime?, permit As DateTime?, otherDoc As DateTime?, email1 As String, email2 As String, remarks As String) As Boolean
        Try
            ' Build dynamic update query
            Dim updateFields As New List(Of String)
            Dim parameters As New Dictionary(Of String, Object)
            
            updateFields.Add("pma = @pma")
            updateFields.Add("btm = '0'")
            updateFields.Add("bdm = '0'")
            updateFields.Add("emailid = @emailid")
            updateFields.Add("emailid2 = @emailid2")
            updateFields.Add("updateddatetime = @updatetime")
            updateFields.Add("remarks = @remarks")
            
            parameters.Add("@pma", DateTime.Now)
            parameters.Add("@emailid", email1)
            parameters.Add("@emailid2", email2)
            parameters.Add("@updatetime", DateTime.Now)
            parameters.Add("@remarks", remarks)
            parameters.Add("@plateno", plateno)
            
            If roadTax.HasValue Then
                updateFields.Add("roadtax = @roadtax")
                parameters.Add("@roadtax", roadTax.Value)
            End If
            
            If puspakom.HasValue Then
                updateFields.Add("puspakomtest = @puspakomtest")
                parameters.Add("@puspakomtest", puspakom.Value)
            End If
            
            If insurance.HasValue Then
                updateFields.Add("insurance = @insurance")
                parameters.Add("@insurance", insurance.Value)
            End If
            
            If permit.HasValue Then
                updateFields.Add("permitexpiry = @permitexpiry")
                parameters.Add("@permitexpiry", permit.Value)
            End If
            
            If otherDoc.HasValue Then
                updateFields.Add("otherdocexpiry = @otherdocexpiry")
                parameters.Add("@otherdocexpiry", otherDoc.Value)
            End If
            
            Dim updateQuery As String = "UPDATE documents_date SET " & String.Join(", ", updateFields) & " WHERE plateno = @plateno"
            
            Dim rowsAffected As Integer = ExecuteNonQuery(updateQuery, parameters)
            
            If rowsAffected = 0 Then
                ' Insert new record if update affected 0 rows
                Dim insertFields As New List(Of String) From {"plateno", "userid", "pma", "btm", "bdm", "emailid", "updateddatetime", "emailid2", "remarks"}
                Dim insertValues As New List(Of String) From {"@plateno", "@userid", "@pma", "'0'", "'0'", "@emailid", "@updatetime", "@emailid2", "@remarks"}
                
                parameters.Add("@userid", userId)
                
                If roadTax.HasValue Then
                    insertFields.Add("roadtax")
                    insertValues.Add("@roadtax")
                End If
                
                If puspakom.HasValue Then
                    insertFields.Add("puspakomtest")
                    insertValues.Add("@puspakomtest")
                End If
                
                If insurance.HasValue Then
                    insertFields.Add("insurance")
                    insertValues.Add("@insurance")
                End If
                
                If permit.HasValue Then
                    insertFields.Add("permitexpiry")
                    insertValues.Add("@permitexpiry")
                End If
                
                If otherDoc.HasValue Then
                    insertFields.Add("otherdocexpiry")
                    insertValues.Add("@otherdocexpiry")
                End If
                
                Dim insertQuery As String = "INSERT INTO documents_date (" & String.Join(", ", insertFields) & ") VALUES (" & String.Join(", ", insertValues) & ")"
                
                rowsAffected = ExecuteNonQuery(insertQuery, parameters)
            End If
            
            Return rowsAffected > 0
            
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error updating document data: " & ex.Message)
            Return False
        End Try
    End Function
    
End Class