using ControlzEx.Theming;
using HuaZi.Library.Json;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;


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
                ThemeManager.Current.ChangeTheme(Application.Current, ThemeManager.Current.DetectTheme(Application.Current).BaseColorScheme + "." + Tools.GetColorName((Color)RootColor.SelectedValue));
                //MessageBox.Show(Tools.GetColorName((Color)RootColor.SelectedValue));
                config.ThemeColor = Tools.GetColorName((Color)RootColor.SelectedValue);
                Json.WriteJson(Variables.Configpath, config);
            }
            catch (Exception) { }

        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(ThemeManager.Current.DetectTheme(Application.Current).ColorScheme);
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //UserHead.Source = Tools.ConvertByteArrayToImageSource(ApplicationResources.UserIcon);
            if (File.Exists(Environment.CurrentDirectory + @"\Head.png"))
            {
                UserHead.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + @"\Head.png");
            }
            UserName.Text = config.Username;

            if (config.ChangeThemeWithSystem)
            {
                Darkmode.IsEnabled = false;
                SystemChangeTip.Visibility = Visibility.Visible;
            }
            else
            {
                if (config.ThemeMode == "Dark")
                {
                    Darkmode.IsOn = true;
                }
            }

            if (File.Exists(Environment.CurrentDirectory + @"\Head.png"))
            {
                RestoreHead.Visibility = Visibility.Visible;
            }

            for(int i = 0;i<config.MusicInfos.Count;i++)
            {
                var menuitem = new System.Windows.Controls.MenuItem();
                menuitem.Header = config.MusicInfos[i].MusicShowName;
                menuitem.Tag = config.MusicInfos[i].MusicHashCode;
                menuitem.Click += Menuitem_Click;
                MusicChooser.Items.Add(menuitem);
            }
            if(config.MusicInfos.Count == 0)
            {
                MusicChooser.Content = "请先添加音乐再来管理哦";
            }

        }

        private void Menuitem_Click(object sender, RoutedEventArgs e)
        {
            if (MusicChooser.Content != ((System.Windows.Controls.MenuItem)sender).Header.ToString())
            {
                MusicChooser.Content = ((System.Windows.Controls.MenuItem)sender).Header.ToString();
                MusicChooser.Tag = ((System.Windows.Controls.MenuItem)sender).Tag.ToString();
                CostomMusicControl.Visibility = Visibility.Visible;
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
                    MessageBox.Show("不能选择同一张头像", "错误", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                else if (openFileDialog.FileName != Environment.CurrentDirectory + @"\Head.png")
                {
                    File.Delete(Environment.CurrentDirectory + @"\Head.png");
                    File.Copy(openFileDialog.FileName, Environment.CurrentDirectory + @"\Head.png");
                    Tools.RefreshAllImageCaches(this);
                    MessageBox.Show("操作成功,为了保证您的游戏体验,重启后生效！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定还原默认头像吗", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                File.Delete(Environment.CurrentDirectory + @"\Head.png");
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new Personality());
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

                var MusicGame = Tools.OpenInputWindow("请输入歌曲名，推荐格式：[歌曲名] - [歌手] (置空则取文件名)");
                if (File.Exists($"{Environment.CurrentDirectory}\\Musics\\{Path.GetFileName(dialog.FileName)}"))
                {
                    File.Delete($"{Environment.CurrentDirectory}\\Musics\\{Path.GetFileName(dialog.FileName)}");
                }
                File.Copy(dialog.FileName, $"{Environment.CurrentDirectory}\\Musics\\{Path.GetFileName(dialog.FileName)}");
                var newmusicconfig = new MusicConfig
                {
                    MusicHashCode = Tools.RandomHashGenerate()
                };
                if (MusicGame != null)
                {
                    

                    try
                    {
                        newmusicconfig.MusicShowName = MusicGame;
                        newmusicconfig.MusicPath = $"{Environment.CurrentDirectory}\\Musics\\{Path.GetFileName(dialog.FileName)}";
                        config.MusicInfos.Add(newmusicconfig);
                        Json.WriteJson(Variables.Configpath, config);
                        /*Variables.MusicList.Clear();
                        var musicdir = Directory.GetFiles(Environment.CurrentDirectory + $"\\Musics");
                        for (int i = 0; i < musicdir.Length; i++)
                        {
                            Variables.MusicList.Add(musicdir[i]);
                        }*/
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"错误:{ex.Message}", "发生错误");
                    }
                }
                else if(MusicGame == null)
                {
                    try
                    {
                        newmusicconfig.MusicShowName = $"{Environment.CurrentDirectory}\\Musics\\{Path.GetFileName(dialog.FileName)}";
                        newmusicconfig.MusicPath = $"{Environment.CurrentDirectory}\\Musics\\{Path.GetFileName(dialog.FileName)}";
                        config.MusicInfos.Add(newmusicconfig);
                        Json.WriteJson(Variables.Configpath, config);
                        /*Variables.MusicList.Clear();
                        var musicdir = Directory.GetFiles(Environment.CurrentDirectory + $"\\Musics");
                        for (int i = 0; i < musicdir.Length; i++)
                        {
                            Variables.MusicList.Add(musicdir[i]);
                        }*/
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"错误:{ex.Message}", "发生错误");
                    }
                }
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                //win.Show();
                win.RootFrame.Navigate(new Personality());
            }
        }

        private void DelMusic_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show($"确实要删除{MusicChooser.Content}吗?,操作不可逆！","警告",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)
            {
                var deleteitem = MusicChooser.Tag;
                File.Delete(config.MusicInfos[Tools.FindHashcodeinGameinfosint(config,MusicChooser.Tag.ToString())].MusicPath);
                config.MusicInfos.RemoveAt(Tools.FindHashcodeinGameinfosint(config, MusicChooser.Tag.ToString()));
                Json.WriteJson(Variables.Configpath, config);
                MessageBox.Show("操作成功！","提示",MessageBoxButton.OK,MessageBoxImage.Information);
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new Personality());
            }
        }

        private void ReNameMusic_Click(object sender, RoutedEventArgs e)
        {
            var newname = Tools.OpenInputWindow("请输入新音乐名");
            if(newname != null)
            {
                config.MusicInfos[Tools.FindHashcodeinGameinfosint(config, MusicChooser.Tag.ToString())].MusicShowName = newname;
                Json.WriteJson(Variables.Configpath, config);
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                //win.Show();
                win.RootFrame.Navigate(new Personality());
            }
        }
    }
}
