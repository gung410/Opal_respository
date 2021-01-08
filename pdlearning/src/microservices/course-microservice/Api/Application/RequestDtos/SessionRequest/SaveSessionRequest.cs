namespace Microservice.Course.Application.RequestDtos
{
    public class SaveSessionRequest
    {
        public SaveSessionDto Data { get; set; }

        public bool UpdatePreRecordClipOnly { get; set; }
    }
}
