import { StandaloneSurveyQuestionModelAnswerValue } from '../models/form-question.model';

export interface IUpdateSurveyAnswerRequest {
  formAnswerId: string;
  questionAnswers?: IUpdateSurveyQuestionAnswerRequest[];
  isSubmit: boolean;
}

export interface ISaveSurveyAnswer {
  formId: string;
  resourceId?: string;
}

export interface IUpdateSurveyQuestionAnswerRequest {
  formQuestionId: string;
  answerValue: StandaloneSurveyQuestionModelAnswerValue | undefined;
  isSubmit: boolean;
  spentTimeInSeconds?: number;
}
