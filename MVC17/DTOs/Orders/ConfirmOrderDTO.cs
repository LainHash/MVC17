namespace MVC17.DTOs.Orders
{
    public class ConfirmOrderDTO
    {
        public int InvoiceId { get; set; }
        public Guid InvoiceUuid { get; set; }
        public int CustomerId { get; set; }
        public string CustomerCode { get; set; } = null!;
        public string? EmployeeCode { get; set; }
        public int? EmployeeId { get; set; }
        public DateOnly OrderedDate { get; set; }
        public DateOnly RequiredDate { get; set; }
        public DateOnly? ShippedDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string? Note { get; set; }

        // Confirmation fields
        public int? NewEmployeeId { get; set; }
        public string? NewStatus { get; set; }
        public DateOnly? NewShippedDate { get; set; }
        public string? ConfirmationNote { get; set; }

        // Invoice details
        public List<InvoiceDetailDTO> InvoiceDetails { get; set; } = new List<InvoiceDetailDTO>();

        // Available employees for assignment
        public List<EmployeeOption> AvailableEmployees { get; set; } = new List<EmployeeOption>();

        // Available statuses
        public List<StatusOption> AvailableStatuses { get; set; } = new List<StatusOption>();
    }

    public class InvoiceDetailDTO
    {
        public int InvoiceDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class EmployeeOption
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
    }

    public class StatusOption
    {
        public string Status { get; set; } = null!;
        public string StatusVi { get; set; } = null!;
    }
}
