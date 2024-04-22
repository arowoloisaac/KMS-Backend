using System.ComponentModel.DataAnnotations;

namespace Key_Management_System.DTOs.UserDto.SharedDto
{
    public interface LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }    
    }
}
