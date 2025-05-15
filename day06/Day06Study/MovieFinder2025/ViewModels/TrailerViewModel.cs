using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MahApps.Metro.Controls.Dialogs;
using MovieFinder2025.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MovieFinder2025.ViewModels
{
    public partial class TrailerViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator;

        public TrailerViewModel(string movieName ,IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator;
            MovieTitle = movieName; // 영화 제목을 받아옴

            SearchYoutubeApi();

            YoutubeUri = "https://www.youtube.com/watch?v=0"; // 초기 Youtube URL 설정
        }


        private string _movieTitle;

        public string MovieTitle
        {
            get => _movieTitle;
            set => SetProperty(ref _movieTitle, value);
        }


        private ObservableCollection<YoutubeItem> _youtubeItems;
        public ObservableCollection<YoutubeItem> YoutubeItems { 
            get => _youtubeItems; 
            set => SetProperty(ref _youtubeItems, value); } 

        
        private YoutubeItem _selectedYoutube;

        public YoutubeItem SelectedYoutube
        {
            get => _selectedYoutube;
            set
            {
                SetProperty(ref _selectedYoutube, value);
            }
        }

        private string _youtubeUri;
        public string YoutubeUri
        {
            get => _youtubeUri;
            set => SetProperty(ref _youtubeUri, value);
        }


        /// <summary>
        /// 핵심 Youtube Data Api v3 
        /// </summary>
        /// <returns></returns>
        private async Task SearchYoutubeApi()
        {
            await LoadDataCollection();
        }

        private async Task LoadDataCollection()
        {
            var service = new YouTubeService(
                new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyDjWi5PH2tBW1eBRgnhSTgzdJcnBAwEE5M",
                    ApplicationName = this.GetType().ToString()
                });

            var req = service.Search.List("snippet");
            req.Q = $"{MovieTitle} 공식 예고편" ; // 영화 제목 API 를 검색
            req.Order = SearchResource.ListRequest.OrderEnum.Relevance; // 정렬기준
            req.Type = "video"; // 비디오 타입
            req.MaxResults = 8; 

            var res = await req.ExecuteAsync(); // Youtube API 서버에 요청된값 실행하고 결과를 리턴(비동기)

            ObservableCollection<YoutubeItem> youtubeItems = new ObservableCollection<YoutubeItem>();

            foreach (var item in res.Items)
            {
                youtubeItems.Add(new YoutubeItem
                {
                    Title = item.Snippet.Title,
                    ChanelTitle = item.Snippet.ChannelTitle,
                    URL = $"https://www.youtube.com/watch?v={item.Id.VideoId}", // Youtube URL
                    Author = item.Snippet.ChannelId,
                    Thumbnail = new BitmapImage(new Uri(item.Snippet.Thumbnails.Default__.Url, UriKind.RelativeOrAbsolute)) // 썸네일 이미지
                });
            }

            YoutubeItems = youtubeItems;
        }


        [RelayCommand]
        public async Task YoutubeDoubleClick()
        {
            if (SelectedYoutube == null) return;

            // 일반 URL을 Embed URL로 변환
            var videoId = SelectedYoutube.URL.Split('=')[1];
            YoutubeUri = $"https://www.youtube.com/embed/{videoId}?autoplay=1&fs=1&rel=0&modestbranding=1";
        }
    }

}
