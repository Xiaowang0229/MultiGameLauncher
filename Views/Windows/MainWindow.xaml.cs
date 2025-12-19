using MahApps.Metro.Controls;
using MultiGameLauncher.Views.Pages;
using System.Collections.ObjectModel;
using System.Security.Permissions;
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
        private FrameworkElement _previousPage;
        public MainWindow()
        {
            InitializeComponent();


            



        }



        private async void RootWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RootIcon.Source = Tools.ConvertByteArrayToImageSource(ApplicationResources.ApplicationIcon);
            //RootFrame.Navigate();

        }

       
        private async void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            
            //Animation
            //await Task.Delay(1000);
            if (RootFrame.Content is FrameworkElement newPage)
            {
                newPage.BeginAnimation(MarginProperty, null);

                newPage.Margin = new Thickness(-2000, 0, 0, -10);



                var slideIn = new ThicknessAnimation
                {
                    To = new Thickness(0, 0, 0, 10),
                    Duration = TimeSpan.FromMilliseconds(500),
                    EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                };


                newPage.BeginAnimation(MarginProperty, slideIn);
            }


        }

            
         

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(new Launch());
            BackButton.Width = 0;
            
        }

        private async void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {

            var oldPage = (FrameworkElement)RootFrame.Content;
            
                oldPage.BeginAnimation(MarginProperty, null);

                oldPage.Margin = new Thickness(0, 0, 0, -10);



                var slideOut = new ThicknessAnimation
                {
                    To = new Thickness(-2000, 0, 0, 10),
                    Duration = TimeSpan.FromMilliseconds(500),
                    EasingFunction = new PowerEase { Power = 5, EasingMode = EasingMode.EaseOut }
                };


                oldPage.BeginAnimation(MarginProperty, slideOut);
            

        }
    }
}