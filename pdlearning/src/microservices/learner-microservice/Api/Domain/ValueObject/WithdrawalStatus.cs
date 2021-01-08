namespace Microservice.Learner.Domain.ValueObject
{
    public enum WithdrawalStatus
    {
        PendingConfirmation,

        /// <summary>
        /// (approve or reject by AO in PDPM).
        /// </summary>
        Rejected,
        Approved,

        /// <summary>
        /// (confirmed or rejected by CA in CAM).
        /// </summary>
        Withdrawn,
        RejectedByCA
    }
}
