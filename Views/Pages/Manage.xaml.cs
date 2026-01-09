using HuaZi.Library.Json;
using MultiGameLauncher.Views.Pages.OOBE;
using MultiGameLauncher.Views.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Path = System.IO.Path;

namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// Manage.xaml 的交互逻辑
    /// </summary>
    public partial class Manage : Page
    {
        private List<StackPanel> animationSP = new();
        MainConfig config = new MainConfig();
        private bool MenuItemVisiblity = false;
        private LaunchConfig newconfig;
        private string dialogFileName;
        private bool IsBackGroundChange = false;
        public bool IsBackGroundDelete = false;

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
                        if (((StackPanel)sp).Tag != null)
                            if (((StackPanel)sp).Tag.ToString() == "ani")
                            {
                                animationSP.Add((StackPanel)sp);
                            }
                    }

                    foreach (var spp in animationSP)
                    {
                        spp.Margin = new Thickness(-2000, 0, 0, 10);
                        spp.Visibility = Visibility.Hidden;
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
                menuitem.Header = config.GameInfos[i].ShowName;
                menuitem.Tag = config.GameInfos[i].HashCode;
                menuitem.Click += MenuItemSelectionChanged;
                RootDropper.Items.Add(menuitem);
            }




        }

        private async void MenuItemSelectionChanged(object sender, RoutedEventArgs e)
        {

            if (RootDropper.Content != ((System.Windows.Controls.MenuItem)sender).Header.ToString())
            {
                RootDropper.Content = ((System.Windows.Controls.MenuItem)sender).Header.ToString();
                RootDropper.Tag = ((System.Windows.Controls.MenuItem)sender).Tag.ToString();
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




                    var outanimation = new ThicknessAnimation
                    {
                        From = new Thickness(0, 0, 0, 10),
                        To = new Thickness(-1500, 0, 0, 10),
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                    };
                    var inanimation = new ThicknessAnimation
                    {
                        From = new Thickness(-2000, 0, 0, 10),
                        To = new Thickness(0, 0, 0, 10),
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                    };




                    foreach (var aniSP in animationSP)
                    {
                        if (aniSP.Visibility == Visibility.Visible)
                        {
                            aniSP.BeginAnimation(MarginProperty, outanimation);
                            await Task.Delay(100);
                        }
                    }

                    if (MenuItemVisiblity)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(600));
                    }




                    //初始化控件状态
                    //e.g. RootDropper.Content = ((System.Windows.Controls.MenuItem)sender).Header.ToString();
                    var currentgameinfo = config.GameInfos.FirstOrDefault(x => x.HashCode == ((System.Windows.Controls.MenuItem)sender).Tag.ToString());
                    newconfig = currentgameinfo;

                    LaunchPathView.Content = currentgameinfo.Launchpath + " " + currentgameinfo.Arguments;
                    ApplicationNameBlock.Content = currentgameinfo.ShowName;
                    MainTitleBlock.Content = currentgameinfo.MainTitle;
                    PlayedTimeBlock.Content = currentgameinfo.GamePlayedMinutes.ToString() + "分钟";
                    ApplicationIcon.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");

                    sp_ani.Visibility = Visibility.Visible;
                    foreach (var aniSP in animationSP)
                    {
                        aniSP.Visibility = Visibility.Visible;
                        aniSP.BeginAnimation(MarginProperty, null);
                        aniSP.BeginAnimation(MarginProperty, inanimation);
                        await Task.Delay(100);
                    }
                }
                catch (Exception ex)
                {

                }
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
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new OOBEImport(true));
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (RootDropper.Content != "请选择对象")
            {
                if (MessageBox.Show($"警告：操作不可逆，请确认您是否要删除当前项目:{RootDropper.Content}", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    //config.GameInfos.RemoveAll(x => x.HashCode == RootDropper.Tag);
                    var deleteitemindex = newconfig.HashCode;
                    config.GameInfos.RemoveAt(Tools.FindHashcodeinGameinfosint(config, deleteitemindex));
                    Variables.GameProcess.RemoveAt(Tools.FindHashcodeinGameinfosint(config, deleteitemindex));
                    Variables.GameProcessStatus.RemoveAt(Tools.FindHashcodeinGameinfosint(config, deleteitemindex));
                    Variables.PlayingTimeintList.RemoveAt(Tools.FindHashcodeinGameinfosint(config, deleteitemindex));
                    Variables.PlayingTimeRecorder.RemoveAt(Tools.FindHashcodeinGameinfosint(config, deleteitemindex));
                    Json.WriteJson(Variables.Configpath, config);
                    Directory.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\", true);
                    MessageBox.Show("操作成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (config.GameInfos.Count == 0)
                    {
                        Tools.KillTaskBar();
                        var win3 = new OOBEWindow(true);
                        win3.Show();
                        var win2 = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                        win2.Close();
                        return;
                    }
                    var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    win.RootFrame.Navigate(new Manage());

                }
            }
            else
            {
                MessageBox.Show($"请选择对象后再进行当前操作", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {


            if (newconfig.Launchpath != "" && newconfig.MainTitle != "" && newconfig.ShowName != "")
            {
                BackgroundCopyTip.Visibility = Visibility.Visible;
                /*config.GameInfos.RemoveAll(x => x.HashCode == RootDropper.Tag);
                
                config.GameInfos.Add(newconfig);*/


                config.GameInfos[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)] = newconfig;
                ApplicationIcon.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");
                //await Task.Delay(100);
                this.IsEnabled = false;
                if (IsBackGroundChange)
                {
                    if (!Directory.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}"))
                    {
                        Directory.CreateDirectory(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}");
                    }
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background.png"))
                    {
                        File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background.png");
                    }
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background.mp4"))
                    {
                        File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background.mp4");
                    }
                    File.Copy(dialogFileName, Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(dialogFileName));
                }

                if (IsBackGroundDelete)
                {
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background.png"))
                    {
                        File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background.png");
                    }
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background.mp4"))
                    {
                        File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background.mp4");
                    }
                }

                BackgroundCopyTip.Visibility = Visibility.Hidden;






                //MessageBox.Show(config.GameInfos[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)].Launchpath);
                Json.WriteJson(Variables.Configpath, config);
                this.IsEnabled = true;
                await Task.Delay(100);
                MessageBox.Show("操作成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new Manage());
            }
            else
            {
                MessageBox.Show("请将带星号的必填项填写完整，以便正常写入Json文件进行程序启动！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }







        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择文件",
                Filter = "背景文件(*.png;*.webp;*.bmp;*.jpg;*.jpeg;*.mp4)|*.png;*.webp;*.bmp;*.jpg;*.jpeg;*.mp4",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false
            };
            if (dialog.ShowDialog() == true)
            {

                dialogFileName = dialog.FileName;
                IsBackGroundChange = true;

                await Task.Delay(500);
                Directory.CreateDirectory(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}");
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(dialog.FileName)))
                {
                    File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(dialog.FileName));
                }
                File.Copy(dialog.FileName, Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(dialog.FileName));
                BackgroundCopyTip.Visibility = Visibility.Hidden;
                MessageBox.Show($"设置成功，路径为:{dialog.FileName}", "提示", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            else
            {
                if (MessageBox.Show($"您什么也没有设置，请问是否清除已有的背景？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background.mp4"))
                    {
                        File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background.mp4");
                    }
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background.png"))
                    {
                        File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background.png");
                    }
                }
            }
        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择文件",
                Filter = "可执行文件(*.exe)|*.exe",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false
            };
            if (dialog.ShowDialog() == true)
            {

                newconfig.Launchpath = dialog.FileName;
                LaunchPathView.Content = dialog.FileName;
                string a = await Tools.OpenInputWindow("请输入参数，无可置空");
                if (a != null)
                {
                    newconfig.Arguments = a;


                }
                if (MessageBox.Show("是否使用exe内的图标替换旧图标？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png"))
                    {
                        File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");
                    }
                    Tools.ExtractExeIconToPng(dialog.FileName, Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");
                    ApplicationIcon.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");
                    var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    win.RootFrame.Navigate(new Manage());
                }

                //MessageBox.Show("1");
                try
                {
                    Variables.GameProcess[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)].Kill();
                    Variables.GameProcess[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)].Close();
                }
                catch { }
                Variables.GameProcess[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)].StartInfo = new ProcessStartInfo
                {
                    FileName = newconfig.Launchpath,
                    Arguments = newconfig.Arguments,
                    UseShellExecute = true
                };
                Variables.GameProcessStatus[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)] = false;
            }
        }

        private void ArgumentsBlock_LostFocus(object sender, RoutedEventArgs e)
        {

            try
            {
                Variables.GameProcess[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)].Kill();
                Variables.GameProcess[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)].Close();
            }
            catch { }
            Variables.GameProcess[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)].StartInfo = new ProcessStartInfo
            {
                FileName = newconfig.Launchpath,
                Arguments = newconfig.Arguments,
                UseShellExecute = true
            };
        }



        private void DeleteBackground_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ChangeApplicationName_Click(object sender, RoutedEventArgs e)
        {
            string a = await Tools.OpenInputWindow("请输入新程序名");
            if (a != null)
            {
                newconfig.ShowName = a;
                config.GameInfos[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)] = newconfig;
                Json.WriteJson(Variables.Configpath, config);
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new Manage());
            }


        }

        private async void ChangeTitle_Click(object sender, RoutedEventArgs e)
        {
            string a =await Tools.OpenInputWindow("请输入新标题文本");
            if (a != null)
            {
                newconfig.MainTitle = a;
                config.GameInfos[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)] = newconfig;
                Json.WriteJson(Variables.Configpath, config);

            }

            var fontdialog = new System.Windows.Forms.FontDialog();
            var colorDialog = new ColorDialog();

            if (fontdialog.ShowDialog() == DialogResult.OK)
            {
                newconfig.MaintitleFontName = new System.Windows.Media.FontFamily(fontdialog.Font.FontFamily.Name);

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    newconfig.MainTitleFontColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(
                    colorDialog.Color.A,
                    colorDialog.Color.R,
                    colorDialog.Color.G,
                    colorDialog.Color.B));



                    config.GameInfos[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)] = newconfig;
                    Json.WriteJson(Variables.Configpath, config);


                }
                config.GameInfos[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)] = newconfig;
                Json.WriteJson(Variables.Configpath, config);

            }

            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new Manage());
        }

        private void DeleteTime_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"您确定要清空游玩的时间吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                newconfig.GamePlayedMinutes = 0;
                config.GameInfos[Tools.FindHashcodeinGameinfosint(config, newconfig.HashCode)] = newconfig;
                Json.WriteJson(Variables.Configpath, config);
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new Manage());
            }
        }

        private void ChangeApplicationIcon_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择文件",
                Filter = "图标文件(*.png;*.webp;*.bmp;*.jpg;*.jpeg)|*.png;*.webp;*.bmp;*.jpg;*.jpeg",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false
            };
            if (dialog.ShowDialog() == true)
            {

                try
                {
                    if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png"))
                    {
                        File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");
                    }
                    File.Copy(dialog.FileName, Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");
                    Tools.RefreshAllImageCaches(this);
                    ApplicationIcon.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
    }
}
