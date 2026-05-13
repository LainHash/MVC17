using Microsoft.AspNetCore.Mvc;
using MVC17.ViewModels;
using System.Collections.Generic;

namespace MVC17.ViewComponents
{
    public class FilterDropdownViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(FilterDropdownVM model)
        {
            return View(model);
        }
    }
}
