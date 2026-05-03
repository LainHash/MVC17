using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Products
{
    public class RamBaseDTO
    {
        [Display(Name = "Dùng Lượng (GB)")]
        [Required(ErrorMessage = "Dùng lượng là bắt buộc")]
        [Range(1, 192, ErrorMessage = "Dùng lượng phải từ 1 đến 192 GB")]
        public int Capacity { get; set; }

        [Display(Name = "Thế Họ")]
        [Required(ErrorMessage = "Thế họ là bắt buộc")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Thế họ phải từ 2 đến 50 ký tự")]
        public string Gen { get; set; } = null!;

        [Display(Name = "Tốc Độ (MHz)")]
        [Required(ErrorMessage = "Tốc độ là bắt buộc")]
        [Range(1600, 8000, ErrorMessage = "Tốc độ phải từ 1600 đến 8000 MHz")]
        public int Speed { get; set; }

        [Display(Name = "Kit")]
        [Required(ErrorMessage = "Kit là bắt buộc")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Kit phải từ 1 đến 50 ký tự")]
        public string Kit { get; set; } = null!;
    }
}
