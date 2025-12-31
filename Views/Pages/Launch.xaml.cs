using HuaZi.Library.Json;
using Markdig;
using NAudio.Wave;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// Launch.xaml 的交互逻辑
    /// </summary>
    public partial class Launch : Page
    {
        private List<StackPanel> animationSP = new();
        private CancellationTokenSource _animationCts;
        private MainConfig config;
        private LaunchConfig launchConfig;
        private int musicplayingindex = 0;
        private bool MusicPageUnload = false;
        private bool isMusicPlaying = false;

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
                        spp.Margin = new Thickness(-2000, 0, 0, 10);
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
                    //Variables.RootMusicPlayer.Init(new AudioFileReader());

                }
                catch (InvalidOperationException) { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }


            });
            Unloaded += (s, e) =>
            {
                try
                {
                    Variables.RootMusicPlayer.Dispose();
                }
                catch { }
            };


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

            try
            {
                /*Tools.Process.StartInfo = new ProcessStartInfo
                {
                    FileName = launchConfig.Launchpath,
                    Arguments = launchConfig.Arguments,
                    UseShellExecute = true
                };
                Tools.Process.Start();*/
                LaunchTile.Visibility = Visibility.Hidden;
                StopTile.Visibility = Visibility.Visible;

                Tools.StartMonitingGameStatus(RootTabControl.SelectedIndex);
                await Tools.WaitMonitingGameExitAsync(RootTabControl.SelectedIndex);
                Tools.StopMonitingGameStatus(RootTabControl.SelectedIndex);

                /*var proc = Variables.GameProcess[RootTabControl.SelectedIndex];
                proc.Start();
                Variables.GameProcessStatus[RootTabControl.SelectedIndex] = true;
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.Hide();
                Variables.MainWindowHideStatus = true;
                Tools.InitializeTaskBarContentMenu();
                Variables.PlayingTimeRecorder[RootTabControl.SelectedIndex].Start();
                var toast = new ToastContentBuilder().AddText("程序已启动").AddText($"程序名：{config.GameInfos[RootTabControl.SelectedIndex].ShowName}").AddText($"进程监测已开启").AddAppLogoOverride(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[RootTabControl.SelectedIndex].HashCode}\\Icon.png"));
                toast.Show();

                

                //计时器清理逻辑，防卡顿
                
                Variables.PlayingTimeRecorder[RootTabControl.SelectedIndex].Stop();
                var time = Variables.PlayingTimeintList[RootTabControl.SelectedIndex];  
                config.GameInfos[RootTabControl.SelectedIndex].GamePlayedMinutes += time;
                var toast0 = new ToastContentBuilder().AddText("程序已结束").AddText($"程序名：{config.GameInfos[RootTabControl.SelectedIndex].ShowName}").AddText($"游戏时长：{time}分钟").AddAppLogoOverride(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[RootTabControl.SelectedIndex].HashCode}\\Icon.png"));
                toast0.Show();
                Tools.InitializeTaskBarContentMenu();
                Variables.GameProcessStatus[RootTabControl.SelectedIndex] = false;
                Variables.PlayingTimeintList[RootTabControl.SelectedIndex] = 0;
                Variables.MainWindowHideStatus = false;
                win.Show();*/

                LaunchTile.Visibility = Visibility.Visible;
                StopTile.Visibility = Visibility.Hidden;




            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动游戏时发生错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            var proc = Variables.GameProcess[RootTabControl.SelectedIndex];
            proc.Kill();
            /*Tools.InitializeTaskBarContentMenu();
            Variables.GameProcessStatus[RootTabControl.SelectedIndex] = false;*/
            LaunchTile.Visibility = Visibility.Visible;
            StopTile.Visibility = Visibility.Hidden;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {


            for (int i = 0; i < config.GameInfos.Count; i++)
            {
                var menuitem = new TabItem();
                menuitem.ToolTip = config.GameInfos[i].ShowName;
                menuitem.Tag = config.GameInfos[i].HashCode;
                menuitem.MouseLeftButtonUp += RootTabItemSelectionChanged;
                RootTabControl.Items.Add(menuitem);
            }
            launchConfig = config.GameInfos[0];
            RootTabControl.Tag = launchConfig.HashCode;
            /*Tools.Process.StartInfo = new ProcessStartInfo
            {
                FileName = launchConfig.Launchpath,
                Arguments = launchConfig.Arguments,
                UseShellExecute = true
            };*/
            if (File.Exists(Environment.CurrentDirectory + @"\Head.png"))
            {
                UserHead.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + @"\Head.png");
            }

            MainTitle.Text = launchConfig.MainTitle;
            MainTitle.FontFamily = launchConfig.MaintitleFontName;
            MainTitle.Foreground = launchConfig.MainTitleFontColor;
            LaunchTile.Tag = launchConfig.Launchpath;

            if (Variables.GameProcessStatus[RootTabControl.SelectedIndex] == true)
            {
                LaunchTile.Visibility = Visibility.Hidden;
                StopTile.Visibility = Visibility.Visible;
            }

            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {
                BackgroundImage.Visibility = Visibility.Hidden;
                BackgroundVideo.Visibility = Visibility.Visible;



                //BackgroundVideo.Source = new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4");
                BackgroundVideo.Open(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"));


            }
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.png"))
            {
                Tools.RefreshAllImageCaches(this);
                BackgroundVideo.Visibility = Visibility.Hidden;
                BackgroundImage.Visibility = Visibility.Visible;

                BackgroundImage.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.png");
            }
            if (!(File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.png")) && !(File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4")))
            {
                try
                {
                    BackgroundVideo.Stop();
                    BackgroundVideo.Close();
                    BackgroundVideo.Visibility = Visibility.Hidden;
                    BackgroundImage.Visibility = Visibility.Hidden;
                }
                catch { }
            }

            /*if(Tools.Process.HasExited == false)
            {
                LaunchTile.Visibility = Visibility.Hidden;
                StopTile.Visibility = Visibility.Visible;
            }*/
            if(config.MusicInfos.Count != 0 && config.PlayMusicStarted)
            {
                var accentBrush = this.TryFindResource("MahApps.Brushes.Accent") as SolidColorBrush;
                PlayButton.Background = accentBrush;
                isMusicPlaying = true;
                Variables.RootMusicPlayer.Init(new AudioFileReader(config.MusicInfos[musicplayingindex].MusicPath));
                Variables.RootMusicPlayer.PlaybackStopped += RootMusicPlayer_PlaybackStopped;
                Variables.RootMusicPlayer.Play();
                PopUpMusicTips();
            }
            if (Variables.GameProcessStatus[RootTabControl.SelectedIndex] == true)
            {
                LaunchTile.Visibility = Visibility.Hidden;
                StopTile.Visibility = Visibility.Visible;
                await Tools.WaitMonitingGameExitAsync(RootTabControl.SelectedIndex);
                Tools.StopMonitingGameStatus(RootTabControl.SelectedIndex);
                LaunchTile.Visibility = Visibility.Visible;
                StopTile.Visibility = Visibility.Hidden;
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

        private void RootMusicPlayer_PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            if(MusicPageUnload != true)
            {
                musicplayingindex += 1;
                if (musicplayingindex <= config.MusicInfos.Count-1)
                {
                    Variables.RootMusicPlayer.Init(new AudioFileReader(config.MusicInfos[musicplayingindex].MusicPath));
                    Variables.RootMusicPlayer.Play();
                    PopUpMusicTips();
                }
                else
                {
                    musicplayingindex = 0;
                    Variables.RootMusicPlayer.Init(new AudioFileReader(config.MusicInfos[musicplayingindex].MusicPath));
                    Variables.RootMusicPlayer.Play();
                    PopUpMusicTips();
                }
            }
            if(MusicPageUnload == true)
            {
                Variables.RootMusicPlayer.Dispose();
            }
        }

        private async void PopUpMusicTips()
        {
            

            MusicPlayTip.BeginAnimation(MarginProperty, null);
            CurrentPlayBlock.Text = $"当前播放:{config.MusicInfos[musicplayingindex].MusicShowName}";
            MusicProgressBlock.Text = $"进度：{musicplayingindex+1}/{config.MusicInfos.Count}";

            var inanimation = new ThicknessAnimation
            {
                From = new Thickness(0,-50,0,0),
                To = new Thickness(0, 10, 0, 0),
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseIn }
            };
            var outanimation = new ThicknessAnimation
            {
                
                From = new Thickness(0, 10, 0, 0),
                To = new Thickness(0, -50, 0, 0),
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
            };
            MusicPlayTip.BeginAnimation(MarginProperty, null);
            MusicPlayTip.BeginAnimation(MarginProperty, inanimation);
            await Task.Delay(TimeSpan.FromSeconds(3));
            MusicPlayTip.BeginAnimation(MarginProperty, null);
            MusicPlayTip.BeginAnimation(MarginProperty, outanimation);
        }

        private async void RootTabItemSelectionChanged(object sender, EventArgs e)
        {

            if (RootTabControl.Tag != ((System.Windows.Controls.TabItem)sender).Tag.ToString())
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
                /*Tools.Process.StartInfo = new ProcessStartInfo
                {
                    FileName = launchConfig.Launchpath,
                    Arguments = launchConfig.Arguments,
                    UseShellExecute = true
                };*/
                //MessageBox.Show($"{Variables.GameProcessStatus[RootTabControl.SelectedIndex]}");

                MainTitle.Text = launchConfig.MainTitle;
                MainTitle.FontFamily = launchConfig.MaintitleFontName;
                MainTitle.Foreground = launchConfig.MainTitleFontColor;
                LaunchTile.Tag = launchConfig.Launchpath;
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
                {
                    BackgroundImage.Visibility = Visibility.Hidden;
                    BackgroundVideo.Visibility = Visibility.Visible;
                    BackgroundVideo.Open(new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"));

                }
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.png"))
                {
                    BackgroundVideo.Stop();
                    Tools.RefreshAllImageCaches(this);
                    BackgroundVideo.Close();

                    BackgroundVideo.Visibility = Visibility.Hidden;
                    BackgroundImage.Visibility = Visibility.Visible;
                    BackgroundImage.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.png");
                }
                if (!(File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.png")) && !(File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4")))
                {
                    try
                    {
                        BackgroundVideo.Stop();
                        BackgroundVideo.Close();
                        BackgroundVideo.Visibility = Visibility.Hidden;
                        BackgroundImage.Visibility = Visibility.Hidden;
                    }
                    catch { }
                }

                if (Variables.GameProcessStatus[RootTabControl.SelectedIndex] == true)
                {
                    LaunchTile.Visibility = Visibility.Hidden;
                    StopTile.Visibility = Visibility.Visible;
                    await Tools.WaitMonitingGameExitAsync(RootTabControl.SelectedIndex);
                    Tools.StopMonitingGameStatus(RootTabControl.SelectedIndex);
                    LaunchTile.Visibility = Visibility.Visible;
                    StopTile.Visibility = Visibility.Hidden;
                }

                if (Variables.GameProcessStatus[RootTabControl.SelectedIndex] == false)
                {
                    LaunchTile.Visibility = Visibility.Visible;
                    StopTile.Visibility = Visibility.Hidden;
                }
            }


        }



        private async void UserHead_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {
                BackgroundVideo.Close();
                BackgroundVideo.Stop();

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
            MusicPageUnload = true;
            Variables.RootMusicPlayer.Stop();
        }

        private void MusicController_Click(object sender, RoutedEventArgs e)
        {

            if(isMusicPlaying)//未播放时
            {
                MusicPageUnload = true;
                isMusicPlaying = false;
                Variables.RootMusicPlayer.Stop();
                PlayButton.Background = new SolidColorBrush(System.Windows.Media.Colors.Gray);
            }
            else
            {
                MusicPageUnload = false;
                isMusicPlaying = true;
                Variables.RootMusicPlayer.Init(new AudioFileReader(config.MusicInfos[musicplayingindex].MusicPath));
                Variables.RootMusicPlayer.Play();
                var accentBrush = this.TryFindResource("MahApps.Brushes.Accent") as SolidColorBrush;
                PlayButton.Background = accentBrush;
                PopUpMusicTips();
                

            }
        }
    }
}
