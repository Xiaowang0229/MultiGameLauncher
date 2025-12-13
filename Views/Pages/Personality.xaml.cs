using ControlzEx.Theming;
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


namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// Personality.xaml 的交互逻辑
    /// </summary>
    public partial class Personality : Page
    {
        private List<StackPanel> animationSP = new();
        public Personality()
        {
            InitializeComponent();
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

        private void ColorPalette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //MessageBox.Show(Tools.GetColorName((Color)RootColor.SelectedValue));
            try
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Light."+Tools.GetColorName((Color)RootColor.SelectedValue));
                //MessageBox.Show(Tools.GetColorName((Color)RootColor.SelectedValue));
            }
            catch(Exception ex)
            {

            }

        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
            if(Darkmode.IsOn)
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Dark." + ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);

            }
            else
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Light." + ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);

            }
        }
    }
}
