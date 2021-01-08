import { SurveyQuestionModel } from './form-question.model';

export interface ISurveySection {
  id: string;
  formId: string;
  mainDescription?: string;
  additionalDescription?: string;
  priority: number;
  nextQuestionId?: string;
  isDeleted: boolean | undefined;
}

export class SurveySection implements ISurveySection {
  public id: string;
  public formId: string;
  public mainDescription?: string;
  public additionalDescription?: string;
  public nextQuestionId?: string;
  public priority: number = 0;
  public isDeleted: boolean | undefined;
  constructor(formSection: ISurveySection) {
    this.id = formSection.id;
    this.formId = formSection.formId;
    this.mainDescription = formSection.mainDescription;
    this.additionalDescription = formSection.additionalDescription;
    this.priority = formSection.priority;
    this.nextQuestionId = formSection.nextQuestionId;
    this.isDeleted = this.isDeleted;
  }
}

export interface ISurveySectionViewModel {
  sectionId: string;
  questions: SurveyQuestionModel[];
  title?: string;
  description?: string;
}

export class SurveySectionViewModel implements ISurveySectionViewModel {
  public sectionId: string;
  public questions: SurveyQuestionModel[];
  public title?: string;
  public description?: string;

  constructor(formSectionVm: ISurveySectionViewModel) {
    this.sectionId = formSectionVm.sectionId;
    this.questions = formSectionVm.questions;
    this.title = formSectionVm.title ? formSectionVm.title : undefined;
    this.description = formSectionVm.description ? formSectionVm.description : undefined;
  }
}
