using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.Helpers.Constants.Sessions;
using MVC17.Models;

public class CartController : Controller
{
    private readonly Dbmvc05Context _context;

    public CartController(Dbmvc05Context context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32(SessionConstants.userId);
        var sessionId = HttpContext.Session.GetString(SessionConstants.sessionId);

        var cart = await _context.VwShoppingCarts
            .Where(x =>
                (userId != null && x.UserId == userId) ||
                (userId == null && x.SessionId == sessionId))
            .ToListAsync();

        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int id, int quantity = 1)
    {
        if (quantity <= 0)
        {
            quantity = 1;
        }

        var product = await _context.VwProducts
            .FirstOrDefaultAsync(x => x.ProductId == id && !x.IsDeleted && !x.Discontinued);

        if (product == null)
        {
            return NotFound();
        }

        if (product.UnitsInStock < quantity)
        {
            return BadRequest("Not enough stock.");
        }

        var cart = await GetOrCreateCartAsync();

        var item = await _context.CartItems
            .FirstOrDefaultAsync(x => x.ShoppingCartId == cart.ShoppingCartId && x.ProductId == id);

        if (item == null)
        {
            item = new CartItem
            {
                ShoppingCartId = cart.ShoppingCartId,
                ProductId = id,
                Quantity = quantity,
                UnitPrice = product.UnitPrice,
                AddedDate = DateTime.Now
            };

            _context.CartItems.Add(item);
        }
        else
        {
            var newQty = item.Quantity + quantity;

            if (newQty > product.UnitsInStock)
                return BadRequest("Not enough stock.");

            item.Quantity = newQty;
            item.UnitPrice = product.UnitPrice;
        }

        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            message = "Đã thêm vào giỏ hàng!"
        });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateItemQuantity(int cartItemId, int quantity)
    {
        var item = await _context.CartItems
            .Include(x => x.ShoppingCart)
            .FirstOrDefaultAsync(x => x.CartItemId == cartItemId);

        if (item == null)
            return NotFound();

        if (quantity <= 0)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        var product = await _context.VwProducts
            .FirstOrDefaultAsync(x => x.ProductSkuId == item.ProductId);

        if (product == null)
            return NotFound();

        if (quantity > product.UnitsInStock)
            return BadRequest("Not enough stock.");

        item.Quantity = quantity;
        item.UnitPrice = product.UnitPrice;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        var item = await _context.CartItems
            .Include(x => x.ShoppingCart)
            .FirstOrDefaultAsync(x => x.CartItemId == cartItemId);

        if (item == null)
            return NotFound();

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();

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
        return HttpContext.Session.GetInt32(SessionConstants.userId);
    }

    private async Task<ShoppingCart?> GetCartAsync()
    {
        var sessionId = GetOrCreateSessionId();
        var userId = GetUserId();

        return await _context.ShoppingCarts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c =>
                (userId != null && c.UserId == userId) ||
                (userId == null && c.SessionId == sessionId));
    }

    private async Task<ShoppingCart> GetOrCreateCartAsync()
    {
        var sessionId = GetOrCreateSessionId();
        var userId = GetUserId();

        var cart = await _context.ShoppingCarts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c =>
                (userId != null && c.UserId == userId) ||
                (userId == null && c.SessionId == sessionId));

        if (cart != null)
            return cart;

        cart = new ShoppingCart
        {
            SessionId = sessionId,
            UserId = userId
        };

        _context.ShoppingCarts.Add(cart);
        await _context.SaveChangesAsync();

        return cart;
    }

    private async Task<int> CleanupExpiredGuestCartsAsync()
    {
        var cutoff = DateTime.Now.AddDays(-2);

        var expiredCartIds = await _context.ShoppingCarts
            .Where(x => x.UserId == null
                        && x.SessionId != null
                        && x.CreatedAt <= cutoff)
            .Select(x => x.ShoppingCartId)
            .ToListAsync();

        if (!expiredCartIds.Any())
            return 0;

        await _context.CartItems
            .Where(x => expiredCartIds.Contains(x.ShoppingCartId))
            .ExecuteDeleteAsync();

        var deletedCount = await _context.ShoppingCarts
            .Where(x => expiredCartIds.Contains(x.ShoppingCartId))
            .ExecuteDeleteAsync();

        return deletedCount;
    }
}