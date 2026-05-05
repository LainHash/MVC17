using System.ComponentModel.DataAnnotations;

namespace MVC17.ViewModels
{
    public class TrendingLaptopVM
    {
        [Display(Name = "Mã laptop")]
        public int LaptopId { get; set; }

        [Display(Name = "Loại laptop")]
        public string LaptopType { get; set; } = null!;

        [Display(Name = "Hệ điều hành")]
        public string Os { get; set; } = null!;

        [Display(Name = "Độ phân giải màn hình")]
        public string ScreenResolution { get; set; } = null!;

        [Display(Name = "Kích thước")]
        [DisplayFormat(DataFormatString = "{0:0.##} inch")]
        public float Length { get; set; }

        [Display(Name = "Khối lượng")]
        [DisplayFormat(DataFormatString = "{0:0.##} kg")]
        public float Weight { get; set; }

        [Display(Name = "Card đồ họa")]
        public string GpuName { get; set; } = null!;

        [Display(Name = "Bộ xử lý")]
        public string CpuName { get; set; } = null!;

        [Display(Name = "RAM")]
        public string RamName { get; set; } = null!;

        [Display(Name = "Ổ cứng")]
        public string StorageName { get; set; } = null!;

        [Display(Name = "Đơn giá")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Số lượng tồn")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int UnitsInStock { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; } = null!;

        [Display(Name = "Số hóa đơn đã tạo")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? CreatedInvoices { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Hãng sản xuất")]
        public string CompanyName { get; set; } = null!;

        [Display(Name = "Danh mục")]
        public string CategoryName { get; set; } = null!;

        [Display(Name = "Ảnh sản phẩm")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Mã sản phẩm")]
        public int ProductId { get; set; }
    }
}
