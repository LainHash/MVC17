using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.DTOs.Products.Create;
using MVC17.DTOs.Products.Update;
using MVC17.Helpers.Constants.Products;
using MVC17.Models;
using MVC17.ViewModels;

namespace MVC17.Controllers
{
    public class ProductController : Controller
    {
        private readonly Dbmvc05Context _context;
        private readonly IMapper _mapper;

        public ProductController(Dbmvc05Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string filterString = "", int categoryId = 0, int supplierId = 0, int orderBy = 0, int page = 1, int itemPerPage = 15)
        {
            var query = _context.VwProducts;
            int totalPages = (int)Math.Ceiling((double)await query.CountAsync() / itemPerPage);
            var products = await query
                .ToListAsync();

            var vm = _mapper.Map<List<ProductVM>>(products);

            vm = ProductFilterring(vm, filterString, categoryId, supplierId, orderBy);

            vm = vm.Skip((page - 1) * itemPerPage).Take(itemPerPage).ToList();

            var supplierByCategory = _context.Suppliers.Where(s => s.Products.Any(p => p.CategoryId == categoryId))
                .ToList();

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);
            ViewBag.Suppliers = new SelectList(supplierByCategory, "SupplierId", "CompanyName", supplierId);
            ViewBag.OrderTypes = new SelectList(ProductConstants.sortDict, "Key", "Value", orderBy);

            ViewData["ActivePage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentFilter"] = filterString;

            return View(vm);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.VwProducts
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if(product.IsDeleted != null && product.IsDeleted == true)
            {
                return NotFound("Sản phẩm này đã bị xóa!");
            }

            if (product == null)
            {
                return NotFound();
            }

            var vm = _mapper.Map<ProductVM>(product);

            vm = await GetProductSpec(vm);

            return View(vm);
        }

        public IActionResult Create(int categoryId = 0)
        {
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);
            ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");
            ViewBag.Cpus = new SelectList(_context.VwCpuSpecs, "CpuId", "CpuName");
            ViewBag.Gpus = new SelectList(_context.VwGpuSpecs, "GpuId", "GpuName");
            ViewBag.Rams = new SelectList(_context.VwRamSpecs, "RamId", "RamName");
            ViewBag.Storages = new SelectList(_context.VwStorageSpecs, "StorageId", "StorageName");

            ViewBag.SelectedCategory = categoryId;

            var dto = new CreateProductDTO()
            {
                CategoryId = categoryId
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            var product = PostCreateProduct(dto);

            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Image)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if(product.IsDeleted == true)
            {
                return NotFound("Sản phẩm này đã bị xóa!");
            }

            var sku = await _context.ProductSkus
                .Include(ps => ps.Cpu)
                .Include(ps => ps.Gpu)
                .Include(ps => ps.Ram)
                .Include(ps => ps.Storage)
                .Include(ps => ps.Laptop)
                    .ThenInclude(l => l.LaptopComponent)
                .FirstOrDefaultAsync(ps => ps.ProductId == id);

