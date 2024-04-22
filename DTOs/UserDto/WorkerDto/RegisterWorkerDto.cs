using System.ComponentModel.DataAnnotations;

namespace Key_Management_System.DTOs.UserDto.WorkerDto
{
    public class RegisterWorkerDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Faculty { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
