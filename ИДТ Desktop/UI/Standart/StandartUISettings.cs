using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ИТД_Desktop.Script;

namespace ИТД_Desktop.UI.Standart
{
    public partial class StandartUISettings : Form
    {
        StandartUI main = Application.OpenForms.OfType<StandartUI>().FirstOrDefault();
        private bool isInitializing = false;

        public StandartUISettings()
        {
            InitializeComponent();
            ChangeTheme();
            isInitializing = true;
            checkBox1.Checked = Properties.Settings.Default.DarkTheme;
            checkBox2.Checked = ClientBridgeHelper.IsAutostartEnabled();
            isInitializing = false;
            textBox1.Text = Properties.Settings.Default.FilePath;
            if (Properties.Settings.Default.ThemeSelected == 0)
            {
                button5.Enabled = false;
                button5.Text = "Установленно!";
            }
            richTextBox1.Text = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LICENCE.txt"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialog = folderBrowserDialog1.ShowDialog();
            if (dialog == DialogResult.Cancel)
                return;
            textBox1.Text = folderBrowserDialog1.SelectedPath;
            Properties.Settings.Default.FilePath = folderBrowserDialog1.SelectedPath;
            Properties.Settings.Default.Save();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DarkTheme = checkBox1.Checked;
            Properties.Settings.Default.Save();
            ChangeTheme();
            main?.UpdateTheme();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ClientBridgeHelper.SetAutostart(checkBox2.Checked);
        }

        private void pictureBox2_Click(object sender, EventArgs e) => new ImageViewerStandartUI(Properties.Resources.ITD_Aero).ShowDialog();
        private void pictureBox1_Click(object sender, EventArgs e) => new ImageViewerStandartUI(Properties.Resources.ITD_Standart).ShowDialog();
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://github.com/xachapyri-dev/itd_Desktop/issues");
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://github.com/xachapyri-dev/itd_Desktop");
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://github.com/xachapyri-dev");
        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://t.me/nowkies");
        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://t.me/kroshidanielYouTube");

        private void ChangeTheme()
        {
            if (!Properties.Settings.Default.DarkTheme)
            {
                this.BackColor = Color.White;
                tabPage1.ForeColor = Color.Black;
                tabPage2.ForeColor = Color.Black;
                tabPage3.ForeColor = Color.Black;
                tabPage1.BackColor = Color.White;
                tabPage2.BackColor = Color.White;
                tabPage3.BackColor = Color.White;
                button1.BackColor = Color.White;
                button2.BackColor = Color.White;
                button3.BackColor = Color.White;
                button4.BackColor = Color.White;
                button5.BackColor = Color.White;
                button5.ForeColor = Color.Black;
                button6.BackColor = Color.White;
                textBox1.ForeColor = Color.Black;
                textBox1.BackColor = Color.White;
                checkBox1.ForeColor = Color.Black;
                checkBox2.ForeColor = Color.Black;
                linkLabel1.LinkColor = Color.Blue;
                linkLabel2.LinkColor = Color.Blue;
                linkLabel3.LinkColor = Color.Blue;
                linkLabel4.LinkColor = Color.Blue;
                linkLabel5.LinkColor = Color.Blue;
            }
            else
            {
                this.BackColor = Color.Black;
                tabPage1.ForeColor = Color.White;
                tabPage2.ForeColor = Color.White;
                tabPage3.ForeColor = Color.White;
                tabPage1.BackColor = Color.Black;
                tabPage2.BackColor = Color.Black;
                tabPage3.BackColor = Color.Black;
                button1.BackColor = Color.Black;
                button2.BackColor = Color.Black;
                button3.BackColor = Color.Black;
                button4.BackColor = Color.Black;
                button5.BackColor = Color.Black;
                button5.ForeColor = Color.White;
                button6.BackColor = Color.Black;
                textBox1.BackColor = Color.Black;
                textBox1.ForeColor = Color.White;
                checkBox1.ForeColor = Color.White;
                checkBox2.ForeColor = Color.White;
                linkLabel1.LinkColor = Color.White;
                linkLabel2.LinkColor = Color.White;
                linkLabel3.LinkColor = Color.White;
                linkLabel4.LinkColor = Color.White;
                linkLabel5.LinkColor = Color.White;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Предупреждение!\n" +
                "Эта кнопка сбрасывает ТОЛЬКО те настройки, которые были внесены пользователем (например: папка загрузок, оформление).",
                "Предупреждение!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                DialogResult result1 = MessageBox.Show(
                    "Для того чтобы изменения вошли в силу, необходимо перезапустить клиент.\n" +
                    "Сделать сейчас?", "Успешно", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result1 == DialogResult.Yes)
                    Application.Restart();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Предупреждение!\n" +
                "Эта кнопка сбрасывает абсолютно ВСЕ пользовательские настройки (например: автозагрузка приложения, данные браузера WebView2, папка загрузок, оформление).",
                "Предупреждение!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // Удаляем приложение из автозагрузки в реестре Windows
                ClientBridgeHelper.SetAutostart(false);

                Properties.Settings.Default.Reset();
                Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CleanerAll.exe"));
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ThemeSelected = 0;
            Properties.Settings.Default.Save();
            button5.Enabled = false;
            button5.Text = "Установленно!";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ThemeSelected = 1;
            Properties.Settings.Default.Save();
            button5.Text = "Установить";
            button5.Enabled = true;

            DialogResult result = MessageBox.Show(
                "Для того чтобы изменения вошли в силу, необходимо перезапустить клиент.\n" +
                "Сделать сейчас?", "Успешно", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                Application.Restart();
        }
    }
}