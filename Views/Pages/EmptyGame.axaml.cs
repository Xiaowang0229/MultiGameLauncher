using Avalonia.Controls;
using Avalonia.Interactivity;
using RocketLauncherRemake.Utils;
using System;

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
            Filter = "Json 配置项文件(*.json)|*.json",
            Multiselect = false
        };
        if (dialog.ShowDialog() == true)
        {
            var dialog2 = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "选择配置项所对应的 Backgrounds 文件夹",
                Multiselect = false
            };
            if (dialog2.ShowDialog() == true)
            {
                Variables._MainWindow.Tip.IsVisible = true;
                await FileHelper.CopyFileAsync(dialog.FileName, Variables.Configpath);
                Variables._MainWindow.Tip.IsVisible = true;
                await FileHelper.CopyDirectoryAsync(dialog2.FolderName,$"{Environment.CurrentDirectory}\\Backgrounds");
                WindowHelper.Restart();
            }
            
        }
    }
}