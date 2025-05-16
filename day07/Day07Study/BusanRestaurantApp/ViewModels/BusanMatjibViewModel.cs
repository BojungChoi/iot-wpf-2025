using BusanRestaurantApp.Helpers;
using BusanRestaurantApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BusanRestaurantApp.ViewModels
{
    public partial class BusanMatjibViewModel : ObservableObject
    {
        IDialogCoordinator dialogCoordinator;
        public BusanMatjibViewModel(IDialogCoordinator coordinator)
        {
            dialogCoordinator = coordinator;

            GetDataFromOpenApi();
        }

        [ObservableProperty]
        ObservableCollection<BusanItem> _busanItems;
        private async Task GetDataFromOpenApi()
        {
            string baseUri= "http://apis.data.go.kr/6260000/FoodService/getFoodKr";
            string myServiceKey = "NE9Hd%2FEVKMnL0FNyOa3XWVQGM2nWO0%2FtxHcwGAkeA3OtQnnfY6yQIdZgyknfdxfwZR7hEiDVnf7RWil7m7oarQ%3D%3D";
            // serviceKey=NE9Hd%2FEVKMnL0FNyOa3XWVQGM2nWO0%2FtxHcwGAkeA3OtQnnfY6yQIdZgyknfdxfwZR7hEiDVnf7RWil7m7oarQ%3D%3D&pageNo=1&numOfRows=10&resultType=json

            StringBuilder strUri = new StringBuilder();
            strUri.Append($"serviceKey={myServiceKey}&");
            strUri.Append($"pageNo={1}&");
            strUri.Append($"&numOfRows={3}&");
            strUri.Append($"resultType=json");
            string totalOpenApi = $"{baseUri}?{strUri}";
            Common.LOGGER.Info($"OpenApi URL: {totalOpenApi}");


            try
            {
                HttpClient client = new HttpClient();
                ObservableCollection<BusanItem> busanItems = new ObservableCollection<BusanItem>();
                var response = await client.GetStringAsync(totalOpenApi);
                Common.LOGGER.Info(response);

                // Newtonsoft.Json으로 Json 파싱
                var jsonResult = JObject.Parse(response);
                var message = jsonResult["getFoodKr"]["header"]["message"];
                //await this.dialogCoordinator.ShowMessageAsync(this, "Message", message.ToString());
                var status = Convert.ToString(jsonResult["getFoodKr"]["header"]["resultCode"]); // 00 이면 성공!

                if (status == "00")
                {
                    var item = jsonResult["getFoodKr"]["item"];
                    var jsonArray = item as JArray;

                    foreach (var subitem in jsonArray)
                    {
                        //Common.LOGGER.Info(subitem.ToString());
                        busanItems.Add(new BusanItem
                        {
                            UcSeq = Convert.ToInt32(subitem["UC_SEQ"]),
                            MainTitle = Convert.ToString(subitem["MAIN_TITLE"]),
                            GugunNm = Convert.ToString(subitem["GUGUN_NM"]),
                            Lat = Convert.ToDouble(subitem["LAT"]),
                            Lng = Convert.ToDouble(subitem["LNG"]),
                            Place = Convert.ToString(subitem["PLACE"]),
                            Title = Convert.ToString(subitem["TITLE"]),
                            Subtitle = Convert.ToString(subitem["SUBTITLE"]),
                            Addr1 = Convert.ToString(subitem["ADDR1"]),
                            Addr2 = Convert.ToString(subitem["ADDR2"]),
                            CntctTel = Convert.ToString(subitem["CNTCT_TEL"]),
                            HomepageUrl = Convert.ToString(subitem["HOMEPAGE_URL"]),
                            UsageDayWeekAndTime = Convert.ToString(subitem["USAGE_DAY_WEEK_AND_TIME"]),
                            RprsntvMenu = Convert.ToString(subitem["RPRSNTV_MENU"]),
                            MainImgNormal = Convert.ToString(subitem["MAIN_IMG_NORMAL"]),
                            MainImgThumb = Convert.ToString(subitem["MAIN_IMG_THUMB"]),
                            Itemcntnts = Convert.ToString(subitem["ITEMCNTNTS"])
                        });
                    }
                    BusanItems = busanItems;

                }
            }
            catch (Exception ex)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "Error", ex.Message);
                Common.LOGGER.Fatal($"Error: {ex.Message}");

            }
        }
    }
}
