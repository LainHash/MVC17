using Microsoft.AspNetCore.Mvc;
using MVC17.DTOs.Products.Update;

namespace MVC17.ViewComponents
{
    public class UpdateProductSpec : ViewComponent
    {
        public IViewComponentResult Invoke(UpdateProductDTO dto, int categoryId)
        {
            ViewBag.CategoryId = categoryId;
            return View(dto);
        }
    }
}
