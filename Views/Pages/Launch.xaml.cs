using HuaZi.Library.Json;
using Markdig;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// Launch.xaml 的交互逻辑
    /// </summary>
    public partial class Launch : Page
    {
        private List<StackPanel> animationSP = new();
        private MainConfig config;
        private LaunchConfig launchConfig;

        public Launch()
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
                        spp.Margin = new Thickness(-2000, 0,0, 10);
                    }

                    var animation = new ThicknessAnimation
                    {
                        To = new Thickness(0, 0, 0, 10),
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseInOut }
                    };

                    foreach (var aniSP in animationSP)
                    {
                        aniSP.BeginAnimation(MarginProperty, animation);
                        await Task.Delay(20);
                    }
                }
                catch (InvalidOperationException) { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }


            });
           

        }

        

        

        private async void SettingsTile_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {

                BackgroundVideo.Stop();
                BackgroundVideo.Close();
                await Task.Delay(50);

            }
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Settings());
            win.BackButton.Width = 40;
        }

        private async void UserTile_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {

                BackgroundVideo.Stop();
                BackgroundVideo.Close();
                await Task.Delay(50);

            }
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Personality());
            win.BackButton.Width = 40;
        }

        private async void AboutTile_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {

                BackgroundVideo.Stop();
                BackgroundVideo.Close();
                await Task.Delay(50);


            }
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new About());
            win.BackButton.Width = 40;
        }

        private async void ManageTile_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {

                BackgroundVideo.Stop();
                BackgroundVideo.Close();
                await Task.Delay(50);

            }
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Manage());
            win.BackButton.Width = 40;
        }

        private async void UpdateTile_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {

                BackgroundVideo.Stop();
                BackgroundVideo.Close();
                await Task.Delay(50);

            }
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new About(true));
            win.BackButton.Width = 40;
        }

        private async void LaunchTile_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {

                BackgroundVideo.Stop();
                BackgroundVideo.Close();
                await Task.Delay(50);

            }
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Loading
            {
                GamePath = launchConfig.Launchpath,
                GameName = System.IO.Path.GetFileName(launchConfig.Launchpath),
                ShowName = launchConfig.ShowName,
                Arguments =launchConfig.Arguments,
            });

        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Tools.Process.Close();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {


            for (int i = 0;i<config.GameInfos.Count;i++)
            {
                var menuitem = new TabItem();
                menuitem.ToolTip = config.GameInfos[i].ShowName;
                menuitem.Tag = config.GameInfos[i].HashCode;
                menuitem.MouseLeftButtonUp += RootTabItemSelectionChanged;
                RootTabControl.Items.Add(menuitem);
            }
            launchConfig = config.GameInfos[0];
            UserHead.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + @"\Head.png");

            MainTitle.Text = launchConfig.MainTitle;
            MainTitle.FontFamily = launchConfig.MaintitleFontName;
            MainTitle.Foreground = launchConfig.MainTitleFontColor;
            SubTitle.Text = launchConfig.SubTitle;
            LaunchTile.Tag = launchConfig.Launchpath;
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {
                BackgroundImage.Visibility = Visibility.Hidden;
                BackgroundVideo.Visibility = Visibility.Visible;



                //BackgroundVideo.Source = new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4");
                BackgroundVideo.Open(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"));


            }
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.png"))
            {
                BackgroundVideo.Visibility = Visibility.Hidden;
                BackgroundImage.Visibility = Visibility.Visible;
                BackgroundImage.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.png");
            }

            Welcome.Content = "欢迎，" + config.Username;
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()  // 启用扩展功能
                .Build();

            // 转换为 FlowDocument
            FlowDocument document = Markdig.Wpf.Markdown.ToFlowDocument(Variables.VersionLog, pipeline);

            // 将 FlowDocument 设置到 XAML 中的控件（假设你的 XAML 有名为 viewer 的 FlowDocumentScrollViewer）
            LogText.Document = document;
        }

        private async void RootTabItemSelectionChanged(object sender, EventArgs e)
        {
            RootTabControl.Tag = ((System.Windows.Controls.TabItem)sender).Tag.ToString();
            launchConfig = config.GameInfos.FirstOrDefault(x => x.HashCode == ((System.Windows.Controls.TabItem)sender).Tag.ToString());
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
                    spp.Margin = new Thickness(0, 0, 0, 10);
                }


                var animationout = new ThicknessAnimation
                {
                    To = new Thickness(-2000, 0, 0, 10),
                    Duration = TimeSpan.FromMilliseconds(500),
                    EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseInOut }
                };

                foreach (var aniSP in animationSP)
                {
                    aniSP.BeginAnimation(MarginProperty, null);
                    aniSP.BeginAnimation(MarginProperty, animationout);
                    await Task.Delay(20);
                }

                await Task.Delay(TimeSpan.FromMilliseconds(250));

                foreach (var spp in animationSP)
                {
                    spp.Margin = new Thickness(-2000, 0, 0, 10);
                }

                animationSP.Clear();
                foreach (var sp in sp_ani.Children)
                {
                    if (((StackPanel)sp).Tag != null)
                        if (((StackPanel)sp).Tag.ToString() == "ani")
                        {
                            animationSP.Add((StackPanel)sp);
                        }
                }



                var animationin = new ThicknessAnimation
                {
                    To = new Thickness(0, 0, 0, 10),
                    Duration = TimeSpan.FromMilliseconds(500),
                    EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseInOut }
                };

                foreach (var aniSP in animationSP)
                {
                    aniSP.BeginAnimation(MarginProperty, null);
                    aniSP.BeginAnimation(MarginProperty, animationin);
                    await Task.Delay(20);
                }
            }
            catch (InvalidOperationException) { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            MainTitle.Text = launchConfig.MainTitle;
            MainTitle.FontFamily = launchConfig.MaintitleFontName;
            MainTitle.Foreground = launchConfig.MainTitleFontColor;
            SubTitle.Text = launchConfig.SubTitle;
            LaunchTile.Tag = launchConfig.Launchpath;
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {
                BackgroundImage.Visibility = Visibility.Hidden;
                BackgroundVideo.Visibility = Visibility.Visible;
                BackgroundVideo.Open(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"));
                
            }
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.png"))
            {
                BackgroundVideo.Visibility = Visibility.Hidden;
                BackgroundImage.Visibility = Visibility.Visible;
                BackgroundImage.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.png");
            }
        }

        

        private async void UserHead_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {

                BackgroundVideo.Stop();
                BackgroundVideo.Close();
                await Task.Delay(50);

            }
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Personality());
            win.BackButton.Width = 40;
        }

        

        

        private async void BackgroundVideo_MediaEnded(object sender, EventArgs e)
        {
            await Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    BackgroundVideo.Pause();           // 先暂停
                    await Task.Delay(10);               // 稍等一小会儿
                    BackgroundVideo.Position = TimeSpan.Zero;
                    BackgroundVideo.Play();
                }
                catch (Exception ex)
                {
                    // 可选：记录日志
                    Console.WriteLine("循环播放异常: " + ex.Message);
                }
            });
        }

        private void BackgroundVideo_MediaFailed(object sender, Unosquare.FFME.Common.MediaFailedEventArgs e)
        {
            MessageBox.Show($"媒体播放失败:{e.ErrorException}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BackgroundVideo_MediaOpened(object sender, Unosquare.FFME.Common.MediaOpenedEventArgs e)
        {
            BackgroundVideo.Play();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            /*if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {

                BackgroundVideo.Stop();
                BackgroundVideo.Close();


            }*/
        }
    }
}
