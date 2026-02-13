using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Windows.Forms;
using ИТД_Desktop.Script.Data;
using ИТД_Desktop.UI.Aero_UI;
using ИТД_Desktop.UI.Standart;

namespace ИТД_Desktop
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CheckVersionFromWeb();
            if (Properties.Settings.Default.ThemeSelected == 0)
            {
                Application.Run(new StandartUI());
            }
            else if (Properties.Settings.Default.ThemeSelected == 1)
            {
                ThemeHelper.Initialize();
                Application.Run(new UI.Aero_UI.AeroUI());
            }
        }
        private static async void CheckVersionFromWeb()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                    string json = await httpClient.GetStringAsync("http://k90052gj.beget.tech/API/versions.json");
                    var jObject = JObject.Parse(json);
                    string onlineVersion = jObject["ITDdes"]?["version"]?.ToString();
                    if (!string.IsNullOrEmpty(onlineVersion) && onlineVersion != Data.version)
                    {
                        DialogResult result = MessageBox.Show($"Обнаружена новая версия {onlineVersion}\n\n" +
                            $"У вас установлнена {Data.version}\n\n" +
                            $"Хотите установить новую версию?", "Новая версия", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (result == DialogResult.Yes)
                            Process.Start("https://github.com/xachapyri-dev/itd_Desktop");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось проверить обновления: " + ex.Message + "\nПроверьте ваше интернет-соединение или файл Launcher.log.", "Ошибка сети", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}