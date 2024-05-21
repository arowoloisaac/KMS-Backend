using System.ComponentModel.DataAnnotations;

namespace Key_Management_System.DTOs.UserDto.SharedDto
{
    public class LoginDto
    {
        [EmailAddress(ErrorMessage ="Must be email")]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
