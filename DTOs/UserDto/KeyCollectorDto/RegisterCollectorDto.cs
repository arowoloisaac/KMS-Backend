using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Key_Management_System.DTOs.UserDto.KeyCollectorDto
{
    public class RegisterCollectorDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage ="invalid email address, e.g doe@gmail.ru")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage ="invalid phone number")]
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}

