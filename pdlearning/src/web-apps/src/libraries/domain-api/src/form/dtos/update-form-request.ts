import { AnswerFeedbackDisplayOption, FormModel, FormStatus, FormSurveyType, FormType, SqRatingType } from '../models/form.model';
import { FormQuestionModel, QuestionAnswerValue, QuestionType } from '../models/form-question.model';

import { FormStandaloneMode } from '../models/form-standalone-mode.enum';
import { QuestionOption } from '../models/form-question-option.model';
import { UpdateFormSectionRequest } from '../../form-section/dtos/update-form-section-request';

export class UpdateFormRequest {
  public id: string;
  public title: string;
  public type: FormType;
  public surveyType: FormSurveyType;
  public status: FormStatus;
  public inSecondTimeLimit: number;
  public randomizedQuestions: boolean = false;
  public maxAttempt: number | undefined;
  public passingMarkPercentage: number | undefined;
  public passingMarkScore: number | undefined;
  public toSaveFormQuestions: UpdateFormRequestFormQuestion[] = [];
  public toDeleteFormQuestionIds: string[] = [];
  public formSections: UpdateFormSectionRequest[] = [];
  public toDeleteFormSectionIds: string[] = [];
  public primaryApprovingOfficerId: string | undefined;
  public alternativeApprovingOfficerId: string | undefined;
  public isAllowedDisplayPollResult: boolean | undefined;
  public isSurveyTemplate: boolean | undefined;
  public isShowFreeTextQuestionInPoll: boolean | undefined;
  public answerFeedbackDisplayOption: AnswerFeedbackDisplayOption | undefined;
  public attemptToShowFeedback: number | undefined;
  public isAutoSave: boolean;
  public sqRatingType: SqRatingType | undefined;
  public startDate?: Date;
  public endDate?: Date;
  public archiveDate?: Date;
  public isUpdateToNewVersion: boolean;
  public isStandalone?: boolean;
  public standaloneMode?: FormStandaloneMode;
  public formRemindDueDate?: Date;
  public remindBeforeDays?: number;
  public isSendNotification?: boolean;
  public publishToCatalogue?: boolean;
  public comment?: string;

  constructor(
    form?: FormModel,
    toSaveFormQuestions: UpdateFormRequestFormQuestion[] = [],
    deleteFormQuestionIds: string[] = [],
    formSections: UpdateFormSectionRequest[] = [],
    deleteFormSectionIds: string[] = [],
    isAutoSave: boolean = false,
    isUpdateToNewVersion: boolean = false,
    comment?: string
  ) {
    if (form === undefined) {
      return;
    }
    this.id = form.id;
    this.title = form.title;
    this.type = form.type;
    this.surveyType = form.surveyType;
    this.status = form.status;
    this.inSecondTimeLimit = form.inSecondTimeLimit;
    this.randomizedQuestions = form.randomizedQuestions;
    this.maxAttempt = form.maxAttempt;
    this.passingMarkPercentage = form.passingMarkPercentage;
    this.passingMarkScore = form.passingMarkScore;
    this.toSaveFormQuestions = toSaveFormQuestions !== undefined ? toSaveFormQuestions : [];
    this.toDeleteFormQuestionIds = deleteFormQuestionIds !== undefined ? deleteFormQuestionIds : [];
    this.formSections = formSections !== undefined ? formSections : [];
    this.toDeleteFormSectionIds = deleteFormSectionIds !== undefined ? deleteFormSectionIds : [];
    this.primaryApprovingOfficerId = form.primaryApprovingOfficerId;
    this.alternativeApprovingOfficerId = form.alternativeApprovingOfficerId;
    this.isAllowedDisplayPollResult = form.isAllowedDisplayPollResult;
    this.isSurveyTemplate = form.isSurveyTemplate;
    this.isAutoSave = isAutoSave;
    this.isShowFreeTextQuestionInPoll = form.isShowFreeTextQuestionInPoll;
    this.answerFeedbackDisplayOption = form.answerFeedbackDisplayOption;
    this.attemptToShowFeedback = form.attemptToShowFeedback;
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
    this.publishToCatalogue = form.publishToCatalogue;
    this.comment = comment;
  }
}

export class UpdateFormRequestFormQuestion {
  public id: string | undefined;
  public questionType: QuestionType;
  public questionTitle: string;
  public questionCorrectAnswer: QuestionAnswerValue;
  public questionOptions: QuestionOption[] = [];
  public priority: number;
  public minorPriority: number;
  public questionHint: string | null;
  public answerExplanatoryNote: string | null;
  public feedbackCorrectAnswer: string | null;
  public feedbackWrongAnswer: string | null;
  public questionLevel: number | undefined;
  public randomizedOptions: boolean | undefined;
  public score: number | undefined;
  public isSurveyTemplateQuestion: boolean | undefined;
  public nextQuestionId: string | undefined;
  public formSectionId?: string;
  public isScoreEnabled?: boolean;
  public description?: string;

  constructor(formQuestion?: FormQuestionModel) {
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
    this.questionHint = formQuestion.questionHint;
    this.answerExplanatoryNote = formQuestion.answerExplanatoryNote;
    this.feedbackCorrectAnswer = formQuestion.feedbackCorrectAnswer;
    this.feedbackWrongAnswer = formQuestion.feedbackWrongAnswer;
    this.questionLevel = formQuestion.questionLevel;
    this.randomizedOptions = formQuestion.randomizedOptions;
    this.score = formQuestion.score;
    this.isSurveyTemplateQuestion = formQuestion.isSurveyTemplateQuestion;
    this.nextQuestionId = formQuestion.nextQuestionId;
    this.formSectionId = formQuestion.formSectionId;
    this.isScoreEnabled = formQuestion.isScoreEnabled;
    this.description = formQuestion.description;
  }
}
