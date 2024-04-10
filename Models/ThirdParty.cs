namespace Key_Management_System.Models
{
    public class ThirdParty
    {
        public Guid Id { get; set; }

        public Guid CurrentHolder { get; set; }

        public Guid KeyId { get; set; }

        public Guid KeyCollectorId { get; set; }

        public RequestKey? RequestKey { get; set; }
    }
}
