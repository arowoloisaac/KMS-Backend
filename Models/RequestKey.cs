using Key_Management_System.Enums;

namespace Key_Management_System.Models
{
    public class RequestKey
    {
        public Guid Id { get; set; }

        public Status Status { get; set; } = Status.Pending;

        public Activity Activity { get; set; } = Activity.Lecture;

        public Guid KeyId { get; set; }

        public Guid KeyCollectorId { get; set; }

        //public Guid WorkerId { get; set; }

        public Worker? Worker { get; set; }
    }
}
