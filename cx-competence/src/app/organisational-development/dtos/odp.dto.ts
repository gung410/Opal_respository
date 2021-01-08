import { GetAssignedPDOParams } from 'app-models/mpj/assign-pdo.model';

export class GetMassNominationLearnerParams extends GetAssignedPDOParams {
  // The assignment Id of ad-hoc mass-nomination id
  resultId: string | number;

  constructor(payload?: Partial<GetMassNominationLearnerParams>) {
    super(payload);
    if (!payload) {
      return;
    }
  }
}
