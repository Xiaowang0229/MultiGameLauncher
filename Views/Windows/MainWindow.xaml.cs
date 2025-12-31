using HuaZi.Library.Json;
using MahApps.Metro.Controls;
using MultiGameLauncher.Views.Pages;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

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

            RootWindow.Title = $"Rocket Launcher {Variables.ShowVersion}";




        }



        private async void RootWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

            //RootWindow.Icon = Tools.ConvertByteArrayToImageSource(ApplicationResources.ApplicationIcon);
            //RootFrame.Navigate();


        }


        private async void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {

            //Animation
            //await Task.Delay(1000);
            var newPage = (FrameworkElement)RootFrame.Content;

            newPage.BeginAnimation(MarginProperty, null);

            newPage.Margin = new Thickness(-2000, 0, 0, -10);



            var slideIn = new ThicknessAnimation
            {
                To = new Thickness(0, 0, 0, 10),
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
            };


            newPage.BeginAnimation(MarginProperty, slideIn);
            /*if (e.Content is FrameworkElement newPage)
            {
                newPage.BeginAnimation(FrameworkElement.MarginProperty, null);
                newPage.Margin = new Thickness(3000, 0, 0, 0);

                var slideIn = (Storyboard)FindResource("SlideInStoryboard");
                slideIn.Begin(newPage, true);

                OldPage = newPage;

                _pendingNewPage = null; // 清空临时变量
            }*/



        }






        private async void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {

            /*if(RootFrame.Content != null)
            {
                 var oldPage = (FrameworkElement)RootFrame.Content;

                 oldPage.BeginAnimation(MarginProperty, null);





                 var slideOut = new ThicknessAnimation
                 {
                     From = new Thickness(0,0,0,0),
                     To = new Thickness(2000, 0, 0, 10),
                     Duration = TimeSpan.FromMilliseconds(500),
                     EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                 };


                 oldPage.BeginAnimation(MarginProperty, slideOut);
                 //await Task.Delay(1000);
            }*/
            /*if (RootFrame.Content is FrameworkElement oldPage && oldPage != null)
            {
                oldPage.BeginAnimation(FrameworkElement.MarginProperty, null);
                oldPage.Margin = new Thickness(0);

                var slideOut = (Storyboard)FindResource("SlideOutStoryboard");
                slideOut.Begin(oldPage, true);

                e.Cancel = true;

                await Task.Delay(550);

                oldPage.Visibility = Visibility.Collapsed;
                OldPage = null;

                // 【关键】直接用 e.Content（就是你 Navigate 时传入的新页面实例）
                if (e.Content is FrameworkElement newPage)
                {
                    // 手动设置新页面
                    RootFrame.Content = newPage;
                }
            }*/


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
    }
}