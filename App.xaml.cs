using ControlzEx.Theming;
using FFmpeg.AutoGen;
using HuaZi.Library.Json;
using MahApps.Metro.Controls;
using MultiGameLauncher.Views.Windows;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Threading;
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
            RegisterGlobalExceptionHandlers();
            Variables.ShowVersion = Variables.Version.Substring(Variables.Version.Length - 1);
            Variables.VersionLog = Tools.ReadEmbeddedMarkdown("MultiGameLauncher.LocalLog.md");
            Library.FFmpegDirectory = Environment.CurrentDirectory + "\\FFmpeg";
            Library.LoadFFmpeg();
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
            
            for(int i = 0;i<config.GameInfos.Count;i++)
            {
                int index = i;
                var proc = new Process();
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = config.GameInfos[i].Launchpath,
                    Arguments = config.GameInfos[i].Arguments,
                    UseShellExecute = true
                };
                Variables.GameProcess.Add(proc);
                Variables.PlayingTimeintList.Add(0);
                Variables.GameProcessStatus.Add(false);

                var dt = new DispatcherTimer();
                dt.Interval = TimeSpan.FromMinutes(1);

                dt.Tick += async (s, e) =>
                {
                    Variables.PlayingTimeintList[index] += 1;
                };
                Variables.PlayingTimeRecorder.Add(dt);
                
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
                Tools.IntializeTaskbar();
                //return;
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
                        //await Task.Delay();
                        MetroWindow win2 = new UpdatePrepareWindow(Variables.Version, content, Onlinelog, OnlineLink);
                        win2.ShowDialog();

                    }
                }
                catch { }

            }

        }

        private void RegisterGlobalExceptionHandlers()
        {
            // 捕获 UI 线程未处理的异常
            this.DispatcherUnhandledException += UnhandledDispatherException;

            // 捕获非 UI 线程未处理的异常
            AppDomain.CurrentDomain.UnhandledException += UnhandledDomainException;

            // 捕获 Task 线程未处理的异常
            TaskScheduler.UnobservedTaskException += UnhandledException;
        }

        private void UnhandledException(object sender,  UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show($"程序内部发生错误：{e.Exception}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(0);
        }
        private void UnhandledDomainException(object sender,  UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"程序内部发生错误：{e.ExceptionObject}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(0);
        }
        private void UnhandledDispatherException(object sender,  DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"程序内部发生错误：{e.Exception}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(0);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            

        }
    }

}
