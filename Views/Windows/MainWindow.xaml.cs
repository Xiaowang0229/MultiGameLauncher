using HuaZi.Library.Json;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MultiGameLauncher.Views.Pages;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace MultiGameLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : MetroWindow
    {
        private static CancellationTokenSource TileLoadingTokenSource = new CancellationTokenSource();
        public MainWindow()
        {


            InitializeComponent();




        }



        private async void RootWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ShowTileLoadingAsync(FluentIcons.Common.Icon.Home,new Launch());
        }


        private async void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {

            
            //Animation
            var newPage = (FrameworkElement)RootFrame.Content;

            {
                newPage.BeginAnimation(MarginProperty, null);

                newPage.Margin = new Thickness(-2000, 0, 0, -10);



                var slideIn = new ThicknessAnimation
                {
                    To = new Thickness(0, 0, 0, 10),
                    Duration = TimeSpan.FromMilliseconds(500),
                    EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                };


                newPage.BeginAnimation(MarginProperty, slideIn);
            }
            





        }



        private async void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            



        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(new Launch());
            BackButton.Width = 0;

        }

        private void RootFrame_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.OriginalSource is System.Windows.Controls.Primitives.TextBoxBase)
            {
                return;
            }
            if (e.Key == Key.Back ||
            (e.Key == Key.Left && Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) ||
            (e.Key == Key.Right && Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)))
            {
                e.Handled = true;  // 标记事件已处理，阻止默认导航
            }
        }

        private void RootWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {



            foreach (var i in Variables.GameProcessStatus)
            {
                if (i == true)
                {
                    this.Hide();
                    e.Cancel = true;
                    Page currentPage = RootFrame.Content as Page;
                    var config = Json.ReadJson<MainConfig>(Variables.Configpath);
                    if (currentPage is Launch launchpage)
                    {
                        if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{config.GameInfos[launchpage.TabIndex].HashCode}\\Background.mp4"))
                        {
                            launchpage.BackgroundImage.Visibility = Visibility.Hidden;
                            launchpage.BackgroundVideo.Visibility = Visibility.Visible;
                            launchpage.BackgroundVideo.Pause();
                        }
                    }
                    return;
                }
            }
        }

        public async void GametimeAlarm()
        {
            await Tools.StartAlarmMonitingAsync();
        }

        public void ShowUpdatePreparing()
        {
            var dialog = new CustomDialog(this.MetroDialogOptions) { Content = "启动时不会使用流量，请稍候……", Title = "启动更新中" };
            this.ShowMetroDialogAsync(dialog);
        }

        public async Task ShowTileLoadingAsync(FluentIcons.Common.Icon LoadingSymbolIcon, Page TargetPage)
        {
            var cfg = Json.ReadJson<MainConfig>(Variables.Configpath);
            if(cfg.DisableTileLoadingScreen != true)
            {
                TileLoadingTokenSource.Cancel();
                TileLoadingTokenSource = new CancellationTokenSource();
                var AnimationDuration = TimeSpan.FromMilliseconds(150);
                try
                {
                    TileLoadingIcon.Visibility = Visibility.Hidden;
                    TileLoading.Opacity = 0;
                    TileLoading.Visibility = Visibility.Visible;
                    var Gridin = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        Duration = AnimationDuration,

                    };


                    TileLoading.BeginAnimation(OpacityProperty, null);
                    TileLoadingIcon.BeginAnimation(OpacityProperty, null);

                    TileLoading.Visibility = Visibility.Visible;
                    TileLoading.BeginAnimation(OpacityProperty, Gridin);
                    await Task.Delay(AnimationDuration, TileLoadingTokenSource.Token);
                    TileLoadingIcon.Icon = LoadingSymbolIcon;
                    TileLoadingIcon.Visibility = Visibility.Visible;
                    var animationin = new DoubleAnimation
                    {
                        From = 0.0,
                        To = 1.0,
                        Duration = AnimationDuration,

                    };
                    TileLoadingIcon.BeginAnimation(OpacityProperty, animationin);
                    RootFrame.Navigate(TargetPage);
                    await Task.Delay(TimeSpan.FromSeconds(0.75), TileLoadingTokenSource.Token);
                    var animationout = new DoubleAnimation
                    {
                        From = 1.0,
                        To = 0.0,
                        Duration = AnimationDuration,
                    };
                    TileLoadingIcon.BeginAnimation(OpacityProperty, animationout);
                    await Task.Delay(AnimationDuration, TileLoadingTokenSource.Token);
                    TileLoadingIcon.Visibility = Visibility.Hidden;
                    var Gridout = new DoubleAnimation
                    {

                        From = 1,
                        To = 0,
                        Duration = AnimationDuration,


                    };
                    TileLoading.BeginAnimation(OpacityProperty, Gridout);
                    await Task.Delay(AnimationDuration, TileLoadingTokenSource.Token);
                    TileLoading.Visibility = Visibility.Hidden;

                }
                catch (TaskCanceledException)
                {
                    
                    return;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    TileLoadingIcon.Visibility = Visibility.Hidden;
                    TileLoading.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                RootFrame.Navigate(TargetPage);
            }

        }
    }
}