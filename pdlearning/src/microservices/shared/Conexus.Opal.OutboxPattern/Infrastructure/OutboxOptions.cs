namespace Conexus.Opal.OutboxPattern
{
    public class OutboxOptions
    {
        /// <summary>
        /// Number of messages to be sent at a time.
        /// </summary>
        public int SendBatchSize { get; set; }

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
