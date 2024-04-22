using System.ComponentModel.DataAnnotations;

namespace Key_Management_System.DTOs.UserDto.KeyCollectorDto
{
    public class RegisterCollectorDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage ="Must be valid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}

