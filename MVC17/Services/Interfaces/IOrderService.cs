using MVC17.DTOs.Orders;
using MVC17.Models;

namespace MVC17.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<VwInvoice>> GetAllInvoicesAsync();
        Task<VwInvoice?> GetVwInvoiceByIdAsync(int id, int? userId = null);
        Task<Invoice?> GetInvoiceByIdAsync(int id, int? userId = null);
        Task<IEnumerable<VwInvoiceDetail>> GetInvoiceDetailsAsync(int id);
        Task<CheckoutDTO?> PrepareCheckoutAsync(int userId, int? productId, int quantity, bool isBuyMany);
        Task<(bool Success, string Message, int? InvoiceId)> ProcessCheckoutAsync(int userId, CheckoutDTO model);
        Task<ConfirmOrderDTO?> PrepareConfirmOrderAsync(int invoiceId, int employeeUserId);
        Task<(bool Success, string Message)> ConfirmOrderAsync(ConfirmOrderDTO model);
        List<Discount> GetDiscount(string type);
    }
}
