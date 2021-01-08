import { StandaloneSurveyModel, StandaloneSurveySqRatingType, SurveyStatus } from '../models/lna-form.model';
import { StandaloneSurveyQuestionAnswerValue, StandaloneSurveyQuestionType, SurveyQuestionModel } from '../models/form-question.model';

import { CreateFormSectionRequest } from '../../form-section/dtos/create-form-section-request';
import { StandaloneSurveyQuestionOption } from '../models/form-question-option.model';
import { SurveyParticipantMode } from '../models/form-standalone-mode.enum';

export class CreateSurveyRequest {
  public title: string;
  public status: SurveyStatus;
  public formQuestions: CreateSurveyRequestSurveyQuestion[] = [];
  public formSections: CreateFormSectionRequest[] = [];
  public isAllowedDisplayPollResult: boolean | undefined;
  public isAutoSave: boolean;
  public isShowFreeTextQuestionInPoll: boolean | undefined;
  public sqRatingType: StandaloneSurveySqRatingType;
  public startDate?: Date;
  public endDate?: Date;
  public isStandalone?: boolean;
  public standaloneMode?: SurveyParticipantMode;

  constructor(
    form?: StandaloneSurveyModel,
    questions: CreateSurveyRequestSurveyQuestion[] = [],
    formSections: CreateFormSectionRequest[] = [],
    isAutoSave: boolean = false
  ) {
    if (form === undefined) {
      return;
    }
    this.title = form.title;
    this.status = form.status;
    this.formQuestions = questions !== undefined ? questions : [];
    this.formSections = formSections !== undefined ? formSections : [];
    this.isAllowedDisplayPollResult = form.isAllowedDisplayPollResult;
    this.isShowFreeTextQuestionInPoll = form.isShowFreeTextQuestionInPoll;
    this.isAutoSave = isAutoSave;
    this.sqRatingType = form.sqRatingType;
    this.startDate = form.startDate;
    this.endDate = form.endDate;
    this.isStandalone = form.isStandalone;
    this.standaloneMode = form.standaloneMode;
  }
}

export class CreateSurveyRequestSurveyQuestion {
  public questionType: StandaloneSurveyQuestionType;
  public questionTitle: string;
  public questionCorrectAnswer: StandaloneSurveyQuestionAnswerValue;
  public questionOptions: StandaloneSurveyQuestionOption[] = [];
  public priority: number;
  public minorPriority: number;
  public nextQuestionId: string | undefined;

  constructor(formQuestion?: SurveyQuestionModel) {
    if (formQuestion === undefined) {
      return;
    }
    this.questionType = formQuestion.questionType;
    this.questionTitle = formQuestion.questionTitle;
    this.questionCorrectAnswer = formQuestion.questionCorrectAnswer;
    this.questionOptions = formQuestion.questionOptions;
    this.priority = formQuestion.priority;
    this.minorPriority = formQuestion.minorPriority;
    this.nextQuestionId = formQuestion.nextQuestionId;
  }
}
