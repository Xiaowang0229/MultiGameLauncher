using HuaZi.Library.Json;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MultiGameLauncher.Views.Pages;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace MultiGameLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : MetroWindow
    {
        private const int WM_SIZING = 0x0214;
        private const int WMSZ_LEFT = 1;
        private const int WMSZ_RIGHT = 2;
        private const int WMSZ_TOP = 3;
        private const int WMSZ_TOPLEFT = 4;
        private const int WMSZ_TOPRIGHT = 5;
        private const int WMSZ_BOTTOM = 6;
        private const int WMSZ_BOTTOMLEFT = 8;
        private const int WMSZ_BOTTOMRIGHT = 7;

        // 你想要保持的比例（宽:高）
        private readonly double _aspectRatio = 10.0 / 6.0;  // ← 改成你想要的比例
        public MainWindow()
        {


            InitializeComponent();

            


        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hwnd)?.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SIZING)
            {
                var rect = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));

                double width = rect.Right - rect.Left;
                double height = rect.Bottom - rect.Top;

                switch ((int)wParam)
                {
                    // 左右边缘拖动 → 调整高度
                    case WMSZ_LEFT:
                    case WMSZ_RIGHT:
                        height = width / _aspectRatio;
                        rect.Bottom = rect.Top + (int)height;
                        break;

                    // 上下边缘拖动 → 调整宽度
                    case WMSZ_TOP:
                    case WMSZ_BOTTOM:
                        width = height * _aspectRatio;
                        rect.Right = rect.Left + (int)width;
                        break;

                    // 四个角（最常用情况）
                    case WMSZ_TOPLEFT:
                    case WMSZ_TOPRIGHT:
                    case WMSZ_BOTTOMLEFT:
                    case WMSZ_BOTTOMRIGHT:
                        // 以宽度为准（也可以改成以高度为准，看你更想要哪个主导）
                        height = width / _aspectRatio;
                        rect.Bottom = rect.Top + (int)height;
                        break;
                }

                Marshal.StructureToPtr(rect, lParam, true);
                handled = true;
            }

            return IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
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