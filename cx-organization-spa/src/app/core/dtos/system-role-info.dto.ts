export class SystemRoleInfo {
  id: number;
  name: string;
  isEditMode: boolean = false;

  constructor(data?: SystemRoleInfo) {
    if (!data) {
      return;
    }

    this.id = data.id;
    this.name = data.name;
    this.isEditMode = data.isEditMode;
  }
}
