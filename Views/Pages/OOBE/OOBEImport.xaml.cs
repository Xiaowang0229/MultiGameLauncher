using HuaZi.Library.Json;
using Microsoft.Win32;
using MultiGameLauncher.Views.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Brushes = System.Windows.Media.Brushes;

namespace MultiGameLauncher.Views.Pages.OOBE
{
    /// <summary>
    /// OOBEImport.xaml 的交互逻辑
    /// </summary>
    public partial class OOBEImport : Page
    {
        private List<StackPanel> animationSP = new();
        private MainConfig config;
        private LaunchConfig newconfig;
        private string DialogFileName;
        public bool IsBackGroundChange;
        public bool Iscreatenewgame;

        public OOBEImport(bool isCreateNewGame=false)
        {
            InitializeComponent();
            Iscreatenewgame = isCreateNewGame;
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            newconfig = new LaunchConfig
            {
                HashCode = Tools.RandomHashGenerate(),
                Arguments = "",
                Launchpath = null,
                MainTitle = null,
                MainTitleFontColor = Brushes.Black,
                MaintitleFontName = new System.Windows.Media.FontFamily("Microsoft YaHei UI"),
                ShowName = null,
                SubTitle = ""
            };
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

        private void Button_Click(object sender, EventArgs e)
        {
            newconfig.ShowName = ProgramNameBlock.Text;
        }

        private void Button_Click_1(object sender, EventArgs e)
        {
            newconfig.MainTitle = MainTitleBox.Text;
        }

        private void Font_Click(object sender, EventArgs e)
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

        private void Button_Click_2(object sender, EventArgs e)
        {
            newconfig.SubTitle = SubTitleBox.Text;
        }

        private async void Button_Click_3(object sender, EventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择文件",
                Filter = "背景文件(*.png;*.mp4)|*.png;*.mp4",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false
            };
            if(dialog.ShowDialog() == true)
            {
               
                DialogFileName = dialog.FileName;
                IsBackGroundChange = true;
                if (Path.GetExtension(DialogFileName) == ".mp4")
                {
                    Backgroundview.Content = "动态背景";
                }
                if (Path.GetExtension(DialogFileName) == ".png")
                {
                    Backgroundview.Content = "静态背景";
                }
                DeleteBackground.Visibility = Visibility.Visible;
                /*await Task.Delay(500);
                Directory.CreateDirectory(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}");
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(dialog.FileName)))
                {
                    File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(dialog.FileName));
                }
                File.Copy(dialog.FileName, Environment.CurrentDirectory+$"\\Backgrounds\\{newconfig.HashCode}\\Background"+Path.GetExtension(dialog.FileName));
                BackgroundCopyTip.Visibility = Visibility.Hidden;
                MessageBox.Show($"设置成功，路径为:{dialog.FileName}","提示",MessageBoxButton.OK,MessageBoxImage.Information);*/


            }
        }

        

        private void Button_Click_5(object sender, EventArgs e)
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
            }
        }

        private void Button_Click_6(object sender, EventArgs e)
        {
            newconfig.Arguments = ArgumentsBlock.Text;
        }

        private async void Button_Click_7(object sender, EventArgs e)
        {
            if( newconfig.Launchpath!= null && newconfig.MainTitle != null && newconfig.ShowName != null)
            {
                BackgroundCopyTip.Visibility = Visibility.Visible;
                await Task.Delay(100);
                config.GameInfos.Add(newconfig);
                config.OOBEStatus = true;
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
                    File.Copy(DialogFileName, Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Background" + Path.GetExtension(DialogFileName));
                   
                }
                BackgroundCopyTip.Visibility = Visibility.Hidden;
                this.IsEnabled = true;

                var proc = new Process();
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = newconfig.Launchpath,
                    Arguments = newconfig.Arguments,
                    UseShellExecute = true
                };
                Variables.GameProcess.Add(proc);
                Variables.GameProcessStatus.Add(false);
                Json.WriteJson(Variables.Configpath, config);

                MessageBox.Show($"操作成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                
                if(Iscreatenewgame == true)
                {
                    var win = System.Windows.Application.Current.Windows.OfType<OOBEWindow>().FirstOrDefault();
                    win.Close();
                }
                else
                {
                    var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    win.RootFrame.Navigate(new Manage());
                }
            }
            else
            {
                MessageBox.Show("请将启动路径，程序名称和主标题填写完整，以便正常写入Json文件进行程序启动！","警告",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }

        private void DeleteBackground_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("确定删除背景吗？此操作不可逆","提示",MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                IsBackGroundChange = false;
                DeleteBackground.Visibility = Visibility.Hidden;
            }
        }
    }
}
