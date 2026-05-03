using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwInvoiceDetail
{
    public int InvoiceId { get; set; }

    public string ProductName { get; set; } = null!;

    public int Quantity { get; set; }

    public string CategoryName { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public decimal? LineTotal { get; set; }
}
