using Key_Management_System.Enums;

namespace Key_Management_System.DTOs
{
    public class KeyWith
    {
        public Guid CollectorId { get; set; }

        public string Room { get; set; } = string.Empty;

        public Activity Activity {  get; set; }

        public DateTime CollectionTime { get; set; }

        public DateTime AssignedTime { get; set; }
    }
}
