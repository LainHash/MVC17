using MVC17.Models;

namespace MVC17.ViewModels
{
    public class OrderHistoryVM
    {
        public int InvoiceId { get; set; }
        public Guid InvoiceUuid { get; set; }
        public int CustomerId { get; set; }
        public DateOnly OrderedDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
    }
}
