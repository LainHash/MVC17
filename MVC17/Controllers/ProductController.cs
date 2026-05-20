using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.DTOs.Products.Create;
using MVC17.DTOs.Products.Update;
using MVC17.Helpers.Constants.Products;
using MVC17.Models;
using MVC17.Services.Interfaces;
using MVC17.ViewModels;

namespace MVC17.Controllers
{
    public class ProductController : Controller
    {
        private readonly Dbmvc05Context _context;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductController(Dbmvc05Context context, IMapper mapper, IProductService productService)
        {
            _context = context;
            _mapper = mapper;
            _productService = productService;
        }

        public async Task<IActionResult> Index(
                string filterString = "", 
                int categoryId = 0, 
                int supplierId = 0, 
                int orderBy = 0, 
                int page = 1, 
                int itemPerPage = 15
            )
        {
            var query = _context.VwProducts;

            var products = await query
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            var vm = _mapper.Map<List<ProductVM>>(products);

            vm = ProductFilterring(vm, filterString, categoryId, supplierId, orderBy);
            vm = ProductPaginating(vm, page, itemPerPage);

            LoadIndexViewBags(categoryId, supplierId, orderBy);


            ViewData["CurrentCategory"] = categoryId;
            ViewData["CurrentSupplier"] = supplierId;
            ViewData["CurrentOrderBy"] = orderBy;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductListPartial", vm);
            }

            return View(vm);
        }

        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> AdminIndex(
                string filterString = "", 
                int categoryId = 0, 
                int supplierId = 0, 
                int orderBy = 0, 
                int page = 1, 
                int itemPerPage = 50
            )
        {
            var query = _context.VwProducts;
            var products = await query.ToListAsync();
            var vm = _mapper.Map<List<ProductVM>>(products);

            vm = ProductFilterring(vm, filterString, categoryId, supplierId, orderBy);
            vm = ProductPaginating(vm, page, itemPerPage);

            LoadIndexViewBags(categoryId, supplierId, orderBy);
            

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AdminProductListPartial", vm);
            }

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
            if (product == null)
            {
                return NotFound("Sản phẩm không tồn tại!");
            }

            //if (product.IsDeleted)
            //{
            //    return NotFound("Sản phẩm này đã bị xóa!");
            //}

            if (product == null)
            {
                return NotFound();
            }

            var vm = _mapper.Map<ProductVM>(product);

            vm.Reviews = await _context.ProductReviews
                .Include(r => r.User)
                    .ThenInclude(u => u.Customer)
                        .ThenInclude(c => c.Pi)
                .Include(r => r.ProductReviewReplies)
                    .ThenInclude(rr => rr.Employee)
                        .ThenInclude(e => e.User)
                .Where(r => r.ProductId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            vm = await GetProductSpec(vm);

            return View(vm);
        }

