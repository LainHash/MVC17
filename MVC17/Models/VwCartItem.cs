using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwCartItem
{
    public string ProductName { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public int ShoppingCartId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? LineTotal { get; set; }
}
