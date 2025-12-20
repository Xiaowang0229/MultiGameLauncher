using HuaZi.Library.Json;
using Microsoft.Win32;
using MultiGameLauncher.Views.Windows;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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
        public OOBEImport()
        {
            InitializeComponent();
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            newconfig = new LaunchConfig
            {
                Arguments = "",
                BackgroundImagestatus = null,
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
            using (ColorDialog colorDialog = new ColorDialog())

            if (fontdialog.ShowDialog() == DialogResult.OK)
            {
                    

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    newconfig.MaintitleFontName = new System.Windows.Media.FontFamily(fontdialog.Font.FontFamily.Name); ;
                    newconfig.MainTitleFontColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(
                    colorDialog.Color.A,
                    colorDialog.Color.R,
                    colorDialog.Color.G,
                    colorDialog.Color.B));
                        MessageBox.Show(
                            "字体设置成功！",
                            "提示",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                        
                }
            }
        }

        private void Button_Click_2(object sender, EventArgs e)
        {
            newconfig.SubTitle = SubTitleBox.Text;
        }

        private void Button_Click_3(object sender, EventArgs e)
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
                newconfig.BackgroundImagestatus = true;
                File.Copy(dialog.FileName, Environment.CurrentDirectory+@"\Background"+Path.GetExtension(dialog.FileName));
                MessageBox.Show($"设置成功，路径为:{dialog.FileName}","提示",MessageBoxButton.OK,MessageBoxImage.Information);
                
                
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
                MessageBox.Show($"设置成功，路径为:{dialog.FileName}","提示",MessageBoxButton.OK,MessageBoxImage.Information);
            }
        }

        private void Button_Click_6(object sender, EventArgs e)
        {
            newconfig.Arguments = ArgumentsBlock.Text;
        }

        private void Button_Click_7(object sender, EventArgs e)
        {
            if( newconfig.Launchpath!= null && newconfig.MainTitle != null && newconfig.ShowName != null)
            {
                config.GameInfos.Add(newconfig);
                config.OOBEStatus = true;
                Json.WriteJson(Variables.Configpath, config);
                
                var win = System.Windows.Application.Current.Windows.OfType<OOBEWindow>().FirstOrDefault();
                win.Close();
            }
            else
            {
                MessageBox.Show("请将启动路径，程序名称和主标题填写完整，以便正常写入Json文件进行程序启动！","警告",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }

        
    }
}
