using System.ComponentModel.DataAnnotations;

namespace MVC17.DTOs.Products.Update
{
    public class UpdateProductDTO : ProductBaseDTO
    {
        [Display(Name = "Ngừng Cấp")]
        public bool Discontinued { get; set; }

        [Display(Name = "Laptop")]
        public UpdateLaptopDTO? Laptop { get; set; }

        [Display(Name = "CPU")]
        public UpdateCpuDTO? Cpu { get; set; }

        [Display(Name = "GPU")]
        public UpdateGpuDTO? Gpu { get; set; }

        [Display(Name = "RAM")]
        public UpdateRamDTO? Ram { get; set; }

        [Display(Name = "Bộ Nhớ")]
        public UpdateStorageDTO? Storage { get; set; }
    }
}
