using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.Models;
using MVC17.ViewModels;
using System.Security.Claims;

namespace MVC17.Controllers
{
    public class OrderController : Controller
    {
        private readonly Dbmvc05Context _context;
        private readonly IMapper _mapper;

        public OrderController(Dbmvc05Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ─── Public Actions ────────────────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            var invoices = await _context.VwInvoices
                .OrderByDescending(iv => iv.OrderDate)
                .ToListAsync();
            return View(invoices);
        }

        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.VwInvoices
                .FirstOrDefaultAsync(iv => iv.InvoiceId == id);
            if (invoice == null)
                return NotFound();

            ViewBag.InvoiceDetails = await _context.VwInvoiceDetails
                .Where(ivd => ivd.InvoiceId == id)
                .ToListAsync();

            return View(invoice);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Checkout(int? productId, int quantity = 1, bool isBuyMany = false)
        {
            if (quantity <= 0) quantity = 1;

            if (!TryGetCurrentUserId(out int userId))
                return RedirectToAction("Login", "Account");

            var customer = await GetCustomerWithPiAsync(userId);
            if (customer == null)
                return BadRequest("Không tìm thấy thông tin khách hàng.");

            var model = BuildCheckoutModelBase(customer, isBuyMany);

            if (!isBuyMany)
            {
                if (productId == null) return BadRequest("Thiếu sản phẩm.");
                var error = await FillSingleProductAsync(model, productId.Value, quantity);
                if (error != null) return error;
            }
            else
            {
                var error = await FillCartItemsAsync(model, userId);
                if (error != null) return error;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            if (!TryGetCurrentUserId(out int userId))
                return RedirectToAction("Login", "Account");

            var customer = await GetCustomerWithUserAsync(userId);
            if (customer == null)
                return BadRequest("Không tìm thấy thông tin khách hàng.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                IActionResult? earlyReturn = model.IsBuyMany
                    ? await ProcessCartCheckoutAsync(customer, userId)
                    : await ProcessSingleProductCheckoutAsync(customer, model);

                if (earlyReturn != null)
                {
                    await transaction.RollbackAsync();
                    return earlyReturn;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Thanh toán thành công!";
                return RedirectToAction("CheckoutResult");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IActionResult> Success(int id)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(iv => iv.InvoiceId == id);
            return View(invoice);
        }

        [HttpGet]
        public IActionResult CheckoutResult()
        {
            return View();
        }

        // ─── Private Helpers ───────────────────────────────────────────────────

        private bool TryGetCurrentUserId(out int userId)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out userId);
        }

        private Task<Customer?> GetCustomerWithPiAsync(int userId) =>
            _context.Customers
                .Include(c => c.Pi)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsDeleted != true);

        private Task<Customer?> GetCustomerWithUserAsync(int userId) =>
            _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsDeleted != true);

        private static CheckoutVM BuildCheckoutModelBase(Customer customer, bool isBuyMany) => new()
        {
            IsBuyMany = isBuyMany,
            FullName = customer.Pi != null ? $"{customer.Pi.FirstName} {customer.Pi.LastName}" : "",
            Phone = customer.Pi?.Phone ?? "",
            Email = customer.Pi?.Email ?? "",
            Address = customer.Pi?.Address ?? "",
            ShippingFee = 0
        };

        private async Task<IActionResult?> FillSingleProductAsync(CheckoutVM model, int productId, int quantity)
        {
            var product = await _context.Products
                .Include(p => p.ProductSku)
                .Include(p => p.Image)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null) return NotFound("Sản phẩm không tồn tại.");

            var unitPrice = product.ProductSku.UnitPrice;
            var lineTotal = unitPrice * quantity;

            model.ProductId = productId;
            model.Quantity = quantity;
            model.Subtotal = lineTotal;
            model.Items =
            [
                new CheckoutItemVM
                {
                    ProductId   = product.ProductId,
                    ProductName = product.ProductName,
                    ImageUrl    = product.Image.ImageUrl,
                    Quantity    = quantity,
                    UnitPrice   = unitPrice,
                    LineTotal   = lineTotal
                }
            ];
            return null;
        }

        private async Task<IActionResult?> FillCartItemsAsync(CheckoutVM model, int userId)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Image)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return BadRequest("Giỏ hàng trống.");

            model.Items = cart.CartItems.Select(item => new CheckoutItemVM
            {
                ProductId = item.ProductId,
                ProductName = item.Product.ProductName,
                ImageUrl = item.Product.Image.ImageUrl,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.UnitPrice * item.Quantity
            }).ToList();

            model.Subtotal = cart.CartItems.Sum(x => x.LineTotal);
            return null;
        }

        private async Task<IActionResult?> ProcessSingleProductCheckoutAsync(Customer customer, CheckoutVM model)
        {
            var productId = model.ProductId ?? 0;
            var quantity = model.Quantity ?? 1;

            var product = await _context.Products
                .Include(p => p.ProductSku)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null) return NotFound("Sản phẩm không tồn tại.");

            var unitPrice = product.ProductSku.UnitPrice;
            var lineTotal = unitPrice * quantity;

            if (customer.User.Balance < lineTotal)
            {
                TempData["Error"] = "Số dư không đủ để thanh toán.";
                return RedirectToAction("CheckoutResult");
            }

            var invoice = CreateInvoiceBase(customer.CustomerId, lineTotal);
            customer.User.Balance -= (int)lineTotal;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            _context.InvoiceDetails.Add(new InvoiceDetail
            {
                InvoiceId = invoice.InvoiceId,
                ProductId = product.ProductId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                LineTotal = lineTotal
            });
            return null;
        }

        private async Task<IActionResult?> ProcessCartCheckoutAsync(Customer customer, int userId)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return BadRequest("Giỏ hàng trống.");

            var subtotal = cart.CartItems.Sum(x => x.LineTotal);

            if (customer.User.Balance < subtotal)
            {
                TempData["Error"] = "Số dư không đủ để thanh toán.";
                return RedirectToAction("CheckoutResult");
            }

            var invoice = CreateInvoiceBase(customer.CustomerId, subtotal);
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            _context.InvoiceDetails.AddRange(cart.CartItems.Select(item => new InvoiceDetail
            {
                InvoiceId = invoice.InvoiceId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.LineTotal
            }));

            _context.CartItems.RemoveRange(cart.CartItems);
            cart.Subtotal = 0;
            cart.UpdatedAt = DateTime.Now;
            return null;
        }

        private static Invoice CreateInvoiceBase(int customerId, decimal amount) => new()
        {
            InvoiceUuid = Guid.NewGuid(),
            CustomerId = customerId,
            OrderDate = DateOnly.FromDateTime(DateTime.Now),
            RequiredDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
            Status = "Pending",
            Subtotal = amount,
            ProductDiscount = 0,
            ShippingFee = 0,
            ShippingDiscount = 0,
            TotalAmount = amount,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
    }
}
