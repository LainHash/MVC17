using System;
using System.Collections.Generic;

namespace MVC17.ViewModels
{
    public class CheckoutVM
    {
        public bool IsBuyMany { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }

        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;

        public List<CheckoutItemVM> Items { get; set; } = new List<CheckoutItemVM>();
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount => Subtotal + ShippingFee;
    }

    public class CheckoutItemVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
