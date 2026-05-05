using Microsoft.AspNetCore.Mvc;
using MVC17.Data;

namespace MVC17.ViewComponents
{
    public class ProductCategories : ViewComponent
    {
        private readonly Dbmvc05Context _context;

        public ProductCategories(Dbmvc05Context context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var products = _context.VwProducts.Take(5);
            return View(products);
        }
    }
}
