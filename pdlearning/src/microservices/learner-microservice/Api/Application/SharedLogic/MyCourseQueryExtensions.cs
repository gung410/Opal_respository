using Microservice.Learner.Application.Common;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.SharedLogic
{
    internal static class MyCourseQueryExtensions
    {
        /// <summary>
        /// Notify users that their registration is in which step in the workflow.
        /// </summary>
        /// <param name="myCourse">My course entity.</param>
        /// <param name="registrationStatus">
        /// Registration status as value object <see cref="RegistrationStatus"/>.</param>
        /// <param name="registrationType">Registration type as value object.</param>
        public static void SetDisplayStatus(
            this MyCourse myCourse,
            RegistrationStatus registrationStatus,
            RegistrationType registrationType)
        {
            var displayStatus = DisplayStatus.PendingConfirmation;
            switch (registrationType)
            {
                case RegistrationType.Manual:
                    displayStatus =
                        DisplayStatusMapper.MapFromRegistrationStatus(registrationStatus);
                    break;

                case RegistrationType.AddedByCA:
                    displayStatus =
                        DisplayStatusMapper.MapFromAddedByCAStatus(registrationStatus);
                    break;

                case RegistrationType.Nominated:
                    displayStatus =
                        DisplayStatusMapper.MapFromNominatedStatus(registrationStatus);
                    break;

                case RegistrationType.None:
                    break;

                case RegistrationType.Application:
                    break;

                default:
                    throw new UnexpectedStatusException($"{registrationType}");
            }

            myCourse.SetDisplayStatus(displayStatus);
        }

        /// <summary>
        /// Notify users that their change class is in which step in the workflow.
        /// </summary>
        /// <param name="myCourse">MyCourse entity.</param>
        /// <param name="classRunChangeStatus">
        /// Class run change status as value object <see cref="ClassRunChangeStatus"/>.</param>
        public static void SetClassRunChangeDisplayStatus(
            this MyCourse myCourse,
            ClassRunChangeStatus? classRunChangeStatus)
        {
            if (!classRunChangeStatus.HasValue)
            {
                return;
            }

            myCourse.SetDisplayStatus(
                DisplayStatusMapper.MapFromClassRunChangeStatus(classRunChangeStatus.Value));
        }

        /// <summary>
        /// Notify users that their withdraw is in which step in the workflow.
        /// </summary>
        /// <param name="myCourse">MyCourse entity.</param>
        /// <param name="withdrawalStatus">
        /// Withdrawal status as value object <see cref="WithdrawalStatus"/>.</param>
        public static void SetWithdrawalDisplayStatus(
            this MyCourse myCourse,
            WithdrawalStatus? withdrawalStatus)
        {
            if (!withdrawalStatus.HasValue)
            {
                return;
            }

            myCourse.SetDisplayStatus(
                DisplayStatusMapper.MapFromWithdrawalStatus(withdrawalStatus.Value));
        }
    }
}
