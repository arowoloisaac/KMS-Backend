using Key_Management_System.Enums;

namespace Key_Management_System.DTOs.ThirdPartyDto
{
    public class ThirdPartyRequest
    {
        public Guid Id { get; set; }

        public Guid KeyId { get; set; }

        public string Name { get; set; } = string.Empty;

        public Activity Activity { get; set; }
    }
}
