import { ISurveyQuestionModel, SurveyQuestionModel } from './form-question.model';
import { ISurveySection, SurveySection } from './form-section';

export interface IStandaloneSurveySectionsQuestions {
  formQuestions: ISurveyQuestionModel[];
  formSections: ISurveySection[];
}
export class StandaloneSurveySectionsQuestions implements IStandaloneSurveySectionsQuestions {
  public formQuestions: SurveyQuestionModel[] = [];
  public formSections: SurveySection[] = [];

  constructor(data?: IStandaloneSurveySectionsQuestions) {
    if (data != null) {
      this.formQuestions = data.formQuestions.map(_ => new SurveyQuestionModel(_));
      this.formSections = data.formSections.map(_ => new SurveySection(_));
    }
  }
}
