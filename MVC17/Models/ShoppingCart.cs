using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class ShoppingCart
{
    public int ShoppingCartId { get; set; }

    public int? UserId { get; set; }

    public string? SessionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public decimal Subtotal { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User? User { get; set; }
}
