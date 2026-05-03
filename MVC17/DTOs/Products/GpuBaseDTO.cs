using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Products
{
    public class GpuBaseDTO
    {
        [Display(Name = "Dùng Lượng Bộ Nhớ (GB)")]
        [DisplayFormat(DataFormatString = "{0:F1}")]
        [Required(ErrorMessage = "Dùng lượng bộ nhớ là bắt buộc")]
        [Range(0.5f, 48f, ErrorMessage = "Dùng lượng phải từ 0.5 đến 48 GB")]
        public float MemorySize { get; set; }

        [Display(Name = "Loại Bộ Nhớ")]
        [Required(ErrorMessage = "Loại bộ nhớ là bắt buộc")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Loại bộ nhớ phải từ 2 đến 50 ký tự")]
        public string MemoryType { get; set; } = null!;

        [Display(Name = "Tốc Độ đồng Hẻ (MHz)")]
        [Required(ErrorMessage = "Tốc độ đồng hẻ là bắt buộc")]
        [Range(600, 3000, ErrorMessage = "Tốc độ phải từ 600 đến 3000 MHz")]
        public int Clock { get; set; }

        [Display(Name = "Uniform Shader")]
        [Required(ErrorMessage = "Uniform Shader là bắt buộc")]
        [Range(64, 16384, ErrorMessage = "Uniform Shader phải từ 64 đến 16384")]
        public int UnifiedShader { get; set; }

        [Display(Name = "TMU")]
        [Required(ErrorMessage = "TMU là bắt buộc")]
        [Range(16, 2048, ErrorMessage = "TMU phải từ 16 đến 2048")]
        public int Tmu { get; set; }

        [Display(Name = "ROP")]
        [Required(ErrorMessage = "ROP là bắt buộc")]
        [Range(8, 512, ErrorMessage = "ROP phải từ 8 đến 512")]
        public int Rop { get; set; }

        [Display(Name = "Bus")]
        [Required(ErrorMessage = "Bus là bắt buộc")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Bus phải từ 2 đến 50 ký tự")]
        public string Bus { get; set; } = null!;

        [Display(Name = "IGPU")]
        public bool? Igpu { get; set; }
    }
}
