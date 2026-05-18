using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class ProductReviewReply
{
    public int ReplyId { get; set; }

    public int ReviewId { get; set; }

    public int EmployeeId { get; set; }

    public string? ReplyContent { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ProductReview Review { get; set; } = null!;
}
