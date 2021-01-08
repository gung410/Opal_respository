export class CommentActionPrefix {
  public static readonly coursePrefix = 'course';
  public static readonly courseContentPrefix = 'course-content';
  public static readonly classRunPrefix = 'classrun';
  public static readonly classRunContentPrefix = 'classrun-content';
  public static readonly registrationPrefix = 'registration';
  public static readonly participantAssignmentTrackQuizAnswerPrefix = 'participant-assignment-track-quiz-answer';
}

export class CommentAction {
  public static readonly courseReply = `${CommentActionPrefix.coursePrefix}-reply`;
  public static readonly courseApproved = `${CommentActionPrefix.coursePrefix}-approved`;
  public static readonly courseRejected = `${CommentActionPrefix.coursePrefix}-rejected`;
  public static readonly classRunReply = `${CommentActionPrefix.classRunPrefix}-reply`;
  public static readonly classRunCancellationPendingApproval = `${CommentActionPrefix.classRunPrefix}-cancellation-pending-approval`;
  public static readonly classRunCancellationApproved = `${CommentActionPrefix.classRunPrefix}-cancellation-approved`;
  public static readonly classRunCancellationRejected = `${CommentActionPrefix.classRunPrefix}-cancellation-rejected`;
  public static readonly classRunReschedulePendingApproval = `${CommentActionPrefix.classRunPrefix}-reschedule--pending-approval`;
  public static readonly classRunRescheduleApproved = `${CommentActionPrefix.classRunPrefix}-reschedule-approved`;
  public static readonly classRunRescheduleRejected = `${CommentActionPrefix.classRunPrefix}-reschedule-rejected`;
  public static readonly courseContentReply = `${CommentActionPrefix.courseContentPrefix}-reply`;
  public static readonly courseContentApproved = `${CommentActionPrefix.courseContentPrefix}-approved`;
  public static readonly courseContentRejected = `${CommentActionPrefix.courseContentPrefix}-rejected`;
  public static readonly classRunContentReply = `${CommentActionPrefix.classRunContentPrefix}-reply`;
  public static readonly classRunContentApproved = `${CommentActionPrefix.classRunContentPrefix}-approved`;
  public static readonly classRunContentRejected = `${CommentActionPrefix.classRunContentPrefix}-rejected`;
  public static readonly registrationApproved = `${CommentActionPrefix.registrationPrefix}-approved`;
  public static readonly registrationRejected = `${CommentActionPrefix.registrationPrefix}-rejected`;
  public static readonly registrationWaitlistConfirmed = `${CommentActionPrefix.registrationPrefix}-waitlist-confirmed`;
  public static readonly registrationConfirmedByCA = `${CommentActionPrefix.registrationPrefix}-confirmed-by-ca`;
  public static readonly registrationRejectedByCA = `${CommentActionPrefix.registrationPrefix}-rejected-by-ca`;
  public static readonly registrationWaitlistPendingApprovalByLearner = `${CommentActionPrefix.registrationPrefix}-waitlist-pending-approval-by-learner`;
  public static readonly registrationClassRunChangePendingConfirmation = `${CommentActionPrefix.registrationPrefix}-classrun-change-pending-confirmation`;
  public static readonly registrationClassRunChangeApproved = `${CommentActionPrefix.registrationPrefix}-classrun-change-approved`;
  public static readonly registrationClassRunChangeRejected = `${CommentActionPrefix.registrationPrefix}-classrun-change-rejected`;
  public static readonly registrationClassRunChangeConfirmedByCA = `${CommentActionPrefix.registrationPrefix}-classrun-change-confirmed-by-ca`;
  public static readonly registrationClassRunChangeRejectedByCA = `${CommentActionPrefix.registrationPrefix}-classrun-change-rejected-by-ca`;
  public static readonly registrationWithdrawnApproved = `${CommentActionPrefix.registrationPrefix}-withdrawn-approved`;
  public static readonly registrationWithdrawnRejected = `${CommentActionPrefix.registrationPrefix}-withdrawn-rejected`;
  public static readonly registrationWithdrawnPendingConfirmation = `${CommentActionPrefix.registrationPrefix}-withdrawn-pending-confirmation`;
  public static readonly registrationWithdrawnConfirmedByCA = `${CommentActionPrefix.registrationPrefix}-withdrawn-confirmed-by-ca`;
  public static readonly registrationWithdrawnRejectedByCA = `${CommentActionPrefix.registrationPrefix}-withdrawn-rejected-by-ca`;

  public static readonly participantAssignmentTrackQuizAnswerFeedback = `${CommentActionPrefix.participantAssignmentTrackQuizAnswerPrefix}-reply`;
}

export const COMMENT_ACTION_MAPPING: Dictionary<string> = {
  [CommentAction.courseReply]: '',
  [CommentAction.courseApproved]: 'Course-Approved',
  [CommentAction.courseRejected]: 'Course-Rejected',
  [CommentAction.classRunReply]: '',
  [CommentAction.classRunCancellationPendingApproval]: 'ClassRun-Cancellation-Pending-Approval',
  [CommentAction.classRunCancellationApproved]: 'ClassRun-Cancellation-Approved',
  [CommentAction.classRunCancellationRejected]: 'classRun-Cancellation-Rejected',
  [CommentAction.classRunReschedulePendingApproval]: 'ClassRun-Reschedule-Pending-Approval',
  [CommentAction.classRunRescheduleApproved]: 'ClassRun-Reschedule-Approved',
  [CommentAction.classRunRescheduleRejected]: 'ClassRun-Reschedule-Rejected',
  [CommentAction.courseContentReply]: '',
  [CommentAction.courseContentApproved]: 'Course-Content-Approved',
  [CommentAction.courseContentRejected]: 'Course-Content-Approved',
  [CommentAction.classRunContentReply]: '',
  [CommentAction.classRunContentApproved]: 'ClassRun-Content-Approved',
  [CommentAction.classRunContentRejected]: 'ClassRun-Content-Rejected',
  [CommentAction.participantAssignmentTrackQuizAnswerFeedback]: ''
};
