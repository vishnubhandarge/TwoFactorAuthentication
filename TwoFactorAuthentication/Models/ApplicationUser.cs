using System.ComponentModel.DataAnnotations;

namespace TwoFactorAuthentication.Models
{
    public class ApplicationUser
    {
        [Key]
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string TwoFactorSecret { get; set; }
    }
}
