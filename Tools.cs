global using Application = System.Windows.Application;
global using MessageBox = System.Windows.MessageBox;
global using Page = System.Windows.Controls.Page;
using HuaZi.Library.Json;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;


namespace MultiGameLauncher
{
    public class Variables //变量集
    {
        public readonly static string Version = "Indev 251214";
        public readonly static string Configpath = Environment.CurrentDirectory + @"\Config.json";
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
