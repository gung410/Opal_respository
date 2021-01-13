import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';

export interface IUserAction {
  targetAction: StatusActionTypeEnum;
  targetIcon: string;
  isSimpleAction: boolean;
  allowActionSingle: boolean;
  message: string;
  currentStatus: string[];
  hasPermission?: boolean;
}

export class UserAction implements IUserAction {
  targetAction: StatusActionTypeEnum;
  targetIcon: string;
  isSimpleAction: boolean;
  allowActionSingle: boolean;
  message: string;
  currentStatus: string[];
  hasPermission?: boolean = false;

  constructor(data: Partial<IUserAction>) {
    if (data == null) {
      return;
    }

    this.targetAction = data.targetAction;
    this.targetIcon = data.targetIcon;
    this.isSimpleAction = data.isSimpleAction;
    this.allowActionSingle = data.allowActionSingle;
    this.message = data.message;
    this.currentStatus = data.currentStatus;
    this.hasPermission = data.hasPermission ?? false;
  }
}
