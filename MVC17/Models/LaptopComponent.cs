using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class LaptopComponent
{
    public int LaptopComponentId { get; set; }

    public int CpuId { get; set; }

    public int RamId { get; set; }

    public int StorageId { get; set; }

    public int? GpuId { get; set; }

    public virtual Cpu Cpu { get; set; } = null!;

    public virtual Gpu? Gpu { get; set; }

    public virtual ICollection<Laptop> Laptops { get; set; } = new List<Laptop>();

    public virtual Ram Ram { get; set; } = null!;

    public virtual Storage Storage { get; set; } = null!;
}
