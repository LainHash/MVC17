using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwShoppingCart
{
    public int? UserId { get; set; }

    public string? SessionId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedDate { get; set; }

    public decimal? LineTotal { get; set; }

    public int CartItemId { get; set; }
}
