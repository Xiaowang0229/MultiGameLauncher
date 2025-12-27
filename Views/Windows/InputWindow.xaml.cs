using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MultiGameLauncher.Views.Windows
{
    /// <summary>
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : MetroWindow
    {
        private string Title;
        public string Results;
        public InputWindow(string title)
        {
            InitializeComponent();
            Title = title;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(InputBox.Text == null || InputBox.Text == "")
            {
                MessageBox.Show("不允许置空！","错误",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            else
            {
                Results = InputBox.Text;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InputTitleblock.Content = Title;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }
}
