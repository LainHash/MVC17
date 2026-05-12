using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MVC17.Data;
using MVC17.DTOs.Orders;
using MVC17.Helpers.Constants.Orders;
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


        public async Task<IActionResult> Index()
        {
            var invoices = await _context.VwInvoices
                .OrderByDescending(iv => iv.OrderedDate)
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
            if (quantity <= 0)
            {
                quantity = 1;
            }

            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await GetCustomerWithPiAsync(userId);
            if (customer == null)
            {
                return BadRequest("Không tìm thấy thông tin khách hàng.");
            }

            var model = new CheckoutDTO()
            {
                IsBuyMany = isBuyMany,
                FullName = customer.Pi != null ? $"{customer.Pi.FirstName} {customer.Pi.LastName}" : "",
                Phone = customer.Pi?.Phone ?? "",
                Email = customer.Pi?.Email ?? "",
                Country = customer.Pi?.Country ?? "",
                City = customer.Pi?.City ?? "",
                Address = customer.Pi?.Address ?? "",
                ShippingFee = Distances.CalculateShippingFee(customer.Pi.City)
            };

            if (!isBuyMany)
            {
                if (productId == null)
                {
                    return BadRequest("Thiếu sản phẩm.");
                }
                var error = await FillSingleProductAsync(model, productId.Value, quantity);
                if (error != null)
                {
                    return error;
                }
            }
            else
            {
                var error = await FillCartItemsAsync(model, userId);
                if (error != null)
                {
                    return error;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Checkout(CheckoutDTO model)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await GetCustomerWithUserAsync(userId);
            if (customer == null)
            {
                return BadRequest("Không tìm thấy thông tin khách hàng.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var (earlyReturn, invoiceId) = model.IsBuyMany
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
                return RedirectToAction("Success", new { id = invoiceId });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            
        }

        public async Task<IActionResult> Success(int id)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(iv => iv.InvoiceId == id);
            return View(invoice);
        }

        [HttpGet]
        public IActionResult CheckoutResult()
        {
            return View();
        }


        private bool TryGetCurrentUserId(out int userId)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out userId);
        }

        private Task<Customer?> GetCustomerWithPiAsync(int userId)
        {
            return _context.Customers
                .Include(c => c.Pi)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsDeleted != true);
        }

        private Task<Customer?> GetCustomerWithUserAsync(int userId)
        {
            return _context.Customers
                .Include(c => c.User)
                .Include(c => c.Pi)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsDeleted != true);
        }

        private async Task<IActionResult?> FillSingleProductAsync(CheckoutDTO model, int productId, int quantity)
        {
            var product = await _context.Products
                .Include(p => p.ProductSku)
                .Include(p => p.Image)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

            var unitPrice = product.ProductSku.UnitPrice;
            var lineTotal = unitPrice * quantity;

            model.ProductId = productId;
            model.Quantity = quantity;
            model.Subtotal = lineTotal;
            model.Items =
            [
                new CheckoutItemDTO
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

        private async Task<IActionResult?> FillCartItemsAsync(CheckoutDTO model, int userId)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Image)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                ViewData["Error"] = "Giỏ hàng trống!";
                return View(model);
            }

            model.Items = cart.CartItems.Select(item => new CheckoutItemDTO
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

        private static Invoice CreateInvoiceBase(Customer customer, decimal subtotal, decimal shippingFee)
        {
            return new Invoice()
            {
                InvoiceUuid = Guid.NewGuid(),
                CustomerId = customer.CustomerId,
                OrderedDate = DateOnly.FromDateTime(DateTime.Now),
                RequiredDate = DateOnly.FromDateTime(DateTime.Now.AddDays(Distances.CalculateShippingDays(customer.Pi.City))),
                Status = "Pending",
                Subtotal = subtotal,
                ProductDiscount = 0,
                ShippingFee = shippingFee,
                ShippingDiscount = 0,
                TotalAmount = subtotal + shippingFee,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        private async Task<(IActionResult? ErrorResult, int? InvoiceId)> ProcessSingleProductCheckoutAsync(Customer customer, CheckoutDTO model)
        {
            var productId = model.ProductId ?? 0;
            var quantity = model.Quantity ?? 1;

            var product = await _context.Products
                .Include(p => p.ProductSku)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return (NotFound("Sản phẩm không tồn tại."), null);
            }

            var unitPrice = product.ProductSku.UnitPrice;
            var lineTotal = unitPrice * quantity;
            var shippingFee = Distances.CalculateShippingFee(customer.Pi.City);

            var invoice = CreateInvoiceBase(customer, lineTotal, shippingFee);


            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();


            if (customer.User.Balance < invoice.TotalAmount)
            {
                TempData["Error"] = "Số dư không đủ để thanh toán.";
                return (View(model), null);
            }
            customer.User.Balance -= invoice.TotalAmount;

            _context.InvoiceDetails.Add(new InvoiceDetail
            {
                InvoiceId = invoice.InvoiceId,
                ProductId = product.ProductId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                LineTotal = lineTotal
            });
            return (null, invoice.InvoiceId);
        }

        private async Task<(IActionResult? ErrorResult, int? InvoiceId)> ProcessCartCheckoutAsync(Customer customer, int userId)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return (BadRequest("Giỏ hàng trống."), null);

            var subtotal = cart.CartItems.Sum(x => x.LineTotal);
            var shippingFee = Distances.CalculateShippingFee(customer.Pi.City);


            var invoice = CreateInvoiceBase(customer, subtotal, shippingFee);
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            if (customer.User.Balance < subtotal)
            {
                TempData["Error"] = "Số dư không đủ để thanh toán.";
                return (View(), null);
            }
            customer.User.Balance -= invoice.TotalAmount;

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
            return (null, invoice.InvoiceId);
        }


    }
}
