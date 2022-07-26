using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookReviewer.Models;
using BookReviewer.Data;

namespace BookReviewer.Controllers
{
    public class ReviewersController : Controller
    {
        private readonly DefaultDBContext _context;

        public ReviewersController(DefaultDBContext context)
        {
            _context = context;
        }

        // GET: Reviewers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Reviewer.ToListAsync());
        }

        // GET: Reviewers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviewer = await _context.Reviewer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reviewer == null)
            {
                return NotFound();
            }

            return View(reviewer);
        }

        // GET: Reviewers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reviewers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,MiddleName,LastName")] Reviewer reviewer)
        {
            if (ModelState.IsValid)
            {
                _context.Reviewer.Add(reviewer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reviewer);
        }

        // GET: Reviewers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviewer = await _context.Reviewer.FindAsync(id);
            if (reviewer == null)
            {
                return NotFound();
            }
            return View(reviewer);
        }

        // POST: Reviewers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,MiddleName,LastName")] Reviewer reviewer)
        {
            if (id != reviewer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewerExists(reviewer.Id))
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
            return View(reviewer);
        }

        // GET: Reviewers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviewer = await _context.Reviewer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reviewer == null)
            {
                return NotFound();
            }

            return View(reviewer);
        }

        // POST: Reviewers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var reviewer = await _context.Reviewer.FindAsync(id);
            _context.Reviewer.Remove(reviewer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewerExists(string id)
        {
            return _context.Reviewer.Any(a => a.Id == id);
        }
    }
}
