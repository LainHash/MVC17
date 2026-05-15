namespace MVC17.DTOs.Orders
{
    public class InvoiceDetailDTO
    {
        public int InvoiceDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public decimal? UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal? LineTotal { get; set; }
    }
}
