using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xiaowang0229.JsonLibrary;
using TaskDialogButton = Ookii.Dialogs.Wpf.TaskDialogButton;

namespace RocketLauncherRemake.Utils
{
    public static partial class Variables
    {
        public static MainWindow _MainWindow = new MainWindow();
    }
    public static class WindowHelper
    {


        public static void Restart()
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Environment.Exit(0);
        }


        public async static Task<bool> ShowMessageAsync(this AppWindow owner, string Title, object Content)
        {
            var dialog = new ContentDialog
            {
                Title = Title,
                Content = Content,
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary
            };
            var res = await dialog.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public static void ShowExceptionDialog(object exception)
        {
            var mb = new Ookii.Dialogs.Wpf.TaskDialog
            {
                WindowTitle = "错误",
                MainIcon = Ookii.Dialogs.Wpf.TaskDialogIcon.Error,
                MainInstruction = "程序发生错误，您可将下方内容截图并上报错误",

                Content = $"{exception}",
                ButtonStyle = TaskDialogButtonStyle.CommandLinks,


            };
            var mbb1 = new TaskDialogButton
            {

                Text = "打开错误报告页面(推荐)",
                CommandLinkNote = "将会自动复制错误信息到剪贴板,可能需要启动网络代理以进入Github",

            };
            mb.Buttons.Add(mbb1);
            var mbb2 = new TaskDialogButton
            {
                Text = "退出程序",
                CommandLinkNote = "退出程序以保证错误不再发生",

            };
            mb.Buttons.Add(mbb2);
            var mbb3 = new TaskDialogButton
            {
                Text = "继续运行程序(不推荐)",
                CommandLinkNote = "程序可能随时崩溃或内存泄漏",

            };
            mb.Buttons.Add(mbb3);
            var mbbx = new TaskDialogButton
            {
                Text = "初始化配置项(极端情况下请使用)",
                CommandLinkNote = "程序将丢失所有配置文件并退出,请务必慎用",

            };
            mb.Buttons.Add(mbbx);
            var mbb4 = new TaskDialogButton
            {
                ButtonType = ButtonType.Close
            };
            mb.Buttons.Add(mbb4);
            var res = mb.ShowDialog();
            if (res == mbb1)
            {
                System.Windows.Forms.Clipboard.SetText($"{exception}");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/Xiaowang0229/MultiGameLauncher/issues/new",
                    UseShellExecute = true
                });
                TaskBar.KillTaskBar();
                Environment.Exit(0);
            }
            else if (res == mbb2)
            {
                TaskBar.KillTaskBar();
                Environment.Exit(0);
            }
            else if(res == mbbx)
            {
                var config2 = new MainConfig
                {
                    Username = "Administrator",
                    StartUpCheckUpdate = true,
                    LaunchWithMinize = true,
                    GameInfos = new List<LaunchConfig>()
                };
                config2.WriteConfig();
                TaskBar.KillTaskBar();
                Restart();
            }
        }
    }
}
