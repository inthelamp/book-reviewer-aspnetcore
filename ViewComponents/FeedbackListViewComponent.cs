using BookReviewer.Data;
using BookReviewer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookReviewer.ViewComponents
{
    public class FeedbackListViewComponent : ViewComponent
    {
        private readonly DefaultDBContext _context;


        public FeedbackListViewComponent(DefaultDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid reviewId, string menu)
        {
            var feedbacks = await GetFeedbacksAsync(reviewId);

            string MyView = "Default";

            if (menu != "Home")
            {
                MyView = "Create";
            }

            ViewBag.ReviewId = reviewId;
            ViewBag.Menu = menu;
            return View(MyView, feedbacks);
        }

        private Task<List<Feedback>> GetFeedbacksAsync(Guid reviewId)
        {
            return _context!.Feedback!.Where(f => f.ReviewId == reviewId).ToListAsync();
        }        
    }
}