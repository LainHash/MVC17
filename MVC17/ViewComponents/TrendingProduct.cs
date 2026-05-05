using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MVC17.Data;
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

        public IViewComponentResult Invoke()
        {
            var laptops = _context.VwTrendingLaptops
                .OrderByDescending(l => l.CreatedInvoices)
                .Take(5);
            var vm = _mapper.Map<List<TrendingLaptopVM>>(laptops);
            return View(vm);
        }
    }
}
