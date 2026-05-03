using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Products.Create
{
    public class CreateProductDTO : ProductBaseDTO
    {
        [Display(Name = "Laptop")]
        public CreateLaptopDTO? Laptop { get; set; }

        [Display(Name = "CPU")]
        public CreateCpuDTO? Cpu { get; set; }

        [Display(Name = "GPU")]
        public CreateGpuDTO? Gpu { get; set; }

        [Display(Name = "RAM")]
        public CreateRamDTO? Ram { get; set; }

        [Display(Name = "Bộ Nhớ")]
        public CreateStorageDTO? Storage { get; set; }
    }
}
