using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookReviewer.Models;
using BookReviewer.ViewModels;
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
        private readonly DefaultDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, DefaultDBContext context)
        {
            _logger = logger;
            _context = context;            
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Gets review subjects from reviews which the user posted.
        /// </summary>  
        private async Task<List<ReviewSubjectViewModel>> getMyReviewSubjects()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);

                var reviews = _context.Review
                                    .Where(re => re.AuthorId == userId)
                                    .OrderByDescending (re => re.UpdateDate)                                   
                                    .Select(re => new ReviewSubjectViewModel() 
                                    { 
                                        Id = re.Id, 
                                        Subject = re.Subject
                                    });
                
                return await reviews.ToListAsync<ReviewSubjectViewModel>();   

            }
            return null;
        }

        /// <summary>
        /// Gets review subjects from reviews which the user didn't post.
        /// </summary>  
        private async Task<List<ReviewSubjectViewModel>> getReviewSubjects()
        {
            // Excluding book reviews which the user posted.
            if (_signInManager.IsSignedIn(User))
            {
                var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);

                var reviews = _context.Review
                                    .Where(re => re.AuthorId != userId)
                                    .OrderByDescending (re => re.UpdateDate)
                                    .Select(re => new ReviewSubjectViewModel() 
                                    { 
                                        Id = re.Id, 
                                        Subject = re.Subject
                                    });  

                return await reviews.ToListAsync<ReviewSubjectViewModel>();
                
            }                   
            else
            {
                // Containing all book reviews               
                var reviews = _context.Review
                                    .OrderByDescending (re => re.UpdateDate)                
                                    .Select(re => new ReviewSubjectViewModel() 
                                    { 
                                        Id = re.Id, 
                                        Subject = re.Subject
                                    });
                return await reviews.ToListAsync<ReviewSubjectViewModel>();                    
            }
        }

        /// <summary>
        /// GET: /
        /// Gets review subjects and passes them to the view by using Dynamic Model.
        /// </summary>  
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            dynamic dynamicReviews = new ExpandoObject();

            dynamicReviews.MyReviewSubjects = await getMyReviewSubjects();
            dynamicReviews.ReviewSubjects = await getReviewSubjects();
            
            return View(dynamicReviews);
        }

        /// <summary>
        /// GET: Review?id=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
        /// Gets review properties.
        /// </summary>  
        [AllowAnonymous]
        public async Task<IActionResult> Review(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }            

            TempData.Put("Review", review);

            return PartialView("_ContentPartial");
        }        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}