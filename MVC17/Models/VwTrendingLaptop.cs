using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwTrendingLaptop
{
    public int LaptopId { get; set; }

    public string LaptopType { get; set; } = null!;

    public string Os { get; set; } = null!;

    public string ScreenResolution { get; set; } = null!;

    public float Length { get; set; }

    public float Weight { get; set; }

    public string GpuName { get; set; } = null!;

    public string CpuName { get; set; } = null!;

    public string RamName { get; set; } = null!;

    public string StorageName { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int UnitsInStock { get; set; }

    public string ProductName { get; set; } = null!;

    public int? CreatedInvoices { get; set; }

    public string? Description { get; set; }

    public string CompanyName { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public int ProductId { get; set; }
}
