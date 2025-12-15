using MahApps.Metro.Controls;
using MultiGameLauncher.Views.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
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

namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Page
    {
        private List<StackPanel> animationSP = new();
        public bool IsCheckUpdate {get;set;}
        public About(bool isCheckUpdate = false)
        {
            InitializeComponent();
            IsCheckUpdate = isCheckUpdate;
            VersionBlock.Content = "\n当前版本:" + Variables.Version;
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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RootImage.Source = Tools.ConvertByteArrayToImageSource(ApplicationResources.ApplicationImage);
            UpdateLog.Text = Variables.VersionLog;
            if(IsCheckUpdate)
            {
                CheckupdateButton.IsEnabled = false;
                try
                {
                    CheckProgress.Visibility = Visibility.Visible;
                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                    var content = await client.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestVersion");
                    if (content == Variables.Version)
                    {
                        CheckProgress.Visibility = Visibility.Hidden;
                        MessageBox.Show("当前版本已是最新版本", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        CheckupdateButton.IsEnabled = true;
                    }
                    else
                    {


                        using var log = new HttpClient();
                        log.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                        var Onlinelog = await log.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestLog");
                        using var link = new HttpClient();
                        client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                        var OnlineLink = await client.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestLink");
                        CheckProgress.Visibility = Visibility.Hidden;
                        CheckupdateButton.IsEnabled = true;
                        MetroWindow win = new UpdatePrepareWindow(Variables.Version, content, Onlinelog, OnlineLink);
                        win.ShowDialog();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"检查更新时遇到错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Stop);
                    CheckProgress.Visibility = Visibility.Hidden;
                    CheckupdateButton.IsEnabled = true;
                }
            }
        }




        private void Library_Click(object sender, RoutedEventArgs e)
        {
Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Xiaowang0229/MultiGameLauncher",
                UseShellExecute = true
            });
        }

        private void Issue_Click(object sender, RoutedEventArgs e)
        {
Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Xiaowang0229/MultiGameLauncher/issues/new",
                UseShellExecute = true
            });
        }

        private void Afdian_Click(object sender, RoutedEventArgs e)
        {
Process.Start(new ProcessStartInfo
            {
                FileName = "https://afdian.com/a/csharpfadian",
                UseShellExecute = true
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckupdateButton.IsEnabled = false;
            try
            {
                CheckProgress.Visibility = Visibility.Visible;
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                var content = await client.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestVersion");
                if (content == Variables.Version)
                {
                    CheckProgress.Visibility = Visibility.Hidden;
                    MessageBox.Show("当前版本已是最新版本", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    CheckupdateButton.IsEnabled = true;
                }
                else
                {
                    
                    
                    using var log = new HttpClient();
                    log.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                    var Onlinelog = await log.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestLog");
                    using var link = new HttpClient();
                    client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                    var OnlineLink = await client.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestLink");
                    CheckProgress.Visibility = Visibility.Hidden;
                    CheckupdateButton.IsEnabled = true;
                    MetroWindow win = new UpdatePrepareWindow(Variables.Version,content,Onlinelog,OnlineLink);
                    win.ShowDialog();
                    
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"检查更新时遇到错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Stop);
                CheckProgress.Visibility = Visibility.Hidden;
                CheckupdateButton.IsEnabled = true;
            }
            //MessageBox.Show(content);
            

        }

        private void Apache_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://raw.githubusercontent.com/Xiaowang0229/MultiGameLauncher/refs/heads/master/LICENSE",
                UseShellExecute = true
            });
        }

        private void MahMetro_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/MahApps/MahApps.Metro",
                UseShellExecute = true
            });
        }

        private void MahLicense_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://raw.githubusercontent.com/MahApps/MahApps.Metro/refs/heads/develop/LICENSE",
                UseShellExecute = true
            });
        }
    }
}
