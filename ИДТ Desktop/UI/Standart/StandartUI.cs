using Microsoft.Web.WebView2.Core;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ИТД_Desktop.Script;
using ИТД_Desktop.UI.Aero_UI;

namespace ИТД_Desktop.UI.Standart
{
    public partial class StandartUI : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private bool reallyExit = false;

        public StandartUI()
        {
            InitializeComponent();
            InitTrayIcon();

            webView21.Source = new Uri("https://xn--d1ah4a.com/");
            UpdateTheme(false);
        }

        private void InitTrayIcon()
        {
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }

            Icon appIcon;
            try
            {
                byte[] iconBytes = Properties.Resources.itd;
                using (var ms = new MemoryStream(iconBytes))
                {
                    appIcon = new Icon(ms);
                }
            }
            catch
            {
                appIcon = SystemIcons.Application;
            }
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add(new ToolStripMenuItem("Открыть", null, (s, e) => RestoreWindow()));
            trayMenu.Items.Add(new ToolStripMenuItem("Настройки", null, (s, e) => new StandartUISettings().ShowDialog()));
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(new ToolStripMenuItem("Переключить на тему Aero", null, (s, e) => SwitchToAeroTheme()));
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(new ToolStripMenuItem("Выход", null, (s, e) => ExitApplication()));

            trayIcon = new NotifyIcon
            {
                Icon = appIcon,
                ContextMenuStrip = trayMenu,
                Text = "ИТД Desktop",
                Visible = true
            };

            trayIcon.DoubleClick += (s, e) => RestoreWindow();
            trayIcon.BalloonTipClicked += (s, e) => RestoreWindow();

            trayIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    RestoreWindow();
                }
            };
        }

        private void RestoreWindow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        private void SwitchToAeroTheme()
        {
            Properties.Settings.Default.ThemeSelected = 1;
            Properties.Settings.Default.Save();

            reallyExit = true;
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }

            ThemeHelper.Initialize();
            var aeroForm = new AeroUI();
            Program.AppContext.MainForm = aeroForm;
            aeroForm.Show();

            this.Close();
        }

        private void ExitApplication()
        {
            reallyExit = true;
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!reallyExit && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                this.ShowInTaskbar = false;
                trayIcon?.ShowBalloonTip(2000, "ИТД Desktop", "Клиент свернут в трей и продолжит присылать уведомления.", ToolTipIcon.Info);
                return;
            }

            base.OnFormClosing(e);
        }
        private void ApplyDownloadPath()
        {
            try
            {
                string originalPath = Properties.Settings.Default.FilePath;
                string downloadPath = !string.IsNullOrWhiteSpace(originalPath) && IsValidPath(CleanPath(originalPath))
                    ? CleanPath(originalPath)
                    : GetDefaultDownloadsPath();

                webView21.CoreWebView2.Profile.DefaultDownloadFolderPath = downloadPath;
            }
            catch
            {
                string defaultPath = GetDefaultDownloadsPath();
                webView21.CoreWebView2.Profile.DefaultDownloadFolderPath = defaultPath;
            }
        }
        #region WebView2
        private void webView21_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess) return;
            webView21.CoreWebView2.WebMessageReceived += (s, args) =>
                ClientBridgeHelper.HandleWebMessage(args.WebMessageAsJson, trayIcon);
            ClientBridgeHelper.InjectBridgeScripts(webView21.CoreWebView2);
            ApplyDownloadPath();
        }

        private async void webView21_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            this.Text = $"ИТД Desktop | {webView21.Source}";
            label1.Text = webView21.Source.ToString();
            string theme = Properties.Settings.Default.DarkTheme ? "dark" : "light";
            await webView21.CoreWebView2.ExecuteScriptAsync(
                $"document.documentElement.setAttribute('data-theme', '{theme}'); localStorage.setItem('nowkie_theme', '{theme}');");
            if (Properties.Settings.Default.FirstRunClient)
            {
                ClientBridgeHelper.ShowWelcomeModal(webView21.CoreWebView2);
            }
        }
        #endregion

        public void UpdateTheme(bool isRestartpage = true)
        {
            if (!Properties.Settings.Default.DarkTheme)
            {
                this.BackColor = Color.White;
                this.ForeColor = Color.Black;
                panel1.BackColor = Color.White;
                panel2.BackColor = Color.White;
                label1.ForeColor = Color.Black;
                button1.ForeColor = Color.Black;
                button2.ForeColor = Color.Black;
                button3.ForeColor = Color.Black;
                button4.ForeColor = Color.Black;
                button5.ForeColor = Color.Black;
                button6.ForeColor = Color.Black;
                button7.ForeColor = Color.Black;
                button8.ForeColor = Color.Black;
                button10.ForeColor = Color.Black;
                button9.ForeColor = Color.Black;
                button1.BackColor = Color.White;
                button2.BackColor = Color.White;
                button3.BackColor = Color.White;
                button4.BackColor = Color.White;
                button5.BackColor = Color.White;
                button6.BackColor = Color.White;
                button7.BackColor = Color.White;
                button8.BackColor = Color.White;
                button10.BackColor = Color.White;
                button9.BackColor = Color.White;
            }
            else
            {
                this.BackColor = Color.Black;
                this.ForeColor = Color.White;
                panel1.BackColor = Color.Black;
                panel2.BackColor = Color.Black;
                label1.ForeColor = Color.White;
                button1.ForeColor = Color.White;
                button2.ForeColor = Color.White;
                button3.ForeColor = Color.White;
                button4.ForeColor = Color.White;
                button5.ForeColor = Color.White;
                button6.ForeColor = Color.White;
                button7.ForeColor = Color.White;
                button8.ForeColor = Color.White;
                button9.ForeColor = Color.White;
                button10.ForeColor = Color.White;
                button1.BackColor = Color.Black;
                button2.BackColor = Color.Black;
                button3.BackColor = Color.Black;
                button4.BackColor = Color.Black;
                button5.BackColor = Color.Black;
                button6.BackColor = Color.Black;
                button7.BackColor = Color.Black;
                button8.BackColor = Color.Black;
                button9.BackColor = Color.Black;
                button10.BackColor = Color.Black;
            }
            if (isRestartpage)
                webView21.CoreWebView2.Reload();
        }

        private void button1_Click(object sender, EventArgs e) => webView21.CoreWebView2.GoBack();
        private void button2_Click(object sender, EventArgs e) => webView21.CoreWebView2.GoForward();
        private void button3_Click(object sender, EventArgs e) => webView21.CoreWebView2.Reload();
        private void button4_Click(object sender, EventArgs e) => webView21.CoreWebView2.Navigate("https://xn--d1ah4a.com/");
        private void button5_Click(object sender, EventArgs e) => webView21.CoreWebView2.Navigate("https://xn--d1ah4a.com/search");
        private void button6_Click(object sender, EventArgs e) => webView21.CoreWebView2.Navigate("https://xn--d1ah4a.com/notifications");
        private void button7_Click(object sender, EventArgs e) => new StandartUISettings().ShowDialog();
        private void button8_Click(object sender, EventArgs e) => webView21.CoreWebView2.Navigate("https://xn--d1ah4a.com/@me");
        private void button9_Click_1(object sender, EventArgs e) => webView21.CoreWebView2.Navigate("https://xn--d1ah4a.com/shop");
        private void button10_Click(object sender, EventArgs e) => webView21.CoreWebView2.Navigate("https://xn--d1ah4a.com/event");

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