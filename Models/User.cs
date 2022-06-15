using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookReviewer.Models
{  
    /// <summary>
    /// Additional properties of user.
    /// </summary>
    public class User : IdentityUser
    {
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
    }
}