using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace WpfBookRentalShop01.Models
{
    public class Rental : ObservableObject
    {
        private int _idx;
        public int Idx
        {
            get => _idx;
            set => SetProperty(ref _idx, value);
        }

        private int _memberIdx;
        public int MemberIdx
        {
            get => _memberIdx;
            set => SetProperty(ref _memberIdx, value);
        }

        private int _bookIdx;
        public int BookIdx
        {
            get => _bookIdx;
            set => SetProperty(ref _bookIdx, value);
        }

        private DateTime _rentalDate = DateTime.Now;
        public DateTime RentalDate
        {
            get => _rentalDate;
            set => SetProperty(ref _rentalDate, value);
        }

        private DateTime? _returnDate;
        public DateTime? ReturnDate
        {
            get => _returnDate;
            set => SetProperty(ref _returnDate, value);
        }

        private string _memberNames;
        public string MemberNames
        {
            get => _memberNames;
            set => SetProperty(ref _memberNames, value);
        }

        private string _bookNames;
        public string BookNames
        {
            get => _bookNames;
            set => SetProperty(ref _bookNames, value);
        }

        public override string ToString()
        {
            return $"[{Idx}] {MemberNames} - {BookNames} ({RentalDate:yyyy-MM-dd})";
        }
    }
}
