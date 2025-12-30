using MahApps.Metro.Controls;
using System.Windows;

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
