using ControlzEx.Theming;
using HuaZi.Library.Json;
using MahApps.Metro.Controls;
using MultiGameLauncher.Views.Windows;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Windows;
using Unosquare.FFME;

namespace MultiGameLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainConfig config;
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            Library.FFmpegDirectory = Environment.CurrentDirectory + "\\FFmpeg";
            if (!File.Exists(Variables.Configpath))
            {
                Tools.InitalizeConfig();
            }
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            //读取逻辑
            Tools.ApplicationLogo = Tools.ConvertByteArrayToImageSource(ApplicationResources.ApplicationIcon);
            if(!Directory.Exists(Environment.CurrentDirectory + $"\\Backgrounds"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + $"\\Backgrounds");
            }
            

            ThemeManager.Current.ChangeTheme(Current,config.ThemeMode+"."+config.ThemeColor);

            if(config.ChangeThemeWithSystem)
            {
                Tools.StartThemeMonitoring();
            }

            


            //主加载逻辑
            if(config.OOBEStatus && config.GameInfos.Count != 0)
            {
                var win = new MainWindow();
                win.Show();
            }
            else if (config.GameInfos.Count == 0 && config.OOBEStatus == true)
            {
                var win = new OOBEWindow(true);
                win.Show();
            }
            else
            {
                var win = new OOBEWindow();
                win.Show();
            }
            











            //后加载逻辑
            if (config.StartUpCheckUpdate)
            {
                try
                {

                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                    var content = await client.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestVersion");
                    if (content != Variables.Version)
                    {
                        using var log = new HttpClient();
                        log.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                        var Onlinelog = await log.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestLog");
                        using var link = new HttpClient();
                        client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                        var OnlineLink = await client.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestLink");
                        MetroWindow win2 = new UpdatePrepareWindow(Variables.Version, content, Onlinelog, OnlineLink);
                        win2.ShowDialog();

                    }
                }
                catch { }

            }

        }
    }

}
