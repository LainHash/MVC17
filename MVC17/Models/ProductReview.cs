using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class ProductReview
{
    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Rating { get; set; }

    public string? Title { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductReviewReply> ProductReviewReplies { get; set; } = new List<ProductReviewReply>();

    public virtual User User { get; set; } = null!;
}
