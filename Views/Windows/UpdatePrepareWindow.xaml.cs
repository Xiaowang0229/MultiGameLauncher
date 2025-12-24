using HuaZi.Library.Downloader;
using MahApps.Metro.Controls;
using Markdig;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Path = System.IO.Path;

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
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()  // 启用扩展功能
                .Build();

            // 转换为 FlowDocument
            FlowDocument document = Markdig.Wpf.Markdown.ToFlowDocument(Variables.VersionLog, pipeline);

            // 将 FlowDocument 设置到 XAML 中的控件（假设你的 XAML 有名为 viewer 的 FlowDocumentScrollViewer）
            UpdateLogViewer.Document = document;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show($"{OnlineLink.Substring(0,OnlineLink.Length - 1)} {Environment.ProcessPath}");
            this.IsEnabled = false;
            UpdatePrepareProgress.Visibility = Visibility.Visible;
            bool DownloadStatus = false;
            if (File.Exists(Path.GetTempPath() + "\\Temp.exe"))
            {
                File.Delete(Path.GetTempPath() + "\\Temp.exe");
            }
            try
            {

                var downloader = new Downloader
                {
                    Url = "https://lz.qaiu.top/parser?url=https://xiaowang-hanxing.lanzouv.com/iXrER3ee8pkf&pwd=f3pa",
                    SavePath = Path.GetTempPath() + "\\Temp.exe",
                    Completed = (async (s, e) =>
                    {
                        if (s)
                        {


                            DownloadStatus = true;

                        }
                        else
                        {
                            MessageBox.Show($"启动失败！错误为:{e}", "错误", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                            Environment.Exit(0);
                        }
                    })
                };
                downloader.StartDownload();
                while (DownloadStatus == false)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"{Path.GetTempPath()}Temp.exe",
                    Arguments = $"{OnlineLink.Substring(0, OnlineLink.Length - 1)} {Environment.ProcessPath}",
                    UseShellExecute = true
                });
                Tools.KillTaskBar();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"准备更新时发生错误：{ex.Message}", "错误", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

        }
    
    }
}
