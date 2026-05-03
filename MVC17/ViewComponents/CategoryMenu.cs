using Microsoft.AspNetCore.Mvc;
using MVC17.Data;

namespace MVC17.ViewComponents
{
    public class CategoryMenu : ViewComponent
    {
        private readonly Dbmvc05Context _context;

        public CategoryMenu(Dbmvc05Context context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var categories = _context.Categories
                .Select(c => new ViewModels.CategoryMenuVM
                {
                    CategoryId = c.CategoryId,
                    Name = c.CategoryName,
                    Count = c.Products.Count(p => !p.IsDeleted)
                })
                .ToList();
            return View(categories);
        }
    }
}
