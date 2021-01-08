namespace Microservice.Webinar.Application.Services.BigBlueButton
{
    public class BigBlueButtonServerOptions
    {
        public string CloudfrontUrl { get; set; }

        public string WebinarUrl { get; set; }

        public string BBBSecretKey { get; set; }

        public string ProxyUrl { get; set; }

        public string ProxySecretKey { get; set; }

        public string AttendeePassword { get; set; }

        public string ModeratorPassword { get; set; }

        public string WaitingServerPageUrl { get; set; }

        public string LogoutUrl { get; set; }
    }
}
