using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class ProductSku
{
    public int ProductSkuId { get; set; }

    public decimal UnitPrice { get; set; }

    public int UnitsInStock { get; set; }

    public bool Discontinued { get; set; }

    public bool IsDeleted { get; set; }

    public int ProductId { get; set; }

    public virtual Cpu? Cpu { get; set; }

    public virtual Gpu? Gpu { get; set; }

    public virtual Laptop? Laptop { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Ram? Ram { get; set; }

    public virtual Storage? Storage { get; set; }
}
