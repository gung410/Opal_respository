import { FormQuestionModel } from '../../form/models/form-question.model';

export interface IFormSection {
  id: string;
  formId: string;
  mainDescription?: string;
  additionalDescription?: string;
  priority: number;
  nextQuestionId?: string;
  isDeleted: boolean | undefined;
}

export class FormSection implements IFormSection {
  public id: string;
  public formId: string;
  public mainDescription?: string;
  public additionalDescription?: string;
  public nextQuestionId?: string;
  public priority: number = 0;
  public isDeleted: boolean | undefined;
  constructor(formSection: IFormSection) {
    this.id = formSection.id;
    this.formId = formSection.formId;
    this.mainDescription = formSection.mainDescription;
    this.additionalDescription = formSection.additionalDescription;
    this.priority = formSection.priority;
    this.nextQuestionId = formSection.nextQuestionId;
    this.isDeleted = this.isDeleted;
  }
}

export interface IFormSectionViewModel {
  sectionId: string;
  questions: FormQuestionModel[];
  title?: string;
  description?: string;
}

export class FormSectionViewModel implements IFormSectionViewModel {
  public sectionId: string;
  public questions: FormQuestionModel[];
  public title?: string;
  public description?: string;

  constructor(formSectionVm: IFormSectionViewModel) {
    this.sectionId = formSectionVm.sectionId;
    this.questions = formSectionVm.questions;
    this.title = formSectionVm.title ? formSectionVm.title : undefined;
    this.description = formSectionVm.description ? formSectionVm.description : undefined;
  }
}
