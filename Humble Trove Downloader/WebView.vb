Imports System.IO
Imports Microsoft.Win32
Imports HumbleHelper
Public Class WebView

    Private Sub WebView_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Registry.CurrentUser.CreateSubKey("Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\humblebundle.com")
        Dim rk = Registry.CurrentUser.CreateSubKey("Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\humblebundle.com\www")
        rk.SetValue("https", 2, RegistryValueKind.DWord)

        Registry.CurrentUser.CreateSubKey("Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\gstatic.com")
        rk = Registry.CurrentUser.CreateSubKey("Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\gstatic.com\www")
        rk.SetValue("https", 2, RegistryValueKind.DWord)

        rk = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\2", True)
        rk.SetValue("CurrentLevel", 65536, RegistryValueKind.DWord)
        rk.SetValue("1001", 0, RegistryValueKind.DWord)
        rk.SetValue("1004", 1, RegistryValueKind.DWord)
        rk.SetValue("1201", 1, RegistryValueKind.DWord)
        rk.SetValue("1206", 0, RegistryValueKind.DWord)
        rk.SetValue("1209", 0, RegistryValueKind.DWord)
        rk.SetValue("1406", 0, RegistryValueKind.DWord)
        rk.SetValue("1407", 0, RegistryValueKind.DWord)
        rk.SetValue("1409", 3, RegistryValueKind.DWord)
        rk.SetValue("1607", 0, RegistryValueKind.DWord)
        rk.SetValue("1804", 0, RegistryValueKind.DWord)
        rk.SetValue("1809", 3, RegistryValueKind.DWord)
        rk.SetValue("1A00", 0, RegistryValueKind.DWord)
        rk.SetValue("1A04", 0, RegistryValueKind.DWord)
        rk.SetValue("1A05", 0, RegistryValueKind.DWord)
        rk.SetValue("1A10", 0, RegistryValueKind.DWord)
        rk.SetValue("1C00", 196608, RegistryValueKind.DWord)
        rk.SetValue("2101", 1, RegistryValueKind.DWord)
        rk.SetValue("2102", 0, RegistryValueKind.DWord)
        rk.SetValue("2200", 0, RegistryValueKind.DWord)
        rk.SetValue("2201", 0, RegistryValueKind.DWord)
        rk.SetValue("2301", 3, RegistryValueKind.DWord)
        rk.SetValue("2702", 3, RegistryValueKind.DWord)
        AddHandler WebBrowser1.DocumentCompleted, AddressOf navigate_event

    End Sub

    Public Sub navigate_event(sender As Object, e As WebBrowserDocumentCompletedEventArgs)
        If Path.GetFileName(New Uri(WebBrowser1.Url.AbsoluteUri).LocalPath) = "library" Then
            Dim result = WebHelper.GetGlobalCookies(WebBrowser1.Url.AbsoluteUri)
            Dim baseSessionKey = result.Split(",")(0)
            sessionCookie = baseSessionKey.Split("=")(1)
            Humble_Trove_Downloader.Form1.BreakOut()
        End If
    End Sub
End Class