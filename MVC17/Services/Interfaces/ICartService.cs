using MVC17.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVC17.Services.Interfaces
{
    public interface ICartService
    {
        Task<List<VwShoppingCart>> GetAllAsync(int? userId, string? sessionId);
        Task<(bool Success, string Message)> AddAsync(int id, int quantity, int? userId, string sessionId);
        Task<(bool Success, string Message)> UpdateItemQuantityAsync(int cartItemId, int quantity);
        Task<(bool Success, string Message)> RemoveItemAsync(int cartItemId);
        Task<int> CleanupExpiredGuestCartsAsync();
    }
}
