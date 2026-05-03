using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Laptop
{
    public int LaptopId { get; set; }

    public string LaptopType { get; set; } = null!;

    public int ProductSkuId { get; set; }

    public string Os { get; set; } = null!;

    public string ScreenResolution { get; set; } = null!;

    public float Length { get; set; }

    public float Weight { get; set; }

    public int LaptopComponentId { get; set; }

    public virtual LaptopComponent LaptopComponent { get; set; } = null!;

    public virtual ProductSku ProductSku { get; set; } = null!;
}
