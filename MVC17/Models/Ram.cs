using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Ram
{
    public int RamId { get; set; }

    public int Capacity { get; set; }

    public string Gen { get; set; } = null!;

    public int Speed { get; set; }

    public string Kit { get; set; } = null!;

    public int ProductSkuId { get; set; }

    public virtual ICollection<LaptopComponent> LaptopComponents { get; set; } = new List<LaptopComponent>();

    public virtual ProductSku ProductSku { get; set; } = null!;
}
