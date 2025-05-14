using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieFinder2025.Models
{
    public partial class MovieItem : ObservableObject
    {
        /*
               API 항목
              "adult": false,
              "backdrop_path": "/l33oR0mnvf20avWyIMxW02EtQxn.jpg",
              "genre_ids": [12, 18, 878],
              "id": 157336,
              "original_language": "en",
              "original_title": "Interstellar",
              "overview": "세계 각국의 정부와 경제가 ...
              "popularity": 36.3951,
              "poster_path": "/evoEi8SBSvIIEveM3V6nCJ6vKj8.jpg",
              "release_date": "2014-11-05",
              "title": "인터스텔라",
              "video": false,
              "vote_average": 8.455,
              "vote_count": 37092
         */
        [ObservableProperty]
        private bool _adult;
        [ObservableProperty]
        private string _backdrop_path; // 사용안함
        [ObservableProperty]
        private List<int> _genre_ids; // 사용안함
        [ObservableProperty]
        private int _id;
        [ObservableProperty]
        private string _original_language;
        [ObservableProperty]
        private string _original_title;
        [ObservableProperty]
        private string _overview;
        [ObservableProperty]
        private double _popularity;
        [ObservableProperty]
        private string _poster_path;
        [ObservableProperty]
        private DateTime _release_date;
        [ObservableProperty]
        private string _title;
        [ObservableProperty]
        private bool _video;
        [ObservableProperty]
        private double _vote_average;
        [ObservableProperty]
        private int _vote_count;

        //public bool Adult { get => _adult; set => SetProperty(ref _Adult, value);  --> [ObservableProperty] 로 대체가능
    }
}
