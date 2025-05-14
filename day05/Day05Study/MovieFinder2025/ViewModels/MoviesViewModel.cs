using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MovieFinder2025.Helpers;
using MovieFinder2025.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Web;

namespace MovieFinder2025.ViewModels
{
    public partial class MoviesViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator;
        [ObservableProperty] private string _movieName;
        [ObservableProperty] private ObservableCollection<MovieItem> _movieItems;

        public MoviesViewModel(IDialogCoordinator _cd)
        {
            dialogCoordinator = _cd;
            MovieItems = new ObservableCollection<MovieItem>();
            Common.LOGGER.Info("PROGRAM START");

            PosterUri = new Uri("/No_Picture.png",UriKind.RelativeOrAbsolute);
        }

        private MovieItem _SelectedMovieItem;

        public MovieItem SelectedMovieItem
        {
            get => _SelectedMovieItem;
            set
            {
                SetProperty(ref _SelectedMovieItem, value);
                Common.LOGGER.Info($"SelectedMovieItem : {value?.Title}");
                // 포스터이미지를 포스터영역에 표시
                PosterUri = new Uri($"{_base_url}{value.Poster_path}", UriKind.Absolute);

            }
        }
        private Uri _posterUri;
        public Uri PosterUri
        {
            get => _posterUri;
            set
            {
                SetProperty(ref _posterUri, value);
            }
        }

        private string _base_url = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";

        [RelayCommand]
        public async Task SearchMovie()
        {
            if (string.IsNullOrEmpty(MovieName))
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "검색", "영화이름을 입력하세요!");
            }
            var controller = await dialogCoordinator.ShowProgressAsync(this, "대기중", "검색 중....");
            controller.SetIndeterminate();
            SearchMovie(MovieName);
            await Task.Delay(1500); // 1.5초 대기
            await controller.CloseAsync();
        }

        private async Task SearchMovie(string movieName)
        {
            MovieItems.Clear();

            string tmdb_apikey = "b6f3cfa101372de6784dd227f907e4a9";
            string encoding_moviename = HttpUtility.UrlEncode(movieName, Encoding.UTF8);
            string openApiUri = $"https://api.themoviedb.org/3/search/movie?api_key={tmdb_apikey}" +
                                $"&language=ko-KR&page=1&include_adult=false&query={encoding_moviename}";
            Common.LOGGER.Info($"TMDB URI : {openApiUri}");
            string result = string.Empty;

            // OpenAPI 실행할 웹 객체
            HttpClient httpClient = new HttpClient();
            MovieSearchResponse? response;
            StreamReader reader = null;
            try
            {
                response = await httpClient.GetFromJsonAsync<MovieSearchResponse?>(openApiUri);
                foreach (var movie in response.Results)
                {
                    Common.LOGGER.Info($"{movie.Title} : {movie.Release_date.ToString("yyyy-MM-dd")}");
                    MovieItems.Add(movie);
                }
            }
            catch (Exception ex)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
                Common.LOGGER.Fatal($"API FATAL ERROR : {ex.Message}");
            }
        }
        [RelayCommand]
        public async Task MovieItemDoubleClick()
        {
            var currMovie = SelectedMovieItem;
            if (currMovie != null)
            {
                StringBuilder sb = new StringBuilder();
                // Environment.NewLine == "\r\n"
                //sb.Append(currMovie.Original_title + " (" + currMovie.Release_date.ToString("yyyy-MM-dd") + ")" + Environment.NewLine);
                sb.Append($"{currMovie.Original_title} ({currMovie.Release_date.ToString("yyyy-MM-dd")})\n");
                sb.Append($"평점 : {currMovie.Vote_average.ToString("F2")}\n\n");
                sb.Append(currMovie.Overview);

                await this.dialogCoordinator.ShowMessageAsync(this, currMovie.Title, sb.ToString());
            }

        }
    }
}
