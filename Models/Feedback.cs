using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace BookReviewer.Models
{
    /// <summary>
    /// Properties of Comment.
    /// </summary>

    public class Feedback
    {
        public Feedback()
        {
            UpdateDate = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; }

        public string UserId { get; set; } = null!;

        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        public DateTime UpdateDate { get; set; } 

        public Guid ReviewId { get; set; }

        [ForeignKey("ReviewId")]
        public Review Review { get; set; }       
    }
}