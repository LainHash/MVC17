using Microsoft.AspNetCore.Mvc;
using MVC17.Helpers.Constants.Sessions;
using MVC17.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var sessionId = GetOrCreateSessionId();

        var cart = await _cartService.GetAllAsync(userId, sessionId);

        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int id, int quantity = 1)
    {
        var userId = GetUserId();
        var sessionId = GetOrCreateSessionId();

        var result = await _cartService.AddAsync(id, quantity, userId, sessionId);

        if (!result.Success)
        {
            if (result.Message == "Sản phẩm không tồn tại.")
            {
                return NotFound();
            }
            return BadRequest(result.Message);
        }

        return Json(new
        {
            success = true,
            message = result.Message
        });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateItemQuantity(int cartItemId, int quantity)
    {
        var result = await _cartService.UpdateItemQuantityAsync(cartItemId, quantity);

        if (!result.Success)
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        var result = await _cartService.RemoveItemAsync(cartItemId);

        if (!result.Success)
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private string GetOrCreateSessionId()
    {
        var sessionId = HttpContext.Session.GetString(SessionConstants.sessionId);

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            sessionId = Guid.NewGuid().ToString();
            HttpContext.Session.SetString(SessionConstants.sessionId, sessionId);
        }

        return sessionId;
    }

    private int? GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdString, out int userId))
            return userId;
        return null;
    }
}