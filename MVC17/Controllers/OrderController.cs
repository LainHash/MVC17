using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC17.DTOs.Orders;
using MVC17.Helpers.Constants.Auths.Accounts;
using MVC17.Helpers.Constants.Orders;
using MVC17.Services.Interfaces;
using System.Security.Claims;

namespace MVC17.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Index()
        {
            var invoices = await _orderService.GetAllInvoicesAsync();
            return View(invoices);
        }

        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _orderService.GetVwInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound();

            ViewBag.InvoiceDetails = await _orderService.GetInvoiceDetailsAsync(id);

            return View(invoice);
        }

        [HttpGet]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> Checkout(int? productId, int quantity = 1, bool isBuyMany = false)
        {
            if (quantity <= 0) quantity = 1;

            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Customer");
            }

            

            var model = await _orderService.PrepareCheckoutAsync(userId, productId, quantity, isBuyMany);
            if (model == null)
            {
                if (!isBuyMany && productId == null)
                {
                    return Json(new { success = false, message = "Thiếu sản phẩm." });
                }
                return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng hoặc lỗi xử lý." });
            }

            // Handling the error case for BuyMany specifically if needed
            if (isBuyMany && (model.Items == null || !model.Items.Any()))
            {
                ViewData["Error"] = "Giỏ hàng trống!";
                return View(model);
            }

            ViewBag.Cities = new SelectList(UserProfileConstants.Cities);
            //ViewBag.Countries = new SelectList(UserProfileConstants.Countries);
            ViewBag.ProductDiscounts = new SelectList(_orderService
                .GetDiscount("Product")
                .Select(d => new { Value = d.DiscountAmount, Text = $"{(d.DiscountAmount * 100):0}%" }),
                "Value",
                "Text");
            ViewBag.ShippingDiscounts = new SelectList(_orderService
                .GetDiscount("Shipping")
                .Select(d => new { Value = d.DiscountAmount, Text = $"{(d.DiscountAmount * 100):0}%" }),
                "Value",
                "Text");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> Checkout(CheckoutDTO model)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Customer");
            }

            var result = await _orderService.ProcessCheckoutAsync(userId, model);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                if (result.Message == "Số dư không đủ để thanh toán.")
                {
                    var checkoutModel = await _orderService.PrepareCheckoutAsync(userId, model.ProductId, model.Quantity ?? 1, model.IsBuyMany);
                    return View(checkoutModel);
                }
                return BadRequest(result.Message);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("Success", new { id = result.InvoiceId });
        }

        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> Success(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Customer");
            }
            var invoice = await _orderService.GetVwInvoiceByIdAsync(id, userId);
            return View(invoice);
        }

        [HttpGet]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> CheckoutResult(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Customer");
            }

            var invoice = await _orderService.GetVwInvoiceByIdAsync(id, userId);

            // Basic check if the invoice belongs to the customer (Service should ideally handle this check too)
            // For now, keeping it simple as the previous code did
            if (invoice == null)
                return NotFound();

            ViewBag.InvoiceDetails = await _orderService.GetInvoiceDetailsAsync(id);

            return View(invoice);
        }

        [HttpGet]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Confirm(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return RedirectToAction("Login", "Manager");
            }

            var model = await _orderService.PrepareConfirmOrderAsync(id, userId);
            if (model == null)
            {
                return NotFound("Không tìm thấy đơn hàng hoặc nhân viên.");
            }

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

            var result = await _orderService.ConfirmOrderAsync(model);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Confirm", new { id = model.InvoiceId });
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("Details", new { id = model.InvoiceId });
        }

        [HttpGet]
        public IActionResult GetShippingInfo(string city)
        {
            try
            {
                var fee = Distances.CalculateShippingFee(city);
                var days = Distances.CalculateShippingDays(city);
                var estimatedDate = DateTime.Now.AddDays(days);

                return Json(new
                {
                    success = true,
                    fee = fee,
                    days = days,
                    estimatedDate = estimatedDate.ToString("dd/MM/yyyy")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        private bool TryGetCurrentUserId(out int userId)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out userId);
        }
    }

}
