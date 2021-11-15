using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookReviewer.Models
{  
    /// <summary>
    /// Properties of user.
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// Pen name of user
        /// </summary>        
        [Display(Name = "Pen Name")]
        [StringLength(200)]
        public string PenName { get; set; }
    }
}