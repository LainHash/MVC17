using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? CustomerCode { get; set; }

    public int Piid { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual PersonalInformation Pi { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
