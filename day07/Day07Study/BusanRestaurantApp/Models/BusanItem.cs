using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusanRestaurantApp.Models
{
    public partial class BusanItem : ObservableObject
    {
        /*
            UC_SEQ: 70
            MAIN_TITLE: "만드리곤드레밥"
            GUGUN_NM: "강서구"
            LAT: 35.177387
            LNG: 128.95245
            PLACE: "만드리곤드레밥"
            TITLE: "만드리곤드레밥"
            SUBTITLE: ""
            ADDR1: "강서구 공항앞길 85번길 13"
            ADDR2: ""
            CNTCT_TEL: "051-941-3669"
            HOMEPAGE_URL: ""
            USAGE_DAY_WEEK_AND_TIME: "10:00-20:00 (19:50 라스트오더)"
            RPRSNTV_MENU: "돌솥곤드레정식, 단호박오리훈제"
            MAIN_IMG_NORMAL: "https://www.visitbusan.net/uploadImgs/files/cntnts/20191209162810545_ttiel"
            MAIN_IMG_THUMB: "https://www.visitbusan.net/uploadImgs/files/cntnts/20191209162810545_thumbL"
            ITEMCNTNTS: "곤드레밥에는 일반적으로 건조 곤드레나물이 사용되는데, 이곳은 생 곤드레나물을 사용하여 돌솥밥을 만든다. 된장찌개와 함께 열 가지가 넘는 반찬이 제공되는 돌솥곤드레정식이 인기있다 "
         */
        [ObservableProperty] private int _ucSeq;
        [ObservableProperty] private string _mainTitle;
        [ObservableProperty] private string _gugunNm;
        [ObservableProperty] private double _lat;
        [ObservableProperty] private double _lng;
        [ObservableProperty] private string _place;
        [ObservableProperty] private string _title;
        [ObservableProperty] private string _subtitle;
        [ObservableProperty] private string _addr1;
        [ObservableProperty] private string _addr2;
        [ObservableProperty] private string _cntctTel;
        [ObservableProperty] private string _homepageUrl;
        [ObservableProperty] private string _usageDayWeekAndTime;
        [ObservableProperty] private string _rprsntvMenu;
        [ObservableProperty] private string _mainImgNormal;
        [ObservableProperty] private string _mainImgThumb;
        [ObservableProperty] private string _itemcntnts;
    }
}
