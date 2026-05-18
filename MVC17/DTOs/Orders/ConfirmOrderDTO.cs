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
        public DateTime OrderedDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string? Note { get; set; }

        public int? NewEmployeeId { get; set; }
        public string? NewStatus { get; set; }
        public DateTime? NewShippedDate { get; set; }
        public string? ConfirmationNote { get; set; }

        public List<InvoiceDetailDTO> InvoiceDetails { get; set; } = new List<InvoiceDetailDTO>();

        public List<EmployeeOption> AvailableEmployees { get; set; } = new List<EmployeeOption>();

        public List<StatusOption> AvailableStatuses { get; set; } = new List<StatusOption>();
    }
}
