using MVC17.DTOs.Products.Create;
using MVC17.DTOs.Products.Update;
using MVC17.Models;

namespace MVC17.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<VwProduct>> GetAllAsync();
        Task<VwProduct?> GetByIdAsync(int id);
        Task<bool> CreateProductAsync(CreateProductDTO dto);
        Task<bool> UpdateProductAsync(int id, UpdateProductDTO dto);
        Task<bool> DeleteProductAsync(int id);
    }
}
