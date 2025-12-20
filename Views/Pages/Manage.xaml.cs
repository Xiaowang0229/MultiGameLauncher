using HuaZi.Library.Json;
using MahApps.Metro.Controls;
using MultiGameLauncher.Views.Windows;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging.Effects;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
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
            
            RootDropper.Content = ((System.Windows.Controls.MenuItem)sender).Header.ToString();
            RootDropper.Tag = ((System.Windows.Controls.MenuItem)sender).Tag.ToString();

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
                To = new Thickness(-600, 0, 0, 10),
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



            foreach (var aniSP in animationSP)
            {
                aniSP.Visibility = Visibility.Visible;
                aniSP.BeginAnimation(MarginProperty, null);
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

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show($"警告：操作不可逆，请确认您是否要删除当前项目:{RootDropper.Content}","警告",MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                config.GameInfos.RemoveAll(x => x.HashCode == RootDropper.Tag);
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
                config.GameInfos.RemoveAll(x => x.HashCode == RootDropper.Tag);
                config.GameInfos.Add(newconfig);
                await Task.Delay(500);
                Directory.CreateDirectory(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}");
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(dialogFileName)))
                {
                    File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(dialogFileName));
                }
                File.Copy(dialogFileName, Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(dialogFileName));
                BackgroundCopyTip.Visibility = Visibility.Hidden;

                Json.WriteJson(Variables.Configpath, config);
                MessageBox.Show("操作成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                win.RootFrame.Navigate(new Manage());
            }
            else
            {
                MessageBox.Show("请将启动路径，程序名称和主标题填写完整，以便正常写入Json文件进行程序启动！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    MessageBox.Show(
                        "字体及颜色设置成功！",
                        "提示",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    return;

                }

                MessageBox.Show(
                        "字体设置成功！",
                        "提示",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                );
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
                
                newconfig.BackgroundImagestatus = true;
                dialogFileName = dialog.FileName;
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
                MessageBox.Show($"设置成功，路径为:{dialog.FileName}", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ArgumentsBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            newconfig.Arguments = ArgumentsBlock.Text;
        }
    }
}
