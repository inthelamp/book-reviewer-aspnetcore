using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
 
namespace BookReviewer.Data
{
    public class UserContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public UserContext(DbContextOptions<UserContext> options)
           : base(options)
        {
        }

        public DbSet<BookReviewer.Models.User> User { get; set; }
    }
}