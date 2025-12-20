using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MultiGameLauncher.Views.Windows
{
    /// <summary>
    /// OOBEWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OOBEWindow : MetroWindow
    {
        public OOBEWindow()
        {
            InitializeComponent();
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
            if (e.Key == Key.Back ||
            (e.Key == Key.Left && Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) ||
            (e.Key == Key.Right && Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)))
            {
                e.Handled = true;  // 标记事件已处理，阻止默认导航
            }
        }
    }
}
