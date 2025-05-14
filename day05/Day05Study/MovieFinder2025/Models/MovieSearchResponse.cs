using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieFinder2025.Models
{
    public partial class MovieSearchResponse : ObservableObject
    {
        [ObservableProperty]
        private int _page;
        [ObservableProperty]
        private List<MovieItem> _results;
        [ObservableProperty]
        private int _total_pages;
        [ObservableProperty]
        private int _total_results;
    }
}
