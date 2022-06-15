using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using BookReviewer.CustomValidation;
using System.Text.Json.Serialization;

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
    [Index(nameof(IsbnNumber))]       
    [Index(nameof(Status))]       
    public class Review
    {
        public Review () 
        {
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
        }    

        [Key]
        public Guid Id { get; set; }

        [StringLength(100)]
        public string Subject { get; set; }

        [Display(Name = "Book Title")]
        [StringLength(100)]
        public string BookTitle { get; set; }

        [Display(Name = "Book Authors")]
        [StringLength(200)]
        public string BookAuthors { get; set; }      

        [Display(Name = "ISBN Number")]
        public string IsbnNumber { get; set; }

        [Display(Name = "Book Cover")]
        public byte[] BookCoverImage { get; set; }

        [Content]
        public string Content { get; set; }

        public ReviewStatus Status { get; set; }

        [Display(Name = "Create Date")]
        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }        

        [Display(Name = "Update Date")]
        [DataType(DataType.DateTime)]
        public DateTime UpdateDate { get; set; } 

        public string AuthorId { get; set; }
        
        [JsonIgnore] //Fixes JsonException: A possible object cycle was detected
        public Author Author { get; set; }
        
        public List<Image> Images { get; set; }            
    }
}