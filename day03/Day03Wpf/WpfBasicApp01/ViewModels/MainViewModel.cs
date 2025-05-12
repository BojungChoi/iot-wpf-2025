using CommunityToolkit.Mvvm.ComponentModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfBasicApp01.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        #region 속성 영역
        // NLog 객체 생성
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private string _greeting;
        public string Greeting { 
            get => _greeting; 
            set => SetProperty(ref _greeting, value);  // CommunityToolkit.Mvvm의 핵심 
        }

        private string _currentTime;
        public string CurrentTime 
        { 
            get => _currentTime; 
            set => SetProperty(ref _currentTime, value); 
        }

        private readonly DispatcherTimer _timer;

        #endregion
        public MainViewModel()
        {
            _logger.Info("뷰 모델 시작");
            Greeting = "Hello, WPF MVVM!";
            CurrentTime = DateTime.Now.ToString();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString();
            _logger.Info($"[DEBUG] {CurrentTime}");
        }
    }
}
