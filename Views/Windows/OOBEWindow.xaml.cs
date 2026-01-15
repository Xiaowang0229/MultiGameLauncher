using HuaZi.Library.Json;
using MahApps.Metro.Controls;
using MultiGameLauncher.Views.Pages.OOBE;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Windows.Services.Maps;

namespace MultiGameLauncher.Views.Windows
{
    /// <summary>
    /// OOBEWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OOBEWindow : MetroWindow
    {
        private bool IsCreateNewGame = false;
        private MainConfig config;
        private bool IsOOBE;
        public OOBEWindow(bool iscreatenewgame = false)
        {
            InitializeComponent();
            IsCreateNewGame = iscreatenewgame;
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
        }

        private void RootFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
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

        private void RootWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsCreateNewGame)
            {
                RootFrame.Navigate(new OOBEImport(true));
            }
        }

        private void RootWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            if (config.GameInfos.Count == 1 && !IsCreateNewGame)
            {
                Tools.Restart();
                return;
            }

            if (config.GameInfos.Count == 0)
            {
                Application.Current.Shutdown();
            }
            
            else if (config.GameInfos.Count != 0)
            {
                var win = new MainWindow();
                win.Show();
                Tools.InitalizeConfig();

            }
        }
    }
}
