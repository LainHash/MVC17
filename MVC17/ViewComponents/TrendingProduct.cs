using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.Helpers.Constants.Products;
using MVC17.Models;
using MVC17.ViewModels;

namespace MVC17.ViewComponents
{
    public class TrendingProduct : ViewComponent
    {
        private readonly Dbmvc05Context _context;
        private readonly IMapper _mapper;

        public TrendingProduct(Dbmvc05Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var products = await _context.VwTrendingProducts
                .Where(p => p.CategoryId == CategoryConstants.Laptop)
                .OrderByDescending(x => x.CreatedInvoices)
                .Take(5)
                .ToListAsync();
            return View(products);
        }
    }
}
