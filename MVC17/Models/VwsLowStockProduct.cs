using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwsLowStockProduct
{
    public int ProductId { get; set; }

    public Guid ProductUuid { get; set; }

    public string ProductName { get; set; } = null!;

    public int UnitsInStock { get; set; }
}
