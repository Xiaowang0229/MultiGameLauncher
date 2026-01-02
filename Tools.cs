global using Application = System.Windows.Application;
global using MessageBox = System.Windows.MessageBox;
global using Page = System.Windows.Controls.Page;
using ControlzEx.Theming;
using FFmpeg.AutoGen;
using Hardcodet.Wpf.TaskbarNotification;
using HuaZi.Library.Json;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using MultiGameLauncher.Views.Pages;
using MultiGameLauncher.Views.Windows;
using NAudio;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TsudaKageyu;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Color = System.Windows.Media.Color;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Image = System.Drawing.Image;
using MenuItem = System.Windows.Controls.MenuItem;


namespace MultiGameLauncher
{
    public static class Variables //变量集
    {
        public readonly static string Version = "Release 1.4.1.0\n";
        public static string ShowVersion = Version.Substring(0, Version.Length - 1);
        public static string ApplicationTitle = $"Rocket Launcher {ShowVersion}";
        public readonly static string Configpath = Environment.CurrentDirectory + @"\Config.json";
        public static List<Process> GameProcess = new List<Process>();
        public static TaskbarIcon RootTaskBarIcon;
        public static ContextMenu TaskBarMenu = new ContextMenu();
        public static List<bool> GameProcessStatus = new List<bool>();
        public static List<DispatcherTimer> PlayingTimeRecorder = new List<DispatcherTimer>();
        public static List<long> PlayingTimeintList = new List<long>();
        public static List<string> MusicList = new List<string>();
        public static string VersionLog;
        public static string EULAString;
        public static bool MainWindowHideStatus = false;
        public static IWavePlayer RootMusicPlayer=new WaveOutEvent();
        public static  bool? UsingRealTimeAlarm = null;
        public static TimeSpan AlarmTime = new TimeSpan();
        public static string AlarmRealTime;
        public static DispatcherTimer RealTimeAlarm = new DispatcherTimer();
        public static CancellationTokenSource AlarmCTS = new CancellationTokenSource();
        public static CancellationTokenSource LaunchCTS = new CancellationTokenSource();
    }


    public static class Tools //工具集
    {
        //public static Process Process = new();
        public static FrameworkElement OldPage = null;
        public static ImageSource ApplicationLogo;

        


