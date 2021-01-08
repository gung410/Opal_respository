namespace Microservice.Content.Application.Events
{
    public class NotifyContentChangeEvent : BaseContentCommunicationEvent
    {
        public NotifyContentChangeEvent(string subject, string displayMessage, string userId)
        {
            Subject = subject;
            DisplayMessage = displayMessage;
            UserId = userId;
        }
    }
}
