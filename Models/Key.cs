using Key_Management_System.Enums;

namespace Key_Management_System.Models
{
    public class Key
    {
        public Guid Id { get; set; } 

        public string Room { get; set; } = string.Empty;

        public KeyStatus Status { get; set; } = KeyStatus.Available;

        public ICollection<RequestKey>? RequestKeys { get; set; }

        public ICollection<ThirdParty>? ThirdParties { get; set; }

        //foreign key
        //public Guid WorkerId { get; set; }  

        //public Worker? Worker { get; set; }

    }
}
