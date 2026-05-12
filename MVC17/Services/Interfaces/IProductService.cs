using MVC17.DTOs.Products.Create;
using MVC17.DTOs.Products.Update;
using MVC17.Models;

namespace MVC17.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<VwProduct>> GetAllAsync();
        Task<VwProduct?> GetByIdAsync(int id);
        Task<(bool Success, string Message)> CreateProductAsync(CreateProductDTO dto);
        Task<(bool Success, string Message)> UpdateProductAsync(int id, UpdateProductDTO dto);
        Task<(bool Success, string Message)> DeleteProductAsync(int id);
    }
}
