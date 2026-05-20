using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.DTOs.Products.Create;
using MVC17.DTOs.Products.Update;
using MVC17.Models;
using MVC17.Services.Interfaces;

namespace MVC17.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly Dbmvc05Context _context;
        private readonly IMapper _mapper;

        public ProductService(Dbmvc05Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VwProduct>> GetAllAsync()
        {
            var products = await _context.VwProducts
                .ToListAsync();
            return products;
        }

        public async Task<VwProduct?> GetByIdAsync(int id)
        {
            var product = await _context.VwProducts
                .FirstOrDefaultAsync(p => p.ProductId == id);

            return product;
        }

        public async Task<(bool Success, string Message)> CreateProductAsync(CreateProductDTO dto)
        {
            var product = new Product()
            {
                ProductName = dto.ProductName,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId,
                Description = dto.Description,
                Image = new Image()
                {
                    ImageUrl = dto.ImageUrl ?? null,
                },
                IsDeleted = dto.IsDeleted,
                ProductSku = new ProductSku()
                {
                    UnitPrice = dto.UnitPrice,
                    UnitsInStock = dto.UnitsInStock,
                    Cpu = dto.Cpu != null ? _mapper.Map<Cpu>(dto.Cpu) : null,
                    Gpu = dto.Gpu != null ? _mapper.Map<Gpu>(dto.Gpu) : null,
                    Ram = dto.Ram != null ? _mapper.Map<Ram>(dto.Ram) : null,
                    Storage = dto.Storage != null ? _mapper.Map<Storage>(dto.Storage) : null
                }
            };

            if (dto.Laptop != null)
            {
                product.ProductSku.Laptop = new Laptop()
                {
                    LaptopType = dto.Laptop.LaptopType,
                    Os = dto.Laptop.Os,
                    ScreenResolution = dto.Laptop.ScreenResolution,
                    Weight = dto.Laptop.Weight,
                    Length = dto.Laptop.Length,
                    LaptopComponent = new LaptopComponent()
                    {
                        CpuId = dto.Laptop.CpuId,
                        GpuId = dto.Laptop.GpuId,
                        RamId = dto.Laptop.RamId,
                        StorageId = dto.Laptop.StorageId
                    }
                };
            }

            _context.Add(product);
            await _context.SaveChangesAsync();
            return (true, "Thêm sản phẩm thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateProductAsync(int id, UpdateProductDTO dto)
        {
            var product = await _context.Products
                .Include(p => p.Image)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            var sku = await _context.ProductSkus
                .Include(ps => ps.Cpu)
                .Include(ps => ps.Gpu)
                .Include(ps => ps.Ram)
                .Include(ps => ps.Storage)
                .Include(ps => ps.Laptop)
                    .ThenInclude(l => l.LaptopComponent)
                .FirstOrDefaultAsync(ps => ps.ProductId == id);

            if (product == null)
            {
                return (false, "Sản phẩm không tồn tại");
            }

            product.ProductName = dto.ProductName;
            product.Description = dto.Description;
            product.SupplierId = dto.SupplierId;
            if (dto.ImageUrl != null)
            {
                product.Image = new Image { ImageUrl = dto.ImageUrl };
            }

            if (sku != null)
            {
                sku.UnitPrice = dto.UnitPrice;
                sku.UnitsInStock = dto.UnitsInStock;
                sku.Discontinued = dto.Discontinued;

                sku.Cpu = dto.Cpu != null ? _mapper.Map(dto.Cpu, sku.Cpu) : null;
                sku.Gpu = dto.Gpu != null ? _mapper.Map(dto.Gpu, sku.Gpu) : null;
                sku.Ram = dto.Ram != null ? _mapper.Map(dto.Ram, sku.Ram) : null;
                sku.Storage = dto.Storage != null ? _mapper.Map(dto.Storage, sku.Storage) : null;

                if (dto.Laptop != null)
                {
                    if (sku.Laptop == null)
                    {
                        sku.Laptop = new Laptop { LaptopComponent = new LaptopComponent() };
                    }
                    else if (sku.Laptop.LaptopComponent == null)
                    {
                        sku.Laptop.LaptopComponent = new LaptopComponent();
                    }

                    sku.Laptop.LaptopType = dto.Laptop.LaptopType;
                    sku.Laptop.Os = dto.Laptop.Os;
                    sku.Laptop.ScreenResolution = dto.Laptop.ScreenResolution;
                    sku.Laptop.Weight = dto.Laptop.Weight;
                    sku.Laptop.Length = dto.Laptop.Length;
                    sku.Laptop.LaptopComponent.CpuId = dto.Laptop.CpuId;
                    sku.Laptop.LaptopComponent.GpuId = dto.Laptop.GpuId;
                    sku.Laptop.LaptopComponent.RamId = dto.Laptop.RamId;
                    sku.Laptop.LaptopComponent.StorageId = dto.Laptop.StorageId;
                }
                else
                {
                    sku.Laptop = null;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return (true, "Sửa sản phẩm thành công.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.ProductId == id))
                {
                    return (false, "Sản phẩm không tồn tại.");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<(bool Success, string Message)> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return (true, "Xóa sản phẩm thành công.");
            }
            return (false, "Xóa sản phẩm thất bại.");
        }
    }
}
