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
        private const string PrimaryVersionUrl = "http://k90052gj.beget.tech/API/versions.json";
        private const string FallbackVersionUrl = "https://raw.githubusercontent.com/xachapyri-dev/VersionProgramCheck/refs/heads/main/versions.json";

        public static ApplicationContext AppContext { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CheckVersionFromWeb();

            AppContext = new ApplicationContext();

            if (Properties.Settings.Default.ThemeSelected == 1)
            {
                ThemeHelper.Initialize();
                var aero = new AeroUI();
                AppContext.MainForm = aero;
                aero.Show();
            }
            else
            {
                var std = new StandartUI();
                AppContext.MainForm = std;
                std.Show();
            }

            Application.Run(AppContext);
        }

        private static async void CheckVersionFromWeb()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                    httpClient.Timeout = TimeSpan.FromSeconds(5);

                    string json = null;
                    try
                    {
                        json = await httpClient.GetStringAsync(PrimaryVersionUrl);
                    }
                    catch
                    {
                        try
                        {
                            json = await httpClient.GetStringAsync(FallbackVersionUrl);
                        }
                        catch
                        {
                            json = null;
                        }
                    }

                    if (string.IsNullOrEmpty(json)) return;

                    var jObject = JObject.Parse(json);
                    string onlineVersion = jObject["ITDdes"]?["version"]?.ToString();

                    if (!string.IsNullOrEmpty(onlineVersion) && onlineVersion != Data.version)
                    {
                        DialogResult result = MessageBox.Show(
                            $"Обнаружена новая версия {onlineVersion}\n\n" +
                            $"У вас установлена {Data.version}\n\n" +
                            $"Хотите установить новую версию?",
                            "Новая версия",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            Process.Start("https://github.com/xachapyri-dev/itd_Desktop");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка проверки обновлений: " + ex.Message);
            }
        }
    }
}