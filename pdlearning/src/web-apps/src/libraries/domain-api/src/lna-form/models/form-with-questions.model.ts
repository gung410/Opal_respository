import { IStandaloneSurveyModel, StandaloneSurveyModel } from './lna-form.model';
import { ISurveyQuestionModel, SurveyQuestionModel } from './form-question.model';

import { FormSection } from '../../form-section/models/form-section';
import { SurveyAnswerModel } from './form-answer.model';

export interface ISurveyWithQuestionsModel {
  form: IStandaloneSurveyModel;
  formQuestions: ISurveyQuestionModel[];
  formSections: FormSection[];
}
export class SurveyWithQuestionsModel implements ISurveyWithQuestionsModel {
  public form: StandaloneSurveyModel = new StandaloneSurveyModel();
  public formQuestions: SurveyQuestionModel[] = [];
  public formSections: FormSection[] = [];

  constructor(data?: ISurveyWithQuestionsModel) {
    if (data != null) {
      this.form = new StandaloneSurveyModel(data.form);
      this.formQuestions = data.formQuestions.map(_ => new SurveyQuestionModel(_));
      this.formSections = data.formSections.map(_ => new FormSection(_));
    }
  }
}

export interface SurveyQuestionWithAnswerModel {
  formQuestion: SurveyQuestionModel[];
  formAnswer: SurveyAnswerModel;
}
