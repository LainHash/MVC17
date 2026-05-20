using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwsRevenueByCustomer
{
    public string Username { get; set; } = null!;

    public string? CustomerCode { get; set; }

    public int? Day { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public int? CompletedOrders { get; set; }

    public decimal? TotalRevenue { get; set; }

    public int? TotalQuantity { get; set; }
}
