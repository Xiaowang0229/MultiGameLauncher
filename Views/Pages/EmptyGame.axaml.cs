using Avalonia.Controls;
using Avalonia.Interactivity;
using RocketLauncherRemake.Utils;
using System;
using System.IO;
using Xiaowang0229.JsonLibrary;

namespace RocketLauncherRemake;

public partial class EmptyGame : UserControl
{
    public EmptyGame()
    {
        InitializeComponent();
    }

    private void Create_Click(object sender,RoutedEventArgs e)
    {
        Variables._MainWindow.RootFrame.Navigate(typeof(ImportPage));
    }

    private async void Import_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "选择已有配置项",
            Filter = "Json 旧配置项文件|Config.json|Bson 新配置项文件|Config.Bson",
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
                if(Path.GetExtension(dialog.FileName) == ".json")
                {
                    var j = Json.ReadJson<MainConfig>(dialog.FileName);
                    j.WriteConfig();
                }
                else if (Path.GetExtension(dialog.FileName) == ".bson")
                {
                    File.Copy(dialog.FileName, Variables.Configpath);
                }
                else
                {
                    await Variables._MainWindow.ShowMessageAsync("错误!","文件格式不正确");
                    Variables._MainWindow.Tip.IsVisible = false;
                    return;
                }
                Variables._MainWindow.Tip.IsVisible = true;
                await FileHelper.CopyDirectoryAsync(dialog2.FolderName,$"{Environment.CurrentDirectory}\\Backgrounds");
                WindowHelper.Restart();
            }
            
        }
    }
}