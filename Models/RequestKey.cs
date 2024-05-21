using Key_Management_System.Enums;

namespace Key_Management_System.Models
{
    public class RequestKey
    {
        public Guid Id { get; set; }

        public Status Status { get; set; } = Status.Pending;

        public string _Key { get; set; } = string.Empty;

        public Activity Activity { get; set; } = Activity.Lecture;

        public CheckWith Availability {  get; set; }

        public DateTime CollectionTime { get; set; }    

        public DateTime ReturnedTime { get; set; }

        //public Guid KeyId { get; set; }

        public Key? Key { get; set; }

        public Guid KeyCollectorId { get; set; }

        //public Guid WorkerId { get; set; }

        public Worker? Worker { get; set; }

        public ICollection<ThirdParty>? ThirdParty { get; set; }

    }
}
