namespace Conexus.Opal.BrokenLinkChecker
{
    public class LinkCheckStatus
    {
        public bool IsValid { get; set; }

        public string InvalidReason { get; set; }

        public static LinkCheckStatus InvalidUrl()
        {
            return new LinkCheckStatus { IsValid = false, InvalidReason = "An invalid URL has been entered." };
        }

        public static LinkCheckStatus ValidUrl()
        {
            return new LinkCheckStatus { IsValid = true };
        }
    }
}
