using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBasicApp01.ViewModels
{
    internal class MainViewModel : Conductor<object>
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        private string _greeting;
        public string Greeting { 
            get => _greeting; 
            set
            {
                _greeting = value;
                NotifyOfPropertyChange(() => Greeting);
            }
                 
        }
        
        public void SayHello() {
            Greeting = "Hello~";
            //await _dialogCoordinator.ShowMessageAsync(this, "Greeting", "Hello~~~");
        }

        public MainViewModel(IDialogCoordinator dialog) 
        {
            _dialogCoordinator = dialog;
            Greeting = "Hello Cariburn.Micro !!";
        }
    }
}
