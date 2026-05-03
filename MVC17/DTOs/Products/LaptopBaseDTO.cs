using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Products
{
    public class LaptopBaseDTO
    {
        [Display(Name = "Loại Laptop")]
        [Required(ErrorMessage = "Loại laptop là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Loại laptop phải từ 2 đến 100 ký tự")]
        public string LaptopType { get; set; } = null!;

        [Display(Name = "ID SKU Sản Phẩm")]
        [Required(ErrorMessage = "SKU sản phẩm là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "SKU sản phẩm không hợp lệ")]
        public int ProductSkuId { get; set; }

        [Display(Name = "Hệ Điều Hành")]
        [Required(ErrorMessage = "Hệ điều hành là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Hệ điều hành phải từ 2 đến 100 ký tự")]
        public string Os { get; set; } = null!;

        [Display(Name = "Phân Giải Màn Hình")]
        [Required(ErrorMessage = "Phân giải màn hình là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Phân giải phải từ 2 đến 100 ký tự")]
        public string ScreenResolution { get; set; } = null!;

        [Display(Name = "Chiều Dài (cm)")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Required(ErrorMessage = "Chiều dài là bắt buộc")]
        [Range(20, 50, ErrorMessage = "Chiều dài phải từ 20 đến 50 cm")]
        public float Length { get; set; }

        [Display(Name = "Trọn Lượng (kg)")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Required(ErrorMessage = "Trọn lượng là bắt buộc")]
        [Range(0.5f, 10f, ErrorMessage = "Trọn lượng phải từ 0.5 đến 10 kg")]
        public float Weight { get; set; }

        [Display(Name = "ID CPU")]
        [Required(ErrorMessage = "CPU là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "CPU không hợp lệ")]
        public int CpuId { get; set; }

        [Display(Name = "ID RAM")]
        [Required(ErrorMessage = "RAM là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "RAM không hợp lệ")]
        public int RamId { get; set; }

        [Display(Name = "ID Bộ Nhớ")]
        [Required(ErrorMessage = "Bộ nhớ là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Bộ nhớ không hợp lệ")]
        public int StorageId { get; set; }

        [Display(Name = "ID GPU")]
        [Range(1, int.MaxValue, ErrorMessage = "GPU không hợp lệ")]
        public int? GpuId { get; set; }
    }
}
