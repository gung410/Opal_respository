export interface IFormQuestionAnswerStatisticsModel {
  answerCode: number;
  answerValue: string;
  answerCount: number;
  answerPercentage: number;
}

export class FormQuestionAnswerStatisticsModel implements IFormQuestionAnswerStatisticsModel {
  public answerCode: number;
  public answerValue: string;
  public answerCount: number;
  public answerPercentage: number;
}
