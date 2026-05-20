using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwTopSellingProduct
{
    public int ProductId { get; set; }

    public Guid ProductUuid { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public int? TotalSelling { get; set; }
}
