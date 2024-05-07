using Key_Management_System.Enums;

namespace Key_Management_System.Models
{
    public class ThirdParty
    {
        public Guid Id { get; set; }

        //the key is currently with this person
        public Guid CurrentHolder { get; set; }

        public Guid KeyId { get; set; }

        public Activity Activity { get; set; } = Activity.Lecture;

        //public General General { get; set; }
        public TPRequest Request { get; set; }

        //the person who request the key from currentholder
        public Guid KeyCollectorId { get; set; }

        public RequestKey? RequestKey { get; set; }
    }
}
