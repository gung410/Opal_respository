import { PDPlanDto } from 'app-models/pdplan.model';

export class LearningPlanContent {
  public pdplanDto: PDPlanDto;
  public formJSON: any;
  public id: string;
  constructor(data?) {
    if (!data) {
      return;
    }
    this.pdplanDto = data.pdplanDto ? data.pdplanDto : null;
    this.formJSON = data.formJSON ? data.formJSON : null;
    this.id = data.id ? data.id : null;
  }
}
