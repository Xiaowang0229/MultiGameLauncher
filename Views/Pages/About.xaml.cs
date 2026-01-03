using MahApps.Metro.Controls;
using Markdig;
using MultiGameLauncher.Views.Pages.OOBE;
using MultiGameLauncher.Views.Windows;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Page
    {
        private List<StackPanel> animationSP = new();
        public bool IsCheckUpdate { get; set; }
        public About(bool isCheckUpdate = false)
        {
            InitializeComponent();
            IsCheckUpdate = isCheckUpdate;
            VersionBlock.Content = "当前版本:" + Variables.Version;
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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            // 构建 Markdown 管道（支持高级扩展，如表格、脚注等）
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()  // 启用扩展功能
                .Build();

            // 转换为 FlowDocument
            FlowDocument document = Markdig.Wpf.Markdown.ToFlowDocument(Variables.VersionLog, pipeline);

            // 将 FlowDocument 设置到 XAML 中的控件（假设你的 XAML 有名为 viewer 的 FlowDocumentScrollViewer）
            UpdateLog.Document = document;
            if (IsCheckUpdate)
            {
                CheckupdateButton.IsEnabled = false;
                CheckProgress.Visibility = Visibility.Visible;
                await Tools.CheckUpdate();
                CheckProgress.Visibility = Visibility.Hidden;
                CheckupdateButton.IsEnabled = true;
            }
        }




        private void Library_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Xiaowang0229/MultiGameLauncher",
                UseShellExecute = true
            });
        }

        private void Issue_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Xiaowang0229/MultiGameLauncher/issues/new",
                UseShellExecute = true
            });
        }

        private void Afdian_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://afdian.com/a/csharpfadian",
                UseShellExecute = true
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckupdateButton.IsEnabled = false;
            CheckProgress.Visibility = Visibility.Visible;
            await Tools.CheckUpdate(true,true);
            CheckProgress.Visibility = Visibility.Hidden;
            CheckupdateButton.IsEnabled = true;

            //MessageBox.Show(content);


        }

        

        private void MahMetro_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/MahApps/MahApps.Metro",
                UseShellExecute = true
            });
        }

        private void MahLicense_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://raw.githubusercontent.com/MahApps/MahApps.Metro/refs/heads/develop/LICENSE",
                UseShellExecute = true
            });
        }

        private void UsingEULABlock_Click(object sender, RoutedEventArgs e)
        {
            var win = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new OOBEEULA(true));
        }
    }
}
