using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string DiscountCode { get; set; } = null!;

    public double DiscountAmount { get; set; }

    public DateOnly ExpiredDate { get; set; }

    public int Amount { get; set; }

    public string Type { get; set; } = null!;
}
