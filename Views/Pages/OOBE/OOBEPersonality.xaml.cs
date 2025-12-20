using ControlzEx.Theming;
using HuaZi.Library.Json;
using MultiGameLauncher.Views.Windows;
using System;
using System.Collections.Generic;
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
using Color = System.Windows.Media.Color;

namespace MultiGameLauncher.Views.Pages.OOBE
{
    /// <summary>
    /// OOBEPersonality.xaml 的交互逻辑
    /// </summary>
    public partial class OOBEPersonality : Page
    {
        private List<StackPanel> animationSP = new();
        private MainConfig config;
        public OOBEPersonality()
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
                        if (((StackPanel)sp).Tag != null)
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
            if(UserName.Text != "")
            {
                config.Username = UserName.Text;
                Json.WriteJson(Variables.Configpath, config);
                var win = System.Windows.Application.Current.Windows.OfType<OOBEWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new OOBEImport());
            }
            if(UserName.Text == "")
            {
                MessageBox.Show("用户名不可置空！","警告",MessageBoxButton.OK,MessageBoxImage.Warning);

            }
            
        }

        private void ColorPalette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ThemeManager.Current.ChangeTheme(Application.Current, ThemeManager.Current.DetectTheme(Application.Current).BaseColorScheme + "." + Tools.GetColorName((Color)RootColor.SelectedValue));
                //MessageBox.Show(Tools.GetColorName((Color)RootColor.SelectedValue));
                config.ThemeColor = Tools.GetColorName((Color)RootColor.SelectedValue);
                Json.WriteJson(Variables.Configpath, config);
            }
            catch (Exception) { }
        }
    }
}
