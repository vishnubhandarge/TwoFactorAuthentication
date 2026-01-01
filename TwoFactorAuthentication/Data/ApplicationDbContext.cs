using Microsoft.EntityFrameworkCore;
using TwoFactorAuthentication.Models;

namespace TwoFactorAuthentication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }
    }
}
