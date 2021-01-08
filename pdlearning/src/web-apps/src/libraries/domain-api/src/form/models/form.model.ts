import { FormStandaloneMode } from './form-standalone-mode.enum';
import { IValidatorDefinition } from '@opal20/infrastructure';
import { PublicUserInfo } from '../../share/models/user-info.model';
import { Validators } from '@angular/forms';

export enum FormStatus {
  All = 'All',
  Draft = 'Draft',
  Published = 'Published',
  Unpublished = 'Unpublished',
  PendingApproval = 'PendingApproval',
  Approved = 'Approved',
  Rejected = 'Rejected',
  ReadyToUse = 'ReadyToUse',
  Archived = 'Archived'
}

export enum FormType {
  Quiz = 'Quiz',
  Survey = 'Survey',
  Poll = 'Poll',
  Onboarding = 'Onboarding',
  Holistic = 'Holistic',
  Analytic = 'Analytic'
}

export enum FormSurveyType {
  Standalone = 'Standalone',
  PreCourse = 'PreCourse',
  DuringCourse = 'DuringCourse',
  PostCourse = 'PostCourse',
  FollowUpPostCourse = 'FollowUpPostCourse'
}

export enum AnswerFeedbackDisplayOption {
  AfterAnsweredQuestion = 'AfterAnsweredQuestion',
  AfterXAtemps = 'AfterXAtemps',
  AfterCompletedQuiz = 'AfterCompletedQuiz'
}

export enum SqRatingType {
  CourseWorkshopMasterclassSeminarConference = 'CourseWorkshopMasterclassSeminarConference', // Type 1
  ELearningCourse = 'ELearningCourse', // Type 2
  BlendedCourseWorkshopMasterclass = 'BlendedCourseWorkshopMasterclass', // Type 3
  LearningEvent = 'LearningEvent' // Type 4
}

export enum FormQueryModeEnum {
  All = 'All',
  PendingApproval = 'PendingApproval',
  Archived = 'Archived',
  PostCourseTemplate = 'PostCourseTemplate'
}

export const FORM_QUERY_MODE = new Map<FormQueryModeEnum, string>([
  [FormQueryModeEnum.All, 'Forms'],
  [FormQueryModeEnum.PendingApproval, 'Pending Approval'],
  [FormQueryModeEnum.PostCourseTemplate, 'Post Course Template'],
  [FormQueryModeEnum.Archived, 'Archival']
]);

export interface IFormConfiguration {
  inSecondTimeLimit: number | undefined;
  randomizedQuestions: boolean;
  maxAttempt: number | undefined;
  passingMarkPercentage: number | undefined;
  passingMarkScore: number | undefined;
  attemptToShowFeedback: number | undefined;
  answerFeedbackDisplayOption: AnswerFeedbackDisplayOption | undefined;
}

export class FormConfiguration implements IFormConfiguration {
  public inSecondTimeLimit: number | undefined;
  public randomizedQuestions: boolean = false;
  public maxAttempt: number | undefined;
  public passingMarkPercentage: number | undefined;
  public passingMarkScore: number | undefined;
  public attemptToShowFeedback: number | undefined;
  public answerFeedbackDisplayOption: AnswerFeedbackDisplayOption | undefined;

  constructor(data?: IFormConfiguration) {
    if (data == null) {
      return;
    }

    this.inSecondTimeLimit = data.inSecondTimeLimit;
    this.randomizedQuestions = data.randomizedQuestions;
    this.maxAttempt = data.maxAttempt;
    this.passingMarkPercentage = data.passingMarkPercentage;
    this.passingMarkScore = data.passingMarkScore;
    this.attemptToShowFeedback = data.attemptToShowFeedback;
    this.answerFeedbackDisplayOption = data.answerFeedbackDisplayOption;
  }
}

export class FormModel implements IFormModel {
  public static titleValidators: IValidatorDefinition[] = [
    { validator: Validators.required, validatorType: 'required' },
    { validator: Validators.maxLength(1000), validatorType: 'maxLength' }
  ];

