import { StandaloneSurveyModel, StandaloneSurveySqRatingType, SurveyStatus } from '../models/lna-form.model';
import { StandaloneSurveyQuestionAnswerValue, StandaloneSurveyQuestionType, SurveyQuestionModel } from '../models/form-question.model';

import { StandaloneSurveyQuestionOption } from '../models/form-question-option.model';
import { SurveyParticipantMode } from '../models/form-standalone-mode.enum';
import { UpdateFormSectionRequest } from '../../form-section/dtos/update-form-section-request';

export class UpdateSurveyRequest {
  public id: string;
  public title: string;
  public status: SurveyStatus;
  public toSaveFormQuestions: UpdateSurveyRequestSurveyQuestion[] = [];
  public toDeleteFormQuestionIds: string[] = [];
  public formSections: UpdateFormSectionRequest[] = [];
  public toDeleteFormSectionIds: string[] = [];
  public isAllowedDisplayPollResult: boolean | undefined;
  public isShowFreeTextQuestionInPoll: boolean | undefined;
  public isAutoSave: boolean;
  public sqRatingType: StandaloneSurveySqRatingType | undefined;
  public startDate?: Date;
  public endDate?: Date;
  public archiveDate?: Date;
  public isUpdateToNewVersion: boolean;
  public isStandalone?: boolean;
  public standaloneMode?: SurveyParticipantMode;
  public formRemindDueDate?: Date;
  public remindBeforeDays?: number;
  public isSendNotification?: boolean;

  constructor(
    form?: StandaloneSurveyModel,
    toSaveFormQuestions: UpdateSurveyRequestSurveyQuestion[] = [],
    deleteFormQuestionIds: string[] = [],
    formSections: UpdateFormSectionRequest[] = [],
    deleteFormSectionIds: string[] = [],
    isAutoSave: boolean = false,
    isUpdateToNewVersion: boolean = false
  ) {
    if (form === undefined) {
      return;
    }
    this.id = form.id;
    this.title = form.title;
    this.status = form.status;
    this.toSaveFormQuestions = toSaveFormQuestions !== undefined ? toSaveFormQuestions : [];
    this.toDeleteFormQuestionIds = deleteFormQuestionIds !== undefined ? deleteFormQuestionIds : [];
    this.formSections = formSections !== undefined ? formSections : [];
    this.toDeleteFormSectionIds = deleteFormSectionIds !== undefined ? deleteFormSectionIds : [];
    this.isAllowedDisplayPollResult = form.isAllowedDisplayPollResult;
    this.isAutoSave = isAutoSave;
    this.isShowFreeTextQuestionInPoll = form.isShowFreeTextQuestionInPoll;
    this.sqRatingType = form.sqRatingType;
    this.startDate = form.startDate;
    this.endDate = form.endDate;
    this.archiveDate = form.archiveDate;
    this.isUpdateToNewVersion = isUpdateToNewVersion;
    this.isStandalone = form.isStandalone;
    this.standaloneMode = form.standaloneMode;
    this.formRemindDueDate = form.formRemindDueDate ? new Date(form.formRemindDueDate) : null;
    this.remindBeforeDays = form.remindBeforeDays;
    this.isSendNotification = form.isSendNotification;
  }
}

export class UpdateSurveyRequestSurveyQuestion {
  public id: string | undefined;
  public questionType: StandaloneSurveyQuestionType;
  public questionTitle: string;
  public questionCorrectAnswer: StandaloneSurveyQuestionAnswerValue;
  public questionOptions: StandaloneSurveyQuestionOption[] = [];
  public priority: number;
  public minorPriority: number;
  public nextQuestionId: string | undefined;
  public formSectionId?: string;

  constructor(formQuestion?: SurveyQuestionModel) {
    if (formQuestion === undefined) {
      return;
    }
    this.id = formQuestion.id;
    this.questionType = formQuestion.questionType;
    this.questionTitle = formQuestion.questionTitle;
    this.questionCorrectAnswer = formQuestion.questionCorrectAnswer;
    this.questionOptions = formQuestion.questionOptions;
    this.priority = formQuestion.priority;
    this.minorPriority = formQuestion.minorPriority;
    this.nextQuestionId = formQuestion.nextQuestionId;
    this.formSectionId = formQuestion.formSectionId;
  }
}
