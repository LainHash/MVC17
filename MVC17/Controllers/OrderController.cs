using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.Helpers.Constants.Sessions;
using MVC17.Models;
using MVC17.ViewModels;

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
                .OrderByDescending(iv => iv.OrderDate)
                .ToListAsync();
            return View(invoices);
        }
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.VwInvoices
                .FirstOrDefaultAsync(iv => iv.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            ViewBag.InvoiceDetails = await _context.VwInvoiceDetails
                .Where(ivd => ivd.InvoiceId == id)
                .ToListAsync();

            return View(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(int? productId, int quantity = 1, bool isBuyMany = false)
        {
            if (quantity <= 0) quantity = 1;

            var userId = HttpContext.Session.GetInt32(SessionConstants.userId);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var customer = await _context.Customers
                .Include(c => c.Pi)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsDeleted != true);

            if (customer == null)
                return BadRequest("Không tìm thấy thông tin khách hàng.");

            var model = new CheckoutVM
            {
                IsBuyMany = isBuyMany,
                FullName = customer.Pi != null ? $"{customer.Pi.FirstName} {customer.Pi.LastName}" : "",
                Phone = customer.Pi?.Phone ?? "",
                Email = customer.Pi?.Email ?? "",
                Address = customer.Pi?.Address ?? "",
                ShippingFee = 0
            };

            if (!isBuyMany)
            {
                if (productId == null)
                    return BadRequest("Thiếu sản phẩm.");

                var product = await _context.Products
                    .Include(p => p.ProductSku)
                    .Include(p => p.Image)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                    return NotFound("Sản phẩm không tồn tại.");

                var unitPrice = product.ProductSku.UnitPrice;
                var lineTotal = unitPrice * quantity;

                model.ProductId = productId;
                model.Quantity = quantity;
                model.Items = new List<CheckoutItemVM>
                {
                    new CheckoutItemVM
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ImageUrl = product.Image.ImageUrl,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        LineTotal = lineTotal
                    }
                };
                model.Subtotal = lineTotal;
            }
            else
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
                    LineTotal = item.LineTotal
                }).ToList();

                model.Subtotal = cart.CartItems.Sum(x => x.LineTotal);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            var userId = HttpContext.Session.GetInt32(SessionConstants.userId);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var customer = await _context.Customers
                .Include(cst => cst.User)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsDeleted != true);

            if (customer == null)
                return BadRequest("Không tìm thấy thông tin khách hàng.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Invoice invoice;

                if (!model.IsBuyMany)
                {
                    var productId = model.ProductId ?? 0;
                    var quantity = model.Quantity ?? 1;

                    var product = await _context.Products
                        .Include(p => p.ProductSku)
                        .FirstOrDefaultAsync(p => p.ProductId == productId);

                    if (product == null)
                        return NotFound("Sản phẩm không tồn tại.");

                    var unitPrice = product.ProductSku.UnitPrice;
                    var lineTotal = unitPrice * quantity;

                    if (customer.User.Balance < lineTotal)
                    {
                        ModelState.AddModelError("", "Số dư không đủ để thanh toán.");
                        return View(model);
                    }

                    invoice = new Invoice
                    {
                        InvoiceUuid = Guid.NewGuid(),
                        CustomerId = customer.CustomerId,
                        OrderDate = DateOnly.FromDateTime(DateTime.Now),
                        RequiredDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                        Status = "Pending",
                        Subtotal = lineTotal,
                        ProductDiscount = 0,
                        ShippingFee = 0,
                        ShippingDiscount = 0,
                        TotalAmount = lineTotal,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

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
                }
                else
                {
                    var cart = await _context.ShoppingCarts
                        .Include(c => c.CartItems)
                        .FirstOrDefaultAsync(c => c.UserId == userId);

                    if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                        return BadRequest("Giỏ hàng trống.");

                    var subtotal = cart.CartItems.Sum(x => x.LineTotal);

                    if (customer.User.Balance < subtotal)
                    {
                        ModelState.AddModelError("", "Số dư không đủ để thanh toán.");
                        return View(model);
                    }

                    invoice = new Invoice
                    {
                        InvoiceUuid = Guid.NewGuid(),
                        CustomerId = customer.CustomerId,
                        OrderDate = DateOnly.FromDateTime(DateTime.Now),
                        RequiredDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                        Status = "Pending",
                        Subtotal = subtotal,
                        ProductDiscount = 0,
                        ShippingFee = 0,
                        ShippingDiscount = 0,
                        TotalAmount = subtotal,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Invoices.Add(invoice);
                    await _context.SaveChangesAsync();

                    var invoiceDetails = cart.CartItems.Select(item => new InvoiceDetail
                    {
                        InvoiceId = invoice.InvoiceId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = item.LineTotal
                    }).ToList();

                    _context.InvoiceDetails.AddRange(invoiceDetails);

                    _context.CartItems.RemoveRange(cart.CartItems);
                    cart.Subtotal = 0;
                    cart.UpdatedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("Success", new { id = invoice.InvoiceId });
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
    }
}
