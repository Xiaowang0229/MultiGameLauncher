using Markdig;
using Markdig.Wpf;
using MultiGameLauncher.Views.Windows;
using System;
using System.Collections.Generic;
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

namespace MultiGameLauncher.Views.Pages.OOBE
{
    /// <summary>
    /// OOBEEULA.xaml 的交互逻辑
    /// </summary>
    public partial class OOBEEULA : Page
    {
        private List<StackPanel> animationSP = new();
        public OOBEEULA()
        {
            InitializeComponent();
            // 示例 Markdown 文本
            string markdownText = "# Apache 许可证  \r\n**版本 2.0，2004 年 1 月**  \r\nhttp://www.apache.org/licenses/\r\n\r\n### 使用、复制和分发条款\r\n\r\n#### 1. 定义\r\n\r\n- **“许可证”** 指本文件第 1–9 节中的条款。\r\n- **“许可方”** 指拥有版权的原始版权所有者，或由原始版权所有者授予本许可证的实体。\r\n- **“被许可方”（或“您”）** 指根据本许可证接受或许可的个人或组织。\r\n- **“衍生作品”** 指基于（或从）本软件源代码衍生而来的任何作品，无论是在源代码还是二进制形式中。\r\n- **“本软件”** 指被许可方根据本许可证获得的原始软件（包括源代码和/或二进制形式）。\r\n\r\n#### 2. 授予版权许可\r\n\r\n在遵守本许可证的前提下，许可方在此授予您永久、全球、免版税、非独占、不可撤销的版权许可，以：\r\n\r\n- 复制\r\n- 准备衍生作品\r\n- 公开展示\r\n- 公开表演\r\n- 再授权\r\n- 分发本软件及其衍生作品（无论是源代码还是二进制形式）\r\n\r\n#### 3. 授予专利许可\r\n\r\n在遵守本许可证的前提下，许可方在此授予您永久、全球、免版税、非独占、不可撤销（本段另有说明除外）的专利许可，使您可以制造、使用、销售、提供销售、进口，以及/或以其他方式转让本软件。\r\n\r\n如果您对任何实体提起专利诉讼（包括交叉索赔或反索赔），指控本软件或其任何部分构成直接或间接的专利侵权，则本第 3 节授予您的所有专利许可将立即终止。\r\n\r\n#### 4. 重新分发\r\n\r\n您可以复制并分发本软件（源代码或二进制形式），但必须满足以下条件：\r\n\r\na. 必须在每个分发的副本中附带本许可证的副本；  \r\nb. 必须保留所有版权、专利、商标和归属声明；  \r\nc. 如果您分发的是修改后的文件，则必须在修改的文件中附带显著的修改说明；  \r\nd. 不得使用许可方的名称或商标（除非用于合理且习惯性的说明来源），也不得暗示许可方认可您的修改。\r\n\r\n#### 5. 提交贡献\r\n\r\n除非您明确另行声明，否则您有意将任何您故意提交给许可方的贡献纳入本软件，即表示您同意按照本许可证的条款授予许可方所有必要的版权许可。\r\n\r\n#### 6. 商标\r\n\r\n本许可证不授予使用许可方或贡献者名称、商标、服务标记或产品名称的权利，除非用于合理且习惯性的说明本软件来源或再分发。\r\n\r\n#### 7. 免责声明\r\n\r\n在任何情况下，根据本许可证提供的软件均按“原样”提供，不附带任何明示或暗示的担保，包括但不限于适销性、特定用途适用性和非侵权性的担保。许可方不对因使用或无法使用本软件而产生的任何索赔或损害负责。\r\n\r\n#### 8. 责任限制\r\n\r\n在任何情况下，许可方或任何贡献者对任何直接、间接、特殊、附带或后果性损害（包括但不限于采购替代商品或服务的费用、使用、数据或利润的损失，或业务中断）不承担责任，即使已被告知可能发生此类损害。\r\n\r\n#### 9. 适用法律\r\n\r\n本许可证受中华人民共和国法律管辖，不考虑其法律冲突原则。";

            // 构建 Markdown 管道（支持高级扩展，如表格、脚注等）
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()  // 启用扩展功能
                .Build();

            // 转换为 FlowDocument
            FlowDocument document = Markdig.Wpf.Markdown.ToFlowDocument(markdownText, pipeline);

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
    }
}
