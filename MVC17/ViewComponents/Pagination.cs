using Microsoft.AspNetCore.Mvc;
using MVC17.Models;
using MVC17.ViewModels;

namespace MVC17.ViewComponents
{
    public class Pagination : ViewComponent
    {
        public IViewComponentResult Invoke( 
            int active = 1,
            int index = 1,
            int total = 1,
            int categoryId = 0,
            int supplierId = 0,
            int orderBy = 0)
        {
            var paginate = new PaginateVM()
            {
                ActivePage = active,
                IndexPage = index,
                TotalPages = total,
                CategoryId = categoryId,
                SupplierId = supplierId,
                OrderBy = orderBy
            };
            return View(paginate);
        }
    }
}
