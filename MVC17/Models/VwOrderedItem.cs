using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwOrderedItem
{
    public int CustomerId { get; set; }

    public int InvoiceId { get; set; }

    public Guid InvoiceUuid { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal { get; set; }

    public string CategoryName { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public string? ImageUrl { get; set; }
}
