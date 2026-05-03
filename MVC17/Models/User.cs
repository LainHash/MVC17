using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class User
{
    public int UserId { get; set; }

    public Guid? UserUuid { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int Balance { get; set; }

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
