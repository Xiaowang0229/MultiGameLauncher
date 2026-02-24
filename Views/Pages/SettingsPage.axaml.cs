using Avalonia.Controls;
using Avalonia.Interactivity;
using RocketLauncherRemake.Utils;
using System;
using System.IO;
using System.Threading.Tasks;
using Xiaowang0229.JsonLibrary;

namespace RocketLauncherRemake;

public partial class SettingsPage : UserControl
{
    MainConfig config = JsonConfig.ReadConfig();
    public SettingsPage()
    {
        InitializeComponent();
        
    }
    private void Page_Loaded(object sender,RoutedEventArgs e)
    {
        config = JsonConfig.ReadConfig();
        LaunchWithMinize.IsChecked = config.LaunchWithMinize;
        StartUpCheckUpdate.IsChecked = config.StartUpCheckUpdate;
    }
    private void StartUpCheckUpdate_Toggled(object sender, RoutedEventArgs e)
    {

        if (StartUpCheckUpdate.IsChecked == true)
        {
            config.StartUpCheckUpdate = true;
            config.WriteConfig();

        }
        else
        {
            config.StartUpCheckUpdate = false;
            config.WriteConfig();

        }
    }

    private void LaunchWithMinize_Toggled(object sender, RoutedEventArgs e)
    {
        if (LaunchWithMinize.IsChecked == true)
        {
            config.LaunchWithMinize = true;
            config.WriteConfig();
        }
        else
        {
            config.LaunchWithMinize = false;
            config.WriteConfig();
        }
    }

    private async void ResetConfig_Click(object sender, RoutedEventArgs e)
    {
        var qdr = await Variables._MainWindow.ShowMessageAsync("警告", "确定要重置所有配置项（包含个性化设置，主题设置和所有已经添加的游戏等）吗？此操作不可逆");
        if (qdr)
        {
            Variables._MainWindow.Tip.IsVisible = true;
            JsonConfig.InitalizeConfig(true);
            await Task.Run(() => Directory.Delete(Variables.BackgroundPath, true));
            WindowHelper.Restart();
        }
    }

    private void ImportGame_Click(object sender, RoutedEventArgs e)
    {
        Variables._MainWindow.RootFrame.Navigate(typeof(ImportPage));
    }

    private void ManageGame_Click(object sender, RoutedEventArgs e)
    {
        if (config.GameInfos.Count == 0)
        {
            Variables._MainWindow.RootFrame.Navigate(typeof(EmptyGame));
        }
        else
        {
            Variables._MainWindow.RootFrame.Navigate(typeof(ManagePage));
        }
    }

    private async void Import_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "选择已有配置项",
            Filter = "Json 旧版配置文件|Config.json|Bson 新版配置文件|Config.bson",
            Multiselect = false
        };
        if (dialog.ShowDialog() == true)
        {
            if (dialog.FileName == Variables.Configpath)
            {
                await Variables._MainWindow.ShowMessageAsync("错误!", "不能选择现存配置文件");
                Variables._MainWindow.Tip.IsVisible = false;
                return;
            }
            var dialog2 = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "选择配置项所对应的 Backgrounds 文件夹",
                Multiselect = false
            };
            if (dialog2.ShowDialog() == true)
            {
                Variables._MainWindow.Tip.IsVisible = true;
                //await FileHelper.CopyFileAsync(dialog.FileName, Variables.Configpath);
                if (Path.GetExtension(dialog.FileName) == ".json")
                {
                    var j = Json.ReadJson<MainConfig>(dialog.FileName);
                    j.WriteConfig();
                }
                else if (Path.GetExtension(dialog.FileName) == ".bson")
                {
                    File.Delete(Variables.Configpath);
                    File.Copy(dialog.FileName, Variables.Configpath);
                }
                else
                {
                    await Variables._MainWindow.ShowMessageAsync("错误!", "文件格式不正确");
                    Variables._MainWindow.Tip.IsVisible = false;
                    return;
                }
                Variables._MainWindow.Tip.IsVisible = true;
                await FileHelper.CopyDirectoryAsync(dialog2.FolderName, $"{Environment.CurrentDirectory}\\Backgrounds");
                WindowHelper.Restart();
            }

        }
    }
}