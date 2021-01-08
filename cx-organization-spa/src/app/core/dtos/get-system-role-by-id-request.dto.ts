export interface IGetSystemRoleByIdRequest {
  id: number;
  includeLocalizedData?: boolean;
  includeSystemRolePermissionSubjects?: boolean;
}

export class GetSystemRoleByIdRequest implements IGetSystemRoleByIdRequest {
  id: number;
  includeLocalizedData?: boolean;
  includeSystemRolePermissionSubjects?: boolean;

  constructor(data?: IGetSystemRoleByIdRequest) {
    if (!data) {
      return;
    }

    this.id = data.id;
    this.includeLocalizedData = data.includeLocalizedData;
    this.includeSystemRolePermissionSubjects =
      data.includeSystemRolePermissionSubjects;
  }
}
