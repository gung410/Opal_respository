namespace Microservice.Webinar.Application.Models
{
    public class ResultGetJoinUrlModel
    {
        public string JoinUrl { get; set; }

        public bool IsSuccess { get; set; }

        public string MessageCode { get; set; }

        public string Message { get; set; }
    }
}
