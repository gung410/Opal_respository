import {
  IdpStatusEnum,
  IdpStatusCodeEnum,
} from 'app/individual-development/idp.constant';
import {
  OdpStatusEnum,
  OdpStatusCode,
  NominateStatusEnum,
  NominateStatusCodeEnum,
} from 'app/organisational-development/learning-plan-detail/odp.constant';
import { environment } from 'app-environments/environment';

export class ObjectiveInfo {
  identity?: ResultIdentity;
  name?: string;
  email?: string;
}

export class ResultIdentity {
  archetype?: string;
  id?: number;
  ownerId?: number;
  customerId?: number;
  extId?: string;
  constructor(data?: Partial<ResultIdentity>) {
    if (!data) {
      this.ownerId = environment.OwnerId;
      this.customerId = environment.CustomerId;
      return;
    }
    this.extId = data.extId ? data.extId : undefined;
    this.ownerId = data.ownerId ? data.ownerId : environment.OwnerId;
    this.customerId = data.customerId
      ? data.customerId
      : environment.CustomerId;
    this.archetype = data.archetype ? data.archetype : undefined;
    this.id = data.id !== null ? data.id : undefined;
  }
}

export class AssessmentStatusInfo {
  assessmentStatusId?: IdpStatusEnum | OdpStatusEnum | NominateStatusEnum;
  assessmentStatusCode?:
    | IdpStatusCodeEnum
    | OdpStatusCode
    | NominateStatusCodeEnum;
  assessmentStatusDescription?: string;
  assessmentStatusName?: string;
  constructor(data?: Partial<AssessmentStatusInfo>) {
    if (!data) {
      return;
    }
    this.assessmentStatusId = data.assessmentStatusId
      ? data.assessmentStatusId
      : 0;
    this.assessmentStatusCode = data.assessmentStatusCode
      ? data.assessmentStatusCode
      : IdpStatusCodeEnum.NotAdded;
    this.assessmentStatusDescription = data.assessmentStatusDescription
      ? data.assessmentStatusDescription
      : '';
    this.assessmentStatusName = data.assessmentStatusName
      ? data.assessmentStatusName
      : '';
  }
}
