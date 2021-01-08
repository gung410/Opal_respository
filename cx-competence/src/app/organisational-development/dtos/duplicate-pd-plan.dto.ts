import { Identity } from 'app-models/common.model';

export class DuplicatePDPlanRequest {
  sourceIdentity: Identity;
  newParentResultExtId: string;
  newAnswer: any;
}
