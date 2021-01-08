import { Identity } from './common.model';
import { PDPlanDto } from './pdplan.model';

export class DeactivateAssessmentParams {
  pdPlan: PDPlanDto;
  deactivateAllVersion?: boolean;
  deactivateAllDescendants?: boolean;
}

export class DeactivateAssessmentResponseDto {
  identity: Identity;
  status: number;
  message: string;
  otherVersions: any[];
  descendants: any[];
}
