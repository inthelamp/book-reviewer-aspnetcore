using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReviewer.Models
{
    /// <summary>
    /// Properties of book author.
    /// </summary>
    public class Author
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Review")]
        public Guid ReviewId { get; set; } 

        [Display(Name = "First Name")]
        [StringLength(100)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(100)]        
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(100)]
        [Required]
        public string LastName { get; set; }
    }
}