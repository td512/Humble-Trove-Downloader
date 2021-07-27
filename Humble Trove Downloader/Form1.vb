Imports System.IO
Imports System.Net
Imports System.Threading
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Microsoft.WindowsAPICodePack.Taskbar
Imports Syroot.Windows.IO
Imports Microsoft.Win32
Public Class Form1

    Public baseuri = "https://www.humblebundle.com/api/v1/trove/chunk?property=popularity&direction=desc&index="
    Public counter = 0
    Public chunked = False
    Public download = ""
    Public download_type = ""
    Public session_key = ""
    Public save_to
    Public win_downloads = New List(Of String)
    Public mac_downloads = New List(Of String)
    Public lin_downloads = New List(Of String)
    Public win_links = New Integer
    Public mac_links = New Integer
    Public lin_links = New Integer
    Public all_links = New Integer
    Public downloaded = New Integer
    Public downloads_to_complete = New Integer
    Public this_filepath = ""
    Public wc As New WebClient
    Public wb
    Public exit_do = 0

    Public Function ChunkDownloader(sender As Object, e As DownloadStringCompletedEventArgs)
        If e.Result = "[]" Then
            exit_do = 1
        Else
            counter += 1
        End If
        Dim trove = JArray.Parse(e.Result)
        For Each Row In trove
            If Row("downloads")("windows") IsNot Nothing Then
                win_downloads.Add($"https://www.humblebundle.com/api/v1/user/download/sign?filename={Row("downloads")("windows")("url")("web")}&machine_name={Row("downloads")("windows")("machine_name")}")
            End If
            If Row("downloads")("mac") IsNot Nothing Then
                mac_downloads.Add($"https://www.humblebundle.com/api/v1/user/download/sign?filename={Row("downloads")("mac")("url")("web")}&machine_name={Row("downloads")("mac")("machine_name")}")
            End If
            If Row("downloads")("linux") IsNot Nothing Then
                lin_downloads.Add($"https://www.humblebundle.com/api/v1/user/download/sign?filename={Row("downloads")("linux")("url")("web")}&machine_name={Row("downloads")("linux")("machine_name")}")
            End If
        Next
    End Function
    Public Async Function InitialLoad() As Task
        Do
            If exit_do = 1 Then
                Exit Do
            End If

            Dim client As New WebClient
            AddHandler client.DownloadProgressChanged, AddressOf DownloadProgressChanged
            AddHandler client.DownloadStringCompleted, AddressOf ChunkDownloader
            Await client.DownloadStringTaskAsync(New Uri(baseuri + counter.ToString))

        Loop
    End Function
    Private Async Sub Form1_VisibleChanged(sender As Object, e As EventArgs) Handles MyBase.VisibleChanged
        RichTextBox1.AppendText("Fetching Humble Trove Items")

        Await Task.Run(Function() InitialLoad())
        win_links = win_downloads.Count
        mac_links = mac_downloads.Count
        lin_links = lin_downloads.Count
        all_links = win_links + mac_links + lin_links
        RichTextBox1.AppendText(Environment.NewLine)
        RichTextBox1.AppendText("Fetched Chunks! Parsing...")
        RichTextBox1.AppendText(Environment.NewLine)
        RichTextBox1.AppendText($"Windows: {win_links} available downloads")
        RichTextBox1.AppendText(Environment.NewLine)
        RichTextBox1.AppendText($"Mac: {mac_links} available downloads")
        RichTextBox1.AppendText(Environment.NewLine)
        RichTextBox1.AppendText($"Linux: {lin_links} available downloads")
        RichTextBox1.AppendText(Environment.NewLine)
        RichTextBox1.AppendText($"Total: {all_links} available downloads")

        Label1.Visible = False
        Label2.Visible = True
        Label3.Visible = True
        Label4.Visible = True
        RadioButton1.Visible = True
        RadioButton2.Visible = True
        RadioButton3.Visible = True
        RadioButton4.Visible = True
        RadioButton5.Visible = True
        RadioButton6.Visible = True
        TextBox1.Visible = True
        Button1.Visible = True
        Button2.Visible = True
        Button3.Visible = True
        Label3.Visible = True
        ProgressBar1.Visible = False
        GroupBox1.Visible = True
        GroupBox2.Visible = True
        Label3.Text = save_to
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
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal)
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
            Label4.Visible = False
            TextBox1.Visible = False
            Button1.Visible = False
            Button2.Visible = False
            Button3.Visible = False
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
            RichTextBox1.AppendText(Environment.NewLine)
            RichTextBox1.AppendText("Starting downloads (this could take a while)")

            Select Case download
                Case "Windows"
                    Await GetFiles(win_downloads, win_links, "Windows")
                Case "Mac"
                    Await GetFiles(mac_downloads, mac_links, "Mac")
                Case "Linux"
                    Await GetFiles(lin_downloads, lin_links, "Linux")
                Case "All"
                    Await GetFiles(win_downloads, all_links, "Windows")
                    Await GetFiles(mac_downloads, all_links, "Mac")
                    Await GetFiles(lin_downloads, all_links, "Linux")
            End Select
        End If
    End Sub

    Public Sub DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs)
        ProgressBar1.Invoke(Sub() ProgressBar1.Value = e.ProgressPercentage)
    End Sub
    Public Sub DownloadProgressCompleted(sender As Object, e As EventArgs)

        File.SetLastWriteTime(this_filepath, wc.ResponseHeaders("Last-Modified"))
        TaskbarManager.Instance.SetProgressValue((downloaded / downloads_to_complete) * 100, 100)

        If downloaded = downloads_to_complete Then
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress)
            win_downloads = New List(Of String)
            mac_downloads = New List(Of String)
            lin_downloads = New List(Of String)
            downloaded = 0
            Label1.Visible = False
            Label2.Visible = True
            Label3.Visible = True
            Label4.Visible = True
            RadioButton1.Visible = True
            RadioButton2.Visible = True
            RadioButton3.Visible = True
            RadioButton4.Visible = True
            RadioButton5.Visible = True
            RadioButton6.Visible = True
            TextBox1.Visible = True
            Button1.Visible = True
            Button2.Visible = True
            Button3.Visible = True
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
        client.Headers.Set("cookie", $"_simpleauth_sess=""{session_key}""")
        Dim responsebytes As Byte()
        Try
            responsebytes = client.UploadValues(url, "POST", reqparm)
        Catch ex As WebException
            client.Dispose()
            Return "<!docty"
        End Try
        If responsebytes Is Nothing Then
            client.Dispose()
            Return "<!docty"
        End If
        Dim responsebody = (New Text.UTF8Encoding).GetString(responsebytes)
        client.Dispose()
        Return responsebody
    End Function

    Public Async Function GetFiles(lnk_downloads As List(Of String), linkCount As Integer, osName As String) As Task
        downloads_to_complete = linkCount
        RichTextBox1.AppendText($"There are {linkCount} files for me to download{Environment.NewLine}")
        RichTextBox1.AppendText($"Creating directory {save_to}\{osName}{Environment.NewLine}")
        Directory.CreateDirectory(save_to & $"\{osName}")
        For Each link In lnk_downloads
            downloaded += 1
            Dim qs = New Uri(link).Query
            Dim qd = Web.HttpUtility.ParseQueryString(qs)
            Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
            If download_urls.Substring(0, 7) = "<!docty" Then
                RichTextBox1.AppendText($"Download Failed (Not Subscribed){Environment.NewLine}")
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress)
                win_downloads = New List(Of String)
                mac_downloads = New List(Of String)
                lin_downloads = New List(Of String)
                downloaded = 0
                Label1.Visible = False
                Label2.Visible = True
                Label3.Visible = True
                Label4.Visible = True
                RadioButton1.Visible = True
                RadioButton2.Visible = True
                RadioButton3.Visible = True
                RadioButton4.Visible = True
                RadioButton5.Visible = True
                RadioButton6.Visible = True
                TextBox1.Visible = True
                Button1.Visible = True
                Button2.Visible = True
                Button3.Visible = True
                Label3.Visible = True
                ProgressBar1.Visible = False
                GroupBox1.Visible = True
                GroupBox2.Visible = True
                Exit For
            End If
            Dim downloads = JObject.Parse(download_urls)
            Dim this_dl = ""
            If download_type = "Direct" Then
                this_dl = downloads("signed_url")
            Else
                this_dl = downloads("signed_torrent_url")
            End If
            If this_dl Is Nothing Then
                Continue For
            End If
            Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
            this_filepath = $"{save_to}\{osName}\{this_filename}"

            If File.Exists(this_filepath) Then
                RichTextBox1.AppendText($"File already exits, skipping. {osName}\{this_filename} - {downloaded} / {linkCount} processed{Environment.NewLine}")
                AddHandler wc.DownloadFileCompleted, AddressOf DownloadProgressCompleted
                Continue For
            End If

            RichTextBox1.AppendText($"Starting download for {this_filename} ({osName}){Environment.NewLine}")
            AddHandler wc.DownloadProgressChanged, AddressOf DownloadProgressChanged
            AddHandler wc.DownloadFileCompleted, AddressOf DownloadProgressCompleted
            Await wc.DownloadFileTaskAsync(New Uri(this_dl), this_filepath)
            wc.Dispose()
            RichTextBox1.AppendText($"I've downloaded {downloaded} / {linkCount} processed{Environment.NewLine}")
        Next
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            Dim rk = Registry.CurrentUser.OpenSubKey("Software\Trove Downloader", True)
            Label3.Text = FolderBrowserDialog1.SelectedPath
            save_to = FolderBrowserDialog1.SelectedPath
            rk.SetValue("last_saved", save_to, RegistryValueKind.String)
            RichTextBox1.AppendText(Environment.NewLine)
            RichTextBox1.AppendText($"New save location set ({save_to})")
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        wb = New WebView
        wb.Show()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim rk = Registry.CurrentUser.CreateSubKey("Software\Trove Downloader")
        If rk.GetValue("WBV") = 0 Then
            SetBrowserFeatureControl()
            rk.SetValue("WBV", 1, RegistryValueKind.DWord)
            Process.Start(My.Application.Info.DirectoryPath & "\Humble Trove Downloader.exe")
            Close()
        End If
        If rk.GetValue("last_saved") Is Nothing Then
            save_to = KnownFolders.Downloads.Path.ToString
            rk.SetValue("last_saved", KnownFolders.Downloads.Path.ToString, RegistryValueKind.String)
        Else
            save_to = rk.GetValue("last_saved")
            Label3.Text = save_to
        End If
    End Sub

    Public Sub BreakOut()
        wb.Dispose()
        Button3.Enabled = False
        TextBox1.Text = sessionCookie
        TextBox1.Enabled = False
    End Sub

