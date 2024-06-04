using Key_Management_System.Enums;

namespace Key_Management_System.DTOs
{
    public class ViewUsage
    {
        public string RoomNumber { get; set; }

        public DateTime CollectionTime { get; set; }

        public Activity Activity { get; set; }

        public Status Status { get; set; }
    }
}
