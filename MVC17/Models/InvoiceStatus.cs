using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class InvoiceStatus
{
    public string Status { get; set; } = null!;

    public string? NameVi { get; set; }
}
