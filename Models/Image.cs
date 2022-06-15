using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;  

namespace BookReviewer.Models
{
    /// <summary>
    /// Properties of image to be uploaded.
    /// </summary>    
    public class Image
    {
        public Image ()
        {
            CreateDate = DateTime.Now;
        }
        
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Image Name")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Subject { get; set; }
        
        [StringLength(500)]
        public string Note { get; set; }

        [Display(Name = "Image File")]
        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Size (bytes)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long Size { get; set; }

        [Display(Name = "Create Date")]
        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; } 

        public Guid ReviewId { get; set; }
        public Review Review { get; set; }        
    }
}