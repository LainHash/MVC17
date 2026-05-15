using Microsoft.AspNetCore.Mvc;
using MVC17.Data;
using MVC17.Models;
using Microsoft.EntityFrameworkCore;

namespace MVC17.Controllers;

public class StatisticController : Controller
{
    private readonly Dbmvc05Context _context;
    private readonly ILogger<StatisticController> _logger;

    public StatisticController(Dbmvc05Context context, ILogger<StatisticController> logger)
    {
        _context = context;
        _logger = logger;
    }


    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Orders()
    {
        return View();
    }
    public IActionResult Products()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetTotalRevenueAllTime()
    {
        try
        {
            var totalRevenue = await _context.VwsRevenueByCategories
                .SumAsync(x => x.TotalRevenue ?? 0);

            var totalOrders = await _context.VwsCompletedOrders
                .SumAsync(x => x.CompletedOrders ?? 0);

            var totalQuantity = await _context.VwsRevenueByCategories
                .SumAsync(x => x.TotalQuantity ?? 0);

            return Json(new
            {
                success = true,
                totalRevenue,
                totalOrders,
                totalQuantity
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total revenue all time");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRevenueByYear(int year)
    {
        try
        {
            var data = await _context.VwsRevenueByCategories
                .Where(x => x.Year == year)
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(x => x.TotalRevenue ?? 0),
                    TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0)
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting revenue by year");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRevenueByMonth(int year, int month)
    {
        try
        {
            var data = await _context.VwsRevenueByCategories
                .Where(x => x.Year == year && x.Month == month)
                .GroupBy(x => x.Day)
                .Select(g => new
                {
                    Day = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue ?? 0),
                    TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0)
                })
                .OrderBy(x => x.Day)
                .ToListAsync();

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting revenue by month");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRevenueByCategory()
    {
        try
        {
            var data = await _context.VwsRevenueByCategories
                .GroupBy(x => x.CategoryName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue ?? 0),
                    TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting revenue by category");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRevenueByYearCategory(int year)
    {
        try
        {
            var data = await _context.VwsRevenueByCategories
                .Where(x => x.Year == year)
                .GroupBy(x => x.CategoryName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue ?? 0),
                    TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting revenue by year category");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRevenueByMonthCategory(int year, int month)
    {
        try
        {
            var data = await _context.VwsRevenueByCategories
                .Where(x => x.Year == year && x.Month == month)
                .GroupBy(x => x.CategoryName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue ?? 0),
                    TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting revenue by month category");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRevenueBySupplier()
    {
        try
        {
            var data = await _context.VwsRevenueBySuppliers
                .GroupBy(x => x.CompanyName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue ?? 0),
                    TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting revenue by supplier");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRevenueByYearSupplier(int year)
    {
        try
        {
            var data = await _context.VwsRevenueBySuppliers
                .Where(x => x.Year == year)
                .GroupBy(x => x.CompanyName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue ?? 0),
                    TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting revenue by year supplier");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRevenueByMonthSupplier(int year, int month)
    {
        try
        {
            var data = await _context.VwsRevenueBySuppliers
                .Where(x => x.Year == year && x.Month == month)
                .GroupBy(x => x.CompanyName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue ?? 0),
                    TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting revenue by month supplier");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableYears()
    {
        try
        {
            var yearsFromRevenue = await _context.VwsRevenueByCategories
                .Select(x => x.Year)
                .Distinct()
                .OrderByDescending(x => x)
                .ToListAsync();

            return Json(new { success = true, years = yearsFromRevenue });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available years");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTotalOrdersAllTime()
    {
        try
        {
            var totalOrders = await _context.VwsCompletedOrders
                .SumAsync(x => x.CompletedOrders ?? 0);

            var totalQuantity = await _context.VwsCompletedOrders
                .SumAsync(x => x.TotalQuantity ?? 0);

            var totalRevenue = await _context.VwsCompletedOrders
                .SumAsync(x => x.TotalRevenue ?? 0);

            return Json(new
            {
                success = true,
                totalOrders,
                totalQuantity,
                totalRevenue
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total orders all time");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersByYear(int year)
    {
        try
        {
            var totalOrders = await _context.VwsCompletedOrders
                .Where(x => x.Year == year)
                .SumAsync(x => x.CompletedOrders ?? 0);

            var totalQuantity = await _context.VwsCompletedOrders
                .Where(x => x.Year == year)
                .SumAsync(x => x.TotalQuantity ?? 0);

            var totalRevenue = await _context.VwsCompletedOrders
                .Where(x => x.Year == year)
                .SumAsync(x => x.TotalRevenue ?? 0);

            return Json(new
            {
                success = true,
                totalOrders,
                totalQuantity,
                totalRevenue
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders by year");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersByMonth(int year, int month)
    {
        try
        {
            var totalOrders = await _context.VwsCompletedOrders
                .Where(x => x.Year == year && x.Month == month)
                .SumAsync(x => x.CompletedOrders ?? 0);

            var totalQuantity = await _context.VwsCompletedOrders
                .Where(x => x.Year == year && x.Month == month)
                .SumAsync(x => x.TotalQuantity ?? 0);

            var totalRevenue = await _context.VwsCompletedOrders
                .Where(x => x.Year == year && x.Month == month)
                .SumAsync(x => x.TotalRevenue ?? 0);

            return Json(new
            {
                success = true,
                totalOrders,
                totalQuantity,
                totalRevenue
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders by month");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersToday()
    {
        try
        {
            var today = DateTime.Now;

            var totalOrders = await _context.VwsCompletedOrders
                .Where(x => x.Year == today.Year && x.Month == today.Month && x.Day == today.Day)
                .SumAsync(x => x.CompletedOrders ?? 0);

            var totalQuantity = await _context.VwsCompletedOrders
                .Where(x => x.Year == today.Year && x.Month == today.Month && x.Day == today.Day)
                .SumAsync(x => x.TotalQuantity ?? 0);

            var totalRevenue = await _context.VwsCompletedOrders
                .Where(x => x.Year == today.Year && x.Month == today.Month && x.Day == today.Day)
                .SumAsync(x => x.TotalRevenue ?? 0);

            return Json(new
            {
                success = true,
                totalOrders,
                totalQuantity,
                totalRevenue
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders today");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCancelledOrders()
    {
        try
        {
            var totalOrders = await _context.VwsCancelledOrders
                .CountAsync();

            var totalQuantity = await _context.VwsCancelledOrders
                .SumAsync(x => x.TotalQuantity ?? 0);

            var totalRevenue = await _context.VwsCancelledOrders
                .SumAsync(x => x.TotalRevenue ?? 0);

            return Json(new
            {
                success = true,
                totalOrders,
                totalQuantity,
                totalRevenue
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cancelled orders");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCompletedOrders()
    {
        try
        {
            var totalOrders = await _context.VwsCompletedOrders
                .SumAsync(x => x.CompletedOrders ?? 0);

            var totalQuantity = await _context.VwsCompletedOrders
                .SumAsync(x => x.TotalQuantity ?? 0);

            var totalRevenue = await _context.VwsCompletedOrders
                .SumAsync(x => x.TotalRevenue ?? 0);

            return Json(new
            {
                success = true,
                totalOrders,
                totalQuantity,
                totalRevenue
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting completed orders");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRefundedOrders()
    {
        try
        {
            var totalOrders = await _context.VwsRefundedOrders
                .SumAsync(x => x.RefundedOrders ?? 0);

            var totalQuantity = await _context.VwsRefundedOrders
                .SumAsync(x => x.TotalQuantity ?? 0);

            var totalRevenue = await _context.VwsRefundedOrders
                .SumAsync(x => x.TotalRevenue ?? 0);

            return Json(new
            {
                success = true,
                totalOrders,
                totalQuantity,
                totalRevenue
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refunded orders");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTotalProductsAllTime()
    {
        try
        {
            var totalProducts = await _context.VwsTotalProducts
                .SumAsync(x => x.TotalProducts ?? 0);

            return Json(new { success = true, totalProducts });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total products all time");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetLowStockProducts()
    {
        try
        {
            var data = await _context.VwsLowStockProducts
                .OrderBy(x => x.UnitsInStock)
                .Take(20)
                .Select(x => new
                {
                    x.ProductName,
                    x.UnitsInStock
                })
                .ToListAsync();

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low stock products");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTopSellingProducts()
    {
        try
        {
            var data = await _context.VwsRevenueByProducts
                .GroupBy(x => x.ProductName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0),
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(10)
                .ToListAsync();

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top selling products");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTopRevenueProducts(int? year = null, int? month = null)
    {
        try
        {
            IQueryable<VwsRevenueByProduct> query = _context.VwsRevenueByProducts;

            if (year.HasValue)
                query = query.Where(x => x.Year == year);

            if (month.HasValue)
                query = query.Where(x => x.Month == month);

            var data = await query
                .GroupBy(x => x.ProductName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue ?? 0),
                    TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(10)
                .ToListAsync();

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top revenue products");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetLowSellingProducts(int? year = null, int? month = null)
    {
        try
        {
            IQueryable<VwsRevenueByProduct> query = _context.VwsRevenueByProducts;

            if (year.HasValue)
                query = query.Where(x => x.Year == year);

            if (month.HasValue)
                query = query.Where(x => x.Month == month);

            var data = await query
                .GroupBy(x => x.ProductName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue ?? 0),
                    TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0)
                })
                .OrderBy(x => x.TotalRevenue)
                .Take(10)
                .ToListAsync();

            return Json(new { success = true, data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low selling products");
            return Json(new { success = false, message = ex.Message });
        }
    }

}
