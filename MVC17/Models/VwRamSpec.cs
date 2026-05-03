using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwRamSpec
{
    public int RamId { get; set; }

    public int ProductSkuId { get; set; }

    public string RamName { get; set; } = null!;

    public int RamCapacity { get; set; }

    public string Gen { get; set; } = null!;

    public int RamSpeed { get; set; }

    public string Kit { get; set; } = null!;
}
