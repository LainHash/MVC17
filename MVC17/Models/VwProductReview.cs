using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwProductReview
{
    public Guid UserUuid { get; set; }

    public string Username { get; set; } = null!;

    public Guid ProductUuid { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public string CategoryName { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Rating { get; set; }

    public string? Title { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? ImageUrl { get; set; }

    public int IsPurchased { get; set; }
}
