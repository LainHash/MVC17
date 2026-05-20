using MVC17.Models;

namespace MVC17.ViewModels
{
    public class ReviewVM : VwProductReview
    {
        public List<VwProductReviewReply>? Replies { get; set; } 
    }
}
