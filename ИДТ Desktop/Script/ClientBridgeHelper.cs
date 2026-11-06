using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;

namespace ИТД_Desktop.Script
{
    public static class ClientBridgeHelper
    {
        private const string AppName = "ITD_Desktop";
        private const string RunRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

        public static void SetAutostart(bool enable)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunRegistryKey, true))
                {
                    if (key == null) return;

                    if (enable)
                    {
                        key.SetValue(AppName, $"\"{Application.ExecutablePath}\"");
                    }
                    else
                    {
                        if (key.GetValue(AppName) != null)
                            key.DeleteValue(AppName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка изменения автозагрузки: " + ex.Message);
            }
        }
        public static bool IsAutostartEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunRegistryKey, false))
                {
                    if (key == null) return false;
                    return key.GetValue(AppName) != null;
                }
            }
            catch
            {
                return false;
            }
        }

        public static async void InjectBridgeScripts(CoreWebView2 webView)
        {
            string bridgeScript = @"
            (() => {
                const origGetReader = ReadableStream.prototype.getReader;
                ReadableStream.prototype.getReader = function(...args) {
                    const reader = origGetReader.apply(this, args);
                    const origRead = reader.read;
                    let isNotificationStream = false;
                    let buffer = '';
                    const decoder = new TextDecoder();

                    reader.read = async function() {
                        const result = await origRead.apply(this, arguments);
                        const { done, value } = result;

                        if (!done && value) {
                            const chunk = decoder.decode(value, { stream: true });
                            if (!isNotificationStream && (chunk.includes('data:') || chunk.includes('notifications'))) {
                                isNotificationStream = true;
                            }

                            if (isNotificationStream) {
                                buffer += chunk;
                                const lines = buffer.split('\n');
                                buffer = lines.pop();

                                for (const line of lines) {
                                    const trimmed = line.trim();
                                    if (trimmed.startsWith('data:')) {
                                        const raw = trimmed.replace(/^data:\s*/, '');
                                        try {
                                            const json = JSON.parse(raw);
                                            if (window.chrome && window.chrome.webview) {
                                                window.chrome.webview.postMessage(json);
                                            }
                                        } catch (e) {}
                                    }
                                }
                            }
                        }
                        return result;
                    };
                    return reader;
                };
            })();";

            await webView.AddScriptToExecuteOnDocumentCreatedAsync(bridgeScript);
        }

        public static async void ShowWelcomeModal(CoreWebView2 webView)
        {
            string modalScript = @"
            (() => {
                if (document.getElementById('itd-welcome-popup')) return;

                const overlay = document.createElement('div');
                overlay.id = 'itd-welcome-popup';
                overlay.style.cssText = `
                    position: fixed;
                    top: 0; left: 0; width: 100vw; height: 100vh;
                    background: rgba(0, 0, 0, 0.6);
                    backdrop-filter: blur(6px);
                    -webkit-backdrop-filter: blur(6px);
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    z-index: 1000000;
                    padding: 16px;
                    box-sizing: border-box;
                    animation: itdFadeIn 0.25s ease-out;
                `;

                const modal = document.createElement('div');
                modal.style.cssText = `
                    background: var(--modal-bg, #111111);
                    color: var(--text-primary, #f5f5f5);
                    border: 1px solid var(--border-color, rgba(255, 255, 255, 0.15));
                    border-radius: var(--radius-md, 12px);
                    box-shadow: var(--shadow-elevated, 0 8px 32px rgba(0, 0, 0, 0.4));
                    width: 100%;
                    max-width: 480px;
                    padding: 24px;
                    box-sizing: border-box;
                    display: flex;
                    flex-direction: column;
                    gap: 16px;
                    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
                `;

                modal.innerHTML = `
                    <div style='display: flex; flex-direction: column; gap: 8px;'>
                        <h2 style='margin: 0; font-size: 20px; font-weight: 700; color: var(--text-primary);'>
                            Приветствуем в ИТД Desktop! 👋
                        </h2>
                        <p style='margin: 0; font-size: 14px; line-height: 1.5; color: var(--text-secondary);'>
                            Спасибо, что скачали наш клиент!
                        </p>
                    </div>

                    <div style='background: var(--block-bg-secondary, rgba(255,255,255,0.05)); border: 1px solid var(--border-subtle); padding: 14px; border-radius: 8px; font-size: 14px; line-height: 1.5;'>
                        В приложении работает мгновенная доставка уведомлений прямо в систему Windows. Чтобы не пропускать реакции и комментарии, клиенту требуется запуск вместе с системой.
                    </div>

                    <div style='display: flex; gap: 10px; margin-top: 4px;'>
                        <button id='itd-btn-yes' style='
                            flex: 1;
                            background: var(--btn-primary-bg, #ffffff);
                            color: var(--btn-primary-text, #000000);
                            border: none;
                            padding: 10px 14px;
                            border-radius: 6px;
                            font-size: 14px;
                            font-weight: 600;
                            cursor: pointer;
                        '>Да, включить автозагрузку</button>

                        <button id='itd-btn-no' style='
                            flex: 1;
                            background: transparent;
                            color: var(--text-primary, #ffffff);
                            border: 1px solid var(--border-color, rgba(255, 255, 255, 0.2));
                            padding: 10px 14px;
                            border-radius: 6px;
                            font-size: 14px;
                            font-weight: 500;
                            cursor: pointer;
                        '>Нет, не надо</button>
                    </div>

                    <div style='display: flex; flex-direction: column; gap: 8px; border-top: 1px solid var(--border-subtle); padding-top: 12px;'>
                        <div style='font-size: 11px; line-height: 1.4; color: var(--text-secondary); text-align: center;'>
                            Данное клиентское приложение ""ИТД Desktop"" является неофициальным клиентом для социальной сети ИТД. Официальная поддержка от ООО «НОУКИ» не предоставляется.
                        </div>
                        <div style='font-size: 11px; line-height: 1.4; color: var(--text-secondary); text-align: center;'>
                            Приложение не включает автозагрузку без вашего ведома. При подозрениях можно 
                            <a href='https://github.com/xachapyri-dev/itd_Desktop' target='_blank' style='color: var(--link-color, #6a9fd4); text-decoration: underline;'>посмотреть открытый исходный код</a>.
                        </div>
                    </div>
                `;

                overlay.appendChild(modal);
                document.body.appendChild(overlay);

                document.getElementById('itd-btn-yes').onclick = () => {
                    overlay.remove();
                    window.chrome.webview.postMessage({ itdAction: 'enable_autostart' });
                };

                document.getElementById('itd-btn-no').onclick = () => {
                    overlay.remove();
                    window.chrome.webview.postMessage({ itdAction: 'dismiss_welcome' });
                };
            })();";

            await webView.ExecuteScriptAsync(modalScript);
        }

        public static void HandleWebMessage(string jsonString, NotifyIcon trayIcon)
        {
            try
            {
                var data = JObject.Parse(jsonString);

                string action = data["itdAction"]?.ToString();
                if (!string.IsNullOrEmpty(action))
                {
                    if (action == "enable_autostart")
                    {
                        SetAutostart(true);
                        trayIcon?.ShowBalloonTip(3000, "ИТД Desktop", "Автозагрузка успешно включена!", ToolTipIcon.Info);
                    }

                    Properties.Settings.Default.FirstRunClient = false;
                    Properties.Settings.Default.Save();
                    return;
                }

                string type = data["type"]?.ToString();
                if (string.IsNullOrEmpty(type)) return;

                string author = data["actor"]?["displayName"]?.ToString()
                             ?? data["actor"]?["username"]?.ToString()
                             ?? "Пользователь";
                string preview = data["preview"]?.ToString();

                string title = "ИТД Desktop";
                string message = "";

                switch (type)
                {
                    case "like":
                        title = "Новая реакция";
                        message = string.IsNullOrEmpty(preview) ? $"{author} оценил(а) ваш пост" : $"{author} оценил(а) ваш пост: \"{preview}\"";
                        break;
                    case "comment":
                        title = "Новый комментарий";
                        message = string.IsNullOrEmpty(preview) ? $"{author} оставил(а) комментарий" : $"{author}: \"{preview}\"";
                        break;
                    case "comment_like":
                        title = "Реакция на комментарий";
                        message = string.IsNullOrEmpty(preview) ? $"{author} оценил(а) ваш комментарий" : $"{author} оценил(а) комментарий: \"{preview}\"";
                        break;
                    case "reply":
                        title = "Ответ на комментарий";
                        message = string.IsNullOrEmpty(preview) ? $"{author} ответил(а) на ваш комментарий" : $"{author} ответил(а): \"{preview}\"";
                        break;
                    case "mention":
                        title = "Новое упоминание";
                        message = string.IsNullOrEmpty(preview) ? $"{author} упомянул(а) вас" : $"{author} упомянул(а) вас: \"{preview}\"";
                        break;
                    case "follow":
                        title = "Новый подписчик";
                        message = $"{author} подписался(-ась) на вас";
                        break;
                    case "repost":
                        title = "Репост";
                        message = $"{author} сделал(а) репост вашей записи";
                        break;
                    case "wall_post":
                        title = "Запись на стене";
                        message = string.IsNullOrEmpty(preview) ? $"{author} опубликовал(а) запись на вашей стене" : $"{author} на стене: \"{preview}\"";
                        break;
                    default:
                        return;
                }

                if (message.Length > 120)
                    message = message.Substring(0, 117) + "...";

                trayIcon?.ShowBalloonTip(4000, title, message, ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка обработки WebMessage: " + ex.Message);
            }
        }
    }
}