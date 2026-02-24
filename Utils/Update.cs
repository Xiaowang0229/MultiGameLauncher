using Avalonia;
using Avalonia.Controls;
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



                    if (Variables.UpdatePackageDownloadStatus == true)
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
                        Variables._MainWindow.Tip.IsVisible = false;
                        var results = await win.ShowMessageAsync("更新可用", sp);

                        if (results == true)
                        {
                            if (File.Exists(Path.GetTempPath() + "\\" + filname + ".exe"))
                            {
                                File.Delete(Path.GetTempPath() + "\\" + filname + ".exe");
                            }
                            try
                            {

                                File.Copy($"{Environment.CurrentDirectory}\\UpdateAPI.exe", Path.GetTempPath() + "\\" + filname + ".exe");
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = $"{Path.GetTempPath()}\\{filname}.exe",
                                    Arguments = $"\"{updcfg.UpdateLink}\" \"{Environment.ProcessPath}\"",
                                    UseShellExecute = true
                                });
                                TaskBar.KillTaskBar();
                                Environment.Exit(0);
                            }
                            catch (Exception ex)
                            {
                                if (ShowMessages)
                                {
                                    await Variables._MainWindow.ShowMessageAsync("更新时发现错误", $"{ex.Message}");
                                }
                                Variables._MainWindow.Tip.IsVisible = false;
                                Environment.Exit(0);
                            }

                        }
                    }
                    else if (Variables.UpdatePackageDownloadStatus == false)
                    {
                        if (ShowMessages)
                        {
                            var win = Variables._MainWindow;
                            await win.ShowMessageAsync("请稍候", "资源包正在下载中......");
                            return;
                        }
                    }
                    else if (Variables.UpdatePackageDownloadStatus == null)
                    {
                        if (ShowMessages)
                        {
                            var win = Variables._MainWindow;
                            var result = await win.ShowMessageAsync("错误", "资源包下载失败,是否重新下载?");
                            if(result)
                            {
                                DownloadUpdatePackage();
                                return;
                            }
                        }
                    }




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

        public static async Task DownloadUpdatePackage()
        {
            Variables.UpdatePackageDownloadStatus = false;
            var client = new HttpClient();
            try
            {
                var content = await client.GetStringAsync("https://gitee.com/xiaowangupdate/update-service/raw/master/MultiGameLauncher", Variables.UpdateCTS.Token);
                var updcfg = Json.ReadJson<UpdateConfig>(content);
                if (updcfg.UpdateVersion != Variables.Version)
                {
                    if(File.Exists($"{Environment.CurrentDirectory}\\Temp\\Update.zip"))
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
                                var cfg = JsonConfig.ReadConfig();
                                Variables.UpdatePackageDownloadStatus = true;
                                if (cfg.StartUpCheckUpdate == true)
                                {
                                    await CheckUpdate(true);
                                }
                            }
                            else
                            {
                                Variables.UpdatePackageDownloadStatus = null;
                                return;
                            }
                        })
                    };
                    downloader.StartDownload();
                }
                else
                {
                    Variables.UpdatePackageDownloadStatus = true;
                }
            }
            catch
            {
                return;
            }
        }
    }
}
