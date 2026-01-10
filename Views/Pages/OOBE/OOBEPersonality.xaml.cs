using ControlzEx.Theming;
using HuaZi.Library.Json;
using MahApps.Metro.Controls.Dialogs;
using MultiGameLauncher.Views.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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
                    
                }


            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (UserName.Text != "")
            {
                config.Username = UserName.Text;
                Json.WriteJson(Variables.Configpath, config);
                var win = System.Windows.Application.Current.Windows.OfType<OOBEWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new OOBEImport());
            }
            if (UserName.Text == "")
            {
                Tools.GetShowingWindow().ShowMessageAsync("错误", $"用户名不可置空！");

            }

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
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
                    Tools.GetShowingWindow().ShowMessageAsync("错误", $"不得选择同一张头像！");
                }
                else if (openFileDialog.FileName != Environment.CurrentDirectory + @"\Head.png")
                {
                    File.Delete(Environment.CurrentDirectory + @"\Head.png");
                    File.Copy(openFileDialog.FileName, Environment.CurrentDirectory + @"\Head.png");
                    Tools.RefreshAllImageCaches(this);
                    await Task.Delay(300);
                    if (File.Exists(Environment.CurrentDirectory + @"\Head.png"))
                    {
                        UserHead.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + @"\Head.png");
                    }



                }
            }
        }

        private void ColorPalette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ThemeManager.Current.ChangeTheme(Application.Current, ThemeManager.Current.DetectTheme(Application.Current).BaseColorScheme + "." + Tools.GetColorName((Color)RootColor.SelectedValue));
                
                config.ThemeColor = Tools.GetColorName((Color)RootColor.SelectedValue);
                Json.WriteJson(Variables.Configpath, config);
            }
            catch (Exception) { }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            if (config.ThemeMode == "Dark")
            {
                Darkmode.IsOn = true;
            }
            if (File.Exists(Environment.CurrentDirectory + @"\Head.png"))
            {
                RestoreHead.Visibility = Visibility.Visible;
            }

        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (Darkmode.IsOn)
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

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var qdr = await Tools.ShowQuestionDialogMetro("确定还原默认头像吗", "警告");
            if (qdr)
            {
                File.Delete(Environment.CurrentDirectory + @"\Head.png");
                var win = System.Windows.Application.Current.Windows.OfType<OOBEWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new OOBEPersonality());
            }
        }

        private void OpenMusicFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"{Environment.CurrentDirectory}\\Musics\\",
                UseShellExecute = true
            });
        }

        private void AddMusic_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择文件",
                Filter = "音频文件(*.mp3)|*.mp3",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false
            };
            if (dialog.ShowDialog() == true)
            {
                if (File.Exists($"{Environment.CurrentDirectory}\\Musics\\{Path.GetFileName(dialog.FileName)}"))
                {
                    File.Delete($"{Environment.CurrentDirectory}\\Musics\\{Path.GetFileName(dialog.FileName)}");
                }
                File.Copy(dialog.FileName, $"{Environment.CurrentDirectory}\\Musics\\{Path.GetFileName(dialog.FileName)}");

                
            }
        }

        private void DelMusic_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择文件",
                Filter = "音频文件(*.mp3)|*.mp3",
                InitialDirectory = Environment.CurrentDirectory + "\\Musics",
                Multiselect = false
            };
            if (dialog.ShowDialog() == true)
            {
                File.Delete(dialog.FileName);
                Variables.MusicList.Clear();
                var musicdir = Directory.GetFiles(Environment.CurrentDirectory + $"\\Musics");
                for (int i = 0; i <= musicdir.Length; i++)
                {
                    Variables.MusicList.Add(musicdir[i]);
                }
            }
        }
    }
}
