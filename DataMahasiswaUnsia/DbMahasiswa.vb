Imports MySql.Data.MySqlClient

Public Class DbMahasiswa

    Public Conn As MySqlConnection
    Dim ConnStr As String

    Private Const DB_HOST = "localhost"
    Private Const DB_NAME = "datamhsunsia"
    Private Const DB_USER = "root"
    Private Const DB_PASS = "toor"
    Private Const DB_PORT = "3306"

    Public Sub ConnectDB()
        ConnStr = "server=" & DB_HOST & ";user=" & DB_USER & ";database=" & DB_NAME & ";port=" & DB_PORT & ";password=" & DB_PASS & ";"
        Conn = New MySqlConnection(ConnStr)

        Try
            Conn.Open()
            ' Your code to interact with the database goes here
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            Conn.Close()
        End Try


    End Sub

End Class
