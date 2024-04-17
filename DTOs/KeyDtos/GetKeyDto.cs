using Key_Management_System.Enums;

namespace Key_Management_System.DTOs.KeyDtos
{
    public class GetKeyDto
    {
        public Guid Id { get; set; }

        public string Room {  get; set; } = string.Empty;

        public KeyStatus Status { get; set; } = KeyStatus.Available;
    }
}
