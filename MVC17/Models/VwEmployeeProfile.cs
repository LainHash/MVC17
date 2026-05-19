using System;
using System.Collections.Generic;

namespace MVC17.Models;

public partial class VwEmployeeProfile
{
    public string EmployeeCode { get; set; } = null!;

    public int UserId { get; set; }

    public Guid UserUuid { get; set; }

    public string Username { get; set; } = null!;

    public decimal Balance { get; set; }

    public bool IsActive { get; set; }

    public string RoleName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public bool Gender { get; set; }

    public DateOnly Dob { get; set; }

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string CitizenIdentityCard { get; set; } = null!;

    public string? AvatarImage { get; set; }

    public DateOnly HiredDate { get; set; }

    public string? Status { get; set; }

    public string PositionName { get; set; } = null!;

    public string DepartmentName { get; set; } = null!;

    public string Email { get; set; } = null!;
}
