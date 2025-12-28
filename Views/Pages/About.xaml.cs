using MahApps.Metro.Controls;
using Markdig;
using MultiGameLauncher.Views.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
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

namespace MultiGameLauncher.Views.Pages
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Page
    {
        private List<StackPanel> animationSP = new();
        public bool IsCheckUpdate {get;set;}
        public About(bool isCheckUpdate = false)
        {
            InitializeComponent();
            IsCheckUpdate = isCheckUpdate;
            VersionBlock.Content = "\n当前版本:" + Variables.Version;
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
                try
                {
                    CheckProgress.Visibility = Visibility.Visible;
                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                    var content = await client.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestVersion");
                    if (content == Variables.Version)
                    {
                        CheckProgress.Visibility = Visibility.Hidden;
                        MessageBox.Show("当前版本已是最新版本", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        CheckupdateButton.IsEnabled = true;
                    }
                    else
                    {


                        using var log = new HttpClient();
                        log.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                        var Onlinelog = await log.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestLog");
                        using var link = new HttpClient();
                        client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                        var OnlineLink = await client.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestLink");
                        CheckProgress.Visibility = Visibility.Hidden;
                        CheckupdateButton.IsEnabled = true;
                        MetroWindow win = new UpdatePrepareWindow(Variables.Version, content, Onlinelog, OnlineLink);
                        win.ShowDialog();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"检查更新时遇到错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Stop);
                    CheckProgress.Visibility = Visibility.Hidden;
                    CheckupdateButton.IsEnabled = true;
                }
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
            try
            {
                CheckProgress.Visibility = Visibility.Visible;
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                var content = await client.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestVersion");
                if (content == Variables.Version)
                {
                    CheckProgress.Visibility = Visibility.Hidden;
                    MessageBox.Show("当前版本已是最新版本", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    CheckupdateButton.IsEnabled = true;
                }
                else
                {
                    
                    
                    using var log = new HttpClient();
                    log.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                    var Onlinelog = await log.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestLog");
                    using var link = new HttpClient();
                    client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                    var OnlineLink = await client.GetStringAsync("https://raw.bgithub.xyz/Xiaowang0229/UpdateService/refs/heads/main/MultiGameLauncher/LatestLink");
                    CheckProgress.Visibility = Visibility.Hidden;
                    CheckupdateButton.IsEnabled = true;
                    MetroWindow win = new UpdatePrepareWindow(Variables.Version,content,Onlinelog,OnlineLink);
                    win.ShowDialog();
                    
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"检查更新时遇到错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Stop);
                CheckProgress.Visibility = Visibility.Hidden;
                CheckupdateButton.IsEnabled = true;
            }
            //MessageBox.Show(content);
            

        }

        private void Apache_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("RocketLauncher 聚合式游戏启动器 使用条款\r\n欢迎使用 RocketLauncher 聚合式游戏启动器（以下简称“本软件”或“RocketLauncher”）。本软件由开发者独立开发并提供，使用前请您仔细阅读本使用条款（以下简称“本条款”）。您下载、安装、使用或以任何方式访问本软件，即视为您已完全理解并同意接受本条款的约束。如您不同意本条款的任何内容，请立即停止使用本软件并卸载相关文件。\r\n一、软件许可\r\n\r\n本软件的源代码及相关文件采用 Apache License 2.0（以下简称“Apache 2.0 许可”）开源协议授权。您可以在遵守 Apache 2.0 许可的前提下，免费获取、使用、修改、分发本软件的源代码。Apache 2.0 许可的具体内容请参阅：https://www.apache.org/licenses/LICENSE-2.0。\r\n本软件在编译、运行过程中可能依赖第三方 NuGet 库。根据本软件开源仓库（https://github.com/Xiaowang0229/MultiGameLauncher）显示，目前版本未引入需特别声明的第三方 NuGet 开源库。若未来版本引入第三方库，相关库的开源许可将独立适用，用户需自行遵守各第三方库的许可条款。本软件开发者不对第三方库的许可合规性承担任何责任。\r\n本许可仅授予您在 Apache 2.0 许可范围内的使用权。除该许可明确允许外，您不得将本软件用于任何商业目的、逆向工程、去除版权声明或进行其他违反许可的行为。\r\n\r\n二、使用范围及限制\r\n\r\n本软件为聚合式游戏启动器，提供游戏管理、启动辅助等功能。本软件仅支持直接启动游戏主程序（非通过官方或平台外壳的方式）。对于某些游戏官方明确要求必须通过其指定的外壳程序（如 Steam、Epic Games Launcher、Origin、Battle.net 等客户端）启动的游戏，本软件不支持导入此类游戏，也不提供任何通过外壳启动的相关功能或配置选项。\r\n若某游戏官方已明确规定不允许直接启动其主程序（.exe 文件），或必须通过外壳启动，则本软件明确不允许导入该游戏。用户在导入游戏时，若尝试导入此类受限游戏，本软件将拒绝该导入操作或不予支持。\r\n您在使用本软件导入或管理游戏时，必须确保：\r\n游戏来源合法，您已获得游戏的正版授权或许可；\r\n严格遵守游戏开发商、发行商或平台的服务条款和使用规则，特别是关于启动方式的规定；\r\n不得通过本软件进行任何绕过正版验证、反外挂机制或其他违法操作。\r\n\r\n本软件不提供、不暗示、不担保任何形式的通过外壳启动游戏或违反游戏官方启动规则的行为。任何因用户违反游戏平台规则、擅自尝试导入受限游戏或其他不当操作导致的法律责任、账号封禁、游戏损坏或其他后果，由用户自行承担，与本软件开发者无关。\r\n您不得使用本软件从事任何违法、侵权或侵害第三方权益的行为，包括但不限于传播盗版游戏、恶意软件或进行网络攻击。\r\n\r\n三、免责声明\r\n\r\n本软件按“现状”提供，不提供任何明示或暗示的担保，包括但不限于适销性、特定用途适用性、无病毒或无侵权的担保。本软件开发者不对因使用本软件引起的任何直接、间接、附带或后果性损害承担责任。\r\n本软件可能依赖第三方服务或平台（如游戏平台 API），因第三方服务变更、中断或政策调整导致的本软件功能异常，不属于本软件开发者的责任范围。\r\n在适用法律允许的最大范围内，本软件开发者对任何因不可抗力、网络故障、用户操作不当或其他非开发者原因导致的损失不承担责任。\r\n\r\n四、知识产权\r\n\r\n本软件的商标、图标、界面设计及其他非源代码部分（如有）的知识产权归开发者所有。除 Apache 2.0 许可明确授权外，未经书面许可不得复制、模仿或用于商业用途。\r\n您使用本软件不得侵犯任何第三方的知识产权，包括游戏版权、商标权等。\r\n\r\n五、条款更新与终止\r\n\r\n本软件开发者保留随时修改本条款的权利。更新后的条款将在软件仓库或相关渠道公布，继续使用本软件即视为接受更新条款。\r\n如您违反本条款，本软件开发者有权随时终止您对本软件的使用权限。\r\n\r\n六、适用法律与争议解决\r\n本条款适用中华人民共和国法律法规。因本条款引起的任何争议，双方应友好协商解决；协商不成的，提交开发者所在地有管辖权的人民法院诉讼解决。\r\n七、其他\r\n本条款自您首次使用本软件之日起生效。若本条款与 Apache 2.0 许可冲突，以 Apache 2.0 许可为准。\r\n感谢您选择 RocketLauncher。如有疑问，请通过软件仓库 Issues 或相关渠道联系开发者。\r\n寒星科技开发组 敬上\r\n日期：2025 年 12 月 28 日","许可协议");
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
    }
}
