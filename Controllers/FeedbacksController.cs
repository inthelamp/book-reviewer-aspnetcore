using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookReviewer.Models;
using BookReviewer.Data;
using Microsoft.AspNetCore.Authorization;
using BookReviewer.ViewComponents;
using System.Security.Claims;

namespace BookReviewer.Controllers
{
    public class FeedbacksController : Controller
    {

        private readonly DefaultDBContext _context;

        public FeedbacksController(DefaultDBContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFeedback(Guid reviewId, string menu, [Bind("Comment")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                var review = await _context.Review
                            .FirstOrDefaultAsync(m => m.Id == reviewId);
                feedback.Review = review;

                ClaimsPrincipal currentUser = this.User;
                if (currentUser.Identity.IsAuthenticated)
                {
                    string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

                    feedback.UserId = userId;
                }

                _context.Feedback.Add(feedback);
                _context.Review.Update(review);

                await _context.SaveChangesAsync();     
            }

            return RedirectToAction("Details", "Reviews", new { id = reviewId, menu = menu});
        }
    }
}