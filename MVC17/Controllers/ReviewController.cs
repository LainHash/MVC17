using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.Models;
using System.Security.Claims;

namespace MVC17.Controllers
{
    public class ReviewController : Controller
    {
        private readonly Dbmvc05Context _context;

        public ReviewController(Dbmvc05Context context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Policy = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, int rating, string title, string comment)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized();
            }

            var review = new ProductReview
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Title = title,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đánh giá của bạn đã được gửi thành công!";
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        [HttpPost]
        [Authorize(Policy = "Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply(int reviewId, string replyContent)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized();
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId && !e.IsDeleted);
            if (employee == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin nhân viên.";
                return RedirectBack();
            }

            var review = await _context.ProductReviews.FindAsync(reviewId);
            if (review == null)
            {
                TempData["Error"] = "Không tìm thấy đánh giá.";
                return RedirectBack();
            }

            var reply = new ProductReviewReply
            {
                ReviewId = reviewId,
                EmployeeId = employee.EmployeeId,
                ReplyContent = replyContent,
                CreatedAt = DateTime.Now
            };

            _context.ProductReviewReplies.Add(reply);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã phản hồi đánh giá thành công!";
            return RedirectToAction("Details", "Product", new { id = review.ProductId });
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out userId);
        }

        private IActionResult RedirectBack()
        {
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }
    }
}
