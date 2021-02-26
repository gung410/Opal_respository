export interface IRoleValue {
  label: string;
  value: boolean;
}

export class AccessRightRoles {
  accessRightId: number;
  accessRight: string = '';
  roles: IRoleValue[] = [];
  level?: number;
  isHideAccessRight: boolean;

  constructor(data?: Partial<AccessRightRoles>) {
    if (!data) {
      return;
    }
    this.accessRightId = data.accessRightId;
    this.accessRight = data.accessRight;
    this.roles = data.roles;
    this.level = data.level;
    this.isHideAccessRight = data.isHideAccessRight;
  }
}
