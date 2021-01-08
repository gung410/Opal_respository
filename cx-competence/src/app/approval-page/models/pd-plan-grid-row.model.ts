import { AssessmentInfo } from 'app-models/assessment-info.model';
import { Identity } from 'app-models/common.model';
import {
  IdpStatusCodeEnum,
  IdpStatusEnum,
} from 'app/individual-development/idp.constant';
import {
  NominateStatusCodeEnum,
  NominateStatusEnum,
  OdpStatusCode,
  OdpStatusEnum,
} from 'app/organisational-development/learning-plan-detail/odp.constant';
import { ApprovalLearnerModel } from './class-registration.model';

export interface PdPlanGridRowModel {
  id: string | number;
  identity: Identity;
  learner: ApprovalLearnerModel;

  PdPlanStatus: {
    userId: number;
    assessmentInfo: AssessmentInfo;
  };
  learnerDetailUrl: string;
}

export interface ChangeStatusPdPlanDto {
  resultIdentities: Identity[];
  targetStatusType: {
    assessmentStatusId?: IdpStatusEnum | OdpStatusEnum | NominateStatusEnum;
    assessmentStatusCode?:
      | IdpStatusCodeEnum
      | OdpStatusCode
      | NominateStatusCodeEnum;
  };
  comment?: string;
}
