using Krypton.Toolkit;
using System;
using System.Collections.Generic;

namespace ИТД_Desktop.UI.Aero_UI
{
    public static class ThemeHelper
    {
        private static PaletteMode _currentTheme = PaletteMode.Office2007Blue;
        private static Dictionary<KryptonForm, KryptonManager> _formManagers = new Dictionary<KryptonForm, KryptonManager>();
        public static void Initialize()
        {
            string savedTheme = Properties.Settings.Default.ThemeName;
            if (!string.IsNullOrEmpty(savedTheme) && Enum.TryParse(savedTheme, out PaletteMode theme))
            {
                _currentTheme = theme;
            }
        }
        public static void ApplyTheme(PaletteMode theme)
        {
            _currentTheme = theme;
            foreach (var kvp in _formManagers)
            {
                if (kvp.Value != null && !kvp.Key.IsDisposed)
                {
                    kvp.Value.GlobalPaletteMode = theme;
                }
            }
            Properties.Settings.Default.ThemeName = theme.ToString();
            Properties.Settings.Default.Save();
        }
        public static void RegisterForm(KryptonForm form, KryptonManager manager)
        {
            if (!_formManagers.ContainsKey(form))
            {
                _formManagers[form] = manager;
                form.FormClosed += (s, e) => _formManagers.Remove(form);
                if (manager != null)
                {
                    manager.GlobalPaletteMode = _currentTheme;
                }
            }
        }
        public static PaletteMode GetCurrentTheme()
        {
            return _currentTheme;
        }
        public static Dictionary<string, PaletteMode> GetAllThemes()
        {
            return new Dictionary<string, PaletteMode>
            {
                { "Office 2007 - Blue", PaletteMode.Office2007Blue },
                { "Office 2007 - Blue (Темный)", PaletteMode.Office2007BlueDarkMode },
                { "Office 2007 - Blue (Светлый)", PaletteMode.Office2007BlueLightMode },
                { "Office 2007 - Silver", PaletteMode.Office2007Silver },
                { "Office 2007 - Silver (Темный)", PaletteMode.Office2007SilverDarkMode },
                { "Office 2007 - Silver (Светлый)", PaletteMode.Office2007SilverLightMode },
                { "Office 2007 - White", PaletteMode.Office2007White },
                { "Office 2007 - Black", PaletteMode.Office2007Black },
                { "Office 2007 - Black (Темный)", PaletteMode.Office2007BlackDarkMode },
                { "Office 2010 - Blue", PaletteMode.Office2010Blue },
                { "Office 2010 - Blue (Темный)", PaletteMode.Office2010BlueDarkMode },
                { "Office 2010 - Blue (Светлый)", PaletteMode.Office2010BlueLightMode },
                { "Office 2010 - Silver", PaletteMode.Office2010Silver },
                { "Office 2010 - Silver (Темный)", PaletteMode.Office2010SilverDarkMode },
                { "Office 2010 - Silver (Светлый)", PaletteMode.Office2010SilverLightMode },
                { "Office 2010 - White", PaletteMode.Office2010White },
                { "Office 2010 - Black", PaletteMode.Office2010Black },
                { "Office 2010 - Black (Темный)", PaletteMode.Office2010BlackDarkMode },
                { "Office 2013 - White", PaletteMode.Office2013White },
                { "Sparkle - Blue", PaletteMode.SparkleBlue },
                { "Sparkle - Blue (Темный)", PaletteMode.SparkleBlueDarkMode },
                { "Sparkle - Blue (Светлый)", PaletteMode.SparkleBlueLightMode },
                { "Sparkle - Orange", PaletteMode.SparkleOrange },
                { "Sparkle - Orange (Темный)", PaletteMode.SparkleOrangeDarkMode },
                { "Sparkle - Orange (Светлый)", PaletteMode.SparkleOrangeLightMode },
                { "Sparkle - Purple", PaletteMode.SparklePurple },
                { "Sparkle - Purple (Темный)", PaletteMode.SparklePurpleDarkMode },
                { "Sparkle - Purple (Светлый)", PaletteMode.SparklePurpleLightMode },
                { "Microsoft 365 - Blue", PaletteMode.Microsoft365Blue },
                { "Microsoft 365 - Blue (Темный)", PaletteMode.Microsoft365BlueDarkMode },
                { "Microsoft 365 - Blue (Светлый)", PaletteMode.Microsoft365BlueLightMode },
                { "Microsoft 365 - Silver", PaletteMode.Microsoft365Silver },
                { "Microsoft 365 - Silver (Темный)", PaletteMode.Microsoft365SilverDarkMode },
                { "Microsoft 365 - Silver (Светлый)", PaletteMode.Microsoft365SilverLightMode },
                { "Microsoft 365 - White", PaletteMode.Microsoft365White },
                { "Microsoft 365 - Black", PaletteMode.Microsoft365Black },
                { "Microsoft 365 - Black (Темный)", PaletteMode.Microsoft365BlackDarkMode },
                { "Microsoft 365 - Black (Альт)", PaletteMode.Microsoft365BlackDarkModeAlternate },
                { "Visual Studio 2010 (2007)", PaletteMode.VisualStudio2010Render2007 },
                { "Visual Studio 2010 (2010)", PaletteMode.VisualStudio2010Render2010 },
                { "Visual Studio 2010 (2013)", PaletteMode.VisualStudio2010Render2013 },
                { "Visual Studio 2010 (365)", PaletteMode.VisualStudio2010Render365 },
                { "Professional - System", PaletteMode.ProfessionalSystem },
                { "Professional - Office 2003", PaletteMode.ProfessionalOffice2003 },
                { "Material - Light", PaletteMode.MaterialLight },
                { "Material - Dark", PaletteMode.MaterialDark },
                { "Material - Light (Ripple)", PaletteMode.MaterialLightRipple },
                { "Material - Dark (Ripple)", PaletteMode.MaterialDarkRipple }
            };
        }
    }
}