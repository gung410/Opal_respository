export interface IAssessmentScale {
  id?: string;
  name: string;
  value: number;
}

export class AssessmentScale implements IAssessmentScale {
  public id?: string;
  public name: string;
  public value: number = 0;

  constructor(data?: IAssessmentScale) {
    if (data) {
      this.id = data.id;
      this.name = data.name;
      this.value = data.value != null ? data.value : 0;
    }
  }
}
