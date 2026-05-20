using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwsCompletedOrder
{
    public int? Day { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public int? CompletedOrders { get; set; }

    public decimal? TotalRevenue { get; set; }

    public int? TotalQuantity { get; set; }

    public int CategoryId { get; set; }

    public int SupplierId { get; set; }

    public int ProductId { get; set; }

    public int CustomerId { get; set; }

    public int? EmployeeId { get; set; }
}