  public id: string | undefined;
  public ownerId: string | undefined;
  public title: string = '';
  public type: FormType = FormType.Quiz;
  public surveyType?: FormSurveyType;
  public status: FormStatus = FormStatus.Draft;
  public inSecondTimeLimit: number | undefined;
  public randomizedQuestions: boolean = false;
  public maxAttempt: number | undefined;
  public passingMarkPercentage: number | undefined;
  public passingMarkScore: number | undefined;
  public createdDate: Date = new Date();
  public changedDate: Date | undefined;
  public primaryApprovingOfficerId: string;
  public alternativeApprovingOfficerId: string;
  public originalObjectId: string;
  public owner: PublicUserInfo;
  public isAllowedDisplayPollResult: boolean | undefined;
  public isSurveyTemplate: boolean | undefined;
  public isShowFreeTextQuestionInPoll: boolean | undefined;
  public attemptToShowFeedback: number | undefined;
  public answerFeedbackDisplayOption: AnswerFeedbackDisplayOption | undefined;
  public sqRatingType: SqRatingType | undefined;
  public startDate?: Date = new Date();
  public endDate?: Date = new Date();
  public archiveDate?: Date = new Date();
  public archivedBy: string;
  public archivedByUser: PublicUserInfo;
  public isStandalone: boolean = false;
  public standaloneMode: FormStandaloneMode = FormStandaloneMode.Restricted;
  public canUnpublishFormStandalone: boolean;
  public isArchived?: boolean;
  public formRemindDueDate?: Date;
  public remindBeforeDays?: number;
  public isSendNotification?: boolean;
  public publishToCatalogue?: boolean;

  constructor(data?: IFormModel) {
    if (data != null) {
      this.id = data.id;
      this.ownerId = data.ownerId;
      this.title = data.title;
      this.type = data.type;
      this.surveyType = data.surveyType;
      this.status = data.status;
      this.inSecondTimeLimit = data.inSecondTimeLimit;
      this.randomizedQuestions = data.randomizedQuestions;
      this.maxAttempt = data.maxAttempt;
      this.passingMarkPercentage = data.passingMarkPercentage;
      this.passingMarkScore = data.passingMarkScore;
      this.createdDate = data.createdDate;
      this.changedDate = data.changedDate;
      this.primaryApprovingOfficerId = data.primaryApprovingOfficerId;
      this.alternativeApprovingOfficerId = data.alternativeApprovingOfficerId;
      this.originalObjectId = data.originalObjectId;
      this.owner = data.owner;
      this.isAllowedDisplayPollResult = data.isAllowedDisplayPollResult;
      this.isSurveyTemplate = data.isSurveyTemplate;
      this.isShowFreeTextQuestionInPoll = data.isShowFreeTextQuestionInPoll;
      this.attemptToShowFeedback = data.attemptToShowFeedback;
      this.answerFeedbackDisplayOption = data.answerFeedbackDisplayOption;
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
      this.publishToCatalogue = data.publishToCatalogue;
    }
  }

  public initBasicFormData(formType: FormType): FormModel {
    this.title = 'Draft';
    this.randomizedQuestions = false;
    this.type = formType;
    this.status = FormStatus.Draft;
    this.isShowFreeTextQuestionInPoll = false;
    this.answerFeedbackDisplayOption = AnswerFeedbackDisplayOption.AfterAnsweredQuestion;
    this.endDate = undefined;
    this.startDate = undefined;
    return this;
  }
}

export interface IFormModel {
  id: string | undefined;
  ownerId: string | undefined;
  title: string;
  type: FormType;
  surveyType?: FormSurveyType;
  status: FormStatus;
  inSecondTimeLimit: number | undefined;
  randomizedQuestions: boolean;
  maxAttempt: number | undefined;
  passingMarkPercentage: number | undefined;
  passingMarkScore: number | undefined;
  createdDate: Date;
  changedDate: Date | undefined;
  primaryApprovingOfficerId?: string | undefined;
  alternativeApprovingOfficerId?: string | undefined;
  originalObjectId: string | undefined;
  owner: PublicUserInfo | undefined;
  isAllowedDisplayPollResult: boolean | undefined;
  isSurveyTemplate: boolean | undefined;
  isShowFreeTextQuestionInPoll: boolean | undefined;
  attemptToShowFeedback: number | undefined;
  answerFeedbackDisplayOption: AnswerFeedbackDisplayOption | undefined;
  sqRatingType: SqRatingType | undefined;
  startDate?: Date;
  endDate?: Date;
  archiveDate?: Date | undefined;
  archivedBy?: string | undefined;
  archivedByUser: PublicUserInfo | undefined;
  isStandalone?: boolean;
  standaloneMode?: FormStandaloneMode;
  canUnpublishFormStandalone: boolean;
  isArchived?: boolean;
  formRemindDueDate?: Date;
  remindBeforeDays?: number;
  isSendNotification?: boolean;
  publishToCatalogue?: boolean;
}

export interface IDueDate {
  formRemindDueDate: Date;
  remindBeforeDays: number;
  isSendNotification: boolean;
}
