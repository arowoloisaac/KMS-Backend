using Key_Management_System.Enums;

namespace Key_Management_System.DTOs.AssignDto
{
    public class KeyCollectorRequest
    {

        public Guid KeyId { get; set; }

        public string? Room { get; set; }
    }
}
