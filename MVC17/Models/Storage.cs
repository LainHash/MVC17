using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Storage
{
    public int StorageId { get; set; }

    public int Capacity { get; set; }

    public string MemoryType { get; set; } = null!;

    public string InterfaceType { get; set; } = null!;

    public int ReadSpeed { get; set; }

    public int WriteSpeed { get; set; }

    public int? ProductSkuId { get; set; }

    public virtual ICollection<LaptopComponent> LaptopComponents { get; set; } = new List<LaptopComponent>();

    public virtual ProductSku? ProductSku { get; set; }
}
