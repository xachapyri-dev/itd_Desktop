using Krypton.Toolkit;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ИТД_Desktop.UI.Aero_UI
{
    public partial class AeroSettingsUI : KryptonForm
    {
        AeroUI main = Application.OpenForms.OfType<AeroUI>().FirstOrDefault();
        public AeroSettingsUI()
        {
            InitializeComponent();
            ThemeHelper.RegisterForm(this, kryptonManager1);
            InitializeThemeComboBox();
            kryptonWebBrowser1.Url = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LICENCE.txt"));
            if (Properties.Settings.Default.DarkTheme)
                kryptonCheckBox1.Checked = true;
            else
                kryptonCheckBox1.Checked = false;
            kryptonTextBox1.Text = Properties.Settings.Default.FilePath;
            if (Properties.Settings.Default.ThemeSelected == 1)
            {
                kryptonButton6.Enabled = false;
                kryptonButton6.Text = "Установленно!";
            }
        }
        private void InitializeThemeComboBox()
        {
            kryptonComboBox1.Items.Clear();
            kryptonComboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            var themes = ThemeHelper.GetAllThemes();
            foreach (var theme in themes.Keys)
            {
                kryptonComboBox1.Items.Add(theme);
            }
            var currentTheme = ThemeHelper.GetCurrentTheme();
            foreach (var theme in themes)
            {
                if (theme.Value == currentTheme)
                {
                    kryptonComboBox1.SelectedItem = theme.Key;
                    break;
                }
            }
        }
        private void KryptonComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (kryptonComboBox1.SelectedItem != null)
            {
                string selectedThemeName = kryptonComboBox1.SelectedItem.ToString();
                var themes = ThemeHelper.GetAllThemes();
                if (themes.ContainsKey(selectedThemeName))
                {
                    ThemeHelper.ApplyTheme(themes[selectedThemeName]);
                }
            }
        }
        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            Process.Start(AppDomain.CurrentDomain.BaseDirectory);
        }
        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            DialogResult dialog = folderBrowserDialog1.ShowDialog();
            if (dialog == DialogResult.Cancel)
                return;
            kryptonTextBox1.Text = folderBrowserDialog1.SelectedPath;
            Properties.Settings.Default.FilePath = folderBrowserDialog1.SelectedPath;
            Properties.Settings.Default.Save();
        }
        private void kryptonButton3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Предупреждение!\n" +
                "Эта кнопка сбрасывает ТОЛЬКО те настройки, которые были внесены пользователем (например: папка загрузок, оформление).",
                "Предупреждение!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                DialogResult result1 = MessageBox.Show("Для того чтобы изменения вошли в силу, необходимо перезапустить клиент.\n" +
                    "Сделать сейчас?", "Успешно", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result1 == DialogResult.Yes)
                    Application.Restart();
            }
        }
        private void kryptonButton4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Предупреждение!\n" +
                "Эта кнопка сбрасывает ТОЛЬКО те настройки, которые были внесены пользователем (например: папка загрузок, оформление).",
                "Предупреждение!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CleanerAll.exe"));
            }
        }
        private void kryptonCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (kryptonCheckBox1.Checked)
                Properties.Settings.Default.DarkTheme = true;
            else
                Properties.Settings.Default.DarkTheme = false;
            Properties.Settings.Default.Save();
            main.UpdateThemeForWeb();
        }
        private void kryptonPictureBox1_Click(object sender, EventArgs e) => new ImageViewerAeroUI(Properties.Resources.ITD_Standart).ShowDialog();
        private void kryptonPictureBox2_Click(object sender, EventArgs e) => new ImageViewerAeroUI(Properties.Resources.ITD_Aero).ShowDialog();
        private void kryptonButton5_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ThemeSelected = 0;
            Properties.Settings.Default.Save();
            kryptonButton5.Text = "Установить";
            kryptonButton5.Enabled = true;
            DialogResult result = MessageBox.Show("Для того чтобы изменения вошли в силу, необхадимо перезапустить клиент.\n" +
                "Сделать сейчас?", "Успешно", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                Application.Restart();
        }
        private void kryptonButton6_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ThemeSelected = 1;
            Properties.Settings.Default.Save();
            kryptonButton6.Enabled = false;
            kryptonButton6.Text = "Установленно!";
        }
        private void kryptonLinkLabel1_LinkClicked(object sender, EventArgs e) => Process.Start("https://github.com/xachapyri-dev/itd_Desktop/issues");
        private void kryptonLinkLabel2_LinkClicked(object sender, EventArgs e) => Process.Start("https://github.com/xachapyri-dev/itd_Desktop");
        private void kryptonLinkLabel3_LinkClicked(object sender, EventArgs e) => Process.Start("https://github.com/xachapyri-dev");
        private void kryptonLinkLabel4_LinkClicked(object sender, EventArgs e) => Process.Start("https://t.me/nowkies");
        private void kryptonLinkLabel5_LinkClicked(object sender, EventArgs e) => Process.Start("https://t.me/kroshidanielYouTube");
    }
}