using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfBasicApp01.ViewModels;

namespace WpfBasicApp01
{
    internal class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container;

        public Bootstrapper()
        {
            Initialize();
        }

        //protected override void Configure()
        //{
        //    _container = new SimpleContainer();
        //    _container.Singleton<IWindowManager, WindowManager>();
        //    _container.Singleton<IDialogCoordinator, DialogCoordinator>();
        //    _container.PerRequest<MainViewModel>();
        //}

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewForAsync<MainViewModel>();
        }
    }

}