            var dto = GetUpdateProduct(product, sku);

            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", dto.CategoryId);
            ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", dto.SupplierId);
            if (dto.Laptop != null)
            {
                ViewBag.Cpus = new SelectList(_context.VwCpuSpecs, "CpuId", "CpuName", dto.Laptop?.CpuId);
                ViewBag.Gpus = new SelectList(_context.VwGpuSpecs, "GpuId", "GpuName", dto.Laptop?.GpuId);
                ViewBag.Rams = new SelectList(_context.VwRamSpecs, "RamId", "RamName", dto.Laptop?.RamId);
                ViewBag.Storages = new SelectList(_context.VwStorageSpecs, "StorageId", "StorageName", dto.Laptop?.StorageId);
            }


            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDTO dto)
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

            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);

            }

            try
            {
                product = PostUpdateProduct(dto, product, sku);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.VwProducts
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product.IsDeleted != null && product.IsDeleted == true)
            {
                return NotFound("Sản phẩm này đã bị xóa!");
            }

            if (product == null)
            {
                return NotFound();
            }

            var vm = _mapper.Map<ProductVM>(product);
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                _context.Products.Update(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        private List<ProductVM> ProductFilterring(List<ProductVM> products, string filterString = "", int categoryId = 0, int supplierId = 0, int orderBy = 0)
        {
            if (categoryId > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();
            }

            if (supplierId > 0)
            {
                products = products.Where(p => p.SupplierId == supplierId).ToList();
            }

            switch (orderBy)
            {
                case 0:
                    products = products.OrderByDescending(p => p.ProductId).ToList();
                    break;
                case 1:
                    products = products.OrderBy(p => p.ProductId).ToList();
                    break;
                case 2:
                    products = products.OrderBy(p => p.UnitPrice).ToList();
                    break;
                case 3:
                    products = products.OrderByDescending(p => p.UnitPrice).ToList();
                    break;
            }

            products = products
                .Where(
                    p => p.ProductName.Contains(filterString) ||
                    p.CategoryName.Contains(filterString) ||
                    p.CompanyName.Contains(filterString)
                ).ToList();

            return products;
        }

        private async Task<ProductVM> GetProductSpec(ProductVM vm)
        {
            switch (vm.CategoryId)
            {
                case CategoryType.Laptop:
                    vm.Laptop = await _context.VwLaptopSpecs
                        .FirstOrDefaultAsync(x => x.ProductSkuId == vm.ProductSkuId);
                    break;

                case CategoryType.CPU:
                    vm.Cpu = await _context.VwCpuSpecs
                        .FirstOrDefaultAsync(x => x.ProductSkuId == vm.ProductSkuId);
                    break;

                case CategoryType.GPU:
                    vm.Gpu = await _context.VwGpuSpecs
                        .FirstOrDefaultAsync(x => x.ProductSkuId == vm.ProductSkuId);
                    break;

                case CategoryType.Storage:
                    vm.Storage = await _context.VwStorageSpecs
                        .FirstOrDefaultAsync(x => x.ProductSkuId == vm.ProductSkuId);
                    break;

                case CategoryType.RAM:
                    vm.Ram = await _context.VwRamSpecs
                        .FirstOrDefaultAsync(x => x.ProductSkuId == vm.ProductSkuId);
                    break;
            }

            return vm;
        }

        private Product PostCreateProduct(CreateProductDTO dto)
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
            return product;
        }

        private UpdateProductDTO GetUpdateProduct(Product product, ProductSku sku)
        {
            var dto = new UpdateProductDTO()
            {
                ProductName = product.ProductName,
                Description = product.Description,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId,
                ImageUrl = product.Image?.ImageUrl,
                UnitPrice = product.ProductSku?.UnitPrice ?? 0,
                UnitsInStock = product.ProductSku?.UnitsInStock ?? 0,
                Cpu = sku.Cpu != null ? _mapper.Map<UpdateCpuDTO>(sku.Cpu) : null,
                Gpu = sku.Gpu != null ? _mapper.Map<UpdateGpuDTO>(sku.Gpu) : null,
                Ram = sku.Ram != null ? _mapper.Map<UpdateRamDTO>(sku.Ram) : null,
                Storage = sku.Storage != null ? _mapper.Map<UpdateStorageDTO>(sku.Storage) : null,
                Laptop = sku.Laptop != null ? new UpdateLaptopDTO()
                {
                    LaptopType = sku.Laptop.LaptopType,
                    Os = sku.Laptop.Os,
                    ScreenResolution = sku.Laptop.ScreenResolution,
                    Weight = sku.Laptop.Weight,
                    Length = sku.Laptop.Length,
                    CpuId = sku.Laptop.LaptopComponent.CpuId,
                    GpuId = sku.Laptop.LaptopComponent.GpuId,
                    RamId = sku.Laptop.LaptopComponent.RamId,
                    StorageId = sku.Laptop.LaptopComponent.StorageId
                } : null
            };
            return dto;
        }

        private Product PostUpdateProduct(UpdateProductDTO dto, Product product, ProductSku sku)
        {
            product.ProductName = dto.ProductName;
            product.Description = dto.Description;
            product.SupplierId = dto.SupplierId;
            product.Image.ImageUrl = dto.ImageUrl ?? "~img/NotFoundImage.png";

            sku.UnitPrice = dto.UnitPrice;
            sku.UnitsInStock = dto.UnitsInStock;
            sku.Discontinued = dto.Discontinued;

            sku.Cpu = dto.Cpu != null ? _mapper.Map(dto.Cpu, sku.Cpu) : null;
            sku.Gpu = dto.Gpu != null ? _mapper.Map(dto.Gpu, sku.Gpu) : null;
            sku.Ram = dto.Ram != null ? _mapper.Map(dto.Ram, sku.Ram) : null;
            sku.Storage = dto.Storage != null ? _mapper.Map(dto.Storage, sku.Storage) : null;

            if (dto.Laptop != null)
            {
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
            product.ProductSku = sku;

            return product;
        }

    }
}
