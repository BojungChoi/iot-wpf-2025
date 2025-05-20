using System.Configuration;
using System.Data;
using System.Runtime.Serialization.DataContracts;
using System.Windows;
using WpfSmartHomeApp.ViewModels;
using WpfSmartHomeApp.Views;

namespace WpfSmartHomeApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(Object sender, StartupEventArgs e)
        {
            var viewModel = new MainViewModel();
            var view = new MainWindow
            {
                DataContext = viewModel,
            };
            view.ShowDialog();
        }
    }

}
