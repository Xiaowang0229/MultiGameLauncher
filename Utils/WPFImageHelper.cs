using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Xiaowang0229.ImageLibrary.WPF;

public class Image
{
    public static BitmapImage ConvertByteArrayToImageSource(byte[] imageBytes)
    {
        if (imageBytes == null || imageBytes.Length == 0)
        {
            return null;
        }

        using MemoryStream streamSource = new MemoryStream(imageBytes);
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = streamSource;
        bitmapImage.EndInit();
        bitmapImage.Freeze();
        return bitmapImage;
    }

    public static bool ConvertToPngAndSave(byte[] imageBytes, string savePath)
    {
        if (imageBytes == null || imageBytes.Length == 0)
        {
            throw new ArgumentException("byte[] 不能为空");
        }

        try
        {
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                using System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
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
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            bitmapImage.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }
        catch (Exception ex)
        {
            Console.WriteLine("加载图片失败: " + imagePath + "，错误: " + ex.Message);
            return null;
        }
    }
}