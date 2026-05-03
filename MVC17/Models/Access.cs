using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Access
{
    public int RoleId { get; set; }

    public int RouteId { get; set; }

    public bool? IsAllowed { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual RouteEntity Route { get; set; } = null!;
}
