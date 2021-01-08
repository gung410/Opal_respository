import { AssessmentCriteriaScale, IAssessmentCriteriaScale } from './assessment-criteria-scale.model';

export interface IAssessmentCriteria {
  id?: string;
  name: string;
  scales: IAssessmentCriteriaScale[];
}

export class AssessmentCriteria implements IAssessmentCriteria {
  public id?: string;
  public name: string;
  public scales: AssessmentCriteriaScale[] = [];

  constructor(data?: IAssessmentCriteria) {
    if (data) {
      this.id = data.id;
      this.name = data.name;
      this.scales = data.scales != null ? data.scales.map(x => new AssessmentCriteriaScale(x)) : [];
    }
  }
}
