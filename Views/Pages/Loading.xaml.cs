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
        public string GamePath { get; set; }
        public string GameName { get; set; }
        public string ShowName { get; set; }
        public string Arguments { get; set; }
        public Loading()
        {
            InitializeComponent();
            InfoBlock.Text = $"正在启动 {GamePath} 请稍候……";

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

        private void Tile_Click(object sender, RoutedEventArgs e)//Stop
        {
            try
            {
                Tools.Process.Kill();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"结束游戏时发生错误：{ex.Message}","错误",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Launch());
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Launch());
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)//StartUp
        {
            try
            {
                Tools.Process.StartInfo = new ProcessStartInfo
                {
                    FileName = GamePath,
                    Arguments = $"{Arguments}",
                    UseShellExecute = true
                };
                Tools.Process.Start();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动游戏时发生错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Launch());
        }
    }
}
