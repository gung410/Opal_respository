import { FormModel, IFormModel } from './form.model';
import { FormQuestionModel, IFormQuestionModel } from './form-question.model';

import { FormAnswerModel } from './form-answer.model';
import { FormSection } from '../../form-section/models/form-section';

export interface IFormWithQuestionsModel {
  form: IFormModel;
  formQuestions: IFormQuestionModel[];
  formSections: FormSection[];
}
export class FormWithQuestionsModel implements IFormWithQuestionsModel {
  public form: FormModel = new FormModel();
  public formQuestions: FormQuestionModel[] = [];
  public formSections: FormSection[] = [];

  constructor(data?: IFormWithQuestionsModel) {
    if (data != null) {
      this.form = new FormModel(data.form);
      this.formQuestions = data.formQuestions.map(_ => new FormQuestionModel(_));
      this.formSections = data.formSections.map(_ => new FormSection(_));
    }
  }
}

export interface FormQuestionWithAnswerModel {
  formQuestion: FormQuestionModel[];
  formAnswer: FormAnswerModel;
}
