using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.Models;
using MVC17.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC17.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly Dbmvc05Context _context;

        public CartService(Dbmvc05Context context)
        {
            _context = context;
        }

        public async Task<List<VwShoppingCart>> GetAllAsync(int? userId, string? sessionId)
        {
            return await _context.VwShoppingCarts
                .Where(x =>
                    (userId != null && x.UserId == userId) ||
                    (userId == null && x.SessionId == sessionId))
                .ToListAsync();
        }

        private async Task<ShoppingCart> GetOrCreateCartAsync(int? userId, string sessionId)
        {
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

        public async Task<(bool Success, string Message)> AddAsync(int id, int quantity, int? userId, string sessionId)
        {
            if (quantity <= 0)
            {
                quantity = 1;
            }

            var product = await _context.VwProducts
                .FirstOrDefaultAsync(x => x.ProductId == id && !x.IsDeleted && !x.Discontinued);

            if (product == null)
            {
                return (false, "Sản phẩm không tồn tại.");
            }

            if (product.UnitsInStock < quantity)
            {
                return (false, "Not enough stock.");
            }

            var cart = await GetOrCreateCartAsync(userId, sessionId);

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
                    LineTotal = product.UnitPrice * quantity,
                    AddedDate = DateTime.Now
                };

                _context.CartItems.Add(item);
            }
            else
            {
                var newQty = item.Quantity + quantity;

                if (newQty > product.UnitsInStock)
                    return (false, "Not enough stock.");

                item.Quantity = newQty;
                item.UnitPrice = product.UnitPrice;
                item.LineTotal = product.UnitPrice * newQty;
            }

            await _context.SaveChangesAsync();
            return (true, "Đã thêm vào giỏ hàng!");
        }

        public async Task<(bool Success, string Message)> UpdateItemQuantityAsync(int cartItemId, int quantity)
        {
            var item = await _context.CartItems
                .Include(x => x.ShoppingCart)
                .FirstOrDefaultAsync(x => x.CartItemId == cartItemId);

            if (item == null)
            {
                return (false, "Không tìm thấy sản phẩm trong giỏ hàng.");
            }

            if (quantity <= 0)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
                return (true, "Đã xoá sản phẩm khỏi giỏ hàng.");
            }

            var product = await _context.VwProducts
                .FirstOrDefaultAsync(x => x.ProductSkuId == item.ProductId);

            if (product == null)
            {
                return (false, "Sản phẩm không tồn tại hoặc đã ngừng kinh doanh.");
            }

            if (quantity > product.UnitsInStock)
            {
                return (false, $"Số lượng yêu cầu vượt quá số lượng tồn kho. Chỉ còn {product.UnitsInStock} sản phẩm.");
            }

            item.Quantity = quantity;
            item.UnitPrice = product.UnitPrice;
            item.LineTotal = product.UnitPrice * quantity;

            await _context.SaveChangesAsync();
            return (true, "Cập nhật thành công.");
        }

        public async Task<(bool Success, string Message)> RemoveItemAsync(int cartItemId)
        {
            var item = await _context.CartItems
                .Include(x => x.ShoppingCart)
                .FirstOrDefaultAsync(x => x.CartItemId == cartItemId);

            if (item == null)
            {
                return (false, "Không tìm thấy sản phẩm trong giỏ hàng để xoá.");
            }

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return (true, "Xoá thành công.");
        }

        public async Task<int> CleanupExpiredGuestCartsAsync()
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
}
