using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace MultiGameLauncher.Views.Windows
{
    /// <summary>
    /// UpdatePrepareWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdatePrepareWindow : MetroWindow
    {
        private List<StackPanel> animationSP = new();
        private string OnlineVersion {get;set;}
        private string OnlineLog { get; set; }
        private string OnlineLink { get; set; }
        private string LocalVersion { get; set; }
        public UpdatePrepareWindow(string localVersion, string onlineVersion, string onlineLog, string onlineLink)
        {

            InitializeComponent();
            LocalVersion = localVersion;
            OnlineLink = onlineLink;
            OnlineVersion = onlineVersion;
            OnlineLog = onlineLog;
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

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentVersion.Text = "当前版本：" + LocalVersion;
            LatestVersion.Text = "最新版本：" + OnlineVersion;
            UpdateLog.Text = OnlineLog;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Process.Start(new ProcessStartInfo
            {
                FileName = Environment.CurrentDirectory + @"\Update\UpdateAPI.exe",
                Arguments = OnlineLink+" "+ Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true
            });
            Application.Current.Shutdown();
        }
    
    }
}
