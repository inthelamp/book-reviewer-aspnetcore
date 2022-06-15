using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BookReviewer.Models
{
    /// <summary>
    /// Properties of book author.
    /// </summary>

    public class Author
    {
        /// <summary>
        /// Id is UserId from User.
        /// </summary>
        [Key]
        public string Id { get; set; }

        [Display(Name = "First Name")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(100)]        
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(100)]
        public string LastName { get; set; }

        public List<Review> Reviews { get; set; }
    }
}