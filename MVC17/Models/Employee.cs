using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public int UserId { get; set; }

    public int? Piid { get; set; }

    public string CompanyEmail { get; set; } = null!;

    public DateOnly HiredDate { get; set; }

    public string? Status { get; set; }

    public int? PositionId { get; set; }

    public int? DepartmentId { get; set; }

    public int? ManagerId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Employee> InverseManager { get; set; } = new List<Employee>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Employee? Manager { get; set; }

    public virtual PersonalInformation? Pi { get; set; }

    public virtual Position? Position { get; set; }

    public virtual User User { get; set; } = null!;
}
