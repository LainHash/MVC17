using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwsRevenueByEmployee
{
    public string EmployeeCode { get; set; } = null!;

    public string Username { get; set; } = null!;

    public int? Day { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public int? CompletedOrders { get; set; }

    public int? TotalQuantity { get; set; }

    public decimal? TotalRevenue { get; set; }
}
