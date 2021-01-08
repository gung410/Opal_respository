import { IValidatorDefinition } from '@opal20/infrastructure';
import { PublicUserInfo } from '../../share/models/user-info.model';
import { SurveyParticipantMode } from './form-standalone-mode.enum';
import { Validators } from '@angular/forms';

export enum SurveyStatus {
  All = 'All',
  Draft = 'Draft',
  Published = 'Published',
  Unpublished = 'Unpublished',
  Archived = 'Archived'
}

export enum StandaloneSurveySqRatingType {
  CourseWorkshopMasterclassSeminarConference = 'CourseWorkshopMasterclassSeminarConference', // Type 1
  ELearningCourse = 'ELearningCourse', // Type 2
  BlendedCourseWorkshopMasterclass = 'BlendedCourseWorkshopMasterclass', // Type 3
  LearningEvent = 'LearningEvent' // Type 4
}

export enum SurveyQueryModeEnum {
  All = 'All',
  Archived = 'Archived'
}

export const STANDALONE_SURVEY_QUERY_MODE = new Map<SurveyQueryModeEnum, string>([
  [SurveyQueryModeEnum.All, 'Forms'],
  [SurveyQueryModeEnum.Archived, 'Archival']
]);

export interface ISurveyConfiguration {}

export class SurveyConfiguration implements ISurveyConfiguration {
  constructor(data?: ISurveyConfiguration) {
    if (data == null) {
      return;
    }
  }
}

export class StandaloneSurveyModel implements IStandaloneSurveyModel {
  public static titleValidators: IValidatorDefinition[] = [
    { validator: Validators.required, validatorType: 'required' },
    { validator: Validators.maxLength(1000), validatorType: 'maxLength' }
  ];

  public id: string | undefined;
  public ownerId: string | undefined;
  public title: string = '';
  public status: SurveyStatus = SurveyStatus.Draft;
  public createdDate: Date = new Date();
  public changedDate: Date | undefined;
  public originalObjectId: string;
  public owner: PublicUserInfo;
  public isAllowedDisplayPollResult: boolean | undefined;
  public isShowFreeTextQuestionInPoll: boolean | undefined;
  public sqRatingType: StandaloneSurveySqRatingType | undefined;
  public startDate?: Date = new Date();
  public endDate?: Date = new Date();
  public archiveDate?: Date = new Date();
  public archivedBy: string;
  public archivedByUser: PublicUserInfo;
  public isStandalone: boolean = false;
  public standaloneMode: SurveyParticipantMode = SurveyParticipantMode.Restricted;
  public canUnpublishFormStandalone: boolean;
  public isArchived?: boolean;
  public formRemindDueDate?: Date;
  public remindBeforeDays?: number;
  public isSendNotification?: boolean;

  constructor(data?: IStandaloneSurveyModel) {
    if (data != null) {
      this.id = data.id;
      this.ownerId = data.ownerId;
      this.title = data.title;
      this.status = data.status;
      this.createdDate = data.createdDate;
      this.changedDate = data.changedDate;
      this.originalObjectId = data.originalObjectId;
      this.owner = data.owner;
      this.isAllowedDisplayPollResult = data.isAllowedDisplayPollResult;
      this.isShowFreeTextQuestionInPoll = data.isShowFreeTextQuestionInPoll;
      this.sqRatingType = data.sqRatingType;
      this.startDate = data.startDate ? new Date(data.startDate) : null;
      this.endDate = data.endDate ? new Date(data.endDate) : null;
      this.archiveDate = data.archiveDate ? new Date(data.archiveDate) : null;
      this.archivedBy = data.archivedBy;
      this.archivedByUser = data.archivedByUser;
      this.isStandalone = data.isStandalone ? data.isStandalone : false;
      this.standaloneMode = data.standaloneMode;
      this.canUnpublishFormStandalone = data.canUnpublishFormStandalone;
      this.isArchived = data.isArchived;
      this.formRemindDueDate = data.formRemindDueDate ? new Date(data.formRemindDueDate) : null;
      this.remindBeforeDays = data.remindBeforeDays;
      this.isSendNotification = data.isSendNotification;
    }
  }
}

export interface IStandaloneSurveyModel {
  id: string | undefined;
  ownerId: string | undefined;
  title: string;
  status: SurveyStatus;
  createdDate: Date;
  changedDate: Date | undefined;
  originalObjectId: string | undefined;
  owner: PublicUserInfo | undefined;
  isAllowedDisplayPollResult: boolean | undefined;
  isShowFreeTextQuestionInPoll: boolean | undefined;
  sqRatingType: StandaloneSurveySqRatingType | undefined;
  startDate?: Date;
  endDate?: Date;
  archiveDate?: Date | undefined;
  archivedBy?: string | undefined;
  archivedByUser: PublicUserInfo | undefined;
  isStandalone?: boolean;
  standaloneMode?: SurveyParticipantMode;
  canUnpublishFormStandalone: boolean;
  isArchived?: boolean;
  formRemindDueDate?: Date;
  remindBeforeDays?: number;
  isSendNotification?: boolean;
}

export interface IStandaloneSurveyDueDate {
  formRemindDueDate: Date;
  remindBeforeDays: number;
  isSendNotification: boolean;
}
