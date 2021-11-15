using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReviewer.Models
{
    /// <summary>
    /// Statuses of book review.
    /// </summary>   
    public enum ReviewStatus 
     {
        Draft,
        Published,    
        Inactive
    }

    /// <summary>
    /// Properties of book review to be posted by user.
    /// </summary>   
    public class Review
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")]        
        [StringLength(450)]
        public string UserId { get; set; } 
        
        [StringLength(100)]
        public string Subject { get; set; }

        [Display(Name = "Book Title")]
        [StringLength(100)]
        public string BookTitle { get; set; }

        [Display(Name = "Book Authors")]
        public ICollection<Author> BookAuthors { get; set; }      

        [Display(Name = "ISBN Number")]
        public string IsbnNumber { get; set; }

        [Display(Name = "Book Cover")]
        public byte[] BookCoverImage { get; set; }

        public ICollection<Image> Images { get; set; }

        //JSON serialize string of Quill
        public string Content { get; set; }

        public ReviewStatus Status { get; set; }

        [Display(Name = "Create Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }        

        [Display(Name = "Update Date")]
        [DataType(DataType.Date)]
        public DateTime UpdatedDate { get; set; }
    }
}