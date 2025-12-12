using MahApps.Metro.Controls;
using MultiGameLauncher.Views.Pages;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace MultiGameLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();


            



        }



        private async void RootWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RootWindow.Icon = Tools.ConvertByteArrayToImageSource(ApplicationResources.ApplicationIcon);
            RootFrame.Navigate(new Launch());

        }
    }
}