        //函数
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
        public static string RandomHashGenerate(int byteLength = 16)
        {
            byte[] bytes = new byte[byteLength];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToHexString(bytes);
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
            catch { }

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
                OOBEStatus = false,
                Username = "Administrator",
                ThemeColor = "Blue",
                ThemeMode = "Light",
                AutoStartUp = false,
                StartUpCheckUpdate = true,
                ChangeThemeWithSystem = false,
                PlayMusicStarted = false,
                GameInfos = new List<LaunchConfig>(),
                MusicInfos = new List<MusicConfig>()
            };
            Json.WriteJson(Variables.Configpath, config);
            //ConvertToPngAndSave(ApplicationResources.UserIcon, Environment.CurrentDirectory+@"\Head.png");
        }
        public static string ReadEmbeddedMarkdown(string resourceName)
        {
            // 获取当前执行的程序集
            var assembly = Assembly.GetExecutingAssembly();

            // 获取嵌入资源的流
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Resource {resourceName} not found.");

                // 使用 StreamReader 读取流中的文本
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public static int FindHashcodeinGameinfosint(MainConfig config, string hashcode)
        {
            for (int i = 0; i < config.GameInfos.Count; i++)
            {
                if (config.GameInfos[i].HashCode == hashcode)
                {
                    return i;
                }
            }
            return 0;
        }
        public static void IntializeTaskbar()
        {
            MainConfig config = new MainConfig();
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            //本体初始化
            Variables.RootTaskBarIcon = new TaskbarIcon();
            Variables.RootTaskBarIcon.IconSource = ConvertByteArrayToImageSource(ApplicationResources.ApplicationIcon);
            Variables.RootTaskBarIcon.ToolTipText = $"Rocket Launcher 主程序";

            //列表项初始化
            var tbcm = new System.Windows.Controls.ContextMenu();
            var OpenMainWindowItem = new MenuItem { Header = "显示主窗口" };
            var ControlGameProcess = new MenuItem { Header = "快捷管理游戏" };
            var SettingsItem = new MenuItem { Header = "打开设置页" };
            var ExitApplicationItem = new MenuItem { Header = "退出主程序" };



            tbcm.Items.Add(OpenMainWindowItem);


            //Variables.RootTaskBarIcon.ContextMenu.Items.Add(ForceQuitGameItem);
            tbcm.Items.Add(new Separator());
            tbcm.Items.Add(SettingsItem);
            tbcm.Items.Add(ExitApplicationItem);



            //绑定事件
            OpenMainWindowItem.Click += (s, e) =>
            {
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                Page currentPage = win.RootFrame.Content as Page;
                win.WindowState = WindowState.Normal;
                win.Show();
                if (currentPage is Launch launchpage)
                {
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                    {
                        launchpage.BackgroundImage.Visibility = Visibility.Hidden;
                        launchpage.BackgroundVideo.Visibility = Visibility.Visible;
                        launchpage.BackgroundVideo.Play();
                    }
                }
                win.Topmost = true;
                win.Topmost = false;
            };

            SettingsItem.Click += (s, e) =>
            {
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.Show();
                win.RootFrame.Navigate(new Settings());
            };
            ExitApplicationItem.Click += (s, e) =>
            {
                KillTaskBar();
                Environment.Exit(0);
            };

            Variables.RootTaskBarIcon.ContextMenu = tbcm;


            Variables.RootTaskBarIcon.TrayLeftMouseDown += (s, e) =>
            {
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                Page currentPage = win.RootFrame.Content as Page;

                
                if (Variables.MainWindowHideStatus)
                {
                    win.Show();
                    win.WindowState = WindowState.Normal;
                    Variables.MainWindowHideStatus = false;
                    win.Topmost = true;
                    win.Topmost = false;
                    if(currentPage is Launch launchpage)
                    {
                        if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                        {
                            launchpage.BackgroundImage.Visibility = Visibility.Hidden;
                            launchpage.BackgroundVideo.Visibility = Visibility.Visible;
                            launchpage.BackgroundVideo.Play();
                        }
                    }
                }
                else
                {
                    if (currentPage is Launch launchpage)
                    {
                        if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                        {
                            launchpage.BackgroundImage.Visibility = Visibility.Hidden;
                            launchpage.BackgroundVideo.Visibility = Visibility.Visible;
                            launchpage.BackgroundVideo.Pause();
                        }
                    }
                    win.Hide();
                    Variables.MainWindowHideStatus = true;
                }

            };

        }
        public static void KillTaskBar()
        {
            Variables.RootTaskBarIcon?.Dispose();
        }
        public static void InitializeTaskBarContentMenu()
        {
            //列表项初始化
            MainConfig config = new MainConfig();
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            var tbcm = new System.Windows.Controls.ContextMenu();
            var OpenMainWindowItem = new MenuItem { Header = "显示主窗口" };
            var ControlGameProcess = new MenuItem { Header = "快捷管理游戏" };
            var SettingsItem = new MenuItem { Header = "打开设置页" };
            var ExitApplicationItem = new MenuItem { Header = "退出主程序" };

            for (int i = 0; i < Variables.GameProcess.Count; i++)
            {
                if (Variables.GameProcessStatus[i] == true)
                {
                    int index = i;
                    var subitem = new MenuItem
                    {
                        Header = $"结束 {config.GameInfos[i].ShowName}",
                        //Header = $"结束 {Variables.GameProcess[i].ProcessName}",

                    };
                    subitem.Click += async (s, e) =>
                    {


                        Variables.GameProcess[index].Kill();
                        await Task.Delay(100);
                        InitializeTaskBarContentMenu();


                    };
                    ControlGameProcess.Items.Add(subitem);
                }
            }

            tbcm.Items.Add(OpenMainWindowItem);
            try
            {
                for (int i = 0; i < Variables.GameProcess.Count; i++)
                {
                    if (Variables.GameProcessStatus[i] == true)
                    {
                        tbcm.Items.Add(ControlGameProcess);
                        break;
                    }
                }
            }
            catch { }

            //Variables.RootTaskBarIcon.ContextMenu.Items.Add(ForceQuitGameItem);
            tbcm.Items.Add(new Separator());
            tbcm.Items.Add(SettingsItem);
            tbcm.Items.Add(ExitApplicationItem);




            Variables.RootTaskBarIcon.ContextMenu = tbcm;
            //绑定事件
            OpenMainWindowItem.Click += (s, e) =>
            {
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                Page currentPage = win.RootFrame.Content as Page;
                
                win.Show();
                win.WindowState = WindowState.Normal;
                if (currentPage is Launch launchpage)
                {
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                    {
                        launchpage.BackgroundImage.Visibility = Visibility.Hidden;
                        launchpage.BackgroundVideo.Visibility = Visibility.Visible;
                        launchpage.BackgroundVideo.Play();
                    }
                }
                win.Topmost = true;
                win.Topmost = false;
            };

            SettingsItem.Click += (s, e) =>
            {
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.Show();
                win.RootFrame.Navigate(new Settings());
            };
            ExitApplicationItem.Click += (s, e) =>
            {
                KillTaskBar();
                Environment.Exit(0);
            };


        }
        public async static void StartMonitingGameStatus(int index)
        {
            var config = Json.ReadJson<MainConfig>(Variables.Configpath);
            var proc = Variables.GameProcess[index];
            proc.Start();
            Variables.GameProcessStatus[index] = true;
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.Hide();
            Variables.MainWindowHideStatus = true;
            Tools.InitializeTaskBarContentMenu();
            Variables.PlayingTimeRecorder[index].Start();
            var toast = new ToastContentBuilder().AddText("程序已启动").AddText($"程序名：{config.GameInfos[index].ShowName}").AddText($"进程监测已开启").AddAppLogoOverride(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[index].HashCode}\\Icon.png"));
            toast.Show();




        }
        public static async Task WaitMonitingGameExitAsync(int index)
        {
            Variables.LaunchCTS.Cancel();
            Variables.LaunchCTS = new CancellationTokenSource();
            var config = Json.ReadJson<MainConfig>(Variables.Configpath);
            var proc = Variables.GameProcess[index];
            try
            {
                await proc.WaitForExitAsync(Variables.LaunchCTS.Token);
            }
            catch {
                return;
            }
            StopMonitingGameStatus(index);
            
        }
        private static void StopMonitingGameStatus(int index)
        {
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            var config = Json.ReadJson<MainConfig>(Variables.Configpath);
            Variables.PlayingTimeRecorder[index].Stop();
            var time = Variables.PlayingTimeintList[index];
            var toast0 = new ToastContentBuilder().AddText("程序已结束").AddText($"程序名：{config.GameInfos[index].ShowName}").AddText($"游戏时长：{time} 分钟,退出码：{Variables.GameProcess[index].ExitCode}").AddAppLogoOverride(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[index].HashCode}\\Icon.png"));
            toast0.Show();

            config.GameInfos[index].GamePlayedMinutes += time;
            Json.WriteJson(Variables.Configpath, config);

            Tools.InitializeTaskBarContentMenu();
            Variables.GameProcessStatus[index] = false;
            Variables.PlayingTimeintList[index] = 0;
            Variables.MainWindowHideStatus = false;
            Page currentPage = win.RootFrame.Content as Page;
            win.Show();
            win.WindowState = WindowState.Normal;
            if (currentPage is Launch launchpage)
            {
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                {
                    launchpage.BackgroundImage.Visibility = Visibility.Hidden;
                    launchpage.BackgroundVideo.Visibility = Visibility.Visible;
                    launchpage.BackgroundVideo.Play();
                }
            }
            win.Topmost = true;
            win.Topmost = false;

        }
        public static string? OpenInputWindow(string Title)
        {
            var win = new InputWindow(Title);
            if (win.ShowDialog() == true)
            {
                return win.Results;
            }
            else
            {
                return null;
            }
        }
        public static void ExtractExeIconToPng(string exePath, string pngPath)
        {
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
                throw new FileNotFoundException("EXE 文件不存在", exePath);

            if (string.IsNullOrWhiteSpace(pngPath))
                throw new ArgumentException("PNG 输出路径不能为空");

            // 使用 IconExtractor.dll 提取图标资源
            var extractor = new IconExtractor(exePath);

            if (extractor.Count == 0)
                throw new InvalidOperationException($"指定的 EXE 文件不包含任何图标: {exePath}");

            // 通常第 0 个图标就是主图标（最大、最清晰的那个）
            // GetIcon(0) 返回的是包含所有尺寸变体（包括 256x256）的完整 Icon 对象
            using (Icon fullIcon = extractor.GetIcon(0))
            {
                // 确保输出目录存在
                string dir = Path.GetDirectoryName(pngPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // 优先尝试提取 256x256 高清尺寸（现代 Windows 程序几乎都支持）
                try
                {
                    using (Icon largeIcon = new Icon(fullIcon, 256, 256))
                    using (Bitmap bmp = largeIcon.ToBitmap())  // 自动保留透明度
                    {
                        bmp.Save(pngPath, ImageFormat.Png);
                        return;  // 成功提取 256x256，直接返回
                    }
                }
                catch
                {
                    // 如果没有 256x256 尺寸，回退到图标自带的最大尺寸
                    // ToBitmap() 会选择最佳可用尺寸并保留 Alpha 通道
                }

                using (Bitmap bmp = fullIcon.ToBitmap())
                {
                    bmp.Save(pngPath, ImageFormat.Png);
                }
            }
        }
        public static void RefreshAllImageCaches(DependencyObject parent)
        {
            if (parent == null) return;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                



                if (child is System.Windows.Controls.Image imageControl)
                {
                    if (imageControl.Source is BitmapImage bitmapImage && bitmapImage.UriSource != null)
                    {
                        // 创建新 BitmapImage，忽略缓存并立即加载
                        BitmapImage newBitmap = new BitmapImage();
                        newBitmap.BeginInit();
                        newBitmap.UriSource = bitmapImage.UriSource;
                        newBitmap.CacheOption = BitmapCacheOption.OnLoad;           // 立即加载以释放文件锁（如需）
                        newBitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache; // 关键：忽略现有缓存
                        newBitmap.EndInit();

                        // 可选：Freeze 以提高性能（多线程安全）
                        if (newBitmap.CanFreeze)
                        {
                            newBitmap.Freeze();
                        }

                        imageControl.Source = newBitmap;
                    }
                }

                // 递归处理子控件
                RefreshAllImageCaches(child);
            }
        }
        public static bool CheckTime(string hhmm)
        {
            if(hhmm == DateTime.Now.ToString("HH:mm"))
            {
                return true;
            }

            return false;
        }
        public static void AlarmTick(object s,EventArgs e)
        {
            if(Variables.UsingRealTimeAlarm == true)
            {
                //MessageBox.Show(Variables.AlarmRealTime, DateTime.Now.ToString("HH:mm"));
                if (CheckTime(Variables.AlarmRealTime))
                {
                    Variables.UsingRealTimeAlarm = null;
                    var config = Json.ReadJson<MainConfig>(Variables.Configpath);
                    var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    var toast0 = new ToastContentBuilder().AddText("提示").AddText("时间已达到，请您尽快退出游戏！").AddAppLogoOverride(new Uri($"{Environment.CurrentDirectory}\\Alarm.png"));
                    toast0.Show();
                    Variables.RealTimeAlarm.Stop();
                    win.Show();
                    win.WindowState = WindowState.Normal;
                    Page currentPage = win.RootFrame.Content as Page;
                    if (currentPage is Launch launchpage)
                    {
                        if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                        {
                            launchpage.BackgroundImage.Visibility = Visibility.Hidden;
                            launchpage.BackgroundVideo.Visibility = Visibility.Visible;
                            launchpage.BackgroundVideo.Play();
                        }
                    }
                    win.Topmost = true;
                    win.Topmost = false;

                }
            }
        }
        public async static Task StartAlarmMonitingAsync()
        {
            try
            {
                await Task.Delay(Variables.AlarmTime, Variables.AlarmCTS.Token);
            }
            catch {
                return;
            }
            Variables.UsingRealTimeAlarm = null;
            var config = Json.ReadJson<MainConfig>(Variables.Configpath);
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            var toast0 = new ToastContentBuilder().AddText("提示").AddText("时间已达到，请您尽快退出游戏！").AddAppLogoOverride(new Uri($"{Environment.CurrentDirectory}\\Alarm.png"));
            toast0.Show();
            win.Show();
            win.WindowState = WindowState.Normal;
            Page currentPage = win.RootFrame.Content as Page;
            if (currentPage is Launch launchpage)
            {
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                {
                    launchpage.BackgroundImage.Visibility = Visibility.Hidden;
                    launchpage.BackgroundVideo.Visibility = Visibility.Visible;
                    launchpage.BackgroundVideo.Play();
                }
            }
            win.Topmost = true;
            win.Topmost = false;
        }
    }


    public class LaunchConfig
    {

        public string HashCode { get; set; }
        public string ShowName { get; set; }
        public string MainTitle { get; set; }
        public System.Windows.Media.FontFamily MaintitleFontName { get; set; }
        public System.Windows.Media.Brush MainTitleFontColor { get; set; }
        public long GamePlayedMinutes { get; set; }
        public string Launchpath { get; set; }
        public string Arguments { get; set; }

    }

    public class MainConfig //主体配置项
    {
        //OOBE状态
        public bool OOBEStatus { get; set; }

        //用户名
        public string Username { get; set; }

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

        //启动时播放音乐
        public bool PlayMusicStarted { get; set; }

        //游戏配置项，勿动
        public List<LaunchConfig> GameInfos { get; set; }

        //音乐配置项，勿动
        public List<MusicConfig> MusicInfos { get; set; }
    }

    public class MusicConfig
    {
        public string MusicPath { get; set; }
        public string MusicShowName { get; set; }
        public string MusicHashCode { get; set; }
    }



}
