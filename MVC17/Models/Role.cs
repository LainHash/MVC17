using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Access> Accesses { get; set; } = new List<Access>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
