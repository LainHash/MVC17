using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwCpuSpec
{
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
