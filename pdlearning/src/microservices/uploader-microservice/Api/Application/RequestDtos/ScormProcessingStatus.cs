namespace Microservice.Uploader.Application.RequestDtos
{
    public enum ScormProcessingStatus
    {
        Failure,
        Extracting,
        ExtractingFailure,
        Processing,
        Completed,
        Timeout,
        Invalid,
    }
}
