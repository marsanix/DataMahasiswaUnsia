Imports System.Data.SqlClient
Imports Microsoft.Data.SqlClient
Imports MySql.Data.MySqlClient
Imports Org.BouncyCastle.Asn1.X500
Imports Org.BouncyCastle.Pqc.Crypto.Cmce
Imports System.IO


' Database:
' CREATE DATABASE datamhsunsia;
' USE datamhsunsia;
' 
' CREATE TABLE `mahasiswa` (
'  `id` int NOT NULL AUTO_INCREMENT,
'  `nim` varchar(12) NOT NULL,
'  `nama` varchar(100) NOT NULL,
'  `jenis_kelamin` char(1) NOT NULL,
'  `hp` varchar(30) NOT NULL,
'  `email` varchar(50) NOT NULL,
'  `alamat` text,
'  `prodi` varchar(20) NOT NULL,
'  `foto` text,
'  PRIMARY KEY (`id`),
'  UNIQUE KEY `mahasiswa_nim_IDX` (`nim`) USING BTREE
') ;

Public Class Form1

    Dim Cmd As MySqlCommand
    Dim Da As MySqlDataAdapter
    Dim Rd As MySqlDataReader
    Dim Ds As DataSet

    Dim DB As New DbMahasiswa()


    Sub InsertData()

        Dim jenisKelamin As String = If(ComboBox1.Text = "Laki-laki", "L", "P")
        Dim filename As String = Path.GetFileName(Label9.Text)

        Try
            Call DB.ConnectDB()
            DB.Conn.Open()
            Dim sql As String = "INSERT INTO mahasiswa
                                (id, nim, nama, jenis_kelamin, hp, email, alamat, prodi, foto)
                                VALUES(0, '" & TextBox1.Text & "', '" & TextBox2.Text & "', '" & jenisKelamin & "', '" & TextBox3.Text & "', '" & TextBox4.Text & "', '" & TextBox5.Text & "', '" & ComboBox2.Text & "', '" & filename & "');"
            Cmd = New MySqlCommand(sql, DB.Conn)
            Cmd.ExecuteNonQuery()

            Call SaveFoto()

            MsgBox("Data mahasiswa berhasil disimpan")
            Call KosongkanForm()
            Call LoadData()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            DB.Conn.Close()
        End Try


    End Sub

    Sub UpdateData()

        If DataGridView1.SelectedRows.Count > 0 Then
            ' Tampilkan dialog konfirmasi
            Dim result As DialogResult = MessageBox.Show("Anda yakin akan menyimpan perubahan data ini?", "Konfirmasi Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            ' Jika pengguna memilih Yes, hapus baris
            If result = DialogResult.Yes Then

                Dim jenisKelamin As String = If(ComboBox1.Text = "Laki-laki", "L", "P")
                Dim filename As String = Path.GetFileName(Label9.Text)


                ' Ambil ID atau kunci utama dari baris yang dipilih
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                Dim id As Integer = Convert.ToInt32(selectedRow.Cells("id").Value)
                Dim filenameFromDb = selectedRow.Cells("foto").Value.ToString

                Dim ubahFoto As String = If(filename = filenameFromDb, "", ", foto='" & filename & "'")

                Try

                    Call DB.ConnectDB()
                    DB.Conn.Open()
                    Dim sql As String = "UPDATE mahasiswa
                                    SET nama='" & TextBox2.Text & "', 
                                    jenis_kelamin='" & jenisKelamin & "', 
                                    hp='" & TextBox3.Text & "', 
                                    email='" & TextBox4.Text & "', 
                                    alamat='" & TextBox5.Text & "', 
                                    prodi='" & ComboBox2.Text & "'
                                    " & ubahFoto & "
                                    WHERE id = @id"

                    Cmd = New MySqlCommand(sql, DB.Conn)
                    Cmd.Parameters.AddWithValue("@id", id)
                    Cmd.ExecuteNonQuery()

                    Call SaveFoto()


                Catch ex As Exception
                    'MessageBox.Show("Error: " & ex.Message)
                Finally
                    DB.Conn.Close()
                End Try

                Call LoadData()
                MessageBox.Show("Data mahasiswa yang dipilih berhasil diubah.")
            End If
        Else
            MessageBox.Show("Tidak ada baris yang dipilih untuk diubah")
        End If


    End Sub

    Sub LoadData()

        Call DB.ConnectDB()
        Da = New MySqlDataAdapter("SELECT id, nim, nama, jenis_kelamin, hp, email, prodi, alamat, foto FROM mahasiswa ORDER BY id DESC", DB.Conn)
        Ds = New DataSet
        Ds.Clear()
        Da.Fill(Ds, "mahasiswa")
        DataGridView1.DataSource = (Ds.Tables("mahasiswa"))

        DataGridView1.Columns(0).HeaderText = "ID"
        DataGridView1.Columns(0).Width = 50
        DataGridView1.Columns(1).HeaderText = "NIM"
        DataGridView1.Columns(2).HeaderText = "NAMA"
        DataGridView1.Columns(2).Width = 150
        DataGridView1.Columns(3).HeaderText = "JENIS KELAMIN"
        DataGridView1.Columns(3).Width = 130
        DataGridView1.Columns(4).HeaderText = "HP"
        DataGridView1.Columns(5).HeaderText = "EMAIL"
        DataGridView1.Columns(5).Width = 120
        DataGridView1.Columns(6).HeaderText = "PRODI"
        DataGridView1.Columns(6).Width = 120
        DataGridView1.Columns(7).Visible = False
        DataGridView1.Columns(8).Visible = False


        For Each row As DataGridViewRow In DataGridView1.Rows
            If row.Cells("jenis_kelamin").Value = "L" Then
                row.Cells("jenis_kelamin").Value = "Laki-laki"
            Else
                If row.Cells("jenis_kelamin").Value IsNot Nothing Then
                    row.Cells("jenis_kelamin").Value = "Perempuan"
                End If
            End If
        Next


    End Sub

    Sub HapusData()

        If DataGridView1.SelectedRows.Count > 0 Then
            ' Tampilkan dialog konfirmasi
            Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus data ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            ' Jika pengguna memilih Yes, hapus baris
            If result = DialogResult.Yes Then

                Try
                    DB.Conn.Open()

                    ' Ambil ID atau kunci utama dari baris yang dipilih
                    Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
                    Dim id As Integer = Convert.ToInt32(selectedRow.Cells("id").Value)

                    ' Perintah SQL untuk menghapus data
                    Dim sql As String = "DELETE FROM mahasiswa WHERE id = @id"
                    Dim cmd As New MySqlCommand(sql, DB.Conn)
                    cmd.Parameters.AddWithValue("@id", id)

                    ' Jalankan perintah SQL
                    cmd.ExecuteNonQuery()

                    Call LoadData()

                Catch ex As Exception
                    MessageBox.Show("Error: " & ex.Message)
                Finally
                    DB.Conn.Close()
                End Try

                MessageBox.Show("Data mahasiswa yang dipilih berhasil dihapus.")
            End If
        Else
            MessageBox.Show("Tidak ada baris yang dipilih untuk dihapus")
        End If


    End Sub

    Sub KosongkanForm()
        TextBox1.Text = ""
        TextBox2.Text = ""
        TextBox3.Text = ""
        TextBox4.Text = ""
        TextBox5.Text = ""
        ComboBox1.Text = ""
        ComboBox2.Text = ""
        Label9.Text = "Nama file:"
        PictureBox2.Image = Nothing
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Call LoadData()
        Call KondisiAwal()

        If System.IO.File.Exists("Logs.txt") Then
            Label11.Text = System.IO.File.ReadAllText("Logs.txt")

        End If


    End Sub

    Sub KondisiAwal()
        Button2.Text = "Input"
        Button3.Enabled = False
        Button4.Enabled = False
        Button5.Enabled = False

        TextBox1.Enabled = False
        TextBox2.Enabled = False
        TextBox3.Enabled = False
        TextBox4.Enabled = False
        TextBox5.Enabled = False
        ComboBox1.Enabled = False
        ComboBox2.Enabled = False
        Button1.Enabled = False

    End Sub

    Sub BukaForm()


        TextBox1.Enabled = True
        TextBox2.Enabled = True
        TextBox3.Enabled = True
        TextBox4.Enabled = True
        TextBox5.Enabled = True
        ComboBox1.Enabled = True
        ComboBox2.Enabled = True
        Button1.Enabled = True

    End Sub

    Sub KondisiDataSudahTampil()
        Button2.Text = "Input"

        Button3.Enabled = True
        Button4.Enabled = True

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        OpenFileDialog1.Filter = "*.jpg|"
        OpenFileDialog1.ShowDialog()

        Label9.Text = OpenFileDialog1.FileName
        PictureBox2.ImageLocation = Label9.Text
        PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage


    End Sub

    Private Sub SaveFoto()


        ' Get the selected file name
        Dim selectedFile As String = OpenFileDialog1.FileName

        ' Define the destination directory
        Dim destinationDirectory As String = "C:\PhotoMahasiswaUNSIA\"

        ' Ensure the destination directory exists
        If Not Directory.Exists(destinationDirectory) Then
            Directory.CreateDirectory(destinationDirectory)
        End If

        ' Define the destination file path
        Dim destinationFilePath As String = Path.Combine(destinationDirectory, Path.GetFileName(selectedFile))

        ' Copy the file to the destination directory
        File.Copy(selectedFile, destinationFilePath, True)


    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        If Button2.Text = "Input" Then
            Button2.Text = "Simpan"
            Button5.Enabled = True
            Button3.Enabled = False
            Button4.Enabled = False

            Call KosongkanForm()
            Call BukaForm()

        Else

            If TextBox1.Text = "" Or
               TextBox2.Text = "" Or
               TextBox3.Text = "" Or
               TextBox4.Text = "" Or
               ComboBox1.Text = "" Or
               ComboBox2.Text = "" Then

                MsgBox("Data NIM, Nama, HP/WA, Email tidak boleh kosong!")

            Else

                InsertData()

            End If

        End If

    End Sub


    Private Sub TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress
        If e.KeyChar = Chr(13) Then
            Call DB.ConnectDB()
            DB.Conn.Open()

            Cmd = New MySqlCommand("SELECT id, nim, nama, jenis_kelamin, hp, email, alamat, prodi, foto  FROM mahasiswa WHERE nim='" & TextBox1.Text & "'", DB.Conn)
            Rd = Cmd.ExecuteReader
            Rd.Read()
            If Rd.HasRows Then

                Call KondisiDataSudahTampil()

                TextBox2.Text = Rd.Item("nama")
                ComboBox1.Text = If(Rd.Item("jenis_kelamin") = "L", "Laki-laki", "Perempuan")
                TextBox3.Text = Rd.Item("hp")
                TextBox4.Text = Rd.Item("email")
                TextBox5.Text = Rd.Item("alamat")
                ComboBox2.Text = Rd.Item("prodi")

                Label9.Text = Rd.Item("foto")
                Dim imageName As String = Rd.Item("foto")
                Dim imagePath As String = Path.Combine("C:\PhotoMahasiswaUNSIA\", imageName)

                ' Check if file exists before loading
                If File.Exists(imagePath) Then
                    PictureBox2.Image = Image.FromFile(imagePath)
                    PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage
                Else
                    MessageBox.Show("File foto tidak ditemukan: " & imagePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

            Else
                MsgBox("Data tidak ditemukan")
            End If

            DB.Conn.Close()


        End If
    End Sub


    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            ' Get the selected row
            Dim selectedRow As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

            ' Retrieve data from specific columns
            Dim nim As String = selectedRow.Cells("nim").Value.ToString() ' Change "Column1" to your column name
            Dim nama As String = selectedRow.Cells("nama").Value.ToString()
            Dim jenis_kelamin As String = selectedRow.Cells("jenis_kelamin").Value.ToString()
            Dim hp As String = selectedRow.Cells("hp").Value.ToString()
            Dim email As String = selectedRow.Cells("email").Value.ToString()
            Dim alamat As String = selectedRow.Cells("alamat").Value.ToString()
            Dim prodi As String = selectedRow.Cells("prodi").Value.ToString()
            Dim foto As String = selectedRow.Cells("foto").Value.ToString()

            ' Display data (example: show in textboxes)
            TextBox1.Text = nim
            TextBox2.Text = nama
            ComboBox1.Text = If(jenis_kelamin = "L", "Laki-laki", "Perempuan")
            TextBox3.Text = hp
            TextBox4.Text = email
            TextBox5.Text = alamat
            ComboBox2.Text = prodi
            Label9.Text = foto

            Dim imageName As String = foto
            Dim imagePath As String = Path.Combine("C:\PhotoMahasiswaUNSIA\", imageName)

            ' Check if file exists before loading
            If File.Exists(imagePath) Then
                PictureBox2.Image = Image.FromFile(imagePath)
                PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage
            Else
                PictureBox2.Image = Nothing
                'MessageBox.Show("File foto tidak ditemukan: " & imagePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            Call KondisiDataSudahTampil()
            Call BukaForm()

        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Application.Exit()
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Call KondisiAwal()
        Call KosongkanForm()

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Call HapusData()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Call UpdateData()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Dim currentDateTime As DateTime = DateTime.Now
        System.IO.File.WriteAllText("Logs.txt", currentDateTime)
    End Sub
End Class
