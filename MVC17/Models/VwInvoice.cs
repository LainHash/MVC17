using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwInvoice
{
    public int CustomerId { get; set; }

    public string? CustomerCode { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public int InvoiceId { get; set; }

    public DateTime OrderedDate { get; set; }

    public DateTime RequiredDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public string Status { get; set; } = null!;

    public double? ProductDiscount { get; set; }

    public double? ShippingDiscount { get; set; }

    public decimal ShippingFee { get; set; }

    public string? Note { get; set; }

    public Guid InvoiceUuid { get; set; }

    public decimal Subtotal { get; set; }

    public decimal TotalAmount { get; set; }
}
