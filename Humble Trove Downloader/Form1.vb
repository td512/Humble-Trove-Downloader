Imports System.IO
Imports System.Net
Imports System.Threading
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class Form1

    Public baseuri = "https://www.humblebundle.com/api/v1/trove/chunk?index="
    Public json
    Public counter = 1
    Public chunked = False
    Public download = ""
    Public download_type = ""
    Public session_key = ""
    Public save_to = ""
    Public win_downloads = New List(Of String)
    Public mac_downloads = New List(Of String)
    Public lin_downloads = New List(Of String)
    Public win_links = New Integer
    Public mac_links = New Integer
    Public lin_links = New Integer
    Public all_links = New Integer
    Public downloaded = New Integer
    Public downloads_to_complete = New Integer
    Public wc As New WebClient

    Private Sub Form1_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        If Me.Visible Then
            RichTextBox1.AppendText("Fetching Humble Trove Items")

            Do
                Threading.Thread.Sleep(500)

                If chunked = True Then
                    Exit Do
                End If

                Dim thread As New Thread(
                    Sub()
                        Dim client As New WebClient
                        Dim result As String = client.DownloadString(New Uri("https://www.humblebundle.com/api/v1/trove/chunk?index=" + counter.ToString))
                        client.Dispose()
                        If result = "[]" Then
                            chunked = True
                        Else
                            If json Is Nothing Then
                                'RichTextBox1.Invoke(Sub() RichTextBox1.Text = result)

                                json = JArray.Parse(result.ToString).ToString
                            Else
                                Dim orig_json = JArray.Parse(json)
                                Dim new_json = JArray.Parse(result)
                                orig_json.Merge(new_json)
                                json = orig_json.ToString
                            End If
                        End If
                    End Sub
                    )
                thread.Start()

                counter += 1
            Loop
            RichTextBox1.AppendText(Environment.NewLine)
            RichTextBox1.AppendText("Fetched Chunks! Parsing...")
            RichTextBox1.AppendText(Environment.NewLine)
            RichTextBox1.AppendText("Everything looks good, setting up UI")
            Label1.Visible = False
            Label2.Visible = True
            RadioButton1.Visible = True
            RadioButton2.Visible = True
            RadioButton3.Visible = True
            RadioButton4.Visible = True
            RadioButton5.Visible = True
            RadioButton6.Visible = True
            TextBox1.Visible = True
            Button1.Visible = True
            Button2.Visible = True
            Label3.Visible = True
            ProgressBar1.Visible = False
            GroupBox1.Visible = True
            GroupBox2.Visible = True
        End If
    End Sub

    Public Sub Invoke(ByVal control As Control, ByVal action As Action)
        If control.InvokeRequired Then
            control.Invoke(New MethodInvoker(Sub() action()), Nothing)
        Else
            action.Invoke()
        End If
    End Sub

    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If String.IsNullOrEmpty(TextBox1.Text) Then
            Panel1.BackColor = Color.Red
        Else
            Label1.Visible = True
            Label2.Visible = False
            RadioButton1.Visible = False
            RadioButton2.Visible = False
            RadioButton3.Visible = False
            RadioButton4.Visible = False
            RadioButton5.Visible = False
            RadioButton6.Visible = False
            Button2.Visible = False
            Label3.Visible = False
            TextBox1.Visible = False
            Button1.Visible = False
            Button2.Visible = False
            ProgressBar1.Visible = True
            GroupBox1.Visible = False
            GroupBox2.Visible = False
            Panel1.BackColor = SystemColors.Control

            If RadioButton1.Checked Then
                download = "Windows"
            ElseIf RadioButton2.Checked Then
                download = "Mac"
            ElseIf RadioButton3.Checked Then
                download = "Linux"
            ElseIf RadioButton4.Checked Then
                download = "All"
            End If

            If RadioButton5.Checked Then
                download_type = "Direct"
            Else
                download_type = "Torrent"
            End If

            session_key = TextBox1.Text

            RichTextBox1.AppendText(Environment.NewLine)
            RichTextBox1.AppendText("Download operation: " + download)
            RichTextBox1.AppendText(Environment.NewLine)
            RichTextBox1.AppendText("Download type: " + download_type)
            RichTextBox1.AppendText(Environment.NewLine)
            RichTextBox1.AppendText("Session key set!")

            Dim trove = JArray.Parse(json)
            For Each Row In trove
                RichTextBox1.AppendText(Environment.NewLine)
                RichTextBox1.AppendText($"Building links for {Row("human-name")}")
                If Row("downloads")("windows") IsNot Nothing Then
                    win_downloads.Add($"https://www.humblebundle.com/api/v1/user/download/sign?machine_name={Row("downloads")("windows")("url")("web")}&filename={Row("downloads")("windows")("machine_name")}")
                End If
                If Row("downloads")("mac") IsNot Nothing Then
                    mac_downloads.Add($"https://www.humblebundle.com/api/v1/user/download/sign?machine_name={Row("downloads")("mac")("url")("web")}&filename={Row("downloads")("mac")("machine_name")}")
                End If
                If Row("downloads")("linux") IsNot Nothing Then
                    lin_downloads.Add($"https://www.humblebundle.com/api/v1/user/download/sign?machine_name={Row("downloads")("linux")("url")("web")}&filename={Row("downloads")("linux")("machine_name")}")
                End If
            Next
            RichTextBox1.AppendText(Environment.NewLine)
            RichTextBox1.AppendText("Links built!")
            win_links = win_downloads.Count
            mac_links = mac_downloads.Count
            lin_links = lin_downloads.Count
            all_links = win_links + mac_links + lin_links
            RichTextBox1.AppendText(Environment.NewLine)
            RichTextBox1.AppendText("Starting downloads (this could take a while)")

            Select Case download
                Case "Windows"
                    downloads_to_complete = win_links
                    RichTextBox1.AppendText(Environment.NewLine)
                    RichTextBox1.AppendText($"There are {win_links} files for me to download")
                    If String.IsNullOrEmpty(save_to) Then
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Creating directory {My.Application.Info.DirectoryPath}\Windows")
                        Directory.CreateDirectory(My.Application.Info.DirectoryPath & "\Windows")
                    Else
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Creating directory {save_to}\Windows")
                        Directory.CreateDirectory(save_to & "\Windows")
                    End If
                    For Each link In win_downloads
                        downloaded += 1
                        Dim qs = New Uri(link).Query
                        Dim qd = Web.HttpUtility.ParseQueryString(qs)
                        Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
                        Dim downloads = JObject.Parse(download_urls)
                        Dim this_dl = ""
                        If download_type = "Direct" Then
                            this_dl = downloads("signed_url")
                        Else
                            this_dl = downloads("signed_torrent_url")
                        End If
                        Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Starting download for {this_filename} (Windows)")
                        AddHandler wc.DownloadProgressChanged, AddressOf DownloadProgressChanged
                        AddHandler wc.DownloadFileCompleted, AddressOf DownloadProgressCompleted
                        If String.IsNullOrEmpty(save_to) Then
                            Await wc.DownloadFileTaskAsync(New Uri(this_dl), My.Application.Info.DirectoryPath & $"\Windows\{this_filename}")
                            wc.Dispose()
                        Else
                            Await wc.DownloadFileTaskAsync(New Uri(this_dl), save_to & $"\Windows\{this_filename}")
                            wc.Dispose()
                        End If
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"I've downloaded {downloaded} / {win_links} files, {win_links - downloaded} files left")
                    Next
                Case "Mac"
                    downloads_to_complete = mac_links
                    RichTextBox1.AppendText(Environment.NewLine)
                    RichTextBox1.AppendText($"There are {mac_links} files for me to download")
                    If String.IsNullOrEmpty(save_to) Then
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Creating directory {My.Application.Info.DirectoryPath}\Mac")
                        Directory.CreateDirectory(My.Application.Info.DirectoryPath & "\Mac")
                    Else
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Creating directory {save_to}\Mac")
                        Directory.CreateDirectory(save_to & "\Mac")
                    End If
                    For Each link In mac_downloads
                        downloaded += 1
                        Dim qs = New Uri(link).Query
                        Dim qd = Web.HttpUtility.ParseQueryString(qs)
                        Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
                        Dim downloads = JObject.Parse(download_urls)
                        Dim this_dl = ""
                        If download_type = "Direct" Then
                            this_dl = downloads("signed_url")
                        Else
                            this_dl = downloads("signed_torrent_url")
                        End If
                        Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Starting download for {this_filename} (Mac)")
                        AddHandler wc.DownloadProgressChanged, AddressOf DownloadProgressChanged
                        AddHandler wc.DownloadFileCompleted, AddressOf DownloadProgressCompleted
                        If String.IsNullOrEmpty(save_to) Then
                            Await wc.DownloadFileTaskAsync(New Uri(this_dl), My.Application.Info.DirectoryPath & $"\Mac\{this_filename}")
                            wc.Dispose()
                        Else
                            Await wc.DownloadFileTaskAsync(New Uri(this_dl), save_to & $"\Mac\{this_filename}")
                            wc.Dispose()
                        End If
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"I've downloaded {downloaded} / {mac_links} files, {mac_links - downloaded} files left")
                    Next
                Case "Linux"
                    downloads_to_complete = lin_links
                    RichTextBox1.AppendText(Environment.NewLine)
                    RichTextBox1.AppendText($"There are {lin_links} files for me to download")
                    If String.IsNullOrEmpty(save_to) Then
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Creating directory {My.Application.Info.DirectoryPath}\Linux")
                        Directory.CreateDirectory(My.Application.Info.DirectoryPath & "\Linux")
                    Else
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Creating directory {save_to}\Linux")
                        Directory.CreateDirectory(save_to & "\Linux")
                    End If
                    For Each link In lin_downloads
                        downloaded += 1
                        Dim qs = New Uri(link).Query
                        Dim qd = Web.HttpUtility.ParseQueryString(qs)
                        Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
                        Dim downloads = JObject.Parse(download_urls)
                        Dim this_dl = ""
                        If download_type = "Direct" Then
                            this_dl = downloads("signed_url")
                        Else
                            this_dl = downloads("signed_torrent_url")
                        End If
                        Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Starting download for {this_filename} (Linux)")
                        AddHandler wc.DownloadProgressChanged, AddressOf DownloadProgressChanged
                        AddHandler wc.DownloadFileCompleted, AddressOf DownloadProgressCompleted
                        If String.IsNullOrEmpty(save_to) Then
                            Await wc.DownloadFileTaskAsync(New Uri(this_dl), My.Application.Info.DirectoryPath & $"\Linux\{this_filename}")
                            wc.Dispose()
                        Else
                            Await wc.DownloadFileTaskAsync(New Uri(this_dl), save_to & $"\Linux\{this_filename}")
                            wc.Dispose()
                        End If

                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"I've downloaded {downloaded} / {lin_links} files, {lin_links - downloaded} files left")
                    Next
                Case "All"
                    downloads_to_complete = all_links
                    RichTextBox1.AppendText(Environment.NewLine)
                    RichTextBox1.AppendText($"There are {all_links} files for me to download")
                    If String.IsNullOrEmpty(save_to) Then
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Creating directory {My.Application.Info.DirectoryPath}\Windows")
                        Directory.CreateDirectory(My.Application.Info.DirectoryPath & "\Windows")
                    Else
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Creating directory {save_to}\Windows")
                        Directory.CreateDirectory(save_to & "\Windows")
                    End If
                    If String.IsNullOrEmpty(save_to) Then
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Creating directory {My.Application.Info.DirectoryPath}\Mac")
                        Directory.CreateDirectory(My.Application.Info.DirectoryPath & "\Mac")
                    Else
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Creating directory {save_to}\Mac")
                        Directory.CreateDirectory(save_to & "\Mac")
                    End If
                    If String.IsNullOrEmpty(save_to) Then
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Creating directory {My.Application.Info.DirectoryPath}\Linux")
                        Directory.CreateDirectory(My.Application.Info.DirectoryPath & "\Linux")
                    Else
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Creating directory {save_to}\Linux")
                        Directory.CreateDirectory(save_to & "\Linux")
                    End If

                    For Each link In win_downloads
                        downloaded += 1
                        Dim qs = New Uri(link).Query
                        Dim qd = Web.HttpUtility.ParseQueryString(qs)
                        Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
                        Dim downloads = JObject.Parse(download_urls)
                        Dim this_dl = ""
                        If download_type = "Direct" Then
                            this_dl = downloads("signed_url")
                        Else
                            this_dl = downloads("signed_torrent_url")
                        End If
                        Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Starting download for {this_filename} (Windows)")
                        AddHandler wc.DownloadProgressChanged, AddressOf DownloadProgressChanged
                        AddHandler wc.DownloadFileCompleted, AddressOf DownloadProgressCompleted
                        If String.IsNullOrEmpty(save_to) Then
                            Await wc.DownloadFileTaskAsync(New Uri(this_dl), My.Application.Info.DirectoryPath & $"\Windows\{this_filename}")
                            wc.Dispose()
                        Else
                            Await wc.DownloadFileTaskAsync(New Uri(this_dl), save_to & $"\Windows\{this_filename}")
                            wc.Dispose()
                        End If
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"I've downloaded {downloaded} / {all_links} files, {all_links - downloaded} files left")
                    Next
                    For Each link In mac_downloads
                        downloaded += 1
                        Dim qs = New Uri(link).Query
                        Dim qd = Web.HttpUtility.ParseQueryString(qs)
                        Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
                        Dim downloads = JObject.Parse(download_urls)
                        Dim this_dl = ""
                        If download_type = "Direct" Then
                            this_dl = downloads("signed_url")
                        Else
                            this_dl = downloads("signed_torrent_url")
                        End If
                        Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Starting download for {this_filename} (Mac)")
                        AddHandler wc.DownloadProgressChanged, AddressOf DownloadProgressChanged
                        AddHandler wc.DownloadFileCompleted, AddressOf DownloadProgressCompleted
                        If String.IsNullOrEmpty(save_to) Then
                            Await wc.DownloadFileTaskAsync(New Uri(this_dl), My.Application.Info.DirectoryPath & $"\Mac\{this_filename}")
                            wc.Dispose()
                        Else
                            Await wc.DownloadFileTaskAsync(New Uri(this_dl), save_to & $"\Mac\{this_filename}")
                            wc.Dispose()
                        End If
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"I've downloaded {downloaded} / {all_links} files, {all_links - downloaded} files left")
                    Next
                    For Each link In lin_downloads
                        downloaded += 1
                        Dim qs = New Uri(link).Query
                        Dim qd = Web.HttpUtility.ParseQueryString(qs)
                        Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
                        Dim downloads = JObject.Parse(download_urls)
                        Dim this_dl = ""
                        If download_type = "Direct" Then
                            this_dl = downloads("signed_url")
                        Else
                            this_dl = downloads("signed_torrent_url")
                        End If
                        Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"Starting download for {this_filename} (Linux)")
                        AddHandler wc.DownloadProgressChanged, AddressOf DownloadProgressChanged
                        AddHandler wc.DownloadFileCompleted, AddressOf DownloadProgressCompleted
                        If String.IsNullOrEmpty(save_to) Then
                            Await wc.DownloadFileTaskAsync(New Uri(this_dl), My.Application.Info.DirectoryPath & $"\Linux\{this_filename}")
                            wc.Dispose()
                        Else
                            Await wc.DownloadFileTaskAsync(New Uri(this_dl), save_to & $"\Linux\{this_filename}")
                            wc.Dispose()
                        End If
                        RichTextBox1.AppendText(Environment.NewLine)
                        RichTextBox1.AppendText($"I've downloaded {downloaded} / {all_links} files, {all_links - downloaded} files left")
                    Next
            End Select
        End If
    End Sub

    Public Sub DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs)
        ProgressBar1.Value = e.ProgressPercentage
    End Sub
    Public Sub DownloadProgressCompleted(sender As Object, e As EventArgs)
        If downloaded = downloads_to_complete Then
            win_downloads = New List(Of String)
            mac_downloads = New List(Of String)
            lin_downloads = New List(Of String)
            downloaded = 0
            Label1.Visible = False
            Label2.Visible = True
            RadioButton1.Visible = True
            RadioButton2.Visible = True
            RadioButton3.Visible = True
            RadioButton4.Visible = True
            RadioButton5.Visible = True
            RadioButton6.Visible = True
            TextBox1.Visible = True
            Button1.Visible = True
            Button2.Visible = True
            Label3.Visible = True
            ProgressBar1.Visible = False
            GroupBox1.Visible = True
            GroupBox2.Visible = True
        End If
    End Sub

    Public Function PostURL(ByVal url As String, ByVal filename As String, ByVal machine_name As String)
        Dim client As New WebClient
        Dim reqparm As New Specialized.NameValueCollection
        reqparm.Add("machine_name", machine_name)
        reqparm.Add("filename", filename)
        client.Headers.Set("cookie", $"_simpleauth_sess={session_key}")
        Dim responsebytes = client.UploadValues("https://www.humblebundle.com/api/v1/user/download/sign?machine_name=" + machine_name + "&filename=" + filename, "POST", reqparm)
        Dim responsebody = (New Text.UTF8Encoding).GetString(responsebytes)
        client.Dispose()
        Return responsebody
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            Label3.Text = FolderBrowserDialog1.SelectedPath
            save_to = FolderBrowserDialog1.SelectedPath
            RichTextBox1.AppendText(Environment.NewLine)
            RichTextBox1.AppendText($"New save location set ({save_to})")
        End If
    End Sub
End Class