using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReviewer.Models
{
    /// <summary>
    /// Properties of book review subject to be listed.
    /// </summary>
    [NotMapped]
    public class ReviewSubject
    {
        public Guid Id { get; set; }
        
        public string Subject { get; set; }
    }
}