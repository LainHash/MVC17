using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Products
{
    public class CpuBaseDTO
    {
        [Display(Name = "Số Nhân")]
        [Required(ErrorMessage = "Số nhân là bắt buộc")]
        [Range(1, 128, ErrorMessage = "Số nhân phải từ 1 đến 128")]
        public int Cores { get; set; }

        [Display(Name = "Tối Đa Threads")]
        [Required(ErrorMessage = "Số threads tối đa là bắt buộc")]
        [Range(1, 512, ErrorMessage = "Số threads phải từ 1 đến 512")]
        public int Logicals { get; set; }

        [Display(Name = "TDP (W)")]
        [DisplayFormat(DataFormatString = "{0:F1}")]
        [Required(ErrorMessage = "TDP là bắt buộc")]
        [Range(5, 500, ErrorMessage = "TDP phải từ 5 đến 500 W")]
        public float Tdp { get; set; }

        [Display(Name = "Socket")]
        [Required(ErrorMessage = "Socket là bắt buộc")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Socket phải từ 2 đến 50 ký tự")]
        public string Socket { get; set; } = null!;

        [Display(Name = "Tốc Độ Cơ Bản (MHz)")]
        [Required(ErrorMessage = "Tốc độ cơ bản là bắt buộc")]
        [Range(800, 8000, ErrorMessage = "Tốc độ phải từ 800 đến 8000 MHz")]
        public int Speed { get; set; }

        [Display(Name = "Tốc Độ Tùrbô (MHz)")]
        [Required(ErrorMessage = "Tốc độ tùrbô là bắt buộc")]
        [Range(800, 8000, ErrorMessage = "Tốc độ tùrbô phải từ 800 đến 8000 MHz")]
        public int Turbo { get; set; }
    }
}
