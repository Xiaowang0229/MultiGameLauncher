global using Application = System.Windows.Application;
global using MessageBox = System.Windows.MessageBox;
global using Page = System.Windows.Controls.Page;
using MultiGameLauncher.Views.Pages;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;


namespace MultiGameLauncher
{
    public class Variables
    {
        public static string Version = "Indev 251213";
    }
    public class Tools
    {



        public static ImageSource ApplicationLogo;
        //读图片函数
        public static ImageSource ConvertByteArrayToImageSource(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0) return null;

            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // 确保立即加载数据
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // 可选：跨线程使用时冻结对象
                return bitmapImage;
            }
        }

        public static string GetColorName(Color color)
        {
            // 忽略 Alpha（透明度），因为命名颜色 Alpha 总是 255
            foreach (PropertyInfo prop in typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (prop.PropertyType == typeof(Color))
                {
                    Color namedColor = (Color)prop.GetValue(null);
                    if (namedColor.R == color.R &&
                        namedColor.G == color.G &&
                        namedColor.B == color.B &&
                        namedColor.A == color.A) // 或忽略 A：namedColor.A == 255 && color.A == 255
                    {
                        return prop.Name;
                    }
                }
            }
            return color.ToString(); // 如果不是命名颜色，返回 #AARRGGBB
        }
    }

    


    public class LaunchConfig
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

    public class Config
    {
        public class Personality
        {
            //颜色
            public string ThemeColor { get; set; }

            //主题
            public string IsDarkModeEnable { get; set; }

            //底图（除主页外）
            public string DefaultBackgroundImagePath { get; set; }
        }
        public class Configs
        {
            //自动更新
            public bool StartUpCheckUpdate { get; set; }

            //开机自启
            public bool AutoStartUp { get; set; }
        }
    }
}
