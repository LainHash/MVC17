using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwLaptopSpec
{
    public int LaptopId { get; set; }

    public string LaptopType { get; set; } = null!;

    public string LaptopName { get; set; } = null!;

    public string Os { get; set; } = null!;

    public string ScreenResolution { get; set; } = null!;

    public float Length { get; set; }

    public float Weight { get; set; }

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

    public int RamId { get; set; }

    public string RamName { get; set; } = null!;

    public int RamCapacity { get; set; }

    public string Gen { get; set; } = null!;

    public int RamSpeed { get; set; }

    public string Kit { get; set; } = null!;

    public int StorageCapacity { get; set; }

    public string StorageType { get; set; } = null!;

    public string InterfaceType { get; set; } = null!;

    public int ReadSpeed { get; set; }

    public int WriteSpeed { get; set; }

    public string StorageName { get; set; } = null!;

    public int StorageId { get; set; }

    public int CpuId { get; set; }

    public string CpuName { get; set; } = null!;

    public int Cores { get; set; }

    public int Logicals { get; set; }

    public float Tdp { get; set; }

    public string Socket { get; set; } = null!;

    public int CpuSpeed { get; set; }

    public int Turbo { get; set; }

    public int ProductSkuId { get; set; }
}
