using System;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    /// <summary>
    /// Basic information of <see cref="MyClassRun"/>.
    /// </summary>
    public class MyClassRunBasicInfoModel
    {
        public Guid? CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public Guid? RegistrationId { get; set; }

        public RegistrationStatus? Status { get; set; }

        public WithdrawalStatus? WithdrawalStatus { get; set; }

        public ClassRunChangeStatus? ClassRunChangeStatus { get; set; }

        public RegistrationType? RegistrationType { get; set; }

        /// <summary>
        /// Filter to display apply again on client.
        /// </summary>
        /// <returns>Returns true if the status is
        /// Rejected
        /// or RejectedByCA
        /// or OfferRejected
        /// or WaitListRejected
        /// and the withdraw status is not Withdrawn.</returns>
        public bool IsRejected()
        {
            return
                (Status == RegistrationStatus.Rejected
                || Status == RegistrationStatus.RejectedByCA
                || Status == RegistrationStatus.OfferRejected
                || Status == RegistrationStatus.WaitlistRejected)
                && WithdrawalStatus != Domain.ValueObject.WithdrawalStatus.Withdrawn;
        }

        /// <summary>
        /// Filter to display withdraw reason and apply again button on client.
        /// </summary>
        /// <returns>Returns true if the withdraw status is Withdrawn.</returns>
        public bool IsWithdrawn()
        {
            return WithdrawalStatus == Domain.ValueObject.WithdrawalStatus.Withdrawn;
        }
    }
}
