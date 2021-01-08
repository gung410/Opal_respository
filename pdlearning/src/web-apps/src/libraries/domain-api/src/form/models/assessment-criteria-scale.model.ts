export interface IAssessmentCriteriaScale {
  id?: string;
  content: string;
  scaleId: string;
}

export class AssessmentCriteriaScale implements IAssessmentCriteriaScale {
  public id?: string;
  public content: string;
  public scaleId: string;

  constructor(data?: IAssessmentCriteriaScale) {
    if (data) {
      this.id = data.id;
      this.content = data.content;
      this.scaleId = data.scaleId;
    }
  }
}
