namespace Microservice.WebinarAutoscaler.Domain.Enums
{
    public enum BBBServerStatus
    {
        /// <summary>
        /// New server was just created by the AWS scale group.
        /// </summary>
        New,

        /// <summary>
        /// Scalelite unregisters.
        /// </summary>
        Ready,
    }
}
