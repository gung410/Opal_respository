namespace Conexus.Opal.InboxPattern.Infrastructure
{
    public class InboxOptions
    {
        /// <summary>
        /// The configuration to indicate the number of days before removing a message with SENT status.
        /// </summary>
        public int DeleteMessageAfterDays { get; set; }

        /// <summary>
        /// Number of messages to be deleted at a time.
        /// </summary>
        public int DeleteBatchSize { get; set; }
    }
}
