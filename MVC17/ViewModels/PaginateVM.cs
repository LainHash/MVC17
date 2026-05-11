using MVC17.Models;

namespace MVC17.ViewModels
{
    public class PaginateVM
    {
        public int ActivePage { get; set; }
        public int IndexPage { get; set; }
        public int TotalPages { get; set; }

        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public int OrderBy { get; set; }
    }
}
