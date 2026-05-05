using System.ComponentModel.DataAnnotations;

namespace MVC17.ViewModels
{
    public class TrendingProductVM
    {
        [Display(Name = "Đơn giá")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ", ApplyFormatInEditMode = false)]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Số lượng tồn kho")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int UnitsInStock { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; } = null!;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Mã sản phẩm")]
        public int ProductId { get; set; }

        [Display(Name = "Số hóa đơn đã tạo")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? CreatedInvoices { get; set; }

        [Display(Name = "Mã danh mục")]
        public int CategoryId { get; set; }
    }
}
