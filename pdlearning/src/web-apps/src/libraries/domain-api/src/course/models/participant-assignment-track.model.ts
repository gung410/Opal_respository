import {
  IParticipantAssignmentTrackQuizAnswer,
  ParticipantAssignmentTrackQuizAnswer
} from './participant-assignment-track-quiz-answer.model';

import { Assignment } from './assignment.model';
import { LMM_PERMISSIONS } from './../../share/permission-keys/lmm-permission-key';
import { UserInfoModel } from '@opal20/domain-api';
import { Utils } from '@opal20/infrastructure';

export interface IParticipantAssignmentTrack {
  id?: string;
  registrationId: string;
  userId: string;
  assignmentId: string;
  assignedDate: Date;
  startDate: Date;
  endDate: Date;
  submittedDate: Date;
  status: ParticipantAssignmentTrackStatus;
  changedBy: string;
  createdBy: string;
  quizAnswer?: IParticipantAssignmentTrackQuizAnswer;
  isAutoAssigedAssessorOnce: boolean;
}

export class ParticipantAssignmentTrack implements IParticipantAssignmentTrack {
  public static optionalProps: (keyof IParticipantAssignmentTrack)[] = ['quizAnswer'];

  public id?: string;
  public registrationId: string;
  public userId: string;
  public assignmentId: string;
  public assignedDate: Date;
  public submittedDate: Date;
  public startDate: Date = new Date();
  public endDate: Date = new Date();
  public status: ParticipantAssignmentTrackStatus = ParticipantAssignmentTrackStatus.NotStarted;
  public changedBy: string;
  public createdBy: string;
  public quizAnswer?: ParticipantAssignmentTrackQuizAnswer;
  public isAutoAssigedAssessorOnce: boolean;

  public static hasScoreGivingAssignmentPermission(currentUser: UserInfoModel): boolean {
    return currentUser.hasPermissionPrefix(LMM_PERMISSIONS.ScoreGivingAssignment);
  }

  public static hasViewAnswerDoneAssignmentPermission(currentUser: UserInfoModel): boolean {
    return currentUser.hasPermissionPrefix(LMM_PERMISSIONS.ViewAnswerDoneAssignment);
  }

  public static hasViewLearnerAssignmentTrackPermission(currentUser: UserInfoModel): boolean {
    return currentUser.hasPermissionPrefix(LMM_PERMISSIONS.ViewLearnerAssignmentTrack);
  }

  public static hasAssignAssignmentPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.AssignAssignment);
  }

  constructor(data?: IParticipantAssignmentTrack) {
    if (data != null) {
      this.id = data.id;
      this.registrationId = data.registrationId;
      this.userId = data.userId;
      this.assignmentId = data.assignmentId;
      this.assignedDate = data.assignedDate ? new Date(data.assignedDate) : undefined;
      this.submittedDate = data.submittedDate ? new Date(data.submittedDate) : undefined;
      this.startDate = data.startDate ? new Date(data.startDate) : undefined;
      this.endDate = data.endDate ? new Date(data.endDate) : undefined;
      this.status = data.status;
      this.changedBy = data.changedBy;
      this.createdBy = data.createdBy;
      this.isAutoAssigedAssessorOnce = data.isAutoAssigedAssessorOnce;
      this.quizAnswer = data.quizAnswer != null ? new ParticipantAssignmentTrackQuizAnswer(data.quizAnswer) : null;
    }
  }

  public getParticipantAssignmentTrackCompletionRate(assignment: Assignment): number {
    const quizAssignmentForm = assignment.quizAssignmentForm ? assignment.quizAssignmentForm : null;
    const questionAnswers = this.quizAnswer ? this.quizAnswer.questionAnswers : null;
    const completionRate =
      quizAssignmentForm == null || questionAnswers == null || quizAssignmentForm.questions.length === 0
        ? 0
        : (questionAnswers.length / quizAssignmentForm.questions.length) * 100;
    return Utils.round(completionRate, 2);
  }

  public hasViewCommentFeedbackAssignmentPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.ViewCommentFeedbackAssignment);
  }
}

export enum ParticipantAssignmentTrackStatus {
  NotStarted = 'NotStarted',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Incomplete = 'Incomplete',
  IncompletePendingSubmission = 'IncompletePendingSubmission',
  LateSubmission = 'LateSubmission'
}
