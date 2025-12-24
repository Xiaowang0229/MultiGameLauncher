using HuaZi.Library.Json;
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
                        if(((StackPanel)sp).Tag!=null)
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
            if(config.GameInfos.Count == 0)
            {
                var createwin = new OOBEWindow(true);
                createwin.Show();
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.Close();
                return;
            }
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
            
            RootDropperText.Content = ((System.Windows.Controls.MenuItem)sender).Header.ToString();
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
                From = new Thickness(0,0,0,10),
                To = new Thickness(-1500, 0, 0, 10),
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
            };
            var inanimation = new ThicknessAnimation
            {
                From = new Thickness(-2000,0,0,10),
                To = new Thickness(0, 0, 0, 10),
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
            };

            


            foreach (var aniSP in animationSP)
            {
                if(aniSP.Visibility == Visibility.Visible)
                {
                    aniSP.BeginAnimation(MarginProperty, outanimation);
                    await Task.Delay(100);
                }
            }
            
            if(MenuItemVisiblity)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(600));
            }


            MenuItemVisiblity = true;

            //初始化控件状态
            //e.g. RootDropper.Content = ((System.Windows.Controls.MenuItem)sender).Header.ToString();
            var currentgameinfo =  config.GameInfos.FirstOrDefault(x => x.HashCode == ((System.Windows.Controls.MenuItem)sender).Tag.ToString());
            newconfig = currentgameinfo;
            ProgramNameBlock.Text = currentgameinfo.ShowName;
            MainTitleBox.Text = currentgameinfo.MainTitle;
            SubTitleBox.Text = currentgameinfo.SubTitle;
            ArgumentsBlock.Text = currentgameinfo.Arguments;

            Fontview.FontFamily = currentgameinfo.MaintitleFontName;
            Fontview.Foreground = currentgameinfo.MainTitleFontColor;
            Backgroundview.Content = "未设置壁纸";
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{currentgameinfo.HashCode}\\Background.mp4"))
            {
                DeleteBackground.Visibility = Visibility.Visible;
                Backgroundview.Content = "动态壁纸";
            }
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{currentgameinfo.HashCode}\\Background.png"))
            {
                DeleteBackground.Visibility = Visibility.Visible;
                Backgroundview.Content = "静态壁纸";
            }
            LaunchPathView.Content = currentgameinfo.Launchpath;




                foreach (var aniSP in animationSP)
                {
                    aniSP.Visibility = Visibility.Visible;
                    aniSP.BeginAnimation(MarginProperty, null);
                    aniSP.BeginAnimation(MarginProperty, inanimation);
                    await Task.Delay(100);
                }
            }
            catch(Exception ex)
            {
                
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

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show($"警告：操作不可逆，请确认您是否要删除当前项目:{RootDropper.Content}","警告",MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //config.GameInfos.RemoveAll(x => x.HashCode == RootDropper.Tag);
                var deleteitemindex = newconfig.HashCode;
                config.GameInfos.RemoveAt(Tools.FindHashcodeinGameinfosint(config, deleteitemindex));
                Variables.GameProcess.RemoveAt(Tools.FindHashcodeinGameinfosint(config, deleteitemindex));
                Variables.GameProcessStatus.RemoveAt(Tools.FindHashcodeinGameinfosint(config, deleteitemindex));
                Json.WriteJson(Variables.Configpath, config);
                Directory.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}");
                MessageBox.Show("操作成功","提示",MessageBoxButton.OK,MessageBoxImage.Information);
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new Manage());

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
               
                //await Task.Delay(100);
                this.IsEnabled = false;
                if(IsBackGroundChange)
                {
                    if(!Directory.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}"))
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

                if(IsBackGroundDelete)
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

        private void ProgramNameBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            newconfig.ShowName = ProgramNameBlock.Text;
        }

        private void MainTitleBox_LostFocus(object sender, RoutedEventArgs e)
        {
            newconfig.MainTitle = MainTitleBox.Text;
        }

        private void Font_Click(object sender, RoutedEventArgs e)
        {
            var fontdialog = new System.Windows.Forms.FontDialog();
            var colorDialog = new ColorDialog();

            if (fontdialog.ShowDialog() == DialogResult.OK)
            {
                newconfig.MaintitleFontName = new System.Windows.Media.FontFamily(fontdialog.Font.FontFamily.Name);

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    newconfig.MaintitleFontName = new System.Windows.Media.FontFamily(fontdialog.Font.FontFamily.Name);
                    newconfig.MainTitleFontColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(
                    colorDialog.Color.A,
                    colorDialog.Color.R,
                    colorDialog.Color.G,
                    colorDialog.Color.B));



                    Fontview.Foreground = newconfig.MainTitleFontColor;

                }
                Fontview.FontFamily = newconfig.MaintitleFontName;

            }
        }

        private void SubTitleBox_LostFocus(object sender, RoutedEventArgs e)
        {
            newconfig.SubTitle = SubTitleBox.Text;
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择文件",
                Filter = "背景文件(*.png;*.mp4)|*.png;*.mp4",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false
            };
            if (dialog.ShowDialog() == true)
            {
                
                dialogFileName = dialog.FileName;
                IsBackGroundChange = true;
                if(Path.GetExtension(dialogFileName) == ".mp4")
                {
                    Backgroundview.Content = "动态背景";
                }
                if (Path.GetExtension(dialogFileName) == ".png")
                {
                    Backgroundview.Content = "静态背景";
                }
                /*await Task.Delay(500);
                Directory.CreateDirectory(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}");
                if(File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(dialog.FileName)))
                {
                    File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(dialog.FileName));
                }
                File.Copy(dialog.FileName, Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(dialog.FileName));
                BackgroundCopyTip.Visibility = Visibility.Hidden;
                MessageBox.Show($"设置成功，路径为:{dialog.FileName}", "提示", MessageBoxButton.OK, MessageBoxImage.Information);*/


            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
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
            newconfig.Arguments = ArgumentsBlock.Text;
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
            if (MessageBox.Show("确定删除背景吗？此操作不可逆", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                IsBackGroundDelete = true;
                DeleteBackground.Visibility = Visibility.Hidden;
            }
        }
    }
}
