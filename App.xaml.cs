using ControlzEx.Theming;
using HuaZi.Library.Json;
using MultiGameLauncher.Views.Windows;
using System.Diagnostics;
using System.IO;
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

            Tools.RegisterGlobalExceptionHandlers();
            Variables.VersionLog = Tools.ReadEmbeddedMarkdown("MultiGameLauncher.LocalLog.md");
            Variables.EULAString = Tools.ReadEmbeddedMarkdown("MultiGameLauncher.EULA.md");
            
            if (!File.Exists(Variables.Configpath))
            {
                Tools.InitalizeConfig();
            }
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            if (config.OOBEStatus != true)
            {
                Tools.InitalizeConfig();
            }
            //读取逻辑

            if (!Directory.Exists(Environment.CurrentDirectory + $"\\Backgrounds"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + $"\\Backgrounds");
            }

            if (!File.Exists($"{Environment.CurrentDirectory}\\Alarm.png"))
            {
                Tools.ConvertToPngAndSave(ApplicationResources.Alarm, $"{Environment.CurrentDirectory}\\Alarm.png");
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

            ThemeManager.Current.ChangeTheme(Current, config.ThemeMode + "." + config.ThemeColor);

            if (config.ChangeThemeWithSystem)
            {
                Tools.StartThemeMonitoring();
            }




            //主加载逻辑
            if (config.OOBEStatus && config.GameInfos.Count != 0)
            {
                Library.FFmpegDirectory = Environment.CurrentDirectory + "\\FFmpeg";
                Library.LoadFFmpeg();
                /*outerloop: foreach (var folder in subFolderNames)
                {
                    foreach (var hashcode in config.GameInfos)
                    {
                        if (hashcode.HashCode == folder)
                        {
                            
                            continue outerloop;
                        }
                    }
                    
                }*/


                var win = new MainWindow();
                win.Show();

                Tools.IntializeTaskbar();
                Variables.RealTimeAlarm.Interval = TimeSpan.FromSeconds(1);
                Variables.RealTimeAlarm.Tick += Tools.AlarmTick;
                Variables.RealTimeAlarm.Start();


            }
            else if (config.GameInfos.Count == 0 && config.OOBEStatus == true)
            {
                Library.FFmpegDirectory = Environment.CurrentDirectory + "\\FFmpeg";
                Library.LoadFFmpeg();
                var win = new OOBEWindow(true);
                win.Show();
            }
            else
            {
                Tools.InitalizeConfig();
                var win = new OOBEWindow();
                win.Show();
            }

            if (config.StartUpCheckUpdate)
            {
                Tools.CheckUpdate();

            }


        }




    }

}
