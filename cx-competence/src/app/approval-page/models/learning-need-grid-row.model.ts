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

export interface LearningNeedGridRowModel {
  id: string | number;
  identity: Identity;
  learner: ApprovalLearnerModel;

  LNAStatus: {
    userId: number;
    assessmentInfo: AssessmentInfo;
  };
  learnerDetailUrl: string;
}

export interface IChangeStatusModel {
  resultIdentities: Identity[];
  targetStatusType: {
    assessmentStatusId?: IdpStatusEnum | OdpStatusEnum | NominateStatusEnum;
    assessmentStatusCode?:
      | IdpStatusCodeEnum
      | OdpStatusCode
      | NominateStatusCodeEnum;
  };
}
export interface IChangeStatusRedultDto {
  identity: Identity;
  targetStatusType: {
    assessmentStatusId?: IdpStatusEnum | OdpStatusEnum | NominateStatusEnum;
    assessmentStatusCode?:
      | IdpStatusCodeEnum
      | OdpStatusCode
      | NominateStatusCodeEnum;
  };
}
