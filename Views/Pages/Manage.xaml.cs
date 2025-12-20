using HuaZi.Library.Json;
using MultiGameLauncher.Views.Windows;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging.Effects;
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
    /// Manage.xaml 的交互逻辑
    /// </summary>
    public partial class Manage : Page
    {
        private List<StackPanel> animationSP = new();
        MainConfig config = new MainConfig();

        public Manage()
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
                        if(((StackPanel)sp).Tag!=null)
                        if (((StackPanel)sp).Tag.ToString() == "ani")
                        {
                            animationSP.Add((StackPanel)sp);
                        }
                    }

                    foreach (var spp in animationSP)
                    {
                        spp.Margin = new Thickness(-2000, 0, 0, 10);
                    }

                    var animation = new ThicknessAnimation
                    {
                        To = new Thickness(0, 0, 0, 10),
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < config.GameInfos.Count; i++)
            {
                var menuitem = new System.Windows.Controls.MenuItem();
                menuitem.Header = config.GameInfos[i].ShowName +" (" + i+1.ToString() + ")";
                menuitem.MouseLeftButtonDown += MenuItemSelectionChanged;
                RootDropper.Items.Add(menuitem);
            }
        }

        private async void MenuItemSelectionChanged(object sender, MouseButtonEventArgs e)
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

            

            var outanimation = new ThicknessAnimation
            {
                To = new Thickness(-2000, 0, 0, 10),
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
            };
            var inanimation = new ThicknessAnimation
            {
                To = new Thickness(0, 0, 0, 10),
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
            };

            foreach (var aniSP in animationSP)
            {
                aniSP.BeginAnimation(MarginProperty, outanimation);
                await Task.Delay(100);
            }
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            foreach (var aniSP in animationSP)
            {
                aniSP.BeginAnimation(MarginProperty, inanimation);
                await Task.Delay(100);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Launch());
            win.BackButton.Width = 0;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var createwin = new OOBEWindow(true);
            createwin.Show();
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.Close();
        }
    }
}
