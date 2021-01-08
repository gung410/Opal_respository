export interface ISurveyQuestionAnswerStatisticsModel {
  answerCode: number;
  answerValue: string;
  answerCount: number;
  answerPercentage: number;
}

export class SurveyQuestionAnswerStatisticsModel implements ISurveyQuestionAnswerStatisticsModel {
  public answerCode: number;
  public answerValue: string;
  public answerCount: number;
  public answerPercentage: number;
}
