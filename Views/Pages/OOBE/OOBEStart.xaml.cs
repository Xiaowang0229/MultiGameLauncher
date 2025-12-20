using MultiGameLauncher.Views.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;


namespace MultiGameLauncher.Views.Pages.OOBE
{
    /// <summary>
    /// OOBEStart.xaml 的交互逻辑
    /// </summary>
    public partial class OOBEStart : Page
    {
        private List<StackPanel> animationSP = new();
        public OOBEStart()
        {
            InitializeComponent();
            Loaded += (async (s, e) =>
            {
                try
                {
                    this.Visibility = Visibility.Collapsed;
                    await Task.Delay(TimeSpan.FromMilliseconds(300));
                    this.Visibility = Visibility.Visible;
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

                    var animationin = new ThicknessAnimation
                    {
                        From = new Thickness(-2000,0,0,10),
                        To = new Thickness(0, 0, 0, 10),
                        Duration = TimeSpan.FromSeconds(1),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                    };
                    var animationout = new ThicknessAnimation
                    {
                        To = new Thickness(2000, 0, 0, 10),
                        Duration = TimeSpan.FromSeconds(1),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                    };

                    foreach (var aniSP in animationSP)
                    {
                        aniSP.BeginAnimation(MarginProperty, animationin);
                        await Task.Delay(20);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    foreach (var aniSP in animationSP)
                    {
                        aniSP.BeginAnimation(MarginProperty, animationout);
                        await Task.Delay(20);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    var win = System.Windows.Application.Current.Windows.OfType<OOBEWindow>().FirstOrDefault();
                    win.RootFrame.Navigate(new OOBEEULA());
                }
                catch (InvalidOperationException) { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }


            });
        }
    }
}
