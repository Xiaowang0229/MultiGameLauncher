using HuaZi.Library.Json;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MultiGameLauncher.Views.Pages;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Point = System.Windows.Point;

namespace MultiGameLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : MetroWindow
    {

        public MainWindow()
        {


            InitializeComponent();




        }



        private async void RootWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {


        }


        private async void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {

            //Animation
            var newPage = (FrameworkElement)RootFrame.Content;

            if (!(newPage is TileClick))
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
            else
            {
                // 获取窗口的中心点
                Point windowCenter = new Point(this.ActualWidth / 2, this.ActualHeight / 2);

                // 获取控件的中心点
                Point controlCenter = new Point(newPage.RenderSize.Width / 2, newPage.RenderSize.Height / 2);

                // 计算控件相对窗口中心的偏移量
                double offsetX = windowCenter.X - controlCenter.X;
                double offsetY = windowCenter.Y - controlCenter.Y;

                
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
    }
}