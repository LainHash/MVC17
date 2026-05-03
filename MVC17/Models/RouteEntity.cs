using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class RouteEntity
{
    public int RouteId { get; set; }

    public string RouteCode { get; set; } = null!;

    public string Path { get; set; } = null!;

    public string Endpoint { get; set; } = null!;

    public string Module { get; set; } = null!;

    public string Method { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Access> Accesses { get; set; } = new List<Access>();
}
