using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MQTTnet;
using Newtonsoft.Json;
using System.Windows.Threading;
using WpfMqttSubApp.Models;
using WpfSmartHomeApp.Helpers;

namespace WpfSmartHomeApp.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        private double _homeTemp;
        private double _homeHumid;

        private bool _isDetectOn;
        private bool _isRainOn;
        private bool _isAirConOn;
        private bool _isLightOn;
        private string _detectResult;
        private string _rainResult;
        private string _airConResult;
        private string _lightResult;
        private string? _currDateTime;

        // readonly는 생성자에서만 값을 할당, 그 외는 불가
        private readonly DispatcherTimer _timer;

        // MQTT용 변수들
        private string TOPIC;
        private IMqttClient mqttClient;
        private string BROKERHOST;

        public MainViewModel()
        {
            CurrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

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
        public bool IsDetectOn
        {
            get => _isDetectOn;
            set => SetProperty(ref _isDetectOn, value);
        }

        public string RainResult
        {
            get => _rainResult;
            set => SetProperty(ref _rainResult, value);
        }

        public bool IsRainOn
        {
            get => _isRainOn;
            set => SetProperty(ref _isRainOn, value);
        }

        public string AirConResult
        {
            get => _airConResult;
            set => SetProperty(ref _airConResult, value);
        }

        public bool IsAirConOn
        {
            get => _isAirConOn;
            set => SetProperty(ref _isAirConOn, value);
        }

        public string LightResult
        {
            get => _lightResult;
            set => SetProperty(ref _lightResult, value);
        }

        public bool IsLightOn
        {
            get => _isLightOn;
            set => SetProperty(ref _isLightOn, value);
        }

        // LoadedCommand 에서 앞에 On이 붙고 Command는 삭제
        [RelayCommand]
        public async Task OnLoaded()
        {
            // 테스트로 집어넣은 가짜 데이터
            //HomeTemp = 30;
            //HomeHumid = 43.2;

            //DetectResult = "Detected Human!";
            //IsDetectOn = true;
            //RainResult = "Raining";
            //IsRainOn = true;
            //AirConResult = "AirCon On!";
            //IsAirConOn = true;
            //LightResult = "Light On~";
            //IsLightOn = true;

            // MQTT 접속부터 실행까지
            TOPIC = "pknu/sh01/data";   // publish, subscribe 동시에 사용!!
            BROKERHOST = "210.119.12.52";   // SmartHome MQTT Broker IP

            var mqttFactory = new MqttClientFactory();
            mqttClient = mqttFactory.CreateMqttClient();

            // MQTT 클라이언트접속 설정변수
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(BROKERHOST)
                .WithCleanSession(true)
                .Build();

            // MQTT 접속 후 이벤트 처리 메서드 선언
            mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            // MQTT 구독메시지 확인 메서드 선언
            mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;

            await mqttClient.ConnectAsync(mqttClientOptions);   // MQTT 브로커에 접속
        }

        // MQTT 구독메시지 확인 메서드
        private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
        {
            var topic = args.ApplicationMessage.Topic;  // pknu/sh01/data
            var payload = args.ApplicationMessage.ConvertPayloadToString(); // byte -> UTF-8 문자열로 변환

            var data = JsonConvert.DeserializeObject<SensingInfo>(payload);
            //Common.LOGGER.Info($@"Light : {data.L} / Rain : {data.R} / Temp : {data.T} / Humid : {data.H}
            //                     FAN : {data.F} / Vulernability : {data.V} / RealLight : {data.RL} / ChaimBell : {data.CB}");

            HomeTemp = data.T;
            HomeHumid = data.H;

            IsDetectOn = data.V == "ON" ? true : false;
            DetectResult = data.V == "ON" ? "Detect State!!" : "Normal Sate";

            IsLightOn = data.RL == "ON" ? true : false;
            LightResult = data.RL == "ON" ? "Light On!" : "Light Off";

            IsAirConOn = data.F == "ON" ? true : false;
            AirConResult = data.F == "ON" ? "AirCon On!" : "AirCon Off";

            IsRainOn = data.R <= 350 ? true : false;
            RainResult = data.R <= 350 ? "Raining" : "No Rain";

            return Task.CompletedTask;  // 구독이 종료됨을 알려주는 리턴문
        }

        // MQTT 접속확인 이벤트 메서드
        private async Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs args)
        {
            Common.LOGGER.Info($"{args}");
            Common.LOGGER.Info("MQTT Broker 접속 성공!!");
            // 연결 이후 Subscribe 구독 시작
            await mqttClient.SubscribeAsync(TOPIC);
        }

        public void Dispose()
        {
            // TODO...
        }
    }
}