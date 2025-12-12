global using Page = System.Windows.Controls.Page;
global using MessageBox = System.Windows.MessageBox;

using MultiGameLauncher.Views.Pages;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace MultiGameLauncher
{
    class Tools
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
    }

    public class Variables
    {
        public class Launches
        {
            public string Name { get; set; }
            public string MainTitle { get; set; }
            public string SubTitle { get; set; }
            public string BackgroundImagepath { get; set; }
            public string Launchpath { get; set; }

        }
    }


    public class JsonConfig
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
}
