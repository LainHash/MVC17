using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Position
{
    public int PositionId { get; set; }

    public string PositionName { get; set; } = null!;

    public int? Level { get; set; }

    public int? BaseSalary { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
