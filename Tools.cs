global using Application = System.Windows.Application;
global using MessageBox = System.Windows.MessageBox;
global using Page = System.Windows.Controls.Page;

using ControlzEx.Theming;
using Hardcodet.Wpf.TaskbarNotification;
using HuaZi.Library.Json;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using MultiGameLauncher.Views.Pages;
using MultiGameLauncher.Views.Windows;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Image = System.Drawing.Image;
using MenuItem = System.Windows.Controls.MenuItem;


namespace MultiGameLauncher
{
    public class Variables //变量集
    {
        public readonly static string Version = "Release 1.3.0.0 RC2-Hotfix 1\n";
        public static string ShowVersion;
        public readonly static string Configpath = Environment.CurrentDirectory + @"\Config.json";
        public static List<Process> GameProcess = new List<Process>();
        public static Hardcodet.Wpf.TaskbarNotification.TaskbarIcon RootTaskBarIcon;
        public static ContextMenu TaskBarMenu = new ContextMenu();
        public static List<bool> GameProcessStatus = new List<bool>();
        public static List<DispatcherTimer> PlayingTimeRecorder = new List<DispatcherTimer>();
        public static List<Int64> PlayingTimeintList = new List<Int64>();
        public static string VersionLog { get; set; }
        public static bool MainWindowHideStatus { get; set; } = false;
    }

    
    public class Tools //工具集
    {
        //public static Process Process = new();
        public static FrameworkElement OldPage = null; 
        public static ImageSource ApplicationLogo;

        // ========== 必要的 Win32 API 声明 ==========
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, uint nIconIndex);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }


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
            catch{ }

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
                GameInfos = new List<LaunchConfig>()
            };
            Json.WriteJson(Variables.Configpath,config);
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

        public static int FindHashcodeinGameinfosint(MainConfig config,string hashcode)
        {
            for (int i = 0;i< config.GameInfos.Count;i++)
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
                win.Show();
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
                if (Variables.MainWindowHideStatus)
                {
                    win.Show();
                    Variables.MainWindowHideStatus = false;
                    win.Topmost = true;
                    win.Topmost = false;
                }
                else
                {
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
                        StopMonitingGameStatus(index);
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
                win.Show();
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
            var proc = Variables.GameProcess[index];
            await proc.WaitForExitAsync();
        }

        public static void StopMonitingGameStatus(int index)
        {
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            var config = Json.ReadJson<MainConfig>(Variables.Configpath);
            Variables.PlayingTimeRecorder[index].Stop();
            var time = Variables.PlayingTimeintList[index];
            config.GameInfos[index].GamePlayedMinutes += time;
            Json.WriteJson(Variables.Configpath, config);
            
            Tools.InitializeTaskBarContentMenu();
            Variables.GameProcessStatus[index] = false;
            Variables.PlayingTimeintList[index] = 0;
            Variables.MainWindowHideStatus = false;
            win.Show();
            var pg = new Launch();
            win.RootFrame.Navigate(pg);
            pg.RootTabControl.SelectedIndex = index;
            var toast0 = new ToastContentBuilder().AddText("程序已结束").AddText($"程序名：{config.GameInfos[index].ShowName}").AddText($"游戏时长：{time} 分钟").AddAppLogoOverride(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[index].HashCode}\\Icon.png"));
            toast0.Show();
        }
        public static string? OpenInputWindow(string Title)
        {
            var win = new InputWindow(Title);
            if(win.ShowDialog() == true)
            {
                return win.Results;
            }
            return null;
        }

        public static void ExtractExeIconToPng(string exePath, string pngPath)
        {

            // ========== 尝试使用 SHGetFileInfo 获取系统关联的大图标（最可靠）==========
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr result = SHGetFileInfo(exePath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), 0x100); // SHGFI_ICON = 0x100

            Icon icon = null;
            bool iconFromSH = false;

            if (result != IntPtr.Zero && shinfo.hIcon != IntPtr.Zero)
            {
                icon = Icon.FromHandle(shinfo.hIcon);
                iconFromSH = true;
            }
            else
            {
                // ========== 备选：直接从资源提取第一个图标 ==========
                IntPtr hIcon = ExtractIcon(IntPtr.Zero, exePath, 0);
                if (hIcon != IntPtr.Zero && hIcon != new IntPtr(-1))
                {
                    icon = Icon.FromHandle(hIcon);
                }
            }
            string dir = Path.GetDirectoryName(pngPath);
            if (icon == null)
            {   
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    { Directory.CreateDirectory(dir); }
                
            }

            

            // 确保输出目录存在
            
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            try
            {
                // 优先提取 64x64 尺寸的图标
                using (Icon sizedIcon = new Icon(icon, 64, 64))
                using (Bitmap bmp = sizedIcon.ToBitmap())
                {
                    bmp.Save(pngPath, ImageFormat.Png);
                }
            }
            catch
            {
                // 如果没有正好 64x64 的尺寸，回退到原始图标转 Bitmap
                using (Bitmap bmp = icon.ToBitmap())
                {
                    bmp.Save(pngPath, ImageFormat.Png);
                }
            }
            finally
            {
                icon.Dispose();
                // 如果是从 SHGetFileInfo 拿到的图标，需要手动销毁原始句柄（FromHandle 已复制）
                if (iconFromSH && shinfo.hIcon != IntPtr.Zero)
                    DestroyIcon(shinfo.hIcon);
            }
        }

        

    }

    
    public class LaunchConfig
    {

        public string HashCode { get; set; }
        public string ShowName { get; set; }
        public string MainTitle { get; set; }
        public System.Windows.Media.FontFamily MaintitleFontName { get; set; }
        public System.Windows.Media.Brush MainTitleFontColor { get; set; }
        public Int64 GamePlayedMinutes { get; set; }
        public string Launchpath { get; set; }
        public string Arguments { get; set; }

    }

    public class MainConfig //主体配置项
    {
        //OOBE状态
        public bool OOBEStatus { get;set; }

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

        //游戏配置项，勿动
        public List<LaunchConfig> GameInfos { get; set; }
    }

    //Stackpanel页动画
    //private List<StackPanel> animationSP = new();
    /*Loaded += (async (s, e) =>
            {
                try
                {
                    animationSP.Clear();
                    foreach (var sp in sp_ani.Children)
                    {
                        if (((StackPanel)sp).Tag != null)
                            if (((StackPanel)sp).Tag.ToString() == "ani")
                            {
                                animationSP.Add((StackPanel)sp);
                            }
                    }

                    foreach (var spp in animationSP)
                    {
                        spp.Margin = new Thickness(-2000, 0, 0, 10);
                    }

                    var animation = new ThicknessAnimation
                    {
                        To = new Thickness(0, 0, 0, 10),
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                    };

                    foreach (var aniSP in animationSP)
                    {
                        aniSP.BeginAnimation(MarginProperty, animation);
                        await Task.Delay(100);
                    }
                }
                catch (InvalidOperationException) { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }


            });*/
}
