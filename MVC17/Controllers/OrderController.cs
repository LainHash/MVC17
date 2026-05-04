using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.Helpers.Constants.Sessions;
using MVC17.Models;

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

        [HttpPost]
        public async Task<IActionResult> BuyOne(int productId, int quantity = 1)
        {
            if (quantity <= 0) quantity = 1;

            // 1. lấy user hiện tại
            var userId = HttpContext.Session.GetInt32(SessionConstants.userId);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. lấy customer từ user
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsDeleted != true);

            if (customer == null)
            {
                return BadRequest("Không tìm thấy thông tin khách hàng.");
            }

            // 3. lấy sản phẩm
            var product = await _context.Products
                .Include(p => p.ProductSku)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

            var unitPrice = product.ProductSku.UnitPrice;
            var lineTotal = unitPrice * quantity;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 4. tạo invoice
                var invoice = new Invoice
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

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                // 5. tạo invoice detail
                var invoiceDetail = new InvoiceDetail
                {
                    InvoiceId = invoice.InvoiceId,
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    LineTotal = lineTotal
                };

                _context.InvoiceDetails.Add(invoiceDetail);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return RedirectToAction("Checkout", new { id = invoice.InvoiceId });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> BuyMany()
        {
            // 1. lấy user hiện tại
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // 2. lấy customer
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsDeleted != true);

            if (customer == null)
                return BadRequest("Không tìm thấy thông tin khách hàng.");

            // 3. lấy cart hiện tại
            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return BadRequest("Không tìm thấy giỏ hàng.");

            if (cart.CartItems == null || !cart.CartItems.Any())
                return BadRequest("Giỏ hàng trống.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var subtotal = cart.CartItems.Sum(x => x.LineTotal);

                // 4. tạo invoice
                var invoice = new Invoice
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

                // 5. tạo invoice details
                var invoiceDetails = cart.CartItems.Select(item => new InvoiceDetail
                {
                    InvoiceId = invoice.InvoiceId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.LineTotal
                }).ToList();

                _context.InvoiceDetails.AddRange(invoiceDetails);

                // 6. xóa cart items
                _context.CartItems.RemoveRange(cart.CartItems);

                // 7. reset cart
                cart.Subtotal = 0;
                cart.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("Checkout", new { id = invoice.InvoiceId });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
