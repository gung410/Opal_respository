import { FormQuestionModel, IFormQuestionModel } from './form-question.model';
import { FormSection, IFormSection } from '../../form-section/models/form-section';

export interface IFormSectionsQuestions {
  formQuestions: IFormQuestionModel[];
  formSections: IFormSection[];
}
export class FormSectionsQuestions implements IFormSectionsQuestions {
  public formQuestions: FormQuestionModel[] = [];
  public formSections: FormSection[] = [];

  constructor(data?: IFormSectionsQuestions) {
    if (data != null) {
      this.formQuestions = data.formQuestions.map(_ => new FormQuestionModel(_));
      this.formSections = data.formSections.map(_ => new FormSection(_));
    }
  }
}
