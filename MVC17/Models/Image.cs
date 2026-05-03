using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
