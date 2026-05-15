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


        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.VwInvoices
                .OrderByDescending(iv => iv.OrderedDate)
                .ToListAsync();
            return View(invoices);
        }

        [Authorize(Policy = "Manager")]
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
        [Authorize(Policy = "Customer")]
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
                return Json(new
                {
                    success = false,
                    message = "Không tìm thấy thông tin khách hàng."
                });
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
                    return Json(new
                    {
                        success = false,
                        message = "Thiếu sản phẩm."
                    });
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
        [Authorize(Policy = "Customer")]
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

        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> Success(int id)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(iv => iv.InvoiceId == id);
            return View(invoice);
        }

        [HttpGet]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> CheckoutResult(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await GetCustomerWithPiAsync(userId);
            if (customer == null)
            {
                return NotFound("Không tìm thấy thông tin khách hàng.");
            }

            var invoice = await _context.VwInvoices
                .FirstOrDefaultAsync(iv => iv.InvoiceId == id && iv.CustomerId == customer.CustomerId);

            if (invoice == null)
                return NotFound();

            ViewBag.InvoiceDetails = await _context.VwInvoiceDetails
                .Where(ivd => ivd.InvoiceId == id)
                .ToListAsync();

            return View(invoice);
        }

        [HttpGet]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Confirm(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(iv => iv.InvoiceId == id);
            if (invoice == null)
                return NotFound("Không tìm thấy đơn hàng.");

            var vwInvoice = await _context.VwInvoices
                .FirstOrDefaultAsync(iv => iv.InvoiceId == id);

            var customer = await _context.Customers
                .Include(c => c.Pi)
                .FirstOrDefaultAsync(c => c.CustomerId == invoice.CustomerId);
            if (customer == null)
            {
                return NotFound();
            }

            var model = new ConfirmOrderDTO
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceUuid = invoice.InvoiceUuid,
                CustomerId = invoice.CustomerId,
                CustomerCode = vwInvoice?.CustomerCode ?? "",
                EmployeeCode = employee.EmployeeCode,
                EmployeeId = employee.EmployeeId,
                OrderedDate = invoice.OrderedDate,
                RequiredDate = invoice.RequiredDate,
                ShippedDate = invoice.ShippedDate,
                NewShippedDate = DateOnly.FromDateTime(DateTime.Now),
                Status = invoice.Status,
                TotalAmount = invoice.TotalAmount,
                Note = invoice.Note
            };

            var invoiceDetails = await _context.VwInvoiceDetails
                .Where(ivd => ivd.InvoiceId == id)
                .ToListAsync();

            model.InvoiceDetails = invoiceDetails.Select(ivd => new InvoiceDetailDTO
            {
                InvoiceDetailId = ivd.InvoiceDetailId,
                ProductId = ivd.ProductId,
                ProductName = ivd.ProductName,
                CategoryName = ivd.CategoryName,
                CompanyName = ivd.CompanyName,
                UnitPrice = ivd.UnitPrice,
                Quantity = ivd.Quantity,
                LineTotal = ivd.LineTotal
            }).ToList();

            model.AvailableEmployees = await _context.Employees
                .Where(e => e.IsDeleted != true)
                .Select(e => new EmployeeOption
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeCode = e.EmployeeCode,
                    EmployeeName = $"{e.Pi.FirstName} {e.Pi.LastName}"
                })
                .ToListAsync();

            model.AvailableStatuses = new List<StatusOption>
            {
                new StatusOption { Status = "Pending", StatusVi = "Đang xử lý" },
                new StatusOption { Status = "Shipping", StatusVi = "Đang giao hàng" },
                new StatusOption { Status = "Delivered", StatusVi = "Đã giao hàng" },
                new StatusOption { Status = "Completed", StatusVi = "Hoàn thành" },
                new StatusOption { Status = "Cancelled", StatusVi = "Đã hủy" },
                new StatusOption { Status = "Refunded", StatusVi = "Đã hoàn tiền" }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Confirm(ConfirmOrderDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(iv => iv.InvoiceId == model.InvoiceId);
            if (invoice == null)
                return NotFound("Không tìm thấy đơn hàng.");

            var customer = await _context.Customers
                .Include(c => c.Pi)
                .FirstOrDefaultAsync(c => c.CustomerId == invoice.CustomerId);
            if(customer == null)
            {
                return NotFound();
            }

            try
            {
                if (model.NewEmployeeId.HasValue && model.NewEmployeeId > 0)
                {
                    invoice.EmployeeId = model.NewEmployeeId;
                }

                if (!string.IsNullOrEmpty(model.NewStatus))
                {
                    invoice.Status = model.NewStatus;
                }

                invoice.ShippedDate = model.NewShippedDate;
                invoice.RequiredDate = model.NewShippedDate.AddDays((int)Distances.CalculateShippingDays(customer.Pi.City));

                if (!string.IsNullOrEmpty(model.ConfirmationNote))
                {
                    invoice.Note = model.ConfirmationNote;
                }

                invoice.UpdatedAt = DateTime.Now;

                _context.Invoices.Update(invoice);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xác nhận đơn hàng thành công!";
                return RedirectToAction("Details", new { id = model.InvoiceId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Confirm", new { id = model.InvoiceId });
            }
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
                return Json(new
                {
                    success = false,
                    message = "Sản phẩm không tồn tại!."
                }); ;
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
