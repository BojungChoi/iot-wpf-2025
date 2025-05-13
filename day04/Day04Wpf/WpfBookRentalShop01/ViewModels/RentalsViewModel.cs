using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Models;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class RentalsViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator;

        private ObservableCollection<Rental> _rentals;
        public ObservableCollection<Rental> Rentals
        {
            get => _rentals;
            set => SetProperty(ref _rentals, value);
        }

        private Rental _selectedRental;
        public Rental SelectedRental
        {
            get => _selectedRental;
            set
            {
                SetProperty(ref _selectedRental, value);
                IsUpdate = true;
            }
        }

        private bool _isUpdate;
        public bool IsUpdate
        {
            get => _isUpdate;
            set => SetProperty(ref _isUpdate, value);
        }

        public RentalsViewModel(IDialogCoordinator coordinator)
        {
            dialogCoordinator = coordinator;
            LoadGridFromDb();
        }

        private void InitVariable()
        {
            SelectedRental = new Rental
            {
                Idx = 0,
                MemberIdx = 0,
                BookIdx = 0,
                RentalDate = DateTime.Now,
                ReturnDate = null
            };
            IsUpdate = false;
        }

        [RelayCommand]
        public void SetInit()
        {
            InitVariable();
        }

        [RelayCommand]
        public async void SaveData()
        {
            try
            {
                string query = string.Empty;

                using var conn = new MySqlConnection(Common.CONNSTR);
                conn.Open();

                if (_isUpdate)
                {
                    query = @"UPDATE rentaltbl
                      SET memberIdx = @memberIdx,
                          bookIdx = @bookIdx,
                          rentalDate = @rentalDate,
                          returnDate = @returnDate
                      WHERE idx = @idx";
                }
                else
                {
                    query = @"INSERT INTO rentaltbl (memberIdx, bookIdx, rentalDate, returnDate)
                      VALUES (@memberIdx, @bookIdx, @rentalDate, @returnDate)";
                }

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@memberIdx", SelectedRental.MemberIdx);
                cmd.Parameters.AddWithValue("@bookIdx", SelectedRental.BookIdx);
                cmd.Parameters.AddWithValue("@rentalDate", SelectedRental.RentalDate);
                cmd.Parameters.AddWithValue("@returnDate", SelectedRental.ReturnDate ?? (object)DBNull.Value);
                if (_isUpdate)
                    cmd.Parameters.AddWithValue("@idx", SelectedRental.Idx);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    await dialogCoordinator.ShowMessageAsync(this, "저장", "저장 성공!");
                }
                else
                {
                    await dialogCoordinator.ShowMessageAsync(this, "저장", "저장 실패!");
                }
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }

            LoadGridFromDb();
        }



        [RelayCommand]
        public async void DelData()
        {
            if (!IsUpdate)
            {
                await dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제할 데이터를 선택하세요.");
                return;
            }

            var result = await dialogCoordinator.ShowMessageAsync(this, "삭제", "정말 삭제하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
            if (result != MessageDialogResult.Affirmative) return;

            try
            {
                string query = "DELETE FROM rentaltbl WHERE idx = @idx";
                using var conn = new MySqlConnection(Common.CONNSTR);
                conn.Open();

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idx", SelectedRental.Idx);
                int deleteResult = cmd.ExecuteNonQuery();

                if (deleteResult > 0)
                    await dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제 완료!");
                else
                    await dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제 실패!");
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }

            LoadGridFromDb();
        }

        private async void LoadGridFromDb()
        {
            try
            {
                string query = @"
            SELECT r.idx, r.memberIdx, m.names AS memberName,
                   r.bookIdx, b.Names AS bookTitle,
                   r.rentalDate, r.returnDate
            FROM rentaltbl r
            JOIN membertbl m ON r.memberIdx = m.idx
            JOIN bookstbl b ON r.bookIdx = b.idx";

                var list = new ObservableCollection<Rental>();

                using var conn = new MySqlConnection(Common.CONNSTR);
                conn.Open();
                var cmd = new MySqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Rental
                    {
                        Idx = reader.GetInt32("idx"),
                        MemberIdx = reader.GetInt32("memberIdx"),
                        BookIdx = reader.GetInt32("bookIdx"),
                        RentalDate = reader.GetDateTime("rentalDate"),
                        ReturnDate = reader.IsDBNull(reader.GetOrdinal("returnDate"))
                                        ? null
                                        : reader.GetDateTime("returnDate"),

                        MemberNames = reader.GetString("memberName"),
                        BookNames = reader.GetString("bookTitle")
                    });
                }

                Rentals = list;
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }
        }

    }
}
