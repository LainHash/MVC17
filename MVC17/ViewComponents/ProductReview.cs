using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;

namespace MVC17.ViewComponents
{
    public class ProductReview : ViewComponent
    {
        public readonly Dbmvc05Context _context;
        private readonly IMapper _mapper;

        public ProductReview(Dbmvc05Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            var reviews = await _context.VwProductReviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            var replies = await _context.VwProductReviewReplies
                .Where(r => reviews.Select(rv => rv.ReviewId).Contains(r.ReviewId))
                .ToListAsync();
            var vm = _mapper.Map<List<ViewModels.ReviewVM>>(reviews);
            foreach (var reviewVM in vm)
            {
                reviewVM.Replies = replies.Where(r => r.ReviewId == reviewVM.ReviewId).ToList();
            }
            return View(vm);
        }
    }
}
