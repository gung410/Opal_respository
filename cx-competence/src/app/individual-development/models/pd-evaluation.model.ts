import { PDEvaluationType } from '../idp.constant';
import { IdpDto } from 'app/organisational-development/models/idp.model';

export class PDEvaluationModel {
  type: PDEvaluationType;
  reason: string;
  constructor(data?: Partial<PDEvaluationModel>) {
    if (!data) {
      return;
    }
    this.type = data.type;
    this.reason = data.reason;
  }
}

export class SubmittedLNAEventData {
  result?: IdpDto;
  navigateToPDPlan?: boolean;
}
