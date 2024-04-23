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
    }
}

//
//eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Inppa29AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvYXV0aGVudGljYXRpb24iOiJiZDExM2JhMy1jODczLTRkNGEtNjg3Ny0wOGRjNjM3YmU3NGYiLCJuYmYiOjE3MTM4NjYzMjksImV4cCI6MTcxMzg2OTkyOSwiaWF0IjoxNzEzODY2MzI5LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdCIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0LyJ9.teh481s3_rkp3DGE1H4w5EPLdHaQVY2f16QWHOjo-wQ