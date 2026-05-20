using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwTopRatingProduct
{
    public Guid ProductUuid { get; set; }

    public string ProductName { get; set; } = null!;

    public int ProductId { get; set; }

    public string? ImageUrl { get; set; }

    public int? TotalRating { get; set; }

    public int? AvgRating { get; set; }

    public int? TotalComment { get; set; }
}
