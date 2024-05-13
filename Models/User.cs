using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Key_Management_System.Models
{
    public class User: IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
