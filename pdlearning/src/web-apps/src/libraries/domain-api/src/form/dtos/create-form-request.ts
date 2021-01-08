import { AnswerFeedbackDisplayOption, FormModel, FormStatus, FormType, SqRatingType } from '../models/form.model';
import { FormQuestionModel, QuestionAnswerValue, QuestionType } from '../models/form-question.model';

import { CreateFormSectionRequest } from '../../form-section/dtos/create-form-section-request';
import { FormStandaloneMode } from '../models/form-standalone-mode.enum';
import { QuestionOption } from '../models/form-question-option.model';

export class CreateFormRequest {
  public title: string;
  public type: FormType;
  public status: FormStatus;
  public inSecondTimeLimit: number;
  public randomizedQuestions: boolean = false;
  public maxAttempt: number | undefined;
  public formQuestions: CreateFormRequestFormQuestion[] = [];
  public formSections: CreateFormSectionRequest[] = [];
  public primaryApprovingOfficerId: string | undefined;
  public alternativeApprovingOfficerId: string | undefined;
  public isAllowedDisplayPollResult: boolean | undefined;
  public isSurveyTemplate: boolean | undefined;
  public isAutoSave: boolean;
  public isShowFreeTextQuestionInPoll: boolean | undefined;
  public answerFeedbackDisplayOption: AnswerFeedbackDisplayOption;
  public sqRatingType: SqRatingType;
  public startDate?: Date;
  public endDate?: Date;
  public isStandalone?: boolean;
  public standaloneMode?: FormStandaloneMode;
  public publishToCatalogue?: boolean;

  constructor(
    form?: FormModel,
    questions: CreateFormRequestFormQuestion[] = [],
    formSections: CreateFormSectionRequest[] = [],
    isAutoSave: boolean = false
  ) {
    if (form === undefined) {
      return;
    }
    this.title = form.title;
    this.type = form.type;
    this.status = form.status;
    this.inSecondTimeLimit = form.inSecondTimeLimit;
    this.randomizedQuestions = form.randomizedQuestions;
    this.maxAttempt = form.maxAttempt;
    this.formQuestions = questions !== undefined ? questions : [];
    this.formSections = formSections !== undefined ? formSections : [];
    this.primaryApprovingOfficerId = form.primaryApprovingOfficerId;
    this.alternativeApprovingOfficerId = form.alternativeApprovingOfficerId;
    this.isAllowedDisplayPollResult = form.isAllowedDisplayPollResult;
    this.isSurveyTemplate = form.isSurveyTemplate;
    this.isShowFreeTextQuestionInPoll = form.isShowFreeTextQuestionInPoll;
    this.answerFeedbackDisplayOption = form.answerFeedbackDisplayOption;
    this.isAutoSave = isAutoSave;
    this.sqRatingType = form.sqRatingType;
    this.startDate = form.startDate;
    this.endDate = form.endDate;
    this.isStandalone = form.isStandalone;
    this.standaloneMode = form.standaloneMode;
    this.publishToCatalogue = form.publishToCatalogue;
  }
}

export class CreateFormRequestFormQuestion {
  public id: string;
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

  constructor(formQuestion?: FormQuestionModel) {
    if (formQuestion === undefined) {
      return;
    }
    this.id = formQuestion.id || undefined;
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
  }
}
