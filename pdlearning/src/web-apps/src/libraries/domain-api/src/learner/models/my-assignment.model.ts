export interface IMyAssignment {
  participantAssignmentTrackId: string;
  registrationId: string;
  userId: string;
  assignmentId: string;
  status: MyAssignmentStatus;
  submittedDate?: Date;
  commentNotSeenIds?: string[];
  startDate?: Date;
  endDate?: Date;
}

export class MyAssignment implements IMyAssignment {
  public participantAssignmentTrackId: string;
  public registrationId: string;
  public userId: string;
  public assignmentId: string;
  public status: MyAssignmentStatus;
  public submittedDate?: Date;
  public commentNotSeenIds?: string[];
  public startDate?: Date = new Date();
  public endDate?: Date = new Date();
  constructor(data?: IMyAssignment) {
    if (!data) {
      return;
    }
    this.participantAssignmentTrackId = data.participantAssignmentTrackId;
    this.registrationId = data.registrationId;
    this.userId = data.userId;
    this.assignmentId = data.assignmentId;
    this.status = data.status;
    this.submittedDate = data.submittedDate ? new Date(data.submittedDate) : undefined;
    this.commentNotSeenIds = data.commentNotSeenIds;
    this.startDate = data.startDate ? new Date(data.startDate) : undefined;
    this.endDate = data.endDate ? new Date(data.endDate) : undefined;
  }
}

export enum MyAssignmentStatus {
  NotStarted = 'NotStarted',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Incomplete = 'Incomplete',
  IncompletePendingSubmission = 'IncompletePendingSubmission',
  LateSubmission = 'LateSubmission'
}
