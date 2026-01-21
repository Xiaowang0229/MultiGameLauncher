using FluentIcons.Wpf;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Point = System.Windows.Point;


namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// TileClick.xaml 的交互逻辑
    /// </summary>
    public partial class TileClick : Page
    {
        private CancellationTokenSource cls = new CancellationTokenSource();
        private System.Windows.Controls.Page NavigatePage;
        public TileClick(Page _navigatepage,FluentIcons.Common.Icon _pageicon)
        {
            InitializeComponent();
            RootIcon.Icon = _pageicon;
            NavigatePage = _navigatepage;

        }


        private async void Page_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(3), cls.Token);
            }
            catch (TaskCanceledException) { }
            
            var anim = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(0.1),
            };
            RootGrid.BeginAnimation(OpacityProperty, anim);
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5), cls.Token);
            }
            catch (TaskCanceledException)
            { }
            Tools.GetMainWindow().RootFrame.Navigate(NavigatePage);
            Tools.GetMainWindow().BackButton.Width = 40;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            
                cls.Cancel();
            

        }
    }
}
