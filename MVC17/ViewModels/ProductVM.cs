using System.ComponentModel.DataAnnotations;
using MVC17.Models;

namespace MVC17.ViewModels
{
    public class ProductVM : VwProduct
    {
        [Display(Name = "Chi Tiết Laptop")]
        public VwLaptopSpec? Laptop { get; set; }

        [Display(Name = "Chi Tiết CPU")]
        public VwCpuSpec? Cpu { get; set; }

        [Display(Name = "Chi Tiết GPU")]
        public VwGpuSpec? Gpu { get; set; }

        [Display(Name = "Chi Tiết RAM")]
        public VwRamSpec? Ram { get; set; }

        [Display(Name = "Chi Tiết Bộ Nhớ")]
        public VwStorageSpec? Storage { get; set; }

    }
}
