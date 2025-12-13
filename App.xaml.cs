using MultiGameLauncher.Views.Pages;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MultiGameLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //读取逻辑
            Tools.ApplicationLogo = Tools.ConvertByteArrayToImageSource(ApplicationResources.ApplicationIcon);



            //主加载逻辑
            Window win = new MainWindow();
            win.Show();
        }
    }

}
