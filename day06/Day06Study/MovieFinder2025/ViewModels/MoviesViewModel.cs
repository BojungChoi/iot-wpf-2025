using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MovieFinder2025.Helpers;
using MovieFinder2025.Models;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Web;
using System.Windows.Threading;

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

            // 시계작업
            CurrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // 1초마다 변경
            _timer.Tick += (s, e) =>
            {
                CurrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            _timer.Start();

        }

        private string _base_url = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";
        private readonly DispatcherTimer _timer;

        private MovieItem _SelectedMovieItem;


        [ObservableProperty] private string _searchResult;

        [ObservableProperty] private string _currDateTime;
        //public string CurrDateTime
        //{
        //    get => _currDateTime;
        //    set => SetProperty(ref _currDateTime, value);
        //}

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
                    //Common.LOGGER.Info($"{movie.Title} : {movie.Release_date.ToString("yyyy-MM-dd")}");
                    MovieItems.Add(movie);
                }

                SearchResult = $"{MovieItems.Count.ToString()}개의 검색결과가 있습니다.";
                Common.LOGGER.Info(MovieName + " / " + SearchResult + "검색완료!!");

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


        [RelayCommand]
        public async Task AddFavoriteMovies()
        {
            //await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기", "즐겨찾기 추가 기능은 준비중입니다.");
            if (SelectedMovieItem == null)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기추가", "즐겨찾기 추가할 영화를 선택하세요.");
                return;
            }

            try
            {
                var query = @"INSERT INTO movieitems
                                    (id, adult, backdrop_path, original_language, original_title, overview,
                                     popularity, poster_path, release_date, title, vote_average, vote_count)
                              VALUES
                                    (@id, @adult, @backdrop_path, @original_language, @original_title, @overview,
                                    @popularity,  @poster_path, @release_date, @title, @vote_average, @vote_count);";
                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", SelectedMovieItem.Id);
                    cmd.Parameters.AddWithValue("@adult", SelectedMovieItem.Adult);
                    cmd.Parameters.AddWithValue("@backdrop_path", SelectedMovieItem.Backdrop_path);
                    cmd.Parameters.AddWithValue("@original_language", SelectedMovieItem.Original_language);
                    cmd.Parameters.AddWithValue("@original_title", SelectedMovieItem.Original_title);
                    cmd.Parameters.AddWithValue("@overview", SelectedMovieItem.Overview);
                    cmd.Parameters.AddWithValue("@popularity", SelectedMovieItem.Popularity);
                    cmd.Parameters.AddWithValue("@poster_path", SelectedMovieItem.Poster_path);
                    cmd.Parameters.AddWithValue("@release_date", SelectedMovieItem.Release_date);
                    cmd.Parameters.AddWithValue("@title", SelectedMovieItem.Title);
                    cmd.Parameters.AddWithValue("@vote_average", SelectedMovieItem.Vote_average);
                    cmd.Parameters.AddWithValue("@vote_count", SelectedMovieItem.Vote_count);

                    var resultCnt = cmd.ExecuteNonQuery();

                    if (resultCnt > 0)
                    {
                        await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기추가", "즐겨찾기 추가 완료");
                        Common.LOGGER.Info($"즐겨찾기 추가 완료 : {SelectedMovieItem.Title}");
                    }
                    else
                    {
                        await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기추가", "즐겨찾기 추가 실패");
                        Common.LOGGER.Error($"즐겨찾기 추가 실패 : {SelectedMovieItem.Title}");
                    }

                }
            }

            catch (MySqlException ex)
            {
                if (ex.Message.ToUpper().Contains("DUPLICATE ENTRY"))
                {
                    await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기추가", "이미 추가된 즐겨찾기입니다.");
                }
                else
                {
                    await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
                }

                Common.LOGGER.Fatal(ex.Message);
            }

            catch (Exception ex)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
                Common.LOGGER.Fatal(ex.Message);
            }
        

        }
        [RelayCommand]
        public async Task ViewFavoriteMovies()
        {
            //await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기", "즐겨찾기 보기 기능은 준비중입니다.");
            ObservableCollection<MovieItem> movieItems = new ObservableCollection<MovieItem>();
            try
            {
                using(MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    var query = @"SELECT id, adult, backdrop_path, original_language, original_title, overview,
                                         popularity, poster_path, release_date, title, vote_average, vote_count
                                  FROM movieitems";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        movieItems.Add(new MovieItem
                        {
                            Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                            Adult = reader.IsDBNull(reader.GetOrdinal("adult")) ? false : reader.GetBoolean(reader.GetOrdinal("adult")),
                            Backdrop_path = reader.IsDBNull(reader.GetOrdinal("backdrop_path")) ? "" : reader.GetString(reader.GetOrdinal("backdrop_path")),
                            Original_language = reader.IsDBNull(reader.GetOrdinal("original_language")) ? "" : reader.GetString(reader.GetOrdinal("original_language")),
                            Original_title = reader.IsDBNull(reader.GetOrdinal("original_title")) ? "" : reader.GetString(reader.GetOrdinal("original_title")),
                            Overview = reader.IsDBNull(reader.GetOrdinal("overview")) ? "" : reader.GetString(reader.GetOrdinal("overview")),
                            Popularity = reader.IsDBNull(reader.GetOrdinal("popularity")) ? 0.0 : reader.GetDouble(reader.GetOrdinal("popularity")),
                            Poster_path = reader.IsDBNull(reader.GetOrdinal("poster_path")) ? "" : reader.GetString(reader.GetOrdinal("poster_path")),
                            Release_date = reader.IsDBNull(reader.GetOrdinal("release_date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("release_date")),
                            Title = reader.IsDBNull(reader.GetOrdinal("title")) ? "" : reader.GetString(reader.GetOrdinal("title")),
                            Vote_average = reader.IsDBNull(reader.GetOrdinal("vote_average")) ? 0.0 : reader.GetDouble(reader.GetOrdinal("vote_average")),
                            Vote_count = reader.IsDBNull(reader.GetOrdinal("vote_count")) ? 0 : reader.GetInt32(reader.GetOrdinal("vote_count")),
                        });
                    }
                }

                MovieItems = movieItems;
                SearchResult = $"즐겨찾기 검색 건수 : {MovieItems.Count.ToString()}개의 즐겨찾기 항목이 있습니다.";
                Common.LOGGER.Info(SearchResult + " 검색완료!!");
            }
            catch (Exception ex)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
                Common.LOGGER.Fatal(ex.Message);
            }
        }

        [RelayCommand]
        public async Task DelFavoriteMovies()
        {
            // 즐겨찾기 목록이 비었을 경우
            if (MovieItems == null || MovieItems.Count == 0)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 삭제", "즐겨찾기 항목이 없습니다.");
                return;
            }

            // 선택한 항목이 없을 경우
            if (SelectedMovieItem == null)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 삭제", "삭제할 영화를 선택하세요.");
                return;
            }

            try
            {
                var query = @"DELETE FROM movieitems WHERE id = @id";

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", SelectedMovieItem.Id);

                    var resultCnt = cmd.ExecuteNonQuery();

                    if (resultCnt > 0)
                    {
                        await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 삭제", "즐겨찾기 삭제 완료");
                        Common.LOGGER.Info($"즐겨찾기 삭제 완료 : {SelectedMovieItem.Title}");
                        await ViewFavoriteMovies(); // 화면 갱신
                    }
                    else
                    {
                        await this.dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 삭제", "삭제할 항목이 없습니다.");
                        Common.LOGGER.Warn($"삭제 실패 또는 항목 없음 : {SelectedMovieItem.Title}");
                    }
                }
            }
            
            catch (Exception ex)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
                Common.LOGGER.Fatal(ex.Message);
            }
        }

        [RelayCommand]
        public async Task ViewMoviesTrailer()
        {
            //await this.dialogCoordinator.ShowMessageAsync(this, "예고편", "예고편 기능은 준비중입니다.");
        }

    }
}
