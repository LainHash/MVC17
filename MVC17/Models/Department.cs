using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public int? ParentDepartmentId { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Department> InverseParentDepartment { get; set; } = new List<Department>();

    public virtual Department? ParentDepartment { get; set; }
}
