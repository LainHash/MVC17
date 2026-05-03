using Microsoft.AspNetCore.Mvc;
using MVC17.ViewModels;

namespace MVC17.ViewComponents
{
    public class ProductSpec : ViewComponent
    {
        public IViewComponentResult Invoke(ProductVM vm)
        {
            return View(vm);
        }
    }
}
