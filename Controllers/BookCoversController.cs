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
    public class BookCoversController : Controller
    {

        private readonly DefaultDBContext _context;

        public BookCoversController(DefaultDBContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetBookCover(Guid? reviewId)
        {
            if (reviewId == null)
            {
                return NotFound();
            }

            // fetch image data from database
            var bookCover = await _context.BookCover
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.ReviewId == reviewId);

            if (bookCover != null) {
                string imageType = "image/" + bookCover.FileExtension.Substring(1);
                return  File(bookCover.Bytes, imageType);
            } 
            else
            {
                string defaultBookCover = "images/no_book_cover.png";
                return File(defaultBookCover, "image/png");
            }   
        }

    }
}