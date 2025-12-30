using Markdig;
using MultiGameLauncher.Views.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace MultiGameLauncher.Views.Pages.OOBE
{
    /// <summary>
    /// OOBEEULA.xaml 的交互逻辑
    /// </summary>
    public partial class OOBEEULA : Page
    {
        private List<StackPanel> animationSP = new();
        private bool IsViewEULAMode;
        public OOBEEULA(bool isViewEULAMode=false)
        {
            InitializeComponent();
            IsViewEULAMode = isViewEULAMode;
            // 示例 Markdown 文本
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()  // 启用扩展功能
                .Build();

            // 转换为 FlowDocument
            FlowDocument document = Markdig.Wpf.Markdown.ToFlowDocument(Variables.EULAString, pipeline);

            // 将 FlowDocument 设置到 XAML 中的控件（假设你的 XAML 有名为 viewer 的 FlowDocumentScrollViewer）
            RootEULAViewer.Document = document;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var win = System.Windows.Application.Current.Windows.OfType<OOBEWindow>().FirstOrDefault();
            win.RootFrame.Navigate(new OOBEPersonality());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsViewEULAMode)
            {
                AgreePanel.Visibility = Visibility.Hidden;
            }
            else if (!IsViewEULAMode)
            {
                AgreePanel.Visibility = Visibility.Visible;
            }
        }
    }
}
