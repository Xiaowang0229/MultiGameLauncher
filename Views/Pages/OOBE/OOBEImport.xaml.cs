using HuaZi.Library.Json;
using MahApps.Metro.Controls.Dialogs;
using MultiGameLauncher.Views.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
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
        private bool IsOOBE;
        public bool Iscreatenewgame;

        public bool SaveGame = false;

        public OOBEImport(bool isCreateNewGame = false, bool isOOBE = false)
        {
            InitializeComponent();
            Iscreatenewgame = isCreateNewGame;
            IsOOBE = isOOBE;
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
                GamePlayedMinutes = 0
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



        private async void Button_Click_3(object sender, EventArgs e)
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
                */


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
                if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png"))
                {
                    File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");
                }
                Tools.ExtractExeIconToPng(dialog.FileName, Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");
                Tools.RefreshAllImageCaches(this);
                ApplicationIcon.Source = Tools.LoadImageFromPath(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");
                newconfig.Launchpath = dialog.FileName;
                LaunchPathView.Content = dialog.FileName;
                ProgramNameBlock.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
                newconfig.ShowName = ProgramNameBlock.Text;
            }
        }

        private void Button_Click_6(object sender, EventArgs e)
        {
            newconfig.Arguments = ArgumentsBlock.Text;
        }

        private async void Button_Click_7(object sender, EventArgs e)
        {
            SaveGame = true;
            if (newconfig.Launchpath != null && newconfig.MainTitle != null && newconfig.ShowName != null && File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png"))
            {
                BackgroundCopyTip.Visibility = Visibility.Visible;
                await Task.Delay(100);
                config.GameInfos.Add(newconfig);
                config.OOBEStatus = true;
                this.IsEnabled = false;
                if (IsBackGroundChange)
                {

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

                if (Iscreatenewgame == true)
                {
                    var proc = new Process();
                    proc.StartInfo = new ProcessStartInfo
                    {
                        FileName = newconfig.Launchpath,
                        Arguments = newconfig.Arguments,
                        UseShellExecute = true
                    };
                    Variables.GameProcess.Add(proc);
                    Variables.GameProcessStatus.Add(false);
                    Variables.PlayingTimeintList.Add(0);
                    var dt = new DispatcherTimer();
                    dt.Interval = TimeSpan.FromMinutes(1);
                    dt.Tick += async (s, e) =>
                    {
                        Variables.PlayingTimeintList[Variables.PlayingTimeintList.Count - 1] += 1;
                    };
                    Variables.PlayingTimeRecorder.Add(dt);
                }
                Json.WriteJson(Variables.Configpath, config);

                

                if (Iscreatenewgame == true)
                {
                    var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    win.RootFrame.Navigate(new Manage());

                }
                else
                {
                    var win = System.Windows.Application.Current.Windows.OfType<OOBEWindow>().FirstOrDefault();
                    win.Close();
                }
            }
            else
            {
                Tools.GetShowingWindow().ShowMessageAsync("错误", $"请将带星号的必填项填写完整！");
            }
        }

        private async void DeleteBackground_Click(object sender, RoutedEventArgs e)
        {
            var qdr = await Tools.ShowQuestionDialogMetro("确定删除背景吗？此操作不可逆", "警告");
            if (qdr)
            {
                IsBackGroundChange = false;
                DeleteBackground.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
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
                    Tools.GetShowingWindow().ShowMessageAsync("图标应用时错误", $"{ex.Message}");
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}");
            }
            if (File.Exists(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png"))
            {
                File.Delete(Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");
            }
            Tools.ConvertToPngAndSave(ApplicationResources.DefaultGameIcon, Environment.CurrentDirectory + $"\\Backgrounds\\{newconfig.HashCode}\\Icon.png");
        }

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        
    }
}
