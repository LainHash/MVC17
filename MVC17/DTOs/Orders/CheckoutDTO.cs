using MVC17.ViewModels;

namespace MVC17.DTOs.Orders
{
    public class CheckoutDTO
    {
        public int InvoiceId { get; set; }
        public bool IsBuyMany { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }

        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;

        public List<CheckoutItemDTO> Items { get; set; } = new List<CheckoutItemDTO>();
        public decimal Subtotal { get; set; }
        public float ProductDiscount { get; set; }
        public decimal ShippingFee { get; set; }
        public float ShippingDiscount { get; set; }
        public decimal TotalAmount => Subtotal * (1 - (decimal)ProductDiscount) + ShippingFee * (1 - (decimal)ShippingDiscount);

        public string? Note { get; set; }
    }
}
