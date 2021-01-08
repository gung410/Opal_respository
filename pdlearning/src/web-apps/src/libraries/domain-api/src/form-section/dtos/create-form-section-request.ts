import { FormSection } from '../models/form-section';

export interface ICreateFormSectionRequest {
  id?: string;
  formId: string;
  mainDescription: string;
  additionalDescription?: string;
  priority: number;
  nextQuestionId?: string;
  isDeleted: boolean | undefined;
}

export class CreateFormSectionRequest implements ICreateFormSectionRequest {
  public id?: string;
  public formId: string;
  public mainDescription: string;
  public additionalDescription?: string;
  public nextQuestionId?: string;
  public priority: number = 0;
  public isDeleted: boolean | undefined;

  constructor(formSection: FormSection) {
    this.id = formSection.id;
    this.formId = formSection.formId;
    this.mainDescription = formSection.mainDescription;
    this.additionalDescription = formSection.additionalDescription;
    this.priority = formSection.priority;
    this.nextQuestionId = formSection.nextQuestionId;
    this.isDeleted = this.isDeleted;
  }
}
