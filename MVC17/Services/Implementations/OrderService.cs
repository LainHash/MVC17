using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.DTOs.Orders;
using MVC17.Helpers.Constants.Orders;
using MVC17.Models;
using MVC17.Services.Interfaces;

namespace MVC17.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly Dbmvc05Context _context;
        private readonly IMapper _mapper;

        public OrderService(Dbmvc05Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VwInvoice>> GetAllInvoicesAsync()
        {
            return await _context.VwInvoices
                .OrderByDescending(iv => iv.OrderedDate)
                .ToListAsync();
        }

        public List<Discount> GetDiscount(string type)
        {
            var discounts = _context.Discounts
                .Where(d => d.Type == type)
                .ToList();

            return discounts;
        }

        public async Task<VwInvoice?> GetVwInvoiceByIdAsync(int id, int? userId = null)
        {
            var query = _context.VwInvoices.AsQueryable();
            if (userId.HasValue)
            {
                var customer = await GetCustomerWithPiAsync(userId.Value);
                if (customer == null) return null;
                query = query.Where(iv => iv.CustomerId == customer.CustomerId);
            }
            return await query.FirstOrDefaultAsync(iv => iv.InvoiceId == id);
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int id, int? userId = null)
        {
            var query = _context.Invoices.AsQueryable();
            if (userId.HasValue)
            {
                var customer = await GetCustomerWithPiAsync(userId.Value);
                if (customer == null) return null;
                query = query.Where(iv => iv.CustomerId == customer.CustomerId);
            }
            return await query.FirstOrDefaultAsync(iv => iv.InvoiceId == id);
        }

        public async Task<IEnumerable<VwInvoiceDetail>> GetInvoiceDetailsAsync(int id)
        {
            return await _context.VwInvoiceDetails
                .Where(ivd => ivd.InvoiceId == id)
                .ToListAsync();
        }

        public async Task<CheckoutDTO?> PrepareCheckoutAsync(int userId, int? productId, int quantity, bool isBuyMany)
        {
            var customer = await GetCustomerWithPiAsync(userId);
            if (customer == null) return null;

            var model = new CheckoutDTO
            {
                IsBuyMany = isBuyMany,
                FullName = customer.Pi != null ? $"{customer.Pi.FirstName} {customer.Pi.LastName}" : "",
                Phone = customer.Pi?.Phone ?? "",
                Email = customer.Pi?.Email ?? "",
                Country = customer.Pi?.Country ?? "",
                City = customer.Pi?.City ?? "",
                Address = customer.Pi?.Address ?? "",
                ShippingFee = Distances.CalculateShippingFee(customer.Pi?.City ?? "Hồ Chí Minh")
            };

            if (!isBuyMany)
            {
                if (productId == null) return null;
                await FillSingleProductAsync(model, productId.Value, quantity);
            }
            else
            {
                await FillCartItemsAsync(model, userId);
            }

            return model;
        }

        public async Task<(bool Success, string Message, int? InvoiceId)> ProcessCheckoutAsync(int userId, CheckoutDTO model)
        {
            var customer = await GetCustomerWithUserAsync(userId);
            if (customer == null) return (false, "Không tìm thấy thông tin khách hàng.", null);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int? invoiceId;
                if (model.IsBuyMany)
                {
                    var result = await ProcessCartCheckoutAsync(customer, userId, model);
                    if (!result.Success)
                    {
                        await transaction.RollbackAsync();
                        return (false, result.Message, null);
                    }
                    invoiceId = result.InvoiceId;
                }
                else
                {
                    var result = await ProcessSingleProductCheckoutAsync(customer, model);
                    if (!result.Success)
                    {
                        await transaction.RollbackAsync();
                        return (false, result.Message, null);
                    }
                    invoiceId = result.InvoiceId;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (true, "Thanh toán thành công!", invoiceId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Lỗi hệ thống: {ex.Message}", null);
            }
        }

        public async Task<ConfirmOrderDTO?> PrepareConfirmOrderAsync(int invoiceId, int employeeUserId)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == employeeUserId);
            if (employee == null)
            {
                return null;
            }

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(iv => iv.InvoiceId == invoiceId);
            if (invoice == null)
            {
                return null;
            }

            var vwInvoice = await _context.VwInvoices
                .FirstOrDefaultAsync(iv => iv.InvoiceId == invoiceId);

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
                ShippedDate = null,
                NewShippedDate = null,
                Status = invoice.Status,
                TotalAmount = invoice.TotalAmount,
                Note = invoice.Note
            };

            var invoiceDetails = await GetInvoiceDetailsAsync(invoiceId);
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

            return model;
        }

        public async Task<(bool Success, string Message)> ConfirmOrderAsync(ConfirmOrderDTO model)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(iv => iv.InvoiceId == model.InvoiceId);
            if (invoice == null) return (false, "Không tìm thấy đơn hàng.");

            var customer = await _context.Customers
                .Include(c => c.Pi)
                .FirstOrDefaultAsync(c => c.CustomerId == invoice.CustomerId);
            if (customer == null) return (false, "Không tìm thấy thông tin khách hàng.");

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
                invoice.RequiredDate = DateOnly.FromDateTime(DateTime.Now).AddDays((int)Distances.CalculateShippingDays(customer.Pi.City));

                if (!string.IsNullOrEmpty(model.ConfirmationNote))
                {
                    invoice.Note = model.ConfirmationNote;
                }

                invoice.UpdatedAt = DateTime.Now;

                _context.Invoices.Update(invoice);
                await _context.SaveChangesAsync();
                return (true, "Xác nhận đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        #region Private Helpers

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

        private async Task FillSingleProductAsync(CheckoutDTO model, int productId, int quantity)
        {
            var product = await _context.Products
                .Include(p => p.ProductSku)
                .Include(p => p.Image)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product != null)
            {
                var unitPrice = product.ProductSku.UnitPrice;
                var lineTotal = unitPrice * quantity;

                model.ProductId = productId;
                model.Quantity = quantity;
                model.Subtotal = lineTotal;
                model.Items = new List<CheckoutItemDTO>
                {
                    new CheckoutItemDTO
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ImageUrl = product.Image.ImageUrl,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        LineTotal = lineTotal
                    }
                };
            }
        }

        private async Task FillCartItemsAsync(CheckoutDTO model, int userId)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Image)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null && cart.CartItems != null && cart.CartItems.Any())
            {
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
            }
        }

        private static Invoice CreateInvoiceBase(Customer customer, decimal subtotal, CheckoutDTO model)
        {
            var shippingFee = Distances.CalculateShippingFee(model.City ?? customer.Pi.City);
            var productDiscountAmount = subtotal * (decimal)model.ProductDiscount;
            var shippingDiscountAmount = shippingFee * (decimal)model.ShippingDiscount;

            var totalAmount = (subtotal - productDiscountAmount) + (shippingFee - shippingDiscountAmount);

            return new Invoice()
            {
                InvoiceUuid = Guid.NewGuid(),
                CustomerId = customer.CustomerId,
                OrderedDate = DateOnly.FromDateTime(DateTime.Now),
                RequiredDate = DateOnly.FromDateTime(DateTime.Now.AddDays(Distances.CalculateShippingDays(model.City ?? customer.Pi.City))),
                Status = "Pending",
                Subtotal = subtotal,
                ProductDiscount = model.ProductDiscount,
                ShippingFee = shippingFee,
                ShippingDiscount = model.ShippingDiscount,
                TotalAmount = totalAmount,
                Note = model.Note,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        private async Task<(bool Success, string Message, int? InvoiceId)> ProcessSingleProductCheckoutAsync(Customer customer, CheckoutDTO model)
        {
            var productId = model.ProductId ?? 0;
            var quantity = model.Quantity ?? 1;

            var product = await _context.Products
                .Include(p => p.ProductSku)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return (false, "Sản phẩm không tồn tại.", null);
            }

            var unitPrice = product.ProductSku.UnitPrice;
            var lineTotal = unitPrice * quantity;

            var invoice = CreateInvoiceBase(customer, lineTotal, model);

            if (customer.User.Balance < invoice.TotalAmount)
            {
                return (false, "Số dư không đủ để thanh toán.", null);
            }

            customer.User.Balance -= invoice.TotalAmount;
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

            return (true, "Success", invoice.InvoiceId);
        }

        private async Task<(bool Success, string Message, int? InvoiceId)> ProcessCartCheckoutAsync(Customer customer, int userId, CheckoutDTO model)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return (false, "Giỏ hàng trống.", null);

            var subtotal = cart.CartItems.Sum(x => x.LineTotal);
            var invoice = CreateInvoiceBase(customer, subtotal, model);

            if (customer.User.Balance < invoice.TotalAmount)
                return (false, "Số dư không đủ để thanh toán.", null);

            customer.User.Balance -= invoice.TotalAmount;
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

            return (true, "Success", invoice.InvoiceId);
        }
        #endregion
    }
}
