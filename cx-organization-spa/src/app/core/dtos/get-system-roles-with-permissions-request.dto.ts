export class GetSystemRolesWithPermissionsRequest {
  systemRoleIds?: number[];
  systemRoleExtIds?: string[];
  includeLocalizedData?: boolean;
  includeSystemRolePermissionSubjects?: boolean;

  constructor(data?: GetSystemRolesWithPermissionsRequest) {
    if (!data) {
      return;
    }

    this.systemRoleExtIds = data.systemRoleExtIds;
    this.systemRoleIds = data.systemRoleIds;
    this.includeLocalizedData = data.includeLocalizedData;
    this.includeSystemRolePermissionSubjects =
      data.includeSystemRolePermissionSubjects;
  }
}
