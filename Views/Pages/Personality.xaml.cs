using ControlzEx.Theming;
using HuaZi.Library.Json;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;
using ContextMenu = System.Windows.Controls.ContextMenu;


namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// Personality.xaml 的交互逻辑
    /// </summary>
    public partial class Personality : Page
    {
        private List<StackPanel> animationSP = new();
        private MainConfig config;
        public Personality()
        {
            InitializeComponent();
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            Loaded += (async (s, e) =>
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


            });
        }

        private void ColorPalette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //MessageBox.Show(Tools.GetColorName((Color)RootColor.SelectedValue));
            try
            {
                ThemeManager.Current.ChangeTheme(Application.Current, ThemeManager.Current.DetectTheme(Application.Current).BaseColorScheme + "." + Tools.GetColorName((Color)RootColor.SelectedValue));
                //MessageBox.Show(Tools.GetColorName((Color)RootColor.SelectedValue));
                config.ThemeColor = Tools.GetColorName((Color)RootColor.SelectedValue);
                Json.WriteJson(Variables.Configpath, config);
            }
            catch (Exception) { }

        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
            if (Darkmode.IsOn)
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Dark." + ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
                config.ThemeMode = "Dark";
                Json.WriteJson(Variables.Configpath, config);
            }
            else
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Light." + ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
                config.ThemeMode = "Light";
                Json.WriteJson(Variables.Configpath, config);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //UserHead.Source = Tools.ConvertByteArrayToImageSource(ApplicationResources.UserIcon);
            if (File.Exists(Environment.CurrentDirectory + @"\Head.png"))
            {
                UserHead.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + @"\Head.png");
            }
            UserName.Content = config.Username;

            if (config.ChangeThemeWithSystem)
            {
                Darkmode.IsEnabled = false;
                SystemChangeTip.Visibility = Visibility.Visible;
            }
            else
            {
                if (config.ThemeMode == "Dark")
                {
                    Darkmode.IsOn = true;
                }
            }

            if (File.Exists(Environment.CurrentDirectory + @"\Head.png"))
            {
                RestoreHead.Visibility = Visibility.Visible;
            }

            
            

            if(Variables.UsingRealTimeAlarm != null)
            {
                AlarmStackPanel.Visibility = Visibility.Visible;
                if(Variables.UsingRealTimeAlarm == true)
                {
                    AlarmMode.Content = "真实时间模式：";
                    AlarmContent.Content = Variables.AlarmRealTime;
                }
                if (Variables.UsingRealTimeAlarm != true)
                {
                    AlarmMode.Content = "倒计时模式：";
                    AlarmContent.Content = Variables.AlarmTime.TotalMinutes.ToString() + "分钟";
                }
            }

        }

        

        private void ChooseBackground_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var result =await Tools.OpenInputWindow("输入新的用户名");
            if(result != null)
            {
                config.Username = result;
                Json.WriteJson(Variables.Configpath, config);
                UserName.Content = result;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {

                Title = "选择文件",
                Filter = "头像图片(*.png)|*.png",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false

            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                if (openFileDialog.FileName == Environment.CurrentDirectory + @"\Head.png")
                {
                    MessageBox.Show("不能选择同一张头像", "错误", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                else if (openFileDialog.FileName != Environment.CurrentDirectory + @"\Head.png")
                {
                    File.Delete(Environment.CurrentDirectory + @"\Head.png");
                    File.Copy(openFileDialog.FileName, Environment.CurrentDirectory + @"\Head.png");
                    Tools.RefreshAllImageCaches(this);
                    MessageBox.Show("操作成功,为了保证您的游戏体验,重启后生效！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
        }

        

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定还原默认头像吗", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                File.Delete(Environment.CurrentDirectory + @"\Head.png");
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new Personality());
            }
        }

        private void OpenMusicFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"{Environment.CurrentDirectory}\\Musics\\",
                UseShellExecute = true
            });
        }

        

        private void SetAlarm_Click(object sender, RoutedEventArgs e)
        {
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new AlarmPage());
        }

        private void DisableAlarm_Click(object sender, RoutedEventArgs e)
        {
            if(Variables.UsingRealTimeAlarm == true)
            {
                Variables.RealTimeAlarm.Stop();
                Variables.UsingRealTimeAlarm = null;
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new Personality());
            }
            else
            {
                Variables.AlarmCTS.Cancel();
                Variables.UsingRealTimeAlarm = null;
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new Personality());
            }
        }
    }
}
