import { AssessmentCriteria, IAssessmentCriteria } from './assessment-criteria.model';
import { AssessmentScale, IAssessmentScale } from './assessment-scale.model';
export interface IAssessment {
  id?: string;
  name: string;
  status: AssessmentStatus;
  criteria: IAssessmentCriteria[];
  scales: IAssessmentScale[];
  type: AssessmentType;
}

export class Assessment implements IAssessment {
  public id?: string;
  public name: string;
  public status: AssessmentStatus;
  public criteria: AssessmentCriteria[] = [];
  public scales: AssessmentScale[] = [];
  public type: AssessmentType;

  constructor(data?: IAssessment) {
    if (data) {
      this.id = data.id;
      this.name = data.name;
      this.status = data.status;
      this.criteria = data.criteria != null ? data.criteria.map(x => new AssessmentCriteria(x)) : [];
      this.scales = data.scales != null ? data.scales.map(x => new AssessmentScale(x)) : [];
      this.type = data.type;
    }
  }
}

export enum AssessmentStatus {
  Published = 'Published'
}

export enum AssessmentType {
  Holistic = 'Holistic',
  Analytic = 'Analytic'
}
