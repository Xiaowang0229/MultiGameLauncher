using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Windowing;
using Ookii.Dialogs.Wpf;
using RocketLauncherRemake.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Xiaowang0229.JsonLibrary;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;




namespace RocketLauncherRemake
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.ShutdownRequested += App_OnClosing;
                desktop.MainWindow = OnLaunch();
                
            }

            base.OnFrameworkInitializationCompleted();
        }

        private AppWindow OnLaunch()
        {
            RegisterGlobalExceptionHandlers();
            if (!File.Exists(Variables.Configpath))
            {
                var config2 = new MainConfig
                {
                    Username = "Administrator",
                    StartUpCheckUpdate = true,
                    LaunchWithMinize = true,
                    GameInfos = new List<LaunchConfig>()
                };
                config2.WriteConfig();
            }
            if(File.Exists($"{Environment.CurrentDirectory}\\Config.json"))
            {
                var j = Json.ReadJson<MainConfig>($"{Environment.CurrentDirectory}\\Config.json");
                j.WriteConfig();
                File.Delete($"{Environment.CurrentDirectory}\\Config.json");
                WindowHelper.Restart();
            }
            

            
 



            var config = JsonConfig.ReadConfig();

            //读取逻辑

            if (!Directory.Exists(Environment.CurrentDirectory + $"\\Backgrounds"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + $"\\Backgrounds");
            }

            for (int i = 0; i < config.GameInfos.Count; i++)
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


            if (config.StartUpCheckUpdate)
            {
                Update.CheckUpdate();

            }
            TaskBar.IntializeTaskbar();
            return Variables._MainWindow;
        }

        public static void RegisterGlobalExceptionHandlers()
        {
            // 捕获 UI 线程未处理的异常
            Avalonia.Threading.Dispatcher.UIThread.UnhandledException += (s, e) =>
            {


                WindowHelper.ShowExceptionDialog($"{e.Exception}");
                

            };

            // 捕获非 UI 线程未处理的异常
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {


                WindowHelper.ShowExceptionDialog($"{e.ExceptionObject}");

            };

            // 捕获 Task 线程未处理的异常
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {

                WindowHelper.ShowExceptionDialog($"{e.Exception}");

            };
        }

        

        private void App_OnClosing(object? sender,ShutdownRequestedEventArgs e)
        {
            TaskBar.KillTaskBar();
        }

        public static void ChangeThemeColor(Color color)
        {
            var faTheme = Application.Current.Styles
    .OfType<FluentAvaloniaTheme>()
    .FirstOrDefault();

            faTheme.CustomAccentColor = color;
        }
    }
}