        [Authorize(Policy = "Manager")]
        public IActionResult Create(int categoryId = 0)
        {
            LoadCreateViewBags(categoryId);

            var dto = new CreateProductDTO()
            {
                CategoryId = categoryId
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(dto);
            }
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Create(CreateProductDTO dto)
        {
            if (!ModelState.IsValid)
            {
                LoadCreateViewBags(dto.CategoryId);
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView(dto);
                }
                return View(dto);
            }

            var result = await _productService.CreateProductAsync(dto);
            return Json(new
            {
                success = true,
                message = result.Message
            });
        }

        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {

            var product = await _context.Products
                .Include(p => p.Image)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Sản phẩm không tồn tại!"
                });
            }

            //if (product.IsDeleted == true)
            //{
            //    return Json(new
            //    {
            //        success = false,
            //        message = "Sản phẩm đã bị xóa!"
            //    });
            //}

            var sku = await _context.ProductSkus
                .Include(ps => ps.Cpu)
                .Include(ps => ps.Gpu)
                .Include(ps => ps.Ram)
                .Include(ps => ps.Storage)
                .Include(ps => ps.Laptop)
                    .ThenInclude(l => l.LaptopComponent)
                .FirstOrDefaultAsync(ps => ps.ProductId == id);
            //if (sku == null)
            //{
            //    return BadRequest();
            //}

            var dto = GetUpdateProduct(product, sku);


            LoadEditViewBags(dto);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(dto);
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Edit(int id, UpdateProductDTO dto)
        {
            if (!ModelState.IsValid)
            {
                LoadEditViewBags(dto);
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView(dto);
                }
                return View(dto);
            }

            var result = await _productService.UpdateProductAsync(id, dto);
            if (!result.Success)
            {
                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }

            return Json(new
            {
                success = true,
                message = result.Message
            });
        }

        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.VwProducts
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Sản phẩm không tồn tại!"
                });
            }

            if (product.IsDeleted)
            {
                return Json(new
                {
                    success = false,
                    message = "Sản phẩm đã bị xóa!"
                });
            }

            var vm = _mapper.Map<ProductVM>(product);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView(vm);
            }
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result.Success)
            {
                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }

            return Json(new
            {
                success = true,
                message = result.Message
            });
        }

        [HttpGet]
        public IActionResult GetSuppliersJson(int categoryId = 0)
        {
            var supplierByCategory = _context.Suppliers
                .Select(s => new
                {
                    s.SupplierId,
                    s.CompanyName,
                    ProductCount = s.Products.Count(p => (categoryId == 0 || p.CategoryId == categoryId) && p.IsDeleted != true)
                })
                .Where(s => categoryId == 0 || s.ProductCount > 0)
                .OrderByDescending(s => s.ProductCount)
                .ToList();

            return Json(supplierByCategory);
        }

        private void GetCategoryViewBags(int categoryId)
        {
            var categories = _context.Categories
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    ProductCount = c.Products.Count(p => p.IsDeleted != true)
                })
                .ToList();

            ViewBag.CategoriesList = categories.Select(c => new DropdownItemVM
            {
                Value = c.CategoryId.ToString(),
                Text = CategoryConstants.CategoryTranslations.ContainsKey(c.CategoryName)
                    ? CategoryConstants.CategoryTranslations[c.CategoryName]
                    : c.CategoryName,
                Count = c.ProductCount
            }).ToList();
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);
        }

        private void GetSupplierViewBags(int categoryId, int supplierId)
        {
            var supplierByCategory = _context.Suppliers
                .Select(s => new
                {
                    s.SupplierId,
                    s.CompanyName,
                    ProductCount = s.Products.Count(p => (categoryId == 0 || p.CategoryId == categoryId) && p.IsDeleted != true)
                })
                .Where(s => categoryId == 0 || s.ProductCount > 0)
                .OrderByDescending(s => s.ProductCount)
                .ToList();

            ViewBag.SuppliersList = supplierByCategory.Select(s => new DropdownItemVM
            {
                Value = s.SupplierId.ToString(),
                Text = s.CompanyName,
                Count = s.ProductCount
            }).ToList();
        }

        private void LoadIndexViewBags(int categoryId, int supplierId, int orderBy)
        {
            GetCategoryViewBags(categoryId);
            GetSupplierViewBags(categoryId, supplierId);
            ViewBag.OrderTypes = new SelectList(ProductConstants.sortDict, "Key", "Value", orderBy);
            ViewData["CurrentCategory"] = categoryId;
            ViewData["CurrentSupplier"] = supplierId;
            ViewData["CurrentOrderBy"] = orderBy;
        }

        private void LoadCreateViewBags(int categoryId)
        {
            GetCategoryViewBags(categoryId);
            ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");
            ViewBag.Cpus = new SelectList(_context.VwCpuSpecs, "CpuId", "CpuName");
            ViewBag.Gpus = new SelectList(_context.VwGpuSpecs, "GpuId", "GpuName");
            ViewBag.Rams = new SelectList(_context.VwRamSpecs, "RamId", "RamName");
            ViewBag.Storages = new SelectList(_context.VwStorageSpecs, "StorageId", "StorageName");

            ViewBag.SelectedCategory = categoryId;
        }

        private void LoadEditViewBags(UpdateProductDTO dto)
        {
            GetCategoryViewBags(dto.CategoryId);
            ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", dto.SupplierId);

            if (dto.Laptop != null)
            {
                ViewBag.Cpus = new SelectList(_context.VwCpuSpecs, "CpuId", "CpuName", dto.Laptop.CpuId);
                ViewBag.Gpus = new SelectList(_context.VwGpuSpecs, "GpuId", "GpuName", dto.Laptop.GpuId);
                ViewBag.Rams = new SelectList(_context.VwRamSpecs, "RamId", "RamName", dto.Laptop.RamId);
                ViewBag.Storages = new SelectList(_context.VwStorageSpecs, "StorageId", "StorageName", dto.Laptop.StorageId);
            }
        }

        private List<ProductVM> ProductFilterring(List<ProductVM> products, 
            string filterString = "", 
            int categoryId = 0, 
            int supplierId = 0, 
            int orderBy = 0)
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
                    p => p.ProductName.ToLower().Contains(filterString.ToLower()) ||
                    p.CategoryName.ToLower().Contains(filterString.ToLower()) ||
                    p.CompanyName.ToLower().Contains(filterString.ToLower())
                ).ToList();

            ViewData["CurrentFilter"] = filterString;

            return products;
        }

        private List<ProductVM> ProductPaginating(List<ProductVM> vm, 
            int page, 
            int itemPerPage)
        {
            int totalPages = (int)Math.Ceiling((double)vm.Count() / itemPerPage);

            vm = vm.Skip((page - 1) * itemPerPage).Take(itemPerPage).ToList();

            ViewData["ActivePage"] = page;
            ViewData["TotalPages"] = totalPages;

            return vm;

        }


        private async Task<ProductVM> GetProductSpec(ProductVM vm)
        {
            switch (vm.CategoryId)
            {
                case CategoryConstants.Laptop:
                    vm.Laptop = await _context.VwLaptopSpecs
                        .FirstOrDefaultAsync(x => x.ProductSkuId == vm.ProductSkuId);
                    break;

                case CategoryConstants.CPU:
                    vm.Cpu = await _context.VwCpuSpecs
                        .FirstOrDefaultAsync(x => x.ProductSkuId == vm.ProductSkuId);
                    break;

                case CategoryConstants.GPU:
                    vm.Gpu = await _context.VwGpuSpecs
                        .FirstOrDefaultAsync(x => x.ProductSkuId == vm.ProductSkuId);
                    break;

                case CategoryConstants.Storage:
                    vm.Storage = await _context.VwStorageSpecs
                        .FirstOrDefaultAsync(x => x.ProductSkuId == vm.ProductSkuId);
                    break;

                case CategoryConstants.RAM:
                    vm.Ram = await _context.VwRamSpecs
                        .FirstOrDefaultAsync(x => x.ProductSkuId == vm.ProductSkuId);
                    break;
            }

            return vm;
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
    }
}
