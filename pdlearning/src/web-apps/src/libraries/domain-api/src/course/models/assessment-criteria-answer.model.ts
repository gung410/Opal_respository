import { ValidationErrors } from '@angular/forms';

export enum AssessmentCriteriaModelValidationKey {
  criteriaMustHaveAnswer = 'criteriaMustHaveAnswer'
}

export interface IAssessmentCriteriaAnswer {
  criteriaId: string;
  scaleId: string;
  comment: string;
}

export class AssessmentCriteriaAnswer implements IAssessmentCriteriaAnswer {
  public criteriaId: string;
  public scaleId: string;
  public comment: string;

  public static validate(model: AssessmentCriteriaAnswer): ValidationErrors | undefined {
    if (!model.hasScale()) {
      return { [AssessmentCriteriaModelValidationKey.criteriaMustHaveAnswer]: 'Criteria need to have a answer' };
    }

    return null;
  }

  constructor(data?: IAssessmentCriteriaAnswer) {
    if (data) {
      this.criteriaId = data.criteriaId;
      this.scaleId = data.scaleId;
      this.comment = data.comment;
    }
  }

  public hasScale(): boolean {
    return this.scaleId != null && this.scaleId !== '';
  }
}
