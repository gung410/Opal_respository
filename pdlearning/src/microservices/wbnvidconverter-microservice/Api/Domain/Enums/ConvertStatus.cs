namespace Microservice.WebinarVideoConverter.Domain.Enums
{
    public enum ConvertStatus
    {
        /// <summary>
        /// A new playback folder found.
        /// </summary>
        New,

        /// <summary>
        /// In process of convert from playback to mp4 file.
        /// </summary>
        Converting,

        /// <summary>
        /// Convert from playback into mp4 successfully.
        /// </summary>
        Converted,

        /// <summary>
        /// The action convert failed and in progress of retrying.
        /// </summary>
        Failed,

        /// <summary>
        /// In process of uploading to S3.
        /// </summary>
        Uploading,

        /// <summary>
        /// Could not convert the record.
        /// </summary>
        IgnoreRetry,

        /// <summary>
        /// Record is ready for use.
        /// </summary>
        Ready
    }
}
