global using Application = System.Windows.Application;
global using MessageBox = System.Windows.MessageBox;
global using Page = System.Windows.Controls.Page;
using ControlzEx.Theming;
using HuaZi.Library.Json;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Color = System.Windows.Media.Color;


namespace MultiGameLauncher
{
    public class Variables //变量集
    {
        public readonly static string Version = "Indev 251214\n";
        public readonly static string Configpath = Environment.CurrentDirectory + @"\Config.json";
        public readonly static string VersionLog = $"[{Version.Substring(0, Version.Length - 1)} 版本日志]\r\n-1.xxx\r\n-2.xxx\r\n-3.xxx";
    }

    
    public class Tools //工具集
    {
        public static ImageSource ApplicationLogo;
        //读图片函数
        public static void Restart()
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Environment.Exit(0);
        }
        public static ImageSource ConvertByteArrayToImageSource(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0) return null;

            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        public static string GetColorName(Color color)
        {
            foreach (PropertyInfo prop in typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (prop.PropertyType == typeof(Color))
                {
                    Color namedColor = (Color)prop.GetValue(null);
                    if (namedColor.R == color.R &&
                        namedColor.G == color.G &&
                        namedColor.B == color.B &&
                        namedColor.A == color.A) 
                    {
                        return prop.Name;
                    }
                }
            }
            return color.ToString(); 
        }

        public static bool ConvertToPngAndSave(byte[] imageBytes, string savePath)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                throw new ArgumentException("byte[] 不能为空");

            try
            {
                using (MemoryStream ms = new MemoryStream(imageBytes))
                using (Image image = Image.FromStream(ms))
                {
                    image.Save(savePath, ImageFormat.Png);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("转换失败: " + ex.Message);
                return false;
            }
        }

        public static BitmapImage LoadImageFromPath(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                return null;
            }

            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.EndInit();
                bitmap.Freeze(); 

                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载图片失败: {imagePath}，错误: {ex.Message}");
                return null;
            }
        }

        public static bool AppsUseLightTheme()
        {
            const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string ValueName = "AppsUseLightTheme";

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
                {
                    var value = key?.GetValue(ValueName);
                    if (value is int intValue)
                    {
                        return intValue > 0; 
                    }
                }
            }
            catch
            {
                
            }

            return true; 
        }
        public static void StartThemeMonitoring()
        {

            bool isLight = AppsUseLightTheme();
            string theme = isLight ? "Light" : "Dark";
            ThemeManager.Current.ChangeTheme(Application.Current, theme + "." + ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
            var config = Json.ReadJson<MainConfig>(Variables.Configpath);
            config.ThemeMode = theme;
            Json.WriteJson(Variables.Configpath, config);
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }
        public static void StopThemeMonitoring()
        {
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
        }
        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.VisualStyle ||
                e.Category == UserPreferenceCategory.Color ||
                e.Category == UserPreferenceCategory.General)
            {
                OutputCurrentTheme();
            }
        }

        private static void OutputCurrentTheme()
        {
            bool isLight = AppsUseLightTheme();
            string theme = isLight ? "Light" : "Dark";
            ThemeManager.Current.ChangeTheme(Application.Current, theme + "." + ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
            var config = Json.ReadJson<MainConfig>(Variables.Configpath);
            config.ThemeMode = theme;
            Json.WriteJson(Variables.Configpath, config);
        }

        public static void InitalizeConfig()
        {
            var config = new MainConfig
            {
                Username = "Administrator",
                ThemeColor = "Blue",
                ThemeMode = "Light",
                AutoStartUp = false,
                StartUpCheckUpdate = true,
                ChangeThemeWithSystem = false
            };
            Json.WriteJson(Variables.Configpath,config);
            ConvertToPngAndSave(ApplicationResources.UserIcon, Environment.CurrentDirectory+@"\Head.png");
        }
    }

    public class LaunchConfig //游戏配置项
    {
        public class Index
        {
            //[JsonProperty("1")]
            public Dictionary<string, Launches> launches { get; set; }

        }

        public class Launches
        {
            public string Name { get; set; }
            public string MainTitle { get; set; }
            public string SubTitle { get; set; }
            public string BackgroundImagepath { get; set; }
            public string Launchpath { get; set; }

        }
    }

    public class MainConfig //主体配置项
    {
        //用户名
        public string Username { get; set; }

        //此处不再写头像位置，因为使能头像时已经复制
        //public string UserImage { get; set; }

        //主题(颜色)
        public string ThemeColor { get; set; }

        //主题(深浅)
        public string ThemeMode { get; set; }

        //自动更新
        public bool StartUpCheckUpdate { get; set; }

        //开机自启
        public bool AutoStartUp { get; set; }

        //主题跟随系统
        public bool ChangeThemeWithSystem { get; set; }
    }

}
