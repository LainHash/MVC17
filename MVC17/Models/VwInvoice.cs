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

    public DateOnly? OrderedDate { get; set; }

    public DateOnly? RequiredDate { get; set; }

    public DateOnly? ShippedDate { get; set; }

    public string? Status { get; set; }

    public decimal? Subtotal { get; set; }

    public double? ProductDiscount { get; set; }

    public double? ShippingDiscount { get; set; }

    public decimal? ShippingFee { get; set; }

    public double? TotalAmount { get; set; }

    public string? Note { get; set; }
}
