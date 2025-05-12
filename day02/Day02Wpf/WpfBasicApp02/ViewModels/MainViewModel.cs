using Caliburn.Micro;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using WpfBasicApp02.Models;
using static System.Reflection.Metadata.BlobBuilder;
using System.Net.Sockets;
using MahApps.Metro.Controls.Dialogs;
namespace WpfBasicApp02.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private readonly IDialogCoordinator _dialogCoordinator; // msg박스, 다이얼로그 실행을 위한 방식
        public ObservableCollection<KeyValuePair<string, string>> Divisions {  get; set; }
        public ObservableCollection<Book> Books { get; set; } = new ObservableCollection<Book>();

        private Book _selectedBook;

        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                NotifyOfPropertyChange(() => SelectedBook);
            }
        }

        private string NetTest()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("127.0.0.1", 9000);

            const string Value = "CHAT 박관호 반갑습니다.\n";
            byte[] msg = Encoding.UTF8.GetBytes(Value);
            socket.Send(msg);

            // 응답 받기
            byte[] buffer = new byte[1024];
            int receivedLength = socket.Receive(buffer);
            string receivedText = Encoding.UTF8.GetString(buffer, 0, receivedLength);
            Console.WriteLine($"받은 메시지: {receivedText}");

            socket.Close();
            return receivedText;
        }

        public async void DoAction()
        {
            string msg = NetTest();
            System.Console.WriteLine(msg);
            //await _dialogCoordinator.ShowMessageAsync(this,"서버통신 결과", msg);
        }


        public MainViewModel()
        {
            LoadControlFromDb();
            LoadGridFromDb();
        }

        private void LoadControlFromDb()
        {
            string connStr = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=root;Charset=utf8;";
            string query = "SELECT division, names FROM divtbl";

            ObservableCollection<KeyValuePair<string, string>> divisions = new ObservableCollection<KeyValuePair<string, string>>();

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var division = reader.GetString("division");
                        var names = reader.GetString("names");

                        divisions.Add(new KeyValuePair<string, string>(division, names));
                    }
                }
                catch (MySqlException ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
            Divisions = divisions;
            NotifyOfPropertyChange(() => Divisions);
        }

        private void LoadGridFromDb()
        {
            string connStr = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=root;Charset=utf8;";
            string query = @"SELECT b.Idx, b.Author, b.Division, b.Names, b.ReleaseDate, b.ISBN, b.Price,
                            d.Names AS dNames
                            FROM bookstbl AS b, divtbl AS d
                            WHERE b.Division = d.Division
                            ORDER by b.Idx";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        Books.Add(new Book
                        {
                            Idx = Convert.ToInt32(dr["Idx"]),
                            Division = Convert.ToString(dr["Division"]),
                            DNames = Convert.ToString(dr["dNames"]),
                            Names = Convert.ToString(dr["Names"]),
                            Author = Convert.ToString(dr["Author"]),
                            ISBN = Convert.ToString(dr["ISBN"]),
                            ReleaseDate = Convert.ToDateTime(dr["ReleaseDate"]),
                            Price = Convert.ToInt32(dr["Price"])
                        });
                    }
                }
                catch (MySqlException ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
            NotifyOfPropertyChange(() => Books);
        }
    }
}
