using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfSmartHomeApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {

        private double _homeTemp;
        private double _homeHumid;
        private string _detectResult;
        private string _isDetecOn;
        private string _rainResult;
        private string _isRainon;
        private string _airConResult;
        private string _isAirConOn;
        private string _lightResult;
        private string _isLightOn;
        private string? _currDateTime;

        private readonly DispatcherTimer _timer;

        public MainViewModel()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (sender, e) =>
            {
                CurrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            _timer.Start();
        }
        public string? CurrDateTime
        {

            get => _currDateTime;
            set => SetProperty(ref _currDateTime, value);
        }
        // 온도 속성
        public double HomeTemp
        {
            get => _homeTemp;
            set => SetProperty(ref _homeTemp, value);
        }


        // 습도 속성
        public double HomeHumid
        {
            get => _homeHumid;
            set => SetProperty(ref _homeHumid, value);
        }

        // 사람인지
        public string DetectResult
        {
            get => _detectResult;
            set => SetProperty(ref _detectResult, value);
        }

        // 사람인지 여부
        public string IsDetecOn
        {
            get => _isDetecOn;
            set => SetProperty(ref _isDetecOn, value);
        }

        // 비
        public string RainResult
        {
            get => _rainResult;
            set => SetProperty(ref _rainResult, value);
        }

        // 비인지 여부
        public string IsRainon
        {
            get => _isRainon;
            set => SetProperty(ref _isRainon, value);
        }

        // 에어컨인지
        public string AirConResult
        {
            get => _airConResult;
            set => SetProperty(ref _airConResult, value);
        }

        // 에어컨인지 여부
        public string IsAirConOn
        {
            get => _isAirConOn;
            set => SetProperty(ref _isAirConOn, value);
        }

        // 빛
        public string LightResult
        {
            get => _lightResult;
            set => SetProperty(ref _lightResult, value);
        }



        // 빛인지 여부
        public string IsLightOn
        {
            get => _isLightOn;
            set => SetProperty(ref _isLightOn, value);
        }

        // LoadedCommand 에서 On이 붙고 Command는 삭제
        [RelayCommand]
        public void OnLoaded()
        {
            HomeTemp = 30;
            HomeHumid = 43.2;

            DetectResult = "Detected Human!";
            IsDetecOn = "true";
            RainResult = "Raining";
            IsRainon = "true";
            AirConResult = "Aircon On!";
            IsAirConOn = "true";
            LightResult = "Light On!";
            IsLightOn = "true";
        }
    }
}
