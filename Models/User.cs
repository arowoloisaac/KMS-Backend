using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Key_Management_System.Models
{
    public class User: IdentityUser<Guid>
    {
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
