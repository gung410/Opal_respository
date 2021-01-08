import { User } from 'app-models/auth.model';
import { isEmpty } from 'lodash';
import { ApprovalTargetEnum } from './approval.enum';

export class ApprovalRequestTargetItem {
  target: ApprovalTargetEnum;
  titleTranslatePath: string;

  // The name of group permission takes from environment file. Eg: 'approvePendingRequest.ODP.learningPlan'
  // only affect in case the value was not null and was configured in environment.
  permissionKeys?: string[];
  constructor(
    target: ApprovalTargetEnum,
    titleTranslatePath: string,
    permissionKeys: string[] = []
  ) {
    this.target = target;
    this.titleTranslatePath = titleTranslatePath;
    this.permissionKeys = permissionKeys;
  }

  hasPerrmission(user: User): boolean {
    if (isEmpty(this.permissionKeys)) {
      return true;
    }

    if (!user) {
      return false;
    }

    let isValid = false;
    this.permissionKeys.forEach((key) => {
      if (user.hasPermission(key)) {
        isValid = true;
      }
    });

    return isValid;
  }
}
