using System.ComponentModel.DataAnnotations;

namespace MVC17.ViewModels
{
    public class CategoryMenuVM
    {
        [Display(Name = "ID Danh Mục")]
        public int CategoryId { get; set; }

        [Display(Name = "Tên Danh Mục")]
        public string Name { get; set; } = null!;

        [Display(Name = "Số Lượng Sản Phẩm")]
        public int Count { get; set; } = 0;
    }
}
