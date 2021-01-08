namespace Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads
{
    public class LearnerActivityResumePayload
    {
        public bool LoginFromMobile { get; set; }

        public string ClientId { get; set; }
    }
}
