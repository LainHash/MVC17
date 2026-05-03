using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwGpuSpec
{
    public int GpuId { get; set; }

    public string GpuName { get; set; } = null!;

    public float MemorySize { get; set; }

    public string MemoryType { get; set; } = null!;

    public int Clock { get; set; }

    public int UnifiedShader { get; set; }

    public int Tmu { get; set; }

    public int Rop { get; set; }

    public bool? Igpu { get; set; }

    public string Bus { get; set; } = null!;

    public int ProductSkuId { get; set; }
}
