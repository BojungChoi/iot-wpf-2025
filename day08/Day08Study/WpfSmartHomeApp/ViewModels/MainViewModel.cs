using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSmartHomeApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // 온도
        private double _homeTemp;
        public double HomeTemp
        {
            get => _homeTemp;
            set => SetProperty(ref _homeTemp, value);
        }

        // 습도
        private double _homeHumid;
        public double HomeHumid
        {
            get => _homeHumid;
            set => SetProperty(ref _homeHumid, value);
        }

        //================================================

        // 사람 인지
        private string _detectResult;
        public string DetectResult
        {
            get => _detectResult;
            set => SetProperty(ref _detectResult, value);
        }

        // 사람 인지 여부
        private bool _isDetectOn;
        public bool IsDetectOn
        {
            get => _isDetectOn;
            set => SetProperty(ref _isDetectOn, value);
        }

        //================================================

        private string _rainResult;
        public string RainResult
        {
            get => _rainResult;
            set => SetProperty(ref _rainResult, value);
        }

        private bool _isRainOn;
        public bool IsRainOn
        {
            get => _isRainOn;
            set => SetProperty(ref _isRainOn, value);
        }

        //================================================

        private string _airconResult;
        public string AirconResult
        {
            get => _airconResult;
            set => SetProperty(ref _airconResult, value);
        }

        private bool _isAirconOn;
        public bool IsAirconOn
        {
            get => _isAirconOn;
            set => SetProperty(ref _isAirconOn, value);
        }

        //================================================

        private string _lightsResult;
        public string LightsResult
        {
            get => _lightsResult;
            set => SetProperty(ref _lightsResult, value);
        }

        private bool _isLightsOn;
        public bool IsLightsOn
        {
            get => _isLightsOn;
            set => SetProperty(ref _isLightsOn, value);
        }

        //================================================


        [RelayCommand]
        public void OnLoaded()
        {
            HomeTemp = 22;
            HomeHumid = 43.5;

            DetectResult = "Detected Human";
            IsDetectOn = true;

            RainResult = "Raining";
            IsRainOn = true;

            AirconResult = "Air Condition";
            IsAirconOn = false;

            LightsResult = "Light";
            IsLightsOn = true;
        }
    }
}
