using System.ComponentModel.DataAnnotations;

namespace Key_Management_System.DTOs.UserDto.WorkerDto
{
    public class RegisterWorkerDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage ="Invalid email address")]
        [Required]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage ="invalid phone number")]
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Faculty { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
