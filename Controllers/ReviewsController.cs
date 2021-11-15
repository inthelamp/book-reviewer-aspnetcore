using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookReviewer.Models;
using BookReviewer.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace BookReviewer.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly BookReviewerContext _context;
        private readonly IConfiguration _configuration;        

        public ReviewsController(BookReviewerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.Review.ToListAsync());
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            return View();
        }


        /// <summary>
        /// Creating a subject because there is no subject assigned to the draft version of the review.
        /// </summary>          
        private string makeSubject(string content)
        {
            string subject = content.Replace("\"ops\"", String.Empty).Replace("\"insert\"", String.Empty);
            subject = subject.Replace("\"attributes\"", String.Empty).Replace("\"underline\"", String.Empty);
            subject = subject.Replace("\"italic\"", String.Empty).Replace("\"bold\":true", String.Empty);     
            subject = subject.Replace("\"header\":1", String.Empty).Replace("\"header\":2", String.Empty).Replace("\"header\":3", String.Empty);                       
            subject = subject.Replace("[", String.Empty).Replace("]", String.Empty).Replace("\\n", String.Empty);
            subject = subject.Replace(":", String.Empty).Replace("\\", String.Empty).Replace("\"", String.Empty).Replace("{", String.Empty);
            subject = subject.Replace("},", String.Empty).Replace("}", String.Empty);
            int maxSubjectSize = Convert.ToInt16(_configuration["Application:SubjectMaxSize"]);

            if (subject.Length <= maxSubjectSize) {
                return subject;
            } else {
                return subject.Substring(0, maxSubjectSize-1);
            }
        }

        /// <summary>
        /// GET: Saving the draft version of the review when the user is not signed in.
        /// </summary>  
        public async Task<IActionResult> SaveDraft()
        {
            Review review = new Review();

            // Getting Json string from cookie
            string content = Request.Cookies["content"];

            if (!string.IsNullOrEmpty(content))
            {
                review.Content = content;

                // Saving review draft
                await SaveDraft(review);
            }

            return RedirectToAction("Index","Home");
        }        

        /// <summary>
        /// POST: Saving the draft version of the review when the user is signed in.
        /// </summary>  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDraft([Bind("Content")] Review review)
        {
            if (ModelState.IsValid)
            {
                review.Id = new Guid();
                ClaimsPrincipal currentUser = this.User;
                var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                review.UserId = currentUserID;

                review.Subject = makeSubject(review.Content);

                // Saving review draft
                _context.Add(review);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index","Home");
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(Guid? id)
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

        // GET: Editing review
        public async Task<IActionResult> Edit(string id)
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,Subject,BookTitle,IsbnNumber,BookCoverImage,Status,CreatedDate,UpdatedDate")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(review);
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
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(Guid id)
        {
            return _context.Review.Any(e => e.Id == id);
        }
    }
}