#Region "IE Fix"
    Private Shared Sub SetBrowserFeatureControlKey(feature As String, appName As String, value As UInteger)
        Using key = Registry.CurrentUser.CreateSubKey([String].Concat("Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature), RegistryKeyPermissionCheck.ReadWriteSubTree)
            key.SetValue(appName, DirectCast(value, UInt32), RegistryValueKind.DWord)
        End Using
    End Sub

    Private Sub SetBrowserFeatureControl()
        ' http://msdn.microsoft.com/en-us/library/ee330720(v=vs.85).aspx

        ' FeatureControl settings are per-process
        Dim fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName)

        ' make the control is not running inside Visual Studio Designer
        If [String].Compare(fileName, "devenv.exe", True) = 0 OrElse [String].Compare(fileName, "XDesProc.exe", True) = 0 Then
            Return
        End If

        SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, GetBrowserEmulationMode())
        ' Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode.
        SetBrowserFeatureControlKey("FEATURE_AJAX_CONNECTIONEVENTS", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_MANAGE_SCRIPT_CIRCULAR_REFS", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_DOMSTORAGE ", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_GPU_RENDERING ", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_IVIEWOBJECTDRAW_DMLT9_WITH_GDI  ", fileName, 0)
        SetBrowserFeatureControlKey("FEATURE_DISABLE_LEGACY_COMPRESSION", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_LOCALMACHINE_LOCKDOWN", fileName, 0)
        SetBrowserFeatureControlKey("FEATURE_BLOCK_LMZ_OBJECT", fileName, 0)
        SetBrowserFeatureControlKey("FEATURE_BLOCK_LMZ_SCRIPT", fileName, 0)
        SetBrowserFeatureControlKey("FEATURE_DISABLE_NAVIGATION_SOUNDS", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_SCRIPTURL_MITIGATION", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_SPELLCHECKING", fileName, 0)
        SetBrowserFeatureControlKey("FEATURE_STATUS_BAR_THROTTLING", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_TABBED_BROWSING", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_VALIDATE_NAVIGATE_URL", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_WEBOC_DOCUMENT_ZOOM", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_WEBOC_POPUPMANAGEMENT", fileName, 0)
        SetBrowserFeatureControlKey("FEATURE_WEBOC_MOVESIZECHILD", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_ADDON_MANAGEMENT", fileName, 0)
        SetBrowserFeatureControlKey("FEATURE_WEBSOCKET", fileName, 1)
        SetBrowserFeatureControlKey("FEATURE_WINDOW_RESTRICTIONS ", fileName, 0)
        SetBrowserFeatureControlKey("FEATURE_XMLHTTP", fileName, 1)
    End Sub

    Private Function GetBrowserEmulationMode() As UInt32
        Dim browserVersion As Integer = 7
        Using ieKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Internet Explorer", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues)
            Dim version = ieKey.GetValue("svcVersion")
            If Nothing = version Then
                version = ieKey.GetValue("Version")
                If Nothing = version Then
                    Throw New ApplicationException("Microsoft Internet Explorer is required!")
                End If
            End If
            Integer.TryParse(version.ToString().Split("."c)(0), browserVersion)
        End Using

        Dim mode As UInt32 = 10000
        ' Internet Explorer 10. Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode. Default value for Internet Explorer 10.
        Select Case browserVersion
            Case 7
                mode = 7000
                ' Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode. Default value for applications hosting the WebBrowser Control.
                Exit Select
            Case 8
                mode = 8000
                ' Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode. Default value for Internet Explorer 8
                Exit Select
            Case 9
                mode = 9000
                ' Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
                Exit Select
            Case 10
                mode = 10000
                ' Internet Explorer 10. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
                Exit Select
            Case 11
                mode = 11001
                ' Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
                Exit Select
            Case Else
                ' use IE10 mode by default
                Exit Select
        End Select

        Return mode
    End Function
#End Region
End Class