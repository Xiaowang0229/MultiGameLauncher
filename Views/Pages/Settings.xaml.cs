using HuaZi.Library.Json;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : Page
    {
        private List<StackPanel> animationSP = new();
        private MainConfig config;
        private double cachesizeint = 0;
        public Settings()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("确定要重置所有配置项（包含个性化设置，主题设置和所有已经添加的游戏等）吗？此操作不可逆","警告",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)
            {
                Tools.InitalizeConfig();
                Tools.Restart();
            }
        }

        private void StartUpCheckUpdate_Toggled(object sender, RoutedEventArgs e)
        {
            if (StartUpCheckUpdate.IsOn)
            {
                config.StartUpCheckUpdate = true;
                Json.WriteJson(Variables.Configpath, config);
                //MessageBox.Show("设置成功，重启程序后生效","提示",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            else if (StartUpCheckUpdate.IsOn == false)
            {
                config.StartUpCheckUpdate = false;
                Json.WriteJson(Variables.Configpath, config);
                //MessageBox.Show("设置成功，重启程序后生效", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
        }

        private void ChangeThemeWithSystem_Toggled(object sender, RoutedEventArgs e)
        {
            if (ChangeThemeWithSystem.IsOn)
            {
                config.ChangeThemeWithSystem = true;
                Json.WriteJson(Variables.Configpath, config);
                Tools.StartThemeMonitoring();
            }
            else if (ChangeThemeWithSystem.IsOn == false)
            {
                config.ChangeThemeWithSystem = false;
                Json.WriteJson(Variables.Configpath, config);
                Tools.StopThemeMonitoring();
            }
        }

        private void AutoStartWithSystem_Toggled(object sender, RoutedEventArgs e)
        {
            if (AutoStartWithSystem.IsOn)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                key.SetValue("Rocket Launcher", Process.GetCurrentProcess().MainModule.FileName, RegistryValueKind.String); // 设置字符串类型的键值
                key.Close();
                config.AutoStartUp = true;
                Json.WriteJson(Variables.Configpath, config);
            }
            else if (AutoStartWithSystem.IsOn == false)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                key.DeleteValue("Rocket Launcher"); // 设置字符串类型的键值
                key.Close();
                config.AutoStartUp = false;
                Json.WriteJson(Variables.Configpath, config);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StartUpCheckUpdate.IsOn = config.StartUpCheckUpdate;
            ChangeThemeWithSystem.IsOn = config.ChangeThemeWithSystem;
            AutoStartWithSystem.IsOn = config.AutoStartUp;
            if (File.Exists(Path.GetTempPath() + "\\Temp.exe"))
            {
                FileInfo fi = new FileInfo(Path.GetTempPath() + "\\Temp.exe");
                cachesizeint += fi.Length;
            }
            if (File.Exists(Path.GetTempPath() + "\\Temp.zip"))
            {
                FileInfo fi = new FileInfo(Path.GetTempPath() + "\\Temp.zip");
                cachesizeint += fi.Length;
            }

            if(cachesizeint != 0)
            {
                CacheSizeBlock.Content = $"{cachesizeint.ToString().Substring(0, 2)}.{cachesizeint.ToString().Substring(2, 2)}MB";
            }
            else if(cachesizeint == 0)
            {
                CacheSizeBlock.Content = "0.0MB";
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(Path.GetTempPath() + "\\Temp.exe"))
                {
                    File.Delete(Path.GetTempPath() + "\\Temp.exe");
                }
                if (File.Exists(Path.GetTempPath() + "\\Temp.zip"))
                {
                    File.Delete(Path.GetTempPath() + "\\Temp.zip");
                }
                MessageBox.Show("操作成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"操作失败，原因:{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Settings());
        }
    }
}
