import { Injectable } from '@angular/core';
import { IDictionary } from 'app-models/dictionary';
import {
  ClassRegistrationStatusEnum,
  WithrawalStatusEnum,
} from '../models/class-registration.model';

export interface StatusMapper {
  color: string;
  text: string;
}

@Injectable()
export class StatusMapperService {
  private pendingColor: string = '#EFDC33';
  private pendingText: string = 'Common.StatusText.Pending';

  private rejectColor: string = '#FF6262';
  private rejectText: string = 'Common.StatusText.Rejected';

  private approvedColor: string = '#3BDC87';
  private approvedText: string = 'Common.StatusText.Approved';

  private classRegistrationStatusMapping: IDictionary<StatusMapper> = {
    [ClassRegistrationStatusEnum.PendingConfirmation]: {
      color: this.pendingColor,
      text: this.pendingText,
    },
    [ClassRegistrationStatusEnum.Approved]: {
      color: this.approvedColor,
      text: this.approvedText,
    },
    [ClassRegistrationStatusEnum.Rejected]: {
      color: this.rejectColor,
      text: this.rejectText,
    },
    [ClassRegistrationStatusEnum.ConfirmedByCA]: {
      color: this.approvedColor,
      text: this.approvedText,
    },
    [ClassRegistrationStatusEnum.RejectedByCA]: {
      color: this.rejectColor,
      text: this.rejectText,
    },
  };

  private withrawalStatusStatusMapping: IDictionary<StatusMapper> = {
    [WithrawalStatusEnum.PendingConfirmation]: {
      color: this.pendingColor,
      text: this.pendingText,
    },
    [WithrawalStatusEnum.Approved]: {
      color: this.approvedColor,
      text: this.approvedText,
    },
    [WithrawalStatusEnum.Rejected]: {
      color: this.rejectColor,
      text: this.rejectText,
    },
    [WithrawalStatusEnum.RejectedByCA]: {
      color: this.rejectColor,
      text: this.rejectText,
    },
  };

  private classRunChangeStatusStatusMapping: IDictionary<StatusMapper> = {
    [ClassRegistrationStatusEnum.PendingConfirmation]: {
      color: this.pendingColor,
      text: this.pendingText,
    },
    [ClassRegistrationStatusEnum.Approved]: {
      color: this.approvedColor,
      text: this.approvedText,
    },
    [ClassRegistrationStatusEnum.ConfirmedByCA]: {
      color: this.approvedColor,
      text: this.approvedText,
    },
    [ClassRegistrationStatusEnum.Rejected]: {
      color: this.rejectColor,
      text: this.rejectText,
    },
    [ClassRegistrationStatusEnum.RejectedByCA]: {
      color: this.rejectColor,
      text: this.rejectText,
    },
  };

  constructor() {}

  getStatusMapper(status: ClassRegistrationStatusEnum): StatusMapper {
    const statusMapping = {
      ...this.classRegistrationStatusMapping,
      ...this.withrawalStatusStatusMapping,
      ...this.classRunChangeStatusStatusMapping,
    };

    return statusMapping[status];
  }
}
