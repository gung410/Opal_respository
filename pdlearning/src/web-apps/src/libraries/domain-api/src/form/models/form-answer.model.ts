import { FormQuestionModelAnswerValue, FormQuestionModelSingleAnswerValue } from './form-question.model';

import { PersonalFileModel } from './../../personal-space/models/personal-file.model';

export interface IFormAnswerModel {
  id: string | undefined;
  formId: string;
  courseId: string | undefined;
  startDate: Date;
  endDate: Date | undefined;
  submitDate: Date | undefined;
  score: number | undefined;
  scorePercentage: number | undefined;
  attempt: number;
  formMetaData: IFormAnswerFormMetaDataModel;
  createdBy: string;
  questionAnswers: IFormQuestionAnswerModel[];
  isCompleted: boolean;
}

export interface IFormAnswerFormMetaDataModel {
  questionIdOrderList: string[] | undefined;
  formQuestionOptionsOrderInfoList: IFormQuestionOptionsOrderInfoModel[] | undefined;
}

export interface IFormQuestionOptionsOrderInfoModel {
  formQuestionId: string;
  optionCodeOrderList: string[];
}

export interface IFormQuestionAnswerModel {
  id: string;
  formAnswerId: string;
  formQuestionId: string;
  answerValue: FormQuestionModelAnswerValue | undefined;
  maxScore: number | undefined;
  score: number | undefined;
  markedScore: number | undefined;
  scoredBy: number | undefined;
  answerFeedback: string | undefined;
  submittedDate: Date | undefined;
  spentTimeInSeconds: number | undefined;
  formAnswerAttachments: PersonalFileModel[];
}

export class FormQuestionAnswerModel implements IFormQuestionAnswerModel {
  public id: string;
  public formAnswerId: string;
  public formQuestionId: string;
  public answerValue: FormQuestionModelAnswerValue | undefined;
  public maxScore: number | undefined;
  public score: number | undefined;
  public markedScore: number | undefined;
  public scoredBy: number | undefined;
  public answerFeedback: string | undefined;
  public submittedDate: Date | undefined;
  public spentTimeInSeconds: number | undefined;
  public formAnswerAttachments: PersonalFileModel[];

  public static isCorrect(model: FormQuestionAnswerModel): boolean {
    return model.answerValue !== undefined && model.score !== undefined && model.score > 0 && model.score === model.maxScore;
  }

  public static isMultipleChoiceAnswerMatch(model: FormQuestionAnswerModel, optionValue: FormQuestionModelSingleAnswerValue): boolean {
    return model.answerValue !== undefined && model.answerValue instanceof Array && model.answerValue.indexOf(<never>optionValue) >= 0;
  }

  constructor(data?: IFormQuestionAnswerModel) {
    if (data != null) {
      this.id = data.id;
      this.formAnswerId = data.formAnswerId;
      this.formQuestionId = data.formQuestionId;
      this.answerValue = data.answerValue;
      this.maxScore = data.maxScore;
      this.score = data.score;
      this.markedScore = data.markedScore;
      this.scoredBy = data.scoredBy;
      this.answerFeedback = data.answerFeedback;
      this.submittedDate = data.submittedDate;
      this.spentTimeInSeconds = data.spentTimeInSeconds;
      this.formAnswerAttachments = data.formAnswerAttachments;
    }
  }
}

export class FormAnswerModel implements IFormAnswerModel {
  public id: string | undefined;
  public formId: string;
  public courseId: string | undefined;
  public startDate: Date;
  public endDate: Date | undefined;
  public submitDate: Date | undefined;
  public score: number | undefined;
  public scorePercentage: number | undefined;
  public attempt: number;
  public formMetaData: IFormAnswerFormMetaDataModel;
  public createdBy: string;
  public questionAnswers: FormQuestionAnswerModel[];
  public isCompleted: boolean;

  constructor(data?: IFormAnswerModel) {
    if (data != null) {
      this.id = data.id;
      this.formId = data.formId;
      this.courseId = data.courseId;
      this.startDate = new Date(data.startDate);
      this.endDate = data.endDate !== undefined ? new Date(data.endDate) : undefined;
      this.submitDate = data.submitDate !== undefined ? new Date(data.submitDate) : undefined;
      this.score = data.score;
      this.scorePercentage = data.scorePercentage;
      this.attempt = data.attempt;
      this.formMetaData = data.formMetaData;
      this.createdBy = data.createdBy;
      this.questionAnswers = data.questionAnswers.map(p => new FormQuestionAnswerModel(p));
      this.isCompleted = data.isCompleted;
    }
  }

  public isAvailable(): boolean {
    return this.endDate === undefined || this.endDate.getTime() > new Date().getTime();
  }

  public isPassed(passingMarkPercentage: number, passingMarkScore: number): boolean {
    if (passingMarkPercentage) {
      return this.scorePercentage >= passingMarkPercentage;
    }
    if (passingMarkScore) {
      return this.score >= passingMarkScore;
    }
    return true;
  }
  public durationInMiliseconds(): number | undefined {
    if (this.startDate == null || this.endDate == null) {
      return undefined;
    }
    return this.endDate.getTime() - this.startDate.getTime();
  }
}
