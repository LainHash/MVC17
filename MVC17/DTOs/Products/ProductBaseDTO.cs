using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Products
{
    public class ProductBaseDTO
    {
        [Display(Name = "Tên Sản Phẩm")]
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên sản phẩm phải từ 2 đến 255 ký tự")]
        public string ProductName { get; set; } = null!;

        [Display(Name = "Mô Tả")]
        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "ID Danh Mục")]
        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Danh mục không hợp lệ")]
        public int CategoryId { get; set; }

        [Display(Name = "ID Nhà Cung Cấp")]
        [Required(ErrorMessage = "Nhà cung cấp là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Nhà cung cấp không hợp lệ")]
        public int SupplierId { get; set; }

        [Display(Name = "URL Hình Ảnh")]
        [StringLength(500)]
        [Url(ErrorMessage = "URL hình Ảnh không hợp lệ")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Giá Đơn Vị")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Required(ErrorMessage = "Giá đơn vị là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá phải là số dương")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Tổng Số Hàng")]
        [Required(ErrorMessage = "Tổng số hàng là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Tổng số hàng phải là số dương")]
        public int UnitsInStock { get; set; }
    }
}
