using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookReviewer.Models;
using BookReviewer.Data;
using BookReviewer.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookReviewer.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly DefaultDBContext _context;
        private readonly IConfiguration _configuration;      

        public ReviewsController(DefaultDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [TempData]
        public string Message { get; set; }

        /// <summary>
        /// Change status of the review 
        /// </summary>  
        public async Task<IActionResult> ChangeStatus(Guid? id, ReviewStatus status)
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
      
            review.Status = status;
            await _context.SaveChangesAsync();

            TempData.Put("Review", review);     

            return RedirectToAction("Index","Home");
        }

        // GET: Reviews
        /// <summary>
        /// Gets the reviews published
        /// </summary>  
        [AllowAnonymous]
        public async Task<IActionResult> GetReviews()
        {
            var reviews = await _context.Review
                        .Where(r => r.Status == ReviewStatus.Published) 
                        .ToListAsync();

            ViewBag.Menu = "Reviews";
            return View("Index", reviews);
        }


        // GET: Reviews
        /// <summary>
        /// Gets the reviews the user wrote
        /// </summary>  
        public async Task<IActionResult> GetMyReviews()
        {            
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        
            var myReviews = await _context.Review
                        .Where(r => r.ReviewerId == userId) 
                        .ToListAsync();

            ViewBag.Menu = "My Reviews";
            return View("Index", myReviews);
        }        
              
        /// <summary>
        /// Creates a subject because there is no subject assigned to the draft version of review.
        /// </summary>          
        private string createSubject(string content)
        {
            string subject = RichEditorHelper.GetContentText(content);
            int maxSubjectSize = Convert.ToInt16(_configuration["Application:SubjectMaxSize"]);

            if (subject.Length <= maxSubjectSize) {
                return subject;
            } else {
                return subject.Substring(0, maxSubjectSize-1);
            }
        }

        /// <summary>
        /// Gets reviewer 
        /// </summary>  
        private async Task<Reviewer> getReviewer(string userId)
        {
            var reviewer = await _context.Reviewer
                .FirstOrDefaultAsync(a => a.Id == userId);

            if (reviewer == null)
            {
                reviewer = new Reviewer();
                reviewer.Id = userId;

                // Saving Reviewer
                _context.Reviewer.Add(reviewer);
                await _context.SaveChangesAsync();                
            }

            return reviewer;
        }

        /// <summary>
        /// Saves review
        /// </summary>  
        private async Task save(Review review)
        {
            if (string.IsNullOrEmpty(review.ReviewerId)) 
            {                
                ClaimsPrincipal currentUser = this.User;
                string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;       

                review.Reviewer = await getReviewer(userId);     
            }

            // Review properties
            review.Subject = createSubject(review.Content);
            if (!_context.Review.Contains(review))
            {
                _context.Review.Add(review);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// GET: /Reviews/SaveDraft
        /// Saves the draft version of review if user is not signed in.
        /// </summary>  
        [HttpGet]    
        public async Task<IActionResult> SaveDraft()
        {
            // Getting Json string from cookie
            string content = Request.Cookies["content"];

            Review review = new Review();    
            review.Content = content;

            // Checks the validation of the review for the content assigned
            if (TryValidateModel(review))
            {
                await save(review);

                TempData.Put("Review", review);                
                Message = "The review is successfully saved.";          
            }
            else
            {
                Message = ModelState.GetModelErrorMessages();
            }            

            return RedirectToAction("Index","Home");
        }          

        /// <summary>
        /// POST: /Reviews/SaveDraft
        /// Saves the draft version of review if user is signed in.
        /// </summary>  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDraft([Bind("Content")] Review review)
        {
            if (ModelState.IsValid)
            {
                await save(review);

                TempData.Put("Review", review);       
                Message = "The review is successfully saved.";                
            }
            else
            {
                Message = ModelState.GetModelErrorMessages();
            }

            return RedirectToAction("Index","Home");
        }

        /// <summary>
        /// POST: /Reviews/Update
        /// Updates review
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]          
        public async Task<IActionResult> Update(Guid id, [FromForm] Review reviewUpdated)
        {
            if (id != reviewUpdated.Id)
            {
                return NotFound();
            }

            var review = await _context.Review
                .FirstOrDefaultAsync(m => m.Id == reviewUpdated.Id);

            if (review == null)
            {
                Message = "No review is found.";

                return RedirectToAction("Index","Home");
            }                            

            bool isChanged = false;

            if (review.Content != reviewUpdated.Content)
            {
                review.Content = reviewUpdated.Content;
                isChanged = true;
            }        

            if (review.Subject != reviewUpdated.Subject) 
            {
                review.Subject = reviewUpdated.Subject;
                isChanged = true;                
            } 

            if (review.BookTitle != reviewUpdated.BookTitle)
            {
                review.BookTitle = reviewUpdated.BookTitle;
                isChanged = true;                  
            }          
                          
            if (review.BookAuthors != reviewUpdated.BookAuthors) 
            {
                review.BookAuthors = reviewUpdated.BookAuthors;
                isChanged = true;                 
            } 

            BookCover bookCover = null;   
            var bookCoverFile = reviewUpdated.BookCoverFile;
            if (bookCoverFile != null && bookCoverFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await bookCoverFile.CopyToAsync(memoryStream);

                    // Uploading image with less than 2 MB
                    if (memoryStream.Length < 2097152)
                    {
                        _context.Entry(review).Reference(b => b.BookCover).Load();
                        if (review.BookCover != null) 
                        {
                            bookCover = review.BookCover;
                            bookCover.Bytes = memoryStream.ToArray();
                            bookCover.FileName = bookCoverFile.FileName;
                            bookCover.FileExtension = Path.GetExtension(bookCoverFile.FileName);
                            bookCover.Size = bookCoverFile.Length; 
                        }    
                        else 
                        {
                            // Creating Image instance.
                            bookCover = new BookCover()
                            {
                                Bytes = memoryStream.ToArray(),
                                FileName = bookCoverFile.FileName,
                                FileExtension = Path.GetExtension(bookCoverFile.FileName),
                                Size = bookCoverFile.Length, 
                                Review = review                        
                            };
                                                        
                            review.BookCover = bookCover;
                            _context.BookCover.Add(bookCover);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("BookCover", "The image is too large to save.");
                    }
                }

                isChanged = true;
            }

            if (!isChanged) 
            {
                TempData.Put("Review", review);
                Message = "No change is made.";                

                return RedirectToAction("Index","Home");
            } 

            // Checks the validation of the review for the content assigned
            if (TryValidateModel(review))
            {
                review.UpdateDate = DateTime.Now;
                               
                await _context.SaveChangesAsync();
                
                Message = "The review is successfully updated.";          
            }
            else
            {
                Message = ModelState.GetModelErrorMessages();
            }

            TempData.Put("Review", review);
            return RedirectToAction("Index","Home");
        }  

        // GET: Reviews/Create
        public IActionResult Create()
        {
            return View();
        }         

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Subject,BookTitle,BookAuthors,IsbnNumber,Content,Status,BookCoverFile")] Review review)
        {
            if (ModelState.IsValid)
            {
                BookCover bookCover = null;   
                var bookCoverFile = review.BookCoverFile;
                if (bookCoverFile != null && bookCoverFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await bookCoverFile.CopyToAsync(memoryStream);

                        // Uploading image with less than 2 MB
                        if (memoryStream.Length < 2097152)
                        {
                            // Creating Image instance.
                            bookCover = new BookCover()
                            {
                                Bytes = memoryStream.ToArray(),
                                FileName = bookCoverFile.FileName,
                                FileExtension = Path.GetExtension(bookCoverFile.FileName),
                                Size = bookCoverFile.Length, 
                                Review = review                        
                            };
                                                        
                            review.BookCover = bookCover;
                            _context.BookCover.Add(bookCover);
                        }
                        else
                        {
                            ModelState.AddModelError("BookCover", "The image is too large to save.");
                        }
                    }
                }

                ClaimsPrincipal currentUser = this.User;
                string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

                review.Reviewer = await getReviewer(userId);
                _context.Review.Add(review);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(GetMyReviews));
            }
            return View(review);
        }    

        // GET: Reviews/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id, string menu)
        {
            if (id == null)
            {
                return RedirectToAction("Index","Home");
            }

            Review review = null;
            if (menu == "My Reviews")
            {
                ClaimsPrincipal currentUser = this.User;
                if (currentUser.Identity.IsAuthenticated)
                {
                    string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

                    review = await _context.Review
                        .FirstOrDefaultAsync(m => m.Id == id && m.ReviewerId == userId);

                    ViewBag.Menu = "My Reviews";
                }
            }
            else
            {
                review = await _context.Review
                    .FirstOrDefaultAsync(m => m.Id == id);

                 ViewBag.Menu = "Reviews";
            }

            if (review == null)
            {
                return RedirectToAction("Index","Home");
            }

            return View(review);
        }    

        // GET: Editing review
        public async Task<IActionResult> Edit(Guid? id)
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
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] Review reviewUpdated)
        {
            if (id != reviewUpdated.Id)
            {
                return NotFound();
            }

            var review = await _context.Review
                .FirstOrDefaultAsync(m => m.Id == reviewUpdated.Id);    

            if (ModelState.IsValid)
            {
                bool isChanged = false;

                if (reviewUpdated.Content != null && review.Content != reviewUpdated.Content)
                {
                    review.Content = reviewUpdated.Content;
                    isChanged = true;
                }        

                if (reviewUpdated.Subject != null && review.Subject != reviewUpdated.Subject) 
                {
                    review.Subject = reviewUpdated.Subject;
                    isChanged = true;                
                } 

                if (reviewUpdated.BookTitle != null && review.BookTitle != reviewUpdated.BookTitle)
                {
                    review.BookTitle = reviewUpdated.BookTitle;
                    isChanged = true;                  
                }          
                            
                if (reviewUpdated.BookAuthors != null && review.BookAuthors != reviewUpdated.BookAuthors) 
                {
                    review.BookAuthors = reviewUpdated.BookAuthors;
                    isChanged = true;                 
                } 

                if (review.Status != reviewUpdated.Status) 
                {
                    review.Status = reviewUpdated.Status;
                    isChanged = true;                 
                }                 

                BookCover bookCover = null;   
                var bookCoverFile = reviewUpdated.BookCoverFile;
                if (bookCoverFile != null && bookCoverFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await bookCoverFile.CopyToAsync(memoryStream);

                        // Uploading image with less than 2 MB
                        if (memoryStream.Length < 2097152)
                        {
                            _context.Entry(review).Reference(b => b.BookCover).Load();
                            if (review.BookCover != null) 
                            {
                                bookCover = review.BookCover;
                                bookCover.Bytes = memoryStream.ToArray();
                                bookCover.FileName = bookCoverFile.FileName;
                                bookCover.FileExtension = Path.GetExtension(bookCoverFile.FileName);
                                bookCover.Size = bookCoverFile.Length; 
                            }    
                            else 
                            {
                                // Creating Image instance.
                                bookCover = new BookCover()
                                {
                                    Bytes = memoryStream.ToArray(),
                                    FileName = bookCoverFile.FileName,
                                    FileExtension = Path.GetExtension(bookCoverFile.FileName),
                                    Size = bookCoverFile.Length, 
                                    Review = review                        
                                };
                                                            
                                review.BookCover = bookCover;
                                _context.BookCover.Add(bookCover);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("BookCover", "The image is too large to save.");
                        }
                    }

                    isChanged = true;
                }

                if (!isChanged) 
                {
                    Message = "No change is made.";                

                    return View(reviewUpdated);
                } 

                try
                {
                    review.UpdateDate = DateTime.Now;
                                
                    await _context.SaveChangesAsync();
                    
                    Message = "The review is successfully updated.";                          
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetMyReviews));
            }
            return View(reviewUpdated);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var review = await _context.Review.FindAsync(id);

            _context.Review.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetMyReviews));
        }

        private bool ReviewExists(Guid id)
        {
            return _context.Review.Any(e => e.Id == id);
        }
    }
}
