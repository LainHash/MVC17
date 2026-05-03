using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC17.Data;
using MVC17.DTOs.Products.Create;

namespace MVC17.ViewComponents
{
    public class CreateProductSpec : ViewComponent
    {
        private readonly Dbmvc05Context _context;
        public CreateProductSpec(Dbmvc05Context context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke(CreateProductDTO dto, int categoryId)
        {
            dto.CategoryId = categoryId;
            return View(dto);
        }
    }
}
