using HuaZi.Library.Json;
using Markdig;
using NAudio.Wave;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading.Tasks;
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
        private AudioFileReader Audio;
        private bool _isTipAnimating = false;
        private MainConfig config;
        private LaunchConfig launchConfig;
        private int musicplayingindex = 0;
        private bool MusicPageUnload = false;
        private bool isMusicPlaying = false;
        public int TabIndex = 0;
        public DispatcherTimer musicupdater = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(750) };
        private bool MusicLoop = false;

        public Launch()
        {
            InitializeComponent();
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            musicupdater.Tick += Musicupdater_Tick;


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

        private async void Musicupdater_Tick(object? sender, EventArgs e)
        {
            if (Audio == null) return;

            // 总时长（始终不变）
            string totalTime = Audio.TotalTime.ToString(@"mm\:ss");

            // 当前进度
            string currentTime = Audio.CurrentTime.ToString(@"mm\:ss");

            // 更新你的两个 TextBlock
            CurrentPlayBlock.Text = $"当前播放：{config.MusicInfos[musicplayingindex].MusicShowName}";
            MusicProgressBlock.Text = $"{currentTime} / {totalTime}";  // 就是 xx:xx / xx:xx

            if(totalTime == currentTime)
            {
                await Task.Delay(500);
                PlayStopped();
            }
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
                BackgroundVideo.Pause();
                Tools.StartMonitingGameStatus(RootTabControl.SelectedIndex);

                await Tools.WaitMonitingGameExitAsync(RootTabControl.SelectedIndex);
                
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
                {
                    //BackgroundVideo.Source = new Uri(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4");
                    BackgroundVideo.Play();


                }

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
            ChangeGameBlockText(config.GameInfos[0].ShowName);

            if (Variables.GameProcessStatus[RootTabControl.SelectedIndex] == true)
            {
                LaunchTile.Visibility = Visibility.Hidden;
                StopTile.Visibility = Visibility.Visible;
                await Tools.WaitMonitingGameExitAsync(0);
                LaunchTile.Visibility = Visibility.Visible;
                StopTile.Visibility = Visibility.Hidden;
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
            if(config.MusicInfos.Count != 0)
            {
                Audio = new AudioFileReader(config.MusicInfos[musicplayingindex].MusicPath);
                Variables.RootMusicPlayer.Init(Audio);
                if(config.PlayMusicStarted)
                {
                    PlayingControls.Visibility = Visibility.Visible;
                    var inanimation = new ThicknessAnimation
                    {
                        From = new Thickness(0, 0, 2000, 0),
                        To = new Thickness(0, 0, 0, 0),
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseInOut }
                    };
                    PlayingControls.BeginAnimation(MarginProperty, inanimation);
                    var accentBrush = this.TryFindResource("MahApps.Brushes.Accent") as SolidColorBrush;
                    PlayButton.Background = accentBrush;
                    PlayLabel.Content = "";
                    isMusicPlaying = true;
                    Variables.RootMusicPlayer.PlaybackStopped += RootMusicPlayer_PlaybackStopped;
                    Variables.RootMusicPlayer.Play();
                    musicupdater.Start();
                    PopUpMusicTips();
                }
            }

            if(config.MusicInfos.Count == 0)
            {
                MusicController.Visibility = Visibility.Hidden;
            }

            if (Variables.GameProcessStatus[RootTabControl.SelectedIndex] == true)
            {
                LaunchTile.Visibility = Visibility.Hidden;
                StopTile.Visibility = Visibility.Visible;
                await Tools.WaitMonitingGameExitAsync(RootTabControl.SelectedIndex);
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
            ChangeGameBlockText(launchConfig.ShowName);

        }

        private void RootMusicPlayer_PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            if (MusicPageUnload != true)
            {
                if(MusicLoop)
                {
                    Variables.RootMusicPlayer.Dispose();
                    Audio = new AudioFileReader(config.MusicInfos[musicplayingindex].MusicPath);
                    Variables.RootMusicPlayer.Init(Audio);
                    Variables.RootMusicPlayer.Play();
                    PopUpMusicTips();
                }
                else if(MusicLoop != true)
                {
                    musicplayingindex += 1;
                    if (musicplayingindex <= config.MusicInfos.Count - 1)
                    {
                        Variables.RootMusicPlayer.Dispose();
                        Audio = new AudioFileReader(config.MusicInfos[musicplayingindex].MusicPath);
                        Variables.RootMusicPlayer.Init(Audio);
                        Variables.RootMusicPlayer.Play();
                        PopUpMusicTips();
                    }
                    else
                    {
                        musicplayingindex = 0;
                        Variables.RootMusicPlayer.Dispose();
                        Audio = new AudioFileReader(config.MusicInfos[musicplayingindex].MusicPath);
                        Variables.RootMusicPlayer.Init(Audio);
                        Variables.RootMusicPlayer.Play();
                        PopUpMusicTips();
                    }
                }
            }
            if (MusicPageUnload == true)
            {
                Variables.RootMusicPlayer.Dispose();
            }
        }

        private void PlayStopped()
        {
            if (MusicPageUnload != true)
            {
                if (MusicLoop)
                {
                    Variables.RootMusicPlayer.Dispose();
                    Audio = new AudioFileReader(config.MusicInfos[musicplayingindex].MusicPath);
                    Variables.RootMusicPlayer.Init(Audio);
                    Variables.RootMusicPlayer.Play();
                    PopUpMusicTips();
                }
                else if (MusicLoop != true)
                {
                    musicplayingindex += 1;
                    if (musicplayingindex <= config.MusicInfos.Count - 1)
                    {
                        Variables.RootMusicPlayer.Dispose();
                        Audio = new AudioFileReader(config.MusicInfos[musicplayingindex].MusicPath);
                        Variables.RootMusicPlayer.Init(Audio);
                        Variables.RootMusicPlayer.Play();
                        PopUpMusicTips();
                    }
                    else
                    {
                        musicplayingindex = 0;
                        Variables.RootMusicPlayer.Dispose();
                        Audio = new AudioFileReader(config.MusicInfos[musicplayingindex].MusicPath);
                        Variables.RootMusicPlayer.Init(Audio);
                        Variables.RootMusicPlayer.Play();
                        PopUpMusicTips();
                    }
                }
            }
            if (MusicPageUnload == true)
            {
                Variables.RootMusicPlayer.Dispose();
            }
        }

        private async void PopUpMusicTips()  // 或 Task，根据你原来调用方式
        {
            // 如果正在动画中，直接返回（防止重复触发导致紊乱）
            if (_isTipAnimating) return;

            _isTipAnimating = true;

            // 确保从正确初始位置开始
            MusicPlayTip.Margin = new Thickness(0, -50, 0, 0);
            MusicPlayTip.Visibility = Visibility.Visible;

            var inanimation = new ThicknessAnimation
            {
                From = new Thickness(0, -50, 0, 0),
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

            string totalTime = Audio.TotalTime.ToString(@"mm\:ss");

            // 当前进度
            string currentTime = Audio.CurrentTime.ToString(@"mm\:ss");

            // 更新你的两个 TextBlock
            CurrentPlayBlock.Text = $"当前播放：{config.MusicInfos[musicplayingindex].MusicShowName}";
            MusicProgressBlock.Text = $"{currentTime} / {totalTime}";  // 就是 xx:xx / xx:xx

            // 滑入（关键：使用 Compose 平滑接管可能残留的旧动画）
            MusicPlayTip.BeginAnimation(MarginProperty, inanimation, HandoffBehavior.Compose);

            await Task.Delay(TimeSpan.FromSeconds(3) + TimeSpan.FromMilliseconds(500));  // 等待显示3秒 + 滑入时间

            // 滑出
            outanimation.Completed += (s, e) =>
            {
                MusicPlayTip.Visibility = Visibility.Collapsed;
                _isTipAnimating = false;  // 动画完全结束后才允许下一次显示
            };

            MusicPlayTip.BeginAnimation(MarginProperty, outanimation, HandoffBehavior.Compose);
        }

        private async void RootTabItemSelectionChanged(object sender, EventArgs e)
        {

            if (RootTabControl.Tag != ((System.Windows.Controls.TabItem)sender).Tag.ToString())
            {
                TabIndex = RootTabControl.SelectedIndex;
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
                ChangeGameBlockText(launchConfig.ShowName);
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
            musicupdater.Stop();
            Variables.RootMusicPlayer.Stop();
        }

        private async void MusicController_Click(object sender, RoutedEventArgs e)
        {

            if(isMusicPlaying)
            {
                var outanimation = new ThicknessAnimation
                {
                    From = new Thickness(0,0,0,0),
                    To = new Thickness(0, 0, -100, 0),
                    Duration = TimeSpan.FromMilliseconds(100),
                    EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseInOut }
                };
                PlayingControls.BeginAnimation(MarginProperty,outanimation);
                await Task.Delay(150);
                PlayingControls.Visibility = Visibility.Hidden;
                Variables.RootMusicPlayer.Pause();
                isMusicPlaying = false;
                PlayButton.Background = new SolidColorBrush(System.Windows.Media.Colors.Gray);
                PlayLabel.Content = "";
                musicupdater.Stop();
            }
            else
            {
                
                PlayingControls.Visibility = Visibility.Visible;
                var inanimation = new ThicknessAnimation
                {
                    From = new Thickness(0, 0, 2000, 0),
                    To = new Thickness(0, 0, 0, 0),
                    Duration = TimeSpan.FromMilliseconds(500),
                    EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseInOut }
                };
                PlayingControls.BeginAnimation(MarginProperty, inanimation);
                isMusicPlaying = true;
                Variables.RootMusicPlayer.Play();
                var accentBrush = this.TryFindResource("MahApps.Brushes.Accent") as SolidColorBrush;
                PlayLabel.Content = "";
                PlayButton.Background = accentBrush;
                musicupdater.Start();
                PopUpMusicTips();
                

            }
        }

        private void ChangeGameBlockText(string gamename)
        {
            CurrentGameStart.Text = "当前游戏：" + gamename;
            CurrentGameStop.Text = "当前游戏：" + gamename;
        }

        private async void ReplayController_Click(object sender, RoutedEventArgs e)
        {

            PlayStopped();
            ReplayBorderChangeColor();

        }

        private async void ReplayBorderChangeColor()
        {
            var accentBrush = this.TryFindResource("MahApps.Brushes.Accent") as SolidColorBrush;
            ReplayBorder.Background = accentBrush;
            await Task.Delay(100);
            ReplayBorder.Background = new SolidColorBrush(Colors.Gray);
        }

        private void LoopController_Click(object sender, RoutedEventArgs e)
        {
            if(MusicLoop)
            {
                MusicLoop = false;
                LoopBorder.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                MusicLoop = true;
                var accentBrush = this.TryFindResource("MahApps.Brushes.Accent") as SolidColorBrush;
                LoopBorder.Background = accentBrush;
            }
        }



        private void ReplayController_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void ReplayController_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReplayBorder.Background = new SolidColorBrush(Colors.Gray);
        }
    }
}
