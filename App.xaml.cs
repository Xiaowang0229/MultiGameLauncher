using ControlzEx.Theming;
using HuaZi.Library.Json;
using System.IO;
using System.Windows;

namespace MultiGameLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainConfig config;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            config = Json.ReadJson<MainConfig>(Variables.Configpath);
            //读取逻辑
            Tools.ApplicationLogo = Tools.ConvertByteArrayToImageSource(ApplicationResources.ApplicationIcon);

            if(!File.Exists(Variables.Configpath))
            {
                Tools.InitalizeConfig();
            }

            ThemeManager.Current.ChangeTheme(Current,config.ThemeMode+"."+config.ThemeColor);


            //主加载逻辑
            Window win = new MainWindow();
            win.Show();
        }
    }

}
