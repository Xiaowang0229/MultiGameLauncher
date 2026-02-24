using Avalonia;
using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using Markdown.Avalonia;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xiaowang0229.JsonLibrary;

namespace RocketLauncherRemake.Utils
{
    public static partial class Variables
    {
        public static string ShowVersion = $"版本:{Version}";
        public static CancellationTokenSource UpdateCTS = new CancellationTokenSource();
        public static bool? UpdatePackageDownloadStatus = false;
    }
    public class Update
    {
        public async static Task CheckUpdate(bool ShowMessages = false)
        {
            if(Variables.IsDevelopmentMode)
            {
                if(ShowMessages)
                {
                    Variables._MainWindow.ShowMessageAsync("提示", $"当前版本是内测版本,暂无法更新");
                    Variables._MainWindow.Tip.IsVisible = false;
                }
                return;
            }
            if(ShowMessages)
            {
                Variables._MainWindow.Tip.IsVisible = true;
            }
            var client = new HttpClient();
            try
            {
                var content = await client.GetStringAsync("https://gitee.com/xiaowangupdate/update-service/raw/master/MultiGameLauncher", Variables.UpdateCTS.Token);
                var updcfg = Json.ReadJson<UpdateConfig>(content);
                
                if (updcfg.UpdateVersion != Variables.Version)
                {



                   
                    
                        var filname = HashCode.RandomHashGenerate();
                        var win = Variables._MainWindow;

                        //var results = await win.ShowMessageAsync("更新可用", $"当前版本:{Variables.Version},最新版本:{updcfg.UpdateVersion},请问是否更新？", MessageDialogStyle.AffirmativeAndNegative, settings);
                        var sp = new StackPanel { Margin = new Thickness(5) };
                        sp.Children.Add(new TextBlock { Text = $"当前版本:{Variables.Version}\n最新版本:{updcfg.UpdateVersion}\r更新日志:" });
                        sp.Children.Add(new MarkdownScrollViewer
                        {
                            Markdown = updcfg.UpdateLog
                        });
                    var pgb = new ProgressBar { Value = 0, Margin = new Thickness(0,5,0,0), HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, IsVisible = false };
                    sp.Children.Add(pgb);
                        Variables._MainWindow.Tip.IsVisible = false;
                    //var results = await win.ShowMessageAsync("更新可用", sp);
                    var dialog = new ContentDialog
                    {
                        Title = "新版本可用,是否更新?",
                        Content = sp,
                        PrimaryButtonText = "现在更新",
                        SecondaryButtonText = "推迟",
                        DefaultButton = ContentDialogButton.Primary,
                        
                    };
                    
                    dialog.PrimaryButtonClick += async (s, e) =>
                    {
                        dialog.IsPrimaryButtonEnabled = false;
                        dialog.IsSecondaryButtonEnabled = false;
                        e.Cancel = true;
                        pgb.IsVisible = true;
                        var DownloadStatus = false;
                        try
                        {

                            if (File.Exists($"{Environment.CurrentDirectory}\\Temp\\Update.zip"))
                            {
                                File.Delete($"{Environment.CurrentDirectory}\\Temp\\Update.zip");
                            }
                            var downloader = new Downloader
                            {
                                Url = updcfg.UpdateLink,
                                SavePath = $"{Environment.CurrentDirectory}\\Temp\\Update.zip",
                                Completed = (async (s, e) =>
                                {
                                    if (s)
                                    {
                                        DownloadStatus = true;
                                    }
                                    else
                                    {
                                        if (ShowMessages)
                                        {
                                            dialog.Hide();
                                            Variables._MainWindow.ShowMessageAsync("下载错误", $"错误为:{e}");
                                            Variables._MainWindow.Tip.IsVisible = false;
                                            return;
                                        }
                                    }
                                }),
                                Progress = ((p, s) =>
                                {
                                    pgb.Value = Convert.ToInt32(p);

                                })
                            };
                            downloader.StartDownload();
                            while (DownloadStatus == false)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(2));
                            }

                            Process.Start(new ProcessStartInfo
                            {
                                FileName = $"{Environment.CurrentDirectory}\\UpdateAPI.exe",
                                Arguments=$"\"{Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName)}\"",
                                WorkingDirectory = Environment.CurrentDirectory,
                                UseShellExecute = true
                            });
                            TaskBar.KillTaskBar();
                            Environment.Exit(0);
                        }
                        catch (Exception ex)
                        {
                            dialog.Hide();
                            if (ShowMessages)
                            {
                                await Variables._MainWindow.ShowMessageAsync("更新时发现错误", $"{ex.Message}");
                            }
                            Variables._MainWindow.Tip.IsVisible = false;
                            Environment.Exit(0);
                        }

                    };

                    await dialog.ShowAsync();






                }
                else if (updcfg.UpdateVersion == Variables.Version && ShowMessages)
                {
                    Variables._MainWindow.ShowMessageAsync("提示", $"当前版本已是最新版本:{updcfg.UpdateVersion}！");
                    Variables._MainWindow.Tip.IsVisible = false;
                }
            }
            catch (TaskCanceledException)
            {
                Variables._MainWindow.Tip.IsVisible = false;
            }
            catch (Exception ex)
            {
                if (ShowMessages)
                {
                    Variables._MainWindow.ShowMessageAsync("检测更新时发现错误", $"{ex.Message}");

                }
                Variables._MainWindow.Tip.IsVisible = false;
            }

        }

        public static void OpenBrowser(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            }); return;
        }

        
    }
}
