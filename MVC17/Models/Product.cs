using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public Guid ProductUuid { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public int SupplierId { get; set; }

    public int? ImageId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; } = null!;

    public virtual Image? Image { get; set; }

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual ProductSku? ProductSku { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;
}
