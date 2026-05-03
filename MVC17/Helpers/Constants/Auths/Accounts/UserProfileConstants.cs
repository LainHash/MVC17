namespace MVC17.Helpers.Constants.Auths.Accounts
{
    public static class UserProfileConstants
    {
        public static List<string> Cities = new List<string>() {
            "Tuyên Quang",
            "Cao Bằng",
            "Lai Châu",
            "Lào Cai",
            "Thái Nguyên",
            "Điện Biên",
            "Lạng Sơn",
            "Sơn La",
            "Phú Thọ",
            "Bắc Ninh",
            "Quảng Ninh",
            "Hà Nội",
            "Hải Phòng",
            "Hưng Yên",
            "Ninh Bình",
            "Thanh Hóa",
            "Nghệ An",
            "Hà Tĩnh",
            "Quảng Trị",
            "Huế",
            "Đà Nẵng",
            "Quảng Ngãi",
            "Gia Lai",
            "Đắk Lắk",
            "Khánh Hòa",
            "Lâm Đồng",
            "Đồng Nai",
            "Tây Ninh",
            "Hồ Chí Minh",
            "Đồng Tháp",
            "An Giang",
            "Vĩnh Long",
            "Cần Thơ",
            "Cà Mau"
        }.OrderBy(city => city).ToList();

        public static List<string> Countries = new List<string>() {
            "Việt Nam"
            //"Mỹ",
            //"Anh",
            //"Pháp",
            //"Đức",
            //"Nhật Bản",
            //"Hàn Quốc",
            //"Trung Quốc",
            //"Úc",
            //"Canada"
        }.OrderBy(country => country).ToList();

        public static Dictionary<bool, string> Genders = new Dictionary<bool, string>()
        {
            { true, "Nam" },
            { false, "Nữ" }
        };
    }
}


