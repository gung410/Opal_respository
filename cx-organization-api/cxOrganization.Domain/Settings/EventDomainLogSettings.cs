namespace cxOrganization.Domain.Settings
{
    public class EventDomainLogSettings
    {
        public bool Enable { get; set; }
        public string EventAPIBaseUrl { get; set; }
        public string EventAPIAuthorization { get; set; }
    }
}
