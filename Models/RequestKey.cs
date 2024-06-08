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
        
        //this can be either when the worker reject or accepts the request
        public DateTime AssignedTime { get; set; }

        public DateTime ReturnedTime { get; set; }

        public Guid GetKeyId { get; set; }

        public Key? Key { get; set; }

        public Guid KeyCollectorId { get; set; }

        // to set the id of the accepted user
        public Guid GetWorkerId { get; set; }

        //updated in here
        //public ? WorkerId { get; set; }

        public ICollection<ThirdParty>? ThirdParty { get; set; }

    }
}
