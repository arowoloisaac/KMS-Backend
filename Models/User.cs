using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Key_Management_System.Models
{
    public class User: IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage ="Invalid Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage ="Invalid Phone Number")]
        public string PhoneNumber {  get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
