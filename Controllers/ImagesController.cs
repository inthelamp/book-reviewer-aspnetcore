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

namespace BookReviewer.Controllers
{
    public class ImagesController : Controller
    {
        private readonly DefaultDBContext _context;

        public ImagesController(DefaultDBContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetImage(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // fetch image data from database
            var image = await _context.Image
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            string imageType = "image/" + image.FileExtension.Substring(1);
            return  File(image.Bytes, imageType);
        }        

        // GET: Images
        public async Task<IActionResult> Index()
        {
            return View(await _context.Image.ToListAsync());
        }

        // GET: Images/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // GET: Images/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Images/Create
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid reviewId, string description, string menu, IFormFile imageFile)
        {
            var review = await _context.Review
                .FirstOrDefaultAsync(m => m.Id == reviewId);

            Image image = null;   
            if (imageFile != null && imageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);

                    // Uploading image with less than 2 MB
                    if (memoryStream.Length < 2097152)
                    {
                        // Creating Image instance.
                        image = new Image()
                        {
                            Bytes = memoryStream.ToArray(),
                            Description = description,
                            FileName = imageFile.FileName,
                            FileExtension = Path.GetExtension(imageFile.FileName),
                            Size = imageFile.Length, 
                            Review = review                        
                        };
                                                
                        _context.Image.Add(image);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Details", "Reviews", new { id = reviewId, menu = menu});                 
                    }
                    else
                    {
                        ModelState.AddModelError("BookCover", "The image is too large to save.");
                    }
                }
            }            

            return View(image);
        }        

        // POST: Images/Create
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("FileName,Bytes,Description,Size,CreateDate")] Image image)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         _context.Image.Add(image);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }
        //     return View(image);
        // }

        // GET: Images/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return View(image);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FileName,Bytes,Description,Size,CreateDate")] Image image)
        {
            if (id != image.Id)
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
                    if (!ImageExists(image.Id))
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
            return View(image);
        }

        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var image = await _context.Image.FindAsync(id);
            _context.Image.Remove(image);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(Guid id)
        {
            return _context.Image.Any(e => e.Id == id);
        }
    }
}
