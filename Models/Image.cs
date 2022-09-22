using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReviewer.Models
{
    /// <summary>
    /// Properties of image to be uploaded.
    /// </summary>    
    public class Image : IBookImage
    {
        public Image ()
        {
            CreateDate = DateTime.Now;
        }
        
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "File Name")]
        [StringLength(100)]
        public string FileName { get; set; }

        public byte[] Bytes { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string FileExtension { get; set; }
        
        public decimal Size { get; set; }

        [Display(Name = "Create Date")]
        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; } 

        public Guid ReviewId { get; set; }

        [ForeignKey("ReviewId")]
        public Review Review { get; set; }        
    }
}