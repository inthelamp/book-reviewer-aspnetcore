using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookReviewer.Models;
using BookReviewer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using BookReviewer.Helpers;

namespace BookReviewer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookReviewerContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, BookReviewerContext context)
        {
            _logger = logger;
            _context = context;            
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Getting review subjects from book reviews which the user posted.
        /// </summary>  
        private async Task<List<ReviewSubject>> getMyReviewSubjects()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);

                var reviews = _context.Review
                                    .Where(re => re.UserId == userId)
                                    .Select(re => new ReviewSubject() 
                                    { 
                                        Id = re.Id, 
                                        Subject = re.Subject 
                                    });
                
                return await reviews.ToListAsync<ReviewSubject>();   

            }
            return null;
        }

        /// <summary>
        /// Getting review subjects from book reviews which the user didn't post.
        /// </summary>  
        private async Task<List<ReviewSubject>> getReviewSubjects()
        {
            // Excluding book reviews which the user posted.
            if (_signInManager.IsSignedIn(User))
            {
                var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);

                var reviews = _context.Review
                                    .Where(re => re.UserId != userId)
                                    .Select(re => new ReviewSubject() 
                                    { 
                                        Id = re.Id, 
                                        Subject = re.Subject 
                                    });                

                return await reviews.ToListAsync<ReviewSubject>();
                
            }                   
            else
            {
                // Containing all book reviews               
                var reviews = _context.Review
                                    .Select(re => new ReviewSubject() 
                                    { 
                                        Id = re.Id, 
                                        Subject = re.Subject 
                                    });
                return await reviews.ToListAsync<ReviewSubject>();                    
            }
        }

        /// <summary>
        /// GET: Getting and passing review subjects to the view using Dynamic Model.
        /// </summary>  
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            dynamic dynamicReviews = new ExpandoObject();

            dynamicReviews.MyReviewSubjects = await getMyReviewSubjects();
            dynamicReviews.ReviewSubjects = await getReviewSubjects();
            
            return View(dynamicReviews);
        }

        // Showing if Read tab is clicked or not in the view
        [TempData]
        public bool IsReviewContentIn { get; set; }

        /// <summary>
        /// GET: Getting review details by id and passing them to the view using TempData.
        /// </summary>  
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                IsReviewContentIn = false;
                return NotFound();
            }

            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                IsReviewContentIn = false;
                return NotFound();
            }

            IsReviewContentIn = true;
            TempData.Put("review", review);
            return RedirectToAction("Index");
        }
    
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}