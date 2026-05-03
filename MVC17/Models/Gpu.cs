using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Gpu
{
    public int GpuId { get; set; }

    public float MemorySize { get; set; }

    public string MemoryType { get; set; } = null!;

    public int Clock { get; set; }

    public int UnifiedShader { get; set; }

    public int Tmu { get; set; }

    public int Rop { get; set; }

    public string Bus { get; set; } = null!;

    public bool? Igpu { get; set; }

    public int ProductSkuId { get; set; }

    public virtual ICollection<LaptopComponent> LaptopComponents { get; set; } = new List<LaptopComponent>();

    public virtual ProductSku ProductSku { get; set; } = null!;
}
