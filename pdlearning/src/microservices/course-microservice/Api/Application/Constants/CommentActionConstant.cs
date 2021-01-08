using System;
using System.Collections.Generic;
using Microservice.Course.Application.Enums;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Constants
{
    public class CommentActionConstant
    {
        public static readonly string CoursePrefix = "course";
        public static readonly string CourseContentPrefix = "course-content";
        public static readonly string ClassRunPrefix = "classrun";
        public static readonly string ClassRunContentPrefix = "classrun-content";
        public static readonly string RegistrationPrefix = "registration";
        public static readonly string ParticipantAssignmentTrackQuizAnswerPrefix = "participant-assignment-track-quiz-answer";

        public static readonly string CourseReply = $"{CoursePrefix}-reply";
        public static readonly string CourseApproved = $"{CoursePrefix}-approved";
        public static readonly string CourseRejected = $"{CoursePrefix}-rejected";

        public static readonly string ClassRunReply = $"{ClassRunPrefix}-reply";
        public static readonly string ClassRunCancellationPendingApproval = $"{ClassRunPrefix}-cancellation-pending-approval";
        public static readonly string ClassRunCancellationApproved = $"{ClassRunPrefix}-cancellation-approved";
        public static readonly string ClassRunCancellationRejected = $"{ClassRunPrefix}-cancellation-rejected";
        public static readonly string ClassRunReschedulePendingApproval = $"{ClassRunPrefix}-reschedule-pending-approval";
        public static readonly string ClassRunRescheduleApproved = $"{ClassRunPrefix}-reschedule-approved";
        public static readonly string ClassRunRescheduleRejected = $"{ClassRunPrefix}-reschedule-rejected";

        public static readonly string CourseContentReply = $"{CourseContentPrefix}-reply";
        public static readonly string CourseContentApproved = $"{CourseContentPrefix}-approved";
        public static readonly string CourseContentRejected = $"{CourseContentPrefix}-rejected";

        public static readonly string ClassRunContentReply = $"{ClassRunContentPrefix}-reply";
        public static readonly string ClassRunContentApproved = $"{ClassRunContentPrefix}-approved";
        public static readonly string ClassRunContentRejected = $"{ClassRunContentPrefix}-rejected";

        public static readonly string RegistrationApproved = $"{RegistrationPrefix}-approved";
        public static readonly string RegistrationRejected = $"{RegistrationPrefix}-rejected";
        public static readonly string RegistrationWaitlistConfirmed = $"{RegistrationPrefix}-waitlist-confirmed";
        public static readonly string RegistrationConfirmedByCA = $"{RegistrationPrefix}-confirmed-by-ca";
        public static readonly string RegistrationRejectedByCA = $"{RegistrationPrefix}-rejected-by-ca";
        public static readonly string RegistrationWaitlistPendingApprovalByLearner = $"{RegistrationPrefix}-waitlist-pending-approval-by-learner";
        public static readonly string RegistrationClassRunChangePendingConfirmation = $"{RegistrationPrefix}-classrun-change-pending-confirmation";
        public static readonly string RegistrationClassRunChangeApproved = $"{RegistrationPrefix}-classrun-change-approved";
        public static readonly string RegistrationClassRunChangeRejected = $"{RegistrationPrefix}-classrun-change-rejected";
        public static readonly string RegistrationClassRunChangeConfirmedByCA = $"{RegistrationPrefix}-classrun-change-confirmed-by-ca";
        public static readonly string RegistrationClassRunChangeRejectedByCA = $"{RegistrationPrefix}-classrun-change-rejected-by-ca";
        public static readonly string RegistrationWithdrawnApproved = $"{RegistrationPrefix}-withdrawn-approved";
        public static readonly string RegistrationWithdrawnRejected = $"{RegistrationPrefix}-withdrawn-rejected";
        public static readonly string RegistrationWithdrawnPendingConfirmation = $"{RegistrationPrefix}-withdrawn-pending-confirmation";
        public static readonly string RegistrationWithdrawnConfirmedByCA = $"{RegistrationPrefix}-withdrawn-confirmed-by-ca";
        public static readonly string RegistrationWithdrawnRejectedByCA = $"{RegistrationPrefix}-withdrawn-rejected-by-ca";

        public static readonly string ParticipantAssignmentTrackQuizAnswerFeedback =
            $"{ParticipantAssignmentTrackQuizAnswerPrefix}-reply";

        public static readonly Dictionary<Enum, string> RegistrationActionDict = new Dictionary<Enum, string>()
        {
            {
                RegistrationStatus.ConfirmedByCA,
                RegistrationConfirmedByCA
            },
            {
                RegistrationStatus.RejectedByCA,
                RegistrationRejectedByCA
            },
            {
                RegistrationStatus.Approved,
                RegistrationApproved
            },
            {
                RegistrationStatus.Rejected,
                RegistrationRejected
            },
            {
                RegistrationStatus.WaitlistConfirmed,
                RegistrationWaitlistConfirmed
            },
            {
                RegistrationStatus.WaitlistPendingApprovalByLearner,
                RegistrationWaitlistPendingApprovalByLearner
            },
            {
                ClassRunChangeStatus.PendingConfirmation,
                RegistrationClassRunChangePendingConfirmation
            },
            {
                ClassRunChangeStatus.Approved,
                RegistrationClassRunChangeApproved
            },
            {
                ClassRunChangeStatus.Rejected,
                RegistrationClassRunChangeRejected
            },
            {
                ClassRunChangeStatus.ConfirmedByCA,
                RegistrationClassRunChangeConfirmedByCA
            },
            {
                ClassRunChangeStatus.RejectedByCA,
                RegistrationClassRunChangeRejectedByCA
            },
            {
                WithdrawalStatus.Approved,
                RegistrationWithdrawnApproved
            },
            {
                WithdrawalStatus.Rejected,
                RegistrationWithdrawnRejected
            },
            {
                WithdrawalStatus.PendingConfirmation,
                RegistrationWithdrawnPendingConfirmation
            },
            {
                WithdrawalStatus.Withdrawn,
                RegistrationWithdrawnConfirmedByCA
            },
            {
                WithdrawalStatus.RejectedByCA,
                RegistrationWithdrawnRejectedByCA
            },
            {
                DefaultEnum.None,
                RegistrationPrefix
            }
        };

        public static readonly Dictionary<Enum, string> CourseActionDict = new Dictionary<Enum, string>()
        {
            {
                CourseStatus.Approved,
                CourseApproved
            },
            {
                CourseStatus.Rejected,
                CourseRejected
            },
            {
                DefaultEnum.None,
                CourseReply
            }
        };

        public static readonly Dictionary<Enum, string> ClassRunActionDict = new Dictionary<Enum, string>()
        {
            {
                ClassRunCancellationStatus.PendingApproval,
                ClassRunCancellationPendingApproval
            },
            {
                ClassRunCancellationStatus.Approved,
                ClassRunCancellationApproved
            },
            {
                ClassRunCancellationStatus.Rejected,
                ClassRunCancellationRejected
            },
            {
                ClassRunRescheduleStatus.PendingApproval,
                ClassRunReschedulePendingApproval
            },
            {
                ClassRunRescheduleStatus.Approved,
                ClassRunRescheduleApproved
            },
            {
                ClassRunRescheduleStatus.Rejected,
                ClassRunRescheduleRejected
            },
            {
                DefaultEnum.None,
                ClassRunReply
            }
        };

        public static readonly Dictionary<Enum, string> CourseContentActionDict = new Dictionary<Enum, string>()
        {
            {
                ContentStatus.Approved,
                CourseContentApproved
            },
            {
                ContentStatus.Rejected,
                CourseContentRejected
            },
            {
                DefaultEnum.None,
                CourseContentReply
            }
        };

        public static readonly Dictionary<Enum, string> ClassRunContentActionDict = new Dictionary<Enum, string>()
        {
            {
                ContentStatus.Approved,
                ClassRunContentApproved
            },
            {
                ContentStatus.Rejected,
                ClassRunContentRejected
            },
            {
                DefaultEnum.None,
                ClassRunContentReply
            }
        };

        public static readonly Dictionary<Enum, string> ParticipantAssignmentTrackQuizAnswerActionDict = new Dictionary<Enum, string>
        {
            {
                DefaultEnum.None,
                ParticipantAssignmentTrackQuizAnswerFeedback
            }
        };

        public static readonly Dictionary<EntityCommentType, Dictionary<Enum, string>> EntityActionDict = new Dictionary<EntityCommentType, Dictionary<Enum, string>>()
        {
            {
                EntityCommentType.Course,
                CourseActionDict
            },
            {
                EntityCommentType.ClassRun,
                ClassRunActionDict
            },
            {
                EntityCommentType.Registration,
                RegistrationActionDict
            },
            {
                EntityCommentType.CourseContent,
                CourseContentActionDict
            },
            {
                EntityCommentType.ClassRunContent,
                ClassRunContentActionDict
            },
            {
                EntityCommentType.ParticipantAssignmentTrackQuizAnswer,
                ParticipantAssignmentTrackQuizAnswerActionDict
            }
        };
    }
}
