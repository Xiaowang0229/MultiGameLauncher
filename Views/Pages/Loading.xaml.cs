using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// Loading.xaml 的交互逻辑
    /// </summary>
    public partial class Loading : Page
    {
        private List<StackPanel> animationSP = new();
        private string Gamepath;
        private string Gamename;
        private string Showname;
        public Loading(string Gamepath)
        {
            InitializeComponent();
            InfoBlock.Text = $"正在启动 {Gamepath} 请稍候……";
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
                        spp.Margin = new Thickness(0, 0, 0, 10);
                    }

                    var animation = new ThicknessAnimation
                    {
                        To = new Thickness(0, 0, 0, 10),
                        Duration = TimeSpan.FromMilliseconds(700),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseInOut }
                    };

                    foreach (var aniSP in animationSP)
                    {
                        aniSP.BeginAnimation(MarginProperty, animation);
                        await Task.Delay(100);
                    }
                }
                catch (InvalidOperationException) { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }


            });

        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "taskkill.exe",
                    Arguments = "-f -im {}",
                    UseShellExecute = true
                });
            }
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Launch());
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Launch());
        }
    }
}
