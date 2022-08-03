using BookReviewer.Data;
using BookReviewer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookReviewer.ViewComponents
{
    public class SocialMediaViewComponent : ViewComponent
    {

        private readonly DefaultDBContext _context;
        
        public SocialMediaViewComponent(DefaultDBContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(Guid reviewId)
        {
            var review = GetReview(reviewId);
            return View(review);
        }

        private Review GetReview(Guid reviewId)
        {
            return _context!.Review!.Where(r => r.Id == reviewId).FirstOrDefault();
        }               
    }
}