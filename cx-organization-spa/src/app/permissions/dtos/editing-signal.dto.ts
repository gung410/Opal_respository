export class EditingSignal {
  isEditing: boolean;
  systemRoleId: string;

  constructor(data?: Partial<EditingSignal>) {
    if (!data) {
      return;
    }

    this.isEditing = data.isEditing;
    this.systemRoleId = data.systemRoleId;
  }
}
