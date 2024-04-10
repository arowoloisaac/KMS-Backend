namespace Key_Management_System.Models
{
    public class KeyCollector : User
    {
        public ICollection<RequestKey>? RequestKeys { get; set; }    

        public ICollection<ThirdParty>? ThirdParties { get; set; }
    }
}
