using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Cpu
{
    public int CpuId { get; set; }

    public int Cores { get; set; }

    public int Logicals { get; set; }

    public float Tdp { get; set; }

    public string Socket { get; set; } = null!;

    public int Speed { get; set; }

    public int Turbo { get; set; }

    public int ProductSkuId { get; set; }

    public virtual ICollection<LaptopComponent> LaptopComponents { get; set; } = new List<LaptopComponent>();

    public virtual ProductSku ProductSku { get; set; } = null!;
}
