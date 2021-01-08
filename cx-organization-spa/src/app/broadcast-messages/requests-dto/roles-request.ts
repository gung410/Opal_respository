export class RolesRequest {
  extIds?: string[];
  roleIds?: number[];
  includeLocalizedData?: boolean;
  archetypeIds?: number[];

  constructor(data?: Partial<RolesRequest>) {
    if (data == null) {
      return;
    }
    this.extIds = data.extIds;
    this.includeLocalizedData = data.includeLocalizedData;
    this.roleIds = data.roleIds;
    this.archetypeIds = data.archetypeIds;
  }
}
