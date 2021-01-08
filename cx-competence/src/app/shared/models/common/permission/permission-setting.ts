import { User } from 'app-models/auth.model';
export interface ICRUDPermission {
  allowView?: boolean;
  allowCreate?: boolean;
  allowEdit?: boolean;
  allowDelete?: boolean;
}
export interface IUserPermission extends ICRUDPermission {
  allowSubmit: boolean;
  allowApprove: boolean;
  allowReject: boolean;
  allowReview: boolean;
  allowChangeStatus: boolean;
}

export class BaseUserPermission implements IUserPermission {
  allowView: boolean;
  allowCreate: boolean;
  allowEdit: boolean;
  allowDelete: boolean;
  allowSubmit: boolean;
  allowApprove: boolean;
  allowReject: boolean;
  allowReview: boolean;
  allowChangeStatus: boolean;
  constructor(loginUser?: User) {
    if (!loginUser) {
      return;
    }
  }
}
