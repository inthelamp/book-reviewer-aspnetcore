using BookReviewer.Data;
using BookReviewer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookReviewer.ViewComponents
{
    public class CarouselViewComponent : ViewComponent
    {

        private readonly DefaultDBContext _context;
        
        public CarouselViewComponent(DefaultDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid reviewId, string menu)
        {
            var images = await GetImagesAsync(reviewId);
            
            string MyView = "Default";

            if (menu != "Home")
            {
                MyView = "Create";
            }

            ViewBag.ReviewId = reviewId;
            ViewBag.Menu = menu;
            return View(MyView, images);
        }

        private Task<List<Image>> GetImagesAsync(Guid reviewId)
        {
            return _context!.Image!.Where(i => i.ReviewId == reviewId).ToListAsync();
        }             
    }
}