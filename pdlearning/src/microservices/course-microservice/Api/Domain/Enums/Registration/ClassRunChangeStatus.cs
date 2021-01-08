namespace Microservice.Course.Domain.Enums
{
    public enum ClassRunChangeStatus
    {
        PendingConfirmation,

        /// <summary>
        /// (approve or reject by AO in PDPM).
        /// </summary>
        Rejected,
        Approved,

        /// <summary>
        /// (ConfirmByCA or RejectedByCA by CA in CAM).
        /// </summary>
        ConfirmedByCA,
        RejectedByCA
    }
}
