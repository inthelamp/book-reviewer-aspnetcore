using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;  

namespace BookReviewer.Models
{
    /// <summary>
    /// Properties of BookCover to be uploaded.
    /// </summary>    
    public class BookCover
    {
        public BookCover ()
        {
            CreateDate = DateTime.Now;
        }
        
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "File Name")]
        [StringLength(100)]
        public string FileName { get; set; }

        public byte[] Bytes { get; set; }

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