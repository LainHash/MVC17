using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwTrendingProduct
{
    public decimal UnitPrice { get; set; }

    public int UnitsInStock { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int ProductId { get; set; }

    public int? CreatedInvoices { get; set; }

    public int CategoryId { get; set; }
}
