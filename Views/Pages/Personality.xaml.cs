using ControlzEx.Theming;
using HuaZi.Library.Json;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;
using System.IO;


namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// Personality.xaml 的交互逻辑
    /// </summary>
    public partial class Personality : Page
    {
        private List<StackPanel> animationSP = new();
        private MainConfig config;
        public Personality()
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

        private void ColorPalette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //MessageBox.Show(Tools.GetColorName((Color)RootColor.SelectedValue));
            try
            {
                ThemeManager.Current.ChangeTheme(Application.Current, ThemeManager.Current.DetectTheme(Application.Current).BaseColorScheme +"."+ Tools.GetColorName((Color)RootColor.SelectedValue));
                //MessageBox.Show(Tools.GetColorName((Color)RootColor.SelectedValue));
                config.ThemeColor = Tools.GetColorName((Color)RootColor.SelectedValue);
                Json.WriteJson(Variables.Configpath,config);
            }
            catch(Exception){ }

        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
            if(Darkmode.IsOn)
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Dark." + ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
                config.ThemeMode = "Dark";
                Json.WriteJson(Variables.Configpath, config);
            }
            else
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Light." + ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
                config.ThemeMode = "Light";
                Json.WriteJson(Variables.Configpath, config);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //UserHead.Source = Tools.ConvertByteArrayToImageSource(ApplicationResources.UserIcon);
            UserHead.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + @"\Head.png");
            UserName.Text = config.Username;
            if(config.ThemeMode == "Dark")
            {
                Darkmode.IsOn = true;
            }
        }

        private void ChooseBackground_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (UserName.Text != "")
            {
                config.Username = UserName.Text;
                Json.WriteJson(Variables.Configpath, config);
            }
            if (UserName.Text == "")
            {
                MessageBox.Show("用户名不可置空！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                
                Title = "选择文件",
                Filter = "头像图片(*.png)|*.png",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false

            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                if (openFileDialog.FileName == Environment.CurrentDirectory + @"\Head.png")
                {
                    MessageBox.Show("不能选择同一张头像","错误",MessageBoxButton.OK,MessageBoxImage.Stop);
                }
                else if(openFileDialog.FileName!= Environment.CurrentDirectory + @"\Head.png")
                {
                    File.Delete(Environment.CurrentDirectory + @"\Head.png");
                    File.Copy(openFileDialog.FileName, Environment.CurrentDirectory + @"\Head.png");
                    MessageBox.Show("操作成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    Tools.Restart();
                }
            }
        }

        private void UserName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UserName.Text != "")
            {
                config.Username = UserName.Text;
                Json.WriteJson(Variables.Configpath, config);
            }
            if (UserName.Text == "")
            {
                MessageBox.Show("用户名不可置空！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
