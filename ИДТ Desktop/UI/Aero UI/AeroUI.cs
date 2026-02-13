using Krypton.Toolkit;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ИТД_Desktop.UI.Aero_UI
{
    public partial class AeroUI : KryptonForm
    {
        public AeroUI()
        {
            InitializeComponent();
            ThemeHelper.RegisterForm(this, kryptonManager1);
            webView21.Source = new Uri("https://xn--d1ah4a.com/");
            buttonSpecAny1.Click += (s, e) =>
            {
                var settings = new AeroSettingsUI();
                settings.Show();
            };
        }
        #region WebView2
        private void webView21_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                MessageBox.Show("Ошибка инициализации WebView2: " + e.InitializationException?.Message);
                return;
            }
            try
            {
                string originalPath = Properties.Settings.Default.FilePath;
                Console.WriteLine($"Original path from settings: '{originalPath}'");
                string downloadPath;
                if (!string.IsNullOrWhiteSpace(originalPath))
                {
                    string cleanedPath = CleanPath(originalPath);
                    Console.WriteLine($"Cleaned path: '{cleanedPath}'");
                    if (IsValidPath(cleanedPath))
                    {
                        downloadPath = cleanedPath;
                    }
                    else
                    {
                        Console.WriteLine($"Path is invalid, using default");
                        downloadPath = GetDefaultDownloadsPath();
                    }
                }
                else
                {
                    downloadPath = GetDefaultDownloadsPath();
                }
                Console.WriteLine($"Setting download path to: '{downloadPath}'");
                webView21.CoreWebView2.Profile.DefaultDownloadFolderPath = downloadPath;
            }
            catch (Exception ex)
            {
                string defaultPath = GetDefaultDownloadsPath();
                webView21.CoreWebView2.Profile.DefaultDownloadFolderPath = defaultPath;
                MessageBox.Show($"Ошибка при установке папки загрузок: {ex.Message}\n\n" +
                               $"Используется стандартная папка: {defaultPath}",
                               "Ошибка",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Warning);
                Console.WriteLine($"Exception details: {ex}");
            }
        }
        private async void webView21_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            this.Text = $"ИТД Desktop | {webView21.Source}";
            kryptonLabel1.Text = webView21.Source.ToString();
            if (Properties.Settings.Default.DarkTheme)
                await webView21.CoreWebView2.ExecuteScriptAsync("document.documentElement.setAttribute(\"data-theme\", \"dark\");\n" +
                    "localStorage.setItem(\"nowkie_theme\", \"dark\")");
            else
                await webView21.CoreWebView2.ExecuteScriptAsync("document.documentElement.setAttribute(\"data-theme\", \"light\");\n" +
                    "localStorage.setItem(\"nowkie_theme\", \"light\")");
        }
        #endregion
        private void kryptonButton1_Click(object sender, EventArgs e) => webView21.CoreWebView2.GoBack();
        private void kryptonButton2_Click(object sender, EventArgs e) => webView21.CoreWebView2.GoForward();
        private void kryptonButton3_Click(object sender, EventArgs e) => webView21.CoreWebView2.Reload();
        private void kryptonButton4_Click(object sender, EventArgs e) => webView21.CoreWebView2.Navigate("https://xn--d1ah4a.com/feed");
        private void kryptonButton5_Click(object sender, EventArgs e) => webView21.CoreWebView2.Navigate("https://xn--d1ah4a.com/explore");
        private void kryptonButton6_Click(object sender, EventArgs e) => webView21.CoreWebView2.Navigate("https://xn--d1ah4a.com/notifications");
        private void kryptonButton7_Click(object sender, EventArgs e) => webView21.CoreWebView2.Navigate("https://xn--d1ah4a.com/me");
        public void UpdateThemeForWeb() => webView21.CoreWebView2.Reload();
        private string GetDefaultDownloadsPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads"
            );
        }
        private string CleanPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;
            path = path.Trim();
            if (path.StartsWith("\"") && path.EndsWith("\""))
            {
                path = path.Substring(1, path.Length - 2);
            }
            return path;
        }
        private bool IsValidPath(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return false;
                if (!Path.IsPathRooted(path))
                {
                    path = Path.GetFullPath(path);
                }
                string invalidChars = new string(Path.GetInvalidPathChars());
                if (path.Any(c => invalidChars.Contains(c)))
                    return false;
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                        if (Directory.GetFileSystemEntries(path).Length == 0)
                            Directory.Delete(path);
                    }
                    catch
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}