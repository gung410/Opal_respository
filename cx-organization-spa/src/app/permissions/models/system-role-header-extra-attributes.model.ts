export class SystemRoleHeaderExtraAttributesModel {
  isDefaultSystemRole: boolean;

  constructor(data?: Partial<SystemRoleHeaderExtraAttributesModel>) {
    if (!data) {
      return;
    }

    this.isDefaultSystemRole = data.isDefaultSystemRole;
  }
}
