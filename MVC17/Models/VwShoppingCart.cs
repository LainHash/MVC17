using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwShoppingCart
{
    public int? UserId { get; set; }

    public string? SessionId { get; set; }

    public int ShoppingCartId { get; set; }

    public decimal? Subtotal { get; set; }
}
