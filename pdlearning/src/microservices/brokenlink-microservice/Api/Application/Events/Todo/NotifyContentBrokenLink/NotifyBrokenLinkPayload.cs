namespace Microservice.BrokenLink.Application.Events
{
    public class NotifyBrokenLinkPayload
    {
        public string AssetOwnerName { get; set; }

        public string AssetName { get; set; }

        public string AssetDetailUrl { get; set; }

        public string ActionName { get; set; }

        public string ActionUrl { get; set; }

        public string ReporterName { get; set; }
    }
}
