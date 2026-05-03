using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Products
{
    public class StorageBaseDTO
    {
        [Display(Name = "Dùng Lượng (GB)")]
        [Required(ErrorMessage = "Dùng lượng là bắt buộc")]
        [Range(64, 8192, ErrorMessage = "Dùng lượng phải từ 64 đến 8192 GB")]
        public int Capacity { get; set; }

        [Display(Name = "Loại Bộ Nhớ")]
        [Required(ErrorMessage = "Loại bộ nhớ là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Loại bộ nhớ phải từ 2 đến 100 ký tự")]
        public string MemoryType { get; set; } = null!;

        [Display(Name = "Loại Giao Tiếp")]
        [Required(ErrorMessage = "Loại giao tiếp là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Loại giao tiếp phải từ 2 đến 100 ký tự")]
        public string InterfaceType { get; set; } = null!;

        [Display(Name = "Tốc Độ Đọc (MB/s)")]
        [Required(ErrorMessage = "Tốc độ đọc là bắt buộc")]
        [Range(50, 20000, ErrorMessage = "Tốc độ đọc phải từ 50 đến 20000 MB/s")]
        public int ReadSpeed { get; set; }

        [Display(Name = "Tốc Độ Ghi (MB/s)")]
        [Required(ErrorMessage = "Tốc độ ghi là bắt buộc")]
        [Range(50, 20000, ErrorMessage = "Tốc độ ghi phải từ 50 đến 20000 MB/s")]
        public int WriteSpeed { get; set; }
    }
}
