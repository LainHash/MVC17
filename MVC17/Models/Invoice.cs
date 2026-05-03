using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public Guid InvoiceUuid { get; set; }

    public int CustomerId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly RequiredDate { get; set; }

    public DateOnly? ShippedDate { get; set; }

    public string Status { get; set; } = null!;

    public decimal Subtotal { get; set; }

    public double? ProductDiscount { get; set; }

    public decimal ShippingFee { get; set; }

    public double? ShippingDiscount { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
}
