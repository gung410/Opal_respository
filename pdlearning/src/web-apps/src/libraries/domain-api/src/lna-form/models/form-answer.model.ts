import { StandaloneSurveyQuestionModelAnswerValue, StandaloneSurveyQuestionModelSingleAnswerValue } from './form-question.model';

export interface ISurveyAnswerModel {
  id: string | undefined;
  formId: string;
  resourceId: string | undefined;
  startDate: Date;
  endDate: Date | undefined;
  submitDate: Date | undefined;
  attempt: number;
  formMetaData: ISurveyAnswerSurveyMetaDataModel;
  ownerId: string;
  questionAnswers: ISurveyQuestionAnswerModel[];
  isCompleted: boolean;
}

export interface ISurveyAnswerSurveyMetaDataModel {
  questionIdOrderList: string[] | undefined;
  formQuestionOptionsOrderInfoList: ISurveyQuestionOptionsOrderInfoModel[] | undefined;
}

export interface ISurveyQuestionOptionsOrderInfoModel {
  formQuestionId: string;
  optionCodeOrderList: string[];
}

export interface ISurveyQuestionAnswerModel {
  formAnswerId: string;
  formQuestionId: string;
  answerValue: StandaloneSurveyQuestionModelAnswerValue | undefined;
  submittedDate: Date | undefined;
  spentTimeInSeconds: number | undefined;
}

export class StandaloneSurveyQuestionAnswerModel implements ISurveyQuestionAnswerModel {
  public formAnswerId: string;
  public formQuestionId: string;
  public answerValue: StandaloneSurveyQuestionModelAnswerValue | undefined;
  public submittedDate: Date | undefined;
  public spentTimeInSeconds: number | undefined;

  public static isMultipleChoiceAnswerMatch(
    model: StandaloneSurveyQuestionAnswerModel,
    optionValue: StandaloneSurveyQuestionModelSingleAnswerValue
  ): boolean {
    return model.answerValue !== undefined && model.answerValue instanceof Array && model.answerValue.indexOf(<never>optionValue) >= 0;
  }

  constructor(data?: ISurveyQuestionAnswerModel) {
    if (data != null) {
      this.formAnswerId = data.formAnswerId;
      this.formQuestionId = data.formQuestionId;
      this.answerValue = data.answerValue;
      this.submittedDate = data.submittedDate;
      this.spentTimeInSeconds = data.spentTimeInSeconds;
    }
  }
}

export class SurveyAnswerModel implements ISurveyAnswerModel {
  public id: string | undefined;
  public formId: string;
  public resourceId: string | undefined;
  public startDate: Date;
  public endDate: Date | undefined;
  public submitDate: Date | undefined;
  public attempt: number;
  public formMetaData: ISurveyAnswerSurveyMetaDataModel;
  public ownerId: string;
  public questionAnswers: StandaloneSurveyQuestionAnswerModel[];
  public isCompleted: boolean;

  constructor(data?: ISurveyAnswerModel) {
    if (data != null) {
      this.id = data.id;
      this.formId = data.formId;
      this.resourceId = data.resourceId;
      this.startDate = new Date(data.startDate);
      this.endDate = data.endDate !== undefined ? new Date(data.endDate) : undefined;
      this.submitDate = data.submitDate !== undefined ? new Date(data.submitDate) : undefined;
      this.attempt = data.attempt;
      this.formMetaData = data.formMetaData;
      this.ownerId = data.ownerId;
      this.questionAnswers = data.questionAnswers.map(p => new StandaloneSurveyQuestionAnswerModel(p));
      this.isCompleted = data.isCompleted;
    }
  }

  public isAvailable(): boolean {
    return this.endDate === undefined || this.endDate.getTime() > new Date().getTime();
  }
  public durationInMiliseconds(): number | undefined {
    if (this.startDate == null || this.endDate == null) {
      return undefined;
    }
    return this.endDate.getTime() - this.startDate.getTime();
  }
}
