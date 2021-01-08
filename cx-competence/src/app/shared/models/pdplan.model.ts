import { Chart } from 'angular-highcharts';
import { AssessmentStatusInfo, ObjectiveInfo } from './assessment.model';
import { Identity } from './common.model';

export class PDPlanDto {
  // If the objectiveInfo.identity is NULL
  // then the department identity or the user identity of the current logged-in user will be used to process.
  objectiveInfo?: ObjectiveInfo;
  resultIdentity?: Identity;
  previousResultIdentity?: Identity;
  timestamp?: string;
  answer?: any; // The JSON result of the surveyJS form after the user submits the form.
  assessmentRoleId?: number; // The role identifier of the assessment.
  dueDate?: string;
  assessmentStatusInfo?: AssessmentStatusInfo;
  parentResultExtId?: string;
  forceCreateResult?: boolean;
  errorIfExistingResult?: boolean;
  pdPlanActivity?: string;
  pdPlanType?: string;
  children?: PDPlanDto[];
  childrenCount?: number;
  createdBy?: ObjectiveInfo;
  created?: string;
  lastUpdatedBy?: ObjectiveInfo;
  lastUpdated?: string;
  additionalProperties?: any;

  surveyInfo?: SurveyInfo;
  startDate?: string;
}

export class SurveyInfo {
  description?: string;
  displayName?: string;
  endDate?: string;
  finishText?: string;
  identity?: Identity;
  info?: string;
  name?: string;
  startDate?: string;
}

export class PDPlanConfig {
  configuration: any;
  pdPlanType: string;
  pdPlanActivity: string;
}

export class ReportData {
  description: string;
  identity: Identity;
  name: string;
  series: Serie[];
}

export class Serie {
  description: string;
  identity: Identity;
  name: string;
  value: number;
  color: string;
}

export class ChartInfo {
  header: string;
  series: Serie[];
  chart: Chart;
  constructor(data?: Partial<ChartInfo>) {
    if (!data) {
      return;
    }
    this.header = data.header;
    this.series = data.series;
    this.chart = data.chart;
  }
}

export enum PdPlanType {
  Odp = 'Odp',
  Idp = 'Idp',
}

export class IdpConfigParams {
  public resultId: number;
  public userId: number;
  constructor(data?: Partial<IdpConfigParams>) {
    if (!data) {
      return;
    }
    this.resultId = data.resultId;
    this.userId = data.userId;
  }
}

export interface MyCourseInfo {
  id: string;
  courseId: string;
  userId: string;
  status: string;
  progressMeasure: number;
  lastLogin: Date;
  startDate: Date;
  createdDate: Date;
  createdBy: string;
  changedDate: Date;
  courseType: string;
  myRegistrationStatus: string;
  resultId: string;
  displayStatus: string;
  hasContentChanged: boolean;
}

export interface MyClassRun {
  id: string;
  userId: string;
  courseId: string;
  classRunId: string;
  status: MyRegistrationStatus;
  withdrawalStatus: WithdrawalStatus;
  registrationId: string;
  registrationType: string;
  changedBy: string;
  changedDate: Date;
}

export interface LearnerCourseInfo {
  courseId: string;
  myCourseInfo: MyCourseInfo;
  myClassRun: MyClassRun;
}

export enum WithdrawalStatus {
  PendingConfirmation = 'PendingConfirmation',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Withdrawn = 'Withdrawn',
  RejectedByCA = 'RejectedByCA',
}

export enum MyRegistrationStatus {
  PendingConfirmation = 'PendingConfirmation',
  Approved = 'Approved',
  Rejected = 'Rejected',
  ConfirmedByCA = 'ConfirmedByCA',
  RejectedByCA = 'RejectedByCA',
  WaitlistPendingApprovalByLearner = 'WaitlistPendingApprovalByLearner',
  WaitlistConfirmed = 'WaitlistConfirmed',
  WaitlistRejected = 'WaitlistRejected',
  OfferPendingApprovalByLearner = 'OfferPendingApprovalByLearner',
  OfferRejected = 'OfferRejected',
  OfferConfirmed = 'OfferConfirmed',
}

export enum ClassRunStatus {
  Published = 'Published',
  Unpublished = 'Unpublished',
  Cancelled = 'Cancelled',
}

export const REJECTED_REGISTRATION_STATUSES = [
  MyRegistrationStatus.Rejected,
  MyRegistrationStatus.OfferRejected,
  MyRegistrationStatus.WaitlistRejected,
  MyRegistrationStatus.RejectedByCA,
];
