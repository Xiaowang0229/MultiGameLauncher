using HuaZi.Library.Json;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Markdig;
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
        private bool _isTipAnimating = false;
        private MainConfig config;
        private LaunchConfig launchConfig;
        private int musicplayingindex = 0;
        private bool MusicPageUnload = false;
        private bool isMusicPlaying = false;
        public int TabIndex = 0;

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

                    

                    var animation = new ThicknessAnimation
                    {
                        From = new Thickness(-2000, 0, 0, 10),
                        To = new Thickness(0, 0, 0, 10),
                        Duration = TimeSpan.FromMilliseconds(50),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                    };

                    
                    foreach (var aniSP in animationSP)
                    {
                        aniSP.BeginAnimation(MarginProperty, animation);
                        await Task.Delay(50);
                    }
                    MainTitle.BeginAnimation(MarginProperty,animation);
                    //Variables.RootMusicPlayer.Init(new AudioFileReader());

                }
                catch (InvalidOperationException) { }
                catch (Exception ex)
                {
                    
                }


            });

            LaunchTile.Tag = "false";

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
            if (File.Exists(Environment.CurrentDirectory + @"\Head.png"))
            {
                UserHead.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + @"\Head.png");
            }
            MainTitle.Text = launchConfig.MainTitle;
            MainTitle.FontFamily = launchConfig.MaintitleFontName;
            MainTitle.Foreground = launchConfig.MainTitleFontColor;
            ChangeGameBlockText(launchConfig.ShowName,launchConfig.GamePlayedMinutes.ToString());
            if (Variables.GameProcessStatus[RootTabControl.SelectedIndex] == true)
            {
                ChangeStartStopStatus(true);
                await Tools.WaitMonitingGameExitAsync(0);
                ChangeStartStopStatus(false);
            }

            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
            {
                BackgroundImage.Visibility = Visibility.Hidden;
                BackgroundVideo.Visibility = Visibility.Visible;
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
                
                ChangeStartStopStatus(true);
            }*/
            

            if (Variables.GameProcessStatus[RootTabControl.SelectedIndex] == true)
            {
                LaunchTile.Tag = "false";
                ChangeStartStopStatus(true);
                await Tools.WaitMonitingGameExitAsync(RootTabControl.SelectedIndex);
                
                ChangeStartStopStatus(false);
            }

            Welcome.Content = "欢迎，" + config.Username;
            
            ChangeGameBlockText(launchConfig.ShowName,launchConfig.GamePlayedMinutes.ToString());

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

                    


                    var animationout = new ThicknessAnimation
                    {
                        From = new Thickness(0, 0, 0, 10),
                        To = new Thickness(-2000, 0, 0, 10),
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                    };
                    MainTitle.BeginAnimation(MarginProperty,null);
                    MainTitle.BeginAnimation(MarginProperty, animationout);
                    foreach (var aniSP in animationSP)
                    {
                        aniSP.BeginAnimation(MarginProperty, null);
                        aniSP.BeginAnimation(MarginProperty, animationout);
                        await Task.Delay(20);
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(250));

                    




                    var animationin = new ThicknessAnimation
                    {
                        From = new Thickness(-2000, 0, 0, 10),
                        To = new Thickness(0, 0, 0, 10),
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                    };
                    

                    foreach (var aniSP in animationSP)
                    {
                        aniSP.BeginAnimation(MarginProperty, null);
                        aniSP.BeginAnimation(MarginProperty, animationin);
                        await Task.Delay(50);
                    }
                    MainTitle.BeginAnimation(MarginProperty, null);
                    MainTitle.BeginAnimation(MarginProperty, animationin);
                }
                catch (InvalidOperationException) { }
                catch (Exception ex)
                {
                    
                }
                /*Tools.Process.StartInfo = new ProcessStartInfo
                {
                    FileName = launchConfig.Launchpath,
                    Arguments = launchConfig.Arguments,
                    UseShellExecute = true
                };*/
                

                MainTitle.Text = launchConfig.MainTitle;
                MainTitle.FontFamily = launchConfig.MaintitleFontName;
                MainTitle.Foreground = launchConfig.MainTitleFontColor;
                ChangeGameBlockText(launchConfig.ShowName,launchConfig.GamePlayedMinutes.ToString());
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
                    LaunchTile.Tag = "false";
                    ChangeStartStopStatus(true);
                    await Tools.WaitMonitingGameExitAsync(RootTabControl.SelectedIndex);
                    
                    ChangeStartStopStatus(false);
                }

                if (Variables.GameProcessStatus[RootTabControl.SelectedIndex] == false)
                {
                    
                    ChangeStartStopStatus(false);
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
                    BackgroundVideo.Pause();           
                    await Task.Delay(10);               
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
            Tools.GetShowingWindow().ShowMessageAsync("动态背景初始化错误", $"{e.ErrorException}");
        }

        private void BackgroundVideo_MediaOpened(object sender, Unosquare.FFME.Common.MediaOpenedEventArgs e)
        {
            BackgroundVideo.Play();
        }




        private void ChangeGameBlockText(string gamename,string gametime)
        {
            NewGameNameBlock.Content = $"当前游戏：{gamename}";
            NewGameTimeBlock.Content = $"游戏总时长：{gametime}分钟";
            NewGameImage.Source = Tools.LoadImageFromPath($"{Environment.CurrentDirectory}\\Backgrounds\\{launchConfig.HashCode}\\Icon.png");
        }





        private void ChangeStartStopStatus(bool ChangeMode)
        {
            if(ChangeMode)
            {
                NewLaunchLabel.Content = "";
                LaunchTile.Title = "结束游戏";
                LaunchTile.Tag = "true";
            }
            else
            {
                NewLaunchLabel.Content = "";
                LaunchTile.Title = "开始游戏";
                LaunchTile.Tag = "false";
            }
        }

        private async void NewLaunchTile_Click(object sender, RoutedEventArgs e)
        {
            if(LaunchTile.Tag == "true")//处理结束逻辑
            {
                var proc = Variables.GameProcess[RootTabControl.SelectedIndex];
                proc.Kill();
                ChangeStartStopStatus(false);
            }
            else if(LaunchTile.Tag == "false")//处理开始逻辑
            {
                try
                {
                    ChangeStartStopStatus(true);
                    BackgroundVideo.Pause();
                    Tools.StartMonitingGameStatus(RootTabControl.SelectedIndex);
                    await Tools.WaitMonitingGameExitAsync(RootTabControl.SelectedIndex);
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{launchConfig.HashCode}\\Background.mp4"))
                    {
                        BackgroundVideo.Play();
                    }

                    ChangeStartStopStatus(false);
                }
                catch (Exception ex)
                {
                    Tools.GetShowingWindow().ShowMessageAsync("游戏启动时错误", $"{ex.Message}");
                }
            }

        }

        private async void NewGameImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
    }
}
