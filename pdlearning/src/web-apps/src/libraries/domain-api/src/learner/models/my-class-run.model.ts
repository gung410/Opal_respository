import { ClassRunChangeStatus, RegistrationType, WithdrawalStatus } from '../../course/models/registrations.model';

import { MyRegistrationStatus } from './my-course.model';
export interface IMyClassRunModel {
  id?: string;
  userId: string;
  courseId: string;
  classRunId: string;
  status: MyRegistrationStatus;
  withdrawalStatus?: WithdrawalStatus;
  classRunChangeStatus?: ClassRunChangeStatus;
  classRunChangeId?: string;
  registrationId: string;
  comment?: string;
  reason?: string;
  registrationType: RegistrationType;
  administratedBy?: string;
  changedBy?: string;
  changedDate?: Date;
  learningStatus?: LearningStatus;
  postCourseEvaluationFormCompleted?: boolean;
}

export class MyClassRunModel implements IMyClassRunModel {
  public id?: string;
  public userId: string;
  public courseId: string;
  public classRunId: string;
  public status: MyRegistrationStatus;
  public withdrawalStatus?: WithdrawalStatus;
  public classRunChangeStatus?: ClassRunChangeStatus;
  public classRunChangeId?: string;
  public registrationId: string;
  public comment?: string;
  public reason?: string;
  public registrationType: RegistrationType;
  public administratedBy?: string;
  public changedBy?: string;
  public changedDate?: Date;
  public learningStatus?: LearningStatus;
  public postCourseEvaluationFormCompleted?: boolean;

  constructor(data?: IMyClassRunModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.userId = data.userId;
    this.courseId = data.courseId;
    this.classRunId = data.classRunId;
    this.status = data.status;
    this.withdrawalStatus = data.withdrawalStatus;
    this.classRunChangeStatus = data.classRunChangeStatus;
    this.classRunChangeId = data.classRunChangeId;
    this.registrationId = data.registrationId;
    this.comment = data.comment;
    this.reason = data.reason;
    this.registrationType = data.registrationType;
    this.administratedBy = data.administratedBy;
    this.changedBy = data.changedBy;
    this.changedDate = data.changedDate;
    this.learningStatus = data.learningStatus;
    this.postCourseEvaluationFormCompleted = data.postCourseEvaluationFormCompleted;
  }

  public get isClassRunFinished(): boolean {
    return this.learningStatus === LearningStatus.Failed || this.learningStatus === LearningStatus.Completed;
  }
}

export enum LearningStatus {
  InProgress = 'InProgress',
  Completed = 'Completed',
  NotStarted = 'NotStarted',
  Passed = 'Passed',
  Failed = 'Failed'
}
