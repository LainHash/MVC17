using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwStorageSpec
{
    public int StorageCapacity { get; set; }

    public string StorageType { get; set; } = null!;

    public string InterfaceType { get; set; } = null!;

    public int ReadSpeed { get; set; }

    public int WriteSpeed { get; set; }

    public int? ProductSkuId { get; set; }

    public string StorageName { get; set; } = null!;

    public int StorageId { get; set; }
}
