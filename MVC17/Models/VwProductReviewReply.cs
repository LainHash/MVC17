using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwProductReviewReply
{
    public int ReviewId { get; set; }

    public string? ReplyContent { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Username { get; set; } = null!;

    public Guid UserUuid { get; set; }
}
